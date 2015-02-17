//---------------------------------------------------------------------
// <copyright file="WorkspaceInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Represents a workspace in a service document.
    /// </summary>
    public sealed class WorkspaceInstance : ODataPayloadElement
    {
        /// <summary>
        /// Initializes a new instance of the WorkspaceInstance class.
        /// </summary>
        public WorkspaceInstance()
        {
            this.ResourceCollections = new List<ResourceCollectionInstance>();
        }

        /// <summary>
        /// Initializes a new instance of the WorkspaceInstance class.
        /// </summary>
        /// <param name="resourceCollections">The initial set of resource collections.</param>
        public WorkspaceInstance(params ResourceCollectionInstance[] resourceCollections)
        {
            this.ResourceCollections = new List<ResourceCollectionInstance>(resourceCollections);
        }

        /// <summary>
        /// Gets or sets the title of the workspace.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the resource collections of the workspace.
        /// </summary>
        public IList<ResourceCollectionInstance> ResourceCollections { get; private set; }

        /// <summary>
        /// Gets a string representation to make debugging easier.
        /// </summary>
        public override string StringRepresentation
        {
            get { return string.Format(CultureInfo.InvariantCulture, "Workspace: {0}", this.Title ?? "<null>"); }
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
