//---------------------------------------------------------------------
// <copyright file="EdmValueUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Core.Evaluation
#endif
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;
#if ASTORIA_CLIENT
    using Microsoft.OData.Core;
    using ErrorStrings = Microsoft.OData.Client.Strings;
    using PlatformHelpers = Microsoft.OData.Client.PlatformHelper;
#else
    using ErrorStrings = Microsoft.OData.Core.Strings;
    using PlatformHelpers = Microsoft.OData.Core.PlatformHelper;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to deal with EDM values
    /// </summary>
    internal static class EdmValueUtils
    {
        /// <summary>
        /// Converts a primitive OData value to the corresponding <see cref="IEdmDelayedValue"/>.
        /// </summary>
        /// <param name="primitiveValue">The primitive OData value to convert.</param>
        /// <param name="type">The <see cref="IEdmTypeReference"/> for the primitive value (if available).</param>
        /// <returns>An <see cref="IEdmDelayedValue"/> for the <paramref name="primitiveValue"/>.</returns>
        internal static IEdmDelayedValue ConvertPrimitiveValue(object primitiveValue, IEdmPrimitiveTypeReference type)
        {
#if !ASTORIA_CLIENT
#endif
            Debug.Assert(primitiveValue != null, "primitiveValue != null");

            TypeCode typeCode = PlatformHelpers.GetTypeCode(primitiveValue.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Boolean);
                    return new EdmBooleanConstant(type, (bool)primitiveValue);

                case TypeCode.Byte:
                    type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Byte);
                    return new EdmIntegerConstant(type, (byte)primitiveValue);

                case TypeCode.SByte:
                    type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.SByte);
                    return new EdmIntegerConstant(type, (sbyte)primitiveValue);

                case TypeCode.Int16:
                    type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Int16);
                    return new EdmIntegerConstant(type, (Int16)primitiveValue);

                case TypeCode.Int32:
                    type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Int32);
                    return new EdmIntegerConstant(type, (Int32)primitiveValue);

                case TypeCode.Int64:
                    type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Int64);
                    return new EdmIntegerConstant(type, (Int64)primitiveValue);

                case TypeCode.Decimal:
                    IEdmDecimalTypeReference decimalType = (IEdmDecimalTypeReference)EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Decimal);
                    return new EdmDecimalConstant(decimalType, (decimal)primitiveValue);

                case TypeCode.Single:
                    type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Single);
                    return new EdmFloatingConstant(type, (Single)primitiveValue);

                case TypeCode.Double:
                    type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Double);
                    return new EdmFloatingConstant(type, (double)primitiveValue);

                case TypeCode.String:
                    IEdmStringTypeReference stringType = (IEdmStringTypeReference)EnsurePrimitiveType(type, EdmPrimitiveTypeKind.String);
                    return new EdmStringConstant(stringType, (string)primitiveValue);

                default:
                    return ConvertPrimitiveValueWithoutTypeCode(primitiveValue, type);
            }
        }

        /// <summary>
        /// Gets the clr value of the edm value based on its type.
        /// </summary>
        /// <param name="edmValue">The edm value.</param>
        /// <returns>The clr value</returns>
        internal static object ToClrValue(this IEdmPrimitiveValue edmValue)
        {
#if !ASTORIA_CLIENT
#endif
            Debug.Assert(edmValue != null, "edmValue != null");
            EdmPrimitiveTypeKind primitiveKind = edmValue.Type.PrimitiveKind();
            switch (edmValue.ValueKind)
            {
                case EdmValueKind.Binary:
                    return ((IEdmBinaryValue)edmValue).Value;

                case EdmValueKind.Boolean:
                    return ((IEdmBooleanValue)edmValue).Value;

                case EdmValueKind.DateTimeOffset:
                    return ((IEdmDateTimeOffsetValue)edmValue).Value;

                case EdmValueKind.Decimal:
                    return ((IEdmDecimalValue)edmValue).Value;

                case EdmValueKind.Guid:
                    return ((IEdmGuidValue)edmValue).Value;

                case EdmValueKind.String:
                    return ((IEdmStringValue)edmValue).Value;

                case EdmValueKind.Duration:
                    return ((IEdmDurationValue)edmValue).Value;

                case EdmValueKind.Floating:
                    return ConvertFloatingValue((IEdmFloatingValue)edmValue, primitiveKind);

                case EdmValueKind.Integer:
                    return ConvertIntegerValue((IEdmIntegerValue)edmValue, primitiveKind);

                case EdmValueKind.Date:
                    return ((IEdmDateValue)edmValue).Value;

                case EdmValueKind.TimeOfDay:
                    return ((IEdmTimeOfDayValue)edmValue).Value;
            }

            throw new ODataException(ErrorStrings.EdmValueUtils_CannotConvertTypeToClrValue(edmValue.ValueKind));
        }

