//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData.Query.SemanticAst
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
