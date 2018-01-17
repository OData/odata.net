//---------------------------------------------------------------------
// <copyright file="ResponseWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;

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
        /// <param name="entitySet">The entity set in the model that the entry belongs to.</param>
        /// <param name="model">The data store model.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <param name="expandedNavigationProperties">A list of navigation property names to expand.</param>
        public static void WriteEntry(ODataWriter writer, object element, IEdmEntitySet entitySet, IEdmModel model, ODataVersion targetVersion, IEnumerable<string> expandedNavigationProperties) 
        {
            var entry = ODataObjectModelConverter.ConvertToODataEntry(element, entitySet, targetVersion);

            writer.WriteStart(entry);

            // Here, we write out the links for the navigation properties off of the entity type
            WriteNavigationLinks(writer, element, entry.ReadLink, entitySet.EntityType(), model, targetVersion, expandedNavigationProperties);

            writer.WriteEnd();
        }

        /// <summary>
        /// Writes an OData feed.
        /// </summary>
        /// <param name="writer">The ODataWriter that will write the feed.</param>
        /// <param name="entries">The items from the data store to write to the feed.</param>
        /// <param name="entitySet">The entity set in the model that the feed belongs to.</param>
        /// <param name="model">The data store model.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <param name="expandedNavigationProperties">A list of navigation property names to expand.</param>
        public static void WriteFeed(ODataWriter writer, IEnumerable entries, IEdmEntitySet entitySet, IEdmModel model, ODataVersion targetVersion, IEnumerable<string> expandedNavigationProperties)
        {
            var feed = new ODataResourceSet {Id = new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name)};
            writer.WriteStart(feed);

            foreach (var element in entries)
            {
                WriteEntry(writer, element, entitySet, model, targetVersion, expandedNavigationProperties);
            }

            writer.WriteEnd();
        }

        private static void WriteNavigationLinks(ODataWriter writer, object element, Uri parentEntryUri, IEdmEntityType parentEntityType, IEdmModel model, ODataVersion targetVersion, IEnumerable<string> expandedNavigationProperties) 
        {
            foreach (var navigationProperty in parentEntityType.NavigationProperties())
            {
                IEdmTypeReference propertyTypeReference = navigationProperty.Type;
                bool isCollection = navigationProperty.Type.IsCollection();

                var navigationLink = new ODataNestedResourceInfo
                                         {
                                             Url = new Uri(parentEntryUri, navigationProperty.Name),
                                             IsCollection = isCollection,
                                             Name = navigationProperty.Name,
                                         };

                writer.WriteStart(navigationLink);

                if (expandedNavigationProperties.Contains(navigationProperty.Name))
                {
                    var propertyValue = DataContext.GetPropertyValue(element, navigationProperty.Name);

                    if (propertyValue != null)
                    {
                        var propertyEntityType = propertyTypeReference.Definition as IEdmEntityType;
                        IEdmEntitySet targetEntitySet = model.EntityContainer.EntitySets().Single(s => s.EntityType() == propertyEntityType);

                        if (isCollection)
                        {
                            WriteFeed(writer, propertyValue as IEnumerable, targetEntitySet, model, targetVersion, Enumerable.Empty<string>());
                        }
                        else
                        {
                            WriteEntry(writer, propertyValue, targetEntitySet, model, targetVersion, Enumerable.Empty<string>());
                        }
                    }
                }

                writer.WriteEnd();
            }
            
        }
    }
}
