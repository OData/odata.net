//---------------------------------------------------------------------
// <copyright file="PayloadGeneratorExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Extension methods used by the <see cref="ReaderPayloadGenerator"/> class.
    /// </summary>
    public static class PayloadGeneratorExtensions
    {
        /// <summary>
        /// Puts the specified <paramref name="payload"/> into an expanded link inside of a newly constructed entry.
        /// </summary>
        /// <param name="payload">The payload to be used as content for the expanded link.</param>
        /// <param name="isSingletonRelationship">true if the navigation property is of singleton cardinality; false for a cardinality many. Default is false.</param>
        /// <param name="randomDataGeneratorResolver">Random dataGeneration resolver</param>
        /// <param name="randomNumberGenerator">random number generator</param>
        /// <returns>An entry payload with an expanded link that contains the specified <paramref name="payload"/>.</returns>
        public static T InEntryWithExpandedLink<T>(
            this T payload,
            bool isSingletonRelationship = false,
            IRandomDataGeneratorResolver randomDataGeneratorResolver = null,
            IRandomNumberGenerator randomNumberGenerator = null) where T : PayloadTestDescriptor
        {
            ODataPayloadKind payloadKind = payload.PayloadElement.GetPayloadKindFromPayloadElement();
            Debug.Assert(
                payloadKind == ODataPayloadKind.Resource || payloadKind == ODataPayloadKind.ResourceSet,
                "Expanded links can only contain entries and feeds.");

            if (payload.PayloadEdmModel != null)
            {
                return payload.InEdmEntryWithExpandedLink(isSingletonRelationship, randomDataGeneratorResolver, randomNumberGenerator);
            }
            
            EntityInstance entityInstance = PayloadBuilder.Entity(null)
                .PrimitiveProperty("Id", 1)
                .PrimitiveProperty("Name", string.Empty);

            if (randomDataGeneratorResolver != null && randomNumberGenerator != null)
            {
                entityInstance.OverwriteWithRandomPropertyValues(randomDataGeneratorResolver, randomNumberGenerator);
            }

            EntitySetInstance feed = payload.PayloadElement as EntitySetInstance;
            if (feed != null)
            {
                feed.InlineCount = null;
            }

            entityInstance.ExpandedNavigationProperty("NavigationProp", feed == null ? payload.PayloadElement : feed, "http://services.odata.org/OData.svc/Table(1)/NavigationProp");

            T entryPayloadDescriptor = (T)payload.Clone();
            entryPayloadDescriptor.PayloadEdmModel = null;
            entryPayloadDescriptor.PayloadElement = entityInstance;

            return entryPayloadDescriptor;

        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into an expanded link inside of a newly constructed entry.
        /// </summary>
        /// <param name="payload">The payload to be used as content for the expanded link.</param>
        /// <param name="isSingletonRelationship">true if the navigation property is of singleton cardinality; false for a cardinality many. Default is false.</param>
        /// <param name="randomDataGeneratorResolver">Random dataGeneration resolver</param>
        /// <param name="randomNumberGenerator">random number generator</param>
        /// <returns>An entry payload with an expanded link that contains the specified <paramref name="payload"/>.</returns>
        public static T InEdmEntryWithExpandedLink<T>(
            this T payload,
            bool isSingletonRelationship = false,
            IRandomDataGeneratorResolver randomDataGeneratorResolver = null,
            IRandomNumberGenerator randomNumberGenerator = null) where T : PayloadTestDescriptor
        {
            ODataPayloadKind payloadKind = payload.PayloadElement.GetPayloadKindFromPayloadElement();
            Debug.Assert(
                payloadKind == ODataPayloadKind.Resource || payloadKind == ODataPayloadKind.ResourceSet,
                "Expanded links can only contain entries and feeds.");

            EdmEntityType entityType = null;
            // create an entity type with a navigation property for the specified payload
            EdmModel model = payload.PayloadEdmModel as EdmModel;
            if (model != null)
            {
                IEdmEntityType payloadEntityType = ModelBuilder.GetPayloadElementEntityType(payload.PayloadElement, model);
                // Adding a container always adds 2 (for backcompat) so we must use first instead of single
                var container = model.EntityContainer as EdmEntityContainer;
                // create a new entity type with two fixed properties
                string entityTypeName = "PGEntityTypeWithExpandedLink" + ModelBuilder.NextUniqueId();
                entityType = new EdmEntityType("TestModel", entityTypeName);
                entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
                entityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
                var navProp = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "NavigationProp", Target = payloadEntityType, TargetMultiplicity = isSingletonRelationship ? EdmMultiplicity.One : EdmMultiplicity.Many });
                model.AddElement(entityType);
                var entitySet = container.AddEntitySet(entityTypeName, entityType);
                var targetSet = container.EntitySets().Where(s => payloadEntityType.IsOrInheritsFrom(s.EntityType())).Single();
                entitySet.AddNavigationTarget(navProp, targetSet);
            }
            string fullTypeName = entityType == null ? null : entityType.FullName();
            EntityInstance entityInstance = PayloadBuilder.Entity(fullTypeName)
                .PrimitiveProperty("Id", 1)
                .PrimitiveProperty("Name", string.Empty);

            if (randomDataGeneratorResolver != null && randomNumberGenerator != null)
            {
                entityInstance.OverwriteWithRandomPropertyValues(randomDataGeneratorResolver, randomNumberGenerator);
            }

            EntitySetInstance feed = payload.PayloadElement as EntitySetInstance;
            if (feed != null)
            {
                feed.InlineCount = null;
            }

            entityInstance.ExpandedNavigationProperty("NavigationProp", feed == null ? payload.PayloadElement : feed, "http://services.odata.org/OData.svc/Table(1)/NavigationProp");

            if (entityType != null)
            {
                entityInstance = entityInstance.WithTypeAnnotation(entityType);
            }

            T entryPayloadDescriptor = (T)payload.Clone();
            entryPayloadDescriptor.PayloadEdmModel = model;
            entryPayloadDescriptor.PayloadElement = entityInstance;

            return entryPayloadDescriptor;
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a nested expanded feed hierachy, inside a
        /// top level entry.
        /// </summary>
        /// <typeparam name="T">the payload test descriptor type.</typeparam>
        /// <param name="payload">The payload to be used inside of the feed. This must represent an entity instance payload.</param>
        /// <param name="nestingLevel">The depth of the expanded feed hierachy.</param>
        /// <returns>An entry payload with the <paramref name="payload"/> nested within <paramref name="nestedLevel"/> expanded feeds.</returns>  
        public static T InEntryWithNestedExpandedFeeds<T>(this T payload, int nestingLevel) where T : PayloadTestDescriptor
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");
            ExceptionUtilities.Assert(nestingLevel > 0, "Nesting level must be a positive integer");

            T entry = payload;
            for (int i = 0; i < nestingLevel; ++i)
            {
                entry = entry.InFeed().InEntryWithExpandedLink();
            }
            return entry;
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a feed. It accepts optional parameters to control other
        /// properties of the feed and the position of the <paramref name="payload"/> inside the feed.
        /// </summary>
        /// <param name="payload">The paylaod to be used inside of the feed. This must represent an entity instance payload.</param>
        /// <param name="inlineCount">An optional inline count value for the feed.</param>
        /// <param name="nextLink">An optional next-link value for the feed.</param>
        /// <param name="elementsBefore">An optional number of entries that should exist before the <paramref name="payload"/> in the feed.</param>
        /// <param name="elementsAfter">An optional number of entries that should exist after the <paramref name="payload"/> in the feed.</param>
        /// <returns>A feed payload with the <paramref name="payload"/> as one of its entities.</returns>
        public static T InFeed<T>(
            this T payload,
            long? inlineCount = null,
            string nextLink = null,
            int elementsBefore = 0,
            int elementsAfter = 0) where T : PayloadTestDescriptor
        {
            Debug.Assert(payload.PayloadElement.GetPayloadKindFromPayloadElement() == ODataPayloadKind.Resource, "only entries are supported.");
            EntityInstance payloadEntity = (EntityInstance)payload.PayloadElement;

            EntitySetInstance entitySetInstance = PayloadBuilder.EntitySet();
            
            if (payload.PayloadEdmModel != null)
            {
                entitySetInstance.WithTypeAnnotation(ModelBuilder.GetPayloadElementEntityType(payloadEntity, payload.PayloadEdmModel));
            }

            entitySetInstance.AddRange(payloadEntity.GenerateSimilarEntries(elementsBefore));
            entitySetInstance.Add(payload.PayloadElement);
            entitySetInstance.AddRange(payloadEntity.GenerateSimilarEntries(elementsAfter));
            entitySetInstance.InlineCount((int?)inlineCount).NextLink(nextLink);
            bool hasInlineCountOrNextLink = inlineCount.HasValue || nextLink != null;

            T feedPayloadDescriptor = (T)payload.Clone();
            feedPayloadDescriptor.PayloadEdmModel = payload.PayloadEdmModel;
            feedPayloadDescriptor.PayloadElement = entitySetInstance;
            feedPayloadDescriptor.SkipTestConfiguration = tc =>
            {
                bool skip = payload.SkipTestConfiguration == null ? false : payload.SkipTestConfiguration(tc);

                // Inline count or next link are valid only in V2 and in responses
                return skip || (hasInlineCountOrNextLink && tc.IsRequest);
            };

            return feedPayloadDescriptor;
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a primitive or complex collection.
        /// </summary>
        /// <param name="payload">The payload to put into a collection. This payload must represent a value.</param>
        /// <param name="propertiesValuesBefore">Number of properties to put in the complex value before the <paramref name="payload"/>.</param>
        /// <param name="propertiesValuesAfter">Number of properties to put in the complex value after the <paramref name="payload"/>.</param>
        /// <returns>A collection payload with the <paramref name="payload"/> as one of its properties.</returns>
        public static T InCollection<T>(
            this T payload,
            int propertiesValuesBefore = 0,
            int propertiesValuesAfter = 0) where T : PayloadTestDescriptor
        {
            PrimitiveValue primitiveValue = payload.PayloadElement as PrimitiveValue;
            ComplexInstance complexValue = payload.PayloadElement as ComplexInstance;
            ExceptionUtilities.Assert(primitiveValue != null || complexValue != null, "payload must be either primitive or complex value");
            var edmModel = payload.PayloadEdmModel;
            if (primitiveValue != null)
            {
                var payloadTypeAnnotation = primitiveValue.Annotations.OfType<EntityModelTypeAnnotation>().SingleOrDefault();
                ExceptionUtilities.Assert(edmModel == null || payloadTypeAnnotation != null, "value payload should have a EntityModelTypeAnnotation");

                var extraValues = TestValues.CreatePrimitiveValuesWithMetadata(true).Where(value => value != null && value.ClrValue != null &&
                    value.ClrValue.GetType() == primitiveValue.ClrValue.GetType());
                var values = BuildPayloadElements(propertiesValuesBefore, propertiesValuesAfter, primitiveValue, extraValues.ToArray());

                // Create collection
                var primitiveCollection = PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName(payloadTypeAnnotation.EdmModelType.FullName()));

                foreach (var v in values)
                {
                    primitiveCollection.Add(v);
                }
                if (edmModel != null)
                {
                    primitiveCollection.WithTypeAnnotation(EdmCoreModel.GetCollection(payloadTypeAnnotation.EdmModelType));
                }
                T collectionDescriptor = (T)payload.Clone();
                collectionDescriptor.PayloadElement = primitiveCollection;
                collectionDescriptor.PayloadEdmModel = edmModel;
                return collectionDescriptor;
            }
            else
            {
                var payloadTypeAnnotation = complexValue.Annotations.OfType<EntityModelTypeAnnotation>().SingleOrDefault();
                ExceptionUtilities.Assert(edmModel == null || payloadTypeAnnotation != null, "value payload should have a EntityModelTypeAnnotation");

                List<ComplexInstance> values = new List<ComplexInstance>(
                    Enumerable.Range(0, propertiesValuesBefore).Select(i => TestValues.GetComplexValueWithDifferentValues(complexValue)));
                values.Add(complexValue);
                values.AddRange(Enumerable.Range(0, propertiesValuesAfter).Select(i => TestValues.GetComplexValueWithDifferentValues(complexValue)));

                // Create collection
                string collectionTypeName = null;
                ComplexMultiValue complexMultiValue = null;
                if (edmModel != null)
                {
                    var complexDataType = payloadTypeAnnotation.EdmModelType;
                    ExceptionUtilities.CheckObjectNotNull(complexDataType, "complexValue must have a ComplexDataType annotation");
                    collectionTypeName = EntityModelUtils.GetCollectionTypeName(payloadTypeAnnotation.EdmModelType.FullName());
                    complexMultiValue = PayloadBuilder.ComplexMultiValue(collectionTypeName);
                    foreach (var v in values)
                    {
                        complexMultiValue.Add(v);
                    }

                    complexMultiValue.WithTypeAnnotation(EdmCoreModel.GetCollection(payloadTypeAnnotation.EdmModelType));
                }
                else
                {
                    complexMultiValue = PayloadBuilder.ComplexMultiValue();

                    foreach (var v in values)
                    {
                        complexMultiValue.Add(v);
                    }
                }

                T collectionDescriptor = (T)payload.Clone();
                collectionDescriptor.PayloadElement = complexMultiValue;
                collectionDescriptor.PayloadEdmModel = edmModel;
                return collectionDescriptor;

            }
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a complex value.
        /// </summary>
        /// <param name="payload">The payload to put into a complex value. This payload must represent a property instance.</param>
        /// <param name="propertiesBefore">Number of properties to put in the complex value before the <paramref name="payload"/>.</param>
        /// <param name="propertiesAfter">Number of properties to put in the complex value after the <paramref name="payload"/>.</param>
        /// <param name="version">Highest version feature to include.</param>
        /// <returns>A complex value payload with the <paramref name="payload"/> as one of its properties.</returns>
        public static T InComplexValue<T>(
            this T payload,
            int propertiesBefore = 0,
            int propertiesAfter = 0,
            ODataVersion version = ODataVersion.V4) where T : PayloadTestDescriptor
        {
            PropertyInstance propertyInstance = payload.PayloadElement as PropertyInstance;
            Debug.Assert(propertyInstance != null, "InComplexValue can only be applied to property instances.");

            EdmModel edmModel = (EdmModel) payload.PayloadEdmModel;
            // Build the list of all properties including the extra ones and the property from payload
            IEnumerable<PropertyInstance> properties = BuildProperties(propertiesBefore, propertiesAfter, propertyInstance, ExtraProperties(ref edmModel, version));

            // Build the complex type and value
            string typename = "PGInComplexValue_ResultComplexType" + ModelBuilder.NextUniqueId();

            ComplexInstance complexValue = PayloadBuilder.ComplexValue("TestModel." + typename);
            
            if (edmModel != null)
            {
                EdmComplexType resultComplexType = 
                    edmModel.SchemaElements.OfType<EdmComplexType>().SingleOrDefault(type => type.Name == typename) 
                    ?? new EdmComplexType("TestModel", typename);

                foreach (var property in properties)
                {
                    resultComplexType.AddStructuralProperty(property.Name, ModelBuilder.GetPayloadEdmElementPropertyValueType(property));
                }

                edmModel.AddElement(resultComplexType);
                complexValue.WithTypeAnnotation(resultComplexType);
            }

            foreach (var p in properties)
            {
                complexValue.Add(p);
            }

            T complexDescriptor = (T)payload.Clone();
            complexDescriptor.PayloadElement = complexValue;
            complexDescriptor.PayloadEdmModel = edmModel;
            return complexDescriptor;
        }

        /// <summary>
        /// Builds specified properties around the property instance list using the extraProperties array
        /// </summary>
        /// <param name="propertiesBefore">properties before the property instance</param>
        /// <param name="propertiesAfter">properties before the property instance</param>
        /// <param name="propertyInstance">property instance around which to build additional properties</param>
        /// <param name="extraProperties">extra properties to build the additional properties</param>
        /// <returns>An IEnumerable of properties with specified number of properties before and after the property instance</returns>
        private static IEnumerable<PropertyInstance> BuildProperties(int propertiesBefore, int propertiesAfter, PropertyInstance propertyInstance,
            PropertyInstance[] extraProperties)
        {
            IEnumerable<PropertyInstance> properties = BuildPayloadElements<PropertyInstance>(propertiesBefore, propertiesAfter, propertyInstance,
                extraProperties);

            // Fix the names to have index in the suffix so that they are unique
            properties = properties.Select((p, i) =>
            {
                var newProperty = p.DeepCopy();
                if (p != propertyInstance)
                {
                    newProperty.Name = newProperty.Name + "_" + i.ToString();
                }

                return newProperty;
            });
            return properties;
        }

        /// <summary>
        /// Builds extra properties and updates the model
        /// </summary>
        /// <param name="model">model to update</param>
        /// <returns>extra properties</returns>
        private static PropertyInstance[] ExtraProperties(ref EdmModel model, ODataVersion version)
        {
            EdmComplexType extraComplexType = null;
            if (model != null)
            {
                extraComplexType = model.FindDeclaredType("TestModel.PGInComplexValue_ExtraComplexType") as EdmComplexType;

                // Generate a complex type for our extra property
                if (extraComplexType == null)
                {
                    //model = model.Clone();
                    extraComplexType = new EdmComplexType("TestModel", "PGInComplexValue_ExtraComplexType");
                    extraComplexType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
                    extraComplexType.AddStructuralProperty("Value", EdmCoreModel.Instance.GetInt32(false));
                    model.AddElement(extraComplexType);
                }
            }
            // Build the list of extra properties 
            var extraProperties = new PropertyInstance[]
            {
                PayloadBuilder.PrimitiveProperty("Extra_Number", 42),
                PayloadBuilder.Property("Extra_Complex", 
                    PayloadBuilder.ComplexValue(model == null ? null : extraComplexType.FullName())
                        .PrimitiveProperty("Name", "Value1")
                        .PrimitiveProperty("Value", 3)
                        .WithTypeAnnotation(model == null ? null : extraComplexType)),
                PayloadBuilder.PrimitiveProperty("Extra_Null", null).WithTypeAnnotation(model == null ? null : EdmCoreModel.Instance.GetInt32(true)),
            };
            if (version >= ODataVersion.V4)
            {
                extraProperties = extraProperties.Concat<PropertyInstance>(new PropertyInstance[]
                {
                    PayloadBuilder.Property("Extra_PrimitiveMultiValue", 
                        PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName(EdmConstants.EdmStringTypeName))
                            .Item("item value")
                            .WithTypeAnnotation(model == null ? null : EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)))),
                    PayloadBuilder.Property("Extra_ComplexMultiValue",
                        PayloadBuilder.ComplexMultiValue(model == null ? null : EntityModelUtils.GetCollectionTypeName(extraComplexType.FullName()))
                            // NOTE: if we create a collection without model we have to use a dummy type name - without a type
                            //       name we would assume string and fail since we find a complex value
                            .Item(PayloadBuilder.ComplexValue(model == null ? "DummyModel.DummyName" : extraComplexType.FullName())
                                .PrimitiveProperty("Name", "Value2")
                                .PrimitiveProperty("Value", 4)
                                .WithTypeAnnotation(model == null ? null : extraComplexType))
                            .WithTypeAnnotation(model == null ? null : EdmCoreModel.GetCollection(new EdmComplexTypeReference(extraComplexType, false))))
                }).ToArray();
            }
            return extraProperties;
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a property.
        /// </summary>
        /// <param name="payload">The payload to put into a property. This payload must represent a simple value (primitive, complex, collection).</param>
        /// <param name="propertyName">Optional name of the property to generate.</param>
        /// <returns>A property payload with the <paramref name="payload"/> as its value.</returns>
        public static T InProperty<T>(this T payload, string propertyName = null) where T : PayloadTestDescriptor
        {
            Debug.Assert(new[] { ODataPayloadElementType.PrimitiveValue, ODataPayloadElementType.ComplexInstance, ODataPayloadElementType.PrimitiveMultiValue, ODataPayloadElementType.ComplexMultiValue }.Contains(payload.PayloadElement.ElementType),
                "Only simple properties are supported, so the input payload must be a primitive value, complex value or collection");

            PropertyInstance property = PayloadBuilder.Property(propertyName, payload.PayloadElement);
            
            if (payload.PayloadEdmModel != null)
            {
                EntityModelTypeAnnotation typeAnnotation = payload.PayloadElement.GetAnnotation<EntityModelTypeAnnotation>();
                IEdmTypeReference edmDataType = typeAnnotation == null ? null : typeAnnotation.EdmModelType;
                //TODO: May be need to try to get the value from PayloadElement if TypeAnnotations is null, just may be
                if (edmDataType != null)
                {
                    property = property.ExpectedPropertyType(edmDataType);
                }
            }

            T propertyTestDescriptor = (T)payload.Clone();
            propertyTestDescriptor.PayloadElement = property;
            return propertyTestDescriptor;
        }

        //TODO: replace InEntity with InEdmEntity and move InEntity to Microsoft.Test.OData.Utils.ODataLibTest
        /// <summary>
        /// Puts the specified <paramref name="payload"/> into an entity.
        /// </summary>
        /// <param name="payload">The payload to put into an entity. This payload must represent a property instance.</param>
        /// <param name="propertiesBefore">Number of properties to put in the entity before the <paramref name="payload"/>.</param>
        /// <param name="propertiesAfter">Number of properties to put in the entity after the <paramref name="payload"/>.</param>
        /// <param name="version">Maximum version feature to include.</param>
        /// <returns>An entity payload with the <paramref name="payload"/> as one of its properties.</returns>
        public static T InEntity<T>(
            this T payload,
            int propertiesBefore = 0,
            int propertiesAfter = 0,
            ODataVersion version = ODataVersion.V4) where T : PayloadTestDescriptor
        {
            PropertyInstance propertyInstance = (payload.PayloadElement as PropertyInstance).DeepCopy();
            Debug.Assert(propertyInstance != null, "InEntity can only be applied to property instances.");

            IEdmModel edmModel = payload.PayloadEdmModel;
            if (edmModel != null)
            {
                return payload.InEdmEntity(propertiesBefore, propertiesAfter, version);
            }

            // Build the list of extra properties 
            var extraProperties = new PropertyInstance[]
            {
                PayloadBuilder.PrimitiveProperty("Extra_Number", 42),
                PayloadBuilder.Property("Extra_Complex", 
                    PayloadBuilder.ComplexValue("TestModel.PGInEntity_ExtraComplexType")
                        .PrimitiveProperty("Name", "Value1")
                        .PrimitiveProperty("Value", 3)),
                PayloadBuilder.PrimitiveProperty("Extra_Null", null).WithTypeAnnotation(EdmCoreModel.Instance.GetInt32(true)),
                PayloadBuilder.NavigationProperty("Extra_DeferredSingletonNav", "http://odata.org/singletonnav"),
                PayloadBuilder.NavigationProperty("Extra_DeferredCollectionNav", "http://odata.org/collectionnav"),
                PayloadBuilder.ExpandedNavigationProperty("Extra_SingletonNav", PayloadBuilder.Entity("TestModel.PGInEntity_ExtraEntityType").PrimitiveProperty("ID", 1), "http://www.odata.org/link"),
                PayloadBuilder.ExpandedNavigationProperty("Extra_CollectionNav", PayloadBuilder.EntitySet(), "http://www.odata.org/link"),
            };

            extraProperties = extraProperties.Concat<PropertyInstance>(new PropertyInstance[]
                {
                    PayloadBuilder.StreamProperty("Extra_Stream", "http://odata.org/namedstream/readlink", "http://odata.org/namedstream/editlink"),
                    PayloadBuilder.Property("Extra_PrimitiveCollection", 
                        PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName(EdmConstants.EdmStringTypeName))
                            .Item("item value")),
                    PayloadBuilder.Property("Extra_ComplexCollection",
                        PayloadBuilder.ComplexMultiValue(EntityModelUtils.GetCollectionTypeName("TestModel.PGInEntity_ExtraComplexType"))
                            .Item(PayloadBuilder.ComplexValue("TestModel.PGInEntity_ExtraComplexType")
                                .PrimitiveProperty("Name", "Value2")
                                .PrimitiveProperty("Value", 4)))
                }).ToArray();

            // Build the list of all properties
            IEnumerable<PropertyInstance> properties = Enumerable.Range(0, propertiesBefore)
                .Select(i => extraProperties[i % extraProperties.Length]);
            properties = properties.ConcatSingle(propertyInstance);
            properties = properties.Concat(Enumerable.Range(0, propertiesAfter)
                .Select(i => extraProperties[extraProperties.Length - 1 - (i % extraProperties.Length)]));

            // Fix the names to have index in the suffix so that they are unique
            properties = properties.Select((p, i) =>
            {
                if (p == propertyInstance)
                {
                    // Don't modify the property we're wrapping since the name and such might be important.
                    return p;
                }
                else
                {
                    var newProperty = p.DeepCopy();
                    newProperty.Name = newProperty.Name + "_" + i.ToString();
                    return newProperty;
                }
            }).ToList();
            
            // Build the entity
            EntityInstance entityInstance = PayloadBuilder.Entity("TestModel.PGInEntity_ResultEntityType").PrimitiveProperty("ID", 1);
            foreach (var p in properties)
            {
                entityInstance.Add(p);
            }
            T entityDescriptor = (T)payload.Clone();
            entityDescriptor.PayloadElement = entityInstance;
            entityDescriptor.PayloadEdmModel = null;
            return entityDescriptor;
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into an entity.
        /// </summary>
        /// <param name="payload">The payload to put into an entity. This payload must represent a property instance.</param>
        /// <param name="propertiesBefore">Number of properties to put in the entity before the <paramref name="payload"/>.</param>
        /// <param name="propertiesAfter">Number of properties to put in the entity after the <paramref name="payload"/>.</param>
        /// <param name="version">Maximum version feature to include.</param>
        /// <returns>An entity payload with the <paramref name="payload"/> as one of its properties.</returns>
        public static T InEdmEntity<T>(
            this T payload,
            int propertiesBefore = 0,
            int propertiesAfter = 0,
            ODataVersion version = ODataVersion.V4) where T : PayloadTestDescriptor
        {
            PropertyInstance propertyInstance = (payload.PayloadElement as PropertyInstance).DeepCopy();
            Debug.Assert(propertyInstance != null, "InEntity can only be applied to property instances.");

            IEdmModel model = payload.PayloadEdmModel;
            EdmComplexType extraComplexType = null;
            EdmEntityType extraEntityType = null;
            if (model != null)
            {
                //model = model.Clone();
                extraComplexType = model.FindDeclaredType("TestModel.PGInEntity_ExtraComplexType") as EdmComplexType;
                extraEntityType = model.FindDeclaredType("TestModel.PGInEntity_ExtraEntityType") as EdmEntityType;
                var container = model.EntityContainer as EdmEntityContainer;
                if (extraComplexType == null)
                {
                    // Generate a complex type for our extra property
                    extraComplexType = new EdmComplexType("TestModel", "PGInEntity_ExtraComplexType");
                    extraComplexType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
                    extraComplexType.AddStructuralProperty("Value", EdmCoreModel.Instance.GetInt32(false));
                    ((EdmModel)model).AddElement(extraComplexType);
                }

                if (extraEntityType == null)
                {
                    // Add an entity type for nav. props to use
                    extraEntityType = new EdmEntityType("TestModel", "PGInEntity_ExtraEntityType");
                    extraEntityType.AddKeys(extraEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
                    ((EdmModel)model).AddElement(extraEntityType);
                    container.AddEntitySet("PGInEntity_ExtraEntitySet", extraEntityType);
                }
            }


            // Build the list of extra properties 
            var extraProperties = new PropertyInstance[]
            {
                PayloadBuilder.PrimitiveProperty("Extra_Number", 42),
                PayloadBuilder.Property("Extra_Complex", 
                    PayloadBuilder.ComplexValue("TestModel.PGInEntity_ExtraComplexType")
                        .PrimitiveProperty("Name", "Value1")
                        .PrimitiveProperty("Value", 3)
                        .WithTypeAnnotation(extraComplexType)),
                PayloadBuilder.PrimitiveProperty("Extra_Null", null).WithTypeAnnotation(EdmCoreModel.Instance.GetInt32(true)),
                PayloadBuilder.NavigationProperty("Extra_DeferredSingletonNav", "http://odata.org/singletonnav"),
                PayloadBuilder.NavigationProperty("Extra_DeferredCollectionNav", "http://odata.org/collectionnav"),
                PayloadBuilder.ExpandedNavigationProperty("Extra_SingletonNav", PayloadBuilder.Entity("TestModel.PGInEntity_ExtraEntityType").PrimitiveProperty("ID", 1), "http://www.odata.org/link"),
                PayloadBuilder.ExpandedNavigationProperty("Extra_CollectionNav", PayloadBuilder.EntitySet().WithTypeAnnotation(extraEntityType), "http://www.odata.org/link"),
            };
            if (version >= ODataVersion.V4)
            {
                extraProperties = extraProperties.Concat<PropertyInstance>(new PropertyInstance[]
                {
                    PayloadBuilder.StreamProperty("Extra_Stream", "http://odata.org/namedstream/readlink", "http://odata.org/namedstream/editlink"),
                    PayloadBuilder.Property("Extra_PrimitiveCollection", 
                        PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName(EdmConstants.EdmStringTypeName))
                            .Item("item value")
                            .WithTypeAnnotation(model == null ? null : EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)) as IEdmCollectionType)),
                    PayloadBuilder.Property("Extra_ComplexCollection",
                        PayloadBuilder.ComplexMultiValue(EntityModelUtils.GetCollectionTypeName("TestModel.PGInEntity_ExtraComplexType"))
                            .Item(PayloadBuilder.ComplexValue("TestModel.PGInEntity_ExtraComplexType")
                                .PrimitiveProperty("Name", "Value2")
                                .PrimitiveProperty("Value", 4)
                                .WithTypeAnnotation(extraComplexType))
                            .WithTypeAnnotation(extraComplexType == null ? null : EdmCoreModel.GetCollection(new EdmComplexTypeReference(extraComplexType, false)) as IEdmCollectionType))
                }).ToArray();
            }

            // Build the list of all properties
            //TODO: need change to use EdmModel type
            IEnumerable<PropertyInstance> properties = Enumerable.Range(0, propertiesBefore)
                .Select(i => extraProperties[i % extraProperties.Length]);
            properties = properties.ConcatSingle(propertyInstance);
            properties = properties.Concat(Enumerable.Range(0, propertiesAfter)
                .Select(i => extraProperties[extraProperties.Length - 1 - (i % extraProperties.Length)]));

            // Fix the names to have index in the suffix so that they are unique
            properties = properties.Select((p, i) =>
            {
                if (p == propertyInstance)
                {
                    // Don't modify the property we're wrapping since the name and such might be important.
                    return p;
                }
                else
                {
                    var newProperty = p.DeepCopy();
                    newProperty.Name = newProperty.Name + "_" + i.ToString();
                    return newProperty;
                }
            }).ToList();

            // Build the entity type
            EdmEntityType resultEntityType = null;
            EdmEntitySet resultSet = null;
            if (model != null)
            {
                resultEntityType = model.FindDeclaredType("TestModel.PGInEntity_ResultEntityType") as EdmEntityType;
                var container = model.EntityContainer as EdmEntityContainer;
                if (resultEntityType == null)
                {
                    resultEntityType = new EdmEntityType("TestModel", "PGInEntity_ResultEntityType");
                    resultEntityType.AddKeys(resultEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
                    ((EdmModel)model).AddElement(resultEntityType);
                    resultSet = container.AddEntitySet("PGInEntity_ResultEntitySet", resultEntityType);
                }
                else
                {
                    resultSet = container.EntitySets().Where(s => resultEntityType.IsOrInheritsFrom(s.EntityType())).Single() as EdmEntitySet;
                }
                foreach (var p in properties)
                {
                    NavigationPropertyInstance navProp = p as NavigationPropertyInstance;
                    if (navProp == null)
                    {
                        if (resultEntityType.FindProperty(p.Name) == null)
                        {
                            if (p.Name.Contains("Number"))
                            {
                                resultEntityType.AddStructuralProperty(p.Name, EdmCoreModel.Instance.GetInt32(false));
                            }
                            else if (p.Name.Contains("Null"))
                            {
                                resultEntityType.AddStructuralProperty(p.Name, EdmCoreModel.Instance.GetInt32(true));
                            }
                            else if (p.Name.Contains("PrimitiveCollection"))
                            {
                                resultEntityType.AddStructuralProperty(p.Name, EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
                            }
                            else if (p.Name.Contains("ComplexCollection"))
                            {
                                resultEntityType.AddStructuralProperty(p.Name, EdmCoreModel.GetCollection(new EdmComplexTypeReference(extraComplexType, false)));
                            }
                            else if (p.Name.Contains("Stream"))
                            {
                                resultEntityType.AddStructuralProperty(p.Name, EdmPrimitiveTypeKind.Stream, isNullable: false);
                            }
                            else
                            {
                                resultEntityType.AddStructuralProperty(p.Name, ModelBuilder.GetPayloadEdmElementPropertyValueType(p) as EdmTypeReference);
                            }
                        }

                    }
                    else if (resultEntityType.FindProperty(navProp.Name) == null)
                    {
                        IEdmEntityType navigationPropertyType;
                        EntityModelTypeAnnotation typeAnnotation = navProp.GetAnnotation<EntityModelTypeAnnotation>();
                        if (typeAnnotation != null)
                        {
                            // get this type from the cloned model
                            var type = typeAnnotation.EdmModelType;
                            navigationPropertyType = model.FindDeclaredType(type.FullName()) as IEdmEntityType;
                        }
                        else
                        {
                            navigationPropertyType = extraEntityType;
                        }

                        NavigationPropertyCardinalityAnnotation navigationPropertyCardinality = navProp.GetAnnotation<NavigationPropertyCardinalityAnnotation>();
                        bool isCollection = !navProp.Name.Contains("Singleton");
                        if (navigationPropertyCardinality != null && navigationPropertyCardinality.IsCollection.HasValue)
                        {
                            isCollection = navigationPropertyCardinality.IsCollection.Value;
                        }
                        else if (navigationPropertyCardinality == null)
                        {
                            navigationPropertyCardinality = new NavigationPropertyCardinalityAnnotation() { IsCollection = isCollection };
                            navProp.SetAnnotation(navigationPropertyCardinality);
                        }
                        var resultNavProp = resultEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = navProp.Name, Target = navigationPropertyType, TargetMultiplicity = isCollection ? EdmMultiplicity.Many : EdmMultiplicity.One });
                        var targetSet = container.EntitySets().Where(s => navigationPropertyType.IsOrInheritsFrom(s.EntityType())).Single();
                        resultSet.AddNavigationTarget(resultNavProp, targetSet);
                    }
                }
            }

            // Build the entity
            EntityInstance entityInstance = PayloadBuilder.Entity("TestModel.PGInEntity_ResultEntityType").PrimitiveProperty("ID", 1);
            foreach (var p in properties)
            {
                entityInstance.Add(p);
            }
            T entityDescriptor = (T)payload.Clone();
            entityDescriptor.PayloadElement = entityInstance.WithTypeAnnotation(resultEntityType);
            entityDescriptor.PayloadEdmModel = model;
            return entityDescriptor;
        }

        /// <summary>
        /// Takes the <paramref name="navPropertyPayload"/> that represents a deferred navigation property and expands it by creating a new entity type,
        /// adding it to the model and then creating <paramref name="count"/> entities for the new type that act as content of the expanded link.
        /// </summary>
        /// <param name="navPropertyPayload">The deferred navigation property payload to be expanded.</param>
        /// <param name="isSingleton">true if a singleton navigation property should be expanded; otherwise false.</param>
        /// <param name="count">The number of entries to be used as expanded payload. Has to be '1' for singleton navigation properties.</param>
        /// <param name="nextLink">An optional next link to be used in an expanded link with a feed payload (requires <paramref name="isSingleton"/> to be false).</param>
        /// <returns>A new test descriptor with the expanded navigation property.</returns>
        public static T ExpandNavigationProperty<T>(this T navPropertyPayload, bool isSingleton, uint count = 1, string nextLink = null) where T : PayloadTestDescriptor
        {
            Debug.Assert(navPropertyPayload != null, "navPropertyPayload != null");
            Debug.Assert(navPropertyPayload.PayloadElement != null, "navPropertyPayload.PayloadElement != null");
            Debug.Assert(!isSingleton || count == 1, "For singleton navigation properties the count must be 1.");

            NavigationPropertyInstance navPropertyInstance = navPropertyPayload.PayloadElement as NavigationPropertyInstance;
            Debug.Assert(navPropertyInstance != null, "navPropertyInstance != null");
            Debug.Assert(navPropertyInstance.Value is DeferredLink, "Expected a deferred link navigation property.");

            IEdmModel edmModel = navPropertyPayload.PayloadEdmModel;
            if (edmModel != null)
            {
                return navPropertyPayload.ExpandEdmNavigationProperty(isSingleton, count, nextLink);
            }

            string expandedName = navPropertyInstance.Name + "_" + (isSingleton ? "Singleton" : "Collection");
            
            // create 'count' entity instances
            List<EntityInstance> entities = new List<EntityInstance>();
            for (int i = 0; i < count; ++i)
            {
                entities.Add(
                    PayloadBuilder.Entity("TestModel.PGExpandNavigationProperty_ResultType")
                        .Property("ID", PayloadBuilder.PrimitiveValue(i)));
            }

            ODataPayloadElement expandedElementContent;
            if (isSingleton)
            {
                Debug.Assert(entities.Count == 1, "Only a single entity expected for a singleton navigation property.");
                expandedElementContent = entities[0];
            }
            else
            {
                expandedElementContent = PayloadBuilder.EntitySet().Append(entities).NextLink(nextLink);
            }

            T descriptor = (T)navPropertyPayload.Clone();

            descriptor.PayloadElement = new NavigationPropertyInstance(expandedName, expandedElementContent, navPropertyInstance.AssociationLink);
            descriptor.PayloadEdmModel = null;


            if (nextLink != null)
            {
                var skipFunc = navPropertyPayload.SkipTestConfiguration;

                // Next link is only valid in V2 and in response
                descriptor.SkipTestConfiguration = tc => (skipFunc == null ? false : skipFunc(tc)) || tc.IsRequest;
            }

            return descriptor;
        }

        /// <summary>
        /// Takes the <paramref name="navPropertyPayload"/> that represents a deferred navigation property and expands it by creating a new entity type,
        /// adding it to the model and then creating <paramref name="count"/> entities for the new type that act as content of the expanded link.
        /// </summary>
        /// <param name="navPropertyPayload">The deferred navigation property payload to be expanded.</param>
        /// <param name="isSingleton">true if a singleton navigation property should be expanded; otherwise false.</param>
        /// <param name="count">The number of entries to be used as expanded payload. Has to be '1' for singleton navigation properties.</param>
        /// <param name="nextLink">An optional next link to be used in an expanded link with a feed payload (requires <paramref name="isSingleton"/> to be false).</param>
        /// <returns>A new test descriptor with the expanded navigation property.</returns>
        public static T ExpandEdmNavigationProperty<T>(this T navPropertyPayload, bool isSingleton, uint count = 1, string nextLink = null) where T : PayloadTestDescriptor
        {
            Debug.Assert(navPropertyPayload != null, "navPropertyPayload != null");
            Debug.Assert(navPropertyPayload.PayloadElement != null, "navPropertyPayload.PayloadElement != null");
            Debug.Assert(!isSingleton || count == 1, "For singleton navigation properties the count must be 1.");

            NavigationPropertyInstance navPropertyInstance = navPropertyPayload.PayloadElement as NavigationPropertyInstance;
            Debug.Assert(navPropertyInstance != null, "navPropertyInstance != null");
            Debug.Assert(navPropertyInstance.Value is DeferredLink, "Expected a deferred link navigation property.");

            IEdmModel model = navPropertyPayload.PayloadEdmModel;

            string expandedName = navPropertyInstance.Name + "_" + (isSingleton ? "Singleton" : "Collection");

            EdmEntityType navigationPropertyResultType = null;
            if (model != null)
            {
                //model = model.Clone();

                // Add an entity type for the navigation property result type
                navigationPropertyResultType = model.FindDeclaredType("TestModel.PGExpandNavigationProperty_ResultType") as EdmEntityType;
                if (navigationPropertyResultType == null)
                {
                    navigationPropertyResultType = new EdmEntityType("TestModel", "PGExpandNavigationProperty_ResultType");
                    navigationPropertyResultType.AddKeys(navigationPropertyResultType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
                    ((EdmModel)model).AddElement(navigationPropertyResultType);
                    var container = model.EntityContainer as EdmEntityContainer;
                    container.AddEntitySet("PGExpandNavigationProperty_ResultSet", navigationPropertyResultType);
                }
            }

            // create 'count' entity instances
            List<EntityInstance> entities = new List<EntityInstance>();
            for (int i = 0; i < count; ++i)
            {
                entities.Add(
                    PayloadBuilder.Entity("TestModel.PGExpandNavigationProperty_ResultType")
                        .Property("ID", PayloadBuilder.PrimitiveValue(i)
                        .WithTypeAnnotation(navigationPropertyResultType)));
            }

            ODataPayloadElement expandedElementContent;
            if (isSingleton)
            {
                Debug.Assert(entities.Count == 1, "Only a single entity expected for a singleton navigation property.");
                expandedElementContent = entities[0];
            }
            else
            {
                expandedElementContent = PayloadBuilder.EntitySet().Append(entities).NextLink(nextLink);
            }

            T descriptor = (T)navPropertyPayload.Clone();
            descriptor.PayloadElement = new NavigationPropertyInstance(expandedName, expandedElementContent, navPropertyInstance.AssociationLink).WithTypeAnnotation(navigationPropertyResultType);
            descriptor.PayloadEdmModel = model;


            if (nextLink != null)
            {
                var skipFunc = navPropertyPayload.SkipTestConfiguration;

                // Next link is only valid in V2 and in response
                descriptor.SkipTestConfiguration = tc => (skipFunc == null ? false : skipFunc(tc)) || tc.IsRequest;
            }

            return descriptor;
        }

        /// <summary>
        /// Makes sure to ignore top-level feed payloads in requests if the version is greater than V1.
        /// This is needed because for feeds in requests the MS-OData spec does not define a format and we 
        /// thus use the V1 format.
        /// </summary>
        /// <param name="descriptor">The test descriptor to filter.</param>
        /// <returns>
        /// The same or a new <see cref="PayloadReaderTestDescriptor"/> that ensures that top-level feed
        /// payloads are ignored in requests of versions greater than V1.
        /// </returns>
        public static T FilterTopLevelFeed<T>(this T descriptor) where T : PayloadTestDescriptor
        {
            EntitySetInstance entitySetInstance = descriptor.PayloadElement as EntitySetInstance;
            if (entitySetInstance != null)
            {
                // For top-level feed payloads we ignore the test descriptor if
                // (a) we use a versions >= 2.0 and 
                // (b) this is a request payload descriptor and
                // (c) the entity set instance has either next link or inline count
                // The reason is that the spec does not specify a format for such feeds in requests
                // (and we thus use v1 format as a result that does not support next link or inline count)
                T testDescriptor = (T)descriptor.Clone();
                testDescriptor.SkipTestConfiguration = tc =>
                {
                    bool usesInlineCountOrNextLink = entitySetInstance.NextLink != null || entitySetInstance.InlineCount.HasValue;
                    bool skip = descriptor.SkipTestConfiguration == null ? false : descriptor.SkipTestConfiguration(tc);

                    // Inline count or next link is only valid in V2 and in responses
                    return skip || (usesInlineCountOrNextLink && tc.IsRequest);
                };
                return testDescriptor;
            }

            return descriptor;
        }

        /// <summary>
        /// Builds an IEnumerable of payload elements around the payload element using the extraElements array
        /// </summary>
        /// <typeparam name="T"> ODataPayload Element</typeparam>
        /// <param name="payloadsBefore">paylods before the property instance</param>
        /// <param name="payloadsAfter">paylods after the property instance</param>
        /// <param name="payload">payload around which to build</param>
        /// <param name="payloadElements">payload Elements to use to build additional payloads</param>
        /// <returns>An IEnumerable of payloads with specified number of payloads before and after the payload</returns>
        private static IEnumerable<T> BuildPayloadElements<T>(int payloadsBefore, int payloadsAfter, T payload,
            T[] payloadElements) where T : ODataPayloadElement
        {
            IEnumerable<T> payloads = Enumerable.Range(0, payloadsBefore)
                .Select(i => payloadElements[i % payloadElements.Length]);
            payloads = payloads.ConcatSingle(payload);
            payloads = payloads.Concat(Enumerable.Range(0, payloadsAfter)
                .Select(i => payloadElements[payloadElements.Length - 1 - (i % payloadElements.Length)]));

            return payloads;
        }
    }
}
