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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData writer to write ATOM metadata.
    /// </summary>
    internal static class ODataAtomWriterMetadataUtils
    {
        /// <summary>
        /// Creates a new <see cref="AtomLinkMetadata"/> instance by merging the given
        /// <paramref name="metadata"/> (if any) with the specified <paramref name="href"/>,
        /// <paramref name="relation"/> and (optional) <paramref name="title"/>.
        /// </summary>
        /// <param name="metadata">The metadata to merge with the <paramref name="href"/>, <paramref name="relation"/> and (optional) <paramref name="title"/>.</param>
        /// <param name="relation">The relation to use in the merged metadata.</param>
        /// <param name="href">The href to use in the merged metadata.</param>
        /// <param name="title">The (optional) title to use in the merged metadata.</param>
        /// <param name="mediaType">The (optional) media type to use in the merged metadata.</param>
        /// <returns>A new <see cref="AtomLinkMetadata"/> instance created by merging all the arguments.</returns>
        /// <remarks>
        /// If the <paramref name="metadata"/> already holds values for <paramref name="href"/>,
        /// <paramref name="relation"/>, <paramref name="title"/>, or <paramref name="mediaType"/> this method validates that they
        /// are the same as the ones specified in the method arguments.
        /// </remarks>
        internal static AtomLinkMetadata MergeLinkMetadata(
            AtomLinkMetadata metadata, 
            string relation,
            Uri href,
            string title,
            string mediaType)
        {
            Debug.Assert(relation != null, "relation != null");

            AtomLinkMetadata mergedMetadata = new AtomLinkMetadata(metadata);

            // set the relation
            string metadataRelation = mergedMetadata.Relation;
            if (metadataRelation != null)
            {
                // validate that the relations are the same
                if (string.CompareOrdinal(relation, metadataRelation) != 0)
                {
                    throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkRelationsMustMatch(relation, metadataRelation));
                }
            }
            else
            {
                mergedMetadata.Relation = relation;
            }

            // set the href if it was specified
            if (href != null)
            {
                Uri metadataHref = mergedMetadata.Href;
                if (metadataHref != null)
                {
                    // validate that the hrefs are the same
                    if (!href.Equals(metadataHref))
                    {
                        throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkHrefsMustMatch(href, metadataHref));
                    }
                }
                else
                {
                    mergedMetadata.Href = href;
                }
            }

            // set the title if it was specified
            if (title != null)
            {
                string metadataTitle = mergedMetadata.Title;
                if (metadataTitle != null)
                {
                    // validate that the relations are the same
                    if (string.CompareOrdinal(title, metadataTitle) != 0)
                    {
                        throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkTitlesMustMatch(title, metadataTitle));
                    }
                }
                else
                {
                    mergedMetadata.Title = title;
                }
            }

            // set the content type if it was specified
            if (mediaType != null)
            {
                string metadataMediaType = mergedMetadata.MediaType;
                if (metadataMediaType != null)
                {
                    // validate that the relations are the same
                    if (!HttpUtils.CompareMediaTypeNames(mediaType, metadataMediaType))
                    {
                        throw new ODataException(Strings.ODataAtomWriterMetadataUtils_LinkMediaTypesMustMatch(mediaType, metadataMediaType));
                    }
                }
                else
                {
                    mergedMetadata.MediaType = mediaType;
                }
            }

            return mergedMetadata;
        }

        /// <summary>
        /// Creates a new <see cref="AtomCategoryMetadata"/> instance by merging the given
        /// <paramref name="categoryMetadata"/> (if any) with the specified <paramref name="term"/> and <paramref name="scheme"/>.
        /// </summary>
        /// <param name="categoryMetadata">The metadata to merge with the <paramref name="term"/> and <paramref name="scheme"/>.</param>
        /// <param name="term">The term to use in the merged metadata.</param>
        /// <param name="scheme">The scheme to use in the merged metadata.</param>
        /// <returns>A new <see cref="AtomCategoryMetadata"/> instance created by merging all the arguments.</returns>
        /// <remarks>
        /// If the <paramref name="categoryMetadata"/> already holds values for <paramref name="term"/> or <paramref name="scheme"/>
        /// this method validates that they are the same as the ones specified in the method arguments.
        /// </remarks>
        internal static AtomCategoryMetadata MergeCategoryMetadata(AtomCategoryMetadata categoryMetadata, string term, string scheme)
        {
            Debug.Assert(term != null, "term != null");
            Debug.Assert(scheme != null, "scheme != null");

            AtomCategoryMetadata mergedCategoryMetadata = new AtomCategoryMetadata(categoryMetadata);

            // Set the term.
            string metadataTerm = mergedCategoryMetadata.Term;
            if (metadataTerm != null)
            {
                // Validate that terms are the same.
                if (string.CompareOrdinal(term, metadataTerm) != 0)
                {
                    throw new ODataException(Strings.ODataAtomWriterMetadataUtils_CategoryTermsMustMatch(term, metadataTerm));
                }
            }
            else
            {
                mergedCategoryMetadata.Term = term;
            }

            // Set the scheme.
            string metadataScheme = mergedCategoryMetadata.Scheme;
            if (metadataScheme != null)
            {
                // Validate that schemes are the same.
                if (string.CompareOrdinal(scheme, metadataScheme) != 0)
                {
                    throw new ODataException(Strings.ODataAtomWriterMetadataUtils_CategorySchemesMustMatch(scheme, metadataScheme));
                }
            }
            else
            {
                mergedCategoryMetadata.Scheme = scheme;
            }

            return mergedCategoryMetadata;
        }
    }
}
