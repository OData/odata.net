//---------------------------------------------------------------------
// <copyright file="TestModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    internal class TestModel
    {
        public EdmModel Model { get; private set; }
        public IEdmEntityContainer Container { get; set; }
        public IEdmEntitySet ProductsSet { get; set; }
        public IEdmEntitySet MultipleKeysSet { get; set; }
        public IEdmEntityType ProductType { get; set; }
        public IEdmEntityType DerivedProductType { get; set; }
        public IEdmEntityType MultipleKeyType { get; set; }
        public IEdmEntityType ProductWithNavPropsType { get; set; }
        public IEdmStructuredValue OneMultipleKeyValue { get; set; }
        public IEdmStructuredValue OneProductValue { get; set; }
        public IEdmStructuredValue OneDerivedProductValue { get; set; }
        public IEdmStructuredValue OneProductWithNavPropsValue { get; set; }

        private TestModel(EdmModel model)
        {
            Model = model;
        }

        internal static EdmModel BuildDefaultTestModel()
        {
            return Initialize().Model;
        }

        public static TestModel Initialize()
        {
            EdmModel model = new EdmModel();
            var result = new TestModel(model);
            var productType = new EdmEntityType("TestModel", "Product");
            result.ProductType = productType;
            EdmStructuralProperty idProperty = new EdmStructuralProperty(productType, "Id", EdmCoreModel.Instance.GetInt32(false));
            productType.AddProperty(idProperty);
            productType.AddKeys(idProperty);
            productType.AddProperty(new EdmStructuralProperty(productType, "Name", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(productType);
            EdmEntityContainer defaultContainer = new EdmEntityContainer("TestModel", "Default");
            result.Container = defaultContainer;
            result.ProductsSet = defaultContainer.AddEntitySet("Products", productType);
            model.AddElement(defaultContainer);

            var derivedProductType = new EdmEntityType("TestModel", "DerivedProduct", productType);
            result.DerivedProductType = derivedProductType;

            EdmEntityType multipleKeyType = new EdmEntityType("TestModel", "MultipleKeyType");
            result.MultipleKeyType = multipleKeyType;
            EdmStructuralProperty keyAProperty = new EdmStructuralProperty(multipleKeyType, "KeyA", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty keyBProperty = new EdmStructuralProperty(multipleKeyType, "KeyB", EdmCoreModel.Instance.GetInt32(false));
            multipleKeyType.AddProperty(keyAProperty);
            multipleKeyType.AddProperty(keyBProperty);
            multipleKeyType.AddKeys(keyAProperty, keyBProperty);
            model.AddElement(multipleKeyType);
            result.MultipleKeysSet = defaultContainer.AddEntitySet("MultipleKeySet", multipleKeyType);

            EdmEntityType productTypeWithNavProps = new EdmEntityType("TestModel", "ProductWithNavProps", productType);
            result.ProductWithNavPropsType = productTypeWithNavProps;
            productTypeWithNavProps.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
                {
                    Name = "RelatedProducts",
                    Target = productType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });

            productTypeWithNavProps.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "RelatedDerivedProduct",
                Target = derivedProductType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            model.AddElement(productTypeWithNavProps);

            EdmAction action = new EdmAction("TestModel", "SimpleAction", null/*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            model.AddElement(action);
            defaultContainer.AddActionImport("SimpleAction", action);
            
            EdmAction action1 = new EdmAction("TestModel", "SimpleFunctionWithOverload", EdmCoreModel.Instance.GetInt32(false), true /*isbound*/, null);
            action1.AddParameter("p1", EdmCoreModel.Instance.GetInt32(false));
            defaultContainer.AddActionImport("SimpleFunctionWithOverload", action1);
            
            EdmAction action2 = new EdmAction("TestModel", "SimpleFunctionWithOverload", EdmCoreModel.Instance.GetInt32(false), true /*isbound*/, null);
            action2.AddParameter("p1", EdmCoreModel.Instance.GetString(false));
            defaultContainer.AddActionImport("SimpleFunctionWithOverload", action2);

            result.OneProductValue = BuildDefaultProductValue(productType);
            result.OneDerivedProductValue = BuildDefaultProductValue(derivedProductType);
            result.OneMultipleKeyValue = BuildDefaultMultipleKeyValue(model);
            result.OneProductWithNavPropsValue = BuildDefaultProductValue(productTypeWithNavProps);

            return result;
        }

        internal static IEdmStructuredValue BuildDefaultProductValue(IEdmEntityType entityType)
        {
            return new EdmStructuredValueSimulator(
                entityType,
                new Dictionary<string, IEdmValue>
                {
                    { "Id", new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(false), 42) },
                    { "Name", new EdmStringConstant(EdmCoreModel.Instance.GetString(true), "Value") },
                });
        }

        internal static IEdmStructuredValue BuildDefaultMultipleKeyValue(IEdmModel defaultModel)
        {
            return new EdmStructuredValueSimulator(
                GetEntityType(defaultModel, "TestModel.MultipleKeyType"),
                new Dictionary<string, IEdmValue>
                {
                    { "KeyA", new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "keya") },
                    { "KeyB", new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(false), 1) },
                });
        }

        internal static IEdmEntityType GetEntityType(IEdmModel model, string typeName)
        {
            IEdmSchemaType type = model.FindType(typeName);
            return type as IEdmEntityType;
        }

        public EdmEntitySet MakeTempSet(string entitySetName = "EntitySetName", string containerName = "Container", string namespaceName = "Namespace")
        {
            return new EdmEntitySet(new EdmEntityContainer(namespaceName, containerName), entitySetName, this.ProductType);
        }
    }
}