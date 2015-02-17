//---------------------------------------------------------------------
// <copyright file="TestAtomConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Common
{
    using System.Xml.Linq;
 
    /// <summary>
    /// Constant values related to the ATOM format.
    /// </summary>
    public static class TestAtomConstants
    {
        #region Xml constants -----------------------------------------------------------------------------------------

        /// <summary>'http://www.w3.org/2000/xmlns/' - namespace for namespace declarations.</summary>
        public const string XmlNamespacesNamespace = "http://www.w3.org/2000/xmlns/";

        /// <summary>Attribute use to add xml: namespaces specific attributes.</summary>
        public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

        /// <summary> Schema Namespace prefix For xmlns.</summary>
        public const string XmlnsNamespacePrefix = "xmlns";

        /// <summary> Schema Namespace prefix For xml.</summary>
        public const string XmlNamespacePrefix = "xml";

        /// <summary>XML attribute value to indicate the base URI for a document or element.</summary>
        public const string XmlBaseAttributeName = "base";

        /// <summary>Name of the xml:space attribute.</summary>
        public const string XmlSpaceAttributeName = "space";

        /// <summary>'preserve' value for the xml:space attribute.</summary>
        public const string XmlPreserveSpaceAttributeValue = "preserve";
        #endregion Xml constants

        #region OData constants ---------------------------------------------------------------------------------------

        /// <summary>XML namespace for data service annotations.</summary>
        public const string ODataMetadataNamespace = "http://docs.oasis-open.org/odata/ns/metadata";

        /// <summary>XML namespace prefix for data service annotations.</summary>
        public const string ODataMetadataNamespacePrefix = "m";

        /// <summary>XML namespace for data services.</summary>
        public const string ODataNamespace = "http://docs.oasis-open.org/odata/ns/data";

        /// <summary>Prefix for data services namespace.</summary>
        public const string ODataNamespacePrefix = "d";

        /// <summary> OData attribute which indicates the etag value for the declaring entry element.</summary>
        public const string ODataETagAttributeName = "etag";

        /// <summary> OData attribute which indicates the null value for the element.</summary>
        public const string ODataNullAttributeName = "null";

        /// <summary>OData element name for the 'count' element</summary>
        public const string ODataCountElementName = "count";

        /// <summary>OData scheme namespace for data services category scheme in atom:category elements.</summary>
        public const string ODataSchemeNamespace = "http://docs.oasis-open.org/odata/ns/scheme";

        /// <summary>'Inline' - wrapping element for inlined entry/feed content.</summary>
        public const string ODataInlineElementName = "inline";

        /// <summary>Name of the error element for Xml error responses.</summary>
        public const string ODataErrorElementName = "error";

        /// <summary>Name of the error code element for Xml error responses.</summary>
        public const string ODataErrorCodeElementName = "code";

        /// <summary>Name of the error message element for Xml error responses.</summary>
        public const string ODataErrorMessageElementName = "message";

        /// <summary>Element name for the items in a collection.</summary>
        public const string ODataCollectionItemElementName = "element";

        /// <summary>Name of the inner error message element for Xml error responses.</summary>
        public const string ODataInnerErrorElementName = "innererror";

        /// <summary>Name of the message element in inner errors for Xml error responses.</summary>
        public const string ODataInnerErrorMessageElementName = "message";

        /// <summary>Name of the type element in inner errors for Xml error responses.</summary>
        public const string ODataInnerErrorTypeElementName = "type";

        /// <summary>Name of the stack trace element in inner errors for Xml error responses.</summary>
        public const string ODataInnerErrorStackTraceElementName = "stacktrace";

        /// <summary>Name of the inner error element nested in inner errors for Xml error responses.</summary>
        public const string ODataInnerErrorInnerErrorElementName = "internalexception";
        #endregion OData constants

        #region Atom Format constants ---------------------------------------------------------------------------------

        /// <summary> Schema namespace for Atom.</summary>
        public const string AtomNamespace = "http://www.w3.org/2005/Atom";

        /// <summary> Prefix for the Atom namespace - empty since it is the default namespace.</summary>
        public const string AtomNamespacePrefix = "";

        /// <summary>The namespace for ATOM application.</summary>
        public const string AtomApplicationNamespace = "http://www.w3.org/2007/app";

        /// <summary>XML element name to mark entry element in Atom.</summary>
        public const string AtomEntryElementName = "entry";

        /// <summary>XML element name to mark feed element in Atom.</summary>
        public const string AtomFeedElementName = "feed";

        /// <summary>XML element name to mark content element in Atom.</summary>
        public const string AtomContentElementName = "content";

        /// <summary>XML element name to mark title element in Atom.</summary>
        public const string AtomTypeAttributeName = "type";

        /// <summary>Element containing property values when 'content' is used for media link entries</summary>
        public const string AtomPropertiesElementName = "properties";

        /// <summary>XML element name to mark id element in Atom.</summary>
        public const string AtomIdElementName = "id";

        /// <summary>XML element name to mark title element in Atom.</summary>
        public const string AtomTitleElementName = "title";

        /// <summary>XML element name to mark the subtitle element in Atom.</summary>
        public const string AtomSubtitleElementName = "subtitle";

        /// <summary>XML element name to mark the summary element in Atom.</summary>
        public const string AtomSummaryElementName = "summary";

        /// <summary>XML element name to mark the 'published' element in Atom.</summary>
        public const string AtomPublishedElementName = "published";

        /// <summary>XML element name to mark the 'source' element in Atom.</summary>
        public const string AtomSourceElementName = "source";

        /// <summary>XML element name to mark the 'rights' element in Atom.</summary>
        public const string AtomRightsElementName = "rights";

        /// <summary>XML element name to mark the 'logo' element in Atom.</summary>
        public const string AtomLogoElementName = "logo";

        /// <summary>XML element name to mark the 'author' element in Atom.</summary>
        public const string AtomAuthorElementName = "author";

        /// <summary>XML element name to mark the 'author name' element in Atom.</summary>
        public const string AtomAuthorNameElementName = "name";

        /// <summary>XML element name to mark the 'contributor' element in Atom.</summary>
        public const string AtomContributorElementName = "contributor";

        /// <summary>XML element name to mark the 'generator' element in Atom.</summary>
        public const string AtomGeneratorElementName = "generator";

        /// <summary>XML attribute name of the 'uri' attribute of a 'generator' element in Atom.</summary>
        public const string AtomGeneratorUriAttributeName = "uri";

        /// <summary>XML attribute name of the 'version' attribute of a 'generator' element in Atom.</summary>
        public const string AtomGeneratorVersionAttributeName = "version";

        /// <summary>XML element name to mark the 'icon' element in Atom.</summary>
        public const string AtomIconElementName = "icon";

        /// <summary>XML element name to mark the 'name' element in an Atom person construct.</summary>
        public const string AtomPersonNameElementName = "name";

        /// <summary>XML element name to mark the 'uri' element in an Atom person construct.</summary>
        public const string AtomPersonUriElementName = "uri";

        /// <summary>XML element name to mark the 'email' element in an Atom person construct.</summary>
        public const string AtomPersonEmailElementName = "email";

        /// <summary>'updated' - XML element name for ATOM 'updated' element for entries.</summary>
        public const string AtomUpdatedElementName = "updated";

        /// <summary>'category' - XML element name for ATOM 'category' element for entries.</summary>
        public const string AtomCategoryElementName = "category";

        /// <summary>'term' - XML attribute name for ATOM 'term' attribute for categories.</summary>
        public const string AtomCategoryTermAttributeName = "term";

        /// <summary>'scheme' - XML attribute name for ATOM 'scheme' attribute for categories.</summary>
        public const string AtomCategorySchemeAttributeName = "scheme";

        /// <summary>'scheme' - XML attribute name for ATOM 'label' attribute for categories.</summary>
        public const string AtomCategoryLabelAttributeName = "label";

        /// <summary> Atom link relation attribute value for edit links.</summary>
        public const string AtomEditRelationAttributeValue = "edit";

        /// <summary> Atom link relation attribute value for self links.</summary>
        public const string AtomSelfRelationAttributeValue = "self";

        /// <summary>XML element name to mark link element in Atom.</summary>
        public const string AtomLinkElementName = "link";

        /// <summary>XML attribute name of the link relation attribute in Atom.</summary>
        public const string AtomLinkRelationAttributeName = "rel";

        /// <summary>XML attribute name of the type attribute of a link in Atom.</summary>
        public const string AtomLinkTypeAttributeName = "type";

        /// <summary>XML attribute name of the href attribute of a link in Atom.</summary>
        public const string AtomLinkHrefAttributeName = "href";

        /// <summary>XML attribute name of the hreflang attribute of a link in Atom.</summary>
        public const string AtomLinkHrefLangAttributeName = "hreflang";

        /// <summary>XML attribute name of the title attribute of a link in Atom.</summary>
        public const string AtomLinkTitleAttributeName = "title";

        /// <summary>XML attribute name of the length attribute of a link in Atom.</summary>
        public const string AtomLinkLengthAttributeName = "length";

        /// <summary>XML element name to mark href attribute element in Atom.</summary>
        public const string AtomHRefAttributeName = "href";

        /// <summary>Atom source attribute name for the content of media link entries.</summary>
        public const string MediaLinkEntryContentSourceAttributeName = "src";

        /// <summary> Atom link relation attribute value for edit-media links.</summary>
        public const string AtomEditMediaRelationAttributeValue = "edit-media";

        /// <summary>XML attribute value of the link relation attribute for next page links in Atom.</summary>
        public const string AtomNextRelationAttributeValue = "next";

        /// <summary>XML attribute value of the link relation attribute for delta links in Atom.</summary>
        public const string AtomDeltaRelationAttributeValue = "http://docs.oasis-open.org/odata/ns/delta";

        /// <summary>XML element name for a next link in a response to a $ref request.</summary>
        public const string ODataNextLinkElementName = "next";

        /// <summary>Atom metadata text construct kind: plain text</summary>
        public const string AtomTextConstructTextKind = "text";

        /// <summary>Atom metadata text construct kind: html</summary>
        public const string AtomTextConstructHtmlKind = "html";

        /// <summary>Atom metadata text construct kind: xhtml</summary>
        public const string AtomTextConstructXHtmlKind = "xhtml";

        /// <summary>Default title for service document workspaces.</summary>
        public const string AtomWorkspaceDefaultTitle = "Default";

        /// <summary>'true' literal</summary>
        public const string AtomTrueLiteral = "true";

        /// <summary>'false' literal</summary>
        public const string AtomFalseLiteral = "false";

        /// <summary>IANA link relations namespace.</summary>
        public const string IanaLinkRelationsNamespace = "http://www.iana.org/assignments/relation/";
        #endregion Atom Format constants

        #region Atom Publishing Protocol constants
        /// <summary>The name of the top-level 'service' element when writing service documents in Xml format.</summary>
        public const string AtomPublishingServiceElementName = "service";

        /// <summary>The name of the 'workspace' element when writing service documents in Xml format.</summary>
        public const string AtomPublishingWorkspaceElementName = "workspace";

        /// <summary>The name of the 'collection' element when writing service documents in Xml format.</summary>
        public const string AtomPublishingCollectionElementName = "collection";

        /// <summary>The name of the 'categories' element encountered while reading a service document in XML format.</summary>
        public const string AtomPublishingCategoriesElementName = "categories";

        /// <summary>The Atom Publishing Protocol (APP) namespace: 'http://www.w3.org/2007/app'.</summary>
        public const string AtomPublishingNamespace = "http://www.w3.org/2007/app";

        /// <summary>XML attribute name of the accept attribute of a categories element in APP.</summary>
        public const string AtomPublishingAcceptAttributeName = "accept";

        /// <summary>XML element name for APP 'control' element for entries and feeds.</summary>
        public const string AtomPublishingControlElementName = "control";

        /// <summary>XML element name for APP 'draft' element for app:control.</summary>
        public const string AtomPublishingDraftElementName = "draft";

        /// <summary>XML element name for APP 'edited' element for entries.</summary>
        public const string AtomPublishingEditedElementName = "edited";

        /// <summary>XML attribute name of the fixed attribute of an inline categories element in APP.</summary>
        public const string AtomPublishingFixedAttributeName = "fixed";

        /// <summary>The value 'yes' of the 'fixed' attribute of an inline categories element in APP.</summary>
        public const string AtomPublishingFixedYesValue = "yes";

        /// <summary>The value 'no' of the 'fixed' attribute of an inline categories element in APP.</summary>
        public const string AtomPublishingFixedNoValue = "no";
        #endregion

        #region Atom format constants XLinq names
        /// <summary>Schema namespace for Atom as an XNamespace.</summary>
        public static readonly XNamespace AtomXNamespace = XNamespace.Get(TestAtomConstants.AtomNamespace);

        /// <summary>Schema namespace for Atom Publishing Protocol as an XNamespace.</summary>
        public static readonly XNamespace AtomPublishingXNamespace = XNamespace.Get(TestAtomConstants.AtomPublishingNamespace);

        /// <summary>XML namespace for data service annotations as XNamespace.</summary>
        public static readonly XNamespace ODataMetadataXNamespace = XNamespace.Get(TestAtomConstants.ODataMetadataNamespace);

        /// <summary>XML namespace for data service data annotations as XNamespace.</summary>
        public static readonly XNamespace ODataXNamespace = XNamespace.Get(TestAtomConstants.ODataNamespace);
        #endregion

        #region Spatial constants -------------------------------------------------------------------------------------

        /// <summary>XML namespace for GeoRss format</summary>
        public const string GeoRssNamespace = "http://www.georss.org/georss";

        /// <summary>XML namespace prefix for GeoRss format</summary>
        public const string GeoRssPrefix = "georss";

        /// <summary>XML namespace for GML format</summary>
        public const string GmlNamespace = "http://www.opengis.net/gml";

        /// <summary>XML namespace prefix for GML format</summary>
        public const string GmlPrefix = "gml";

        #endregion
    }
}