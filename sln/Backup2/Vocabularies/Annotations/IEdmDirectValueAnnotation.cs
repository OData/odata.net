//---------------------------------------------------------------------
// <copyright file="IEdmDirectValueAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM annotation with an immediate value.
    /// </summary>
    public interface IEdmDirectValueAnnotation : IEdmNamedElement
    {
        /// <summary>
        /// Gets the namespace Uri of the annotation.
        /// </summary>
        string NamespaceUri { get; }

        /// <summary>
        /// Gets the value of this annotation.
        /// </summary>
        object Value { get; }
    }
}
