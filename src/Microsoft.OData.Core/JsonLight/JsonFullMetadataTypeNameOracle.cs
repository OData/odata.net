//---------------------------------------------------------------------
// <copyright file="JsonFullMetadataTypeNameOracle.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for determining the type name that should be written on the wire for entries and values in JSON full metadata mode.
    /// </summary>
    internal sealed class JsonFullMetadataTypeNameOracle : JsonLightTypeNameOracle
    {
        /// <summary>
        /// Determines the resource set type name to write to the payload.
        /// </summary>
        /// <param name="expectedResourceTypeName">The expected resource type name of the items in the resource set.</param>
        /// <param name="resourceSet">The ODataResourceSet whose type is to be written.</param>
        /// <param name="isUndeclared">true if the resource set is for some undeclared property</param>
        /// <returns>Type name to write to the payload, or null if no type name should be written.</returns>
        internal override string GetResourceSetTypeNameForForWriting(string expectedResourceTypeName, ODataResourceSet resourceSet, bool isUndeclared)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            if (resourceSet.TypeAnnotation != null)
            {
                return resourceSet.TypeAnnotation.TypeName;
            }

            if (isUndeclared)
            {
                return resourceSet.TypeName;
            }

            return null;
        }

        /// <summary>
        /// Determines the resource type name to write to the payload.
        /// </summary>
        /// <param name="expectedTypeName">The expected type name, e.g. the base type of the an entity set or a collection of complex or the nav prop or a complex property.</param>
        /// <param name="resource">The ODataResource whose type is to be written.</param>
        /// <param name="isUndeclared">true if the ODataResource is for some undeclared property</param>
        /// <returns>Type name to write to the payload, or null if no type name should be written.</returns>
        internal override string GetResourceTypeNameForWriting(string expectedTypeName, ODataResourceBase resource, bool isUndeclared = false)
        {
            Debug.Assert(resource != null, "resource != null");

            if (resource.TypeAnnotation != null)
            {
                return resource.TypeAnnotation.TypeName;
            }

            return resource.TypeName;
        }

        /// <summary>
        /// Determines the type name to write to the payload. Json Light type names are only written into the payload for open properties
        /// or if the payload type name is more derived than the model type name.
        /// </summary>
        /// <param name="value">The ODataValue whose type name is to be written.</param>
        /// <param name="typeReferenceFromMetadata">The type as expected by the model.</param>
        /// <param name="typeReferenceFromValue">The type resolved from the value.</param>
        /// <param name="isOpenProperty">true if the type name belongs to an open property, false otherwise.</param>
        /// <returns>Type name to write to the payload, or null if no type should be written.</returns>
        internal override string GetValueTypeNameForWriting(
            ODataValue value,
            IEdmTypeReference typeReferenceFromMetadata,
            IEdmTypeReference typeReferenceFromValue,
            bool isOpenProperty)
        {
            string typeNameToWrite;
            if (TypeNameOracle.TryGetTypeNameFromAnnotation(value, out typeNameToWrite))
            {
                return typeNameToWrite;
            }

            // Do not write type name when the type is native json type.
            if (typeReferenceFromValue != null
                && typeReferenceFromValue.IsPrimitive()
                && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue)value, typeReferenceFromValue.AsPrimitive()))
            {
                return null;
            }

            return GetTypeNameFromValue(value);
        }

        /// <summary>
        /// Determines the type name to write to the payload.  Json Light type names are only written into the payload for open properties
        /// or if the payload type name is more derived than the model type name.
        /// </summary>
        /// <param name="value">The ODataValue whose type name is to be written.</param>
        /// <param name="propertyInfo">The serialization info of current property</param>
        /// <param name="isOpenProperty">true if the type name belongs to an open property, false otherwise.</param>
        /// <returns>Type name to write to the payload, or null if no type should be written.</returns>
        internal override string GetValueTypeNameForWriting(
            ODataValue value,
            PropertySerializationInfo propertyInfo,
            bool isOpenProperty)
        {
            PropertyValueTypeInfo valueType = propertyInfo.ValueType;

            string typeNameToWrite;
            if (TypeNameOracle.TryGetTypeNameFromAnnotation(value, out typeNameToWrite))
            {
                return typeNameToWrite;
            }

            if (valueType.TypeReference != null)
            {
                // Do not write type name when the type is native json type.
                if (valueType.IsPrimitive && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue)value, valueType.PrimitiveTypeKind))
                {
                    return null;
                }
            }

            return GetTypeNameFromValue(value);
        }
    }
}