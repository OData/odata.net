//---------------------------------------------------------------------
// <copyright file="ODataObjectModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    /// <summary>
    /// Static methods for converting CLR objects from the data store into OData objects.
    /// </summary>
    internal static class ODataObjectModelConverter
    {
        /// <summary>
        /// Converts an item from the data store into an ODataEntry.
        /// </summary>
        /// <param name="element">The item to convert.</param>
        /// <param name="navigationSource">The navigation source that the item belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>The converted ODataEntry.</returns>
        public static ODataEntry ConvertToODataEntry(object element, IEdmNavigationSource entitySource, ODataVersion targetVersion)
        {
            IEdmStructuredType entityType = EdmClrTypeUtils.GetEdmType(DataSourceManager.GetCurrentDataSource().Model, element) as IEdmStructuredType;

            if (entityType == null)
            {
                throw new InvalidOperationException("Can not create an entry for " + entitySource.Name);
            }

            var entry = new ODataEntry
            {
                Properties = GetProperties(element, entityType)
            };

            // Add Annotation in Entity Level
            if (((ClrObject)element).Annotations != null)
            {
                foreach (InstanceAnnotationType annotation in ((ClrObject)element).Annotations)
                {
                    if (string.IsNullOrEmpty(annotation.Target))
                    {
                        entry.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.Name, annotation.ConvertValueToODataValue()));
                    }
                }
            }

            // Work around for entities from different entity set.
            if (!string.IsNullOrEmpty(((ClrObject)element).EntitySetName) && entitySource is IEdmEntitySet && entityType is IEdmEntityType)
            {
                entitySource = new EdmEntitySet(((IEdmEntitySet)entitySource).Container, ((ClrObject)element).EntitySetName, (IEdmEntityType)entityType);
            }

            string typeName;
            if (entityType is IEdmEntityType)
            {
                typeName = (entityType as IEdmEntityType).Name;
            }
            else if (entityType is IEdmComplexType)
            {
                typeName = (entityType as IEdmComplexType).Name;
            }
            else
            {
                throw new InvalidOperationException("Not Supported Edmtype to convert to OData Entry.");
            }
            entry.TypeName = element.GetType().Namespace + "." + typeName;

            // TODO: work around for now
            if (entitySource != null && !(entitySource is IEdmContainedEntitySet))
            {
                Uri entryUri = BuildEntryUri(element, entitySource, targetVersion);
                if (element.GetType().BaseType != null && entitySource.EntityType().Name != typeName)
                {
                    var editLink = new Uri(entryUri.AbsoluteUri.TrimEnd('/') + "/" + entry.TypeName);
                    entry.EditLink = editLink;
                    entry.ReadLink = editLink;
                }
                else
                {
                    entry.EditLink = entryUri;
                    entry.ReadLink = entryUri;
                }
                entry.Id = entryUri;
            }

            if (Utility.IsMediaEntity(element.GetType()))
            {
                var streamProvider = DataSourceManager.GetCurrentDataSource().StreamProvider;
                entry.MediaResource = new ODataStreamReferenceValue()
                {
                    ContentType = streamProvider.GetContentType(element),
                    ETag = streamProvider.GetETag(element),
                };
            }

            return entry;
        }

        /// <summary>
        /// Converts an item from the data store into an ODataEntityReferenceLink.
        /// </summary>
        /// <param name="element">The item to convert.</param>
        /// <param name="navigationSource">The navigation source that the item belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>The converted ODataEntityReferenceLink represent with ODataEntry.</returns>
        public static ODataEntry ConvertToODataEntityReferenceLink(object element, IEdmNavigationSource entitySource, ODataVersion targetVersion)
        {
            IEdmStructuredType entityType = EdmClrTypeUtils.GetEdmType(DataSourceManager.GetCurrentDataSource().Model, element) as IEdmStructuredType;

            if (entityType == null)
            {
                throw new InvalidOperationException("Can not create an entry for " + entitySource.Name);
            }

            var link = new ODataEntry();
            if (!string.IsNullOrEmpty(((ClrObject)element).EntitySetName) && entitySource is IEdmEntitySet && entityType is IEdmEntityType)
            {
                entitySource = new EdmEntitySet(((IEdmEntitySet)entitySource).Container, ((ClrObject)element).EntitySetName, (IEdmEntityType)entityType);
            }

            if (!(entitySource is IEdmContainedEntitySet))
            {
                Uri Url = BuildEntryUri(element, entitySource, targetVersion);
                link.Id = Url;
            }

            // This is workaround now to make Photo/$ref works or it will fail validation as it is MediaEntity but no stream
            if (Utility.IsMediaEntity(element.GetType()))
            {
                var streamProvider = DataSourceManager.GetCurrentDataSource().StreamProvider;
                link.MediaResource = new ODataStreamReferenceValue()
                {
                    ContentType = streamProvider.GetContentType(element),
                    ETag = streamProvider.GetETag(element),
                };
            }
            return link;
        }

        /// <summary>
        /// Converts an item from the data store into an ODataEntityReferenceLinks.
        /// </summary>
        /// <param name="element">The item to convert.</param>
        /// <param name="navigationSource">The navigation source that the item belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>The converted ODataEntityReferenceLinks represent with list of ODataEntry.</returns>
        public static IEnumerable<ODataEntry> ConvertToODataEntityReferenceLinks(IEnumerable element, IEdmNavigationSource entitySource, ODataVersion targetVersion)
        {
            List<ODataEntry> links = new List<ODataEntry>();
            foreach (var each in element)
            {
                ODataEntry link = ConvertToODataEntityReferenceLink(each, entitySource, targetVersion);
                links.Add(link);
            }

            return links;
        }

        public static IEnumerable<ODataProperty> GetProperties(object instance, IEdmStructuredType structuredType)
        {
            var nonOpenProperties = new List<ODataProperty>();
            var structuralProperties = structuredType.StructuralProperties();
            foreach (var sp in structuralProperties)
            {
                nonOpenProperties.Add(ConvertToODataProperty(instance, sp.Name));
            }

            if (structuredType.IsOpen)
            {
                var openProperties = GetOpenProperties(instance);
                return MergeOpenAndNonOpenProperties(nonOpenProperties, openProperties);
            }
            else
            {
                return nonOpenProperties;
            }
        }

        private static IEnumerable<ODataProperty> GetOpenProperties(object instance)
        {
            List<ODataProperty> openProperties = new List<ODataProperty>();
            var inst = instance as OpenClrObject;
            if (inst != null)
            {
                Dictionary<string, object> openPropertiesInClr = inst.OpenProperties;
                openProperties = new List<ODataProperty>();
                if (openPropertiesInClr != null)
                {
                    foreach (var p in openPropertiesInClr)
                    {
                        openProperties.Add(CreateODataProperty(p.Value, p.Key));
                    }
                }
            }
            return openProperties;
        }

        private static IEnumerable<ODataProperty> MergeOpenAndNonOpenProperties(IEnumerable<ODataProperty> nonOpenProperties, IEnumerable<ODataProperty> openProperties)
        {
            return nonOpenProperties.Concat(openProperties);
        }

        /// <summary>
        /// Converts a value from the data store into an ODataProperty.
        /// </summary>
        /// <param name="instance">The item from the data store.</param>
        /// <param name="propertyName">The name of the property to convert.</param>
        /// <returns>The converted ODataProperty.</returns>
        public static ODataProperty ConvertToODataProperty(object instance, string propertyName)
        {
            object value = instance.GetType().GetProperty(propertyName).GetValue(instance, null);
            ODataProperty property = CreateODataProperty(value, propertyName);

            // Add Annotation in property level
            if (((ClrObject)instance).Annotations != null)
            {
                foreach (InstanceAnnotationType annotation in ((ClrObject)instance).Annotations)
                {
                    if (!string.IsNullOrEmpty(annotation.Target) && annotation.Target.Equals(propertyName))
                    {
                        property.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.Name, annotation.ConvertValueToODataValue()));
                    }
                }
            }

            return property;
        }

        /// <summary>
        /// Create ODataProperty given property name and value.
        /// </summary>
        /// <param name="instance">The value of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The converted ODataProperty.</returns>
        public static ODataProperty CreateODataProperty(object value, string propertyName)
        {
            return new ODataProperty { Name = propertyName, Value = CreateODataValue(value) };
        }

        public static object CreateODataValue(object value)
        {
            if (value != null)
            {
                Type t = value.GetType();
                if (t.IsGenericType)
                {
                    // Build a collection type property
                    string itemTypeName = t.GetGenericArguments().First().FullName;
                    if (itemTypeName == "System.UInt16" || itemTypeName == "System.UInt32" || itemTypeName == "System.UInt64")
                    {
                        string modelNS = DataSourceManager.GetCurrentDataSource().Model.DeclaredNamespaces.First();
                        itemTypeName = itemTypeName.Replace("System", modelNS);
                    }
                    else if (itemTypeName == "System.TimeSpan")
                    {
                        itemTypeName = "Edm.Duration";
                    }
                    else
                    {
                        itemTypeName = itemTypeName.Replace("System.", "Edm.");
                    }

                    var genericTypeName = "Collection(" + itemTypeName + ")";
                    ICollection<object> tmp = new Collection<object>();
                    foreach (var o in (value as IEnumerable))
                    {
                        tmp.Add(CreateODataValue(o));
                    }
                    return new ODataCollectionValue() { TypeName = genericTypeName, Items = tmp };
                }
                else if (t.Namespace != "System" && !t.Namespace.StartsWith("Microsoft.Data.Spatial") && !t.Namespace.StartsWith("Microsoft.OData.Edm.Library"))
                {
                    if (t.IsEnum == true)
                    {
                        return new ODataEnumValue(value.ToString(), t.FullName);
                    }
                    else
                    {
                        // Build a complex type property. We consider type t to be primitive if t.Namespace is  "System" or if t is spatial type.
                        List<ODataProperty> properties = new List<ODataProperty>();
                        IEdmStructuredType structuredType = (IEdmStructuredType)EdmClrTypeUtils.GetEdmType(DataSourceManager.GetCurrentDataSource().Model, value);
                        foreach (var p in GetProperties(value, structuredType))
                        {
                            if (t.GetProperty(p.Name) != null)
                            {
                                properties.Add(p);
                            }
                            else if (structuredType.IsOpen)
                            {
                                var instance = value as OpenClrObject;
                                properties.Add(CreateODataProperty(instance.OpenProperties[p.Name], p.Name));
                            }
                        }

                        var complexValue = new ODataComplexValue() { TypeName = t.FullName, Properties = properties, };

                        // Add Annotation in complex level
                        if (((ClrObject)value).Annotations != null)
                        {
                            foreach (InstanceAnnotationType annotation in ((ClrObject)value).Annotations)
                            {
                                if (string.IsNullOrEmpty(annotation.Target))
                                {
                                    complexValue.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.Name, annotation.ConvertValueToODataValue()));
                                }
                            }
                        }
                        return complexValue;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Create the CLR object given an ODataProperty.
        /// </summary>
        /// <param name="propertyValue">The value of the ODataProperty.</param>
        /// <param name="propertyType">The CLR type to be converted to.</param>
        /// <returns>The CLR object.</returns>
        public static object ConvertPropertyValue(object propertyValue, Type propertyType)
        {
            if (propertyValue == null)
            {
                return null;
            }

            bool isNullable = propertyType.IsNullable();
            Type propertyUnderlyingType;
            if (isNullable)
            {
                propertyUnderlyingType = Nullable.GetUnderlyingType(propertyType);
            }
            else
            {
                propertyUnderlyingType = propertyType;
            }

            if (propertyUnderlyingType == typeof(DateTime) && !(propertyValue is DateTime))
            {
                return XmlConvert.ToDateTimeOffset(propertyValue.ToString());
            }
            else if (propertyUnderlyingType == typeof(float))
            {
                return Convert.ToSingle(propertyValue);
            }
            else if (propertyUnderlyingType.IsEnum)
            {
                ODataEnumValue enumValue = (ODataEnumValue)propertyValue;
                return Enum.Parse(propertyUnderlyingType, enumValue.Value);
            }

            return propertyValue;
        }

        public static object ConvertPropertyValue(object propertyValue)
        {
            if (propertyValue == null)
            {
                return null;
            }

            if (propertyValue is ODataEnumValue)
            {
                ODataEnumValue enumValue = (ODataEnumValue)propertyValue;
                return Enum.Parse(EdmClrTypeUtils.GetInstanceType(enumValue.TypeName), enumValue.Value);
            }

            if (propertyValue is ODataCollectionValue)
            {
                ODataCollectionValue collection = (ODataCollectionValue)propertyValue;
                IList collectionList = null;
                foreach (object odataItem in collection.Items)
                {
                    var item = ConvertPropertyValue(odataItem);
                    if (collectionList == null)
                    {
                        Type itemType = item.GetType();
                        Type listType = typeof(List<>).MakeGenericType(new[] { itemType });
                        collectionList = (IList)Utility.QuickCreateInstance(listType);
                    }

                    collectionList.Add(item);
                }
                return collectionList;
            }

            if (propertyValue is ODataComplexValue)
            {
                ODataComplexValue complexValue = (ODataComplexValue)propertyValue;
                var type = EdmClrTypeUtils.GetInstanceType(complexValue.TypeName);
                var newInstance = Utility.QuickCreateInstance(type);
                foreach (var p in complexValue.Properties)
                {
                    PropertyInfo targetProperty = type.GetProperty(p.Name);
                    targetProperty.SetValue(newInstance, ConvertPropertyValue(p.Value, targetProperty.PropertyType), new object[] { });
                }

                return newInstance;
            }

            if (propertyValue is ODataEntry)
            {
                ODataEntry entry = (ODataEntry)propertyValue;
                var type = EdmClrTypeUtils.GetInstanceType(entry.TypeName);
                var newInstance = Utility.QuickCreateInstance(type);
                foreach (var p in entry.Properties)
                {
                    PropertyInfo targetProperty = type.GetProperty(p.Name);
                    // If the value is a odata collection value that contains no element, set the value to a empty collection.
                    // ConvertPropertyValue won't work here because it could not know what the type it its.
                    var collectionValue = p.Value as ODataCollectionValue;
                    if (collectionValue != null && !collectionValue.Items.Cast<object>().Any())
                    {
                        targetProperty.SetValue(newInstance, Utility.QuickCreateInstance(targetProperty.PropertyType), new object[] { });
                    }
                    else
                    {
                        targetProperty.SetValue(newInstance, ConvertPropertyValue(p.Value), new object[] { });
                    }
                }

                return newInstance;
            }

            return propertyValue;
        }

        public static ODataCollectionValue ReadCollectionParameterValue(ODataCollectionReader collectionReader)
        {
            List<object> collectionItems = new List<object>();
            while (collectionReader.Read())
            {
                if (collectionReader.State == ODataCollectionReaderState.Completed)
                {
                    break;
                }

                if (collectionReader.State == ODataCollectionReaderState.Value)
                {
                    collectionItems.Add(collectionReader.Item);
                }
            }

            ODataCollectionValue result = new ODataCollectionValue();
            result.Items = collectionItems;
            return result;
        }

        public static ODataEntry ReadEntryParameterValue(ODataReader entryReader)
        {
            ODataEntry entry = null;
            while (entryReader.Read())
            {
                if (entryReader.State == ODataReaderState.EntryEnd)
                {
                    entry = entryReader.Item as ODataEntry;
                }
            }

            return entry;
        }

        public static ODataFeed ReadFeedParameterValue(ODataReader feedReader)
        {
            ODataFeed entry = null;
            while (feedReader.Read())
            {
                if (feedReader.State == ODataReaderState.FeedEnd)
                {
                    entry = feedReader.Item as ODataFeed;
                }
            }

            return entry;
        }

        /// <summary>
        /// Generates the URI used to reference an entry in the data store.
        /// </summary>
        /// <param name="entry">The entry instance to build the URI for.</param>
        /// <param name="navigationSource">The navigation source that the entry belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>The generated URI.</returns>
        public static Uri BuildEntryUri(object entry, IEdmNavigationSource navigationSource, ODataVersion targetVersion)
        {
            if (navigationSource is IEdmEntitySet)
            {
                var entitySet = navigationSource as IEdmEntitySet;
                string keySegment = BuildKeyString(entry, entitySet.EntityType(), targetVersion);
                return new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name + "(" + keySegment + ")");
            }
            else if (navigationSource is IEdmSingleton)
            {
                var singleton = navigationSource as IEdmSingleton;
                return new Uri(ServiceConstants.ServiceBaseUri, singleton.Name);
            }
            else if (navigationSource is IEdmContainedEntitySet)
            {
                // TODO: [tian] Current the URI for containment resource is incorrect.
                var containedEntitySet = navigationSource as IEdmContainedEntitySet;
                return new Uri(ServiceConstants.ServiceBaseUri, containedEntitySet.Name);
            }
            else
            {
                throw new InvalidOperationException("Unsupported entry uri for " + navigationSource.Name);
            }
        }

        /// <summary>
        /// Creates the key segment to use in the ID/EDitLink fields of an entry.
        /// </summary>
        /// <param name="entityInstance">The instance to get the key values from</param>
        /// <param name="entityType">The entity type of the instance</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>A Key segment that contains a literal encoded string key-value pairs for property names and their values</returns>
        private static string BuildKeyString(object entityInstance, IEdmEntityType entityType, ODataVersion targetVersion)
        {
            if (entityType.DeclaredKey != null && entityType.DeclaredKey.Count() == 1)
            {
                var keyMember = entityType.Key().Single();
                PropertyInfo property = entityInstance.GetType().GetProperty(keyMember.Name);
                return ODataUriUtils.ConvertToUriLiteral(property.GetValue(entityInstance, null), targetVersion, DataSourceManager.GetCurrentDataSource().Model);
            }
            else
            {
                var keyStringFragments = new List<string>();
                foreach (var keyMember in entityType.Key())
                {
                    PropertyInfo property = entityInstance.GetType().GetProperty(keyMember.Name);
                    keyStringFragments.Add(String.Format("{0}={1}", keyMember.Name, ODataUriUtils.ConvertToUriLiteral(property.GetValue(entityInstance, null), targetVersion, DataSourceManager.GetCurrentDataSource().Model)));
                }

                return String.Join(",", keyStringFragments);
            }
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
