//---------------------------------------------------------------------
// <copyright file="ResourceCollectionInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// Represents a resource collection in a workspace.
    /// </summary>
    public sealed class ResourceCollectionInstance : ODataPayloadElement
    {
        /// <summary>
        /// Gets or sets the title of the resource collection.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource collection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the HREF of the resource collection.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Gets a string representation to make debugging easier.
        /// </summary>
        public override string StringRepresentation
        {
            get { return string.Format(CultureInfo.InvariantCulture, "Collection: {0}", this.Title ?? "<null>"); }
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
