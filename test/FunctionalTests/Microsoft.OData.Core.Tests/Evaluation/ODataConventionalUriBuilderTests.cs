//---------------------------------------------------------------------
// <copyright file="ODataConventionalUriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Evaluation;
using Xunit;

namespace Microsoft.OData.Tests.Evaluation
{
    public class ODataConventionalUriBuilderTests : ODataUriBuilderTestsBase
    {
        private Uri defaultBaseUri;
        private ODataConventionalUriBuilder builder;
        private IEdmStructuredValue defaultProductInstance;
        private IEdmStructuredValue defaultMultipleKeyInstance;
        private IEdmModel model;
        private Dictionary<string, IEdmValue> idPropertyList;
        private EdmEntityType typeWithStringKey;

        public ODataConventionalUriBuilderTests()
        {
            this.defaultBaseUri = new Uri("http://odata.org/base/");
            this.builder = new ODataConventionalUriBuilder(this.defaultBaseUri, ODataUrlKeyDelimiter.Parentheses);

            this.model = TestModel.BuildDefaultTestModel();
            this.defaultProductInstance = TestModel.BuildDefaultProductValue(TestModel.GetEntityType(this.model, "TestModel.Product"));
            this.defaultMultipleKeyInstance = TestModel.BuildDefaultMultipleKeyValue(this.model);

            this.idPropertyList = new Dictionary<string, IEdmValue>()
                {
                    { "Id", new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(false), 42) }
                };

