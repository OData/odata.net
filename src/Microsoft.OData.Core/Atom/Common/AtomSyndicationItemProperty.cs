//---------------------------------------------------------------------
// <copyright file="AtomSyndicationItemProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    /// <summary>Enumeration type that is used to identify the syndication item element or attribute in the Open Data Protocol (OData) feed to which an entity property is mapped.</summary>
        /// <remarks>
        /// Potentially the following atom specific elements could also be considered:
        /// * Content?
        /// * Id
        /// * Source?
        /// </remarks>
    public enum AtomSyndicationItemProperty
    {
        /// <summary>
        /// User specified a non-syndication property
        /// </summary>
        CustomProperty,

        /// <summary>
        /// author/email
        /// </summary>
        AuthorEmail,

        /// <summary>
        /// author/name
        /// </summary>
        AuthorName,         // Author*

        /// <summary>
        /// author/uri
        /// </summary>
        AuthorUri,

        /// <summary>
        /// contributor/email
        /// </summary>
        ContributorEmail,

        /// <summary>
        /// contributor/name
        /// </summary>
        ContributorName,    // Contributor*

        /// <summary>
        /// contributor/uri
        /// </summary>
        ContributorUri,

        /// <summary>
        /// updated
        /// </summary>
        Updated,

        /// <summary>
        /// published
        /// </summary>
        Published,

        /// <summary>
        /// rights
        /// </summary>
        Rights,

        /// <summary>
        /// summary
        /// </summary>
        Summary,

        /// <summary>
        /// title
        /// </summary>
        Title,
    }
}
