//---------------------------------------------------------------------
// <copyright file="CollectionTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation to represent the type to use when generating code for a collection property
    /// </summary>
    public class CollectionTypeAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the full type name
        /// </summary>
        public string FullTypeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the type is generic
        /// </summary>
        public bool IsGeneric { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use this type for declaration or instantiation
        /// </summary>
        public CodeGenerationTypeUsage Usage { get; set; }
    }
}