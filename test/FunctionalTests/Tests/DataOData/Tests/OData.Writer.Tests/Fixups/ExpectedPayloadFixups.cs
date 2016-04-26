//---------------------------------------------------------------------
// <copyright file="ExpectedPayloadFixups.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Common;

    /// <summary>
    /// Applies fixups to the expected payload
    /// </summary>
    public class ExpectedPayloadFixups
    {
        /// <summary>
        /// Fixes up payload based on format
        /// </summary>
        /// <param name="payloadElement">The payload element to fix.</param>
        /// <param name="format">The format to fix up for.</param>
        public string Fixup(ODataPayloadElement payload, ODataFormat format)
        {
            if (format == ODataFormat.Json)
            {
                return this.JsonFixup(payload);
            }
            else if (format == null && (payload is BatchRequestPayload || payload is BatchResponsePayload))
            {
                return this.DefaultFixup(payload);
            }

            return MimeTypes.ApplicationXml;
        }

        private string JsonFixup(ODataPayloadElement payload)
        {
            // We have removed all the annotations except for Self Link which also we can remove since it shows up as ID and 
            // convert primitives to expected JSON representations
            payload.Accept(new ODataPayloadJsonNormalizer());
            payload.Accept(new RemoveFeedIDFixup());
            payload.Accept(new ReorderProperties());
            payload.Accept(new ODataPayloadElementNullTypenameVisitor());
            payload.Accept(new RemoveMLEAnnotation());
            payload.Accept(new RemoveAnnotations());
            payload.Accept(new JsonSelfLinkToEditLinkFixup());
            payload.Accept(new RemoveExpandedLinkUriStringVisitor());
            return MimeTypes.ApplicationJson;
        }

        private string AtomFixup(ODataPayloadElement payload)
        {
            // We have removed all the annotations except for Self Link which also we can remove since it shows up as ID and 
            // convert primitives to expected JSON representations
            // TODO: Remove visitor
            payload.Accept(new ODataPayloadElementNullIDVisitor());
            payload.Accept(new AddExpandedLinkMetadata());
            payload.Accept(new ODataPayloadElementAddDefaultAtomMetadata());
            payload.Accept(new ODataPayloadElementNullTypenameVisitor());
            payload.Accept(new NullStringTypenameFixup());
            payload.Accept(new RemoveAnnotations());
            return MimeTypes.ApplicationAtomXml;
        }

        private string DefaultFixup(ODataPayloadElement payload)
        {
            payload.Accept(new ExpectedBatchPayloadFixup());
            return MimeTypes.MultipartMixed;
        }
    }
}
