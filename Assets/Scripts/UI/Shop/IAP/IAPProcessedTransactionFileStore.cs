using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Persists processed IAP transaction phases beside save.bin (does not change PlayerData format).
/// </summary>
public sealed class IAPProcessedTransactionFileStore : IIAPProcessedTransactionStore
{
    const int FileVersion = 1;
    const string DefaultFileName = "iap_processed_transactions_v1.bin";

    readonly string m_FilePath;
    readonly object m_Lock = new object();
    readonly Dictionary<string, TransactionEntry> m_Entries = new Dictionary<string, TransactionEntry>();

    struct TransactionEntry
    {
        public string ProductId;
        public int PremiumAmount;
        public IAPTransactionPhase Phase;
        public long UtcTicks;
    }

    public IAPProcessedTransactionFileStore(string filePath = null)
    {
        m_FilePath = filePath ?? Path.Combine(Application.persistentDataPath, DefaultFileName);
        LoadFromDisk();
    }

    public IAPTransactionPhase GetPhase(string transactionId)
    {
        if (string.IsNullOrEmpty(transactionId))
            return IAPTransactionPhase.None;

        lock (m_Lock)
        {
            return m_Entries.TryGetValue(transactionId, out var entry)
                ? entry.Phase
                : IAPTransactionPhase.None;
        }
    }

    public bool TryBeginTransaction(string transactionId, string productId, int premiumAmount)
    {
        if (string.IsNullOrWhiteSpace(transactionId) || string.IsNullOrWhiteSpace(productId) || premiumAmount <= 0)
            return false;

        lock (m_Lock)
        {
            if (m_Entries.ContainsKey(transactionId))
                return false;

            m_Entries[transactionId] = new TransactionEntry
            {
                ProductId = productId,
                PremiumAmount = premiumAmount,
                Phase = IAPTransactionPhase.Pending,
                UtcTicks = DateTime.UtcNow.Ticks,
            };

            return SaveToDiskLocked();
        }
    }

    public bool TryMarkPremiumPersisted(string transactionId)
    {
        return TryAdvancePhase(transactionId, IAPTransactionPhase.Pending, IAPTransactionPhase.PremiumPersisted);
    }

    public bool TryMarkCompleted(string transactionId)
    {
        return TryAdvancePhase(transactionId, IAPTransactionPhase.PremiumPersisted, IAPTransactionPhase.Completed);
    }

    bool TryAdvancePhase(string transactionId, IAPTransactionPhase expected, IAPTransactionPhase next)
    {
        if (string.IsNullOrEmpty(transactionId))
            return false;

        lock (m_Lock)
        {
            if (!m_Entries.TryGetValue(transactionId, out var entry) || entry.Phase != expected)
                return false;

            entry.Phase = next;
            entry.UtcTicks = DateTime.UtcNow.Ticks;
            m_Entries[transactionId] = entry;
            return SaveToDiskLocked();
        }
    }

    public void ClearAllForTesting()
    {
        lock (m_Lock)
        {
            m_Entries.Clear();
            if (File.Exists(m_FilePath))
                File.Delete(m_FilePath);
        }
    }

    void LoadFromDisk()
    {
        lock (m_Lock)
        {
            m_Entries.Clear();
            if (!File.Exists(m_FilePath))
                return;

            try
            {
                using (var stream = File.OpenRead(m_FilePath))
                using (var reader = new BinaryReader(stream))
                {
                    var version = reader.ReadInt32();
                    if (version != FileVersion)
                    {
                        Debug.LogWarning(
                            "IAP transaction sidecar version mismatch (found "
                            + version
                            + ", expected "
                            + FileVersion
                            + "). Idempotency store ignored until migrated.");
                        return;
                    }

                    var count = reader.ReadInt32();
                    for (var i = 0; i < count; i++)
                    {
                        var txId = reader.ReadString();
                        var productId = reader.ReadString();
                        var amount = reader.ReadInt32();
                        var phase = (IAPTransactionPhase)reader.ReadByte();
                        var utcTicks = reader.ReadInt64();
                        m_Entries[txId] = new TransactionEntry
                        {
                            ProductId = productId,
                            PremiumAmount = amount,
                            Phase = phase,
                            UtcTicks = utcTicks,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to load IAP processed transaction sidecar: " + ex.Message);
                m_Entries.Clear();
            }
        }
    }

    bool SaveToDiskLocked()
    {
        try
        {
            var directory = Path.GetDirectoryName(m_FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var tempPath = m_FilePath + ".tmp";
            using (var stream = File.Open(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(FileVersion);
                writer.Write(m_Entries.Count);
                foreach (var pair in m_Entries)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value.ProductId);
                    writer.Write(pair.Value.PremiumAmount);
                    writer.Write((byte)pair.Value.Phase);
                    writer.Write(pair.Value.UtcTicks);
                }
            }

            if (File.Exists(m_FilePath))
                File.Delete(m_FilePath);
            File.Move(tempPath, m_FilePath);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save IAP processed transaction sidecar: " + ex.Message);
            return false;
        }
    }
}
