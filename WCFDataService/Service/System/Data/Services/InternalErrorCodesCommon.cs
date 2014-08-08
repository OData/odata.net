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
