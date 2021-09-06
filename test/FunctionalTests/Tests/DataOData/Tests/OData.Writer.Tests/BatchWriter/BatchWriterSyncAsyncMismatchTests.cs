//---------------------------------------------------------------------
// <copyright file="BatchWriterSyncAsyncMismatchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;

    /// <summary>
    /// Tests for verifies that mismatch between synchronous and asynchronous calls is not allowed.
    /// </summary>
    [TestClass, TestCase]
    public class BatchWriterSyncAsyncMismatchTests : ODataWriterTestCase
    {
        private class SyncAsyncCall
        {
            public Action<ODataBatchWriterTestWrapper, WriterTestConfiguration> Sync { get; set; }
            public Action<ODataBatchWriterTestWrapper, WriterTestConfiguration> Async { get; set; }
            public bool ShouldNotFail { get; set; }
            public string DebugDescription { get; set; }
            public override string ToString()
            {
                return this.DebugDescription;
            }
        }

        private SyncAsyncCall[] TestCalls = new SyncAsyncCall[]
        {
            // Flush
            new SyncAsyncCall
            {
                Sync = (w, tc) => w.BatchWriter.Flush(),
                Async = (w, tc) => w.BatchWriter.FlushAsync().Wait(),
                DebugDescription = "Flush"
            },

            // WriteStartBatch
            new SyncAsyncCall
            {
                Sync = (w, tc) => w.BatchWriter.WriteStartBatch(),
                Async = (w, tc) => w.BatchWriter.WriteStartBatchAsync().Wait(),
                DebugDescription = "WriteStartBatch"
            },

            // WriteEndBatch
            new SyncAsyncCall
            {
                Sync = (w, tc) => { w.WriteStartBatch(); w.BatchWriter.WriteEndBatch(); },
                Async = (w, tc) => { w.WriteStartBatch(); w.BatchWriter.WriteEndBatchAsync().Wait(); },
                DebugDescription = "WriteEndBatch"
            },

            // WriteStartChangeset
            new SyncAsyncCall
            {
                Sync = (w, tc) => { w.WriteStartBatch(); w.BatchWriter.WriteStartChangeset(); },
                Async = (w, tc) => { w.WriteStartBatch(); w.BatchWriter.WriteStartChangesetAsync().Wait(); },
                DebugDescription = "WriteStartChangeset"
            },

            // WriteEndChangeSet
            new SyncAsyncCall
            {
                Sync = (w, tc) => { w.WriteStartBatch(); w.WriteStartChangeset(); w.BatchWriter.WriteEndChangeset(); },
                Async = (w, tc) => { w.WriteStartBatch(); w.WriteStartChangeset(); w.BatchWriter.WriteEndChangesetAsync().Wait(); },
                DebugDescription = "WriteEndChangeset"
            },

            // CreateOperation on top-level
            new SyncAsyncCall
            {
                Sync = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            w.BatchWriter.CreateOperationRequestMessage("GET", new Uri("http://odata.org"), null);
                        }
                        else
                        {
                            w.BatchWriter.CreateOperationResponseMessage(null);
                        }
                    },
                Async = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            w.BatchWriter.CreateOperationRequestMessageAsync("GET", new Uri("http://odata.org"), null).WaitForResult();
                        }
                        else
                        {
                            w.BatchWriter.CreateOperationResponseMessageAsync(null).WaitForResult();
                        }
                    },
                DebugDescription = "CreateOperation on top-level"
            },

            // CreateOperation in changeset
            new SyncAsyncCall
            {
                Sync = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        if (tc.IsRequest)
                        {
                            w.BatchWriter.CreateOperationRequestMessage("PUT", new Uri("http://odata.org"),"1");
                        }
                        else
                        {
                            w.BatchWriter.CreateOperationResponseMessage("1");
                        }
                    },
                Async = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        if (tc.IsRequest)
                        {
                            w.BatchWriter.CreateOperationRequestMessageAsync("PUT", new Uri("http://odata.org"),"2").WaitForResult();
                        }
                        else
                        {
                            w.BatchWriter.CreateOperationResponseMessageAsync("2").WaitForResult();
                        }
                    },
                DebugDescription = "CreateOperation in changeset"
            },

            // Read message get stream - this should never fail, since we must support mix/match of sync/async on the returned message
            new SyncAsyncCall
            {
                Sync = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            var message = w.CreateOperationRequestMessage("GET", new Uri("http://odata.org"));
                            using (Stream s = message.GetStream()) { }
                        }
                        else
                        {
                            var message = w.CreateOperationResponseMessage();
                            using (Stream s = message.GetStream()) { }
                        }
                    },
                Async = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            var message = w.CreateOperationRequestMessage("GET", new Uri("http://odata.org"));
                            var t = message.GetStreamAsync();
                            t.Wait();
                            t.Result.Dispose();
                        }
                        else
                        {
                            var message = w.CreateOperationResponseMessage();
                            var t = message.GetStreamAsync();
                            t.Wait();
                            t.Result.Dispose();
                        }
                    },
                ShouldNotFail = true,
                DebugDescription = "Read message get stream"
            },

            // Read message write to stream - this should never fail, since we must support mix/match of sync/async on the returned message
            new SyncAsyncCall
            {
                Sync = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            var message = w.CreateOperationRequestMessage("GET", new Uri("http://odata.org"));
                            using (Stream s = GetMessageStream(message, tc))
                            {
                                s.Write(new byte[] { 1, 2, 3 }, 0, 3);
                            }
                        }
                        else
                        {
                            var message = w.CreateOperationResponseMessage();
                            using (Stream s = GetMessageStream(message, tc))
                            {
                                s.Write(new byte[] { 1, 2, 3 }, 0, 3);
                            }
                        }
                    },
                Async = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            var message = w.CreateOperationRequestMessage("GET", new Uri("http://odata.org"));
                            using (Stream s = GetMessageStream(message, tc))
                            {
                                WriteToStreamAsync(s);
                            }
                        }
                        else
                        {
                            var message = w.CreateOperationResponseMessage();
                            using (Stream s = GetMessageStream(message, tc))
                            {
                                WriteToStreamAsync(s);
                            }
                        }
                    },
                ShouldNotFail = true,
                DebugDescription = "Read message write to stream"
            },

            // Update message get stream - this should never fail, since we must support mix/match of sync/async on the returned message
            new SyncAsyncCall
            {
                Sync = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        if (tc.IsRequest)
                        {
                            var message = w.CreateOperationRequestMessage("POST", new Uri("http://odata.org"));
                            using (Stream s = message.GetStream()) { }
                        }
                        else
                        {
                            var message = w.CreateOperationResponseMessage();
                            using (Stream s = message.GetStream()) { }
                        }
                    },
                Async = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        if (tc.IsRequest)
                        {
                            var message = w.CreateOperationRequestMessage("POST", new Uri("http://odata.org"));
                            var t = message.GetStreamAsync();
                            t.Wait();
                            t.Result.Dispose();
                        }
                        else
                        {
                            var message = w.CreateOperationResponseMessage();
                            var t = message.GetStreamAsync();
                            t.Wait();
                            t.Result.Dispose();
                        }
                    },
                ShouldNotFail = true,
                DebugDescription = "Update message get stream"
            },

            // Update message write to stream - this should never fail, since we must support mix/match of sync/async on the returned message
            new SyncAsyncCall
            {
                Sync = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        if (tc.IsRequest)
                        {
                            var message = w.CreateOperationRequestMessage("PUT", new Uri("http://odata.org"));
                            using (Stream s = GetMessageStream(message, tc))
                            {
                                s.Write(new byte[] { 1, 2, 3 }, 0, 3);
                            }
                        }
                        else
                        {
                            var message = w.CreateOperationResponseMessage();
                            using (Stream s = GetMessageStream(message, tc))
                            {
                                s.Write(new byte[] { 1, 2, 3 }, 0, 3);
                            }
                        }
                    },
                Async = (w, tc) =>
                    {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        if (tc.IsRequest)
                        {
                            var message = w.CreateOperationRequestMessage("PUT", new Uri("http://odata.org"));
                            using (Stream s = GetMessageStream(message, tc))
                            {
                                WriteToStreamAsync(s);
                            }
                        }
                        else
                        {
                            var message = w.CreateOperationResponseMessage();
                            using (Stream s = GetMessageStream(message, tc))
                            {
                                WriteToStreamAsync(s);
                            }
                        }
                    },
                ShouldNotFail = true,
                DebugDescription = "Update message write to stream"
            },
        };

        [TestMethod, Variation(Description = "Verifies that sync and async calls cannot be mixed on a single batch writer.")]
        public void SyncAsyncMismatchTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                TestCalls,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurations,
                new bool[] { false, true },
                (testCall, testConfiguration, testSynchronousCall) =>
                {
                    using(var memoryStream = new TestStream())
                    {
                        // We purposely don't use the using pattern around the messageWriter here. Disposing the message writer will
                        // fail here because the writer is not left in a valid state.
                        var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert);
                        ODataBatchWriterTestWrapper writer = messageWriter.CreateODataBatchWriter();
                        this.Assert.ExpectedException<ODataException>(
                            () =>
                            {
                                if (testSynchronousCall)
                                {
                                    testCall.Sync(writer, testConfiguration);
                                }
                                else
                                {
                                    testCall.Async(writer, testConfiguration);
                                }
                            },
                            testConfiguration.Synchronous == testSynchronousCall || testCall.ShouldNotFail ?
                                null :
                                testSynchronousCall ?
                                "A synchronous operation was called on an asynchronous batch writer. Calls on a batch writer instance must be either all synchronous or all asynchronous." :
                                "An asynchronous operation was called on a synchronous batch writer. Calls on a batch writer instance must be either all synchronous or all asynchronous.");
                    }
                });
        }

        private static Stream GetMessageStream(ODataBatchOperationRequestMessage requestMessage, WriterTestConfiguration testConfiguration)
        {
            if (testConfiguration.Synchronous)
            {
                return requestMessage.GetStream();
            }
            else
            {
                var t = requestMessage.GetStreamAsync();
                t.Wait();
                return t.Result;
            }
        }

        private static Stream GetMessageStream(ODataBatchOperationResponseMessage responseMessage, WriterTestConfiguration testConfiguration)
        {
            if (testConfiguration.Synchronous)
            {
                return responseMessage.GetStream();
            }
            else
            {
                var t = responseMessage.GetStreamAsync();
                t.Wait();
                return t.Result;
            }
        }

        private static void WriteToStreamAsync(Stream s)
        {
            var t = Task.Factory.FromAsync(
                (callback, state) => s.BeginWrite(new byte[] { 1, 2, 3 }, 0, 3, callback, state),
                (ar) => s.EndWrite(ar),
                null);
            t.Wait();
        }
    }
}
