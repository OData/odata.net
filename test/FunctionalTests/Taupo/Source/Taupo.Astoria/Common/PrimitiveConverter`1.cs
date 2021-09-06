//---------------------------------------------------------------------
// <copyright file="PrimitiveConverter`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Handles converting primitive values between CLR instances and a wire representation
    /// </summary>
    /// <typeparam name="TWire">The type of the wire representation, usually string or byte[]</typeparam>
    [CLSCompliant(false)]
    public abstract class PrimitiveConverter<TWire>
    {
        /// <summary>
        /// Returns whether the given type is a primitive type
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if it is a primitive type, false otherwise</returns>
        public virtual bool IsPrimitiveType(Type type)
        {
            MethodInfo method = typeof(PrimitiveConverter<TWire>).GetMethod("Serialize", new Type[] { type });
            return method != null;
        }

        /// <summary>
        /// Infers the type of the primitive using reflection and invokes the appropriate serialize method
        /// Throws if the value is not of an appropriate type
        /// </summary>
        /// <param name="value">The primitive value to serialize</param>
        /// <returns>The wire representation of the primitive</returns>
        public virtual TWire SerializePrimitive(object value)
        {
            if (value == null || value is DBNull)
            {
                return this.SerializeNull();
            }

            Type valueType = value.GetType();

            // find a serialize method of the right type
            MethodInfo method = typeof(PrimitiveConverter<TWire>).GetMethod("Serialize", new Type[] { valueType });
            ExceptionUtilities.CheckObjectNotNull(method, "Cannot serialize object of type '{0}'", valueType.FullName);

            // invoke the method
            return (TWire)method.Invoke(this, new object[] { value });
        }

        /// <summary>
        /// Uses the given target type to invoke the appropriate deserialize method
        /// Throws if the given type is not valid
        /// </summary>
        /// <param name="value">The wire representation of a primitive value</param>
        /// <param name="targetType">The type to deserialize into</param>
        /// <returns>A clr instance of the primitive value</returns>
        public virtual object DeserializePrimitive(TWire value, Type targetType)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetType, "targetType");

            bool nullableType = false;

            // if its nullable, just get the generic argument
            if (targetType.IsGenericType() &&
                targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                nullableType = true;
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            if ((nullableType || !targetType.IsValueType()) && this.ValueIsNull(value, targetType))
            {
                return null;
            }

            // find a method with the right return type and a single parameter of type TWire
            MethodInfo method = typeof(PrimitiveConverter<TWire>).GetMethods()
                .SingleOrDefault(m => m.ReturnType == targetType 
                    && m.Name.StartsWith("Deserialize", StringComparison.Ordinal)
                    && m.GetParameters().Count(p => p.ParameterType == typeof(TWire)) == 1);
            ExceptionUtilities.CheckObjectNotNull(method, "Cannot deserialize into target type '{0}'", targetType.FullName);

            // invoke the method
            object result = method.Invoke(this, new object[] { value });

            // ensure its of the right type before returning it
            ExceptionUtilities.Assert(targetType.IsInstanceOfType(result), "Result of deserialization was not of expected type");

            if (nullableType)
            {
                Type nullable = typeof(Nullable<>).MakeGenericType(targetType);
                ConstructorInfo constructor = nullable.GetInstanceConstructor(true, new Type[] { targetType });
                ExceptionUtilities.CheckObjectNotNull(constructor, "Cannot find constructor for Nullable<> with type argument: " + targetType.FullName);
                result = constructor.Invoke(new object[] { result });
            }

            return result;
        }

        /// <summary>
        /// Serialize a null value
        /// </summary>
        /// <returns>Wire representation of null</returns>
        public abstract TWire SerializeNull();

        /// <summary>
        /// Serialize a boolean value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(bool value);

        /// <summary>
        /// Serialize a byte value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(byte value);

        /// <summary>
        /// Serialize a char value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(char value);

        /// <summary>
        /// Serialize a decimal value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(decimal value);

        /// <summary>
        /// Serialize a double value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(double value);

        /// <summary>
        /// Serialize a float value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(float value);

        /// <summary>
        /// Serialize a short value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(short value);

        /// <summary>
        /// Serialize an int value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(int value);

        /// <summary>
        /// Serialize a long value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(long value);

        /// <summary>
        /// Serialize an sbyte value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(sbyte value);

        /// <summary>
        /// Serialize a string value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(string value);

        /// <summary>
        /// Serialize a binary value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(byte[] value);

        /// <summary>
        /// Serialize a datetime value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(DateTime value);

        /// <summary>
        /// Serialize a guid value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(Guid value);

        /// <summary>
        /// Serialize a datetimeoffset value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(DateTimeOffset value);

        /// <summary>
        /// Serialize a TimeSpan value
        /// </summary>
        /// <param name="value">The value as a clr instance</param>
        /// <returns>The wire representation of the value</returns>
        public abstract TWire Serialize(TimeSpan value);

        /// <summary>
        /// Deserialize a boolean value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool", 
            Justification = "Method is intended to be used with boolean values only")]
        public abstract bool DeserializeBool(TWire value);

        /// <summary>
        /// Deserialize a byte value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract byte DeserializeByte(TWire value);

        /// <summary>
        /// Deserialize a char value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract char DeserializeChar(TWire value);

        /// <summary>
        /// Deserialize a decimal value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract decimal DeserializeDecimal(TWire value);

        /// <summary>
        /// Deserialize a double value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract double DeserializeDouble(TWire value);

        /// <summary>
        /// Deserialize a float value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float",
            Justification = "Method is meant to be used with floats only")]
        public abstract float DeserializeFloat(TWire value);

        /// <summary>
        /// Deserialize a short value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract short DeserializeInt16(TWire value);

        /// <summary>
        /// Deserialize an int value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract int DeserializeInt32(TWire value);

        /// <summary>
        /// Deserialize a long value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract long DeserializeInt64(TWire value);

        /// <summary>
        /// Deserialize an sbyte value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract sbyte DeserializeSByte(TWire value);

        /// <summary>
        /// Deserialize a string value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract string DeserializeString(TWire value);

        /// <summary>
        /// Deserialize a binary value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract byte[] DeserializeBinary(TWire value);

        /// <summary>
        /// Deserialize a datetime value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract DateTime DeserializeDateTime(TWire value);

        /// <summary>
        /// Deserialize a guid value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract Guid DeserializeGuid(TWire value);

        /// <summary>
        /// Deserialize a datetimeoffset value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract DateTimeOffset DeserializeDateTimeOffset(TWire value);

        /// <summary>
        /// Deserialize a timespan value
        /// </summary>
        /// <param name="value">The wire representation of the value</param>
        /// <returns>the value as a clr instance</returns>
        public abstract TimeSpan DeserializeTimeSpan(TWire value);

        /// <summary>
        /// Determines whether the given serialized value represents null for the given type
        /// </summary>
        /// <param name="value">The serialized value to compare to null</param>
        /// <param name="targetType">The type that the value represents</param>
        /// <returns>Whether or not the value represents null</returns>
        protected abstract bool ValueIsNull(TWire value, Type targetType);
    }
}