//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;

    /// <summary> Represents a parameter associated with a service action.  </summary>
    public sealed class BodyOperationParameter : OperationParameter
    {
        /// <summary> Instantiates a new BodyOperationParameter </summary>
        /// <param name="name">The name of the body operation parameter.</param>
        /// <param name="value">The value of the body operation parameter.</param>
        public BodyOperationParameter(string name, Object value)
            : base(name, value)
        {
        }
    }
}
