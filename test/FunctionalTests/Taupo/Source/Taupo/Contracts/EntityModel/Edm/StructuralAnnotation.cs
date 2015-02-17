//---------------------------------------------------------------------
// <copyright file="StructuralAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using System;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Structural Annotation: XElement
    /// </summary>
    public class StructuralAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the StructuralAnnotation class.
        /// </summary>
        public StructuralAnnotation()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the StructuralAnnotation class.
        /// </summary>
        /// <param name="content">The content.</param>
        public StructuralAnnotation(XElement content)
        {
            this.Content = content;
        }

        /// <summary>
        /// Gets or sets the content of StructuralAnnotation
        /// </summary>
        public XElement Content { get; set; }
    }
}
