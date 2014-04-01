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
    using Microsoft.OData.Core.JsonLight;
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

        /// <summary>Gets or sets the URI to get metadata for the <see cref="T:Microsoft.OData.Core.ODataAction" />.</summary>
        /// <returns>The URI to get metadata for the <see cref="T:Microsoft.OData.Core.ODataAction" />.</returns>
        public Uri Metadata { get; set; }

        /// <summary>Gets or sets a human-readable description of the <see cref="T:Microsoft.OData.Core.ODataAction" />.</summary>
        /// <returns>A human-readable description of the <see cref="T:Microsoft.OData.Core.ODataAction" />.</returns>
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

        /// <summary>Gets or sets the URI to invoke the <see cref="T:Microsoft.OData.Core.ODataAction" />.</summary>
        /// <returns> The URI to invoke the <see cref="T:Microsoft.OData.Core.ODataAction" />.</returns>
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
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(metadataDocumentUri.IsAbsoluteUri, "metadataDocumentUri.IsAbsoluteUri");

            ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, this);
            this.metadataBuilder = builder;
            this.operationFullName = ODataJsonLightUtils.GetFullyQualifiedOperationName(metadataDocumentUri, UriUtils.UriToString(this.Metadata), out this.bindingParameterTypeName);
            this.computedTitle = null;
            this.computedTarget = null;
        }

        /// <summary>
        /// Gets the metadata builder for this operation.
        /// </summary>
        /// <returns>The metadata builder used to compute values.</returns>
        internal ODataEntityMetadataBuilder GetMetadataBuilder()
        {
            return this.metadataBuilder;
        }
    }
}
