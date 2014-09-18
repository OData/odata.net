//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
