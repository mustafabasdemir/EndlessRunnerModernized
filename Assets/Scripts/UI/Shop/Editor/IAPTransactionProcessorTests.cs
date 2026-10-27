using System.IO;
using NUnit.Framework;

/// <summary>
/// Edit Mode tests for IAP idempotency (synthetic transaction ids, no store).
/// </summary>
public class IAPTransactionProcessorTests
{
    sealed class TestPremiumWriter : IPremiumBalanceWriter
    {
        public int Premium;
        public int AddCallCount;
        public int RollbackCallCount;

        public bool TryAddPremium(int amount, out string error)
        {
            error = null;
            AddCallCount++;
            Premium += amount;
            return true;
        }

        public bool TryRollbackPremium(int amount, out string error)
        {
            error = null;
            RollbackCallCount++;
            Premium -= amount;
            return true;
        }
    }

    static IAPPremiumFulfillmentProcessor CreateProcessor(
        IIAPProcessedTransactionStore store,
        IPremiumBalanceWriter writer)
    {
        return new IAPPremiumFulfillmentProcessor(
            store,
            new IAPDevelopmentReceiptValidator(),
            writer);
    }

    static IAPPurchaseContext Context(string txId, string productId = "10_premium")
    {
        return new IAPPurchaseContext
        {
            TransactionId = txId,
            ProductId = productId,
            ReceiptPayload = "synthetic-receipt",
        };
    }

    [Test]
    public void FulfillmentDisabled_DoesNotGrant()
    {
        var store = new IAPInMemoryProcessedTransactionStore();
        var writer = new TestPremiumWriter();
        var processor = CreateProcessor(store, writer);

        var result = processor.TryFulfill(Context("tx-disabled-1"), out _);

        Assert.AreEqual(IAPFulfillmentResult.FulfillmentDisabled, result);
        Assert.AreEqual(0, writer.Premium);
        Assert.AreEqual(0, writer.AddCallCount);
    }

    [Test]
    public void DuplicateTransaction_IsIdempotent_WhenFulfillmentForcedOn()
    {
        var previous = IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride;
        IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride = true;
        try
        {
            var store = new IAPInMemoryProcessedTransactionStore();
            var writer = new TestPremiumWriter();
            var processor = CreateProcessor(store, writer);
            var ctx = Context("tx-dup-001");

            Assert.AreEqual(IAPFulfillmentResult.Success, processor.TryFulfill(ctx, out _));
            Assert.AreEqual(IAPFulfillmentResult.AlreadyProcessed, processor.TryFulfill(ctx, out _));
            Assert.AreEqual(10, writer.Premium);
            Assert.AreEqual(1, writer.AddCallCount);
        }
        finally
        {
            IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride = previous;
        }
    }

    [Test]
    public void PersistFailureAfterGrant_RollsBackPremium()
    {
        var previous = IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride;
        IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride = true;
        try
        {
            var store = new IAPInMemoryProcessedTransactionStore { FailMarkPremiumPersistedOnce = true };
            var writer = new TestPremiumWriter();
            var processor = CreateProcessor(store, writer);
            var ctx = Context("tx-rollback-1");

            var first = processor.TryFulfill(ctx, out _);
            Assert.AreEqual(IAPFulfillmentResult.TransactionStoreFailed, first);
            Assert.AreEqual(0, writer.Premium);
            Assert.AreEqual(1, writer.RollbackCallCount);

            var second = processor.TryFulfill(ctx, out _);
            Assert.AreEqual(IAPFulfillmentResult.Success, second);
            Assert.AreEqual(10, writer.Premium);
            Assert.AreEqual(1, writer.AddCallCount);
        }
        finally
        {
            IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride = previous;
        }
    }

    [Test]
    public void FileStore_ReloadsCompletedTransaction()
    {
        var previous = IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride;
        IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride = true;
        var tempFile = Path.Combine(Path.GetTempPath(), "iap_test_" + System.Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            var writer = new TestPremiumWriter();
            var store1 = new IAPProcessedTransactionFileStore(tempFile);
            var processor1 = CreateProcessor(store1, writer);
            var ctx = Context("tx-file-001");

            Assert.AreEqual(IAPFulfillmentResult.Success, processor1.TryFulfill(ctx, out _));

            var store2 = new IAPProcessedTransactionFileStore(tempFile);
            var processor2 = CreateProcessor(store2, new TestPremiumWriter());
            Assert.AreEqual(IAPFulfillmentResult.AlreadyProcessed, processor2.TryFulfill(ctx, out _));
        }
        finally
        {
            IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride = previous;
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Test]
    public void UnknownProduct_IsRejected()
    {
        var previous = IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride;
        IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride = true;
        try
        {
            var store = new IAPInMemoryProcessedTransactionStore();
            var writer = new TestPremiumWriter();
            var processor = CreateProcessor(store, writer);

            var result = processor.TryFulfill(Context("tx-unknown", "invalid_sku"), out _);

            Assert.AreEqual(IAPFulfillmentResult.UnknownProduct, result);
            Assert.AreEqual(0, writer.AddCallCount);
        }
        finally
        {
            IAPPurchaseFulfillmentPolicyTestHook.FulfillmentEnabledOverride = previous;
        }
    }
}
