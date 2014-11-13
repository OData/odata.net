//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Common
{
    /// <summary>Enumeration type that is used to identify the syndication item element or attribute in the Open Data Protocol (OData) feed to which an entity property is mapped.</summary>
        /// <remarks>
        /// Potentially the following atom specific elements could also be considered:
        /// * Content?
        /// * Id
        /// * Source?
        /// </remarks>
    public enum SyndicationItemProperty
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
