//---------------------------------------------------------------------
// <copyright file="ODataUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataUtilsTests
    {
        #region AppendDefaultHeaderValue

        [Fact]
        public void CreateInstanceAnnotationsFilterShouldTranslateFilterStringToFunc()
        {
            Func<string, bool> filter = ODataUtils.CreateAnnotationFilter("*,-ns.*");
            Assert.NotNull(filter);
            Assert.False(filter("ns.name"));
            Assert.True(filter("foo.bar"));
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

            string test = mediaTypeObject.ToText();
            Assert.DoesNotContain(MimeConstants.MimeStreamingParameterName, test);
            Assert.DoesNotContain(MimeConstants.MimeIeee754CompatibleParameterName, test);
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

            Assert.Contains("charset=utf-8", appendedHeaderValue);
        }

        [Fact]
        public void CharsetShouldNotBeIgnored_ISO_8859_1()
        {
            const string headerValue = MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse + ";charset=ISO_8859-1";

            var appendedHeaderValue = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, headerValue);

            Assert.Contains("charset=iso-8859-1", appendedHeaderValue);
        }

        [Fact]
        public void InvalidCharsetShouldBeIgnored()
        {
            const string headerValue = MimeConstants.MimeApplicationJson + ";" +
                MimeConstants.MimeMetadataParameterName + "=" + MimeConstants.MimeMetadataParameterValueNone + ";" +
                MimeConstants.MimeIeee754CompatibleParameterName + "=" + MimeConstants.MimeParameterValueTrue + ";" +
                MimeConstants.MimeStreamingParameterName + "=" + MimeConstants.MimeParameterValueFalse + ";charset=utf-9";

            var appendedHeaderValue = ODataUtils.AppendDefaultHeaderValue(ODataConstants.ContentTypeHeader, headerValue);

            Assert.DoesNotContain("charset", appendedHeaderValue);
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
            Assert.Equal(2, entitySets.Count);
            Assert.Equal("Products", entitySets[0].Name);
            Assert.Equal("Products", entitySets[0].Url.ToString());
            Assert.Equal("Customers", entitySets[1].Name);
            Assert.Equal("Customers", entitySets[1].Url.ToString());

            var singletons = serviceDocument.Singletons.ToList();
            Assert.Equal(2, singletons.Count);
            Assert.Equal("SingleProduct", singletons[0].Name);
            Assert.Equal("SingleProduct", singletons[0].Url.ToString());
            Assert.Equal("SingleCustomer", singletons[1].Name);
            Assert.Equal("SingleCustomer", singletons[1].Url.ToString());

            var functionImports = serviceDocument.FunctionImports.ToList();
            var functionImport = Assert.Single(functionImports);
            Assert.Equal("SimpleFunctionImport3", functionImport.Name);
            Assert.Equal("SimpleFunctionImport3", functionImport.Url.ToString());
        }

        [Fact]
        public void TestServiceDocumentWhenNoContainer()
        {
            Action action = () => (new EdmModel()).GenerateServiceDocument();
            action.Throws<ODataException>(Strings.ODataUtils_ModelDoesNotHaveContainer);
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
            defaultContainer.AddFunctionImport("SimpleFunctionImport2", function2, null, true /*IncludeInServiceDocument*/);

            // Parameterless unbound function.
            EdmFunction function3 = new EdmFunction("TestModel", "SimpleFunction3", EdmCoreModel.Instance.GetInt32(false), false /*isbound*/, null, true);
            defaultContainer.AddFunctionImport("SimpleFunctionImport3", function3, null, true /*IncludeInServiceDocument*/);

            return model;
        }

        #endregion

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_RelativeSecondUrlIsNotAllowed()
        {
            Action method = () => UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("/One/Two", UriKind.Relative));
            Assert.Throws<InvalidOperationException>(method);
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_RelativeFirstUrlIsNotAllowed()
        {
            Action method = () => UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("/One/", UriKind.Relative), new Uri("http://www.example.com/One/Two"));
            Assert.Throws<InvalidOperationException>(method);
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_SameBaseShouldReturnTrue()
        {
            var result = UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("http://www.example.com/One/Two"));
            Assert.True(result);
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_IdenticalUrisShouldReturnTrue()
        {
            var result = UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/One/"), new Uri("http://www.example.com/One/"));
            Assert.True(result);
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_ShouldBeCaseInsensitive()
        {
            var result = UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("HTTP://WwW.ExAmPlE.cOm/OnE/"), new Uri("http://www.example.com/One/"));
            Assert.True(result);
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_ShouldIgnoreHostAndSchemeAndPort()
        {
            var result = UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("https://different.org:1234/One/"), new Uri("http://www.example.com:4567/One/Two"));
            Assert.True(result);
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_ShouldIgnoreStuffAfterFinalSlash()
        {
            var result = UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/OData.svc"), new Uri("http://www.example.com/One/Two"));
            Assert.True(result);
        }

        [Fact]
        public void UriInvariantInsensitiveIsBaseOf_DifferentBaseShouldReturnFalse()
        {
            var result = UriUtils.UriInvariantInsensitiveIsBaseOf(new Uri("http://www.example.com/OData.svc/"), new Uri("http://www.example.com/One/Two"));
            Assert.False(result);
        }
    }
}
