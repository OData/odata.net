//---------------------------------------------------------------------
// <copyright file="ODataOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Evaluation;
    using Microsoft.OData.JsonLight;
    #endregion

    /// <summary>
    /// Represents a function or an action.
    /// </summary>
    public abstract class ODataOperation : ODataAnnotatable
    {
        /// <summary>the metadata builder for this operation.</summary>
        private ODataResourceMetadataBuilder metadataBuilder;

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

        /// <summary>The parameter names for this operation.</summary>
        private string parameterNames;

        /// <summary>Gets or sets the URI to get metadata for the <see cref="Microsoft.OData.ODataAction" />.</summary>
        /// <returns>The URI to get metadata for the <see cref="Microsoft.OData.ODataAction" />.</returns>
        public Uri Metadata { get; set; }

        /// <summary>Gets or sets a human-readable description of the <see cref="Microsoft.OData.ODataAction" />.</summary>
        /// <returns>A human-readable description of the <see cref="Microsoft.OData.ODataAction" />.</returns>
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

        /// <summary>Gets or sets the URI to invoke the <see cref="Microsoft.OData.ODataAction" />.</summary>
        /// <returns> The URI to invoke the <see cref="Microsoft.OData.ODataAction" />.</returns>
        public Uri Target
        {
            get
            {
                return this.hasNonComputedTarget
                    ? this.target
                    : (this.computedTarget ?? (this.metadataBuilder == null ? null : this.computedTarget = this.metadataBuilder.GetOperationTargetUri(this.operationFullName, this.BindingParameterTypeName, this.parameterNames)));
            }

            set
            {
                this.target = value;
                this.hasNonComputedTarget = true;
            }
        }

        /// <summary>
        /// The binding parameter type name for this operation.
        /// </summary>
        internal string BindingParameterTypeName { get; set; }

        /// <summary>
        /// Sets the metadata builder for this operation.
        /// </summary>
        /// <param name="builder">The metadata builder used to compute values from model annotations.</param>
        /// <param name="metadataDocumentUri">The metadata document Uri.</param>
        internal void SetMetadataBuilder(ODataResourceMetadataBuilder builder, Uri metadataDocumentUri)
        {
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(metadataDocumentUri.IsAbsoluteUri, "metadataDocumentUri.IsAbsoluteUri");

            ODataJsonLightValidationUtils.ValidateOperation(metadataDocumentUri, this);
            this.metadataBuilder = builder;
            this.operationFullName = ODataJsonLightUtils.GetFullyQualifiedOperationName(metadataDocumentUri, UriUtils.UriToString(this.Metadata), out this.parameterNames);
            this.computedTitle = null;
            this.computedTarget = null;
        }

        /// <summary>
        /// Gets the metadata builder for this operation.
        /// </summary>
        /// <returns>The metadata builder used to compute values.</returns>
        internal ODataResourceMetadataBuilder GetMetadataBuilder()
        {
            return this.metadataBuilder;
        }
    }
}
