//---------------------------------------------------------------------
// <copyright file="ReaderTestExpectedResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Base test expected result class for reader tests.
    /// </summary>
    public abstract class ReaderTestExpectedResult
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class ReaderTestExpectedResultSettings
        {
            [InjectDependency(IsRequired = true)]
            public AssertionHandler Assert { get; set; }

            [InjectDependency(IsRequired = true)]
            public IExceptionVerifier ExceptionVerifier { get; set; }
        }

        /// <summary>
        /// If set the test is expected to fail with the specified exception.
        /// If left null, the test is not expected to throw an exception.
        /// </summary>
        public ExpectedException ExpectedException { get; set; }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        private readonly ReaderTestExpectedResultSettings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">The settings use.</param>
        public ReaderTestExpectedResult(ReaderTestExpectedResultSettings settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(settings, "settings");

            this.settings = settings;
        }

        /// <summary>
        /// Verifies that the result of the test (the message reader) is what the test expected.
        /// </summary>
        /// <param name="messageReader">The message reader which is the result of the test. This method should use it to read the results
        /// of the parsing and verify those.</param>
        /// <param name="payloadKind">The payload kind specified in the test descriptor.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public abstract void VerifyResult(
            ODataMessageReaderTestWrapper messageReader,
            ODataPayloadKind payloadKind,
            ReaderTestConfiguration testConfiguration);

        /// <summary>
        /// Verifies that the test correctly threw an exception.
        /// </summary>
        /// <param name="exception">null if the test didn't throw, the exception thrown by the test otherwise.</param>
        public virtual void VerifyException(Exception exception)
        {
            exception = TestExceptionUtils.UnwrapAggregateException(exception, this.settings.Assert);
            if (this.ExpectedException != null)
            {
                this.settings.Assert.IsNotNull(exception,
                    "Expected exception of type '{0}' with message resource ID '{1}' but none was thrown.",
                    this.ExpectedException.ExpectedExceptionType.ToString(),
                    this.ExpectedException.ExpectedMessage == null ? "<null>" : this.ExpectedException.ExpectedMessage.ResourceIdentifier);
                this.settings.ExceptionVerifier.VerifyExceptionResult(this.ExpectedException, exception);
            }
            else
            {
                this.settings.Assert.IsNull(exception, "Unexpected exception was thrown: {0}", (exception == null) ? string.Empty : exception.ToString());
            }
        }
    }
}
