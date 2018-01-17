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
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    /// <summary>
    /// Static methods for converting CLR objects from the data store into OData objects.
    /// </summary>
    public static class ODataObjectModelConverter
    {
        /// <summary>
        /// Converts an item from the data store into an ODataEntry.
        /// </summary>
        /// <param name="element">The item to convert.</param>
        /// <param name="navigationSource">The navigation source that the item belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>The converted ODataEntry.</returns>
        public static ODataResourceWrapper ConvertToODataEntry(object element, IEdmNavigationSource entitySource, ODataVersion targetVersion)
        {
            IEdmStructuredType entityType = EdmClrTypeUtils.GetEdmType(DataSourceManager.GetCurrentDataSource().Model, element) as IEdmStructuredType;

            if (entityType == null)
            {
                throw new InvalidOperationException("Can not create an entry for " + entitySource.Name);
            }

            var propertiesOrNestedResourceInfos = GetPropertiesOrNestedResourceInfos(element, entityType);
            var entry = new ODataResource
            {
                Properties = propertiesOrNestedResourceInfos.OfType<ODataProperty>(),
            };

            var resourceWrapper = new ODataResourceWrapper()
            {
                Resource = entry,
                NestedResourceInfoWrappers = propertiesOrNestedResourceInfos.OfType<ODataNestedResourceInfoWrapper>().ToList(),
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

            return resourceWrapper;
        }

        /// <summary>
        /// Converts an item from the data store into an ODataEntityReferenceLink.
        /// </summary>
        /// <param name="element">The item to convert.</param>
        /// <param name="navigationSource">The navigation source that the item belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>The converted ODataEntityReferenceLink represent with ODataEntry.</returns>
        public static ODataResource ConvertToODataEntityReferenceLink(object element, IEdmNavigationSource entitySource, ODataVersion targetVersion)
        {
            IEdmStructuredType entityType = EdmClrTypeUtils.GetEdmType(DataSourceManager.GetCurrentDataSource().Model, element) as IEdmStructuredType;

            if (entityType == null)
            {
                throw new InvalidOperationException("Can not create an entry for " + entitySource.Name);
            }

            var link = new ODataResource();
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
        public static IEnumerable<ODataResource> ConvertToODataEntityReferenceLinks(IEnumerable element, IEdmNavigationSource entitySource, ODataVersion targetVersion)
        {
            List<ODataResource> links = new List<ODataResource>();
            foreach (var each in element)
            {
                ODataResource link = ConvertToODataEntityReferenceLink(each, entitySource, targetVersion);
                links.Add(link);
            }

            return links;
        }

        public static IEnumerable<object> GetPropertiesOrNestedResourceInfos(object instance, IEdmStructuredType structuredType)
        {
            var nonOpenProperties = new List<object>();
            var structuralProperties = structuredType.StructuralProperties();
            foreach (var sp in structuralProperties)
            {
                nonOpenProperties.Add(ConvertToODataProperty(instance, sp.Name));
            }

            if (structuredType.IsOpen)
            {
                var openProperties = GetOpenPropertiesOrNestedResourceInfos(instance);
                return MergeOpenAndNonOpenProperties(nonOpenProperties, openProperties);
            }
            else
            {
                return nonOpenProperties;
            }
        }

        public static object ParseJsonToPrimitiveValue(string rawValue)
        {
            Debug.Assert(rawValue != null && rawValue.Length > 0 && rawValue.IndexOf('{') != 0 && rawValue.IndexOf('[') != 0,
                  "rawValue != null && rawValue.Length > 0 && rawValue.IndexOf('{') != 0 && rawValue.IndexOf('[') != 0");
            ODataCollectionValue collectionValue = (ODataCollectionValue)
                Microsoft.OData.ODataUriUtils.ConvertFromUriLiteral(string.Format("[{0}]", rawValue), ODataVersion.V4);
            foreach (object tmp in collectionValue.Items)
            {
                return tmp;
            }

            return null;
        }

        private static IEnumerable<object> GetOpenPropertiesOrNestedResourceInfos(object instance)
        {
            List<object> openProperties = new List<object>();
            var inst = instance as OpenClrObject;
            if (inst != null)
            {
                Dictionary<string, object> openPropertiesInClr = inst.OpenProperties;
                if (openPropertiesInClr != null)
                {
                    foreach (var p in openPropertiesInClr)
                    {
                        object primitiveVal = p.Value;
                        if (p.Value is ODataUntypedValue)
                        {
                            openProperties.Add(new ODataProperty { Name = p.Key, Value = p.Value });
                        }
                        else
                        {
                            openProperties.Add(CreateODataPropertyOrNestedResourceInfo(primitiveVal, p.Key));
                        }
                    }
                }
            }

            return openProperties;
        }

        private static IEnumerable<T> MergeOpenAndNonOpenProperties<T>(IEnumerable<T> nonOpenProperties, IEnumerable<T> openProperties)
        {
            return nonOpenProperties.Concat(openProperties);
        }

        public static object ConvertToODataProperty(object instance, string propertyName)
        {
            object value = instance.GetType().GetProperty(propertyName).GetValue(instance, null);
            object property = CreateODataPropertyOrNestedResourceInfo(value, propertyName);

            var odataProperty = property as ODataProperty;

            // Add Annotation in property level
            if (odataProperty != null && ((ClrObject)instance).Annotations != null)
            {
                foreach (InstanceAnnotationType annotation in ((ClrObject)instance).Annotations)
                {
                    if (!string.IsNullOrEmpty(annotation.Target) && annotation.Target.Equals(propertyName))
                    {
                        odataProperty.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.Name, annotation.ConvertValueToODataValue()));
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

        public static object CreateODataPropertyOrNestedResourceInfo(object value, string propertyName)
        {
            var cValue = CreateODataValue(value);

            var nestedResourceOrResourceSet = cValue as ODataItemWrapper;
            if (nestedResourceOrResourceSet != null)
            {
                return new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = propertyName,
                        IsCollection = !(cValue is ODataResourceWrapper)
                    },
                    NestedResourceOrResourceSet = nestedResourceOrResourceSet,
                };
            }

            return new ODataProperty { Name = propertyName, Value = cValue };
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

                    if (tmp.Count > 0 && tmp.First() is ODataResourceWrapper)
                    {
                        return new ODataResourceSetWrapper()
                        {
                            ResourceSet = new ODataResourceSet(),
                            Resources = new List<ODataResourceWrapper>(tmp.Select(obj => obj as ODataResourceWrapper))
                        };
                    }

                    return new ODataCollectionValue() { TypeName = genericTypeName, Items = tmp };
                }
                else if (t.Namespace != "System" && !t.Namespace.StartsWith("Microsoft.Spatial") && !t.Namespace.StartsWith("Microsoft.OData.Edm"))
                {
                    if (t.IsEnum == true)
                    {
                        return new ODataEnumValue(value.ToString(), t.FullName);
                    }
                    else
                    {
                        return ConvertToODataEntry(value, null, ODataVersion.V4);
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
            else if (propertyUnderlyingType.IsGenericType && propertyValue is IEnumerable)
            {
                var collection = Utility.QuickCreateInstance(propertyUnderlyingType);

                foreach (var item in propertyValue as IEnumerable)
                {
                    var itemType = propertyUnderlyingType.GetGenericArguments().Single();
                    var collectionItem = ODataObjectModelConverter.ConvertPropertyValue(item, itemType);

                    propertyUnderlyingType.GetMethod("Add").Invoke(collection, new object[] { collectionItem });
                }

                return collection;
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
                        Type listType = string.IsNullOrEmpty(collection.TypeName) ? null : EdmClrTypeUtils.GetInstanceType(collection.TypeName);
                        if (listType == null)
                        {
                            Type itemType = item.GetType();
                            listType = typeof(List<>).MakeGenericType(new[] { itemType });
                        }
                        collectionList = (IList)Utility.QuickCreateInstance(listType);
                    }

                    collectionList.Add(item);
                }
                return collectionList;
            }

            if (propertyValue is ODataResource)
            {
                ODataResource entry = (ODataResource)propertyValue;
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

        public static ODataResource ReadEntryParameterValue(ODataReader entryReader)
        {
            ODataResource entry = null;
            while (entryReader.Read())
            {
                if (entryReader.State == ODataReaderState.ResourceEnd)
                {
                    entry = entryReader.Item as ODataResource;
                }
            }

            return entry;
        }

        public static ODataResourceSet ReadFeedParameterValue(ODataReader feedReader)
        {
            ODataResourceSet entry = null;
            while (feedReader.Read())
            {
                if (feedReader.State == ODataReaderState.ResourceSetEnd)
                {
                    entry = feedReader.Item as ODataResourceSet;
                }
            }

            return entry;
        }

        public static object ReadEntityOrEntityCollection(ODataReader reader, bool forFeed)
        {
            MethodInfo addMethod = null;

            // Store the last entry of the top-level feed or the top-level entry;
            ODataResource entry = null;

            // Store entries at each level
            Stack<ODataItem> items = new Stack<ODataItem>();

            // Store the objects with its parent for current level.
            // Example:
            //    CompleCollection: [ complex1, complex2 ]
            //    objects contains [{CompleCollection, complex1Obj}, {CompleCollection, complex2Obj}] when NestedResourceInfoEnd for CompleCollection
            Stack<KeyValuePair<ODataItem, object>> objects = new Stack<KeyValuePair<ODataItem, object>>();

            // Store the SetValue action for complex property.
            Stack<KeyValuePair<ODataItem, Action<object>>> actions = new Stack<KeyValuePair<ODataItem, Action<object>>>();
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceStart:
                    case ODataReaderState.NestedResourceInfoStart:
                        {
                            items.Push(reader.Item);
                            break;
                        }
                    case ODataReaderState.NestedResourceInfoEnd:
                        {
                            items.Pop();
                            // Create current complex property value.
                            var currentProperty = reader.Item as ODataNestedResourceInfo;
                            var parent = items.Peek() as ODataResource;
                            var parentType = EdmClrTypeUtils.GetInstanceType(parent.TypeName);
                            var propertyInfo = parentType.GetProperty(currentProperty.Name);
                            if (propertyInfo != null)
                            {
                                var propertyType = propertyInfo.PropertyType;
                                if (propertyType.IsGenericType)
                                {
                                    Type listType = typeof(List<>).MakeGenericType(propertyType.GetGenericArguments()[0]);
                                    addMethod = listType.GetMethod("Add");
                                    var currentList = Activator.CreateInstance(listType);
                                    while (objects.Count > 0 && objects.Peek().Key == currentProperty)
                                    {
                                        addMethod.Invoke(currentList, new[] { objects.Pop().Value });
                                    }

                                    // Keep the order of all the items
                                    MethodInfo reverseMethod = listType.GetMethod("Reverse", new Type[0]);
                                    reverseMethod.Invoke(currentList, new object[0]);
                                    actions.Push(new KeyValuePair<ODataItem, Action<object>>(parent, obj => propertyInfo.SetValue(obj, currentList)));
                                }
                                else
                                {
                                    var propertyValue = objects.Pop().Value;
                                    actions.Push(new KeyValuePair<ODataItem, Action<object>>(parent, obj => propertyInfo.SetValue(obj, propertyValue)));
                                }
                            }

                            break;
                        }
                    case ODataReaderState.ResourceEnd:
                        {
                            // Create object for current resource.
                            entry = reader.Item as ODataResource;
                            object item = ODataObjectModelConverter.ConvertPropertyValue(entry);
                            while (actions.Count > 0 && actions.Peek().Key == entry)
                            {
                                actions.Pop().Value.Invoke(item);
                            }

                            items.Pop();
                            var parent = items.Count > 0 ? items.Peek() : null;
                            objects.Push(new KeyValuePair<ODataItem, object>(parent, item));
                            break;
                        }
                    default:
                        break;
                }
            }
            if (forFeed)
            {
                // create the list. This would require the first type is not derived type.
                List<object> topLeveObjects = new List<object>();
                while (objects.Count > 0)
                {
                    topLeveObjects.Add(objects.Pop().Value);
                }

                Type type = null;
                // Need to fix this if all items in the collection are null;
                if (entry == null || string.IsNullOrEmpty(entry.TypeName))
                {
                    for (int i = 0; i < topLeveObjects.Count; i++)
                    {
                        if (topLeveObjects[i] != null)
                        {
                            type = topLeveObjects[i].GetType();
                            break;
                        }
                    }
                }
                else
                {
                    type = EdmClrTypeUtils.GetInstanceType(entry.TypeName);
                }

                Type listType = typeof(List<>).MakeGenericType(type);
                addMethod = listType.GetMethod("Add");
                var list = Activator.CreateInstance(listType);
                for (int i = topLeveObjects.Count - 1; i >= 0; i--)
                {
                    addMethod.Invoke(list, new[] { topLeveObjects[i] });
                }

                return list;
            }
            else
            {
                return objects.Pop().Value;
            }
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
