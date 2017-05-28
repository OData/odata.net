//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamOrBufferTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    #endregion Namespaces

    /// <summary>
    /// Base class for all unit tests for the ODataMultipartMixedBatchReaderStream and ODataBatchReaderStreamBuffer.
    /// </summary>
    public abstract class BatchReaderStreamOrBufferTestCase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="encoding">The encoding to use to convert bytes to and from strings.</param>
        /// <param name="lineFeedChars">The line feed characters to use in the test.</param>
        public BatchReaderStreamOrBufferTestCase(Encoding encoding = null, char[] lineFeedChars = null)
        {
#if SILVERLIGHT || WINDOWS_PHONE
            this.Encoding = encoding ?? Encoding.UTF8;
#else
            this.Encoding = encoding ?? Encoding.ASCII;
#endif
            this.LineFeedChars = lineFeedChars ?? BatchReaderStreamTestUtils.DefaultLineFeedChars;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The test case instance to copy.</param>
        public BatchReaderStreamOrBufferTestCase(BatchReaderStreamOrBufferTestCase other)
        {
            this.Encoding = other.Encoding;
            this.LineFeedChars = other.LineFeedChars;
            this.DebugDescription = other.DebugDescription;
            this.Injector = other.Injector;
            this.Assert = other.Assert;
            this.ExceptionVerifier = other.ExceptionVerifier;
            this.PayloadFunc = other.PayloadFunc;
            this.ExpectedBufferState = other.ExpectedBufferState;
            this.ExpectedException = other.ExpectedException;
        }


        /// <summary>
        /// Gets or sets the dependency injector.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDependencyInjector Injector { get; set; }

        /// <summary>
        /// Gets or sets assertion class to be used.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        [InjectDependency]
        public IExceptionVerifier ExceptionVerifier { get; set; }

        /// <summary>Debug description of the test.</summary>
        public string DebugDescription { get; set; }

        /// <summary>The encoding to use to convert bytes to and from strings.</summary>
        public Encoding Encoding { get; set; }

        /// <summary>The line feed characters to use in the test.</summary>
        public char[] LineFeedChars { get; set; }

        public virtual Func<MemoryStreamBatchPayloadBuilder, MemoryStream> PayloadFunc { get; set; }

        public BatchReaderStreamBufferState ExpectedBufferState { get; set; }

        /// <summary>The expected exception (if any).</summary>
        public ExpectedException ExpectedException { get; set; }

        /// <summary>
        /// Runs the test case.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Verifies that the resulting stream buffer is in the expected state.
        /// </summary>
        /// <param name="assert">The assertion handler.</param>
        /// <param name="streamBuffer">The stream buffer whose state to verify.</param>
        public virtual void VerifyResult(BatchReaderStreamBufferWrapper streamBuffer)
        {
            if (this.ExpectedBufferState != null)
            {
                this.ExpectedBufferState.VerifyResult(this.Assert, streamBuffer, this.DebugDescription);
            }
        }

        /// <summary>
        /// Verifies that the test correctly threw an exception.
        /// </summary>
        /// <param name="exception">null if the test didn't throw, the exception thrown by the test otherwise.</param>
        public void VerifyException(Exception exception)
        {
            exception = TestExceptionUtils.UnwrapAggregateException(exception, this.Assert);
            if (this.ExpectedException != null)
            {
                this.Assert.IsNotNull(exception,
                    "Expected exception of type '{0}' with message resource ID '{1}' but none was thrown.",
                    this.ExpectedException.ExpectedExceptionType.ToString(),
                    this.ExpectedException.ExpectedMessage == null ? "<null>" : this.ExpectedException.ExpectedMessage.ResourceIdentifier);
                this.ExceptionVerifier.VerifyExceptionResult(this.ExpectedException, exception);
            }
            else
            {
                this.Assert.IsNull(exception, "Unexpected exception was thrown: {0}", (exception == null) ? string.Empty : exception.ToString());
            }
        }
    }
}
