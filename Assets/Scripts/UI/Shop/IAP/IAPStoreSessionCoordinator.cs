using System;
using UnityEngine;
using UnityEngine.Purchasing;

/// <summary>
/// IAP 5 StoreController lifecycle (Connect → FetchProducts → FetchPurchases → pending → fulfill → ConfirmPurchase).
/// Live store connection and ConfirmPurchase stay disabled until IAPPurchaseFulfillmentPolicy flags are enabled.
/// </summary>
public sealed class IAPStoreSessionCoordinator : IDisposable
{
    StoreController m_Controller;
    bool m_EventsRegistered;
    readonly IAPPremiumFulfillmentProcessor m_FulfillmentProcessor;

    public IAPStoreSessionCoordinator(IAPPremiumFulfillmentProcessor fulfillmentProcessor = null)
    {
        m_FulfillmentProcessor = fulfillmentProcessor ?? IAPPremiumFulfillmentServices.DefaultProcessor;
    }

    public bool IsConnected =>
        m_Controller != null && m_Controller.GetConnectionState() == ConnectionState.Connected;

    /// <summary>
    /// Registers IAP 5 callbacks once. Does not connect when StoreConnectionEnabled is false.
    /// </summary>
    public async System.Threading.Tasks.Task InitializeAsync()
    {
        if (!IAPPurchaseFulfillmentPolicy.StoreConnectionEnabled)
        {
            Debug.Log(
                "IAPStoreSessionCoordinator: store connection disabled (StoreConnectionEnabled=false).");
            return;
        }

        if (m_Controller == null)
            m_Controller = UnityIAPServices.StoreController();

        RegisterEventsOnce();

        // Next phase (when StoreConnectionEnabled): await Connect(), FetchProducts(), FetchPurchases().
        await System.Threading.Tasks.Task.CompletedTask;
    }

    void RegisterEventsOnce()
    {
        if (m_EventsRegistered || m_Controller == null)
            return;

        m_Controller.OnPurchasePending += OnPurchasePending;
        m_Controller.OnPurchasesFetched += OnPurchasesFetched;
        m_Controller.OnPurchaseConfirmed += OnPurchaseConfirmed;
        m_Controller.OnPurchaseFailed += OnPurchaseFailed;
        m_EventsRegistered = true;
    }

    void OnPurchasePending(PendingOrder order)
    {
        var context = IAPPurchaseContextFactory.FromPendingOrder(order);
        var result = m_FulfillmentProcessor.TryFulfill(context, out var diagnostic);

        if (result == IAPFulfillmentResult.FulfillmentDisabled)
        {
            Debug.LogWarning("IAP pending purchase ignored (fulfillment disabled). " + diagnostic);
            return;
        }

        if (result == IAPFulfillmentResult.AlreadyProcessed)
        {
            TryConfirmIfAllowed(order, "duplicate pending callback");
            return;
        }

        if (result != IAPFulfillmentResult.Success)
        {
            Debug.LogError(
                "IAP pending purchase not confirmed — fulfillment failed: "
                + result
                + " "
                + diagnostic);
            return;
        }

        TryConfirmIfAllowed(order, "fulfillment success");
    }

    void OnPurchasesFetched(Orders orders)
    {
        // Recovery: pending consumables may be replayed here when ProcessPendingOrdersOnPurchasesFetched is true.
        Debug.Log("IAPStoreSessionCoordinator: purchases fetched (recovery hook).");
    }

    void OnPurchaseConfirmed(Order order)
    {
        Debug.Log(
            "IAPStoreSessionCoordinator: purchase confirmed by store. Tx="
            + (order?.Info?.TransactionID ?? "n/a"));
    }

    void OnPurchaseFailed(FailedOrder failedOrder)
    {
        Debug.LogWarning(
            "IAPStoreSessionCoordinator: purchase failed. "
            + (failedOrder?.Info?.TransactionID ?? "unknown tx"));
    }

    void TryConfirmIfAllowed(PendingOrder order, string reason)
    {
        if (!IAPPurchaseFulfillmentPolicy.ConfirmPurchasesEnabled)
        {
            Debug.Log(
                "IAP ConfirmPurchase skipped (ConfirmPurchasesEnabled=false). Reason: "
                + reason);
            return;
        }

        m_Controller?.ConfirmPurchase(order);
    }

    public void Dispose()
    {
        if (m_Controller != null && m_EventsRegistered)
        {
            m_Controller.OnPurchasePending -= OnPurchasePending;
            m_Controller.OnPurchasesFetched -= OnPurchasesFetched;
            m_Controller.OnPurchaseConfirmed -= OnPurchaseConfirmed;
            m_Controller.OnPurchaseFailed -= OnPurchaseFailed;
            m_EventsRegistered = false;
        }
    }
}
