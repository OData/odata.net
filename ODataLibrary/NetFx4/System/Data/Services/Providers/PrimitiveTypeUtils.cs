//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    #region Namespaces.
    using System;
    using System.Collections.Generic;
    using System.Data.OData;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces.

    /// <summary>
    /// Helper class to list and generate all supported primitive types.
    /// </summary>
    internal static class PrimitiveTypeUtils
    {
        /// <summary>String resource type.</summary>
        internal static readonly ResourceType StringResourceType;

        /// <summary>bool resource type.</summary>
        internal static readonly ResourceType BoolResourceType;

        /// <summary>bool? resource type.</summary>
        internal static readonly ResourceType NullableBoolResourceType;

        /// <summary>int resource type.</summary>
        internal static readonly ResourceType ByteResourceType;

        /// <summary>int? resource type.</summary>
        internal static readonly ResourceType NullableByteResourceType;

        /// <summary>int resource type.</summary>
        internal static readonly ResourceType Int16ResourceType;

        /// <summary>int? resource type.</summary>
        internal static readonly ResourceType NullableInt16ResourceType;

        /// <summary>int resource type.</summary>
        internal static readonly ResourceType Int32ResourceType;

        /// <summary>int? resource type.</summary>
        internal static readonly ResourceType NullableInt32ResourceType;

        /// <summary>long resource type.</summary>
        internal static readonly ResourceType Int64ResourceType;

        /// <summary>long? resource type.</summary>
        internal static readonly ResourceType NullableInt64ResourceType;

        /// <summary>long resource type.</summary>
        internal static readonly ResourceType SByteResourceType;

        /// <summary>long? resource type.</summary>
        internal static readonly ResourceType NullableSByteResourceType;

        /// <summary>float resource type.</summary>
        internal static readonly ResourceType FloatResourceType;

        /// <summary>float? resource type.</summary>
        internal static readonly ResourceType NullableFloatResourceType;

        /// <summary>double resource type.</summary>
        internal static readonly ResourceType DoubleResourceType;

        /// <summary>double? resource type.</summary>
        internal static readonly ResourceType NullableDoubleResourceType;

        /// <summary>decimal resource type.</summary>
        internal static readonly ResourceType DecimalResourceType;

        /// <summary>decimal? resource type.</summary>
        internal static readonly ResourceType NullableDecimalResourceType;

        /// <summary>Guid resource type.</summary>
        internal static readonly ResourceType GuidResourceType;

        /// <summary>Guid? resource type.</summary>
        internal static readonly ResourceType NullableGuidResourceType;

        /// <summary>DateTime resource type.</summary>
        internal static readonly ResourceType DateTimeResourceType;

        /// <summary>DateTime? resource type.</summary>
        internal static readonly ResourceType NullableDateTimeResourceType;

        /// <summary>String resource type.</summary>
        internal static readonly ResourceType BinaryResourceType;

        /// <summary>String resource type.</summary>
        internal static readonly ResourceType StreamResourceType;

        /// <summary>
        /// List of primitive types supported by Astoria and their corresponding edm names.
        /// </summary>
        private static readonly KeyValuePair<Type, string>[] PrimitiveTypesEdmNameMapping =
            new KeyValuePair<Type, string>[]
            {
                new KeyValuePair<Type, string>(typeof(string), EdmConstants.EdmStringTypeName),
                new KeyValuePair<Type, string>(typeof(Boolean), EdmConstants.EdmBooleanTypeName),
                new KeyValuePair<Type, string>(typeof(Boolean?), EdmConstants.EdmBooleanTypeName),
                new KeyValuePair<Type, string>(typeof(Byte), EdmConstants.EdmByteTypeName),
                new KeyValuePair<Type, string>(typeof(Byte?), EdmConstants.EdmByteTypeName),
                new KeyValuePair<Type, string>(typeof(DateTime), EdmConstants.EdmDateTimeTypeName),
                new KeyValuePair<Type, string>(typeof(DateTime?), EdmConstants.EdmDateTimeTypeName),
                new KeyValuePair<Type, string>(typeof(Decimal), EdmConstants.EdmDecimalTypeName),
                new KeyValuePair<Type, string>(typeof(Decimal?), EdmConstants.EdmDecimalTypeName),
                new KeyValuePair<Type, string>(typeof(Double), EdmConstants.EdmDoubleTypeName),
                new KeyValuePair<Type, string>(typeof(Double?), EdmConstants.EdmDoubleTypeName),
                new KeyValuePair<Type, string>(typeof(Guid), EdmConstants.EdmGuidTypeName),
                new KeyValuePair<Type, string>(typeof(Guid?), EdmConstants.EdmGuidTypeName),
                new KeyValuePair<Type, string>(typeof(Int16), EdmConstants.EdmInt16TypeName),
                new KeyValuePair<Type, string>(typeof(Int16?), EdmConstants.EdmInt16TypeName),
                new KeyValuePair<Type, string>(typeof(Int32), EdmConstants.EdmInt32TypeName),
                new KeyValuePair<Type, string>(typeof(Int32?), EdmConstants.EdmInt32TypeName),
                new KeyValuePair<Type, string>(typeof(Int64), EdmConstants.EdmInt64TypeName),
                new KeyValuePair<Type, string>(typeof(Int64?), EdmConstants.EdmInt64TypeName),
                new KeyValuePair<Type, string>(typeof(SByte), EdmConstants.EdmSByteTypeName),
                new KeyValuePair<Type, string>(typeof(SByte?), EdmConstants.EdmSByteTypeName),
                new KeyValuePair<Type, string>(typeof(Single), EdmConstants.EdmSingleTypeName),
                new KeyValuePair<Type, string>(typeof(Single?), EdmConstants.EdmSingleTypeName),
                new KeyValuePair<Type, string>(typeof(byte[]), EdmConstants.EdmBinaryTypeName),
                new KeyValuePair<Type, string>(typeof(System.IO.Stream), EdmConstants.EdmStreamTypeName),

                // TODO: When in WCF DS we will have to support these two types as well.
                // Keep the Binary and XElement in the end, since there are not the default mappings for Edm.Binary and Edm.String.
                ////new KeyValuePair<Type, string>(typeof(XElement), XmlConstants.EdmStringTypeName),
                ////new KeyValuePair<Type, string>(typeof(Binary), XmlConstants.EdmBinaryTypeName),
            };

        /// <summary>
        /// List of primitive resource types. We don't want to create them again and again, so creating them once and caching them.
        /// </summary>
        private static readonly ResourceType[] primitiveResourceTypes;

        /// <summary>
        /// Static initializer to initialize the primitive resource types.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Cannot initialize the static fields inline.")]
        static PrimitiveTypeUtils()
        {
            primitiveResourceTypes = new ResourceType[PrimitiveTypesEdmNameMapping.Length];
            for (int i = 0; i < primitiveResourceTypes.Length; i++)
            {
                string fullName = PrimitiveTypesEdmNameMapping[i].Value;
                Debug.Assert(fullName.StartsWith(EdmConstants.EdmNamespace, StringComparison.Ordinal), "fullName.StartsWith(XmlConstants.EdmNamespace, StringComparison.Ordinal)");
                string name = fullName.Substring(EdmConstants.EdmNamespace.Length + 1);
                primitiveResourceTypes[i] = new ResourceType(PrimitiveTypesEdmNameMapping[i].Key, ResourceTypeKind.Primitive, EdmConstants.EdmNamespace, name);
            }

            StringResourceType = primitiveResourceTypes[0];
            BoolResourceType = primitiveResourceTypes[1];
            NullableBoolResourceType = primitiveResourceTypes[2];
            ByteResourceType = primitiveResourceTypes[3];
            NullableByteResourceType = primitiveResourceTypes[4];
            DateTimeResourceType = primitiveResourceTypes[5];
            NullableDateTimeResourceType = primitiveResourceTypes[6];
            DecimalResourceType = primitiveResourceTypes[7];
            NullableDecimalResourceType = primitiveResourceTypes[8];
            DoubleResourceType = primitiveResourceTypes[9];
            NullableDoubleResourceType = primitiveResourceTypes[10];
            GuidResourceType = primitiveResourceTypes[11];
            NullableGuidResourceType = primitiveResourceTypes[12];
            Int16ResourceType = primitiveResourceTypes[13];
            NullableInt16ResourceType = primitiveResourceTypes[14];
            Int32ResourceType = primitiveResourceTypes[15];
            NullableInt32ResourceType = primitiveResourceTypes[16];
            Int64ResourceType = primitiveResourceTypes[17];
            NullableInt64ResourceType = primitiveResourceTypes[18];
            SByteResourceType = primitiveResourceTypes[19];
            NullableSByteResourceType = primitiveResourceTypes[20];
            FloatResourceType = primitiveResourceTypes[21];
            NullableFloatResourceType = primitiveResourceTypes[22];
            BinaryResourceType = primitiveResourceTypes[23];
            StreamResourceType = primitiveResourceTypes[24];
        }

        /// <summary>
        /// Returns an array of primitive types supported.
        /// </summary>
        /// <returns>An array of primitive types supported.</returns>
        /// <remarks>Most of the time ResourceType.GetPrimitiveResourceType should be used instead of 
        /// searching this array directly, as it takes into account nullable types.</remarks>
        internal static ResourceType[] PrimitiveTypes
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(primitiveResourceTypes != null, "primitiveResourceTypes != null");

                return primitiveResourceTypes;
            }
        }

#if DEBUG
        /// <summary>
        /// Checks whether the specified type is a known primitive type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if the specified type is known to be a primitive type; false otherwise.</returns>
        internal static bool IsPrimitiveType(Type type)
        {
            DebugUtils.CheckNoExternalCallers();

            if (type == null)
            {
                return false;
            }

            foreach (KeyValuePair<Type, string> primitiveTypeInfo in PrimitiveTypesEdmNameMapping)
            {
                if (type == primitiveTypeInfo.Key)
                {
                    return true;
                }
            }

            return false;
        }
#endif
    }
}
