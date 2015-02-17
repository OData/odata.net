//---------------------------------------------------------------------
// <copyright file="PropertyMappingAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// PropertyMappingAnnotation holds all the information required to build feed property mappings
    /// </summary>
    public class PropertyMappingAnnotation : CompositeAnnotation, IComparableAnnotation, IDeepCloneableAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the PropertyMappingAnnotation class
        /// </summary>
        /// <param name="sourcePath">SourcePath of mapping</param>
        /// <param name="targetPath">TargetPath of mapping</param>
        /// <param name="targetNamespacePrefix">targetNamespace Prefix of mapping</param>
        /// <param name="targetNamespaceUri">targetNamespace namespace of mapping</param>
        /// <param name="keepInContent">Whether to keep the mapped property outputing to the feed</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "Keeping test api and Product api the same")]
        public PropertyMappingAnnotation(
            string sourcePath,
            string targetPath,
            string targetNamespacePrefix,
            string targetNamespaceUri,
            bool keepInContent)
            : this(sourcePath, keepInContent)
        {
            this.TargetPath = targetPath;
            this.TargetNamespacePrefix = targetNamespacePrefix;
            this.TargetNamespaceUri = targetNamespaceUri;

            this.SyndicationItemProperty = SyndicationItemProperty.CustomProperty;
            this.SyndicationTextContentKind = null;
        }

        /// <summary>
        /// Initializes a new instance of the PropertyMappingAnnotation class
        /// </summary>
        /// <param name="sourcePath">sourcepath to map</param>
        /// <param name="targetSyndicationItem">Atom specific sydication item to target</param>
        /// <param name="targetTextContentKind">ContentKind of mapping</param>
        /// <param name="keepInContent">Whether to keep the mapped property outputing to the feed</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Not normalizing, need lowercase strings for xml")]
        public PropertyMappingAnnotation(
            string sourcePath,
            SyndicationItemProperty targetSyndicationItem,
            SyndicationTextContentKind targetTextContentKind,
            bool keepInContent)
            : this(sourcePath, keepInContent)
        {
            this.SyndicationItemProperty = targetSyndicationItem;
            this.SyndicationTextContentKind = targetTextContentKind;

            this.TargetPath = targetSyndicationItem.ToString().ToLowerInvariant();
            if (this.TargetPath.StartsWith(ODataConstants.AuthorElementName, StringComparison.Ordinal))
            {
                this.TargetPath = this.TargetPath.Insert(ODataConstants.AuthorElementName.Length, "/");
            }
            else if (this.TargetPath.StartsWith(ODataConstants.ContributorElementName, StringComparison.Ordinal))
            {
                this.TargetPath = this.TargetPath.Insert(ODataConstants.ContributorElementName.Length, "/");
            }

            this.TargetNamespacePrefix = null;
            this.TargetNamespaceUri = ODataConstants.AtomNamespaceName;
        }

        /// <summary>
        /// Initializes a new instance of the PropertyMappingAnnotation class
        /// </summary>
        public PropertyMappingAnnotation()
        {
        }

        private PropertyMappingAnnotation(string sourcePath, bool keepInContent)
        {
            this.SourcePath = sourcePath;
            this.KeepInContent = keepInContent;
        }

        /// <summary>
        /// Gets or sets SourcePath
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets TargetPath
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// Gets or sets TargetPrefix
        /// </summary>
        public string TargetNamespacePrefix { get; set; }

        /// <summary>
        /// Gets or sets TargetNamespaceUri
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Keeping test api and Product api the same")]
        public string TargetNamespaceUri { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether KeepInContent is true or false
        /// </summary>
        public bool KeepInContent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SyndicationProperty is mapped to
        /// </summary>
        public SyndicationItemProperty SyndicationItemProperty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the Content Type will be
        /// </summary>
        public SyndicationTextContentKind? SyndicationTextContentKind { get; set; }

        /// <summary>
        /// Gets a value indicating whether its a ATOM mapping or not
        /// </summary>
        public bool IsAtomMapping
        {
            get
            {
                return this.SyndicationItemProperty != SyndicationItemProperty.CustomProperty;
            }
        }

        /// <summary>
        /// Compares two PropertyMappingAnnotations
        /// </summary>
        /// <param name="annotation">annotation to compare</param>
        /// <returns>true if comparison succeeded otherwise false</returns>
        public bool CompareAnnotation(Annotation annotation)
        {
            ExceptionUtilities.CheckArgumentNotNull(annotation, "annotation");

            var expectedAnnotation = annotation as PropertyMappingAnnotation;
            if (expectedAnnotation == null)
            {
                return false;
            }

            // check targetPath first since this is likely to be different if they are different annotations and then 
            // check other properties by alphabetical order
            if (!expectedAnnotation.TargetPath.Equals(this.TargetPath, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (expectedAnnotation.IsAtomMapping != this.IsAtomMapping)
            {
                return false;
            }

            if (expectedAnnotation.KeepInContent != this.KeepInContent)
            {
                return false;
            }

            if ((expectedAnnotation.SourcePath != null && this.SourcePath != null) &&
                !expectedAnnotation.SourcePath.Equals(this.SourcePath, StringComparison.Ordinal))
            {
                return false;
            }

            if (expectedAnnotation.SyndicationItemProperty != this.SyndicationItemProperty)
            {
                return false;
            }

            if (expectedAnnotation.SyndicationTextContentKind != expectedAnnotation.SyndicationTextContentKind)
            {
                return false;
            }

            if ((expectedAnnotation.TargetNamespacePrefix != null && this.TargetNamespacePrefix != null) &&
                !expectedAnnotation.TargetNamespacePrefix.Equals(this.TargetNamespacePrefix, StringComparison.Ordinal))
            {
                return false;
            }

            if ((expectedAnnotation.TargetNamespaceUri != null && this.TargetNamespaceUri != null) &&
                !expectedAnnotation.TargetNamespaceUri.Equals(this.TargetNamespaceUri, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Makes a deep clone of the current instance and returns it
        /// </summary>
        /// <returns>A deep clone</returns>
        public Annotation DeepClone()
        {
            return new PropertyMappingAnnotation()
            {
                KeepInContent = this.KeepInContent,
                SourcePath = this.SourcePath,
                SyndicationItemProperty = this.SyndicationItemProperty,
                SyndicationTextContentKind = this.SyndicationTextContentKind,
                TargetNamespacePrefix = this.TargetNamespacePrefix,
                TargetNamespaceUri = this.TargetNamespaceUri,
                TargetPath = this.TargetPath
            };
        }
    }
}