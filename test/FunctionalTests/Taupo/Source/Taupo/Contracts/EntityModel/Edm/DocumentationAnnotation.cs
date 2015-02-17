//---------------------------------------------------------------------
// <copyright file="DocumentationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Documentation in the EDM metadata
    /// </summary>
    public class DocumentationAnnotation : Annotation
    {
        /// <summary>
        /// Gets or sets Summary of the Documentation element
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets LongDescription of the Documentation element
        /// </summary>
        public string LongDescription { get; set; }
    }
}