using System.Collections.Generic;

/// <summary>
/// IAP premium grant policy for migration phases. Live fulfillment stays off until
/// receipt validation, durable transaction tracking, and store confirmation order are implemented.
/// </summary>
public static class IAPPurchaseFulfillmentPolicy
{
    /// <summary>When false, purchase callbacks must not modify PlayerData or persist rewards.</summary>
    public static bool FulfillmentEnabled => false;

    public static readonly IReadOnlyDictionary<string, int> PremiumAmountByProductId =
        new Dictionary<string, int>
        {
            { "10_premium", 10 },
            { "50_premium", 50 },
            { "100_premium", 100 },
        };

    // Planned deduplication (not active in this phase):
    // 1) On pending/confirmed purchase, read store transaction id from Order.Info (IAP 5) or Product.transactionID (codeless legacy).
    // 2) Before granting premium, check a durable processed-id set (new PlayerData save version or sidecar file).
    // 3) If id already processed, skip grant but still allow ConfirmPurchase when fulfillment is enabled.
    // 4) Persist id only after successful premium increment + Save().
}
