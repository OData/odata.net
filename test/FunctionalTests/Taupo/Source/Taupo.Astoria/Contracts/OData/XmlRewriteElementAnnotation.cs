//---------------------------------------------------------------------
// <copyright file="XmlRewriteElementAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// An annotation that stores a rewrite function that will be applied to
    /// the resultant XElement during conversion.
    /// </summary>
    public class XmlRewriteElementAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets or sets the rewrite function
        /// </summary>
        public Func<XElement, XNode> RewriteFunction { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get 
            {
                return "Rewrite function: " + (this.RewriteFunction == null ? "missing" : "present"); 
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new XmlRewriteElementAnnotation { RewriteFunction = this.RewriteFunction };
        }
    }
}
