//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
