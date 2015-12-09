//---------------------------------------------------------------------
// <copyright file="ODataUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataUtilsTests
    {
        #region AppendDefaultHeaderValue

        [Fact]
        public void CreateInstanceAnnotationsFilterShouldTranslateFilterStringToFunc()
        {
            Func<string, bool> filter = ODataUtils.CreateAnnotationFilter("*,-ns.*");
            filter.Should().NotBeNull();
            filter("ns.name").Should().BeFalse();
            filter("foo.bar").Should().BeTrue();
        }

        [Fact]
        public void MediaTypeOfJsonShouldHaveDefaultValues()
        {
            this.TestMediaTypeOfJson(MimeConstants.MimeApplicationJson, MimeConstants.MimeMetadataParameterValueMinimal, MimeConstants.MimeParameterValueTrue, MimeConstants.MimeParameterValueFalse);
        }

        [Fact]
        public void CustomValueOfStreamingShouldOverrideDefaultValueInMediaTypeOfJson()
        {
            this.TestMediaTypeOfJson(
                MimeConstants.MimeApplicationJson + ";" + MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse,
                MimeConstants.MimeMetadataParameterValueMinimal,
                MimeConstants.MimeParameterValueFalse,
                MimeConstants.MimeParameterValueFalse);
        }

        [Fact]
        public void CustomValueOfIeee754CompatibleShouldOverrideDefaultValueInMediaTypeOfJson()
        {
            this.TestMediaTypeOfJson(
                MimeConstants.MimeApplicationJson + ";" + MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue,
                MimeConstants.MimeMetadataParameterValueMinimal,
                MimeConstants.MimeParameterValueTrue,
                MimeConstants.MimeParameterValueTrue);
        }

        [Fact]
        public void CustomValueOfMetadataShouldOverrideDefaultValueInMediaTypeOfJson()
        {
            this.TestMediaTypeOfJson(
                MimeConstants.MimeApplicationJson + ";" + MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueFull,
                MimeConstants.MimeMetadataParameterValueFull,
                MimeConstants.MimeParameterValueTrue,
                MimeConstants.MimeParameterValueFalse);
        }

        [Fact]
        public void CustomValuesShouldOverrideDefaultValuesInMediaTypeOfJson()
        {
            this.TestMediaTypeOfJson(
                MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse,
                MimeConstants.MimeMetadataParameterValueNone,
                MimeConstants.MimeParameterValueFalse,
                MimeConstants.MimeParameterValueTrue);
        }

        [Fact]
        public void MediaTypeOfNonJsonShouldNotHaveDefaultValues()
        {
            ODataMediaType mediaTypeObject = GetAppendedMediaTypeObject(MimeConstants.MimeApplicationAtomXml);

            Assert.False(mediaTypeObject.ToText().Contains(MimeConstants.MimeStreamingParameterName));
            Assert.False(mediaTypeObject.ToText().Contains(MimeConstants.MimeStreamingParameterName));
            Assert.False(mediaTypeObject.ToText().Contains(MimeConstants.MimeIeee754CompatibleParameterName));
        }

        [Fact]
        public void NonMediaTypeShouldNotHaveDefaultValues()
        {
            const string headerName = "Some-Header";
            const string headerValue = MimeConstants.MimeApplicationJson;
            Assert.Equal(headerValue, ODataUtils.AppendDefaultHeaderValue(headerName, headerValue));
        }

        [Fact]
        public void CharsetShouldNotBeIgnored_UTF8()
        {
            const string headerValue = MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse + ";charset=utf-8";

            var appendedHeaderValue = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, headerValue);

            Assert.True(appendedHeaderValue.Contains("charset=utf-8"));
        }

        [Fact]
        public void CharsetShouldNotBeIgnored_ISO_8859_1()
        {
            const string headerValue = MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse + ";charset=ISO_8859-1";

            var appendedHeaderValue = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, headerValue);

            Assert.True(appendedHeaderValue.Contains("charset=iso-8859-1"));
        }

        [Fact]
        public void InvalidCharsetShouldBeIgnored()
        {
            const string headerValue = MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse + ";charset=utf-9";

            var appendedHeaderValue = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, headerValue);

            Assert.False(appendedHeaderValue.Contains("charset"));
        }

        private void TestMediaTypeOfJson(string mediaType, string metadata, string streaming, string ieee754Compatible)
        {
            ODataMediaType mediaTypeObject = GetAppendedMediaTypeObject(mediaType);

            Assert.True(mediaTypeObject.MediaTypeHasParameterWithValue(MimeConstants.MimeMetadataParameterName, metadata));
            Assert.True(mediaTypeObject.MediaTypeHasParameterWithValue(MimeConstants.MimeStreamingParameterName, streaming));
            Assert.True(mediaTypeObject.MediaTypeHasParameterWithValue(MimeConstants.MimeIeee754CompatibleParameterName, ieee754Compatible));
        }

        private ODataMediaType GetAppendedMediaTypeObject(string mediaType)
        {
            var appendedMediaType = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, mediaType);

            var mediaTypeList = HttpUtils.MediaTypesFromString(appendedMediaType);
            Assert.True(mediaTypeList.Count == 1);

            return mediaTypeList[0].Key;
        }

        #endregion

        #region GenerateServiceDocument

        [Fact]
        public void TestGenerateServiceDocument()
        {
            var serviceDocument = this.CreateTestModel().GenerateServiceDocument();
            var entitySets = serviceDocument.EntitySets.ToList();
            Assert.Equal(entitySets.Count, 2);
            Assert.Equal(entitySets[0].Name, "Products");
            Assert.Equal(entitySets[0].Url.ToString(), "Products");
            Assert.Equal(entitySets[1].Name, "Customers");
            Assert.Equal(entitySets[1].Url.ToString(), "Customers");

            var singletons = serviceDocument.Singletons.ToList();
            Assert.Equal(singletons.Count, 2);
            Assert.Equal(singletons[0].Name, "SingleProduct");
            Assert.Equal(singletons[0].Url.ToString(), "SingleProduct");
            Assert.Equal(singletons[1].Name, "SingleCustomer");
            Assert.Equal(singletons[1].Url.ToString(), "SingleCustomer");

            var functionImports = serviceDocument.FunctionImports.ToList();
            Assert.Equal(functionImports.Count, 1);
            Assert.Equal(functionImports[0].Name, "SimpleFunctionImport2");
            Assert.Equal(functionImports[0].Url.ToString(), "SimpleFunctionImport2");
        }

        [Fact]
        public void TestServiceDocumentWhenNoContainer()
        {
            Action action = () => (new EdmModel()).GenerateServiceDocument();
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

        #endregion
    }
}
