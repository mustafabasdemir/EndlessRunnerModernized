/// <summary>
/// Default wiring for runtime fulfillment (still gated by FulfillmentEnabled).
/// </summary>
public static class IAPPremiumFulfillmentServices
{
    static IAPPremiumFulfillmentProcessor s_DefaultProcessor;

    public static IAPPremiumFulfillmentProcessor DefaultProcessor =>
        s_DefaultProcessor ?? (s_DefaultProcessor = CreateDefaultProcessor());

    public static IAPPremiumFulfillmentProcessor CreateDefaultProcessor()
    {
        return new IAPPremiumFulfillmentProcessor(
            new IAPProcessedTransactionFileStore(),
            new IAPDevelopmentReceiptValidator(),
            new PlayerDataPremiumBalanceWriter());
    }

    internal static void ResetDefaultForTesting()
    {
        s_DefaultProcessor = null;
    }
}
