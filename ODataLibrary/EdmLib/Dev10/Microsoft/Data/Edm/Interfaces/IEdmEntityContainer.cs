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

using System.Collections.Generic;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents an EDM entity container.
    /// </summary>
    public interface IEdmEntityContainer : IEdmNamedElement
    {
        /// <summary>
        /// Gets a collection of the elements of this entity container.
        /// </summary>
        IEnumerable<IEdmEntityContainerElement> Elements { get; }

        /// <summary>
        /// Searches for an entity set with the given name in this entity container and returns null if no such set exists.
        /// </summary>
        /// <param name="setName">The name of the element being found.</param>
        /// <returns>The requested element, or null if the element does not exist.</returns>
        IEdmEntitySet FindEntitySet(string setName);

        /// <summary>
        /// Searches for an association set with the given name in this entity container and returns null if no such set exists.
        /// </summary>
        /// <param name="setName">The name of the element being found.</param>
        /// <returns>The requested element, or null if the element does not exist.</returns>
        IEdmAssociationSet FindAssociationSet(string setName);

        /// <summary>
        /// Searches for function imports with the given name in this entity container and returns null if no such function import exists.
        /// </summary>
        /// <param name="functionName">The name of the function import being found.</param>
        /// <returns>A group of the requested function imports, or null if no such function import exists.</returns>
        IEnumerable<IEdmFunctionImport> FindFunctionImports(string functionName);
    }
}
