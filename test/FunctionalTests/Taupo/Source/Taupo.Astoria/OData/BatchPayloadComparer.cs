//---------------------------------------------------------------------
// <copyright file="BatchPayloadComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Default implementation of the batch payload element comparer contract
    /// </summary>
    [ImplementationName(typeof(IBatchPayloadComparer), "Default")]
    public class BatchPayloadComparer : IBatchPayloadComparer
    {
        /// <summary>
        /// Gets or sets the payload element comparer to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementComparer PayloadElementComparer { get; set; }

        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public StackBasedAssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets the logger used to print diagnostic messages.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Compare batch payloads and throws if they are unequal
        /// </summary>
        /// <param name="expectedPayload">expected payload</param>
        /// <param name="actualPayload">actual payload</param>
        public void CompareBatchPayload(ODataPayloadElement expectedPayload, ODataPayloadElement actualPayload)
        {
            if (expectedPayload == null)
            {
                this.Assert.IsNull(actualPayload, "actual payload should be null since expected payload is null");
                return;
            }

            this.Assert.IsNotNull(actualPayload, "actual payload cannot be null since expected payload is not");

            var expectedRequestPayload = expectedPayload as BatchRequestPayload;
            if (expectedRequestPayload != null)
            {
                var actualRequestPayload = actualPayload as BatchRequestPayload;
                this.Assert.IsNotNull(actualRequestPayload, "actual payload must be a request payload since the expected payload is");
                this.CompareParts<IHttpRequest, BatchRequestChangeset>(
                    expectedRequestPayload.Parts.ToList(),
                    actualRequestPayload.Parts.ToList(),
                    this.CompareRequestParts,
                    this.CompareRequestChangeSets);
            }
            else
            {
                var expectedResponsePayload = expectedPayload as BatchResponsePayload;
                this.Assert.IsNotNull(expectedResponsePayload, "expectedPayload must either be a batch response or a batch request payload");

                var actualResponsePayload = actualPayload as BatchResponsePayload;
                this.Assert.IsNotNull(actualResponsePayload, "actual payload must be a response payload since expected payload is");
                this.CompareParts<HttpResponseData, BatchResponseChangeset>(
                    expectedResponsePayload.Parts.ToList(),
                    actualResponsePayload.Parts.ToList(),
                    this.CompareResponseParts,
                    this.CompareResponseChangeSets);
            }
        }

        /// <summary>
        /// Compares batch parts
        /// </summary>
        /// <typeparam name="TOperation">Operation type</typeparam>
        /// <typeparam name="TChangeSet">ChangeSet type</typeparam>
        /// <param name="expectedParts">Expected parts</param>
        /// <param name="actualParts">Actual parts</param>
        /// <param name="comparePartOperations">A method to compare part operations</param>
        /// <param name="compareChangeSets">A method to compare change Sets</param>
        private void CompareParts<TOperation, TChangeSet>(
            IList<IMimePart> expectedParts,
            IList<IMimePart> actualParts,
            Action<TOperation, TOperation> comparePartOperations,
            Action<TChangeSet, TChangeSet> compareChangeSets)
            where TOperation : class, IMimePart
            where TChangeSet : class, IMimePart
        {
            this.Assert.AreEqual(expectedParts.Count(), actualParts.Count(), "Parts count does not match");
            using (this.Assert.WithMessage("Batch parts did not match"))
            {
                for (int i = 0; i < expectedParts.Count; i++)
                {
                    var expectedPart = expectedParts[i];
                    var actualPart = actualParts[i];

                    var expectedPartOperation = expectedPart as MimePartData<TOperation>;
                    if (expectedPartOperation != null)
                    {
                        var actualPartOperation = actualPart as MimePartData<TOperation>;
                        this.CompareHeaders(expectedPartOperation.Headers, actualPartOperation.Headers);
                        comparePartOperations(expectedPartOperation.Body, actualPartOperation.Body);
                    }
                    else
                    {
                        var expectedChangeSet = expectedPart as TChangeSet;
                        var actualChangeSet = actualPart as TChangeSet;
                        this.CompareHeaders(expectedChangeSet.Headers, actualChangeSet.Headers);
                        compareChangeSets(expectedChangeSet, actualChangeSet);
                    }
                }
            }
        }

        /// <summary>
        /// Compares actual and expected headers
        /// </summary>
        /// <param name="expectedHeaders">expected headers</param>
        /// <param name="actualHeaders">actual headers</param>
        private void CompareHeaders(IDictionary<string, string> expectedHeaders, IDictionary<string, string> actualHeaders)
        {
            ExceptionUtilities.CheckArgumentNotNull(actualHeaders, "actualHeaders");
            ExceptionUtilities.CheckArgumentNotNull(expectedHeaders, "expectedHeaders");

            this.Assert.AreEqual(expectedHeaders.Count, actualHeaders.Count, "actual and expected header counts are equal");

            foreach (var expectedPair in expectedHeaders)
            {
                var actualPair = expectedHeaders.Where(pair => pair.Key.Equals(expectedPair.Key) && pair.Value.Equals(expectedPair.Value)).SingleOrDefault();
                this.Assert.IsNotNull(actualPair, "No actual header matching the expected header found. Expected header name:{0} value:{1}", expectedPair.Key, expectedPair.Value);
            }
        }

        /// <summary>
        /// Compare request parts except the headers
        /// </summary>
        /// <param name="expectedRequestPart">Expected request part</param>
        /// <param name="actualRequestPart">Actual request part</param>
        private void CompareRequestParts(IHttpRequest expectedRequestPart, IHttpRequest actualRequestPart)
        {
            ExceptionUtilities.CheckArgumentNotNull(actualRequestPart, "actualRequestPart");
            ExceptionUtilities.CheckArgumentNotNull(expectedRequestPart, "expectedRequestPart");

            this.Assert.AreEqual(expectedRequestPart.Verb, actualRequestPart.Verb, "actual and expected verbs must be equal");
            this.Assert.AreEqual(expectedRequestPart.GetRequestUri().ToString(), actualRequestPart.GetRequestUri().ToString(), "actual and expected Uris must be equal");

            ODataRequest expectedODataRequest = expectedRequestPart as ODataRequest;
            ODataRequest actualODataRequest = actualRequestPart as ODataRequest;

            if (expectedODataRequest != null && actualODataRequest != null)
            {
                if (expectedODataRequest.Body == null && actualODataRequest.Body == null)
                {
                    return;
                }
                
                if (expectedODataRequest.Body == null || actualODataRequest.Body == null)
                {
                    // If the actual body is not null do all the checks that it fits with what the test deserializer produces when the operation has no body
                    if (actualODataRequest.Body != null && actualODataRequest.Body.RootElement != null)
                    {
                        this.Assert.AreEqual(ODataPayloadElementType.PrimitiveValue, actualODataRequest.Body.RootElement.ElementType, "Either the expected or the actual request body is null (but the other is not); bodies don't match.");
                        this.Assert.IsTrue(((PrimitiveValue)actualODataRequest.Body.RootElement).ClrValue is byte[], "Either the expected or the actual request body is null (but the other is not); bodies don't match.");
                        this.Assert.AreEqual(0, ((byte[])((PrimitiveValue)actualODataRequest.Body.RootElement).ClrValue).Length, "Either the expected or the actual request body is null (but the other is not); bodies don't match.");
                        return;
                    }
                    else if (expectedODataRequest.Body != null && expectedODataRequest.Body.RootElement != null)
                    {
                        this.Assert.AreEqual(ODataPayloadElementType.PrimitiveValue, expectedODataRequest.Body.RootElement.ElementType, "Either the expected or the actual request body is null (but the other is not); bodies don't match.");
                        this.Assert.IsTrue(((PrimitiveValue)expectedODataRequest.Body.RootElement).ClrValue is byte[], "Either the expected or the actual request body is null (but the other is not); bodies don't match.");
                        this.Assert.AreEqual(0, ((byte[])((PrimitiveValue)expectedODataRequest.Body.RootElement).ClrValue).Length, "Either the expected or the actual request body is null (but the other is not); bodies don't match.");
                        return;
                    }
                }

                this.PayloadElementComparer.Compare(expectedODataRequest.Body.RootElement, actualODataRequest.Body.RootElement);
            }
            else
            {
                expectedRequestPart.WriteToLog(this.Logger, LogLevel.Verbose);
                actualRequestPart.WriteToLog(this.Logger, LogLevel.Verbose);
                this.Assert.IsTrue(expectedRequestPart.GetRequestBody().IsEqualTo<byte>(actualRequestPart.GetRequestBody()), "byte bodies from actual and expected parts should be equal");
            }
        }

        /// <summary>
        /// Compare response parts except the headers
        /// </summary>
        /// <param name="expectedResponsePart">Expected response part</param>
        /// <param name="actualResponsePart">Actual response part</param>
        private void CompareResponseParts(HttpResponseData expectedResponsePart, HttpResponseData actualResponsePart)
        {
            ExceptionUtilities.CheckArgumentNotNull(actualResponsePart, "actualResponsePart");
            ExceptionUtilities.CheckArgumentNotNull(expectedResponsePart, "expectedResponsePart");

            this.Assert.AreEqual(expectedResponsePart.StatusCode, actualResponsePart.StatusCode, "status codes should be equal");

            ODataResponse expectedODataResponse = expectedResponsePart as ODataResponse;
            ODataResponse actualODataResponse = actualResponsePart as ODataResponse;
            if (expectedODataResponse != null && actualODataResponse != null)
            {
                if (expectedODataResponse.RootElement == null && actualODataResponse.RootElement == null)
                {
                    return;
                }

                if (expectedODataResponse.RootElement == null || actualODataResponse.RootElement == null)
                {
                    // If the actual body is not null do all the checks that it fits with what the test deserializer produces when the operation has no body
                    if (actualODataResponse.RootElement != null)
                    {
                        this.Assert.AreEqual(ODataPayloadElementType.PrimitiveValue, actualODataResponse.RootElement.ElementType, "Either the expected or the actual response body is null (but the other is not); bodies don't match.");
                        this.Assert.IsTrue(((PrimitiveValue)actualODataResponse.RootElement).ClrValue is byte[], "Either the expected or the actual response body is null (but the other is not); bodies don't match.");
                        this.Assert.AreEqual(0, ((byte[])((PrimitiveValue)actualODataResponse.RootElement).ClrValue).Length, "Either the expected or the actual response body is null (but the other is not); bodies don't match.");
                        return;
                    }
                    else if (expectedODataResponse.RootElement != null)
                    {
                        this.Assert.AreEqual(ODataPayloadElementType.PrimitiveValue, expectedODataResponse.RootElement.ElementType, "Either the expected or the actual response body is null (but the other is not); bodies don't match.");
                        this.Assert.IsTrue(((PrimitiveValue)expectedODataResponse.RootElement).ClrValue is byte[], "Either the expected or the actual response body is null (but the other is not); bodies don't match.");
                        this.Assert.AreEqual(0, ((byte[])((PrimitiveValue)expectedODataResponse.RootElement).ClrValue).Length, "Either the expected or the actual response body is null (but the other is not); bodies don't match.");
                        return;
                    }
                }

                this.PayloadElementComparer.Compare(expectedODataResponse.RootElement, actualODataResponse.RootElement);
            }
            else
            {
                expectedResponsePart.WriteToLog(this.Logger, LogLevel.Verbose);
                actualResponsePart.WriteToLog(this.Logger, LogLevel.Verbose);
                this.Assert.IsTrue(expectedResponsePart.Body.IsEqualTo<byte>(actualResponsePart.Body), "actual and expected body parts should be equal");
            }
        }

        /// <summary>
        /// Compare aspects of batch request changeSets specific to request changeSets
        /// </summary>
        /// <param name="expectedBatchChangeset">Expected batch changeSet</param>
        /// <param name="actualBatchChangeset">Actual batch changeSet</param>
        private void CompareRequestChangeSets(BatchRequestChangeset expectedBatchChangeset, BatchRequestChangeset actualBatchChangeset)
        {
            ExceptionUtilities.CheckArgumentNotNull(actualBatchChangeset, "actualBatchChangeset");
            ExceptionUtilities.CheckArgumentNotNull(expectedBatchChangeset, "expectedBatchChangeset");

            this.Assert.IsNotNull(actualBatchChangeset.Operations, "actual Operations cannot be null");
            this.Assert.IsNotNull(expectedBatchChangeset.Operations, "expected Operations cannot be null");
            var actualOperations = actualBatchChangeset.Operations.ToList();
            var expectedOperations = expectedBatchChangeset.Operations.ToList();

            this.Assert.AreEqual(expectedOperations.Count, actualOperations.Count, "The number of expected and actual operations must match.");

            for (int i = 0; i < actualOperations.Count; i++)
            {
                var actualPart = actualOperations[i];
                var expectedPart = expectedOperations[i];
                this.CompareRequestParts(expectedPart, actualPart);
            }
        }

        /// <summary>
        /// Compare aspects of batch response changeSets specific to response changeSets
        /// </summary>
        /// <param name="expectedBatchChangeset">Expected batch response changeSets</param>
        /// <param name="actualBatchChangeset">Actual batch response changeSets</param>
        private void CompareResponseChangeSets(BatchResponseChangeset expectedBatchChangeset, BatchResponseChangeset actualBatchChangeset)
        {
            ExceptionUtilities.CheckArgumentNotNull(actualBatchChangeset, "actualBatchChangeset");
            ExceptionUtilities.CheckArgumentNotNull(expectedBatchChangeset, "expectedBatchChangeset");

            this.Assert.IsNotNull(actualBatchChangeset.Operations, "actual Operations cannot be null");
            this.Assert.IsNotNull(expectedBatchChangeset.Operations, "expected Operations cannot be null");
            var actualOperations = actualBatchChangeset.Operations.ToList();
            var expectedOperations = expectedBatchChangeset.Operations.ToList();

            for (int i = 0; i < actualOperations.Count; i++)
            {
                var actualPart = actualOperations[i];
                var expectedPart = expectedOperations[i];
                this.CompareResponseParts(expectedPart, actualPart);
            }
        }
    }
}
