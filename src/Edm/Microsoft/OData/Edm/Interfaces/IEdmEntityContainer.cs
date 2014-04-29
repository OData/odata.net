//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM entity container.
    /// </summary>
    public interface IEdmEntityContainer : IEdmSchemaElement, IEdmVocabularyAnnotatable
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
        /// Searches for a singleton with the given name in this entity container and returns null if no such singleton exists.
        /// </summary>
        /// <param name="singletonName">The name of the singleton to search.</param>
        /// <returns>The requested singleton, or null if the singleton does not exist.</returns>
        IEdmSingleton FindSingleton(string singletonName);

        /// <summary>
        /// Searches for operation imports with the given name in this entity container and returns null if no such operation import exists.
        /// </summary>
        /// <param name="operationName">The name of the operations to find.</param>
        /// <returns>A group of the requested operation imports, or an empty enumerable  if no such operation import exists.</returns>
        IEnumerable<IEdmOperationImport> FindOperationImports(string operationName);
    }
}
