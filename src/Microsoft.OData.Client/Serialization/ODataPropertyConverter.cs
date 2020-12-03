//---------------------------------------------------------------------
// <copyright file="ODataPropertyConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Json;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Component for converting properties on client types into instance of <see cref="ODataProperty"/> in order to serialize insert/update payloads.
    /// </summary>
    internal class ODataPropertyConverter
    {
        /// <summary>
        /// The request info.
        /// </summary>
        private readonly RequestInfo requestInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataPropertyConverter"/> class.
        /// </summary>
        /// <param name="requestInfo">The request info.</param>
        internal ODataPropertyConverter(RequestInfo requestInfo)
        {
            Debug.Assert(requestInfo != null, "requestInfo != null");
            this.requestInfo = requestInfo;
        }

        /// <summary>
        /// Creates a list of ODataProperty instances for the given set of properties.
        /// </summary>
        /// <param name="resource">Instance of the resource which is getting serialized.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="properties">The properties to populate into instance of ODataProperty.</param>
        /// <returns>Populated ODataProperty instances for the given properties.</returns>
        internal IEnumerable<ODataProperty> PopulateProperties(object resource, string serverTypeName, IEnumerable<ClientPropertyAnnotation> properties)
        {
            Debug.Assert(properties != null, "properties != null");
            List<ODataProperty> odataProperties = new List<ODataProperty>();
            var populatedProperties = properties.Where(p => !p.IsComplex && !p.IsComplexCollection);
            foreach (ClientPropertyAnnotation property in populatedProperties)
            {
                object propertyValue = property.GetValue(resource);
                ODataValue odataValue;
                if (this.TryConvertPropertyValue(property, propertyValue, serverTypeName, null, out odataValue))
                {
                    odataProperties.Add(
                        new ODataProperty
                        {
                            Name = property.PropertyName,
                            Value = odataValue
                        });

                    this.AddTypeAnnotationNotDeclaredOnServer(serverTypeName, property, odataValue);
                }
            }

            // Process non-structured dynamic properties stored in the container property
            if (ClientTypeUtil.IsInstanceOfOpenType(resource, this.requestInfo.Model))
            {
                PopulateDynamicProperties(resource, serverTypeName, odataProperties);
            }

            return odataProperties;
        }

        /// <summary>
        /// Populates list of odata properties with non-structured dynamic properties in the container property
        /// </summary>
        /// <param name="resource">Instance of the resource which is getting serialized.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="odataProperties">List of ODataProperty</param>
        private void PopulateDynamicProperties(object resource, string serverTypeName, List<ODataProperty> odataProperties)
        {
            Debug.Assert(resource != null, "resource !=null");
            Debug.Assert(odataProperties != null, "odataProperties != null");

            IDictionary<string, object> containerProperty;
            if (ClientTypeUtil.TryGetContainerProperty(resource, out containerProperty))
            {
                ClientEdmModel model = this.requestInfo.Model;

                foreach (KeyValuePair<string, object> kvPair in containerProperty)
                {
                    string dynamicPropertyName = kvPair.Key;
                    object dynamicPropertyValue = kvPair.Value;

                    // Based on the spec, a missing dynamic property is defined to be the same as a dynamic property with value null
                    if (dynamicPropertyValue == null)
                    {
                        continue;
                    }

                    Type dynamicPropertyType = dynamicPropertyValue.GetType();
                    Type dynamicPropertyItemType = !(dynamicPropertyValue is ICollection) ? dynamicPropertyType : dynamicPropertyType.GetGenericArguments().Single();

                    // Do not add any property if a declared property with a matching name exists on the type
                    // Handle only non-structured dynamic properties
                    if (!odataProperties.Any(el => el.Name.Equals(dynamicPropertyName, StringComparison.Ordinal))
                        && !ClientTypeUtil.TypeIsStructured(dynamicPropertyItemType, model))
                    {
                        ODataValue odataValue;
                        if (this.TryConvertDynamicPropertyValue(dynamicPropertyType, dynamicPropertyName, dynamicPropertyValue, serverTypeName, out odataValue))
                        {
                            odataProperties.Add(
                                new ODataProperty
                                {
                                    Name = dynamicPropertyName,
                                    Value = odataValue
                                });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates and returns an ODataResourceWrapper from the given value.
        /// </summary>
        /// <param name="complexType">The value type.</param>
        /// <param name="instance">The complex instance.</param>
        /// <param name="propertyName">If the value is a property, then it represents the name of the property. Can be null, for non-property.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>An ODataResourceWrapper representing the given value.</returns>
        internal ODataResourceWrapper CreateODataResourceWrapperForComplex(Type complexType, object instance, string propertyName, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(complexType != null, "complexType != null");

            ClientEdmModel model = this.requestInfo.Model;
            ClientTypeAnnotation complexTypeAnnotation = model.GetClientTypeAnnotation(complexType);
            Debug.Assert(complexTypeAnnotation != null, "complexTypeAnnotation != null");
            Debug.Assert(!complexTypeAnnotation.IsEntityType, "Unexpected entity");

            if (instance == null)
            {
                return new ODataResourceWrapper() { Resource = null };
            }

            if (visitedComplexTypeObjects == null)
            {
                visitedComplexTypeObjects = new HashSet<object>(ReferenceEqualityComparer<object>.Instance);
            }
            else if (visitedComplexTypeObjects.Contains(instance))
            {
                if (propertyName != null)
                {
                    throw Error.InvalidOperation(Strings.Serializer_LoopsNotAllowedInComplexTypes(propertyName));
                }
                else
                {
                    Debug.Assert(complexTypeAnnotation.ElementTypeName != null, "complexTypeAnnotation.ElementTypeName != null");
                    throw Error.InvalidOperation(Strings.Serializer_LoopsNotAllowedInNonPropertyComplexTypes(complexTypeAnnotation.ElementTypeName));
                }
            }

            visitedComplexTypeObjects.Add(instance);

            ODataResource resource = new ODataResource() { TypeName = complexTypeAnnotation.ElementTypeName };

            string serverTypeName = this.requestInfo.GetServerTypeName(complexTypeAnnotation);
            resource.TypeAnnotation = new ODataTypeAnnotation(serverTypeName);

            resource.Properties = this.PopulateProperties(instance, serverTypeName, complexTypeAnnotation.PropertiesToSerialize(), visitedComplexTypeObjects);

            var wrapper = new ODataResourceWrapper() { Resource = resource, Instance = instance };

            wrapper.NestedResourceInfoWrappers = this.PopulateNestedComplexProperties(instance, serverTypeName, complexTypeAnnotation.PropertiesToSerialize(), visitedComplexTypeObjects);

            visitedComplexTypeObjects.Remove(instance);

            return wrapper;
        }

        /// <summary>
        /// Creates a list of ODataNestedResourceInfoWrapper instances for the given set of properties.
        /// </summary>
        /// <param name="resource">Instance of the resource which is getting serialized.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="properties">The properties to populate into instance of ODataProperty.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>Populated ODataNestedResourceInfoWrapper instances for the given properties.</returns>
        internal IEnumerable<ODataNestedResourceInfoWrapper> PopulateNestedComplexProperties(object resource, string serverTypeName, IEnumerable<ClientPropertyAnnotation> properties, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(properties != null, "properties != null");

            List<ODataNestedResourceInfoWrapper> odataNestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>();
            var populatedProperties = properties.Where(p => p.IsComplex || p.IsComplexCollection);

            foreach (ClientPropertyAnnotation property in populatedProperties)
            {
                object propertyValue = property.GetValue(resource);
                ODataItemWrapper odataItem;
                if (this.TryConvertPropertyToResourceOrResourceSet(property, propertyValue, serverTypeName, visitedComplexTypeObjects, out odataItem))
                {
                    odataNestedResourceInfoWrappers.Add(new ODataNestedResourceInfoWrapper
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = property.PropertyName,
                            IsCollection = property.IsComplexCollection
                        },
                        NestedResourceOrResourceSet = odataItem
                    });
                }
            }

            // Process complex dynamic properties stored in the container property
            if (ClientTypeUtil.IsInstanceOfOpenType(resource, this.requestInfo.Model))
            {
                PopulateNestedComplexDynamicProperties(resource, serverTypeName, visitedComplexTypeObjects, odataNestedResourceInfoWrappers);
            }

            return odataNestedResourceInfoWrappers;
        }

        /// <summary>
        /// Populates list of odata properties with complex dynamic properties in the container property
        /// </summary>
        /// <param name="resource">Instance of the resource which is getting serialized.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="odataNestedResourceInfoWrappers">List of ODataNestedResourceInfoWrapper</param>
        private void PopulateNestedComplexDynamicProperties(object resource, string serverTypeName, HashSet<object> visitedComplexTypeObjects, List<ODataNestedResourceInfoWrapper> odataNestedResourceInfoWrappers)
        {
            Debug.Assert(resource != null, "resource !=null");
            Debug.Assert(odataNestedResourceInfoWrappers != null, "odataNestedResourceInfoWrappers != null");

            IDictionary<string, object> containerProperty;
            if (ClientTypeUtil.TryGetContainerProperty(resource, out containerProperty))
            {
                ClientEdmModel model = this.requestInfo.Model;

                foreach (KeyValuePair<string, object> kvPair in containerProperty)
                {
                    string dynamicPropertyName = kvPair.Key;
                    object dynamicPropertyValue = kvPair.Value;

                    // Based on the spec, a missing dynamic property is defined to be the same as a dynamic property with value null
                    if (dynamicPropertyValue == null)
                    {
                        continue;
                    }

                    Type dynamicPropertyType = dynamicPropertyValue.GetType();
                    bool isCollection = dynamicPropertyValue is ICollection;
                    Type dynamicPropertyItemType = !isCollection ? dynamicPropertyType : dynamicPropertyType.GetGenericArguments().Single();

                    // Do not add any property if a declared property with a matching name exists on the type
                    // Handle only complex types
                    if (!odataNestedResourceInfoWrappers.Any(el => el.NestedResourceInfo.Name.Equals(dynamicPropertyName, StringComparison.Ordinal))
                        && ClientTypeUtil.TypeIsComplex(dynamicPropertyItemType, model))
                    {
                        ODataItemWrapper odataItem = this.ConvertDynamicPropertyToResourceOrResourceSet(dynamicPropertyName, dynamicPropertyValue, serverTypeName, visitedComplexTypeObjects);

                        odataNestedResourceInfoWrappers.Add(new ODataNestedResourceInfoWrapper
                        {
                            NestedResourceInfo = new ODataNestedResourceInfo()
                            {
                                Name = dynamicPropertyName,
                                IsCollection = isCollection
                            },
                            NestedResourceOrResourceSet = odataItem
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Creates and returns an ODataResource from the given value.
        /// </summary>
        /// <param name="entityType">The value type.</param>
        /// <param name="value">The entry value.</param>
        /// <param name="properties">The given properties to serialize.</param>
        /// <returns>An ODataResource representing the given value.</returns>
        internal ODataResource CreateODataEntry(Type entityType, object value, params ClientPropertyAnnotation[] properties)
        {
            Debug.Assert(entityType != null, "entityType != null");

            if (value == null)
            {
                return null;
            }

            ClientEdmModel model = this.requestInfo.Model;
            ClientTypeAnnotation entityTypeAnnotation = model.GetClientTypeAnnotation(value.GetType());
            Debug.Assert(entityTypeAnnotation != null, "entityTypeAnnotation != null");
            Debug.Assert(entityTypeAnnotation.IsStructuredType, "Unexpected type");

            ODataResource odataEntityValue = new ODataResource()
            {
                TypeName = entityTypeAnnotation.ElementTypeName,
            };

            string serverTypeName = this.requestInfo.GetServerTypeName(entityTypeAnnotation);
            odataEntityValue.TypeAnnotation = new ODataTypeAnnotation(serverTypeName);

            odataEntityValue.Properties = this.PopulateProperties(value, serverTypeName, properties.Any() ? properties : entityTypeAnnotation.PropertiesToSerialize(), null);

            return odataEntityValue;
        }

        /// <summary>
        /// Creates and returns an ODataResourceWrapper from the given value.
        /// </summary>
        /// <param name="entityType">The value type.</param>
        /// <param name="value">The resource value.</param>
        /// <param name="properties">The given properties to serialize.</param>
        /// <returns>An ODataResourceWrapper representing the given value.</returns>
        internal ODataResourceWrapper CreateODataResourceWrapper(Type entityType, object value, params ClientPropertyAnnotation[] properties)
        {
            Debug.Assert(entityType != null, "entityType != null");

            if (value == null)
            {
                return null;
            }

            ClientEdmModel model = this.requestInfo.Model;
            ClientTypeAnnotation entityTypeAnnotation = model.GetClientTypeAnnotation(value.GetType());
            Debug.Assert(entityTypeAnnotation != null, "entityTypeAnnotation != null");
            Debug.Assert(entityTypeAnnotation.IsStructuredType, "Unexpected type");

            ODataResource odataEntityValue = new ODataResource()
            {
                TypeName = entityTypeAnnotation.ElementTypeName,
            };

            string serverTypeName = this.requestInfo.GetServerTypeName(entityTypeAnnotation);
            odataEntityValue.TypeAnnotation = new ODataTypeAnnotation(serverTypeName);

            odataEntityValue.Properties = this.PopulateProperties(value, serverTypeName, properties.Any() ? properties : entityTypeAnnotation.PropertiesToSerialize(), null);

            var wrapper = new ODataResourceWrapper() { Resource = odataEntityValue };
            wrapper.NestedResourceInfoWrappers = PopulateNestedComplexProperties(value, serverTypeName, properties.Any() ? properties : entityTypeAnnotation.PropertiesToSerialize(), null);

            return wrapper;
        }

        /// <summary>
        /// Creates and returns an ODataResource from the given value.
        /// </summary>
        /// <param name="entityType">The value type.</param>
        /// <param name="value">The entry value.</param>
        /// <returns>An ODataResource representing the given value.</returns>
        internal IEnumerable<ODataResource> CreateODataEntries(Type entityType, object value)
        {
            Debug.Assert(entityType != null, "entityType != null");
            Debug.Assert(value != null, "value != null");

            var list = value as IEnumerable;
            var entries = new List<ODataResource>();
            if (list != null)
            {
                entries.AddRange(from object o in list select this.CreateODataEntry(entityType, o));
            }

            return entries;
        }

        /// <summary>
        /// Converts CLR value into ODataEnumValue.
        /// </summary>
        /// <param name="enumClrType">The CLR type.</param>
        /// <param name="value">The Enum value.</param>
        /// <param name="isCollectionItem">The bool isCollectionItem.</param>
        /// <returns>An ODataEnumValue instance.</returns>
        internal ODataEnumValue CreateODataEnumValue(Type enumClrType, object value, bool isCollectionItem)
        {
            Debug.Assert(enumClrType != null && enumClrType.IsEnum(), "enumClrType != null && enumClrType.IsEnum");
            Debug.Assert(value != null || !isCollectionItem, "Collection items must not be null");

            ClientEdmModel model = this.requestInfo.Model;
            ClientTypeAnnotation enumTypeAnnotation = model.GetClientTypeAnnotation(enumClrType);
            Debug.Assert(enumTypeAnnotation != null, "enumTypeAnnotation != null");
            Debug.Assert(!enumTypeAnnotation.IsEntityType, "Unexpected entity");

            // Handle null value by putting m:null="true"
            if (value == null)
            {
                Debug.Assert(!isCollectionItem, "Null collection items are not supported. Should have already been checked.");
                return null;
            }

            return new ODataEnumValue(ClientTypeUtil.GetEnumValuesString(value.ToString(), enumClrType), enumTypeAnnotation.ElementTypeName);
        }

        /// <summary>
        /// Creates and returns an ODataCollectionValue from the given value.
        /// </summary>
        /// <param name="collectionItemType">The type of the value.</param>
        /// <param name="propertyName">If the value is a property, then it represents the name of the property. Can be null, for non-property.</param>
        /// <param name="value">The value.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <param name="isDynamicProperty">Whether this collection property is a dynamic property</param>
        /// <param name="setTypeAnnotation">If true, set the type annotation on ODataValue.</param>
        /// <returns>An ODataCollectionValue representing the given value.</returns>
        internal ODataCollectionValue CreateODataCollection(Type collectionItemType, string propertyName, object value, HashSet<object> visitedComplexTypeObjects, bool isDynamicProperty, bool setTypeAnnotation = true)
        {
            Debug.Assert(collectionItemType != null, "collectionItemType != null");

            WebUtil.ValidateCollection(collectionItemType, value, propertyName, isDynamicProperty);

            PrimitiveType ptype;
            bool isCollectionOfPrimitiveTypes = PrimitiveType.TryGetPrimitiveType(collectionItemType, out ptype);

            ODataCollectionValue collection = new ODataCollectionValue();
            IEnumerable enumerablePropertyValue = (IEnumerable)value;
            string collectionItemTypeName;
            string collectionTypeName;
            if (isCollectionOfPrimitiveTypes)
            {
                collectionItemTypeName = ClientConvert.GetEdmType(Nullable.GetUnderlyingType(collectionItemType) ?? collectionItemType);

                if (enumerablePropertyValue != null)
                {
                    collection.Items = Util.GetEnumerable(
                        enumerablePropertyValue,
                        (val) =>
                        {
                            if (val == null)
                            {
                                return null;
                            }

                            WebUtil.ValidatePrimitiveCollectionItem(val, propertyName, collectionItemType);
                            return ConvertPrimitiveValueToRecognizedODataType(val, collectionItemType);
                        });
                }

                // TypeName for primitives should be the EDM name since that's what we will be able to look up in the model
                collectionTypeName = collectionItemTypeName;
            }
            else
            {
                Type collectionItemTypeTmp = Nullable.GetUnderlyingType(collectionItemType) ?? collectionItemType;

                // Note that the collectionItemTypeName will be null if the context does not have the ResolveName func.
                collectionItemTypeName = this.requestInfo.ResolveNameFromType(collectionItemType);

                if (enumerablePropertyValue != null)
                {
                    collection.Items = Util.GetEnumerable(
                        enumerablePropertyValue,
                        (val) =>
                        {
                            if (val == null)
                            {
                                return new ODataEnumValue(null, collectionItemType.FullName) as ODataValue;
                            }

                            return new ODataEnumValue(ClientTypeUtil.GetEnumValuesString(val.ToString(), collectionItemTypeTmp), collectionItemType.FullName) as ODataValue;
                        });
                }

                // TypeName for complex types needs to be the client type name (not the one we resolved above) since it will be looked up in the client model
                collectionTypeName = collectionItemType.FullName;
            }

            // Set the type name to use for client type lookups and validation.
            collection.TypeName = GetCollectionName(collectionTypeName);

            // Ideally, we should not set type annotation on collection value.
            // To keep backward compatibility, we'll keep it in request body, but do not include it in url.
            if (setTypeAnnotation)
            {
                string wireTypeName = GetCollectionName(collectionItemTypeName);
                collection.TypeAnnotation = new ODataTypeAnnotation(wireTypeName);
            }

            return collection;
        }

        /// <summary>
        /// Creates and returns an ODataResourceSetWrapper from the given value.
        /// </summary>
        /// <param name="collectionItemType">The type of the value.</param>
        /// <param name="propertyName">If the value is a property, then it represents the name of the property. Can be null, for non-property.</param>
        /// <param name="value">The value.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <param name="isDynamicProperty">Whether this collection property is a dynamic property</param>
        /// <param name="setTypeAnnotation">If true, set the type annotation on ODataValue.</param>
        /// <returns>An ODataResourceSetWrapper representing the given value.</returns>
        internal ODataResourceSetWrapper CreateODataResourceSetWrapperForComplexCollection(Type collectionItemType, string propertyName, object value, HashSet<object> visitedComplexTypeObjects, bool isDynamicProperty, bool setTypeAnnotation = true)
        {
            Debug.Assert(collectionItemType != null, "collectionItemType != null");

            WebUtil.ValidateCollection(collectionItemType, value, propertyName, isDynamicProperty);

            ODataResourceSet resourceSet = new ODataResourceSet();
            ODataResourceSetWrapper wrapper = new ODataResourceSetWrapper()
            {
                ResourceSet = resourceSet
            };

            // Note that the collectionItemTypeName will be null if the context does not have the ResolveName func.
            string collectionItemTypeName = this.requestInfo.ResolveNameFromType(collectionItemType);

            IEnumerable enumerablePropertyValue = (IEnumerable)value;
            if (enumerablePropertyValue != null)
            {
                wrapper.Resources = Util.GetEnumerable(
                    enumerablePropertyValue,
                    (val) =>
                    {
                        if (val == null)
                        {
                            return null;
                        }

                        WebUtil.ValidateComplexCollectionItem(val, propertyName, collectionItemType);
                        var complexResource = this.CreateODataResourceWrapperForComplex(val.GetType(), val, propertyName, visitedComplexTypeObjects);
                        return complexResource;
                    });
            }

            // TypeName for complex types need to be the client type name since it will be looked up in the client model
            // Set the type name to use for client type lookups and validation
            resourceSet.TypeName = GetCollectionName(collectionItemType.FullName);  // Mandatory for a dynamic property

            // Ideally, we should not set type annotation on collection value.
            // To keep backward compatibility, we'll keep it in request body, but do not include it in url.
            if (setTypeAnnotation)
            {
                string wireTypeName = GetCollectionName(collectionItemTypeName);
                resourceSet.TypeAnnotation = new ODataTypeAnnotation(wireTypeName);
            }

            return wrapper;
        }

        /// <summary>
        /// Returns the primitive property value.
        /// </summary>
        /// <param name="propertyValue">Value of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>Returns the value of the primitive property.</returns>
        internal static object ConvertPrimitiveValueToRecognizedODataType(object propertyValue, Type propertyType)
        {
            Debug.Assert(PrimitiveType.IsKnownNullableType(propertyType), "GetPrimitiveValue must be called only for primitive types");
            Debug.Assert(propertyValue == null || PrimitiveType.IsKnownType(propertyValue.GetType()), "GetPrimitiveValue method must be called for primitive values only");

            if (propertyValue == null)
            {
                return null;
            }

            PrimitiveType primitiveType;
            PrimitiveType.TryGetPrimitiveType(propertyType, out primitiveType);
            Debug.Assert(primitiveType != null, "must be a known primitive type");

            // Do the conversion for types that are not supported by ODataLib e.g. char[], char, etc
            if (propertyType == typeof(Char) ||
                propertyType == typeof(Char[]) ||
                propertyType == typeof(Type) ||
                propertyType == typeof(Uri) ||
                propertyType == typeof(System.Xml.Linq.XDocument) ||
                propertyType == typeof(System.Xml.Linq.XElement))
            {
                return primitiveType.TypeConverter.ToString(propertyValue);
            }
            else if (propertyType == typeof(DateTime))
            {
                return PlatformHelper.ConvertDateTimeToDateTimeOffset((DateTime)propertyValue);
            }
#if !PORTABLELIB
            else if (string.Equals(propertyType.FullName, "System.Data.Linq.Binary", StringComparison.Ordinal))
            {
                // For System.Data.Linq.Binary, it is a delay loaded type. Hence checking it based on name.
                // PrimitiveType.IsKnownType checks for binary type based on name and assembly. Hence just
                // checking name here is sufficient, since any other type with the same name, but in different
                // assembly will return false for PrimitiveType.IsKnownNullableType.
                // Since ODataLib does not understand binary type, we need to convert the value to byte[].
                return ((BinaryTypeConverter)primitiveType.TypeConverter).ToArray(propertyValue);
            }
#endif
            else if (primitiveType.EdmTypeName == null)
            {
                // case StorageType.DateTimeOffset:
                // case StorageType.TimeSpan:
                // case StorageType.UInt16:
                // case StorageType.UInt32:
                // case StorageType.UInt64:
                // don't support reverse mappings for these types in this version
                // allows us to add real server support in the future without a
                // "breaking change" in the future client
                throw new NotSupportedException(Strings.ALinq_CantCastToUnsupportedPrimitive(propertyType.Name));
            }

            return propertyValue;
        }

        /// <summary>
        /// Gets the specified type name as an EDM Collection type, e.g. Collection(Edm.String)
        /// </summary>
        /// <param name="itemTypeName">Type name of the items in the collection.</param>
        /// <returns>Collection type name for the specified item type name.</returns>
        private static string GetCollectionName(string itemTypeName)
        {
            return itemTypeName == null ? null : EdmLibraryExtensions.GetCollectionTypeName(itemTypeName);
        }

        /// <summary>
        /// Creates and returns an <see cref="ODataValue"/> for the given primitive value.
        /// </summary>
        /// <param name="property">The property being converted.</param>
        /// <param name="propertyValue">The property value to convert..</param>
        /// <returns>An ODataValue representing the given value.</returns>
        private static ODataValue CreateODataPrimitivePropertyValue(ClientPropertyAnnotation property, object propertyValue)
        {
            if (propertyValue == null)
            {
                return new ODataNullValue();
            }

            propertyValue = ConvertPrimitiveValueToRecognizedODataType(propertyValue, property.PropertyType);

            return new ODataPrimitiveValue(propertyValue);
        }

        /// <summary>
        /// Creates and returns an <see cref="ODataValue"/> for the given primitive value.
        /// </summary>
        /// <param name="propertyType">The type of the property being converted.</param>
        /// <param name="propertyValue">The property value to convert..</param>
        /// <returns>An ODataValue representing the given value.</returns>
        public static ODataValue CreateODataPrimitiveValue(Type propertyType, object propertyValue)
        {
            if (propertyValue == null)
            {
                return new ODataNullValue();
            }

            object convertedValue = ConvertPrimitiveValueToRecognizedODataType(propertyValue, propertyType);

            ODataPrimitiveValue odataPrimitiveValue = new ODataPrimitiveValue(convertedValue);
            return odataPrimitiveValue;
        }

        /// <summary>
        /// Creates a list of ODataProperty instances for the given set of properties.
        /// </summary>
        /// <param name="resource">Instance of the resource which is getting serialized.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="properties">The properties to populate into instance of ODataProperty.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>Populated ODataProperty instances for the given properties.</returns>
        private IEnumerable<ODataProperty> PopulateProperties(object resource, string serverTypeName, IEnumerable<ClientPropertyAnnotation> properties, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(properties != null, "properties != null");
            List<ODataProperty> odataProperties = new List<ODataProperty>();
            var populatedProperties = properties.Where(p => !p.IsComplex && !p.IsComplexCollection);
            foreach (ClientPropertyAnnotation property in populatedProperties)
            {
                object propertyValue = property.GetValue(resource);
                ODataValue odataValue;
                if (this.TryConvertPropertyValue(property, propertyValue, serverTypeName, visitedComplexTypeObjects, out odataValue))
                {
                    odataProperties.Add(new ODataProperty
                    {
                        Name = property.PropertyName,
                        Value = odataValue
                    });
                }
            }

            // Process non-structured dynamic properties stored in the container property
            if (ClientTypeUtil.IsInstanceOfOpenType(resource, this.requestInfo.Model))
            {
                PopulateDynamicProperties(resource, serverTypeName, odataProperties);
            }

            return odataProperties;
        }

        /// <summary>
        /// Tries to convert the given value into an instance of <see cref="ODataValue"/>.
        /// </summary>
        /// <param name="property">The property being converted.</param>
        /// <param name="propertyValue">The property value to convert..</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <param name="odataValue">The odata value if one was created.</param>
        /// <returns>Whether or not the value was converted.</returns>
        private bool TryConvertPropertyValue(ClientPropertyAnnotation property, object propertyValue, string serverTypeName, HashSet<object> visitedComplexTypeObjects, out ODataValue odataValue)
        {
            if (property.IsKnownType)
            {
                odataValue = CreateODataPrimitivePropertyValue(property, propertyValue);
                return true;
            }

            if (property.IsEnumType)
            {
                string enumValue;
                if (propertyValue == null)
                {
                    enumValue = null;
                }
                else
                {
                    enumValue = ClientTypeUtil.GetEnumValuesString(propertyValue.ToString(), property.PropertyType);
                }

                string typeNameInMetadata = this.requestInfo.ResolveNameFromType(property.PropertyType);
                odataValue = new ODataEnumValue(enumValue, typeNameInMetadata);
                return true;
            }

            if (property.IsPrimitiveOrEnumOrComplexCollection)
            {
                odataValue = this.CreateODataCollectionPropertyValue(property, propertyValue, serverTypeName, visitedComplexTypeObjects);
                return true;
            }

            odataValue = null;
            return false;
        }

        /// <summary>
        /// Tries to convert the given value into an instance of <see cref="ODataValue"/>.
        /// </summary>
        /// <param name="property">The type of the entity whose properties are being populated.</param>
        /// <param name="propertyValue">The property value to convert.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="odataValue">The odata value if one was created.</param>
        /// <returns>true if the value was converted.</returns>
        private bool TryConvertDynamicPropertyValue(Type clientType, string dynamicPropertyName, object value, string serverTypeName, out ODataValue odataValue)
        {
            bool shouldWriteClientType = this.requestInfo.TypeResolver.ShouldWriteClientTypeForOpenServerProperty(dynamicPropertyName, serverTypeName);

            if (PrimitiveType.IsKnownType(clientType))
            {
                odataValue = CreateODataPrimitiveValue(clientType, value);
                
                PrimitiveType primitiveType;
                if (value != null 
                    && PrimitiveType.TryGetPrimitiveType(value.GetType(), out primitiveType)
                    && shouldWriteClientType
                    && !JsonSharedUtils.ValueTypeMatchesJsonType((ODataPrimitiveValue)odataValue, primitiveType.PrimitiveKind))
                {
                    odataValue.TypeAnnotation = new ODataTypeAnnotation(primitiveType.EdmTypeName);
                }
                
                return true;
            }

            if (clientType.IsEnum())
            {
                string enumValue = null;
                if (value != null)
                {
                    enumValue = ClientTypeUtil.GetEnumValuesString(value.ToString(), clientType);
                }

                string typeNameInMetadata = this.requestInfo.ResolveNameFromType(clientType);
                string typeName = typeNameInMetadata;

                // If type name not found in metadata but we're required to write client type, assume client and server typeName match
                if (typeNameInMetadata == null && shouldWriteClientType)
                {
                    typeName = clientType.FullName;
                }

                ODataEnumValue odataEnumValue = new ODataEnumValue(enumValue, typeName);
                odataEnumValue.TypeAnnotation = new ODataTypeAnnotation(typeName);
                odataValue = odataEnumValue;
                return true;
            }

            // Primitive or enum collection
            if (value is ICollection)
            {
                Type dynamicPropertyItemType = clientType.GetGenericArguments().Single();
                odataValue = this.CreateODataCollection(dynamicPropertyItemType, dynamicPropertyName, value, null, true);
                return true;
            }

            odataValue = new ODataNullValue();
            return false;
        }

        /// <summary>
        /// Tries to convert the given value into an instance of <see cref="ODataItemWrapper"/>.
        /// </summary>
        /// <param name="property">The property being converted.</param>
        /// <param name="propertyValue">The property value to convert..</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <param name="odataItem">The odata resource or resource set if one was created.</param>
        /// <returns>Whether or not the value was converted.</returns>
        private bool TryConvertPropertyToResourceOrResourceSet(ClientPropertyAnnotation property, object propertyValue, string serverTypeName, HashSet<object> visitedComplexTypeObjects, out ODataItemWrapper odataItem)
        {
            if (property.IsComplexCollection)
            {
                odataItem = this.CreateODataComplexCollectionPropertyResourceSet(property, propertyValue, serverTypeName, visitedComplexTypeObjects);
                return true;
            }

            if (property.IsComplex)
            {
                odataItem = this.CreateODataComplexPropertyResource(property, propertyValue, visitedComplexTypeObjects);
                return true;
            }

            odataItem = null;
            return false;
        }

        /// <summary>
        /// Tries to convert the given value into an instance of <see cref="ODataItemWrapper"/>.
        /// </summary>
        /// <param name="propertyName">Name of the dynamic property.</param>
        /// <param name="propertyValue">The property value to convert.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <param name="odataItem">The odata resource or resource set if one was created.</param>
        /// <returns>Whether or not the value was converted.</returns>
        private ODataItemWrapper ConvertDynamicPropertyToResourceOrResourceSet(string propertyName, object propertyValue, string serverTypeName, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(propertyValue != null, "propertyValue != null");
            Debug.Assert(serverTypeName != null, "serverTypeName != null");

            if (propertyValue is ICollection)
            {
                return this.CreateODataComplexCollectionDynamicPropertyResourceSet(propertyName, propertyValue, serverTypeName, visitedComplexTypeObjects);
            }
            else
            {
                return this.CreateODataComplexDynamicPropertyResouce(propertyName, propertyValue, visitedComplexTypeObjects);
            }
        }

        /// <summary>
        /// Returns the resource of the complex property.
        /// </summary>
        /// <param name="property">Property which contains name, type, is key (if false and null value, will throw).</param>
        /// <param name="propertyValue">property value</param>
        /// <param name="visitedComplexTypeObjects">List of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>An instance of ODataResourceWrapper containing the resource of the properties of the given complex type.</returns>
        private ODataResourceWrapper CreateODataComplexPropertyResource(ClientPropertyAnnotation property, object propertyValue, HashSet<object> visitedComplexTypeObjects)
        {
            Type propertyType = property.IsComplexCollection ? property.PrimitiveOrComplexCollectionItemType : property.PropertyType;
            if (propertyValue != null && propertyType != propertyValue.GetType())
            {
                Debug.Assert(propertyType.IsAssignableFrom(propertyValue.GetType()), "Type from value should equals to or derived from property type from model.");
                propertyType = propertyValue.GetType();
            }

            return this.CreateODataResourceWrapperForComplex(propertyType, propertyValue, property.PropertyName, visitedComplexTypeObjects);
        }

        /// <summary>
        /// Returns the resource of the complex dynamic property.
        /// </summary>
        /// <param name="propertyName">Name of the dynamic property.</param>
        /// <param name="propertyValue">Property value</param>
        /// <param name="visitedComplexTypeObjects">List of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>An instance of ODataResourceWrapper containing the resource of the properties of the given complex type.</returns>
        private ODataResourceWrapper CreateODataComplexDynamicPropertyResouce(string propertyName, object propertyValue, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(propertyValue != null, "propertyValue != null");
            Debug.Assert(!(propertyValue is ICollection), "!(propertyValue is ICollection)");

            Type propertyType = propertyValue.GetType();

            return this.CreateODataResourceWrapperForComplex(propertyType, propertyValue, propertyName, visitedComplexTypeObjects);
        }

        /// <summary>
        /// Returns the value of the collection property.
        /// </summary>
        /// <param name="property">Collection property details. Must not be null.</param>
        /// <param name="propertyValue">Collection instance.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="visitedComplexTypeObjects">List of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>An instance of ODataCollectionValue representing the value of the property.</returns>
        private ODataCollectionValue CreateODataCollectionPropertyValue(ClientPropertyAnnotation property, object propertyValue, string serverTypeName, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(property.IsPrimitiveOrEnumOrComplexCollection, "This method is supposed to be used only for writing collections");
            Debug.Assert(property.PropertyName != null, "property.PropertyName != null");
            bool isDynamic = this.requestInfo.TypeResolver.ShouldWriteClientTypeForOpenServerProperty(property.EdmProperty, serverTypeName);
            return this.CreateODataCollection(property.PrimitiveOrComplexCollectionItemType, property.PropertyName, propertyValue, visitedComplexTypeObjects, isDynamic);
        }

        /// <summary>
        /// Returns the resource set of the collection property.
        /// </summary>
        /// <param name="property">Collection property details. Must not be null.</param>
        /// <param name="propertyValue">Collection instance.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="visitedComplexTypeObjects">List of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>An instance of ODataResourceSetWrapper representing the value of the property.</returns>
        private ODataResourceSetWrapper CreateODataComplexCollectionPropertyResourceSet(ClientPropertyAnnotation property, object propertyValue, string serverTypeName, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(property.IsComplexCollection, "This method is supposed to be used only for writing collections");
            Debug.Assert(property.PropertyName != null, "property.PropertyName != null");
            bool isDynamic = this.requestInfo.TypeResolver.ShouldWriteClientTypeForOpenServerProperty(property.EdmProperty, serverTypeName);
            return this.CreateODataResourceSetWrapperForComplexCollection(property.PrimitiveOrComplexCollectionItemType, property.PropertyName, propertyValue, visitedComplexTypeObjects, isDynamic);
        }

        /// <summary>
        /// Returns the resource set of the collection dynamic property.
        /// </summary>
        /// <param name="propertyName">Name of the collection dynamic property.</param>
        /// <param name="propertyValue">Collection instance.</param>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="visitedComplexTypeObjects">List of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>An instance of ODataResourceSetWrapper representing the value of the property.</returns>
        private ODataResourceSetWrapper CreateODataComplexCollectionDynamicPropertyResourceSet(string propertyName, object propertyValue, string serverTypeName, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(propertyValue != null, "propertyValue != null");
            Debug.Assert(propertyValue is ICollection, "propertyValue must be a collection");

            Type collectionItemType = propertyValue.GetType().GetGenericArguments().Single();
            bool isDynamicProperty = this.requestInfo.TypeResolver.ShouldWriteClientTypeForOpenServerProperty(propertyName, serverTypeName);

            return this.CreateODataResourceSetWrapperForComplexCollection(collectionItemType, propertyName, propertyValue, visitedComplexTypeObjects, isDynamicProperty);
        }

        /// <summary>
        /// Adds a type annotation to the value if it is primitive and not defined on the server.
        /// </summary>
        /// <param name="serverTypeName">The server type name of the entity whose properties are being populated.</param>
        /// <param name="property">The current property.</param>
        /// <param name="odataValue">The already converted value of the property.</param>
        private void AddTypeAnnotationNotDeclaredOnServer(string serverTypeName, ClientPropertyAnnotation property, ODataValue odataValue)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(property.EdmProperty != null, "property.EdmProperty != null");
            var primitiveValue = odataValue as ODataPrimitiveValue;
            if (primitiveValue == null)
            {
                return;
            }

            if (this.requestInfo.TypeResolver.ShouldWriteClientTypeForOpenServerProperty(property.EdmProperty, serverTypeName)
                && !JsonSharedUtils.ValueTypeMatchesJsonType(primitiveValue, property.EdmProperty.Type.AsPrimitive()))
            {
                // DEVNOTE: it is safe to use the property type name for primitive types because they do not generally support inheritance,
                // and spatial values always contain their specific type inside the GeoJSON/GML representation.
                primitiveValue.TypeAnnotation = new ODataTypeAnnotation(property.EdmProperty.Type.FullName());
            }
        }
    }
}
