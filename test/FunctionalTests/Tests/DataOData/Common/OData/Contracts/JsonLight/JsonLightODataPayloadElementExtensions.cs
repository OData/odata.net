//---------------------------------------------------------------------
// <copyright file="JsonLightODataPayloadElementExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.JsonLight
{
    #region Namespaces
    using System.Text;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Helper extension methods to annotate ODataPayloadElement with JSON Light specific annotations.
    /// </summary>
    public static class JsonLightODataPayloadElementExtensions
    {
        /// <summary>
        /// Adds a JSON Lite property annotation annotaion to the specified PropertyInstance.
        /// </summary>
        /// <typeparam name="T">The type of the property instance to work on.</typeparam>
        /// <param name="propertyInstance">The property instance to annotate.</param>
        /// <param name="annotationName">The annotation name.</param>
        /// <param name="annotationValue">The annotation value.</param>
        /// <returns>The annotated property instance.</returns>
        public static T WithPropertyAnnotation<T>(this T propertyInstance, string annotationName, string annotationValue) where T : PropertyInstance
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyInstance, "propertyInstance");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(annotationName, "annotationName");

            propertyInstance.AddAnnotation(new JsonLightPropertyAnnotationAnnotation { AnnotationName = annotationName, AnnotationValue = annotationValue });
            return propertyInstance;
        }

        /// <summary>
        /// Adds a context URI annotation to the specified ODataPayloadElement.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to work on.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="contextUri">The context URI to annotate with.</param>
        /// <returns>The annotated payload element, for composability.</returns>
        public static T WithContextUri<T>(this T payloadElement, string contextUri) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            JsonLightContextUriAnnotation annotation = null;
            if (contextUri != null)
            {
                annotation = new JsonLightContextUriAnnotation() { ContextUri = contextUri };
                ODataPayloadElementExtensions.SetAnnotation(payloadElement, annotation);
            }

            return payloadElement;
        }

        /// <summary>
        /// Adds a context URI projection annotation to the specified ODataPayloadElement.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to work on.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="contextUriProjection">The context URI projection to annotate with.</param>
        /// <returns>The annotated payload element, for composability.</returns>
        public static T WithContextUriProjection<T>(this T payloadElement, string contextUriProjection) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            JsonLightContextUriProjectionAnnotation annotation = new JsonLightContextUriProjectionAnnotation() { ContextUriProjection = contextUriProjection };
            ODataPayloadElementExtensions.SetAnnotation(payloadElement, annotation);
            return payloadElement;
        }

        /// <summary>
        /// Gets the context URI for the specified ODataPayloadElement.
        /// </summary>
        /// <param name="payloadElement">The annotated payload element to get the context URI for.</param>
        /// <returns>The context URI from the annotated payload element or null if no context URI annotation exists.</returns>
        /// <remarks>If not context URI annotation is found on the <paramref name="payloadElement"/>, this
        /// method will try to compute the context URI from the expected type annotation. If successful,
        /// it will cache the computed context URI as annotation on the payload element.</remarks>
        public static string ContextUri(this ODataPayloadElement payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            JsonLightContextUriAnnotation contextUriAnnotation = (JsonLightContextUriAnnotation)payloadElement.GetAnnotation(typeof(JsonLightContextUriAnnotation));

            string contextUri = null;
            bool cacheContextUri = false;

            // If an explicit context URI exists, return it
            if (contextUriAnnotation != null)
            {
                contextUri = contextUriAnnotation.ContextUri;
            }
            else
            {
                // Otherwise construct a context URI from the expected type annotation
                ExpectedTypeODataPayloadElementAnnotation expectedTypeAnnotation =
                    (ExpectedTypeODataPayloadElementAnnotation)payloadElement.GetAnnotation(typeof(ExpectedTypeODataPayloadElementAnnotation));
                if (expectedTypeAnnotation != null)
                {
                    // Construct a context URI from the exptected type annotation
                    JsonLightMetadataDocumentUriAnnotation metadataDocumentUriAnnotation =
                        (JsonLightMetadataDocumentUriAnnotation)payloadElement.GetAnnotation(typeof(JsonLightMetadataDocumentUriAnnotation));
                    string metadataDocumentUri = metadataDocumentUriAnnotation == null
                        ? JsonLightConstants.DefaultMetadataDocumentUri.AbsoluteUri
                        : metadataDocumentUriAnnotation.MetadataDocumentUri;

                    string projectionString = null;
                    JsonLightContextUriProjectionAnnotation contextUriProjectionAnnotation = (JsonLightContextUriProjectionAnnotation)payloadElement.GetAnnotation(typeof(JsonLightContextUriProjectionAnnotation));
                    if (contextUriProjectionAnnotation != null)
                    {
                        // If we have a context URI projection, apply it to the context URI if the context URI does not already have one.
                        projectionString = contextUriProjectionAnnotation.ContextUriProjection;
                        Regex contextUriSelectExpandPattern = new Regex(@"(?:(?<!#Collection))\(.*\)");

                        // A 'null' projection string means that all properties should be projected.
                        if (projectionString != null)
                        {
                            bool hasProjection = contextUriSelectExpandPattern.IsMatch(projectionString);
                            if (!hasProjection)
                            {
                                // Inject the projection string into the context URI
                                projectionString = JsonLightConstants.ContextUriProjectionStart + projectionString + JsonLightConstants.ContextUriProjectionEnd;
                            }
                        }
                    }

                    contextUri = BuildContextUri(payloadElement.ElementType, metadataDocumentUri, expectedTypeAnnotation, projectionString);
                    cacheContextUri = true;
                }
            }

            // Cache the computed context URI on the payload element (non-comparable annotation)
            if (cacheContextUri)
            {
                payloadElement.WithContextUri(contextUri);
                payloadElement.RemoveAnnotations(typeof(JsonLightContextUriProjectionAnnotation));
            }

            return contextUri;
        }

        /// <summary>
        /// Builds a context URI from the expected type annotation.
        /// </summary>
        /// <param name="payloadElementKind">The payload element kind to build the context URI for.</param>
        /// <param name="metadataDocumentUri">The metadata document URI.</param>
        /// <param name="expectedTypeAnnotation">The expected type annotation.</param>
        /// <returns>The constructed context URI.</returns>
        public static string BuildContextUri(ODataPayloadElementType payloadElementKind, string metadataDocumentUri, ExpectedTypeODataPayloadElementAnnotation expectedTypeAnnotation, string projectionString = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(metadataDocumentUri, "metadataDocumentUri");
            ExceptionUtilities.CheckArgumentNotNull(expectedTypeAnnotation, "expectedTypeAnnotation");

            StringBuilder builder = new StringBuilder(metadataDocumentUri);

            switch (payloadElementKind)
            {
                // Entry payload
                case ODataPayloadElementType.EntityInstance:
                    {
                        EdmEntitySet entitySet = (EdmEntitySet)expectedTypeAnnotation.EdmEntitySet;
                        ExceptionUtilities.Assert(entitySet != null, "Entity set is required for entry payloads.");

                        builder.Append('#');
                        AppendEntityContainerElement(builder, entitySet.Container, entitySet.Name);
                        AppendTypeCastIfNeeded(builder, entitySet, expectedTypeAnnotation.EdmExpectedType.Definition);
                        builder.Append(projectionString);
                        builder.Append("/$entity");

                        break;
                    }

                // Feed payload
                case ODataPayloadElementType.EntitySetInstance:
                    {
                        EdmEntitySet entitySet = (EdmEntitySet)expectedTypeAnnotation.EdmEntitySet;
                        ExceptionUtilities.Assert(entitySet != null, "Entity set is required for feed payloads.");
                        builder.Append('#');
                        AppendEntityContainerElement(builder, entitySet.Container, entitySet.Name);
                        AppendTypeCastIfNeeded(builder, entitySet, expectedTypeAnnotation.EdmExpectedType.Definition);
                        builder.Append(projectionString);

                        break;
                    }

                // Property payload
                case ODataPayloadElementType.PrimitiveProperty:             // fall through
                case ODataPayloadElementType.PrimitiveMultiValueProperty:   // fall through
                case ODataPayloadElementType.ComplexMultiValueProperty:     // fall through
                case ODataPayloadElementType.ComplexProperty:               // fall through
                case ODataPayloadElementType.NullPropertyInstance:
                // Collection payload
                case ODataPayloadElementType.EmptyCollectionProperty:       // fall through
                case ODataPayloadElementType.ComplexInstanceCollection: // fall through
                case ODataPayloadElementType.PrimitiveCollection:
                    builder.Append('#');

                    // NOTE: property payloads can be produced by regular properties as well as function imports
                    IEdmTypeReference edmExpectedType = null;
                    if (expectedTypeAnnotation.EdmProperty != null)
                    {
                        edmExpectedType = expectedTypeAnnotation.EdmProperty.Type;
                    }
                    else if (expectedTypeAnnotation.ProductFunctionImport != null)
                    {
                        edmExpectedType = expectedTypeAnnotation.ProductFunctionImport.Operation.ReturnType;
                    }
                    else if (expectedTypeAnnotation.EdmExpectedType != null)
                    {
                        edmExpectedType = expectedTypeAnnotation.EdmExpectedType;
                    }

                    if (edmExpectedType == null)
                    {
                        if (expectedTypeAnnotation.EdmExpectedType != null)
                        {
                            AppendTypeName(builder, expectedTypeAnnotation.EdmExpectedType.Definition);
                        }
                        else if (expectedTypeAnnotation.ProductFunctionImport != null)
                        {
                            AppendTypeName(builder, expectedTypeAnnotation.ProductFunctionImport.Operation.ReturnType.Definition);
                        }
                    }
                    else
                    {
                        AppendTypeName(builder, edmExpectedType.Definition);
                    }

                    break;

                // Entity reference link payload
                case ODataPayloadElementType.DeferredLink:
                case ODataPayloadElementType.LinkCollection:
                    {
                        IEdmEntitySet entitySet = expectedTypeAnnotation.EdmEntitySet;
                        EdmNavigationProperty navigationProperty = expectedTypeAnnotation.EdmNavigationProperty as EdmNavigationProperty;
                        IEdmEntityType entityType = navigationProperty.DeclaringType as IEdmEntityType;

                        ExceptionUtilities.Assert(entitySet != null, "entitySet is required for entity reference link payloads.");
                        ExceptionUtilities.Assert(navigationProperty != null, "Navigation property is required for entity reference link payloads.");

                        builder.Append('#');

                        if (payloadElementKind == ODataPayloadElementType.DeferredLink)
                        {
                            builder.Append("$ref");
                        }
                        else if (payloadElementKind == ODataPayloadElementType.LinkCollection)
                        {
                            builder.Append("Collection($ref)");
                        }

                        break;
                    }

                // Service document payload
                case ODataPayloadElementType.ServiceDocumentInstance:   // fall through
                case ODataPayloadElementType.WorkspaceInstance:
                    // NOTE: the builder already contains the metadata document URI.
                    break;

                default:
                    return null;
            }

            return builder.ToString();
        }

        private static void AppendTypeName(StringBuilder builder, IEdmType type)
        {
            ExceptionUtilities.Assert(type != null, "AppendTypeName() got a null type.");

            builder.Append(type.TestFullName());
        }

        private static void AppendEntityContainerElement(StringBuilder builder, IEdmEntityContainer container, string elementName)
        {
            ExceptionUtilities.CheckArgumentNotNull(builder, "builder");
            ExceptionUtilities.CheckArgumentNotNull(container, "container");

            builder.Append(container.FullName());
            builder.Append(".");
            builder.Append(elementName);
        }

        private static void AppendTypeCastIfNeeded(StringBuilder builder, IEdmEntitySet entitySet, IEdmType expectedType)
        {
            ExceptionUtilities.CheckArgumentNotNull(builder, "builder");
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");

            IEdmEntityType entityDataType = expectedType as IEdmEntityType;
            if (entityDataType == null)
            {
                return;
            }


            if (entitySet.EntityType() == entityDataType)
            {
                // same types; nothing to add to the context URI
                return;
            }

            if (entityDataType.InheritsFrom(entitySet.EntityType()))
            {
                // derived type; add the type cast segment
                builder.Append("/");
                builder.Append(entityDataType.FullName());
                return;
            }

            ExceptionUtilities.Assert(false, "Expected entity type has to be compatible with the base entity type of the set.");
        }
    }
}
