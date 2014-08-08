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

namespace System.Data.Services
{
    using Microsoft.Data.OData;

    /// <summary>
    /// Handles $callback query parameter.
    /// </summary>
    internal static class CallbackQueryOptionHandler
    {
        /// <summary>
        /// Gets the $callback query parameter if present. Also performs some validation:
        /// We do not allow $callback to be present if the content-type for the response is going to be anythign but JSON or plain text.
        /// </summary>
        /// <param name="message">The request message.</param>
        /// <param name="format">The format we will write the reponse in.</param>
        /// <returns>Returns the name of the callback function $callback specifies, or null if there isn't one.</returns>
        internal static string HandleCallbackQueryOption(AstoriaRequestMessage message, ODataFormatWithParameters format)
        {
            var functionName = message.GetQueryStringItem(XmlConstants.HttpQueryStringCallback);

            if (functionName != null)
            {
                // The verb must be GET (not IsQuery(), GET specifically)
                if (message.HttpVerb != HttpVerbs.GET)
                {
                    throw new DataServiceException(400, Strings.CallbackQueryOptionHandler_GetRequestsOnly);
                }

                // If conneg didn't get a format, that means that there was some error that we will fail at later, or
                // it is going end up being a 204, 304, or something else with no body. For all of these cases we are OK
                // with 'pretending' that $callback was not there, since the other errors are probably more important than
                // ours, and in the non-error cases we really should not be throwing (think 204).
                if (format == null)
                {
                    return null;
                }

                // JSON and text/plain are the only things allowed with $callback
                if (format.Format != ODataFormat.Json && format.Format != ODataFormat.VerboseJson && format.Format != ODataFormat.RawValue)
                {
                    throw new DataServiceException(400, Strings.CallbackQueryOptionHandler_UnsupportedContentType(format.Format.ToString()));
                }
            }

            return functionName;
        }
    }
}
