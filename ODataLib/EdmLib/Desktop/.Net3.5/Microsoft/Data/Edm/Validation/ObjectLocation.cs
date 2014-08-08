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

namespace Microsoft.Data.Edm.Validation
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
