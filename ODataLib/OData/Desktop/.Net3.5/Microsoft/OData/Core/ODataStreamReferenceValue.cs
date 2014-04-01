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
    using Microsoft.OData.Edm;
    #endregion

    /// <summary>
    /// Represents a media resource.
    /// </summary>
    public sealed class ODataStreamReferenceValue : ODataValue
    {
        /// <summary>the metadata builder for this OData entry.</summary>
        private ODataEntityMetadataBuilder metadataBuilder;

        /// <summary>The name of the named stream this value belongs to; null for the default media resource.</summary>
        private string edmPropertyName;

        /// <summary>Edit link for media resource.</summary>
        private Uri editLink;

        /// <summary>Edit link for media resource.</summary>
        private Uri computedEditLink;

        /// <summary>Read link for media resource.</summary>
        private Uri readLink;

        /// <summary>Read link for media resource.</summary>
        private Uri computedReadLink;

        /// <summary>Gets or sets the edit link for media resource.</summary>
        /// <returns>The edit link for media resource.</returns>
        public Uri EditLink
        {
            get
            {
                return this.HasNonComputedEditLink
                    ? this.editLink
                    : (this.computedEditLink ?? (this.metadataBuilder == null ? null : this.computedEditLink = this.metadataBuilder.GetStreamEditLink(this.edmPropertyName)));
            }

            set
            {
                this.editLink = value;
                this.HasNonComputedEditLink = true;
            }
        }

        /// <summary>Gets or sets the read link for media resource.</summary>
        /// <returns>The read link for media resource.</returns>
        public Uri ReadLink
        {
            get
            {
                return this.HasNonComputedReadLink
                    ? this.readLink
                    : (this.computedReadLink ?? (this.metadataBuilder == null ? null : this.computedReadLink = this.metadataBuilder.GetStreamReadLink(this.edmPropertyName)));
            }

            set
            {
                this.readLink = value;
                this.HasNonComputedReadLink = true;
            }
        }

        /// <summary>Gets or sets the content media type.</summary>
        /// <returns>The content media type.</returns>
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>Gets or sets the media resource ETag.</summary>
        /// <returns>The media resource ETag.</returns>
        public string ETag
        {
            get;
            set;
        }

        /// <summary>
        /// true if an edit link was provided by the user or seen on the wire, false otherwise. 
        /// </summary>
        internal bool HasNonComputedEditLink
        {
            get;
            private set;
        }

        /// <summary>
        /// true if a read link was provided by the user or seen on the wire, false otherwise. 
        /// </summary>
        internal bool HasNonComputedReadLink
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the metadata builder for this stream reference value.
        /// </summary>
        /// <param name="builder">The metadata builder used to compute values from model annotations.</param>
        /// <param name="propertyName">The property name for the named stream; null for the default media resource.</param>
        internal void SetMetadataBuilder(ODataEntityMetadataBuilder builder, string propertyName)
        {
            this.metadataBuilder = builder;
            this.edmPropertyName = propertyName;
            this.computedEditLink = null;
            this.computedReadLink = null;
        }

        /// <summary>
        /// Gets the metadata builder for this stream reference value.
        /// </summary>
        /// <returns>The metadata builder used to compute links.</returns>
        internal ODataEntityMetadataBuilder GetMetadataBuilder()
        {
            return this.metadataBuilder;
        }
    }
}
