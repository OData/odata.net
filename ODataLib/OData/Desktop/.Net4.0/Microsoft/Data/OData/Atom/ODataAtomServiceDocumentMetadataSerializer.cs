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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
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
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Writes the ATOM metadata for a single workspace element.
        /// </summary>
        /// <param name="workspace">The workspace element to get the metadata for and write it.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Data.OData.Atom.AtomTextConstruct.set_Text(System.String)", Justification = "The value 'Default' is a spec defined constant which is not to be localized.")]
        internal void WriteWorkspaceMetadata(ODataWorkspace workspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(workspace != null, "workspace != null");

            AtomWorkspaceMetadata metadata = workspace.GetAnnotation<AtomWorkspaceMetadata>();
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
        /// Writes the ATOM metadata for a single (resource) collection element.
        /// </summary>
        /// <param name="collection">The collection element to get the metadata for and write it.</param>
        internal void WriteResourceCollectionMetadata(ODataResourceCollectionInfo collection)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(collection != null, "collection != null");
            Debug.Assert(collection.Url != null, "collection.Url should have been validated at this point");

            AtomResourceCollectionMetadata metadata = collection.GetAnnotation<AtomResourceCollectionMetadata>();

            AtomTextConstruct title = null;
            if (metadata != null)
            {
                title = metadata.Title;
            }

            if (collection.Name != null)
            {
                if (title == null)
                {
                    title = new AtomTextConstruct { Text = collection.Name };
                }
                else if (string.CompareOrdinal(title.Text, collection.Name) != 0)
                {
                    throw new ODataException(ODataErrorStrings.ODataAtomServiceDocumentMetadataSerializer_ResourceCollectionNameAndTitleMismatch(collection.Name, title.Text));
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
