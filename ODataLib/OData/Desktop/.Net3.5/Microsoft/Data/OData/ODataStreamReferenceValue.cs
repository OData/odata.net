//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.Data.OData.Evaluation;
    using Microsoft.Data.Edm;
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

        /// <summary>true if an edit link was provided by the user or seen on the wire, false otherwise.</summary>
        private bool hasNonComputedEditLink;

        /// <summary>Read link for media resource.</summary>
        private Uri readLink;

        /// <summary>Read link for media resource.</summary>
        private Uri computedReadLink;

        /// <summary>true if a read link was provided by the user or seen on the wire, false otherwise.</summary>
        private bool hasNonComputedReadLink;

        /// <summary>Gets or sets the edit link for media resource.</summary>
        /// <returns>The edit link for media resource.</returns>
        public Uri EditLink
        {
            get
            {
                return this.hasNonComputedEditLink
                    ? this.editLink
                    : (this.computedEditLink ?? (this.metadataBuilder == null ? null : this.computedEditLink = this.metadataBuilder.GetStreamEditLink(this.edmPropertyName)));
            }

            set
            {
                this.editLink = value;
                this.hasNonComputedEditLink = true;
            }
        }

        /// <summary>Gets or sets the read link for media resource.</summary>
        /// <returns>The read link for media resource.</returns>
        public Uri ReadLink
        {
            get
            {
                return this.hasNonComputedReadLink
                    ? this.readLink
                    : (this.computedReadLink ?? (this.metadataBuilder == null ? null : this.computedReadLink = this.metadataBuilder.GetStreamReadLink(this.edmPropertyName)));
            }

            set
            {
                this.readLink = value;
                this.hasNonComputedReadLink = true;
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
        /// Sets the metadata builder for this stream reference value.
        /// </summary>
        /// <param name="builder">The metadata builder used to compute values from model annotations.</param>
        /// <param name="propertyName">The property name for the named stream; null for the default media resource.</param>
        internal void SetMetadataBuilder(ODataEntityMetadataBuilder builder, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();
            return this.metadataBuilder;
        }
    }
}
