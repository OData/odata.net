//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Metadata
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
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
            return serviceOperation.Operation.ReturnType;
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
            IEdmStructuredType childStructuredType = childType as IEdmStructuredType;
            if (!childStructuredType.IsOrInheritsFrom(parentType) && !parentType.IsOrInheritsFrom(childStructuredType))
            {
                // If the parentType is an open property, parentType will be null and can't have an ODataFullName.
                string parentTypeName = (parentType != null) ? parentType.ODataFullName() : "<null>";
                throw new ODataException(OData.Core.Strings.MetadataBinder_HierarchyNotFollowed(childStructuredType.FullTypeName(), parentTypeName));
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
        public static bool TryParseEnum(this IEdmEnumType enumType, string value, out ODataEnumValue enumValue)
        {
            long parsedValue;
            bool success = enumType.TryParseEnum(value, true, out parsedValue);
            enumValue = null;
            if (success)
            {
                // ODataEnumValue.Value will always be numeric string like '3', '10' instead of 'Red', 'Solid,Yellow', etc.
                // so user code can easily Enum.Parse() them into CLR value.
                enumValue = new ODataEnumValue(parsedValue.ToString(CultureInfo.InvariantCulture), enumType.ODataFullName());
            }

            return success;
        }
    }
}
