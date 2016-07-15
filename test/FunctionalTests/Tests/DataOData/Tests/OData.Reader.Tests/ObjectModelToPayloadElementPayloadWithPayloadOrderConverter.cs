//---------------------------------------------------------------------
// <copyright file="ObjectModelToPayloadElementPayloadWithPayloadOrderConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData;
using Microsoft.Test.Taupo.Astoria.Contracts.OData;
using Microsoft.Test.Taupo.OData.Common;

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    /// <summary>
    /// Converts OData object model to ODataPayloadElement model.
    /// </summary>
    public class ObjectModelToPayloadElementWithPayloadOrderConverter : ObjectModelToPayloadElementConverter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectModelToPayloadElementWithPayloadOrderConverter()
        {
        }

        /// <summary>
        /// Virtual method to create the visitor to perform the conversion.
        /// </summary>
        /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
        /// <param name="payloadContainsId">Whether or not the payload contains identity values for entries.</param>
        /// <param name="payloadContainsEtagForType">A function for determining whether the payload contains etag property values for a given type.</param>
        /// <returns>The newly created visitor.</returns>
        protected override ObjectModelToPayloadElementConverterVisitor CreateVisitor(bool response, bool payloadContainsId, Func<string, bool> payloadContainsEtagForType)
        {
            return new ObjectModelToPayloadElementWithPayloadOrderConverterVisitor(response, this.RequestManager);
        }

        /// <summary>
        /// The inner visitor which performs the conversion.
        /// </summary>
        protected class ObjectModelToPayloadElementWithPayloadOrderConverterVisitor : ObjectModelToPayloadElementConverterVisitor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="response">true if payload represents a response payload, false if it's a request payload.</param>
            /// <param name="requestManager">The request manager used to convert batch payloads.</param>
            public ObjectModelToPayloadElementWithPayloadOrderConverterVisitor(bool response, IODataRequestManager requestManager)
                : base(response, requestManager)
            {
            }

            /// <summary>
            /// Visits a feed item.
            /// </summary>
            /// <param name="feed">The feed to visit.</param>
            protected override ODataPayloadElement VisitFeed(ODataResourceSet resourceCollection)
            {
                ODataPayloadElement payloadElement = base.VisitFeed(resourceCollection);
                ODataFeedPayloadOrderObjectModelAnnotation payloadOrderFeedAnnotation = resourceCollection.GetAnnotation<ODataFeedPayloadOrderObjectModelAnnotation>();
                if (payloadOrderFeedAnnotation != null)
                {
                    PayloadOrderODataPayloadElementAnnotation payloadOrderElementAnnotation = new PayloadOrderODataPayloadElementAnnotation();
                    payloadOrderElementAnnotation.PayloadItems.AddRange(payloadOrderFeedAnnotation.PayloadItems);
                    payloadElement.Add(payloadOrderElementAnnotation);
                }

                return payloadElement;
            }

            /// <summary>
            /// Visits an entry item.
            /// </summary>
            /// <param name="entry">The entry to visit.</param>
            protected override ODataPayloadElement VisitEntry(ODataResource entry)
            {
                ODataPayloadElement payloadElement = base.VisitEntry(entry);
                ODataEntryPayloadOrderObjectModelAnnotation payloadOrderEntryAnnotation = entry.GetAnnotation<ODataEntryPayloadOrderObjectModelAnnotation>();
                if (payloadOrderEntryAnnotation != null)
                {
                    PayloadOrderODataPayloadElementAnnotation payloadOrderElementAnnotation = new PayloadOrderODataPayloadElementAnnotation();
                    payloadOrderElementAnnotation.PayloadItems.AddRange(payloadOrderEntryAnnotation.PayloadItems);
                    payloadElement.Add(payloadOrderElementAnnotation);
                }

                return payloadElement;
            }
        }
    }
}