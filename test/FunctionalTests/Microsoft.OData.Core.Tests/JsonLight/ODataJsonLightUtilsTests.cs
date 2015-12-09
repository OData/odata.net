//---------------------------------------------------------------------
// <copyright file="ODataJsonLightUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightUtilsTests
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

        public ODataJsonLightUtilsTests()
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
            ODataJsonLightUtils.IsMetadataReferenceProperty("#name").Should().BeTrue();
            ODataJsonLightUtils.IsMetadataReferenceProperty("foo#name").Should().BeTrue();
            ODataJsonLightUtils.IsMetadataReferenceProperty("http://www.example.com/foo#name").Should().BeTrue();
        }

        [Fact]
        public void IsMetadataReferencePropertyShouldReturnFalse()
        {
            ODataJsonLightUtils.IsMetadataReferenceProperty("name").Should().BeFalse();
            ODataJsonLightUtils.IsMetadataReferenceProperty("http://www.example.com/foo").Should().BeFalse();
        }

        [Fact]
        public void GetFullyQualifiedFunctionImportNameFromMetadataReferencePropertyNameShouldReturnSubstringAfterHashAndBeforeLeftParan()
        {
            TestGetFunctionName("#name", name => name.Should().Be("name"), parameter => parameter.Should().BeNull());
            TestGetFunctionName("#namespace.a.b.name", name => name.Should().Be("namespace.a.b.name"), parameter => parameter.Should().BeNull());
            TestGetFunctionName("#namespace.a.b.name()", name => name.Should().Be("namespace.a.b.name"), parameter => parameter.Should().BeEmpty());
            TestGetFunctionName("#namespace.a.b.name(a.b,c.d)", name => name.Should().Be("namespace.a.b.name"), parameter => parameter.Should().Be("a.b,c.d"));
            TestGetFunctionName("#namespace.a.b.name(Edm.Duration)", name => name.Should().Be("namespace.a.b.name"), parameter => parameter.Should().Be("Edm.Duration"));
            TestGetFunctionName("#namespace.a.b.name(Edm.Duration", name => name.Should().Be("namespace.a.b.name"), parameter => parameter.Should().Be("Edm.Duration"));
            TestGetFunctionName("#namespace.a.b.name(((((", name => name.Should().Be("namespace.a.b.name"), parameter => parameter.Should().BeEmpty());
        }

        private void TestGetFunctionName(string metadataPropertyName, Action<string> validateName, Action<string> validateParameters)
        {
            string firstParameterName;
            string name = ODataJsonLightUtils.GetFullyQualifiedOperationName(this.MetadataDocumentUri, metadataPropertyName, out firstParameterName);
            validateName(name);
            validateParameters(firstParameterName);
        }

        [Fact]
        public void GetUriFragmentFromMetadataReferencePropertyNameShouldReturnTheSubstringAfterHash()
        {
            ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentUri, "#name").Should().Be("name");
            ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentUri, "http://example.com/foo#name").Should().Be("name");
            ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentUri, "##").Should().Be("#");
            ODataJsonLightUtils.GetUriFragmentFromMetadataReferencePropertyName(this.MetadataDocumentUri, "#function(Edm.Duration,Edm.Guid)").Should().Be("function(Edm.Duration,Edm.Guid)");
        }

        [Fact]
        public void GetAbsoluteUriFromMetadataReferencePropertyNameShouldReturnAnAbsoluteUri()
        {
            ODataJsonLightUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "#name").Should().Be(new Uri(this.MetadataDocumentUri, "#name"));
            ODataJsonLightUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "http://example.com/foo#name").Should().Be(new Uri("http://example.com/foo#name", UriKind.Absolute));
            ODataJsonLightUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "#").Should().Be(new Uri(this.MetadataDocumentUri, "#"));
            ODataJsonLightUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "http://example.com/foo#").Should().Be(new Uri("http://example.com/foo#", UriKind.Absolute));
            ODataJsonLightUtils.GetAbsoluteUriFromMetadataReferencePropertyName(this.MetadataDocumentUri, "##").Should().Be(new Uri(this.MetadataDocumentUri, "##"));
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithNoOverload()
        {
            ODataJsonLightUtils.GetMetadataReferenceName(this.model, this.operationWithNoOverload).Should().Be("TestModel.FunctionImportWithNoOverload");
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithOverloadAnd0Param()
        {
            ODataJsonLightUtils.GetMetadataReferenceName(this.model, this.operationWithOverloadAnd0Param).Should().Be("TestModel.FunctionImportWithOverload()");
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithOverloadAnd1Param()
        {
            ODataJsonLightUtils.GetMetadataReferenceName(this.model, this.operationWithOverloadAnd1Param).Should().Be("TestModel.FunctionImportWithOverload(p1)");
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithOverloadAnd2Params()
        {
            ODataJsonLightUtils.GetMetadataReferenceName(this.model, this.operationWithOverloadAnd2Params).Should().Be("TestModel.FunctionImportWithOverload(p1,p2)");
        }

        [Fact]
        public void GetMetadataReferenceNameFromFunctionImportShouldReturnCorrectNameForFunctionImportWithOverloadAnd5Params()
        {
            ODataJsonLightUtils.GetMetadataReferenceName(this.model, this.operationWithOverloadAnd5Params).Should().Be("TestModel.FunctionImportWithOverload(p1,p2,p3,p4,p5)");
        }

        [Fact]
        public void CreateODataOperationFromFunctionImportShouldCreateODataAction()
        {
            var action = new EdmAction("TestModel", "FunctionImport1", null /*returnType*/);
            bool isAction;
            ODataOperation operation = ODataJsonLightUtils.CreateODataOperation(new Uri("http://www.example.com/$metadata"), "#Foo", action, out isAction);
            Assert.True(isAction);
            Assert.Equal("http://www.example.com/$metadata#Foo", operation.Metadata.AbsoluteUri);
            Assert.Same(operation.GetType(), typeof(ODataAction));
        }

        [Fact]
        public void CreateODataOperationFromFunctionImportShouldCreateODataFunction()
        {
            var function = new EdmFunction("TestModel", "Default", EdmCoreModel.Instance.GetString( /*isNullable*/false));
            bool isAction;
            ODataOperation operation = ODataJsonLightUtils.CreateODataOperation(new Uri("http://www.example.com/$metadata"), "#Foo", function, out isAction);
            Assert.False(isAction);
            Assert.Equal("http://www.example.com/$metadata#Foo", operation.Metadata.AbsoluteUri);
            Assert.Same(operation.GetType(), typeof(ODataFunction));
        }
    }
}