#if !ASTORIA_CLIENT
        /// <summary>
        /// Tries to get a stream property of the specified name.
        /// </summary>
        /// <param name="entityInstance">The instance of the entity to get the stream property for.</param>
        /// <param name="streamPropertyName">The stream property name to find.</param>
        /// <param name="streamProperty">The stream property found.</param>
        /// <returns>true if the stream property was found or if the stream property name was null (default stream).
        /// false if the stream property doesn't exist.</returns>
        internal static bool TryGetStreamProperty(IEdmStructuredValue entityInstance, string streamPropertyName, out IEdmProperty streamProperty)
        {
            Debug.Assert(entityInstance != null, "entityInstance != null");

            streamProperty = null;
            if (streamPropertyName != null)
            {
                streamProperty = entityInstance.Type.AsEntity().FindProperty(streamPropertyName);
                if (streamProperty == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the the CLR value for a primitive property.
        /// </summary>
        /// <param name="structuredValue">The structured value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The clr value of the property.</returns>
        internal static object GetPrimitivePropertyClrValue(this IEdmStructuredValue structuredValue, string propertyName)
        {
            Debug.Assert(structuredValue != null, "entityInstance != null");
            IEdmStructuredTypeReference valueType = structuredValue.Type.AsStructured();

            IEdmPropertyValue propertyValue = structuredValue.FindPropertyValue(propertyName);
            if (propertyValue == null)
            {
                throw new ODataException(ErrorStrings.EdmValueUtils_PropertyDoesntExist(valueType.FullName(), propertyName));
            }

            if (propertyValue.Value.ValueKind == EdmValueKind.Null)
            {
                return null;
            }

            IEdmPrimitiveValue primitiveValue = propertyValue.Value as IEdmPrimitiveValue;
            if (primitiveValue == null)
            {
                throw new ODataException(ErrorStrings.EdmValueUtils_NonPrimitiveValue(propertyValue.Name, valueType.FullName()));
            }

            return primitiveValue.ToClrValue();
        }
#endif

        /// <summary>
        /// Converts a floating-point edm value to a clr value
        /// </summary>
        /// <param name="floatingValue">The edm floating-point value.</param>
        /// <param name="primitiveKind">Kind of the primitive.</param>
        /// <returns>The converted value</returns>
        private static object ConvertFloatingValue(IEdmFloatingValue floatingValue, EdmPrimitiveTypeKind primitiveKind)
        {
            Debug.Assert(floatingValue != null, "floatingValue != null");
            double doubleValue = floatingValue.Value;

            if (primitiveKind == EdmPrimitiveTypeKind.Single)
            {
                return Convert.ToSingle(doubleValue);
            }

            Debug.Assert(primitiveKind == EdmPrimitiveTypeKind.Double, "primitiveKind == EdmPrimitiveTypeKind.Double");
            return doubleValue;
        }

        /// <summary>
        /// Converts an integer edm value to a clr value.
        /// </summary>
        /// <param name="integerValue">The integer value.</param>
        /// <param name="primitiveKind">Kind of the primitive.</param>
        /// <returns>The converted value</returns>
        private static object ConvertIntegerValue(IEdmIntegerValue integerValue, EdmPrimitiveTypeKind primitiveKind)
        {
            Debug.Assert(integerValue != null, "integerValue != null");
            long longValue = integerValue.Value;

            switch (primitiveKind)
            {
                case EdmPrimitiveTypeKind.Int16:
                    return Convert.ToInt16(longValue);

                case EdmPrimitiveTypeKind.Int32:
                    return Convert.ToInt32(longValue);

                case EdmPrimitiveTypeKind.Byte:
                    return Convert.ToByte(longValue);

                case EdmPrimitiveTypeKind.SByte:
                    return Convert.ToSByte(longValue);

                default:
                    Debug.Assert(primitiveKind == EdmPrimitiveTypeKind.Int64, "primitiveKind == EdmPrimitiveTypeKind.Int64");
                    return longValue;
            }
        }

        /// <summary>
        /// Convert a primitive value which didn't match any of the known values of the TypeCode enumeration.
        /// </summary>
        /// <param name="primitiveValue">The value to convert.</param>
        /// <param name="type">The expected primitive type or null.</param>
        /// <returns>The converted value.</returns>
        private static IEdmDelayedValue ConvertPrimitiveValueWithoutTypeCode(object primitiveValue, IEdmPrimitiveTypeReference type)
        {
            byte[] bytes = primitiveValue as byte[];
            if (bytes != null)
            {
                IEdmBinaryTypeReference binaryType = (IEdmBinaryTypeReference)EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Binary);
                return new EdmBinaryConstant(binaryType, bytes);
            }

            if (primitiveValue is Date)
            {
                IEdmPrimitiveTypeReference dateType = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Date);
                return new EdmDateConstant(dateType, (Date)primitiveValue);
            }

            if (primitiveValue is DateTimeOffset)
            {
                IEdmTemporalTypeReference dateTimeOffsetType = (IEdmTemporalTypeReference)EnsurePrimitiveType(type, EdmPrimitiveTypeKind.DateTimeOffset);
                return new EdmDateTimeOffsetConstant(dateTimeOffsetType, (DateTimeOffset)primitiveValue);
            }

            if (primitiveValue is Guid)
            {
                type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Guid);
                return new EdmGuidConstant(type, (Guid)primitiveValue);
            }

            if (primitiveValue is TimeOfDay)
            {
                IEdmTemporalTypeReference timeOfDayType = (IEdmTemporalTypeReference)EnsurePrimitiveType(type, EdmPrimitiveTypeKind.TimeOfDay);
                return new EdmTimeOfDayConstant(timeOfDayType, (TimeOfDay)primitiveValue);
            }

            if (primitiveValue is TimeSpan)
            {
                IEdmTemporalTypeReference timeType = (IEdmTemporalTypeReference)EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Duration);
                return new EdmDurationConstant(timeType, (TimeSpan)primitiveValue);
            }

            if (primitiveValue is ISpatial)
            {
                // TODO: [JsonLight] Add support for spatial values in ODataEdmStructuredValue
                throw new NotImplementedException();
            }

