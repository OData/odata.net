//---------------------------------------------------------------------
// <copyright file="JsonProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents one Json property.
    /// </summary>
    public class JsonProperty : JsonValue
    {
        /// <summary>
        /// Initializes a new instance of the JsonProperty class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public JsonProperty(string name, JsonValue value) : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the fully-qualified type name for the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the property
        /// </summary>
        public JsonValue Value { get; set; }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that does not return a result.
        /// </summary>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        public override void Accept(IJsonValueVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TReturn">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this value.</param>
        /// <returns>The result of visiting this value.</returns>
        public override TReturn Accept<TReturn>(IJsonValueVisitor<TReturn> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Used by Clone method to clone the type specific properties
        /// </summary>
        /// <returns>a clone of the JsonValue</returns>
        protected override JsonValue CloneInternal()
        {
            return new JsonProperty(this.Name, this.Value.Clone());
        }

        /// <summary>
        /// Writes the value into a specified writer in the form of a debug string.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        protected override void WriteToDebugString(System.IO.TextWriter writer)
        {
            writer.Write('"');
            writer.Write(this.Name);
            writer.Write("\": ");
            JsonValue.WriteValueToDebugString(this.Value, writer);
        }
    }
}
