//---------------------------------------------------------------------
// <copyright file="IEdmIncludeAnnotations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The interface of includeAnnotation information for referenced model.
    /// </summary>
    public interface IEdmIncludeAnnotations : IEdmElement
    {
        /// <summary>
        /// Gets the term namespace to include.
        /// </summary>
        string TermNamespace { get; }

        /// <summary>
        /// Gets the Qualifier.
        /// </summary>
        string Qualifier { get; }

        /// <summary>
        /// Gets the target namespace.
        /// </summary>
        string TargetNamespace { get; }
    }
}
