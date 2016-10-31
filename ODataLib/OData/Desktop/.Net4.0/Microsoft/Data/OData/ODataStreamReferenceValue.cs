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
