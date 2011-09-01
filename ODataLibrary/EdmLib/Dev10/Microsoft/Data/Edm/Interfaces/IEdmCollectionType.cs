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
    /// Represents a definition of an EDM collection type.
    /// </summary>
    public interface IEdmCollectionType : IEdmType
    {
        /// <summary>
        /// Gets the element type of this collection.
        /// </summary>
        IEdmTypeReference ElementType { get; }

        /// <summary>
        /// Gets a value indicating whether a collection should be treated as an atomic unit. If true, the type will be serialized to a CSDL as MultiValue, if false it will be serialized as Collection
        /// </summary>
        bool IsAtomic { get; }
    }
}
