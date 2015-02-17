//---------------------------------------------------------------------
// <copyright file="EntryAttributeReplacementAnnotation.cs" company="Microsoft">
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
    /// EntryAttributeReplacementAnnotation holds all the required for the RelayService to replace entry attributes in payloads
    /// </summary>
    public class EntryAttributeReplacementAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the EntryAttributeReplacementAnnotation class
        /// </summary>
        public EntryAttributeReplacementAnnotation()
            : base()
        {
            this.ReverseETag = false;
            this.AppendRequestIdToName = false;
            this.ModifyXmlBase = null;
        }

        /// <summary>
        /// Initializes a new instance of the EntryAttributeReplacementAnnotation class
        /// </summary>
        /// <param name="reverseETag">Whether to reverse the Entry attribute ETag</param>
        /// <param name="appendRequestIdToName">Whether to append request Id to the annotation name</param>
        /// <param name="modifyXmlBase">Whether to modify xml base attribute</param>
        public EntryAttributeReplacementAnnotation(bool reverseETag, bool appendRequestIdToName, string modifyXmlBase)
            : base()
        {
            this.ReverseETag = reverseETag;
            this.AppendRequestIdToName = appendRequestIdToName;
            this.ModifyXmlBase = modifyXmlBase;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to reverse the attribute ETag string
        /// </summary>
        public bool ReverseETag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to append request Id after attribute value
        /// </summary>
        public bool AppendRequestIdToName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the XML base attribute should be changed
        /// </summary>
        public string ModifyXmlBase { get; set; }
    }
}