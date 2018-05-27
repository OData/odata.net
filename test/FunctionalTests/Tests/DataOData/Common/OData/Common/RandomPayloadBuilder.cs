//---------------------------------------------------------------------
// <copyright file="RandomPayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    class RandomPayloadBuilder
    {
        /// <summary>
        /// Generates an arbitrary top load payload based on the maximum version supplied.
        /// </summary>
        /// <param name="random">Random generator for generating arbitrary payloads</param>
        /// <param name="model">The model to add any new types to</param>
        /// <param name="version">Maximum version for generated payloads.</param>
        /// <returns>A top level payload.</returns>
        public static ODataPayloadElement GetRandomPayload(IRandomNumberGenerator random, EdmModel model, ODataVersion version = ODataVersion.V4)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            Func<ODataPayloadElement>[] payloadCalls = new Func<ODataPayloadElement>[]
            {
                () => { return GetComplexInstanceCollection(random, model, version); }, 
                () => { return GetComplexProperty(random, model, version); }, 
                () => { return GetDeferredLink(); }, 
                () => { return GetLinkCollection(); }, 
                () => { return GetEntityInstance(random, model, version); }, 
                () => { return GetEntitySetInstance(random, model); }, 
                () => { return GetODataErrorPayload(random); }, 
                () => { return GetPrimitiveCollection(random); }, 
                () => { return GetPrimitiveProperty(random, model); }, 
                () => { return GetPrimitiveValue(random, model); },
            };

            payloadCalls.Concat(new Func<ODataPayloadElement>[]
            {
                () => { return GetComplexMultiValueProperty(random, model, version); }, 
                () => { return GetPrimitiveMultiValueProperty(random); },
            });

            var payloadCall = random.ChooseFrom(payloadCalls);
            return payloadCall();
        }

        /// <summary>
        /// Generates the specified number of similar complex instances
        /// </summary>
        /// <param name="random">Random number generator for generating instance property values.</param>
        /// <param name="currentInstance">Instance to copy.</param>
        /// <param name="numberOfInstances">Number of similar instances to generate.</param>
        /// <param name="randomizePropertyValues">If this is false it will copy the instance without changing it otherwise the property values will be randomized.</param>
        /// <returns>A set of similar instances.</returns>
        public static IEnumerable<ComplexInstance> GenerateSimilarComplexInstances(
            IRandomNumberGenerator random,
            ComplexInstance currentInstance,
            int numberOfInstances,
            bool randomizePropertyValues = false)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckArgumentNotNull(currentInstance, "currentInstance");
            ExceptionUtilities.CheckArgumentNotNull(numberOfInstances, "numberOfInstance");

            return Enumerable.Range(0, numberOfInstances).Select(x => GenerateSimilarComplexInstance(random, currentInstance));
        }

        private static ComplexInstance GetComplexInstance(IRandomNumberGenerator random, EdmModel model = null, ODataVersion version = ODataVersion.V4)
        {
            var values = TestValues.CreateComplexValues(model, true, true);

            return random.ChooseFrom(values);
        }

        private static ComplexInstanceCollection GetComplexInstanceCollection(IRandomNumberGenerator random, EdmModel model = null, ODataVersion version = ODataVersion.V4)
        {
            var complex = GetComplexInstance(random, model, version);
            int numinstances = random.ChooseFrom(new[] { 0, 1, 3 });
            var payload = new ComplexInstanceCollection(GenerateSimilarComplexInstances(random, complex, numinstances).ToArray());
            if (model != null)
            {
                var container = model.EntityContainersAcrossModels().Single() as EdmEntityContainer;
                var collectionType = new EdmCollectionType((model.FindDeclaredType(complex.FullTypeName) as EdmComplexType).ToTypeReference());

                var function = new EdmFunction(container.Namespace, "GetComplexInstances", collectionType.ToTypeReference());
                var functionImport = container.AddFunctionImport("GetComplexInstances", function);

                payload.AddAnnotation(new FunctionAnnotation() { FunctionImport = functionImport });
                payload.AddAnnotation(new DataTypeAnnotation() { EdmDataType = collectionType });
            }

            return payload;
        }

        private static ComplexMultiValueProperty GetComplexMultiValueProperty(IRandomNumberGenerator random, EdmModel model = null, ODataVersion version = ODataVersion.V4)
        {
            
            int numinstances = random.ChooseFrom(new[] { 0, 1, 3 });
            var instance = GetComplexInstance(random, model, version);
            var instances = GenerateSimilarComplexInstances(random, instance, numinstances, true);
            var propertyName = "ComplexMultivalue" + instance.FullTypeName;
            var payload = new ComplexMultiValueProperty(propertyName,
                new ComplexMultiValue(propertyName, false, instances.ToArray()));
            if (model != null)
            {
                var entityDataType = instance.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType.Definition as IEdmEntityType;
                ExceptionUtilities.CheckObjectNotNull(entityDataType, "Complex Instance must have an EntityModelTypeAnnotation with an EntityDataType");
                var entityType = model.FindDeclaredType(entityDataType.FullName()) as EdmEntityType;

                ExceptionUtilities.CheckObjectNotNull(entityType, "entityType");
                if (entityType.FindProperty(propertyName) != null)
                {
                    entityType.AddStructuralProperty(propertyName, (model.FindDeclaredType(instance.FullTypeName) as EdmComplexType).ToTypeReference());
                }

                payload.WithTypeAnnotation(entityType);
            }

            return payload;
        }

        private static ComplexProperty GetComplexProperty(IRandomNumberGenerator random, EdmModel model = null, ODataVersion version = ODataVersion.V4)
        {
            var instance = GetComplexInstance(random, model, version);
            var property = new ComplexProperty(instance.FullTypeName, instance);
            property.WithTypeAnnotation(model.FindDeclaredType(instance.FullTypeName));
            return property;
        }

        private static DeferredLink GetDeferredLink(EntityModelSchema model = null)
        {
            var payload = PayloadBuilder.DeferredLink("http://www.odata.org");
            if (model != null)
            {
                var navProperty = model.EntityTypes.Where(et => et.NavigationProperties.Count > 0).FirstOrDefault();
                if (navProperty != null)
                {
                    // Metadata to indicate this is a navigation property
                    payload.AddAnnotation(new NavigationPropertyAnnotation() { Property = navProperty.NavigationProperties.First() });
                }
            }

            return payload;
        }

        private static LinkCollection GetLinkCollection()
        {
            var collection = PayloadBuilder.LinkCollection();
            collection.Add(PayloadBuilder.DeferredLink("http://www.odata.org"));
            collection.Add(PayloadBuilder.DeferredLink("http://www.bubbles.org"));
            collection.Add(PayloadBuilder.DeferredLink("http://www.snowflakes.org"));
            return collection;
        }

        private static EntityInstance GetEntityInstance(IRandomNumberGenerator random, EdmModel model = null, ODataVersion version = ODataVersion.V4)
        {
            var values = TestEntityInstances.CreateEntityInstanceTestDescriptors(model, true);

            var payloadDescriptor = random.ChooseFrom(values);
            var payload = (EntityInstance)payloadDescriptor.PayloadElement;
            
            return payload;
        }

        private static EntitySetInstance GetEntitySetInstance(IRandomNumberGenerator random, EdmModel model = null)
        {
            var payloadDescriptor = random.ChooseFrom(TestFeeds.CreateEntitySetTestDescriptors(model, true));
            return (EntitySetInstance)payloadDescriptor.PayloadElement;
        }

        private static ODataErrorPayload GetODataErrorPayload(IRandomNumberGenerator random)
        {
            var testErrors = random.ChooseFrom(TestErrors.CreateErrorTestDescriptors());
            return (ODataErrorPayload)testErrors.PayloadElement;
        }

        private static PrimitiveCollection GetPrimitiveCollection(IRandomNumberGenerator random)
        {
            TestValues.CreatePrimitiveValuesWithMetadata(true);
            PrimitiveValue val = random.ChooseFrom(TestValues.CreatePrimitiveValuesWithMetadata(true));
            int numItems = random.NextFromRange(1, 3);
            var newItems = Enumerable.Range(0, numItems).Select(x => TestValues.GetDifferentPrimitiveValue(val));
            var payload = new PrimitiveCollection(newItems.ToArray());

            return payload;
        }

        private static PrimitiveMultiValueProperty GetPrimitiveMultiValueProperty(IRandomNumberGenerator random)
        {
            var val = random.ChooseFrom(TestValues.CreatePrimitiveCollections(true, true));
            var payload = new PrimitiveMultiValueProperty("ArbitraryMultivalue", val);

            return payload;
        }

        private static PrimitiveValue GetPrimitiveValue(IRandomNumberGenerator random, EdmModel model = null)
        {
            var payload = random.ChooseFrom(TestValues.CreatePrimitiveValuesWithMetadata(true));
            
            if (model != null)
            {
                EdmEntityType entity = model.FindDeclaredType("AllPrimitiveTypesEntity") as EdmEntityType; 
                if (entity == null)
                {
                    entity = new EdmEntityType("TestModel", "AllPrimitiveTypesEntity");
                    entity.AddStructuralProperty("StringPropNullable", EdmPrimitiveTypeKind.String, true);
                    entity.AddStructuralProperty("StringProp", EdmPrimitiveTypeKind.String, false);
                    entity.AddStructuralProperty("Int32PropNullable", EdmPrimitiveTypeKind.Int32, true);
                    entity.AddStructuralProperty("Int32Prop", EdmPrimitiveTypeKind.Int32, false);
                    entity.AddStructuralProperty("BooleanPropNullable", EdmPrimitiveTypeKind.Boolean, true);
                    entity.AddStructuralProperty("BooleanProp", EdmPrimitiveTypeKind.Boolean, false);
                    entity.AddStructuralProperty("BytePropNullable", EdmPrimitiveTypeKind.Byte, true);
                    entity.AddStructuralProperty("ByteProp", EdmPrimitiveTypeKind.Byte, false);
                    entity.AddStructuralProperty("SBytePropNullable", EdmPrimitiveTypeKind.SByte, true);
                    entity.AddStructuralProperty("SByteProp", EdmPrimitiveTypeKind.SByte, false);
                    entity.AddStructuralProperty("Int16PropNullable", EdmPrimitiveTypeKind.Int16, true);
                    entity.AddStructuralProperty("Int16Prop", EdmPrimitiveTypeKind.Int16, false);
                    entity.AddStructuralProperty("DecimalPropNullable", EdmPrimitiveTypeKind.Decimal, true);
                    entity.AddStructuralProperty("DecimalProp", EdmPrimitiveTypeKind.Decimal, false);
                    entity.AddStructuralProperty("SinglePropNullable", EdmPrimitiveTypeKind.Single, true);
                    entity.AddStructuralProperty("SingleProp", EdmPrimitiveTypeKind.Single, false);
                    entity.AddStructuralProperty("Int64PropNullable", EdmPrimitiveTypeKind.Int64, true);
                    entity.AddStructuralProperty("Int64Prop", EdmPrimitiveTypeKind.Int64, false);
                    entity.AddStructuralProperty("DoublePropNullable", EdmPrimitiveTypeKind.Double, true);
                    entity.AddStructuralProperty("DoubleProp", EdmPrimitiveTypeKind.Double, false);
                    entity.AddStructuralProperty("BinaryPropNullable", EdmPrimitiveTypeKind.Binary, true);
                    entity.AddStructuralProperty("BinaryProp", EdmPrimitiveTypeKind.Binary, false);
                    entity.AddStructuralProperty("GuidPropNullable", EdmPrimitiveTypeKind.Guid, true);
                    entity.AddStructuralProperty("GuidProp", EdmPrimitiveTypeKind.Guid, false);
                    model.AddElement(entity);
                }

                payload.WithTypeAnnotation(entity);
            }

            return payload;
        }

        private static PrimitiveProperty GetPrimitiveProperty(IRandomNumberGenerator random, EdmModel model = null)
        {
            var val = GetPrimitiveValue(random, model);

            var name = "ArbitraryPrimitiveProperty";
            var payload = new PrimitiveProperty()
            {
                Name = name,
                Value = val
            };

            if (model != null)
            {
                var entity = val.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType.Definition as IEdmEntityType;
                // Finding a property name which exists in the model. We are using first because we don't care what property is returned so long as one exists.
                var property = entity.Properties().First(p => p.Type.Definition == val.GetAnnotation<DataTypeAnnotation>().EdmDataType);
                payload.Name = property.Name;
                payload.WithTypeAnnotation(entity);
            }

            return payload;
        }

        private static ComplexInstance GenerateSimilarComplexInstance(IRandomNumberGenerator random, ComplexInstance currentInstance, bool randomizePropertyValues = false)
        {
            ComplexInstance instance = ((ComplexInstance)currentInstance.DeepCopy());
            if (!randomizePropertyValues)
            {
                return instance;
            }

            foreach (var property in instance.Properties)
            {
                PrimitiveProperty primitive = property as PrimitiveProperty;
                if (primitive != null)
                {
                    primitive.Value = TestValues.GetDifferentPrimitiveValue(primitive.Value);
                }

                ComplexProperty complex = property as ComplexProperty;
                if (complex != null)
                {
                    complex.Value = GenerateSimilarComplexInstance(random, complex.Value);
                }

                PrimitiveMultiValueProperty pmultival = property as PrimitiveMultiValueProperty;
                if (pmultival != null)
                {
                    pmultival.Value = GenerateSimilarPrimitiveMultiValue(random, pmultival.Value);
                }

                ComplexMultiValueProperty cmultival = property as ComplexMultiValueProperty;
                if (cmultival != null)
                {
                    cmultival.Value = GenerateSimilarComplexMultiValue(random, cmultival.Value);
                }
            }

            return instance;
        }

        private static PrimitiveMultiValue GenerateSimilarPrimitiveMultiValue(IRandomNumberGenerator random, PrimitiveMultiValue currentInstance)
        {
            PrimitiveMultiValue instance = ((PrimitiveMultiValue)currentInstance.DeepCopy());

            if (instance.Count != 0)
            {
                PrimitiveValue val = instance.First();
                int numItems = random.NextFromRange(1, 3);
                var newItems = Enumerable.Range(0, numItems).Select(x => TestValues.GetDifferentPrimitiveValue(val));
                instance = new PrimitiveMultiValue(val.FullTypeName, false, newItems.ToArray());
            }

            return instance;
        }

        private static ComplexMultiValue GenerateSimilarComplexMultiValue(IRandomNumberGenerator random, ComplexMultiValue currentInstance)
        {
            ComplexMultiValue instance = ((ComplexMultiValue)currentInstance.DeepCopy());

            if (instance.Count != 0)
            {
                ComplexInstance val = instance.First();
                int numItems = random.NextFromRange(1, 3);
                var newItems = Enumerable.Range(0, numItems).Select(x => GenerateSimilarComplexInstance(random, val));
                instance = new ComplexMultiValue(val.FullTypeName, false, newItems.ToArray());
            }

            return instance;
        }
    }
}