#if ASTORIA_CLIENT
            IEdmDelayedValue convertPrimitiveValueWithoutTypeCode;
            if (TryConvertClientSpecificPrimitiveValue(primitiveValue, type, out convertPrimitiveValueWithoutTypeCode))
            {
                return convertPrimitiveValueWithoutTypeCode;
            }
#endif

            throw new ODataException(ErrorStrings.EdmValueUtils_UnsupportedPrimitiveType(primitiveValue.GetType().FullName));
        }

#if ASTORIA_CLIENT
        /// <summary>
        /// Tries to convert the given value if it is of a type specific to the client library but still able to be mapped to EDM.
        /// </summary>
        /// <param name="primitiveValue">The value to convert.</param>
        /// <param name="type">The expected type of the value or null.</param>
        /// <param name="convertedValue">The converted value, if conversion was possible.</param>
        /// <returns>Whether or not conversion was possible.</returns>
        private static bool TryConvertClientSpecificPrimitiveValue(object primitiveValue, IEdmPrimitiveTypeReference type, out IEdmDelayedValue convertedValue)
        {
            byte[] byteArray;
            if (ClientConvert.TryConvertBinaryToByteArray(primitiveValue, out byteArray))
            {
                type = EnsurePrimitiveType(type, EdmPrimitiveTypeKind.Binary);
                convertedValue = new EdmBinaryConstant((IEdmBinaryTypeReference)type, byteArray);
                return true;
            }

            PrimitiveType clientPrimitiveType;
            if (PrimitiveType.TryGetPrimitiveType(primitiveValue.GetType(), out clientPrimitiveType))
            {
                type = EnsurePrimitiveType(type, clientPrimitiveType.PrimitiveKind);
                if (clientPrimitiveType.PrimitiveKind == EdmPrimitiveTypeKind.String)
                {
                    {
                        convertedValue = new EdmStringConstant((IEdmStringTypeReference)type, clientPrimitiveType.TypeConverter.ToString(primitiveValue));
                        return true;
                    }
                }
            }

            convertedValue = null;
            return false;
        }
#endif

        /// <summary>
        /// Ensures a primitive type reference for a given primitive type kind.
        /// </summary>
        /// <param name="type">The possibly null type reference.</param>
        /// <param name="primitiveKindFromValue">The primitive type kind to ensure.</param>
        /// <returns>An <see cref="IEdmPrimitiveTypeReference"/> instance created for the <paramref name="primitiveKindFromValue"/> 
        /// if <paramref name="type"/> is null; if <paramref name="type"/> is not null, validates it and then returns it.</returns>
        private static IEdmPrimitiveTypeReference EnsurePrimitiveType(IEdmPrimitiveTypeReference type, EdmPrimitiveTypeKind primitiveKindFromValue)
        {
            if (type == null)
            {
                type = EdmCoreModel.Instance.GetPrimitive(primitiveKindFromValue, /*isNullable*/ true);
            }
            else
            {
                EdmPrimitiveTypeKind primitiveKindFromType = type.PrimitiveDefinition().PrimitiveKind;

                if (primitiveKindFromType != primitiveKindFromValue)
                {
                    string typeName = type.FullName();
                    if (typeName == null)
                    {
                        throw new ODataException(ErrorStrings.EdmValueUtils_IncorrectPrimitiveTypeKindNoTypeName(primitiveKindFromType.ToString(), primitiveKindFromValue.ToString()));
                    }

                    throw new ODataException(ErrorStrings.EdmValueUtils_IncorrectPrimitiveTypeKind(typeName, primitiveKindFromValue.ToString(), primitiveKindFromType.ToString()));
                }
            }

            return type;
        }
    }
}