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
