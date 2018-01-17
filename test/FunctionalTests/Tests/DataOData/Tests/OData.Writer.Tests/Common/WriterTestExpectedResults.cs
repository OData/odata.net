//---------------------------------------------------------------------
// <copyright file="WriterTestExpectedResults.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;
    #endregion Namespaces

    /// <summary>
    /// Class which represents expected results for a writer test
    /// </summary>
    public class WriterTestExpectedResults
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class Settings
        {
            [InjectDependency(IsRequired = true)]
            public AssertionHandler Assert { get; set; }

            [InjectDependency(IsRequired = true)]
            public IExceptionVerifier ExceptionVerifier { get; set; }
        }

        /// <summary>The settings for the expected result.</summary>
        private readonly Settings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">The settings to use.</param>
        public WriterTestExpectedResults(Settings settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(settings, "settings");
            this.settings = settings;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="WriterTestExpectedResults"/> instance to copy.</param>
        public WriterTestExpectedResults(WriterTestExpectedResults other)
        {
            ExceptionUtilities.CheckArgumentNotNull(other, "other");

            this.settings = other.settings;
            this.ExpectedContentType = other.ExpectedContentType;
            this.ExpectedException = other.ExpectedException;
            this.ExpectedException2 = other.ExpectedException2;
            this.ExpectedODataErrorException = other.ExpectedODataErrorException;
            this.ExpectedODataExceptionMessage = other.ExpectedODataExceptionMessage;
        }

        /// <summary>
        /// If set to a value, the test is expected to fail with an exception of the same type and with the same message string.
        /// </summary>
        // TODO: Remove this property when all tests are converted to resource string based error verification
        public Exception ExpectedException { get; set; }

        /// <summary>
        /// If set to a value, the test is expected to fail with an ODataException and with the specified message string.
        /// </summary>
        // TODO: Remove this property when all tests are converted to resource string based error verification
        public string ExpectedODataExceptionMessage { get; set; }

        /// <summary>
        /// If set to a value, the test is expected to fail with the specified exception.
        /// </summary>
        // TODO: Rename this property when all tests are converted to resource string based error verification
        public ExpectedException ExpectedException2 { get; set; }

        /// <summary>
        /// If set the test is expected to fail with an ODataErrorException equal to the one specified by this property.
        /// If left null, the test is not expected to throw an error exception.
        /// </summary>
        /// <remarks>If this property is set the ExpectedODataExceptionMessage property is ignored.</remarks>
        public ODataErrorException ExpectedODataErrorException { get; set; }

        /// <summary>
        /// The expected content type on the response message.
        /// </summary>
        public string ExpectedContentType { get; set; }

        /// <summary>
        /// Verifies that the result of the test (the message reader) is what the test expected.
        /// </summary>
        /// <param name="stream">Stream which contains the results of the write.</param>
        /// <param name="payloadKind">The payload kind specified in the test descriptor.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public virtual void VerifyResult(
            TestMessage message,
            ODataPayloadKind payloadKind,
            WriterTestConfiguration testConfiguration,
            BaselineLogger logger)
        {
            // throw if not implemented; eventually we can make this method abstract when the WriterTestExpectedResults
            // are not used directly anymore
            throw new NotImplementedException("Subclasses must implement their own validation logic.");
        }

        /// <summary>
        /// Verifies that the test correctly threw an exception.
        /// </summary>
        /// <param name="exception">null if the test didn't throw, the exception thrown by the test otherwise.</param>
        public virtual void VerifyException(Exception exception)
        {
            exception = TestExceptionUtils.UnwrapAggregateException(exception, this.settings.Assert);

            if (this.ExpectedException2 != null)
            {
                this.settings.ExceptionVerifier.VerifyExceptionResult(this.ExpectedException2, exception);
            }
            else if (this.ExpectedException != null)
            {
                this.settings.Assert.IsExpectedException<Exception>(exception, this.ExpectedException.Message);
            }
            else if (this.ExpectedODataErrorException != null)
            {
                this.settings.Assert.IsExpectedException<ODataErrorException>(exception, this.ExpectedODataErrorException.Message);
                this.settings.Assert.IsTrue(ODataObjectModelValidationUtils.AreEqual(this.ExpectedODataErrorException.Error, ((ODataErrorException)exception).Error), "Expected ODataError instances to be equal.");
            }
            else
            {
                this.settings.Assert.IsExpectedException<ODataException>(exception, this.ExpectedODataExceptionMessage);
            }
        }
    }

    /// <summary>
    /// Class which represents expected results for ATOM writer test
    /// </summary>
    internal class AtomWriterTestExpectedResults : WriterTestExpectedResults
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">The settings to use.</param>
        public AtomWriterTestExpectedResults(Settings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="AtomWriterTestExpectedResults"/> instance to copy.</param>
        public AtomWriterTestExpectedResults(AtomWriterTestExpectedResults other)
            : base(other)
        {
            this.Xml = other.Xml;
            this.FragmentExtractor = other.FragmentExtractor;
            this.PreserveWhitespace = other.PreserveWhitespace;
        }

        /// <summary>
        /// The expected XML fragment.
        /// </summary>
        public string Xml { get; set; }

        /// <summary>
        /// Function which is executed on the actual result to extract the fragment to verify.
        /// </summary>
        public Func<XElement, XElement> FragmentExtractor { get; set; }

        /// <summary>
        /// Flag which determines whether whitespace in the expected XML fragment should be preserved.
        /// By default this is false, meaning that whitespace in the <see cref="Xml"/> string will be stripped away before the final comparison is made.
        /// </summary>
        public bool PreserveWhitespace { get; set; }

        /// <summary>
        /// true to disable normalization/removal of namespace nodes before comparison; otherwise false (default).
        /// </summary>
        public bool DisableNamespaceNormalization { get; set; }
    }

    /// <summary>
    /// Class which represents expected results for JSON writer test
    /// </summary>
    internal class JsonWriterTestExpectedResults : WriterTestExpectedResults
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">The settings to use.</param>
        public JsonWriterTestExpectedResults(Settings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="JsonWriterTestExpectedResults"/> instance to copy.</param>
        public JsonWriterTestExpectedResults(JsonWriterTestExpectedResults other)
            : base(other)
        {
            this.Json = other.Json;
            this.FragmentExtractor = other.FragmentExtractor;
        }

        /// <summary>
        /// The expected JSON fragment.
        /// </summary>
        public string Json { get; set; }

        /// <summary>
        /// Function which is executed on the actual result to extract the fragment to verify.
        /// </summary>
        public Func<JsonValue, JsonValue> FragmentExtractor { get; set; }
    }

    /// <summary>
    /// Class which represents expected results for ATOM writer test
    /// </summary>
    internal class RawValueWriterTestExpectedResults : WriterTestExpectedResults
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">The settings to use.</param>
        public RawValueWriterTestExpectedResults(Settings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// The expected text representation of the value(s).
        /// </summary>
        public string RawValueAsText { get; set; }

        /// <summary>
        /// The expected payload representation as bytes (for binary values).
        /// </summary>
        public byte[] RawBytes { get; set; }

        internal static string RawBytesToText(byte[] bytes)
        {
            if (bytes == null)
            {
                return "null";
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("{ ");

            for (int i = 0; i < bytes.Length; ++i)
            {
                builder.Append(bytes[i].ToString());
                if (i < bytes.Length - 1)
                {
                    builder.Append(", ");
                }

                builder.Append(" ");
            }

            builder.Append("}");
            return builder.ToString();
        }

        /// <summary>
        /// Returns a text representation of the expected value.
        /// </summary>
        /// <returns>Returns a text representation of the expected value.</returns>
        internal string ToText()
        {
            if (this.RawValueAsText != null)
                return this.RawValueAsText;

            return RawBytesToText(this.RawBytes);
        }
    }
}
