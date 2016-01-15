//---------------------------------------------------------------------
// <copyright file="ResponseWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    /// <summary>
    /// Static methods for writing OData responses.
    /// </summary>
    public static class ResponseWriter
    {
        /// <summary>
        /// Writes an OData entry.
        /// </summary>
        /// <param name="writer">The ODataWriter that will write the entry.</param>
        /// <param name="element">The item from the data store to write.</param>
        /// <param name="navigationSource">The navigation source in the model that the entry belongs to.</param>
        /// <param name="model">The data store model.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <param name="selectExpandClause">The SelectExpandClause.</param>
        public static void WriteEntry(ODataWriter writer, object element, IEdmNavigationSource entitySource, ODataVersion targetVersion, SelectExpandClause selectExpandClause, Dictionary<string, string> incomingHeaders = null)
        {
            var entry = ODataObjectModelConverter.ConvertToODataEntry(element, entitySource, targetVersion);

            entry.ETag = Utility.GetETagValue(element);

            if (selectExpandClause != null && selectExpandClause.SelectedItems.OfType<PathSelectItem>().Any())
            {
                ExpandSelectItemHandler selectItemHandler = new ExpandSelectItemHandler(entry);
                foreach (var item in selectExpandClause.SelectedItems.OfType<PathSelectItem>())
                {
                    item.HandleWith(selectItemHandler);
                }

                entry = selectItemHandler.ProjectedEntry;
            }

            CustomizeEntry(incomingHeaders, entry);

            writer.WriteStart(entry);

            // gets all of the expandedItems, including ExpandedRefernceSelectItem and ExpandedNavigationItem
            var expandedItems = selectExpandClause == null ? null : selectExpandClause.SelectedItems.OfType<ExpandedReferenceSelectItem>();
            WriteNavigationLinks(writer, element, entry.ReadLink, entitySource, targetVersion, expandedItems);
            writer.WriteEnd();
        }

        /// <summary>
        /// This method is used to do some customerization according to the incoming headers.
        /// </summary>
        /// <param name="incomingHeaders">The headers in the request.</param>
        /// <param name="entry">The entry that need to customize.</param>
        private static void CustomizeEntry(Dictionary<string, string> incomingHeaders, ODataEntry entry)
        {
            if (null != incomingHeaders)
            {
                var stringOfKey = "Test_ODataEntryFieldToModify";
                if (incomingHeaders.ContainsKey(stringOfKey))
                {
                    var longOfCurrentTime = DateTime.UtcNow.Ticks;
                    var uri = new Uri("http://potato" + longOfCurrentTime);//create a URL that points to a none exist host
                    var stringOfValue = incomingHeaders[stringOfKey];
                    if (stringOfValue.Equals("EditLink", StringComparison.CurrentCultureIgnoreCase))
                    {
                        entry.EditLink = uri;
                    }
                    else if (stringOfValue.Equals("ReadLink", StringComparison.CurrentCultureIgnoreCase))
                    {
                        entry.ReadLink = uri;
                    }
                    else if (stringOfValue.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                    {
                        entry.Id = uri;
                    }
                    else if (stringOfValue.Equals("IsTransient", StringComparison.CurrentCultureIgnoreCase))
                    {
                        entry.IsTransient = true;
                    }
                    else if (stringOfValue.Equals("ReadOnly", StringComparison.CurrentCultureIgnoreCase))
                    {
                        entry.ReadLink = new Uri("People(1)", UriKind.Relative);
                        entry.EditLink = null;
                    }
                }
            }
        }

        /// <summary>
        /// Writes an OData feed.
        /// </summary>
        /// <param name="writer">The ODataWriter that will write the feed.</param>
        /// <param name="entityType">The type of the entity in the feed.</param>
        /// <param name="entries">The items from the data store to write to the feed.</param>
        /// <param name="entitySet">The entity set in the model that the feed belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <param name="selectExpandClause">The SelectExpandClause.</param>
        public static void WriteFeed(ODataWriter writer, IEdmEntityType entityType, IEnumerable entries, IEdmEntitySetBase entitySet, ODataVersion targetVersion, SelectExpandClause selectExpandClause, long? count, Uri deltaLink, Uri nextPageLink, Dictionary<string, string> incomingHeaders = null)
        {
            var feed = new ODataFeed
            {
                Id = entitySet == null ? null : new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name),
                DeltaLink = deltaLink,
                NextPageLink = nextPageLink
            };

            if (entitySet == null)
            {
                feed.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo()
                {
                    NavigationSourceEntityTypeName = entityType.FullTypeName(),
                    NavigationSourceName = null,
                    NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet,
                    IsFromCollection = true
                });
            }

            if (count.HasValue)
            {
                feed.Count = count;
            }

            writer.WriteStart(feed);

            foreach (var element in entries)
            {
                WriteEntry(writer, element, entitySet, targetVersion, selectExpandClause, incomingHeaders);
            }

            writer.WriteEnd();
        }

        private static void WriteNavigationLinks(ODataWriter writer, object element, Uri parentEntryUri, IEdmNavigationSource edmParent, ODataVersion targetVersion, IEnumerable<SelectItem> expandedItems)
        {
            foreach (var navigationProperty in ((IEdmEntityType)EdmClrTypeUtils.GetEdmType(DataSourceManager.GetCurrentDataSource().Model, element)).NavigationProperties())
            {
                // give proprity to ExpandedReferenceSelectItem
                var expandedItem = GetExpandedReferenceItem(expandedItems, navigationProperty.Name);
                if (expandedItem == null)
                {
                    expandedItem = GetExpandedNavigationItem(expandedItems, navigationProperty.Name);
                }

                // For Atom, we always manually write out the links for the navigation properties off of the entity type
                // Or if the navigation is expanded, we manually write out the links for the navigation properties along with the expanded entries
                if (writer.GetType().Name != "ODataJsonLightWriter" || expandedItem != null)
                {
                    bool isCollection = navigationProperty.Type.IsCollection();

                    var navigationLink = new ODataNavigationLink
                    {
                        IsCollection = isCollection,
                        Name = navigationProperty.Name,
                    };

                    if (writer.GetType().Name == "ODataAtomWriter")
                    {
                        //If the passed in parentEntryUri is null then exception will be thrown, to avoid this, create a relative 'potato' Uri.
                        navigationLink.Url = (parentEntryUri == null) ? new Uri("potato", UriKind.Relative) : new Uri(new Uri(parentEntryUri.AbsoluteUri + "/"), navigationProperty.Name);
                    }

                    writer.WriteStart(navigationLink);
                    if (expandedItem != null)
                    {
                        ExpandSelectItemHandler expandItemHandler = new ExpandSelectItemHandler(element);
                        expandedItem.HandleWith(expandItemHandler);

                        var propertyValue = expandItemHandler.ExpandedChildElement;

                        if (propertyValue != null)
                        {
                            IEdmNavigationSource targetSource = edmParent.FindNavigationTarget(navigationProperty);

                            if (isCollection)
                            {
                                long? count = null;
                                var collectionValue = propertyValue as IEnumerable;
                                if (collectionValue != null && expandedItem.CountOption == true)
                                {
                                    count = collectionValue.Cast<object>().LongCount();
                                }

                                if (expandedItem.GetType() == typeof(ExpandedReferenceSelectItem))
                                {
                                    WriteReferenceLinks(writer, collectionValue, targetSource as IEdmEntitySetBase, targetVersion, navigationLink);
                                }
                                else
                                {

                                    WriteFeed(writer, null, collectionValue, targetSource as IEdmEntitySetBase, targetVersion, ((ExpandedNavigationSelectItem)expandedItem).SelectAndExpand, count, null, null);
                                }
                            }
                            else
                            {
                                if (expandedItem.GetType() == typeof(ExpandedReferenceSelectItem))
                                {
                                    WriteReferenceLink(writer, propertyValue, targetSource, targetVersion, navigationLink);
                                }
                                else
                                {
                                    WriteEntry(writer, propertyValue, targetSource, targetVersion, ((ExpandedNavigationSelectItem)expandedItem).SelectAndExpand);
                                }
                            }
                        }
                    }
                    writer.WriteEnd();
                }
            }
        }

        /// <summary>
        /// Writes a ReferenceLink
        /// </summary>
        /// <param name="writer">The ODataWriter that will write the ReferenceLink.</param>
        public static void WriteReferenceLink(ODataWriter writer, object element, IEdmNavigationSource entitySource, ODataVersion targetVersion, ODataNavigationLink navigationLink)
        {
            ODataEntry entry = ODataObjectModelConverter.ConvertToODataEntityReferenceLink(element, entitySource, targetVersion);
            entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("Link.AnnotationByEntry", new ODataPrimitiveValue(true)));
            writer.WriteStart(entry);
            writer.WriteEnd();
        }

        /// <summary>
        /// Writes a ReferenceLinks
        /// </summary>
        /// <param name="writer">The ODataWriter that will write the ReferenceLinks.</param>
        public static void WriteReferenceLinks(ODataWriter writer, IEnumerable element, IEdmNavigationSource entitySource, ODataVersion targetVersion, ODataNavigationLink navigationLink)
        {
            IEnumerable<ODataEntry> links = ODataObjectModelConverter.ConvertToODataEntityReferenceLinks(element, entitySource, targetVersion);
            var feed = new ODataFeed { };
            writer.WriteStart(feed);
            foreach (var entry in links)
            {
                entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("Link.AnnotationByFeed", new ODataPrimitiveValue(true)));
                writer.WriteStart(entry);
                writer.WriteEnd();
            }
            writer.WriteEnd();
        }

        private static ExpandedReferenceSelectItem GetExpandedReferenceItem(IEnumerable<SelectItem> expandedItems, string name)
        {
            if (expandedItems != null)
            {
                foreach (ExpandedReferenceSelectItem item in expandedItems.Where(I => I.GetType() == typeof(ExpandedReferenceSelectItem)))
                {
                    if ((item.PathToNavigationProperty.LastSegment as NavigationPropertySegment).NavigationProperty.Name == name)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        private static ExpandedNavigationSelectItem GetExpandedNavigationItem(IEnumerable<SelectItem> expandedItems, string name)
        {
            if (expandedItems != null)
            {
                foreach (ExpandedNavigationSelectItem item in expandedItems.Where(I => I.GetType() == typeof(ExpandedNavigationSelectItem)))
                {
                    if ((item.PathToNavigationProperty.LastSegment as NavigationPropertySegment).NavigationProperty.Name == name)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Write empty response and set the HttpStatusCode to 204 NoContent
        /// </summary>
        /// <param name="responseMessage"></param>
        public static void WriteEmptyResponse(IODataResponseMessage responseMessage, HttpStatusCode statusCode = HttpStatusCode.NoContent)
        {
            responseMessage.SetStatusCode(statusCode);
            responseMessage.SetHeader("OData-Version", "4.0");
            if (!(responseMessage is ODataBatchOperationResponseMessage))
            {
                using (responseMessage.GetStream())
                {
                }
            }
        }

        /// <summary>
        /// Write empty response and set the HttpStatusCode to 202 Accepted
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="asyncToken"></param>
        public static void WriteAsyncPendingResponse(IODataResponseMessage responseMessage, string asyncToken)
        {
            responseMessage.SetStatusCode(HttpStatusCode.Accepted);
            responseMessage.SetHeader(ServiceConstants.HttpHeaders.DataServiceVersion, "4.0");
            Uri location = new Uri(ServiceConstants.ServiceBaseUri, string.Format("{0}?{1}={2}", ServiceConstants.ServicePath_Async, ServiceConstants.QueryOption_AsyncToken, asyncToken));
            responseMessage.SetHeader(ServiceConstants.HttpHeaders.Location, location.OriginalString);

            if (!(responseMessage is ODataBatchOperationResponseMessage))
            {
                using (responseMessage.GetStream())
                {
                }
            }
        }

        public static void WriteDeltaFeed(ODataDeltaWriter deltaWriter, List<ODataItem> items, bool? countOption, Uri newDeltaLink)
        {
            var deltaFeed = new ODataDeltaFeed
            {
                DeltaLink = newDeltaLink
            };

            if (countOption == true)
            {
                deltaFeed.Count = items.Cast<object>().Count();
            }
            deltaWriter.WriteStart(deltaFeed);

            foreach (ODataItem item in items)
            {
                var entry = item as ODataEntry;
                var deletedEntry = item as ODataDeltaDeletedEntry;
                var deltaLink = item as ODataDeltaLink;
                var deltaDeletedLink = item as ODataDeltaDeletedLink;

                if (entry != null)
                {
                    deltaWriter.WriteStart(entry);
                    deltaWriter.WriteEnd();
                }
                else if (deletedEntry != null)
                {
                    deltaWriter.WriteDeltaDeletedEntry(deletedEntry);
                }
                else if (deltaLink != null)
                {
                    deltaWriter.WriteDeltaLink(deltaLink);
                }
                else if (deltaDeletedLink != null)
                {
                    deltaWriter.WriteDeltaDeletedLink(deltaDeletedLink);
                }
            }
            deltaWriter.WriteEnd();
        }
    }
}
