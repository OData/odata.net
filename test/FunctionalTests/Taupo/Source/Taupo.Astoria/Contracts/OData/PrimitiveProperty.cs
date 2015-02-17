//---------------------------------------------------------------------
// <copyright file="PrimitiveProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents a primitive property
    /// </summary>
    public class PrimitiveProperty : TypedProperty<PrimitiveValue>
    {
        /// <summary>
        /// Initializes a new instance of the PrimitiveProperty class
        /// </summary>
        public PrimitiveProperty()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PrimitiveProperty class
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="propertyTypeName">The type name of the property's value</param>
        /// <param name="propertyValue">The clr value of the property</param>
        public PrimitiveProperty(string name, string propertyTypeName, object propertyValue)
            : this(name, new PrimitiveValue(propertyTypeName, propertyValue))
        {
        }

        /// <summary>
        /// Initializes a new instance of the PrimitiveProperty class
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="propertyValue">The value of the property</param>
        public PrimitiveProperty(string name, PrimitiveValue propertyValue)
            : base(name, propertyValue)
        {
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
