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

#if !INTERNAL_DROP

namespace Microsoft.Data.OData
{
    /// <summary>
    /// An enumeration that lists the internal errors that are shared between the OData library and the query library.
    /// </summary>
    internal enum InternalErrorCodesCommon
    {
        /// <summary>Unreachable codepath in EdmLibraryExtensions.ToTypeReference (unsupported type kind).</summary>
        EdmLibraryExtensions_ToTypeReference,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.ToClrType (unsupported type kind).</summary>
        EdmLibraryExtensions_ToClrType,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.PrimitiveTypeReference (unsupported primitive type kind).</summary>
        EdmLibraryExtensions_PrimitiveTypeReference,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.IsAssignableFrom(IEdmPrimitiveType, IEdmPrimitiveType).</summary>
        EdmLibraryExtensions_IsAssignableFrom_Primitive,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.IsAssignableFrom(IEdmType, IEdmType).</summary>
        EdmLibraryExtensions_IsAssignableFrom_Type,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.BaseType.</summary>
        EdmLibraryExtensions_BaseType,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.Clone for unexpected type kind.</summary>
        EdmLibraryExtensions_Clone_TypeKind,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.Clone for unexpected primitive type kind.</summary>
        EdmLibraryExtensions_Clone_PrimitiveTypeKind,
    }
}
#endif
