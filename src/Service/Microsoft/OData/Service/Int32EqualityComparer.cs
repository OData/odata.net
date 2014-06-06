//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System.Collections.Generic;

    /// <summary>This class implements IEqualityComparer for System.In32.</summary>
    /// <remarks>
    /// Using this class rather than EqualityComparer&lt;T&gt;.Default 
    /// saves from JIT'ing it in each AppDomain.
    /// </remarks>
    internal class Int32EqualityComparer : IEqualityComparer<int>
    {
        /// <summary>Checks whether two numbers are equal.</summary>
        /// <param name='x'>First number.</param><param name='y'>Second number.</param>
        /// <returns>true if x equals y; false otherwise.</returns>
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        /// <summary>Gets a hash code for the specified number.</summary>
        /// <param name='obj'>Value.</param>
        /// <returns>The hash code for the specified value.</returns>
        public int GetHashCode(int obj)
        {
            return obj;
        }
    }
}
