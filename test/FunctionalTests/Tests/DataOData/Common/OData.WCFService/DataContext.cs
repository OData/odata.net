//---------------------------------------------------------------------
// <copyright file="DataContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;

    /// <summary>
    /// Data access class for the backing data store.
    /// </summary>
    public class DataContext
    {
        private readonly InMemoryDataSource dataSource;

        /// <summary>
        /// Constructor for the DataContext class.
        /// </summary>
        /// <param name="dataSource">The underlying data store.</param>
        public DataContext(InMemoryDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        /// <summary>
        /// Creates a new instance of a type within the data store.
        /// </summary>
        /// <param name="entitySet">The entity set to create the new item for.</param>
        /// <returns>The new instance.</returns>
        public object CreateNewItem(IEdmEntitySet entitySet)
        {
            IList collection = this.GetItemCollection(entitySet);
            Type itemType = collection.GetType().GetGenericArguments().Single();
            return Activator.CreateInstance(itemType);
        }

        /// <summary>
        /// Retrieves an existing item from the data store.
        /// </summary>
        /// <param name="entitySet">The entity set that the item belongs to.</param>
        /// <param name="itemKeys">The key values identifying the item to </param>
        /// <returns>The requested item.</returns>
        public object GetItem(IEdmEntitySet entitySet, IDictionary<string, object> itemKeys)
        {
            IList collection = this.GetItemCollection(entitySet);
            return this.FindItem(collection, itemKeys);
        }

        /// <summary>
        /// Retrieves the root query for the specified entity set.
        /// </summary>
        /// <param name="entitySet">The entity set.</param>
        /// <returns>The root query for the entity set.</returns>
        public IQueryable GetRootQuery(IEdmEntitySet entitySet)
        {
            return (IQueryable)(typeof(InMemoryDataSource).GetProperty(entitySet.Name).GetValue(this.dataSource, new object[] { }));
        }

        /// <summary>
        /// Retrieves the property value from a specified instance.
        /// </summary>
        /// <param name="item">The instance.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The value of the requested property.</returns>
        public static object GetPropertyValue(object item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, new object[] { });
        }

        /// <summary>
        /// Deletes an item from the data store.
        /// </summary>
        /// <param name="entitySet">The entity set to remove the item from.</param>
        /// <param name="itemKeys">The key values identifying the item to remove.</param>
        public void DeleteItem(IEdmEntitySet entitySet, IDictionary<string, object> itemKeys)
        {
            IList collection = this.GetItemCollection(entitySet);
            collection.Remove(this.FindItem(collection, itemKeys));
        }

        /// <summary>
        /// Adds a new item to the data store.
        /// </summary>
        /// <param name="entitySet">The entity set to add the item to.</param>
        /// <param name="newItem">The new item to add.</param>
        public void AddItem(IEdmEntitySet entitySet, object newItem)
        {
            IList itemCollection = this.GetItemCollection(entitySet);
            itemCollection.Add(newItem);
        }

        /// <summary>
        /// Modifies the value of an exising item's property.
        /// </summary>
        /// <param name="entitySet">The entity set that the item belongs to.</param>
        /// <param name="itemKeys">The key values identifying the item to modify.</param>
        /// <param name="propertyName">The name of the property to change.</param>
        /// <param name="propertyValue">The new value to set the property to.</param>
        public void UpdateItem(IEdmEntitySet entitySet, IDictionary<string, object> itemKeys, string propertyName, object propertyValue)
        {
            IList collection = this.GetItemCollection(entitySet);
            object item = this.FindItem(collection, itemKeys);
            UpdatePropertyValue(item, propertyName, propertyValue);
        }

        /// <summary>
        /// Modifies the value of a specified instance's property.
        /// </summary>
        /// <param name="item">The instance to modify.</param>
        /// <param name="propertyName">The name of the property to change.</param>
        /// <param name="propertyValue">The new value to set the property to.</param>
        public static void UpdatePropertyValue(object item, string propertyName, object propertyValue)
        {
            PropertyInfo targetProperty = item.GetType().GetProperty(propertyName);
            targetProperty.SetValue(item, ConvertPropertyValue(propertyValue, targetProperty.PropertyType), new object[]{});
        }

        private IList GetItemCollection(IEdmEntitySet entitySet)
        {
            var member =
                typeof(InMemoryDataSource)
                    .GetMembers(BindingFlags.NonPublic | BindingFlags.Static)
                    .OfType<FieldInfo>()
                    .SingleOrDefault(f => f.Name.Equals(entitySet.Name, StringComparison.InvariantCultureIgnoreCase));

            return (IList)member.GetValue(this.dataSource);
        }

        private object FindItem(IList itemCollection, IDictionary<string, object> itemKeys)
        {
            Type itemType = itemCollection.GetType().GetGenericArguments().Single();
            foreach (var item in itemCollection)
            {
                if (itemKeys.All(kv => itemType.GetProperty(kv.Key).GetValue(item, new object[] { }).Equals(kv.Value)))
                {
                    return item;
                }
            }

            throw new InvalidOperationException("No entity found matching " +string.Join(",", itemKeys.Select(kv => kv.Key + "=" + kv.Value.ToString())));
        }

        private static object ConvertPropertyValue(object propertyValue, Type propertyType)
        {
            if (propertyType == typeof(DateTime) && !(propertyValue is DateTime))
            {
                return XmlConvert.ToDateTime(propertyValue.ToString(), XmlDateTimeSerializationMode.RoundtripKind);
            }

            if (propertyType == typeof(float))
            {
                return Convert.ToSingle(propertyValue);
            }

            return propertyValue;
        }
    }
}