//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for determining the type name that should be written on the wire for entries and values in JSON minimal metadata mode, 
    /// or the other metadata modes of JSON light when <see cref="ODataMessageWriterSettings.AutoComputePayloadMetadataInJson"/> is false.
    /// </summary>
    internal sealed class JsonMinimalMetadataTypeNameOracle : JsonLightTypeNameOracle
    {
        /// <summary>
        /// Determines the entity type name to write to the payload.
        /// </summary>
        /// <param name="expectedTypeName">The expected type name, e.g. the base type of the set or the nav prop.</param>
        /// <param name="entry">The ODataEntry whose type is to be written.</param>
        /// <returns>Type name to write to the payload, or null if no type name should be written.</returns>
        internal override string GetEntryTypeNameForWriting(string expectedTypeName, ODataEntry entry)
        {
            Debug.Assert(entry != null, "entry != null");

            SerializationTypeNameAnnotation typeNameAnnotation = entry.GetAnnotation<SerializationTypeNameAnnotation>();
            if (typeNameAnnotation != null)
            {
                return typeNameAnnotation.TypeName;
            }

            // We only write entity type names in Json Light if it's more derived (different) from the expected type name.
            string entryTypeName = entry.TypeName;
            if (expectedTypeName != entryTypeName)
            {
                return entryTypeName;
            }

            return null;
        }

        /// <summary>
        /// Determines the type name to write to the payload.  Json Light type names are only written into the payload for open properties
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
            SerializationTypeNameAnnotation typeNameAnnotation = value.GetAnnotation<SerializationTypeNameAnnotation>();
            if (typeNameAnnotation != null)
            {
                return typeNameAnnotation.TypeName;
            }

            if (typeReferenceFromValue != null)
            {
                // Write type name when the type in the payload is more derived than the type from metadata.
                if (typeReferenceFromMetadata != null && typeReferenceFromMetadata.Definition.AsActualType().ODataFullName() != typeReferenceFromValue.ODataFullName())
                {
                    return typeReferenceFromValue.ODataFullName();
                }

                // Note: When writing derived complexType value in a payload, we don't have the expected type. 
                // So always write @odata.type for top-level derived complextype.
                if (typeReferenceFromMetadata == null && typeReferenceFromValue.IsComplex())
                {
                    if ((typeReferenceFromValue as IEdmComplexTypeReference).ComplexDefinition().BaseType != null)
                    {
                        return typeReferenceFromValue.ODataFullName();
                    }
                }

                // Do not write type name when the type is native json type.
                if (typeReferenceFromValue.IsPrimitive() && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue)value, typeReferenceFromValue.AsPrimitive()))
                {
                    return null;
                }
            }

            if (!isOpenProperty)
            {
                // Do not write type name for non-open properties since we expect the reader to have an expected type (via API or context URI) and thus not need it.
                return null;
            }

            return GetTypeNameFromValue(value);
        }
    }
}
