//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
