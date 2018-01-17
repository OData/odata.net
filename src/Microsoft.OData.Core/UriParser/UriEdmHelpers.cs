//---------------------------------------------------------------------
// <copyright file="UriEdmHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
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
        public static IEdmSchemaType FindTypeFromModel(IEdmModel model, string qualifiedName, ODataUriResolver resolver)
        {
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
            IEdmEnumType enumType = FindTypeFromModel(model, qualifiedName, ODataUriResolver.GetUriResolver(null)) as IEdmEnumType;
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
                throw new ODataException(Strings.MetadataBinder_HierarchyNotFollowed(childType.FullTypeName(), parentTypeName));
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
                    throw new ODataException(Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
                }
            }

            if (navPropSegment == null)
            {
                throw new ODataException(Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
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
        /// Returns true if this type is a structured type collection
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>true if the type is a structured type collection</returns>
        public static bool IsStructuredCollection(this IEdmTypeReference type)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");
            IEdmCollectionTypeReference collectionType = type as IEdmCollectionTypeReference;
            if (collectionType != null)
            {
                return collectionType.ElementType().IsStructured();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the type reference of the <paramref name="structuredType"/>
        /// </summary>
        /// <param name="structuredType">The structured type</param>
        /// <returns>The type reference</returns>
        public static IEdmStructuredTypeReference GetTypeReference(this IEdmStructuredType structuredType)
        {
            IEdmEntityType entityType = structuredType as IEdmEntityType;
            IEdmStructuredTypeReference typeReference;

            if (entityType != null)
            {
                typeReference = new EdmEntityTypeReference(entityType, false);
            }
            else
            {
                typeReference = new EdmComplexTypeReference(structuredType as IEdmComplexType, false);
            }

            return typeReference;
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
