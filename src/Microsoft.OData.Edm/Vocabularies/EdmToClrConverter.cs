//---------------------------------------------------------------------
// <copyright file="EdmToClrConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// <see cref="IEdmValue"/> to CLR value converter.
    /// </summary>
    public class EdmToClrConverter
    {
        private static readonly Type TypeICollectionOfT = typeof(ICollection<>);
        private static readonly Type TypeIListOfT = typeof(IList<>);
        private static readonly Type TypeListOfT = typeof(List<>);
        private static readonly Type TypeIEnumerableOfT = typeof(IEnumerable<>);
        private static readonly Type TypeNullableOfT = typeof(Nullable<>);
        private static readonly MethodInfo CastToClrTypeMethodInfo = typeof(CastHelper).GetMethod("CastToClrType");
        private static readonly MethodInfo EnumerableToListOfTMethodInfo = typeof(CastHelper).GetMethod("EnumerableToListOfT");

        private readonly Dictionary<IEdmStructuredValue, object> convertedObjects = new Dictionary<IEdmStructuredValue, object>();
        private readonly Dictionary<Type, MethodInfo> enumerableConverters = new Dictionary<Type, MethodInfo>();
        private readonly Dictionary<Type, MethodInfo> enumTypeConverters = new Dictionary<Type, MethodInfo>();

        private readonly TryCreateObjectInstance tryCreateObjectInstanceDelegate;
        private readonly TryGetClrPropertyInfo tryGetClrPropertyInfoDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmToClrConverter"/> class.
        /// </summary>
        public EdmToClrConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmToClrConverter"/> class.
        /// </summary>
        /// <param name="tryCreateObjectInstanceDelegate">The delegate customizing conversion of structured values.</param>
        public EdmToClrConverter(TryCreateObjectInstance tryCreateObjectInstanceDelegate)
        {
            EdmUtil.CheckArgumentNull(tryCreateObjectInstanceDelegate, "tryCreateObjectInstanceDelegate");

            this.tryCreateObjectInstanceDelegate = tryCreateObjectInstanceDelegate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmToClrConverter"/> class.
        /// </summary>
        /// <param name="tryCreateObjectInstanceDelegate">The delegate customizing conversion of structured values.</param>
        /// <param name="tryGetClrPropertyInfoDelegate">The delegate customizing the behavior to get client CLR property info</param>
        /// <param name="tryGetClrTypeNameDelegate">The delegate customizing the behavior to get client CLR type name</param>
        public EdmToClrConverter(
            TryCreateObjectInstance tryCreateObjectInstanceDelegate,
            TryGetClrPropertyInfo tryGetClrPropertyInfoDelegate,
            TryGetClrTypeName tryGetClrTypeNameDelegate)
        {
            this.tryCreateObjectInstanceDelegate = tryCreateObjectInstanceDelegate;
            this.tryGetClrPropertyInfoDelegate = tryGetClrPropertyInfoDelegate;
            this.TryGetClrTypeNameDelegate = tryGetClrTypeNameDelegate;
        }

        /// <summary>
        /// The delegate to get the CLR type name.
        /// </summary>
        internal TryGetClrTypeName TryGetClrTypeNameDelegate { get; private set; }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a CLR value of the specified type.
        /// Supported values for <typeparamref name="T"/> are:
        ///     CLR primitive types such as <see cref="System.String"/> and <see cref="System.Int32"/>,
        ///     CLR enum types,
        ///     <see cref="IEnumerable&lt;T&gt;"/>,
        ///     <see cref="ICollection&lt;T&gt;"/>,
        ///     <see cref="IList&lt;T&gt;"/>,
        ///     CLR classes with default constructors and public properties with setters and collection properties of the following shapes:
        ///     <see cref="IEnumerable&lt;T&gt;"/> EnumerableProperty  { get; set; },
        ///     <see cref="ICollection&lt;T&gt;"/> CollectionProperty  { get; set; },
        ///     <see cref="IList&lt;T&gt;"/> ListProperty  { get; set; },
        ///     <see cref="ICollection&lt;T&gt;"/> CollectionProperty { get { return this.nonNullCollection; } },
        ///     <see cref="IList&lt;T&gt;"/> ListProperty { get { return this.nonNullList; } }.
        /// </summary>
        /// <typeparam name="T">The CLR type.</typeparam>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>A CLR value converted from <paramref name="edmValue"/>.</returns>
        /// <remarks>This method performs boxing and unboxing for value types. Use value-type specific methods such as <see cref="AsClrString"/> to avoid boxing and unboxing.</remarks>
        public T AsClrValue<T>(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            // convertEnumValues: false -- no need to produce an object of the enum type because
            // the produced underlying value will get converted to the enum type by (T)this.AsClrValue.
            bool convertEnumValues = false;
            return (T)this.AsClrValue(edmValue, typeof(T), convertEnumValues);
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a CLR value of the specified type.
        /// Supported values for <paramref name="clrType"/> are:
        ///     CLR primitive types such as <see cref="System.String"/> and <see cref="System.Int32"/>,
        ///     CLR enum types,
        ///     <see cref="IEnumerable&lt;T&gt;"/>,
        ///     <see cref="ICollection&lt;T&gt;"/>,
        ///     <see cref="IList&lt;T&gt;"/>,
        ///     CLR classes with default constructors and public properties with setters and collection properties of the following shapes:
        ///     <see cref="IEnumerable&lt;T&gt;"/> EnumerableProperty  { get; set; },
        ///     <see cref="ICollection&lt;T&gt;"/> CollectionProperty  { get; set; },
        ///     <see cref="IList&lt;T&gt;"/> ListProperty  { get; set; },
        ///     <see cref="ICollection&lt;T&gt;"/> CollectionProperty { get { return this.nonNullCollection; } },
        ///     <see cref="IList&lt;T&gt;"/> ListProperty { get { return this.nonNullList; } }.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <param name="clrType">The CLR type.</param>
        /// <returns>A CLR value converted from <paramref name="edmValue"/>.</returns>
        /// <remarks>This method performs boxing and unboxing for value types. Use value-type specific methods such as <see cref="AsClrString"/> to avoid boxing and unboxing.</remarks>
        public object AsClrValue(IEdmValue edmValue, Type clrType)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");
            EdmUtil.CheckArgumentNull(clrType, "clrType");

            // convertEnumValues: true -- must produce an object of the requested enum type because there is nothing else
            // down the line that can convert an underlying value to an enum type.
            bool convertEnumValues = true;
            return this.AsClrValue(edmValue, clrType, convertEnumValues);
        }

        /// <summary>
        /// Registers the <paramref name="clrObject"/> corresponding to the <paramref name="edmValue"/>.
        /// All subsequent conversions from this <paramref name="edmValue"/> performed by this instance of <see cref="EdmToClrConverter"/> will return the specified
        /// <paramref name="clrObject"/>. Registration is required to support graph consistency and loops during conversion process.
        /// This method should be called inside the <see cref="TryCreateObjectInstance"/> delegate if the delegate is calling back into <see cref="EdmToClrConverter"/>
        /// in order to populate properties of the <paramref name="clrObject"/>.
        /// </summary>
        /// <param name="edmValue">The EDM value.</param>
        /// <param name="clrObject">The CLR object.</param>
        public void RegisterConvertedObject(IEdmStructuredValue edmValue, object clrObject)
        {
            this.convertedObjects.Add(edmValue, clrObject);
        }

        #region Static converters

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a CLR byte array value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted byte array.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmBinaryValue"/>.</exception>
        internal static byte[] AsClrByteArray(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            if (edmValue is IEdmNullValue)
            {
                return null;
            }

            return ((IEdmBinaryValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.String"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted string.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmStringValue"/>.</exception>
        internal static string AsClrString(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            if (edmValue is IEdmNullValue)
            {
                return null;
            }

            return ((IEdmStringValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Boolean"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted boolean.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmBooleanValue"/>.</exception>
        internal static Boolean AsClrBoolean(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmBooleanValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Int64"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted integer.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmIntegerValue"/>.</exception>
        internal static Int64 AsClrInt64(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmIntegerValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Char"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted char.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmIntegerValue"/>.</exception>
        /// <exception cref="OverflowException">Exception is thrown if <paramref name="edmValue"/> cannot be converted to <see cref="System.Char"/>.</exception>
        internal static Char AsClrChar(IEdmValue edmValue)
        {
            checked
            {
                return (Char)AsClrInt64(edmValue);
            }
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Byte"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted byte.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmIntegerValue"/>.</exception>
        /// <exception cref="OverflowException">Exception is thrown if <paramref name="edmValue"/> cannot be converted to <see cref="System.Byte"/>.</exception>
        internal static Byte AsClrByte(IEdmValue edmValue)
        {
            checked
            {
                return (Byte)AsClrInt64(edmValue);
            }
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Int16"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted integer.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmIntegerValue"/>.</exception>
        /// <exception cref="OverflowException">Exception is thrown if <paramref name="edmValue"/> cannot be converted to <see cref="System.Int16"/>.</exception>
        internal static Int16 AsClrInt16(IEdmValue edmValue)
        {
            checked
            {
                return (Int16)AsClrInt64(edmValue);
            }
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Int32"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted integer.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmIntegerValue"/>.</exception>
        /// <exception cref="OverflowException">Exception is thrown if <paramref name="edmValue"/> cannot be converted to <see cref="System.Int32"/>.</exception>
        internal static Int32 AsClrInt32(IEdmValue edmValue)
        {
            checked
            {
                return (Int32)AsClrInt64(edmValue);
            }
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Double"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted double.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmFloatingValue"/>.</exception>
        internal static Double AsClrDouble(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmFloatingValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Single"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted single.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmFloatingValue"/>.</exception>
        internal static Single AsClrSingle(IEdmValue edmValue)
        {
            return (Single)AsClrDouble(edmValue);
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="Microsoft.OData.Edm.TimeOfDay"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted TimeOfDay.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmTimeOfDayValue"/>.</exception>
        internal static TimeOfDay AsClrTimeOfDay(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmTimeOfDayValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="Microsoft.OData.Edm.Date"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted date.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmDateValue"/>.</exception>
        internal static Date AsClrDate(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmDateValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Decimal"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted decimal.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmDecimalValue"/>.</exception>
        internal static decimal AsClrDecimal(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmDecimalValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.TimeSpan"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted Duration.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmDurationValue"/>.</exception>
        internal static TimeSpan AsClrDuration(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmDurationValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.Guid"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted Guid.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmGuidValue"/>.</exception>
        internal static Guid AsClrGuid(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmGuidValue)edmValue).Value;
        }

        /// <summary>
        /// Converts <paramref name="edmValue"/> to a <see cref="System.DateTimeOffset"/> value.
        /// </summary>
        /// <param name="edmValue">The EDM value to be converted.</param>
        /// <returns>Converted DateTimeOffset.</returns>
        /// <exception cref="InvalidCastException">Exception is thrown if <paramref name="edmValue"/> is not <see cref="IEdmDateTimeOffsetValue"/>.</exception>
        internal static DateTimeOffset AsClrDateTimeOffset(IEdmValue edmValue)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");

            return ((IEdmDateTimeOffsetValue)edmValue).Value;
        }

        #endregion

        #region Private implementation

        private static bool TryConvertAsNonGuidPrimitiveType(Type clrType, IEdmValue edmValue, out object clrValue)
        {
            if (clrType == typeof(Boolean))
            {
                clrValue = AsClrBoolean(edmValue);
                return true;
            }

            if (clrType == typeof(Char))
            {
                clrValue = AsClrChar(edmValue);
                return true;
            }

            if (clrType == typeof(SByte))
            {
                checked
                {
                    clrValue = (SByte)AsClrInt64(edmValue);
                    return true;
                }
            }

            if (clrType == typeof(Byte))
            {
                clrValue = AsClrByte(edmValue);
                return true;
            }

            if (clrType == typeof(Int16))
            {
                clrValue = AsClrInt16(edmValue);
                return true;
            }

            if (clrType == typeof(UInt16))
            {
                checked
                {
                    clrValue = (UInt16)AsClrInt64(edmValue);
                    return true;
                }
            }

            if (clrType == typeof(Int32))
            {
                clrValue = AsClrInt32(edmValue);
                return true;
            }

            if (clrType == typeof(UInt32))
            {
                checked
                {
                    clrValue = (UInt32)AsClrInt64(edmValue);
                    return true;
                }
            }

            if (clrType == typeof(Int64))
            {
                clrValue = AsClrInt64(edmValue);
                return true;
            }

            if (clrType == typeof(UInt64))
            {
                checked
                {
                    clrValue = (UInt64)AsClrInt64(edmValue);
                    return true;
                }
            }

            if (clrType == typeof(Single))
            {
                clrValue = AsClrSingle(edmValue);
                return true;
            }

            if (clrType == typeof(Double))
            {
                clrValue = AsClrDouble(edmValue);
                return true;
            }

            if (clrType == typeof(Decimal))
            {
                clrValue = AsClrDecimal(edmValue);
                return true;
            }

            if (clrType == typeof(String))
            {
                clrValue = AsClrString(edmValue);
                return true;
            }

            clrValue = null;
            return false;
        }

        private static MethodInfo FindICollectionOfElementTypeAddMethod(Type collectionType, Type elementType)
        {
            Type collectionOfElementType = typeof(ICollection<>).MakeGenericType(elementType);
            return collectionOfElementType.GetMethod("Add");
        }

        /// <summary>
        /// Searches the <paramref name="clrObjectType"/> for a property with the <paramref name="propertyName"/>.
        /// Handles the case of multiple properties with the same name (declared via C# "new") by choosing the one on the deepest derived type.
        /// </summary>
        /// <param name="clrObjectType">The clr object type.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The property or null.</returns>
        private PropertyInfo FindProperty(Type clrObjectType, string propertyName)
        {
            if (this.tryGetClrPropertyInfoDelegate != null)
            {
                PropertyInfo propertyInfo = null;
                if (this.tryGetClrPropertyInfoDelegate(clrObjectType, propertyName, out propertyInfo))
                {
                    return propertyInfo;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<PropertyInfo> properties = clrObjectType.GetProperties().Where(p => p.Name == propertyName).ToList();
                switch (properties.Count)
                {
                    case 0:
                        return null;

                    case 1:
                        return properties[0];

                    default:
                        PropertyInfo property = properties[0];
                        for (int i = 1; i < properties.Count; ++i)
                        {
                            PropertyInfo candidate = properties[i];
                            if (property.DeclaringType.IsAssignableFrom(candidate.DeclaringType))
                            {
                                property = candidate;
                            }
                        }

                        return property;
                }
            }
        }

        /// <summary>
        /// Used for error messages only.
        /// </summary>
        /// <param name="edmValue">The EDM value.</param>
        /// <returns>The EDM value interface name.</returns>
        private static string GetEdmValueInterfaceName(IEdmValue edmValue)
        {
            Debug.Assert(edmValue != null, "edmValue != null");

            // We want search to be stable regardless of the order of elements coming from GetInterfaces() method,
            // so we sort first, then find the deepest derived interface descending from IEdmValue.
            Type interfaceType = typeof(IEdmValue);
            foreach (Type candidate in edmValue.GetType().GetInterfaces().OrderBy(i => i.FullName))
            {
                if (interfaceType.IsAssignableFrom(candidate) && interfaceType != candidate)
                {
                    interfaceType = candidate;
                }
            }

            return interfaceType.Name;
        }

        private static bool IsBuiltInOrEnumType(Type type)
        {
            return type.IsPrimitive() || type == typeof(string) || type == typeof(decimal) || type.IsEnum();
        }

        private object AsClrValue(IEdmValue edmValue, Type clrType, bool convertEnumValues)
        {
            if (!IsBuiltInOrEnumType(clrType))
            {
                // First look for nullable primitives, then DateTime, DateTimeOffset and byte[] which don't have dedicated TypeCode, so they are processed here.
                if (clrType.IsGenericType() && clrType.GetGenericTypeDefinition() == TypeNullableOfT)
                {
                    if (edmValue is IEdmNullValue)
                    {
                        return null;
                    }

                    return this.AsClrValue(edmValue, clrType.GetGenericArguments().Single());
                }
                else if (clrType == typeof(Guid))
                {
                    return AsClrGuid(edmValue);
                }
                else if (clrType == typeof(Date))
                {
                    return AsClrDate(edmValue);
                }
                else if (clrType == typeof(DateTimeOffset))
                {
                    return AsClrDateTimeOffset(edmValue);
                }
                else if (clrType == typeof(TimeOfDay))
                {
                    return AsClrTimeOfDay(edmValue);
                }
                else if (clrType == typeof(TimeSpan))
                {
                    return AsClrDuration(edmValue);
                }
                else if (clrType == typeof(byte[]))
                {
                    return AsClrByteArray(edmValue);
                }
                else if (clrType.IsGenericType() && clrType.IsInterface() &&
                         (clrType.GetGenericTypeDefinition() == TypeICollectionOfT ||
                          clrType.GetGenericTypeDefinition() == TypeIListOfT ||
                          clrType.GetGenericTypeDefinition() == TypeIEnumerableOfT))
                {
                    // We are asked to produce an IEnumerable<T>, perform an equivalent of this.AsIEnumerable(edmValue, typeof(T)).Cast<T>().ToList()
                    return this.AsListOfT(edmValue, clrType);
                }
                else
                {
                    return this.AsClrObject(edmValue, clrType);
                }
            }
            else
            {
                // A CLR enum type will report some primitive type code, so we get here.
                // If this is the case and the value is of an edm enumeration type, we want to unbox the primitive type value,
                // otherwise assume the edm value is primitive and let it fail inside the converter if it's not.
                bool isEnum = clrType.IsEnum();
                if (isEnum)
                {
                    Type underlyingType = Enum.GetUnderlyingType(clrType);
                    IEdmEnumValue edmEnumValue = edmValue as IEdmEnumValue;

                    object clrValue = null;
                    if (edmEnumValue != null)
                    {
                        EdmEnumMemberValue memberValue = edmEnumValue.Value as EdmEnumMemberValue;
                        if (memberValue != null
                            && !TryConvertEnumType(underlyingType, memberValue.Value, out clrValue))
                        {
                            throw new InvalidCastException(Strings.EdmToClr_UnsupportedType(underlyingType));
                        }
                    }
                    else if (!TryConvertAsNonGuidPrimitiveType(underlyingType, edmValue, out clrValue))
                    {
                        throw new InvalidCastException(Strings.EdmToClr_UnsupportedType(underlyingType));
                    }

                    // In case of enums, because the converter returns a primitive type value we want to convert it to the CLR enum type.
                    return convertEnumValues ? this.GetEnumValue(clrValue, clrType) : clrValue;
                }

                object nonEnumclrValue = null;
                if (!TryConvertAsNonGuidPrimitiveType(clrType, edmValue, out nonEnumclrValue))
                {
                        throw new InvalidCastException(Strings.EdmToClr_UnsupportedType(clrType));
                }

                return nonEnumclrValue;
            }
        }

        private static bool TryConvertEnumType(Type type, long enumValue, out object clrValue)
        {
            if (type == typeof(SByte))
            {
                clrValue = (SByte)(enumValue);
                return true;
            }

            if (type == typeof(Byte))
            {
                clrValue = (Byte)(enumValue);
                return true;
            }

            if (type == typeof(Int16))
            {
                clrValue = (Int16)(enumValue);
                return true;
            }

            if (type == typeof(UInt16))
            {
                clrValue = (UInt16)(enumValue);
                return true;
            }

            if (type == typeof(Int32))
            {
                clrValue = (Int32)(enumValue);
                return true;
            }

            if (type == typeof(UInt32))
            {
                clrValue = (UInt32)(enumValue);
                return true;
            }

            if (type == typeof(Int64))
            {
                clrValue = (Int64)(enumValue);
                return true;
            }

            clrValue = null;
            return false;
        }

        private object AsListOfT(IEdmValue edmValue, Type clrType)
        {
            Debug.Assert(clrType.IsGenericType(), "clrType.IsGenericType");

            if (edmValue is IEdmNullValue)
            {
                return null;
            }

            Type elementType = clrType.GetGenericArguments().Single();

            MethodInfo enumerableConverter;
            if (!this.enumerableConverters.TryGetValue(elementType, out enumerableConverter))
            {
                enumerableConverter = EnumerableToListOfTMethodInfo.MakeGenericMethod(elementType);
                this.enumerableConverters.Add(elementType, enumerableConverter);
            }

            try
            {
                return enumerableConverter.Invoke(null, new object[] { this.AsIEnumerable(edmValue, elementType) });
            }
            catch (TargetInvocationException targetInvocationException)
            {
                // Unwrap the target invocation exception that masks an interesting invalid cast exception.
                if (targetInvocationException.InnerException != null && targetInvocationException.InnerException is InvalidCastException)
                {
                    throw targetInvocationException.InnerException;
                }
                else
                {
                    throw;
                }
            }
        }

        private object GetEnumValue(object clrValue, Type clrType)
        {
            Debug.Assert(clrType.IsEnum(), "clrType.IsEnum");

            MethodInfo enumTypeConverter;
            if (!this.enumTypeConverters.TryGetValue(clrType, out enumTypeConverter))
            {
                enumTypeConverter = CastToClrTypeMethodInfo.MakeGenericMethod(clrType);
                this.enumTypeConverters.Add(clrType, enumTypeConverter);
            }

            try
            {
                return enumTypeConverter.Invoke(null, new object[] { clrValue });
            }
            catch (TargetInvocationException targetInvocationException)
            {
                // Unwrap the target invocation exception that masks an interesting invalid cast exception.
                if (targetInvocationException.InnerException != null && targetInvocationException.InnerException is InvalidCastException)
                {
                    throw targetInvocationException.InnerException;
                }
                else
                {
                    throw;
                }
            }
        }

        private object AsClrObject(IEdmValue edmValue, Type clrObjectType)
        {
            EdmUtil.CheckArgumentNull(edmValue, "edmValue");
            EdmUtil.CheckArgumentNull(clrObjectType, "clrObjectType");

            if (edmValue is IEdmNullValue)
            {
                return null;
            }

            IEdmStructuredValue edmStructuredValue = edmValue as IEdmStructuredValue;
            if (edmStructuredValue == null)
            {
                if (edmValue is IEdmCollectionValue)
                {
                    throw new InvalidCastException(Strings.EdmToClr_CannotConvertEdmCollectionValueToClrType(clrObjectType.FullName));
                }
                else
                {
                    throw new InvalidCastException(Strings.EdmToClr_CannotConvertEdmValueToClrType(GetEdmValueInterfaceName(edmValue), clrObjectType.FullName));
                }
            }

            object clrObject;

            if (this.convertedObjects.TryGetValue(edmStructuredValue, out clrObject))
            {
                return clrObject;
            }

            // By convention we only support mapping structured values to a CLR class.
            if (!clrObjectType.IsClass())
            {
                throw new InvalidCastException(Strings.EdmToClr_StructuredValueMappedToNonClass);
            }

            // Try user-defined logic before the default logic.
            bool clrObjectInitialized;
            if (this.tryCreateObjectInstanceDelegate != null && this.tryCreateObjectInstanceDelegate(edmStructuredValue, clrObjectType, this, out clrObject, out clrObjectInitialized))
            {
                // The user-defined logic might have produced null, which is Ok, but we need to null the type in to keep them in sync.
                if (clrObject != null)
                {
                    Type newClrObjectType = clrObject.GetType();
                    if (!clrObjectType.IsAssignableFrom(newClrObjectType))
                    {
                        throw new InvalidCastException(Strings.EdmToClr_TryCreateObjectInstanceReturnedWrongObject(newClrObjectType.FullName, clrObjectType.FullName));
                    }

                    clrObjectType = newClrObjectType;
                }
            }
            else
            {
                // Default instance creation logic: use Activator to create the new CLR object from the type.
                clrObject = Activator.CreateInstance(clrObjectType);
                clrObjectInitialized = false;
            }

            // Cache the object before populating its properties as their values might refer to the object.
            this.convertedObjects[edmStructuredValue] = clrObject;

            if (!clrObjectInitialized && clrObject != null)
            {
                this.PopulateObjectProperties(edmStructuredValue, clrObject, clrObjectType);
            }

            return clrObject;
        }

        private void PopulateObjectProperties(IEdmStructuredValue edmValue, object clrObject, Type clrObjectType)
        {
            // Populate properties of the CLR object.
            // All CLR object properties that have no edm counterparts will remain intact.
            // By convention we only support converting from a structured value.
            HashSetInternal<string> populatedProperties = new HashSetInternal<string>();
            foreach (IEdmPropertyValue propertyValue in edmValue.PropertyValues)
            {
                PropertyInfo clrProperty = this.FindProperty(clrObjectType, propertyValue.Name);

                // By convention we ignore an ems property if it has no counterpart on the CLR side.
                if (clrProperty != null)
                {
                    if (populatedProperties.Contains(propertyValue.Name))
                    {
                        throw new InvalidCastException(Strings.EdmToClr_StructuredPropertyDuplicateValue(propertyValue.Name));
                    }

                    if (!this.TrySetCollectionProperty(clrProperty, clrObject, propertyValue))
                    {
                        object convertedClrPropertyValue = this.AsClrValue(propertyValue.Value, clrProperty.PropertyType);
                        clrProperty.SetValue(clrObject, convertedClrPropertyValue, null);
                    }

                    populatedProperties.Add(propertyValue.Name);
                }
            }
        }

        private bool TrySetCollectionProperty(PropertyInfo clrProperty, object clrObject, IEdmPropertyValue propertyValue)
        {
            if (propertyValue.Value is IEdmNullValue)
            {
                return false;
            }

            Type clrPropertyType = clrProperty.PropertyType;

            // Process the following cases:
            // class C1
            // {
            //     IEnumerable<ElementType> EnumerableProperty { get; set; }
            //
            //     ICollection<ElementType> CollectionProperty { get; set; }
            //
            //     IList<ElementType> ListProperty { get; set; }
            //
            //     ICollection<ElementType> CollectionProperty { get { return this.nonNullCollection; } }
            //
            //     IList<ElementType> ListProperty { get { return this.nonNullList; } }
            // }
            if (clrPropertyType.IsGenericType())
            {
                Type genericTypeDefinition = clrPropertyType.GetGenericTypeDefinition();
                bool genericTypeDefinitionIsIEnumerableOfT = genericTypeDefinition == TypeIEnumerableOfT;
                IEnumerable<Type> clrPropertyTypeInterfaces = clrPropertyType.GetInterfaces();
                if (genericTypeDefinitionIsIEnumerableOfT || clrPropertyTypeInterfaces.Any(t => t.GetGenericTypeDefinition() == TypeIEnumerableOfT))
                {
                    object clrPropertyValue = clrProperty.GetValue(clrObject, null);

                    // If property already has a value, we are trying to reuse it and add elements into it (except the case of IEnumerable<T>),
                    // otherwise we create List<elementType>, add elements to it and then assign it to the property.
                    Type elementType = clrPropertyType.GetGenericArguments().Single();
                    Type clrPropertyValueType;
                    if (clrPropertyValue == null)
                    {
                        // If collection property has no value, create an instance of List<elementType> and
                        // assign it to the property.
                        clrPropertyValueType = TypeListOfT.MakeGenericType(elementType);
                        clrPropertyValue = Activator.CreateInstance(clrPropertyValueType);
                        clrProperty.SetValue(clrObject, clrPropertyValue, null);
                    }
                    else
                    {
                        if (genericTypeDefinitionIsIEnumerableOfT)
                        {
                            // Cannot add elements to an existing value of type IEnumerable<T>.
                            throw new InvalidCastException(Strings.EdmToClr_IEnumerableOfTPropertyAlreadyHasValue(clrProperty.Name, clrProperty.DeclaringType.FullName));
                        }

                        clrPropertyValueType = clrPropertyValue.GetType();
                    }

                    // Find the ICollection<elementType>.Add(elementType) method. Note that IList<T> implements
                    MethodInfo clrPropertyValueTypeAddMethod = FindICollectionOfElementTypeAddMethod(clrPropertyValueType, elementType);

                    // Convert the collection elements and add them to the CLR collection.
                    foreach (object convertedElementValue in this.AsIEnumerable(propertyValue.Value, elementType))
                    {
                        try
                        {
                            clrPropertyValueTypeAddMethod.Invoke(clrPropertyValue, new object[] { convertedElementValue });
                        }
                        catch (TargetInvocationException targetInvokationException)
                        {
                            // Unwrap the target invokation exception that masks an interesting invalid cast exception.
                            if (targetInvokationException.InnerException != null && targetInvokationException.InnerException is InvalidCastException)
                            {
                                throw targetInvokationException.InnerException;
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private IEnumerable AsIEnumerable(IEdmValue edmValue, Type elementType)
        {
            // By convention we only support converting from a collection value.
            foreach (IEdmDelayedValue element in ((IEdmCollectionValue)edmValue).Elements)
            {
                if (element.Value != null || elementType.IsGenericType() && elementType.GetGenericTypeDefinition() == TypeNullableOfT)
                {
                    yield return this.AsClrValue(element.Value, elementType);
                }
            }
        }

        /// <summary>
        /// The class contains method that are called thru reflection to produce values of correct CLR types.
        /// For example if one has an int value and a clr type represnting an enum : int, there is no other way to convert the int
        /// to the enum type object.
        /// </summary>
        private static class CastHelper
        {
            public static T CastToClrType<T>(object obj)
            {
                return (T)obj;
            }

            public static List<T> EnumerableToListOfT<T>(IEnumerable enumerable)
            {
                return enumerable.Cast<T>().ToList();
            }
        }

        #endregion
    }
}
