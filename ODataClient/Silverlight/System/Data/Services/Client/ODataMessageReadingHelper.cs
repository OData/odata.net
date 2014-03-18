//---------------------------------------------------------------------
// <copyright file="ODataMessageReadingHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Services.Client
{
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;

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
        /// <param name="entryXmlCustomizer">Optional XML entry customization callback to be used.</param>
        /// <returns>Newly created message reader settings.</returns>
        internal ODataMessageReaderSettings CreateSettings(Func<ODataEntry, XmlReader, Uri, XmlReader> entryXmlCustomizer)
        {
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();

            if (!this.responseInfo.ResponsePipeline.HasAtomReadingEntityHandlers)
            {
                entryXmlCustomizer = null;
            }

            Func<IEdmType, string, IEdmType> resolveWireTypeName = this.responseInfo.TypeResolver.ResolveWireTypeName;
            if (this.responseInfo.Context.Format.ServiceModel != null)
            {
                resolveWireTypeName = null;
            }

            settings.EnableWcfDataServicesClientBehavior(
                resolveWireTypeName,
                this.responseInfo.DataNamespace, 
                UriUtil.UriToString(this.responseInfo.TypeScheme), 
                entryXmlCustomizer);

            settings.BaseUri = this.responseInfo.BaseUriResolver.BaseUriOrNull;
            settings.UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty;
            settings.MaxProtocolVersion = CommonUtil.ConvertToODataVersion(this.responseInfo.MaxProtocolVersion);
            if (this.responseInfo.IgnoreMissingProperties)
            {
                settings.UndeclaredPropertyBehaviorKinds |= ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty;
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

            this.responseInfo.Context.Format.ValidateCanReadResponseFormat(responseMessage);
            return new ODataMessageReader(responseMessage, settings, this.responseInfo.TypeResolver.ReaderModel);
        }
    }
}