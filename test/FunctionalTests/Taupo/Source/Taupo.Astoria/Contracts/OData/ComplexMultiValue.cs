//---------------------------------------------------------------------
// <copyright file="ComplexMultiValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents the value of a complex multi-value property
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
        Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Doesn't need to end in 'Collection'")]
    public class ComplexMultiValue : TypedValueCollection<ComplexInstance>
    {
        /// <summary>
        /// Initializes a new instance of the ComplexMultiValue class
        /// </summary>
        public ComplexMultiValue()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexMultiValue class
        /// </summary>
        /// <param name="fullTypeName">The full type name</param>
        /// <param name="isNull">Whether or not the value is null</param>
        /// <param name="list">the initial contents of the collection</param>
        public ComplexMultiValue(string fullTypeName, bool isNull, params ComplexInstance[] list)
            : base(fullTypeName, isNull, list)
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
