using UnityEngine;

/// <summary>
/// Writes premium grants through the existing PlayerData save pipeline.
/// </summary>
public sealed class PlayerDataPremiumBalanceWriter : IPremiumBalanceWriter
{
    public bool TryAddPremium(int amount, out string error)
    {
        error = null;
        if (amount <= 0)
        {
            error = "Premium amount must be positive.";
            return false;
        }

        if (PlayerData.instance == null)
        {
            error = "PlayerData.instance is null.";
            return false;
        }

        PlayerData.instance.premium += amount;
        PlayerData.instance.Save();
        return true;
    }

    public bool TryRollbackPremium(int amount, out string error)
    {
        error = null;
        if (amount <= 0)
        {
            error = "Premium amount must be positive.";
            return false;
        }

        if (PlayerData.instance == null)
        {
            error = "PlayerData.instance is null.";
            return false;
        }

        PlayerData.instance.premium = Mathf.Max(0, PlayerData.instance.premium - amount);
        PlayerData.instance.Save();
        return true;
    }
}
