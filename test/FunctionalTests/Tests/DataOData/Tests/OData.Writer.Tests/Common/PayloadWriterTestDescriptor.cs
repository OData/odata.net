//---------------------------------------------------------------------
// <copyright file="PayloadWriterTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Fixups;
    #endregion Namespaces

    /// <summary>
    /// Base class for the generic variant of the writer test descriptor.
    /// </summary>
    public abstract class PayloadWriterTestDescriptor : WriterTestDescriptor
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public new class Settings : WriterTestDescriptor.Settings
        {
            [InjectDependency(IsRequired = true)]
            public ObjectModelToMessageWriter ObjectModelToMessageWriter { get; set; }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        protected Settings settings;

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        protected PayloadWriterTestDescriptor(Settings settings)
            : base(settings)
        {
            this.settings = settings;
            this.PayloadDescriptor = new PayloadTestDescriptor();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="PayloadWriterTestDescriptor"/> to clone.</param>
        protected PayloadWriterTestDescriptor(PayloadWriterTestDescriptor other)
            : base(other)
        {
            ExceptionUtilities.CheckArgumentNotNull(other, "other");

            this.settings = other.settings;
            this.PayloadDescriptor = new PayloadTestDescriptor(other.PayloadDescriptor);
            this.Model = other.Model;
            this.PayloadEdmElementContainer = other.PayloadEdmElementContainer;
        }

        /// <summary>
        /// The description of the payload and model to run the test against
        /// </summary>
        public PayloadTestDescriptor PayloadDescriptor { get; set; }

        /// <summary>
        /// The container in the model for the test payload. 
        /// For entity sets this is their entity container, for entries/feeds it is the entity set, for properties it is their owning type.
        /// Added for using EdmLib Elements
        /// </summary>
        public IEdmElement PayloadEdmElementContainer
        {
            get
            {
                return this.PayloadDescriptor.PayloadEdmElementContainer;
            }
            set
            {
                this.PayloadDescriptor.PayloadEdmElementContainer = value;
            }
        }

        /// <summary>
        /// The data type of the value being written.
        /// Added for using EdmLib Elements
        /// </summary>
        public IEdmElement PayloadEdmElementType
        {
            get
            {
                return this.PayloadDescriptor.PayloadEdmElementType;
            }

            set
            {
                this.PayloadDescriptor.PayloadEdmElementType = value;
            }
        }

        /// <summary>
        /// The payload element to write as the input for the writer.
        /// And if the expected result is not specified differently, this will also be the expected result of the writer.
        /// </summary>
        public ODataPayloadElement PayloadElement
        {
            get
            {
                return this.PayloadDescriptor.PayloadElement;
            }
            set
            {
                this.PayloadDescriptor.PayloadElement = value;
            }
        }

        /// <summary>
        /// The payload kind which is being tested.
        /// </summary>
        public override ODataPayloadKind PayloadKind
        {
            get
            {
                return this.PayloadDescriptor.PayloadKind;
            }

            set
            {
                this.PayloadDescriptor.PayloadKind = value;
            }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        public Settings TestDescriptorSettings
        {
            get { return this.settings; }
        }

        /// <summary>
        /// true if this test descriptor was produced via a payload generator; otherwise false.
        /// </summary>
        public bool IsGeneratedPayload { get; set; }

        /// <summary>
        /// Returns description of the test case.
        /// </summary>
        /// <returns>Humanly readable description of the test. Used for debugging.</returns>
        public override string ToString()
        {
            string result = base.ToString();
            if (this.PayloadElement != null)
            {
                result += "\r\nPayload element: " + this.PayloadElement.StringRepresentation;
            }

            if (this.Model != null)
            {
                // There's no short way of serializing the payload model. Instead if the test fails the model
                // will be serialized as CSDL and dumped to the test output.
                result += "\r\nPayload model present";
            }

            return result;
        }

        /// <summary>
        /// Called to create the input message for the reader test.
        /// </summary>
        /// <param name="innerStream">The <see cref="Stream"/> instance to be used as inner stream of the <see cref="TestStream"/>.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The newly created test message to use.</returns>
        protected override TestMessage CreateOutputMessage(Stream innerStream, WriterTestConfiguration testConfiguration)
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

            TestMessage testMessage = TestWriterUtils.CreateOutputMessageFromStream(
                messageStream,
                testConfiguration,
                this.PayloadKind,
                this.PayloadElement.GetCustomContentTypeHeader(),
                this.UrlResolver);

            return testMessage;
        }

        /// <summary>
        /// Called to write the payload to the specified <paramref name="messageWriter"/>.
        /// </summary>
        /// <param name="messageWriter">The <see cref="ODataMessageWriterTestWrapper"/> to use for writing the payload.</param>
        /// <param name="testConfiguration">The test configuration to generate the payload for.</param>
        protected override void WritePayload(ODataMessageWriterTestWrapper messageWriter, WriterTestConfiguration testConfiguration)
        {
            ODataPayloadElement payload = this.PayloadElement.DeepCopy();

            if (testConfiguration.Format == ODataFormat.Json)
            {
                payload.Accept(new ODataPayloadJsonNormalizer());
                //Fixup added as odatalib requires ids on feeds even though it can't be represented in json
                payload.Accept(new AddFeedIDFixup());
            }
            
            ODataPayloadElementToObjectModelConverter converter = new ODataPayloadElementToObjectModelConverter(!testConfiguration.IsRequest);
            if (this.PayloadKind != ODataPayloadKind.Batch)
            {
                this.settings.ObjectModelToMessageWriter.WriteMessage(messageWriter, this.PayloadKind, converter.Convert(payload));
            }
            else
            {
                TestWriterUtils.WriteBatchPayload(messageWriter,
                    payload,
                    converter,
                    this.settings.ObjectModelToMessageWriter,
                    this.Model,
                    this.settings.Assert,
                    testConfiguration,
                    true);
            }
        }

        /// <summary>
        /// Called to get the expected result of the test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The expected result.</returns>
        protected override WriterTestExpectedResults GetExpectedResult(WriterTestConfiguration testConfiguration)
        {
            WriterTestExpectedResults expectedResult = base.GetExpectedResult(testConfiguration);
            if (expectedResult == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return expectedResult;
            }
        }

        /// <summary>
        /// Called before the test is actually executed for the specified test configuration to determine if the test should be skipped.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>true if the test should be skipped for the <paramref name="testConfiguration"/> or false to run the test.</returns>
        /// <remarks>Derived classes should always call the base class and return true if the base class returned true.</remarks>
        protected override bool ShouldSkipForTestConfiguration(WriterTestConfiguration testConfiguration)
        {
            if (this.PayloadDescriptor.SkipTestConfiguration != null && this.PayloadDescriptor.SkipTestConfiguration(testConfiguration))
            {
                return true;
            }
            else if (this.SkipTestConfiguration != null)
            {
                return this.SkipTestConfiguration(testConfiguration);
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Test payload and result descriptor consisting of a payload, an expected ATOM result,
    /// an expected JSON result and optional extractors to identify the part of the result that
    /// needs to be compared against the expected result data.
    /// </summary>
    public class PayloadWriterTestDescriptor<T> : PayloadWriterTestDescriptor
    {
        /// <summary>
        /// Create a new descriptor instance given a payload item, expected ATOM and JSON results
        /// and optional extractors
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">The OData payload item to write.</param>
        /// <param name="expectedResultCallback">The callback to use to determine the expected results.</param>
        public PayloadWriterTestDescriptor(
            Settings settings,
            T payloadItem,
            WriterTestExpectedResultCallback expectedResultCallback)
            : this(settings, new T[] { payloadItem }, expectedResultCallback)
        {
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">payload item for the test</param>
        public PayloadWriterTestDescriptor(Settings settings, T payloadItem)
            : this(settings, new T[] { payloadItem })
        {
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">payloadItems</param>
        public PayloadWriterTestDescriptor(Settings settings, IEnumerable<T> payloadItems)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadItems, "payloadItems");

            this.PayloadItems = new ReadOnlyCollection<T>(payloadItems.ToList());
            this.ThrowUserExceptionAt = -1;
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item for writing raw values
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">payload item for the test</param>
        /// <param name="rawValueAsString">The expected string value of the payload item.</param>
        /// <param name="rawBytes">The expected raw bytes for binary payload items.</param>
        /// <param name="expectedContentType">The expected content type of the raw value.</param>
        public PayloadWriterTestDescriptor(Settings settings, T payloadItem, string rawValueAsString = null, byte[] rawBytes = null, string expectedContentType = null)
            : this(settings, new T[] { payloadItem }, rawValueAsString, rawBytes, expectedContentType)
        {
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item for writing raw values
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">payloadItems</param>
        /// <param name="rawValueAsString">The expected string value of the payload items.</param>
        /// <param name="rawBytes">The expected raw bytes for binary payload items.</param>
        /// <param name="expectedContentType">The expected content type of the raw value.</param>
        public PayloadWriterTestDescriptor(Settings settings, IEnumerable<T> payloadItems, string rawValueAsString = null, byte[] rawBytes = null, string expectedContentType = null)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadItems, "payloadItems");

            this.PayloadItems = new ReadOnlyCollection<T>(payloadItems.ToList());
            this.ThrowUserExceptionAt = -1;
            this.ExpectedResultCallback = (testConfig) =>
            {
                if (testConfig.Format != null)
                {
                    throw new NotSupportedException("Expected ODataFormat.Default but detected " + testConfig.Format.ToString());
                }

                return new RawValueWriterTestExpectedResults(this.settings.ExpectedResultSettings)
                {
                    RawValueAsText = rawValueAsString,
                    RawBytes = rawBytes,
                    ExpectedContentType = expectedContentType,
                };
            };
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item, expected ATOM and JSON results
        /// and optional extractors
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">The OData payload items to write.</param>
        /// <param name="expectedResultCallback">The callback to use to determine the expected results.</param>
        public PayloadWriterTestDescriptor(
            Settings settings,
            IEnumerable<T> payloadItems,
            WriterTestExpectedResultCallback expectedResultCallback)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadItems, "payloadItems");
            ExceptionUtilities.CheckArgumentNotNull(expectedResultCallback, "expectedResultCallback");

            this.PayloadItems = new ReadOnlyCollection<T>(payloadItems.ToList());
            this.ThrowUserExceptionAt = -1;
            this.ExpectedResultCallback = expectedResultCallback;
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item, expected ATOM and JSON results
        /// and optional extractors
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItem">The OData payload item to write.</param>
        /// <param name="atomResult">The expected ATOM result.</param>
        /// <param name="jsonResult">The expected JSON result.</param>
        /// <param name="atomExtractor">
        /// An optional function to get a fragment of the full ATOM result to compare 
        /// against the expected result data (e.g., extract a single element from 
        /// the fully serialized payload).</param>
        /// <param name="jsonExtractor">
        /// An optional function to get a fragment of the full JSON result to compare 
        /// against the expected result data (e.g., extract a substring from 
        /// the fully serialized payload).</param>
        /// <param name="disableXmlNamespaceNormalization">true to disable normalization of Xml namespaces during result comparison; otherwise false.</param>
        internal PayloadWriterTestDescriptor(
            Settings settings,
            T payloadItem,
            string atomResult,
            string jsonResult,
            Func<XElement, XElement> atomExtractor = null,
            Func<JsonValue, JsonValue> jsonExtractor = null,
            bool disableXmlNamespaceNormalization = false)
            : this(settings, new T[] { payloadItem }, atomResult, jsonResult, atomExtractor, jsonExtractor, disableXmlNamespaceNormalization)
        {
        }

        /// <summary>
        /// Create a new descriptor instance given a payload item, expected ATOM and JSON results
        /// and optional extractors
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        /// <param name="payloadItems">The OData payload items to write.</param>
        /// <param name="atomResult">The expected ATOM result.</param>
        /// <param name="jsonResult">The expected JSON result.</param>
        /// <param name="atomExtractor">
        /// An optional function to get a fragment of the full ATOM result to compare 
        /// against the expected result data (e.g., extract a single element from 
        /// the fully serialized payload).</param>
        /// <param name="jsonExtractor">
        /// An optional function to get a fragment of the full JSON result to compare 
        /// against the expected result data (e.g., extract a substring from 
        /// the fully serialized payload).</param>
        /// <param name="disableXmlNamespaceNormalization">true to disable normalization of Xml namespaces during result comparison; otherwise false.</param>
        internal PayloadWriterTestDescriptor(
            Settings settings,
            IEnumerable<T> payloadItems,
            string atomResult,
            string jsonResult,
            Func<XElement, XElement> atomExtractor = null,
            Func<JsonValue, JsonValue> jsonExtractor = null,
            bool disableXmlNamespaceNormalization = false)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadItems, "payloadItems");

            this.PayloadItems = new ReadOnlyCollection<T>(payloadItems.ToList());
            this.ThrowUserExceptionAt = -1;
            this.ExpectedResultCallback = (testConfig) =>
            {
                return new JsonWriterTestExpectedResults(this.settings.ExpectedResultSettings) { Json = jsonResult, FragmentExtractor = jsonExtractor };
            };
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="PayloadWriterTestDescriptor&lt;T&gt;"/> to clone.</param>
        public PayloadWriterTestDescriptor(PayloadWriterTestDescriptor<T> other)
            : base(other)
        {
            ExceptionUtilities.CheckArgumentNotNull(other, "other");

            this.PayloadItems = other.PayloadItems;
            this.ThrowUserExceptionAt = other.ThrowUserExceptionAt;
        }

        /// <summary>
        /// The OData payload items to write to the writer.
        /// </summary>
        public ReadOnlyCollection<T> PayloadItems
        {
            get;
            set;
        }

        /// <summary>
        /// The position in the payload items when to throw a user exception to simulate an exception in user code. -1 for never.
        /// </summary>
        public int ThrowUserExceptionAt
        {
            get;
            set;
        }
    }
}
