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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Represents a reference to an EDM string type.
    /// </summary>
    public interface IEdmStringTypeReference : IEdmPrimitiveTypeReference
    {
        /// <summary>
        /// Gets a value indicating whether this string type specifies fixed length.
        /// </summary>
        bool? IsFixedLength { get; }

        /// <summary>
        /// Gets a value indicating whether this string type specifies the maximum allowed length.
        /// </summary>
        bool IsUnbounded { get; }

        /// <summary>
        /// Gets the maximum length of this string type.
        /// </summary>
        int? MaxLength { get; }

        /// <summary>
        /// Gets a value indicating whether this string type supports unicode encoding.
        /// </summary>
        bool? IsUnicode { get; }

        /// <summary>
        /// Gets a string representing the collation of this string type.
        /// </summary>
        string Collation { get; }
    }
}
