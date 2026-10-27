/// <summary>
/// Normalized purchase data for fulfillment, independent of Codeless vs StoreController callbacks.
/// </summary>
public struct IAPPurchaseContext
{
    public string TransactionId;
    public string ProductId;
    /// <summary>Platform receipt JSON when available (PendingOrder on IAP 5).</summary>
    public string ReceiptPayload;
    public string StoreName;

    public bool HasValidTransactionId => !string.IsNullOrWhiteSpace(TransactionId);
    public bool HasValidProductId => !string.IsNullOrWhiteSpace(ProductId);
}
