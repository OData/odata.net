//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
{
    /// <summary>
    /// Represents an EDM value.
    /// </summary>
    public abstract class EdmValue : IEdmValue, IEdmDelayedValue
    {
        private readonly IEdmTypeReference type;

        /// <summary>
        /// Initializes a new instance of the EdmValue class.
        /// </summary>
        /// <param name="type">Type of the value.</param>
        protected EdmValue(IEdmTypeReference type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets the type of this value.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public abstract EdmValueKind ValueKind
        {
            get;
        }

        IEdmValue IEdmDelayedValue.Value
        {
            get { return this; }
        }
    }
}
