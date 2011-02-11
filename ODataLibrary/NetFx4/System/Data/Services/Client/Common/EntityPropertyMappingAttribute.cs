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

namespace System.Data.Services.Common
{
    #region Namespaces.
    using System;
    using System.Data.OData;
    using System.Data.OData.Atom;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Attribute used for mapping a given property or sub-property of a ResourceType to
    /// an xml element/attribute with arbitrary nesting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
#if INTERNAL_DROP
    internal sealed class EntityPropertyMappingAttribute : Attribute
#else
    public sealed class EntityPropertyMappingAttribute : Attribute
#endif
    {
        #region Private Members
        /// <summary>
        /// Source property path.
        /// </summary>
        private readonly String sourcePath;

        /// <summary>
        /// Target Xml element/attribute.
        /// </summary>
        private readonly String targetPath;

        /// <summary>
        /// If mapping to syndication element, the name of syndication item.
        /// </summary>
        private readonly SyndicationItemProperty targetSyndicationItem;

        /// <summary>
        /// If mapping to syndication content, the content type of syndication item.
        /// </summary>
        private readonly SyndicationTextContentKind targetTextContentKind;

        /// <summary>
        /// If mapping to non-syndication element/attribute, the namespace prefix for the .
        /// target element/attribute
        /// </summary>
        private readonly String targetNamespacePrefix;

        /// <summary>
        /// If mapping to non-syndication element/attribute, the namespace for the 
        /// target element/attribute.
        /// </summary>
        private readonly String targetNamespaceUri;

        /// <summary>
        /// The content can optionally be kept in the original location along with the 
        /// newly mapping location by setting this option to true, false by default.
        /// </summary>
        private readonly bool keepInContent;

        /// <summary>
        /// Criteria value for conditional syndication mapping.
        /// </summary>
        private readonly String criteriaValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Used for mapping a resource property to syndication content.
        /// </summary>
        /// <param name="sourcePath">Source property path.</param>
        /// <param name="targetSyndicationItem">Syndication item to which the <see cref="sourcePath"/> is mapped.</param>
        /// <param name="targetTextContentKind">Syndication content kind for <see cref="targetSyndicationItem"/>.</param>
        /// <param name="keepInContent">If true the property value is kept in the content section as before, 
        /// when false the property value is only placed at the mapped location.</param>
        public EntityPropertyMappingAttribute(String sourcePath, SyndicationItemProperty targetSyndicationItem, SyndicationTextContentKind targetTextContentKind, bool keepInContent)
        {
            if (String.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }

            this.sourcePath            = sourcePath;
            this.targetPath            = SyndicationItemPropertyToPath(targetSyndicationItem);
            this.targetSyndicationItem = targetSyndicationItem;
            this.targetTextContentKind = targetTextContentKind;
            this.targetNamespacePrefix = AtomConstants.NonEmptyAtomNamespacePrefix;
            this.targetNamespaceUri    = AtomConstants.AtomNamespace;
            this.keepInContent         = keepInContent;
        }

        /// <summary>
        /// Used for mapping a resource property to arbitrary xml element/attribute.
        /// </summary>
        /// <param name="sourcePath">Source property path.</param>
        /// <param name="targetPath">Target element/attribute path.</param>
        /// <param name="targetNamespacePrefix">Namespace prefix for the <see cref="targetNamespaceUri"/> to which <see cref="targetPath"/> belongs.</param>
        /// <param name="targetNamespaceUri">Uri of the namespace to which <see cref="targetPath"/> belongs.</param>
        /// <param name="keepInContent">If true the property value is kept in the content section as before, 
        /// when false the property value is only placed at the mapped location.</param>
        public EntityPropertyMappingAttribute(String sourcePath, String targetPath, String targetNamespacePrefix, String targetNamespaceUri, bool keepInContent)
        {
            if (String.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }

            this.sourcePath = sourcePath;

            if (String.IsNullOrEmpty(targetPath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetPath"));
            }

            if (targetPath[0] == '@')
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_InvalidTargetPath(targetPath));
            }

            this.targetPath = targetPath;

            this.targetSyndicationItem = SyndicationItemProperty.CustomProperty;
            this.targetTextContentKind = SyndicationTextContentKind.Plaintext;
            this.targetNamespacePrefix = targetNamespacePrefix;

            if (String.IsNullOrEmpty(targetNamespaceUri))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetNamespaceUri"));
            }

            this.targetNamespaceUri = targetNamespaceUri;

