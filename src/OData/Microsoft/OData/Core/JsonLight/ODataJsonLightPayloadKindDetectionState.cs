//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// JSON Ligth specific state which is created during payload kind detection and reused during standard reading if available.
    /// </summary>
    internal sealed class ODataJsonLightPayloadKindDetectionState
    {
        /// <summary>The parsed context URI.</summary>
        private readonly ODataJsonLightContextUriParseResult contextUriParseResult;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contextUriParseResult">The parsed context URI.</param>
        internal ODataJsonLightPayloadKindDetectionState(ODataJsonLightContextUriParseResult contextUriParseResult)
        {
            this.contextUriParseResult = contextUriParseResult;
        }

        /// <summary>The parsed context URI.</summary>
        internal ODataJsonLightContextUriParseResult ContextUriParseResult
        {
            get
            {
                return this.contextUriParseResult;
            }
        }
    }
}
