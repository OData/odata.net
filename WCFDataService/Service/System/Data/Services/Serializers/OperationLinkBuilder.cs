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

namespace System.Data.Services.Serializers
{
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Microsoft.Data.OData;

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
        private readonly string containerName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationLinkBuilder"/> class.
        /// </summary>
        /// <param name="containerName">Name of the default entity container.</param>
        /// <param name="metadataUri">The metadata URI of the service.</param>
        internal OperationLinkBuilder(string containerName, Uri metadataUri)
        {
            Debug.Assert(!string.IsNullOrEmpty(containerName), "ContainerName was null or empty.");
            Debug.Assert(metadataUri != null, "metadataUri != null");

            this.containerName = containerName;
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
            builder.Append(Uri.EscapeDataString(this.containerName));
            builder.Append('.');
            builder.Append(Uri.EscapeDataString(operation.Name));

            // If there are multiple operations with the same name, then the parameter types should be included in the metadata link.
            if (entityHasMultipleActionsWithSameName)
            {
                AppendParameterTypeNames(operation, builder);
            }

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

            string targetSegment = operation.GetActionTargetSegmentByResourceType(entityToSerialize.ResourceType, this.containerName);

            // If there are multiple operations with the same name, then using the edit link of the entry would cause the target to potentially resolve to the wrong
            // operation. Instead, use the actual binding type of the specific operation.
            if (entityHasMultipleActionsWithSameName)
            {
                Uri editLinkWithBindingType = RequestUriProcessor.AppendUnescapedSegment(entityToSerialize.SerializedKey.AbsoluteEditLinkWithoutSuffix, operation.BindingParameter.ParameterType.FullName);
                return RequestUriProcessor.AppendUnescapedSegment(editLinkWithBindingType, targetSegment);
            }

            return RequestUriProcessor.AppendUnescapedSegment(entityToSerialize.SerializedKey.AbsoluteEditLink, targetSegment);
        }

        /// <summary>
        /// Appends the parameter type names onto the metadata link for an operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="builder">The builder with everything up to the operation name.</param>
        [SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Values passed to this method are metadata item names and not literals.")]
        private static void AppendParameterTypeNames(OperationWrapper operation, StringBuilder builder)
        {
            builder.Append('(');
            bool firstParameter = true;
            foreach (var parameter in operation.Parameters)
            {
                if (!firstParameter)
                {
                    builder.Append(',');
                }

                builder.Append(Uri.EscapeDataString(parameter.ParameterType.FullName));
                firstParameter = false;
            }

            builder.Append(')');
        }
    }
}
