//---------------------------------------------------------------------
// <copyright file="IEdmDirectValueAnnotationBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents the combination of an EDM annotation with an immediate value and the element to which it is attached.
    /// </summary>
    public interface IEdmDirectValueAnnotationBinding
    {
        /// <summary>
        /// Gets the element to which the annotation is attached
        /// </summary>
        IEdmElement Element { get; }

        /// <summary>
        /// Gets the namespace URI of the annotation.
        /// </summary>
        string NamespaceUri { get; }

        /// <summary>
        /// Gets the local name of this annotation.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of this annotation.
        /// </summary>
        object Value { get; }
    }
}
