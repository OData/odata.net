//---------------------------------------------------------------------
// <copyright file="StructuralValueMaterializationPolicy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using DSClient = Microsoft.OData.Client;

    /// <summary>
    /// Contains logic on how to materialize properties into an instance
    /// </summary>
    internal abstract class StructuralValueMaterializationPolicy : MaterializationPolicy
    {
        /// <summary> The collection value materialization policy. </summary>
        private CollectionValueMaterializationPolicy collectionValueMaterializationPolicy;

        /// <summary> The instance annotation materialization policy. </summary>
        private InstanceAnnotationMaterializationPolicy instanceAnnotationMaterializationPolicy;

        /// <summary> The primitive property converter. </summary>
        private DSClient.SimpleLazy<PrimitivePropertyConverter> lazyPrimitivePropertyConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuralValueMaterializationPolicy" /> class.
        /// </summary>
        /// <param name="materializerContext">The materializer context.</param>
        /// <param name="lazyPrimitivePropertyConverter">The lazy primitive property converter.</param>
        protected StructuralValueMaterializationPolicy(IODataMaterializerContext materializerContext, DSClient.SimpleLazy<PrimitivePropertyConverter> lazyPrimitivePropertyConverter)
        {
            Debug.Assert(materializerContext != null, "materializer!=null");
            this.MaterializerContext = materializerContext;
            this.lazyPrimitivePropertyConverter = lazyPrimitivePropertyConverter;
        }

        /// <summary>
        /// Gets the collection value materialization policy.
        /// </summary>
        /// <value>
        /// The collection value materialization policy.
        /// </value>
        protected internal CollectionValueMaterializationPolicy CollectionValueMaterializationPolicy
        {
            get
            {
                Debug.Assert(this.collectionValueMaterializationPolicy != null, "collectionValueMaterializationPolicy != null");
                return this.collectionValueMaterializationPolicy;
            }

            set
            {
                this.collectionValueMaterializationPolicy = value;
            }
        }

        /// <summary>
        /// Gets the instance annotation materialization policy.
        /// </summary>
        /// <value>
        /// The instance annotation materialization policy.
        /// </value>
        protected internal InstanceAnnotationMaterializationPolicy InstanceAnnotationMaterializationPolicy
        {
            get
            {
                Debug.Assert(this.instanceAnnotationMaterializationPolicy != null, "instanceAnnotationMaterializationPolicy != null");
                return this.instanceAnnotationMaterializationPolicy;
            }

            set
            {
                this.instanceAnnotationMaterializationPolicy = value;
            }
        }

        /// <summary>
        /// Gets the primitive property converter.
        /// </summary>
        /// <value>
        /// The primitive property converter.
        /// </value>
        protected PrimitivePropertyConverter PrimitivePropertyConverter
        {
            get
            {
                Debug.Assert(this.lazyPrimitivePropertyConverter != null, "lazyPrimitivePropertyConverter != null");
                return this.lazyPrimitivePropertyConverter.Value;
            }
        }

        /// <summary>
        /// Gets the materializer context.
        /// </summary>
        /// <value>
        /// The materializer context.
        /// </value>
        protected IODataMaterializerContext MaterializerContext { get; private set; }

        /// <summary>Materializes a primitive value. No op for non-primitive values.</summary>
        /// <param name="type">Type of value to set.</param>
        /// <param name="property">Property holding value.</param>
        internal void MaterializePrimitiveDataValue(Type type, ODataProperty property)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(property != null, "atomProperty != null");

            if (!property.HasMaterializedValue())
            {
                object value = property.Value;
                ODataUntypedValue untypedVal = value as ODataUntypedValue;
                if ((untypedVal != null)
                    && this.MaterializerContext.UndeclaredPropertyBehavior == UndeclaredPropertyBehavior.Support)
                {
                    value = CommonUtil.ParseJsonToPrimitiveValue(untypedVal.RawValue);
                }

                object materializedValue = this.PrimitivePropertyConverter.ConvertPrimitiveValue(value, type);
                property.SetMaterializedValue(materializedValue);
            }
        }

        /// <summary>
        /// Applies the values of the specified <paramref name="properties"/> to a given <paramref name="instance"/>.
        /// </summary>
        /// <param name="type">Type to which properties will be applied.</param>
        /// <param name="properties">Properties to assign to the specified <paramref name="instance"/>.</param>
        /// <param name="instance">Instance on which values will be applied.</param>
        internal void ApplyDataValues(ClientTypeAnnotation type, IEnumerable<ODataProperty> properties, object instance)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(properties != null, "properties != null");
            Debug.Assert(instance != null, "instance != context");

            foreach (var p in properties)
            {
                this.ApplyDataValue(type, p, instance);
            }
        }

        /// <summary>Applies a data value to the specified <paramref name="instance"/>.</summary>
        /// <param name="type">Type to which a property value will be applied.</param>
        /// <param name="property">Property with value to apply.</param>
        /// <param name="instance">Instance on which value will be applied.</param>
        internal void ApplyDataValue(ClientTypeAnnotation type, ODataProperty property, object instance)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(instance != null, "instance != null");

            var prop = type.GetProperty(property.Name, this.MaterializerContext.UndeclaredPropertyBehavior);
            if (prop == null)
            {
                return;
            }

            // Is it a collection? (note: property.Properties will be null if the Collection is empty (contains no elements))
            Type enumTypeTmp = null;
            if (prop.IsPrimitiveOrEnumOrComplexCollection)
            {
                // Collections must not be null
                if (property.Value == null)
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.Collection_NullCollectionNotSupported(property.Name));
                }

                // This happens if the payload contain just primitive value for a Collection property
                if (property.Value is string)
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.Deserialize_MixedTextWithComment);
                }

                // ODataLib already parsed the data and materialized all the primitive types. There is nothing more to materialize
                // anymore. Only complex type instance and collection instances need to be materialized, but those will be
                // materialized later on.
                // We need to materialize items before we change collectionInstance since this may throw. If we tried materializing
                // after the Collection is wiped or created we would leave the object in half constructed state.
                object collectionInstance = prop.GetValue(instance);
                if (collectionInstance == null)
                {
                    collectionInstance = this.CollectionValueMaterializationPolicy.CreateCollectionPropertyInstance(property, prop.PropertyType);

                    // allowAdd is false - we need to assign instance as the new property value
                    prop.SetValue(instance, collectionInstance, property.Name, false /* allowAdd? */);
                }
                else
                {
                    // Clear existing Collection
                    prop.ClearBackingICollectionInstance(collectionInstance);
                }

                bool isElementNullable = prop.EdmProperty.Type.AsCollection().ElementType().IsNullable;
                this.CollectionValueMaterializationPolicy.ApplyCollectionDataValues(
                    property,
                    collectionInstance,
                    prop.PrimitiveOrComplexCollectionItemType,
                    prop.AddValueToBackingICollectionInstance,
                    isElementNullable);
            }
            else if ((enumTypeTmp = Nullable.GetUnderlyingType(prop.NullablePropertyType) ?? prop.NullablePropertyType) != null
                && enumTypeTmp.IsEnum())
            {
                ODataEnumValue enumValue = property.Value as ODataEnumValue;
                object tmpValue = EnumValueMaterializationPolicy.MaterializeODataEnumValue(enumTypeTmp, enumValue);

                // TODO: 1. use EnumValueMaterializationPolicy 2. handle nullable enum property
                prop.SetValue(instance, tmpValue, property.Name, false /* allowAdd? */);
            }
            else
            {
                this.MaterializePrimitiveDataValue(prop.NullablePropertyType, property);
                prop.SetValue(instance, property.GetMaterializedValue(), property.Name, true /* allowAdd? */);
            }

            if (!this.MaterializerContext.Context.DisableInstanceAnnotationMaterialization)
            {
                // Apply instance annotation for Property
                this.InstanceAnnotationMaterializationPolicy.SetInstanceAnnotations(property, type.ElementType, instance);
            }
        }

        /// <summary>
        /// Materializes the primitive data values in the given list of <paramref name="values"/>.
        /// </summary>
        /// <param name="actualType">Actual type for properties being materialized.</param>
        /// <param name="values">List of values to materialize.</param>
        /// <param name="undeclaredPropertyBehavior">
        /// Whether properties missing from the client types should be supported or throw exception.
        /// </param>
        /// <remarks>
        /// Values are materialized in-place with each <see cref="ODataProperty"/>
        /// instance.
        /// </remarks>
        internal void MaterializeDataValues(ClientTypeAnnotation actualType, IEnumerable<ODataProperty> values, UndeclaredPropertyBehavior undeclaredPropertyBehavior)
        {
            Debug.Assert(actualType != null, "actualType != null");
            Debug.Assert(values != null, "values != null");

            foreach (var odataProperty in values)
            {
                if (odataProperty.Value is ODataStreamReferenceValue)
                {
                    continue;
                }

                string propertyName = odataProperty.Name;

                var property = actualType.GetProperty(propertyName, undeclaredPropertyBehavior); // may throw
                if (property == null)
                {
                    continue;
                }

                // we should throw if the property type on the client does not match with the property type in the server
                // This is a breaking change from V1/V2 where we allowed materialization of entities into non-entities and vice versa
                if (ClientTypeUtil.TypeOrElementTypeIsEntity(property.PropertyType))
                {
                    throw DSClient.Error.InvalidOperation(DSClient.Strings.AtomMaterializer_InvalidEntityType(property.EntityCollectionItemType ?? property.PropertyType));
                }

                if (property.IsKnownType)
                {
                    // Note: MaterializeDataValue materializes only properties of primitive types. Materialization specific
                    // to complex types and collections is done later.
                    this.MaterializePrimitiveDataValue(property.NullablePropertyType, odataProperty);
                }
            }
        }

        internal Type ResolveClientTypeForDynamicProperty(string serverTypeName, object instance)
        {
            Debug.Assert(!string.IsNullOrEmpty(serverTypeName), "!string.IsNullOrEmpty(serverTypeName)");
            Debug.Assert(instance != null, "instance != null");

            Type clientType;

            // 1. Try to ride on user hook for resolving name into type
            clientType = this.MaterializerContext.Context.ResolveTypeFromName(serverTypeName);
            if (clientType != null)
            {
                return clientType;
            }

            // Assembly where client types are expected to be
            Assembly assembly = instance.GetType().GetAssembly();

            // 2. Try to find type with qualified name matching that of the server type
            // Code generators (e.g. OData Connected Service extension) emit client types with the same qualified name as the server type
            clientType = assembly.GetType(serverTypeName);
            if (clientType != null)
            {
                return clientType;
            }
            
            // 3. Try to find type with a matching name from the same namespace as the containing type
            string containingTypeNamespace = instance.GetType().Namespace;
            int lastIndexOf = serverTypeName.LastIndexOf('.');
            if (lastIndexOf > 0)
            {
                string unqualifiedTypeName = serverTypeName.Substring(serverTypeName.LastIndexOf('.') + 1);
                clientType = assembly.GetType(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", containingTypeNamespace, unqualifiedTypeName));
                if (clientType != null)
                {
                    return clientType;
                }
            }

            return clientType;
        }



        /// <summary>
        /// Adds a data value to the dynamic properties dictionary (where it exists) on the specified <paramref name="instance"/>
        /// </summary>
        /// <param name="property">Property containing unmaterialzed value to apply</param>
        /// <param name="instance">Instance that may optionally contain the dynamic properties dictionary</param>
        internal void MaterializeDynamicProperty(ODataProperty property, object instance)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(instance != null, "instance != null");

            IDictionary<string, object> containerProperty;

            // Stop if owning type is not an open type
            // Or container property is not found
            // Or key with matching name already exists in the dictionary
            // Or value is null - based on the spec, a missing dynamic property is defined to be the same as a dynamic property with value null
            if (!ClientTypeUtil.IsInstanceOfOpenType(instance, this.MaterializerContext.Model)
                || !ClientTypeUtil.TryGetContainerProperty(instance, out containerProperty) 
                || containerProperty.ContainsKey(property.Name)
                || property.Value == null)
            {
                return;
            }

            object value = property.Value;

            // Handles properties of known types returned with type annotations
            if (!(value is ODataValue) && PrimitiveType.IsKnownType(value.GetType()))
            {
                containerProperty.Add(property.Name, value);
                return;
            }

            // Handle untyped value
            ODataUntypedValue untypedVal = value as ODataUntypedValue;
            if (untypedVal != null)
            {
                value = CommonUtil.ParseJsonToPrimitiveValue(untypedVal.RawValue);
                if (value == null)
                {
                    return;
                }

                containerProperty.Add(property.Name, value);
                return;
            }

            // Handle enum value
            ODataEnumValue enumVal = value as ODataEnumValue;
            if (enumVal != null)
            {
                Type clientType = ResolveClientTypeForDynamicProperty(enumVal.TypeName, instance);
                // Unable to resolve type for dynamic property
                if (clientType == null)
                {
                    return;
                }

                object materializedEnumValue;
                if (EnumValueMaterializationPolicy.TryMaterializeODataEnumValue(clientType, enumVal, out materializedEnumValue))
                {
                    containerProperty.Add(property.Name, materializedEnumValue);
                }

                return;
            }

            // Handle collection
            ODataCollectionValue collectionVal = value as ODataCollectionValue;
            if (collectionVal != null)
            {
                string collectionItemTypeName = CommonUtil.GetCollectionItemTypeName(collectionVal.TypeName, false);

                // Highly unlikely, but the method return null if the typeName argument does not meet certain expectations
                if (collectionItemTypeName == null)
                {
                    return;
                }

                Type collectionItemType;

                // ToNamedType will return true for primitive types
                if (!ClientConvert.ToNamedType(collectionItemTypeName, out collectionItemType))
                {
                    // Non-primitive collection
                    collectionItemType = ResolveClientTypeForDynamicProperty(collectionItemTypeName, instance);
                }

                if (collectionItemType == null)
                {
                    return;
                }

                object collectionInstance;
                if (this.CollectionValueMaterializationPolicy.TryMaterializeODataCollectionValue(collectionItemType, property, out collectionInstance))
                {
                    containerProperty.Add(property.Name, collectionInstance);
                }

                return;
            }
        }
    }
}
