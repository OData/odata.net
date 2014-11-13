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
