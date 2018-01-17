//---------------------------------------------------------------------
// <copyright file="OperationDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region namespaces

    using System;
    using System.Diagnostics;
    #endregion

    /// <summary> Holds information about a service operation. </summary>
    public abstract class OperationDescriptor : Descriptor
    {
        #region fields

        /// <summary>Maps to m:action\@title. Human-readable description of the service operation.</summary>
        private string title;

        /// <summary>maps to m:action\@metadata. Identifies the service operation.</summary>
        private Uri metadata;

        /// <summary>maps to m:action\@target. The URI to invoke the service operation.</summary>
        private Uri target;
        #endregion

        /// <summary>
        /// Creates a new instance of the Operation descriptor.
        /// </summary>
        internal OperationDescriptor()
            : base(EntityStates.Unchanged)
        {
        }

        #region public properties

        /// <summary>Human-readable description of the service operation.</summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            internal set
            {
                this.title = value;
            }
        }

        /// <summary>Identifies the service operation.</summary>
        public Uri Metadata
        {
            get { return this.metadata; }
            internal set { this.metadata = value; }
        }

        /// <summary>The URI to invoke the service operation.</summary>
        public Uri Target
        {
            get { return this.target; }
            internal set { this.target = value; }
        }

        /// <summary>
        /// this is an operation descriptor.
        /// </summary>
        internal override DescriptorKind DescriptorKind
        {
            get { return DescriptorKind.OperationDescriptor; }
        }

        #endregion

        /// <summary>
        /// Nothing to clear in case of operation descriptor.
        /// </summary>
        internal override void ClearChanges()
        {
            // Do Nothing.
        }
    }
}
