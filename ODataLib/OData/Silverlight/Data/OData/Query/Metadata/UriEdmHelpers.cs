//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;

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
        /// <returns>The requested type, or null if no such type exists.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static IEdmSchemaType FindTypeFromModel(IEdmModel model, string qualifiedName)
        {
            return model.FindType(qualifiedName);
        }

        /// <summary>
        /// Wraps call to FindTypeFromModel for a Collection type.
        /// </summary>
        /// <param name="model">the model to search</param>
        /// <param name="qualifiedName">the name to find within the model</param>
        /// <returns>a type reference to the collection type, or null if no such type exists.</returns>
        public static IEdmTypeReference FindCollectionTypeFromModel(IEdmModel model, string qualifiedName)
        {
            if (qualifiedName.StartsWith("Collection", StringComparison.Ordinal))
            {
                string[] tokenizedString = qualifiedName.Split('(');
                string baseElementType = tokenizedString[1].Split(')')[0];
                return EdmCoreModel.GetCollection(FindTypeFromModel(model, baseElementType).ToTypeReference());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Wraps a call to IEdmFunctionImport.ReturnType.
        /// </summary>
        /// <param name="serviceOperation">The function import containing the return type.</param>
        /// <returns>Gets the return type of this function.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static IEdmTypeReference GetFunctionReturnType(IEdmFunctionImport serviceOperation)
        {
            return serviceOperation.ReturnType;
        }

        /// <summary>
        /// Wraps a call to IEdmEntitySet.ElementType.
        /// </summary>
        /// <param name="entitySet">The EntitySet to containing the element type.</param>
        /// <returns>The entity type contained in this entity set.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static IEdmEntityType GetEntitySetElementType(IEdmEntitySet entitySet)
        {
            return entitySet.ElementType;
        }

        /// <summary>
        /// Wraps a call to IEdmFunctionParameter.Type.
        /// </summary>
        /// <param name="serviceOperationParameter">The IEdmFunctionParameter containing the typ[e.</param>
        /// <returns>The type of this function parameter.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static IEdmTypeReference GetOperationParameterType(IEdmFunctionParameter serviceOperationParameter)
        {
            return serviceOperationParameter.Type;
        }

        /// <summary>
        /// Check whether the parent and child are properly related types
        /// </summary>
        /// <param name="parentType">the parent type</param>
        /// <param name="childType">the child type</param>
        /// <exception cref="ODataException">Throws if the two types are not related.</exception>
        public static void CheckRelatedTo(IEdmType parentType, IEdmType childType)
        {
            IEdmEntityType childEntityType = childType as IEdmEntityType;
            if (!childEntityType.IsOrInheritsFrom(parentType) && !parentType.IsOrInheritsFrom(childEntityType))
            {
                // If the parentType is an open property, parentType will be null and can't have an ODataFullName.
                string parentTypeName = (parentType != null) ? parentType.ODataFullName() : "<null>";
                throw new ODataException(OData.Strings.MetadataBinder_HierarchyNotFollowed(childEntityType.FullName(), parentTypeName));
            }
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
                    throw new ODataException(OData.Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
                }
            }

            if (navPropSegment == null)
            {
                throw new ODataException(OData.Strings.ExpandItemBinder_TypeSegmentNotFollowedByPath);
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
        /// Is this token a container
        /// </summary>
        /// <param name="containerIdentifier">the containerIdentifier of the container to find</param>
        /// <param name="model">which model to search</param>
        /// <param name="entityContainer">the container we found, if we found one</param>
        /// <returns>true if we find a container, false otherwise</returns>
        public static bool TryGetEntityContainer(string containerIdentifier, IEdmModel model, out IEdmEntityContainer entityContainer)
        {
            entityContainer = model.FindEntityContainer(containerIdentifier);
            return entityContainer != null;
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
        /// Checks whether all function imports have the same return type 
        /// </summary>
        /// <param name="functionImports">the list to check</param>
        /// <returns>true if the list of function imports all have the same return type</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static bool AllHaveEqualReturnTypeAndAttributes(this IList<IEdmFunctionImport> functionImports)
        {
            Debug.Assert(functionImports != null, "functionImports != null");
           
            if (!functionImports.Any())
            {
                return true;
            }

            IEdmType firstReturnType = functionImports[0].ReturnType == null ? null : functionImports[0].ReturnType.Definition;
            bool firstComposability = functionImports[0].IsComposable;
            bool firstSideEffecting = functionImports[0].IsSideEffecting;
            foreach (IEdmFunctionImport f in functionImports)
            {
                if (f.IsComposable != firstComposability)
                {
                    return false;
                }

                if (f.IsSideEffecting != firstSideEffecting)
                {
                    return false;
                }

                if (firstReturnType != null)
                {
                    if (f.ReturnType.Definition.ODataFullName() != firstReturnType.ODataFullName())
                    {
                        return false;
                    }
                }
                else
                {
                    if (f.ReturnType != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Is this a valid binding type. i.e. is this an entity, entity colleciton, or complex type.
        /// </summary>
        /// <param name="bindingType">the binding type</param>
        /// <returns>true if this binding type is valid</returns>
        public static bool IsBindingTypeValid(IEdmType bindingType)
        {
            return bindingType == null || bindingType.IsEntityOrEntityCollectionType() || bindingType.IsODataComplexTypeKind();
        }
    }
}
