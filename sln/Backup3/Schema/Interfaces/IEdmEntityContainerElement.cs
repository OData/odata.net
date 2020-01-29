//---------------------------------------------------------------------
// <copyright file="IEdmEntityContainerElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM container element types.
    /// </summary>
    public enum EdmContainerElementKind
    {
        /// <summary>
        /// Represents an element where the container kind is unknown or in error.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmEntitySet"/>.
        /// </summary>
        EntitySet,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmActionImport"/>.
        /// </summary>
        ActionImport,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmFunctionImport"/>.
        /// </summary>
        FunctionImport,

        /// <summary>
        /// Represents an element implementing <see cref="IEdmSingleton"/>.
        /// </summary>
        Singleton
    }

    /// <summary>
    /// Represents the common elements of all EDM entity container elements.
    /// </summary>
    public interface IEdmEntityContainerElement : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the kind of element of this container element.
        /// </summary>
        EdmContainerElementKind ContainerElementKind { get; }

        /// <summary>
        /// Gets the container that contains this element.
        /// </summary>
        IEdmEntityContainer Container { get; }
    }
}
