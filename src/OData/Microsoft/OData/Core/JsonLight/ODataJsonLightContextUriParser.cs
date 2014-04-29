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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    using UriUtils = Microsoft.OData.Core.UriUtils;

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
        /// <param name="version">The OData version to use for determining the set of built-in functions available.</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="needParseFragment">Whether the fragment after $metadata should be parsed, if set to false, only MetadataDocumentUri is parsed.</param>
        /// <returns>The result from parsing the context URI.</returns>
        internal static ODataJsonLightContextUriParseResult Parse(
            IEdmModel model,
            string contextUriFromPayload,
            ODataPayloadKind payloadKind,
            ODataVersion version,
            ODataReaderBehavior readerBehavior,
            bool needParseFragment)
        {
            if (contextUriFromPayload == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_NullMetadataDocumentUri);
            }

            // Create an absolute URI from the payload string
            Uri contextUri = new Uri(contextUriFromPayload, UriKind.Absolute);
            ODataJsonLightContextUriParser parser = new ODataJsonLightContextUriParser(model, contextUri);

            parser.TokenizeContextUri();
            if (needParseFragment)
            {
                parser.ParseContextUri(payloadKind, readerBehavior, version);
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
            else if (detectedPayloadKind == ODataPayloadKind.Entry)
            {
                this.parseResult.DetectedPayloadKinds = new[] { ODataPayloadKind.Entry, ODataPayloadKind.Delta };
                if (expectedPayloadKind == ODataPayloadKind.Delta)
                {
                    this.parseResult.DeltaKind = ODataDeltaKind.Entry;
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
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind(UriUtils.UriToString(this.parseResult.ContextUri), expectedPayloadKind.ToString()));
            }

            // NOTE: we interpret an empty select query option to mean that nothing should be projected
            //       (whereas a missing select query option means everything should be projected).
            string selectQueryOption = this.parseResult.SelectQueryOption;
            if (selectQueryOption != null)
            {
                if (detectedPayloadKind != ODataPayloadKind.Feed && detectedPayloadKind != ODataPayloadKind.Entry && detectedPayloadKind != ODataPayloadKind.Delta)
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
            bool hasItemSelector = false;
            ODataDeltaKind kind = ODataDeltaKind.None;

            // Deal with /$entity
            if (fragment.EndsWith(ODataConstants.ContextUriFragmentItemSelector, StringComparison.Ordinal))
            {
                hasItemSelector = true;
                fragment = fragment.Substring(0, fragment.Length - ODataConstants.ContextUriFragmentItemSelector.Length);
            }
            else if (fragment.EndsWith(ODataConstants.ContextUriDeltaFeed, StringComparison.Ordinal))
            {
                kind = ODataDeltaKind.Feed;
                fragment = fragment.Substring(0, fragment.Length - ODataConstants.ContextUriDeltaFeed.Length);
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
            EdmTypeResolver edmTypeResolver = new EdmTypeReaderResolver(this.model, readerBehavior, version);

            if (!fragment.Contains(ODataConstants.UriSegmentSeparator) && !hasItemSelector && kind == ODataDeltaKind.None)
            {
                // Service document: no fragment
                if (fragment.Length == 0)
                {
                    detectedPayloadKind = ODataPayloadKind.ServiceDocument;
                }
                else if (fragment.Equals(ODataConstants.ContextUriFragmentNull, StringComparison.OrdinalIgnoreCase))
                {
                    detectedPayloadKind = ODataPayloadKind.Property;
                    this.parseResult.IsNullProperty = true;
                }
                else if (fragment.Equals(ODataConstants.EntityReferenceCollectionSegmentName + "(" + ODataConstants.EntityReferenceSegmentName + ")"))
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
                        // Feed: {schema.entity-container.entity-set} or Singleton: {schema.entity-container.singleton}
                        this.parseResult.NavigationSource = foundNavigationSource;
                        this.parseResult.EdmType = edmTypeResolver.GetElementType(foundNavigationSource);
                        detectedPayloadKind = foundNavigationSource is IEdmSingleton ? ODataPayloadKind.Entry : ODataPayloadKind.Feed;
                    }
                    else
                    {
                        // Property: {schema.type} or Collection({schema.type}) where schema.type is primitive or complex.
                        detectedPayloadKind = this.ResolveType(fragment, readerBehavior, version);
                        Debug.Assert(
                            this.parseResult.EdmType.TypeKind == EdmTypeKind.Primitive || this.parseResult.EdmType.TypeKind == EdmTypeKind.Enum || this.parseResult.EdmType.TypeKind == EdmTypeKind.Complex || this.parseResult.EdmType.TypeKind == EdmTypeKind.Collection || this.parseResult.EdmType.TypeKind == EdmTypeKind.Entity,
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
                        detectedPayloadKind = hasItemSelector ? ODataPayloadKind.Entry : ODataPayloadKind.Feed;
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
                    detectedPayloadKind = ODataPayloadKind.Entry;
                }
                else if (path.IsIndividualProperty())
                {
                    detectedPayloadKind = ODataPayloadKind.Property;

                    IEdmCollectionType collectionType = parseResult.EdmType as IEdmCollectionType;
                    if (collectionType != null)
                    {
                        detectedPayloadKind = ODataPayloadKind.Collection;
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
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <returns>The resolved Edm type.</returns>
        private ODataPayloadKind ResolveType(string typeName, ODataReaderBehavior readerBehavior, ODataVersion version)
        {
            string typeNameToResolve = EdmLibraryExtensions.GetCollectionItemTypeName(typeName) ?? typeName;
            bool isCollection = typeNameToResolve != typeName;

            EdmTypeKind typeKind;
            IEdmType resolvedType = MetadataUtils.ResolveTypeNameForRead(this.model, /*expectedType*/ null, typeNameToResolve, readerBehavior, version, out typeKind);
            if (resolvedType == null || resolvedType.TypeKind != EdmTypeKind.Primitive && resolvedType.TypeKind != EdmTypeKind.Enum && resolvedType.TypeKind != EdmTypeKind.Complex && resolvedType.TypeKind != EdmTypeKind.Entity)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName(UriUtils.UriToString(this.parseResult.ContextUri), typeName));
            }

            if (resolvedType.TypeKind == EdmTypeKind.Entity)
            {
                this.parseResult.EdmType = resolvedType;
                return isCollection ? ODataPayloadKind.Feed : ODataPayloadKind.Entry;
            }

            resolvedType = isCollection ? EdmLibraryExtensions.GetCollectionType(resolvedType.ToTypeReference(true /*nullable*/)) : resolvedType;
            this.parseResult.EdmType = resolvedType;
            return isCollection ? ODataPayloadKind.Collection : ODataPayloadKind.Property;
        }
    }
}
