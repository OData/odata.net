//---------------------------------------------------------------------
// <copyright file="TestFeeds.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper class to create all interesting entity set instances used in payloads.
    /// </summary>
    public static class TestFeeds
    {
        private const string nextLink = "http://odata.org/nextlink";
        
        /// <summary>
        /// Creates a set of interesting entity set instances along with metadata.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <param name="model">If non-null, the method creates types as needed and adds them to the model.</param>
        /// <param name="withTypeNames">true if the payloads should specify type names.</param>
        /// <returns>List of test descriptors with interesting entity instances as payload.</returns>
        public static IEnumerable<PayloadTestDescriptor> CreateEntitySetTestDescriptors(
            EdmModel model, 
            bool withTypeNames)
        {
            EdmEntityType personType = null;
            EdmComplexType carType = null;
            var container = model.EntityContainer as EdmEntityContainer;
            if (container == null)
            {
                container = new EdmEntityContainer("TestModel", "DefaultNamespace");
                model.AddElement(container);
            }

            if (model != null)
            {
                personType = model.FindDeclaredType("TestModel.TFPerson") as EdmEntityType;
                carType = model.FindDeclaredType("TestModel.TFCar") as EdmComplexType;
                
                // Create the metadata types for the entity instance used in the entity set
                if (carType == null)
                {
                    carType = new EdmComplexType("TestModel", "TFCar");
                    carType.AddStructuralProperty("Make", EdmPrimitiveTypeKind.String);
                    carType.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String);
                    model.AddElement(carType);
                }
                
                if (personType == null)
                {
                    personType = new EdmEntityType("TestModel", "TFPerson");
                    var keyProperty = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
                    personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
                    personType.AddStructuralProperty("Car", carType.ToTypeReference());
                    personType.AddKeys(keyProperty);
                    model.AddElement(personType);
                    container.AddEntitySet("TFPerson", personType);
                }
            }

            ComplexInstance carInstance = PayloadBuilder.ComplexValue(withTypeNames ? "TestModel.TFCar" : null)
                .Property("Make", PayloadBuilder.PrimitiveValue("Ford"))
                .Property("Color", PayloadBuilder.PrimitiveValue("Blue"));

            EntityInstance personInstance = PayloadBuilder.Entity(withTypeNames ? "TestModel.TFPerson" : null)
                .Property("Id", PayloadBuilder.PrimitiveValue(1))
                .Property("Name", PayloadBuilder.PrimitiveValue("John Doe"))
                .Property("Car", carInstance);

            // empty feed
            yield return new PayloadTestDescriptor() { PayloadElement = PayloadBuilder.EntitySet().WithTypeAnnotation(personType), PayloadEdmModel = model };
            // entity set with a single entity
            yield return new PayloadTestDescriptor() { PayloadElement = PayloadBuilder.EntitySet().Append(personInstance).WithTypeAnnotation(personType), PayloadEdmModel = model };
            // entity set with the person instance in the middle
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.EntitySet()
                    .Append(personInstance.GenerateSimilarEntries(2))
                    .Append(personInstance)
                    .Append(personInstance.GenerateSimilarEntries(1))
                    .WithTypeAnnotation(personType),
                PayloadEdmModel = model
            };
            // entity set with a single entity and a next link
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.EntitySet().Append(personInstance).NextLink(nextLink).WithTypeAnnotation(personType),
                PayloadEdmModel = model,
                SkipTestConfiguration = tc => tc.IsRequest
            };
            // entity set with a single entity and inline count
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.EntitySet().Append(personInstance).InlineCount(1).WithTypeAnnotation(personType),
                PayloadEdmModel = model,
                SkipTestConfiguration = tc => tc.IsRequest
            };
            // entity set with a single entity, a next link and inline count
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.EntitySet().Append(personInstance).NextLink(nextLink).InlineCount(1).WithTypeAnnotation(personType),
                PayloadEdmModel = model,
                SkipTestConfiguration = tc => tc.IsRequest
            };
            // entity set with a single entity, a next link and a negative inline count
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.EntitySet().Append(personInstance).NextLink(nextLink).InlineCount(-1).WithTypeAnnotation(personType),
                PayloadEdmModel = model,
                SkipTestConfiguration = tc => tc.IsRequest
            };
            // entity set containing many entities of types derived from the same base
            //yield return new PayloadTestDescriptor()
            //{
            //    PayloadElement = CreateDerivedEntitySetInstance(model, withTypeNames),
            //    PayloadEdmModel = model,
            //};
        }
    
        /// <summary>
        /// Generates an open entity set containing entities with different amounts of primitive open properties.
        /// </summary>
        /// <param name="model">The entity model. The method will modify the model and call Fixup().</param>
        /// <param name="withTypeNames">True if the payloads should specify type names.</param>
        /// <param name="primitiveValueFilter">A function for excluding primitives which are unsupported for open properties.</param>
        /// <returns>The open entity set containing entities with primitive open properties.</returns>
        public static ODataPayloadElement CreateOpenEntitySetInstance(EdmModel model, bool withTypeNames, Func<PrimitiveValue, bool> primitiveValueFilter)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckArgumentNotNull(primitiveValueFilter, "primitiveValueFilter");

            var edmEntityType = new EdmEntityType("TestModel", "OpenEntityType", baseType: null, isAbstract: false, isOpen: true);
            model.AddElement(edmEntityType);
            edmEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, true);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("OpenEntityTypes", edmEntityType);
            model.AddElement(container);

            var feed = PayloadBuilder.EntitySet().WithTypeAnnotation(edmEntityType);

            string entityTypeName = withTypeNames ? edmEntityType.FullName() : null;

            var primitiveValues = TestValues.CreatePrimitiveValuesWithMetadata(true).Where(p => primitiveValueFilter(p));

            int idx = 0;
            for (int i = 0; i <= primitiveValues.Count(); ++i)
            {
                var entityInstance = PayloadBuilder.Entity(entityTypeName).PrimitiveProperty("Name", Guid.NewGuid().ToString());
                foreach (var property in primitiveValues.Take(i))
                {
                    entityInstance.PrimitiveProperty("Property" + (idx++), property.ClrValue);
                }
                feed.Add(entityInstance);
            }
            return feed;
        }
    }
}
