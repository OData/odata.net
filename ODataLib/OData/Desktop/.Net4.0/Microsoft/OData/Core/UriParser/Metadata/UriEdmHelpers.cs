//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;

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
        /// Wraps call to FindTypeFromModel for an Enum type.
        /// </summary>
        /// <param name="model">the model to search</param>
        /// <param name="qualifiedName">the name to find within the model</param>
        /// <returns>a type reference to the enum type, or null if no such type exists.</returns>
        public static IEdmEnumType FindEnumTypeFromModel(IEdmModel model, string qualifiedName)
        {
            IEdmEnumType enumType = FindTypeFromModel(model, qualifiedName) as IEdmEnumType;
            return enumType;
        }

        /// <summary>
        /// Wraps a call to IEdmOperationImport.ReturnType.
        /// </summary>
        /// <param name="serviceOperation">The operation import containing the return type.</param>
        /// <returns>Gets the return type of this function.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static IEdmTypeReference GetOperationImportReturnType(IEdmOperationImport serviceOperation)
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
        /// Wraps a call to IEdmOperationParameter.Type.
        /// </summary>
        /// <param name="serviceOperationParameter">The IEdmFunctionParameter containing the typ[e.</param>
        /// <returns>The type of this function parameter.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static IEdmTypeReference GetOperationParameterType(IEdmOperationParameter serviceOperationParameter)
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
                throw new ODataException(OData.Core.Strings.MetadataBinder_HierarchyNotFollowed(childEntityType.FullName(), parentTypeName));
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
        /// Checks whether all operation imports have the same return type 
        /// </summary>
        /// <param name="operationImports">the list to check</param>
        /// <returns>true if the list of operation imports all have the same return type</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Uri Parser does not need to go through the ODL behavior knob.")]
        public static bool AllHaveEqualReturnTypeAndAttributes(this IList<IEdmOperationImport> operationImports)
        {
            Debug.Assert(operationImports != null, "operationImports != null");
           
            if (!operationImports.Any())
            {
                return true;
            }

            IEdmType firstReturnType = operationImports[0].ReturnType == null ? null : operationImports[0].ReturnType.Definition;
            bool firstComposability = operationImports[0].IsComposable;
            bool firstSideEffecting = operationImports[0].IsSideEffecting;
            foreach (IEdmOperationImport f in operationImports)
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

        /// <summary>
        /// Parse string or integer to enum value
        /// </summary>
        /// <param name="enumType">edm enum type</param>
        /// <param name="value">input string value</param>
        /// <param name="enumValue">output edm enum value</param>
        /// <returns>true if parse succeeds, false if fails</returns>
        public static bool TryParseEnum(this IEdmEnumType enumType, string value, out IEdmEnumValue enumValue)
        {
            long parsedValue = 0;
            bool success = enumType.TryParseEnum(value, true, ref parsedValue);
            enumValue = null;
            if (success)
            {
                EdmEnumTypeReference enumTypeReference = new EdmEnumTypeReference(enumType, true);
                enumValue = new EdmEnumValue(enumTypeReference, new EdmIntegerConstant(parsedValue));
            }

            return success;
        }
    }
}
