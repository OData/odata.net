//---------------------------------------------------------------------
// <copyright file="ODataJsonUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonUtilsTests
    {
        private EdmModel model;
        private Uri MetadataDocumentUri;
        private EdmEntityType cityType;

        private EdmFunction operationWithNoOverload;
        private EdmFunction operationWithOverloadAnd0Param;
        private EdmFunction operationWithOverloadAnd1Param;
        private EdmFunction operationWithOverloadAnd2Params;
        private EdmFunction operationWithOverloadAnd5Params;
        private EdmFunctionImport operationImportWithNoOverload;
        private EdmFunctionImport operationImportWithOverloadAnd0Param;
        private EdmFunctionImport operationImportWithOverloadAnd1Param;
        private EdmFunctionImport operationImportWithOverloadAnd2Params;
        private EdmFunctionImport operationImportWithOverloadAnd5Params;

        public ODataJsonUtilsTests()
        {
            this.MetadataDocumentUri = new Uri("http://www.myhost.com/myservice.svc/$metadata");

            this.model = new EdmModel();

            EdmEntityContainer defaultContainer = new EdmEntityContainer("TestModel", "DefaultContainer");
            this.model.AddElement(defaultContainer);

            this.cityType = new EdmEntityType("TestModel", "City");
            EdmStructuralProperty cityIdProperty = cityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            cityType.AddKeys(cityIdProperty);
            cityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            cityType.AddStructuralProperty("Size", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            this.model.AddElement(cityType);

            EdmComplexType complexType = new EdmComplexType("TestModel", "MyComplexType");
            this.model.AddElement(complexType);

            this.operationWithNoOverload = new EdmFunction("TestModel", "FunctionImportWithNoOverload", EdmCoreModel.Instance.GetInt32(true));
            this.operationImportWithNoOverload = defaultContainer.AddFunctionImport("FunctionImportWithNoOverload", operationWithNoOverload);
            this.model.AddElement(operationWithNoOverload);

            this.operationWithOverloadAnd0Param = new EdmFunction("TestModel", "FunctionImportWithOverload", EdmCoreModel.Instance.GetInt32(true));
            this.operationImportWithOverloadAnd0Param = defaultContainer.AddFunctionImport("FunctionImportWithOverload", operationWithOverloadAnd0Param);

            this.operationWithOverloadAnd1Param = new EdmFunction("TestModel", "FunctionImportWithOverload", EdmCoreModel.Instance.GetInt32(true));
            this.operationWithOverloadAnd1Param.AddParameter("p1", EdmCoreModel.Instance.GetInt32(false));
            this.model.AddElement(operationWithOverloadAnd1Param);
            this.operationImportWithOverloadAnd1Param = defaultContainer.AddFunctionImport("FunctionImportWithOverload", operationWithOverloadAnd1Param);
            
            this.operationWithOverloadAnd2Params = new EdmFunction("TestModel", "FunctionImportWithOverload", EdmCoreModel.Instance.GetInt32(true));
            var cityTypeReference = new EdmEntityTypeReference(this.cityType, isNullable: false);
            this.operationWithOverloadAnd2Params.AddParameter("p1", cityTypeReference);
            this.operationWithOverloadAnd2Params.AddParameter("p2", EdmCoreModel.Instance.GetString(true));
            this.model.AddElement(operationWithOverloadAnd2Params);
            this.operationImportWithOverloadAnd2Params = defaultContainer.AddFunctionImport("FunctionImportWithOverload", operationWithOverloadAnd2Params);

            this.operationWithOverloadAnd5Params = new EdmFunction("TestModel", "FunctionImportWithOverload", EdmCoreModel.Instance.GetInt32(true));
            this.operationWithOverloadAnd5Params.AddParameter("p1", new EdmCollectionTypeReference(new EdmCollectionType(cityTypeReference)));
            this.operationWithOverloadAnd5Params.AddParameter("p2", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false))));
            this.operationWithOverloadAnd5Params.AddParameter("p3", EdmCoreModel.Instance.GetString(isNullable: true));
            EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(complexType, isNullable: false);
            this.operationWithOverloadAnd5Params.AddParameter("p4", complexTypeReference);
            this.operationWithOverloadAnd5Params.AddParameter("p5", new EdmCollectionTypeReference(new EdmCollectionType(complexTypeReference)));
            this.model.AddElement(operationWithOverloadAnd5Params);
            this.operationImportWithOverloadAnd5Params = defaultContainer.AddFunctionImport("FunctionImportWithOverload", operationWithOverloadAnd5Params);
        }

        [Fact]
        public void IsMetadataReferencePropertyShouldReturnTrue()
        {
            Assert.True(ODataJsonUtils.IsMetadataReferenceProperty("#name"));
            Assert.True(ODataJsonUtils.IsMetadataReferenceProperty("foo#name"));
            Assert.True(ODataJsonUtils.IsMetadataReferenceProperty("http://www.example.com/foo#name"));
        }

        [Fact]
        public void IsMetadataReferencePropertyShouldReturnFalse()
        {
            Assert.False(ODataJsonUtils.IsMetadataReferenceProperty("name"));
            Assert.False(ODataJsonUtils.IsMetadataReferenceProperty("http://www.example.com/foo"));
        }

        [Fact]
        public void GetFullyQualifiedFunctionImportNameFromMetadataReferencePropertyNameShouldReturnSubstringAfterHashAndBeforeLeftParan()
        {
            TestGetFunctionName("#name", name => Assert.Equal("name", name), parameter => Assert.Null(parameter));
            TestGetFunctionName("#namespace.a.b.name", name => Assert.Equal("namespace.a.b.name", name), parameter => Assert.Null(parameter));
            TestGetFunctionName("#namespace.a.b.name()", name => Assert.Equal("namespace.a.b.name", name), parameter => Assert.Empty(parameter));
            TestGetFunctionName("#namespace.a.b.name(a.b,c.d)", name => Assert.Equal("namespace.a.b.name", name), parameter => Assert.Equal("a.b,c.d", parameter));
            TestGetFunctionName("#namespace.a.b.name(Edm.Duration)", name => Assert.Equal("namespace.a.b.name", name), parameter => Assert.Equal("Edm.Duration", parameter));
            TestGetFunctionName("#namespace.a.b.name(Edm.Duration", name => Assert.Equal("namespace.a.b.name", name), parameter => Assert.Equal("Edm.Duration", parameter));
            TestGetFunctionName("#namespace.a.b.name(((((", name => Assert.Equal("namespace.a.b.name", name), parameter => Assert.Empty(parameter));
        }

        private void TestGetFunctionName(string metadataPropertyName, Action<string> validateName, Action<string> validateParameters)
        {
            string firstParameterName;
            string name = ODataJsonUtils.GetFullyQualifiedOperationName(this.MetadataDocumentUri, metadataPropertyName, out firstParameterName);
            validateName(name);
            validateParameters(firstParameterName);
        }

        [Fact]
        public void GetUriFragmentFromMetadataReferencePropertyNameShouldReturnTheSubstringAfterHash()
        {
            Assert.Equal("name", ODataJsonUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentUri, "#name"));
            Assert.Equal("name", ODataJsonUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentUri, "http://example.com/foo#name"));
            Assert.Equal("#", ODataJsonUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentUri, "##"));
            Assert.Equal("function(Edm.Duration,Edm.Guid)", ODataJsonUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentUri, "#function(Edm.Duration,Edm.Guid)"));
        }

        [Fact]
        public void GetAbsoluteUriFromMetadataReferencePropertyNameShouldReturnAnAbsoluteUri()
        {
            Assert.Equal(new Uri(this.MetadataDocumentUri, "#name"), ODataJsonUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "#name"));
            Assert.Equal(new Uri("http://example.com/foo#name", UriKind.Absolute), ODataJsonUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "http://example.com/foo#name"));
            Assert.Equal(new Uri(this.MetadataDocumentUri, "#"), ODataJsonUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "#"));
            Assert.Equal(new Uri("http://example.com/foo#", UriKind.Absolute), ODataJsonUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "http://example.com/foo#"));
            Assert.Equal(new Uri(this.MetadataDocumentUri, "##"), ODataJsonUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "##"));
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithNoOverload()
        {
            Assert.Equal("TestModel.FunctionImportWithNoOverload", ODataJsonUtils.GetMetadataReferenceName(this.model, this.operationWithNoOverload));
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithOverloadAnd0Param()
        {
            Assert.Equal("TestModel.FunctionImportWithOverload()", ODataJsonUtils.GetMetadataReferenceName(this.model, this.operationWithOverloadAnd0Param));
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithOverloadAnd1Param()
        {
            Assert.Equal("TestModel.FunctionImportWithOverload(p1)", ODataJsonUtils.GetMetadataReferenceName(this.model, this.operationWithOverloadAnd1Param));
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithOverloadAnd2Params()
        {
            Assert.Equal("TestModel.FunctionImportWithOverload(p1,p2)", ODataJsonUtils.GetMetadataReferenceName(this.model, this.operationWithOverloadAnd2Params));
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithOverloadAnd5Params()
        {
            Assert.Equal("TestModel.FunctionImportWithOverload(p1,p2,p3,p4,p5)", ODataJsonUtils.GetMetadataReferenceName(this.model, this.operationWithOverloadAnd5Params));
        }

        [Fact]
        public void CreateODataOperationFromFunctionImportShouldCreateODataAction()
        {
            var action = new EdmAction("TestModel", "FunctionImport1", null /*returnType*/);
            bool isAction;
            ODataOperation operation = ODataJsonUtils.CreateODataOperation(new Uri("http://www.example.com/$metadata"), "#Foo", action, out isAction);
            Assert.True(isAction);
            Assert.Equal("http://www.example.com/$metadata#Foo", operation.Metadata.AbsoluteUri);
            Assert.Same(operation.GetType(), typeof(ODataAction));
        }

        [Fact]
        public void CreateODataOperationFromFunctionImportShouldCreateODataFunction()
        {
            var function = new EdmFunction("TestModel", "Default", EdmCoreModel.Instance.GetString( /*isNullable*/false));
            bool isAction;
            ODataOperation operation = ODataJsonUtils.CreateODataOperation(new Uri("http://www.example.com/$metadata"), "#Foo", function, out isAction);
            Assert.False(isAction);
            Assert.Equal("http://www.example.com/$metadata#Foo", operation.Metadata.AbsoluteUri);
            Assert.Same(operation.GetType(), typeof(ODataFunction));
        }
    }
}
