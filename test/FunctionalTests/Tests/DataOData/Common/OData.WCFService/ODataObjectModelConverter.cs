//---------------------------------------------------------------------
// <copyright file="ODataObjectModelConverter.cs" company="Microsoft">
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
    using Microsoft.OData;
    using Microsoft.OData.UriParser;

    /// <summary>
    /// Static methods for converting CLR objects from the data store into OData objects.
    /// </summary>
    public static class ODataObjectModelConverter
    {
        /// <summary>
        /// Converts an item from the data store into an ODataResource.
        /// </summary>
        /// <param name="element">The item to convert.</param>
        /// <param name="entitySet">The entity set that the item belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>The converted ODataResource.</returns>
        public static ODataResource ConvertToODataEntry(object element, IEdmEntitySet entitySet, ODataVersion targetVersion)
        {
            IEdmEntityType entityType = entitySet.EntityType();

            Uri entryUri = BuildEntryUri(element, entitySet, targetVersion);

            var entry = new ODataResource
            {
                // writes out the edit link including the service base uri  , e.g.: http://<serviceBase>/Customers('ALFKI')
                EditLink = entryUri,

                // writes out the self link including the service base uri  , e.g.: http://<serviceBase>/Customers('ALFKI')
                ReadLink = entryUri,

                // we use the EditLink as the Id for this entity to maintain convention,
                Id = entryUri,

                // writes out the <category term='Customer'/> element
                TypeName = element.GetType().Namespace + "." + entityType.Name,

                Properties = entityType.StructuralProperties().Select(p => ConvertToODataProperty(element, p.Name)),
            };

            return entry;
        }

        /// <summary>
        /// Converts a value from the data store into an ODataProperty.
        /// </summary>
        /// <param name="instance">The item from the data store.</param>
        /// <param name="propertyName">The name of the property to convert.</param>
        /// <returns>The converted ODataProperty.</returns>
        public static ODataProperty ConvertToODataProperty(object instance, string propertyName)
        {
            object value = DataContext.GetPropertyValue(instance, propertyName);
            return CreateODataProperty(value, propertyName);
        }

        /// <summary>
        /// Create ODataProperty given property name and value.
        /// </summary>
        /// <param name="instance">The value of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The converted ODataProperty.</returns>
        public static ODataProperty CreateODataProperty(object value, string propertyName)
        {
            if (value != null && value.GetType().IsGenericType)
            {
                // build a collection type property
                Type t = value.GetType();
                string genericTypeName = t.GetGenericTypeDefinition().Name;
                genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
                genericTypeName += "(" + t.GetGenericArguments().Single().FullName.Replace("System.", "Edm.") + ")";
                return new ODataProperty { Name = propertyName, Value = new ODataCollectionValue() { TypeName = genericTypeName, Items = (value as IEnumerable).Cast<object>() } };
            }

            return new ODataProperty { Name = propertyName, Value = value };
        }

        /// <summary>
        /// Generates the URI used to reference an entry in the data store.
        /// </summary>
        /// <param name="entry">The entry instance to build the URI for.</param>
        /// <param name="entitySet">The entity set that the entry belongs to.</param>
        /// <param name="targetVersion">The OData version this segment is targeting.</param>
        /// <returns>The generated URI.</returns>
        public static Uri BuildEntryUri(object entry, IEdmEntitySet entitySet, ODataVersion targetVersion)
        {
            string keySegment = BuildKeyString(entry, entitySet.EntityType(), targetVersion);
            return new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name + "(" + keySegment + ")");
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
                return ODataUriUtils.ConvertToUriLiteral(property.GetValue(entityInstance, null), targetVersion);
            }
            else
            {
                var keyStringFragments = new List<string>();
                foreach (var keyMember in entityType.Key())
                {
                    PropertyInfo property = entityInstance.GetType().GetProperty(keyMember.Name);
                    keyStringFragments.Add(String.Format("{0}={1}", keyMember.Name, ODataUriUtils.ConvertToUriLiteral(property.GetValue(entityInstance, null), targetVersion)));
                }

                return String.Join(",", keyStringFragments);
            }
        }
    }
}