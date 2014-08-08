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

namespace System.Data.Services
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