            this.typeWithStringKey = new EdmEntityType("Test.Model", "StringKey");
            this.typeWithStringKey.AddKeys(this.typeWithStringKey.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String));
        }

        [Fact]
        public void BuildEntitySetUriShouldValidateArguments()
        {
            base.BuildEntitySetUriShouldValidateArgumentsImpl(this.builder);
        }

        [Fact]
        public void BuildStreamEditLinkUriShouldValidateArguments()
        {
            BuildStreamEditLinkUriShouldValidateArgumentsImpl(this.builder);
        }

        [Fact]
        public void BuildStreamReadLinkUriShouldValidateArguments()
        {
            BuildStreamReadLinkUriShouldValidateArgumentsImpl(this.builder);
        }

        [Fact]
        public void BuildNavigationLinkUriShouldValidateArguments()
        {
            BuildNavigationLinkUriShouldValidateArgumentsImpl(this.builder);
        }

        [Fact]
        public void BuildAssociationLinkUriShouldValidateArguments()
        {
            BuildAssociationLinkUriShouldValidateArgumentsImpl(this.builder);
        }

        [Fact]
        public void BuildOperationTargetUriShouldValidateArguments()
        {
            BuildOperationTargetUriShouldValidateArgumentsImpl(this.builder);
        }

        [Fact]
        public void BuildEntitySetShouldWorkWithBaseUriWithTrailingSlash()
        {
            Assert.Equal(this.builder.BuildEntitySetUri(new Uri("http://odata.org/base/"), "EntitySet"), new Uri("http://odata.org/base/EntitySet"));
        }

        [Fact]
        public void BuildEntitySetShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            Assert.Equal(this.builder.BuildEntitySetUri(new Uri("http://odata.org/base"), "EntitySet"), new Uri("http://odata.org/base/EntitySet"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldWorkWithSingleKey()
        {
            Assert.Equal(this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/Products"), new Collection<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Id", 42) }, this.defaultProductInstance.Type.FullName()),
                new Uri("http://odata.org/base/Products(42)"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldEscapeKeyValue()
        {
            Assert.EndsWith("('%25')", this.GetEntityInstanceUriForStringKey("%"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldEscapeKeyValueEvenIfAlreadyEscaped()
        {
            // regression coverage for: [XTable blocker] ODataLib does not escape literal values in ID/Edit link, which causes issues with key values like '%25' versus '%'.
            Assert.EndsWith("('%2525')", this.GetEntityInstanceUriForStringKey("%25"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldWorkWithMultipleKeys()
        {
            Assert.Equal(this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/MultipleKeys"), new Collection<KeyValuePair<string, object>> { new KeyValuePair<string, object>("KeyA", "keya"), new KeyValuePair<string, object>("KeyB", 1) }, this.defaultMultipleKeyInstance.Type.FullName()),
                new Uri("http://odata.org/base/MultipleKeys(KeyA='keya',KeyB=1)"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldFailOnTypeWithNoKeyProperties()
        {
            Action action = () => this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/MultipleKeys"), new Collection<KeyValuePair<string, object>>(), "TestModel.EntityTypeWithNoKeys");
            action.Throws<ODataException>(Strings.ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties("TestModel.EntityTypeWithNoKeys"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldFailWithNullKeyValueKind()
        {
            Action action = () => this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/Products"), new Collection<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Id", null) }, "TestModel.Product");
            action.Throws<ODataException>(Strings.ODataConventionalUriBuilder_NullKeyValue("Id", "TestModel.Product"));
        }

        [Fact]
        public void BuildStreamEditLinkUriForDefaultStreamShouldWorkWithBaseUriWithTrailingSlash()
        {
            Assert.Equal(this.builder.BuildStreamEditLinkUri(new Uri("http://odata.org/base/Products/"), null),
                new Uri("http://odata.org/base/Products/$value"));
        }

        [Fact]
        public void BuildStreamEditLinkUriForDefaultStreamShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            Assert.Equal(this.builder.BuildStreamEditLinkUri(new Uri("http://odata.org/base/Products"), null),
                new Uri("http://odata.org/base/Products/$value"));
        }

        [Fact]
        public void BuildStreamEditLinkUriForStreamPropertyShouldWorkWithBaseUriWithTrailingSlash()
        {
            Assert.Equal(this.builder.BuildStreamEditLinkUri(new Uri("http://odata.org/base/Products/"), "StreamProperty"),
                new Uri("http://odata.org/base/Products/StreamProperty"));
        }

        [Fact]
        public void BuildStreamEditLinkUriForStreamPropertyShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            Assert.Equal(this.builder.BuildStreamEditLinkUri(new Uri("http://odata.org/base/Products"), "StreamProperty"),
                new Uri("http://odata.org/base/Products/StreamProperty"));
        }

        [Fact]
        public void BuildStreamReadLinkUriForStreamPropertyShouldWorkWithBaseUriWithTrailingSlash()
        {
            Assert.Equal(this.builder.BuildStreamReadLinkUri(new Uri("http://odata.org/base/Products/"), "StreamProperty"),
                new Uri("http://odata.org/base/Products/StreamProperty"));
        }

        [Fact]
        public void BuildStreamReadLinkUriForStreamPropertyShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            Assert.Equal(this.builder.BuildStreamReadLinkUri(new Uri("http://odata.org/base/Products"), "StreamProperty"),
                new Uri("http://odata.org/base/Products/StreamProperty"));
        }

        [Fact]
        public void BuildStreamReadLinkUriForDefaultStreanShouldWorkWithBaseUriWithTrailingSlash()
        {
            Assert.Equal(this.builder.BuildStreamReadLinkUri(new Uri("http://odata.org/base/Products/"), null),
                new Uri("http://odata.org/base/Products/$value"));
        }

        [Fact]
        public void BuildStreamReadLinkUriForDefaultStreamShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            Assert.Equal(this.builder.BuildStreamReadLinkUri(new Uri("http://odata.org/base/Products"), null),
                new Uri("http://odata.org/base/Products/$value"));
        }

        [Fact]
        public void BuildNavigationLinkUriShouldWorkWithBaseUriWithTrailingSlash()
        {
            Assert.Equal(this.builder.BuildNavigationLinkUri(new Uri("http://odata.org/base/Products/"), "NavigationProperty"),
                new Uri("http://odata.org/base/Products/NavigationProperty"));
        }

        [Fact]
        public void BuildNavigationLinkUriShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            Assert.Equal(this.builder.BuildNavigationLinkUri(new Uri("http://odata.org/base/Products(1)"), "NavigationProperty"),
                new Uri("http://odata.org/base/Products(1)/NavigationProperty"));
        }

        [Fact]
        public void BuildAssociationLinkUriShouldWorkWithBaseUriWithTrailingSlash()
        {
            Assert.Equal(this.builder.BuildAssociationLinkUri(new Uri("http://odata.org/base/Products/"), "NavigationProperty"),
                new Uri("http://odata.org/base/Products/NavigationProperty/$ref"));
        }

        [Fact]
        public void BuildAssociationLinkUriShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            Assert.Equal(this.builder.BuildAssociationLinkUri(new Uri("http://odata.org/base/Products(1)"), "NavigationProperty"),
                new Uri("http://odata.org/base/Products(1)/NavigationProperty/$ref"));
        }

        [Fact]
        public void BuildOperationTargetUriShouldWorkWithBaseUriWithTrailingSlash()
        {
            Assert.Equal(this.builder.BuildOperationTargetUri(new Uri("http://odata.org/base/Products/"), "OperationName", null, null),
                new Uri("http://odata.org/base/Products/OperationName"));
        }

        [Fact]
        public void BuildOperationTargetUriShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            Assert.Equal(this.builder.BuildOperationTargetUri(new Uri("http://odata.org/base/Products(1)"), "OperationName", null, null),
                new Uri("http://odata.org/base/Products(1)/OperationName"));
        }

        [Fact]
        public void BuildOperationTargetUriShouldNotEscapeParameterTypeName()
        {
            Assert.Equal(this.builder.BuildOperationTargetUri(new Uri("http://base.org/"), "op", null, " +&?"),
                new Uri("http://base.org/op( +&?=@ +&?)"));
        }

        [Fact]
        public void BuildEntitySetUriShouldEscapeEntitySetName()
        {
            this.RunCharacterEscapingBuilderTest((builder, baseUri, name) => builder.BuildEntitySetUri(baseUri, name));
        }

        [Fact]
        public void BuildStreamEditLinkUriShouldEscapeStreamPropertyName()
        {
            this.RunCharacterEscapingBuilderTest((builder, baseUri, name) => builder.BuildStreamEditLinkUri(baseUri, name));
        }

        [Fact]
        public void BuildStreamReadLinkUriShouldEscapeStreamPropertyName()
        {
            this.RunCharacterEscapingBuilderTest((builder, baseUri, name) => builder.BuildStreamReadLinkUri(baseUri, name));
        }

        [Fact]
        public void BuildNavigationLinkUriShouldEscapeNavigationPropertyName()
        {
            this.RunCharacterEscapingBuilderTest((builder, baseUri, name) => builder.BuildNavigationLinkUri(baseUri, name));
        }

        [Fact]
        public void BuildAssociationLinkUriShouldEscapeNavigationPropertyName()
        {
            string entitySetName = "$注文#";
            string expectedUri = this.defaultBaseUri + Uri.EscapeDataString(entitySetName) + "/$ref";
            Assert.Equal(this.builder.BuildAssociationLinkUri(this.defaultBaseUri, entitySetName).OriginalString, expectedUri);
        }

        [Fact]
        public void BuildOperationTargetUriShouldNotEscapeOperationName()
        {
            this.RunCharacterNonEscapingBuilderTest((builder, baseUri, name) => builder.BuildOperationTargetUri(baseUri, name, null, null));
        }

        [Fact]
        public void AppendTypeSegmentShouldEscapeTypeName()
        {
            this.RunCharacterEscapingBuilderTest((builder, baseUri, name) => builder.AppendTypeSegment(baseUri, name));
        }

        private void RunCharacterEscapingBuilderTest(Func<ODataConventionalUriBuilder, Uri, string, Uri> buildUri)
        {
            string entitySetName = "$注文#";
            string expectedUri = this.defaultBaseUri + Uri.EscapeDataString(entitySetName);
            Assert.Equal(buildUri(this.builder, this.defaultBaseUri, entitySetName).OriginalString, expectedUri);
        }

        private void RunCharacterNonEscapingBuilderTest(Func<ODataConventionalUriBuilder, Uri, string, Uri> buildUri)
        {
            string entitySetName = "$注文#";
            string expectedUri = this.defaultBaseUri + entitySetName;
            Assert.Equal(buildUri(this.builder, this.defaultBaseUri, entitySetName).OriginalString, expectedUri);
        }

        private string GetEntityInstanceUriForStringKey(string keyValue)
        {
            var instance = new EdmStructuredValueSimulator(this.typeWithStringKey, new[] { new KeyValuePair<string, IEdmValue>("Id", new EdmStringConstant(EdmCoreModel.Instance.GetString(true), keyValue)) });
            Uri entityInstanceUri = this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/Products"), new Collection<KeyValuePair<string, object>> { new KeyValuePair<string, object>("Id", keyValue) }, instance.Type.FullName());
            return entityInstanceUri.OriginalString;
        }

        private sealed class EdmStringNullValue : IEdmStringValue
        {
            public string Value
            {
                get { return null; }
            }

            public IEdmTypeReference Type
            {
                get { return EdmCoreModel.Instance.GetString(true); }
            }

            public EdmValueKind ValueKind
            {
                get { return EdmValueKind.String; }
            }
        }
    }
}
