using UnityEngine.Purchasing;

/// <summary>
/// Maps Unity IAP Product / IAP 5 Order models into IAPPurchaseContext.
/// </summary>
public static class IAPPurchaseContextFactory
{
    public static IAPPurchaseContext FromProduct(Product product)
    {
        if (product?.definition == null)
            return default;

        return new IAPPurchaseContext
        {
            TransactionId = product.transactionID,
            ProductId = product.definition.id,
            ReceiptPayload = product.receipt,
            StoreName = product.definition.storeSpecificId,
        };
    }

    public static IAPPurchaseContext FromPendingOrder(PendingOrder order)
    {
        if (order?.Info == null)
            return default;

        var productId = ResolvePrimaryProductId(order);
        return new IAPPurchaseContext
        {
            TransactionId = order.Info.TransactionID,
            ProductId = productId,
            ReceiptPayload = order.Info.Receipt,
            StoreName = null,
        };
    }

    static string ResolvePrimaryProductId(PendingOrder order)
    {
        var items = order.CartOrdered?.Items();
        if (items == null || items.Count == 0)
            return null;

        var product = items[0].Product;
        return product?.definition?.id;
    }
}
