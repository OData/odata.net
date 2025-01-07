//---------------------------------------------------------------------
// <copyright file="EdmLibraryExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OData.Metadata;
using Microsoft.OData.Tests.Evaluation;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

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
            var result = new IEdmOperation[] { action }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(EdmCoreModel.Instance.GetSingle(true).Definition);
            Assert.Single(result);
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
            var result = Assert.Single(filteredResults);
            Assert.Same(action2, result);
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
            var result = Assert.Single(filteredResults);
            Assert.Same(action2, result);
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
            var result = Assert.Single(filteredResults);
            Assert.Same(action2, result);
        }

        [Fact]
        public void FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingTypeShouldNotThrowAndFilterNonBoundOperations()
        {
            EdmEntityType aType = new EdmEntityType("N", "A");
            EdmAction action = new EdmAction("namespace", "action", null, false, null);
            action.AddParameter("bindingParameter", new EdmEntityTypeReference(aType, false));
            var filteredResults = new IEdmOperation[] { action }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(aType).ToList();
            Assert.Empty(filteredResults);
        }

        [Fact]
        public void FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingTypeShouldNotThrowAndFilterBoundOperationsWithNoParameters()
        {
            EdmEntityType aType = new EdmEntityType("N", "A");
            EdmAction action = new EdmAction("namespace", "action", null, false, null);
            var filteredResults = new IEdmOperation[] { action }.FilterBoundOperationsWithSameTypeHierarchyToTypeClosestToBindingType(aType).ToList();
            Assert.Empty(filteredResults);
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

            var functions = new EdmOperation[] { function, functionOverload1, functionOverload2 };
            var resolvedFunction = functions.FilterOperationsByParameterNames(new string[] { "foo" }, false);
            Assert.Same(resolvedFunction.First(), functionOverload1);
        }

        [Fact]
        public void ResolveFunctionsOverloadsByParameterNameShouldNotThrowIfMultipleResolve()
        {
            var function = new EdmFunction("d.s", "function1", EdmCoreModel.Instance.GetSingle(false));
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            var function1 = new EdmFunction("d.s", "function1", EdmCoreModel.Instance.GetSingle(false));
            function1.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            var functions = new EdmOperation[] { function, function1 };

            var selectedFunctions = functions.FilterOperationsByParameterNames(new string[] { "param1" }, false);
            Assert.Equal(2, selectedFunctions.Count());
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
            var result = Assert.Single(returnedOperations);
            Assert.Same(result, function);
        }

        [Fact]
        public void ResolveOperationsWithOperationNameWithoutParameterNamesShouldReturnCorrectOperations()
        {
            var model = new EdmModel();
            var function = new EdmFunction("ds", "function1", EdmCoreModel.Instance.GetSingle(false));
            model.AddElement(function);
            var returnedOperations = model.ResolveOperations("ds.function1", false /*allowParameterTypeNames*/).ToList();
            var result = Assert.Single(returnedOperations);
            Assert.Same(result, function);
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
            var result = Assert.Single(returnedOperations);
            Assert.Same(result, function);
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
            var result = Assert.Single(returnedOperations);
            Assert.Same(result, function);
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
            Assert.Equal("Default.FunctionImportWithOverload", result);
        }

        [Fact]
        public void NameWithParametersShouldReturnCorrectValue()
        {
            var action = new EdmAction("d.s", "checkout", null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            Assert.Equal("checkout(Edm.String)", action.NameWithParameters());
        }

        [Fact]
        public void FullNameWithParametersShouldReturnCorrectValue()
        {
            var action = new EdmAction("d.s", "checkout", null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            Assert.Equal("d.s.checkout(Edm.String)", action.FullNameWithParameters());
        }

        [Fact]
        public void ResolveEntitySetFromModelShouldReturnNullWhenEntitySetNameIsNullOrEmpty()
        {
            Assert.Null(this.model.EntityContainer.FindEntitySet(null));
            Assert.Null(this.model.EntityContainer.FindEntitySet(null));
        }

        [Fact]
        public void ResolveEntitySetFromModelShouldReturnNullWhenEntitySetNameIsNotEntitySetName()
        {
            Assert.Null(this.model.EntityContainer.FindEntitySet(".Products"));
            Assert.Null(this.model.EntityContainer.FindEntitySet("Default."));
            Assert.Null(this.model.EntityContainer.FindEntitySet("Default.Products"));
            Assert.Null(this.model.EntityContainer.FindEntitySet("TestModel.Default.Products"));
        }

        [Fact]
        public void ResolveEntitySetFromModelShouldReturnNullWhenContainerIsNotFound()
        {
            Assert.Null(this.model.EntityContainer.FindEntitySet("UnknownContainer.Products"));
        }

        [Fact]
        public void ResolveEntitySetFromModelShouldReturnEntitySetWhenNameIsEntitySetNameAndFound()
        {
            Assert.NotNull(this.productsSet);
            Assert.Same(this.productsSet, this.model.EntityContainer.FindEntitySet("Products"));
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnNullWhenFunctionImportNameIsNullOrEmpty()
        {
            Assert.Empty(this.defaultContainer.ResolveOperationImports(null));
            Assert.Empty(this.defaultContainer.ResolveOperationImports(string.Empty));
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnNullWhenFunctionImportNameIsNotFullyQualified()
        {
            Assert.Empty(this.defaultContainer.ResolveOperationImports(".SimpleAction"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("Default."));
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnNullWhenContainerIsNotFound()
        {
            Assert.Empty(this.defaultContainer.ResolveOperationImports("UnknownContainer.SimpleAction"));
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnNullWhenThereIsMoreThan1Overload()
        {
            Assert.Equal(2, this.defaultContainer.ResolveOperationImports("Default.SimpleFunctionWithOverload").Count());
            Assert.Equal(2, this.defaultContainer.ResolveOperationImports("TestModel.Default.SimpleFunctionWithOverload").Count());
        }

        [Fact]
        public void ResolveFunctionImportFromModelShouldReturnFunctionImportWhenNameIsFullyQualifiedAndFound()
        {
            IEdmOperationImport action1 = this.defaultContainer.ResolveOperationImports("Default.SimpleAction").Single();
            IEdmOperationImport action2 = this.defaultContainer.ResolveOperationImports("TestModel.Default.SimpleAction").Single();
            IEdmOperationImport action3 = this.defaultContainer.ResolveOperationImports("SimpleAction").Single();
            Assert.NotNull(action1);
            Assert.Same(action1, action2);
            Assert.Equal("Default.SimpleAction", action2.FullName());
            Assert.Same(action3, action2);
        }

        [Fact]
        public void ResolveFunctionImportFromModelForFunctionImportWithOverloadAnd5Param()
        {
            IEdmOperationImport function = this.defaultContainer.ResolveOperationImports("Default.FunctionImportWithOverload(Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType))", true /*allowParameterTypes*/).Single();
            Assert.Same(this.operationImportWithOverloadAnd5Params, function);
        }

        [Fact]
        public void ResolveFunctionImportFromContainerShouldReturnNullWhenFunctionImportNameIsNullOrEmpty()
        {
            Assert.Empty(this.defaultContainer.ResolveOperationImports(null));
            Assert.Empty(this.defaultContainer.ResolveOperationImports(string.Empty));
        }

        [Fact]
        public void ResolveFunctionImportFromContainerShouldReturnNullWhenFunctionImportIsNotFound()
        {
            Assert.Empty(this.defaultContainer.ResolveOperationImports("UnknownAction"));
        }

        [Fact]
        public void ResolveFunctionImportFromContainerShouldReturnNullWhenThereIsMoreThan1Overload()
        {
            IEnumerable<IEdmOperationImport> functionGroup = this.defaultContainer.ResolveOperationImports("SimpleFunctionWithOverload");
            Assert.NotNull(functionGroup);
            Assert.Equal(2, functionGroup.Count());
            functionGroup.All(f => f.Name == "SimpleFunctionWithOverload");
        }

        [Fact]
        public void ResolveFunctionImportFromContainerShouldReturnFunctionImportWhenNameIsFound()
        {
            IEdmOperationImport action = this.defaultContainer.ResolveOperationImports("SimpleAction").Single();
            Assert.NotNull(action);
            Assert.Equal("Default.SimpleAction", action.FullName());

            action = this.defaultContainer.ResolveOperationImports("SimpleAction()").Single();
            Assert.NotNull(action);
            Assert.Equal("Default.SimpleAction", action.FullName());

            Assert.Empty(this.defaultContainer.ResolveOperationImports("SimpleAction( )"));
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithNoOverload()
        {
            Assert.Same(this.defaultContainer.ResolveOperationImports("FunctionImportWithNoOverload").Single(), this.operationImportWithNoOverload);
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithNoOverload()"));
            Assert.Same(this.defaultContainer.ResolveOperationImports("FunctionImportWithNoOverload(Edm.Int32)").Single(), this.operationImportWithNoOverload);
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadFunctionGroup()
        {
            Assert.Equal(4, this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload").Count());
            Assert.True(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload").All(f => f.Name == "FunctionImportWithOverload"));
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadAnd0Param()
        {
            Assert.Same(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload()").Single(), this.operationImportWithOverloadAnd0Param);

            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload( )"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload ()"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload() "));
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadAnd1Param()
        {
            Assert.Same(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Edm.Int32)").Single(), this.operationImportWithOverloadAnd1Param);

            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload( Edm.Int32 )"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload (Edm.Int32)"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Edm.Int32) "));
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadAnd2Param()
        {
            Assert.Same(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(TestModel.Product,Edm.String)").Single(), this.operationImportWithOverloadAnd2Params);

            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload( TestModel.Product , Edm.String )"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload (TestModel.Product,Edm.String)"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(TestModel.Product,Edm.String) "));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(TestModel.Product,Edm.String"));
        }

        [Fact]
        public void ResolveFunctionImportFromContainerForFunctionImportWithOverloadAnd5Param()
        {
            Assert.Same(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType))").Single(), this.operationImportWithOverloadAnd5Params);

            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload( Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType) )"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Collection(TestModel.Product), Collection(Edm.String), Edm.String, TestModel.MyComplexType, Collection(TestModel.MyComplexType))"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload (Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType))"));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType)) "));
            Assert.Empty(this.defaultContainer.ResolveOperationImports("FunctionImportWithOverload(Collection(TestModel.Product),Collection(Edm.String),Edm.String,TestModel.MyComplexType,Collection(TestModel.MyComplexType)"));
        }

        [Fact]
        public void ValidateAllPrimitiveTypesAssignableToEdmPrimitiveType()
        {
            foreach (var typeKind in Enum.GetValues(typeof(EdmPrimitiveTypeKind)).OfType<EdmPrimitiveTypeKind>())
            {
                if (typeKind != EdmPrimitiveTypeKind.None)
                {
                    ValidateAssignableToType(true, EdmPrimitiveTypeKind.PrimitiveType, typeKind);
                }
            }
        }

        [Fact]
        public void ValidateComplexTypeAssignableToEdmComplexType()
        {
            IEdmComplexType baseType = EdmCoreModel.Instance.GetComplexType();
            EdmComplexType complexType = new EdmComplexType("NS", "Complex");
            Assert.True(baseType.IsAssignableFrom(complexType));
        }

        [Fact]
        public void ValidateEntityTypeAssignableToEdmEntityType()
        {
            IEdmEntityType baseType = EdmCoreModel.Instance.GetEntityType();
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            Assert.True(baseType.IsAssignableFrom(entityType));
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
            Assert.Equal("FunctionImportWithOverload()", this.operationImportWithOverloadAnd0Param.NameWithParameters());
        }

        [Fact]
        public void FunctionImportNameWithOneParameter()
        {
            Assert.Equal("FunctionImportWithOverload(Edm.Int32)", this.operationImportWithOverloadAnd1Param.NameWithParameters());
        }

        [Fact]
        public void FunctionImportNameWithManyParameters()
        {
            Assert.Equal("FunctionImportWithOverload(TestModel.Product,Edm.String)",
                this.operationImportWithOverloadAnd2Params.NameWithParameters());
        }

        [Fact]
        public void ResolveFunctionImportWithoutParameterNamesShouldNotCallModelIfParenthesesAreFound()
        {
            var tempModel = new EdmModel();
            var container = new EntityContainerThatThrowsOnLookup("Fake", "Container");
            tempModel.AddElement(new EntityContainerThatThrowsOnLookup("Fake", "Container"));
            Assert.Empty(container.ResolveOperationImports("Action()", false));
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
            Assert.Equal(stringOfExpectedShortQulifiedName, stringOfObservedShortQulifiedName);

            const string stringEntityTypeName = "MyEntityType";
            var edmEntityType = new EdmEntityType(stringOfNamespaceName, stringEntityTypeName);
            edmCollectionType = new EdmCollectionType(new EdmEntityTypeReference(edmEntityType, true));

            stringOfExpectedShortQulifiedName = String.Format("Collection({0}.{1})", stringOfNamespaceName, stringEntityTypeName);
            stringOfObservedShortQulifiedName = edmCollectionType.ODataShortQualifiedName();
            Assert.Equal(stringOfExpectedShortQulifiedName, stringOfObservedShortQulifiedName);
        }

        [Fact]
        public void ShortQualifiedNameForCollectionPrimitiveTypeShouldBeCollectionOfName()
        {
            foreach (EdmPrimitiveTypeKind edmPrimitiveTypeKind in Enum.GetValues(typeof(EdmPrimitiveTypeKind)))
            {
                if (EdmPrimitiveTypeKind.None == edmPrimitiveTypeKind)
                    continue;
                var stringOfName = Enum.GetName(typeof(EdmPrimitiveTypeKind), edmPrimitiveTypeKind);
                Assert.DoesNotContain("EDM.", stringOfName.ToUpper());

                var stringOfExpectedShortQulifiedName = String.Format("Collection({0})", stringOfName);
                var iEdmPrimitiveType = EdmCoreModel.Instance.GetPrimitiveType(edmPrimitiveTypeKind);
                var edmCollectionType=new EdmCollectionType(new EdmPrimitiveTypeReference(iEdmPrimitiveType,true));
                var stringOfObservedShortQulifiedName = edmCollectionType.ODataShortQualifiedName();
                Assert.Equal(stringOfExpectedShortQulifiedName, stringOfObservedShortQulifiedName);
            }
        }

        [Fact]
        public void AsCollectionOrNullForNonCollectionShouldBeNull()
        {
            IEdmCollectionTypeReference collectionTypeReference = EdmLibraryExtensions.AsCollectionOrNull((IEdmTypeReference)productTypeReference);
            Assert.Null(collectionTypeReference);
        }

        [Fact]
        public void AsCollectionOrNullForNonCollectionOfEntityShouldBeNull()
        {
            IEdmTypeReference typeReference = new EdmCollectionTypeReference(new EdmCollectionType(productTypeReference));
            IEdmCollectionTypeReference collectionTypeReference = typeReference.AsCollectionOrNull();
            Assert.Null(collectionTypeReference);
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
            Assert.Null(EdmCoreModel.Instance.GetPrimitiveType(kind).BaseType());
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
            Assert.Same(baseType, EdmCoreModel.Instance.GetPrimitiveType(kind).BaseType());
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
            Assert.Same(commonBaseType, actual);
        }

        [Fact]
        public void CloneForNullShouldBeNull()
        {
            Assert.Null(EdmLibraryExtensions.Clone(null, false));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CloneForEntityShouldBeExpect(bool nullable)
        {
            IEdmTypeReference typeReference = productTypeReference.Clone(nullable);
            Assert.IsType<EdmEntityTypeReference>(typeReference);
            Assert.Equal(nullable, typeReference.IsNullable);
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
            Assert.IsType<EdmCollectionTypeReference>(clonedType);
            Assert.Equal(nullable, clonedType.IsNullable);

            Assert.Equal(nullable, clonedType.AsCollection().ElementType().IsNullable);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CloneForEnumTypeShouldBeExpect(bool nullable)
        {
            EdmEnumType enumType = new EdmEnumType("NS", "MyEnum");
            EdmEnumTypeReference enumTypeReference = new EdmEnumTypeReference(enumType, isNullable: nullable);

            IEdmTypeReference clonedType = enumTypeReference.Clone(nullable);
            Assert.IsType<EdmEnumTypeReference>(clonedType);
            Assert.Equal(nullable, clonedType.IsNullable);
        }

        [Fact]
        public void IsUserModelForUserModelShouldBeTrue()
        {
            bool result = model.IsUserModel();
            Assert.True(result);
        }

        [Fact]
        public void IsUserModelForCoreModelShouldBeFalse()
        {
            bool result = EdmCoreModel.Instance.IsUserModel();
            Assert.False(result);
        }

        [Theory]
        [InlineData(typeof(UInt16))]
        [InlineData(typeof(UInt32))]
        [InlineData(typeof(UInt64))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(float))]
        [InlineData(typeof(Date))]
        [InlineData(typeof(DateOnly))]
        [InlineData(typeof(DateOnly?))]
        [InlineData(typeof(TimeOnly))]
        [InlineData(typeof(TimeOnly?))]
        [InlineData(typeof(Geography))]
        [InlineData(typeof(Geometry))]
        public void IsPrimitiveTypeForSupportedTypesShouldBeTrue(Type type)
        {
            bool result = EdmLibraryExtensions.IsPrimitiveType(type);
            Assert.True(result);
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(EdmLibraryExtensionsTests))]
        public void IsPrimitiveTypeForUnsupportedTypesShouldBeFalse(Type type)
        {
            bool result = EdmLibraryExtensions.IsPrimitiveType(type);
            Assert.False(result);
        }

        [Theory]
        [InlineData(typeof(Date), EdmPrimitiveTypeKind.Date, false)]
        [InlineData(typeof(Date?), EdmPrimitiveTypeKind.Date, true)]
        [InlineData(typeof(TimeOfDay), EdmPrimitiveTypeKind.TimeOfDay, false)]
        [InlineData(typeof(TimeOfDay?), EdmPrimitiveTypeKind.TimeOfDay, true)]
        [InlineData(typeof(DateOnly), EdmPrimitiveTypeKind.Date, false)]
        [InlineData(typeof(DateOnly?), EdmPrimitiveTypeKind.Date, true)]
        [InlineData(typeof(TimeOnly), EdmPrimitiveTypeKind.TimeOfDay, false)]
        [InlineData(typeof(TimeOnly?), EdmPrimitiveTypeKind.TimeOfDay, true)]
        public void GetPrimitiveTypeReferenceForDateOnlyTimeOnlyShouldReturnCorrectEdmType(Type clrType, EdmPrimitiveTypeKind kind, bool nullable)
        {
            IEdmPrimitiveTypeReference primitiveTypeRef = EdmLibraryExtensions.GetPrimitiveTypeReference(clrType);
            Assert.Equal(kind, primitiveTypeRef.PrimitiveKind());
            Assert.Equal(nullable, primitiveTypeRef.IsNullable);
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
            Assert.Equal(expect, actual);
        }

        private static void ValidateAssignableToType(bool isAssignableExpectedResult, EdmPrimitiveTypeKind isAssignableToTypeKind, params EdmPrimitiveTypeKind[] subTypeKinds)
        {
            var isAssignableToType = EdmCoreModel.Instance.GetPrimitiveType(isAssignableToTypeKind);
            foreach (var assignableFromType in subTypeKinds.Select(tk=>EdmCoreModel.Instance.GetPrimitiveType(tk)))
            {
                Assert.Equal(isAssignableExpectedResult, isAssignableToType.IsAssignableFrom(assignableFromType));
            }
        }

        private class EntityContainerThatThrowsOnLookup : EdmEntityContainer, IEdmEntityContainer, IEdmFullNamedElement
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
