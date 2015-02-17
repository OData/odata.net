//---------------------------------------------------------------------
// <copyright file="ServiceConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.Web.Configuration;
    using System.Web.Hosting;

    /// <summary>
    /// Service wide constant values.
    /// </summary>
    public static class ServiceConstants
    {
        public const string ServicePath_Async = "$async";

        public const string QueryOption_Filter = "$filter";
        public const string QueryOption_Search = "$search";
        public const string QueryOption_OrderBy = "$orderby";
        public const string QueryOption_Skip = "$skip";
        public const string QueryOption_Top = "$top";
        public const string QueryOption_Delta = "$deltaToken";
        public const string QueryOption_AsyncToken = "$asyncToken";
        public const string QueryOption_Format = "$format";

        public const string Preference_TrackChanging = "odata.track-changes";
        public const string Preference_RespondAsync = "respond-async";
        public const string Preference_IncludeAnnotations = "odata.include-annotations";
        public const string Preference_MaxPageSize = "odata.maxpagesize";
        public const string Preference_Return = "return";

        public const string PreferenceValue_Return_Minimal = "minimal";

        // The value is small due to the size limitation of data source
        public const int DefaultPageSize = 8;

        public const string ETagValueAsterisk = "*";

        /// <summary>
        /// Gets the value of the service's base URI.
        /// </summary>
        public static Uri ServiceBaseUri
        {
            get
            {
                var baseAddress = Utility.GetServiceBaseAddress();
                var uri = new Uri(baseAddress);
                return Utility.RebuildUri(uri);
            }
        }

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
            /// The 'OData-Version' HTTP header
            /// </summary>
            public const string DataServiceVersion = "OData-Version";

            /// <summary>
            /// The 'OData-MaxVersion' HTTP header
            /// </summary>
            public const string MaxDataServiceVersion = "OData-MaxVersion";

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
            /// The 'OData-EntityId' header
            /// </summary>
            public const string ODataEntityId = "OData-EntityId";

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

        /// <summary>
        /// A collection of constants for commonly-used HTTP header values
        /// </summary>
        public static class HttpHeaderValues
        {
            /// <summary>'no-cache' - HTTP value for Cache-Control header.</summary>
            internal const string NoCache = "no-cache";

            /// <summary>'binary' - HTTP value for  Content-Transfer-Encoding header.</summary>
            public const string Binary = "binary";
        }

        public static class MimeTypes
        {
            public const string ApplicationJson = "application/json";
            public const string ApplicationJsonMetadataNone = "application/json;odata.metadata=none";
            public const string ApplicationJsonMetadataMinimal = "application/json;odata.metadata=minimal";
            public const string ApplicationJsonMetadataFull = "application/json;odata.metadata=full";
        }
    }
}
