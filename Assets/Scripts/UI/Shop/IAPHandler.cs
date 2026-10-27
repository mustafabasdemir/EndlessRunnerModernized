using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class IAPHandler : MonoBehaviour
{
    private void OnEnable()
    {
#if UNITY_ANALYTICS
        AnalyticsEvent.StoreOpened(StoreType.Premium);
#endif
    }

    public void ProductBought(Product product)
    {
        if (product?.definition == null)
            return;

        if (!IAPPurchaseFulfillmentPolicy.FulfillmentEnabled)
        {
            Debug.LogWarning(
                "IAP purchase received but fulfillment is disabled (IAPPurchaseFulfillmentPolicy.FulfillmentEnabled). Product: "
                + product.definition.id);
            return;
        }

        if (!IAPPurchaseFulfillmentPolicy.PremiumAmountByProductId.TryGetValue(product.definition.id, out int amount))
            return;

        // TODO (next phase): deduplicate by transaction id before modifying PlayerData.
        PlayerData.instance.premium += amount;
        PlayerData.instance.Save();

#if UNITY_ANALYTICS
        var transactionId = product.transactionID;
        var transactionContext = "premium_store";
        var itemId = product.definition.id;
        var itemType = "consumable";
        var level = PlayerData.instance.rank.ToString();

        AnalyticsEvent.IAPTransaction(
            transactionContext,
            (float)product.metadata.localizedPrice,
            itemId,
            itemType,
            level,
            transactionId
        );

        AnalyticsEvent.ItemAcquired(
            AcquisitionType.Premium,
            transactionContext,
            amount,
            itemId,
            PlayerData.instance.premium,
            itemType,
            level,
            transactionId
        );
#endif
    }

    public void ProductError(Product product, PurchaseFailureReason reason)
    {
        var productId = product != null && product.definition != null ? product.definition.id : "unknown";
        Debug.LogError("Product : " + productId + " couldn't be bought because " + reason);
    }

    public void ProductError(Product product, PurchaseFailureDescription description)
    {
        var productId = product != null && product.definition != null ? product.definition.id : "unknown";
        var reason = description != null ? description.reason.ToString() : "unknown";
        Debug.LogError("Product : " + productId + " couldn't be bought because " + reason);
    }
}
