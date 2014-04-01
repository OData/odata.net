//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Class that represents a selection of all properties and functions on an entity.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1052:StaticHolderTypesShouldBeSealed", Justification = "This class can't be static because it inherits from selection")]
    internal sealed class AllSelection : Selection
    {
        /// <summary>
        /// Singleton instance of <see cref="AllSelection"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "AllSelection is immutable")]
        public static readonly AllSelection Instance = new AllSelection();

        /// <summary>
        /// Creates the singleton instance of this class.
        /// </summary>
        private AllSelection()
        {
        }
    }
}
