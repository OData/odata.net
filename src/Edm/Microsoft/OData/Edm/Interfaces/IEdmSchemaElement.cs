//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM schema element types.
    /// </summary>
    public enum EdmSchemaElementKind
    {
        /// <summary>
        /// Represents a schema element with unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmSchemaType"/>.
        /// </summary>
        TypeDefinition,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmValueTerm"/>.
        /// </summary>
        ValueTerm,


        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmAction"/>.
        /// </summary>
        Action,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmEntityContainer"/>
        /// </summary>
        EntityContainer,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmAction"/>.
        /// </summary>
        Function,
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
