//---------------------------------------------------------------------
// <copyright file="CollectionWriterSyncAsyncMismatchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// Since the Product API does not expose async on SILVERLIGHT and PHONE these tests don't apply. 
// TODO: Enable async Tests on Phone and Silverlight when Product Supports them 
#if !SILVERLIGHT && !WINDOWS_PHONE
namespace Microsoft.Test.Taupo.OData.Writer.Tests.CollectionWriter
{
    using System;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for verifies that mismatch between synchronous and asynchronous calls is not allowed.
    /// </summary>
    [TestClass, TestCase]
    public class CollectionWriterSyncAsyncMismatchTests : ODataWriterTestCase
    {
        private class SyncAsyncCall
        {
            public Action<ODataCollectionWriterTestWrapper> Sync { get; set; }
            public Action<ODataCollectionWriterTestWrapper> Async { get; set; }
        }

        private SyncAsyncCall[] TestCalls = new SyncAsyncCall[]
        {
            // Flush
            new SyncAsyncCall
            {
                Sync = (w) => w.CollectionWriter.Flush(),
                Async = (w) => w.CollectionWriter.FlushAsync().Wait()
            },

            // WriteStart
            new SyncAsyncCall
            {
                Sync = (w) => w.CollectionWriter.WriteStart(new ODataCollectionStart() { Name = "foo" }),
                Async = (w) => w.CollectionWriter.WriteStartAsync(new ODataCollectionStart() { Name = "foo" }).Wait()
            },

            // WriteItem
            new SyncAsyncCall
            {
                Sync = (w) => { w.WriteStart(new ODataCollectionStart() { Name = "foo" }); w.CollectionWriter.WriteItem(42); },
                Async = (w) => { w.WriteStart(new ODataCollectionStart() { Name = "foo" }); w.CollectionWriter.WriteItemAsync(42).Wait(); }
            },

            // WriteEnd
            new SyncAsyncCall
            {
                Sync = (w) => { w.WriteStart(new ODataCollectionStart() { Name = "foo" }); w.CollectionWriter.WriteEnd(); },
                Async = (w) => { w.WriteStart(new ODataCollectionStart() { Name = "foo" }); w.CollectionWriter.WriteEndAsync().Wait(); }
            },
        };

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that sync and async calls cannot be mixed on a single collection writer.")]
        public void SyncAsyncMismatchTest()
        {
            // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                TestCalls,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                new bool[] { false, true },
                (testCall, testConfiguration, testSynchronousCall) =>
                {
                    using (var memoryStream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                    {
                        ODataCollectionWriter collectionWriter = messageWriter.CreateODataCollectionWriter();
                        ODataCollectionWriterTestWrapper writer = (ODataCollectionWriterTestWrapper)collectionWriter;

                        this.Assert.ExpectedException(
                            () =>
                            {
                                if (testSynchronousCall)
                                {
                                    testCall.Sync(writer);
                                }
                                else
                                {
                                    testCall.Async(writer);
                                }
                            },
                            testConfiguration.Synchronous == testSynchronousCall ?
                                null :
                                testSynchronousCall
                                    ? ODataExpectedExceptions.ODataException("ODataCollectionWriterCore_SyncCallOnAsyncWriter")
                                    : ODataExpectedExceptions.ODataException("ODataCollectionWriterCore_AsyncCallOnSyncWriter"),
                            this.ExceptionVerifier);
                    }
                });
        }
    }
}
#endif
