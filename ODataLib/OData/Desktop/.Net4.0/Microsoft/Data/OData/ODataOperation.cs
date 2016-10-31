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
    using Microsoft.Data.OData.JsonLight;
    #endregion

    /// <summary>
    /// Represents a function or an action.
    /// </summary>
    public abstract class ODataOperation : ODataAnnotatable
    {
        /// <summary>the metadata builder for this operation.</summary>
        private ODataEntityMetadataBuilder metadataBuilder;

        /// <summary>A human-readable description of the <see cref="ODataAction"/> or the <see cref="ODataFunction"/>.</summary>
        private string title;

        /// <summary>true if a title was provided by the user or seen on the wire, false otherwise.</summary>
        private bool hasNonComputedTitle;

        /// <summary>A human-readable description of the <see cref="ODataAction"/> or the <see cref="ODataFunction"/>, computed by the metadata builder.</summary>
        private string computedTitle;

        /// <summary>The URI to invoke the <see cref="ODataAction"/> or the <see cref="ODataFunction"/>.</summary>
        private Uri target;

        /// <summary>true if a target was provided by the user or seen on the wire, false otherwise.</summary>
        private bool hasNonComputedTarget;

        /// <summary>The URI to invoke the <see cref="ODataAction"/> or the <see cref="ODataFunction"/>, computed by the metadata builder.</summary>
        private Uri computedTarget;

        /// <summary>The cached full name of the operation to use.</summary>
        private string operationFullName;

        /// <summary>The binding parameter type name for this operation.</summary>
        private string bindingParameterTypeName;

        /// <summary>Gets or sets the URI to get metadata for the <see cref="T:Microsoft.Data.OData.ODataAction" />.</summary>
        /// <returns>The URI to get metadata for the <see cref="T:Microsoft.Data.OData.ODataAction" />.</returns>
        public Uri Metadata
        {
            get;
            set;
        }

        /// <summary>Gets or sets a human-readable description of the <see cref="T:Microsoft.Data.OData.ODataAction" />.</summary>
        /// <returns>A human-readable description of the <see cref="T:Microsoft.Data.OData.ODataAction" />.</returns>
        public string Title
        {
            get
            {
                return this.hasNonComputedTitle
                    ? this.title :
                    (this.computedTitle ?? (this.metadataBuilder == null ? null : this.computedTitle = this.metadataBuilder.GetOperationTitle(this.operationFullName)));
            }

            set
            {
                this.title = value;
                this.hasNonComputedTitle = true;
            }
        }

        /// <summary>Gets or sets the URI to invoke the <see cref="T:Microsoft.Data.OData.ODataAction" />.</summary>
        /// <returns> The URI to invoke the <see cref="T:Microsoft.Data.OData.ODataAction" />.</returns>
        public Uri Target
        {
            get
            {
                return this.hasNonComputedTarget
                    ? this.target
                    : (this.computedTarget ?? (this.metadataBuilder == null ? null : this.computedTarget = this.metadataBuilder.GetOperationTargetUri(this.operationFullName, this.bindingParameterTypeName)));
            }

            set
            {
                this.target = value;
                this.hasNonComputedTarget = true;
            }
        }

        /// <summary>
        /// Sets the metadata builder for this operation.
        /// </summary>
        /// <param name="builder">The metadata builder used to compute values from model annotations.</param>
        /// <param name="metadataDocumentUri">The metadata document Uri.</param>
        internal void SetMetadataBuilder(ODataEntityMetadataBuilder builder, Uri metadataDocumentUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(metadataDocumentUri.IsAbsoluteUri, "metadataDocumentUri.IsAbsoluteUri");

            ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, this);
            this.metadataBuilder = builder;
            this.operationFullName = ODataJsonLightUtils.GetFullyQualifiedFunctionImportName(metadataDocumentUri, UriUtilsCommon.UriToString(this.Metadata), out this.bindingParameterTypeName);
            this.computedTitle = null;
            this.computedTarget = null;
        }

        /// <summary>
        /// Gets the metadata builder for this operation.
        /// </summary>
        /// <returns>The metadata builder used to compute values.</returns>
        internal ODataEntityMetadataBuilder GetMetadataBuilder()
        {
            DebugUtils.CheckNoExternalCallers();
            return this.metadataBuilder;
        }
    }
}
