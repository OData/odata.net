//---------------------------------------------------------------------
// <copyright file="ODataConventionalUriBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.OData.JsonLight;

namespace Microsoft.OData.Evaluation
{
    /// <summary>
    /// Implementation of OData URI builder based on OData protocol conventions.
    /// </summary>
    internal sealed class ODataConventionalUriBuilder : ODataUriBuilder
    {
        /// <summary>The base URI of the service. This will be used as the base URI for all entity containers.</summary>
        private readonly Uri serviceBaseUri;

        /// <summary>The specific key-serializer to use based on the convention.</summary>
        private readonly KeySerializer keySerializer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceBaseUri">The base URI of the service. This will be used as the base URI for all entity containers.</param>
        /// <param name="urlKeyDelimiter">Key delimiter used in url.</param>
        internal ODataConventionalUriBuilder(Uri serviceBaseUri, ODataUrlKeyDelimiter urlKeyDelimiter)
        {
            Debug.Assert(serviceBaseUri != null && serviceBaseUri.IsAbsoluteUri, "serviceBaseUri != null && serviceBaseUri.IsAbsoluteUri");
            Debug.Assert(urlKeyDelimiter != null, "urlKeyDelimiter != null");

            this.serviceBaseUri = serviceBaseUri;
            this.keySerializer = KeySerializer.Create(urlKeyDelimiter.EnableKeyAsSegment);
        }

        /// <summary>
        /// Builds the base URI for the entity container.
        /// </summary>
        /// <returns>
        /// The base URI for the entity container.
        /// This can be either an absolute URI,
        /// or relative URI which will be combined with the URI of the metadata document for the service.
        /// null if the model doesn't have the service base URI annotation.
        /// </returns>
        internal override Uri BuildBaseUri()
        {
            return this.serviceBaseUri;
        }

        /// <summary>
        /// Builds the URI for an entity set.
        /// </summary>
        /// <param name="baseUri">The URI to append to.</param>
        /// <param name="entitySetName">The entity set name.</param>
        /// <returns>The entity set URI.</returns>
        internal override Uri BuildEntitySetUri(Uri baseUri, string entitySetName)
        {
            ValidateBaseUri(baseUri);
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(entitySetName, "entitySetName");

            return AppendSegment(baseUri, entitySetName, true /*escapeSegment*/);
        }

        /// <summary>
        /// Builds the entity instance URI with the given key property values.
        /// </summary>
        /// <param name="baseUri">The URI to append to.</param>
        /// <param name="keyProperties">The list of name value pair for key properties.</param>
        /// <param name="entityTypeName">The full name of the entity type we are building the key expression for.</param>
        /// <returns>The entity instance URI.</returns>
        internal override Uri BuildEntityInstanceUri(Uri baseUri, ICollection<KeyValuePair<string, object>> keyProperties, string entityTypeName)
        {
            ValidateBaseUri(baseUri);
            Debug.Assert(keyProperties != null, "keyProperties != null");
            Debug.Assert(!string.IsNullOrEmpty(entityTypeName), "!string.IsNullOrEmpty(entityTypeName)");

            StringBuilder builder = new StringBuilder(UriUtils.UriToString(baseUri));

            // TODO: What should be done about escaping the values.
            // TODO: What should happen if the URL does end with a slash?
            this.AppendKeyExpression(builder, keyProperties, entityTypeName);
            return new Uri(builder.ToString(), UriKind.Absolute);
        }

        /// <summary>
        /// Builds the edit link for a stream property.
        /// </summary>
        /// <param name="baseUri">The URI to append to.</param>
        /// <param name="streamPropertyName">
        /// The name of the stream property the link is computed for;
        /// or null for the default media resource.
        /// </param>
        /// <returns>The edit link for the stream.</returns>
        internal override Uri BuildStreamEditLinkUri(Uri baseUri, string streamPropertyName)
        {
            ValidateBaseUri(baseUri);
            ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, "streamPropertyName");

            if (streamPropertyName == null)
            {
                return AppendSegment(baseUri, ODataConstants.DefaultStreamSegmentName, false /*escapeSegment*/);
            }
            else
            {
                return AppendSegment(baseUri, streamPropertyName, true /*escapeSegment*/);
            }
        }

        /// <summary>
        /// Builds the read link for a stream property.
        /// </summary>
        /// <param name="baseUri">The URI to append to.</param>
        /// <param name="streamPropertyName">
        /// The name of the stream property the link is computed for;
        /// or null for the default media resource.
        /// </param>
        /// <returns>The read link for the stream.</returns>
        internal override Uri BuildStreamReadLinkUri(Uri baseUri, string streamPropertyName)
        {
            ValidateBaseUri(baseUri);
            ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, "streamPropertyName");

