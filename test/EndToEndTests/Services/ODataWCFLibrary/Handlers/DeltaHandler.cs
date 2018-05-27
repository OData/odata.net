//---------------------------------------------------------------------
// <copyright file="DeltaHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;

    public class DeltaHandler : RequestHandler
    {
        private string DeltaToken { get; set; }

        private DeltaSnapshot DeltaSnapshot { get; set; }

        private List<ODataItem> DeltaItems { get; set; }

        public DeltaHandler(RequestHandler other, DeltaSnapshot deltaSnapshot, string deltaToken)
            : base(other, HttpMethod.GET, deltaSnapshot.QueryUri)
        {
            DeltaToken = deltaToken;
            DeltaSnapshot = deltaSnapshot.Clone();
            DeltaItems = new List<ODataItem>();
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            // TODO: we will need to add server-driven paging for delta link

            object queryResults = this.QueryContext.ResolveQuery(this.DataSource);

            using (var messageWriter = this.CreateMessageWriter(responseMessage))
            {
                IEdmNavigationSource navigationSource = this.QueryContext.Target.NavigationSource;
                IEnumerable iEnumerableResults = queryResults as IEnumerable;

                if (this.QueryContext.Target.NavigationSource != null && this.QueryContext.Target.TypeKind == EdmTypeKind.Collection)
                {
                    IEdmEntitySetBase entitySet = navigationSource as IEdmEntitySetBase;
                    IEdmEntityType entityType = this.QueryContext.Target.ElementType as IEdmEntityType;
                    if (entitySet == null || entityType == null)
                    {
                        throw new InvalidOperationException("Invalid target when query feed.");
                    }
                    GenerateDeltaItemsFromFeed(iEnumerableResults, entitySet, ODataVersion.V4, this.QueryContext.QuerySelectExpandClause);

                    Uri newDeltaLink = null;
                    if (this.PreferenceContext.TrackingChanges)
                    {
                        var newDeltaToken = DeltaContext.GenerateDeltaToken(this.QueryContext.QueryUri, iEnumerableResults, entitySet, this.QueryContext.QuerySelectExpandClause);
                        newDeltaLink = new Uri(string.Format("{0}?{1}={2}", this.ServiceRootUri, ServiceConstants.QueryOption_Delta, newDeltaToken));
                        responseMessage.AddPreferenceApplied(ServiceConstants.Preference_TrackChanging);
                    }
                    ODataDeltaWriter resultWriter = messageWriter.CreateODataDeltaWriter(entitySet, entityType);
                    ResponseWriter.WriteDeltaFeed(resultWriter, this.DeltaItems, this.QueryContext.CountOption, newDeltaLink);

                    resultWriter.Flush();
                }
                else
                {
                    throw Utility.BuildException(HttpStatusCode.Gone);
                }
            }
        }

        /// <summary>
        /// Handle first level entities, genreate the delta items which need to be write
        /// </summary>
        /// <param name="entries">Query results</param>
        /// <param name="entitySet">EntitySet</param>
        /// <param name="targetVersion">Target Version</param>
        /// <param name="selectExpandClause">Select and Expand Clause</param>
        private void GenerateDeltaItemsFromFeed(IEnumerable entries, IEdmNavigationSource entitySet, ODataVersion targetVersion, SelectExpandClause selectExpandClause)
        {
            foreach (var entry in entries)
            {
                // Handle single element in first level and their related navigation property
                GenerateDeltaItemFromEntry(entry, entitySet, targetVersion, selectExpandClause, string.Empty, string.Empty);
            }
            // Handle deleted element in first level
            GenerateDeltaItemsFromDeletedEntities(string.Empty, string.Empty);
        }

        /// <summary>
        /// Handle single entry, which from first level and $expand
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="entitySet">EntitySet</param>
        /// <param name="targetVersion">Target Version</param>
        /// <param name="selectExpandClause">Select and Expand Caluse</param>
        /// <param name="parentId">ParentId, string.Empty for first level</param>
        /// <param name="relationShip">Navigation property name, string.Empty for first level</param>
        private void GenerateDeltaItemFromEntry(object entry, IEdmNavigationSource entitySet, ODataVersion targetVersion, SelectExpandClause selectExpandClause, string parentId, string relationShip)
        {
            var deltaEntry = ODataObjectModelConverter.ConvertToODataEntry(entry, entitySet, targetVersion).Resource;

            // Verify entry if need to be written
            DateTime lastestUpdated = ((ClrObject)entry).UpdatedTime;
            if (lastestUpdated > DeltaSnapshot.TimeStamp)
            {
                var lastSegmentOfDeltaEntry = new ODataUriParser(this.DataSource.Model, ServiceConstants.ServiceBaseUri, deltaEntry.Id).ParsePath().LastSegment as KeySegment;
                deltaEntry.SetSerializationInfo(new ODataResourceSerializationInfo
                {
                    NavigationSourceEntityTypeName = lastSegmentOfDeltaEntry.EdmType.ToString(),
                    NavigationSourceKind = lastSegmentOfDeltaEntry.NavigationSource.NavigationSourceKind(),
                    NavigationSourceName = lastSegmentOfDeltaEntry.NavigationSource.Name,
                });

                this.DeltaItems.Add(deltaEntry);
            }

            // Verify if need added link
            bool ifDeleted = DeltaSnapshot.Entries.Remove(DeltaSnapshot.Entries.FirstOrDefault(p => p.Id == deltaEntry.Id.AbsoluteUri && p.ParentId == parentId && p.RelationShip == relationShip));
            if ((!string.IsNullOrEmpty(parentId)) && ifDeleted == false)
            {
                var deltaLink = new ODataDeltaLink(new Uri(parentId), deltaEntry.Id, relationShip);
                var lastSegmentOfDeltaEntry = new ODataUriParser(this.DataSource.Model, ServiceConstants.ServiceBaseUri, deltaEntry.Id).ParsePath().LastSegment as KeySegment;
                deltaLink.SetSerializationInfo(new ODataDeltaSerializationInfo
                {
                    NavigationSourceName = lastSegmentOfDeltaEntry.NavigationSource.Name,
                });
                this.DeltaItems.Add(deltaLink);
            }

            // Handle $expand
            var expandedNavigationItems = selectExpandClause == null ? null : selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>();
            GenerateDeltaItemsFromExpand(entry, entitySet, targetVersion, expandedNavigationItems, deltaEntry.Id.AbsoluteUri);
        }

        /// <summary>
        /// Handle $expand
        /// </summary>
        /// <param name="entry">Entry</param>
        /// <param name="edmParent">Parent EntitySet</param>
        /// <param name="targetVersion">Target Version</param>
        /// <param name="expandedNavigationItems">Expand Items</param>
        /// <param name="parentId">Parent Id</param>
        private void GenerateDeltaItemsFromExpand(object entry, IEdmNavigationSource edmParent, ODataVersion targetVersion, IEnumerable<ExpandedNavigationSelectItem> expandedNavigationItems, string parentId)
        {
            foreach (var navigationProperty in ((IEdmEntityType)EdmClrTypeUtils.GetEdmType(this.DataSource.Model, entry)).NavigationProperties())
            {
                var expandedNavigationItem = GetExpandedNavigationItem(expandedNavigationItems, navigationProperty.Name);

                if (expandedNavigationItem != null)
                {
                    bool isCollection = navigationProperty.Type.IsCollection();

                    ExpandSelectItemHandler expandItemHandler = new ExpandSelectItemHandler(entry);
                    expandedNavigationItem.HandleWith(expandItemHandler);
                    var propertyValue = expandItemHandler.ExpandedChildElement;

                    if (propertyValue != null)
                    {
                        IEdmNavigationSource targetSource = edmParent.FindNavigationTarget(navigationProperty);

                        if (isCollection)
                        {
                            var expandedEntities = propertyValue as IEnumerable;
                            foreach (var expandedEntity in expandedEntities)
                            {
                                GenerateDeltaItemFromEntry(expandedEntity, targetSource, targetVersion, expandedNavigationItem.SelectAndExpand, parentId, navigationProperty.Name);
                            }
                        }
                        else
                        {
                            GenerateDeltaItemFromEntry(propertyValue, targetSource, targetVersion, expandedNavigationItem.SelectAndExpand, parentId, navigationProperty.Name);
                        }
                    }

                    // Handle deleted entry and link here
                    GenerateDeltaItemsFromDeletedEntities(parentId, navigationProperty.Name);
                }
            }
        }

        /// <summary>
        /// Generate deleted entry and deleted link items
        /// </summary>
        /// <param name="parentId">Parent Id</param>
        /// <param name="relationShip">Navigation property name</param>
        private void GenerateDeltaItemsFromDeletedEntities(string parentId, string relationShip)
        {
            foreach (DeltaSnapshotEntry entry in this.DeltaSnapshot.Entries)
            {
                // Get all top level entities which do not contained in new results
                if (entry.ParentId == parentId && entry.RelationShip == relationShip)
                {
                    // Verify if the entry is deleted now, since some of them may dispare because $filter
                    var verifyresult = new QueryContext(this.ServiceRootUri, new Uri(entry.Id), this.DataSource.Model, this.RequestContainer);
                    object queryResults = verifyresult.ResolveQuery(this.DataSource);

                    if (queryResults == null)
                    {
                        var deletedEntry = new ODataDeltaDeletedEntry(entry.Id, DeltaDeletedEntryReason.Deleted);
                        deletedEntry.SetSerializationInfo(new ODataDeltaSerializationInfo
                        {
                            NavigationSourceName = (verifyresult.QueryPath.LastSegment as KeySegment).NavigationSource.Name,
                        });
                        this.DeltaItems.Add(deletedEntry);
                        if (!string.IsNullOrEmpty(parentId))
                        {
                            var deltaDeletedLink = new ODataDeltaDeletedLink(new Uri(parentId), new Uri(entry.Id), relationShip);
                            deltaDeletedLink.SetSerializationInfo(new ODataDeltaSerializationInfo
                            {
                                NavigationSourceName = (verifyresult.QueryPath.LastSegment as KeySegment).NavigationSource.Name,
                            });
                            this.DeltaItems.Add(deltaDeletedLink);
                        }
                    }
                    else
                    {
                        var deletedEntry = new ODataDeltaDeletedEntry(entry.Id, DeltaDeletedEntryReason.Changed);
                        deletedEntry.SetSerializationInfo(new ODataDeltaSerializationInfo
                        {
                            NavigationSourceName = (verifyresult.QueryPath.LastSegment as KeySegment).NavigationSource.Name,
                        });
                        this.DeltaItems.Add(deletedEntry);
                    }
                }
            }
        }

        private ExpandedNavigationSelectItem GetExpandedNavigationItem(IEnumerable<ExpandedNavigationSelectItem> expandedNavigationItems, string name)
        {
            if (expandedNavigationItems != null)
            {
                foreach (var item in expandedNavigationItems)
                {
                    if ((item.PathToNavigationProperty.LastSegment as NavigationPropertySegment).NavigationProperty.Name == name)
                    {
                        return item;
                    }
                }
            }

            return null;
        }
    }
}