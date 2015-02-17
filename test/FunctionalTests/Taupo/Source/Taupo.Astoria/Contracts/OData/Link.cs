//---------------------------------------------------------------------
// <copyright file="Link.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Abstract representation of a link.
    /// </summary>
    public abstract class Link : ODataPayloadElement
    {
        /// <summary>
        /// Gets or sets the uri string value for the link
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "string is more accurate")]
        public string UriString { get; set; }
    }
}
