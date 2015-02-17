//---------------------------------------------------------------------
// <copyright file="ODataExtensionMethodsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Test.OData.TDD.Tests.Evaluation;
    using System.Linq;

    [TestClass]
    public class ODataExtensionMethodsTests
    {
        [TestMethod]
        public void TestGenerateServiceDocument()
        {
            var serviceDocument = this.CreateTestModel().GenerateServiceDocument();
            var entitySets = serviceDocument.EntitySets.ToList();
            Assert.AreEqual(entitySets.Count, 2);
            Assert.AreEqual(entitySets[0].Name, "Products");
            Assert.AreEqual(entitySets[0].Url.ToString(), "Products");
            Assert.AreEqual(entitySets[1].Name, "Customers");
            Assert.AreEqual(entitySets[1].Url.ToString(), "Customers");

            var singletons = serviceDocument.Singletons.ToList();
            Assert.AreEqual(singletons.Count, 2);
            Assert.AreEqual(singletons[0].Name, "SingleProduct");
            Assert.AreEqual(singletons[0].Url.ToString(), "SingleProduct");
            Assert.AreEqual(singletons[1].Name, "SingleCustomer");
            Assert.AreEqual(singletons[1].Url.ToString(), "SingleCustomer");

            var functionImports = serviceDocument.FunctionImports.ToList();
            Assert.AreEqual(functionImports.Count, 1);
            Assert.AreEqual(functionImports[0].Name, "SimpleFunctionImport2");
            Assert.AreEqual(functionImports[0].Url.ToString(), "SimpleFunctionImport2");
        }

        [TestMethod]
        public void TestServiceDocumentWhenNoContainer()
        {
            Action action = ()=>(new EdmModel()).GenerateServiceDocument();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataUtils_ModelDoesNotHaveContainer);
        }

        private IEdmModel CreateTestModel()
        {
            EdmModel model = new EdmModel();
            EdmEntityContainer defaultContainer = new EdmEntityContainer("TestModel", "Default");
            model.AddElement(defaultContainer);

            var productType = new EdmEntityType("TestModel", "Product");
            EdmStructuralProperty idProperty = new EdmStructuralProperty(productType, "Id", EdmCoreModel.Instance.GetInt32(false));
            productType.AddProperty(idProperty);
            productType.AddKeys(idProperty);
            productType.AddProperty(new EdmStructuralProperty(productType, "Name", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(productType);

            var customerType = new EdmEntityType("TestModel", "Customer");
            idProperty = new EdmStructuralProperty(customerType, "Id", EdmCoreModel.Instance.GetInt32(false));
            customerType.AddProperty(idProperty);
            customerType.AddKeys(idProperty);
            customerType.AddProperty(new EdmStructuralProperty(customerType, "Name", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(productType);

            defaultContainer.AddEntitySet("Products", productType);
            defaultContainer.AddEntitySet("Customers", customerType);
            defaultContainer.AddSingleton("SingleProduct", productType);
            defaultContainer.AddSingleton("SingleCustomer", customerType);

            EdmAction action = new EdmAction("TestModel", "SimpleAction", null/*returnType*/, false /*isBound*/, null /*entitySetPath*/);
            model.AddElement(action);
            defaultContainer.AddActionImport("SimpleActionImport", action);

            EdmFunction function1 = new EdmFunction("TestModel", "SimpleFunction1", EdmCoreModel.Instance.GetInt32(false), false /*isbound*/, null, true);
            function1.AddParameter("p1", EdmCoreModel.Instance.GetInt32(false));
            defaultContainer.AddFunctionImport("SimpleFunctionImport1", function1);

            EdmFunction function2 = new EdmFunction("TestModel", "SimpleFunction2", EdmCoreModel.Instance.GetInt32(false), false /*isbound*/, null, true);
            function2.AddParameter("p1", EdmCoreModel.Instance.GetInt32(false));
            defaultContainer.AddFunctionImport("SimpleFunctionImport2", function1, null, true /*IncludeInServiceDocument*/);

            return model;
        }
    }
}
