using UnityEngine;
using UnityEngine.UI;

public static class PremiumShopVisuals
{
    static readonly Color UnaffordablePrice = new Color(0.82f, 0.24f, 0.28f, 1f);

    public static void OnItemRefreshed(ShopItemListItem item, bool canAffordCoins)
    {
        if (item == null)
            return;

        if (item.pricetext != null)
            item.pricetext.color = canAffordCoins ? PremiumUIColor.DeepNavy : UnaffordablePrice;

        var style = item.GetComponent<PremiumShopItemStyle>();
        if (style != null)
            style.ApplyBuyState(item);
    }
}
