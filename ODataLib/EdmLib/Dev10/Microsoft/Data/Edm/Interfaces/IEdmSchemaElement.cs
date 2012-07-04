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
    /// Defines EDM schema element types.
    /// </summary>
    public enum EdmSchemaElementKind
    {
        /// <summary>
        /// Represents a schema element with unknown or error kind.
        /// </summary>
        None,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmSchemaType"/>.
        /// </summary>
        TypeDefinition,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmFunction"/>.
        /// </summary>
        Function,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmValueTerm"/>.
        /// </summary>
        ValueTerm, 

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmEntityContainer"/>
        /// </summary>
        EntityContainer
    }

    /// <summary>
    /// Common base interface for all named children of EDM schemata.
    /// </summary>
    public interface IEdmSchemaElement : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        EdmSchemaElementKind SchemaElementKind { get; }

        /// <summary>
        /// Gets the namespace this schema element belongs to.
        /// </summary>
        string Namespace { get; }
    }
}
