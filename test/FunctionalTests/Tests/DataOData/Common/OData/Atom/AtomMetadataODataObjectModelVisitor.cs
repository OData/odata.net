//---------------------------------------------------------------------
// <copyright file="AtomMetadataODataObjectModelVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Visitor base class to use for visiting OData object models with ATOM metadata
    /// </summary>
    public class AtomMetadataODataObjectModelVisitor : ODataObjectModelVisitor
    {
        /// <summary>
        /// Visits an ATOM metadata object.
        /// </summary>
        /// <param name="atomMetadata"></param>
        protected virtual void VisitAtomMetadata(object atomMetadata)
        {
            if (atomMetadata == null)
            {
                return;
            }

            AtomCategoryMetadata atomCategoryMetadata = atomMetadata as AtomCategoryMetadata;
            if (atomCategoryMetadata != null)
            {
                this.VisitAtomCategoryMetadata(atomCategoryMetadata);
                return;
            }

            AtomEntryMetadata atomEntryMetadata = atomMetadata as AtomEntryMetadata;
            if (atomEntryMetadata != null)
            {
                this.VisitAtomEntryMetadata(atomEntryMetadata);
                return;
            }

            AtomFeedMetadata atomFeedMetadata = atomMetadata as AtomFeedMetadata;
            if (atomFeedMetadata != null)
            {
                this.VisitAtomFeedMetadata(atomFeedMetadata);
                return;
            }

            AtomGeneratorMetadata atomGeneratorMetadata = atomMetadata as AtomGeneratorMetadata;
            if (atomGeneratorMetadata != null)
            {
                this.VisitAtomGeneratorMetadata(atomGeneratorMetadata);
                return;
            }

            AtomLinkMetadata atomLinkMetadata = atomMetadata as AtomLinkMetadata;
            if (atomLinkMetadata != null)
            {
                this.VisitAtomLinkMetadata(atomLinkMetadata);
                return;
            }

            AtomPersonMetadata atomPersonMetadata = atomMetadata as AtomPersonMetadata;
            if (atomPersonMetadata != null)
            {
                this.VisitAtomPersonMetadata(atomPersonMetadata);
                return;
            }

            AtomResourceCollectionMetadata atomResourceCollectionMetadata = atomMetadata as AtomResourceCollectionMetadata;
            if (atomResourceCollectionMetadata != null)
            {
                this.VisitAtomResourceCollectionMetadata(atomResourceCollectionMetadata);
                return;
            }

            AtomStreamReferenceMetadata atomStreamReferenceMetadata = atomMetadata as AtomStreamReferenceMetadata;
            if (atomStreamReferenceMetadata != null)
            {
                this.VisitAtomStreamReferenceMetadata(atomStreamReferenceMetadata);
                return;
            }

            AtomTextConstruct atomTextConstruct = atomMetadata as AtomTextConstruct;
            if (atomTextConstruct != null)
            {
                this.VisitAtomTextConstruct(atomTextConstruct);
                return;
            }

            AtomWorkspaceMetadata atomWorkspaceMetadata = atomMetadata as AtomWorkspaceMetadata;
            if (atomWorkspaceMetadata != null)
            {
                this.VisitAtomWorkspaceMetadata(atomWorkspaceMetadata);
                return;
            }

            AtomCategoriesMetadata atomCategoriesMetadata = atomMetadata as AtomCategoriesMetadata;
            if (atomCategoriesMetadata != null)
            {
                this.VisitAtomCategoriesMetadata(atomCategoriesMetadata);
                return;
            }

            ExceptionUtilities.Assert(false, "Unrecognized ATOM metadata object {0} of type {1}.", atomMetadata.ToString(), atomMetadata.GetType().ToString());
        }

        /// <summary>
        /// Visits an ATOM category metadata.
        /// </summary>
        /// <param name="atomCategoryMetadata">The category metadata to visit.</param>
        protected virtual void VisitAtomCategoryMetadata(AtomCategoryMetadata atomCategoryMetadata)
        {
        }

        /// <summary>
        /// Visits an ATOM entry metadata.
        /// </summary>
        /// <param name="atomEntryMetadata">The entry metadata to visit.</param>
        protected virtual void VisitAtomEntryMetadata(AtomEntryMetadata atomEntryMetadata)
        {
            IEnumerable<AtomPersonMetadata> authors = atomEntryMetadata.Authors;
            if (authors != null)
            {
                foreach (AtomPersonMetadata author in authors)
                {
                    this.VisitAtomMetadata(author);
                }
            }

            IEnumerable<AtomCategoryMetadata> categories = atomEntryMetadata.Categories;
            if (categories != null)
            {
                foreach (AtomCategoryMetadata category in categories)
                {
                    this.VisitAtomMetadata(category);
                }
            }

            IEnumerable<AtomPersonMetadata> contributors = atomEntryMetadata.Contributors;
            if (contributors != null)
            {
                foreach (AtomPersonMetadata contributor in contributors)
                {
                    this.VisitAtomMetadata(contributor);
                }
            }

            IEnumerable<AtomLinkMetadata> links = atomEntryMetadata.Links;
            if (links != null)
            {
                foreach (AtomLinkMetadata link in links)
                {
                    this.VisitAtomMetadata(link);
                }
            }

            this.VisitAtomMetadata(atomEntryMetadata.Rights);
            this.VisitAtomMetadata(atomEntryMetadata.EditLink);
            this.VisitAtomMetadata(atomEntryMetadata.SelfLink);
            this.VisitAtomMetadata(atomEntryMetadata.CategoryWithTypeName);
            this.VisitAtomMetadata(atomEntryMetadata.Source);
            this.VisitAtomMetadata(atomEntryMetadata.Summary);
            this.VisitAtomMetadata(atomEntryMetadata.Title);
        }

        /// <summary>
        /// Visits an ATOM feed metadata.
        /// </summary>
        /// <param name="atomFeedMetadata">The feed metadata to visit.</param>
        protected virtual void VisitAtomFeedMetadata(AtomFeedMetadata atomFeedMetadata)
        {
            IEnumerable<AtomPersonMetadata> authors = atomFeedMetadata.Authors;
            if (authors != null)
            {
                foreach (AtomPersonMetadata author in authors)
                {
                    this.VisitAtomMetadata(author);
                }
            }

            IEnumerable<AtomCategoryMetadata> categories = atomFeedMetadata.Categories;
            if (categories != null)
            {
                foreach (AtomCategoryMetadata category in categories)
                {
                    this.VisitAtomMetadata(category);
                }
            }

            IEnumerable<AtomPersonMetadata> contributors = atomFeedMetadata.Contributors;
            if (contributors != null)
            {
                foreach (AtomPersonMetadata contributor in contributors)
                {
                    this.VisitAtomMetadata(contributor);
                }
            }

            this.VisitAtomMetadata(atomFeedMetadata.Generator);

            IEnumerable<AtomLinkMetadata> links = atomFeedMetadata.Links;
            if (links != null)
            {
                foreach (AtomLinkMetadata link in links)
                {
                    this.VisitAtomMetadata(link);
                }
            }

            this.VisitAtomMetadata(atomFeedMetadata.Rights);
            this.VisitAtomMetadata(atomFeedMetadata.SelfLink);
            this.VisitAtomMetadata(atomFeedMetadata.Subtitle);
            this.VisitAtomMetadata(atomFeedMetadata.Title);
        }

        /// <summary>
        /// Visits an ATOM generator metadata.
        /// </summary>
        /// <param name="atomGeneratorMetadata">The generator metadata to visit.</param>
        protected virtual void VisitAtomGeneratorMetadata(AtomGeneratorMetadata atomGeneratorMetadata)
        {
        }

        /// <summary>
        /// Visits an ATOM link metadata.
        /// </summary>
        /// <param name="atomLinkMetadata">The link metadata to visit.</param>
        protected virtual void VisitAtomLinkMetadata(AtomLinkMetadata atomLinkMetadata)
        {
        }

        /// <summary>
        /// Visits an ATOM person metadata.
        /// </summary>
        /// <param name="atomPersonMetadata">The person metadata to visit.</param>
        protected virtual void VisitAtomPersonMetadata(AtomPersonMetadata atomPersonMetadata)
        {
        }

        /// <summary>
        /// Visits an ATOM resource collection metadata.
        /// </summary>
        /// <param name="atomResourceCollectionMetadata">The resource collection metadata to visit.</param>
        protected virtual void VisitAtomResourceCollectionMetadata(AtomResourceCollectionMetadata atomResourceCollectionMetadata)
        {
            this.VisitAtomMetadata(atomResourceCollectionMetadata.Title);
            this.VisitAtomMetadata(atomResourceCollectionMetadata.Categories);
        }

        /// <summary>
        /// Visits an ATOM categories metadata.
        /// </summary>
        /// <param name="atomCategoriesMetadata">The categories metadata to visit.</param>
        protected virtual void VisitAtomCategoriesMetadata(AtomCategoriesMetadata atomCategoriesMetadata)
        {
            IEnumerable<AtomCategoryMetadata> categories = atomCategoriesMetadata.Categories;
            if (categories != null)
            {
                foreach (AtomCategoryMetadata category in categories)
                {
                    this.VisitAtomMetadata(category);
                }
            }
        }

        /// <summary>
        /// Visits an ATOM stream reference metadata.
        /// </summary>
        /// <param name="atomStreamReferenceMetadata">The stream reference metadata to visit.</param>
        protected virtual void VisitAtomStreamReferenceMetadata(AtomStreamReferenceMetadata atomStreamReferenceMetadata)
        {
            this.VisitAtomMetadata(atomStreamReferenceMetadata.EditLink);
            this.VisitAtomMetadata(atomStreamReferenceMetadata.SelfLink);
        }

        /// <summary>
        /// Visits an ATOM text construct.
        /// </summary>
        /// <param name="atomTextConstruct">The text construct to visit.</param>
        protected virtual void VisitAtomTextConstruct(AtomTextConstruct atomTextConstruct)
        {
        }

        /// <summary>
        /// Visits an ATOM serviceDocument metadata.
        /// </summary>
        /// <param name="atomWorkspaceMetadata">The serviceDocument metadata to visit.</param>
        protected virtual void VisitAtomWorkspaceMetadata(AtomWorkspaceMetadata atomWorkspaceMetadata)
        {
            this.VisitAtomMetadata(atomWorkspaceMetadata.Title);
        }

        /// <summary>
        /// Visits an entry item.
        /// </summary>
        /// <param name="entry">The entry to visit.</param>
        protected override void VisitEntry(ODataEntry entry)
        {
            this.VisitAtomMetadata(entry.GetAnnotation<AtomEntryMetadata>());
            base.VisitEntry(entry);
        }

        /// <summary>
        /// Visits a feed item.
        /// </summary>
        /// <param name="feed">The feed to visit.</param>
        protected override void VisitFeed(ODataFeed feed)
        {
            this.VisitAtomMetadata(feed.GetAnnotation<AtomFeedMetadata>());
            base.VisitFeed(feed);
        }

        /// <summary>
        /// Visits a navigation link item.
        /// </summary>
        /// <param name="navigationLink">The navigation link to visit.</param>
        protected override void VisitNavigationLink(ODataNavigationLink navigationLink)
        {
            this.VisitAtomMetadata(navigationLink.GetAnnotation<AtomLinkMetadata>());
            base.VisitNavigationLink(navigationLink);
        }

        /// <summary>
        /// Visits a service document.
        /// </summary>
        /// <param name="serviceDocument">The service document to visit.</param>
        protected override void VisitServiceDocument(ODataServiceDocument serviceDocument)
        {
            this.VisitAtomMetadata(serviceDocument.GetAnnotation<AtomWorkspaceMetadata>());
            base.VisitServiceDocument(serviceDocument);
        }

        /// <summary>
        /// Visits a entity set.
        /// </summary>
        /// <param name="entitySetInfo">The entity set to visit.</param>
        protected override void VisitEntitySet(ODataEntitySetInfo entitySetInfo)
        {
            this.VisitAtomMetadata(entitySetInfo.GetAnnotation<AtomResourceCollectionMetadata>());
            base.VisitEntitySet(entitySetInfo);
        }
    }
}
