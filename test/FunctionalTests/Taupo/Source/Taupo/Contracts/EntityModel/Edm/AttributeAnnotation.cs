//---------------------------------------------------------------------
// <copyright file="AttributeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Attribute annotation: Name-Value pair
    /// </summary>
    public sealed class AttributeAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the AttributeAnnotation class.
        /// </summary>
        public AttributeAnnotation()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AttributeAnnotation class.
        /// </summary>
        /// <param name="content">The content.</param>
        public AttributeAnnotation(XAttribute content)
        {
            this.Content = content;
        }

        /// <summary>
        /// Gets or sets the content of the annotation.
        /// </summary>
        public XAttribute Content { get; set; }
    }
}
