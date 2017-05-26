//---------------------------------------------------------------------
// <copyright file="EdmTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Metadata
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