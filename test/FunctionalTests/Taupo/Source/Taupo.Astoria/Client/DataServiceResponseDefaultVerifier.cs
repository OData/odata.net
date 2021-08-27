//---------------------------------------------------------------------
// <copyright file="DataServiceResponseDefaultVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Default verifier for the data service response which does not verify headers.
    /// </summary>
    [ImplementationName(typeof(IDataServiceResponseVerifier), "Default")]
    public class DataServiceResponseDefaultVerifier : IDataServiceResponseVerifier
    {
        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Verifies the data service response.
        /// </summary>
        /// <param name="responseData">The expected data for the response.</param>
        /// <param name="response">The response to verify.</param>
        /// <param name="cachedOperationsFromResponse">The individual operation responses, pre-enumerated and cached</param>
        /// <exception cref="TaupoNotSupportedException">
        /// When exception is expected in the response data 
        /// </exception>
        public void VerifyDataServiceResponse(DataServiceResponseData responseData, DataServiceResponse response, IList<OperationResponse> cachedOperationsFromResponse)
        {
            ExceptionUtilities.CheckArgumentNotNull(responseData, "responseData");
            ExceptionUtilities.CheckArgumentNotNull(response, "response");
            ExceptionUtilities.CheckArgumentNotNull(cachedOperationsFromResponse, "cachedOperationsFromResponse");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.Assert.AreEqual(responseData.IsBatchResponse, response.IsBatchResponse, "Verifying if it's a batch response.");
            this.Assert.AreEqual(responseData.BatchStatusCode, response.BatchStatusCode, "Verifying batch status code.");

            //// No batch headers verification
            //// Note: verifying order of operation responses as well as the content.

            this.Assert.AreEqual(responseData.Count, cachedOperationsFromResponse.Count, "Unexpected number of operation responses in data service response");

            for (int responseOrder = 0; responseOrder < responseData.Count; responseOrder++)
            {
                var operationResponseData = responseData[responseOrder];
                var currentResponse = cachedOperationsFromResponse[responseOrder];

                ChangeOperationResponseData changeResponseData = operationResponseData as ChangeOperationResponseData;
                if (changeResponseData != null)
                {
                    ChangeOperationResponse changeResponse = currentResponse as ChangeOperationResponse;
                    this.Assert.IsNotNull(changeResponse, GetVerificationFailureMessage(responseOrder, "Unexpected type of the operation response.\r\nExpected: {0}\r\nActual:   {1}", typeof(ChangeOperationResponse).FullName, currentResponse));

                    this.VerifyChangeOperationResponse(changeResponseData, changeResponse, responseOrder);
                }
                else
                {
                    throw new TaupoNotSupportedException(
                        GetVerificationFailureMessage(responseOrder, "Verification for the operation response data of type '{0}' is not supported by this verifier.", operationResponseData.GetType().FullName));
                }
            }
        }

        private static string GetVerificationFailureMessage(int responseOrder, string message, params object[] messageArguments)
        {
            if (messageArguments.Length > 0)
            {
                message = string.Format(CultureInfo.InvariantCulture, message, messageArguments);
            }

            return string.Format(CultureInfo.InvariantCulture, "Verification for the operation response with order {0} failed. Details:\r\n{1}", responseOrder, message);
        }

        private void VerifyChangeOperationResponse(ChangeOperationResponseData expected, ChangeOperationResponse actual, int responseOrder)
        {
            this.Assert.IsNotNull(actual.Descriptor, GetVerificationFailureMessage(responseOrder, "Change operation response descriptor should not be null!"));
            this.VerifyDescriptor(expected.DescriptorData, actual.Descriptor, responseOrder);

            this.Assert.AreEqual(
                expected.StatusCode,
                actual.StatusCode,
                GetVerificationFailureMessage(responseOrder, "Status code verification failed for the change operation response data: {0}.", expected));

            // No headers verification

            // Verify exceptions
            if (!string.IsNullOrEmpty(expected.ExceptionId))
            {
                throw new TaupoNotSupportedException(
                    string.Format(
                    CultureInfo.InvariantCulture,
                    "Change operation response data with order {0} has non-null exception id '{1}'. This verifier does not support exception verification.",
                    responseOrder,
                    expected.ExceptionId));
            }

            this.Assert.IsNull(actual.Error, GetVerificationFailureMessage(responseOrder, "Unexpected exception thrown:\r\n{0}", actual.Error));

            this.Assert.AreEqual(
                expected.DescriptorData.State.ToProductEnum(),
                actual.Descriptor.State,
                GetVerificationFailureMessage(responseOrder, "Descriptor State verification failed for the change operation response data: {0}.", expected));
        }

        private void VerifyDescriptor(DescriptorData expected, Descriptor actual, int responseOrder)
        {
            EntityDescriptorData entityDescriptorData = expected as EntityDescriptorData;
            LinkDescriptorData linkDescriptorData = expected as LinkDescriptorData;
            StreamDescriptorData streamDescriptorData = expected as StreamDescriptorData;
            if (entityDescriptorData != null)
            {
                EntityDescriptor entityDescriptor = actual as EntityDescriptor;
                this.Assert.IsNotNull(entityDescriptor, GetVerificationFailureMessage(responseOrder, "Unexpected descriptor type:\r\nExpected: {0}\r\nActual:   {1}\r\nExpected descriptor data: {2}.", typeof(EntityDescriptor).Name, actual.GetType().Name, entityDescriptorData));

                this.Assert.AreSame(
                    entityDescriptorData.Entity,
                    entityDescriptor.Entity,
                    GetVerificationFailureMessage(responseOrder, "Entity verification failed for the entity descriptor data: {0}.", expected));
            }
            else if (linkDescriptorData != null)
            {
                LinkDescriptor linkDescriptor = actual as LinkDescriptor;
                this.Assert.IsNotNull(linkDescriptor, GetVerificationFailureMessage(responseOrder, "Unexpected descriptor type:\r\nExpected: {0}\r\nActual:   {1}\r\nExpected descriptor data: {2}.", typeof(LinkDescriptor).Name, actual.GetType().Name, linkDescriptorData));

                bool notMatch = linkDescriptorData.SourceDescriptor.Entity != linkDescriptor.Source ||
                    (linkDescriptorData.TargetDescriptor == null && linkDescriptor.Target != null) ||
                    (linkDescriptorData.TargetDescriptor != null && linkDescriptorData.TargetDescriptor.Entity != linkDescriptor.Target) ||
                    linkDescriptorData.SourcePropertyName != linkDescriptor.SourceProperty;

                this.Assert.IsFalse(notMatch, GetVerificationFailureMessage(responseOrder, "Link verification failed.\r\nExpected: {0}\r\nActual:   {1}", linkDescriptorData, linkDescriptor.ToTraceString()));
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(streamDescriptorData, "Expected was not an entity, link, or stream descriptor: {0}", expected);

                StreamDescriptor streamDescriptor = actual as StreamDescriptor;

                this.Assert.IsNotNull(streamDescriptor, GetVerificationFailureMessage(responseOrder, "Unexpected descriptor type:\r\nExpected: {0}\r\nActual:   {1}\r\nExpected descriptor data: {2}.", typeof(StreamDescriptor).Name, actual.GetType().Name, streamDescriptorData));

                this.Assert.AreEqual(streamDescriptorData.State.ToProductEnum(), streamDescriptor.State, GetVerificationFailureMessage(responseOrder, "Stream descriptor state verification failed."));
                this.Assert.AreEqual(streamDescriptorData.Name, streamDescriptor.StreamLink.Name, GetVerificationFailureMessage(responseOrder, "Stream descriptor name verification failed."));
                this.Assert.AreEqual(streamDescriptorData.ETag, streamDescriptor.StreamLink.ETag, GetVerificationFailureMessage(responseOrder, "Stream descriptor etag verification failed."));
                this.Assert.AreEqual(streamDescriptorData.ContentType, streamDescriptor.StreamLink.ContentType, GetVerificationFailureMessage(responseOrder, "Stream descriptor content type verification failed."));
                this.Assert.AreEqual(streamDescriptorData.EditLink, streamDescriptor.StreamLink.EditLink, GetVerificationFailureMessage(responseOrder, "Stream descriptor edit link verification failed."));
                this.Assert.AreEqual(streamDescriptorData.SelfLink, streamDescriptor.StreamLink.SelfLink, GetVerificationFailureMessage(responseOrder, "Stream descriptor self link verification failed."));
            }
        }
    }
}
