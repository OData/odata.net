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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Data.Services.Providers;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Class with utility methods for dealing with OData metadata.
    /// </summary>
    internal static class MetadataUtils
    {
        /// <summary>
        /// Try to determine the primitive type of the <paramref name="value"/> argument and return the name of the primitive type.
        /// </summary>
        /// <param name="value">The value to determine the type for.</param>
        /// <param name="typeName">The name of the primitive type of the <paramref name="value"/>.</param>
        /// <returns>True if the value is of a known primitive type; otherwise false.</returns>
        internal static bool TryGetPrimitiveTypeName(object value, out string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");

            typeName = null;

            TypeCode typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    typeName = EdmConstants.EdmBooleanTypeName;
                    break;

                case TypeCode.Byte:
                    typeName = EdmConstants.EdmByteTypeName;
                    break;

                case TypeCode.DateTime:
                    typeName = EdmConstants.EdmDateTimeTypeName;
                    break;

                case TypeCode.Decimal:
                    typeName = EdmConstants.EdmDecimalTypeName;
                    break;

                case TypeCode.Double:
                    typeName = EdmConstants.EdmDoubleTypeName;
                    break;

                case TypeCode.Int16:
                    typeName = EdmConstants.EdmInt16TypeName;
                    break;

                case TypeCode.Int32:
                    typeName = EdmConstants.EdmInt32TypeName;
                    break;

                case TypeCode.Int64:
                    typeName = EdmConstants.EdmInt64TypeName;
                    break;

                case TypeCode.SByte:
                    typeName = EdmConstants.EdmSByteTypeName;
                    break;

                case TypeCode.String:
                    typeName = EdmConstants.EdmStringTypeName;
                    break;

                case TypeCode.Single:
                    typeName = EdmConstants.EdmSingleTypeName;
                    break;

                default:
                    byte[] bytes = value as byte[];
                    if (bytes != null)
                    {
                        typeName = EdmConstants.EdmBinaryTypeName;
                        break;
                    }

                    if (value is DateTimeOffset)
                    {
                        typeName = EdmConstants.EdmDateTimeOffsetTypeName;
                        break;
                    }

                    if (value is Guid)
                    {
                        typeName = EdmConstants.EdmGuidTypeName;
                        break;
                    }

                    if (value is TimeSpan)
                    {
                        // Edm.Time
                        typeName = EdmConstants.EdmTimeTypeName;
                        break;
                    }

                    return false;
            }

            Debug.Assert(typeName != null, "typeName != null");
            return true;
        }

        /// <summary>
        /// Returns MultiValue item type name or null if the provided type name is not a multiValue.
        /// </summary>
        /// <param name="typeName">MultiValue type name read from payload.</param>
        /// <returns>MultiValue element type name or null if not a multiValue.</returns>
        internal static string GetMultiValueItemTypeName(string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(typeName), "!string.IsNullOrEmpty(typeName)");

            return GetMultiValueItemTypeName(typeName, false);
        }

        /// <summary>
        /// Resolve a type name against the provided metadata. If no type name is given we either throw (if a type name on the value is required, e.g., on entries)
        /// or infer the type from metadata (if available).
        /// </summary>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="typeFromMetadata">The type inferred from metadata or null if no metadata is available.</param>
        /// <param name="typeName">The type name to be resolved.</param>
        /// <param name="typeKind">The expected type kind of the resolved type.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <returns>A resource type for the <paramref name="typeName"/> or null if no metadata is available.</returns>
        internal static ResourceType ResolveTypeName(DataServiceMetadataProviderWrapper metadata, ResourceType typeFromMetadata, ref string typeName, ResourceTypeKind typeKind, bool isOpenPropertyType)
        {
            DebugUtils.CheckNoExternalCallers();

            ResourceType typeFromValue = ValidationUtils.ValidateTypeName(metadata, typeName, typeKind, isOpenPropertyType);
            typeFromValue = ValidationUtils.ValidateMetadataType(typeFromMetadata, typeFromValue, typeKind);

            // derive the type name from the metadata if available
            if (typeName == null && typeFromValue != null)
            {
                typeName = typeFromValue.FullName;
            }

            return typeFromValue;
        }

        /// <summary>
        /// Returns MultiValue item type name or null if the provided type name is not a multiValue.
        /// </summary>
        /// <param name="typeName">MultiValue type name.</param>
        /// <param name="isNested">Whether it is a nested (recursive) call.</param>
        /// <returns>MultiValue element type name or null if not a multiValue.</returns>
        /// <remarks>
        /// The following rules are used for multiValue type names:
        /// - it has to start with "MultiValue(" and end with ")" - trailing and leading whitespaces make the type not to be recognized as multiValue.
        /// - there is to be no characters (including whitespaces) between "MultiValue" and "(" - otherwise it won't berecognized as multiValue
        /// - multiValue item type name has to be a non-empty string - i.e. "MultiValue()" won't be recognized as multiValue
        /// - nested multiValue - e.g. "MultiValue(MultiValue(Edm.Int32))" - are not supported - we will throw
        /// Note the following are examples of valid type names which are not multiValue:
        /// - "MultiValue()"
        /// - " MultiValue(Edm.Int32)"
        /// - "MultiValue (Edm.Int32)"
        /// - "MultiValue("
        /// </remarks>
        private static string GetMultiValueItemTypeName(string typeName, bool isNested)
        {
            int multiValueTypeQualifierLength = EdmConstants.MultiValueTypeQualifier.Length;

            // to be recognized as a multiValue wireTypeName must not be null, has to start with "MultiValue(" and end with ")" and must not be "MultiValue()"
            if (typeName != null &&
                typeName.StartsWith(EdmConstants.MultiValueTypeQualifier + "(", StringComparison.Ordinal) &&
                typeName[typeName.Length - 1] == ')' &&
                typeName.Length != multiValueTypeQualifierLength + 2)
            {
                if (isNested)
                {
                    throw new ODataException(Strings.ODataWriter_NestedMultiValuesAreNotSupported);
                }

                string innerTypeName = typeName.Substring(multiValueTypeQualifierLength + 1, typeName.Length - (multiValueTypeQualifierLength + 2));

                // Check if it is not a nested multiValue and throw if it is
                GetMultiValueItemTypeName(innerTypeName, true);

                return innerTypeName;
            }

            return null;
        }
    }
}
