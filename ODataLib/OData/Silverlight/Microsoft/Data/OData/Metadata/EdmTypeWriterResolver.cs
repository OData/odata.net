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
