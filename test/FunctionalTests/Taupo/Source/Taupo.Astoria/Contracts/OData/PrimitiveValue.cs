//---------------------------------------------------------------------
// <copyright file="PrimitiveValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents an instance of a primitive type
    /// </summary>
    public class PrimitiveValue : TypedValue
    {
        /// <summary>
        /// Initializes a new instance of the PrimitiveValue class
        /// </summary>
        public PrimitiveValue()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PrimitiveValue class with the given values
        /// </summary>
        /// <param name="fullTypeName">The full type name of the value</param>
        /// <param name="clrValue">the clr value, if null then IsNull will be set to true</param>
        public PrimitiveValue(string fullTypeName, object clrValue)
            : base(fullTypeName, clrValue == null)
        {
            this.ClrValue = clrValue;
        }

        /// <summary>
        /// Gets or sets the value represented as a clr instance
        /// </summary>
        public object ClrValue { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                if (this.ClrValue == null)
                {
                    return "null";
                }

                return this.ClrValue.ToString();
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(IODataPayloadElementVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that does not return a result.
        /// </summary>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        public override void Accept(IODataPayloadElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
