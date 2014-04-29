//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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

            if (this.responseInfo.Context.EnableAtom)
            {
                // Enable ATOM in client
                settings.EnableAtomSupport();
            }

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
