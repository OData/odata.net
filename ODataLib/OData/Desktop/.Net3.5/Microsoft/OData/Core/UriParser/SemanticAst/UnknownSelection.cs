//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    /// <summary>
    /// Singleton sentinal instance of <see cref="Selection"/> that indicates that $select has not been processed yet. Should never be exposed to the user.
    /// </summary>
    internal sealed class UnknownSelection : Selection
    {
        /// <summary>
        /// Singleton instance of <see cref="UnknownSelection"/>.
        /// </summary>
        public static readonly UnknownSelection Instance = new UnknownSelection();

        /// <summary>
        /// Creates the singleton instance of this class.
        /// </summary>
        private UnknownSelection()
        {
        }
    }
}
