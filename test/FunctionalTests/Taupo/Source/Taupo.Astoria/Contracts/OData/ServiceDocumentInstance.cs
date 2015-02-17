//---------------------------------------------------------------------
// <copyright file="ServiceDocumentInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Represents service documents
    /// </summary>
    public sealed class ServiceDocumentInstance : ODataPayloadElement
    {
        /// <summary>
        /// Initializes a new instance of the ServiceDocumentInstance class.
        /// </summary>
        public ServiceDocumentInstance()
        {
            this.Workspaces = new List<WorkspaceInstance>();
        }

        /// <summary>
        /// Initializes a new instance of the ServiceDocumentInstance class.
        /// </summary>
        /// <param name="workspaces">The initial set of workspaces.</param>
        public ServiceDocumentInstance(params WorkspaceInstance[] workspaces)
        {
            this.Workspaces = new List<WorkspaceInstance>(workspaces);
        }

        /// <summary>
        /// Gets the list of workspaces in this service document.
        /// </summary>
        public IList<WorkspaceInstance> Workspaces { get; private set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "Service document: {0} workspaces", this.Workspaces.Count);
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
