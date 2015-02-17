//---------------------------------------------------------------------
// <copyright file="JsonValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// This is the basic unit. All the other JsonTypes inherit from this.
    /// </summary>
    public abstract class JsonValue : IAnnotatable<JsonAnnotation>
    {
        /// <summary>
        /// Initializes a new instance of the JsonValue class. 
        /// Infers the ElementType using JsonValueType.GetElementType with the current instance's type
        /// </summary>
        protected JsonValue() : base()
        {
            this.JsonType = JsonValue.GetJsonValueType(this.GetType());
            this.Annotations = new List<JsonAnnotation>();
        }

        /// <summary>
        /// Gets the type of the JsonValue as an enum
        /// </summary>
        public JsonValueType JsonType { get; private set; }

        /// <summary>
        /// Gets the list of annotation on this object
        /// </summary>
        public IList<JsonAnnotation> Annotations { get; private set; }

        /// <summary>
        /// Creates a deep clone of the value
        /// </summary>
        /// <returns>a deep clone of the JsonValue</returns>
        public JsonValue Clone()
        {
            JsonValue clone = this.CloneInternal();
            foreach (var annotation in this.Annotations)
            {
                clone.Annotations.Add(annotation.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Writes a string representation of the object. Meant for debug purposes only!
        /// </summary>
        /// <returns>The string representation of the object for debug purposes only.</returns>
        public override string ToString()
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writer.Write("JSON'");
                WriteValueToDebugString(this, writer);
                writer.Write("'");
                return writer.ToString();
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that does not return a result.
        /// </summary>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        public abstract void Accept(IJsonValueVisitor visitor);

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TReturn">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this value.</param>
        /// <returns>The result of visiting this value.</returns>
        public abstract TReturn Accept<TReturn>(IJsonValueVisitor<TReturn> visitor);

        /// <summary>
        /// Returns the Enum value for the given JsonValue type
        /// </summary>
        /// <param name="t">A non-abstract type which derives from JsonValue</param>
        /// <returns>The enum representation for the given type</returns>
        internal static JsonValueType GetJsonValueType(Type t)
        {
            ExceptionUtilities.Assert(typeof(JsonValue).IsAssignableFrom(t), "!typeof(JsonValue).IsAssignableFrom(t)");
            ExceptionUtilities.Assert(Enum.IsDefined(typeof(JsonValueType), t.Name), "!Enum.IsDefined(typeof(JsonValueType), t.Name)");
            return (JsonValueType)Enum.Parse(typeof(JsonValueType), t.Name, false);
        }

        /// <summary>
        /// Helper method to write a specified value to a debug string.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="writer">The writer to write to.</param>
        /// <remarks>This is needed to overcome the rule that derived classes can't call a protected members 
        /// of potentially unrelated instances.</remarks>
        protected static void WriteValueToDebugString(JsonValue value, TextWriter writer)
        {
            value.WriteToDebugString(writer);
        }

        /// <summary>
        /// Used by Clone method to clone the type specific properties
        /// </summary>
        /// <returns>a clone of the JsonValue</returns>
        protected abstract JsonValue CloneInternal();

        /// <summary>
        /// Writes the value into a specified writer in the form of a debug string.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        protected abstract void WriteToDebugString(TextWriter writer);
    }
}
