#if UNITY_EDITOR
/// <summary>
/// Edit Mode tests may set FulfillmentEnabledOverride; player builds omit this type.
/// </summary>
public static class IAPPurchaseFulfillmentPolicyTestHook
{
    public static bool? FulfillmentEnabledOverride;
}
#endif
