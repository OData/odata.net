//---------------------------------------------------------------------
// <copyright file="ODataContextUrlInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Extensions.Semantic;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class representing required infomation for context URL.
    /// </summary>
    internal sealed class ODataContextUrlInfo
    {
        /// <summary>Whether target is contained</summary>
        private bool isContained;

        /// <summary>The navigation source for current target</summary>
        private string navigationSource;

        /// <summary>ODataUri information for context Url</summary>
        private ODataUri odataUri;

        /// <summary>
        /// Default constructor for <see cref="ODataContextUrlInfo"/>
        /// </summary>
        private ODataContextUrlInfo()
        {
            DeltaKind = ODataDeltaKind.None;
        }

        /// <summary>The delta kind used for building context Url</summary>
        internal ODataDeltaKind DeltaKind { get; set; }

        /// <summary>Whether target is unknown entity set</summary>
        internal bool IsUnknownEntitySet { get; set; }

        /// <summary>Name of navigation path used for building context Url</summary>
        internal string NavigationPath
        {
            get
            {
                if (this.IsUnknownEntitySet)
                {
                    // If the navigation target is not specified, i.e., UnknownEntitySet,
                    // the navigation path should be null so that type name will be used
                    // to build the context Url.
                    return null;
                }

                string navigationPath = null;
                if (this.isContained && this.odataUri != null && this.odataUri.Path != null)
                {
                    ODataPath odataPath = this.odataUri.Path.TrimEndingTypeSegment().TrimEndingKeySegment();
                    if (!(odataPath.LastSegment is NavigationPropertySegment))
                    {
                        throw new ODataException(Strings.ODataContextUriBuilder_ODataPathInvalidForContainedElement(odataPath.ToContextUrlPathString()));
                    }

                    navigationPath = odataPath.ToContextUrlPathString();
                }

                return navigationPath ?? this.navigationSource;
            }
        }

        /// <summary>Name of the navigation source used for building context Url</summary>
        internal string NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>ResourcePath used for building context Url</summary>
        internal string ResourcePath
        {
            get
            {
                if (this.odataUri != null && this.odataUri.Path != null && this.odataUri.Path.IsIndividualProperty())
                {
                    return this.odataUri.Path.ToContextUrlPathString();
                }

                return string.Empty;
            }
        }

        /// <summary>Query clause used for building context Url</summary>
        internal string QueryClause
        {
            get
            {
                if (this.odataUri != null)
                {
                    // TODO: Figure out how to deal with $select after $apply
                    if (this.odataUri.Apply != null)
                    {
                        return CreateApplyUriSegment(this.odataUri.Apply);
                    }
                    else
                    {
                        return CreateSelectExpandContextUriSegment(this.odataUri.SelectAndExpand);
                    }
                }

                return null;
            }
        }

        /// <summary>Entity type name used for building context Url</summary>
        internal string TypeName { get; private set; }

        /// <summary>TypeCast segment used for building context Url</summary>
        internal string TypeCast { get; private set; }

        /// <summary>Whether context Url is for single item used for building context Url</summary>
        internal bool IncludeFragmentItemSelector { get; private set; }

        /// <summary>
        /// Create ODataContextUrlInfo for OdataValue.
        /// </summary>
        /// <param name="value">The ODataValue to be used.</param>
        /// <param name="odataUri">The odata uri info for current query.</param>
        /// <param name="model">The model used to handle unsigned int conversions.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(ODataValue value, ODataUri odataUri = null, IEdmModel model = null)
        {
            return new ODataContextUrlInfo()
            {
                TypeName = GetTypeNameForValue(value, model),
                odataUri = odataUri
            };
        }

        /// <summary>
        /// Create ODataContextUrlInfo from ODataCollectionStartSerializationInfo
        /// </summary>
        /// <param name="info">The ODataCollectionStartSerializationInfo to be used.</param>
        /// <param name="itemTypeReference">ItemTypeReference specifying element type.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(ODataCollectionStartSerializationInfo info, IEdmTypeReference itemTypeReference)
        {
            string collectionTypeName = null;
            if (info != null)
            {
                collectionTypeName = info.CollectionTypeName;
            }
            else if (itemTypeReference != null)
            {
                collectionTypeName = EdmLibraryExtensions.GetCollectionTypeName(itemTypeReference.FullName());
            }

            return new ODataContextUrlInfo()
            {
                TypeName = collectionTypeName,
            };
        }

        /// <summary>
        /// Create ODataContextUrlInfo from basic information
        /// </summary>
        /// <param name="navigationSource">Navigation source for current element.</param>\
        /// <param name="expectedEntityTypeName">The expectedEntity for current element.</param>
        /// <param name="isSingle">Whether target is single item.</param>
        /// <param name="odataUri">The odata uri info for current query.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(IEdmNavigationSource navigationSource, string expectedEntityTypeName, bool isSingle, ODataUri odataUri)
        {
            EdmNavigationSourceKind kind = navigationSource.NavigationSourceKind();
            string navigationSourceEntityType = navigationSource.EntityType().FullName();
            return new ODataContextUrlInfo()
            {
                isContained = kind == EdmNavigationSourceKind.ContainedEntitySet,
                IsUnknownEntitySet = kind == EdmNavigationSourceKind.UnknownEntitySet,
                navigationSource = navigationSource.Name,
                TypeCast = navigationSourceEntityType == expectedEntityTypeName ? null : expectedEntityTypeName,
                TypeName = navigationSourceEntityType,
                IncludeFragmentItemSelector = isSingle && kind != EdmNavigationSourceKind.Singleton,
                odataUri = odataUri
            };
        }

        /// <summary>
        /// Create ODataContextUrlInfo from ODataFeedAndEntryTypeContext
        /// </summary>
        /// <param name="typeContext">The ODataFeedAndEntryTypeContext to be used.</param>
        /// <param name="isSingle">Whether target is single item.</param>
        /// <param name="odataUri">The odata uri info for current query.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(ODataFeedAndEntryTypeContext typeContext, bool isSingle, ODataUri odataUri = null)
        {
            Debug.Assert(typeContext != null, "typeContext != null");

            return new ODataContextUrlInfo()
                {
                    isContained = typeContext.NavigationSourceKind == EdmNavigationSourceKind.ContainedEntitySet,
                    IsUnknownEntitySet = typeContext.NavigationSourceKind == EdmNavigationSourceKind.UnknownEntitySet,
                    navigationSource = typeContext.NavigationSourceName,
                    TypeCast = typeContext.NavigationSourceEntityTypeName == typeContext.ExpectedEntityTypeName ? null : typeContext.ExpectedEntityTypeName,
                    TypeName = typeContext.NavigationSourceFullTypeName,
                    IncludeFragmentItemSelector = isSingle && typeContext.NavigationSourceKind != EdmNavigationSourceKind.Singleton,
                    odataUri = odataUri
                };
        }

        /// <summary>
        /// Create contextUrlInfo for delta
        /// </summary>
        /// <param name="typeContext">The ODataFeedAndEntryTypeContext to be used.</param>
        /// <param name="kind">The delta kind.</param>
        /// <param name="odataUri">The odata uri info for current query.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(ODataFeedAndEntryTypeContext typeContext, ODataDeltaKind kind, ODataUri odataUri = null)
        {
            Debug.Assert(typeContext != null, "typeContext != null");

            ODataContextUrlInfo contextUriInfo = new ODataContextUrlInfo()
            {
                isContained = typeContext.NavigationSourceKind == EdmNavigationSourceKind.ContainedEntitySet,
                IsUnknownEntitySet = typeContext.NavigationSourceKind == EdmNavigationSourceKind.UnknownEntitySet,
                navigationSource = typeContext.NavigationSourceName,
                TypeCast = typeContext.NavigationSourceEntityTypeName == typeContext.ExpectedEntityTypeName ? null : typeContext.ExpectedEntityTypeName,
                TypeName = typeContext.NavigationSourceEntityTypeName,
                IncludeFragmentItemSelector = kind == ODataDeltaKind.Entry && typeContext.NavigationSourceKind != EdmNavigationSourceKind.Singleton,
                DeltaKind = kind,
            };

            // Only use odata uri in with model case.
            if (typeContext is ODataFeedAndEntryTypeContext.ODataFeedAndEntryTypeContextWithModel)
            {
                contextUriInfo.odataUri = odataUri;
            }

            return contextUriInfo;
        }

        /// <summary>
        /// Determine whether current contextUrlInfo could be determined from parent contextUrlInfo.
        /// </summary>
        /// <param name="parentContextUrlInfo">The parent contextUrlInfo.</param>
        /// <returns>Whether current contextUrlInfo could be determined from parent contextUrlInfo.</returns>
        internal bool IsHiddenBy(ODataContextUrlInfo parentContextUrlInfo)
        {
            if (parentContextUrlInfo == null)
            {
                return false;
            }

            if (parentContextUrlInfo.NavigationPath == NavigationPath &&
                parentContextUrlInfo.DeltaKind == ODataDeltaKind.Feed &&
                this.DeltaKind == ODataDeltaKind.Entry)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the type name based on the given odata value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="model">The model used to handle unsigned int conversions.</param>
        /// <returns>The type name for the context URI.</returns>
        private static string GetTypeNameForValue(ODataValue value, IEdmModel model)
        {
            if (value == null)
            {
                return null;
            }

            // special identifier for null values.
            if (value.IsNullValue)
            {
                return ODataConstants.ContextUriFragmentNull;
            }

            var typeAnnotation = value.GetAnnotation<SerializationTypeNameAnnotation>();
            if (typeAnnotation != null && !string.IsNullOrEmpty(typeAnnotation.TypeName))
            {
                return typeAnnotation.TypeName;
            }

            var complexValue = value as ODataComplexValue;
            if (complexValue != null)
            {
                return complexValue.TypeName;
            }

            var collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                return EdmLibraryExtensions.GetCollectionTypeFullName(collectionValue.TypeName);
            }

            var enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                return enumValue.TypeName;
            }

            var untypedValue = value as ODataUntypedValue;
            if (untypedValue != null)
            {
                return ODataConstants.ContextUriFragmentUntyped;
            }

            ODataPrimitiveValue primitive = value as ODataPrimitiveValue;
            if (primitive == null)
            {
                Debug.Assert(value is ODataStreamReferenceValue, "value is ODataStreamReferenceValue");
                throw new ODataException(Strings.ODataContextUriBuilder_StreamValueMustBePropertiesOfODataEntry);
            }

            // Try convert to underlying type if the primitive value is unsigned int.
            IEdmTypeDefinitionReference typeDefinitionReference = model.ResolveUIntTypeDefinition(primitive.Value);
            if (typeDefinitionReference != null)
            {
                return typeDefinitionReference.FullName();
            }

            IEdmPrimitiveTypeReference primitiveValueTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(primitive.Value.GetType());
            return primitiveValueTypeReference == null ? null : primitiveValueTypeReference.FullName();
        }

        private static string CreateApplyUriSegment(ApplyClause applyClause)
        {
            if (applyClause != null)
            {
                return applyClause.GetContextUri();
            }

            return string.Empty;
        }

        #region SelectAndExpand Convert
        /// <summary>
        /// Build the expand clause for a given level in the selectExpandClause
        /// </summary>
        /// <param name="selectExpandClause">the current level select expand clause</param>
        /// <returns>the select and expand segment for context url in this level.</returns>
        private static string CreateSelectExpandContextUriSegment(SelectExpandClause selectExpandClause)
        {
            if (selectExpandClause != null)
            {
                string contextUri;
                selectExpandClause.Traverse(ProcessSubExpand, CombineSelectAndExpandResult, out contextUri);
                if (!string.IsNullOrEmpty(contextUri))
                {
                    return ODataConstants.ContextUriProjectionStart + contextUri + ODataConstants.ContextUriProjectionEnd;
                }
            }

            return string.Empty;
        }

        /// <summary>Process sub expand node, contact with subexpand result</summary>
        /// <param name="expandNode">The current expanded node.</param>
        /// <param name="subExpand">Generated sub expand node.</param>
        /// <returns>The generated expand string.</returns>
        private static string ProcessSubExpand(string expandNode, string subExpand)
        {
            return string.IsNullOrEmpty(subExpand) ? null : expandNode + ODataConstants.ContextUriProjectionStart + subExpand + ODataConstants.ContextUriProjectionEnd;
        }

        /// <summary>Create combined result string using selected items list and expand items list.</summary>
        /// <param name="selectList">A list of selected item names.</param>
        /// <param name="expandList">A list of sub expanded item names.</param>
        /// <returns>The generated expand string.</returns>
        private static string CombineSelectAndExpandResult(IList<string> selectList, IList<string> expandList)
        {
            string currentExpandClause = string.Empty;
            if (selectList.Any())
            {
                currentExpandClause += String.Join(ODataConstants.ContextUriProjectionPropertySeparator, selectList.ToArray());
            }

            if (expandList.Any())
            {
                if (!string.IsNullOrEmpty(currentExpandClause))
                {
                    currentExpandClause += ODataConstants.ContextUriProjectionPropertySeparator;
                }

                currentExpandClause += String.Join(ODataConstants.ContextUriProjectionPropertySeparator, expandList.ToArray());
            }

            return currentExpandClause;
        }
        #endregion SelectAndExpand Convert
    }
}
