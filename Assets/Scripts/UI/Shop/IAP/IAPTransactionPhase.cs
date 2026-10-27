/// <summary>
/// Durable fulfillment phase for crash-safe recovery. Stored in the IAP sidecar file, not in save.bin.
/// </summary>
public enum IAPTransactionPhase
{
    None = 0,
    /// <summary>Transaction id reserved; premium not yet written to player save.</summary>
    Pending = 1,
    /// <summary>Premium granted and player save flushed; store confirmation may still be pending.</summary>
    PremiumPersisted = 2,
    /// <summary>Fulfillment fully completed for this transaction id.</summary>
    Completed = 3,
}
