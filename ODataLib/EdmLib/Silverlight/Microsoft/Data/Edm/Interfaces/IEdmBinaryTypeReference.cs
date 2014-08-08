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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents a reference to an EDM binary type.
    /// </summary>
    public interface IEdmBinaryTypeReference : IEdmPrimitiveTypeReference
    {
        /// <summary>
        /// Gets a value indicating whether this type specifies fixed length.
        /// </summary>
        bool? IsFixedLength { get; }

        /// <summary>
        /// Gets a value indicating whether this type specifies the maximum allowed length.
        /// </summary>
        bool IsUnbounded { get; }

        /// <summary>
        /// Gets the maximum length of this type.
        /// </summary>
        int? MaxLength { get; }
    }
}
