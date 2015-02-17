//---------------------------------------------------------------------
// <copyright file="ODataPayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Default implementation of the odata payload builder contract
    /// </summary>
    [ImplementationName(typeof(IODataPayloadBuilder), "Default")]
    public class ODataPayloadBuilder : IODataPayloadBuilder
    {
        /// <summary>
        /// Gets or sets the primitive data type converter to use
        /// </summary>
        [InjectDependency]
        public IClrToPrimitiveDataTypeConverter PrimitiveTypeConverter { get; set; }

        /// <summary>
        /// Builds a complex type instance of the given type out of the given anonymous object
        /// </summary>
        /// <param name="complexType">The metadata type information for the complex type</param>
        /// <param name="anonymous">The data as an anonymous type</param>
        /// <returns>An entity instance with the given values</returns>
        public ComplexInstance ComplexInstance(ComplexType complexType, object anonymous)
        {
            ComplexInstance instance = new ComplexInstance(complexType.FullName, anonymous == null);
            if (anonymous != null)
            {
                this.PopulatePropertiesFromObject(instance, complexType.Properties, anonymous);
            }

            return instance;
        }

        /// <summary>
        /// Builds an entity instance of the given type out of the given anonymous object
        /// </summary>
        /// <param name="entityType">The metadata type information for the entity</param>
        /// <param name="anonymous">The data as an anonymous type</param>
        /// <returns>An entity instance with the given values</returns>
        public EntityInstance EntityInstance(EntityType entityType, object anonymous)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            EntityInstance instance = new EntityInstance(entityType.FullName, anonymous == null);

            if (entityType.GetBaseTypesAndSelf().Any(t => t.HasStream()))
            {
                instance.AsMediaLinkEntry();
            }

            // TODO: id?
            if (anonymous != null)
            {
                this.PopulatePropertiesFromObject(instance, entityType.AllProperties, anonymous);

                // TODO: populate navigation properties
            }

            return instance;
        }

        /// <summary>
        /// Constructs an entity with the given property values
        /// </summary>
        /// <param name="entityType">The metadata for the entity type</param>
        /// <param name="namedValues">The property values. Keys are expected to be '.' delimited property paths.</param>
        /// <returns>An entity instance with the given values</returns>
        public EntityInstance EntityInstance(EntityType entityType, IEnumerable<NamedValue> namedValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            EntityInstance instance = new EntityInstance(entityType.FullName, namedValues == null);

            if (entityType.GetBaseTypesAndSelf().Any(t => t.HasStream()))
            {
                instance.AsMediaLinkEntry();
            }

            if (namedValues != null)
            {
                this.PopulatePropertiesFromPaths(instance, entityType.AllProperties, null, namedValues);

                // TODO: populate navigation properties
            }

            return instance;
        }

        /// <summary>
        /// Builds a primitive property instance for the given metadata property with the given value
        /// </summary>
        /// <param name="memberProperty">The metadata for the property</param>
        /// <param name="value">The property value</param>
        /// <returns>A primitive property instance</returns>
        public PrimitiveProperty PrimitiveProperty(MemberProperty memberProperty, object value)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberProperty, "memberProperty");
            
            if (value != null)
            {
                ExceptionUtilities.Assert(!value.GetType().Equals(typeof(NamedValue)), "PrimitiveProperty value cannot be a NamedValue");
            }

            PrimitiveDataType primitiveType = memberProperty.PropertyType as PrimitiveDataType;
            ExceptionUtilities.CheckObjectNotNull(primitiveType, "Property '{0}' was not primitive", memberProperty.Name);

            return new PrimitiveProperty(memberProperty.Name, PrimitiveValue(primitiveType, value));
        }

        /// <summary>
        /// Builds a complex property instance for the given metadata property with the given anonymous type value
        /// </summary>
        /// <param name="memberProperty">The metadata for the property</param>
        /// <param name="anonymous">An anonymous type describing the property values of the complex instance</param>
        /// <returns>A complex property instance</returns>
        public ComplexProperty ComplexProperty(MemberProperty memberProperty, object anonymous)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberProperty, "memberProperty");

            ComplexDataType complexType = memberProperty.PropertyType as ComplexDataType;
            ExceptionUtilities.CheckObjectNotNull(complexType, "Property '{0}' was not complex", memberProperty.Name);

            return new ComplexProperty(memberProperty.Name, this.ComplexInstance(complexType.Definition, anonymous));
        }

         /// <summary>
        /// Builds a complex property instance for the given metadata property with the given flattened values
        /// </summary>
        /// <param name="memberProperty">The metadata for the property</param>
        /// <param name="namedValues">The flattened values</param>
        /// <returns>A complex property instance</returns>
        public ComplexProperty ComplexProperty(MemberProperty memberProperty, IEnumerable<NamedValue> namedValues)
        {
            return ComplexProperty(memberProperty, null, namedValues);
        }

        /// <summary>
        /// Builds a collection property instance for the given metadata property with the given anonymous type value
        /// </summary>
        /// <param name="memberProperty">The metadata for the property</param>
        /// <param name="anonymous">An anonymous type contianing the property values of the collection</param>
        /// <returns>A complex property instance</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public PropertyInstance MultiValueProperty(MemberProperty memberProperty, object anonymous)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberProperty, "memberProperty");

            var collectionType = memberProperty.PropertyType as CollectionDataType;
            ExceptionUtilities.CheckObjectNotNull(collectionType, "Property '{0}' was not a collection");

            var enumerable = anonymous as IEnumerable;
            ExceptionUtilities.CheckObjectNotNull(enumerable, "Value for property '{0}' was not enumerable");

            var primitiveType = collectionType.ElementDataType as PrimitiveDataType;
            if (primitiveType != null)
            {
                var primitiveCollection = new PrimitiveMultiValueProperty(memberProperty.Name, new PrimitiveMultiValue(primitiveType.BuildMultiValueTypeName(), false));
                foreach (var thing in enumerable)
                {
                    primitiveCollection.Value.Add(this.PrimitiveProperty(new MemberProperty("temp", primitiveType), thing).Value);
                }

                return primitiveCollection;
            }
            else
            {
                var complexType = collectionType.ElementDataType as ComplexDataType;
                ExceptionUtilities.CheckObjectNotNull(complexType, "Collection property '{0}' did not have a primitive or complex collection type", memberProperty.Name);

                var complexCollection = new ComplexMultiValueProperty(memberProperty.Name, new ComplexMultiValue(complexType.BuildMultiValueTypeName(), false));
                foreach (var thing in enumerable)
                {
                    complexCollection.Value.Add(this.ComplexInstance(complexType.Definition, thing));
                }

                return complexCollection;
            }
        }

        /// <summary>
        /// Builds a property instance for the given value without a metadata representation
        /// Cannot infer type names on complex properties
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="valueType">The clr type of the property's value</param>
        /// <param name="value">The value of the property</param>
        /// <returns>A property instance</returns>
        public PropertyInstance DynamicProperty(string name, Type valueType, object value)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            ExceptionUtilities.CheckObjectNotNull(this.PrimitiveTypeConverter, "Cannot build dynamic property without clr to EDM type converter");

            var dataType = this.PrimitiveTypeConverter.ToDataType(valueType);
            if (dataType != null)
            {
                return new PrimitiveProperty(name, PrimitiveValue(dataType, value));
            }

            ComplexProperty complex = new ComplexProperty();
            complex.Name = name;
            complex.Value.IsNull = value == null;
            
            this.PopulatePropertiesFromObject(complex.Value, new MemberProperty[0], value);
            
            return complex;
        }

        /// <summary>
        /// Constructs a complex instance with the given property values
        /// </summary>
        /// <param name="complexType">The metadata for the complex type</param>
        /// <param name="namedValues">The property values. Keys are expected to be '.' delimited property paths.</param>
        /// <returns>A complex instance with the given values</returns>
        public ComplexInstance ComplexInstance(ComplexType complexType, IEnumerable<NamedValue> namedValues)
        {
            return this.ComplexInstance(complexType, null, namedValues);
        }

        /// <summary>
        /// Builds an empty or null MultiValue property for the ODataPayloadElement
        /// </summary>
        /// <param name="memberProperty">Property to build a null or empty value for</param>
        /// <param name="value">Value MUST be null or EmptyData.Value</param>
        /// <returns>A PropertyInstance ODataPayloadElement</returns>        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public PropertyInstance MultiValuePropertyEmptyOrNull(MemberProperty memberProperty, object value)
        {
            var bagPropertyType = memberProperty.PropertyType as CollectionDataType;
            ExceptionUtilities.CheckObjectNotNull(bagPropertyType, "Expected property to be a BagProperty instead its a '{0}'", memberProperty.PropertyType);
            
            DataType elementDataType = bagPropertyType.ElementDataType;
            if (value == null)
            {
                return new NullPropertyInstance(memberProperty.Name, elementDataType.BuildMultiValueTypeName());
            }
            else
            {
                ExceptionUtilities.Assert(value == EmptyData.Value, "value MUST be null or EmptyData.Value");
                PropertyInstance collectionProperty = null;
                if (elementDataType is ComplexDataType)
                {
                    collectionProperty = new ComplexMultiValueProperty(memberProperty.Name, new ComplexMultiValue(elementDataType.BuildMultiValueTypeName(), false));
                }
                else
                {
                    ExceptionUtilities.Assert(elementDataType is PrimitiveDataType, "DataType is not a PrimitiveDataType '{0}'", elementDataType);
                    collectionProperty = new PrimitiveMultiValueProperty(memberProperty.Name, new PrimitiveMultiValue(elementDataType.BuildMultiValueTypeName(), false));
                }

                // Add an empty one if specified to
                return collectionProperty;
            }
        }

        private static PrimitiveValue PrimitiveValue(PrimitiveDataType dataType, object value)
        {
            // leave behind the metadata as an annotation so other components can use it
            return new PrimitiveValue(dataType.GetEdmTypeName(), value).WithAnnotations(new DataTypeAnnotation() { DataType = dataType });
        }

        /// <summary>
        /// Builds a complex property instance for the given metadata property with the given flattened values
        /// </summary>
        /// <param name="property">The metadata for the property</param>
        /// <param name="propertyPath">Property Path to start on for the memberProperty</param>
        /// <param name="namedValues">The flattened values</param>
        /// <returns>A complex property instance</returns>
        private ComplexProperty ComplexProperty(MemberProperty property, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            ComplexDataType complexType = property.PropertyType as ComplexDataType;
            ExceptionUtilities.CheckObjectNotNull(complexType, "Property '{0}' was not complex", property.Name);

            return new ComplexProperty(property.Name, this.ComplexInstance(complexType.Definition, propertyPath, namedValues));
        }

        /// <summary>
        /// Constructs a complex instance with the given property values
        /// </summary>
        /// <param name="type">The metadata for the complex type</param>
        /// <param name="propertyPath">Property Path to the ComplexInstance</param>
        /// <param name="namedValues">The property values. Keys are expected to be '.' delimited property paths.</param>
        /// <returns>A complex instance with the given values</returns>
        private ComplexInstance ComplexInstance(ComplexType type, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            ComplexInstance instance = new ComplexInstance(type.FullName, namedValues == null);
            if (namedValues != null)
            {
                this.PopulatePropertiesFromPaths(instance, type.Properties, propertyPath, namedValues);
            }

            return instance;
        }

        private void PopulatePropertiesFromObject(ComplexInstance instance, IEnumerable<MemberProperty> properties, object anonymous)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");
            ExceptionUtilities.CheckArgumentNotNull(anonymous, "anonymous");

            Type anonymousType = anonymous.GetType();
            foreach (PropertyInfo info in anonymousType.GetProperties())
            {
                object value = info.GetValue(anonymous, null);

                MemberProperty property = properties.SingleOrDefault(p => p.Name == info.Name);
                if (property == null)
                {
                    instance.Add(this.DynamicProperty(info.Name, info.PropertyType, value));
                }
                else if (property.PropertyType is ComplexDataType)
                {
                    instance.Add(this.ComplexProperty(property, value));
                }
                else if (property.PropertyType is CollectionDataType)
                {
                    instance.Add(this.MultiValueProperty(property, value));
                }
                else
                {
                    instance.Add(this.PrimitiveProperty(property, value));
                }
            }
        }

        private void PopulateMultiValuePropertyFromPaths(ComplexInstance instance, MemberProperty memberProperty, DataType elementType, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            PrimitiveDataType primitiveElementDataType = elementType as PrimitiveDataType;
            ComplexDataType complexTypeElementDataType = elementType as ComplexDataType;

            if (primitiveElementDataType != null)
            {
                this.PopulatePrimitiveBagPropertyFromPaths(instance, memberProperty, propertyPath, namedValues, primitiveElementDataType);
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(complexTypeElementDataType, "PropertyPath '{0}' is an invalid type '{1}'", propertyPath, memberProperty.PropertyType);

                this.PopulateComplexMultiValuePropertyFromPaths(instance, memberProperty, propertyPath, namedValues, complexTypeElementDataType);
            }
        }

        private void PopulateComplexMultiValuePropertyFromPaths(ComplexInstance instance, MemberProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues, ComplexDataType complexTypeElementDataType)
        {
            int i = 0;
            bool completed = false;

            var complexCollection = new ComplexMultiValueProperty(memberProperty.Name, new ComplexMultiValue(complexTypeElementDataType.BuildMultiValueTypeName(), false));
            while (!completed)
            {
                IEnumerable<NamedValue> complexInstanceNamedValues = namedValues.Where(pp => pp.Name.StartsWith(propertyPath + "." + i + ".", StringComparison.Ordinal)).ToList();
                if (complexInstanceNamedValues.Count() == 0)
                {
                    completed = true;
                }
                else
                {
                    ComplexInstance complexInstance = this.ComplexInstance(complexTypeElementDataType.Definition, propertyPath + "." + i, complexInstanceNamedValues);
                    complexCollection.Value.Add(complexInstance);
                }

                i++;
            }

            if (i > 1)
            {
                instance.Add(complexCollection);
            }
        }

        private void PopulatePrimitiveBagPropertyFromPaths(ComplexInstance instance, MemberProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues, PrimitiveDataType primitiveElementDataType)
        {
            int i = 0;
            bool completed = false;

            var primitiveCollection = new PrimitiveMultiValue(primitiveElementDataType.BuildMultiValueTypeName(), false);
            while (!completed)
            {
                IEnumerable<NamedValue> primitiveItemNamedValues = namedValues.Where(pp => pp.Name == propertyPath + "." + i).ToList();
                if (primitiveItemNamedValues.Count() == 0)
                {
                    completed = true;
                }
                else
                {
                    ExceptionUtilities.Assert(primitiveItemNamedValues.Count() < 2, "Should not get more than one value for a primitive Bag item for path '{0}'", propertyPath + "." + i);
                    var value = primitiveItemNamedValues.Single();

                    // Do something with the value
                    primitiveCollection.Add(PrimitiveValue(primitiveElementDataType, value.Value));
                }

                i++;
            }

            if (i > 1)
            {
                instance.Add(new PrimitiveMultiValueProperty(memberProperty.Name, primitiveCollection));
            }
        }

        private void PopulateCollectionPropertyWithNullOrEmpty(ComplexInstance instance, MemberProperty memberProperty, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            IEnumerable<NamedValue> exactMatches = namedValues.Where(nv => nv.Name == propertyPath).ToList();
            ExceptionUtilities.Assert(exactMatches.Count() < 2, "Should only find at most one property path {0} when looking for null value", propertyPath);
            if (exactMatches.Count() == 1)
            {
                instance.Add(this.MultiValuePropertyEmptyOrNull(memberProperty, exactMatches.Single().Value));
            }
        }

        private void PopulatePropertiesFromPaths(ComplexInstance instance, IEnumerable<MemberProperty> properties, string propertyPath, IEnumerable<NamedValue> namedValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");
            ExceptionUtilities.CheckArgumentNotNull(namedValues, "namedValues");

            foreach (MemberProperty property in properties)
            {
                string childPropertyPath = property.Name;
                if (propertyPath != null)
                {
                    childPropertyPath = propertyPath + "." + property.Name;
                }

                CollectionDataType collectionDataType = property.PropertyType as CollectionDataType;
                PrimitiveDataType primitiveDataType = property.PropertyType as PrimitiveDataType;
                if (primitiveDataType != null)
                {
                    NamedValue memberPropertyNamedValue = namedValues.SingleOrDefault(nv => nv.Name == childPropertyPath);
                    if (memberPropertyNamedValue != null)
                    {
                        instance.Add(this.PrimitiveProperty(property, memberPropertyNamedValue.Value));
                    }
                }
                else if (collectionDataType != null)
                {
                    IEnumerable<NamedValue> bagNamedValues = namedValues.Where(nv => nv.Name.StartsWith(childPropertyPath + ".", StringComparison.Ordinal)).ToList();
                    if (bagNamedValues.Count() > 0)
                    {
                        this.PopulateMultiValuePropertyFromPaths(instance, property, collectionDataType.ElementDataType, childPropertyPath, bagNamedValues);
                    }
                    else
                    {
                        this.PopulateCollectionPropertyWithNullOrEmpty(instance, property, childPropertyPath, namedValues);
                    }
                }
                else
                {
                    var complexDataType = property.PropertyType as ComplexDataType;
                    ExceptionUtilities.CheckObjectNotNull(complexDataType, "Property '{0}' was not primitive, a collection, or complex", property.Name);

                    IEnumerable<NamedValue> complexInstanceNamedValues = namedValues.Where(nv => nv.Name.StartsWith(childPropertyPath + ".", StringComparison.Ordinal)).ToList();
                    if (complexInstanceNamedValues.Count() > 0)
                    {
                        PropertyInstance memberPropertyInstance = this.ComplexProperty(property, childPropertyPath, complexInstanceNamedValues);
                        instance.Add(memberPropertyInstance);
                    }
                    else
                    {
                        // Check for null case
                        IEnumerable<NamedValue> exactMatches = namedValues.Where(nv => nv.Name == childPropertyPath).ToList();
                        ExceptionUtilities.Assert(exactMatches.Count() < 2, "Should only find at most one property path {0} when looking for null value", childPropertyPath);
                        if (exactMatches.Count() == 1)
                        {
                            instance.Add(new ComplexProperty(property.Name, new ComplexInstance(complexDataType.Definition.FullName, true)));
                        }
                    }
                }
            }
        }
    }
}
