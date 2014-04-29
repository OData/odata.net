//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Response from SaveChanges.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010", Justification = "required for this feature")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710", Justification = "required for this feature")]
    public sealed class ChangeOperationResponse : OperationResponse
    {
        /// <summary>Descriptor containing the response object.</summary>
        private Descriptor descriptor;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.ChangeOperationResponse" /> class. </summary>
        /// <param name="headers">HTTP headers</param>
        /// <param name="descriptor">response object containing information about resources that got changed.</param>
        internal ChangeOperationResponse(HeaderCollection headers, Descriptor descriptor)
            : base(headers)
        {
            Debug.Assert(descriptor != null, "descriptor != null");
            this.descriptor = descriptor;
        }

        /// <summary>Gets the <see cref="T:Microsoft.OData.Client.EntityDescriptor" /> or <see cref="T:Microsoft.OData.Client.LinkDescriptor" /> modified by a change operation.</summary>
        /// <returns>An <see cref="T:Microsoft.OData.Client.EntityDescriptor" /> or <see cref="T:Microsoft.OData.Client.LinkDescriptor" /> modified by a change operation. </returns>
        public Descriptor Descriptor
        {
            get { return this.descriptor; }
        }
    }
}
