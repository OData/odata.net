//---------------------------------------------------------------------
// <copyright file="WriterTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;
    #endregion Namespaces

    /// <summary>
    /// Base class for the writer test descriptors.
    /// </summary>
    public abstract class WriterTestDescriptor
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class Settings
        {
            [InjectDependency(IsRequired = true)]
            public AssertionHandler Assert { get; set; }

            [InjectDependency(IsRequired = true)]
            public WriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

            [InjectDependency(IsRequired = true)]
            public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }
        }

        private readonly Settings settings;
        protected Stream messageStream;

        public Func<string, string> LoggingCallback { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected WriterTestDescriptor(Settings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="WriterTestDescriptor"/> to clone.</param>
        protected WriterTestDescriptor(WriterTestDescriptor other)
        {
            ExceptionUtilities.CheckArgumentNotNull(other, "other");

            this.settings = other.settings;
            this.Model = other.Model;
            this.ExpectedResultCallback = other.ExpectedResultCallback;
            this.DebugDescription = other.DebugDescription;
            this.SkipTestConfiguration = other.SkipTestConfiguration;
            this.ContentType = other.ContentType;
            this.UrlResolver = other.UrlResolver;
        }

        /// <summary>
        /// Delegate which is called to determine results of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration used.</param>
        /// <returns>The expected test results. If this is null the test case should be skipped.</returns>
        public delegate WriterTestExpectedResults WriterTestExpectedResultCallback(WriterTestConfiguration testConfiguration);

        /// <summary>
        /// The metadata provider.
        /// </summary>
        public IEdmModel Model
        {
            get;
            set;
        }

        /// <summary>
        /// The payload kind which is being tested.
        /// </summary>
        public virtual ODataPayloadKind PayloadKind
        {
            get;
            set;
        }

        /// <summary>
        /// The callback to use to determine the expected results.
        /// </summary>
        public WriterTestExpectedResultCallback ExpectedResultCallback
        {
            get;
            set;
        }

        /// <summary>
        /// A description of the payload, used for debugging.
        /// </summary>
        public string DebugDescription { get; set; }

        /// <summary>
        /// If the func is specified it is executed before the test is ran for a given test configuration.
        /// If the func returns true, the test is not ran and "success" is reported right away.
        /// </summary>
        public Func<TestConfiguration, bool> SkipTestConfiguration { get; set; }

        /// <summary>
        /// The content type header to set, if not-null this overrides the content type derived from test configuration.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// If set, this resolver will be used in the message used durign writing. And that message will implement the IODataPayloadUriConverter interface.
        /// </summary>
        public IODataPayloadUriConverter UrlResolver { get; set; }

        /// <summary>
        /// Runs the test specified by this test descriptor.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use for running the test.</param>
        public virtual void RunTest(WriterTestConfiguration testConfiguration, BaselineLogger logger)
        {
            if (this.ShouldSkipForTestConfiguration(testConfiguration))
            {
                return;
            }

            // Wrap the memory stream in a non-disposing stream so we can dump the message content
            // even in the case of a failure where the message stream normally would get disposed.
            logger.LogConfiguration(testConfiguration);
            logger.LogModelPresence(this.Model);
            this.messageStream = new NonDisposingStream(new MemoryStream());
            TestMessage message = this.CreateOutputMessage(this.messageStream, testConfiguration);
            IEdmModel model = this.GetMetadataProvider();
            WriterTestExpectedResults expectedResult = this.GetExpectedResult(testConfiguration);
            ExceptionUtilities.Assert(expectedResult != null, "The expected result could not be determined for the test. Did you specify it?");

            Exception exception = TestExceptionUtils.RunCatching(() =>
            {
                using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(message, model, testConfiguration, this.settings.Assert))
                {
                    this.WritePayload(messageWriterWrapper, testConfiguration);
                    expectedResult.VerifyResult(message, this.PayloadKind, testConfiguration, logger);
                }
            });

            try
            {
                expectedResult.VerifyException(exception);
            }
            catch (Exception failureException)
            {
                this.TraceFailureInformation(message, this.messageStream, testConfiguration);
                throw failureException;
            }
        }

        /// <summary>
        /// The humanly readable representation.
        /// </summary>
        /// <returns>The debug description of the test.</returns>
        public override string ToString()
        {
            string result = string.Format("Payload Kind: {0}", this.PayloadKind);
            if (this.DebugDescription != null)
            {
                result = this.DebugDescription + Environment.NewLine + result;
            }

            return result;
        }

        /// <summary>
        /// Gets The model to use for the specified test configuration.
        /// </summary>
        /// <returns>The model to use for the test.</returns>
        public virtual IEdmModel GetMetadataProvider()
        {
            return this.Model;
        }

        /// <summary>
        /// Called to write the payload to the specified <paramref name="messageWriter"/>.
        /// </summary>
        /// <param name="messageWriter">The <see cref="ODataMessageWriterTestWrapper"/> to use for writing the payload.</param>
        protected abstract void WritePayload(ODataMessageWriterTestWrapper messageWriter, WriterTestConfiguration config);

        /// <summary>
        /// Called to create the output message for the writer test.
        /// </summary>
        /// <param name="innerStream">The <see cref="Stream"/> instance to be used as inner stream of the <see cref="TestStream"/>.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The newly created test message to use.</returns>
        protected virtual TestMessage CreateOutputMessage(Stream innerStream, WriterTestConfiguration testConfiguration)
        {
            TestStream messageStream = new TestStream(innerStream);
            if (testConfiguration.Synchronous)
            {
                messageStream.FailAsynchronousCalls = true;
            }
            else
            {
                messageStream.FailSynchronousCalls = true;
            }

            TestMessage testMessage = TestWriterUtils.CreateOutputMessageFromStream(messageStream, testConfiguration, this.PayloadKind, this.ContentType, this.UrlResolver);
            return testMessage;
        }

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected virtual WriterTestExpectedResults GetExpectedResult(WriterTestConfiguration testConfiguration)
        {
            if (this.ExpectedResultCallback != null)
            {
                return this.ExpectedResultCallback(testConfiguration);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Called before the test is actually executed for the specified test configuration to determine if the test should be skipped.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>true if the test should be skipped for the <paramref name="testConfiguration"/> or false to run the test.</returns>
        /// <remarks>Derived classes should always call the base class and return true if the base class returned true.</remarks>
        protected virtual bool ShouldSkipForTestConfiguration(WriterTestConfiguration testConfiguration)
        {
            if (this.SkipTestConfiguration != null)
            {
                return this.SkipTestConfiguration(testConfiguration);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// If overriden dumps the content of an output message which would be created for the specified test configuration
        /// into a string and returns it. This is used only for debugging purposes.
        /// </summary>
        /// <param name="messageStream">The stream for the message content.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The string content of the output message.</returns>
        protected virtual string DumpOutputMessageContent(Stream messageStream, WriterTestConfiguration testConfiguration)
        {
            Debug.Assert(messageStream != null, "Require a message stream.");

            if (!messageStream.CanSeek)
            {
                return "<Cannot dump message content since underlying stream does not support seek.>";
            }

            messageStream.Seek(0, SeekOrigin.Begin);

            byte[] payload = new byte[messageStream.Length];
            messageStream.Read(payload, 0, (int)messageStream.Length);
            return Encoding.UTF8.GetString(payload, 0, payload.Length);
        }

        /// <summary>
        /// If overridden dumps additional description of the test descriptor for the specified testConfiguration.
        /// This is used only for debugging purposes.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>String description of the test.</returns>
        protected virtual string DumpAdditionalTestDescriptions(WriterTestConfiguration testConfiguration)
        {
            return string.Empty;
        }

        /// <summary>
        /// Traces interesting information if on test failure.
        /// </summary>
        /// <param name="message">The message to dump the failure information for.</param>
        /// <param name="messageStream">The message stream to get the message content from.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        protected void TraceFailureInformation(TestMessage message, Stream messageStream, WriterTestConfiguration testConfiguration)
        {
            try
            {
                Trace.WriteLine("----- HTTP Message start ------------------------------------------");

                TestRequestMessage requestMessage = message as TestRequestMessage;
                if (requestMessage != null)
                {
                    Trace.WriteLine(requestMessage.Method.ToString() + " " + requestMessage.Url + " HTTP/1.1");
                }

                TestResponseMessage responseMessage = message as TestResponseMessage;
                if (responseMessage != null)
                {
                    Trace.WriteLine(responseMessage.StatusCode.ToString());
                }

                foreach (var header in message.Headers)
                {
                    Trace.WriteLine(header.Key + ": " + header.Value);
                }

                Trace.WriteLine("");

                Trace.WriteLine(this.DumpOutputMessageContent(messageStream, testConfiguration));
                Trace.WriteLine("----- HTTP Message end --------------------------------------------");

                string additionalDescription = this.DumpAdditionalTestDescriptions(testConfiguration);
                if (!string.IsNullOrEmpty(additionalDescription))
                {
                    Trace.WriteLine("");
                    Trace.WriteLine("----- Additional test description ---------------------------------");
                    Trace.WriteLine(additionalDescription);
                }
            }
            catch (Exception innerException)
            {
                // Ignore all exceptions here since we want to fail with the original test exception.
                Trace.WriteLine("Failed to dump the test message.");
                Trace.WriteLine(innerException);
            }
        }

        /// <summary>
        /// Private stream wrapper for the message stream to ignore the Stream.Dispose method so that readers on top of
        /// it can be disposed without affecting it.
        /// </summary>
        protected sealed class NonDisposingStream : Stream
        {
            /// <summary>
            /// Stream that is being wrapped.
            /// </summary>
            private readonly Stream innerStream;

            /// <summary>
            /// Constructs an instance of the stream wrapper class.
            /// </summary>
            /// <param name="innerStream">Stream that is being wrapped.</param>
            public NonDisposingStream(Stream innerStream)
            {
                Debug.Assert(innerStream != null, "innerStream != null");
                this.innerStream = innerStream;
            }

            /// <summary>
            /// Determines if the stream can read.
            /// </summary>
            public override bool CanRead
            {
                get { return this.innerStream.CanRead; }
            }

            /// <summary>
            /// Determines if the stream can seek.
            /// </summary>
            public override bool CanSeek
            {
                get { return this.innerStream.CanSeek; }
            }

            /// <summary>
            /// Determines if the stream can write.
            /// </summary>
            public override bool CanWrite
            {
                get { return this.innerStream.CanWrite; }
            }

            /// <summary>
            /// Returns the length of the stream.
            /// </summary>
            public override long Length
            {
                get { return this.innerStream.Length; }
            }

            /// <summary>
            /// Gets or sets the position in the stream.
            /// </summary>
            public override long Position
            {
                get { return this.innerStream.Position; }
                set { this.innerStream.Position = value; }
            }

            /// <summary>
            /// Flush the stream to the underlying storage.
            /// </summary>
            public override void Flush()
            {
                this.innerStream.Flush();
            }

            /// <summary>
            /// Reads data from the stream.
            /// </summary>
            /// <param name="buffer">The buffer to read the data to.</param>
            /// <param name="offset">The offset in the buffer to write to.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <returns>The number of bytes actually read.</returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.innerStream.Read(buffer, offset, count);
            }

            /// <summary>
            /// Begins a read operation from the stream.
            /// </summary>
            /// <param name="buffer">The buffer to read the data to.</param>
            /// <param name="offset">The offset in the buffer to write to.</param>
            /// <param name="count">The number of bytes to read.</param>
            /// <param name="callback">The async callback.</param>
            /// <param name="state">The async state.</param>
            /// <returns>Async result representing the asynchornous operation.</returns>
            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return this.innerStream.BeginRead(buffer, offset, count, callback, state);
            }

            /// <summary>
            /// Ends a read operation from the stream.
            /// </summary>
            /// <param name="asyncResult">The async result representing the read operation.</param>
            /// <returns>The number of bytes actually read.</returns>
            public override int EndRead(IAsyncResult asyncResult)
            {
                return this.innerStream.EndRead(asyncResult);
            }

            /// <summary>
            /// Seeks the stream.
            /// </summary>
            /// <param name="offset">The offset to seek to.</param>
            /// <param name="origin">The origin of the seek operation.</param>
            /// <returns>The new position in the stream.</returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.innerStream.Seek(offset, origin);
            }

            /// <summary>
            /// Sets the length of the stream.
            /// </summary>
            /// <param name="value">The length in bytes to set.</param>
            public override void SetLength(long value)
            {
                this.innerStream.SetLength(value);
            }

            /// <summary>
            /// Writes to the stream.
            /// </summary>
            /// <param name="buffer">The buffer to get data from.</param>
            /// <param name="offset">The offset in the buffer to start from.</param>
            /// <param name="count">The number of bytes to write.</param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                this.innerStream.Write(buffer, offset, count);
            }

            /// <summary>
            /// Begins an asynchronous write operation to the stream.
            /// </summary>
            /// <param name="buffer">The buffer to get data from.</param>
            /// <param name="offset">The offset in the buffer to start from.</param>
            /// <param name="count">The number of bytes to write.</param>
            /// <param name="callback">The async callback.</param>
            /// <param name="state">The async state.</param>
            /// <returns>Async result representing the write operation.</returns>
            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                return this.innerStream.BeginWrite(buffer, offset, count, callback, state);
            }

            /// <summary>
            /// Ends the asynchronous write operation.
            /// </summary>
            /// <param name="asyncResult">Async result representing the write operation.</param>
            public override void EndWrite(IAsyncResult asyncResult)
            {
                this.innerStream.EndWrite(asyncResult);
            }

            /// <summary>
            /// Release unmanaged resources.
            /// </summary>
            /// <param name="disposing">True if called from Dispose; false if called from the finalizer.</param>
            /// <remarks>This method doesn't dispose the inner stream.</remarks>
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
            }
        }
    }
}
