//---------------------------------------------------------------------
// <copyright file="PageSizeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Annotation for marking the page size of a set
    /// </summary>
    public class PageSizeAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }
    }
}