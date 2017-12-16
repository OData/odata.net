//---------------------------------------------------------------------
// <copyright file="BatchWriterStatesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.Platforms;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for correct handling of batch writer states.
    /// </summary>
    [TestClass, TestCase]
    public class BatchWriterStatesTests : ODataWriterTestCase
    {
        private enum BatchWriterAction
        {
            StartBatch,
            EndBatch,
            StartChangeset,
            EndChangeset,
            Operation,
            GetOperationStream,
            DisposeOperationStream
        }

        private class BatchWriterStatesTestSetupResult
        {
            public object Message { get; set; }
            public Stream MessageStream { get; set; }
        }

        private class BatchWriterStatesTestDescriptor
        {
            public Func<ODataBatchWriterTestWrapper, TestStream, WriterTestConfiguration, BatchWriterStatesTestSetupResult> Setup { get; set; }
            public Dictionary<BatchWriterAction, ExpectedException> ExpectedResults { get; set; }
            public bool ReadOperationReady { get; set; }
        }

        [TestMethod, Variation(Description = "Validates correct checking of states in ODataBatchWriter implementations.")]
        public void BatchWriterStatesTest()
        {
            var testCases = new BatchWriterStatesTestDescriptor[]
            {
                // Start
                new BatchWriterStatesTestDescriptor {
                    Setup = null,
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, null },
                        { BatchWriterAction.EndBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromStart") },
                        { BatchWriterAction.StartChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromStart") },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet") },
                        { BatchWriterAction.Operation, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromStart") },
                        { BatchWriterAction.GetOperationStream, null },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = true
                },

                // BatchStarted
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        return null;
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromBatchStarted") },
                        { BatchWriterAction.EndBatch, null },
                        { BatchWriterAction.StartChangeset, null },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet") },
                        { BatchWriterAction.Operation, null },
                        { BatchWriterAction.GetOperationStream, null },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = true
                },

                // ChangesetStarted
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        return null;
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromChangeSetStarted") },
                        { BatchWriterAction.EndBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet") },
                        { BatchWriterAction.StartChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet") },
                        { BatchWriterAction.EndChangeset, null },
                        { BatchWriterAction.Operation, null },
                        { BatchWriterAction.GetOperationStream, null },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = false
                },

                // OperationCreated - Read
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            return new BatchWriterStatesTestSetupResult { Message = w.CreateOperationRequestMessage("GET", new Uri("http://odata.org")) };
                        }
                        else
                        {
                            return new BatchWriterStatesTestSetupResult { Message = w.CreateOperationResponseMessage() };
                        }
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationCreated") },
                        { BatchWriterAction.EndBatch, null },
                        { BatchWriterAction.StartChangeset, null },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet") },
                        { BatchWriterAction.Operation, null },
                        { BatchWriterAction.GetOperationStream, null },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = true
                },

                // OperationStreamRequested - Read
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            return GetOperationStream(w.CreateOperationRequestMessage("GET", new Uri("http://odata.org")), tc);
                        }
                        else
                        {
                            return GetOperationStream(w.CreateOperationResponseMessage(), tc);
                        }
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.EndBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.StartChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.Operation, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.GetOperationStream, ODataExpectedExceptions.ODataException("ODataBatchOperationMessage_VerifyNotCompleted") },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = true
                },

                // OperationStreamDisposed - Read
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        BatchWriterStatesTestSetupResult result;
                        if (tc.IsRequest)
                        {
                            result = GetOperationStream(w.CreateOperationRequestMessage("GET", new Uri("http://odata.org")), tc);
                        }
                        else
                        {
                            result = GetOperationStream(w.CreateOperationResponseMessage(), tc);
                        }

                        result.MessageStream.Dispose();
                        return result;
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed") },
                        { BatchWriterAction.EndBatch, null },
                        { BatchWriterAction.StartChangeset, null },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet") },
                        { BatchWriterAction.Operation, null },
                        { BatchWriterAction.GetOperationStream, ODataExpectedExceptions.ODataException("ODataBatchOperationMessage_VerifyNotCompleted") },
                        // Calling IDisposable.Dispose  should not throw.
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = true
                },

                // OperationCreated - Update
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        if (tc.IsRequest)
                        {
                            return new BatchWriterStatesTestSetupResult { Message = w.CreateOperationRequestMessage("POST", new Uri("http://odata.org"), "1") };
                        }
                        else
                        {
                            return new BatchWriterStatesTestSetupResult { Message = w.CreateOperationResponseMessage() };
                        }
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationCreated") },
                        { BatchWriterAction.EndBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet") },
                        { BatchWriterAction.StartChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet") },
                        { BatchWriterAction.EndChangeset, null },
                        { BatchWriterAction.Operation, null },
                        { BatchWriterAction.GetOperationStream, null },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = false
                },

                // OperationStreamRequested - Update
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        if (tc.IsRequest)
                        {
                            return GetOperationStream(w.CreateOperationRequestMessage("POST", new Uri("http://odata.org"), "2"), tc);
                        }
                        else
                        {
                            return GetOperationStream(w.CreateOperationResponseMessage(), tc);
                        }
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.EndBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.StartChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.Operation, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested") },
                        { BatchWriterAction.GetOperationStream, ODataExpectedExceptions.ODataException("ODataBatchOperationMessage_VerifyNotCompleted") },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = false
                },

                // OperationStreamDisposed - Update
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        BatchWriterStatesTestSetupResult result;
                        if (tc.IsRequest)
                        {
                            result = GetOperationStream(w.CreateOperationRequestMessage("POST", new Uri("http://odata.org"), "3"), tc);
                        }
                        else
                        {
                            result = GetOperationStream(w.CreateOperationResponseMessage(), tc);
                        }

                        result.MessageStream.Dispose();
                        return result;
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed") },
                        { BatchWriterAction.EndBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet") },
                        { BatchWriterAction.StartChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet") },
                        { BatchWriterAction.EndChangeset, null },
                        { BatchWriterAction.Operation, null },
                        { BatchWriterAction.GetOperationStream, ODataExpectedExceptions.ODataException("ODataBatchOperationMessage_VerifyNotCompleted") },
                        // Calling IDisposable.Dispose  should not throw.
                        { BatchWriterAction.DisposeOperationStream, null},
                    },
                    ReadOperationReady = false
                },

                // ChangesetCompleted
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        w.WriteStartChangeset();
                        w.WriteEndChangeset();
                        return null;
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromChangeSetCompleted") },
                        { BatchWriterAction.EndBatch, null },
                        { BatchWriterAction.StartChangeset, null },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet") },
                        { BatchWriterAction.Operation, null },
                        { BatchWriterAction.GetOperationStream, null },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = true
                },

                // BatchCompleted
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        w.WriteStartBatch();
                        w.WriteEndBatch();
                        return null;
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromBatchCompleted") },
                        { BatchWriterAction.EndBatch, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromBatchCompleted") },
                        { BatchWriterAction.StartChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromBatchCompleted") },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet") },
                        { BatchWriterAction.Operation, ODataExpectedExceptions.ODataException("ODataBatchWriter_InvalidTransitionFromBatchCompleted") },
                        { BatchWriterAction.GetOperationStream, null },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = true
                },

                // FatalExceptionThrown
                new BatchWriterStatesTestDescriptor {
                    Setup = (w, s, tc) => {
                        s.FailNextCall = true;
                        w.WriteStartBatch();
                        if (tc.IsRequest)
                        {
                            w.CreateOperationRequestMessage("GET", new Uri("http://odata.org"));
                        }
                        else
                        {
                            w.CreateOperationResponseMessage();
                        }
                        TestExceptionUtils.RunCatching(() => w.Flush());
                        return null;
                    },
                    ExpectedResults = new Dictionary<BatchWriterAction, ExpectedException> { 
                        { BatchWriterAction.StartBatch, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "BatchStarted") },
                        { BatchWriterAction.EndBatch, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "BatchCompleted") },
                        { BatchWriterAction.StartChangeset, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "ChangesetStarted") },
                        { BatchWriterAction.EndChangeset, ODataExpectedExceptions.ODataException("ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet") },
                        { BatchWriterAction.Operation, ODataExpectedExceptions.ODataException("ODataWriterCore_InvalidTransitionFromError", "Error", "OperationCreated") },
                        { BatchWriterAction.GetOperationStream, null },
                        { BatchWriterAction.DisposeOperationStream, null },
                    },
                    ReadOperationReady = true
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                EnumExtensionMethods.GetValues<BatchWriterAction>().Cast<BatchWriterAction>(),
                this.WriterTestConfigurationProvider.DefaultFormatConfigurations,
                (testCase, writerAction, testConfiguration) =>
                {
                    using (TestStream stream = new TestStream())
                    {
                        // We purposely don't use the using pattern around the messageWriter here. Disposing the message writer will
                        // fail here because the writer is not left in a valid state.
                        var messageWriter = TestWriterUtils.CreateMessageWriter(stream, testConfiguration, this.Assert);
                        ODataBatchWriterTestWrapper writer = messageWriter.CreateODataBatchWriter();
                        BatchWriterStatesTestSetupResult setupResult = null;
                        if (testCase.Setup != null)
                        {
                            setupResult = testCase.Setup(writer, stream, testConfiguration);
                        }

                        ExpectedException expectedException = testCase.ExpectedResults[writerAction];

                        TestExceptionUtils.ExpectedException(
                            this.Assert,
                            () => InvokeBatchWriterAction(writer, writerAction, testConfiguration, testCase.ReadOperationReady, setupResult),
                            expectedException,
                            this.ExceptionVerifier);
                    }
                });
        }

        private void InvokeBatchWriterAction(
            ODataBatchWriterTestWrapper writer,
            BatchWriterAction writerAction,
            WriterTestConfiguration testConfiguration,
            bool readOperationReady,
            BatchWriterStatesTestSetupResult setupResult)
        {
            switch (writerAction)
            {
                case BatchWriterAction.StartBatch:
                    writer.WriteStartBatch();
                    break;
                case BatchWriterAction.EndBatch:
                    writer.WriteEndBatch();
                    break;
                case BatchWriterAction.StartChangeset:
                    writer.WriteStartChangeset();
                    break;
                case BatchWriterAction.EndChangeset:
                    writer.WriteEndChangeset();
                    break;
                case BatchWriterAction.Operation:
                    if (testConfiguration.IsRequest)
                    {
                        writer.CreateOperationRequestMessage(readOperationReady ? "GET" : "POST", new Uri("http://odata.org"), "4");
                    }
                    else
                    {
                        writer.CreateOperationResponseMessage();
                    }

                    break;
                case BatchWriterAction.GetOperationStream:
                    GetOperationStream(setupResult == null ? null : setupResult.Message, testConfiguration);
                    break;
                case BatchWriterAction.DisposeOperationStream:
                    Stream s = setupResult == null ? null : setupResult.MessageStream;
                    if (s != null)
                    {
                        try
                        {
                            s.Dispose();
                        }
                        catch (ObjectDisposedException e)
                        {
                            // Replace the disposed exception with OData exception so that our type checks work correctly.
                            // No need to verify that the Dispose thrown the right type of the exception.
                            throw new ODataException(e.Message);
                        }
                    }
                    break;
            }
        }

        private static BatchWriterStatesTestSetupResult GetOperationStream(object message, WriterTestConfiguration testConfiguration)
        {
            BatchWriterStatesTestSetupResult result = new BatchWriterStatesTestSetupResult { Message = message };

            ODataBatchOperationRequestMessage requestMessage = message as ODataBatchOperationRequestMessage;
            if (requestMessage != null)
            {
                if (testConfiguration.Synchronous)
                {
                    result.MessageStream = requestMessage.GetStream();
                    return result;
                }
                else
                {
                    // TODO: 191417: Enable async Tests on Phone and Silverlight when Product Supports them 
#if SILVERLIGHT || WINDOWS_PHONE
                    throw new TaupoNotSupportedException("This test is not supported in aSynchronous mode in Silverlight or Phone");
#else
                    var t = requestMessage.GetStreamAsync();
                    t.Wait();
                    result.MessageStream = t.Result;
                    return result;
#endif
                }
            }

            ODataBatchOperationResponseMessage responseMessage = message as ODataBatchOperationResponseMessage;
            if (responseMessage != null)
            {
                if (testConfiguration.Synchronous)
                {
                    result.MessageStream = responseMessage.GetStream();
                    return result;
                }
                else
                {
                    // TODO: Enable async Tests on Phone and Silverlight when Product Supports them 
#if SILVERLIGHT || WINDOWS_PHONE
                    throw new TaupoNotSupportedException("This test is not supported in aSynchronous mode in Silverlight or Phone");
#else

                    var t = responseMessage.GetStreamAsync();
                    t.Wait();
                    result.MessageStream = t.Result;
                    return result;
#endif
                }
            }

            return null;
        }
    }
}
