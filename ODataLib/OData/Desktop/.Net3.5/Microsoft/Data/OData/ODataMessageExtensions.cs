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

namespace Microsoft.Data.OData
{
    using System;

    /// <summary>
    /// Extension methods to IODataRequestMessage and IODataResponseMessage.
    /// </summary>
    public static class ODataMessageExtensions
    {
        /// <summary>
        /// Reads the DataServiceVersion header from the <paramref name="message"/> and parses it.
        /// If no DataServiceVersion header is found it sets the default version to be used for reading.
        /// </summary>
        /// <param name="message">The message to get the data service version header from.</param>
        /// <param name="defaultVersion">The default version to use if the header was not specified.</param>
        /// <returns>
        /// The <see cref="ODataVersion"/> retrieved from the DataServiceVersion header of the message.
        /// The default version if none is specified in the header.
        /// </returns>
        public static ODataVersion GetDataServiceVersion(this IODataResponseMessage message, ODataVersion defaultVersion)
        {
            // translation from IODataResponseMessage to ODataMessage to pass into GetDataServiceVersion.
            // we retain all of the data we need from the message, with a few extra data that aren't used in.
            // GetDataServiceVersion. Not ideal, but it works.
            ODataMessage odataMessage = new ODataResponseMessage(message, false /*writing*/, false/*disableMessageStreamDisposal*/, Int64.MaxValue /*maxMessageSize*/);
            return ODataUtilsInternal.GetDataServiceVersion(odataMessage, defaultVersion);
        }

        /// <summary>
        /// Reads the DataServiceVersion header from the <paramref name="message"/> and parses it.
        /// If no DataServiceVersion header is found it sets the default version to be used for reading.
        /// </summary>
        /// <param name="message">The message to get the data service version header from.</param>
        /// <param name="defaultVersion">The default version to use if the header was not specified.</param>
        /// <returns>
        /// The <see cref="ODataVersion"/> retrieved from the DataServiceVersion header of the message.
        /// The default version if none is specified in the header.
        /// </returns>
        public static ODataVersion GetDataServiceVersion(this IODataRequestMessage message, ODataVersion defaultVersion)
        {
            // translation from IODataResponseMessage to ODataMessage to pass into GetDataServiceVersion.
            // we retain all of the data we need from the message, with a few extra data that aren't used in.
            // GetDataServiceVersion. Not ideal, but it works.
            ODataMessage odataMessage = new ODataRequestMessage(message, false /*writing*/, false/*disableMessageStreamDisposal*/, Int64.MaxValue /*maxMessageSize*/);
            return ODataUtilsInternal.GetDataServiceVersion(odataMessage, defaultVersion);
        }

        /// <summary>
        /// Gets the <see cref="ODataPreferenceHeader"/> instance to get or set preferences on the "Prefer" header of the <paramref name="requestMessage"/>.
        /// </summary>
        /// <param name="requestMessage">The request message to get or set the "Prefer" header.</param>
        /// <returns>Returns the <see cref="ODataPreferenceHeader"/> instance to get or set preferences on the "Prefer" header of the <paramref name="requestMessage"/>.</returns>
        public static ODataPreferenceHeader PreferHeader(this IODataRequestMessage requestMessage)
        {
            ExceptionUtils.CheckArgumentNotNull(requestMessage, "requestMessage");
            return new ODataPreferenceHeader(requestMessage);
        }

        /// <summary>
        /// Gets the <see cref="ODataPreferenceHeader"/> instance to get or set preferences on the "Preference-Applied" header of the <paramref name="responseMessage"/>.
        /// </summary>
        /// <param name="responseMessage">The response message to get or set the "Preference-Applied" header.</param>
        /// <returns>Returns the <see cref="ODataPreferenceHeader"/> instance to get or set preferences on the "Preference-Applied" header of the <paramref name="responseMessage"/>.</returns>
        public static ODataPreferenceHeader PreferenceAppliedHeader(this IODataResponseMessage responseMessage)
        {
            ExceptionUtils.CheckArgumentNotNull(responseMessage, "responseMessage");
            return new ODataPreferenceHeader(responseMessage);
        }
    }
}
