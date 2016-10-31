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

using System.Collections.Generic;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Semantic representation of an EDM model.
    /// </summary>
    /// <remarks>
    /// This interface, and all interfaces reachable from it, preserve certain invariants:
    ///    -- The backing implementation of an element can be loaded or created on demand.
    ///    -- No direct element mutation occurs through the interfaces.
    /// </remarks>
    public interface IEdmModel : IEdmElement
    {
        /// <summary>
        /// Gets the collection of schema elements that are contained in this model.
        /// </summary>
        IEnumerable<IEdmSchemaElement> SchemaElements { get; }

        /// <summary>
        /// Gets the collection of vocabulary annotations that are contained in this model.
        /// </summary>
        IEnumerable<Annotations.IEdmVocabularyAnnotation> VocabularyAnnotations { get; }

        /// <summary>
        /// Gets the collection of models referred to by this model.
        /// </summary>
        IEnumerable<IEdmModel> ReferencedModels { get; }

        /// <summary>
        /// Gets the model's annotations manager.
        /// </summary>
        Annotations.IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager { get; }

        /// <summary>
        /// Searches for an entity container with the given name in this model and returns null if no such entity container exists.
        /// </summary>
        /// <param name="name">The name of the entity container being found.</param>
        /// <returns>The requested entity container, or null if no such entity container exists.</returns>
        IEdmEntityContainer FindDeclaredEntityContainer(string name);

        /// <summary>
        /// Searches for a type with the given name in this model and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        IEdmSchemaType FindDeclaredType(string qualifiedName);

        /// <summary>
        /// Searches for functions with the given name in this model and returns an empty enumerable if no such function exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the function being found.</param>
        /// <returns>A set of functions sharing the specified qualified name, or an empty enumerable if no such function exists.</returns>
        IEnumerable<IEdmFunction> FindDeclaredFunctions(string qualifiedName);

        /// <summary>
        /// Searches for a value term with the given name in this model and returns null if no such value term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the value term being found.</param>
        /// <returns>The requested value term, or null if no such value term exists.</returns>
        IEdmValueTerm FindDeclaredValueTerm(string qualifiedName);

        /// <summary>
        /// Searches for vocabulary annotations specified by this model.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        IEnumerable<Annotations.IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element);

        /// <summary>
        /// Finds a list of types that derive directly from the supplied type.
        /// </summary>
        /// <param name="baseType">The base type that derived types are being searched for.</param>
        /// <returns>A list of types from this model that derive directly from the given type.</returns>
        IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType);
    }
}
