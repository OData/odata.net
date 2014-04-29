//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Json;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for determining the type name that should be written on the wire for entries and values in JSON full metadata mode.
    /// </summary>
    internal sealed class JsonFullMetadataTypeNameOracle : JsonLightTypeNameOracle
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

            return entry.TypeName;
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

            // Do not write type name when the type is native json type.
            if (typeReferenceFromValue != null
                && typeReferenceFromValue.IsPrimitive()
                && JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue)value, typeReferenceFromValue.AsPrimitive()))
            {
                return null;
            }

            return GetTypeNameFromValue(value);
        }
    }
}
