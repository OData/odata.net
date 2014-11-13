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

    using System.Collections.Generic;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Class responsible for determining the entity type of an entity set.
    /// </summary>
    internal abstract class EdmTypeResolver
    {
        /// <summary>
        /// Returns the element type of the given entity set.
        /// </summary>
        /// <param name="entitySet">The entity set to get the element type of.</param>
        /// <returns>The <see cref="IEdmEntityType"/> representing the element type of the <paramref name="entitySet" />.</returns>
        internal abstract IEdmEntityType GetElementType(IEdmEntitySet entitySet);

        /// <summary>
        /// Returns the return type of the given function import.
        /// </summary>
        /// <param name="functionImport">The function import to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="functionImport"/>.</returns>
        internal abstract IEdmTypeReference GetReturnType(IEdmFunctionImport functionImport);

        /// <summary>
        /// Returns the return type of the given function import group.
        /// </summary>
        /// <param name="functionImportGroup">The function import group to get the return type from.</param>
        /// <returns>The <see cref="IEdmType"/> representing the return type fo the <paramref name="functionImportGroup"/>.</returns>
        internal abstract IEdmTypeReference GetReturnType(IEnumerable<IEdmFunctionImport> functionImportGroup);

        /// <summary>
        /// Gets the function parameter type.
        /// </summary>
        /// <param name="functionParameter">The function parameter to get the type for.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> representing the type on the function parameter; or null if no such type could be found.</returns>
        internal abstract IEdmTypeReference GetParameterType(IEdmFunctionParameter functionParameter);
    }
}
