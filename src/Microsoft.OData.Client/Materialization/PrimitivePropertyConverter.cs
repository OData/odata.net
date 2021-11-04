//---------------------------------------------------------------------
// <copyright file="PrimitivePropertyConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml.Linq;
    using Microsoft.Spatial;

    /// <summary>
    /// Converter for primitive values which do not match the client property types. This can happen for two reasons:
    ///   1) The client property types do not exist in the protocol (Uri, XElement, etc)
    ///   2) The values were read using the service's model, and the client types are slightly different (ie float vs double, int vs long).
    /// </summary>
    internal class PrimitivePropertyConverter
    {
        /// <summary>Geo JSON formatter used for converting spatial values. Lazily created in case no spatial values are ever converted.</summary>
        private readonly SimpleLazy<GeoJsonObjectFormatter> lazyGeoJsonFormatter = new SimpleLazy<GeoJsonObjectFormatter>(GeoJsonObjectFormatter.Create);

        /// <summary>
        /// Converts a value to primitive value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>The converted value if the value can be converted</returns>
        internal object ConvertPrimitiveValue(object value, Type propertyType)
        {
            Debug.Assert(!(value is ODataUntypedValue), "!(propertyValue is ODataUntypedValue)");

            // System.Xml.Linq.XElement and System.Data.Linq.Binaries primitive types are not supported by ODataLib directly,
            // so if the property is of one of those types, we need to convert the value to that type here.
            if (propertyType != null && value != null)
            {
                if (!PrimitiveType.IsKnownNullableType(propertyType))
                {
                    throw new InvalidOperationException(Client.Strings.ClientType_UnsupportedType(propertyType));
                }

                // Fast path for the supported primitive types that have a type code and are supported by ODataLib.
                Type nonNullablePropertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                if (IsSupportedODataPrimitiveType(nonNullablePropertyType))
                {
                    return this.ConvertValueIfNeeded(value, propertyType);
                }

                // Do the conversion for types that are not supported by ODataLib e.g. char[], char, etc
                // PropertyType might be nullable. Hence to avoid nullable checks, we currently check for
                // primitiveType.ClrType
                if (CanMapToODataPrimitiveType(nonNullablePropertyType))
                {
                    PrimitiveType primitiveType;
                    PrimitiveType.TryGetPrimitiveType(propertyType, out primitiveType);
                    Debug.Assert(primitiveType != null, "must be a known primitive type");

                    string stringValue = (string)this.ConvertValueIfNeeded(value, typeof(string));
                    return primitiveType.TypeConverter.Parse(stringValue);
                }

                if (propertyType == BinaryTypeConverter.BinaryType)
                {
                    byte[] byteArray = (byte[])this.ConvertValueIfNeeded(value, typeof(byte[]));
                    Debug.Assert(byteArray != null, "If the property is of type System.Data.Linq.Binary then ODataLib should have read it as byte[].");
                    return Activator.CreateInstance(BinaryTypeConverter.BinaryType, byteArray);
                }
            }

            return this.ConvertValueIfNeeded(value, propertyType);
        }

        private static bool IsSupportedODataPrimitiveType(Type type)
        {
            return type == typeof(Boolean) || type == typeof(Byte) ||
                   type == typeof(Decimal) || type == typeof(Double) ||
                   type == typeof(Int16) || type == typeof(Int32) ||
                   type == typeof(Int64) || type == typeof(SByte) ||
                   type == typeof(Single) || type == typeof(String);
        }

        private static bool CanMapToODataPrimitiveType(Type type)
        {
            return type == typeof(Char) || type == typeof(UInt16) ||
                   type == typeof(UInt32) || type == typeof(UInt64) ||
                   type == typeof(Char[]) || type == typeof(Type) ||
                   type == typeof(Uri) || type == typeof(XDocument) ||
                   type == typeof(XElement);
        }

        /// <summary>
        /// Converts a non-spatial primitive value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <returns>The converted value.</returns>
        private static object ConvertNonSpatialValue(object value, Type targetType)
        {
            Debug.Assert(value != null, "value != null");

            // These types can be safely converted to directly, as there is no risk of precision being lost.
            if (CanSafelyConvertTo(targetType))
            {
                return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }

            string stringValue = ClientConvert.ToString(value);
            return ClientConvert.ChangeType(stringValue, targetType);
        }

        private static bool CanSafelyConvertTo(Type targetType)
        {
            return targetType == typeof(Boolean) || targetType == typeof(Byte) ||
                   targetType == typeof(SByte) || targetType == typeof(Int16) ||
                   targetType == typeof(Int32) || targetType == typeof(Int64);
        }

        /// <summary>
        /// Converts the value to the target type if needed.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The converted value.</returns>
        private object ConvertValueIfNeeded(object value, Type targetType)
        {
            // if conversion is not needed, just short cut here.
            if (value == null || targetType.IsInstanceOfType(value))
            {
                return value;
            }

            // spatial types require some extra work, as they cannot be easily converted to/from string.
            if (typeof(ISpatial).IsAssignableFrom(targetType) || value is ISpatial)
            {
                return this.ConvertSpatialValue(value, targetType);
            }

            var nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);
            if (nullableUnderlyingType != null)
            {
                targetType = nullableUnderlyingType;
            }

            // re-parse the primitive value.
            return ConvertNonSpatialValue(value, targetType);
        }

        /// <summary>
        /// Converts a spatial value by from geometry to geography or vice versa. Will return the original instance if it is already of the appropriate hierarchy.
        /// Will throw whatever parsing/format exceptions occur if the sub type is not the same.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <returns>The original or converted value.</returns>
        private object ConvertSpatialValue(object value, Type targetType)
        {
            Debug.Assert(value != null, "value != null");
            Debug.Assert(value is ISpatial && typeof(ISpatial).IsAssignableFrom(targetType), "Arguments must be spatial values/types.");

            // because spatial values already encode their specific subtype (ie point vs polygon), then the only way this conversion can work
            // is if the switch is from geometry to geography, but with the same subtype. So if we detect that the value is already in the same
            // hierarchy as the target type, then simply return it.
            if (typeof(Geometry).IsAssignableFrom(targetType))
            {
                var geographyValue = value as Geography;
                if (geographyValue == null)
                {
                    return value;
                }

                return this.ConvertSpatialValue<Geography, Geometry>(geographyValue);
            }

            Debug.Assert(typeof(Geography).IsAssignableFrom(targetType), "Unrecognized spatial target type: " + targetType.FullName);

            // as above, if the hierarchy already matches, simply return it.
            var geometryValue = value as Geometry;
            if (geometryValue == null)
            {
                return value;
            }

            return this.ConvertSpatialValue<Geometry, Geography>(geometryValue);
        }

        /// <summary>
        /// Converts a spatial value by from geometry to geography or vice versa. Will return the original instance if it is already of the appropriate hierarchy.
        /// Will throw whatever parsing/format exceptions occur if the sub type is not the same.
        /// </summary>
        /// <typeparam name="TIn">The type of the value being converted.</typeparam>
        /// <typeparam name="TOut">The target type of the conversion.</typeparam>
        /// <param name="valueToConvert">The value to convert.</param>
        /// <returns>The original or converted value.</returns>
        private TOut ConvertSpatialValue<TIn, TOut>(TIn valueToConvert)
            where TIn : ISpatial
            where TOut : class, ISpatial
        {
            var json = this.lazyGeoJsonFormatter.Value.Write(valueToConvert);
            return this.lazyGeoJsonFormatter.Value.Read<TOut>(json);
        }
    }
}