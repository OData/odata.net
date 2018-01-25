//---------------------------------------------------------------------
// <copyright file="ODataResourceMetadataContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.OData.Edm.Vocabularies.V1;

    /// <summary>
    /// Default implementation of <see cref="IODataResourceMetadataContext"/>
    /// </summary>
    internal abstract class ODataResourceMetadataContext : IODataResourceMetadataContext
    {
        /// <summary>
        /// Empty array of properties.
        /// </summary>
        private static readonly KeyValuePair<string, object>[] EmptyProperties = new KeyValuePair<string, object>[0];

        /// <summary>
        /// The resource instance.
        /// </summary>
        private readonly ODataResourceBase resource;

        /// <summary>
        /// The context object to answer basic questions regarding the type of the resource.
        /// </summary>
        private readonly IODataResourceTypeContext typeContext;

        /// <summary>
        /// The key property name and value pairs of the resource.
        /// </summary>
        private KeyValuePair<string, object>[] keyProperties;

        /// <summary>
        /// The ETag property name and value pairs of the resource.
        /// </summary>
        private IEnumerable<KeyValuePair<string, object>> etagProperties;

        /// <summary>
        /// The selected navigation properties.
        /// </summary>
        private IEnumerable<IEdmNavigationProperty> selectedNavigationProperties;

        /// <summary>
        /// The selected stream properties.
        /// </summary>
        private IDictionary<string, IEdmStructuralProperty> selectedStreamProperties;

        /// <summary>
        /// The selected bindable operations.
        /// </summary>
        private IEnumerable<IEdmOperation> selectedBindableOperations;

        /// <summary>
        /// Constructs an instance of <see cref="ODataResourceMetadataContext"/>.
        /// </summary>
        /// <param name="resource">The resource instance.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource.</param>
        protected ODataResourceMetadataContext(ODataResourceBase resource, IODataResourceTypeContext typeContext)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(typeContext != null, "typeContext != null");

            this.resource = resource;
            this.typeContext = typeContext;
        }

        /// <summary>
        /// The resource instance.
        /// </summary>
        public ODataResourceBase Resource
        {
            get { return this.resource; }
        }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the resource.
        /// </summary>
        public IODataResourceTypeContext TypeContext
        {
            get { return this.typeContext; }
        }

        /// <summary>
        /// The actual structured type of the resource, i.e. ODataResource.TypeName.
        /// </summary>
        public abstract string ActualResourceTypeName { get; }

        /// <summary>
        /// The key property name and value pairs of the resource.
        /// </summary>
        public abstract ICollection<KeyValuePair<string, object>> KeyProperties { get; }

        /// <summary>
        /// The ETag property name and value pairs of the resource.
        /// </summary>
        public abstract IEnumerable<KeyValuePair<string, object>> ETagProperties { get; }

        /// <summary>
        /// The selected navigation properties.
        /// </summary>
        public abstract IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties { get; }

        /// <summary>
        /// The selected stream properties.
        /// </summary>
        public abstract IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties { get; }

        /// <summary>
        /// The selected bindable operations.
        /// </summary>
        public abstract IEnumerable<IEdmOperation> SelectedBindableOperations { get; }

        /// <summary>
        /// Creates an instance of <see cref="ODataResourceMetadataContext"/>.
        /// </summary>
        /// <param name="resource">The resource instance.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource.</param>
        /// <param name="serializationInfo">The serialization info of the resource for writing without model.</param>
        /// <param name="actualResourceType">The structured type of the resource.</param>
        /// <param name="metadataContext">The metadata context to use.</param>
        /// <param name="selectedProperties">The selected properties.</param>
        /// <returns>A new instance of <see cref="ODataResourceMetadataContext"/>.</returns>
        internal static ODataResourceMetadataContext Create(
            ODataResourceBase resource,
            IODataResourceTypeContext typeContext,
            ODataResourceSerializationInfo serializationInfo,
            IEdmStructuredType actualResourceType,
            IODataMetadataContext metadataContext,
            SelectedPropertiesNode selectedProperties)
        {
            if (serializationInfo != null)
            {
                return new ODataResourceMetadataContextWithoutModel(resource, typeContext, serializationInfo);
            }

            return new ODataResourceMetadataContextWithModel(resource, typeContext, actualResourceType, metadataContext, selectedProperties);
        }

        /// <summary>
        /// Get key value pair array for specifc odata resource using specifc entity type
        /// </summary>
        /// <param name="resource">The resource instance.</param>
        /// <param name="serializationInfo">The serialization info of the resource for writing without model.</param>
        /// <param name="actualEntityType">The edm entity type of the resource</param>
        /// <returns>Key value pair array</returns>
        internal static KeyValuePair<string, object>[] GetKeyProperties(
            ODataResourceBase resource,
            ODataResourceSerializationInfo serializationInfo,
            IEdmEntityType actualEntityType)
        {
            KeyValuePair<string, object>[] keyProperties = null;
            string actualEntityTypeName = null;

            if (serializationInfo != null)
            {
                if (String.IsNullOrEmpty(resource.TypeName))
                {
                    throw new ODataException(Strings.ODataResourceTypeContext_ODataResourceTypeNameMissing);
                }

                actualEntityTypeName = resource.TypeName;
                keyProperties = ODataResourceMetadataContextWithoutModel.GetPropertiesBySerializationInfoPropertyKind(resource, ODataPropertyKind.Key, actualEntityTypeName);
            }
            else
            {
                actualEntityTypeName = actualEntityType.FullName();

                IEnumerable<IEdmStructuralProperty> edmKeyProperties = actualEntityType.Key();
                if (edmKeyProperties != null)
                {
                    keyProperties = edmKeyProperties.Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitiveOrEnumPropertyValue(resource, p.Name, actualEntityTypeName, /*isKeyProperty*/false))).ToArray();
                }
            }

            ValidateEntityTypeHasKeyProperties(keyProperties, actualEntityTypeName);
            return keyProperties;
        }

        /// <summary>
        /// Gets the value for a primitive or enum property.
        /// </summary>
        /// <param name="resource">The resource to get the property value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="entityTypeName">The name of the entity type to get the property value.</param>
        /// <param name="isKeyProperty">true if the property is a key property, false otherwise.</param>
        /// <returns>The value of the property.</returns>
        private static object GetPrimitiveOrEnumPropertyValue(ODataResourceBase resource, string propertyName, string entityTypeName, bool isKeyProperty)
        {
            Debug.Assert(resource != null, "resource != null");

            ODataProperty property = resource.NonComputedProperties == null ? null : resource.NonComputedProperties.SingleOrDefault(p => p.Name == propertyName);
            if (property == null)
            {
                throw new ODataException(Strings.EdmValueUtils_PropertyDoesntExist(entityTypeName, propertyName));
            }

            return GetPrimitiveOrEnumPropertyValue(entityTypeName, property, isKeyProperty);
        }

        /// <summary>
        /// Gets the value for a primitive or enum property. For primitive type, returns its CLR value; for enum type, returns OData enum value.
        /// </summary>
        /// <param name="entityTypeName">The name of the entity type to get the property value.</param>
        /// <param name="property">The ODataProperty to get the value from.</param>
        /// <param name="isKeyProperty">true if the property is a key property, false otherwise.</param>
        /// <returns>The value of the property.</returns>
        private static object GetPrimitiveOrEnumPropertyValue(string entityTypeName, ODataProperty property, bool isKeyProperty)
        {
            object propertyValue = property.Value;
            if (propertyValue == null && isKeyProperty)
            {
                throw new ODataException(Strings.ODataResourceMetadataContext_NullKeyValue(property.Name, entityTypeName));
            }

            if (propertyValue is ODataValue && !(propertyValue is ODataEnumValue))
            {
                throw new ODataException(Strings.ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues(property.Name, entityTypeName));
            }

            return propertyValue;
        }

        /// <summary>
        /// Validates that the resource has key properties.
        /// </summary>
        /// <param name="keyProperties">Key properties of the resource.</param>
        /// <param name="actualEntityTypeName">The entity type name of the resource.</param>
        private static void ValidateEntityTypeHasKeyProperties(KeyValuePair<string, object>[] keyProperties, string actualEntityTypeName)
        {
            Debug.Assert(keyProperties != null, "keyProperties != null");
            if (keyProperties == null || keyProperties.Length == 0)
            {
                throw new ODataException(Strings.ODataResourceMetadataContext_EntityTypeWithNoKeyProperties(actualEntityTypeName));
            }
        }

        /// <summary>
        /// Gets the property name value pairs filtered by serialization property kind.
        /// </summary>
        /// <param name="resource">The resource to get the properties from.</param>
        /// <param name="propertyKind">The serialization info property kind.</param>
        /// <param name="actualEntityTypeName">The entity type name of the resource.</param>
        /// <returns>The property name value pairs filtered by serialization property kind.</returns>
        private static KeyValuePair<string, object>[] GetPropertiesBySerializationInfoPropertyKind(ODataResourceBase resource, ODataPropertyKind propertyKind, string actualEntityTypeName)
        {
            Debug.Assert(resource != null, "resource != null");
            Debug.Assert(propertyKind == ODataPropertyKind.Key || propertyKind == ODataPropertyKind.ETag, "propertyKind == ODataPropertyKind.Key || propertyKind == ODataPropertyKind.ETag");

            KeyValuePair<string, object>[] properties = EmptyProperties;
            if (resource.NonComputedProperties != null)
            {
                properties = resource.NonComputedProperties.Where(p => p.SerializationInfo != null && p.SerializationInfo.PropertyKind == propertyKind).Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitiveOrEnumPropertyValue(actualEntityTypeName, p, propertyKind == ODataPropertyKind.Key))).ToArray();
            }

            return properties;
        }

        /// <summary>
        /// Implementation of <see cref="IODataResourceMetadataContext"/> based on serialization info.
        /// </summary>
        private sealed class ODataResourceMetadataContextWithoutModel : ODataResourceMetadataContext
        {
            /// <summary>
            /// Empty array of navigation properties.
            /// </summary>
            private static readonly IEdmNavigationProperty[] EmptyNavigationProperties = new IEdmNavigationProperty[0];

            /// <summary>
            /// Empty dictionary of stream properties.
            /// </summary>
            private static readonly Dictionary<string, IEdmStructuralProperty> EmptyStreamProperties = new Dictionary<string, IEdmStructuralProperty>(StringComparer.Ordinal);

            /// <summary>
            /// Empty array of operations.
            /// </summary>
            private static readonly IEdmOperation[] EmptyOperations = new IEdmOperation[0];

            /// <summary>
            /// The serialization info of the resource for writing without model.
            /// </summary>
            private readonly ODataResourceSerializationInfo serializationInfo;

            /// <summary>
            /// Constructs an instance of <see cref="ODataResourceMetadataContextWithoutModel"/>.
            /// </summary>
            /// <param name="resource">The resource instance.</param>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource.</param>
            /// <param name="serializationInfo">The serialization info of the resource for writing without model.</param>
            internal ODataResourceMetadataContextWithoutModel(ODataResourceBase resource, IODataResourceTypeContext typeContext, ODataResourceSerializationInfo serializationInfo)
                : base(resource, typeContext)
            {
                Debug.Assert(serializationInfo != null, "serializationInfo != null");
                this.serializationInfo = serializationInfo;
            }

            /// <summary>
            /// The key property name and value pairs of the resource.
            /// </summary>
            public override ICollection<KeyValuePair<string, object>> KeyProperties
            {
                get
                {
                    if (this.keyProperties == null)
                    {
                        this.keyProperties = GetPropertiesBySerializationInfoPropertyKind(this.resource, ODataPropertyKind.Key, this.ActualResourceTypeName);
                        ValidateEntityTypeHasKeyProperties(this.keyProperties, this.ActualResourceTypeName);
                    }

                    return this.keyProperties;
                }
            }

            /// <summary>
            /// The ETag property name and value pairs of the resource.
            /// </summary>
            public override IEnumerable<KeyValuePair<string, object>> ETagProperties
            {
                get { return this.etagProperties ?? (this.etagProperties = GetPropertiesBySerializationInfoPropertyKind(this.resource, ODataPropertyKind.ETag, this.ActualResourceTypeName)); }
            }

            /// <summary>
            /// The actual structured type of the resource, i.e. ODataResource.TypeName.
            /// </summary>
            public override string ActualResourceTypeName
            {
                get
                {
                    if (String.IsNullOrEmpty(this.Resource.TypeName))
                    {
                        throw new ODataException(Strings.ODataResourceTypeContext_ODataResourceTypeNameMissing);
                    }

                    return this.Resource.TypeName;
                }
            }

            /// <summary>
            /// The selected navigation properties.
            /// </summary>
            public override IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties
            {
                get { return EmptyNavigationProperties; }
            }

            /// <summary>
            /// The selected stream properties.
            /// </summary>
            public override IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties
            {
                get { return EmptyStreamProperties; }
            }

            /// <summary>
            /// The selected bindable operations.
            /// </summary>
            public override IEnumerable<IEdmOperation> SelectedBindableOperations
            {
                get { return EmptyOperations; }
            }
        }

        /// <summary>
        /// Implementation of <see cref="IODataResourceMetadataContext"/> based on the given model.
        /// </summary>
        private sealed class ODataResourceMetadataContextWithModel : ODataResourceMetadataContext
        {
            /// <summary>
            /// The structured type of the resource.
            /// </summary>
            private readonly IEdmStructuredType actualResourceType;

            /// <summary>
            /// The metadata context to use.
            /// </summary>
            private readonly IODataMetadataContext metadataContext;

            /// <summary>
            /// The selected properties.
            /// </summary>
            private readonly SelectedPropertiesNode selectedProperties;

            /// <summary>
            /// Constructs an instance of <see cref="ODataResourceMetadataContextWithModel"/>.
            /// </summary>
            /// <param name="resource">The resource instance.</param>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the resource.</param>
            /// <param name="actualResourceType">The structured type of the resource.</param>
            /// <param name="metadataContext">The metadata context to use.</param>
            /// <param name="selectedProperties">The selected properties.</param>
            internal ODataResourceMetadataContextWithModel(ODataResourceBase resource, IODataResourceTypeContext typeContext, IEdmStructuredType actualResourceType, IODataMetadataContext metadataContext, SelectedPropertiesNode selectedProperties)
                : base(resource, typeContext)
            {
                Debug.Assert(actualResourceType != null, "actualResourceType != null");
                Debug.Assert(metadataContext != null, "metadataContext != null");
                Debug.Assert(selectedProperties != null, "selectedProperties != null");

                this.actualResourceType = actualResourceType;
                this.metadataContext = metadataContext;
                this.selectedProperties = selectedProperties;
            }

            /// <summary>
            /// The key property name and value pairs of the resource.
            /// </summary>
            public override ICollection<KeyValuePair<string, object>> KeyProperties
            {
                get
                {
                    if (this.keyProperties == null)
                    {
                        var entityType = this.actualResourceType as IEdmEntityType;
                        if (entityType != null)
                        {
                            IEnumerable<IEdmStructuralProperty> edmKeyProperties = entityType.Key();
                            if (edmKeyProperties != null)
                            {
                                this.keyProperties = edmKeyProperties.Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitiveOrEnumPropertyValue(this.resource, p.Name, this.ActualResourceTypeName, /*isKeyProperty*/true))).ToArray();
                            }

                            ValidateEntityTypeHasKeyProperties(this.keyProperties, this.ActualResourceTypeName);
                        }
                        else
                        {
                            this.keyProperties = Enumerable.Empty<KeyValuePair<string, object>>().ToArray();
                        }
                    }

                    return this.keyProperties;
                }
            }

            /// <summary>
            /// The ETag property name and value pairs of the resource.
            /// </summary>
            public override IEnumerable<KeyValuePair<string, object>> ETagProperties
            {
                get
                {
                    if (this.etagProperties == null)
                    {
                        IEnumerable<IEdmStructuralProperty> properties = this.ComputeETagPropertiesFromAnnotation();
                        this.etagProperties = properties.Any()
                            ? properties.Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitiveOrEnumPropertyValue(this.resource, p.Name, this.ActualResourceTypeName, /*isKeyProperty*/false))).ToArray()
                            : EmptyProperties;
                    }

                    return this.etagProperties;
                }
            }

            /// <summary>
            /// The actual structured type name of the resource.
            /// </summary>
            public override string ActualResourceTypeName
            {
                // Note that resource.TypeName can be null. When that happens, we use the expected structured type as the actual structured type.
                get { return this.actualResourceType.FullTypeName(); }
            }

            /// <summary>
            /// The selected navigation properties.
            /// </summary>
            public override IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties
            {
                get
                {
                    return this.selectedNavigationProperties
                        ?? (this.selectedNavigationProperties = this.selectedProperties.GetSelectedNavigationProperties(this.actualResourceType));
                }
            }

            /// <summary>
            /// The selected stream properties.
            /// </summary>
            public override IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties
            {
                get
                {
                    return this.selectedStreamProperties
                        ?? (this.selectedStreamProperties = this.selectedProperties.GetSelectedStreamProperties(this.actualResourceType as IEdmEntityType));
                }
            }

            /// <summary>
            /// The selected bindable operations.
            /// </summary>
            public override IEnumerable<IEdmOperation> SelectedBindableOperations
            {
                get
                {
                    if (this.selectedBindableOperations == null)
                    {
                        bool mustBeContainerQualified = this.metadataContext.OperationsBoundToStructuredTypeMustBeContainerQualified(this.actualResourceType);
                        this.selectedBindableOperations = this.metadataContext.GetBindableOperationsForType(this.actualResourceType)
                            .Where(operation => this.selectedProperties.IsOperationSelected(this.actualResourceType, operation, mustBeContainerQualified))
                            .ToArray();
                    }

                    return this.selectedBindableOperations;
                }
            }

            /// <summary>
            /// Compute ETag from Annotation Org.OData.Core.V1.OptimisticConcurrency on EntitySet
            /// </summary>
            /// <returns>Enumerable of IEdmStructuralProperty</returns>
            private IEnumerable<IEdmStructuralProperty> ComputeETagPropertiesFromAnnotation()
            {
                IEdmModel model = this.metadataContext.Model;
                IEdmEntitySet entitySet = model.FindDeclaredEntitySet(this.typeContext.NavigationSourceName);

                if (entitySet != null)
                {
                    IEdmVocabularyAnnotation annotation = model.FindDeclaredVocabularyAnnotations(entitySet)
                        .SingleOrDefault(t => t.Term.FullName().Equals(CoreVocabularyConstants.OptimisticConcurrency, StringComparison.Ordinal));
                    if (annotation != null)
                    {
                        IEdmExpression collectionExpression = annotation.Value;
                        if (collectionExpression is IEdmCollectionExpression)
                        {
                            IEnumerable<IEdmExpression> pathExpressions = (collectionExpression as IEdmCollectionExpression).Elements.Where(p => p is IEdmPathExpression);
                            foreach (IEdmPathExpression pathExpression in pathExpressions)
                            {
                                // TODO:
                                //  1. Add support for Complex type
                                //  2. Add new exception when collectionExpression is not IEdmCollectionExpression: CoreOptimisticConcurrency must be followed by collection expression
                                IEdmStructuralProperty property = this.actualResourceType.StructuralProperties().FirstOrDefault(p => p.Name == pathExpression.PathSegments.LastOrDefault());
                                if (property == null)
                                {
                                    throw new ODataException(Strings.EdmValueUtils_PropertyDoesntExist(this.ActualResourceTypeName, pathExpression.PathSegments.LastOrDefault()));
                                }

                                yield return property;
                            }
                        }
                    }
                }
            }
        }
    }
}