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
    /// Defines EDM term kinds.
    /// </summary>
    public enum EdmTermKind
    {
        /// <summary>
        /// Represents a term with unknown or error kind.
        /// </summary>
        None,
        /// <summary>
        /// Represents a term implementing <see cref="IEdmEntityType"/>.
        /// </summary>
        Type,
        /// <summary>
        /// Represents a term implementing <see cref="IEdmValueTerm"/>.
        /// </summary>
        Value
    }

    /// <summary>
    /// Term to which an annotation can bind.
    /// </summary>
    public interface IEdmTerm : IEdmNamedElement
    {
        /// <summary>
        /// Gets the kind of a term.
        /// </summary>
        EdmTermKind TermKind { get; }

        /// <summary>
        /// Gets the namespace of this term.
        /// </summary>
        string NamespaceUri { get; }
    }
}
