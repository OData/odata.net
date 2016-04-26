//---------------------------------------------------------------------
// <copyright file="ODataMessageExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;

    /// <summary>
    /// Extension methods to IODataRequestMessage and IODataResponseMessage.
    /// </summary>
    public static class ODataMessageExtensions
    {
        /// <summary>
        /// Reads the OData-Version header from the <paramref name="message"/> and parses it.
        /// If no OData-Version header is found it sets the default version to be used for reading.
        /// </summary>
        /// <param name="message">The message to get the OData-Version header from.</param>
        /// <param name="defaultVersion">The default version to use if the header was not specified.</param>
        /// <returns>
        /// The <see cref="ODataVersion"/> retrieved from the OData-Version header of the message.
        /// The default version if none is specified in the header.
        /// </returns>
        public static ODataVersion GetODataVersion(this IODataResponseMessage message, ODataVersion defaultVersion)
        {
            // translation from IODataResponseMessage to ODataMessage to pass into GetODataVersion.
            // we retain all of the data we need from the message, with a few extra data that aren't used in.
            // GetODataVersion. Not ideal, but it works.
            ODataMessage odataMessage = new ODataResponseMessage(message, false /*writing*/, false/*disableMessageStreamDisposal*/, Int64.MaxValue /*maxMessageSize*/);
            return ODataUtilsInternal.GetODataVersion(odataMessage, defaultVersion);
        }

        /// <summary>
        /// Reads the OData-Version header from the <paramref name="message"/> and parses it.
        /// If no OData-Version header is found it sets the default version to be used for reading.
        /// </summary>
        /// <param name="message">The message to get the OData-Version version header from.</param>
        /// <param name="defaultVersion">The default version to use if the header was not specified.</param>
        /// <returns>
        /// The <see cref="ODataVersion"/> retrieved from the OData-Version header of the message.
        /// The default version if none is specified in the header.
        /// </returns>
        public static ODataVersion GetODataVersion(this IODataRequestMessage message, ODataVersion defaultVersion)
        {
            // translation from IODataResponseMessage to ODataMessage to pass into GetODataVersion.
            // we retain all of the data we need from the message, with a few extra data that aren't used in.
            // GetODataVersion. Not ideal, but it works.
            ODataMessage odataMessage = new ODataRequestMessage(message, false /*writing*/, false/*disableMessageStreamDisposal*/, Int64.MaxValue /*maxMessageSize*/);
            return ODataUtilsInternal.GetODataVersion(odataMessage, defaultVersion);
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
