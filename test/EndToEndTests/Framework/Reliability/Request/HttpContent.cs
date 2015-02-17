//---------------------------------------------------------------------
// <copyright file="HttpContent.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    /// <summary>
    /// Http Content Type
    /// </summary>
    public enum HttpContentType
    {
        /// <summary>
        /// Http Content Type Json
        /// </summary>
        Json,

        /// <summary>
        /// Http Content Type JsonVerbose
        /// </summary>
        JsonVerbose,

        /// <summary>
        /// Http Content Type JsonLight
        /// </summary>
        JsonLight,

        /// <summary>
        /// Http Content Type Atom
        /// </summary>
        Atom,

        /// <summary>
        /// Http Content Type Json
        /// </summary>
        Xml,

        /// <summary>
        /// Http Content Type Plain
        /// </summary>
        Plain,

        /// <summary>
        /// Http Content Type Multipart
        /// </summary>
        Multipart,

        /// <summary>
        /// Http Content Type Json
        /// </summary>
        Any,

        /// <summary>
        /// Http Content Type None
        /// </summary>
        None,

        /// <summary>
        /// Http Content Type Jpeg
        /// </summary>
        Jpeg,

        /// <summary>
        /// Http Content Type Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Http Content Type Batch
        /// </summary>
        Batch
    }

    /// <summary>
    /// Http content type extension
    /// </summary>
    public static class HttpContentTypeExtension
    {
        /// <summary>
        /// To http header
        /// </summary>
        /// <param name="contentType">Http content type</param>
        /// <returns>Http header</returns>
        public static string ToHttpHeader(this HttpContentType contentType)
        {
            switch (contentType)
            {
                case HttpContentType.Json:
                    return "application/json";
                case HttpContentType.JsonLight:
                    return "application/json;odata.metadata=minimal";
                case HttpContentType.Atom:
                    return "application/atom+xml";
                case HttpContentType.Xml:
                    return "application/xml";
                case HttpContentType.Plain:
                    return "text/plain";
                case HttpContentType.Multipart:
                    return "multipart/mixed";
                case HttpContentType.Any:
                    return "*/*";
                case HttpContentType.Jpeg:
                    return "image/jpeg";
                case HttpContentType.None:
                    return null;
            }

            return null;
        }

        /// <summary>
        /// Parse the content type
        /// </summary>
        /// <param name="contentType">the content type header</param>
        /// <returns>The HttpContentType enum</returns>
        public static HttpContentType Parse(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return HttpContentType.None;
            }

            contentType = contentType.ToLower();           
            if (contentType.StartsWith("application/json;odata.metadata=minimal"))
            {
                return HttpContentType.JsonLight;
            }

            if (contentType.StartsWith("application/json"))
            {
                return HttpContentType.Json;
            }

            if (contentType.StartsWith("application/atom+xml"))
            {
                return HttpContentType.Atom;
            }

            if (contentType.StartsWith("application/xml"))
            {
                return HttpContentType.Xml;
            }

            if (contentType.StartsWith("text/plain"))
            {
                return HttpContentType.Plain;
            }

            if (contentType.StartsWith("*/*"))
            {
                return HttpContentType.Any;
            }

            if (contentType.StartsWith("image/jpeg"))
            {
                return HttpContentType.Jpeg;
            }

            return HttpContentType.Unknown;
        }
    }
}
