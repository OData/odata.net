//---------------------------------------------------------------------
// <copyright file="JsonMinimalMetadataTypeNameOracle.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Json;
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for determining the type name that should be written on the wire for entries and values in JSON minimal metadata mode,
    /// </summary>
    internal sealed class JsonMinimalMetadataTypeNameOracle : JsonLightTypeNameOracle
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

            var expectedResourceSetTypeName = expectedResourceTypeName == null ? null : EdmLibraryExtensions.GetCollectionTypeName(expectedResourceTypeName);

            if (expectedResourceSetTypeName != resourceSet.TypeName || isUndeclared)
            {
                return resourceSet.TypeName;
            }

            return null;
        }

        /// <summary>
        /// Determines the entity type name to write to the payload.
        /// </summary>
        /// <param name="expectedTypeName">The expected type name, e.g. the base type of the set or the nav prop.</param>
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

            // We only write entity type names in Json Light if it's more derived (different) from the expected type name.
            string resourceTypeName = resource.TypeName;
            if (expectedTypeName != resourceTypeName || isUndeclared)
            {
                return resourceTypeName;
            }

            return null;
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
            string fullTypeNameFromValue = null;

            string typeNameToWrite;
            if (TypeNameOracle.TryGetTypeNameFromAnnotation(value, out typeNameToWrite))
            {
                return typeNameToWrite;
            }

            if (typeReferenceFromValue != null)
            {
                fullTypeNameFromValue = typeReferenceFromValue.FullName();

                // Write type name when the type in the payload is more derived than the type from metadata.
                if (typeReferenceFromMetadata != null && typeReferenceFromMetadata.Definition.AsActualType().FullTypeName() != fullTypeNameFromValue)
                {
                    return fullTypeNameFromValue;
                }

                // Do not write type name when the type is native json type.
                if (typeReferenceFromValue.IsPrimitive() && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue)value, typeReferenceFromValue.AsPrimitive()))
                {
                    return null;
                }

                // Note: When writing derived complexType value in a payload, we don't have the expected type.
                // So always write @odata.type for top-level derived complex type.
                if (typeReferenceFromMetadata == null && typeReferenceFromValue.IsComplex())
                {
                    if ((typeReferenceFromValue as IEdmComplexTypeReference).ComplexDefinition().BaseType != null)
                    {
                        return fullTypeNameFromValue;
                    }
                }
            }

            if (!isOpenProperty)
            {
                // Do not write type name for non-open properties since we expect the reader to have an expected type (via API or context URI) and thus not need it.
                return null;
            }

            return fullTypeNameFromValue != null ? fullTypeNameFromValue : GetTypeNameFromValue(value);
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
            string fullTypeNameFromValue = null;
            PropertyValueTypeInfo valueType = propertyInfo.ValueType;
            PropertyMetadataTypeInfo modelType = propertyInfo.MetadataType;

            string typeNameToWrite;
            if (TypeNameOracle.TryGetTypeNameFromAnnotation(value, out typeNameToWrite))
            {
                return typeNameToWrite;
            }

            if (valueType.TypeReference != null)
            {
                fullTypeNameFromValue = valueType.FullName;

                // Write type name when the type in the payload is more derived than the type from metadata.
                if (modelType.TypeReference != null && modelType.FullName != fullTypeNameFromValue)
                {
                    return fullTypeNameFromValue;
                }

                // Do not write type name when the type is native json type.
                if (valueType.IsPrimitive && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue)value, valueType.PrimitiveTypeKind))
                {
                    return null;
                }

                // Note: When writing derived complexType value in a payload, we don't have the expected type.
                // So always write @odata.type for top-level derived complex type.
                if (modelType.TypeReference == null && valueType.IsComplex)
                {
                    if ((valueType.TypeReference as IEdmComplexTypeReference).ComplexDefinition().BaseType != null)
                    {
                        return fullTypeNameFromValue;
                    }
                }
            }

            if (!isOpenProperty)
            {
                // Do not write type name for non-open properties since we expect the reader to have an expected type (via API or context URI) and thus not need it.
                return null;
            }

            return fullTypeNameFromValue != null ? fullTypeNameFromValue : GetTypeNameFromValue(value);
        }
    }
}