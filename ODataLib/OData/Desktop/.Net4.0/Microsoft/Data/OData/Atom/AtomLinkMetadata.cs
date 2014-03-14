//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Atom metadata description for a link.
    /// </summary>
    public sealed class AtomLinkMetadata : ODataAnnotatable
    {
        /// <summary>The IRI value coming from EPM.</summary>
        /// <remarks>We use AtomLinkMetadata class to hold navigation links, association links etc. 
        /// They convert Href property to string based on baseURI and whether Href is absolute or not.
        /// Also we do not want to rely on validation done by Uri class, so we are holding EPM values 
        /// mapped to link/@href on a separate field.
        /// </remarks>
        private string hrefFromEpm;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.Atom.AtomLinkMetadata" /> class.</summary>
        public AtomLinkMetadata()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="AtomLinkMetadata"/> instance to copy the values from; can be null.</param>
        internal AtomLinkMetadata(AtomLinkMetadata other)
        {
            DebugUtils.CheckNoExternalCallers();

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
            this.hrefFromEpm = other.hrefFromEpm;
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
            // TODO: ckerer - validation
            // atomMediaType = xsd:string { pattern = ".+/.+" }
            get;
            set;
        }

        /// <summary>Gets or sets the language tag (for example, en-US) of the resource pointed to by the link.</summary>
        /// <returns>The language tag of the resource pointed to by the link.</returns>
        public string HrefLang
        {
            // TODO: ckerer - validation
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
