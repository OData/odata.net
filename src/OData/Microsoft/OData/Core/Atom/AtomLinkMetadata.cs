//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
