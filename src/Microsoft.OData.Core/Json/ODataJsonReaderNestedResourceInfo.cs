//---------------------------------------------------------------------
// <copyright file="ODataJsonReaderNestedResourceInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class which holds information about nested resource info to be reported by the reader.
    /// </summary>
    internal sealed class ODataJsonReaderNestedResourceInfo : ODataJsonReaderNestedInfo
    {
        /// <summary>
        /// The nested resource info to report.
        /// </summary>
        private readonly ODataNestedResourceInfo nestedResourceInfo;

        /// <summary>
        /// true if the nested resource info has a value.
        /// </summary>
        private readonly bool hasValue;

        /// <summary>
        /// The nested resource set for nested resource info to be reported.
        /// </summary>
        private ODataResourceSetBase resourceSet;

        /// <summary>
        /// List of entity reference links to be reported to the navigation link.
        /// </summary>
        /// <remarks>
        /// If the navigation link is a singleton this will hold up to 1 item.
        /// If the navigation link is a collection this will hold any number of items.
        /// When the entity reference link is reported it is removed from this list.
        /// </remarks>
        private LinkedList<ODataEntityReferenceLink> entityReferenceLinks;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to report.</param>
        /// <param name="nestedProperty">The nested property for which the nested resource info will be reported.</param>
        /// <param name="isExpanded">true if the nested resource info is expanded.</param>
        private ODataJsonReaderNestedResourceInfo(ODataNestedResourceInfo nestedResourceInfo, IEdmProperty nestedProperty, bool isExpanded)
            : base(nestedProperty)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(nestedProperty == null || nestedProperty.Name == nestedResourceInfo.Name, "The name of the nested resource info doesn't match the name of the property.");

            this.nestedResourceInfo = nestedResourceInfo;

            // ToStructuredType method needs to be called to resolve element type from the collection which AsStructured does not do.
            NestedResourceTypeReference = (nestedProperty != null) ? nestedProperty.Type.Definition.AsElementType().ToTypeReference(nestedProperty.Type.IsNullable) : null;
            this.hasValue = isExpanded;
        }

        /// <summary>
        /// Constructor which used when the nested resource info is a undeclared property in model.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to report.</param>
        /// <param name="nestedProperty">The nested property for which the nested resource info will be reported.</param>
        /// <param name="nestedResourceType">The resource type of the nested resource info.</param>
        /// <param name="isExpanded">true if the nested resource info is expanded.</param>
        private ODataJsonReaderNestedResourceInfo(ODataNestedResourceInfo nestedResourceInfo, IEdmProperty nestedProperty, IEdmType nestedResourceType, bool isExpanded)
            : base(nestedProperty)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(nestedResourceType != null || nestedProperty != null, "nestedResourceType and nestedProperty are both null");

            this.nestedResourceInfo = nestedResourceInfo;

            // We get the nullablity from the nestedProperty if it exists otherwise we consider the resource to be nullable if only the nestedResourceType is specified.
            bool resourceTypeNullable = true;
            if (nestedProperty != null && nestedProperty.Type != null)
            {
                resourceTypeNullable = nestedProperty.Type.IsNullable;
            }

            IEdmType nestedType = nestedResourceType;

            // We get the resource type from the nestedProperty if not specified.
            if (nestedProperty != null && nestedType == null)
            {
                nestedType = nestedProperty.Type.Definition;
            }

            NestedResourceTypeReference = nestedType.ToTypeReference(resourceTypeNullable);
            this.hasValue = isExpanded;
        }

        /// <summary>
        /// The nested resource info to report.
        /// </summary>
        internal ODataNestedResourceInfo NestedResourceInfo
        {
            get
            {
                return this.nestedResourceInfo;
            }
        }

        /// <summary>
        /// The navigation property for which the navigation link will be reported.
        /// </summary>
        internal IEdmNavigationProperty NavigationProperty
        {
            get
            {
                return this.NestedProperty as IEdmNavigationProperty;
            }
        }

        /// <summary>
        /// The structural property for which the nested resource info will be reported.
        /// </summary>
        internal IEdmStructuralProperty StructuralProperty
        {
            get
            {
                return this.NestedProperty as IEdmStructuralProperty;
            }
        }

        /// <summary>
        /// true if the nested resource info has a value.
        /// </summary>
        internal bool HasValue
        {
            get
            {
                return this.hasValue;
            }
        }

        /// <summary>
        /// The nested resource set for nested resource info to be reported.
        /// </summary>
        internal ODataResourceSetBase NestedResourceSet
        {
            get
            {
                return this.resourceSet;
            }
        }

        /// <summary>
        /// The resource type reference of the nested resource info which will be reported.
        /// </summary>
        internal IEdmTypeReference NestedResourceTypeReference { get; private set; }

        /// <summary>
        /// true if the link info has entity reference link which was not yet reported, false otherwise.
        /// </summary>
        internal bool HasEntityReferenceLink
        {
            get
            {
                return this.entityReferenceLinks != null && this.entityReferenceLinks.First != null;
            }
        }

        /// <summary>
        /// Creates a nested resource info for a deferred link.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <returns>The nested resource info created.</returns>
        internal static ODataJsonReaderNestedResourceInfo CreateDeferredLinkInfo(
            ODataNestedResourceInfo nestedResourceInfo,
            IEdmNavigationProperty navigationProperty)
        {
            return new ODataJsonReaderNestedResourceInfo(nestedResourceInfo, navigationProperty, /*isExpanded*/ false);
        }

        /// <summary>
        /// Creates a nested resource info for an expanded resource link.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to report.</param>
        /// <param name="nestedProperty">The navigation property for which the link will be reported.</param>
        /// <param name="nestedResourceType">The type of the nested resource.</param>
        /// <returns>The nested resource info created.</returns>
        internal static ODataJsonReaderNestedResourceInfo CreateResourceReaderNestedResourceInfo(
            ODataNestedResourceInfo nestedResourceInfo,
            IEdmProperty nestedProperty,
            IEdmStructuredType nestedResourceType)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(nestedResourceInfo.IsCollection == false, "Resource can only be reported for a singleton nested resource info.");

            ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = new ODataJsonReaderNestedResourceInfo(nestedResourceInfo, nestedProperty, nestedResourceType, /*isExpanded*/ true);
            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Creates a nested resource info for nested resources.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to report.</param>
        /// <param name="nestedProperty">The nested property for which the nested resource info will be reported.</param>
        /// <param name="nestedResourceType">The type of the nested resource.</param>
        /// <param name="resourceSet">The nested resource set for the nested resource info to report.</param>
        /// <returns>The nested resource info created.</returns>
        internal static ODataJsonReaderNestedResourceInfo CreateResourceSetReaderNestedResourceInfo(
            ODataNestedResourceInfo nestedResourceInfo,
            IEdmProperty nestedProperty,
            IEdmType nestedResourceType,
            ODataResourceSetBase resourceSet)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(nestedResourceInfo.IsCollection == true, "Resource sets can only be reported for collection nested resource info.");
            Debug.Assert(resourceSet != null, "resourceSet != null");

            ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = new ODataJsonReaderNestedResourceInfo(nestedResourceInfo, nestedProperty, nestedResourceType, /*isExpanded*/ true);
            readerNestedResourceInfo.resourceSet = resourceSet;
            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Creates a nested resource info for a singleton entity reference link.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <param name="entityReferenceLink">The entity reference link for the nested resource info to report.</param>
        /// <param name="isExpanded">true if the nested resource info is expanded.</param>
        /// <returns>The nested resource info created.</returns>
        internal static ODataJsonReaderNestedResourceInfo CreateSingletonEntityReferenceLinkInfo(
            ODataNestedResourceInfo nestedResourceInfo,
            IEdmNavigationProperty navigationProperty,
            ODataEntityReferenceLink entityReferenceLink,
            bool isExpanded)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(nestedResourceInfo.IsCollection == false, "Singleton entity reference can only be reported for a singleton navigation links.");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = new ODataJsonReaderNestedResourceInfo(nestedResourceInfo, navigationProperty, isExpanded);
            if (entityReferenceLink != null)
            {
                readerNestedResourceInfo.entityReferenceLinks = new LinkedList<ODataEntityReferenceLink>();
                readerNestedResourceInfo.entityReferenceLinks.AddFirst(entityReferenceLink);
            }

            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Creates a nested resource info for a collection of entity reference links.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to report.</param>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <param name="entityReferenceLinks">The entity reference links for the nested resource info to report.</param>
        /// <param name="isExpanded">true if the nested resource info is expanded.</param>
        /// <returns>The nested resource info created.</returns>
        internal static ODataJsonReaderNestedResourceInfo CreateCollectionEntityReferenceLinksInfo(
            ODataNestedResourceInfo nestedResourceInfo,
            IEdmNavigationProperty navigationProperty,
            LinkedList<ODataEntityReferenceLink> entityReferenceLinks,
            bool isExpanded)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
            Debug.Assert(nestedResourceInfo.IsCollection == true, "Collection entity reference can only be reported for a collection navigation links.");
            Debug.Assert(navigationProperty != null, "navigationProperty != null");
            Debug.Assert(entityReferenceLinks == null || entityReferenceLinks.Count > 0, "entityReferenceLinks == null || entityReferenceLinks.Count > 0");

            ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = new ODataJsonReaderNestedResourceInfo(nestedResourceInfo, navigationProperty, isExpanded);
            readerNestedResourceInfo.entityReferenceLinks = entityReferenceLinks;
            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Creates a nested resource info for a projected nested resource info that is missing from the payload.
        /// </summary>
        /// <param name="navigationProperty">The navigation property for which the link will be reported.</param>
        /// <returns>The nested resource info created.</returns>
        internal static ODataJsonReaderNestedResourceInfo CreateProjectedNestedResourceInfo(IEdmNavigationProperty navigationProperty)
        {
            Debug.Assert(navigationProperty != null, "navigationProperty != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo { Name = navigationProperty.Name, IsCollection = navigationProperty.Type.IsCollection() };
            ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = new ODataJsonReaderNestedResourceInfo(nestedResourceInfo, navigationProperty, /*isExpanded*/ false);
            return readerNestedResourceInfo;
        }

        /// <summary>
        /// Gets the next entity reference link to report and removes it from the internal storage.
        /// </summary>
        /// <returns>The entity reference link to report or null.</returns>
        internal ODataEntityReferenceLink ReportEntityReferenceLink()
        {
            if (this.entityReferenceLinks != null && this.entityReferenceLinks.First != null)
            {
                ODataEntityReferenceLink entityReferenceLink = this.entityReferenceLinks.First.Value;
                this.entityReferenceLinks.RemoveFirst();
                return entityReferenceLink;
            }

            return null;
        }
    }
}
