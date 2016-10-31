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
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Constant values used by the OData or HTTP protocol or OData library.
    /// </summary>
#if ODATALIB
    public static class ODataConstants
#else
    internal static class ODataInternalConstants
#endif
    {
        /// <summary>
        /// HTTP method name for GET requests.
        /// </summary>
        public const string MethodGet = "GET";

        /// <summary>
        /// HTTP method name for POST requests.
        /// </summary>
        public const string MethodPost = "POST";

        /// <summary>
        /// HTTP method name for PUT requests.
        /// </summary>
        public const string MethodPut = "PUT";

        /// <summary>
        /// HTTP method name for DELETE requests.
        /// </summary>
        public const string MethodDelete = "DELETE";

        /// <summary>
        /// HTTP method name for PATCH requests.
        /// </summary>
        public const string MethodPatch = "PATCH";

        /// <summary>
        /// Custom HTTP method name for MERGE requests.
        /// </summary>
        public const string MethodMerge = "MERGE";

        /// <summary>
        /// Name of the HTTP content type header.
        /// </summary>
        public const string ContentTypeHeader = "Content-Type";

        /// <summary>
        /// Name of the OData 'DataServiceVersion' HTTP header.
        /// </summary>
        public const string DataServiceVersionHeader = "DataServiceVersion";

        /// <summary>
        /// Name of the HTTP content-ID header.
        /// </summary>
        public const string ContentIdHeader = "Content-ID";

        /// <summary>
        /// Name of the Content-Length HTTP header.
        /// </summary>
        internal const string ContentLengthHeader = "Content-Length";

        /// <summary>
        /// 'q' - HTTP q-value parameter name.
        /// </summary>
        internal const string HttpQValueParameter = "q";

        /// <summary>Http Version in batching requests and response.</summary>
        internal const string HttpVersionInBatching = "HTTP/1.1";

        /// <summary>'charset' - HTTP parameter name.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Charset", Justification = "Member name chosen based on HTTP header name.")]
        internal const string Charset = "charset";

        /// <summary>multi-part keyword in content-type to identify batch separator</summary>
        internal const string HttpMultipartBoundary = "boundary";

        /// <summary>Name of the HTTP content transfer encoding header.</summary>
        internal const string ContentTransferEncoding = "Content-Transfer-Encoding";

        /// <summary>Content-Transfer-Encoding value for batch payloads.</summary>
        internal const string BatchContentTransferEncoding = "binary";

        // TODO: What should be the default version of ODataLib?

        /// <summary>The default protocol version to use in ODataLib if none is specified.</summary>
#if DISABLE_V3
        internal const ODataVersion ODataDefaultProtocolVersion = ODataVersion.V2;
#else
        internal const ODataVersion ODataDefaultProtocolVersion = ODataVersion.V3;
#endif

        /// <summary>The template used when computing a batch request boundary.</summary>
        internal const string BatchRequestBoundaryTemplate = "batch_{0}";

        /// <summary>The template used when computing a batch response boundary.</summary>
        internal const string BatchResponseBoundaryTemplate = "batchresponse_{0}";

        /// <summary>The template used when computing a request changeset boundary.</summary>
        internal const string RequestChangeSetBoundaryTemplate = "changeset_{0}";

        /// <summary>The template used when computing a response changeset boundary.</summary>
        internal const string ResponseChangeSetBoundaryTemplate = "changesetresponse_{0}";

        /// <summary>Weak etags in HTTP must start with W/.
        /// Look in http://www.ietf.org/rfc/rfc2616.txt?number=2616 section 14.19 for more information.</summary>
        internal const string HttpWeakETagPrefix = "W/\"";

        /// <summary>Weak etags in HTTP must end with ".
        /// Look in http://www.ietf.org/rfc/rfc2616.txt?number=2616 section 14.19 for more information.</summary>
        internal const string HttpWeakETagSuffix = "\"";

        /// <summary>The default maximum allowed recursion depth for recursive payload definitions, such as complex values inside complex values.</summary>
        internal const int DefaultMaxRecursionDepth = 100;

        /// <summary>The default maximum number of bytes that should be read from a message.</summary>
        internal const long DefaultMaxReadMessageSize = 1024 * 1024;

        /// <summary>The default maximum number of top-level operations and changesets per batch payload.</summary>
        internal const int DefaultMaxPartsPerBatch = 100;

        /// <summary>The default maximum number of operations per changeset.</summary>
        internal const int DefulatMaxOperationsPerChangeset = 1000;

        /// <summary>The default maximum number of entity property mapping attributes for an entity type (on the type itself and all its base types).</summary>
        internal const int DefaultMaxEntityPropertyMappingsPerType = 100;

        /// <summary>The maximum recognized OData version by this library.</summary>
        internal const ODataVersion MaxODataVersion = ODataVersion.V3;

        /// <summary>The '/' (forward slash) which is the URI segment separator.</summary>
        internal const string UriSegmentSeparator = "/";

        /// <summary>The '/' (forward slash) which is the URI segment separator.</summary>
        internal const char UriSegmentSeparatorChar = '/';

        /// <summary>The '$links' segment name for constructing association links.</summary>
        internal const string AssociationLinkSegmentName = "$links";

        /// <summary>The '$value' segment name for the default stream value.</summary>
        internal const string DefaultStreamSegmentName = "$value";
    }
}
