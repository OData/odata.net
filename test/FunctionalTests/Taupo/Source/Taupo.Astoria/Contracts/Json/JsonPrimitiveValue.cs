//---------------------------------------------------------------------
// <copyright file="JsonPrimitiveValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a single Json Primitive.
    /// </summary>
    public class JsonPrimitiveValue : JsonValue
    {
        /// <summary>
        /// Initializes a new instance of the JsonPrimitiveValue class with the given values
        /// </summary>
        /// <param name="clrValue">The clr value</param>
        public JsonPrimitiveValue(object clrValue) : base()
        {
            this.Value = clrValue;
        }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public object Value { get; set; }

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
            return new JsonPrimitiveValue(this.Value);
        }

        /// <summary>
        /// Writes the value into a specified writer in the form of a debug string.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        protected override void WriteToDebugString(System.IO.TextWriter writer)
        {
            if (this.Value == null)
            {
                writer.Write("null");
                return;
            }

            string stringValue = this.Value as string;
            if (stringValue != null)
            {
                writer.Write('"');
                writer.Write(stringValue);
                writer.Write('"');
            }
            else
            {
                writer.Write(this.Value.ToString());
            }
        }
    }
}
