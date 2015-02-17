//---------------------------------------------------------------------
// <copyright file="JsonArray.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represent one Json Array. 
    /// </summary>
    /// <remarks>This class honors the original ordering of the inner elements of the json array.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class JsonArray : JsonValue, IEnumerable
    {
        /// <summary>
        /// The collection of child values. 
        /// This is not exposed publicly to enforce use of Add method
        /// </summary>
        private List<JsonValue> elements = new List<JsonValue>();

        /// <summary>
        /// Gets the Element values for this array type
        /// </summary>
        public ReadOnlyCollection<JsonValue> Elements
        {
            get
            {
                return this.elements.AsReadOnly();
            }
        }

        /// <summary>
        /// Adds the given child to the array
        /// </summary>
        /// <param name="toAdd">The child to add</param>
        public void Add(JsonValue toAdd)
        {
            ExceptionUtilities.Assert(toAdd.JsonType != JsonValueType.JsonProperty, "Cannot add JsonProperty type to JsonArray");
            this.elements.Add(toAdd);
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }

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
            var clone = new JsonArray();
            foreach (JsonValue element in this.elements)
            {
                clone.Add(element.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Writes the value into a specified writer in the form of a debug string.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        protected override void WriteToDebugString(System.IO.TextWriter writer)
        {
            writer.WriteLine("[");
            bool isFirst = true;
            foreach (JsonValue element in this.Elements)
            {
                if (!isFirst)
                {
                    writer.Write(", ");
                }

                isFirst = false;
                JsonValue.WriteValueToDebugString(element, writer);
            }

            writer.WriteLine();
            writer.Write("]");
        }
    }
}
