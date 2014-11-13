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
    /// Represents an association link.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public sealed class ODataAssociationLink : ODataAnnotatable
    {
        /// <summary>the metadata builder for this association link.</summary>
        private ODataEntityMetadataBuilder metadataBuilder;

        /// <summary>URI representing the Unified Resource Locator (Url) of the link.</summary>
        private Uri url;

        /// <summary>true if the association link has been set by the user or seen on the wire or computed by the metadata builder, false otherwise.</summary>
        private bool hasAssociationLinkUrl;

        /// <summary>Gets or sets the name of the association link.</summary>
        /// <returns>The name of the associate link.</returns>
        /// <remarks>This is the name of the navigation property to which the association link belongs.</remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the URI representing the Unified Resource Locator (URL) of the link.</summary>
        /// <returns>The URI representing the Unified Resource Locator (URL) of the link.</returns>
        /// <remarks>This URL should point to a resource which can be used to retrieve or modify the association itself
        /// not the URL to traverse the navigation property.</remarks>
        public Uri Url
        {
            get
            {
                if (this.metadataBuilder != null)
                {
                    this.url = this.metadataBuilder.GetAssociationLinkUri(this.Name, this.url, this.hasAssociationLinkUrl);
                    this.hasAssociationLinkUrl = true;
                }

                return this.url;
            }

            set
            {
                this.url = value;
                this.hasAssociationLinkUrl = true;
            }
        }

        /// <summary>
        /// Sets the metadata builder for this association link.
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
