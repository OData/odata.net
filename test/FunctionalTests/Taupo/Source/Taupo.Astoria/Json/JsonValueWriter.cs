//---------------------------------------------------------------------
// <copyright file="JsonValueWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Json
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Helper class for writing json values to a json writer
    /// </summary>
    public static class JsonValueWriter
    {
        /// <summary>
        /// Writes the value to the writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="convertPrimitive">The callback for converting primitives to strings</param>
        public static void WriteJsonValue(this JsonWriter writer, JsonValue value, Func<object, string> convertPrimitive)
        {
            new JsonValueWritingVisitor(writer, convertPrimitive).Write(value);
        }

        internal class JsonValueWritingVisitor : IJsonValueVisitor
        {
            private readonly JsonWriter writer;
            private readonly Func<object, string> convertPrimitive;

            public JsonValueWritingVisitor(JsonWriter writer, Func<object, string> convertPrimitive)
            {
                ExceptionUtilities.CheckArgumentNotNull(writer, "writer");
                ExceptionUtilities.CheckArgumentNotNull(convertPrimitive, "convertPrimitive");
                this.writer = writer;
                this.convertPrimitive = convertPrimitive;
            }

            /// <summary>
            /// Writes the value to the writer given at construction time
            /// </summary>
            /// <param name="value">The value to write</param>
            public void Write(JsonValue value)
            {
                ExceptionUtilities.CheckArgumentNotNull(value, "value");
                value.Accept(this);
            }

            /// <summary>
            /// Visits a json object.
            /// </summary>
            /// <param name="jsonObject">The json object being visited.</param>
            public void Visit(JsonObject jsonObject)
            {
                ExceptionUtilities.CheckArgumentNotNull(jsonObject, "jsonObject");
                this.writer.StartObjectScope();
                jsonObject.Properties.ForEach(p => p.Accept(this));
                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a json array.
            /// </summary>
            /// <param name="jsonArray">The json array being visited.</param>
            public void Visit(JsonArray jsonArray)
            {
                ExceptionUtilities.CheckArgumentNotNull(jsonArray, "jsonArray");
                this.writer.StartArrayScope();
                jsonArray.Elements.ForEach(e => e.Accept(this));
                this.writer.EndScope();
            }

            /// <summary>
            /// Visits a json primitive value.
            /// </summary>
            /// <param name="primitive">The json primitive being visited.</param>
            public void Visit(JsonPrimitiveValue primitive)
            {
                ExceptionUtilities.CheckArgumentNotNull(primitive, "primitive");
                this.writer.WriteRaw(this.convertPrimitive(primitive.Value));
            }

            /// <summary>
            /// Visits a json property.
            /// </summary>
            /// <param name="jsonProperty">The json property being visited.</param>
            public void Visit(JsonProperty jsonProperty)
            {
                ExceptionUtilities.CheckArgumentNotNull(jsonProperty, "jsonProperty");
                this.writer.WriteName(jsonProperty.Name);
                jsonProperty.Value.Accept(this);
            }
        }
    }
}