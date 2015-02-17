//---------------------------------------------------------------------
// <copyright file="EntitySetInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Represents a collection of entity instances
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "EntitySet already implies it is a collection")]
    public class EntitySetInstance : ODataPayloadElementCollection<EntityInstance>
    {
        /// <summary>
        /// Initializes a new instance of the EntitySetInstance class
        /// </summary>
        public EntitySetInstance()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EntitySetInstance class
        /// </summary>
        /// <param name="list">the initial contents of the collection</param>
        public EntitySetInstance(params EntityInstance[] list)
            : base(list)
        {
        }

        /// <summary>
        /// Gets or sets the next-page link for the collection
        /// </summary>
        public string NextLink { get; set; }

        /// <summary>
        /// Gets or sets the inline count value for the collection
        /// </summary>
        public long? InlineCount { get; set; }

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
