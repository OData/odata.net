//---------------------------------------------------------------------
// <copyright file="ODataMessageReadingHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Helper class for creating ODataLib readers, settings, and other read-related classes based on an instance of <see cref="ResponseInfo"/>.
    /// </summary>
    internal class ODataMessageReadingHelper
    {
        /// <summary>The current response info.</summary>
        private readonly ResponseInfo responseInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataMessageReadingHelper"/> class.
        /// </summary>
        /// <param name="responseInfo">The response info.</param>
        internal ODataMessageReadingHelper(ResponseInfo responseInfo)
        {
            Debug.Assert(responseInfo != null, "responseInfo != null");
            this.responseInfo = responseInfo;
        }

        /// <summary>
        /// Create message reader settings for consuming responses.
        /// </summary>
        /// <returns>Newly created message reader settings.</returns>
        internal ODataMessageReaderSettings CreateSettings()
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            Func<IEdmType, string, IEdmType> resolveWireTypeName = this.responseInfo.TypeResolver.ResolveWireTypeName;
            if (this.responseInfo.Context.Format.ServiceModel != null)
            {
                resolveWireTypeName = null;
            }

            settings.AllowDuplicatePropertyNames = true;
            settings.ClientCustomTypeResolver = resolveWireTypeName;
            settings.EnableLaxMetadataValidation = true;
            settings.BaseUri = this.responseInfo.BaseUriResolver.BaseUriOrNull;
            settings.ODataSimplified = this.responseInfo.Context.ODataSimplified;
            settings.MaxProtocolVersion = CommonUtil.ConvertToODataVersion(this.responseInfo.MaxProtocolVersion);
            settings.UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty;
            settings.UndeclaredPropertyBehaviorKinds |= this.responseInfo.UndeclaredPropertyBehaviorKinds;

            if (this.responseInfo.Context.UrlConventions == DataServiceUrlConventions.KeyAsSegment)
            {
                settings.UseKeyAsSegment = true;
            }

            CommonUtil.SetDefaultMessageQuotas(settings.MessageQuotas);

            this.responseInfo.ResponsePipeline.ExecuteReaderSettingsConfiguration(settings);
            return settings;
        }

        /// <summary>
        /// Creates a new the reader for the given response message and settings.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>Newly created message reader.</returns>
        internal ODataMessageReader CreateReader(IODataResponseMessage responseMessage, ODataMessageReaderSettings settings)
        {
            Debug.Assert(responseMessage != null, "responseMessage != null");
            Debug.Assert(settings != null, "settings != null");

            DataServiceClientFormat.ValidateCanReadResponseFormat(responseMessage);
            return new ODataMessageReader(responseMessage, settings, this.responseInfo.TypeResolver.ReaderModel);
        }
    }
}