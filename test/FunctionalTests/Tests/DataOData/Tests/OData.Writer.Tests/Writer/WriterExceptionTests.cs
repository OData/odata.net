//---------------------------------------------------------------------
// <copyright file="WriterExceptionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for producing and dealing with writer exceptions.
    /// </summary>
    // [TestClass, TestCase]
    public class WriterExceptionTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org");

        // These tests and helpers are disabled on Silverlight and Phone because they  
        // use private reflection not available on Silverlight and Phone. Private reflection is used in ODataWriterCoreInspector IsInState() method.
#if !SILVERLIGHT && !WINDOWS_PHONE

        [TestMethod, Variation(Description = "Test that a writer behaves as expected in the presence of (non-fatal) OData exceptions.")]
        public void DisposeAfterExceptionTest()
        {
            // create a default entry and then set both read and edit links to null to provoke an exception during writing
            ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
            this.Assert.IsNull(entry.EditLink, "entry.EditLink == null");

            this.CombinatorialEngineProvider.RunCombinations(
                new ODataItem[] { entry },
                new bool[] { true, false }, // writeError
                new bool[] { true, false }, // flush
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(c => c.IsRequest == false),
                (payload, writeError, flush, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    // try writing to a memory stream
                    using (var memoryStream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                    {
                        ODataWriter writer = messageWriter.CreateODataWriter(isFeed: false);
                        ODataWriterCoreInspector inspector = new ODataWriterCoreInspector(writer);
                        try
                        {
                            // write the invalid entry and expect an exception
                            TestExceptionUtils.ExpectedException(
                                this.Assert,
                                () => TestWriterUtils.WritePayload(messageWriter, writer, false, entry),
                                ODataExpectedExceptions.ODataException("WriterValidationUtils_EntriesMustHaveNonEmptyId"),
                                this.ExceptionVerifier); 

                            this.Assert.IsTrue(inspector.IsInErrorState, "Writer is not in expected 'error' state.");

                            if (writeError)
                            {
                                // now write an error which is the only valid thing to do
                                ODataAnnotatedError error = new ODataAnnotatedError
                                {
                                    Error = new ODataError()
                                    {
                                        Message = "DisposeAfterExceptionTest error message."
                                    }
                                };
                                Exception ex = TestExceptionUtils.RunCatching(() => TestWriterUtils.WritePayload(messageWriter, writer, false, error));
                                this.Assert.IsNull(ex, "Unexpected error '" + (ex == null ? "<none>" : ex.Message) + "' while writing an error.");
                                this.Assert.IsTrue(inspector.IsInErrorState, "Writer is not in expected 'error' state.");
                            }

                            if (flush)
                            {
                                writer.Flush();
                            }
                        }
                        catch (ODataException oe)
                        {
                            if (writeError && !flush)
                            {
                                this.Assert.AreEqual("A writer or stream has been disposed with data still in the buffer. You must call Flush or FlushAsync before calling Dispose when some data has already been written.", oe.Message, "Did not find expected error message");
                                this.Assert.IsTrue(inspector.IsInErrorState, "Writer is not in expected 'error' state.");
                            }
                            else
                            {
                                this.Assert.Fail("Caught an unexpected ODataException: " + oe.Message + ".");
                            }
                        }
                    }
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test that a writer behaves as expected in the presence of fatal exceptions.")]
        public void FatalExceptionTest()
        {
            ODataResource entry = ObjectModelUtils.CreateDefaultEntry();

            this.CombinatorialEngineProvider.RunCombinations(
                new ODataItem[] { entry },
                new bool[] { true, false }, // flush
                // TODO: also enable this test for the sync scenarios
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => !tc.Synchronous),
                (payload, flush, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var memoryStream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                    {
                        ODataWriter writer = messageWriter.CreateODataWriter(isFeed: false);
                        ODataWriterCoreInspector inspector = new ODataWriterCoreInspector(writer);

                        // close the memory stream so that any attempt to flush will cause a fatal error
                        memoryStream.CloseInner();

                        // write the payload and call FlushAsync() to trigger a fatal exception
                        Exception ex = TestExceptionUtils.RunCatching(() => TestWriterUtils.WritePayload(messageWriter, writer, true, entry));
                        this.Assert.IsNotNull(ex, "Expected exception but none was thrown.");
                        NotSupportedException notSupported = null;
#if SILVERLIGHT || WINDOWS_PHONE
                        var baseEx = ex.GetBaseException();
                        this.Assert.IsNotNull(baseEx, "BaseException of exception:" + ex.ToString() + " should not be null");
                        notSupported = baseEx as NotSupportedException;
                        this.Assert.IsNotNull(notSupported, "Expected NotSupportedException but " + baseEx.GetType().FullName + " was reported.");
#else
                        notSupported = TestExceptionUtils.UnwrapAggregateException(ex, this.Assert) as NotSupportedException;
                        this.Assert.IsNotNull(notSupported, "Expected NotSupportedException but " + ex.ToString() + " was reported.");
#endif

                        this.Assert.AreEqual("Stream does not support writing.", notSupported.Message, "Did not find expected error message.");
                        this.Assert.IsTrue(inspector.IsInErrorState, "Writer is not in expected 'Error' state.");

                        if (flush)
                        {
                            // Flush should work in error state.
                            writer.Flush();
                        }

                        // in all cases we have to be able to dispose the writer without problems.
                    }
                });
        }

        [TestMethod, Variation(Description = "Test that a writer throws when writing non-error content after an exception has been thrown.")]
        public void WriteAfterExceptionTest()
        {
            // create a default entry and then set both read and edit links to null to provoke an exception during writing
            ODataResource faultyEntry = ObjectModelUtils.CreateDefaultEntry();
            this.Assert.IsNull(faultyEntry.EditLink, "entry.EditLink == null");

            ODataResource defaultEntry = ObjectModelUtils.CreateDefaultEntry();
            ODataResourceSet defaultFeed = ObjectModelUtils.CreateDefaultFeed();

            this.CombinatorialEngineProvider.RunCombinations(
                new ODataItem[] { faultyEntry },
                new ODataItem[] { defaultFeed, defaultEntry },
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (faultyPayload, contentPayload, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var memoryStream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                    {
                        ODataWriter writer = messageWriter.CreateODataWriter(isFeed: false);
                        ODataWriterCoreInspector inspector = new ODataWriterCoreInspector(writer);

                        // write the invalid entry and expect an exception
                        this.Assert.ExpectedException(
                            () => TestWriterUtils.WritePayload(messageWriter, writer, false, faultyEntry),
                            ODataExpectedExceptions.ODataException("WriterValidationUtils_EntriesMustHaveNonEmptyId"),
                            this.ExceptionVerifier);
                        this.Assert.IsTrue(inspector.IsInErrorState, "Writer is not in expected 'error' state.");

                        // now write some non-error content which is invalid to do
                        Exception ex = TestExceptionUtils.RunCatching(() => TestWriterUtils.WritePayload(messageWriter, writer, false, contentPayload));
                        ex = TestExceptionUtils.UnwrapAggregateException(ex, this.Assert);
                        this.Assert.IsNotNull(ex, "Expected exception but none was thrown");
                        this.Assert.IsTrue(ex is ODataException, "Expected an ODataException instance but got a " + ex.GetType().FullName + ".");
                        this.Assert.IsTrue(ex.Message.Contains("Cannot transition from state 'Error' to state "), "Did not find expected start of error message.");
                        this.Assert.IsTrue(ex.Message.Contains("Nothing can be written once the writer entered the error state."), "Did not find expected end of error message in '" + ex.Message + "'.");
                        this.Assert.IsTrue(inspector.IsInErrorState, "Writer is not in expected 'error' state.");
                        writer.Flush();
                    }
                });
        }
#endif

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test that a writer can successfully dispose itself when nothing was written out.")]
        public void DisposeAfterWritingNothingTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                new bool[] { true, false },
                (testConfiguration, forFeed) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var stream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(stream, testConfiguration, this.Assert))
                    {
                        ODataWriter writer = messageWriter.CreateODataWriter(forFeed);
                    }
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test that a writer fails when trying to be used after dispose.")]
        public void WriteAfterDisposeTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var stream = new TestStream())
                    {
                        ODataWriter writer;
                        using (var messageWriter = TestWriterUtils.CreateMessageWriter(stream, testConfiguration, this.Assert))
                        {
                            writer = messageWriter.CreateODataWriter(isFeed: true);
                            // Message writer is disposed outside of this scope.
                        }

                        this.VerifyWriterAlreadyDisposed(() => writer.WriteStart(ObjectModelUtils.CreateDefaultFeed()));
                        this.VerifyWriterAlreadyDisposed(() => writer.WriteStart(ObjectModelUtils.CreateDefaultEntry()));
                        this.VerifyWriterAlreadyDisposed(() => writer.WriteStart(ObjectModelUtils.CreateDefaultCollectionLink()));
                    }
                });
        }

        /// <summary>
        /// Verifies that the action fails due to the fact that the writer was already disposed.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void VerifyWriterAlreadyDisposed(Action action)
        {
            TestExceptionUtils.ExpectedException(this.Assert, action, new ExpectedException(typeof(ObjectDisposedException)), this.ExceptionVerifier);
        }

        /// <summary>
        /// Wrapper class which provides access to the internal state of an ODataWriter
        /// implementation based on the ODataWriterCore base class.
        /// </summary>
        private sealed class ODataWriterCoreInspector
        {
            private readonly object writer;

            public ODataWriterCoreInspector(ODataWriter writer)
            {
                ODataWriterTestWrapper writerTestWrapper = writer as ODataWriterTestWrapper;
                this.writer = writerTestWrapper == null ? writer : writerTestWrapper.Writer;
            }

            public bool IsInErrorState
            {
                get
                {
                    return IsInState("Error");
                }
            }

            private bool IsInState(string stateName)
            {
                object state = ReflectionUtils.GetProperty(this.writer, "State");
                string enumName = null;
                enumName = Enum.GetName(state.GetType(), state);
                return enumName == stateName;
            }
        }
    }
}
