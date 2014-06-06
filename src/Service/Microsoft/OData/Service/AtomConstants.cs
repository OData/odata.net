//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Constant values related to the ATOM format.
    /// </summary>
    internal static class AtomConstants
    {
        #region Xml constants -----------------------------------------------------------------------------------------

        /// <summary>'http://www.w3.org/2000/xmlns/' - namespace for namespace declarations.</summary>
        internal const string XmlNamespacesNamespace = "http://www.w3.org/2000/xmlns/";

        /// <summary>Attribute use to add xml: namespaces specific attributes.</summary>
        internal const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

        /// <summary> Schema Namespace prefix For xmlns.</summary>
        internal const string XmlnsNamespacePrefix = "xmlns";

        /// <summary> Schema Namespace prefix For xml.</summary>
        internal const string XmlNamespacePrefix = "xml";

        /// <summary>XML attribute value to indicate the base URI for a document or element.</summary>
        internal const string XmlBaseAttributeName = "base";

        /// <summary>Name of the xml:space attribute.</summary>
        internal const string XmlSpaceAttributeName = "space";

        /// <summary>'preserve' value for the xml:space attribute.</summary>
        internal const string XmlPreserveSpaceAttributeValue = "preserve";
        #endregion Xml constants

        #region OData constants ---------------------------------------------------------------------------------------

        /// <summary>XML namespace for data service annotations.</summary>
        internal const string ODataMetadataNamespace = "http://docs.oasis-open.org/odata/ns/metadata";

        /// <summary>XML namespace prefix for data service annotations.</summary>
        internal const string ODataMetadataNamespacePrefix = "m";

        /// <summary>XML namespace for data services.</summary>
        internal const string ODataNamespace = "http://docs.oasis-open.org/odata/ns/data";

        /// <summary>Prefix for data services namespace.</summary>
        internal const string ODataNamespacePrefix = "d";

        /// <summary>OData attribute which indicates the etag value for the declaring entry element.</summary>
        internal const string ODataETagAttributeName = "etag";

        /// <summary>OData attribute which indicates the null value for the element.</summary>
        internal const string ODataNullAttributeName = "null";

        /// <summary>OData element name for the 'count' element</summary>
        internal const string ODataCountElementName = "count";

        /// <summary>OData element name for the 'value' element</summary>
        internal const string ODataValueElementName = "value";

        /// <summary>OData scheme namespace for data services category scheme in atom:category elements.</summary>
        internal const string ODataSchemeNamespace = "http://docs.oasis-open.org/odata/ns/scheme";

        /// <summary>OData stream property prefix for named stream 'mediaresource' related link relations.</summary>
        internal const string ODataStreamPropertyMediaResourceRelatedLinkRelationPrefix = "http://docs.oasis-open.org/odata/ns/mediaresource/";

        /// <summary>OData stream property prefix for named stream 'edit-media' related link relations.</summary>
        internal const string ODataStreamPropertyEditMediaRelatedLinkRelationPrefix = "http://docs.oasis-open.org/odata/ns/edit-media/";

        /// <summary>OData navigation properties prefix for navigation link relations.</summary>
        internal const string ODataNavigationPropertiesRelatedLinkRelationPrefix = "http://docs.oasis-open.org/odata/ns/related/";

        /// <summary>OData association link prefix for relation attribute.</summary>
        internal const string ODataNavigationPropertiesAssociationLinkRelationPrefix = "http://docs.oasis-open.org/odata/ns/relatedlinks/";

        /// <summary>'Inline' - wrapping element for inlined entry/feed content.</summary>
        internal const string ODataInlineElementName = "inline";

        /// <summary>Name of the error element for Xml error responses.</summary>
        internal const string ODataErrorElementName = "error";

        /// <summary>Name of the error code element for Xml error responses.</summary>
        internal const string ODataErrorCodeElementName = "code";

        /// <summary>Name of the error message element for Xml error responses.</summary>
        internal const string ODataErrorMessageElementName = "message";

        /// <summary>Name of the inner error message element for Xml error responses.</summary>
        internal const string ODataInnerErrorElementName = "innererror";

        /// <summary>Name of the message element in inner errors for Xml error responses.</summary>
        internal const string ODataInnerErrorMessageElementName = "message";

        /// <summary>Name of the type element in inner errors for Xml error responses.</summary>
        internal const string ODataInnerErrorTypeElementName = "type";

        /// <summary>Name of the stack trace element in inner errors for Xml error responses.</summary>
        internal const string ODataInnerErrorStackTraceElementName = "stacktrace";

        /// <summary>Name of the inner error element nested in inner errors for Xml error responses.</summary>
        internal const string ODataInnerErrorInnerErrorElementName = "internalexception";

        /// <summary>Element name for the items in a collection.</summary>
        internal const string ODataCollectionItemElementName = "element";

        /// <summary>Element name for m:action.</summary>
        internal const string ODataActionElementName = "action";

        /// <summary>Element name for m:function.</summary>
        internal const string ODataFunctionElementName = "function";

        /// <summary>Attribute name for m:action|m:function/@metadata.</summary>
        internal const string ODataOperationMetadataAttribute = "metadata";

        /// <summary>Attribute name for m:action|m:function/@title.</summary>
        internal const string ODataOperationTitleAttribute = "title";

        /// <summary>Attribute name for m:action|m:function/@target.</summary>
        internal const string ODataOperationTargetAttribute = "target";

        /// <summary>XML element name for the wrapper 'ref' element around a sequence of Uris in response to a ref request.</summary>
        internal const string ODataRefElementName = "ref";

        /// <summary>XML element name for an Id response to a $ref request.</summary>
        internal const string ODataIdElementName = "id";

        /// <summary>XML element name for a next link in a response to a $ref request.</summary>
        internal const string ODataNextLinkElementName = "next";

        /// <summary>XML element name for an annotation in an ATOM payload.</summary>
        internal const string ODataAnnotationElementName = "annotation";

        /// <summary>Attribute name for m:annotation/@target.</summary>
        internal const string ODataAnnotationTargetAttribute = "target";
        
        /// <summary>Attribute name for m:annotation/@term.</summary>
        internal const string ODataAnnotationTermAttribute = "term";

        /// <summary>Attribute name for m:annotation/@string.</summary>
        internal const string ODataAnnotationStringAttribute = "string";

        /// <summary>Attribute name for m:annotation/@bool.</summary>
        internal const string ODataAnnotationBoolAttribute = "bool";

        /// <summary>Attribute name for m:annotation/@decimal.</summary>
        internal const string ODataAnnotationDecimalAttribute = "decimal";

        /// <summary>Attribute name for m:annotation/@int.</summary>
        internal const string ODataAnnotationIntAttribute = "int";

        /// <summary>Attribute name for m:annotation/@float.</summary>
        internal const string ODataAnnotationFloatAttribute = "float";

        /// <summary>Attribute name for m:name.</summary>
        internal const string ODataNameAttribute = "name";

        #endregion OData constants

        #region Atom Format constants ---------------------------------------------------------------------------------

        /// <summary>Schema namespace for Atom.</summary>
        internal const string AtomNamespace = "http://www.w3.org/2005/Atom";

        /// <summary>Prefix for the Atom namespace - empty since it is the default namespace.</summary>
        internal const string AtomNamespacePrefix = "";

        /// <summary>Prefix for the Atom namespace used in cases where we need a non-empty prefix.</summary>
        internal const string NonEmptyAtomNamespacePrefix = "atom";

        /// <summary>XML element name to mark entry element in Atom.</summary>
        internal const string AtomEntryElementName = "entry";

        /// <summary>XML element name to mark feed element in Atom.</summary>
        internal const string AtomFeedElementName = "feed";

        /// <summary>XML element name to mark content element in Atom.</summary>
        internal const string AtomContentElementName = "content";

        /// <summary>XML element name to mark type attribute in Atom.</summary>
        internal const string AtomTypeAttributeName = "type";

        /// <summary>Element containing property values when 'content' is used for media link entries</summary>
        internal const string AtomPropertiesElementName = "properties";

        /// <summary>XML element name to mark id element in Atom.</summary>
        internal const string AtomIdElementName = "id";

        /// <summary>Format for Atom transient id</summary>
        internal const string AtomTransientIdFormat = @"odata:transient:{{{0}}}";

        /// <summary>Regular expression for Atom transient id</summary>
        internal const string AtomTransientIdRegularExpression = @"^odata:transient:{([\s\S]*)}$";

        /// <summary>XML element name to mark title element in Atom.</summary>
        internal const string AtomTitleElementName = "title";

        /// <summary>XML element name to mark the subtitle element in Atom.</summary>
        internal const string AtomSubtitleElementName = "subtitle";

        /// <summary>XML element name to mark the summary element in Atom.</summary>
        internal const string AtomSummaryElementName = "summary";

        /// <summary>XML element name to mark the 'published' element in Atom.</summary>
        internal const string AtomPublishedElementName = "published";

        /// <summary>XML element name to mark the 'source' element in Atom.</summary>
        internal const string AtomSourceElementName = "source";

        /// <summary>XML element name to mark the 'rights' element in Atom.</summary>
        internal const string AtomRightsElementName = "rights";

        /// <summary>XML element name to mark the 'logo' element in Atom.</summary>
        internal const string AtomLogoElementName = "logo";

        /// <summary>XML element name to mark the 'author' element in Atom.</summary>
        internal const string AtomAuthorElementName = "author";

        /// <summary>XML element name to mark the 'author name' element in Atom.</summary>
        internal const string AtomAuthorNameElementName = "name";

        /// <summary>XML element name to mark the 'contributor' element in Atom.</summary>
        internal const string AtomContributorElementName = "contributor";

        /// <summary>XML element name to mark the 'generator' element in Atom.</summary>
        internal const string AtomGeneratorElementName = "generator";

        /// <summary>XML attribute name of the 'uri' attribute of a 'generator' element in Atom.</summary>
        internal const string AtomGeneratorUriAttributeName = "uri";

        /// <summary>XML attribute name of the 'version' attribute of a 'generator' element in Atom.</summary>
        internal const string AtomGeneratorVersionAttributeName = "version";

        /// <summary>XML element name to mark the 'icon' element in Atom.</summary>
        internal const string AtomIconElementName = "icon";

        /// <summary>XML element name to mark the 'name' element in an Atom person construct.</summary>
        internal const string AtomPersonNameElementName = "name";

        /// <summary>XML element name to mark the 'uri' element in an Atom person construct.</summary>
        internal const string AtomPersonUriElementName = "uri";

        /// <summary>XML element name to mark the 'email' element in an Atom person construct.</summary>
        internal const string AtomPersonEmailElementName = "email";

        /// <summary>The name of the 'singleton' element when writing service documents in Xml format.</summary>
        internal const string AtomServiceDocumentSingletonElementName = "singleton";

        /// <summary>The name of the 'function-import' element when writing service documents in Xml format.</summary>
        internal const string AtomServiceDocumentFunctionImportElementName = "function-import";

        /// <summary>'updated' - XML element name for ATOM 'updated' element for entries.</summary>
        internal const string AtomUpdatedElementName = "updated";

        /// <summary>'category' - XML element name for ATOM 'category' element for entries.</summary>
        internal const string AtomCategoryElementName = "category";

        /// <summary>'term' - XML attribute name for ATOM 'term' attribute for categories.</summary>
        internal const string AtomCategoryTermAttributeName = "term";

        /// <summary>'scheme' - XML attribute name for ATOM 'scheme' attribute for categories.</summary>
        internal const string AtomCategorySchemeAttributeName = "scheme";

        /// <summary>'scheme' - XML attribute name for ATOM 'label' attribute for categories.</summary>
        internal const string AtomCategoryLabelAttributeName = "label";

        /// <summary> Atom link relation attribute value for edit links.</summary>
        internal const string AtomEditRelationAttributeValue = "edit";

        /// <summary> Atom link relation attribute value for self links.</summary>
        internal const string AtomSelfRelationAttributeValue = "self";

        /// <summary> Atom context attribute value for specifying context URLs.</summary>
        internal const string AtomContextAttributeValue = "context";

        /// <summary>XML element name to mark link element in Atom.</summary>
        internal const string AtomLinkElementName = "link";

        /// <summary>XML attribute name of the link relation attribute in Atom.</summary>
        internal const string AtomLinkRelationAttributeName = "rel";

        /// <summary>XML attribute name of the type attribute of a link in Atom.</summary>
        internal const string AtomLinkTypeAttributeName = "type";

        /// <summary>XML attribute name of the href attribute of a link in Atom.</summary>
        internal const string AtomLinkHrefAttributeName = "href";

        /// <summary>XML attribute name of the hreflang attribute of a link in Atom.</summary>
        internal const string AtomLinkHrefLangAttributeName = "hreflang";

        /// <summary>XML attribute name of the title attribute of a link in Atom.</summary>
        internal const string AtomLinkTitleAttributeName = "title";

        /// <summary>XML attribute name of the length attribute of a link in Atom.</summary>
        internal const string AtomLinkLengthAttributeName = "length";

        /// <summary>XML element name to mark href attribute element in Atom.</summary>
        internal const string AtomHRefAttributeName = "href";

        /// <summary>Atom source attribute name for the content of media link entries.</summary>
        internal const string MediaLinkEntryContentSourceAttributeName = "src";

        /// <summary>Atom link relation attribute value for edit-media links.</summary>
        internal const string AtomEditMediaRelationAttributeValue = "edit-media";

        /// <summary>XML attribute value of the link relation attribute for next page links in Atom.</summary>
        internal const string AtomNextRelationAttributeValue = "next";

        /// <summary>XML attribute value of the link relation attribute for delta links in Atom.</summary>
        internal const string AtomDeltaRelationAttributeValue = "http://docs.oasis-open.org/odata/ns/delta";

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

        /// <summary>Atom metadata text construct kind: plain text</summary>
        internal const string AtomTextConstructTextKind = "text";

        /// <summary>Atom metadata text construct kind: html</summary>
        internal const string AtomTextConstructHtmlKind = "html";

        /// <summary>Atom metadata text construct kind: xhtml</summary>
        internal const string AtomTextConstructXHtmlKind = "xhtml";

        /// <summary>Default title for service document workspaces.</summary>
        internal const string AtomWorkspaceDefaultTitle = "Default";

        /// <summary>'true' literal</summary>
        internal const string AtomTrueLiteral = "true";

        /// <summary>'false' literal</summary>
        internal const string AtomFalseLiteral = "false";

        /// <summary>IANA link relations namespace.</summary>
        internal const string IanaLinkRelationsNamespace = "http://www.iana.org/assignments/relation/";
        #endregion Atom Format constants

        #region Atom Publishing Protocol constants ------------------------------------------------------------
        /// <summary>The Atom Publishing Protocol (APP) namespace: 'http://www.w3.org/2007/app'.</summary>
        internal const string AtomPublishingNamespace = "http://www.w3.org/2007/app";

        /// <summary>The name of the top-level 'service' element when writing service documents in Xml format.</summary>
        internal const string AtomPublishingServiceElementName = "service";

        /// <summary>The name of the 'workspace' element when writing service documents in Xml format.</summary>
        internal const string AtomPublishingWorkspaceElementName = "workspace";
        
        /// <summary>The name of the 'collection' element when writing service documents in Xml format.</summary>
        internal const string AtomPublishingCollectionElementName = "collection";

        /// <summary>The name of the 'categories' element encountered while reading a service document in XML format.</summary>
        internal const string AtomPublishingCategoriesElementName = "categories";

        /// <summary>The name of the 'accept' element encountered while reading a service document in XML format.</summary>
        internal const string AtomPublishingAcceptElementName = "accept";

        /// <summary>The name of the 'fixed' attribute of an inline categories element in APP.</summary>
        internal const string AtomPublishingFixedAttributeName = "fixed";

        /// <summary>The value 'yes' of the 'fixed' attribute of an inline categories element in APP.</summary>
        internal const string AtomPublishingFixedYesValue = "yes";

        /// <summary>The value 'no' of the 'fixed' attribute of an inline categories element in APP.</summary>
        internal const string AtomPublishingFixedNoValue = "no";
        #endregion

        #region Spatial constants -------------------------------------------------------------------------------------

        /// <summary>XML namespace for GeoRss format</summary>
        internal const string GeoRssNamespace = "http://www.georss.org/georss";

        /// <summary>XML namespace prefix for GeoRss format</summary>
        internal const string GeoRssPrefix = "georss";

        /// <summary>XML namespace for GML format</summary>
        internal const string GmlNamespace = "http://www.opengis.net/gml";

        /// <summary>XML namespace prefix for GML format</summary>
        internal const string GmlPrefix = "gml";

        #endregion
    }
}
