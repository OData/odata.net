//---------------------------------------------------------------------
// <copyright file="EditLinkReplacementAnnotation.cs" company="Microsoft">
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
    /// EditLinkReplacementAnnotation holds all the required for the RelayService to replace edit links in payloads
    /// </summary>
    public class EditLinkReplacementAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the EditLinkReplacementAnnotation class
        /// </summary>
        public EditLinkReplacementAnnotation()
            : base()
        {
            this.Name = string.Empty;
            this.AppendRequestIdToName = false;
            this.UseRelativeLink = false;
        }

        /// <summary>
        /// Initializes a new instance of the EditLinkReplacementAnnotation class
        /// </summary>
        /// <param name="name">Name of EndPoint</param>
        /// <param name="appendRequestIdToName">Whether to append request Id to the annotation name</param>
        /// <param name="useRelativeLink">Whether to use relative uri</param>
        public EditLinkReplacementAnnotation(string name, bool appendRequestIdToName, bool useRelativeLink) 
            : base()
        {
            this.Name = name;
            this.AppendRequestIdToName = appendRequestIdToName;
            this.UseRelativeLink = useRelativeLink;
        }

        /// <summary>
        /// Gets or sets the edit link value
        /// </summary>
        public string Name { get; set; }

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