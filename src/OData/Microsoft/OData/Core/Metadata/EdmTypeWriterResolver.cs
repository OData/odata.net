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
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "IEntitySetBase.ElementType is allowed here and the writer code paths should call this method to get to the ElementType of a set.")]
        internal override IEdmEntityType GetElementType(IEdmNavigationSource navigationSource)
        {
            return navigationSource.EntityType();
        }

        /// <summary>
        /// Returns the return type of the given operation import.
        /// </summary>
        /// <param name="operationImport">The operation import to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="operationImport"/>.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "operationImport.ReturnType is allowed here and the writer code paths should call this method to get to the ReturnType of a operation import.")]
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
            throw new ODataException(OData.Core.Strings.General_InternalError(InternalErrorCodes.EdmTypeWriterResolver_GetReturnTypeForOperationImportGroup));
        }

        /// <summary>
        /// Gets the operation parameter type for write.
        /// </summary>
        /// <param name="operationParameter">The operation parameter to resolve the type for.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> representing the type on the operation parameter; or null if no such type could be found.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "operationParameter.Type is allowed here and the writer code paths should call this method to get to the Type of a function parameter.")]
        internal override IEdmTypeReference GetParameterType(IEdmOperationParameter operationParameter)
        {
            return operationParameter == null ? null : operationParameter.Type;
        }
    }
}
