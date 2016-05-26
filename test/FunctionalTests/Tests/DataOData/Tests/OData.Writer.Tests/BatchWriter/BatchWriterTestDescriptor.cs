//---------------------------------------------------------------------
// <copyright file="BatchWriterTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Tests.WriterTests.BatchWriter
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.BatchWriter;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    #endregion Namespaces
    
    /// <summary>
    /// Test payload and result descriptor for batch writing.
    /// </summary>
    public sealed class BatchWriterTestDescriptor
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public class Settings : WriterTestDescriptor.Settings
        {
        }

        private readonly InvocationAndOperationDescriptor[] invocationsAndOperationDescriptors;
        private readonly PayloadWriterTestDescriptor.WriterTestExpectedResultCallback expectedResultCallback;
        private readonly Uri baseUri;
        private readonly int? maxPartsPerBatch;
        private readonly int? maxOperationsPerChangeset;
        private readonly IODataPayloadUriConverter urlResolver;

        public BatchWriterTestDescriptor(Settings settings, WriterInvocations[] invocations, Dictionary<string, string> expectedHeaders = null, Uri baseUri = null)
        {
            this.TestDescriptorSettings = settings;
            this.invocationsAndOperationDescriptors = invocations.Select(i => new InvocationAndOperationDescriptor { Invocation = i, OperationDescriptor = null }).ToArray();
            this.expectedResultCallback = CreateExpectedResultCallback(this.invocationsAndOperationDescriptors, expectedHeaders, this.TestDescriptorSettings.ExpectedResultSettings);
            this.baseUri = baseUri;
        }

        public BatchWriterTestDescriptor(Settings settings, InvocationAndOperationDescriptor[] invocationsAndOperationDescriptors, int? maxPartsPerBatch, int? maxOperationsPerChangeset)
        {
            this.TestDescriptorSettings = settings;
            this.invocationsAndOperationDescriptors = invocationsAndOperationDescriptors;
            this.expectedResultCallback = CreateExpectedResultCallback(invocationsAndOperationDescriptors, null, this.TestDescriptorSettings.ExpectedResultSettings);
            this.maxPartsPerBatch = maxPartsPerBatch;
            this.maxOperationsPerChangeset = maxOperationsPerChangeset;
        }

        public BatchWriterTestDescriptor(Settings settings, InvocationAndOperationDescriptor[] invocationsAndOperationDescriptors, Dictionary<string, string> expectedHeaders = null, Uri baseUri = null, IODataPayloadUriConverter urlResolver = null)
        {
            this.TestDescriptorSettings = settings;
            this.invocationsAndOperationDescriptors = invocationsAndOperationDescriptors;
            this.expectedResultCallback = CreateExpectedResultCallback(invocationsAndOperationDescriptors, expectedHeaders, this.TestDescriptorSettings.ExpectedResultSettings);
            this.baseUri = baseUri;
            this.urlResolver = urlResolver;
        }

        public BatchWriterTestDescriptor(Settings settings, WriterInvocations[] invocations, string expectedExceptionMessage, Uri baseUri = null)
            : this(settings, invocations.Select(i => new InvocationAndOperationDescriptor { Invocation = i, OperationDescriptor = null }).ToArray(), expectedExceptionMessage, baseUri)
        {
        }

        public BatchWriterTestDescriptor(Settings settings, InvocationAndOperationDescriptor[] invocationsAndOperationDescriptors, string expectedExceptionMessage, Uri baseUri = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedExceptionMessage, "expectedExceptionMessage");

            this.TestDescriptorSettings = settings;
            this.invocationsAndOperationDescriptors = invocationsAndOperationDescriptors;
            this.expectedResultCallback = CreateExpectedErrorResultCallback(expectedExceptionMessage, null, null, this.TestDescriptorSettings.ExpectedResultSettings);
            this.baseUri = baseUri;
        }

        public BatchWriterTestDescriptor(Settings settings, InvocationAndOperationDescriptor[] invocationsAndOperationDescriptors, ExpectedException expectedException, Uri baseUri = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedException, "expectedException");

            this.TestDescriptorSettings = settings;
            this.invocationsAndOperationDescriptors = invocationsAndOperationDescriptors;
            this.expectedResultCallback = CreateExpectedErrorResultCallback(null, null, expectedException, this.TestDescriptorSettings.ExpectedResultSettings);
            this.baseUri = baseUri;
        }

        public BatchWriterTestDescriptor(Settings settings, InvocationAndOperationDescriptor[] invocationsAndOperationDescriptors, Exception expectedException, Uri baseUri = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedException, "expectedException");

            this.TestDescriptorSettings = settings;
            this.invocationsAndOperationDescriptors = invocationsAndOperationDescriptors;
            this.expectedResultCallback = CreateExpectedErrorResultCallback(null, expectedException, null, this.TestDescriptorSettings.ExpectedResultSettings);
            this.baseUri = baseUri;
        }

        public BatchWriterTestDescriptor(Settings settings, InvocationAndOperationDescriptor[] invocationsAndOperationDescriptors, int? maxPartsPerBatch, int? maxOperationsPerChangeset, ExpectedException expectedException)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedException, "expectedExceptionMessage");

            this.TestDescriptorSettings = settings;
            this.invocationsAndOperationDescriptors = invocationsAndOperationDescriptors;
            this.expectedResultCallback = CreateExpectedErrorResultCallback(null, null, expectedException, this.TestDescriptorSettings.ExpectedResultSettings);
            this.maxPartsPerBatch = maxPartsPerBatch;
            this.maxOperationsPerChangeset = maxOperationsPerChangeset;
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        public Settings TestDescriptorSettings { get; set; }

        internal InvocationAndOperationDescriptor[] InvocationsAndOperationDescriptors
        {
            get { return this.invocationsAndOperationDescriptors; }
        }

        internal Uri BaseUri
        {
            get { return this.baseUri; }
        }

        internal int? MaxPartsPerBatch
        {
            get { return this.maxPartsPerBatch; }
        }

        internal int? MaxOperationsPerChangeset
        {
            get { return this.maxOperationsPerChangeset; }
        }
        
        internal PayloadWriterTestDescriptor.WriterTestExpectedResultCallback ExpectedResultCallback
        {
            get { return this.expectedResultCallback; }
        }

        internal IODataPayloadUriConverter UrlResolver
        {
            get { return this.urlResolver; }
        }

        private static PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateExpectedErrorResultCallback(
            string expectedExceptionMessage,
            Exception exception,
            ExpectedException expectedException,
            WriterTestExpectedResults.Settings settings)
        {
            return (testConfiguration) =>
            {
                string errorMessage = expectedExceptionMessage;
                Exception error = exception;

                return new BatchWriterTestExpectedResults(settings)
                {
                    ExpectedODataExceptionMessage = errorMessage,
                    ExpectedException = error,
                    ExpectedException2 = expectedException,
                };
            };
        }

        private static PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateExpectedResultCallback(
            InvocationAndOperationDescriptor[] invocationsAndOperationDescriptors, 
            Dictionary<string, string> expectedHeaders,
            WriterTestExpectedResults.Settings settings)
        {
            return (testConfiguration) =>
            {
                if (testConfiguration.Format != null && testConfiguration.Format != ODataFormat.Batch)
                {
                    throw new NotSupportedException("Unsupported format " + testConfiguration.Format.ToString() + " for writing batch messages found.");
                }

                return new BatchWriterTestExpectedResults(settings)
                {
                    ExpectedContentType = "multipart/mixed",
                    InvocationsAndOperationDescriptors = invocationsAndOperationDescriptors,
                    ExpectedHeaders = expectedHeaders,
                };
            };
        }

        public abstract class BatchWriterOperationTestDescriptor
        {
            public Dictionary<string, string> Headers { get; set; }
        }

        internal abstract class BatchWriterRequestOperationTestDescriptor : BatchWriterOperationTestDescriptor
        {
            public string Method { get; set; }
            public Uri Uri { get; set; }
            public bool IgnoreCrossReferencingRule { get; set; }
        }

        internal sealed class BatchWriterQueryOperationTestDescriptor : BatchWriterRequestOperationTestDescriptor
        {
        }

        internal sealed class BatchWriterChangeSetOperationTestDescriptor : BatchWriterRequestOperationTestDescriptor
        {
            public string Payload { get; set; }
            public BatchWriterUtils.ODataPayload ODataPayload { get; set; }
            public string ContentId { get; set; }
        }

        internal sealed class BatchWriterResponseOperationTestDescriptor : BatchWriterOperationTestDescriptor
        {
            public int StatusCode { get; set; }
            public string Payload { get; set; }
            public BatchWriterUtils.ODataPayload ODataPayload { get; set; }
        }

        public sealed class InvocationAndOperationDescriptor
        {
            public WriterInvocations Invocation { get; set; }
            public BatchWriterOperationTestDescriptor OperationDescriptor { get; set; }
        }

        public enum WriterInvocations
        {
            WriteStartBatch,
            WriteStartChangeSet,
            WriteQueryOperation,
            WriteChangeSetOperation,
            WriteEndChangeSet,
            WriteEndBatch,
            UserException,
        }
    }
}
