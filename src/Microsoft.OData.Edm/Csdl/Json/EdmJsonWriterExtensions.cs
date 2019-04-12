//---------------------------------------------------------------------
// <copyright file="EdmJsonWriterExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Extensions for <see cref="IEdmJsonWriter"/>.
    /// </summary>
    internal static class EdmJsonWriterExtensions
    {
        /// <summary>
        /// Write the required property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this IEdmJsonWriter writer, string name, string value)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        /// <summary>
        /// Write the required property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this IEdmJsonWriter writer, string name, long value)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        /// <summary>
        /// Write the required property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this IEdmJsonWriter writer, string name, bool value)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        /// <summary>
        /// Write the required property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteRequiredProperty(this IEdmJsonWriter writer, string name, decimal value)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="toString">The value to string Func.</param>
        public static void WriteRequiredProperty<T>(this IEdmJsonWriter writer, string name, T value, Func<T, string> toString)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            writer.WritePropertyName(name);
            writer.WriteValue(toString(value));
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteOptionalProperty(this IEdmJsonWriter writer, string name, int? value)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value.Value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default value.</param>
        public static void WriteOptionalProperty(this IEdmJsonWriter writer, string name, int? value, int defaultValue)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            if (value != null && value.Value != defaultValue)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value.Value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default value.</param>
        public static void WriteOptionalProperty(this IEdmJsonWriter writer, string name, bool value, bool defaultValue)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            if (value != defaultValue)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default value.</param>
        public static void WriteOptionalProperty(this IEdmJsonWriter writer, string name, bool? value, bool defaultValue)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            if (value != null && value.Value != defaultValue)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value.Value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteOptionalProperty(this IEdmJsonWriter writer, string name, string value)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(value);
            }
        }

        /// <summary>
        /// Write the optional property (name/value).
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="toString">The value to string func.</param>
        public static void WriteOptionalProperty<T>(this IEdmJsonWriter writer, string name, T value, Func<T, string> toString)
        {
            EdmUtil.CheckArgumentNull(writer, "writer");

            if (value != null)
            {
                writer.WritePropertyName(name);
                writer.WriteValue(toString(value));
            }
        }
    }
}