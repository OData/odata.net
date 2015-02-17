//---------------------------------------------------------------------
// <copyright file="EditMediaLinkReplacementAnnotation.cs" company="Microsoft">
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
    /// EditMediaLinkReplacementAnnotation holds all the required for the RelayService to replace edit-media links in payloads
    /// </summary>
    public class EditMediaLinkReplacementAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the EditMediaLinkReplacementAnnotation class
        /// </summary>
        public EditMediaLinkReplacementAnnotation()
            : base()
        {
            this.Name = string.Empty;
            this.ReverseETag = false;
            this.AppendRequestIdToName = false;
            this.UseRelativeLink = false;
        }

        /// <summary>
        /// Initializes a new instance of the EditMediaLinkReplacementAnnotation class
        /// </summary>
        /// <param name="name">Name of EndPoint</param>
        /// <param name="reverseETag">Whether to reverse the edit-media link ETag</param>
        /// <param name="appendRequestIdToName">Whether to append request Id to the annotation name</param>
        /// <param name="useRelativeLink">Whether to use relative uri</param>
        public EditMediaLinkReplacementAnnotation(string name, bool reverseETag, bool appendRequestIdToName, bool useRelativeLink)
            : base()
        {
            this.Name = name;
            this.ReverseETag = reverseETag;
            this.AppendRequestIdToName = appendRequestIdToName;
            this.UseRelativeLink = useRelativeLink;
        }

        /// <summary>
        /// Gets or sets the edit-media link
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to reverse the ETag string of the edit-media link
        /// </summary>
        public bool ReverseETag { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to append request Id after annotation name
        /// </summary>
        public bool AppendRequestIdToName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use relative Uri
        /// </summary>
        public bool UseRelativeLink { get; set; }
    }
}