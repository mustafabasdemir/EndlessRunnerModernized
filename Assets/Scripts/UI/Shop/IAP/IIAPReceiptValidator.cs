/// <summary>
/// Receipt validation before premium is granted. Production should use server-side validation
/// (Google Play Developer API / Apple verifyReceipt) and durable remote entitlement storage.
/// </summary>
public interface IIAPReceiptValidator
{
    IAPReceiptValidationResult Validate(IAPPurchaseContext context);
}

public struct IAPReceiptValidationResult
{
    public bool IsValid;
    public string Diagnostic;

    public static IAPReceiptValidationResult Valid(string diagnostic = null)
    {
        return new IAPReceiptValidationResult { IsValid = true, Diagnostic = diagnostic };
    }

    public static IAPReceiptValidationResult Invalid(string diagnostic)
    {
        return new IAPReceiptValidationResult { IsValid = false, Diagnostic = diagnostic };
    }
}
