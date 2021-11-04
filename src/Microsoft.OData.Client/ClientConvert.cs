//---------------------------------------------------------------------
// <copyright file="ClientConvert.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
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
            Debug.Assert(propertyValue != null, "should never be passed null");

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
                    propertyValue = propertyValue.Length == 0 ? "String.Empty" : "String";
                    throw Error.InvalidOperation(Strings.Deserialize_Current(propertyType.ToString(), propertyValue), ex);
                }
                catch (OverflowException ex)
                {
                    propertyValue = propertyValue.Length == 0 ? "String.Empty" : "String";
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
            Type valueType = binaryValue.GetType();
            PrimitiveType ptype;
            if (PrimitiveType.TryGetPrimitiveType(valueType, out ptype) && valueType == BinaryTypeConverter.BinaryType)
            {
                const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;
                converted = (byte[])valueType.InvokeMember("ToArray", Flags, null, binaryValue, null, CultureInfo.InvariantCulture);
                return true;
            }

            converted = null;
            return false;
        }

        /// <summary>
        /// change primitive typeName into non-nullable type
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
        /// </summary>
        /// <param name="propertyValue">incoming object value</param>
        /// <returns>converted value</returns>
        internal static string ToString(object propertyValue)
        {
            Debug.Assert(propertyValue != null, "null should be handled by caller");
            Debug.Assert(!(propertyValue is ODataUntypedValue), "!(propertyValue is ODataUntypedValue)");

            PrimitiveType primitiveType;
            if (PrimitiveType.TryGetPrimitiveType(propertyValue.GetType(), out primitiveType) && primitiveType.TypeConverter != null)
            {
                return primitiveType.TypeConverter.ToString(propertyValue);
            }

            // If the type of a property is enum on server side, but it is System.String on client side,
            // then propertyValue should be ODataEnumValue. We should return the enumValue.Value.
            var enumValue = propertyValue as ODataEnumValue;
            if (enumValue != null)
            {
                return enumValue.Value;
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
                // Map DateTime to DateTimeOffset
                if (primitiveType.ClrType == typeof(DateTime))
                {
                    return XmlConstants.EdmDateTimeOffsetTypeName;
                }

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
