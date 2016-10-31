//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Metadata
{
    using System.Collections.Generic;
    using Microsoft.Data.Edm;

    /// <summary>
    /// Contract for providing implementations of more specific lookups needed for parsing OData URIs that are not efficiently answered
    /// by existing APIs in <see cref="IEdmModel"/> and its related interfaces.
    /// </summary>
    public interface IODataUriParserModelExtensions
    {
        /// <summary>
        /// Finds all function imports with the given name which are bindable to an instance of the giving binding type or a more derived type.
        /// </summary>
        /// <param name="bindingType">The binding entity type.</param>
        /// <param name="functionImportName">The name of the function imports to find. May be qualified with an entity container name.</param>
        /// <returns>The function imports that match the search criteria.</returns>
        IEnumerable<IEdmFunctionImport> FindFunctionImportsByBindingParameterTypeHierarchy(IEdmType bindingType, string functionImportName);

        /// <summary>
        /// Finds an entity set given a name that may be container qualified. If no container name is provided, the default container should be used.
        /// </summary>
        /// <param name="containerQualifiedEntitySetName">The name which might be container qualified. If no container name is provided, the default container should be used.</param>
        /// <returns>The entity set if one was found or null.</returns>
        IEdmEntitySet FindEntitySetFromContainerQualifiedName(string containerQualifiedEntitySetName);

        /// <summary>
        /// Finds a service operation for the given name.
        /// </summary>
        /// <param name="serviceOperationName">The name of the service operation to find. May be qualified with an entity container name.</param>
        /// <returns>The function import representing a service operation or null if one could not be found with the given name.</returns>
        IEdmFunctionImport FindServiceOperation(string serviceOperationName);

        /// <summary>
        /// Finds a function or action bound to the specific type with the given name.
        /// </summary>
        /// <param name="bindingType">The binding type.</param>
        /// <param name="functionImportName">The name of the function imports to find. May be qualified with an entity container name.</param>
        /// <param name="nonBindingParameterNamesFromUri">The parameter names of the non-binding parameters, if provided in the request URI.</param>
        /// <returns>The function import that matches the search criteria or null if there was no match.</returns>
        IEdmFunctionImport FindFunctionImportByBindingParameterType(IEdmType bindingType, string functionImportName, IEnumerable<string> nonBindingParameterNamesFromUri);
    }
}
