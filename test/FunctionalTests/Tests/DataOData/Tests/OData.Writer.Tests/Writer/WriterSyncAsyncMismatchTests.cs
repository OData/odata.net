//---------------------------------------------------------------------
// <copyright file="WriterSyncAsyncMismatchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// Async APIs are not supported in the Product for Silverlight and WP7. 
// TODO: Enable async Tests on Phone and Silverlight when Product Supports them 
#if !SILVERLIGHT && !WINDOWS_PHONE
namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
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
    public class WriterSyncAsyncMismatchTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        private class SyncAsyncCall
        {
            public Action<ODataWriterTestWrapper> Sync { get; set; }
            public Action<ODataWriterTestWrapper> Async { get; set; }
            public bool AssumesFeedWriter { get; set; }
        }

        private SyncAsyncCall[] TestCalls = new SyncAsyncCall[]
        {
            // Flush
            new SyncAsyncCall
            {
                Sync = (w) => w.Writer.Flush(),
                Async = (w) => w.Writer.FlushAsync().Wait()
            },

            // WriteStart(feed)
            new SyncAsyncCall
            {
                Sync = (w) => w.Writer.WriteStart(ObjectModelUtils.CreateDefaultFeed()),
                Async = (w) => w.Writer.WriteStartAsync(ObjectModelUtils.CreateDefaultFeed()).Wait(),
                AssumesFeedWriter = true
            },

            // WriteStart(entry)
            new SyncAsyncCall
            {
                Sync = (w) => w.Writer.WriteStart(ObjectModelUtils.CreateDefaultEntry()),
                Async = (w) => w.Writer.WriteStartAsync(ObjectModelUtils.CreateDefaultEntry()).Wait()
            },

            // WriteStart(link)
            new SyncAsyncCall
            {
                Sync = (w) => { w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); w.Writer.WriteStart(ObjectModelUtils.CreateDefaultSingletonLink()); },
                Async = (w) => { w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); w.Writer.WriteStartAsync(ObjectModelUtils.CreateDefaultSingletonLink()).Wait(); }
            },

            // WriteEnd
            new SyncAsyncCall
            {
                Sync = (w) => { w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); w.Writer.WriteEnd(); },
                Async = (w) => { w.WriteStart(ObjectModelUtils.CreateDefaultEntry()); w.Writer.WriteEndAsync().Wait(); }
            },
        };

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that sync and async calls cannot be mixed on a single writer.")]
        public void SyncAsyncMismatchTest()
        {
            // ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                TestCalls,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                new bool[] { false, true },
                new bool[] { false, true },
                (testCall, testConfiguration, writingFeed, testSynchronousCall) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (TestStream memoryStream = new TestStream())
                    {
                        // We purposely don't use the using pattern around the messageWriter here. Disposing the message writer will
                        // fail here because the writer is not left in a valid state.
                        var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert);

                        // Note that the CreateODataWriter call will call either sync or async variant based on the testConfiguration
                        // which is independent axis to the testSynchronousCall and thus we will end up with async creation but sync write
                        // and vice versa.
                        ODataWriter odataWriter = messageWriter.CreateODataWriter(writingFeed);
                        ODataWriterTestWrapper writer = (ODataWriterTestWrapper)odataWriter;
                        if (testCall.AssumesFeedWriter)
                        {
                            if (!writingFeed)
                            {
                                // Skip this case since we need feed writer for this case.
                                return;
                            }
                        }
                        else
                        {
                            if (writingFeed)
                            {
                                writer.WriteStart(ObjectModelUtils.CreateDefaultFeed());
                            }
                        }

                        this.Assert.ExpectedException<ODataException>(
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
                                testSynchronousCall ?
                                "A synchronous operation was called on an asynchronous writer. Calls on a writer instance must be either all synchronous or all asynchronous." :
                                "An asynchronous operation was called on a synchronous writer. Calls on a writer instance must be either all synchronous or all asynchronous.");
                    }
                });
        }
    }
}
#endif