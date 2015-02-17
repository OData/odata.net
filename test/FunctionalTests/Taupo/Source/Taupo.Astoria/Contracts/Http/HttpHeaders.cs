//---------------------------------------------------------------------
// <copyright file="HttpHeaders.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    /// <summary>
    /// A collection of constants for commonly-used HTTP headers
    /// </summary>
    public static class HttpHeaders
    {
        /// <summary>
        /// The 'Accept' HTTP header
        /// </summary>
        public const string Accept = "Accept";

        /// <summary>
        /// The 'Accept-Charset' HTTP header
        /// </summary>
        public const string AcceptCharset = "Accept-Charset";

        /// <summary>
        /// The 'Cache-Control' HTTP header
        /// </summary>
        public const string CacheControl = "Cache-Control";

        /// <summary>
        /// The 'Content-Type' HTTP header
        /// </summary>
        public const string ContentType = "Content-Type";

        /// <summary>
        /// The 'Content-ID' HTTP header
        /// </summary>
        public const string ContentId = "Content-ID";

        /// <summary>
        /// The 'boundary' mime type parameter
        /// </summary>
        public const string Boundary = "boundary";

        /// <summary>
        /// The 'charset' mime type parameter
        /// </summary>
        public const string Charset = "charset";

        /// <summary>
        /// The 'Content-Transfer-Encoding' HTTP header
        /// </summary>
        public const string ContentTransferEncoding = "Content-Transfer-Encoding";

        /// <summary>
        /// The 'Content-Length' HTTP header
        /// </summary>
        public const string ContentLength = "Content-Length";

        /// <summary>
        /// The 'Transfer-Encoding' HTTP header
        /// </summary>
        public const string TransferEncoding = "Transfer-Encoding";

        /// <summary>
        /// The 'chunked' value for the 'Transfer-Encoding' HTTP header
        /// </summary>
        public const string TransferEncodingChunked = "chunked";
        
        /// <summary>
        /// The 'ETag' HTTP header
        /// </summary>
        public const string ETag = "ETag";

        /// <summary>
        /// The 'If-Match' HTTP header
        /// </summary>
        public const string IfMatch = "If-Match";

        /// <summary>
        /// The 'If-None-Match' HTTP header
        /// </summary>
        public const string IfNoneMatch = "If-None-Match";

        /// <summary>
        /// The 'DataServiceVersion' HTTP header
        /// </summary>
        public const string DataServiceVersion = "DataServiceVersion";

        /// <summary>
        /// The 'MaxDataServiceVersion' HTTP header
        /// </summary>
        public const string MaxDataServiceVersion = "MaxDataServiceVersion";

        /// <summary>
        /// The 'Prefer' HTTP header
        /// </summary>
        public const string Prefer = "Prefer";

        /// <summary>
        /// The 'Preference-Applied' HTTP header
        /// </summary>
        public const string PreferenceApplied = "Preference-Applied";

        /// <summary>
        /// Constant for Prefer Header value in requests for returning content
        /// </summary>
        public const string ReturnContent = "return-content";

        /// <summary>
        /// Constant for Prefer Header value in requests for not returning content
        /// </summary>
        public const string ReturnNoContent = "return-no-content";

        /// <summary>
        /// The 'X-HTTP-Method' header
        /// </summary>
        public const string HttpMethod = "X-HTTP-Method";

        /// <summary>
        /// The 'DataServiceId' header
        /// </summary>
        public const string DataServiceId = "DataServiceId";

        /// <summary>
        /// The 'Location' header
        /// </summary>
        public const string Location = "Location";

        /// <summary>
        /// The 'Slug' header
        /// </summary>
        public const string Slug = "Slug";

        /// <summary>
        /// The 'NetFx' string used for the user Agent
        /// </summary>
        public const string NetFx = "NetFx";

        /// <summary>
        /// The header used to suggest the type of a media-link entry when creating a media resource
        /// </summary>
        public const string MediaLinkEntryEntityTypeHint = "MediaLinkEntry-TypeNameHint";

        /// <summary>
        /// The header used to tell IE from MIME-sniffing, the only defined value for it is "nosniff"
        /// </summary>
        public const string XContentTypeOptions = "X-Content-Type-Options";

        /// <summary>
        /// The 'User-Agent' header
        /// </summary>
        public const string UserAgent = "User-Agent";
    }
}
