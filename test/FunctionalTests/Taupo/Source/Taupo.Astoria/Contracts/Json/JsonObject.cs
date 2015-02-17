//---------------------------------------------------------------------
// <copyright file="JsonObject.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represent one Json Object. 
    /// </summary>
    /// <remarks>This class honors the original ordering of the inner elements of the json object.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class JsonObject : JsonValue, IEnumerable
    {
        /// <summary>
        /// The collection of property values. 
        /// This is not exposed publicly to enforce use of Add/Remove methods
        /// </summary>
        private List<JsonProperty> properties = new List<JsonProperty>();

        /// <summary>
        /// Initializes a new instance of the JsonObject class
        /// </summary>
        public JsonObject()
            : base()
        {
        }

        /// <summary>
        /// Gets the property values for this JsonObject type
        /// </summary>
        public IEnumerable<JsonProperty> Properties
        {
            get
            {
                return this.properties.AsEnumerable();
            }
        }

        /// <summary>
        /// Adds the given property to the JsonObject
        /// </summary>
        /// <param name="toAdd">The property to add</param>
        public void Add(JsonProperty toAdd)
        {
            this.properties.Add(toAdd);
        }

        /// <summary>
        /// Removes all properties from this JsonObject type
        /// </summary>
        public void Clear()
        {
            this.properties.Clear();
        }

        /// <summary>
        /// Inserts the given property into the given index location.
        /// </summary>
        /// <param name="index">Index to insert the property at</param>
        /// <param name="toInsert">The property to insert</param>
        public void Insert(int index, JsonProperty toInsert)
        {
            this.properties.Insert(index, toInsert);
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
            var clone = new JsonObject();
            foreach (JsonProperty property in this.properties)
            {
                clone.Add((JsonProperty)property.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Writes the value into a specified writer in the form of a debug string.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        protected override void WriteToDebugString(System.IO.TextWriter writer)
        {
            writer.WriteLine("{");
            bool isFirst = true;
            foreach (JsonProperty property in this.Properties)
            {
                if (!isFirst)
                {
                    writer.Write(", ");
                }

                isFirst = false;
                JsonValue.WriteValueToDebugString(property, writer);
            }

            writer.WriteLine();
            writer.Write("}");
        }
    }
}
