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

namespace Microsoft.Data.OData.Metadata
{
    /// <summary>
    /// Constant values used related to EPM (entity property mapping).
    /// </summary>
    internal static class EpmConstants
    {
        #region CSDL attribute names
        /// <summary>Attribute name for the FC_KeepInContent attribute used for EPM.</summary>
        internal const string ODataEpmKeepInContent = "FC_KeepInContent";

        /// <summary>Attribute name for the FC_ContentKind attribute used for EPM.</summary>
        internal const string ODataEpmContentKind = "FC_ContentKind";

        /// <summary>Attribute name for the FC_SourcePath attribute used for EPM.</summary>
        internal const string ODataEpmSourcePath = "FC_SourcePath";

        /// <summary>Attribute name for the FC_TargetPath attribute used for EPM.</summary>
        internal const string ODataEpmTargetPath = "FC_TargetPath";

        /// <summary>Attribute name for the FC_CriteriaValue attribute used for EPM.</summary>
        internal const string ODataEpmCriteriaValue = "FC_CriteriaValue";

        /// <summary>Attribute name for the target namespace prefix (FC_NsPrefix) attribute used for custom EPM.</summary>
        internal const string ODataEpmNsPrefix = "FC_NsPrefix";

        /// <summary>Attribute name for the target namespace Uri (FC_NsUri) attribute used for custom EPM.</summary>
        internal const string ODataEpmNsUri = "FC_NsUri";
        #endregion CSDL attribute names

        #region Syndication Item Properties for metadata target path values
        /// <summary>Target path for author/email</summary>
        internal const string ODataSyndItemAuthorEmail = "SyndicationAuthorEmail";

        /// <summary>Target path for author/name</summary>
        internal const string ODataSyndItemAuthorName = "SyndicationAuthorName";

        /// <summary>Target path for author/uri</summary>
        internal const string ODataSyndItemAuthorUri = "SyndicationAuthorUri";

        /// <summary>Target path for contributor/email</summary>
        internal const string ODataSyndItemContributorEmail = "SyndicationContributorEmail";

        /// <summary>Target path for contributor/name</summary>
        internal const string ODataSyndItemContributorName = "SyndicationContributorName";

        /// <summary>Target path for contributor/uri</summary>
        internal const string ODataSyndItemContributorUri = "SyndicationContributorUri";

        /// <summary>Target path for updated</summary>
        internal const string ODataSyndItemUpdated = "SyndicationUpdated";

        /// <summary>Target path for published</summary>
        internal const string ODataSyndItemPublished = "SyndicationPublished";

        /// <summary>Target path for rights</summary>
        internal const string ODataSyndItemRights = "SyndicationRights";

        /// <summary>Target path for summary</summary>
        internal const string ODataSyndItemSummary = "SyndicationSummary";

        /// <summary>Target path for title</summary>
        internal const string ODataSyndItemTitle = "SyndicationTitle";

        /// <summary>Target path for category/@label</summary>
        internal const string ODataSyndItemCategoryLabel = "SyndicationCategoryLabel";

        /// <summary>Target path for category/@scheme</summary>
        internal const string ODataSyndItemCategoryScheme = "SyndicationCategoryScheme";

        /// <summary>Target path for category/@term</summary>
        internal const string ODataSyndItemCategoryTerm = "SyndicationCategoryTerm";

        /// <summary>Target path for link/@href</summary>
        internal const string ODataSyndItemLinkHref = "SyndicationLinkHref";

        /// <summary>Target path for link/@hreflang</summary>
        internal const string ODataSyndItemLinkHrefLang = "SyndicationLinkHrefLang";

        /// <summary>Target path for link/@length</summary>
        internal const string ODataSyndItemLinkLength = "SyndicationLinkLength";

        /// <summary>Target path for link/@rel</summary>
        internal const string ODataSyndItemLinkRel = "SyndicationLinkRel";

        /// <summary>Target path for link/@title</summary>
        internal const string ODataSyndItemLinkTitle = "SyndicationLinkTitle";

        /// <summary>Target path for link/@type</summary>
        internal const string ODataSyndItemLinkType = "SyndicationLinkType";
        #endregion Syndication Item Properties for metadata target path values

        #region Syndication Item Properties for EntityPropertyMappingAttribute target path values
        /// <summary>Target path for author/email</summary>
        internal const string PropertyMappingTargetPathAuthorEmail = "author/email";

        /// <summary>Target path for author/name</summary>
        internal const string PropertyMappingTargetPathAuthorName = "author/name";

        /// <summary>Target path for author/uri</summary>
        internal const string PropertyMappingTargetPathAuthorUri = "author/uri";

        /// <summary>Target path for contributor/email</summary>
        internal const string PropertyMappingTargetPathContributorEmail = "contributor/email";

        /// <summary>Target path for contributor/name</summary>
        internal const string PropertyMappingTargetPathContributorName = "contributor/name";

        /// <summary>Target path for contributor/uri</summary>
        internal const string PropertyMappingTargetPathContributorUri = "contributor/uri";

        /// <summary>Target path for updated</summary>
        internal const string PropertyMappingTargetPathUpdated = "updated";

        /// <summary>Target path for published</summary>
        internal const string PropertyMappingTargetPathPublished = "published";

        /// <summary>Target path for rights</summary>
        internal const string PropertyMappingTargetPathRights = "rights";

        /// <summary>Target path for summary</summary>
        internal const string PropertyMappingTargetPathSummary = "summary";

        /// <summary>Target path for title</summary>
        internal const string PropertyMappingTargetPathTitle = "title";

        /// <summary>Target path for category/@label</summary>
        internal const string PropertyMappingTargetPathCategoryLabel = "category/@label";

        /// <summary>Target path for category/@scheme</summary>
        internal const string PropertyMappingTargetPathCategoryScheme = "category/@scheme";

        /// <summary>Target path for category/@term</summary>
        internal const string PropertyMappingTargetPathCategoryTerm = "category/@term";

        /// <summary>Target path for link/@href</summary>
        internal const string PropertyMappingTargetPathLinkHref = "link/@href";

        /// <summary>Target path for link/@hreflang</summary>
        internal const string PropertyMappingTargetPathLinkHrefLang = "link/@hreflang";

        /// <summary>Target path for link/@length</summary>
        internal const string PropertyMappingTargetPathLinkLength = "link/@length";

        /// <summary>Target path for link/@rel</summary>
        internal const string PropertyMappingTargetPathLinkRel = "link/@rel";

        /// <summary>Target path for link/@title</summary>
        internal const string PropertyMappingTargetPathLinkTitle = "link/@title";

        /// <summary>Target path for link/@type</summary>
        internal const string PropertyMappingTargetPathLinkType = "link/@type";
        #endregion Syndication Item Properties for EntityPropertyMappingAttribute target path values

        #region Syndication Content Kinds
        /// <summary>String value for the syndication content kind 'text'.</summary>
        internal const string ODataSyndContentKindPlaintext = "text";

        /// <summary>String value for the syndication content kind 'HTML'.</summary>
        internal const string ODataSyndContentKindHtml = "html";

        /// <summary>String value for the syndication content kind 'XHTML'.</summary>
        internal const string ODataSyndContentKindXHtml = "xhtml";
        #endregion Syndication Content Kinds
    }
}
