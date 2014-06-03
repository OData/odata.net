//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Expressions;
    using Microsoft.OData.Edm.Vocabularis;

    /// <summary>
    /// Default implementation of <see cref="IODataEntryMetadataContext"/>
    /// </summary>
    internal abstract class ODataEntryMetadataContext : IODataEntryMetadataContext
    {
        /// <summary>
        /// Empty array of properties.
        /// </summary>
        private static readonly KeyValuePair<string, object>[] EmptyProperties = new KeyValuePair<string, object>[0];

        /// <summary>
        /// The entry instance.
        /// </summary>
        private readonly ODataEntry entry;

        /// <summary>
        /// The context object to answer basic questions regarding the type of the entry.
        /// </summary>
        private readonly IODataFeedAndEntryTypeContext typeContext;

        /// <summary>
        /// The key property name and value pairs of the entry.
        /// </summary>
        private KeyValuePair<string, object>[] keyProperties;

        /// <summary>
        /// The ETag property name and value pairs of the entry.
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
        /// Constructs an instance of <see cref="ODataEntryMetadataContext"/>.
        /// </summary>
        /// <param name="entry">The entry instance.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
        protected ODataEntryMetadataContext(ODataEntry entry, IODataFeedAndEntryTypeContext typeContext)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(typeContext != null, "typeContext != null");

            this.entry = entry;
            this.typeContext = typeContext;
        }

        /// <summary>
        /// The entry instance.
        /// </summary>
        public ODataEntry Entry
        {
            get { return this.entry; }
        }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the entry.
        /// </summary>
        public IODataFeedAndEntryTypeContext TypeContext
        {
            get { return this.typeContext; }
        }

        /// <summary>
        /// The actual entity type of the entry, i.e. ODataEntry.TypeName.
        /// </summary>
        public abstract string ActualEntityTypeName { get; }

        /// <summary>
        /// The key property name and value pairs of the entry.
        /// </summary>
        public abstract ICollection<KeyValuePair<string, object>> KeyProperties { get; }

        /// <summary>
        /// The ETag property name and value pairs of the entry.
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
        /// Creates an instance of <see cref="ODataEntryMetadataContext"/>.
        /// </summary>
        /// <param name="entry">The entry instance.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
        /// <param name="serializationInfo">The serialization info of the entry for writing without model.</param>
        /// <param name="actualEntityType">The entity type of the entry.</param>
        /// <param name="metadataContext">The metadata context to use.</param>
        /// <param name="selectedProperties">The selected properties.</param>
        /// <returns>A new instance of <see cref="ODataEntryMetadataContext"/>.</returns>
        internal static ODataEntryMetadataContext Create(
            ODataEntry entry,
            IODataFeedAndEntryTypeContext typeContext,
            ODataFeedAndEntrySerializationInfo serializationInfo,
            IEdmEntityType actualEntityType,
            IODataMetadataContext metadataContext,
            SelectedPropertiesNode selectedProperties)
        {
            if (serializationInfo != null)
            {
                return new ODataEntryMetadataContextWithoutModel(entry, typeContext, serializationInfo);
            }

            return new ODataEntryMetadataContextWithModel(entry, typeContext, actualEntityType, metadataContext, selectedProperties);
        }

        /// <summary>
        /// Get key value pair array for specifc odata entry using specifc entity type
        /// </summary>
        /// <param name="entry">The entry instance.</param>
        /// <param name="serializationInfo">The serialization info of the entry for writing without model.</param>
        /// <param name="actualEntityType">The edm entity type of the entry</param>
        /// <returns>Key value pair array</returns>
        internal static KeyValuePair<string, object>[] GetKeyProperties(
            ODataEntry entry,
            ODataFeedAndEntrySerializationInfo serializationInfo,
            IEdmEntityType actualEntityType)
        {
            KeyValuePair<string, object>[] keyProperties = null;
            string actualEntityTypeName = null;

            if (serializationInfo != null)
            {
                if (String.IsNullOrEmpty(entry.TypeName))
                {
                    throw new ODataException(OData.Core.Strings.ODataFeedAndEntryTypeContext_ODataEntryTypeNameMissing);
                }

                actualEntityTypeName = entry.TypeName;
                keyProperties = ODataEntryMetadataContextWithoutModel.GetPropertiesBySerializationInfoPropertyKind(entry, ODataPropertyKind.Key, actualEntityTypeName);
            }
            else
            {
                actualEntityTypeName = actualEntityType.FullName();

                IEnumerable<IEdmStructuralProperty> edmKeyProperties = actualEntityType.Key();
                if (edmKeyProperties != null)
                {
                    keyProperties = edmKeyProperties.Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitivePropertyClrValue(entry, p.Name, actualEntityTypeName, /*isKeyProperty*/false))).ToArray();
                }
            }

            ValidateEntityTypeHasKeyProperties(keyProperties, actualEntityTypeName);
            return keyProperties;
        }

        /// <summary>
        /// Gets the the CLR value for a primitive property.
        /// </summary>
        /// <param name="entry">The entry to get the property value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="entityTypeName">The name of the entity type to get the property value.</param>
        /// <param name="isKeyProperty">true if the property is a key property, false otherwise.</param>
        /// <returns>The clr value of the property.</returns>
        private static object GetPrimitivePropertyClrValue(ODataEntry entry, string propertyName, string entityTypeName, bool isKeyProperty)
        {
            Debug.Assert(entry != null, "entry != null");

            ODataProperty property = entry.NonComputedProperties == null ? null : entry.NonComputedProperties.SingleOrDefault(p => p.Name == propertyName);
            if (property == null)
            {
                throw new ODataException(OData.Core.Strings.EdmValueUtils_PropertyDoesntExist(entityTypeName, propertyName));
            }

            return GetPrimitivePropertyClrValue(entityTypeName, property, isKeyProperty);
        }

        /// <summary>
        /// Gets the CLR value for a primitive property.
        /// </summary>
        /// <param name="entityTypeName">The name of the entity type to get the property value.</param>
        /// <param name="property">The ODataProperty to get the value from.</param>
        /// <param name="isKeyProperty">true if the property is a key property, false otherwise.</param>
        /// <returns>The clr value of the property.</returns>
        private static object GetPrimitivePropertyClrValue(string entityTypeName, ODataProperty property, bool isKeyProperty)
        {
            object propertyValue = property.Value;
            if (propertyValue == null && isKeyProperty)
            {
                throw new ODataException(OData.Core.Strings.ODataEntryMetadataContext_NullKeyValue(property.Name, entityTypeName));
            }

            if (propertyValue is ODataValue)
            {
                throw new ODataException(OData.Core.Strings.ODataEntryMetadataContext_KeyOrETagValuesMustBePrimitiveValues(property.Name, entityTypeName));
            }

            return propertyValue;
        }

        /// <summary>
        /// Validates that the entry has key properties.
        /// </summary>
        /// <param name="keyProperties">Key properties of the entry.</param>
        /// <param name="actualEntityTypeName">The entity type name of the entry.</param>
        private static void ValidateEntityTypeHasKeyProperties(KeyValuePair<string, object>[] keyProperties, string actualEntityTypeName)
        {
            Debug.Assert(keyProperties != null, "keyProperties != null");
            if (keyProperties == null || keyProperties.Length == 0)
            {
                throw new ODataException(OData.Core.Strings.ODataEntryMetadataContext_EntityTypeWithNoKeyProperties(actualEntityTypeName));
            }
        }

        /// <summary>
        /// Gets the property name value pairs filtered by serialization property kind.
        /// </summary>
        /// <param name="entry">The entry to get the properties from.</param>
        /// <param name="propertyKind">The serialization info property kind.</param>
        /// <param name="actualEntityTypeName">The entity type name of the entry.</param>
        /// <returns>The property name value pairs filtered by serialization property kind.</returns>
        private static KeyValuePair<string, object>[] GetPropertiesBySerializationInfoPropertyKind(ODataEntry entry, ODataPropertyKind propertyKind, string actualEntityTypeName)
        {
            Debug.Assert(entry != null, "entry != null");
            Debug.Assert(propertyKind == ODataPropertyKind.Key || propertyKind == ODataPropertyKind.ETag, "propertyKind == ODataPropertyKind.Key || propertyKind == ODataPropertyKind.ETag");

            KeyValuePair<string, object>[] properties = EmptyProperties;
            if (entry.NonComputedProperties != null)
            {
                properties = entry.NonComputedProperties.Where(p => p.SerializationInfo != null && p.SerializationInfo.PropertyKind == propertyKind).Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitivePropertyClrValue(actualEntityTypeName, p, propertyKind == ODataPropertyKind.Key))).ToArray();
            }

            return properties;
        }

        /// <summary>
        /// Implementation of <see cref="IODataEntryMetadataContext"/> based on serialization info.
        /// </summary>
        private sealed class ODataEntryMetadataContextWithoutModel : ODataEntryMetadataContext
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
            /// The serialization info of the entry for writing without model.
            /// </summary>
            private readonly ODataFeedAndEntrySerializationInfo serializationInfo;

            /// <summary>
            /// Constructs an instance of <see cref="ODataEntryMetadataContextWithoutModel"/>.
            /// </summary>
            /// <param name="entry">The entry instance.</param>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
            /// <param name="serializationInfo">The serialization info of the entry for writing without model.</param>
            internal ODataEntryMetadataContextWithoutModel(ODataEntry entry, IODataFeedAndEntryTypeContext typeContext, ODataFeedAndEntrySerializationInfo serializationInfo)
                : base(entry, typeContext)
            {
                Debug.Assert(serializationInfo != null, "serializationInfo != null");
                this.serializationInfo = serializationInfo;
            }

            /// <summary>
            /// The key property name and value pairs of the entry.
            /// </summary>
            public override ICollection<KeyValuePair<string, object>> KeyProperties
            {
                get
                {
                    if (this.keyProperties == null)
                    {
                        this.keyProperties = GetPropertiesBySerializationInfoPropertyKind(this.entry, ODataPropertyKind.Key, this.ActualEntityTypeName);
                        ValidateEntityTypeHasKeyProperties(this.keyProperties, this.ActualEntityTypeName);
                    }

                    return this.keyProperties;
                }
            }

            /// <summary>
            /// The ETag property name and value pairs of the entry.
            /// </summary>
            public override IEnumerable<KeyValuePair<string, object>> ETagProperties
            {
                get { return this.etagProperties ?? (this.etagProperties = GetPropertiesBySerializationInfoPropertyKind(this.entry, ODataPropertyKind.ETag, this.ActualEntityTypeName)); }
            }

            /// <summary>
            /// The actual entity type of the entry, i.e. ODataEntry.TypeName.
            /// </summary>
            public override string ActualEntityTypeName
            {
                get
                {
                    if (String.IsNullOrEmpty(this.Entry.TypeName))
                    {
                        throw new ODataException(OData.Core.Strings.ODataFeedAndEntryTypeContext_ODataEntryTypeNameMissing);
                    }

                    return this.Entry.TypeName;
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
        /// Implementation of <see cref="IODataEntryMetadataContext"/> based on the given model.
        /// </summary>
        private sealed class ODataEntryMetadataContextWithModel : ODataEntryMetadataContext
        {
            /// <summary>
            /// The entity type of the entry.
            /// </summary>
            private readonly IEdmEntityType actualEntityType;

            /// <summary>
            /// The metadata context to use.
            /// </summary>
            private readonly IODataMetadataContext metadataContext;

            /// <summary>
            /// The selected properties.
            /// </summary>
            private readonly SelectedPropertiesNode selectedProperties;

            /// <summary>
            /// Constructs an instance of <see cref="ODataEntryMetadataContextWithModel"/>.
            /// </summary>
            /// <param name="entry">The entry instance.</param>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
            /// <param name="actualEntityType">The entity type of the entry.</param>
            /// <param name="metadataContext">The metadata context to use.</param>
            /// <param name="selectedProperties">The selected properties.</param>
            internal ODataEntryMetadataContextWithModel(ODataEntry entry, IODataFeedAndEntryTypeContext typeContext, IEdmEntityType actualEntityType, IODataMetadataContext metadataContext, SelectedPropertiesNode selectedProperties)
                : base(entry, typeContext)
            {
                Debug.Assert(actualEntityType != null, "actualEntityType != null");
                Debug.Assert(metadataContext != null, "metadataContext != null");
                Debug.Assert(selectedProperties != null, "selectedProperties != null");

                this.actualEntityType = actualEntityType;
                this.metadataContext = metadataContext;
                this.selectedProperties = selectedProperties;
            }

            /// <summary>
            /// The key property name and value pairs of the entry.
            /// </summary>
            public override ICollection<KeyValuePair<string, object>> KeyProperties
            {
                get
                {
                    if (this.keyProperties == null)
                    {
                        IEnumerable<IEdmStructuralProperty> edmKeyProperties = this.actualEntityType.Key();
                        if (edmKeyProperties != null)
                        {
                            this.keyProperties = edmKeyProperties.Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitivePropertyClrValue(this.entry, p.Name, this.ActualEntityTypeName, /*isKeyProperty*/true))).ToArray();
                        }

                        ValidateEntityTypeHasKeyProperties(this.keyProperties, this.ActualEntityTypeName);
                    }

                    return this.keyProperties;
                }
            }

            /// <summary>
            /// The ETag property name and value pairs of the entry.
            /// </summary>
            public override IEnumerable<KeyValuePair<string, object>> ETagProperties
            {
                get
                {
                    if (this.etagProperties == null)
                    {
                        IEnumerable<IEdmStructuralProperty> properties = this.ComputeETagPropertiesFromAnnotation();
                        if (properties.Any())
                        {
                            this.etagProperties = properties
                                .Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitivePropertyClrValue(this.entry, p.Name, this.ActualEntityTypeName, /*isKeyProperty*/false))).ToArray();
                        }
                        else
                        {
                            properties = this.actualEntityType.StructuralProperties();
                            this.etagProperties = properties != null 
                                ? properties
                                    .Where(p => p.ConcurrencyMode == EdmConcurrencyMode.Fixed)
                                    .Select(p => new KeyValuePair<string, object>(p.Name, GetPrimitivePropertyClrValue(this.entry, p.Name, this.ActualEntityTypeName, /*isKeyProperty*/false))).ToArray() 
                                : EmptyProperties;
                        }
                    }

                    return this.etagProperties;
                }
            }

            /// <summary>
            /// The actual entity type name of the entry.
            /// </summary>
            public override string ActualEntityTypeName
            {
                // Note that entry.TypeName can be null. When that happens, we use the expected entity type as the actual entity type.
                get { return this.actualEntityType.FullName(); }
            }

            /// <summary>
            /// The selected navigation properties.
            /// </summary>
            public override IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties
            {
                get { return this.selectedNavigationProperties ?? (this.selectedNavigationProperties = this.selectedProperties.GetSelectedNavigationProperties(this.actualEntityType)); }
            }

            /// <summary>
            /// The selected stream properties.
            /// </summary>
            public override IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties
            {
                get { return this.selectedStreamProperties ?? (this.selectedStreamProperties = this.selectedProperties.GetSelectedStreamProperties(this.actualEntityType)); }
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
                        bool mustBeContainerQualified = this.metadataContext.OperationsBoundToEntityTypeMustBeContainerQualified(this.actualEntityType);
                        this.selectedBindableOperations = this.metadataContext.GetBindableOperationsForType(this.actualEntityType)
                            .Where(operation => this.selectedProperties.IsOperationSelected(this.actualEntityType, operation, mustBeContainerQualified))
                            .ToArray();
                    }

                    return this.selectedBindableOperations;
                }
            }

            /// <summary>
            /// Compute ETag from Annotation Org.OData.Core.V1.OptimisticConcurrencyControl on EntitySet
            /// </summary>
            /// <returns>Enumerable of IEdmStructuralProperty</returns>
            private IEnumerable<IEdmStructuralProperty> ComputeETagPropertiesFromAnnotation()
            {
                IEdmModel model = this.metadataContext.Model;
                IEdmEntitySet entitySet = model.FindDeclaredEntitySet(this.typeContext.NavigationSourceName);

                if (entitySet != null)
                {
                    IEdmVocabularyAnnotation annotation = model.FindDeclaredVocabularyAnnotations(entitySet)
                        .SingleOrDefault(t => t.Term.FullName().Equals(CoreVocabularyConstants.CoreOptimisticConcurrencyControl, StringComparison.Ordinal));
                    if (annotation is IEdmValueAnnotation)
                    {
                        IEdmExpression collectionExpression = (annotation as IEdmValueAnnotation).Value;
                        if (collectionExpression is IEdmCollectionExpression)
                        {
                            IEnumerable<IEdmExpression> pathExpressions = (collectionExpression as IEdmCollectionExpression).Elements.Where(p => p is IEdmPathExpression);
                            foreach (IEdmPathExpression pathExpression in pathExpressions)
                            {
                                // TODO: 
                                //  1. Add support for Complex type
                                //  2. Add new exception when collectionExpression is not IEdmCollectionExpression: CoreOptimisticConcurrencyControl must be followed by collection expression
                                IEdmStructuralProperty property = this.actualEntityType.StructuralProperties().FirstOrDefault(p => p.Name == pathExpression.Path.LastOrDefault());
                                if (property == null)
                                {
                                    throw new ODataException(Core.Strings.EdmValueUtils_PropertyDoesntExist(this.ActualEntityTypeName, pathExpression.Path.LastOrDefault()));
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
