//---------------------------------------------------------------------
// <copyright file="ODataJsonLightContextUriParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Parser for odata context URIs used in JSON Lite.
    /// </summary>
    internal sealed class ODataJsonLightContextUriParser
    {
        /// <summary>
        /// Pattern for key segments, Examples:
        /// Customer(1), Customer('foo'),
        /// Customer(baf04077-a3c0-454b-ac6f-9fec00b8e170), Message(FromUsername='1',MessageId=-10)
        /// Message(geography'SRID=0;Collection(LineString(142.1 64.1,3.14 2.78))'),Message(duration'P6DT23H59M59.9999S')
        /// </summary>
        private static readonly Regex KeyPattern = new Regex(@"^(?:-{0,1}\d+?|\w*'.+?'|[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}|.+?=.+?)$", RegexOptions.IgnoreCase);

        /// <summary>The model to use when resolving the target of the URI.</summary>
        private readonly IEdmModel model;

        /// <summary>The result of parsing the context URI.</summary>
        private readonly ODataJsonLightContextUriParseResult parseResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataJsonLightContextUriParseResult"/> class.
        /// </summary>
        /// <param name="model">The model to use when resolving the target of the URI.</param>
        /// <param name="contextUriFromPayload">The context URI read from the payload.</param>
        private ODataJsonLightContextUriParser(IEdmModel model, Uri contextUriFromPayload)
        {
            Debug.Assert(model != null, "model != null");

            if (!model.IsUserModel())
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_NoModel);
            }

            this.model = model;
            this.parseResult = new ODataJsonLightContextUriParseResult(contextUriFromPayload);
        }

        /// <summary>
        /// Creates a context URI parser and parses the context URI read from the payload.
        /// </summary>
        /// <param name="model">The model to use when resolving the target of the URI.</param>
        /// <param name="contextUriFromPayload">The string value of the odata.metadata annotation read from the payload.</param>
        /// <param name="payloadKind">The payload kind we expect the context URI to conform to.</param>
        /// <param name="clientCustomTypeResolver">The function of client cuetom type resolver.</param>
        /// <param name="needParseFragment">Whether the fragment after $metadata should be parsed, if set to false, only MetadataDocumentUri is parsed.</param>
        /// <param name="throwIfMetadataConflict">Whether to throw if a type specified in the ContextUri is not found in metadata.</param>
        /// <returns>The result from parsing the context URI.</returns>
        internal static ODataJsonLightContextUriParseResult Parse(
            IEdmModel model,
            string contextUriFromPayload,
            ODataPayloadKind payloadKind,
            Func<IEdmType, string, IEdmType> clientCustomTypeResolver,
            bool needParseFragment,
            bool throwIfMetadataConflict = true)
        {
            if (contextUriFromPayload == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_NullMetadataDocumentUri);
            }

            // Create an absolute URI from the payload string
            // TODO: Support relative context uri and resolving other relative uris
            Uri contextUri;
            if (!Uri.TryCreate(contextUriFromPayload, UriKind.Absolute, out contextUri))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute(contextUriFromPayload));
            }

            ODataJsonLightContextUriParser parser = new ODataJsonLightContextUriParser(model, contextUri);

            parser.TokenizeContextUri();
            if (needParseFragment)
            {
                parser.ParseContextUri(payloadKind, clientCustomTypeResolver, throwIfMetadataConflict);
            }

            return parser.parseResult;
        }

        /// <summary>
        /// Extracts the value of the $select query option from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment to extract the $select query option from.</param>
        /// <returns>The value of the $select query option or null if none exists.</returns>
        private static string ExtractSelectQueryOption(string fragment)
        {
            return fragment;
        }

        /// <summary>
        /// Parses a context URI read from the payload into its parts.
        /// </summary>
        private void TokenizeContextUri()
        {
            Uri contextUriFromPayload = this.parseResult.ContextUri;
            Debug.Assert(contextUriFromPayload != null && contextUriFromPayload.IsAbsoluteUri, "contextUriFromPayload != null && contextUriFromPayload.IsAbsoluteUri");

            // Remove the fragment from the URI read from the payload
            UriBuilder uriBuilderWithoutFragment = new UriBuilder(contextUriFromPayload)
            {
                Fragment = null,
            };

            // Make sure the metadata document URI from the settings matches the context URI in the payload.
            this.parseResult.MetadataDocumentUri = uriBuilderWithoutFragment.Uri;

            // Get the fragment of the context URI
            this.parseResult.Fragment = contextUriFromPayload.GetComponents(UriComponents.Fragment, UriFormat.SafeUnescaped);
        }

        /// <summary>
        /// Applies the model and validates the context URI against it.
        /// </summary>
        /// <param name="expectedPayloadKind">The payload kind we expect the context URI to conform to.</param>
        /// <param name="clientCustomTypeResolver">The function of client custom type resolver.</param>
        /// <param name="throwIfMetadataConflict">Whether to throw if a type specified in the ContextUri is not found in metadata.</param>
        private void ParseContextUri(ODataPayloadKind expectedPayloadKind, Func<IEdmType, string, IEdmType> clientCustomTypeResolver, bool throwIfMetadataConflict)
        {
            bool isUndeclared;
            ODataPayloadKind detectedPayloadKind = this.ParseContextUriFragment(this.parseResult.Fragment, clientCustomTypeResolver, throwIfMetadataConflict, out isUndeclared);

            // unsupported payload kind indicates that this is during payload kind detection, so we should not fail.
            bool detectedPayloadKindMatchesExpectation = detectedPayloadKind == expectedPayloadKind || expectedPayloadKind == ODataPayloadKind.Unsupported;
            IEdmType parseType = this.parseResult.EdmType;
            if (parseType != null && parseType.TypeKind == EdmTypeKind.Untyped)
            {
                if (string.Equals(parseType.FullTypeName(), ODataConstants.ContextUriFragmentUntyped, StringComparison.Ordinal))
                {
                    // Anything matches the built-in Edm.Untyped
                    this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.ResourceSet, ODataPayloadKind.Property, ODataPayloadKind.Collection, ODataPayloadKind.Resource };
                    detectedPayloadKindMatchesExpectation = true;
                }
                else if (expectedPayloadKind == ODataPayloadKind.Property || expectedPayloadKind == ODataPayloadKind.Resource)
                {
                    // If we created an untyped type because the name was not resolved it can match any single value
                    this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.Property, ODataPayloadKind.Resource };
                    detectedPayloadKindMatchesExpectation = true;
                }
            }
            else if (parseType != null && parseType.TypeKind == EdmTypeKind.Collection && ((IEdmCollectionType)parseType).ElementType.TypeKind() == EdmTypeKind.Untyped)
            {
                this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.ResourceSet, ODataPayloadKind.Property, ODataPayloadKind.Collection };
                if (expectedPayloadKind == ODataPayloadKind.ResourceSet || expectedPayloadKind == ODataPayloadKind.Property || expectedPayloadKind == ODataPayloadKind.Collection)
                {
                    detectedPayloadKindMatchesExpectation = true;
                }
            }
            else if (detectedPayloadKind == ODataPayloadKind.ResourceSet && parseType.IsODataComplexTypeKind())
            {
                this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.ResourceSet, ODataPayloadKind.Property, ODataPayloadKind.Collection };

                if (expectedPayloadKind == ODataPayloadKind.Property || expectedPayloadKind == ODataPayloadKind.Collection)
                {
                    detectedPayloadKindMatchesExpectation = true;
                }
            }
            else if (detectedPayloadKind == ODataPayloadKind.Resource && parseType.IsODataComplexTypeKind())
            {
                this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.Resource, ODataPayloadKind.Property };
                if (expectedPayloadKind == ODataPayloadKind.Property)
                {
                    detectedPayloadKindMatchesExpectation = true;
                }
            }
            else if (detectedPayloadKind == ODataPayloadKind.Collection)
            {
                // If the detected payload kind is 'collection' it can always also be treated as a property.
                this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.Collection, ODataPayloadKind.Property };
                if (expectedPayloadKind == ODataPayloadKind.Property)
                {
                    detectedPayloadKindMatchesExpectation = true;
                }
            }
            else if (detectedPayloadKind == ODataPayloadKind.Resource)
            {
                this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.Resource, ODataPayloadKind.Delta };
                if (expectedPayloadKind == ODataPayloadKind.Delta)
                {
                    this.parseResult.DeltaKind = ODataDeltaKind.Resource;
                    detectedPayloadKindMatchesExpectation = true;
                }
            }
            else if (detectedPayloadKind == ODataPayloadKind.Property && isUndeclared
                && (expectedPayloadKind == ODataPayloadKind.Resource || expectedPayloadKind == ODataPayloadKind.ResourceSet))
            {
                // for undeclared, we don't know whether it is a resource/resource set or not.
                this.parseResult.DetectedPayloadKinds = new[] { expectedPayloadKind, ODataPayloadKind.Property };
                detectedPayloadKindMatchesExpectation = true;
            }
            else
            {
                this.parseResult.DetectedPayloadKinds = new[] { detectedPayloadKind };
            }

            // If the expected and detected payload kinds don't match and we are not running payload kind detection
            // right now (payloadKind == ODataPayloadKind.Unsupported) and we did not detect a collection kind for
            // an expected property kind (which is allowed), fail.
            if (!detectedPayloadKindMatchesExpectation)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(UriUtils.UriToString(this.parseResult.ContextUri), expectedPayloadKind.ToString()));
            }

            // NOTE: we interpret an empty select query option to mean that nothing should be projected
            //       (whereas a missing select query option means everything should be projected).
            string selectQueryOption = this.parseResult.SelectQueryOption;
            if (selectQueryOption != null)
            {
                if (detectedPayloadKind != ODataPayloadKind.ResourceSet && detectedPayloadKind != ODataPayloadKind.Resource && detectedPayloadKind != ODataPayloadKind.Delta)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidPayloadKindWithSelectQueryOption(expectedPayloadKind.ToString()));
                }
            }
        }

        /// <summary>
        /// Parses the fragment of a context URI.
        /// </summary>
        /// <param name="fragment">The fragment to parse</param>
        /// <param name="clientCustomTypeResolver">The function of client cuetom type resolver.</param>
        /// <param name="throwIfMetadataConflict">Whether to throw if a type specified in the ContextUri is not found in metadata.</param>
        /// <param name="isUndeclared">Indicates if the fragment is for an unknown path segment</param>
        /// <returns>The detected payload kind based on parsing the fragment.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Will be moving to non case statements later, no point in investing in reducing this now")]
        private ODataPayloadKind ParseContextUriFragment(string fragment, Func<IEdmType, string, IEdmType> clientCustomTypeResolver, bool throwIfMetadataConflict, out bool isUndeclared)
        {
            bool hasItemSelector = false;
            ODataDeltaKind kind = ODataDeltaKind.None;
            isUndeclared = false;

            // Deal with /$entity
            if (fragment.EndsWith(ODataConstants.ContextUriFragmentItemSelector, StringComparison.Ordinal))
            {
                hasItemSelector = true;
                fragment = fragment.Substring(0, fragment.Length - ODataConstants.ContextUriFragmentItemSelector.Length);
            }
            else if (fragment.EndsWith(ODataConstants.ContextUriDeltaResourceSet, StringComparison.Ordinal))
            {
                kind = ODataDeltaKind.ResourceSet;
                fragment = fragment.Substring(0, fragment.Length - ODataConstants.ContextUriDeltaResourceSet.Length);
            }
            else if (fragment.EndsWith(ODataConstants.ContextUriDeletedEntry, StringComparison.Ordinal))
            {
                kind = ODataDeltaKind.DeletedEntry;
                fragment = fragment.Substring(0, fragment.Length - ODataConstants.ContextUriDeletedEntry.Length);
            }
            else if (fragment.EndsWith(ODataConstants.ContextUriDeltaLink, StringComparison.Ordinal))
            {
                kind = ODataDeltaKind.Link;
                fragment = fragment.Substring(0, fragment.Length - ODataConstants.ContextUriDeltaLink.Length);
            }
            else if (fragment.EndsWith(ODataConstants.ContextUriDeletedLink, StringComparison.Ordinal))
            {
                kind = ODataDeltaKind.DeletedLink;
                fragment = fragment.Substring(0, fragment.Length - ODataConstants.ContextUriDeletedLink.Length);
            }

            this.parseResult.DeltaKind = kind;

            // Deal with query option
            if (fragment.EndsWith(")", StringComparison.Ordinal))
            {
                int index = fragment.Length - 2;
                for (int rcount = 1; rcount > 0 && index > 0; --index)
                {
                    switch (fragment[index])
                    {
                        case '(':
                            rcount--;
                            break;
                        case ')':
                            rcount++;
                            break;
                    }
                }

                if (index == 0)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidContextUrl(UriUtils.UriToString(this.parseResult.ContextUri)));
                }

                string previous = fragment.Substring(0, index + 1);

                // Don't treat Collection(Edm.Type) as SelectExpand segment
                if (!previous.Equals("Collection"))
                {
                    string selectExpandStr = fragment.Substring(index + 2);
                    selectExpandStr = selectExpandStr.Substring(0, selectExpandStr.Length - 1);

                    // Do not treat Key as SelectExpand segment
                    if (KeyPattern.IsMatch(selectExpandStr))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_LastSegmentIsKeySegment(UriUtils.UriToString(this.parseResult.ContextUri)));
                    }

                    this.parseResult.SelectQueryOption = ExtractSelectQueryOption(selectExpandStr);
                    fragment = previous;
                }
            }

            ODataPayloadKind detectedPayloadKind = ODataPayloadKind.Unsupported;
            EdmTypeResolver edmTypeResolver = new EdmTypeReaderResolver(this.model, clientCustomTypeResolver);

            if (!fragment.Contains(ODataConstants.UriSegmentSeparator) && !hasItemSelector && kind == ODataDeltaKind.None)
            {
                // Service document: no fragment
                if (fragment.Length == 0)
                {
                    detectedPayloadKind = ODataPayloadKind.ServiceDocument;
                }
                else if (fragment.Equals(ODataConstants.CollectionPrefix + "(" + ODataConstants.EntityReferenceSegmentName + ")"))
                {
                    detectedPayloadKind = ODataPayloadKind.EntityReferenceLinks;
                }
                else if (fragment.Equals(ODataConstants.EntityReferenceSegmentName))
                {
                    detectedPayloadKind = ODataPayloadKind.EntityReferenceLink;
                }
                else
                {
                    var foundNavigationSource = this.model.FindDeclaredNavigationSource(fragment);

                    if (foundNavigationSource != null)
                    {
                        // Resource Set: {schema.entity-container.entity-set} or Singleton: {schema.entity-container.singleton}
                        this.parseResult.NavigationSource = foundNavigationSource;
                        this.parseResult.EdmType = edmTypeResolver.GetElementType(foundNavigationSource);
                        detectedPayloadKind = foundNavigationSource is IEdmSingleton ? ODataPayloadKind.Resource : ODataPayloadKind.ResourceSet;
                    }
                    else
                    {
                        // Property: {schema.type} or Collection({schema.type}) where schema.type is primitive or complex.
                        detectedPayloadKind = this.ResolveType(fragment, clientCustomTypeResolver, throwIfMetadataConflict);
                        Debug.Assert(
                            this.parseResult.EdmType.TypeKind == EdmTypeKind.Primitive || this.parseResult.EdmType.TypeKind == EdmTypeKind.Enum || this.parseResult.EdmType.TypeKind == EdmTypeKind.TypeDefinition || this.parseResult.EdmType.TypeKind == EdmTypeKind.Complex || this.parseResult.EdmType.TypeKind == EdmTypeKind.Collection || this.parseResult.EdmType.TypeKind == EdmTypeKind.Entity || this.parseResult.EdmType.TypeKind == EdmTypeKind.Untyped,
                            "The first context URI segment must be a set or a non-entity type.");
                    }
                }
            }
            else
            {
                Debug.Assert(this.parseResult.MetadataDocumentUri.IsAbsoluteUri, "this.parseResult.MetadataDocumentUri.IsAbsoluteUri");

                string metadataDocumentStr = UriUtils.UriToString(this.parseResult.MetadataDocumentUri);

                if (!metadataDocumentStr.EndsWith(ODataConstants.UriMetadataSegment, StringComparison.Ordinal))
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidContextUrl(UriUtils.UriToString(this.parseResult.ContextUri)));
                }

                Uri serviceRoot = new Uri(metadataDocumentStr.Substring(0, metadataDocumentStr.Length - ODataConstants.UriMetadataSegment.Length));

                ODataUriParser odataUriParser = new ODataUriParser(this.model, serviceRoot, new Uri(serviceRoot, fragment));

                ODataPath path;
                try
                {
                    path = odataUriParser.ParsePath();
                }
                catch (ODataException)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidContextUrl(UriUtils.UriToString(this.parseResult.ContextUri)));
                }

                if (path.Count == 0)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidContextUrl(UriUtils.UriToString(this.parseResult.ContextUri)));
                }

                this.parseResult.Path = path;

                parseResult.NavigationSource = path.NavigationSource();
                parseResult.EdmType = path.LastSegment.EdmType;

                ODataPathSegment lastSegment = path.TrimEndingTypeSegment().LastSegment;
                if (lastSegment is EntitySetSegment || lastSegment is NavigationPropertySegment)
                {
                    if (kind != ODataDeltaKind.None)
                    {
                        detectedPayloadKind = ODataPayloadKind.Delta;
                    }
                    else
                    {
                        detectedPayloadKind = hasItemSelector ? ODataPayloadKind.Resource : ODataPayloadKind.ResourceSet;
                    }

                    if (this.parseResult.EdmType is IEdmCollectionType)
                    {
                        var collectionTypeReference = this.parseResult.EdmType.ToTypeReference().AsCollection();
                        if (collectionTypeReference != null)
                        {
                            this.parseResult.EdmType = collectionTypeReference.ElementType().Definition;
                        }
                    }
                }
                else if (lastSegment is SingletonSegment)
                {
                    detectedPayloadKind = ODataPayloadKind.Resource;
                }
                else if (path.IsIndividualProperty())
                {
                    isUndeclared = path.IsUndeclared();
                    detectedPayloadKind = ODataPayloadKind.Property;
                    IEdmComplexType complexType = parseResult.EdmType as IEdmComplexType;
                    if (complexType != null)
                    {
                        detectedPayloadKind = ODataPayloadKind.Resource;
                    }
                    else
                    {
                        IEdmCollectionType collectionType = parseResult.EdmType as IEdmCollectionType;

                        if (collectionType != null)
                        {
                            if (collectionType.ElementType.IsComplex())
                            {
                                this.parseResult.EdmType = collectionType.ElementType.Definition;
                                detectedPayloadKind = ODataPayloadKind.ResourceSet;
                            }
                            else
                            {
                                detectedPayloadKind = ODataPayloadKind.Collection;
                            }
                        }
                    }
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidContextUrl(UriUtils.UriToString(this.parseResult.ContextUri)));
                }
            }

            return detectedPayloadKind;
        }

        /// <summary>
        /// Resolves a type.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="clientCustomTypeResolver">The function of client cuetom type resolver.</param>
        /// <param name="throwIfMetadataConflict">Whether to throw if a type specified in the ContextUri is not found in metadata.</param>
        /// <returns>The resolved Edm type.</returns>
        private ODataPayloadKind ResolveType(string typeName, Func<IEdmType, string, IEdmType> clientCustomTypeResolver, bool throwIfMetadataConflict)
        {
            string typeNameToResolve = EdmLibraryExtensions.GetCollectionItemTypeName(typeName) ?? typeName;
            bool isCollection = typeNameToResolve != typeName;

            EdmTypeKind typeKind;
            IEdmType resolvedType = MetadataUtils.ResolveTypeNameForRead(this.model, /*expectedType*/ null, typeNameToResolve, clientCustomTypeResolver, out typeKind);
            if (resolvedType == null && !throwIfMetadataConflict)
            {
                string namespaceName;
                string name;
                TypeUtils.ParseQualifiedTypeName(typeName, out namespaceName, out name, out isCollection);
                resolvedType = new EdmUntypedStructuredType(namespaceName, name);
            }

            if (resolvedType == null ||
                resolvedType.TypeKind != EdmTypeKind.Primitive
                && resolvedType.TypeKind != EdmTypeKind.Enum
                && resolvedType.TypeKind != EdmTypeKind.Complex
                && resolvedType.TypeKind != EdmTypeKind.Entity
                && resolvedType.TypeKind != EdmTypeKind.TypeDefinition
                && resolvedType.TypeKind != EdmTypeKind.Untyped)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName(UriUtils.UriToString(this.parseResult.ContextUri), typeName));
            }

            if (resolvedType.TypeKind == EdmTypeKind.Entity || resolvedType.TypeKind == EdmTypeKind.Complex)
            {
                this.parseResult.EdmType = resolvedType;
                return isCollection ? ODataPayloadKind.ResourceSet : ODataPayloadKind.Resource;
            }

            // For structured collection ,the EdmType is element type. for primitive collection, it is collection type
            resolvedType = isCollection ? EdmLibraryExtensions.GetCollectionType(resolvedType.ToTypeReference(true /*nullable*/)) : resolvedType;
            this.parseResult.EdmType = resolvedType;
            return isCollection ? ODataPayloadKind.Collection : ODataPayloadKind.Property;
        }
    }
}
