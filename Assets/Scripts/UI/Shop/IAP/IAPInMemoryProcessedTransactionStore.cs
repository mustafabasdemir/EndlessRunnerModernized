using System.Collections.Generic;

/// <summary>
/// In-memory transaction store for edit-mode tests and simulated persistence failures.
/// </summary>
public sealed class IAPInMemoryProcessedTransactionStore : IIAPProcessedTransactionStore
{
    readonly Dictionary<string, Entry> m_Entries = new Dictionary<string, Entry>();

    public bool FailMarkPremiumPersistedOnce;
    public int SaveAttemptCount;

    struct Entry
    {
        public string ProductId;
        public int PremiumAmount;
        public IAPTransactionPhase Phase;
    }

    public IAPTransactionPhase GetPhase(string transactionId)
    {
        if (string.IsNullOrEmpty(transactionId))
            return IAPTransactionPhase.None;

        return m_Entries.TryGetValue(transactionId, out var entry) ? entry.Phase : IAPTransactionPhase.None;
    }

    public bool TryBeginTransaction(string transactionId, string productId, int premiumAmount)
    {
        if (string.IsNullOrWhiteSpace(transactionId) || m_Entries.ContainsKey(transactionId))
            return false;

        m_Entries[transactionId] = new Entry
        {
            ProductId = productId,
            PremiumAmount = premiumAmount,
            Phase = IAPTransactionPhase.Pending,
        };
        SaveAttemptCount++;
        return true;
    }

    public bool TryMarkPremiumPersisted(string transactionId)
    {
        if (FailMarkPremiumPersistedOnce)
        {
            FailMarkPremiumPersistedOnce = false;
            SaveAttemptCount++;
            return false;
        }

        return Advance(transactionId, IAPTransactionPhase.Pending, IAPTransactionPhase.PremiumPersisted);
    }

    public bool TryMarkCompleted(string transactionId)
    {
        return Advance(transactionId, IAPTransactionPhase.PremiumPersisted, IAPTransactionPhase.Completed);
    }

    bool Advance(string transactionId, IAPTransactionPhase from, IAPTransactionPhase to)
    {
        if (!m_Entries.TryGetValue(transactionId, out var entry) || entry.Phase != from)
            return false;

        entry.Phase = to;
        m_Entries[transactionId] = entry;
        SaveAttemptCount++;
        return true;
    }

    public void ClearAllForTesting()
    {
        m_Entries.Clear();
        FailMarkPremiumPersistedOnce = false;
        SaveAttemptCount = 0;
    }
}
