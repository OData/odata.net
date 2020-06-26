//---------------------------------------------------------------------
// <copyright file="Utf8JsonWriterExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Text.Json;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Extensions for <see cref="Utf8JsonWriter"/>.
    /// </summary>
    internal static class Utf8JsonWriterExtensions
    {
        /// <summary>
        /// Write the required property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this Utf8JsonWriter writer, string name, string value)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            writer.WritePropertyName(name);
            writer.WriteStringValue(value);
        }

        /// <summary>
        /// Write the required property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this Utf8JsonWriter writer, string name, long value)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            writer.WritePropertyName(name);
            writer.WriteNumberValue(value);
        }

        /// <summary>
        /// Write the required property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this Utf8JsonWriter writer, string name, bool value)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            writer.WritePropertyName(name);
            writer.WriteBooleanValue(value);
        }

        /// <summary>
        /// Write the required property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this Utf8JsonWriter writer, string name, decimal value)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            writer.WritePropertyName(name);
            writer.WriteNumberValue(value);
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="getStringFunc">The value to string Func.</param>
        public static void WriteRequiredProperty<T>(this Utf8JsonWriter writer, string name, T value, Func<T, string> getStringFunc)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            writer.WritePropertyName(name);
            writer.WriteStringValue(getStringFunc(value));
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteOptionalProperty(this Utf8JsonWriter writer, string name, int? value)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default value.</param>
        public static void WriteOptionalProperty(this Utf8JsonWriter writer, string name, int? value, int defaultValue)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            if (value != null && value.Value != defaultValue)
            {
                writer.WritePropertyName(name);
                writer.WriteNumberValue(value.Value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default value.</param>
        public static void WriteOptionalProperty(this Utf8JsonWriter writer, string name, bool value, bool defaultValue)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            if (value != defaultValue)
            {
                writer.WritePropertyName(name);
                writer.WriteBooleanValue(value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default value.</param>
        public static void WriteOptionalProperty(this Utf8JsonWriter writer, string name, bool? value, bool defaultValue)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            if (value != null && value.Value != defaultValue)
            {
                writer.WritePropertyName(name);
                writer.WriteBooleanValue(value.Value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteOptionalProperty(this Utf8JsonWriter writer, string name, string value)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteStringValue(value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="getStringFunc">The value to string func.</param>
        public static void WriteOptionalProperty<T>(this Utf8JsonWriter writer, string name, T value, Func<T, string> getStringFunc)
        {
            EdmUtil.CheckArgumentNull(writer, nameof(writer));

            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteStringValue(getStringFunc(value));
            }
        }
    }
}
#endif