//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client
{
    #region namespaces
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
