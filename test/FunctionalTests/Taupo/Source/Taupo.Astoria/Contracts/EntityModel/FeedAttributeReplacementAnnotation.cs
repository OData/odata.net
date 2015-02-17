//---------------------------------------------------------------------
// <copyright file="FeedAttributeReplacementAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// FeedAttributeReplacementAnnotation holds all the required for the RelayService to replace feed attributes in payloads
    /// </summary>
    public class FeedAttributeReplacementAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the FeedAttributeReplacementAnnotation class
        /// </summary>
        public FeedAttributeReplacementAnnotation()
            : base()
        {
            this.ModifyXmlBase = null;
        }

        /// <summary>
        /// Initializes a new instance of the FeedAttributeReplacementAnnotation class
        /// </summary>
        /// <param name="modifyXmlBase">The value used to modify xml base attribute</param>
        public FeedAttributeReplacementAnnotation(string modifyXmlBase)
            : base()
        {
            this.ModifyXmlBase = modifyXmlBase;
        }

        /// <summary>
        /// Gets or sets a value used to change the XML base attribute
        /// </summary>
        public string ModifyXmlBase { get; set; }
    }
}