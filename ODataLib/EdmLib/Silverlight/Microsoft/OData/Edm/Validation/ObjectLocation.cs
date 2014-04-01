//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// Defines an object as a location of itself.
    /// </summary>
    public class ObjectLocation : EdmLocation
    {
        internal ObjectLocation(object obj)
        {
            this.Object = obj;
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        public object Object { get; private set; }

        /// <summary>
        /// Gets a string representation of the location.
        /// </summary>
        /// <returns>A string representation of the location.</returns>
        public override string ToString()
        {
            return "(" + this.Object.ToString() + ")";
        }
    }
}
