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
    /// Defines EDM property types.
    /// </summary>
    public enum EdmPropertyKind
    {
        /// <summary>
        /// Represents a property implementing <see cref="IEdmStructuralProperty"/>.
        /// </summary>
        Structural,

        /// <summary>
        /// Represents a property implementing <see cref="IEdmNavigationProperty"/>. 
        /// </summary>
        Navigation,

        /// <summary>
        /// Represents a property with an unknown or error kind.
        /// </summary>
        None
    }

    /// <summary>
    /// Represents an EDM property.
    /// </summary>
    public interface IEdmProperty : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        EdmPropertyKind PropertyKind { get; }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        IEdmStructuredType DeclaringType { get; }
    }
}
