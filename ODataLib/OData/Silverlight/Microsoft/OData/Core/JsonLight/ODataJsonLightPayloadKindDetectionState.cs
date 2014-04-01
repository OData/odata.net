//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
