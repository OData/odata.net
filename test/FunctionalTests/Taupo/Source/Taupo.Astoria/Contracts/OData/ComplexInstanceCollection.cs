//---------------------------------------------------------------------
// <copyright file="ComplexInstanceCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents a collection of complex instances
    /// </summary>
    public class ComplexInstanceCollection : ODataPayloadElementCollection<ComplexInstance>
    {
        /// <summary>
        /// Initializes a new instance of the ComplexInstanceCollection class
        /// </summary>
        public ComplexInstanceCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexInstanceCollection class
        /// </summary>
        /// <param name="values">The initial set of elements</param>
        public ComplexInstanceCollection(params ComplexInstance[] values)
            : base(values)
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
