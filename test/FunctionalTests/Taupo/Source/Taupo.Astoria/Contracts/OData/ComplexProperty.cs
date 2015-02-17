//---------------------------------------------------------------------
// <copyright file="ComplexProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents a property containing a complex instance
    /// </summary>
    public class ComplexProperty : TypedProperty<ComplexInstance>
    {
        /// <summary>
        /// Initializes a new instance of the ComplexProperty class
        /// </summary>
        public ComplexProperty()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexProperty class
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="value">The property value</param>
        public ComplexProperty(string name, ComplexInstance value)
            : base(name, value)
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
