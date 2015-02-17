//---------------------------------------------------------------------
// <copyright file="ODataPayloadContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Additional payload information that can be used by serializers and deserializers.
    /// </summary>
    public class ODataPayloadContext
    {
        /// <summary>
        /// Gets or sets the PayloadUri.
        /// </summary>
        public ODataUri PayloadUri { get; set; }

        /// <summary>
        /// Gets or sets the payload encoding.
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// Gets or sets the ContentType.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the Effective Request Verb.
        /// </summary>
        public HttpVerb EffectiveRequestVerb { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedPayloadKind.
        /// </summary>
        public ODataPayloadElementType ExpectedPayloadKind { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedEntitySet.
        /// </summary>
        public EntitySet ExpectedEntitySet { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedEntityType.
        /// </summary>
        public EntityType ExpectedEntityType { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedFunction.
        /// </summary>
        public Function ExpectedFunction { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedNavigationProperty.
        /// </summary>
        public NavigationProperty ExpectedNavigationProperty { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedMemberProperty.
        /// </summary>
        public MemberProperty ExpectedMemberProperty { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedPropertyDeclaringParentType.
        /// </summary>
        public NamedStructuralType ExpectedPropertyDeclaringParentType { get; set; }

        /// <summary>
        /// Builds a new instance of the ODataPayloadContext class based on an ODataRequest.
        /// </summary>
        /// <param name="request">The request to build the payload context from.</param>
        /// <returns>The payload context for the request.</returns>
        public static ODataPayloadContext BuildPayloadContextFromRequest(HttpRequestData<ODataUri, ODataPayloadBody> request)
        {
            var payloadContext = new ODataPayloadContext();

            ODataUri uri = request.Uri;
            if (uri != null)
            {
                payloadContext = BuildODataPayloadContextFromODataUri(uri);
            }

            payloadContext.EffectiveRequestVerb = request.GetEffectiveVerb();

            return payloadContext;
        }

        /// <summary>
        /// Builds the ODataPayloadContext context from OODataUri.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>A OData Payload Context</returns>
        public static ODataPayloadContext BuildODataPayloadContextFromODataUri(ODataUri uri)
        {
            var payloadContext = new ODataPayloadContext();
            payloadContext.PayloadUri = uri;
            payloadContext.ExpectedPayloadKind = uri.GetExpectedPayloadType();

            switch (payloadContext.ExpectedPayloadKind)
            {
                case ODataPayloadElementType.EntitySetInstance:
                case ODataPayloadElementType.EntityInstance:
                    SetExpectedEntitySetAndTypeFromUri(payloadContext, uri);
                    break;

                case ODataPayloadElementType.PrimitiveProperty:
                case ODataPayloadElementType.PrimitiveMultiValueProperty:
                case ODataPayloadElementType.ComplexProperty:
                case ODataPayloadElementType.ComplexMultiValueProperty:
                case ODataPayloadElementType.EmptyCollectionProperty:
                    var propertySegment = uri.Segments.OfType<PropertySegment>().LastOrDefault();
                    if (propertySegment != null)
                    {
                        payloadContext.ExpectedMemberProperty = propertySegment.Property;
                        SetExpectedEntitySetAndTypeFromUri(payloadContext, uri);

                        var parentPropertySegment = uri.Segments.GetSecondToLastOrDefault<PropertySegment>();
                        if (parentPropertySegment != null)
                        {
                            var collectionDataType = parentPropertySegment.Property.PropertyType as CollectionDataType;
                            if (collectionDataType != null)
                            {
                                var elementComplexDataType = collectionDataType.ElementDataType as ComplexDataType;
                                ExceptionUtilities.CheckObjectNotNull(elementComplexDataType, "Expected non complex DataType");
                                payloadContext.ExpectedPropertyDeclaringParentType = elementComplexDataType.Definition;
                            }
                            else
                            {
                                var complexDataType = parentPropertySegment.Property.PropertyType as ComplexDataType;
                                ExceptionUtilities.CheckObjectNotNull(complexDataType, "Expected non null complex DataType");
                                payloadContext.ExpectedPropertyDeclaringParentType = complexDataType.Definition;
                            }
                        }
                        else
                        {
                            payloadContext.ExpectedPropertyDeclaringParentType = payloadContext.ExpectedEntityType;
                        }
                    }
                    else
                    {
                        ExceptionUtilities.Assert(
                            uri.IsFunction() || uri.IsServiceOperation() || uri.IsAction(),
                            "Property uri contains no property segments and is not a function/service operation/action.");
                        var functionSegment = uri.Segments.Last() as FunctionSegment ?? uri.Segments[uri.Segments.Count - 2] as FunctionSegment;
                        ExceptionUtilities.CheckObjectNotNull(functionSegment, "Failed to find function segment in finction/service operation uri");
                        payloadContext.ExpectedFunction = functionSegment.Function;
                    }

                    break;

                case ODataPayloadElementType.DeferredLink:
                case ODataPayloadElementType.LinkCollection:
                    var navigationSegment = uri.Segments.OfType<NavigationSegment>().Last();
                    ExceptionUtilities.CheckObjectNotNull(navigationSegment, "No navigation segments found for expected navigation property uri");
                    payloadContext.ExpectedNavigationProperty = navigationSegment.NavigationProperty;
                    SetExpectedEntitySetAndTypeFromUri(payloadContext, uri);
                    break;
            }

            return payloadContext;
        }

        private static void SetExpectedEntitySetAndTypeFromUri(ODataPayloadContext payloadContext, ODataUri uri)
        {
            EntitySet entitySet = null;
            EntityType entityType = null;

            uri.TryGetExpectedEntitySetAndType(out entitySet, out entityType);
            payloadContext.ExpectedEntitySet = entitySet;
            payloadContext.ExpectedEntityType = entityType;
        }
    }
}
