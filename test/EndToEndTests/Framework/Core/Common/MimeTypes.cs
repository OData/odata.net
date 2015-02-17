//---------------------------------------------------------------------
// <copyright file="MimeTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Common
{
    /// <summary>
    /// A collection of constants for commonly-used HTTP mime types
    /// </summary>
    public static class MimeTypes
    {
        /// <summary>
        /// The '*/*' mime type
        /// </summary>
        public const string Any = "*/*";

        /// <summary>
        /// The 'application/atom+xml' mime type
        /// </summary>
        public const string ApplicationAtomXml = "application/atom+xml";

        /// <summary>
        /// The 'application/json' mime type
        /// </summary>
        public const string ApplicationJson = "application/json";

        /// <summary>
        /// The 'odata.metadata=none' parameter and value
        /// </summary>
        public const string ODataParameterNoMetadata = ";odata.metadata=none";

        /// <summary>
        /// The 'odata.metadata=minimal' parameter and value
        /// </summary>
        public const string ODataParameterMinimalMetadata = ";odata.metadata=minimal";

        /// <summary>
        /// The 'odata.metadata=full' parameter and value
        /// </summary>
        public const string ODataParameterFullMetadata = ";odata.metadata=full";

        /// <summary>
        /// The 'IEEE754Compatible=true' parameter and value
        /// </summary>
        public const string ODataParameterIEEE754Compatible = ";IEEE754Compatible=true";

        /// <summary>
        /// The 'streaming=true' parameter and value
        /// </summary>
        public const string StreamingParameterTrue = ";odata.streaming=true";

        /// <summary>
        /// The 'streaming=true' parameter and value
        /// </summary>
        public const string StreamingParameterFalse = ";odata.streaming=false";

        /// <summary>
        /// The 'application/json;odata.metadata=minimal' mime type
        /// </summary>
        public const string ApplicationJsonLight = ApplicationJson + ODataParameterMinimalMetadata;

        /// <summary>
        /// The 'application/json;odata.metadata=minimal;odata.streaming=true' mime type
        /// </summary>
        public const string ApplicationJsonODataLightStreaming = ApplicationJsonLight + StreamingParameterTrue;

        /// <summary>
        /// The 'application/json;odata.metadata=minimal;odata.streaming=false' mime type
        /// </summary>
        public const string ApplicationJsonODataLightNonStreaming = ApplicationJsonLight + StreamingParameterFalse;

        /// <summary>
        /// The 'application/octet-stream' mime type
        /// </summary>
        public const string ApplicationOctetStream = "application/octet-stream";

        /// <summary>
        /// The 'application/xml' mime type
        /// </summary>
        public const string ApplicationXml = "application/xml";
        
        /// <summary>
        /// The 'text/plain' mime type
        /// </summary>
        public const string TextPlain = "text/plain";

        /// <summary>
        /// The 'text/html' mime type
        /// </summary>
        public const string TextHtml = "text/html";

        /// <summary>
        /// The 'multipart/mime' mime type
        /// </summary>
        public const string MultipartMixed = "multipart/mixed";

        /// <summary>
        /// The 'application/http' mime type
        /// </summary>
        public const string ApplicationHttp = "application/http";

        /// <summary>
        /// The 'application/x-www-form-urlencoded' mime type
        /// </summary>
        public const string ApplicationFormUrlEncoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// The 'application/jpeg' mime type
        /// </summary>
        public const string ApplicationJpeg = "application/jpeg";
    }
}
