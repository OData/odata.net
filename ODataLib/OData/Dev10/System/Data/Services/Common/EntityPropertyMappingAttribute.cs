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
    #region Namespaces
    using System;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Attribute used for mapping a given property or sub-property of a ResourceType to
    /// an xml element/attribute with arbitrary nesting
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class EntityPropertyMappingAttribute : Attribute
    {
        /// <summary>
        /// Source property path.
        /// </summary>
        private readonly string sourcePath;

        /// <summary>
        /// Target Xml element/attribute.
        /// </summary>
        private readonly string targetPath;

        /// <summary>
        /// If mapping to syndication element, the name of syndication item.
        /// </summary>
        private readonly SyndicationItemProperty targetSyndicationItem;

        /// <summary>
        /// If mapping to syndication content, the content type of syndication item.
        /// </summary>
        private readonly SyndicationTextContentKind targetTextContentKind;

        /// <summary>
        /// If mapping to non-syndication element/attribute, the namespace prefix for the 
        /// target element/attribute.
        /// </summary>
        private readonly string targetNamespacePrefix;

        /// <summary>
        /// If mapping to non-syndication element/attribute, the namespace for the 
        /// target element/attribute.
        /// </summary>
        private readonly string targetNamespaceUri;

        /// <summary>
        /// The content can optionally be kept in the original location along with the 
        /// newly mapping location by setting this option to true, false by default.
        /// </summary>
        private readonly bool keepInContent;

        /// <summary>
        /// Used for mapping a resource property to syndication content.
        /// </summary>
        /// <param name="sourcePath">Source property path.</param>
        /// <param name="targetSyndicationItem">Syndication item to which the <see cref="sourcePath"/> is mapped.</param>
        /// <param name="targetTextContentKind">Syndication content kind for <see cref="targetSyndicationItem"/>.</param>
        /// <param name="keepInContent">If true the property value is kept in the content section as before, 
        /// when false the property value is only placed at the mapped location.</param>
        public EntityPropertyMappingAttribute(string sourcePath, SyndicationItemProperty targetSyndicationItem, SyndicationTextContentKind targetTextContentKind, bool keepInContent)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }

            this.sourcePath = sourcePath;
            this.targetPath = targetSyndicationItem.ToTargetPath();
            this.targetSyndicationItem = targetSyndicationItem;
            this.targetTextContentKind = targetTextContentKind;
            this.targetNamespacePrefix = AtomConstants.NonEmptyAtomNamespacePrefix;
            this.targetNamespaceUri = AtomConstants.AtomNamespace;
            this.keepInContent = keepInContent;
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
        public EntityPropertyMappingAttribute(string sourcePath, string targetPath, string targetNamespacePrefix, string targetNamespaceUri, bool keepInContent)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("sourcePath"));
            }

            this.sourcePath = sourcePath;

            if (string.IsNullOrEmpty(targetPath))
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

            if (string.IsNullOrEmpty(targetNamespaceUri))
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
        /// Source property path.
        /// </summary>
        public string SourcePath
        {
            get { return this.sourcePath; }
        }

        /// <summary>
        /// Target Xml element/attribute.
        /// </summary>
        public string TargetPath
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
        public string TargetNamespacePrefix
        {
            get { return this.targetNamespacePrefix; }
        }

        /// <summary>
        /// If mapping to non-syndication element/attribute, the namespace for the 
        /// target element/attribute.
        /// </summary>
        public string TargetNamespaceUri
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
    }
}
