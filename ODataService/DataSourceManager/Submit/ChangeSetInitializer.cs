// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Submit;
using DataSourceManager.DataStoreManager;
using DataSourceManager.Utils;

namespace DataSourceManager.Submit
{
    /// <summary>
    ///     ChangeSetInitializer class.
    ///     Since our datasource is in memory,
    ///     we just confirm the data change here, not in SubmitExecutor
    /// </summary>
    public class ChangeSetInitializer<TDataStoreType> : IChangeSetInitializer
    {
        public Task InitializeAsync(SubmitContext context, CancellationToken cancellationToken)
        {
            var requestScope = context.Api.ServiceProvider.GetService(typeof(HttpRequestScope)) as HttpRequestScope;
            var key = InMemoryProviderUtils.GetSessionId(requestScope?.HttpRequest.HttpContext);
            var dataStoreManager = context.GetApiService<IDataStoreManager<string, TDataStoreType>>();
            if (dataStoreManager == null)
            {
                throw new ArgumentNullException("DataStoreManager Not Found",
                    typeof(IDataStoreManager<string, TDataStoreType>).ToString());
            }

            var dataSource = dataStoreManager.GetDataStoreInstance(key);
            foreach (var dataModificationItem in context.ChangeSet.Entries.OfType<DataModificationItem>())
            {
                var resourceType = dataModificationItem.ExpectedResourceType;
                if (dataModificationItem.ActualResourceType != null &&
                    dataModificationItem.ActualResourceType != dataModificationItem.ExpectedResourceType)
                {
                    resourceType = dataModificationItem.ActualResourceType;
                }

                var operation = dataModificationItem.EntitySetOperation;
                object resource = null;
                switch (operation)
                {
                    case RestierEntitySetOperation.Insert:
                        // Here we create a instance of entity, parameters are from the request.
                        // Known issues: not support odata.id
                        resource = Activator.CreateInstance(resourceType);
                        SetValues(resource, resourceType, dataModificationItem.LocalValues);
                        dataModificationItem.Resource = resource;

                        // insert new entity into entity set
                        var entitySetPropForInsert = GetEntitySetPropertyInfoFromDataModificationItem(dataSource,
                            dataModificationItem);

                        if (entitySetPropForInsert != null && entitySetPropForInsert.CanWrite)
                        {
                            var originSet = entitySetPropForInsert.GetValue(dataSource);
                            entitySetPropForInsert.PropertyType.GetMethod("Add").Invoke(originSet, new[] {resource});
                        }
                        break;
                    case RestierEntitySetOperation.Update:
                        resource = FindResource(dataSource, context, dataModificationItem, cancellationToken);
                        dataModificationItem.Resource = resource;

                        // update the entity
                        if (resource != null)
                        {
                            SetValues(resource, resourceType, dataModificationItem.LocalValues);
                        }
                        break;
                    case RestierEntitySetOperation.Delete:
                        resource = FindResource(dataSource, context, dataModificationItem, cancellationToken);
                        dataModificationItem.Resource = resource;

                        // remove the entity
                        if (resource != null)
                        {
                            var entitySetPropForRemove = GetEntitySetPropertyInfoFromDataModificationItem(dataSource,
                                dataModificationItem);

                            if (entitySetPropForRemove != null && entitySetPropForRemove.CanWrite)
                            {
                                var originSet = entitySetPropForRemove.GetValue(dataSource);
                                entitySetPropForRemove.PropertyType.GetMethod("Remove").Invoke(originSet, new[] {resource});
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return Task.WhenAll();
        }

        /// <summary>
        ///     Convert EdmStructuredObject to an object with type.
        /// </summary>
        /// <param name="edmStructuredObject">An object with EdmStructuredType.</param>
        /// <param name="type">Desired object type.</param>
        /// <returns>Result object.</returns>
        private static object ConvertEdmStructuredObjectToTypedObject(
            IEdmStructuredObject edmStructuredObject, Type type)
        {
            if (edmStructuredObject == null)
            {
                return null;
            }

            var obj = Activator.CreateInstance(type);
            var propertyInfos = type.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                if (!propertyInfo.CanWrite)
                {
                    continue;
                }

                object value = null;
                edmStructuredObject.TryGetPropertyValue(propertyInfo.Name, out value);
                if (value == null)
                {
                    propertyInfo.SetValue(obj, value);
                    continue;
                }

                if (!propertyInfo.PropertyType.IsInstanceOfType(value))
                {
                    var edmObj = value as IEdmStructuredObject;
                    if (edmObj == null)
                    {
                        throw new NotSupportedException(string.Format(
                            CultureInfo.InvariantCulture,
                            propertyInfo.PropertyType.ToString()));
                    }

                    value = ConvertEdmStructuredObjectToTypedObject(edmObj, propertyInfo.PropertyType);
                }

                propertyInfo.SetValue(obj, value);
            }

            return obj;
        }

        /// <summary>
        ///     Convert EdmStructuredObjectCollection object to an object with type.
        /// </summary>
        /// <typeparam name="TEdmStructuredObjectCollection">EdmStructuredObjectCollection type.</typeparam>
        /// <param name="edmStructuredObjectCollection">An object with EdmStructuredObjectCollection.</param>
        /// <param name="type">Desired object type.</param>
        /// <returns>Result object.</returns>
        private static object ConvertEdmStructuredObjectCollectionToTypedObject<TEdmStructuredObjectCollection>(
            TEdmStructuredObjectCollection edmStructuredObjectCollection, Type type)
        {
            var valueType = typeof(Collection<>).MakeGenericType(type);
            var value = Activator.CreateInstance(valueType);
            var collection = edmStructuredObjectCollection as System.Collections.IEnumerable;
            if (collection == null)
            {
                return null;
            }

            foreach (var c in collection)
            {
                var obj = ConvertEdmStructuredObjectToTypedObject(c as IEdmStructuredObject, type);
                value.GetType().GetMethod("Add").Invoke(value, new[] {obj});
            }

            return value;
        }

        private static void SetValues(object instance, Type type, IReadOnlyDictionary<string, object> values)
        {
            foreach (var propertyPair in values)
            {
                var value = propertyPair.Value;
                var propertyInfo = type.GetProperty(propertyPair.Key);
                if (propertyInfo == null)
                {
                    continue;
                }

                if (value == null)
                {
                    // If the property value is null, we set null in the object too.
                    propertyInfo.SetValue(instance, null);
                    continue;
                }

                if (!propertyInfo.PropertyType.IsInstanceOfType(value))
                {
                    var dic = value as IReadOnlyDictionary<string, object>;

                    if (dic != null)
                    {
                        value = propertyInfo.GetValue(instance);
                        SetValues(value, propertyInfo.PropertyType, dic);
                    }
                    else if (propertyInfo.PropertyType.IsGenericType)
                    {
                        // EdmStructuredObjectCollection
                        var realType = propertyInfo.PropertyType.GenericTypeArguments[0];
                        value = ConvertEdmStructuredObjectCollectionToTypedObject(value, realType);
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format(
                            CultureInfo.InvariantCulture,
                            propertyPair.Key));
                    }
                }

                propertyInfo.SetValue(instance, value);
            }
        }

        private static object FindResource(
            object instance,
            SubmitContext context,
            DataModificationItem item,
            CancellationToken cancellationToken)
        {
            var entitySetPropertyInfo = GetEntitySetPropertyInfoFromDataModificationItem(instance, item);
            var originSet = entitySetPropertyInfo.GetValue(instance);
            object resource = null;
            Type resourceType = null;
            var enumerableSet = originSet as IEnumerable<object>;
            if (enumerableSet != null)
            {
                resourceType = originSet.GetType().GetGenericArguments()[0];
                foreach (var o in enumerableSet)
                {
                    var foundFlag = true;
                    foreach (var keyVal in item.ResourceKey)
                    {
                        var entityProp = o.GetType().GetProperty(keyVal.Key);
                        if (entityProp != null)
                        {
                            foundFlag &= entityProp.GetValue(o).Equals(keyVal.Value);
                        }
                        else
                        {
                            foundFlag = false;
                        }

                        if (!foundFlag)
                        {
                            break;
                        }
                    }

                    if (foundFlag)
                    {
                        resource = Convert.ChangeType(o, resourceType);
                        break;
                    }
                }
            }

            if (resource == null)
            {
                throw new Exception("Resource Not Found");
            }

            // This means no If-Match or If-None-Match header
            if (item.OriginalValues == null || item.OriginalValues.Count == 0)
            {
                return resource;
            }

            // Make a list of resource as IQueryable to valid etag
            var listOfItemType = typeof(List<>).MakeGenericType(resourceType);
            var list = Activator.CreateInstance(listOfItemType);
            listOfItemType.GetMethod("Add").Invoke(list, new[] {resource});
            var method = typeof(Queryable).GetMethod(
                "AsQueryable",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[] {listOfItemType},
                null);
            resource = item.ValidateEtag(method.Invoke(list, new[] {list}) as IQueryable);
            return resource;
        }

        private static PropertyInfo GetEntitySetPropertyInfoFromDataModificationItem(object instance,
            DataModificationItem dataModificationItem)
        {
            var entitySetName = dataModificationItem.ResourceSetName;
            var entitySetProp = instance.GetType()
                .GetProperty(entitySetName, BindingFlags.Public | BindingFlags.Instance);
            return entitySetProp;
        }
    }
}
