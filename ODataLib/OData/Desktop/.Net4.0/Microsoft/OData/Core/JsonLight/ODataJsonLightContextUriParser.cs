//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// Parser for odata context URIs used in JSON Lite.
    /// </summary>
    internal sealed class ODataJsonLightContextUriParser
    {
        /// <summary>The start of the select query option (including the '=' character).</summary>
        private static readonly string SelectQueryOptionStart = 
            JsonLightConstants.ContextUriSelectQueryOptionName + JsonLightConstants.ContextUriQueryOptionValueSeparator;

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
        /// <param name="version">The OData version to use for determining the set of built-in functions available.</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <returns>The result from parsing the context URI.</returns>
        internal static ODataJsonLightContextUriParseResult Parse(
            IEdmModel model,
            string contextUriFromPayload,
            ODataPayloadKind payloadKind,
            ODataVersion version,
            ODataReaderBehavior readerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            
            if (contextUriFromPayload == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_NullMetadataDocumentUri);
            }

            // Create an absolute URI from the payload string
            Uri contextUri = new Uri(contextUriFromPayload, UriKind.Absolute);
            ODataJsonLightContextUriParser parser = new ODataJsonLightContextUriParser(model, contextUri);

            parser.TokenizeContextUri();

            parser.ParseContextUri(payloadKind, readerBehavior, version);
            return parser.parseResult;
        }

        /// <summary>
        /// Extracts the value of the $select query option from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment to extract the $select query option from.</param>
        /// <returns>The value of the $select query option or null if none exists.</returns>
        private static string ExtractSelectQueryOption(string fragment)
        {
            Debug.Assert(fragment != null, "queryString != null");

            // First find the $select query option
            int startIx = fragment.IndexOf(SelectQueryOptionStart, StringComparison.Ordinal);
            if (startIx < 0)
            {
                return null;
            }

            // Find the end of the $select query option which is either 
            // an '&' character or the end of the string.
            int valueStartIx = startIx + SelectQueryOptionStart.Length;
            int endIx = fragment.IndexOf(JsonLightConstants.ContextUriQueryOptionSeparator, valueStartIx);

            string selectQueryOptionValue;
            if (endIx < 0)
            {
                selectQueryOptionValue = fragment.Substring(valueStartIx);
            }
            else
            {
                selectQueryOptionValue = fragment.Substring(valueStartIx, endIx - valueStartIx);
            }

            return selectQueryOptionValue.Trim();
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
            this.parseResult.Fragment = contextUriFromPayload.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
        }

        /// <summary>
        /// Applies the model and validates the context URI against it.
        /// </summary>
        /// <param name="expectedPayloadKind">The payload kind we expect the context URI to conform to.</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="version">The version of the payload being read.</param>
        private void ParseContextUri(ODataPayloadKind expectedPayloadKind, ODataReaderBehavior readerBehavior, ODataVersion version)
        {
            ODataPayloadKind detectedPayloadKind = this.ParseContextUriFragment(this.parseResult.Fragment, readerBehavior, version);

            // unsupported payload kind indicates that this is during payload kind detection, so we should not fail.
            bool detectedPayloadKindMatchesExpectation = detectedPayloadKind == expectedPayloadKind || expectedPayloadKind == ODataPayloadKind.Unsupported;
            if (detectedPayloadKind == ODataPayloadKind.Collection)
            {
                // If the detected payload kind is 'collection' it can always also be treated as a property.
                this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.Collection, ODataPayloadKind.Property };
                if (expectedPayloadKind == ODataPayloadKind.Property)
                {
                    detectedPayloadKindMatchesExpectation = true;
                }
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
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(UriUtilsCommon.UriToString(this.parseResult.ContextUri), expectedPayloadKind.ToString()));
            }

            // NOTE: we interpret an empty select query option to mean that nothing should be projected
            //       (whereas a missing select query option means everything should be projected).
            string selectQueryOption = this.parseResult.SelectQueryOption;
            if (selectQueryOption != null)
            {
                if (detectedPayloadKind != ODataPayloadKind.Feed && detectedPayloadKind != ODataPayloadKind.Entry)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidPayloadKindWithSelectQueryOption(expectedPayloadKind.ToString()));
                }
            }
        }

        /// <summary>
        /// Parses the fragment of a context URI.
        /// </summary>
        /// <param name="fragment">The fragment to parse</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="version">The OData version to use for determining the set of built-in functions available.</param>
        /// <returns>The detected payload kind based on parsing the fragment.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Will be moving to non case statements later, no point in investing in reducing this now")]
        private ODataPayloadKind ParseContextUriFragment(string fragment, ODataReaderBehavior readerBehavior, ODataVersion version)
        {
            // remove any query options like $select that may have been embedded in the fragment.
            int indexOfQueryOptionSeparator = fragment.IndexOf(JsonLightConstants.ContextUriQueryOptionSeparator);
            if (indexOfQueryOptionSeparator > 0)
            {
                // Extract the $select query option value from the fragment if it exists
                string fragmentQueryOptions = fragment.Substring(indexOfQueryOptionSeparator);
                this.parseResult.SelectQueryOption = ExtractSelectQueryOption(fragmentQueryOptions);

                fragment = fragment.Substring(0, indexOfQueryOptionSeparator);
            }

            string[] parts = fragment.Split(JsonLightConstants.ContextUriFragmentPartSeparator);
            int partCount = parts.Length;

            ODataPayloadKind detectedPayloadKind = ODataPayloadKind.Unsupported;
            EdmTypeResolver edmTypeResolver = new EdmTypeReaderResolver(this.model, readerBehavior, version);

            switch (partCount)
            {
                case 1:
                    // Service document: no fragment
                    if (fragment.Length == 0)
                    {
                        detectedPayloadKind = ODataPayloadKind.ServiceDocument;
                        break;
                    }

                    if (parts[0].Equals(JsonLightConstants.MetadataUriFragmentNull, StringComparison.OrdinalIgnoreCase))
                    {
                        detectedPayloadKind = ODataPayloadKind.Property;
                        this.parseResult.IsNullProperty = true;
                        break;
                    }

                    if (parts[0].Equals(ODataConstants.EntityReferenceCollectionSegmentName + "(" + ODataConstants.EntityReferenceSegmentName + ")"))
                    {
                        detectedPayloadKind = ODataPayloadKind.EntityReferenceLinks;
                        break;
                    }

                    if (parts[0].Equals(ODataConstants.EntityReferenceSegmentName))
                    {
                        detectedPayloadKind = ODataPayloadKind.EntityReferenceLink;
                        break;
                    }

                    IEdmEntitySet entitySet = this.model.ResolveEntitySet(parts[0]);
                    if (entitySet != null)
                    {
                        // Feed: {schema.entity-container.entity-set}
                        this.parseResult.EntitySet = entitySet;
                        this.parseResult.EdmType = edmTypeResolver.GetElementType(entitySet);
                        detectedPayloadKind = ODataPayloadKind.Feed;
                        break;
                    }

                    // Property: {schema.type} or Collection({schema.type}) where schema.type is primitive or complex.
                    this.parseResult.EdmType = this.ResolveType(parts[0], readerBehavior, version);
                        Debug.Assert(
                            this.parseResult.EdmType.TypeKind == EdmTypeKind.Primitive || this.parseResult.EdmType.TypeKind == EdmTypeKind.Complex || this.parseResult.EdmType.IsNonEntityCollectionType(),
                            "The first context URI segment must be a set or a non-entity type.");
                    detectedPayloadKind = this.parseResult.EdmType is IEdmCollectionType ? ODataPayloadKind.Collection : ODataPayloadKind.Property;

                    break;

                    case 2:
                        // Entry: {schema.entity-container.entity-set}/@Element
                        // Feed with type cast: {schema.entity-container.entity-set}/{type-cast}
                        detectedPayloadKind = this.ResolveEntitySet(
                            parts[0], 
                            (IEdmEntitySet resolvedEntitySet) =>
                            {
                                IEdmEntityType entitySetElementType = edmTypeResolver.GetElementType(resolvedEntitySet);

                                if (string.CompareOrdinal(JsonLightConstants.ContextUriFragmentItemSelector, parts[1]) == 0)
                                {
                                    this.parseResult.EdmType = entitySetElementType;
                                    return ODataPayloadKind.Entry;
                                }
                                else
                                {
                                    this.parseResult.EdmType = this.ResolveTypeCast(resolvedEntitySet, parts[1], readerBehavior, version, entitySetElementType);
                                    return ODataPayloadKind.Feed;
                                }
                            });
                        
                        break;

                    case 3:
                        detectedPayloadKind = this.ResolveEntitySet(
                            parts[0], 
                            (IEdmEntitySet resolvedEntitySet) =>
                            {
                                IEdmEntityType entitySetElementType = edmTypeResolver.GetElementType(resolvedEntitySet);

                                // Entry with type cast: {schema.entity-container.entity-set}/{type-cast}/@Element
                                this.parseResult.EdmType = this.ResolveTypeCast(resolvedEntitySet, parts[1], readerBehavior, version, entitySetElementType);
                                this.ValidateMetadataUriFragmentItemSelector(parts[2]);
                                return ODataPayloadKind.Entry;
                            });
                       
                        break;
                    default:
                        throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_FragmentWithInvalidNumberOfParts(UriUtilsCommon.UriToString(this.parseResult.ContextUri), partCount, 3));
                }

            return detectedPayloadKind;
        }

        /// <summary>
        /// Returns the parse results of the context uri if it has a AssociationLink in the uri
        /// </summary>
        /// <param name="edmTypeResolver">Edm Type Resolver to determine entityset type element.</param>
        /// <param name="partCount">Number of split parts the metadata fragment is split into.</param>
        /// <param name="parts">The  actual metadata fragment parts.</param>
        /// <param name="readerBehavior">The reader behavior.</param>
        /// <param name="version">The odata version.</param>
        /// <returns>Returns with an EntityReferenceLink or Links depending on the Uri, sets the parse results with the navigation, and set</returns>
        private ODataPayloadKind ParseAssociationLinks(EdmTypeResolver edmTypeResolver, int partCount, string[] parts, ODataReaderBehavior readerBehavior, ODataVersion version)
        {
            return this.ResolveEntitySet(
                parts[0], 
                (IEdmEntitySet resolvedEntitySet) =>
                {
                    ODataPayloadKind detectedPayloadKind = ODataPayloadKind.Unsupported;
                    IEdmNavigationProperty navigationProperty;
                    switch (partCount)
                    {
                        case 3:
                            if (string.CompareOrdinal(ODataConstants.EntityReferenceSegmentName, parts[1]) == 0)
                            {
                                // Entity reference links: {schema.entity-container.entity-set}/$links/{nav-property}
                                navigationProperty = this.ResolveEntityReferenceLinkMetadataFragment(edmTypeResolver, resolvedEntitySet, (string)null, parts[2], readerBehavior, version);
                                detectedPayloadKind = this.SetEntityLinkParseResults(navigationProperty, null);
                            }
                            else
                            {
                                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidAssociationLink(UriUtilsCommon.UriToString(this.parseResult.ContextUri)));
                            }

                            break;

                        case 4:
                            if (string.CompareOrdinal(ODataConstants.EntityReferenceSegmentName, parts[1]) == 0)
                            {
                                // Entry with property: {schema.entity-container.entity-set}/$links/{col-nav-property}/@Element
                                // Entry with property: {schema.entity-container.entity-set}/$links/{ref-nav-property}/@Element (invalid, will throw)
                                navigationProperty = this.ResolveEntityReferenceLinkMetadataFragment(edmTypeResolver, resolvedEntitySet, null, parts[2], readerBehavior, version);
                                this.ValidateLinkMetadataUriFragmentItemSelector(parts[3]);
                                detectedPayloadKind = this.SetEntityLinkParseResults(navigationProperty, parts[3]);
                            }
                            else if (string.CompareOrdinal(ODataConstants.EntityReferenceSegmentName, parts[2]) == 0)
                            {
                                // Entry with property: {schema.entity-container.entity-set}/type/$links/{colproperty}
                                navigationProperty = this.ResolveEntityReferenceLinkMetadataFragment(edmTypeResolver, resolvedEntitySet, parts[1], parts[3], readerBehavior, version);
                                detectedPayloadKind = this.SetEntityLinkParseResults(navigationProperty, null);
                            }
                            else
                            {
                                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidAssociationLink(UriUtilsCommon.UriToString(this.parseResult.ContextUri)));
                            }

                            break;

                        case 5:
                            if (string.CompareOrdinal(ODataConstants.EntityReferenceSegmentName, parts[2]) == 0)
                            {
                                // Entry with property: {schema.entity-container.entity-set}/type/$links/{navproperty}/@Element
                                navigationProperty = this.ResolveEntityReferenceLinkMetadataFragment(edmTypeResolver, resolvedEntitySet, parts[1], parts[3], readerBehavior, version);
                                this.ValidateLinkMetadataUriFragmentItemSelector(parts[2]);
                                detectedPayloadKind = this.SetEntityLinkParseResults(navigationProperty, parts[4]);
                            }
                            else
                            {
                                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidAssociationLink(UriUtilsCommon.UriToString(this.parseResult.ContextUri)));
                            }
                           
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidAssociationLink(UriUtilsCommon.UriToString(this.parseResult.ContextUri)));
                    }

                    return detectedPayloadKind;
                });
        }

        /// <summary>
        /// Set the EntityLinks Parse results.
        /// </summary>
        /// <param name="navigationProperty">Navigation property to add to the results.</param>
        /// <param name="singleElement">Single element string, used to confirm if this is an error case or not.</param>
        /// <returns>Returns ReferenceLink or Collection Link based on the navigation and at element</returns>
        private ODataPayloadKind SetEntityLinkParseResults(IEdmNavigationProperty navigationProperty, string singleElement)
        {
            ODataPayloadKind detectedPayloadKind  = ODataPayloadKind.Unsupported;
            this.parseResult.NavigationProperty = navigationProperty;
            
            detectedPayloadKind = navigationProperty.Type.IsCollection() ? ODataPayloadKind.EntityReferenceLinks : ODataPayloadKind.EntityReferenceLink;

            if (singleElement != null && string.CompareOrdinal(JsonLightConstants.ContextUriFragmentItemSelector, singleElement) == 0)
            {
                if (navigationProperty.Type.IsCollection())
                {
                    detectedPayloadKind = ODataPayloadKind.EntityReferenceLink;
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidSingletonNavPropertyForEntityReferenceLinkUri(UriUtilsCommon.UriToString(this.parseResult.ContextUri), navigationProperty.Name, singleElement));
                }
            }

            return detectedPayloadKind;
        }

        /// <summary>
        /// Parses the fragment of an entity reference link context URI.
        /// </summary>
        /// <param name="edmTypeResolver">Edm Type Resolver used to get the ElementType of the entity set.</param>
        /// <param name="entitySet">Entity Set used as a starting point to find the navigation property</param>
        /// <param name="typeName">The name of the type declaring the navigation property.</param>
        /// <param name="propertyName">The name of the navigation property.</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <returns>The resolved navigation property.</returns>
        private IEdmNavigationProperty ResolveEntityReferenceLinkMetadataFragment(EdmTypeResolver edmTypeResolver, IEdmEntitySet entitySet, string typeName, string propertyName, ODataReaderBehavior readerBehavior, ODataVersion version)
        {
            //// {entitySet}/$links/{nav-property}
            //// {entitySet}/$links/{nav-property}/@Element
            //// {entitySet}/typeName/$links/{nav-property}
            //// {entitySet}/typeName/$links/{nav-property}/@Element
            IEdmEntityType edmEntityType = edmTypeResolver.GetElementType(entitySet);

            if (typeName != null)
            {
                edmEntityType = this.ResolveTypeCast(entitySet, typeName, readerBehavior, version, edmEntityType);
            }

            IEdmNavigationProperty navigationProperty = this.ResolveNavigationProperty(edmEntityType, propertyName);

            return navigationProperty;
        }

        /// <summary>
        /// Validate the Metadata Uri Fragment is @Element for a $links metadata uri, will throw a $links specific error
        /// </summary>
        /// <param name="elementSelector">Element selector.</param>
        private void ValidateLinkMetadataUriFragmentItemSelector(string elementSelector)
        {
            if (string.CompareOrdinal(JsonLightConstants.ContextUriFragmentItemSelector, elementSelector) != 0)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidEntityReferenceLinkUriSuffix(UriUtilsCommon.UriToString(this.parseResult.ContextUri), elementSelector, JsonLightConstants.ContextUriFragmentItemSelector));
            }
        }

        /// <summary>
        /// Validate the Metadata Uri Fragment is @Element for a non $links metadata uri, throws if its not correct
        /// </summary>
        /// <param name="elementSelector">Element selector.</param>
        private void ValidateMetadataUriFragmentItemSelector(string elementSelector)
        {
            if (string.CompareOrdinal(JsonLightConstants.ContextUriFragmentItemSelector, elementSelector) != 0)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidEntityWithTypeCastUriSuffix(UriUtilsCommon.UriToString(this.parseResult.ContextUri), elementSelector, JsonLightConstants.ContextUriFragmentItemSelector));
            }
        }

        /// <summary>
        /// Resolves a navigation property name to an IEdmNavigationProperty.
        /// </summary>
        /// <param name="entityType">Entity Type to look for the navigation property on.</param>
        /// <param name="navigationPropertyName">Navigation property name to find.</param>
        /// <returns>Returns the navigation property of throws an exception if it cannot be found.</returns>
        private IEdmNavigationProperty ResolveNavigationProperty(IEdmEntityType entityType, string navigationPropertyName)
        {
            IEdmNavigationProperty navigationProperty = null;
            IEdmProperty property = entityType.FindProperty(navigationPropertyName);
            navigationProperty = property as IEdmNavigationProperty;

            if (navigationProperty == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidPropertyForEntityReferenceLinkUri(UriUtilsCommon.UriToString(this.parseResult.ContextUri), navigationPropertyName));
            }

            return navigationProperty;
        }

        /// <summary>
        /// Resolves the entity set.
        /// </summary>
        /// <param name="entitySetPart">The entity set part.</param>
        /// <param name="resolvedEntitySet">The resolved entity set.</param>
        /// <returns>Returns the OData Payload Kind</returns>
        private ODataPayloadKind ResolveEntitySet(string entitySetPart, Func<IEdmEntitySet, ODataPayloadKind> resolvedEntitySet)
        {
            IEdmEntitySet entitySet = this.model.ResolveEntitySet(entitySetPart);
            if (entitySet != null)
            {
                this.parseResult.EntitySet = entitySet;
                return resolvedEntitySet(entitySet);
            }

            throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidEntitySetName(UriUtilsCommon.UriToString(this.parseResult.ContextUri), entitySetPart));
        }

        /// <summary>
        /// Resolves an entity set with an optional type cast and updates the parse result.
        /// </summary>
        /// <param name="entitySet">The entity set to resolve the type cast against.</param>
        /// <param name="typeCast">The optional type cast.</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <param name="entitySetElementType">The type of the given entity set.</param>
        /// <returns>The resolved entity type.</returns>
        private IEdmEntityType ResolveTypeCast(IEdmEntitySet entitySet, string typeCast, ODataReaderBehavior readerBehavior, ODataVersion version, IEdmEntityType entitySetElementType)
        {
            Debug.Assert(entitySet != null, "entitySet != null");

            IEdmEntityType entityType = entitySetElementType;

            // Parse the type cast if it exists
            if (!string.IsNullOrEmpty(typeCast))
            {
                EdmTypeKind typeKind;
                entityType = MetadataUtils.ResolveTypeNameForRead(this.model, /*expectedType*/ null, typeCast, readerBehavior, version, out typeKind) as IEdmEntityType;
                if (entityType == null)
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidEntityTypeInTypeCast(UriUtilsCommon.UriToString(this.parseResult.ContextUri), typeCast));
                }

                // Validate that the entity type is assignable to the base type of the set
                if (!entitySetElementType.IsAssignableFrom(entityType))
                {
                    throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_IncompatibleEntityTypeInTypeCast(UriUtilsCommon.UriToString(this.parseResult.ContextUri), typeCast, entitySetElementType.FullName(), entitySet.FullName()));
                }
            }

            return entityType;
        }

        /// <summary>
        /// Resolves a type.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <returns>The resolved Edm type.</returns>
        private IEdmType ResolveType(string typeName, ODataReaderBehavior readerBehavior, ODataVersion version)
        {
            string typeNameToResolve = EdmLibraryExtensions.GetCollectionItemTypeName(typeName) ?? typeName;

            EdmTypeKind typeKind;
            IEdmType resolvedType = MetadataUtils.ResolveTypeNameForRead(this.model, /*expectedType*/ null, typeNameToResolve, readerBehavior, version, out typeKind);
            if (resolvedType == null || resolvedType.TypeKind != EdmTypeKind.Primitive && resolvedType.TypeKind != EdmTypeKind.Complex)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName(UriUtilsCommon.UriToString(this.parseResult.ContextUri), typeName));
            }

            resolvedType = typeNameToResolve == typeName ? resolvedType : EdmLibraryExtensions.GetCollectionType(resolvedType.ToTypeReference(true /*nullable*/));
            return resolvedType;
        }
    }
}
