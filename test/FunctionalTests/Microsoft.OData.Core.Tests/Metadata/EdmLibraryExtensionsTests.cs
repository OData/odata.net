//---------------------------------------------------------------------
// <copyright file="EdmLibraryExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.OData.Metadata;
using Microsoft.OData.Tests.Evaluation;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight;
using Microsoft.OData.Tests.UriParser;
using Xunit;
using Microsoft.Spatial;
using Xunit.Sdk;

namespace Microsoft.OData.Tests.Metadata
{
    public class EdmLibraryExtensionsTests
    {
        private static readonly EdmPrimitiveTypeKind[] GeographyMultiLeafTypeKinds = new EdmPrimitiveTypeKind[] { EdmPrimitiveTypeKind.GeographyMultiLineString, EdmPrimitiveTypeKind.GeographyMultiPoint, EdmPrimitiveTypeKind.GeographyMultiPolygon };
        private static readonly EdmPrimitiveTypeKind[] GeographySingleLeafTypeKinds = new EdmPrimitiveTypeKind[] { EdmPrimitiveTypeKind.GeographyLineString, EdmPrimitiveTypeKind.GeographyPoint, EdmPrimitiveTypeKind.GeographyPolygon };
        private static readonly EdmPrimitiveTypeKind[] GeometryMultiLeafTypeKinds = new EdmPrimitiveTypeKind[] { EdmPrimitiveTypeKind.GeometryMultiLineString, EdmPrimitiveTypeKind.GeometryMultiPoint, EdmPrimitiveTypeKind.GeometryMultiPolygon };
        private static readonly EdmPrimitiveTypeKind[] GeometrySingleLeafTypeKinds = new EdmPrimitiveTypeKind[] { EdmPrimitiveTypeKind.GeometryLineString, EdmPrimitiveTypeKind.GeometryPoint, EdmPrimitiveTypeKind.GeometryPolygon };
        private static readonly EdmPrimitiveTypeKind[] GeographyAllTypeKinds = GeographyMultiLeafTypeKinds.Union(GeographySingleLeafTypeKinds).Union(new EdmPrimitiveTypeKind[] { EdmPrimitiveTypeKind.GeographyCollection }.AsEnumerable()).ToArray();
        private static readonly EdmPrimitiveTypeKind[] GeometryAllTypeKinds = GeometryMultiLeafTypeKinds.Union(GeometrySingleLeafTypeKinds).Union(new EdmPrimitiveTypeKind[] { EdmPrimitiveTypeKind.GeometryCollection }.AsEnumerable()).ToArray();
        private readonly EdmModel model;
        private readonly EdmEntityContainer defaultContainer;
        private readonly IEdmEntitySet productsSet;
        private readonly IEdmEntityType productType;
        private readonly IEdmEntityTypeReference productTypeReference;
        private readonly EdmFunctionImport operationImportWithOverloadAnd0Param;
        private readonly EdmFunctionImport operationImportWithOverloadAnd1Param;
        private readonly EdmFunctionImport operationImportWithOverloadAnd2Params;
        private readonly EdmFunctionImport operationImportWithOverloadAnd5Params;
        private readonly EdmFunctionImport operationImportWithNoOverload;
        private readonly EdmFunction operationWithOverloadAnd0Param;
        private readonly EdmFunction operationWithOverloadAnd1Param;
        private readonly EdmFunction operationWithOverloadAnd2Params;
        private readonly EdmFunction operationWithOverloadAnd5Params;
        private readonly EdmFunction operationWithNoOverload;

