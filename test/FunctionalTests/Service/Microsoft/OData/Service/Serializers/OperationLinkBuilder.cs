//---------------------------------------------------------------------
// <copyright file="OperationLinkBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Component for generating metadata and target links for operations being serialized in entity payloads.
    /// </summary>
    internal class OperationLinkBuilder
    {
        /// <summary>
        /// The metadata URI of the service.
        /// </summary>
        private readonly Uri metadataUri;

        /// <summary>
        /// The default container name.
        /// </summary>
        private readonly string namespaceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationLinkBuilder"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace of the operation.</param>
        /// <param name="metadataUri">The metadata URI of the service.</param>
        internal OperationLinkBuilder(string namespaceName, Uri metadataUri)
        {
            Debug.Assert(!string.IsNullOrEmpty(namespaceName), "namespaceName was null or empty.");
            Debug.Assert(metadataUri != null, "metadataUri != null");

            this.namespaceName = namespaceName;
            this.metadataUri = metadataUri;
        }

        /// <summary>
        /// Gets the metadata link value for an <see cref="ODataOperation"/>
        /// </summary>
        /// <param name="operation">The operation to generate the link for.</param>
        /// <param name="entityHasMultipleActionsWithSameName">Whether or not there are multiple operations in the current scope with the same name as the current operation.</param>
        /// <returns>Uri representing the link to this operations metadata.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Values passed to this method are metadata item names and not literals.")]
        internal Uri BuildMetadataLink(OperationWrapper operation, bool entityHasMultipleActionsWithSameName)
        {
            Debug.Assert(!String.IsNullOrEmpty(operation.Name), "!string.IsNullOrEmpty(operation.Name)");

            StringBuilder builder = new StringBuilder();
            builder.Append(UriUtil.UriToString(this.metadataUri));
            builder.Append('#');
            builder.Append(Uri.EscapeDataString(namespaceName));
            builder.Append('.');
            builder.Append(Uri.EscapeDataString(operation.Name));

            return new Uri(builder.ToString());
        }

        /// <summary>
        /// Gets the target link value for an <see cref="ODataOperation"/>
        /// </summary>
        /// <param name="entityToSerialize">The current entity being serialized.</param>
        /// <param name="operation">The operation to generate the link for.</param>
        /// <param name="entityHasMultipleActionsWithSameName">Whether or not there are multiple operations in the current scope with the same name as the current operation.</param>
        /// <returns>Uri representing link to use for invoking this operation.</returns>
        internal Uri BuildTargetLink(EntityToSerialize entityToSerialize, OperationWrapper operation, bool entityHasMultipleActionsWithSameName)
        {
            Debug.Assert(entityToSerialize != null, "entityToSerialize != null");
            Debug.Assert(operation != null, "operation != null");
            Debug.Assert(operation.BindingParameter != null, "operation.BindingParameter != null");
            Debug.Assert(operation.BindingParameter.ParameterType != null, "operation.BindingParameter.ParameterType != null");

            string targetSegment = operation.GetActionTargetSegmentByResourceType(entityToSerialize.ResourceType, this.namespaceName);

            // If there are multiple operations with the same name, then using the edit link of the entry would cause the target to potentially resolve to the wrong
            // operation. Instead, use the actual binding type of the specific operation.
            if (entityHasMultipleActionsWithSameName)
            {
                Uri editLinkWithBindingType = RequestUriProcessor.AppendUnescapedSegment(entityToSerialize.SerializedKey.AbsoluteEditLinkWithoutSuffix, operation.BindingParameter.ParameterType.FullName);
                return RequestUriProcessor.AppendUnescapedSegment(editLinkWithBindingType, targetSegment);
            }

            return RequestUriProcessor.AppendUnescapedSegment(entityToSerialize.SerializedKey.AbsoluteEditLink, targetSegment);
        }
    }
}