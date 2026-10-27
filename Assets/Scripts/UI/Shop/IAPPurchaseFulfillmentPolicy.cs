using System.Collections.Generic;

/// <summary>
/// IAP premium grant and store session policy for migration phases.
/// Live fulfillment, store connection, and ConfirmPurchase stay off until explicitly enabled.
/// </summary>
public static class IAPPurchaseFulfillmentPolicy
{
    /// <summary>When false, purchase callbacks must not modify PlayerData or persist rewards.</summary>
    public static bool FulfillmentEnabled
    {
        get
        {
#if UNITY_EDITOR
            if (IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride.HasValue)
                return IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride.Value;
#endif
            return false;
        }
    }

    /// <summary>When false, IAPStoreSessionCoordinator does not connect to Google Play / App Store.</summary>
    public static bool StoreConnectionEnabled => false;

    /// <summary>When false, ConfirmPurchase is never invoked (pending orders stay with the store).</summary>
    public static bool ConfirmPurchasesEnabled => false;

    /// <summary>When true, fulfillment requires a non-empty receipt payload (IAP 5 PendingOrder).</summary>
    public static bool RequireReceiptPayloadForFulfillment => false;

    public static readonly IReadOnlyDictionary<string, int> PremiumAmountByProductId =
        new Dictionary<string, int>
        {
            { "10_premium", 10 },
            { "50_premium", 50 },
            { "100_premium", 100 },
        };

    // Transaction safety (IAP sidecar: iap_processed_transactions_v1.bin):
    // 1) Reserve store transaction id (Pending) before granting premium.
    // 2) Validate receipt (server-side validator required for production — see IIAPReceiptValidator).
    // 3) Grant premium + PlayerData.Save(), then mark PremiumPersisted, then Completed.
    // 4) Duplicate callbacks: Completed → skip grant; ConfirmPurchase only when ConfirmPurchasesEnabled.
    // 5) save.bin version unchanged — existing player saves load without migration.
    // 6) Reinstall / new device: local sidecar alone cannot prove entitlement — use backend ledger.
}
