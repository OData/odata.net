//---------------------------------------------------------------------
// <copyright file="UrlBuilderWithParameterAliasTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriBuilder
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
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;

            odataUri.Path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).Parameters.First().ShouldHaveParameterAliasNode("id", "@p1", EdmCoreModel.Instance.GetDecimal(false));
            aliasNodes["@p1"].ShouldBeConstantQueryNode(1.01M);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildPath_AliasInFunctionImport_enum()
        {
            Uri fullUri = new Uri("http://gobbledygook/GetPetCount(colorPattern=@p1)?@p1=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped'");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();
            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;

            OperationSegmentParameter p = odataUri.Path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPetCount()).Parameters.First();
            Assert.Equal("colorPattern", p.Name);
            var node = Assert.IsType<ParameterAliasNode>(p.Value);
            Assert.Equal("@p1", node.Alias);
            Assert.True(node.TypeReference.IsEnum());
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", node.TypeReference.Definition.FullTypeName());

            var enumValue = Assert.IsType<ODataEnumValue>(Assert.IsType<ConstantNode>(aliasNodes["@p1"]).Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
            Assert.Equal("22", enumValue.Value);
            Assert.True(aliasNodes["@p1"].TypeReference.IsEnum());

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildPath_AliasInPath_AliasInFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/GetPet4(id=@p1)?$filter=ID%20eq%20%40p1&@p1=1.01M");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;

            odataUri.Path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).Parameters.First().ShouldHaveParameterAliasNode("id", "@p1", EdmCoreModel.Instance.GetDecimal(false));
            odataUri.Filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetDecimal(false));
            aliasNodes["@p1"].ShouldBeConstantQueryNode(1.01M);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildPath_AliasInFunctionImport_NullAsValue()
        {
            Uri fullUri = new Uri("http://gobbledygook/GetPet4(id=@p1)?@p1=null");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).Parameters.First().ShouldHaveParameterAliasNode("id", "@p1", null);
            aliasNodes["@p1"].ShouldBeConstantQueryNode((object)null);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildPath_AliasInBoundFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/People(123)/Fully.Qualified.Namespace.HasHat(onCat=@p1)?@p1=true");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;

            var operation = HardCodedTestModel.TestModel.FindOperations("Fully.Qualified.Namespace.HasHat").Single(s => (s as IEdmFunction).Parameters.Count() == 2);
            odataUri.Path.LastSegment.ShouldBeOperationSegment(operation);
            aliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.NotEqual(fullUri, actualUri, new UriComparer<Uri>());
        }
        #endregion

        #region alias in filter
        [Fact]
        public void BuildFilter_AliasInFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.AllHaveDog(inOffice%3D%40p1)&@p1=true");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            var functionCallNode = odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForAllHaveDogWithTwoParameters());

            var queryNode = Assert.IsType<NamedFunctionParameterNode>(functionCallNode.Parameters.Last()).Value;
            queryNode.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));

            aliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildFilter_AliasInFunction_enum()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=null%20ne%20Fully.Qualified.Namespace.GetPetCount(colorPattern%3D%40p1)&@p1=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped'");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            var rightNode = Assert.IsType<BinaryOperatorNode>(odataUri.Filter.Expression).Right;
            NamedFunctionParameterNode p = Assert.IsType<NamedFunctionParameterNode>(Assert.IsType<SingleResourceFunctionCallNode>(rightNode).Parameters.First());
            ParameterAliasNode aliasNode = Assert.IsType<ParameterAliasNode>(p.Value);
            Assert.Equal("@p1", aliasNode.Alias);
            Assert.True(aliasNode.TypeReference.IsEnum());
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", aliasNode.TypeReference.Definition.FullTypeName());

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            var node = Assert.IsType<ConstantNode>(aliasNodes["@p1"]);
            var enumValue = Assert.IsType<ODataEnumValue>(node.Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
            Assert.Equal("22", enumValue.Value);
            Assert.True(aliasNodes["@p1"].TypeReference.IsEnum());

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildFilter_AliasInFunction_enum_undefined()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=null%20ne%20Fully.Qualified.Namespace.GetPetCount(colorPattern%3D%40p1)&@p1=Fully.Qualified.Namespace.ColorPattern'238563'");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            var rightNode = Assert.IsType<BinaryOperatorNode>(odataUri.Filter.Expression).Right;
            NamedFunctionParameterNode p = Assert.IsType<NamedFunctionParameterNode>(Assert.IsType<SingleResourceFunctionCallNode>(rightNode).Parameters.First());
            ParameterAliasNode aliasNode = Assert.IsType<ParameterAliasNode>(p.Value);
            Assert.Equal("@p1", aliasNode.Alias);
            Assert.True(aliasNode.TypeReference.IsEnum());
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", aliasNode.TypeReference.Definition.FullTypeName());

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            var node = Assert.IsType<ConstantNode>(aliasNodes["@p1"]);
            var enumValue = Assert.IsType<ODataEnumValue>(node.Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
            Assert.Equal("238563", enumValue.Value);
            Assert.True(aliasNodes["@p1"].TypeReference.IsEnum());

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildFilter_AliasInFunction_PropertyAsValue()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice%3Dfalse%2Cname%3D%40p1)&@p1=Name");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;

            var function = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters() as IEdmFunction;
            Assert.NotNull(function);
            var functionCallNode = odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(function);
            Assert.IsType<NamedFunctionParameterNode>(functionCallNode.Parameters.Last()).Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType.FindProperty("Name"));

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildFilter_AliasInFunction_Recursive_PropertyAsValue()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice%3Dfalse%2Cname%3D%40p1)&@p2=Name&@p1=%40p2");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            var function = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters() as IEdmFunction;
            Assert.NotNull(function);
            var functionCallNode = odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(function);
            Assert.IsType<NamedFunctionParameterNode>(functionCallNode.Parameters.Last()).Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p2"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType.FindProperty("Name"));

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildFilter_AliasInFunction_PropertyAsValue_TypeMismatch()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice%3D%40p1%2Cname%3D%40p1)&@p2=Name&@p1=%40p2");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            IEdmFunction function = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters() as IEdmFunction;
            Assert.NotNull(function);
            var functionCallNode = odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(function);

            Assert.IsType<NamedFunctionParameterNode>(functionCallNode.Parameters.Last()).Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p2"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType.FindProperty("Name"));

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildFilter_AliasInFunction_WithoutValue()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice%3D%40p1%2Cname%3D%40p2)");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            var function = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters() as IEdmFunction;
            Assert.NotNull(function);
            var functionCallNode = odataUri.Filter.Expression.ShouldBeSingleValueFunctionCallQueryNode(function);
            Assert.IsType<NamedFunctionParameterNode>(functionCallNode.Parameters.Last()).Value.ShouldBeParameterAliasNode("@p2", null);
            Assert.Null(aliasNodes["@p1"]);
            Assert.Null(aliasNodes["@p2"]);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildFilter_AliasInBinaryOp_ValueAsExpression()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=ID%20eq%20%40p1&@p1=1 add 2");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.Filter.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetInt32(false));
            aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add).Right.ShouldBeConstantQueryNode(2);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri);

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri);
        }

        [Fact]
        public void BuildFilter_AliasInFunction_BuiltIn()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$filter=contains(%40p1%2CName)&@p1=Name");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            Assert.IsType<SingleValueFunctionCallNode>(odataUri.Filter.Expression).Parameters.First().ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType.FindProperty("Name"));

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri,new UriComparer<Uri>());
        }

        [Fact]
        public void BuildFullUri_FilterSegmentWithAliasInFilterOption_AliasAsBoolean()
        {
            Uri fullUri = new Uri("http://gobbledygook/People/$filter(ID%20eq%201)?$filter=%40p1&@p1=true");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            List<FilterSegment> filterSegments = odataUri.Path.OfType<FilterSegment>().ToList();
            Assert.Single(filterSegments);
            Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
            Assert.False(filterSegments[0].SingleResult);

            BinaryOperatorNode binaryOperatorNode = filterSegments[0].Expression as BinaryOperatorNode;
            Assert.NotNull(binaryOperatorNode);
            Assert.Equal(BinaryOperatorKind.Equal, binaryOperatorNode.OperatorKind);
            binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonIdProp());
            binaryOperatorNode.Right.ShouldBeConstantQueryNode(1);

            odataUri.Filter.Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
            odataUri.ParameterAliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildRelativeUri_FilterSegmentWithAliasInFilterOption_AliasAsBoolean()
        {
            Uri relativeUri = new Uri("People/$filter(ID%20eq%201)?$filter=%40p1&@p1=true", UriKind.Relative);
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, relativeUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            List<FilterSegment> filterSegments = odataUri.Path.OfType<FilterSegment>().ToList();
            Assert.Single(filterSegments);
            Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
            Assert.False(filterSegments[0].SingleResult);

            BinaryOperatorNode binaryOperatorNode = filterSegments[0].Expression as BinaryOperatorNode;
            Assert.NotNull(binaryOperatorNode);
            Assert.Equal(BinaryOperatorKind.Equal, binaryOperatorNode.OperatorKind);
            binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonIdProp());
            binaryOperatorNode.Right.ShouldBeConstantQueryNode(1);

            odataUri.Filter.Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
            odataUri.ParameterAliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            Uri expectedUri = new Uri("http://gobbledygook/People/$filter(ID%20eq%201)?$filter=%40p1&@p1=true");

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(expectedUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(expectedUri, actualUri, new UriComparer<Uri>());
        }
        #endregion

        #region alias in orderby
        [Fact]
        public void BuildOrderby_AliasInFunction()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$orderby=Fully.Qualified.Namespace.HasDog(inOffice%3D%40p1)&@p1=true");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            var expectedFunc = HardCodedTestModel.GetAllHasDogFunctionOverloadsForPeople().Single(s => s.Parameters.Count() == 2);
            var funciontCallNode = odataUri.OrderBy.Expression.ShouldBeSingleValueFunctionCallQueryNode(expectedFunc);
            Assert.IsType<NamedFunctionParameterNode>(funciontCallNode.Parameters.Last()).Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
            aliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildOrderby_AliasInBinaryOp()
        {
            Uri fullUri = new Uri("http://gobbledygook/People?$orderby=ID%20mul%20%40p1%20asc&@p1=3 div 2");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            odataUri.OrderBy.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Multiply).Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetInt32(false));
            aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Divide).Right.ShouldBeConstantQueryNode(2);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(new Uri("http://gobbledygook/People?$orderby=ID%20mul%20%40p1&@p1=3 div 2"), actualUri);

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
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
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            IDictionary<string, SingleValueNode> aliasNodes = odataUri.ParameterAliasNodes;
            SingleValueFunctionCallNode node = (odataUri.SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem).OrderByOption.Expression as SingleValueFunctionCallNode;
            node.Parameters.Last().ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
            aliasNodes["@p1"].ShouldBeConstantQueryNode("abc");

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyPet2Set($orderby=concat(Color,@p1))") + "&@p1=" + Uri.EscapeDataString("'abc'"), actualUri.OriginalString);

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal("http://gobbledygook/People?$expand=" + Uri.EscapeDataString("MyPet2Set($orderby=concat(Color,@p1))") + "&@p1=" + Uri.EscapeDataString("'abc'"), actualUri.OriginalString);
        }
        #endregion

        #region alias in path segment
        [Fact]
        public void BuildFullUri_AliasInFilterPathSegment_AliasAsBoolean()
        {
            Uri fullUri = new Uri("http://gobbledygook/People/$filter(@p1)?@p1=true");
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, fullUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            List<FilterSegment> filterSegments = odataUri.Path.OfType<FilterSegment>().ToList();
            Assert.Single(filterSegments);
            Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
            Assert.False(filterSegments[0].SingleResult);
            filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));

            Assert.Null(odataUri.Filter);
            odataUri.ParameterAliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(fullUri, actualUri, new UriComparer<Uri>());
        }

        [Fact]
        public void BuildRelativeUri_AliasInFilterPathSegment_AliasAsBoolean()
        {
            Uri relativeUri = new Uri("People/$filter(@p1)?@p1=true", UriKind.Relative);
            ODataUriParser odataUriParser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, relativeUri);
            SetODataUriParserSettingsTo(this.settings, odataUriParser.Settings);
            odataUriParser.UrlKeyDelimiter = ODataUrlKeyDelimiter.Parentheses;
            ODataUri odataUri = odataUriParser.ParseUri();

            List<FilterSegment> filterSegments = odataUri.Path.OfType<FilterSegment>().ToList();
            Assert.Single(filterSegments);
            Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
            Assert.False(filterSegments[0].SingleResult);
            filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));

            Assert.Null(odataUri.Filter);
            odataUri.ParameterAliasNodes["@p1"].ShouldBeConstantQueryNode(true);

            Uri actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Parentheses);
            Assert.Equal(new Uri("http://gobbledygook/People/$filter(@p1)?@p1=true"), actualUri, new UriComparer<Uri>());

            actualUri = odataUri.BuildUri(ODataUrlKeyDelimiter.Slash);
            Assert.Equal(new Uri("http://gobbledygook/People/$filter(@p1)?@p1=true"), actualUri, new UriComparer<Uri>());
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
