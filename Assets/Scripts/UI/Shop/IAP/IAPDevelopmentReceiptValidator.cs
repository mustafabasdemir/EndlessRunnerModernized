/// <summary>
/// Client-only stub. Accepts non-empty transaction ids when fulfillment is enabled in development.
/// Does NOT protect against forged receipts — document and replace before enabling live IAP.
/// </summary>
public sealed class IAPDevelopmentReceiptValidator : IIAPReceiptValidator
{
    public IAPReceiptValidationResult Validate(IAPPurchaseContext context)
    {
        if (!context.HasValidTransactionId)
            return IAPReceiptValidationResult.Invalid("Missing transaction id.");

        if (!IAPPurchaseFulfillmentPolicy.RequireReceiptPayloadForFulfillment
            || !string.IsNullOrEmpty(context.ReceiptPayload))
        {
            return IAPReceiptValidationResult.Valid(
                "Development validator only — schedule server-side receipt validation before production.");
        }

        return IAPReceiptValidationResult.Invalid("Receipt payload required but empty.");
    }
}
