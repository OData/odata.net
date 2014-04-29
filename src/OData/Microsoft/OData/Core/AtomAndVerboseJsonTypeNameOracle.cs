//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Class responsible for determining the type name that should be written on the wire for entries and values in the ATOM and Verbose JSON formats.
    /// </summary>
    internal sealed class AtomAndVerboseJsonTypeNameOracle : TypeNameOracle
    {
        /// <summary>
        /// Determines the type name for the given entry to write to the payload.
        /// </summary>
        /// <param name="entry">The ODataEntry whose type name is to be written</param>
        /// <returns>Type name to write to the payload, or null if no type name should be written.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method will eventually become an override of a method in the base class, but more refactoring work needs to happen first.")]
        internal string GetEntryTypeNameForWriting(ODataEntry entry)
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
        /// Determines the type name for the given value to write to the payload.
        /// </summary>
        /// <param name="value">The value whose type name is to be written. This can be an ODataPrimitiveValue, an ODataComplexValue, an ODataCollectionValue or a Clr primitive object.</param>
        /// <param name="typeReferenceFromValue">The type resolved from the value.</param>
        /// <param name="typeNameAnnotation">The serialization type name annotation.</param>
        /// <param name="collectionValidator">true if the type name belongs to an open property, false otherwise.</param>
        /// <param name="collectionItemTypeName">Returns the item type name of the collection type if <paramref name="value"/> is a collection value and its type name can be determined.</param>
        /// <returns>Type name to write to the payload, or null if no type should be written.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method will eventually become an override of a method in the base class, but more refactoring work needs to happen first.")]
        internal string GetValueTypeNameForWriting(
            object value, 
            IEdmTypeReference typeReferenceFromValue, 
            SerializationTypeNameAnnotation typeNameAnnotation, 
            CollectionWithoutExpectedTypeValidator collectionValidator,
            out string collectionItemTypeName)
        {
            Debug.Assert(value != null, "value != null");

            collectionItemTypeName = null;

            // if no type name is specified we will use the type name inferred from metadata
            string typeName = GetTypeNameFromValue(value);
            if (typeName == null && typeReferenceFromValue != null)
            {
                typeName = typeReferenceFromValue.ODataFullName();
            }

            if (typeName != null)
            {
                // If the type is the same as the one specified by the parent collection, omit the type name, since it's not needed.
                if (collectionValidator != null && string.CompareOrdinal(collectionValidator.ItemTypeNameFromCollection, typeName) == 0)
                {
                    typeName = null;
                }

                // If value is a collection value, get the item type name.
                if (typeName != null && value is ODataCollectionValue)
                {
                    collectionItemTypeName = ValidationUtils.ValidateCollectionTypeName(typeName);
                }
            }

            if (typeNameAnnotation != null)
            {
                // If the value of TypeName is null, we'll flow it through here, thereby instructing the caller to write no type name.
                typeName = typeNameAnnotation.TypeName;
            }

            return typeName;
        }
    }
}
