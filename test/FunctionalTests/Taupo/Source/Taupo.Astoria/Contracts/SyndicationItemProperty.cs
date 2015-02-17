//---------------------------------------------------------------------
// <copyright file="SyndicationItemProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// Used in place of Microsoft.OData.Service.Common.SyndicationItemProperty to avoid a direct dependency on the product
    /// Should never be converted by value, because the values may be compiler-assigned on either side, and could change between builds.
    /// </summary>
    public enum SyndicationItemProperty
    {
        /// <summary>
        /// User specified a non-syndication property
        /// </summary>
        CustomProperty,

        /// <summary>
        /// The 'author/email' item property
        /// </summary>
        AuthorEmail,

        /// <summary>
        /// The 'author/name' item property
        /// </summary>
        AuthorName,

        /// <summary>
        /// The 'author/uri' item property
        /// </summary>
        AuthorUri,

        /// <summary>
        /// The 'contributor/email' item property
        /// </summary>
        ContributorEmail,

        /// <summary>
        /// The 'contributor/name' item property
        /// </summary>
        ContributorName,

        /// <summary>
        /// The 'contributor/uri' item property
        /// </summary>
        ContributorUri,

        /// <summary>
        /// The 'updated' item property
        /// </summary>
        Updated,

        /// <summary>
        /// The 'published' item property
        /// </summary>
        Published,

        /// <summary>
        /// The 'rights' item property
        /// </summary>
        Rights,

        /// <summary>
        /// The 'summary' item property
        /// </summary>
        Summary,

        /// <summary>
        /// The 'title' item property
        /// </summary>
        Title,
    }
}