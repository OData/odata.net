//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// JSON Ligth specific state which is created during payload kind detection and reused during standard reading if available.
    /// </summary>
    internal sealed class ODataJsonLightPayloadKindDetectionState
    {
        /// <summary>The parsed metadata URI.</summary>
        private readonly ODataJsonLightMetadataUriParseResult metadataUriParseResult;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="metadataUriParseResult">The parsed metadata URI.</param>
        internal ODataJsonLightPayloadKindDetectionState(ODataJsonLightMetadataUriParseResult metadataUriParseResult)
        {
            DebugUtils.CheckNoExternalCallers();

            this.metadataUriParseResult = metadataUriParseResult;
        }

        /// <summary>The parsed metadata URI.</summary>
        internal ODataJsonLightMetadataUriParseResult MetadataUriParseResult
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.metadataUriParseResult;
            }
        }
    }
}
