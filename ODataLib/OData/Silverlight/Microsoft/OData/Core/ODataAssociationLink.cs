//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.Evaluation;
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
