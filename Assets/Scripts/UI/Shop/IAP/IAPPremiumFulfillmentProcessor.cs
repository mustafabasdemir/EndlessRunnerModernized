using UnityEngine;

/// <summary>
/// Idempotent premium fulfillment: validate receipt, reserve transaction id, grant premium, mark completed.
/// Store ConfirmPurchase must run only after Successful fulfillment (handled by IAPStoreSessionCoordinator).
/// </summary>
public sealed class IAPPremiumFulfillmentProcessor
{
    readonly IIAPProcessedTransactionStore m_TransactionStore;
    readonly IIAPReceiptValidator m_ReceiptValidator;
    readonly IPremiumBalanceWriter m_PremiumWriter;

    public IAPPremiumFulfillmentProcessor(
        IIAPProcessedTransactionStore transactionStore,
        IIAPReceiptValidator receiptValidator,
        IPremiumBalanceWriter premiumWriter)
    {
        m_TransactionStore = transactionStore;
        m_ReceiptValidator = receiptValidator;
        m_PremiumWriter = premiumWriter;
    }

    public IAPFulfillmentResult TryFulfill(IAPPurchaseContext context, out string diagnostic)
    {
        diagnostic = null;

        if (!IAPPurchaseFulfillmentPolicy.FulfillmentEnabled)
        {
            diagnostic = "Fulfillment disabled by IAPPurchaseFulfillmentPolicy.";
            return IAPFulfillmentResult.FulfillmentDisabled;
        }

        if (!context.HasValidTransactionId || !context.HasValidProductId)
        {
            diagnostic = "Missing transaction id or product id.";
            return IAPFulfillmentResult.InvalidInput;
        }

        if (!IAPPurchaseFulfillmentPolicy.PremiumAmountByProductId.TryGetValue(context.ProductId, out int amount))
        {
            diagnostic = "Unknown product id: " + context.ProductId;
            return IAPFulfillmentResult.UnknownProduct;
        }

        var phase = m_TransactionStore.GetPhase(context.TransactionId);
        if (phase == IAPTransactionPhase.Completed)
        {
            diagnostic = "Transaction already fulfilled: " + context.TransactionId;
            return IAPFulfillmentResult.AlreadyProcessed;
        }

        if (phase == IAPTransactionPhase.None)
        {
            if (!m_TransactionStore.TryBeginTransaction(context.TransactionId, context.ProductId, amount))
            {
                phase = m_TransactionStore.GetPhase(context.TransactionId);
                if (phase == IAPTransactionPhase.Completed)
                {
                    diagnostic = "Transaction already fulfilled (race): " + context.TransactionId;
                    return IAPFulfillmentResult.AlreadyProcessed;
                }

                diagnostic = "Could not reserve transaction id (storage failure or duplicate).";
                return IAPFulfillmentResult.TransactionStoreFailed;
            }

            phase = IAPTransactionPhase.Pending;
        }

        if (phase == IAPTransactionPhase.Pending)
        {
            var validation = m_ReceiptValidator.Validate(context);
            if (!validation.IsValid)
            {
                diagnostic = validation.Diagnostic ?? "Receipt validation failed.";
                return IAPFulfillmentResult.ReceiptValidationFailed;
            }

            if (!m_PremiumWriter.TryAddPremium(amount, out var premiumError))
            {
                diagnostic = premiumError ?? "Premium persist failed.";
                return IAPFulfillmentResult.PremiumPersistFailed;
            }

            if (!m_TransactionStore.TryMarkPremiumPersisted(context.TransactionId))
            {
                m_PremiumWriter.TryRollbackPremium(amount, out _);
                diagnostic = "Premium grant rolled back because transaction phase could not be persisted.";
                Debug.LogError("IAP fulfillment: " + diagnostic + " Tx=" + context.TransactionId);
                return IAPFulfillmentResult.TransactionStoreFailed;
            }

            phase = IAPTransactionPhase.PremiumPersisted;
        }

        if (phase == IAPTransactionPhase.PremiumPersisted)
        {
            if (!m_TransactionStore.TryMarkCompleted(context.TransactionId))
            {
                diagnostic = "Could not mark transaction completed after premium persist.";
                return IAPFulfillmentResult.CompletionMarkFailed;
            }
        }

        diagnostic = "Fulfillment completed for transaction " + context.TransactionId;
        return IAPFulfillmentResult.Success;
    }
}
