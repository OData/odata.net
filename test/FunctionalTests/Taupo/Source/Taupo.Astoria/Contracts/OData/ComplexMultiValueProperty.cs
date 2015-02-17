//---------------------------------------------------------------------
// <copyright file="ComplexMultiValueProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents a complex collection property
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
        Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
    public class ComplexMultiValueProperty : TypedProperty<ComplexMultiValue>
    {
        /// <summary>
        /// Initializes a new instance of the ComplexMultiValueProperty class
        /// </summary>
        public ComplexMultiValueProperty()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexMultiValueProperty class
        /// </summary>
        /// <param name="name">The property's name</param>
        /// <param name="collection">The property's value</param>
        public ComplexMultiValueProperty(string name, ComplexMultiValue collection)
            : base(name, collection)
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
