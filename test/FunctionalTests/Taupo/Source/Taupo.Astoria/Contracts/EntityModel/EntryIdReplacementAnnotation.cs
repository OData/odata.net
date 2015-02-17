//---------------------------------------------------------------------
// <copyright file="EntryIdReplacementAnnotation.cs" company="Microsoft">
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
    /// EntryIdReplacementAnnotation holds all the required for the RelayService to replace entry id in payloads
    /// </summary>
    public class EntryIdReplacementAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the EntryIdReplacementAnnotation class
        /// </summary>
        public EntryIdReplacementAnnotation()
            : base()
        {
            this.Name = string.Empty;
            this.AppendRequestIdToName = false;
        }

        /// <summary>
        /// Initializes a new instance of the EntryIdReplacementAnnotation class
        /// </summary>
        /// <param name="name">Name of EndPoint</param>
        /// <param name="appendRequestIdToName">Whether to append request Id to the annotation name</param>
        public EntryIdReplacementAnnotation(string name, bool appendRequestIdToName)
            : base()
        {
            this.Name = name;
            this.AppendRequestIdToName = appendRequestIdToName;
        }

        /// <summary>
        /// Gets or sets the entry id
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to append request Id after annotation name
        /// </summary>
        public bool AppendRequestIdToName { get; set; }
    }
}