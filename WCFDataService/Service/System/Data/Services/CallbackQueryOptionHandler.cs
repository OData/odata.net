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
