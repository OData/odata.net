//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace System.Data.Services.Client
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;

    #endregion Namespaces

    /// <summary>
    /// static utility functions for conversions
    /// </summary>
    internal static class ClientConvert
    {
        /// <summary>
        /// convert from string to the appropriate type
        /// </summary>
        /// <param name="propertyValue">incoming string value</param>
        /// <param name="propertyType">type to convert to</param>
        /// <returns>converted value</returns>
        internal static object ChangeType(string propertyValue, Type propertyType)
        {
            Debug.Assert(null != propertyValue, "should never be passed null");

            PrimitiveType primitiveType;
            if (PrimitiveType.TryGetPrimitiveType(propertyType, out primitiveType) && primitiveType.TypeConverter != null)
            {
                try
                {
                    // functionality provided by type converter
                    return primitiveType.TypeConverter.Parse(propertyValue);
                }
                catch (FormatException ex)
                {
                    propertyValue = (0 == propertyValue.Length ? "String.Empty" : "String");
                    throw Error.InvalidOperation(Strings.Deserialize_Current(propertyType.ToString(), propertyValue), ex);
                }
                catch (OverflowException ex)
                {
                    propertyValue = (0 == propertyValue.Length ? "String.Empty" : "String");
                    throw Error.InvalidOperation(Strings.Deserialize_Current(propertyType.ToString(), propertyValue), ex);
                }
            }
            else
            {
                Debug.Assert(false, "new StorageType without update to knownTypes");
                return propertyValue;
            }
        }

        /// <summary>
        /// Tries to converts a binary value to a byte array.
        /// </summary>
        /// <param name="binaryValue">The binary value to convert.</param>
        /// <param name="converted">The equivalent value converted to a byte array.</param>
        /// <returns>Whether the value was binary.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "binaryValue",
            Justification = "Method is compiled into desktop and SL assemblies, and the parameter is used in the desktop version.")]
        internal static bool TryConvertBinaryToByteArray(object binaryValue, out byte[] converted)
        {
            Debug.Assert(binaryValue != null, "binaryValue != null");
#if !ASTORIA_LIGHT && !PORTABLELIB
            Type valueType = binaryValue.GetType();
            PrimitiveType ptype;
            if (PrimitiveType.TryGetPrimitiveType(valueType, out ptype) && valueType == BinaryTypeConverter.BinaryType)
            {
                const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;
                converted = (byte[])valueType.InvokeMember("ToArray", Flags, null, binaryValue, null, CultureInfo.InvariantCulture);
                return true;
            }
#endif
            converted = null;
            return false;
        }

        /// <summary>
        /// change primtive typeName into non-nullable type
        /// </summary>
        /// <param name="typeName">like Edm.String or Edm.Binary</param>
        /// <param name="type">the mapped output type</param>
        /// <returns>true if named</returns>
        internal static bool ToNamedType(string typeName, out Type type)
        {
            type = typeof(string);
            if (String.IsNullOrEmpty(typeName))
            {
                return true;
            }
            else
            {
                PrimitiveType ptype;
                if (PrimitiveType.TryGetPrimitiveType(typeName, out ptype))
                {
                    type = ptype.ClrType;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        /// <summary>
        /// Convert from primitive value to an xml payload string. 
        /// NOTE: We need to pay special attention to DateTimes - if the converted value is going to be used as a content of 
        /// atom:updated or atom:published element we have to ensure it contains information about time zone. At the same time we 
        /// must not touch datetime values that in content or are mapped to custom elements. 
        /// </summary>
        /// <param name="propertyValue">incoming object value</param>
        /// <returns>converted value</returns>
        internal static string ToString(object propertyValue)
        {
            Debug.Assert(null != propertyValue, "null should be handled by caller");

            PrimitiveType primitiveType;
            if (PrimitiveType.TryGetPrimitiveType(propertyValue.GetType(), out primitiveType) && primitiveType.TypeConverter != null)
            {
                return primitiveType.TypeConverter.ToString(propertyValue);
            }

            Debug.Assert(false, "new StorageType without update to knownTypes");
            return propertyValue.ToString();
        }

        /// <summary>type edm type string for content</summary>
        /// <param name="propertyType">type to analyze</param>
        /// <returns>edm type string for payload, null for unknown</returns>
        internal static string GetEdmType(Type propertyType)
        {
            PrimitiveType primitiveType;
            if (PrimitiveType.TryGetPrimitiveType(propertyType, out primitiveType))
            {
                if (primitiveType.EdmTypeName != null)
                {
                    return primitiveType.EdmTypeName;
                }
                else
                {
                    // case StorageType.UInt16:
                    // case StorageType.UInt32:
                    // case StorageType.UInt64:
                    // don't support reverse mappings for these types in this version
                    // allows us to add real server support in the future without a
                    // "breaking change" in the future client
                    throw new NotSupportedException(Strings.ALinq_CantCastToUnsupportedPrimitive(propertyType.Name));
                }
            }
            else
            {
                Debug.Assert(false, "knowntype without reverse mapping");
                return null;
            }
        }
    }
}
