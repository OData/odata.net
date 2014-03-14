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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
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

        /// <summary>Returns the element type of the given entity set.</summary>
        /// <param name="entitySet">The entity set to get the element type of.</param>
        /// <returns>The <see cref="IEdmEntityType"/> representing the element type of the <paramref name="entitySet" />.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "entitySet.ElementType is allowed here and the writer code paths should call this method to get to the ElementType of a set.")]
        internal override IEdmEntityType GetElementType(IEdmEntitySet entitySet)
        {
            DebugUtils.CheckNoExternalCallers();

            return entitySet == null ? null : entitySet.ElementType;
        }

        /// <summary>
        /// Returns the return type of the given function import.
        /// </summary>
        /// <param name="functionImport">The function import to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="functionImport"/>.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "functionImport.ReturnType is allowed here and the writer code paths should call this method to get to the ReturnType of a function import.")]
        internal override IEdmTypeReference GetReturnType(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();

            return functionImport == null ? null : functionImport.ReturnType;
        }

        /// <summary>
        /// Returns the return type of the given function import group.
        /// </summary>
        /// <param name="functionImportGroup">The function import group to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="functionImportGroup"/>.</returns>
        internal override IEdmTypeReference GetReturnType(IEnumerable<IEdmFunctionImport> functionImportGroup)
        {
            DebugUtils.CheckNoExternalCallers();
            throw new ODataException(OData.Strings.General_InternalError(InternalErrorCodes.EdmTypeWriterResolver_GetReturnTypeForFunctionImportGroup));
        }

        /// <summary>
        /// Gets the function parameter type for write.
        /// </summary>
        /// <param name="functionParameter">The function parameter to resolve the type for.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> representing the type on the function parameter; or null if no such type could be found.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "functionParameter.Type is allowed here and the writer code paths should call this method to get to the Type of a function parameter.")]
        internal override IEdmTypeReference GetParameterType(IEdmFunctionParameter functionParameter)
        {
            DebugUtils.CheckNoExternalCallers();

            return functionParameter == null ? null : functionParameter.Type;
        }
    }
}
