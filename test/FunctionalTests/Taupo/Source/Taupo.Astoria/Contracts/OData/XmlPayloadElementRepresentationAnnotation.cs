//---------------------------------------------------------------------
// <copyright file="XmlPayloadElementRepresentationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// An annotation which stores the exact XML representation of an ODataPayloadElement.
    /// </summary>
    public class XmlPayloadElementRepresentationAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets or sets the XML representing the payload element the annotation is on.
        /// </summary>
        public IEnumerable<XNode> XmlNodes { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return (this.XmlNodes == null) ? "XML representation missing." : "XML representation: " +
                    string.Join(Environment.NewLine, this.XmlNodes.Select(n => n.ToString()));
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new XmlPayloadElementRepresentationAnnotation { XmlNodes = (this.XmlNodes == null) ? null : new List<XNode>(this.XmlNodes.Select(n => n.DeepClone())) };
        }
    }
}
