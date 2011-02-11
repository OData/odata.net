//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData.Atom
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

        /// <summary>Name of the xml:lang attribute.</summary>
        internal const string XmlLangAttributeName = "lang";

        /// <summary>Name of the xml:space attribute.</summary>
        internal const string XmlSpaceAttributeName = "space";

        /// <summary>'preserve' value for the xml:space attribute.</summary>
        internal const string XmlPreserveSpaceAttributeValue = "preserve";
        #endregion Xml constants

        #region OData constants ---------------------------------------------------------------------------------------

        /// <summary>XML namespace for data service annotations.</summary>
        internal const string ODataMetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        /// <summary>XML namespace prefix for data service annotations.</summary>
        internal const string ODataMetadataNamespacePrefix = "m";

        /// <summary>XML namespace for data services.</summary>
        internal const string ODataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices";

        /// <summary>Prefix for data services namespace.</summary>
        internal const string ODataNamespacePrefix = "d";

        /// <summary> OData attribute which indicates the etag value for the declaring entry element.</summary>
        internal const string ODataETagAttributeName = "etag";

        /// <summary> OData attribute which indicates the null value for the element.</summary>
        internal const string ODataNullAttributeName = "null";

        /// <summary>OData element name for the 'count' element</summary>
        internal const string ODataCountElementName = "count";

        /// <summary>OData scheme namespace for data services category scheme in atom:category elements.</summary>
        internal const string ODataSchemeNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/scheme";

        /// <summary>OData named streams 'mediaresource' Uri segment name used in named stream link relations.</summary>
        internal const string ODataNamedStreamsMediaResourceSegmentName = "mediaresource";

        /// <summary>OData named streams 'edit-media' Uri segment name used in named stream link relations.</summary>
        internal const string ODataNamedStreamsEditMediaSegmentName = "edit-media";

        /// <summary>OData navigation properties 'related' Uri segment name used in link relations.</summary>
        internal const string ODataNavigationPropertiesRelatedSegmentName = "related";

        /// <summary>OData navigation properties 'relatedLinks' Uri segment name used in association link relations.</summary>
        internal const string ODataNavigationPropertiesAssociationRelatedSegmentName = "relatedLinks";

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

        /// <summary>Name of the internal exception element (nested exceptions inside the innererror element) for Xml error responses.</summary>
        internal const string ODataInternalExceptionElementName = "internalexception";

        /// <summary>Element name for the items in a MultiValue.</summary>
        internal const string ODataMultiValueItemElementName = "element";

        /// <summary>Element name for the items in a collection.</summary>
        internal const string ODataCollectionItemElementName = "element";

        /// <summary>XML element name for an inner error exception type name.</summary>
        internal const string ODataInnerErrorTypeElementName = "type";

        /// <summary>XML element name for an inner error exception stack trace.</summary>
        internal const string ODataInnerErrorStackTraceElementName = "stacktrace";

        /// <summary>XML element name for an inner error message.</summary>
        internal const string ODataInnerErrorMessageElementName = "message";

        /// <summary>XML element name for the wrapper 'links' element around a sequence of Uris in response to a $links request.</summary>
        internal const string ODataLinksElementName = "links";

        /// <summary>XML element name for a Uri response to a $links request.</summary>
        internal const string ODataUriElementName = "uri";

        /// <summary>XML element name for a next link in a response to a $links request.</summary>
        internal const string ODataNextLinkElementName = "next";
        #endregion OData constants

        #region Atom Format constants ---------------------------------------------------------------------------------

        /// <summary>Schema Namespace For Atom.</summary>
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

        /// <summary>XML element name to mark title element in Atom.</summary>
        internal const string AtomTypeAttributeName = "type";

        /// <summary>Element containing property values when 'content' is used for media link entries</summary>
        internal const string AtomPropertiesElementName = "properties";

        /// <summary>XML element name to mark id element in Atom.</summary>
        internal const string AtomIdElementName = "id";

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
        internal const string AtomLinkRelationNextAttributeValue = "next";

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
        
        #endregion
    }
}
