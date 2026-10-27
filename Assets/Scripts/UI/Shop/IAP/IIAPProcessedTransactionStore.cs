/// <summary>
/// Durable idempotency store for store transaction ids. Sidecar persistence keeps save.bin version unchanged.
/// </summary>
public interface IIAPProcessedTransactionStore
{
    IAPTransactionPhase GetPhase(string transactionId);

    /// <summary>Creates a Pending record. Returns false if the id already exists in any phase.</summary>
    bool TryBeginTransaction(string transactionId, string productId, int premiumAmount);

    bool TryMarkPremiumPersisted(string transactionId);

    bool TryMarkCompleted(string transactionId);

    void ClearAllForTesting();
}
