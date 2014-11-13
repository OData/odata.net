//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;

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
#if !WINRT
            if (this.responseInfo.Context.EnableAtom)
            {
                // Enable ATOM in client
                settings.EnableAtomSupport();
            }
#endif
            Func<IEdmType, string, IEdmType> resolveWireTypeName = this.responseInfo.TypeResolver.ResolveWireTypeName;
            if (this.responseInfo.Context.Format.ServiceModel != null)
            {
                resolveWireTypeName = null;
            }

            settings.EnableWcfDataServicesClientBehavior(resolveWireTypeName);

            settings.BaseUri = this.responseInfo.BaseUriResolver.BaseUriOrNull;
            settings.UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty;
            settings.MaxProtocolVersion = CommonUtil.ConvertToODataVersion(this.responseInfo.MaxProtocolVersion);
            if (this.responseInfo.IgnoreMissingProperties)
            {
                settings.UndeclaredPropertyBehaviorKinds |= ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty;
            }

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

            this.responseInfo.Context.Format.ValidateCanReadResponseFormat(responseMessage);
            return new ODataMessageReader(responseMessage, settings, this.responseInfo.TypeResolver.ReaderModel);
        }
    }
}