            Uri uri;
            if (!Uri.TryCreate(targetNamespaceUri, UriKind.Absolute, out uri))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_TargetNamespaceUriNotValid(targetNamespaceUri));
            }

            this.keepInContent = keepInContent;
        }

        /// <summary>
        /// Used for conditional mapping to atom:link[@href] and atom:category[@term].
        /// </summary>
        /// <param name="sourcePath">Source property path.</param>
        /// <param name="targetSyndicationItem">Syndication item to which the <see cref="sourcePath"/> is mapped.</param>
        /// <param name="keepInContent">If true the property value is kept in the content section as before, 
        /// when false the property value is only placed at the mapped location.</param>
        /// <param name="criteriaValue">Criteria value that is used as a condition.</param>
        public EntityPropertyMappingAttribute(String sourcePath, SyndicationItemProperty targetSyndicationItem, bool keepInContent, String criteriaValue)
        {
            if (String.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }

            this.sourcePath = sourcePath;

            if (String.IsNullOrEmpty(criteriaValue))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("criteriaValue"));
            }

            this.criteriaValue = criteriaValue;
            this.targetPath = SyndicationItemPropertyToPath(targetSyndicationItem);
            this.targetSyndicationItem = targetSyndicationItem;
            this.targetTextContentKind = SyndicationTextContentKind.Plaintext;
            this.targetNamespacePrefix = AtomConstants.NonEmptyAtomNamespacePrefix;
            this.targetNamespaceUri = AtomConstants.AtomNamespace;
            this.keepInContent = keepInContent;
        }
        #endregion Constructors
        
        #region Properties
        /// <summary>
        /// Source property path.
        /// </summary>
        public String SourcePath
        {
            get { return this.sourcePath; }
        }

        /// <summary>
        /// Target Xml element/attribute.
        /// </summary>
        public String TargetPath
        {
            get { return this.targetPath; }
        }

        /// <summary>
        /// If mapping to syndication element, the name of syndication item.
        /// </summary>
        public SyndicationItemProperty TargetSyndicationItem
        {
            get { return this.targetSyndicationItem; }
        }

        /// <summary>
        /// If mapping to non-syndication element/attribute, the namespace prefix for the 
        /// target element/attribute.
        /// </summary>
        public String TargetNamespacePrefix
        {
            get { return this.targetNamespacePrefix; }
        }

        /// <summary>
        /// If mapping to non-syndication element/attribute, the namespace for the 
        /// target element/attribute.
        /// </summary>
        public String TargetNamespaceUri
        {
            get { return this.targetNamespaceUri; }
        }

        /// <summary>
        /// If mapping to syndication content, the content type of syndication item.
        /// </summary>
        public SyndicationTextContentKind TargetTextContentKind
        {
            get { return this.targetTextContentKind; }
        }

        /// <summary>
        /// The content can optionally be kept in the original location along with the 
        /// newly mapping location by setting this option to true, false by default.
        /// </summary>
        public bool KeepInContent
        {
            get { return this.keepInContent; }
        }

        /// <summary>
        /// Criteria value for conditional syndication mapping.
        /// </summary>
        public String CriteriaValue
        {
            get { return this.criteriaValue; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Maps the enumeration of allowed <see cref="SyndicationItemProperty"/> values to their string representations.
        /// </summary>
        /// <param name="targetSyndicationItem">Value of the <see cref="SyndicationItemProperty"/> given in 
        /// the <see cref="EntityPropertyMappingAttribute"/> contstructor.</param>
        /// <returns>String representing the xml element path in the syndication property.</returns>
        internal static String SyndicationItemPropertyToPath(SyndicationItemProperty targetSyndicationItem)
        {
            DebugUtils.CheckNoExternalCallers();

            switch (targetSyndicationItem)
            {
                case SyndicationItemProperty.AuthorEmail:
                    return "author/email";
                case SyndicationItemProperty.AuthorName:
                    return "author/name";
                case SyndicationItemProperty.AuthorUri:
                    return "author/uri";
                case SyndicationItemProperty.ContributorEmail:
                    return "contributor/email";
                case SyndicationItemProperty.ContributorName:
                    return "contributor/name";
                case SyndicationItemProperty.ContributorUri:
                    return "contributor/uri";
                case SyndicationItemProperty.Updated:
                    return "updated";
                case SyndicationItemProperty.Published:
                    return "published";
                case SyndicationItemProperty.Rights:
                    return "rights";
                case SyndicationItemProperty.Summary:
                    return "summary";
                case SyndicationItemProperty.Title:
                    return "title";
                case SyndicationItemProperty.CategoryLabel:
                    return "category/@label";
                case SyndicationItemProperty.CategoryScheme:
                    return "category/@scheme";
                case SyndicationItemProperty.CategoryTerm:
                    return "category/@term";
                case SyndicationItemProperty.LinkHref:
                    return "link/@href";
                case SyndicationItemProperty.LinkHrefLang:
                    return "link/@hreflang";
                case SyndicationItemProperty.LinkLength:
                    return "link/@length";
                case SyndicationItemProperty.LinkRel:
                    return "link/@rel";
                case SyndicationItemProperty.LinkTitle:
                    return "link/@title";
                case SyndicationItemProperty.LinkType:
                    return "link/@type";
                default:
                    throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetSyndicationItem"));
            }
        }
        #endregion Methods
    }
}
