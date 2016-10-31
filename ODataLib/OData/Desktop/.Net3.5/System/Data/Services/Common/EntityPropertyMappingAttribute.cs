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

namespace System.Data.Services.Common
{
    #region Namespaces
    using System;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>Attribute that specifies a custom mapping between properties of an entity type and elements of an entry in an Open Data Protocol (OData) feed returned by the data service. </summary>
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

        /// <summary>Creates a new instance of the <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" />.</summary>
        /// <param name="sourcePath">The name of the property, as string, of the entity type that is mapped to the specified property of the feed item.</param>
        /// <param name="targetSyndicationItem">A <see cref="T:System.Data.Services.Common.SyndicationItemProperty" /> value that represents the element in the feed to which to map the property. This value must be set to None if the <see cref="P:System.Data.Services.Common.EntityPropertyMappingAttribute.TargetPath" /> is not null.</param>
        /// <param name="targetTextContentKind">A <see cref="P:System.Data.Services.Common.EntityPropertyMappingAttribute.TargetTextContentKind" /> value that identifies the format of the content to display in the feed.</param>
        /// <param name="keepInContent">Boolean value that is true when the property being mapped must appear both in its mapped-to location and in the content section of the feed. </param>
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

        /// <summary>Creates an instance of the <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" /> to map a property to a custom feed element.</summary>
        /// <param name="sourcePath">The name of the property of the entity type, as string, that is mapped to the specified property in the feed.</param>
        /// <param name="targetPath">The name of the target, as string, in the resulting feed to which the property is mapped.</param>
        /// <param name="targetNamespacePrefix">This parameter, together with <paramref name="targetNamespaceUri" />, specifies the namespace in which the <paramref name="targetPath " />element exists.</param>
        /// <param name="targetNamespaceUri">Specifies the namespace URI of the element, as string, specified by the <paramref name="targetNamespaceUri" /> property. </param>
        /// <param name="keepInContent">Boolean value that is true when the property being mapped must appear both in its mapped-to location and in the content section of the feed. </param>
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
        
        /// <summary>Gets the name of the property of the syndication item that will be mapped to the specified element of the feed.</summary>
        /// <returns>String value that contains property name.</returns>
        public string SourcePath
        {
            get { return this.sourcePath; }
        }

        /// <summary>Gets the name of the custom target in the feed to which the property is mapped.</summary>
        /// <returns>String value with target XML element or attribute.</returns>
        public string TargetPath
        {
            get { return this.targetPath; }
        }

        /// <summary>Gets the syndication item in the entry targeted by the mapping.</summary>
        /// <returns>A <see cref="T:System.Data.Services.Common.SyndicationItemProperty" /> value that is the target of the mapping.</returns>
        public SyndicationItemProperty TargetSyndicationItem
        {
            get { return this.targetSyndicationItem; }
        }

        /// <summary>Gets a string value that, together with <see cref="P:System.Data.Services.Common.EntityPropertyMappingAttribute.TargetNamespaceUri" />, specifies the namespace in which the <see cref="P:System.Data.Services.Common.EntityPropertyMappingAttribute.TargetPath" />element exists.</summary>
        /// <returns>String value that contains the target namespace prefix.</returns>
        public string TargetNamespacePrefix
        {
            get { return this.targetNamespacePrefix; }
        }

        /// <summary>Gets a string value that specifies the namespace URI of the element specified by the <see cref="P:System.Data.Services.Common.EntityPropertyMappingAttribute.TargetPath" /> property.</summary>
        /// <returns>String that contains the namespace URI.</returns>
        public string TargetNamespaceUri
        {
            get { return this.targetNamespaceUri; }
        }

        /// <summary>Gets the type of content of the property mapped by <see cref="T:System.Data.Services.Common.EntityPropertyMappingAttribute" />.</summary>
        /// <returns>A string that identifies the type of content in the feed element.</returns>
        public SyndicationTextContentKind TargetTextContentKind
        {
            get { return this.targetTextContentKind; }
        }

        /// <summary>Gets a Boolean value that indicates whether a property value should be repeated both in the content section of the feed and in the mapped location.</summary>
        /// <returns>A <see cref="T:System.Boolean" /> value that is true when the property is mapped into both locations in the feed; otherwise, false.</returns>
        public bool KeepInContent
        {
            get { return this.keepInContent; }
        }
    }
}