        public EdmLibraryExtensionsTests()
        {
            this.model = TestModel.BuildDefaultTestModel();
            this.defaultContainer = (EdmEntityContainer)this.model.FindEntityContainer("Default");
            this.productsSet = this.defaultContainer.FindEntitySet("Products");
            this.productType = (IEdmEntityType)this.model.FindDeclaredType("TestModel.Product");
            this.productTypeReference = new EdmEntityTypeReference(this.productType, false);

            EdmComplexType complexType = new EdmComplexType("TestModel", "MyComplexType");

            this.operationWithNoOverload = new EdmFunction("TestModel", "FunctionImportWithNoOverload", EdmCoreModel.Instance.GetInt32(true));
            this.operationWithNoOverload.AddParameter("p1", EdmCoreModel.Instance.GetInt32(false));
            this.model.AddElement(operationWithNoOverload);
            this.operationImportWithNoOverload = this.defaultContainer.AddFunctionImport("FunctionImportWithNoOverload", operationWithNoOverload);

            this.operationWithOverloadAnd0Param = new EdmFunction("TestModel", "FunctionImportWithNoOverload", EdmCoreModel.Instance.GetInt32(true));
            this.model.AddElement(operationWithOverloadAnd0Param);
            this.operationImportWithOverloadAnd0Param = defaultContainer.AddFunctionImport("FunctionImportWithOverload", operationWithOverloadAnd0Param);

            this.operationWithOverloadAnd1Param = new EdmFunction("TestModel", "FunctionImportWithNoOverload", EdmCoreModel.Instance.GetInt32(true));
            this.operationWithOverloadAnd1Param.AddParameter("p1", EdmCoreModel.Instance.GetInt32(false));
            this.model.AddElement(operationWithOverloadAnd1Param);
            this.operationImportWithOverloadAnd1Param = defaultContainer.AddFunctionImport("FunctionImportWithOverload", operationWithOverloadAnd1Param);

            this.operationWithOverloadAnd2Params = new EdmFunction("TestModel", "FunctionImportWithNoOverload", EdmCoreModel.Instance.GetInt32(true));
            var productTypeReference = new EdmEntityTypeReference(productType, isNullable: false);
            this.operationWithOverloadAnd2Params.AddParameter("p1", productTypeReference);
            this.operationWithOverloadAnd2Params.AddParameter("p2", EdmCoreModel.Instance.GetString(true));
            this.model.AddElement(operationWithOverloadAnd2Params);
            this.operationImportWithOverloadAnd2Params = defaultContainer.AddFunctionImport("FunctionImportWithOverload", operationWithOverloadAnd2Params);

            this.operationWithOverloadAnd5Params = new EdmFunction("TestModel", "FunctionImportWithNoOverload", EdmCoreModel.Instance.GetInt32(true));
            this.operationWithOverloadAnd5Params.AddParameter("p1", new EdmCollectionTypeReference(new EdmCollectionType(productTypeReference)));
            this.operationWithOverloadAnd5Params.AddParameter("p2", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false))));
            this.operationWithOverloadAnd5Params.AddParameter("p3", EdmCoreModel.Instance.GetString(isNullable: true));
            EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(complexType, isNullable: false);
            this.operationWithOverloadAnd5Params.AddParameter("p4", complexTypeReference);
            this.operationWithOverloadAnd5Params.AddParameter("p5", new EdmCollectionTypeReference(new EdmCollectionType(complexTypeReference)));
            this.model.AddElement(operationWithOverloadAnd5Params);
            this.operationImportWithOverloadAnd5Params = defaultContainer.AddFunctionImport("FunctionImportWithOverload", operationWithOverloadAnd5Params);
        }

        #region FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType Operation Tests
        [Fact]
        public void FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingTypeShouldNotFilterIfBindingIsNotStructuralType()
        {
            EdmAction action = new EdmAction("namespace", "action", null);
            new IEdmOperation[] { action }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(EdmCoreModel.Instance.GetSingle(true).Definition).Should().HaveCount(1);
        }

        [Fact]
        public void FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingTypeShouldFilterReturnTypeClosestToTypeA()
        {
            EdmEntityType aType = new EdmEntityType("N", "A");
            EdmEntityType bType = new EdmEntityType("N", "B", aType);
            EdmEntityType cType = new EdmEntityType("N", "C", bType);
            EdmAction action = new EdmAction("namespace", "action", null, true, null);
            action.AddParameter("bindingParameter", new EdmEntityTypeReference(aType, false));
            EdmAction action2 = new EdmAction("namespace", "action", null, true, null);
            action2.AddParameter("bindingParameter", new EdmEntityTypeReference(bType, false));
            var filteredResults = new IEdmOperation[] { action, action2 }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(cType).ToList();
            filteredResults.Should().HaveCount(1);
            filteredResults[0].Should().BeSameAs(action2);
        }

        [Fact]
        public void FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingTypeShouldFilterReturnTypeClosestToTypeCollectionA()
        {
            EdmEntityType aType = new EdmEntityType("N", "A");
            EdmEntityType bType = new EdmEntityType("N", "B", aType);
            EdmEntityType cType = new EdmEntityType("N", "C", bType);
            EdmAction action = new EdmAction("namespace", "action", null, true, null);
            action.AddParameter("bindingParameter", EdmCoreModel.GetCollection(new EdmEntityTypeReference(aType, false)));
            EdmAction action2 = new EdmAction("namespace", "action", null, true, null);
            action2.AddParameter("bindingParameter", EdmCoreModel.GetCollection(new EdmEntityTypeReference(bType, false)));
            var filteredResults = new IEdmOperation[] { action, action2 }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(EdmCoreModel.GetCollection(new EdmEntityTypeReference(cType, false)).Definition).ToList();
            filteredResults.Should().HaveCount(1);
            filteredResults[0].Should().BeSameAs(action2);
        }

        [Fact]
        public void FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingTypeShouldFilterReturnSameAType()
        {
            EdmEntityType aType = new EdmEntityType("N", "A");
            EdmEntityType bType = new EdmEntityType("N", "B", aType);
            EdmEntityType cType = new EdmEntityType("N", "C", bType);
            EdmAction action = new EdmAction("namespace", "action", null, true, null);
            action.AddParameter("bindingParameter", new EdmEntityTypeReference(cType, false));
            EdmAction action2 = new EdmAction("namespace", "action2", null, true, null);
            action2.AddParameter("bindingParameter", new EdmEntityTypeReference(aType, false));
            var filteredResults = new IEdmOperation[] { action, action2 }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(aType).ToList();
            filteredResults.Should().HaveCount(1);
            filteredResults[0].Should().BeSameAs(action2);
        }

        [Fact]
        public void FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingTypeShouldNotThrowAndFilterNonBoundOperations()
        {
            EdmEntityType aType = new EdmEntityType("N", "A");
            EdmAction action = new EdmAction("namespace", "action", null, false, null);
            action.AddParameter("bindingParameter", new EdmEntityTypeReference(aType, false));
            var filteredResults = new IEdmOperation[] { action }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(aType).ToList();
            filteredResults.Should().HaveCount(0);
        }

        [Fact]
        public void FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingTypeShouldNotThrowAndFilterBoundOperationsWithNoParameters()
        {
            EdmEntityType aType = new EdmEntityType("N", "A");
            EdmAction action = new EdmAction("namespace", "action", null, false, null);
            var filteredResults = new IEdmOperation[] { action }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(aType).ToList();
            filteredResults.Should().HaveCount(0);
        }

        #endregion

        #region FilterFunctionsByParameterNames

        [Fact]
        public void ResolveFunctionsOverloadsByParameterNameShouldResolve()
        {
            var function = new EdmFunction("d.s", "function1", EdmCoreModel.Instance.GetSingle(false));
            var functionOverload1 = new EdmFunction("d.s", "function1", EdmCoreModel.Instance.GetSingle(false));
            functionOverload1.AddParameter("foo", EdmCoreModel.Instance.GetSingle(false));
            var functionOverload2 = new EdmFunction("d.s", "function1", EdmCoreModel.Instance.GetSingle(false));
            functionOverload2.AddParameter("foo", EdmCoreModel.Instance.GetSingle(false));
            functionOverload2.AddParameter("foo2", EdmCoreModel.Instance.GetSingle(false));

            var functions = new EdmFunction[] { function, functionOverload1, functionOverload2 };
            var resolvedFunction = functions.FilterFunctionsByParameterNames(new string[] { "foo" });
            resolvedFunction.First().Should().BeSameAs(functionOverload1);
        }

        [Fact]
        public void ResolveFunctionsOverloadsByParameterNameShouldNotThrowIfMultipleResolve()
        {
            var function = new EdmFunction("d.s", "function1", EdmCoreModel.Instance.GetSingle(false));
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            var function1 = new EdmFunction("d.s", "function1", EdmCoreModel.Instance.GetSingle(false));
            function1.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            var functions = new EdmFunction[] { function, function1 };

            var selectedFunctions = functions.FilterFunctionsByParameterNames(new string[] { "param1" }).ToList();
            selectedFunctions.Count.Should().Be(2);
        }

        #endregion

        #region ResolveOperations
        // These tests are not comprehensive, just covering the basics with these tests.
        [Fact]
        public void ResolveOperationsWithFullyQualifiedOperationNameWithoutParameterNamesShouldReturnCorrectOperations()
        {
            var model = new EdmModel();
            var function = new EdmFunction("ds", "function1", EdmCoreModel.Instance.GetSingle(false));
            model.AddElement(function);
            var returnedOperations = model.ResolveOperations("ds.function1", false /*allowParameterTypeNames*/).ToList();
            returnedOperations.Count.Should().Be(1);
            returnedOperations[0].Should().BeSameAs(function);
        }

        [Fact]
        public void ResolveOperationsWithOperationNameWithoutParameterNamesShouldReturnCorrectOperations()
        {
            var model = new EdmModel();
            var function = new EdmFunction("ds", "function1", EdmCoreModel.Instance.GetSingle(false));
            model.AddElement(function);
            var returnedOperations = model.ResolveOperations("ds.function1", false /*allowParameterTypeNames*/).ToList();
            returnedOperations.Count.Should().Be(1);
            returnedOperations[0].Should().BeSameAs(function);
        }

        [Fact]
        public void ResolveOperationsWithOperationNameAndParametersWithoutParameterNamesShouldReturnCorrectOperations()
        {
            var model = new EdmModel();
            var function = new EdmFunction("ds", "function1", EdmCoreModel.Instance.GetSingle(false), true /*isBound*/, null /*entitysetpath*/, false /*isComposable*/);
            function.AddParameter("bindingParameter", this.productTypeReference);
            function.AddParameter("param1", EdmCoreModel.Instance.GetSingle(true));

            var functionOverload1 = new EdmFunction("ds", "function1", EdmCoreModel.Instance.GetSingle(false), true /*isBound*/, null /*entitysetpath*/, false /*isComposable*/);
            functionOverload1.AddParameter("bindingParameter", this.productTypeReference);
            functionOverload1.AddParameter("param1", EdmCoreModel.Instance.GetSingle(true));
            functionOverload1.AddParameter("param2", EdmCoreModel.Instance.GetSingle(true));

            model.AddElement(function);
            model.AddElement(functionOverload1);
            var returnedOperations = model.ResolveOperations("ds.function1(param1)", true /*allowParameterTypeNames*/).ToList();
            returnedOperations.Count.Should().Be(1);
            returnedOperations[0].Should().BeSameAs(function);
        }

        [Fact]
        public void ResolveOperationsWithFullyQualifiedOperationNameAndParametersWithoutParameterNamesShouldReturnCorrectOperations()
        {
            var model = new EdmModel();
            var function = new EdmFunction("ds", "function1", EdmCoreModel.Instance.GetSingle(false), true /*isBound*/, null /*entitysetpath*/, false /*isComposable*/);
            function.AddParameter("bindingParameter", this.productTypeReference);
            function.AddParameter("param1", EdmCoreModel.Instance.GetSingle(true));

            var functionOverload1 = new EdmFunction("ds", "function1", EdmCoreModel.Instance.GetSingle(false), true /*isBound*/, null /*entitysetpath*/, false /*isComposable*/);
            functionOverload1.AddParameter("bindingParameter", this.productTypeReference);
            functionOverload1.AddParameter("param1", EdmCoreModel.Instance.GetSingle(true));
            functionOverload1.AddParameter("param2", EdmCoreModel.Instance.GetSingle(true));

            model.AddElement(function);
            model.AddElement(functionOverload1);
            var returnedOperations = model.ResolveOperations("ds.function1(param1)", true /*allowParameterTypeNames*/).ToList();
            returnedOperations.Count.Should().Be(1);
            returnedOperations[0].Should().BeSameAs(function);
        }
        #endregion

        [Fact]
        public void OperationImportGroupFullNameForOperationImportListShouldReturnExpect()
        {
            var operationImports = new[]
            {
                operationImportWithOverloadAnd0Param,
                operationImportWithOverloadAnd1Param,
                operationImportWithOverloadAnd2Params,
                operationImportWithOverloadAnd5Params
            };
            string result = operationImports.OperationImportGroupFullName();
            result.Should().Be("Default.FunctionImportWithOverload");
        }

        [Fact]
        public void NameWithParametersShouldReturnCorrectValue()
        {
            var action = new EdmAction("d.s", "checkout", null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            action.NameWithParameters().Should().Be("checkout(Edm.String)");
        }

        [Fact]
        public void FullNameWithParametersShouldReturnCorrectValue()
        {
            var action = new EdmAction("d.s", "checkout", null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            action.FullNameWithParameters().Should().Be("d.s.checkout(Edm.String)");
        }

        [Fact]
        public void ResolveEntitySetFromModelShouldReturnNullWhenEntitySetNameIsNullOrEmpty()
        {
            this.model.EntityContainer.FindEntitySet(null).Should().BeNull();
            this.model.EntityContainer.FindEntitySet(null).Should().BeNull();
        }

        [Fact]
        public void ResolveEntitySetFromModelShouldReturnNullWhenEntitySetNameIsNotEntitySetName()
        {
            this.model.EntityContainer.FindEntitySet(".Products").Should().BeNull();
            this.model.EntityContainer.FindEntitySet("Default.").Should().BeNull();
            this.model.EntityContainer.FindEntitySet("Default.Products").Should().BeNull();
            this.model.EntityContainer.FindEntitySet("TestModel.Default.Products").Should().BeNull();
        }

        [Fact]
        public void ResolveEntitySetFromModelShouldReturnNullWhenContainerIsNotFound()
        {
            this.model.EntityContainer.FindEntitySet("UnknownContainer.Products").Should().BeNull();
        }

        [Fact]
        public void ResolveEntitySetFromModelShouldReturnEntitySetWhenNameIsEntitySetNameAndFound()
        {
            this.productsSet.Should().NotBeNull();
            this.model.EntityContainer.FindEntitySet("Products").Should().Be(this.productsSet);
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnNullWhenFunctionImportNameIsNullOrEmpty()
        {
            this.defaultContainer.ResolveOperationImports(null).Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports(string.Empty).Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnNullWhenFunctionImportNameIsNotFullyQualified()
        {
            this.defaultContainer.ResolveOperationImports(".SimpleAction").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("Default.").Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnNullWhenContainerIsNotFound()
        {
            this.defaultContainer.ResolveOperationImports("UnknownContainer.SimpleAction").Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnNullWhenThereIsMoreThan1Overload()
        {
            this.defaultContainer.ResolveOperationImports("Default.SimpleFunctionWithOverload").Count().Should().Be(2);
            this.defaultContainer.ResolveOperationImports("TestModel.Default.SimpleFunctionWithOverload").Count().Should().Be(2);
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnFunctionImportWhenNameIsFullyQualifiedAndFound()
        {
            IEdmOperationImport action1 = this.defaultContainer.ResolveOperationImports("Default.SimpleAction").Single();
            IEdmOperationImport action2 = this.defaultContainer.ResolveOperationImports("TestModel.Default.SimpleAction").Single();
            IEdmOperationImport action3 = this.defaultContainer.ResolveOperationImports("SimpleAction").Single();
            action1.Should().NotBeNull();
            action1.Should().Be(action2);
            action2.FullName().Should().Be("Default.SimpleAction");
            action3.Should().Be(action2);
        }

        [Fact]
        public void ResolveFunctionImportFromModelForFunctionImportWithOverloadAnd5Param()
        {
            IEdmOperationImport function = this.defaultContainer.ResolveOperationImports("Default.FunctionImportWithOverload(Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType))", true /*allowParameterTypes*/).Single();
            function.Should().Be(this.operationImportWithOverloadAnd5Params);
        }

        [Fact]
        public void ResolveFunctionImportFromContainerShouldReturnNullWhenFunctionImportNameIsNullOrEmpty()
        {
            this.defaultContainer.ResolveOperationImports(null).Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports(string.Empty).Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromContainerShouldReturnNullWhenFunctionImportIsNotFound()
        {
            this.defaultContainer.ResolveOperationImports("UnknownAction").Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromContainerShouldReturnNullWhenThereIsMoreThan1Overload()
        {
            IEnumerable<IEdmOperationImport> functionGroup = this.defaultContainer.ResolveOperationImports("SimpleFunctionWithOverload");
            functionGroup.Should().NotBeNull();
            functionGroup.Count().Should().Be(2);
            functionGroup.All(f => f.Name == "SimpleFunctionWithOverload");
        }

        [Fact]
        public void ResolveFunctionImportFromContainerShouldReturnFunctionImportWhenNameIsFound()
        {
            IEdmOperationImport action = this.defaultContainer.ResolveOperationImports("SimpleAction").Single();
            action.Should().NotBeNull();
            action.FullName().Should().Be("Default.SimpleAction");

            action = this.defaultContainer.ResolveOperationImports("SimpleAction()").Single();
            action.Should().NotBeNull();
            action.FullName().Should().Be("Default.SimpleAction");

            this.defaultContainer.ResolveOperationImports("SimpleAction( )").Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithNoOverload()
        {
            this.defaultContainer.ResolveOperationImports("FunctionImportWithNoOverload").Single().Should().Be(this.operationImportWithNoOverload);
            this.defaultContainer.ResolveOperationImports("FunctionImportWithNoOverload()").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithNoOverload(Edm.Int32)").Single().Should().Be(this.operationImportWithNoOverload);
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadFunctionGroup()
        {
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload").Count().Should().Be(4);
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload").All(f => f.Name == "FunctionImportWithOverload").Should().BeTrue();
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadAnd0Param()
        {
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload()").Single().Should().Be(this.operationImportWithOverloadAnd0Param);

            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload( )").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload ()").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload() ").Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadAnd1Param()
        {
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Edm.Int32)").Single().Should().Be(this.operationImportWithOverloadAnd1Param);

            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload( Edm.Int32 )").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload (Edm.Int32)").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Edm.Int32) ").Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadAnd2Param()
        {
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(TestModel.Product,Edm.String)").Single().Should().Be(this.operationImportWithOverloadAnd2Params);

            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload( TestModel.Product , Edm.String )").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload (TestModel.Product,Edm.String)").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(TestModel.Product,Edm.String) ").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(TestModel.Product,Edm.String").Should().BeEmpty();
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadAnd5Param()
        {
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType))").Single().Should().Be(this.operationImportWithOverloadAnd5Params);

            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload( Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType) )").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Collection(TestModel.Product), Collection(Edm.String), Edm.String, TestModel.MyComplexType, Collection(TestModel.MyComplexType))").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload (Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType))").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType)) ").Should().BeEmpty();
            this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType)").Should().BeEmpty();
        }

        [Fact]
        public void ValidateGeometrySubTypesAssignableToGeometryBaseType()
        {
            ValidateAssignableToType(true, EdmPrimitiveTypeKind.Geometry, GeometryAllTypeKinds);
        }

        [Fact]
        public void ValidateIsGeometryCollectionSubTypesAssignableToGeometryBaseType()
        {
            ValidateAssignableToType(true, EdmPrimitiveTypeKind.GeometryCollection, GeometryMultiLeafTypeKinds);
        }

        [Fact]
        public void ValidateIsGeometryCollectionNotAssignableToSingleLeafTypes()
        {
            ValidateAssignableToType(false, EdmPrimitiveTypeKind.GeometryCollection, GeometrySingleLeafTypeKinds);
        }

        [Fact]
        public void ValidateIsGeographySubTypesAssignableToGeographyBaseType()
        {
            ValidateAssignableToType(true, EdmPrimitiveTypeKind.Geography, GeographyAllTypeKinds);
        }

        [Fact]
        public void ValidateIsGeographyMultiSubTypesAssignableToGeographyCollectionBaseType()
        {
            ValidateAssignableToType(true, EdmPrimitiveTypeKind.GeographyCollection, GeographyMultiLeafTypeKinds);
        }

        [Fact]
        public void ValidateIsGeographySingleSubTypesNotAssignableToGeographyCollectionBaseType()
        {
            ValidateAssignableToType(false, EdmPrimitiveTypeKind.GeographyCollection, GeographySingleLeafTypeKinds);
        }

        [Fact]
        public void ValidateGeographyIsNotAssignableToGeometry()
        {
            ValidateAssignableToType(false, EdmPrimitiveTypeKind.Geometry, EdmPrimitiveTypeKind.Geography);
        }

        [Fact]
        public void FunctionImportNameWithNoParameters()
        {
            this.operationImportWithOverloadAnd0Param.NameWithParameters().Should().Be("FunctionImportWithOverload()");
        }

        [Fact]
        public void FunctionImportNameWithOneParameter()
        {
            this.operationImportWithOverloadAnd1Param.NameWithParameters().Should().Be("FunctionImportWithOverload(Edm.Int32)");
        }

        [Fact]
        public void FunctionImportNameWithManyParameters()
        {
            this.operationImportWithOverloadAnd2Params.NameWithParameters().Should().Be("FunctionImportWithOverload(TestModel.Product,Edm.String)");
        }

        [Fact]
        public void ResolveFunctionImportWithoutParameterNamesShouldNotCallModelIfParenthesesAreFound()
        {
            var tempModel = new EdmModel();
            var container = new EntityContainerThatThrowsOnLookup("Fake", "Container");
            tempModel.AddElement(new EntityContainerThatThrowsOnLookup("Fake", "Container"));
            container.ResolveOperationImports("Action()", false).Should().BeEmpty();
        }

        [Fact]
        public void ShortQualifiedNameForCollectionOfNonPrimitiveTypeShouldBeCollectionOfFullName()
        {
            const string stringOfNamespaceName = "TestModel";
            const string stringOfComplexTypeName = "MyComplexType";
            
            var edmComplexType = new EdmComplexType(stringOfNamespaceName, stringOfComplexTypeName);
            var edmCollectionType = new EdmCollectionType(new EdmComplexTypeReference(edmComplexType, true));

            var stringOfExpectedShortQulifiedName = String.Format("Collection({0}.{1})", stringOfNamespaceName, stringOfComplexTypeName);
            var stringOfObservedShortQulifiedName = edmCollectionType.ODataShortQualifiedName();
            stringOfObservedShortQulifiedName.Should().Be(stringOfExpectedShortQulifiedName);

            const string stringEntityTypeName = "MyEntityType";
            var edmEntityType = new EdmEntityType(stringOfNamespaceName, stringEntityTypeName);
            edmCollectionType = new EdmCollectionType(new EdmEntityTypeReference(edmEntityType, true));

            stringOfExpectedShortQulifiedName = String.Format("Collection({0}.{1})", stringOfNamespaceName, stringEntityTypeName);
            stringOfObservedShortQulifiedName = edmCollectionType.ODataShortQualifiedName();
            stringOfObservedShortQulifiedName.Should().Be(stringOfExpectedShortQulifiedName);
        }

        [Fact]
        public void ShortQualifiedNameForCollectionPrimitiveTypeShouldBeCollectionOfName()
        {
            foreach (EdmPrimitiveTypeKind edmPrimitiveTypeKind in Enum.GetValues(typeof(EdmPrimitiveTypeKind)))
            {
                if (EdmPrimitiveTypeKind.None == edmPrimitiveTypeKind)
                    continue;
                var stringOfName = Enum.GetName(typeof(EdmPrimitiveTypeKind), edmPrimitiveTypeKind);
                stringOfName.ToUpper().Should().NotContain("EDM.");

                var stringOfExpectedShortQulifiedName = String.Format("Collection({0})", stringOfName);
                var iEdmPrimitiveType = EdmCoreModel.Instance.GetPrimitiveType(edmPrimitiveTypeKind);
                var edmCollectionType=new EdmCollectionType(new EdmPrimitiveTypeReference(iEdmPrimitiveType,true));
                var stringOfObservedShortQulifiedName = edmCollectionType.ODataShortQualifiedName();
                stringOfObservedShortQulifiedName.Should().Be(stringOfExpectedShortQulifiedName);
            }
        }

        [Fact]
        public void AsCollectionOrNullForNonCollectionShouldBeNull()
        {
            IEdmCollectionTypeReference collectionTypeReference = EdmLibraryExtensions.AsCollectionOrNull((IEdmTypeReference)productTypeReference);
            collectionTypeReference.Should().BeNull();
        }

        [Fact]
        public void AsCollectionOrNullForNonCollectionOfEntityShouldBeNull()
        {
            IEdmTypeReference typeReference = new EdmCollectionTypeReference(new EdmCollectionType(productTypeReference));
            IEdmCollectionTypeReference collectionTypeReference = typeReference.AsCollectionOrNull();
            collectionTypeReference.Should().BeNull();
        }

        [Theory]
        [InlineData(EdmPrimitiveTypeKind.Binary)]
        [InlineData(EdmPrimitiveTypeKind.Decimal)]
        [InlineData(EdmPrimitiveTypeKind.Int32)]
        [InlineData(EdmPrimitiveTypeKind.TimeOfDay)]
        [InlineData(EdmPrimitiveTypeKind.Geography)]
        [InlineData(EdmPrimitiveTypeKind.Geometry)]
        public void BaseTypeForBuiltInPrimitiveTypesShouldBeNull(EdmPrimitiveTypeKind kind)
        {
            EdmCoreModel.Instance.GetPrimitiveType(kind).BaseType().Should().BeNull();
        }

        [Theory]
        [InlineData(EdmPrimitiveTypeKind.GeographyPoint, EdmPrimitiveTypeKind.Geography)]
        [InlineData(EdmPrimitiveTypeKind.GeographyLineString, EdmPrimitiveTypeKind.Geography)]
        [InlineData(EdmPrimitiveTypeKind.GeographyPolygon, EdmPrimitiveTypeKind.Geography)]
        [InlineData(EdmPrimitiveTypeKind.GeographyCollection, EdmPrimitiveTypeKind.Geography)]
        [InlineData(EdmPrimitiveTypeKind.GeographyMultiPolygon, EdmPrimitiveTypeKind.GeographyCollection)]
        [InlineData(EdmPrimitiveTypeKind.GeographyMultiLineString, EdmPrimitiveTypeKind.GeographyCollection)]
        [InlineData(EdmPrimitiveTypeKind.GeographyMultiPoint, EdmPrimitiveTypeKind.GeographyCollection)]
        [InlineData(EdmPrimitiveTypeKind.GeometryPoint, EdmPrimitiveTypeKind.Geometry)]
        [InlineData(EdmPrimitiveTypeKind.GeometryLineString, EdmPrimitiveTypeKind.Geometry)]
        [InlineData(EdmPrimitiveTypeKind.GeometryPolygon, EdmPrimitiveTypeKind.Geometry)]
        [InlineData(EdmPrimitiveTypeKind.GeometryCollection, EdmPrimitiveTypeKind.Geometry)]
        [InlineData(EdmPrimitiveTypeKind.GeometryMultiPolygon, EdmPrimitiveTypeKind.GeometryCollection)]
        [InlineData(EdmPrimitiveTypeKind.GeometryMultiLineString, EdmPrimitiveTypeKind.GeometryCollection)]
        [InlineData(EdmPrimitiveTypeKind.GeometryMultiPoint, EdmPrimitiveTypeKind.GeometryCollection)]
        public void BaseTypeForSpatialTypesShouldBeSameAsExpect(EdmPrimitiveTypeKind kind, EdmPrimitiveTypeKind expect)
        {
            var baseType = EdmCoreModel.Instance.GetPrimitiveType(expect);
            EdmCoreModel.Instance.GetPrimitiveType(kind).BaseType().Should().BeSameAs(baseType);
        }

        [Theory]
        [InlineData(EdmPrimitiveTypeKind.Int32, EdmPrimitiveTypeKind.Int32, EdmPrimitiveTypeKind.Int32)]
        [InlineData(EdmPrimitiveTypeKind.Int32, EdmPrimitiveTypeKind.Int64, EdmPrimitiveTypeKind.None)]
        [InlineData(EdmPrimitiveTypeKind.Int64, EdmPrimitiveTypeKind.Int32, EdmPrimitiveTypeKind.None)]
        [InlineData(EdmPrimitiveTypeKind.GeographyPoint, EdmPrimitiveTypeKind.Geography, EdmPrimitiveTypeKind.Geography)]
        [InlineData(EdmPrimitiveTypeKind.Geography, EdmPrimitiveTypeKind.GeographyPoint, EdmPrimitiveTypeKind.Geography)]
        [InlineData(EdmPrimitiveTypeKind.GeometryPoint, EdmPrimitiveTypeKind.Geometry, EdmPrimitiveTypeKind.Geometry)]
        [InlineData(EdmPrimitiveTypeKind.Geometry, EdmPrimitiveTypeKind.GeometryPoint, EdmPrimitiveTypeKind.Geometry)]
        public void GetCommonBaseTypeForBuiltInTypesShouldBeExpect(EdmPrimitiveTypeKind first, EdmPrimitiveTypeKind second, EdmPrimitiveTypeKind commonBase)
        {
            var firstType = EdmCoreModel.Instance.GetPrimitiveType(first);
            var secondType = EdmCoreModel.Instance.GetPrimitiveType(second);
            var commonBaseType = EdmCoreModel.Instance.GetPrimitiveType(commonBase);
            var actual = firstType.GetCommonBaseType(secondType);
            actual.Should().BeSameAs(commonBaseType);
        }

        [Fact]
        public void CloneForNullShouldBeNull()
        {
            EdmLibraryExtensions.Clone(null, false).Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CloneForEntityShouldBeExpect(bool nullable)
        {
            IEdmTypeReference typeReference = productTypeReference.Clone(nullable);
            typeReference.Should().BeOfType<EdmEntityTypeReference>();
            typeReference.IsNullable.Should().Be(nullable);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CloneForCollectionShouldBeExpect(bool nullable)
        {
            EdmComplexType complexType = new EdmComplexType("TestModel", "MyComplexType");
            EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(complexType, isNullable: nullable);
            IEdmTypeReference typeReference = new EdmCollectionTypeReference(new EdmCollectionType(complexTypeReference));

            IEdmTypeReference clonedType = typeReference.Clone(nullable);
            clonedType.Should().BeOfType<EdmCollectionTypeReference>();
            clonedType.IsNullable.Should().Be(nullable);

            clonedType.AsCollection().ElementType().IsNullable.Should().Be(nullable);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CloneForEnumTypeShouldBeExpect(bool nullable)
        {
            EdmEnumType enumType = new EdmEnumType("NS", "MyEnum");
            EdmEnumTypeReference enumTypeReference = new EdmEnumTypeReference(enumType, isNullable: nullable);

            IEdmTypeReference clonedType = enumTypeReference.Clone(nullable);
            clonedType.Should().BeOfType<EdmEnumTypeReference>();
            clonedType.IsNullable.Should().Be(nullable);
        }

        [Fact]
        public void IsUserModelForUserModelShouldBeTrue()
        {
            bool result = model.IsUserModel();
            result.Should().BeTrue();
        }

        [Fact]
        public void IsUserModelForCoreModelShouldBeFalse()
        {
            bool result = EdmCoreModel.Instance.IsUserModel();
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(UInt16))]
        [InlineData(typeof(UInt32))]
        [InlineData(typeof(UInt64))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(float))]
        [InlineData(typeof(Geography))]
        [InlineData(typeof(Geometry))]
        public void IsPrimitiveTypeForSupportedTypesShouldBeTrue(Type type)
        {
            bool result = EdmLibraryExtensions.IsPrimitiveType(type);
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(EdmLibraryExtensionsTests))]
        public void IsPrimitiveTypeForUnsupportedTypesShouldBeFalse(Type type)
        {
            bool result = EdmLibraryExtensions.IsPrimitiveType(type);
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(EdmPrimitiveTypeKind.Boolean, true, typeof(bool?))]
        [InlineData(EdmPrimitiveTypeKind.Boolean, false, typeof(bool))]
        [InlineData(EdmPrimitiveTypeKind.Int32, true, typeof(int?))]
        [InlineData(EdmPrimitiveTypeKind.Int32, false, typeof(int))]
        [InlineData(EdmPrimitiveTypeKind.Stream, true, typeof(Stream))]
        [InlineData(EdmPrimitiveTypeKind.Stream, false, typeof(Stream))]
        [InlineData(EdmPrimitiveTypeKind.Geography, true, typeof(Geography))]
        [InlineData(EdmPrimitiveTypeKind.Geography, false, typeof(Geography))]
        [InlineData(EdmPrimitiveTypeKind.GeographyCollection, true, typeof(GeographyCollection))]
        [InlineData(EdmPrimitiveTypeKind.GeographyCollection, false, typeof(GeographyCollection))]
        [InlineData(EdmPrimitiveTypeKind.Geometry, true, typeof(Geometry))]
        [InlineData(EdmPrimitiveTypeKind.Geometry, false, typeof(Geometry))]
        [InlineData(EdmPrimitiveTypeKind.GeometryCollection, true, typeof(GeometryCollection))]
        [InlineData(EdmPrimitiveTypeKind.GeometryCollection, false, typeof(GeometryCollection))]
        public void GetPrimitiveClrTypeForBuiltInTypesShouldBeExpect(EdmPrimitiveTypeKind kind, bool nullable, Type expect)
        {
            IEdmPrimitiveType primitiveType = EdmCoreModel.Instance.GetPrimitiveType(kind);
            Type actual = EdmLibraryExtensions.GetPrimitiveClrType(primitiveType, nullable);
            actual.Should().Be(expect);
        }

        private static void ValidateAssignableToType(bool isAssignableExpectedResult, EdmPrimitiveTypeKind isAssignableToTypeKind, params EdmPrimitiveTypeKind[] subTypeKinds)
        {
            var isAssignableToType = EdmCoreModel.Instance.GetPrimitiveType(isAssignableToTypeKind);
            foreach (var assignableFromType in subTypeKinds.Select(tk=>EdmCoreModel.Instance.GetPrimitiveType(tk)))
            {
                Assert.Equal(isAssignableExpectedResult, isAssignableToType.IsAssignableFrom(assignableFromType));
            }
        }

        private class EntityContainerThatThrowsOnLookup : EdmEntityContainer, IEdmEntityContainer
        {
            public EntityContainerThatThrowsOnLookup(string namespaceName, string name) 
                : base(namespaceName, name)
            {
            }

            public override IEdmEntitySet FindEntitySet(string setName)
            {
                throw new NotImplementedException();
            }

            public new IEnumerable<IEdmOperationImport> FindOperationImports(string operationName)
            {
                throw new NotImplementedException();
            }

            IEnumerable<IEdmOperationImport> IEdmEntityContainer.FindOperationImports(string operationName)
            {
                throw new NotImplementedException();
            }
        }
    }
}
