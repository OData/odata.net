//---------------------------------------------------------------------
// <copyright file="EdmTypeWriterResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Metadata
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Responsible for resolving the element type of an entity set with writer semantics.
    /// </summary>
    internal sealed class EdmTypeWriterResolver : EdmTypeResolver
    {
        /// <summary>
        /// Singleton instance of the resolver.
        /// </summary>
        internal static EdmTypeWriterResolver Instance = new EdmTypeWriterResolver();

        /// <summary>
        /// Private constructor to ensure all access goes through the singleton Instance.
        /// </summary>
        private EdmTypeWriterResolver()
        {
        }

        /// <summary>Returns the entity type of the given navigation source.</summary>
        /// <param name="navigationSource">The navigation source to get the entity type of.</param>
        /// <returns>The <see cref="IEdmEntityType"/> representing the entity type of the <paramref name="navigationSource" />.</returns>
        internal override IEdmEntityType GetElementType(IEdmNavigationSource navigationSource)
        {
            return navigationSource.EntityType();
        }

        /// <summary>
        /// Returns the return type of the given operation import.
        /// </summary>
        /// <param name="operationImport">The operation import to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="operationImport"/>.</returns>
        internal override IEdmTypeReference GetReturnType(IEdmOperationImport operationImport)
        {
            return operationImport == null ? null : operationImport.Operation.ReturnType;
        }

        /// <summary>
        /// Returns the return type of the given operation import group.
        /// </summary>
        /// <param name="functionImportGroup">The operation import group to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="functionImportGroup"/>.</returns>
        internal override IEdmTypeReference GetReturnType(IEnumerable<IEdmOperationImport> functionImportGroup)
        {
            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EdmTypeWriterResolver_GetReturnTypeForOperationImportGroup));
        }

        /// <summary>
        /// Gets the operation parameter type for write.
        /// </summary>
        /// <param name="operationParameter">The operation parameter to resolve the type for.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> representing the type on the operation parameter; or null if no such type could be found.</returns>
        internal override IEdmTypeReference GetParameterType(IEdmOperationParameter operationParameter)
        {
            return operationParameter == null ? null : operationParameter.Type;
        }
    }
}