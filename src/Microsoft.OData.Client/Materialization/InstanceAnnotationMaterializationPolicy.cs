//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationMaterializationPolicy.cs" company="Microsoft">
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
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Use this class to materialize instance annnotations in an <see cref="ODataAnnotatable"/>.
    /// </summary>
    internal class InstanceAnnotationMaterializationPolicy
    {
        /// <summary>
        /// The collection value materialization policy.
        /// </summary>
        private CollectionValueMaterializationPolicy collectionValueMaterializationPolicy;

        /// <summary>
        /// The complex value materialization policy.
        /// </summary>
        private ComplexValueMaterializationPolicy complexValueMaterializerPolicy;

        /// <summary>
        /// The enum value materialization policy
        /// </summary>
        private EnumValueMaterializationPolicy enumValueMaterializationPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceAnnotationMaterializationPolicy" /> class.
        /// </summary>
        /// <param name="materializerContext">The materializer context.</param>
        internal InstanceAnnotationMaterializationPolicy(IODataMaterializerContext materializerContext)
        {
            Debug.Assert(materializerContext != null, "materializer!=null");
            this.MaterializerContext = materializerContext;
        }

        /// <summary>
        /// The collection value materialization policy.
        /// </summary>
        internal CollectionValueMaterializationPolicy CollectionValueMaterializationPolicy
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
        /// The complex value materialization policy.
        /// </summary>
        internal ComplexValueMaterializationPolicy ComplexValueMaterializationPolicy
        {
            get
            {
                Debug.Assert(this.complexValueMaterializerPolicy != null, "complexValueMaterializerPolicy != null");
                return this.complexValueMaterializerPolicy;
            }

            set
            {
                this.complexValueMaterializerPolicy = value;
            }
        }

        /// <summary>
        /// The Enum value materialization policy.
        /// </summary>
        internal EnumValueMaterializationPolicy EnumValueMaterializationPolicy
        {
            get
            {
                Debug.Assert(this.EnumValueMaterializationPolicy != null, "enumValueMaterializationPolicy != null");
                return this.enumValueMaterializationPolicy;
            }

            set
            {
                this.enumValueMaterializationPolicy = value;
            }
        }

        /// <summary>
        /// The materializer context.
        /// </summary>
        internal IODataMaterializerContext MaterializerContext { get; private set; }

        /// <summary>
        /// Materialize instance annotation for an OData entry
        /// </summary>
        /// <param name="entry">Odata entry</param>
        /// <param name="entity">Client clr object for the OData entry</param>
        internal void SetInstanceAnnotations(ODataEntry entry, object entity)
        {
            if (entry != null)
            {
                IDictionary<string, object> instanceAnnotations = this.ConvertToClrInstanceAnnotations(entry.InstanceAnnotations);
                SetInstanceAnnotations(entity, instanceAnnotations);
            }
        }

        /// <summary>
        /// Materialize instance annotation for an OData property
        /// </summary>
        /// <param name="property">OData property</param>
        /// <param name="instance">the clr object of the property</param>
        internal void SetInstanceAnnotations(ODataProperty property, object instance)
        {
            if (property != null)
            {
                IDictionary<string, object> instanceAnnotations = this.GetClrInstanceAnnotationsFromODataProperty(property);
                SetInstanceAnnotations(instance, instanceAnnotations);
            }
        }

        /// <summary>
        /// Materialize instance annotation for an OData complex value
        /// </summary>
        /// <param name="complexValue">OData complex value</param>
        /// <param name="complexInstance">Client clr object for the complex value</param>
        internal void SetInstanceAnnotations(ODataComplexValue complexValue, object complexInstance)
        {
            if (complexValue != null)
            {
                IDictionary<string, object> instanceAnnotations = this.ConvertToClrInstanceAnnotations(complexValue.InstanceAnnotations);
                SetInstanceAnnotations(complexInstance, instanceAnnotations);
            }
        }

        /// <summary>
        /// Materialize instance annotation for OData property
        /// </summary>
        /// <param name="property">OData property</param>
        /// <param name="type">The type of declaringInstance</param>
        /// <param name="declaringInstance">the client object that the property belongs to</param>
        internal void SetInstanceAnnotations(ODataProperty property, Type type, object declaringInstance)
        {
            if (property != null)
            {
                IDictionary<string, object> instanceAnnotations = this.GetClrInstanceAnnotationsFromODataProperty(property);
                SetInstanceAnnotations(property.Name, instanceAnnotations, type, declaringInstance);
            }
        }

        /// <summary>
        /// Materialize instance annotation for OData navigation property
        /// </summary>
        /// <param name="navigationPropertyName">navigation property name</param>
        /// <param name="navigationProperty">OData single navigation property</param>
        /// <param name="type">The type of the declaringInstance</param>
        /// <param name="declaringInstance">the client object that the navigation property belongs to</param>
        internal void SetInstanceAnnotations(string navigationPropertyName, ODataEntry navigationProperty, Type type, object declaringInstance)
        {
            if (navigationProperty != null)
            {
                IDictionary<string, object> instanceAnnotations = this.ConvertToClrInstanceAnnotations(navigationProperty.InstanceAnnotations);
                SetInstanceAnnotations(navigationPropertyName, instanceAnnotations, type, declaringInstance);
            }
        }

        /// <summary>
        /// Convert a collection of instance annotations to clr objects.
        /// </summary>
        /// <param name="instanceAnnotations">A collection of instance annotation to be converted</param>
        /// <returns>A dictionary of clr-typed instance annotation</returns>
        internal IDictionary<string, object> ConvertToClrInstanceAnnotations(ICollection<ODataInstanceAnnotation> instanceAnnotations)
        {
            var clientInstanceAnnotationValue = new Dictionary<string, object>(StringComparer.Ordinal);
            if (instanceAnnotations != null && instanceAnnotations.Count > 0)
            {
                foreach (var instanceAnnotation in instanceAnnotations)
                {
                    object clrInstanceAnnotation;
                    if (TryConvertToClrInstanceAnnotation(instanceAnnotation, out clrInstanceAnnotation))
                    {
                        clientInstanceAnnotationValue.Add(instanceAnnotation.Name, clrInstanceAnnotation);
                    }
                }
            }

            return clientInstanceAnnotationValue;
        }

        /// <summary>
        /// Set instance annotations for an object
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <param name="instanceAnnotations">The instance annotations to be set</param>
        private void SetInstanceAnnotations(object instance, IDictionary<string, object> instanceAnnotations)
        {
            if (instance != null)
            {
                this.MaterializerContext.Context.InstanceAnnotations.Remove(instance);
                if (instanceAnnotations != null && instanceAnnotations.Count > 0)
                {
                    this.MaterializerContext.Context.InstanceAnnotations.Add(instance, instanceAnnotations);
                }
            }
        }

        /// <summary>
        /// Set instance annotation for a property
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="instanceAnnotations">Intance annotations to be set</param>
        /// <param name="type">The type of the containing object</param>
        /// <param name="declaringInstance">The containing object instance</param>
        private void SetInstanceAnnotations(string propertyName, IDictionary<string, object> instanceAnnotations, Type type, object declaringInstance)
        {
            if (declaringInstance != null)
            {
                bool ignoreMissingProperty = this.MaterializerContext.Context.IgnoreMissingProperties;

                // Get the client property info
                ClientEdmModel edmModel = this.MaterializerContext.Model;
                ClientTypeAnnotation clientTypeAnnotation = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(type));
                ClientPropertyAnnotation clientPropertyAnnotation = clientTypeAnnotation.GetProperty(propertyName, ignoreMissingProperty);
                Tuple<object, MemberInfo> annotationKeyForProperty = new Tuple<object, MemberInfo>(declaringInstance, clientPropertyAnnotation.PropertyInfo);
                SetInstanceAnnotations(annotationKeyForProperty, instanceAnnotations);
            }
        }

        /// <summary>
        /// Convert instance annotations of the ODataProperty to clr objects.
        /// </summary>
        /// <param name="property">OData property</param>
        /// <returns>A dictionary of clr-typed instance annotation which is materialized from the instance annotations of the ODataProperty</returns>
        private IDictionary<string, object> GetClrInstanceAnnotationsFromODataProperty(ODataProperty property)
        {
            IDictionary<string, object> clientInstanceAnnotationValue = null;

            // If the property is a complex type property, instance annotations are stored in the complex value.
            var odataComplexPropertyValue = property.Value as ODataComplexValue;
            if (odataComplexPropertyValue != null)
            {
                clientInstanceAnnotationValue = ConvertToClrInstanceAnnotations(odataComplexPropertyValue.InstanceAnnotations);
            }

            if (clientInstanceAnnotationValue == null)
            {
                clientInstanceAnnotationValue = new Dictionary<string, object>(StringComparer.Ordinal);
            }

            if (property.InstanceAnnotations != null)
            {
                foreach (var instanceAnnotation in property.InstanceAnnotations)
                {
                    object clrInstanceAnnotation;
                    if (TryConvertToClrInstanceAnnotation(instanceAnnotation, out clrInstanceAnnotation))
                    {
                        clientInstanceAnnotationValue.Add(instanceAnnotation.Name, clrInstanceAnnotation);
                    }
                }
            }

            return clientInstanceAnnotationValue;
        }

        /// <summary>
        /// Convert an instance annotation to clr object.
        /// </summary>
        /// <param name="instanceAnnotation">Instance annotation to be converted</param>
        /// <param name="clrInstanceAnnotation">The clr object</param>
        /// <returns>A dictionary of clr-typed instance annotation</returns>
        private bool TryConvertToClrInstanceAnnotation(ODataInstanceAnnotation instanceAnnotation, out object clrInstanceAnnotation)
        {
            clrInstanceAnnotation = null;

            var primitiveValue = instanceAnnotation.Value as ODataPrimitiveValue;
            if (primitiveValue != null)
            {
                clrInstanceAnnotation = primitiveValue.Value;
                return true;
            }

            var enumValue = instanceAnnotation.Value as ODataEnumValue;
            if (enumValue != null)
            {
                var type = this.MaterializerContext.Context.ResolveTypeFromName(enumValue.TypeName);
                if (type != null)
                {
                    clrInstanceAnnotation = EnumValueMaterializationPolicy.MaterializeODataEnumValue(type, enumValue);
                    return true;
                }

                return false;
            }

            var complexValue = instanceAnnotation.Value as ODataComplexValue;
            if (complexValue != null)
            {
                var type = this.MaterializerContext.Context.ResolveTypeFromName(complexValue.TypeName);
                if (type != null)
                {
                    ClientEdmModel edmModel = this.MaterializerContext.Model;
                    ClientTypeAnnotation complexType = edmModel.GetClientTypeAnnotation(edmModel.GetOrCreateEdmType(type));

                    // TODO: check if ComplexType inheritance will cause any issue
                    var complexInstance = this.ComplexValueMaterializationPolicy.CreateNewInstance(complexType.EdmTypeReference, complexType.ElementType);
                    this.ComplexValueMaterializationPolicy.MaterializeDataValues(complexType, complexValue.Properties, this.MaterializerContext.IgnoreMissingProperties);
                    this.ComplexValueMaterializationPolicy.ApplyDataValues(complexType, complexValue.Properties, complexInstance);
                    clrInstanceAnnotation = complexInstance;
                    return true;
                }

                return false;
            }

            var collectionValue = instanceAnnotation.Value as ODataCollectionValue;
            if (collectionValue != null)
            {
                var serverSideModel = this.MaterializerContext.Context.Format.LoadServiceModel();
                var valueTerm = serverSideModel.FindValueTerm(instanceAnnotation.Name);

                if (valueTerm != null && valueTerm.Type != null && valueTerm.Type.Definition != null)
                {
                    var edmCollectionType = valueTerm.Type.Definition as IEdmCollectionType;
                    if (edmCollectionType != null)
                    {
                        Type collectionItemType = null;
                        var elementType = edmCollectionType.ElementType;
                        PrimitiveType primitiveType;
                        if (PrimitiveType.TryGetPrimitiveType(elementType.FullName(), out primitiveType))
                        {
                            collectionItemType = primitiveType.ClrType;
                        }
                        else
                        {
                            collectionItemType = this.MaterializerContext.Context.ResolveTypeFromName(elementType.FullName());
                        }

                        if (collectionItemType != null)
                        {
                            Type collectionICollectionType = typeof(ICollection<>).MakeGenericType(new Type[] { collectionItemType });

                            ClientTypeAnnotation collectionClientTypeAnnotation = this.MaterializerContext.ResolveTypeForMaterialization(
                                collectionICollectionType,
                                collectionValue.TypeName);
                            bool isElementNullable = edmCollectionType.ElementType.IsNullable;

                            var collectionInstance = this.CollectionValueMaterializationPolicy.CreateCollectionInstance(
                                collectionClientTypeAnnotation.EdmTypeReference as IEdmCollectionTypeReference,
                                collectionClientTypeAnnotation.ElementType);
                            this.CollectionValueMaterializationPolicy.ApplyCollectionDataValues(
                                collectionValue.Items,
                                collectionValue.TypeName,
                                collectionInstance,
                                collectionItemType,
                                ClientTypeUtil.GetAddToCollectionDelegate(collectionICollectionType),
                                isElementNullable);
                            clrInstanceAnnotation = collectionInstance;
                            return true;
                        }
                    }
                }

                return false;
            }

            var nullValue = instanceAnnotation.Value as ODataNullValue;
            if (nullValue != null)
            {
                clrInstanceAnnotation = null;
                return true;
            }

            return false;
        }
    }
}
