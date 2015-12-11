﻿//---------------------------------------------------------------------
// <copyright file="UriEdmHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Metadata
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// Class to provide methods that wrap EdmLib calls that are normally not allows in ODataLib, but
    /// are OK in the Uri Parser. These are OK to suppress because the Uri Parser
    /// does not need to go through the behavior knob that the ODL reader/writer does.
    /// This should only be used by the Uri Parser.
    /// </summary>
    internal static class UriEdmHelpers
    {
        /// <summary>
        /// Wraps a call to IEdmModel.FindType.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the type to find within the model.</param>
        /// <param name="resolver">Resolver for this func.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static IEdmSchemaType FindTypeFromModel(IEdmModel model, string qualifiedName, ODataUriResolver resolver)
        {
            if (resolver == null)
            {
                resolver = ODataUriResolver.Default;
            }

            return resolver.ResolveType(model, qualifiedName);
        }

        /// <summary>
        /// Wraps call to FindTypeFromModel for an Enum type.
        /// </summary>
        /// <param name="model">the model to search</param>
        /// <param name="qualifiedName">the name to find within the model</param>
        /// <returns>a type reference to the enum type, or null if no such type exists.</returns>
        public static IEdmEnumType FindEnumTypeFromModel(IEdmModel model, string qualifiedName)
        {
            IEdmEnumType enumType = FindTypeFromModel(model, qualifiedName, ODataUriResolver.Default) as IEdmEnumType;
            return enumType;
        }

        /// <summary>
        /// Check whether the parent and child are properly related types
        /// </summary>
        /// <param name="parentType">the parent type</param>
        /// <param name="childType">the child type</param>
        /// <exception cref="ODataException">Throws if the two types are not related.</exception>
        public static void CheckRelatedTo(IEdmType parentType, IEdmType childType)
        {
            if (!IsRelatedTo(parentType, childType))
            {
                // If the parentType is an open property, parentType will be null and can't have an ODataFullName.
                string parentTypeName = (parentType != null) ? parentType.FullTypeName() : "<null>";
                throw new ODataException(OData.Core.Strings.MetadataBinder_HierarchyNotFollowed(childType.FullTypeName(), parentTypeName));
            }
        }

        /// <summary>
        /// Check whether the two are properly related types
        /// </summary>
        /// <param name="first">the first type</param>
        /// <param name="second">the second type</param>
        /// <returns>Whether the two types are related.</returns>
        public static bool IsRelatedTo(IEdmType first, IEdmType second)
        {
            return second.IsOrInheritsFrom(first) || first.IsOrInheritsFrom(second);
        }

        /// <summary>
        /// Follow an ODataPath from an Expand to get the Final Nav Prop
        /// </summary>
        /// <param name="path">the path to follow</param>
        /// <returns>the navigation property at the end of that path.</returns>
        /// <exception cref="ODataException">Throws if the last segment in the path is not a nav prop.</exception>
        public static IEdmNavigationProperty GetNavigationPropertyFromExpandPath(ODataPath path)
        {
            NavigationPropertySegment navPropSegment = null;
            foreach (ODataPathSegment currentSegment in path)
            {
                TypeSegment typeSegment = currentSegment as TypeSegment;
                navPropSegment = currentSegment as NavigationPropertySegment;
                if (typeSegment == null && navPropSegment == null)
                {
                    throw new ODataException(OData.Core.Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
                }
            }

            if (navPropSegment == null)
            {
                throw new ODataException(OData.Core.Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
            }
            else
            {
                return navPropSegment.NavigationProperty;
            }
        }

        /// <summary>
        /// Follow an ODataPath from to get the most derived type
        /// </summary>
        /// <param name="path">the path to follow</param>
        /// <param name="startingType">the starting type before beginning to walk the path.</param>
        /// <returns>the most derived type in the path.</returns>
        public static IEdmType GetMostDerivedTypeFromPath(ODataPath path, IEdmType startingType)
        {
            IEdmType currentType = startingType;
            foreach (ODataPathSegment currentSegment in path)
            {
                TypeSegment typeSegment = currentSegment as TypeSegment;
                if (typeSegment != null && typeSegment.EdmType.IsOrInheritsFrom(currentType))
                {
                    currentType = typeSegment.EdmType;
                }
            }

            return currentType;
        }

        /// <summary>
        /// Returns true if this type is an EntityCollection
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>true if the type is an entity collection</returns>
        public static bool IsEntityCollection(this IEdmTypeReference type)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");
            IEdmCollectionTypeReference collectionType = type as IEdmCollectionTypeReference;
            if (collectionType != null)
            {
                return collectionType.ElementType().IsEntity();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Is this a valid binding type. i.e. is this an entity, entity collection, or complex type.
        /// </summary>
        /// <param name="bindingType">the binding type</param>
        /// <returns>true if this binding type is valid</returns>
        public static bool IsBindingTypeValid(IEdmType bindingType)
        {
            return bindingType == null || bindingType.IsEntityOrEntityCollectionType() || bindingType.IsODataComplexTypeKind();
        }
    }
}
