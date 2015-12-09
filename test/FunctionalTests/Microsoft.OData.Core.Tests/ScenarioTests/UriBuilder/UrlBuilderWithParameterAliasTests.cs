//---------------------------------------------------------------------
// <copyright file="UrlBuilderWithParameterAliasTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriBuilder;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriBuilder
{
    public class UrlBuilderWithParameterAliasTests
    {
        private readonly Uri serviceRoot = new Uri("http://gobbledygook/");
        private readonly ODataUriParserSettings settings = new ODataUriParserSettings();

        #region alias in path
        [Fact]
        public void BuildPath_AliasInFunctionImport()
        {
            Uri fullUri = new Uri("http://gobbledygook/GetPet4(id=@p1)?@p1=1.01M");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;

            odataUri.Path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).And.Parameters.First().ShouldHaveParameterAliasNode("id", "@p1", EdmCoreModel.Instance.GetDecimal(false));
            aliasNodes["@p1"].ShouldBeConstantQueryNode(1.01M);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildPath_AliasInFunctionImport_enum()
        {
            Uri fullUri = new Uri("http://gobbledygook/GetPetCount(colorPattern=@p1)?@p1=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped'");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();
            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;

            OperationSegmentParameter p = odataUri.Path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPetCount()).And.Parameters.First();
            p.Name.Should().Be("colorPattern");
            p.Value.As<ParameterAliasNode>().Alias.Should().Be("@p1");
            p.Value.As<ParameterAliasNode>().TypeReference.IsEnum().Should().Be(true);
            p.Value.As<ParameterAliasNode>().TypeReference.Definition.FullTypeName().ShouldBeEquivalentTo("Fully.Qualified.Namespace.ColorPattern");
            aliasNodes["@p1"].As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
            aliasNodes["@p1"].As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("22");
            aliasNodes["@p1"].TypeReference.IsEnum().Should().Be(true);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildPath_AliasInPath_AliasInFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/GetPet4(id=@p1)?$filter=ID%20eq%20%40p1&@p1=1.01M");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;

            odataUri.Path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).And.Parameters.First().ShouldHaveParameterAliasNode("id", "@p1", EdmCoreModel.Instance.GetDecimal(false));
            odataUri.Filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetDecimal(false));
            aliasNodes["@p1"].ShouldBeConstantQueryNode(1.01M);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildPath_AliasInFunctionImport_NullAsValue()
        {
            Uri fullUri = new Uri("http://gobbledygook/GetPet4(id=@p1)?@p1=null");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).And.Parameters.First().ShouldHaveParameterAliasNode("id", "@p1", null);
            aliasNodes["@p1"].ShouldBeConstantQueryNode((object)null);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildPath_AliasInBoundFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/People(123)/Fully.Qualified.Namespace.HasHat(onCat=@p1)?@p1=true");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.TestModel.FindOperations("Fully.Qualified.Namespace.HasHat").Single(s => (s as IEdmFunction).Parameters.Count() == 2)).As<IEdmFunction>();
            aliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.NotEqual(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }
        #endregion

        #region alias in filter
        [Fact]
        public void BuildFilter_AliasInFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.AllHaveDog(inOffice%3D%40p1)&@p1=true");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForAllHaveDogWithTwoParameters()).And.Parameters.Last().As<NamedFunctionParameterNode>().Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
            aliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInFunction_enum()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=null%20ne%20Fully.Qualified.Namespace.GetPetCount(colorPattern%3D%40p1)&@p1=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped'");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            NamedFunctionParameterNode p = odataUri.Filter.Expression.As<BinaryOperatorNode>().Right.As<SingleEntityFunctionCallNode>().Parameters.First().As<NamedFunctionParameterNode>();
            p.Value.As<ParameterAliasNode>().Alias.ShouldBeEquivalentTo("@p1");
            p.Value.As<ParameterAliasNode>().TypeReference.IsEnum().Should().Be(true);
            p.Value.As<ParameterAliasNode>().TypeReference.Definition.FullTypeName().ShouldBeEquivalentTo("Fully.Qualified.Namespace.ColorPattern");

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            aliasNodes["@p1"].As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
            aliasNodes["@p1"].As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("22");
            aliasNodes["@p1"].TypeReference.IsEnum().Should().Be(true);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInFunction_enum_undefined()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=null%20ne%20Fully.Qualified.Namespace.GetPetCount(colorPattern%3D%40p1)&@p1=Fully.Qualified.Namespace.ColorPattern'238563'");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            NamedFunctionParameterNode p = odataUri.Filter.Expression.As<BinaryOperatorNode>().Right.As<SingleEntityFunctionCallNode>().Parameters.First().As<NamedFunctionParameterNode>();
            p.Value.As<ParameterAliasNode>().Alias.ShouldBeEquivalentTo("@p1");
            p.Value.As<ParameterAliasNode>().TypeReference.IsEnum().Should().Be(true);
            p.Value.As<ParameterAliasNode>().TypeReference.Definition.FullTypeName().ShouldBeEquivalentTo("Fully.Qualified.Namespace.ColorPattern");

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            aliasNodes["@p1"].As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
            aliasNodes["@p1"].As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("238563");
            aliasNodes["@p1"].TypeReference.IsEnum().Should().Be(true);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInFunction_PropertyAsValue()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice%3Dfalse%2Cname%3D%40p1)&@p1=Name");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters().As<IEdmFunction>()).And.Parameters.Last().As<NamedFunctionParameterNode>().Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType().FindProperty("Name"));

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInFunction_Recursive_PropertyAsValue()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice%3Dfalse%2Cname%3D%40p1)&@p2=Name&@p1=%40p2");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters().As<IEdmFunction>()).And.Parameters.Last().As<NamedFunctionParameterNode>().Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p2"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType().FindProperty("Name"));

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInFunction_PropertyAsValue_TypeMismatch()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice%3D%40p1%2Cname%3D%40p1)&@p2=Name&@p1=%40p2");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters().As<IEdmFunction>()).And.Parameters.Last().As<NamedFunctionParameterNode>().Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p2"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType().FindProperty("Name"));

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInFunction_WithoutValue()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice%3D%40p1%2Cname%3D%40p2)");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters().As<IEdmFunction>()).And.Parameters.Last().As<NamedFunctionParameterNode>().Value.ShouldBeParameterAliasNode("@p2", null);
            aliasNodes["@p1"].Should().BeNull();
            aliasNodes["@p2"].Should().BeNull();

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInBinaryOp_ValueAsExpression()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=ID%20eq%20%40p1&@p1=1 add 2");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetInt32(false));
            aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add).And.Right.ShouldBeConstantQueryNode(2);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInFunction_BuiltIn()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=contains(%40p1%2CName)&@p1=Name");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Filter.Expression.As<SingleValueFunctionCallNode>().Parameters.First().ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType().FindProperty("Name"));

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }
        #endregion

        #region alias in orderby
        [Fact]
        public void BuildOrderby_AliasInFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$orderby=Fully.Qualified.Namespace.HasDog(inOffice%3D%40p1)&@p1=true");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            var expectedFunc = HardCodedTestModel.GetAllHasDogFunctionOverloadsForPeople().Single(s => s.Parameters.Count() == 2);
            odataUri.OrderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode(expectedFunc).And.Parameters.Last().As<NamedFunctionParameterNode>().Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
            aliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(fullUri, actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildOrderby_AliasInBinaryOp()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$orderby=ID%20mul%20%40p1%20asc&@p1=3 div 2");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.OrderBy.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Multiply).And.Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetInt32(false));
            aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Divide).And.Right.ShouldBeConstantQueryNode(2);

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(odataUriParser.UrlConventions, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=ID%20mul%20%40p1&@p1=3 div 2"), actualUri);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=ID%20mul%20%40p1&@p1=3 div 2"), actualUri);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=ID%20mul%20%40p1&@p1=3 div 2"), actualUri);
        }
        #endregion

        #region expand
        [Fact]
        public void BuildExpandOrderby_AliasInFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$expand=MyPet2Set($orderby=concat(Color, @p1 )asc )&@p1='abc'");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlConventions = ODataUrlConventions.Default;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            SingleValueFunctionCallNode node = (odataUri.SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem).OrderByOption.Expression as SingleValueFunctionCallNode;
            node.Parameters.Last().ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeConstantQueryNode("abc");

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(odataUriParser.UrlConventions, odataUri);
            Uri actualUri = odataUriBuilder.BuildUri();
            Assert.Equal("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyPet2Set($orderby=concat(Color,@p1))") + "&@p1=" + Uri.EscapeDataString("'abc'"), actualUri.OriginalString);

            ODataUriBuilder uriBuilderWithKeyAsSegment = new ODataUriBuilder(ODataUrlConventions.KeyAsSegment, odataUri);
            actualUri = uriBuilderWithKeyAsSegment.BuildUri();
            Assert.Equal("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyPet2Set($orderby=concat(Color,@p1))") + "&@p1=" + Uri.EscapeDataString("'abc'"), actualUri.OriginalString);

            ODataUriBuilder uriBuilderWithODataSimplified = new ODataUriBuilder(ODataUrlConventions.ODataSimplified, odataUri);
            actualUri = uriBuilderWithODataSimplified.BuildUri();
            Assert.Equal("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyPet2Set($orderby=concat(Color,@p1))") + "&@p1=" + Uri.EscapeDataString("'abc'"), actualUri.OriginalString);
        }
        #endregion

        #region set ODataUriParserSettings method
        private static void SetODataUriParserSettingsTo(ODataUriParserSettings sourceSettings, ODataUriParserSettings destSettings)
        {
            if (sourceSettings != null)
            {
                destSettings.MaximumExpansionCount = sourceSettings.MaximumExpansionCount;
                destSettings.MaximumExpansionDepth = sourceSettings.MaximumExpansionDepth;
            }
        }
        #endregion
    }
}
