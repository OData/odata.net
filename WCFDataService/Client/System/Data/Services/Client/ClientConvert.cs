//   OData .NET Libraries ver. 5.6.3
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
