//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.OData.Common.Annotations;

    /// <summary>
    /// This class contains a bunch of utilities to modify ODataPayloadElement paylods to make them suitable for ODataLib.
    /// Many of these methods might be required because the payloads coming out of PayloadGenerator are not exactly what we want or
    /// the result of applying EPM mappings or some other action does not quite generate the results we want.
    /// </summary>
    public static class ODataPayloadElementUtils
    {
        /// <summary>
        /// Remove duplicate annotations that might be added
        /// </summary>
        /// <param name="payload">payload to remove duplicate annotations for</param>
        public static void RemoveEPMDuplicationAnnotations(ODataPayloadElement payload)
        {
            ODataLibPayloadElementComparer comparer = new ODataLibPayloadElementComparer();
            for (int i = 0; i < payload.Annotations.Count; i++)
            {
                var epmAnn1 = payload.Annotations[i] as XmlTreeAnnotation;
                if (epmAnn1 == null)
                {
                    continue;
                }
                for (int j = i + 1; j < payload.Annotations.Count; j++)
                {
                    var epmAnn2 = payload.Annotations[j] as XmlTreeAnnotation;
                    if (epmAnn2 == null)
                    {
                        continue;
                    }
                    if (epmAnn1.LocalName == epmAnn2.LocalName && epmAnn1.NamespaceName == epmAnn2.NamespaceName)
                    {
                        payload.Annotations.RemoveAt(j);
                    }
                }
            }
        }

        /// <summary>
        /// Determins the payload kind appropriate for the payload element used for this test.
        /// </summary>
        /// <param name="payloadElement">The payload element to get the kind for.</param>
        /// <returns>The payload kind to be used for this test.</returns>
        public static ODataPayloadKind GetPayloadKindFromPayloadElement(this ODataPayloadElement payloadElement)
        {
            if (payloadElement == null)
            {
                return ODataPayloadKind.Unsupported;
            }

            switch (payloadElement.ElementType)
            {
                case ODataPayloadElementType.NullPropertyInstance:
                case ODataPayloadElementType.PrimitiveProperty:
                case ODataPayloadElementType.PrimitiveMultiValueProperty:
                case ODataPayloadElementType.NamedStreamInstance:
                case ODataPayloadElementType.NavigationPropertyInstance:
                    return ODataPayloadKind.Property;

                case ODataPayloadElementType.ComplexMultiValueProperty:
                case ODataPayloadElementType.EntitySetInstance:
                    return ODataPayloadKind.ResourceSet;

                case ODataPayloadElementType.ComplexProperty:
                case ODataPayloadElementType.EntityInstance:
                    return ODataPayloadKind.Resource;

                case ODataPayloadElementType.PrimitiveCollection:
                case ODataPayloadElementType.ComplexInstanceCollection:
                case ODataPayloadElementType.PrimitiveMultiValue:
                case ODataPayloadElementType.ComplexMultiValue:
                    return ODataPayloadKind.Collection;

                case ODataPayloadElementType.ODataErrorPayload:
                    return ODataPayloadKind.Error;

                case ODataPayloadElementType.DeferredLink:
                    return ODataPayloadKind.EntityReferenceLink;

                case ODataPayloadElementType.LinkCollection:
                    return ODataPayloadKind.EntityReferenceLinks;

                case ODataPayloadElementType.ServiceDocumentInstance:
                    return ODataPayloadKind.ServiceDocument;

                case ODataPayloadElementType.PrimitiveValue:
                    EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
                    if (typeAnnotation != null && ((IEdmPrimitiveType)typeAnnotation.EdmModelType.Definition).PrimitiveKind == EdmPrimitiveTypeKind.Binary)
                    {
                        return ODataPayloadKind.BinaryValue;
                    }

                    return ODataPayloadKind.Value;

                case ODataPayloadElementType.BatchRequestPayload:
                case ODataPayloadElementType.BatchResponsePayload:
                    return ODataPayloadKind.Batch;

                case ODataPayloadElementType.ComplexInstance:
                    // TODO: ODataLib test item: Add new ODataPayloadElement for parameters payload
                    return ODataPayloadKind.Parameter;

                default:
                    ExceptionUtilities.Assert(
                        false,
                        "Payload element type '{0}' is not yet recognized by GetPayloadKindFromPayloadElement.",
                        payloadElement.ElementType);
                    return ODataPayloadKind.Unsupported;
            }
        }

        /// <summary>
        /// Gets the custom content type header for the payload element (if specified).
        /// </summary>
        /// <param name="payloadElement">The payload element to get the custom content type header for.</param>
        /// <returns>The custom content types header or null if none is specified.</returns>
        public static string GetCustomContentTypeHeader(this ODataPayloadElement payloadElement)
        {
            if (payloadElement == null)
            {
                return null;
            }

            CustomContentTypeHeaderAnnotation contentTypeAnnotation = payloadElement.GetAnnotation<CustomContentTypeHeaderAnnotation>();
            if (contentTypeAnnotation != null)
            {
                return contentTypeAnnotation.ContentType;
            }

            BatchBoundaryAnnotation batchBoundaryAnnotation = payloadElement.GetAnnotation<BatchBoundaryAnnotation>();
            if (batchBoundaryAnnotation != null)
            {
                Debug.Assert(batchBoundaryAnnotation.BatchBoundaryInHeader != null, "If an annotation is found the boundary must not be null.");
                return MimeTypes.MultipartMixed + "; boundary=" + batchBoundaryAnnotation.BatchBoundaryInHeader;
            }

            return null;
        }

        /// <summary>
        /// Get the default content type for the specified payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to compute the default content type for.</param>
        /// <returns>The default content type for the <paramref name="payloadElement"/>.</returns>
        public static string GetDefaultContentType(this ODataPayloadElement payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            ODataPayloadKind payloadKind = GetPayloadKindFromPayloadElement(payloadElement);
            switch (payloadKind)
            {
                case ODataPayloadKind.ResourceSet: return MimeTypes.ApplicationAtomXml;
                case ODataPayloadKind.Resource: return MimeTypes.ApplicationAtomXml;
                case ODataPayloadKind.Property: return MimeTypes.ApplicationXml;
                case ODataPayloadKind.EntityReferenceLink: return MimeTypes.ApplicationXml;
                case ODataPayloadKind.EntityReferenceLinks: return MimeTypes.ApplicationXml;
                case ODataPayloadKind.Value: return MimeTypes.TextPlain;
                case ODataPayloadKind.BinaryValue: return MimeTypes.ApplicationOctetStream;
                case ODataPayloadKind.Collection: return MimeTypes.ApplicationXml;
                case ODataPayloadKind.ServiceDocument: return MimeTypes.ApplicationXml;
                case ODataPayloadKind.MetadataDocument: return MimeTypes.ApplicationXml;
                case ODataPayloadKind.Error: return MimeTypes.ApplicationXml;
                case ODataPayloadKind.Batch: return MimeTypes.MultipartMixed;
                case ODataPayloadKind.Parameter: return MimeTypes.ApplicationJson;
                case ODataPayloadKind.Unsupported:
                default:
                    throw new TaupoNotSupportedException("The payload kind " + payloadKind + " is not supported.");
            }
        }
    }
}
