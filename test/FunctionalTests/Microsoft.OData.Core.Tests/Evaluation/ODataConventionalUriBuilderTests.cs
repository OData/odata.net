//---------------------------------------------------------------------
// <copyright file="ODataConventionalUriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.Evaluation
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
            this.builder = new ODataConventionalUriBuilder(this.defaultBaseUri, UrlConvention.CreateWithExplicitValue(false));

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
            base.BuildEntitySetUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildStreamEditLinkUriShouldValidateArguments()
        {
            BuildStreamEditLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildStreamReadLinkUriShouldValidateArguments()
        {
            BuildStreamReadLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildNavigationLinkUriShouldValidateArguments()
        {
            BuildNavigationLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildAssociationLinkUriShouldValidateArguments()
        {
            BuildAssociationLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildOperationTargetUriShouldValidateArguments()
        {
            BuildOperationTargetUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildEntitySetShouldWorkWithBaseUriWithTrailingSlash()
        {
            this.builder.BuildEntitySetUri(new Uri("http://odata.org/base/"), "EntitySet").Should().Be(new Uri("http://odata.org/base/EntitySet"));
        }

        [Fact]
        public void BuildEntitySetShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            this.builder.BuildEntitySetUri(new Uri("http://odata.org/base"), "EntitySet").Should().Be(new Uri("http://odata.org/base/EntitySet"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldWorkWithSingleKey()
        {
            this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/Products"), new Collection<KeyValuePair<string, object>> {new KeyValuePair<string, object>("Id", 42)}, this.defaultProductInstance.Type.FullName())
                .Should().Be(new Uri("http://odata.org/base/Products(42)"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldEscapeKeyValue()
        {
            this.GetEntityInstanceUriForStringKey("%").Should().EndWith("('%25')");
        }

        [Fact]
        public void BuildEntityInstanceUriShouldEscapeKeyValueEvenIfAlreadyEscaped()
        {
            // regression coverage for: [XTable blocker] ODataLib does not escape literal values in ID/Edit link, which causes issues with key values like '%25' versus '%'.
            this.GetEntityInstanceUriForStringKey("%25").Should().EndWith("('%2525')");
        }

        [Fact]
        public void BuildEntityInstanceUriShouldWorkWithMultipleKeys()
        {
            this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/MultipleKeys"), new Collection<KeyValuePair<string, object>> {new KeyValuePair<string, object>("KeyA", "keya"), new KeyValuePair<string, object>("KeyB", 1)}, this.defaultMultipleKeyInstance.Type.FullName())
                .Should().Be(new Uri("http://odata.org/base/MultipleKeys(KeyA='keya',KeyB=1)"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldFailOnTypeWithNoKeyProperties()
        {
            Action action = () => this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/MultipleKeys"), new Collection<KeyValuePair<string, object>>(), "TestModel.EntityTypeWithNoKeys");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataConventionalUriBuilder_EntityTypeWithNoKeyProperties("TestModel.EntityTypeWithNoKeys"));
        }

        [Fact]
        public void BuildEntityInstanceUriShouldFailWithNullKeyValueKind()
        {
            Action action = () => this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/Products"), new Collection<KeyValuePair<string, object>>{new KeyValuePair<string, object>("Id", null)}, "TestModel.Product");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataConventionalUriBuilder_NullKeyValue("Id", "TestModel.Product"));
        }

        [Fact]
        public void BuildStreamEditLinkUriForDefaultStreamShouldWorkWithBaseUriWithTrailingSlash()
        {
            this.builder.BuildStreamEditLinkUri(new Uri("http://odata.org/base/Products/"), null)
                .Should().Be(new Uri("http://odata.org/base/Products/$value"));
        }

        [Fact]
        public void BuildStreamEditLinkUriForDefaultStreamShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            this.builder.BuildStreamEditLinkUri(new Uri("http://odata.org/base/Products"), null)
                .Should().Be(new Uri("http://odata.org/base/Products/$value"));
        }

        [Fact]
        public void BuildStreamEditLinkUriForStreamPropertyShouldWorkWithBaseUriWithTrailingSlash()
        {
            this.builder.BuildStreamEditLinkUri(new Uri("http://odata.org/base/Products/"), "StreamProperty")
                .Should().Be(new Uri("http://odata.org/base/Products/StreamProperty"));
        }

        [Fact]
        public void BuildStreamEditLinkUriForStreamPropertyShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            this.builder.BuildStreamEditLinkUri(new Uri("http://odata.org/base/Products"), "StreamProperty")
                .Should().Be(new Uri("http://odata.org/base/Products/StreamProperty"));
        }

        [Fact]
        public void BuildStreamReadLinkUriForStreamPropertyShouldWorkWithBaseUriWithTrailingSlash()
        {
            this.builder.BuildStreamReadLinkUri(new Uri("http://odata.org/base/Products/"), "StreamProperty")
                .Should().Be(new Uri("http://odata.org/base/Products/StreamProperty"));
        }

        [Fact]
        public void BuildStreamReadLinkUriForStreamPropertyShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            this.builder.BuildStreamReadLinkUri(new Uri("http://odata.org/base/Products"), "StreamProperty")
                .Should().Be(new Uri("http://odata.org/base/Products/StreamProperty"));
        }

        [Fact]
        public void BuildStreamReadLinkUriForDefaultStreanShouldWorkWithBaseUriWithTrailingSlash()
        {
            this.builder.BuildStreamReadLinkUri(new Uri("http://odata.org/base/Products/"), null)
                .Should().Be(new Uri("http://odata.org/base/Products/$value"));
        }

        [Fact]
        public void BuildStreamReadLinkUriForDefaultStreamShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            this.builder.BuildStreamReadLinkUri(new Uri("http://odata.org/base/Products"), null)
                .Should().Be(new Uri("http://odata.org/base/Products/$value"));
        }

        [Fact]
        public void BuildNavigationLinkUriShouldWorkWithBaseUriWithTrailingSlash()
        {
            this.builder.BuildNavigationLinkUri(new Uri("http://odata.org/base/Products/"), "NavigationProperty")
                .Should().Be(new Uri("http://odata.org/base/Products/NavigationProperty"));
        }

        [Fact]
        public void BuildNavigationLinkUriShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            this.builder.BuildNavigationLinkUri(new Uri("http://odata.org/base/Products(1)"), "NavigationProperty")
                .Should().Be(new Uri("http://odata.org/base/Products(1)/NavigationProperty"));
        }

        [Fact]
        public void BuildAssociationLinkUriShouldWorkWithBaseUriWithTrailingSlash()
        {
            this.builder.BuildAssociationLinkUri(new Uri("http://odata.org/base/Products/"), "NavigationProperty")
                .Should().Be(new Uri("http://odata.org/base/Products/NavigationProperty/$ref"));
        }

        [Fact]
        public void BuildAssociationLinkUriShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            this.builder.BuildAssociationLinkUri(new Uri("http://odata.org/base/Products(1)"), "NavigationProperty")
                .Should().Be(new Uri("http://odata.org/base/Products(1)/NavigationProperty/$ref"));
        }

        [Fact]
        public void BuildOperationTargetUriShouldWorkWithBaseUriWithTrailingSlash()
        {
            this.builder.BuildOperationTargetUri(new Uri("http://odata.org/base/Products/"), "OperationName", null, null)
                .Should().Be(new Uri("http://odata.org/base/Products/OperationName"));
        }

        [Fact]
        public void BuildOperationTargetUriShouldWorkWithBaseUriWithoutTrailingSlash()
        {
            this.builder.BuildOperationTargetUri(new Uri("http://odata.org/base/Products(1)"), "OperationName", null, null)
                .Should().Be(new Uri("http://odata.org/base/Products(1)/OperationName"));
        }

        [Fact]
        public void BuildOperationTargetUriShouldNotEscapeParameterTypeName()
        {
            this.builder.BuildOperationTargetUri(new Uri("http://base.org/"), "op", null, " +&?")
                .Should().Be(new Uri("http://base.org/op( +&?=@ +&?)"));
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
            this.builder.BuildAssociationLinkUri(this.defaultBaseUri, entitySetName).OriginalString.Should().Be(expectedUri);
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
            buildUri(this.builder, this.defaultBaseUri, entitySetName).OriginalString.Should().Be(expectedUri);
        }

        private void RunCharacterNonEscapingBuilderTest(Func<ODataConventionalUriBuilder, Uri, string, Uri> buildUri)
        {
            string entitySetName = "$注文#";
            string expectedUri = this.defaultBaseUri + entitySetName;
            buildUri(this.builder, this.defaultBaseUri, entitySetName).OriginalString.Should().Be(expectedUri);
        }

        private string GetEntityInstanceUriForStringKey(string keyValue)
        {
            var instance = new EdmStructuredValueSimulator(this.typeWithStringKey, new[] { new KeyValuePair<string, IEdmValue>("Id", new EdmStringConstant(EdmCoreModel.Instance.GetString(true), keyValue)) });
            Uri entityInstanceUri = this.builder.BuildEntityInstanceUri(new Uri("http://odata.org/base/Products"), new Collection<KeyValuePair<string, object>>{new KeyValuePair<string, object>("Id", keyValue)}, instance.Type.FullName());
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
