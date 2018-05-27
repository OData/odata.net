//---------------------------------------------------------------------
// <copyright file="ODataContextUrlInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.UriParser.Aggregation;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class representing required information for context URL.
    /// </summary>
    internal sealed class ODataContextUrlInfo
    {
        /// <summary>
        /// Default constructor for <see cref="ODataContextUrlInfo"/>
        /// </summary>
        private ODataContextUrlInfo()
        {
            DeltaKind = ODataDeltaKind.None;
        }

        /// <summary>The delta kind used for building context Url</summary>
        internal ODataDeltaKind DeltaKind { get; private set; }

        /// <summary>Whether target is unknown entity set</summary>
        internal bool IsUnknownEntitySet { get; private set; }

        /// <summary>
        /// Whether the current target has a navigation source or has a meanful the navigation source kind
        /// e.g. When writing a Resource or Resource Set for Complex type.
        ///      EdmNavigationSourceKind.None means the target doesn't have a navigation source
        /// </summary>
        internal bool HasNavigationSourceInfo { get; private set; }

        /// <summary>Name of navigation path used for building context Url</summary>
        internal string NavigationPath { get; private set; }

        /// <summary>Name of the navigation source used for building context Url</summary>
        internal string NavigationSource { get; private set; }

        /// <summary>ResourcePath used for building context Url</summary>
        internal string ResourcePath { get; private set; }

        /// <summary>if the context url represents an undeclared property</summary>
        internal bool? IsUndeclared { get; private set; }

        /// <summary>Query clause used for building context Url</summary>
        internal string QueryClause { get; private set; }

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
        /// <param name="version">OData Version.</param>
        /// <param name="odataUri">The odata uri info for current query.</param>
        /// <param name="model">The model used to handle unsigned int conversions.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(ODataValue value, ODataVersion version, ODataUri odataUri = null, IEdmModel model = null)
        {
            return new ODataContextUrlInfo()
            {
                TypeName = GetTypeNameForValue(value, model),
                ResourcePath = ComputeResourcePath(odataUri),
                QueryClause = ComputeQueryClause(odataUri, version),
                IsUndeclared = ComputeIfIsUndeclared(odataUri)
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
        /// <param name="version">The OData Version of the response.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(IEdmNavigationSource navigationSource, string expectedEntityTypeName, bool isSingle, ODataUri odataUri, ODataVersion version)
        {
            EdmNavigationSourceKind kind = navigationSource.NavigationSourceKind();
            string navigationSourceEntityType = navigationSource.EntityType().FullName();
            return new ODataContextUrlInfo()
            {
                IsUnknownEntitySet = kind == EdmNavigationSourceKind.UnknownEntitySet,
                NavigationSource = navigationSource.Name,
                TypeCast = navigationSourceEntityType == expectedEntityTypeName ? null : expectedEntityTypeName,
                TypeName = navigationSourceEntityType,
                IncludeFragmentItemSelector = isSingle && kind != EdmNavigationSourceKind.Singleton,
                NavigationPath = ComputeNavigationPath(kind, odataUri, navigationSource.Name),
                ResourcePath = ComputeResourcePath(odataUri),
                QueryClause = ComputeQueryClause(odataUri, version),
                IsUndeclared = ComputeIfIsUndeclared(odataUri)
            };
        }

        /// <summary>
        /// Create ODataContextUrlInfo from ODataResourceTypeContext
        /// </summary>
        /// <param name="typeContext">The ODataResourceTypeContext to be used.</param>
        /// <param name="version">The OData Version of the response</param>
        /// <param name="isSingle">Whether target is single item.</param>
        /// <param name="odataUri">The odata uri info for current query.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(ODataResourceTypeContext typeContext, ODataVersion version, bool isSingle, ODataUri odataUri = null)
        {
            Debug.Assert(typeContext != null, "typeContext != null");

            var hasNavigationSourceInfo = typeContext.NavigationSourceKind != EdmNavigationSourceKind.None
                || !string.IsNullOrEmpty(typeContext.NavigationSourceName);

            var typeName = hasNavigationSourceInfo
                           ? typeContext.NavigationSourceFullTypeName
                           : typeContext.ExpectedResourceTypeName == null
                             ? null
                             : isSingle
                               ? typeContext.ExpectedResourceTypeName
                               : EdmLibraryExtensions.GetCollectionTypeName(typeContext.ExpectedResourceTypeName);

            return new ODataContextUrlInfo()
            {
                HasNavigationSourceInfo = hasNavigationSourceInfo,
                IsUnknownEntitySet = typeContext.NavigationSourceKind == EdmNavigationSourceKind.UnknownEntitySet,
                NavigationSource = typeContext.NavigationSourceName,
                TypeCast = typeContext.NavigationSourceEntityTypeName == null
                           || typeContext.ExpectedResourceTypeName == null
                           || typeContext.ExpectedResourceType is IEdmComplexType
                           || typeContext.NavigationSourceEntityTypeName == typeContext.ExpectedResourceTypeName
                           ? null : typeContext.ExpectedResourceTypeName,
                TypeName = typeName,
                IncludeFragmentItemSelector = isSingle && typeContext.NavigationSourceKind != EdmNavigationSourceKind.Singleton,
                NavigationPath = ComputeNavigationPath(typeContext.NavigationSourceKind, odataUri, typeContext.NavigationSourceName),
                ResourcePath = ComputeResourcePath(odataUri),
                QueryClause = ComputeQueryClause(odataUri, version),
                IsUndeclared = ComputeIfIsUndeclared(odataUri)
            };
        }

        /// <summary>
        /// Create contextUrlInfo for delta
        /// </summary>
        /// <param name="typeContext">The ODataResourceTypeContext to be used.</param>
        /// <param name="version">The OData version of the response.</param>
        /// <param name="kind">The delta kind.</param>
        /// <param name="odataUri">The odata uri info for current query.</param>
        /// <returns>The generated ODataContextUrlInfo.</returns>
        internal static ODataContextUrlInfo Create(ODataResourceTypeContext typeContext, ODataVersion version, ODataDeltaKind kind, ODataUri odataUri = null)
        {
            Debug.Assert(typeContext != null, "typeContext != null");

            ODataContextUrlInfo contextUriInfo = new ODataContextUrlInfo()
            {
                IsUnknownEntitySet = typeContext.NavigationSourceKind == EdmNavigationSourceKind.UnknownEntitySet,
                NavigationSource = typeContext.NavigationSourceName,
                TypeCast = typeContext.NavigationSourceEntityTypeName == typeContext.ExpectedResourceTypeName ? null : typeContext.ExpectedResourceTypeName,
                TypeName = typeContext.NavigationSourceEntityTypeName,
                IncludeFragmentItemSelector = kind == ODataDeltaKind.Resource && typeContext.NavigationSourceKind != EdmNavigationSourceKind.Singleton,
                DeltaKind = kind,
                NavigationPath = ComputeNavigationPath(typeContext.NavigationSourceKind, null, typeContext.NavigationSourceName),
            };

            // Only use odata uri in with model case.
            if (typeContext is ODataResourceTypeContext.ODataResourceTypeContextWithModel)
            {
                contextUriInfo.NavigationPath = ComputeNavigationPath(typeContext.NavigationSourceKind, odataUri,
                    typeContext.NavigationSourceName);
                contextUriInfo.ResourcePath = ComputeResourcePath(odataUri);
                contextUriInfo.QueryClause = ComputeQueryClause(odataUri, version);
                contextUriInfo.IsUndeclared = ComputeIfIsUndeclared(odataUri);
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
                parentContextUrlInfo.DeltaKind == ODataDeltaKind.ResourceSet &&
                this.DeltaKind == ODataDeltaKind.Resource)
            {
                return true;
            }

            return false;
        }

        private static string ComputeNavigationPath(EdmNavigationSourceKind kind, ODataUri odataUri, string navigationSource)
        {
            bool isContained = kind == EdmNavigationSourceKind.ContainedEntitySet;
            bool isUnknownEntitySet = kind == EdmNavigationSourceKind.UnknownEntitySet;

            if (isUnknownEntitySet)
            {
                // If the navigation target is not specified, i.e., UnknownEntitySet,
                // the navigation path should be null so that type name will be used
                // to build the context Url.
                return null;
            }

            string navigationPath = null;
            if (isContained && odataUri != null && odataUri.Path != null)
            {
                ODataPath odataPath = odataUri.Path.TrimEndingTypeSegment().TrimEndingKeySegment();
                if (!(odataPath.LastSegment is NavigationPropertySegment) && !(odataPath.LastSegment is OperationSegment))
                {
                    throw new ODataException(Strings.ODataContextUriBuilder_ODataPathInvalidForContainedElement(odataPath.ToContextUrlPathString()));
                }

                navigationPath = odataPath.ToContextUrlPathString();
            }

            return navigationPath ?? navigationSource;
        }

        private static string ComputeResourcePath(ODataUri odataUri)
        {
            if (odataUri != null && odataUri.Path != null && odataUri.Path.IsIndividualProperty())
            {
                return odataUri.Path.ToContextUrlPathString();
            }

            return string.Empty;
        }

        private static string ComputeQueryClause(ODataUri odataUri, ODataVersion version)
        {
            if (odataUri != null)
            {
                // TODO: Figure out how to deal with $select after $apply
                if (odataUri.Apply != null)
                {
                    return CreateApplyUriSegment(odataUri.Apply);
                }
                else
                {
                    return CreateSelectExpandContextUriSegment(odataUri.SelectAndExpand, version);
                }
            }

            return null;
        }

        private static bool? ComputeIfIsUndeclared(ODataUri odataUri)
        {
            if (odataUri != null && odataUri.Path != null)
            {
                return odataUri.Path.IsUndeclared();
            }

            return null;
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

            if (value.TypeAnnotation != null && !string.IsNullOrEmpty(value.TypeAnnotation.TypeName))
            {
                return value.TypeAnnotation.TypeName;
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
                throw new ODataException(Strings.ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource);
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
        /// <param name="version">OData Version of the response</param>
        /// <returns>the select and expand segment for context url in this level.</returns>
        private static string CreateSelectExpandContextUriSegment(SelectExpandClause selectExpandClause, ODataVersion version)
        {
            if (selectExpandClause != null)
            {
                string contextUri;
                selectExpandClause.Traverse(ProcessSubExpand, CombineSelectAndExpandResult, version, out contextUri);
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
        /// <param name="version">OData Version of the generated response.</param>
        /// <returns>The generated expand string.</returns>
        private static string ProcessSubExpand(string expandNode, string subExpand, ODataVersion version)
        {
            return string.IsNullOrEmpty(subExpand) && version <= ODataVersion.V4 ? null :
                expandNode + ODataConstants.ContextUriProjectionStart + subExpand + ODataConstants.ContextUriProjectionEnd;
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
                // https://github.com/OData/odata.net/issues/1104
                // If the user explicitly selects and expands a nav prop, we should include both forms in contextUrl
                // We can't, though, because SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary adds all of
                // the expanded items to the select before it gets here, so we can't tell what is explicitly selected by the user.
                foreach (var item in expandList)
                {
                    string expandNode = item.Substring(0, item.IndexOf(ODataConstants.ContextUriProjectionStart));
                    selectList.Remove(expandNode);
                }

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
