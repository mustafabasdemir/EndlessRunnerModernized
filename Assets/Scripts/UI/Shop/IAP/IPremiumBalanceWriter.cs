/// <summary>
/// Applies premium currency to durable player state. Separated for unit testing without PlayerData.
/// </summary>
public interface IPremiumBalanceWriter
{
    bool TryAddPremium(int amount, out string error);

    /// <summary>Used when durable transaction state fails after a grant attempt.</summary>
    bool TryRollbackPremium(int amount, out string error);
}
