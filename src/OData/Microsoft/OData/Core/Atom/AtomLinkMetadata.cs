//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Atom metadata description for a link.
    /// </summary>
    public sealed class AtomLinkMetadata : ODataAnnotatable
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.Atom.AtomLinkMetadata" /> class.</summary>
        public AtomLinkMetadata()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="AtomLinkMetadata"/> instance to copy the values from; can be null.</param>
        internal AtomLinkMetadata(AtomLinkMetadata other)
        {
            if (other == null)
            {
                return;
            }

            this.Relation = other.Relation;
            this.Href = other.Href;
            this.HrefLang = other.HrefLang;
            this.Title = other.Title;
            this.MediaType = other.MediaType;
            this.Length = other.Length;
        }

        /// <summary>Gets or sets the URI of the link.</summary>
        /// <returns>The URI of the link.</returns>
        public Uri Href
        {
            get; 
            set;
        }

        /// <summary>Gets or sets the link's relation type.</summary>
        /// <returns>The link’s relation type.</returns>
        public string Relation
        {
            // TODO: should we check that the string is valid
            // The value of "rel" MUST be a string that is non-empty and matches
            // either the "isegment-nz-nc" or the "IRI" production in [RFC3987].
            get;
            set;
        }

        /// <summary>Gets or sets the media type of the data returned by the link.</summary>
        /// <returns>The media type of the data returned by the link.</returns>
        public string MediaType
        {
            // TODO: validation
            // atomMediaType = xsd:string { pattern = ".+/.+" }
            get;
            set;
        }

        /// <summary>Gets or sets the language tag (for example, en-US) of the resource pointed to by the link.</summary>
        /// <returns>The language tag of the resource pointed to by the link.</returns>
        public string HrefLang
        {
            // TODO: validation
            // atomLanguageTag = xsd:string {
            //     pattern = "[A-Za-z]{1,8}(-[A-Za-z0-9]{1,8})*"
            //   }
            get;
            set;
        }

        /// <summary>Gets or sets a human-readable description of the link.</summary>
        /// <returns>A human-readable description of the link.</returns>
        public string Title
        {
            get;
            set;
        }

        /// <summary>Gets or sets a hint at the length of the content returned from the link.</summary>
        /// <returns>A hint at the length of the content returned from the link.</returns>
        public int? Length
        {
            get;
            set;
        }
    }
}
