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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Data.OData.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Represents a single link.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Name}")]
    public sealed class ODataNavigationLink : ODataItem
    {
        /// <summary>the metadata builder for this navigation link.</summary>
        private ODataEntityMetadataBuilder metadataBuilder;

        /// <summary>URI representing the Unified Resource Locator (Url) of the link as provided by the user or seen on the wire (never computed).</summary>
        private Uri url;

        /// <summary>true if the navigation link has been set by the user or seen on the wire or computed by the metadata builder, false otherwise.</summary>
        private bool hasNavigationLink;

        /// <summary>The association link URL for this navigation link as provided by the user or seen on the wire (never computed).</summary>
        private Uri associationLinkUrl;

        /// <summary>true if the association link has been set by the user or seen on the wire or computed by the metadata builder, false otherwise.</summary>
        private bool hasAssociationUrl;

        /// <summary>Gets or sets a value that indicates whether the navigation link represents a collection or an entry.</summary>
        /// <returns>true if the navigation link represents a collection; false if the navigation represents an entry.</returns>
        /// <remarks>This property is required to have a value for ATOM payloads and is optional for JSON payloads.</remarks>
        public bool? IsCollection
        {
            get;
            set;
        }

        /// <summary>Gets or sets the name of the link.</summary>
        /// <returns>The name of the link.</returns>
        public string Name 
        { 
            get; 
            set; 
        }

        /// <summary>Gets or sets the URI representing the Unified Resource Locator (URL) of the link.</summary>
        /// <returns>The URI representing the Unified Resource Locator (URL) of the link.</returns>
        public Uri Url
        {
            get
            {
                if (this.metadataBuilder != null)
                {
                    this.url = this.metadataBuilder.GetNavigationLinkUri(this.Name, this.url, this.hasNavigationLink);
                    this.hasNavigationLink = true;
                }

                return this.url;
            }

            set
            {
                this.url = value;
                this.hasNavigationLink = true;
            }
        }

        /// <summary>The association link URL for this navigation link. </summary>
        public Uri AssociationLinkUrl
        {
            get
            {
                if (this.metadataBuilder != null)
                {
                    this.associationLinkUrl = this.metadataBuilder.GetAssociationLinkUri(this.Name, this.associationLinkUrl, this.hasAssociationUrl);
                    this.hasAssociationUrl = true;
                }

                return this.associationLinkUrl;
            }

            set
            {
                this.associationLinkUrl = value;
                this.hasAssociationUrl = true;
            }
        }

        /// <summary>
        /// Sets the metadata builder for this navigation link.
        /// </summary>
        /// <param name="builder">The metadata builder used to compute values from model annotations.</param>
        internal void SetMetadataBuilder(ODataEntityMetadataBuilder builder)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(builder != null, "builder != null");

            this.metadataBuilder = builder;
        }
    }
}
