//---------------------------------------------------------------------
// <copyright file="XmlConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Service
#endif
{
    /// <summary>
    /// Class that contains all the constants for various schemas.
    /// </summary>
    internal static class XmlConstants
    {
        #region CLR / Reflection constants.

        /// <summary>"InitializeService" method name for service initialize.</summary>
        internal const string ClrServiceInitializationMethodName = "InitializeService";

        #endregion CLR / Reflection constants.

        #region HTTP constants.

        /// <summary>id of the corresponding body</summary>
        internal const string HttpContentID = "Content-ID";

        /// <summary>byte-length of the corresponding body</summary>
        internal const string HttpContentLength = "Content-Length";

        /// <summary>mime-type of the corresponding body</summary>
        internal const string HttpContentType = "Content-Type";

        /// <summary>content disposition of the response (a hint how to handle the response)</summary>
        internal const string HttpContentDisposition = "Content-Disposition";

        /// <summary>'OData-Version' - HTTP header name for OData version.</summary>
        internal const string HttpODataVersion = "OData-Version";

        /// <summary>'OData-MaxVersion' - HTTP header name for maximum understood OData version.</summary>
        internal const string HttpODataMaxVersion = "OData-MaxVersion";

        /// <summary>
        /// 'Prefer' - HTTP request header name for client preferences.
        /// Refer to: http://tools.ietf.org/id/draft-snell-http-prefer-02.txt for details.
        /// </summary>
        internal const string HttpPrefer = "Prefer";

#if ODATA_CLIENT
        /// <summary>Return no content Prefer header hint value.</summary>
        internal const string HttpPreferReturnNoContent = "return=minimal";

        /// <summary>Return content Prefer header hint value.</summary>
        internal const string HttpPreferReturnContent = "return=representation";
#endif

#if !ODATA_CLIENT
        /// <summary>
        /// 'Preference-Applied' - HTTP response header name for client preference response.
        /// </summary>
        internal const string HttpPreferenceApplied = "Preference-Applied";
#endif

        /// <summary>'no-cache' - HTTP value for Cache-Control header.</summary>
        internal const string HttpCacheControlNoCache = "no-cache";

        /// <summary>'charset' - HTTP parameter name.</summary>
        internal const string HttpCharsetParameter = "charset";

        /// <summary>HTTP method name for GET requests.</summary>
        internal const string HttpMethodGet = "GET";

        /// <summary>HTTP method name for POST requests.</summary>
        internal const string HttpMethodPost = "POST";

        /// <summary> Http Put Method name - basically used for updating resource.</summary>
        internal const string HttpMethodPut = "PUT";

        /// <summary>HTTP method name for delete requests.</summary>
        internal const string HttpMethodDelete = "DELETE";

        /// <summary>HTTP method name for PATCH requests.</summary>
        internal const string HttpMethodPatch = "PATCH";

        /// <summary>HTTP query string parameter value for expand.</summary>
        internal const string HttpQueryStringExpand = "$expand";

        /// <summary>HTTP query string parameter value for filtering.</summary>
        internal const string HttpQueryStringFilter = "$filter";

        /// <summary>HTTP query string parameter value for ordering.</summary>
        internal const string HttpQueryStringOrderBy = "$orderby";

        /// <summary>HTTP query string parameter value for skipping elements.</summary>
        internal const string HttpQueryStringSkip = "$skip";

        /// <summary>HTTP query string parameter value for limiting the number of elements.</summary>
        internal const string HttpQueryStringTop = "$top";

        /// <summary>HTTP query string parameter value for counting query result set, $count=true</summary>
        internal const string HttpQueryStringQueryCount = "$count";

        /// <summary>HTTP query string parameter value for skipping results based on paging.</summary>
        internal const string HttpQueryStringSkipToken = "$skiptoken";

        /// <summary>Property prefix for the skip token property in expanded results for a skip token</summary>
        internal const string SkipTokenPropertyPrefix = "SkipTokenProperty";

        /// <summary>HTTP query string parameter value for counting query result set</summary>
        internal const string HttpQueryStringSegmentCount = "$count";

        /// <summary>HTTP query string parameter value for projection.</summary>
        internal const string HttpQueryStringSelect = "$select";

        /// <summary>HTTP query string parameter for specifying the requested content-type of the response.</summary>
        internal const string HttpQueryStringFormat = "$format";

        /// <summary>HTTP query string parameter for specifying the a callback function name for JSONP (JSON padding).</summary>
        internal const string HttpQueryStringCallback = "$callback";

        /// <summary>HTTP query string parameter for specifying the a entity id.</summary>
        internal const string HttpQueryStringId = "$id";

        /// <summary>'q' - HTTP q-value parameter name.</summary>
        internal const string HttpQValueParameter = "q";

        /// <summary>'X-HTTP-Method' - HTTP header name for requests that want to tunnel a method through POST.</summary>
        internal const string HttpXMethod = "X-HTTP-Method";

        /// <summary>HTTP name for Accept header</summary>
        internal const string HttpRequestAccept = "Accept";

        /// <summary>HTTP name for Accept-Charset header</summary>
        internal const string HttpRequestAcceptCharset = "Accept-Charset";

        /// <summary>HTTP name for If-Match header</summary>
        internal const string HttpRequestIfMatch = "If-Match";

        /// <summary>HTTP name for If-None-Match header</summary>
        internal const string HttpRequestIfNoneMatch = "If-None-Match";

        /// <summary>HTTP name for User-Agent header</summary>
        internal const string HttpUserAgent = "User-Agent";

        /// <summary>multi-part keyword in content-type to identify batch separator</summary>
        internal const string HttpMultipartBoundary = "boundary";

        /// <summary>'X-Content-Type-Options' - HTTP header for Internet Explorer 8 and 9 to specify options for content-type handling.</summary>
        internal const string XContentTypeOptions = "X-Content-Type-Options";

        /// <summary>An 'X-Content-Type-Options' HTTP header argument to instruct IE8/9 not to sniff the content and instead display it according to the content-type header.</summary>
        internal const string XContentTypeOptionNoSniff = "nosniff";
#if ODATA_CLIENT
        /// <summary>multi-part mixed batch separator</summary>
        internal const string HttpMultipartBoundaryBatch = "batch";

        /// <summary>multi-part mixed changeset separator</summary>
        internal const string HttpMultipartBoundaryChangeSet = "changeset";
#endif

        /// <summary>'Allow' - HTTP response header for allowed verbs.</summary>
        internal const string HttpResponseAllow = "Allow";

        /// <summary>HTTP name for Cache-Control header.</summary>
        internal const string HttpResponseCacheControl = "Cache-Control";

        /// <summary>HTTP name for ETag header</summary>
        internal const string HttpResponseETag = "ETag";

        /// <summary>HTTP name for location header</summary>
        internal const string HttpResponseLocation = "Location";

        /// <summary>HTTP name for OData-EntityId header</summary>
        internal const string HttpODataEntityId = "OData-EntityId";

        /// <summary>HTTP name for Status-Code header</summary>
        internal const string HttpResponseStatusCode = "Status-Code";

        /// <summary>multi-part mixed batch separator for response stream</summary>
        internal const string HttpMultipartBoundaryBatchResponse = "batchresponse";

        /// <summary>multi-part mixed changeset separator</summary>
        internal const string HttpMultipartBoundaryChangesetResponse = "changesetresponse";

        /// <summary>Content-Transfer-Encoding header for batch requests.</summary>
        internal const string HttpContentTransferEncoding = "Content-Transfer-Encoding";

        /// <summary>Http Version in batching requests and response.</summary>
        internal const string HttpVersionInBatching = "HTTP/1.1";

        /// <summary>To checks if the resource exists or not.</summary>
        internal const string HttpAnyETag = "*";

        /// <summary>Weak etags in HTTP must start with W/.
        /// Look in http://www.ietf.org/rfc/rfc2616.txt?number=2616 section 14.19 for more information.</summary>
        internal const string HttpWeakETagPrefix = "W/\"";

        /// <summary>The mime type that client wants the response to be in.</summary>
        internal const string HttpAccept = "Accept";

        /// <summary>The character set the client wants the response to be in.</summary>
        internal const string HttpAcceptCharset = "Accept-Charset";

        /// <summary>The name of the Cookie HTTP header</summary>
        internal const string HttpCookie = "Cookie";

        /// <summary>The Slug header name. Used by ATOM to hint the server on which MR is being POSTed.</summary>
        internal const string HttpSlug = "Slug";

        #endregion HTTP constants.

        #region MIME constants.

        /// <summary>MIME type for requesting any media type.</summary>
        internal const string MimeAny = "*/*";

        /// <summary>MIME type general binary bodies (http://www.iana.org/assignments/media-types/application/).</summary>
        internal const string MimeApplicationOctetStream = "application/octet-stream";
#if !ODATA_CLIENT
        /// <summary>MIME type for ATOM bodies (http://www.iana.org/assignments/media-types/application/).</summary>
        internal const string MimeApplicationAtom = "application/atom+xml";

        /// <summary>MIME type for JSON bodies in light mode with minimal metadata.</summary>
        internal const string MimeApplicationJsonODataMinimalMetadata = "application/json;odata.metadata=minimal";

        /// <summary>MIME type for JSON bodies in light mode with full metadata.</summary>
        internal const string MimeApplicationJsonODataFullMetadata = "application/json;odata.metadata=full";

        /// <summary>MIME type for JSON bodies in light mode with no metadata.</summary>
        internal const string MimeApplicationJsonODataNoMetadata = "application/json;odata.metadata=none";

        /// <summary>MIME type for JSON bodies (implies light in V3, verbose otherwise) (http://www.iana.org/assignments/media-types/application/).</summary>
        internal const string MimeApplicationJson = "application/json";

        /// <summary>MIME type for batch requests - this mime type must be specified in CUD changesets or GET batch requests.</summary>
        internal const string MimeApplicationHttp = "application/http";

        /// <summary>MIME type for XML bodies.</summary>
        internal const string MimeApplicationXml = "application/xml";

        /// <summary>"application/xml", MIME type for metadata requests.</summary>
        internal const string MimeMetadata = MimeApplicationXml;
#endif
        /// <summary>'application' - MIME type for application types.</summary>
        internal const string MimeApplicationType = "application";

        /// <summary>'json' - constant for MIME JSON subtypes.</summary>
        internal const string MimeJsonSubType = "json";

        /// <summary>'xml' - constant for MIME xml subtypes.</summary>
        internal const string MimeXmlSubType = "xml";

        /// <summary>'odata' - parameter name for JSON MIME types.</summary>
        internal const string MimeODataParameterName = "odata.metadata";

        /// <summary>MIME type for changeset multipart/mixed</summary>
        internal const string MimeMultiPartMixed = "multipart/mixed";

        /// <summary>MIME type for plain text bodies.</summary>
        internal const string MimeTextPlain = "text/plain";

        /// <summary>'text' - MIME type for text subtypes.</summary>
        internal const string MimeTextType = "text";

        /// <summary>MIME type for XML bodies (deprecated).</summary>
        internal const string MimeTextXml = "text/xml";

        /// <summary>Content-Transfer-Encoding value for batch requests.</summary>
        internal const string BatchRequestContentTransferEncoding = "binary";

#if !ODATA_CLIENT
        /// <summary>text for the utf8 encoding</summary>
        internal const string Utf8Encoding = "UTF-8";
#endif
#if ODATA_CLIENT
        /// <summary>Default encoding used for writing textual media link entries</summary>
        internal const string MimeTypeUtf8Encoding = ";charset=UTF-8";
#endif
        #endregion MIME constants.

        #region URI constants.

        /// <summary>A prefix that turns an absolute-path URI into an absolute-URI.</summary>
        internal const string UriHttpAbsolutePrefix = "http://host";

        /// <summary>A segment name in a URI that indicates metadata is being requested.</summary>
        internal const string UriMetadataSegment = "$metadata";

        /// <summary>A segment name in a URI that indicates a plain primitive value is being requested.</summary>
        internal const string UriValueSegment = "$value";

        /// <summary>A segment name in a URI that indicates metadata is being requested.</summary>
        internal const string UriBatchSegment = "$batch";

        /// <summary>A segment name in a URI that indicates that this is a link operation.</summary>
        internal const string UriLinkSegment = "$ref";

        /// <summary>A segment name in a URI that indicates that this is a count operation.</summary>
        internal const string UriCountSegment = "$count";

        /// <summary>A const value for query parameter $count to set counting mode to true </summary>
        internal const string UriCountTrueOption = "true";

        /// <summary>A const value for query parameter $count to set counting mode to false </summary>
        internal const string UriCountFalseOption = "false";

        /// <summary>A segment name in a URI that indicates that this is a filter operation.</summary>
        internal const string UriFilterSegment = "$filter";

        /// <summary>Uri method name for Enumerable.Any().</summary>
        internal const string AnyMethodName = "any";

        /// <summary>Uri method name for Enumerable.All().</summary>
        internal const string AllMethodName = "all";

        /// <summary>Implicit parameter "it" used for Queryable.Where lambda.</summary>
        internal const string ImplicitFilterParameter = "$it";

        #endregion URI constants.

        #region OData constants

        /// <summary>The context URL for a collection, entity, primitive value, or service document.</summary>
        internal const string ODataContext = "@odata.context";

        /// <summary>The ID of th entity.</summary>
        internal const string ODataID = "@odata.id";

        /// <summary>The total count of a collection of entities or collection of entity references, if requested.</summary>
        internal const string ODataCount = "@odata.count";

        /// <summary>the type of the containing object or targeted property if the type of the object or targeted property cannot be heuristically determined.</summary>
        internal const string ODataType = "@odata.type";

        #endregion OData constants

        #region WCF constants.

        /// <summary>"Binary" - WCF element name for binary content in XML-wrapping streams.</summary>
        internal const string WcfBinaryElementName = "Binary";

        #endregion WCF constants.

        #region ATOM constants
        /// <summary> Schema Namespace prefix for atom.</summary>
        internal const string AtomNamespacePrefix = "atom";

        /// <summary>XML element name to mark content element in Atom.</summary>
        internal const string AtomContentElementName = "content";

        /// <summary>XML element name to mark entry element in Atom.</summary>
        internal const string AtomEntryElementName = "entry";

        /// <summary>XML element name to mark feed element in Atom.</summary>
        internal const string AtomFeedElementName = "feed";

        /// <summary>'author' - XML element name for ATOM 'author' element for entries.</summary>
        internal const string AtomAuthorElementName = "author";

        /// <summary>'contributor' - XML element name for ATOM 'author' element for entries.</summary>
        internal const string AtomContributorElementName = "contributor";

        /// <summary>'category' - XML element name for ATOM 'category' element for entries.</summary>
        internal const string AtomCategoryElementName = "category";

        /// <summary>XML element name to mark link element in Atom.</summary>
        internal const string AtomLinkElementName = "link";

#if ODATA_CLIENT
        /// <summary>'scheme' - XML attribute name for ATOM 'scheme' attribute for categories.</summary>
        internal const string AtomCategorySchemeAttributeName = "scheme";

        /// <summary>'term' - XML attribute name for ATOM 'term' attribute for categories.</summary>
        internal const string AtomCategoryTermAttributeName = "term";

        /// <summary>XML element name to mark id element in Atom.</summary>
        internal const string AtomIdElementName = "id";

        /// <summary>XML element name to mark link relation attribute in Atom.</summary>
        internal const string AtomLinkRelationAttributeName = "rel";

        /// <summary>Atom attribute that indicates the actual location for an entry's content.</summary>
        internal const string AtomContentSrcAttributeName = "src";

        /// <summary>XML element string for "next" links: [atom:link rel="next"]</summary>
        internal const string AtomLinkNextAttributeString = "next";

#endif
        /// <summary>author/email</summary>
        internal const string SyndAuthorEmail = "SyndicationAuthorEmail";

        /// <summary>author/name</summary>
        internal const string SyndAuthorName = "SyndicationAuthorName";

        /// <summary>author/uri</summary>
        internal const string SyndAuthorUri = "SyndicationAuthorUri";

        /// <summary>published</summary>
        internal const string SyndPublished = "SyndicationPublished";

        /// <summary>rights</summary>
        internal const string SyndRights = "SyndicationRights";

        /// <summary>summary</summary>
        internal const string SyndSummary = "SyndicationSummary";

        /// <summary>title</summary>
        internal const string SyndTitle = "SyndicationTitle";

        /// <summary>'updated' - XML element name for ATOM 'updated' element for entries.</summary>
        internal const string AtomUpdatedElementName = "updated";

        /// <summary>contributor/email</summary>
        internal const string SyndContributorEmail = "SyndicationContributorEmail";

        /// <summary>contributor/name</summary>
        internal const string SyndContributorName = "SyndicationContributorName";

        /// <summary>contributor/uri</summary>
        internal const string SyndContributorUri = "SyndicationContributorUri";

        /// <summary>updated</summary>
        internal const string SyndUpdated = "SyndicationUpdated";

        /// <summary>Plaintext</summary>
        internal const string SyndContentKindPlaintext = "text";

        /// <summary>HTML</summary>
        internal const string SyndContentKindHtml = "html";

        /// <summary>XHTML</summary>
        internal const string SyndContentKindXHtml = "xhtml";

        /// <summary>XML element name to mark href attribute element in Atom.</summary>
        internal const string AtomHRefAttributeName = "href";

        /// <summary>XML attribute name to mark the hreflang attribute in Atom.</summary>
        internal const string AtomHRefLangAttributeName = "hreflang";

        /// <summary>XML element name to mark summary element in Atom.</summary>
        internal const string AtomSummaryElementName = "summary";

        /// <summary>XML element name to mark author/name or contributor/name element in Atom.</summary>
        internal const string AtomNameElementName = "name";

        /// <summary>XML element name to mark author/email or contributor/email element in Atom.</summary>
        internal const string AtomEmailElementName = "email";

        /// <summary>XML element name to mark published element in Atom.</summary>
        internal const string AtomPublishedElementName = "published";

        /// <summary>XML element name to mark rights element in Atom.</summary>
        internal const string AtomRightsElementName = "rights";

        /// <summary>XML element name to mark 'collection' element in APP.</summary>
        internal const string AtomPublishingCollectionElementName = "collection";

        /// <summary>XML element name to mark 'service' element in APP.</summary>
        internal const string AtomPublishingServiceElementName = "service";

        /// <summary>XML value for a default workspace in APP.</summary>
        internal const string AtomPublishingWorkspaceDefaultValue = "Default";

        /// <summary>XML element name to mark 'workspace' element in APP.</summary>
        internal const string AtomPublishingWorkspaceElementName = "workspace";

        /// <summary>XML element name to mark title element in Atom.</summary>
        internal const string AtomTitleElementName = "title";

        /// <summary>XML attribute name to specify the type of the element.</summary>
        internal const string AtomTypeAttributeName = "type";

        /// <summary> Atom link relation attribute value for self links.</summary>
        internal const string AtomSelfRelationAttributeValue = "self";

        /// <summary> Atom link relation attribute value for edit links.</summary>
        internal const string AtomEditRelationAttributeValue = "edit";

        /// <summary> Atom link relation attribute value for edit-media links.</summary>
        internal const string AtomEditMediaRelationAttributeValue = "edit-media";

        /// <summary>Link relation: alternate - refers to a substitute for this context.</summary>
        internal const string AtomAlternateRelationAttributeValue = "alternate";

        /// <summary>Link relation: related - identifies a related resource.</summary>
        internal const string AtomRelatedRelationAttributeValue = "related";

        /// <summary>Link relation: enclosure - identifies a related resource that is potentially large and might require special handling.</summary>
        internal const string AtomEnclosureRelationAttributeValue = "enclosure";

        /// <summary>Link relation: via - identifies a resource that is the source of the information in the link's context.</summary>
        internal const string AtomViaRelationAttributeValue = "via";

        /// <summary>Link relation: describedby - refers to a resource providing information about the link's context.</summary>
        internal const string AtomDescribedByRelationAttributeValue = "describedby";

        /// <summary>Link relation: service - indicates a URI that can be used to retrieve a service document.</summary>
        internal const string AtomServiceRelationAttributeValue = "service";

        /// <summary> Atom attribute which indicates the null value for the element.</summary>
        internal const string AtomNullAttributeName = "null";

        /// <summary> Atom attribute which indicates the etag value for the declaring entry element.</summary>
        internal const string AtomETagAttributeName = "etag";

        /// <summary>'Inline' - wrapping element for inlined entry/feed content.</summary>
        internal const string AtomInlineElementName = "inline";

        /// <summary>Element containing property values when 'content' is used for media link entries</summary>
        internal const string AtomPropertiesElementName = "properties";

        /// <summary>'count' element</summary>
        internal const string RowCountElement = "count";

        #endregion ATOM constants

        #region XML constants.

        /// <summary>'element', the XML element name for items in enumerations.</summary>
        internal const string XmlCollectionItemElementName = "element";

        /// <summary>XML element name for an error.</summary>
        internal const string XmlErrorElementName = "error";

        /// <summary>XML element name for an error code.</summary>
        internal const string XmlErrorCodeElementName = "code";

        /// <summary>XML element name for the inner error details.</summary>
        internal const string XmlErrorInnerElementName = "innererror";

        /// <summary>XML element name for an internal exception.</summary>
        internal const string XmlErrorInternalExceptionElementName = "internalexception";

        /// <summary>XML element name for an exception type.</summary>
        internal const string XmlErrorTypeElementName = "type";

        /// <summary>XML element name for an exception stack trace.</summary>
        internal const string XmlErrorStackTraceElementName = "stacktrace";

        /// <summary>XML element name for an error message.</summary>
        internal const string XmlErrorMessageElementName = "message";

        /// <summary>'false' literal, as used in XML.</summary>
        internal const string XmlFalseLiteral = "false";

        /// <summary>'true' literal, as used in XML.</summary>
        internal const string XmlTrueLiteral = "true";

        /// <summary>XML attribute value to indicate the base URI for a document or element.</summary>
        internal const string XmlBaseAttributeName = "base";

        /// <summary>XML attribute name for whitespace parsing control.</summary>
        internal const string XmlSpaceAttributeName = "space";

        /// <summary>XML attribute value to indicate whitespace should be preserved.</summary>
        internal const string XmlSpacePreserveValue = "preserve";

        /// <summary>XML attribute name to pass to the XMLReader.GetValue API to get the xml:base attribute value.</summary>
        internal const string XmlBaseAttributeNameWithPrefix = "xml:base";

        #endregion XML constants.

        #region XML namespaces.

        /// <summary> Schema Namespace For Edm Oasis.</summary>
        internal const string EdmOasisNamespace = "http://docs.oasis-open.org/odata/ns/edm";

        /// <summary>XML namespace for data services.</summary>
        internal const string DataWebNamespace = "http://docs.oasis-open.org/odata/ns/data";

        /// <summary>XML namespace for data service annotations.</summary>
        internal const string DataWebMetadataNamespace = "http://docs.oasis-open.org/odata/ns/metadata";

        /// <summary>XML namespace for data service links.</summary>
        internal const string DataWebRelatedNamespace = "http://docs.oasis-open.org/odata/ns/related/";

        /// <summary>XML namespace for data service related $ref.</summary>
        internal const string DataWebRelatedLinkNamespace = "http://docs.oasis-open.org/odata/ns/relatedlinks/";

        /// <summary>XML namespace for data service named media resources.</summary>
        internal const string DataWebMediaResourceNamespace = "http://docs.oasis-open.org/odata/ns/mediaresource/";

        /// <summary>XML namespace for data service edit-media link for named media resources.</summary>
        internal const string DataWebMediaResourceEditNamespace = "http://docs.oasis-open.org/odata/ns/edit-media/";

        /// <summary>ATOM Scheme Namespace For DataWeb.</summary>
        internal const string DataWebSchemeNamespace = "http://docs.oasis-open.org/odata/ns/scheme";

        /// <summary>Schema Namespace for Atom Publishing Protocol.</summary>
        internal const string AppNamespace = "http://www.w3.org/2007/app";

        /// <summary> Schema Namespace For Atom.</summary>
        internal const string AtomNamespace = "http://www.w3.org/2005/Atom";

        /// <summary> Schema Namespace prefix For xmlns.</summary>
        internal const string XmlnsNamespacePrefix = "xmlns";

        /// <summary> Schema Namespace prefix For xml.</summary>
        internal const string XmlNamespacePrefix = "xml";

        /// <summary> Schema Namespace Prefix For DataWeb.</summary>
        internal const string DataWebNamespacePrefix = "d";

        /// <summary>'adsm' - namespace prefix for DataWebMetadataNamespace.</summary>
        internal const string DataWebMetadataNamespacePrefix = "m";

        /// <summary>'http://www.w3.org/2000/xmlns/' - namespace for namespace declarations.</summary>
        internal const string XmlNamespacesNamespace = "http://www.w3.org/2000/xmlns/";

        /// <summary> Edmx namespace in metadata document.</summary>
        internal const string EdmxNamespace = "http://docs.oasis-open.org/odata/ns/edmx";

        /// <summary> Prefix for Edmx Namespace in metadata document.</summary>
        internal const string EdmxNamespacePrefix = "edmx";

        /// <summary>IANA link relations namespace.</summary>
        internal const string IanaLinkRelationsNamespace = "http://www.iana.org/assignments/relation/";

        /// <summary>The empty namespace.</summary>
        internal const string EmptyNamespace = "";

        #endregion XML namespaces.

        #region CDM Schema Xml NodeNames

        #region Constant node names in the CDM schema xml

        /// <summary> Association Element Name in csdl.</summary>
        internal const string Association = "Association";

        /// <summary> AssociationSet Element Name in csdl.</summary>
        internal const string AssociationSet = "AssociationSet";

        /// <summary> ComplexType Element Name in csdl.</summary>
        internal const string ComplexType = "ComplexType";

        /// <summary> Dependent Element Name in csdl.</summary>
        internal const string Dependent = "Dependent";

        /// <summary>The name of the EDM collection type.</summary>
        internal const string EdmCollectionTypeName = "Collection";

        /// <summary>
        /// Attribute name used to indicate the real type of an EDM property or parameter, in cases where it needs to be different
        /// from the Type attribute of the Property or Parameter element. This is used to support collection types and binary keys,
        /// which are incompatible with EDM 1.1, which we are currently using for validation purposes.
        /// This attribute is inserted into the CSDL in memory while codegen is processing properties that require special
        /// type handling and should only be used in that scenario. This is not a real EDM or Data Services attribute.
        /// </summary>
        internal const string ActualEdmType = "ActualEdmType";

        /// <summary>TypeRef element name in CSDL document.</summary>
        internal const string EdmTypeRefElementName = "TypeRef";

        /// <summary>EntitySet attribute name in CSDL documents.</summary>
        internal const string EdmEntitySetAttributeName = "EntitySet";

        /// <summary>EntitySetPath attribute name in CSDL documents.</summary>
        internal const string EdmEntitySetPathAttributeName = "EntitySetPath";

        /// <summary>ExtensionMethod attribute name in CSDL documents.</summary>
        internal const string EdmBindableAttributeName = "Bindable";

        /// <summary>Composable attribute name in CSDL documents.</summary>
        internal const string EdmComposableAttributeName = "Composable";

        /// <summary>SideEffecting attribute name in CSDL documents.</summary>
        internal const string EdmSideEffectingAttributeName = "SideEffecting";

        /// <summary>FunctionImport element name in CSDL documents.</summary>
        internal const string EdmFunctionImportElementName = "FunctionImport";

        /// <summary>Mode attribute name in CSDL documents.</summary>
        internal const string EdmModeAttributeName = "Mode";

        /// <summary>Mode attribute value for 'in' direction in CSDL documents.</summary>
        internal const string EdmModeInValue = "In";

        /// <summary>Parameter element name in CSDL documents.</summary>
        internal const string EdmParameterElementName = "Parameter";

        /// <summary>ReturnType attribute name in CSDL documents.</summary>
        internal const string EdmReturnTypeAttributeName = "ReturnType";

        /// <summary>
        /// Attribute name used to indicate the real type of an EDM operation import return type, in cases where it needs to be different
        /// from the ReturnType attribute of the operation import element. This is used to support special primitive types,
        /// which are incompatible with EDM 1.1, which we are currently using for validation purposes.
        /// This attribute is inserted into the CSDL in memory while codegen is processing operation imports that require special
        /// type handling and should only be used in that scenario. This is not a real EDM or Data Services attribute.
        /// </summary>
        internal const string ActualReturnTypeAttributeName = "ActualReturnType";

        /// <summary> End Element Name in csdl.</summary>
        internal const string End = "End";

        /// <summary> EntityType Element Name in csdl.</summary>
        internal const string EntityType = "EntityType";

        /// <summary> EntityContainer Element Name in csdl.</summary>
        internal const string EntityContainer = "EntityContainer";

        /// <summary> Key Element Name in csdl.</summary>
        internal const string Key = "Key";

        /// <summary> NavigationProperty Element Name in csdl.</summary>
        internal const string NavigationProperty = "NavigationProperty";

        /// <summary> OnDelete Element Name in csdl.</summary>
        internal const string OnDelete = "OnDelete";

        /// <summary> Principal Element Name in csdl.</summary>
        internal const string Principal = "Principal";

        /// <summary> Property Element Name in csdl.</summary>
        internal const string Property = "Property";

        /// <summary> PropetyRef Element Name in csdl.</summary>
        internal const string PropertyRef = "PropertyRef";

        /// <summary> ReferentialConstraint Element Name in csdl.</summary>
        internal const string ReferentialConstraint = "ReferentialConstraint";

        /// <summary> Role Element Name in csdl.</summary>
        internal const string Role = "Role";

        /// <summary> Schema Element Name in csdl.</summary>
        internal const string Schema = "Schema";

        /// <summary> Edmx Element Name in the metadata document.</summary>
        internal const string EdmxElement = "Edmx";

        /// <summary> Edmx DataServices Element Name in the metadata document.</summary>
        internal const string EdmxDataServicesElement = "DataServices";

        /// <summary>Version attribute for the root Edmx Element in the metadata document.</summary>
        internal const string EdmxVersion = "Version";

        /// <summary>Value of the version attribute in the root edmx element in metadata document.</summary>
        internal const string EdmxVersionValue = "4.0";

        #endregion //Constant node names in the CDM schema xml

        #region const attribute names in the CDM schema XML

        /// <summary>Element name for m:action.</summary>
        internal const string ActionElementName = "action";

        /// <summary>Element name for m:function</summary>
        internal const string FunctionElementName = "function";

        /// <summary>maps to m:action|m:function\@metadata</summary>
        internal const string ActionMetadataAttributeName = "metadata";

        /// <summary>maps to m:action|m:function\@target</summary>
        internal const string ActionTargetAttributeName = "target";

        /// <summary>maps to m:action|m:function\@title</summary>
        internal const string ActionTitleAttributeName = "title";

        /// <summary> BaseType attribute Name in csdl.</summary>
        internal const string BaseType = "BaseType";

        /// <summary> EntitySet attribute and Element Name in csdl.</summary>
        internal const string EntitySet = "EntitySet";

        /// <summary> EntitySetPath attribute and Element Name in csdl.</summary>
        internal const string EntitySetPath = "EntitySetPath";

        /// <summary> FromRole attribute Name in csdl.</summary>
        internal const string FromRole = "FromRole";

        /// <summary>Abstract attribute Name in csdl.</summary>
        internal const string Abstract = "Abstract";

        /// <summary>Multiplicity attribute Name in csdl.</summary>
        internal const string Multiplicity = "Multiplicity";

        /// <summary>Name attribute Name in csdl.</summary>
        internal const string Name = "Name";

        /// <summary>Namespace attribute Element Name in csdl.</summary>
        internal const string Namespace = "Namespace";

        /// <summary>ToRole attribute Name in csdl.</summary>
        internal const string ToRole = "ToRole";

        /// <summary>Type attribute Name in csdl.</summary>
        internal const string Type = "Type";

        /// <summary>Relationship attribute Name in csdl.</summary>
        internal const string Relationship = "Relationship";

        #endregion //const attribute names in the CDM schema XML

        #region values for multiplicity in Edm

        /// <summary>Value for Many multiplicity in csdl.</summary>
        internal const string Many = "*";

        /// <summary>Value for One multiplicity in csdl.</summary>
        internal const string One = "1";

        /// <summary>Value for ZeroOrOne multiplicity in csdl.</summary>
        internal const string ZeroOrOne = "0..1";
        #endregion

        #region Edm Facets Names and Values

        /// <summary>Nullable facet name in CSDL.</summary>
        internal const string CsdlNullableAttributeName = "Nullable";

        /// <summary>The attribute name of the 'Precision' facet.</summary>
        internal const string CsdlPrecisionAttributeName = "Precision";

        /// <summary>The attribute name of the 'Scale' facet.</summary>
        internal const string CsdlScaleAttributeName = "Scale";

        /// <summary>The attribute name of the 'MaxLength' facet.</summary>
        internal const string CsdlMaxLengthAttributeName = "MaxLength";

        /// <summary>The attribute name of the 'FixedLength' facet.</summary>
        internal const string CsdlFixedLengthAttributeName = "FixedLength";

        /// <summary>The attribute name of the 'Unicode' facet.</summary>
        internal const string CsdlUnicodeAttributeName = "Unicode";

        /// <summary>The attribute name of the 'Collation' facet.</summary>
        internal const string CsdlCollationAttributeName = "Collation";

        /// <summary>The attribute name of the 'SRID' facet.</summary>
        internal const string CsdlSridAttributeName = "SRID";

        /// <summary>Name of the default value attribute.</summary>
        internal const string CsdlDefaultValueAttributeName = "DefaultValue";

        /// <summary>The special value for the 'MaxLength' facet to indicate that it has the max length.</summary>
        internal const string CsdlMaxLengthAttributeMaxValue = "Max";

        #endregion

        #endregion // CDM Schema Xml NodeNames

        #region DataWeb Elements and Attributes.

        /// <summary>'MimeType' - attribute name for property MIME type attributes.</summary>
        internal const string DataWebMimeTypeAttributeName = "MimeType";

        /// <summary>'OpenType' - attribute name to indicate a type is an OpenType property.</summary>
        internal const string DataWebOpenTypeAttributeName = "OpenType";

        /// <summary>'HasStream' - attribute name to indicate a type has a default stream property.</summary>
        internal const string DataWebAccessHasStreamAttribute = "HasStream";

        /// <summary>'true' - attribute value to indicate a type has a default stream property.</summary>
        internal const string DataWebAccessDefaultStreamPropertyValue = "true";

        /// <summary>Attribute name in the csdl to indicate whether the service operation must be called using POST or GET verb.</summary>
        internal const string ServiceOperationHttpMethodName = "HttpMethod";

        /// <summary>next element name for link paging</summary>
        internal const string NextElementName = "next";

        #endregion DataWeb Elements and Attributes.

        #region JSON Format constants

        /// <summary>JSON property name for an error.</summary>
        internal const string JsonError = "error";

        /// <summary>JSON property name for an error code.</summary>
        internal const string JsonErrorCode = "code";

        /// <summary>JSON property name for the inner error details.</summary>
        internal const string JsonErrorInner = "innererror";

        /// <summary>JSON property name for an internal exception.</summary>
        internal const string JsonErrorInternalException = "internalexception";

        /// <summary>JSON property name for an error message.</summary>
        internal const string JsonErrorMessage = "message";

        /// <summary>JSON property name for an exception stack trace.</summary>
        internal const string JsonErrorStackTrace = "stacktrace";

        /// <summary>JSON property name for an exception type.</summary>
        internal const string JsonErrorType = "type";

        /// <summary>JSON property name for an error message value.</summary>
        internal const string JsonErrorValue = "value";
        #endregion //JSON Format constants

        #region Edm Primitive Type Names + Collection
        /// <summary>namespace for edm primitive types.</summary>
        internal const string EdmNamespace = "Edm";

        /// <summary>edm binary primitive type name</summary>
        internal const string EdmBinaryTypeName = "Edm.Binary";

        /// <summary>edm boolean primitive type name</summary>
        internal const string EdmBooleanTypeName = "Edm.Boolean";

        /// <summary>edm byte primitive type name</summary>
        internal const string EdmByteTypeName = "Edm.Byte";

        /// <summary>edm decimal primitive type name</summary>
        internal const string EdmDecimalTypeName = "Edm.Decimal";

        /// <summary>edm date primitive type name</summary>
        internal const string EdmDateTypeName = "Edm.Date";

        /// <summary>edm double primitive type name</summary>
        internal const string EdmDoubleTypeName = "Edm.Double";

        /// <summary>edm guid primitive type name</summary>
        internal const string EdmGuidTypeName = "Edm.Guid";

        /// <summary>edm single primitive type name</summary>
        internal const string EdmSingleTypeName = "Edm.Single";

        /// <summary>edm sbyte primitive type name</summary>
        internal const string EdmSByteTypeName = "Edm.SByte";

        /// <summary>edm int16 primitive type name</summary>
        internal const string EdmInt16TypeName = "Edm.Int16";

        /// <summary>edm int32 primitive type name</summary>
        internal const string EdmInt32TypeName = "Edm.Int32";

        /// <summary>edm int64 primitive type name</summary>
        internal const string EdmInt64TypeName = "Edm.Int64";

        /// <summary>edm string primitive type name</summary>
        internal const string EdmStringTypeName = "Edm.String";

        /// <summary>edm stream primitive type name</summary>
        internal const string EdmStreamTypeName = "Edm.Stream";

        /// <summary>edm timeofday primitive type name</summary>
        internal const string EdmTimeOfDayTypeName = "Edm.TimeOfDay";

        /// <summary>edm string indicating that the value may be collection.</summary>
        internal const string CollectionTypeQualifier = "Collection";

        /// <summary>Edm Geography type name</summary>
        internal const string EdmGeographyTypeName = "Edm.Geography";

        /// <summary>Edm Geodetic point type name</summary>
        internal const string EdmPointTypeName = "Edm.GeographyPoint";

        /// <summary>Edm Geodetic linestring type name</summary>
        internal const string EdmLineStringTypeName = "Edm.GeographyLineString";

        /// <summary>Represents a geography Polygon type.</summary>
        internal const string EdmPolygonTypeName = "Edm.GeographyPolygon";

        /// <summary>Represents a geography GeomCollection type.</summary>
        internal const string EdmGeographyCollectionTypeName = "Edm.GeographyCollection";

        /// <summary>Represents a geography MultiPolygon type.</summary>
        internal const string EdmMultiPolygonTypeName = "Edm.GeographyMultiPolygon";

        /// <summary>Represents a geography MultiLineString type.</summary>
        internal const string EdmMultiLineStringTypeName = "Edm.GeographyMultiLineString";

        /// <summary>Represents a geography MultiPoint type.</summary>
        internal const string EdmMultiPointTypeName = "Edm.GeographyMultiPoint";

        /// <summary>Represents an arbitrary Geometry type.</summary>
        internal const string EdmGeometryTypeName = "Edm.Geometry";

        /// <summary>Represents a geometry Point type.</summary>
        internal const string EdmGeometryPointTypeName = "Edm.GeometryPoint";

        /// <summary>Represents a geometry LineString type.</summary>
        internal const string EdmGeometryLineStringTypeName = "Edm.GeometryLineString";

        /// <summary>Represents a geometry Polygon type.</summary>
        internal const string EdmGeometryPolygonTypeName = "Edm.GeometryPolygon";

        /// <summary>Represents a geometry GeomCollection type.</summary>
        internal const string EdmGeometryCollectionTypeName = "Edm.GeometryCollection";

        /// <summary>Represents a geometry MultiPolygon type.</summary>
        internal const string EdmGeometryMultiPolygonTypeName = "Edm.GeometryMultiPolygon";

        /// <summary>Represents a geometry MultiLineString type.</summary>
        internal const string EdmGeometryMultiLineStringTypeName = "Edm.GeometryMultiLineString";

        /// <summary>Represents a geometry MultiPoint type.</summary>
        internal const string EdmGeometryMultiPointTypeName = "Edm.GeometryMultiPoint";

        /// <summary>edm duration primitive type name</summary>
        internal const string EdmDurationTypeName = "Edm.Duration";

        /// <summary>edm string primitive type name</summary>
        internal const string EdmDateTimeOffsetTypeName = "Edm.DateTimeOffset";

        #endregion

        #region Astoria Constants

        /// <summary>'4.0' - the version 4.0 text for a data service.</summary>
        internal const string ODataVersion4Dot0 = "4.0";

        /// <summary>'binary' constant prefixed to binary literals.</summary>
        internal const string LiteralPrefixBinary = "binary";

        /// <summary>'geography' constant prefixed to geography literals.</summary>
        internal const string LiteralPrefixGeography = "geography";

        /// <summary>'geometry' constant prefixed to geometry literals.</summary>
        internal const string LiteralPrefixGeometry = "geometry";

        /// <summary>'duration' constant prefixed to duration literals.</summary>
        internal const string LiteralPrefixDuration = "duration";

        /// <summary>'M': Suffix for decimal type's string representation</summary>
        internal const string LiteralSuffixDecimal = "M";

        /// <summary>'L': Suffix for long (int64) type's string representation</summary>
        internal const string LiteralSuffixInt64 = "L";

        /// <summary>'f': Suffix for float (single) type's string representation</summary>
        internal const string LiteralSuffixSingle = "f";

        /// <summary>'D': Suffix for double (Real) type's string representation</summary>
        internal const string LiteralSuffixDouble = "D";

        /// <summary>null liternal that needs to be return in ETag value when the value is null</summary>
        internal const string NullLiteralInETag = "null";

        /// <summary>Incoming message property name for the original reqeust uri</summary>
        internal const string MicrosoftDataServicesRequestUri = "MicrosoftDataServicesRequestUri";

        /// <summary>Incoming message property name for the original root service uri</summary>
        internal const string MicrosoftDataServicesRootUri = "MicrosoftDataServicesRootUri";

        #endregion // Astoria Constants

        #region Spatial / GeoRss / GeoJson

        /// <summary>
        /// GeoRss namespace
        /// </summary>
        internal const string GeoRssNamespace = "http://www.georss.org/georss";

        /// <summary>
        /// The "georss" prefix
        /// </summary>
        internal const string GeoRssPrefix = "georss";

        /// <summary>
        /// Gml Namespace
        /// </summary>
        internal const string GmlNamespace = "http://www.opengis.net/gml";

        /// <summary>
        /// Gml Prefix
        /// </summary>
        internal const string GmlPrefix = "gml";

        /// <summary>
        /// Embedded Gml tag inside Georss
        /// </summary>
        internal const string GeoRssWhere = "where";

        /// <summary>
        /// GeoRss representation of a point
        /// </summary>
        internal const string GeoRssPoint = "point";

        /// <summary>
        /// GeoRss representation of a line
        /// </summary>
        internal const string GeoRssLine = "line";

        /// <summary>
        /// Gml representation of a point
        /// </summary>
        internal const string GmlPosition = "pos";

        /// <summary>
        /// Gml representation of a point array
        /// </summary>
        internal const string GmlPositionList = "posList";

        /// <summary>
        /// Gml representation of a linestring
        /// </summary>
        internal const string GmlLineString = "LineString";

        #endregion
    }
}
