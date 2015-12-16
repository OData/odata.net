//---------------------------------------------------------------------
// <copyright file="ODataConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
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
        /// Name of the HTTP content type header.
        /// </summary>
        public const string ContentTypeHeader = "Content-Type";

        /// <summary>
        /// Name of the OData 'OData-Version' HTTP header.
        /// </summary>
        public const string ODataVersionHeader = "OData-Version";

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

        /// <summary>Http Version in async responses.</summary>
        internal const string HttpVersionInAsync = "HTTP/1.1";

        /// <summary>'charset' - HTTP parameter name.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Charset", Justification = "Member name chosen based on HTTP header name.")]
        internal const string Charset = "charset";

        /// <summary>multi-part keyword in content-type to identify batch separator</summary>
        internal const string HttpMultipartBoundary = "boundary";

        /// <summary>Name of the HTTP content transfer encoding header.</summary>
        internal const string ContentTransferEncoding = "Content-Transfer-Encoding";

        /// <summary>Content-Transfer-Encoding value for batch payloads.</summary>
        internal const string BatchContentTransferEncoding = "binary";

        /// <summary>The default protocol version to use in ODataLib if none is specified.</summary>
        internal const ODataVersion ODataDefaultProtocolVersion = ODataVersion.V4;

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

        /// <summary>The '/' (forward slash) which is the URI segment separator.</summary>
        internal const string UriSegmentSeparator = "/";

        /// <summary>The '/' (forward slash) which is the URI segment separator.</summary>
        internal const char UriSegmentSeparatorChar = '/';

        /// <summary>The '$ref' segment name for constructing association links.</summary>
        internal const string EntityReferenceSegmentName = "$ref";

        /// <summary>The 'Collection' segment name for constructing collection of association links.</summary>
        internal const string EntityReferenceCollectionSegmentName = "Collection";

        /// <summary>The '$value' segment name for the default stream value.</summary>
        internal const string DefaultStreamSegmentName = "$value";

        /// <summary>The prefix of type name.</summary>
        internal const string TypeNamePrefix = "#";

        /// <summary>A segment name in a URI that indicates metadata is being requested.</summary>
        internal const string UriMetadataSegment = "$metadata";

        #region Context URL

        /// <summary>
        /// Constant "#Collection($ref)" used to represent collection of entity references in Context URL
        /// Note that if a response is a collection of entity references, the context URL does not contain the type of the referenced entities
        /// </summary>
        internal const string CollectionOfEntityReferencesContextUrlSegment = "#Collection($ref)";

        /// <summary>
        /// Constant "#$ref"used to represent single entity reference in Context URL
        /// Note that if a response is a collection of entity references, the context URL does not contain the type of the referenced entities
        /// </summary>
        internal const string SingleEntityReferencesContextUrlSegment = "#$ref";

        /// <summary>The hash sign acting as fragment indicator in a context URI.</summary>
        internal const char ContextUriFragmentIndicator = '#';

        /// <summary>The $entity token that indicates that the payload is a single item from a set.</summary>
        internal const string ContextUriFragmentItemSelector = "/$entity";

        /// <summary>The '(' used to mark the start of Select and Expand clauses in the fragment of a context URI.</summary>
        internal const char ContextUriProjectionStart = '(';

        /// <summary>The ')' used to mark the end of Select and Expand clauses in the fragment of a context URI.</summary>
        internal const char ContextUriProjectionEnd = ')';

        /// <summary>The "," used to split properties of Select and Expand fragment a context URI.</summary>
        internal const string ContextUriProjectionPropertySeparator = ",";

        /// <summary>The token that indicates the payload is a property with null value.</summary>
        internal const string ContextUriFragmentNull = "Edm.Null";

        /// <summary>The token that indicates the payload is a property with an untyped value.</summary>
        internal const string ContextUriFragmentUntyped = "Edm.Untyped";

        /// <summary>The $delta token indicates delta feed.</summary>
        internal const string ContextUriDeltaFeed = "/$delta";

        /// <summary>The $deletedEntity token indicates delta entry.</summary>
        internal const string ContextUriDeletedEntry = "/$deletedEntity";

        /// <summary>The $delta token indicates delta link.</summary>
        internal const string ContextUriDeltaLink = "/$link";

        /// <summary>The $deletedLink token indicates delta deleted link.</summary>
        internal const string ContextUriDeletedLink = "/$deletedLink";
        #endregion Context URL
    }
}
