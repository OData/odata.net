//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Semantic representation of an EDM model.
    /// </summary>
    /// <remarks>
    /// This interface, and all interfaces reachable from it, preserve certain invariants:
    ///    -- The backing implementation of an element can be loaded or created on demand.
    ///    -- No direct element mutation occurs through the interfaces.
    /// Only the MainModel and ReferencedModels properties are for referneced models scenario. all other properties and methods only focus on this model, not main/sibling/referenced models .
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
        /// Gets the collection of models referred to by this model (mainly by the this.References).
        /// </summary>
        IEnumerable<IEdmModel> ReferencedModels { get; }

        /// <summary>
        /// Gets the collection of namespaces that schema elements use contained in this model.
        /// </summary>
        IEnumerable<string> DeclaredNamespaces { get; }

        /// <summary>
        /// Gets the model's annotations manager.
        /// </summary>
        Annotations.IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager { get; }

        /// <summary>
        /// Gets the only one entity container of the model.
        /// </summary>
        IEdmEntityContainer EntityContainer { get; }

        /// <summary>
        /// Searches for a type with the given name in this model only and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        IEdmSchemaType FindDeclaredType(string qualifiedName);

        /// <summary>
        /// Searches for bound operations based on the binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>A set of operations that share the binding type or empty enumerable if no such operation exists.</returns>
        IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType);

        /// <summary>
        /// Searches for bound operations based on the qualified name and binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation.</param>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>A set of operations that share the qualified name and binding type or empty enumerable if no such operation exists.</returns>
        IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType);

        /// <summary>
        /// Searches for operations with the given name in this model and returns an empty enumerable if no such operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation being found.</param>
        /// <returns>A set of operations sharing the specified qualified name, or an empty enumerable if no such operation exists.</returns>
        IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName);

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
