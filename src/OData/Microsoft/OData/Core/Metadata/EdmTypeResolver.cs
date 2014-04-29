//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Metadata
{
    #region Namespaces

    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for determining the entity type of an entity set.
    /// </summary>
    internal abstract class EdmTypeResolver
    {
        /// <summary>
        /// Returns the entity type of the given navigation source.
        /// </summary>
        /// <param name="navigationSource">The navigation source to get the entity type of.</param>
        /// <returns>The <see cref="IEdmEntityType"/> representing the entity type of the <paramref name="navigationSource" />.</returns>
        internal abstract IEdmEntityType GetElementType(IEdmNavigationSource navigationSource);

        /// <summary>
        /// Returns the return type of the given operation import.
        /// </summary>
        /// <param name="operationImport">The operation import to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="operationImport"/>.</returns>
        internal abstract IEdmTypeReference GetReturnType(IEdmOperationImport operationImport);

        /// <summary>
        /// Returns the return type of the given operation import group.
        /// </summary>
        /// <param name="functionImportGroup">The operation import group to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="functionImportGroup"/>.</returns>
        internal abstract IEdmTypeReference GetReturnType(IEnumerable<IEdmOperationImport> functionImportGroup);

        /// <summary>
        /// Gets the function parameter type.
        /// </summary>
        /// <param name="operationParameter">The function parameter to get the type for.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> representing the type on the function parameter; or null if no such type could be found.</returns>
        internal abstract IEdmTypeReference GetParameterType(IEdmOperationParameter operationParameter);
    }
}
