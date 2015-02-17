//---------------------------------------------------------------------
// <copyright file="PayloadFormatVersionAnnotatingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Payload element visitor which adds format version annotations as appropriate to "fix" the payload for a given version.
    /// </summary>
    /// <remarks>Note that the visitor will annotate the payload in-place, so it's usually
    /// necessary to clone the payload before using this.
    /// Also note that the visitor will only add the necessary annotation if they are not already present.
    /// If there already is annotation in place where the visitor would have put one, it skips that element
    /// and does not verify that the annotation in place is correct (this allows manual construction of invalid payloads)</remarks>
    public sealed class PayloadFormatVersionAnnotatingVisitor : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// The version to annotate the payload with.
        /// </summary>
        private readonly DataServiceProtocolVersion version;

        /// <summary>
        /// true if the payload should represent a request payload, false if it's a response payload.
        /// </summary>
        private readonly bool requestPayload;

        /// <summary>
        /// The depth of the payload.
        /// </summary>
        private int depth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="version">The version to annotate the payload with.</param>
        /// <param name="requestPayload">true if the payload should represent a request payload, false if it's a response payload.</param>
        private PayloadFormatVersionAnnotatingVisitor(DataServiceProtocolVersion version, bool requestPayload)
        {
            this.version = version;
            this.requestPayload = requestPayload;
            this.depth = 0;
        }

        /// <summary>
        /// Annotates payload with version as appropriate.
        /// </summary>
        /// <param name="payloadElement">The root of the payload to annotate.</param>
        /// <param name="version">The format version to annotate with.</param>
        /// <param name="requestPayload">true if the payload should represent a request payload, false if it's a response payload.</param>
        public static void AnnotateVerboseJson(ODataPayloadElement payloadElement, DataServiceProtocolVersion version, bool requestPayload)
        {
            PayloadFormatVersionAnnotatingVisitor visitor = new PayloadFormatVersionAnnotatingVisitor(version, requestPayload);
            visitor.Recurse(payloadElement);

            // Add the version/response annotation to the root if it's a response.
            if (!requestPayload && !(payloadElement is ODataErrorPayload))
            {
                payloadElement.SetAnnotation(new PayloadFormatVersionAnnotation() { Version = version, Response = true, ResponseWrapper = true });
            }
        }

        /// <summary>
        /// Annotates payload with version as appropriate.
        /// </summary>
        /// <param name="payloadElement">The root of the payload to annotate.</param>
        /// <param name="version">The format version to annotate with.</param>
        /// <param name="requestPayload">true if the payload should represent a request payload, false if it's a response payload.</param>
        public static void AnnotateJsonLight(ODataPayloadElement payloadElement, DataServiceProtocolVersion version, bool requestPayload)
        {
            PayloadFormatVersionAnnotatingVisitor visitor = new PayloadFormatVersionAnnotatingVisitor(version, requestPayload);
            visitor.Recurse(payloadElement);
        }

        /// <summary>
        /// Visits a payload element whose root is an EntitySetInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            base.Visit(payloadElement);

            // Entity set instances (Feeds) use different format in JSON for V2 (and higher) payloads in responses
            // so we need to add an annotation marking it with the right version so that the serializer will use the
            // V2 format.
            if (!this.requestPayload)
            {
                this.AddVersionAnnotation(payloadElement);
            }
        }

        /// <summary>
        /// Visits a payload element whose root is an <see cref="ComplexInstanceCollection"/>.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(ComplexInstanceCollection payloadElement)
        {
            base.Visit(payloadElement);

            // Collections use a different format in JSON for V2 (and higher) payloads (in responses)
            if (!this.requestPayload)
            {
                this.AddVersionAnnotation(payloadElement);
            }
        }

        /// <summary>
        /// Visits a payload element whose root is an <see cref="PrimitiveCollection"/>.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(PrimitiveCollection payloadElement)
        {
            base.Visit(payloadElement);

            // Collections use a different format in JSON for V2 (and higher) payloads (in responses)
            if (!this.requestPayload)
            {
                this.AddVersionAnnotation(payloadElement);
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a LinkCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(LinkCollection payloadElement)
        {
            base.Visit(payloadElement);

            // Link collection (response to the $ref request which returns multiple entity reference links)
            // uses a different format in JSON for V2 (and higher) payloads - but only in responses.
            // So we need to add an annotation marking it with the right version so that the serializer will use the
            // V2 format.
            if (!this.requestPayload)
            {
                this.AddVersionAnnotation(payloadElement);
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a DeferredLink.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(DeferredLink payloadElement)
        {
            base.Visit(payloadElement);

            if (!this.requestPayload || this.depth > 2)
            {
                // Any deferred link inside a navigation link needs to be annotated (request/response payload differs)
                // Top level response payloads need to be annotated as they serialize differently in JSON-L
                this.AddVersionAnnotation(payloadElement);
            }
        }

        /// <summary>
        /// Visits a payload element whose root is a NavigationPropertyInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            base.Visit(payloadElement);

            if (!this.requestPayload)
            {
                this.AddVersionAnnotation(payloadElement);
            }
        }

        /// <summary>
        /// Wrapper for recursively visiting the given element. Used with the callback property to make unit tests easier.
        /// </summary>
        /// <param name="element">The element to visit</param>
        protected override void Recurse(ODataPayloadElement element)
        {
            this.depth++;
            base.Recurse(element);
            this.depth--;
        }

        /// <summary>
        /// Annotates the <paramref name="payloadElement"/> with the format version if it's not already annotated.
        /// </summary>
        /// <param name="payloadElement">The payload element to annotate.</param>
        private void AddVersionAnnotation(ODataPayloadElement payloadElement)
        {
            PayloadFormatVersionAnnotation versionAnnotation =
                (PayloadFormatVersionAnnotation)payloadElement.GetAnnotation(typeof(PayloadFormatVersionAnnotation));

            if (versionAnnotation == null)
            {
                versionAnnotation = new PayloadFormatVersionAnnotation()
                {
                    Version = this.version,
                    Response = !this.requestPayload,
                };

                payloadElement.AddAnnotation(versionAnnotation);
            }
        }
    }
}
