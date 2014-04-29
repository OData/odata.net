//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    using System;

    /// <summary>
    /// The HTTP headers that may be specified in a client request.
    /// </summary>
    internal enum HttpRequestHeader
    {
        /// <summary>
        /// The Cache-Control header, which specifies directives that must be obeyed by all cache control mechanisms along the request/response chain. 
        /// </summary>
        CacheControl,

        /// <summary>
        /// The Connection header, which specifies options that are desired for a particular connection. 
        /// </summary>
        Connection,

        /// <summary>
        /// The Date header, which specifies the date and time at which the request originated. 
        /// </summary>
        Date,

        /// <summary>
        /// The Keep-Alive header, which specifies a parameter used into order to maintain a persistent connection. 
        /// </summary>
        KeepAlive,

        /// <summary>
        /// The Pragma header, which specifies implementation-specific directives that might apply to any agent along the request/response chain. 
        /// </summary>
        Pragma,

        /// <summary>
        /// The Trailer header, which specifies the header fields present in the trailer of a message encoded with chunked transfer-coding. 
        /// </summary>
        Trailer,

        /// <summary>
        /// The Transfer-Encoding header, which specifies what (if any) type of transformation that has been applied to the message body. 
        /// </summary>
        TransferEncoding,

        /// <summary>
        /// The Upgrade header, which specifies additional communications protocols that the client supports. 
        /// </summary>
        Upgrade,

        /// <summary>
        /// The Via header, which specifies intermediate protocols to be used by gateway and proxy agents. 
        /// </summary>
        Via,

        /// <summary>
        /// The Warning header, which specifies additional information about that status or transformation of a message that might not be reflected in the message. 
        /// </summary>
        Warning,

        /// <summary>
        /// The Allow header, which specifies the set of HTTP methods supported. 
        /// </summary>
        Allow,

        /// <summary>
        /// The Content-Length header, which specifies the length, in bytes, of the accompanying body data. 
        /// </summary>
        ContentLength,

        /// <summary>
        /// The Content-Type header, which specifies the MIME type of the accompanying body data. 
        /// </summary>
        ContentType,

        /// <summary>
        /// The Content-Encoding header, which specifies the encodings that have been applied to the accompanying body data. 
        /// </summary>
        ContentEncoding,

        /// <summary>
        /// The Content-Langauge header, which specifies the natural language(s) of the accompanying body data. 
        /// </summary>
        ContentLanguage,

        /// <summary>
        /// The Content-Location header, which specifies a URI from which the accompanying body may be obtained. 
        /// </summary>
        ContentLocation,

        /// <summary>
        /// The Content-MD5 header, which specifies the MD5 digest of the accompanying body data, for the purpose of providing an end-to-end message integrity check. 
        /// </summary>
        ContentMd5,

        /// <summary>
        /// The Content-Range header, which specifies where in the full body the accompanying partial body data should be applied. 
        /// </summary>
        ContentRange,

        /// <summary>
        /// The Expires header, which specifies the date and time after which the accompanying body data should be considered stale. 
        /// </summary>
        Expires,

        /// <summary>
        /// The Last-Modified header, which specifies the date and time at which the accompanying body data was last modified. 
        /// </summary>
        LastModified,

        /// <summary>
        /// The Accept header, which specifies the MIME types that are acceptable for the response. 
        /// </summary>
        Accept,

        /// <summary>
        /// The Accept-Charset header, which specifies the character sets that are acceptable for the response. 
        /// </summary>
        AcceptCharset,

        /// <summary>
        /// The Accept-Encoding header, which specifies the content encodings that are acceptable for the response. 
        /// </summary>
        AcceptEncoding,

        /// <summary>
        /// The Accept-Langauge header, which specifies that natural languages that are preferred for the response. 
        /// </summary>
        AcceptLanguage,

        /// <summary>
        /// The Authorization header, which specifies the credentials that the client presents in order to authenticate itself to the server. 
        /// </summary>
        Authorization,

        /// <summary>
        /// The Cookie header, which specifies cookie data presented to the server. 
        /// </summary>
        Cookie,

        /// <summary>
        /// The Expect header, which specifies particular server behaviors that are required by the client. 
        /// </summary>
        Expect,

        /// <summary>
        /// The From header, which specifies an Internet E-mail address for the human user who controls the requesting user agent. 
        /// </summary>
        From,

        /// <summary>
        /// The Host header, which specifies the host name and port number of the resource being requested. 
        /// </summary>
        Host,

        /// <summary>
        /// The If-Match header, which specifies that the requested operation should be performed only if the client's cached copy of the indicated resource is current. 
        /// </summary>
        IfMatch,

        /// <summary>
        /// The If-Modified-Since header, which specifies that the requested operation should be performed only if the requested resource has been modified since the indicated data and time. 
        /// </summary>
        IfModifiedSince,

        /// <summary>
        /// The If-None-Match header, which specifies that the requested operation should be performed only if none of client's cached copies of the indicated resources are current. 
        /// </summary>
        IfNoneMatch,

        /// <summary>
        /// The If-Range header, which specifies that only the specified range of the requested resource should be sent, if the client's cached copy is current. 
        /// </summary>
        IfRange,

        /// <summary>
        /// The If-Unmodified-Since header, which specifies that the requested operation should be performed only if the requested resource has not been modified since the indicated date and time. 
        /// </summary>
        IfUnmodifiedSince,

        /// <summary>
        /// The Max-Forwards header, which specifies an integer indicating the remaining number of times that this request may be forwarded. 
        /// </summary>
        MaxForwards,

        /// <summary>
        /// The Proxy-Authorization header, which specifies the credentials that the client presents in order to authenticate itself to a proxy. 
        /// </summary>
        ProxyAuthorization,

        /// <summary>
        /// The Referer header, which specifies the URI of the resource from which the request URI was obtained. 
        /// </summary>
        Referer,

        /// <summary>
        /// The Range header, which specifies the the sub-range(s) of the response that the client requests be returned in lieu of the entire response. 
        /// </summary>
        Range,

        /// <summary>
        /// The TE header, which specifies the transfer encodings that are acceptable for the response. 
        /// </summary>
        Te,

        /// <summary>
        /// The Translate header, a Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality. 
        /// </summary>
        Translate,

        /// <summary>
        /// The User-Agent header, which specifies information about the client agent. 
        /// </summary>
        UserAgent,

        // NOTE: values beyond UseAgent may be out of order from the desktop version of the framework.

        /// <summary>The Wlc-Safe-Agent header.</summary>
        WlcSafeAgentHeaderName,

        /// <summary>The Slug header.</summary>
        Slug
    }
}

