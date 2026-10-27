/// <summary>
/// Outcome of attempting to fulfill a premium IAP transaction locally.
/// </summary>
public enum IAPFulfillmentResult
{
    Success,
    FulfillmentDisabled,
    InvalidInput,
    UnknownProduct,
    AlreadyProcessed,
    ReceiptValidationFailed,
    TransactionStoreFailed,
    PremiumPersistFailed,
    CompletionMarkFailed,
}
