﻿//---------------------------------------------------------------------
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
            foreach (ClientPropertyAnnotation property in properties)
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

            return odataProperties;
        }

        /// <summary>
        /// Creates and returns an ODataComplexValue from the given value.
        /// </summary>
        /// <param name="complexType">The value type.</param>
        /// <param name="value">The complex value.</param>
        /// <param name="propertyName">If the value is a property, then it represents the name of the property. Can be null, for non-property.</param>
        /// <param name="isCollectionItem">True, if the value is an item in a collection, false otherwise.</param>
        /// <param name="visitedComplexTypeObjects">Set of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>An ODataComplexValue representing the given value.</returns>
        internal ODataComplexValue CreateODataComplexValue(Type complexType, object value, string propertyName, bool isCollectionItem, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(complexType != null, "complexType != null");
            Debug.Assert(value != null || !isCollectionItem, "Collection items must not be null");

            ClientEdmModel model = this.requestInfo.Model;
            ClientTypeAnnotation complexTypeAnnotation = model.GetClientTypeAnnotation(complexType);
            Debug.Assert(complexTypeAnnotation != null, "complexTypeAnnotation != null");
            Debug.Assert(!complexTypeAnnotation.IsEntityType, "Unexpected entity");

            // Handle null values for complex types by putting m:null="true"
            if (value == null)
            {
                Debug.Assert(!isCollectionItem, "Null collection items are not supported. Should have already been checked.");
                return null;
            }

            if (visitedComplexTypeObjects == null)
            {
                visitedComplexTypeObjects = new HashSet<object>(ReferenceEqualityComparer<object>.Instance);
            }
            else if (visitedComplexTypeObjects.Contains(value))
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

            visitedComplexTypeObjects.Add(value);
            ODataComplexValue odataComplexValue = new ODataComplexValue();

            odataComplexValue.TypeName = complexTypeAnnotation.ElementTypeName;

            string serverTypeName = this.requestInfo.GetServerTypeName(complexTypeAnnotation);
            odataComplexValue.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = serverTypeName });

            odataComplexValue.Properties = this.PopulateProperties(value, serverTypeName, complexTypeAnnotation.PropertiesToSerialize(), visitedComplexTypeObjects);

            visitedComplexTypeObjects.Remove(value);
            return odataComplexValue;
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
            Debug.Assert(value != null, "value != null");

            ClientEdmModel model = this.requestInfo.Model;
            ClientTypeAnnotation entityTypeAnnotation = model.GetClientTypeAnnotation(value.GetType());
            Debug.Assert(entityTypeAnnotation != null, "entityTypeAnnotation != null");
            Debug.Assert(entityTypeAnnotation.IsStructuredType, "Unexpected type");

            ODataResource odataEntityValue = new ODataResource()
            {
                TypeName = entityTypeAnnotation.ElementTypeName,
            };

            string serverTypeName = this.requestInfo.GetServerTypeName(entityTypeAnnotation);
            odataEntityValue.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = serverTypeName });

            odataEntityValue.Properties = this.PopulateProperties(value, serverTypeName, properties.Any() ? properties : entityTypeAnnotation.PropertiesToSerialize(), null);

            return odataEntityValue;
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
                bool areEnumItems = collectionItemTypeTmp.IsEnum();

                // Note that the collectionItemTypeName will be null if the context does not have the ResolveName func.
                collectionItemTypeName = this.requestInfo.ResolveNameFromType(collectionItemType);

                if (enumerablePropertyValue != null)
                {
                    collection.Items = Util.GetEnumerable(
                        enumerablePropertyValue,
                        (val) =>
                        {
                            if (areEnumItems)
                            {
                                if (val == null)
                                {
                                    return new ODataEnumValue(null, collectionItemType.FullName) as ODataValue;
                                }

                                return new ODataEnumValue(ClientTypeUtil.GetEnumValuesString(val.ToString(), collectionItemTypeTmp), collectionItemType.FullName) as ODataValue;
                            }
                            else
                            {
                                if (val == null)
                                {
                                    return null;
                                }

                                WebUtil.ValidateComplexCollectionItem(val, propertyName, collectionItemType);
                                return this.CreateODataComplexValue(val.GetType(), val, propertyName, true /*isCollectionItem*/, visitedComplexTypeObjects)
                                     as ODataValue;
                            }
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
                collection.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = wireTypeName });
            }

            return collection;
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
#if !PORTABLELIB
            else if (propertyType.FullName == "System.Data.Linq.Binary")
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
            foreach (ClientPropertyAnnotation property in properties)
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

            if (!property.IsEntityCollection && !ClientTypeUtil.TypeIsEntity(property.PropertyType, this.requestInfo.Model))
            {
                odataValue = this.CreateODataComplexPropertyValue(property, propertyValue, visitedComplexTypeObjects);
                return true;
            }

            odataValue = null;
            return false;
        }

        /// <summary>
        /// Returns the value of the complex property.
        /// </summary>
        /// <param name="property">Property which contains name, type, is key (if false and null value, will throw).</param>
        /// <param name="propertyValue">property value</param>
        /// <param name="visitedComplexTypeObjects">List of instances of complex types encountered in the hierarchy. Used to detect cycles.</param>
        /// <returns>An instance of ODataComplexValue containing the value of the properties of the given complex type.</returns>
        private ODataComplexValue CreateODataComplexPropertyValue(ClientPropertyAnnotation property, object propertyValue, HashSet<object> visitedComplexTypeObjects)
        {
            Debug.Assert(propertyValue != null || !property.IsPrimitiveOrEnumOrComplexCollection, "Collection items must not be null");

            Type propertyType = property.IsPrimitiveOrEnumOrComplexCollection ? property.PrimitiveOrComplexCollectionItemType : property.PropertyType;
            if (propertyValue != null && propertyType != propertyValue.GetType())
            {
                Debug.Assert(propertyType.IsAssignableFrom(propertyValue.GetType()), "Type from value should equals to or derived from property type from model.");
                propertyType = propertyValue.GetType();
            }

            return this.CreateODataComplexValue(propertyType, propertyValue, property.PropertyName, property.IsPrimitiveOrEnumOrComplexCollection, visitedComplexTypeObjects);
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
                primitiveValue.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = property.EdmProperty.Type.FullName() });
            }
        }
    }
}
