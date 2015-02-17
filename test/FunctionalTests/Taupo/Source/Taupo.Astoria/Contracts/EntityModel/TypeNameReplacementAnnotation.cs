//---------------------------------------------------------------------
// <copyright file="TypeNameReplacementAnnotation.cs" company="Microsoft">
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
    /// TypeNameReplacementAnnotation holds all the required for the RelayService to replace server type name in payloads
    /// </summary>
    public class TypeNameReplacementAnnotation : CompositeAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the TypeNameReplacementAnnotation class
        /// </summary>
        public TypeNameReplacementAnnotation()
            : base()
        {
            this.ReverseOriginal = false;
            this.AppendRequestIdToName = false;
        }

        /// <summary>
        /// Initializes a new instance of the TypeNameReplacementAnnotation class
        /// </summary>
        /// <param name="reverseOriginal">Whether to reverse the type name string</param>
        /// <param name="appendRequestIdToName">Whether to append request Id to the annotation name</param>
        public TypeNameReplacementAnnotation(bool reverseOriginal, bool appendRequestIdToName)
            : base()
        {
            this.ReverseOriginal = reverseOriginal;
            this.AppendRequestIdToName = appendRequestIdToName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to reverse the type name string
        /// </summary>
        public bool ReverseOriginal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to append request Id after annotation name
        /// </summary>
        public bool AppendRequestIdToName { get; set; }
    }
}