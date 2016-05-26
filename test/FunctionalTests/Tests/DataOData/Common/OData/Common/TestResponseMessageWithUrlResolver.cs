//---------------------------------------------------------------------
// <copyright file="TestResponseMessageWithUrlResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.IO;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// A test response message which implements the URL resolver
    /// </summary>
    public class TestResponseMessageWithUrlResolver : TestResponseMessage, IODataPayloadUriConverter
    {
        /// <summary>
        /// The URL resolver to forward all URI resolution calls to.
        /// </summary>
        private IODataPayloadUriConverter urlResolver;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream with the content of the message.</param>
        /// <param name="flags">The flags modifying the behavior of the message.</param>
        public TestResponseMessageWithUrlResolver(Stream stream, IODataPayloadUriConverter urlResolver, TestMessageFlags flags = TestMessageFlags.None)
            : base(stream, flags)
        {
            this.urlResolver = urlResolver;
        }

        /// <summary>
        /// Method to implement a custom URL resolution scheme.
        /// This method returns null if no custom resolution is desired.
        /// If the method returns a non-null URL that value will be used without further validation.
        /// </summary>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        /// <returns>
        /// A <see cref="Uri"/> instance that reflects the custom resolution of the method arguments
        /// into a URL or null if no custom resolution is desired; in that case the default resolution is used.
        /// </returns>
        public Uri ConvertPayloadUri(Uri baseUri, Uri payloadUri)
        {
            return this.urlResolver.ConvertPayloadUri(baseUri, payloadUri);
        }
    }
}
