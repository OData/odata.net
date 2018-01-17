//---------------------------------------------------------------------
// <copyright file="DeltaContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    static class DeltaContext
    {
        static Dictionary<string, DeltaSnapshot> DeltaTokenDic = new Dictionary<string, DeltaSnapshot>();

        /// <summary>
        /// Generate delta token and save (delta token, delta query) mapping
        /// </summary>
        /// <param name="query">Delta query</param>
        /// <returns>Delta token</returns>
        public static string GenerateDeltaToken(Uri query, IEnumerable entries, IEdmNavigationSource entitySource, SelectExpandClause selectExpandClause)
        {
            // TODO: Consider multiple threads here, may need add lock here.
            //       May need to optimize here, if $top/$skip/$count

            //var builder = new ODataAnnotationUriBuilder(baseUri);
            var deltaSnapshot = new DeltaSnapshot(query);
            SnapResults(deltaSnapshot.Entries, entries, entitySource, selectExpandClause, string.Empty, string.Empty);
            string deltaToken = deltaSnapshot.TimeStamp.Ticks.ToString(CultureInfo.InvariantCulture);
            DeltaTokenDic[deltaToken] = deltaSnapshot;
            return deltaToken;
        }

        /// <summary>
        /// Check if delta token in mapping and return delta query
        /// </summary>
        /// <param name="token">Delta token</param>
        /// <returns>Delta query, or null if delta token is not in mapping</returns>
        public static DeltaSnapshot GetDeltaQueryByDeltaToken(string token)
        {
            if (DeltaTokenDic.Keys.Contains(token))
            {
                return DeltaTokenDic[token];
            }
            else
            {
                throw Utility.BuildException(HttpStatusCode.BadRequest);
            }
        }

        public static void SnapResults(List<DeltaSnapshotEntry> results, IEnumerable entries, IEdmNavigationSource entitySource, SelectExpandClause selectExpandClause, string parentId, string relationShip)
        {
            foreach (object entry in entries)
            {
                SnapResult(results, entry, entitySource, selectExpandClause, parentId, relationShip);
            }
        }

        private static void SnapResult(List<DeltaSnapshotEntry> results, object entry, IEdmNavigationSource entitySource, SelectExpandClause selectExpandClause, string parentId, string relationShip)
        {
            var oDataEntry = ODataObjectModelConverter.ConvertToODataEntry(entry, entitySource, ODataVersion.V4).Resource;
            results.Add(new DeltaSnapshotEntry(oDataEntry.Id.AbsoluteUri, parentId, relationShip));
            var expandedNavigationItems = selectExpandClause == null ? null : selectExpandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>();
            SnapExpandedEntry(results, entry, entitySource, expandedNavigationItems, oDataEntry.Id.AbsoluteUri);
        }

        private static void SnapExpandedEntry(List<DeltaSnapshotEntry> results, object element, IEdmNavigationSource edmParent, IEnumerable<ExpandedNavigationSelectItem> expandedNavigationItems, string parentId)
        {
            foreach (var navigationProperty in ((IEdmEntityType)EdmClrTypeUtils.GetEdmType(DataSourceManager.GetCurrentDataSource().Model, element)).NavigationProperties())
            {
                var expandedNavigationItem = GetExpandedNavigationItem(expandedNavigationItems, navigationProperty.Name);

                if (expandedNavigationItem != null)
                {
                    bool isCollection = navigationProperty.Type.IsCollection();

                    ExpandSelectItemHandler expandItemHandler = new ExpandSelectItemHandler(element);
                    expandedNavigationItem.HandleWith(expandItemHandler);
                    var propertyValue = expandItemHandler.ExpandedChildElement;

                    if (propertyValue != null)
                    {
                        IEdmNavigationSource targetSource = edmParent.FindNavigationTarget(navigationProperty);

                        if (isCollection)
                        {
                            SnapResults(results, propertyValue as IEnumerable, targetSource as IEdmEntitySetBase, expandedNavigationItem.SelectAndExpand, parentId, navigationProperty.Name);
                        }
                        else
                        {
                            SnapResult(results, propertyValue, targetSource, expandedNavigationItem.SelectAndExpand, parentId, navigationProperty.Name);
                        }
                    }

                }
            }
        }

        private static ExpandedNavigationSelectItem GetExpandedNavigationItem(IEnumerable<ExpandedNavigationSelectItem> expandedNavigationItems, string name)
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