            if (streamPropertyName == null)
            {
                return AppendSegment(baseUri, ODataConstants.DefaultStreamSegmentName, false /*escapeSegment*/);
            }
            else
            {
                return AppendSegment(baseUri, streamPropertyName, true /*escapeSegment*/);
            }
        }

        /// <summary>
        /// Builds the navigation link for the navigation property.
        /// </summary>
        /// <param name="baseUri">The URI to append to.</param>
        /// <param name="navigationPropertyName">The name of the navigation property to get the navigation link URI for.</param>
        /// <returns>The navigation link URI for the navigation property.</returns>
        internal override Uri BuildNavigationLinkUri(Uri baseUri, string navigationPropertyName)
        {
            ValidateBaseUri(baseUri);
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");

            return AppendSegment(baseUri, navigationPropertyName, true /*escapeSegment*/);
        }

        /// <summary>
        /// Builds the association link for the navigation property.
        /// </summary>
        /// <param name="baseUri">The URI to append to.</param>
        /// <param name="navigationPropertyName">The name of the navigation property to get the association link URI for.</param>
        /// <returns>The association link URI for the navigation property.</returns>
        internal override Uri BuildAssociationLinkUri(Uri baseUri, string navigationPropertyName)
        {
            ValidateBaseUri(baseUri);
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");

            Uri baseUriWithLinksSegment = AppendSegment(baseUri, navigationPropertyName, true /*escapeSegment*/);

            // We don't want the $ref segment to be escaped, so append that first without escaping, then append the property name with escaping
            return AppendSegment(baseUriWithLinksSegment, ODataConstants.EntityReferenceSegmentName, false /*escapeSegment*/);
        }

        /// <summary>
        /// Builds the operation target URI for the specified <paramref name="operationName"/>.
        /// </summary>
        /// <param name="baseUri">The URI to append to.</param>
        /// <param name="operationName">The fully qualified name of the operation for which to get the target URI.</param>
        /// <param name="bindingParameterTypeName">The binding parameter type name to include in the target, or null/empty if there is none.</param>
        /// <param name="parameterNames">The parameter names to include in the target, or null/empty if there is none.</param>
        /// <returns>The target URI for the operation.</returns>
        internal override Uri BuildOperationTargetUri(Uri baseUri, string operationName, string bindingParameterTypeName, string parameterNames)
        {
            ValidateBaseUri(baseUri);
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            Uri targetUri = baseUri;

            if (!string.IsNullOrEmpty(bindingParameterTypeName))
            {
                targetUri = AppendSegment(baseUri, bindingParameterTypeName, true /*escapeSegment*/);
            }

            if (!string.IsNullOrEmpty(parameterNames))
            {
                operationName += JsonLightConstants.FunctionParameterStart;
                operationName += string.Join(JsonLightConstants.FunctionParameterSeparator, parameterNames.Split(JsonLightConstants.FunctionParameterSeparatorChar).Select(p => p + JsonLightConstants.FunctionParameterAssignment + p).ToArray());
                operationName += JsonLightConstants.FunctionParameterEnd;
            }

            return AppendSegment(targetUri, operationName, false /*escapeSegment*/);
        }

        /// <summary>
        /// Builds a URI with the given type name appended as a new segment on the base URI.
        /// </summary>
        /// <param name="baseUri">The URI to append to.</param>
        /// <param name="typeName">The fully qualified type name to append.</param>
        /// <returns>The URI with the type segment appended.</returns>
        internal override Uri AppendTypeSegment(Uri baseUri, string typeName)
        {
            ValidateBaseUri(baseUri);
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(typeName, "typeName");

            return AppendSegment(baseUri, typeName, true /*escapeSegment*/);
        }

        /// <summary>
        /// Validates the base URI parameter to be a non-null absolute URI.
        /// </summary>
        /// <param name="baseUri">The base URI parameter to validate.</param>
        [Conditional("DEBUG")]
        private static void ValidateBaseUri(Uri baseUri)
        {
            Debug.Assert(baseUri != null, "baseUri != null");
        }

        /// <summary>
        /// Appends a segment to the specified base URI.
        /// </summary>
        /// <param name="baseUri">The base Uri to append the segment to.</param>
        /// <param name="segment">The segment to append.</param>
        /// <param name="escapeSegment">True if the new segment should be escaped, otherwise False.</param>
        /// <returns>New URI with the appended segment and no trailing slash added.</returns>
        private static Uri AppendSegment(Uri baseUri, string segment, bool escapeSegment)
        {
            string baseUriString = UriUtils.UriToString(baseUri);

            if (escapeSegment)
            {
                segment = Uri.EscapeDataString(segment);
            }

            if (baseUriString[baseUriString.Length - 1] != ODataConstants.UriSegmentSeparatorChar)
            {
                return new Uri(baseUriString + ODataConstants.UriSegmentSeparator + segment, UriKind.RelativeOrAbsolute);
            }
            else
            {
                return new Uri(baseUri, segment);
            }
        }

        /// <summary>
        /// Gets the CLR value of a primitive key property.
        /// </summary>
        /// <param name="keyPropertyName">The key property name.</param>
        /// <param name="keyPropertyValue">The key property value.</param>
        /// <param name="entityTypeName">The entity type name we are validating the key value for.</param>
        /// <returns>The primitive value of the key property.</returns>
        private static object ValidateKeyValue(string keyPropertyName, object keyPropertyValue, string entityTypeName)
        {
            if (keyPropertyValue == null)
            {
                throw new ODataException(Strings.ODataConventionalUriBuilder_NullKeyValue(keyPropertyName, entityTypeName));
            }

            return keyPropertyValue;
        }

        /// <summary>
        /// Appends the key expression for the given entity to the given <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="builder">The builder to append onto.</param>
        /// <param name="keyProperties">The list of name value pair for key properties.</param>
        /// <param name="entityTypeName">The full name of the entity type we are building the key expression for.</param>
        private void AppendKeyExpression(StringBuilder builder, ICollection<KeyValuePair<string, object>> keyProperties, string entityTypeName)
        {
            Debug.Assert(builder != null, "builder != null");
            Debug.Assert(keyProperties != null, "keyProperties != null");

            if (!keyProperties.Any())
            {
                throw new ODataException(Strings.ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties(entityTypeName));
            }

            this.keySerializer.AppendKeyExpression(builder, keyProperties, p => p.Key, p => ValidateKeyValue(p.Key, p.Value, entityTypeName));
        }
    }
}