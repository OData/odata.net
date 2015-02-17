//---------------------------------------------------------------------
// <copyright file="EntitySetDataRow.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Row of data in the EntitySet.
    /// </summary>
    [DebuggerDisplay("{Key.ToString()}")]
    public class EntitySetDataRow
    {
        private readonly EntitySetDataRowData data;

        /// <summary>
        /// Initializes a new instance of the EntitySetDataRow class
        /// </summary>
        /// <param name="parent">The parent of the row</param>
        /// <param name="entityType">The entity type of the row</param>
        protected internal EntitySetDataRow(EntitySetData parent, EntityType entityType)
        {
            this.Parent = parent;
            this.EntityType = entityType;
            this.Key = new EntityDataKey(parent.Parent.GetKeyPropertyNames(entityType));

            string[] nonKeyProperties;
            string[] dynamicPropertyPathList;
            this.BuildPropertyPaths(out nonKeyProperties, out dynamicPropertyPathList);
            this.data = new EntitySetDataRowData(this.Key, nonKeyProperties, dynamicPropertyPathList, entityType.FullName);
        }

        /// <summary>
        /// Gets the EntitySetData this row belongs to.
        /// </summary>
        /// <value>The parent EntitySetData.</value>
        public EntitySetData Parent { get; private set; }

        /// <summary>
        /// Gets the EntityType this data row represents.
        /// </summary>
        /// <value>The the entity type.</value>
        public EntityType EntityType { get; private set; }

        /// <summary>
        /// Gets the key of the row.
        /// </summary>
        /// <value>The key of the row.</value>
        public EntityDataKey Key { get; internal set; }

        /// <summary>
        /// Gets property paths.
        /// </summary>
        public IEnumerable<string> PropertyPaths
        {
            get
            {
                return this.data.PropertyPaths;
            }
        }

        /// <summary>
        /// Gets or sets the value of the property with the specified path.
        /// </summary>
        /// <param name="propertyPath">Path of the property.</param>
        /// <value>Value of the property.</value>
        public object this[string propertyPath]
        {
            get { return this.data.GetValue(propertyPath); }
            set { this.data.SetValue(propertyPath, value); }
        }

        /// <summary>
        /// Gets the value of the property with the specified path.
        /// </summary>
        /// <param name="propertyPath">Path of the property.</param>
        /// <returns>Value of the property.</returns>
        public object GetValue(string propertyPath)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyPath, "propertyPath");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyPath, "propertyPath");

            return this.data.GetValue(propertyPath);
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="propertyPath">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void SetValue(string propertyPath, object value)
        {
            ExceptionUtilities.CheckArgumentNotNull(propertyPath, "propertyPath");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(propertyPath, "propertyPath");

            this.data.SetValue(propertyPath, value);
        }

        internal void ImportFrom(object item)
        {
            ExceptionUtilities.CheckObjectNotNull(item, "Cannot import row from null object.");

            foreach (PropertyInfo propertyInfo in item.GetType().GetProperties())
            {
                MemberProperty mp = this.EntityType.AllProperties.Where(p => p.Name == propertyInfo.Name).SingleOrDefault();
                if (mp == null)
                {
                    if (this.EntityType.AllNavigationProperties.Any(p => p.Name == propertyInfo.Name))
                    {
                        // ignore navigation properties
                        continue;
                    }

                    throw new TaupoInvalidOperationException(
                        string.Format(CultureInfo.InvariantCulture, "Entity type '{0}' does not contain property '{1}'.", this.EntityType.FullName, propertyInfo.Name));
                }

                this.SetDataFromPropertyInfo(item, propertyInfo, mp, string.Empty);
            }
        }

        internal string ToTraceString()
        {
            var sb = new StringBuilder();
            sb.Append(" {");
            string separator = " ";

            foreach (string path in this.PropertyPaths)
            {
                object value = this.GetValue(path);
                if (value != UninitializedData.Value)
                {
                    sb.Append(separator);
                    sb.Append(path);
                    sb.Append("=");
                    sb.Append(ToStringConverter.ConvertObjectToString(value));
                    separator = "; ";
                }
            }

            sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// Builds the sets of property paths 
        /// </summary>
        /// <param name="nonKeyPropertyPaths">Returns as an out parameter the list of non key properties that are not collections</param>
        /// <param name="dynamicPropertyPathList">Returns as an out parameter this list of collection properties in the entitytype</param>
        private void BuildPropertyPaths(out string[] nonKeyPropertyPaths, out string[] dynamicPropertyPathList)
        {
            this.BuildPropertyPaths(string.Empty, this.EntityType.AllProperties.Where(p => p.IsPrimaryKey == false), out nonKeyPropertyPaths, out dynamicPropertyPathList);
        }

        /// <summary>
        /// Builds the sets of property paths 
        /// </summary>
        /// <param name="propertyPath">The PropertyPath is the starting point that will be appended on to</param>
        /// <param name="properties">Properties to iterate over to build property path list</param>
        /// <param name="nonKeyPropertyPaths">Returns as an out parameter the list of non key properties that are not collections</param>
        /// <param name="dynamicPropertyPathList">Returns as an out parameter this list of collection properties in the entitytype</param>
        private void BuildPropertyPaths(string propertyPath, IEnumerable<MemberProperty> properties, out string[] nonKeyPropertyPaths, out string[] dynamicPropertyPathList)
        {
            var nonKeyPropertyPathList = new List<string>();
            var dynamicPathList = new List<string>();
            foreach (MemberProperty childProperty in properties)
            {
                string newPropertyPath = childProperty.Name;

                if (!string.IsNullOrEmpty(propertyPath))
                {
                    newPropertyPath = propertyPath + "." + childProperty.Name;
                }

                if (childProperty.PropertyType is CollectionDataType)
                {
                    dynamicPathList.Add(newPropertyPath);
                    continue;
                }

                var enumDataType = childProperty.PropertyType as EnumDataType;
                if (enumDataType != null)
                {
                    nonKeyPropertyPathList.Add(newPropertyPath);
                    continue;
                }

                var primitiveDataType = childProperty.PropertyType as PrimitiveDataType;
                if (primitiveDataType != null)
                {
                    nonKeyPropertyPathList.Add(newPropertyPath);
                    continue;
                }

                var childComplexDataType = childProperty.PropertyType as ComplexDataType;
                if (childComplexDataType != null)
                {
                    string[] childNonKeyPropertyPaths;
                    string[] childCollectionPropertyPaths;
                    if (childComplexDataType.IsNullable)
                    {
                        dynamicPathList.Add(newPropertyPath);
                    }
                    else
                    {
                        this.BuildPropertyPaths(newPropertyPath, childComplexDataType.Definition.Properties, out childNonKeyPropertyPaths, out childCollectionPropertyPaths);
                        nonKeyPropertyPathList.AddRange(childNonKeyPropertyPaths);
                        dynamicPathList.AddRange(childCollectionPropertyPaths);
                    }
                }
            }

            nonKeyPropertyPaths = nonKeyPropertyPathList.ToArray();
            dynamicPropertyPathList = dynamicPathList.ToArray();
        }

        private void ImportFrom(object item, string currentPath, ComplexType complexType)
        {
            if (null == item)
            {
                this.SetValue(currentPath, item);
            }
            else
            {
                foreach (PropertyInfo propertyInfo in item.GetType().GetProperties())
                {
                    MemberProperty mp = complexType.Properties.Where(p => p.Name == propertyInfo.Name).SingleOrDefault();
                    if (mp == null)
                    {
                        throw new TaupoInvalidOperationException(
                            string.Format(CultureInfo.InvariantCulture, "Complex type '{0}' does not contain property '{1}'.", complexType.FullName, propertyInfo.Name));
                    }

                    this.SetDataFromPropertyInfo(item, propertyInfo, mp, currentPath + ".");
                }
            }
        }

        private void SetDataFromPropertyInfo(object item, PropertyInfo propertyInfo, MemberProperty memberProperty, string currentPath)
        {
            object propertyValue = propertyInfo.GetValue(item, null);
            if (memberProperty.PropertyType is PrimitiveDataType || memberProperty.PropertyType is EnumDataType)
            {
                this[currentPath + memberProperty.Name] = propertyValue;
            }
            else if (memberProperty.PropertyType is ComplexDataType)
            {
                this.ImportFrom(propertyValue, currentPath + memberProperty.Name, (memberProperty.PropertyType as ComplexDataType).Definition);
            }
            else
            {
                var collectionDataType = memberProperty.PropertyType as CollectionDataType;
                ExceptionUtilities.Assert(collectionDataType != null, "Only properties with primitive, complex, or collection of primitive and complex data types are supported. Property: '{0}', type: '{1}'.", memberProperty.Name, memberProperty.PropertyType);

                var primitiveElementDataType = collectionDataType.ElementDataType as PrimitiveDataType;
                var complexElementDataType = collectionDataType.ElementDataType as ComplexDataType;

                ExceptionUtilities.CheckObjectNotNull(propertyValue, "Collection Property MUST not be Null at PropertyPath '{0}'", currentPath + memberProperty.Name);
                var enumerableItem = propertyValue as IEnumerable;
                ExceptionUtilities.CheckObjectNotNull(enumerableItem, "propertyValue should be IEnumerable but is not, its type is '{0};", propertyValue.GetType().Name);
                if (primitiveElementDataType != null)
                {
                    int i = 0;
                    foreach (object o in enumerableItem)
                    {
                        this[currentPath + memberProperty.Name + "." + i] = o;
                        i++;
                    }
                }
                else
                {
                    ExceptionUtilities.Assert(complexElementDataType != null, "ElementType {0} of CollectionType Property is not supported", collectionDataType.ElementDataType);
                    int i = 0;
                    foreach (object o in enumerableItem)
                    {
                        this.ImportFrom(o, currentPath + memberProperty.Name + "." + i, complexElementDataType.Definition);
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// A class stores data of the EntitySetDataRow
        /// </summary>
        private class EntitySetDataRowData
        {
            private readonly string[] dynamicPropertyPaths;
            private readonly Dictionary<string, object> dynamicData;
            private readonly string[] nonKeyPropertyPaths;
            private readonly EntityDataKey key;
            private readonly string fullEntityTypeName;

            internal EntitySetDataRowData(EntityDataKey key, string[] nonKeyPropertyPaths, string[] dynamicPropertyPaths, string fullEntityTypeName)
            {
                this.dynamicData = new Dictionary<string, object>();
                this.dynamicPropertyPaths = dynamicPropertyPaths;
                this.nonKeyPropertyPaths = nonKeyPropertyPaths;
                this.key = key;
                this.fullEntityTypeName = fullEntityTypeName;

                // Initialize all the data for property non collection non key property paths to unitialized
                foreach (string propertyPath in this.nonKeyPropertyPaths)
                {
                    this.dynamicData.Add(propertyPath, UninitializedData.Value);
                }
            }

            /// <summary>
            /// Gets the property paths that include the keys and all the property paths already stored in the internal dictionary
            /// </summary>
            internal IEnumerable<string> PropertyPaths
            {
                get { return this.key.KeyNames.Concat(this.dynamicData.Keys.ToList()); }
            }

            /// <summary>
            /// Gets the value of the property with the specified path.
            /// </summary>
            /// <param name="propertyPath">Path of the property.</param>
            /// <returns>Value of the property.</returns>
            internal object GetValue(string propertyPath)
            {
                if (this.key.KeyNames.Contains(propertyPath))
                {
                    return this.key.GetValue(propertyPath);
                }
                else
                {
                    if (this.dynamicData.ContainsKey(propertyPath))
                    {
                        return this.dynamicData[propertyPath];
                    }
                    else
                    {
                        throw new TaupoArgumentException(string.Format(
                            CultureInfo.InvariantCulture,
                            "Property with the path '{0}' does not exist on entity type '{1}'.",
                            propertyPath,
                            this.fullEntityTypeName));
                    }
                }
            }

            /// <summary>
            /// Sets the value of the property path, will throw a TaupoArgumentException if the path is invalid
            /// </summary>
            /// <param name="propertyPath">Property Path to set the value of</param>
            /// <param name="value">value to set</param>
            internal void SetValue(string propertyPath, object value)
            {
                if (this.key.KeyNames.Contains(propertyPath))
                {
                    this.key.SetValue(propertyPath, value);
                }
                else
                {
                    if (this.dynamicData.ContainsKey(propertyPath))
                    {
                        this.dynamicData[propertyPath] = value;
                    }
                    else
                    {
                        // If the propertyPath does not already exist verify that its a proper CollectionPath or a Complex Type path, throw if it isn't
                        if (this.IsValidDynamicPropertyPath(propertyPath))
                        {
                            this.dynamicData[propertyPath] = value;
                        }
                        else
                        {
                            throw new TaupoArgumentException(string.Format(
                                CultureInfo.InvariantCulture,
                                "Property with the path '{0}' does not exist on entity type '{1}'.",
                                propertyPath,
                                this.fullEntityTypeName));
                        }
                    }
                }
            }

            /// <summary>
            /// Determines if the propertyPath specified is a valid collection property path or not against a previously generated set of collection paths
            /// </summary>
            /// <param name="propertyPath">The path of the property</param>
            /// <returns>True or false of if its a collection propertypath or not</returns>
            private bool IsValidDynamicPropertyPath(string propertyPath)
            {
                // Review the list of collection properties to see if it exists
                List<string> matchingCollectionPaths = this.dynamicPropertyPaths.Where(cp => cp.StartsWith(propertyPath, StringComparison.OrdinalIgnoreCase) || propertyPath.StartsWith(cp, StringComparison.OrdinalIgnoreCase)).ToList();
                if (matchingCollectionPaths.Count() == 0)
                {
                    return false;
                }
                else
                {
                    string[] foundPropertySegments = matchingCollectionPaths.First().Split('.');
                    string nextSegment = foundPropertySegments.First();
                    return this.dynamicPropertyPaths.Any(path => path.StartsWith(nextSegment, StringComparison.OrdinalIgnoreCase));
                }
            }
        }
    }
}
