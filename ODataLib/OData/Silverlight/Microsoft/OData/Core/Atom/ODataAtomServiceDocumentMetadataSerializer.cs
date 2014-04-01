//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM serializer for ATOM metadata in a service document
    /// </summary>
    internal sealed class ODataAtomServiceDocumentMetadataSerializer : ODataAtomMetadataSerializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomOutputContext">The output context to write to.</param>
        internal ODataAtomServiceDocumentMetadataSerializer(ODataAtomOutputContext atomOutputContext)
            : base(atomOutputContext)
        {
        }

        /// <summary>
        /// Writes the ATOM metadata for a single serviceDocument element.
        /// </summary>
        /// <param name="serviceDocument">The serviceDocument element to get the metadata for and write it.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Core.Atom.AtomTextConstruct.set_Text(System.String)", Justification = "The value 'Default' is a spec defined constant which is not to be localized.")]
        internal void WriteServiceDocumentMetadata(ODataServiceDocument serviceDocument)
        {
            Debug.Assert(serviceDocument != null, "serviceDocument != null");

            AtomWorkspaceMetadata metadata = serviceDocument.GetAnnotation<AtomWorkspaceMetadata>();
            AtomTextConstruct title = null;
            if (metadata != null)
            {
                title = metadata.Title;
            }

            if (title == null)
            {
                title = new AtomTextConstruct { Text = AtomConstants.AtomWorkspaceDefaultTitle };
            }

            if (this.UseServerFormatBehavior && title.Kind == AtomTextConstructKind.Text)
            {
                // For WCF DS server we must not write the type attribute, just a simple <atom:title>Default<atom:title>
                this.WriteElementWithTextContent(
                    AtomConstants.NonEmptyAtomNamespacePrefix,
                    AtomConstants.AtomTitleElementName,
                    AtomConstants.AtomNamespace,
                    title.Text);
            }
            else
            {
                // <atom:title>title</atom:title>
                this.WriteTextConstruct(AtomConstants.NonEmptyAtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, title);
            }
        }

        /// <summary>
        /// Writes the ATOM metadata for a single entity set element.
        /// </summary>
        /// <param name="entitySetInfo">The entity set element to get the metadata for and write it.</param>
        internal void WriteEntitySetInfoMetadata(ODataEntitySetInfo entitySetInfo)
        {
            Debug.Assert(entitySetInfo != null, "collection != null");
            Debug.Assert(entitySetInfo.Url != null, "collection.Url should have been validated at this point");

            AtomResourceCollectionMetadata metadata = entitySetInfo.GetAnnotation<AtomResourceCollectionMetadata>();

            AtomTextConstruct title = null;
            if (metadata != null)
            {
                title = metadata.Title;
            }

            if (entitySetInfo.Name != null)
            {
                if (title == null)
                {
                    title = new AtomTextConstruct { Text = entitySetInfo.Name };
                }
                else if (string.CompareOrdinal(title.Text, entitySetInfo.Name) != 0)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomServiceDocumentMetadataSerializer_ResourceCollectionNameAndTitleMismatch(entitySetInfo.Name, title.Text));
                }
            }

            // The ATOMPUB specification requires a title.
            // <atom:title>title</atom:title>
            // Note that this will write an empty atom:title element even if the title is null.
            if (this.UseServerFormatBehavior && title.Kind == AtomTextConstructKind.Text)
            {
                // For WCF DS server we must not write the type attribute, just a simple <atom:title>title<atom:title>
                this.WriteElementWithTextContent(
                    AtomConstants.NonEmptyAtomNamespacePrefix,
                    AtomConstants.AtomTitleElementName,
                    AtomConstants.AtomNamespace,
                    title.Text);
            }
            else
            {
                this.WriteTextConstruct(AtomConstants.NonEmptyAtomNamespacePrefix, AtomConstants.AtomTitleElementName, AtomConstants.AtomNamespace, title);
            }

            if (metadata != null)
            {
                string accept = metadata.Accept;
                if (accept != null)
                {
                    // <app:accept>accept</app:accept>
                    this.WriteElementWithTextContent(
                        string.Empty,
                        AtomConstants.AtomPublishingAcceptElementName,
                        AtomConstants.AtomPublishingNamespace,
                        accept);
                }

                AtomCategoriesMetadata categories = metadata.Categories;
                if (categories != null)
                {
                    // <app:categories>
                    this.XmlWriter.WriteStartElement(string.Empty, AtomConstants.AtomPublishingCategoriesElementName, AtomConstants.AtomPublishingNamespace);

                    Uri href = categories.Href;
                    bool? fixedValue = categories.Fixed;
                    string scheme = categories.Scheme;
                    IEnumerable<AtomCategoryMetadata> categoriesCollection = categories.Categories;
                    if (href != null)
                    {
                        // Out of line categories document
                        if (fixedValue.HasValue || scheme != null ||
                            (categoriesCollection != null && categoriesCollection.Any()))
                        {
                            throw new ODataException(ODataErrorStrings.ODataAtomWriterMetadataUtils_CategoriesHrefWithOtherValues);
                        }

                        this.XmlWriter.WriteAttributeString(AtomConstants.AtomHRefAttributeName, this.UriToUrlAttributeValue(href));
                    }
                    else
                    {
                        // Inline categories document

                        // fixed='yes|no'
                        if (fixedValue.HasValue)
                        {
                            this.XmlWriter.WriteAttributeString(
                                AtomConstants.AtomPublishingFixedAttributeName,
                                fixedValue.Value ? AtomConstants.AtomPublishingFixedYesValue : AtomConstants.AtomPublishingFixedNoValue);
                        }

                        // scheme='scheme'
                        if (scheme != null)
                        {
                            this.XmlWriter.WriteAttributeString(AtomConstants.AtomCategorySchemeAttributeName, scheme);
                        }

                        if (categoriesCollection != null)
                        {
                            foreach (AtomCategoryMetadata category in categoriesCollection)
                            {
                                // <atom:category/>
                                this.WriteCategory(AtomConstants.NonEmptyAtomNamespacePrefix, category.Term, category.Scheme, category.Label);
                            }
                        }
                    }

                    // </app:categories>
                    this.XmlWriter.WriteEndElement();
                }
            }
        }
    }
}
