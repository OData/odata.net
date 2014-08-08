//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Client
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

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.Client.ChangeOperationResponse" /> class. </summary>
        /// <param name="headers">HTTP headers</param>
        /// <param name="descriptor">response object containing information about resources that got changed.</param>
        internal ChangeOperationResponse(HeaderCollection headers, Descriptor descriptor)
            : base(headers)
        {
            Debug.Assert(descriptor != null, "descriptor != null");
            this.descriptor = descriptor;
        }

        /// <summary>Gets the <see cref="T:System.Data.Services.Client.EntityDescriptor" /> or <see cref="T:System.Data.Services.Client.LinkDescriptor" /> modified by a change operation.</summary>
        /// <returns>An <see cref="T:System.Data.Services.Client.EntityDescriptor" /> or <see cref="T:System.Data.Services.Client.LinkDescriptor" /> modified by a change operation. </returns>
        public Descriptor Descriptor
        {
            get { return this.descriptor; }
        }
    }
}
