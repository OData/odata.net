//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Metadata
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Contract for providing implementations of more specific lookups needed for parsing OData URIs that are not efficiently answered
    /// by existing APIs in <see cref="IEdmModel"/> and its related interfaces.
    /// </summary>
    public interface IODataUriParserModelExtensions
    {
        /// <summary>
        /// Finds all operation imports with the given name which are bindable to an instance of the giving binding type or a more derived type.
        /// </summary>
        /// <param name="bindingType">The binding entity type.</param>
        /// <param name="functionImportName">The name of the operation imports to find. May be qualified with an entity container name.</param>
        /// <returns>The operation imports that match the search criteria.</returns>
        IEnumerable<IEdmOperationImport> FindFunctionImportsByBindingParameterTypeHierarchy(IEdmType bindingType, string functionImportName);

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
        /// <returns>The operation import representing a service operation or null if one could not be found with the given name.</returns>
        IEdmOperationImport FindServiceOperation(string serviceOperationName);

        /// <summary>
        /// Finds a function or action bound to the specific type with the given name.
        /// </summary>
        /// <param name="bindingType">The binding type.</param>
        /// <param name="functionImportName">The name of the operation imports to find. May be qualified with an entity container name.</param>
        /// <param name="nonBindingParameterNamesFromUri">The parameter names of the non-binding parameters, if provided in the request URI.</param>
        /// <returns>The operation import that matches the search criteria or null if there was no match.</returns>
        IEdmOperationImport FindFunctionImportByBindingParameterType(IEdmType bindingType, string functionImportName, IEnumerable<string> nonBindingParameterNamesFromUri);
    }
}
