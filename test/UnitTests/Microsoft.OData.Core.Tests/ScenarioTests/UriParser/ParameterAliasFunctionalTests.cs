﻿//---------------------------------------------------------------------
// <copyright file="ParameterAliasFunctionalTests.cs" company="Microsoft">
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
using Microsoft.OData.Core;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    public class ParameterAliasFunctionalTests
    {
        #region alias in path
        [Fact]
        public void ParsePath_AliasInFunctionImport()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/GetPet4(id=@p1)?@p1=1.01M"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationImportSegment(
                        HardCodedTestModel.GetFunctionImportForGetPet4())
                        .Parameters.First()
                        .ShouldHaveParameterAliasNode("id", "@p1", EdmCoreModel.Instance.GetDecimal(false));
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(1.01M);
                });
        }

        [Fact]
        public void ParsePath_AliasInFunctionImport_Date()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/GetPersonByDate(date=@p1)?@p1=1997-12-12"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationImportSegment(
                        HardCodedTestModel.GetFunctionImportForGetPersonByDate())
                        .Parameters.First()
                        .ShouldHaveParameterAliasNode("date", "@p1", EdmCoreModel.Instance.GetDate(false));
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(new Date(1997, 12, 12));
                });
        }

        [Fact]
        public void ParsePath_AliasInFunctionImport_DateTimeOffsetPromote()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/GetPersonByDTO(dto=@p1)?@p1=1997-12-12"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationImportSegment(
                        HardCodedTestModel.GetFunctionImportForGetPersonByDTO())
                        .Parameters.First()
                        .ShouldHaveConvertNode("dto", EdmCoreModel.Instance.GetDateTimeOffset(false))
                        .Source.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetDate(false));
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(new Date(1997, 12, 12));
                });
        }

        [Fact]
        public void ParsePath_AliasInFunctionImport_DateTimeOffset()
        {
            ParseUriAndVerify(
               new Uri("http://gobbledygook/GetPersonByDTO(dto=@p1)?@p1=2014-09-19T07:13:14Z"),
               (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
               {
                   oDataPath.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPersonByDTO()).Parameters.First().ShouldHaveParameterAliasNode("dto", "@p1", EdmCoreModel.Instance.GetDateTimeOffset(false));
                   aliasNodes["@p1"].ShouldBeConstantQueryNode(new DateTimeOffset(2014, 9, 19, 7, 13, 14, new TimeSpan(0)));
               });
        }

        [Fact]
        public void ParsePath_AliasInFunctionImport_enum()
        {
            ParseUriAndVerify(
               new Uri("http://gobbledygook/GetPetCount(colorPattern=@p1)?@p1=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped'"),
               (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
               {
                   OperationSegmentParameter p = oDataPath.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPetCount()).Parameters.First();
                   Assert.Equal("colorPattern", p.Name);
                   var aliasNode = Assert.IsType<ParameterAliasNode>(p.Value);
                   Assert.Equal("@p1", aliasNode.Alias);
                   Assert.True(aliasNode.TypeReference.IsEnum());
                   Assert.Equal("Fully.Qualified.Namespace.ColorPattern", aliasNode.TypeReference.Definition.FullTypeName());
                   var constNode = Assert.IsType<ConstantNode>(aliasNodes["@p1"]);
                   var enumValue = Assert.IsType<ODataEnumValue>(constNode.Value);
                   Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
                   Assert.Equal("22", enumValue.Value);
                   Assert.True(aliasNodes["@p1"].TypeReference.IsEnum());
               });
        }

        [Fact]
        public void ParsePath_AliasInPath_AliasInFunction()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/GetPet4(id=@p1)?$filter=ID eq @p1&@p1=1.01M"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).Parameters.First().ShouldHaveParameterAliasNode("id", "@p1", EdmCoreModel.Instance.GetDecimal(false));
                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetDecimal(false));
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(1.01M);
                });
        }

        [Fact]
        public void ParsePath_AliasInFunctionImport_InvalidAliasName()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/GetPet4(id=@p!1)?@p!1=1.01M"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "!", 5, "id=@p!1"));
        }

        [Fact]
        public void ParsePath_AliasInFunctionImport_NullAsValue()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/GetPet4(id=@p1)?@p1=null"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).Parameters.First().ShouldHaveParameterAliasNode("id", "@p1", null);
                    aliasNodes["@p1"].ShouldBeConstantQueryNode((object)null);
                });
        }

        [Fact]
        public void ParsePath_AliasInBoundFunction()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People(123)/Fully.Qualified.Namespace.HasHat(onCat=@p1)?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.TestModel.FindOperations("Fully.Qualified.Namespace.HasHat").Single(s => (s as IEdmFunction).Parameters.Count() == 2));
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                });
        }

        [Fact]
        public void ParsePath_ComplexAliasInBoundFunction()
        {
            const string relativeUri = "People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address=@address)?@address={\"@odata.type\":\"%23Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"}";
            ParseUriAndVerify(
                new Uri("http://gobbledygook/" + relativeUri),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddress());
                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@address"]);
                    Assert.Equal("{\"@odata.type\":\"#Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"}", constNode.Value);
                });
        }

        [Fact]
        public void ParsePath_CollectionComplexAliasInBoundFunction()
        {
            const string relativeUri = "People(1)/Fully.Qualified.Namespace.CanMoveToAddresses(addresses=@addresses)?@addresses=[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}]";
            ParseUriAndVerify(
                new Uri("http://gobbledygook/" + relativeUri),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddresses());
                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@addresses"]);
                    Assert.Equal("[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}]", constNode.Value);
                });
        }

        [Fact]
        public void ParsePath_CollectionStringAliasInBoundFunction()
        {
            const string relativeUri = "People(1)/Fully.Qualified.Namespace.OwnsTheseDogs(dogNames=@dogNames)?@dogNames=[\"Barky\",\"Junior\"]";
            ParseUriAndVerify(
                new Uri("http://gobbledygook/" + relativeUri),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForOwnsTheseDogs());
                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@dogNames"]);
                    var items = constNode.Value.ShouldBeODataCollectionValue().ItemsShouldBeAssignableTo<string>();
                    Assert.Equal(2, items.Count());
                });
        }

        [Fact]
        public void ParsePath_IntArgumentOnShortParameter()
        {
            const string relativeUri = "People(1)/Fully.Qualified.Namespace.IsOlderThanShort(age=@age)?@age=1234";
            Action parseUri = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/" + relativeUri),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForIsOlderThanShort());
                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@age"]);
                    Assert.Equal(1234, constNode.Value);
                    Assert.Equal("Edm.Int16", constNode.TypeReference.FullName());
                });
            // TODO: This is a bug repro. Remove this assertion after the bug is fixed.
            parseUri.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_CannotConvertToType, "Edm.Int32", "Edm.Int16"));
        }

        [Theory]
        [InlineData("{\"@odata.type\":\"%23Fully.Qualified.Namespace.Film\",\"ID\":1,\"Title\":\"The Ogre's Lair\"}")]
        [InlineData("{\"@odata.type\":\"%23Fully.Qualified.Namespace.Film\",\"ID\":2,\"Title\":\"The \\\"Benevolent\\\" Dictator\"}")]
        [InlineData("{\"@odata.type\":\"%23Fully.Qualified.Namespace.Film\",\"ID\":3,\"Title\":\"The \\\"Gardener's\\\" Story\"}")]
        public void ParsePath_AliasInUnboundFunction_QuotesWithinDoubleQuotedStrings(string parameterValue)
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/GetRating(film=@p)?@p=" + parameterValue),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetRating());

                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@p"]);
                    Assert.Equal(parameterValue.Replace("%23", "#"), constNode.Value);
                });
        }

        [Theory]
        [InlineData("[{\"ID\":1,\"Title\":\"The Ogre's Lair\"},{\"ID\":2,\"Title\":\"The \\\"Benevolent\\\" Dictator\"},{\"ID\":3,\"Title\":\"The \\\"Gardener's\\\" Story\"}]")]
        public void ParsePath_AliasInUnboundFunction_QuotesWithinDoubleQuotedStringsInJsonArray(string parameterValue)
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/GetRatings(films=@p)?@p=" + parameterValue),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetRatings());

                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@p"]);
                    Assert.Equal(parameterValue.Replace("%23", "#"), constNode.Value);
                });
        }

        [Fact]
        public void ParsePath_AliasInUnboundFunction_WithinDollarRootPath()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/GetRatings(films=@c)?@c=[$root/Films(1),$root/Films(3)]"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    oDataPath.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetRatings());

                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@c"]);
                    Assert.Equal("[$root/Films(1),$root/Films(3)]", constNode.Value);
                });
        }

        #endregion

        #region alias in filter
        [Fact]
        public void ParseFilter_AliasInFunction()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.AllHaveDog(inOffice=@p1)&@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var parameters = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForAllHaveDogWithTwoParameters()).Parameters;
                    var paramNode = Assert.IsType<NamedFunctionParameterNode>(parameters.Last());
                    paramNode.Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFunction_enum()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=null ne Fully.Qualified.Namespace.GetPetCount(colorPattern=@p1)&@p1=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped'"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var bon = Assert.IsType<BinaryOperatorNode>(filterClause.Expression);
                    var functionCallNode = Assert.IsType<SingleResourceFunctionCallNode>(bon.Right);
                    NamedFunctionParameterNode p = Assert.IsType<NamedFunctionParameterNode>(functionCallNode.Parameters.First());

                    var aliasNode = Assert.IsType<ParameterAliasNode>(p.Value);
                    Assert.Equal("@p1", aliasNode.Alias);
                    Assert.True(aliasNode.TypeReference.IsEnum());
                    Assert.Equal("Fully.Qualified.Namespace.ColorPattern", aliasNode.TypeReference.Definition.FullTypeName());

                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@p1"]);
                    var enumValue = Assert.IsType<ODataEnumValue>(constNode.Value);
                    Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
                    Assert.Equal("22", enumValue.Value);
                    Assert.True(aliasNodes["@p1"].TypeReference.IsEnum());
                });
        }

        [Fact]
        public void ParseFilter_AliasInFunction_enum_undefined()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=null ne Fully.Qualified.Namespace.GetPetCount(colorPattern=@p1)&@p1=Fully.Qualified.Namespace.ColorPattern'238563'"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var bon = Assert.IsType<BinaryOperatorNode>(filterClause.Expression);
                    var functionCallNode = Assert.IsType<SingleResourceFunctionCallNode>(bon.Right);
                    NamedFunctionParameterNode p = Assert.IsType<NamedFunctionParameterNode>(functionCallNode.Parameters.First());

                    var aliasNode = Assert.IsType<ParameterAliasNode>(p.Value);
                    Assert.Equal("@p1", aliasNode.Alias);
                    Assert.True(aliasNode.TypeReference.IsEnum());
                    Assert.Equal("Fully.Qualified.Namespace.ColorPattern", aliasNode.TypeReference.Definition.FullTypeName());

                    var constNode = Assert.IsType<ConstantNode>(aliasNodes["@p1"]);
                    var enumValue = Assert.IsType<ODataEnumValue>(constNode.Value);
                    Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
                    Assert.Equal("238563", enumValue.Value);
                    Assert.True(aliasNodes["@p1"].TypeReference.IsEnum());
                });
        }

        [Fact]
        public void ParseFilter_AliasInFunction_PropertyAsValue()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice=false,name=@p1)&@p1=Name"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var function = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters() as IEdmFunction;
                    Assert.NotNull(function);

                    var parameters = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(function).Parameters;
                    var paramNode = Assert.IsType<NamedFunctionParameterNode>(parameters.Last());
                    paramNode.Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
                    aliasNodes["@p1"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType.FindProperty("Name"));
                });
        }

        [Fact]
        public void ParseFilter_AliasInFunction_Recursive_PropertyAsValue()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice=false,name=@p1)&@p2=Name&@p1=@p2"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var function = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters() as IEdmFunction;
                    Assert.NotNull(function);

                    var parameters = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(function).Parameters;
                    var paramNode = Assert.IsType<NamedFunctionParameterNode>(parameters.Last());
                    paramNode.Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
                    aliasNodes["@p1"].ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetString(true));
                    aliasNodes["@p2"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType.FindProperty("Name"));
                });
        }

        [Fact]
        public void ParseFilter_AliasInFunction_CircleReference()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice=false,name=@p1)&@p2=@p1&@p1=@p2"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(SRResources.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void ParseFilter_AliasInFunction_PropertyAsValue_TypeMismatch()
        {
            // TODO: FunctionOverloadResolver.ResolveOperationFromList() should check type mismatch and throw exception.
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice=@p1,name=@p1)&@p2=Name&@p1=@p2"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var function = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters() as IEdmFunction;
                    Assert.NotNull(function);

                    var parameters = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(function).Parameters;
                    var paramNode = Assert.IsType<NamedFunctionParameterNode>(parameters.Last());
                    paramNode.Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
                    aliasNodes["@p1"].ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetString(true));
                    aliasNodes["@p2"].ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPeopleSet().EntityType.FindProperty("Name"));
                });
        }

        [Fact]
        public void ParseFilter_AliasInFunction_WithoutValue()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=Fully.Qualified.Namespace.HasDog(inOffice=@p1,name=@p2)"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var function = HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters() as IEdmFunction;
                    Assert.NotNull(function);
                    var parameters = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(function).Parameters;
                    var paramNode = Assert.IsType<NamedFunctionParameterNode>(parameters.Last());
                    paramNode.Value.ShouldBeParameterAliasNode("@p2", null);
                    Assert.Null(aliasNodes["@p1"]);
                    Assert.Null(aliasNodes["@p2"]);
                });
        }

        [Fact]
        public void ParseFilter_AliasInBinaryOp_ValueAsExpression()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=ID eq @p1&@p1=1 add 2"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetInt32(false));
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add).Right.ShouldBeConstantQueryNode(2);
                });
        }

        [Fact]
        public void ParseFilter_AliasInsideInOperator_OperandsAsExpressions()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$filter=@p1 in RelatedIDs&@p1=ID"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var inNode = Assert.IsType<InNode>(filterClause.Expression);
                    inNode.Left.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetInt32(false));

                    var propAccessNode = Assert.IsType<CollectionPropertyAccessNode>(inNode.Right);
                    Assert.Equal("RelatedIDs", propAccessNode.Property.Name);

                    var singlePropAccessNode = Assert.IsType<SingleValuePropertyAccessNode>(aliasNodes["@p1"]);
                    Assert.Equal("ID", singlePropAccessNode.Property.Name);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AliasAsFirstSegment()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/$filter(@p1)&@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(SRResources.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_FilterSegmentOnSingleton()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/Boss/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(Error.Format(SRResources.RequestUriProcessor_CannotApplyFilterOnSingleEntities, "Boss"));
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_SingleEntity()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(Error.Format(SRResources.RequestUriProcessor_CannotApplyFilterOnSingleEntities, "People"));
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AliasAsBoolean()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AliasAsExpression()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?@p1=ID eq 42"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(42);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AliasAsNestedExpression()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?@p1=ID eq @p2&@p2=9001"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal)
                        .Right.ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetInt32(false));
                    aliasNodes["@p2"].ShouldBeConstantQueryNode(9001);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_InvalidAliasName()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p!1)?@p!1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "!", 2, "@p!1"));
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AliasAsNonBoolean()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?@p1=1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(SRResources.MetadataBinder_FilterExpressionNotSingleValue);
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AliasAsNull()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?@p1=null"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", null);
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    Assert.Null(aliasNodes["@p1"]);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AliasWithCircleReference()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?@p1=@p2&@p2=@p1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(SRResources.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AliasIsItself()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?@p1=@p1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(SRResources.UriQueryExpressionParser_TooDeep);
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_WithoutValue()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", null);
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    Assert.Null(aliasNodes["@p1"]);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_ValueAsExpression()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(ID eq @p1)?@p1=1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    BinaryOperatorNode binaryOperatorNode = filterSegments[0].Expression as BinaryOperatorNode;
                    Assert.NotNull(binaryOperatorNode);
                    Assert.Equal(BinaryOperatorKind.Equal, binaryOperatorNode.OperatorKind);
                    binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonIdProp());
                    binaryOperatorNode.Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetInt32(false));

                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(1);
                });
        }

        [Fact]
        public void ParseFilter_BinaryExpressionInFilterPathSegment_ValueAsExpression()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(ID eq 1)"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    BinaryOperatorNode binaryOperatorNode = filterSegments[0].Expression as BinaryOperatorNode;
                    Assert.NotNull(binaryOperatorNode);
                    Assert.Equal(BinaryOperatorKind.Equal, binaryOperatorNode.OperatorKind);
                    binaryOperatorNode.Left.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonIdProp());
                    binaryOperatorNode.Right.ShouldBeConstantQueryNode(1);

                    Assert.Null(filterClause);
                    Assert.Empty(aliasNodes);
                });
        }

        [Fact]
        public void ParseFilter_ExpressionInFilterPathSegment_ValueAsBoolean()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(true)"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeConstantQueryNode(true);
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    Assert.Empty(aliasNodes);
                });
        }

        [Fact]
        public void ParseFilter_ExpressionInFilterPathSegment_ValueAsInteger()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(9001)"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });

            parse.Throws<ODataException>(SRResources.MetadataBinder_FilterExpressionNotSingleValue);
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_ExpressionResolvesToInteger()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@a1)?@a1=1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });

            parse.Throws<ODataException>(SRResources.MetadataBinder_FilterExpressionNotSingleValue);
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_ValueAsFunction()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?@p1=Fully.Qualified.Namespace.AllHaveDog(inOffice=true)"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(true));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForAllHaveDogWithTwoParameters());
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_MultipleFilterSegments()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$filter(@p2)?@p1=true&@p2=false"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Equal(2, filterSegments.Count);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    filterSegments[1].Expression.ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[1].TargetEdmType.ToString());
                    Assert.False(filterSegments[1].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                    aliasNodes["@p2"].ShouldBeConstantQueryNode(false);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_WithFilterQueryOption()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)?$filter=@p2&@p1=SSN eq 'num'&@p2=ID eq 1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(true));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    filterClause.Expression.ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterClause.ItemType.Definition.ToString());

                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode("num");
                    aliasNodes["@p2"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(1);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_NavigationProperty()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/MyFriendsDogs/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetDogType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_WithRef()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/MyFriendsDogs/$filter(@p1)/$ref?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(5, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetDogType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_RefThenFilterSegment()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/MyFriendsDogs/$ref/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataUnrecognizedPathException>(Error.Format(SRResources.RequestUriProcessor_MustBeLeafSegment, "$ref"));
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_WithCount()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$count?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_CountThenFilterSegment()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$count/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataUnrecognizedPathException>(Error.Format(SRResources.RequestUriProcessor_MustBeLeafSegment, "$count"));
        }

        // NOTE: Per OData 4.01 spec, the $filter query option must not be used in conjunction with both
        // a $count path segment and a $filter path segment. However, we are allowing this syntactically.
        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_WithCountAndFilterQueryOption()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$count?$filter=@p2&@p1=true&@p2=ID eq 1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    filterClause.Expression.ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterClause.ItemType.Definition.ToString());
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                    aliasNodes["@p2"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(1);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_AppliedOnBoundFunctionResults()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.GetPeopleWhoHaveDogs/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_FilterSegmentThenBoundFunctionThenFilterSegment()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/Fully.Qualified.Namespace.GetPeopleWhoHaveDogs/$filter(@p2)?@p1=ID eq 1&@p2=SSN eq 'num'"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Equal(2, filterSegments.Count);
                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    filterSegments[1].Expression.ShouldBeParameterAliasNode("@p2", EdmCoreModel.Instance.GetBoolean(true));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[1].TargetEdmType.ToString());
                    Assert.False(filterSegments[1].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(1);
                    aliasNodes["@p2"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode("num");
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_FilterSegmentThenBoundAction()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/Fully.Qualified.Namespace.AdoptShibaInu?@p1=ID eq 1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);
                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(1);
                });
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_FilterSegmentAfterNonComposableOperation()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.AdoptShibaInu/$filter(@p1)?@p1=ID eq 1"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(Error.Format(SRResources.RequestUriProcessor_MustBeLeafSegment, "Fully.Qualified.Namespace.AdoptShibaInu"));
        }

        [Fact]
        public void ParseFilter_AliasInFilterPathSegment_FilterSegmentOnTypeCastedMembers()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$filter(@p1)?@p1=WorkEmail eq 'example@contoso.com'"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);
                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(true));
                    Assert.Equal(HardCodedTestModel.GetEmployeeType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode("example@contoso.com");
                });
        }
        #endregion

        #region alias in orderby
        [Fact]
        public void ParseOrderby_AliasInFunction()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$orderby=Fully.Qualified.Namespace.HasDog(inOffice=@p1)&@p1=true"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    var expectedFunc = HardCodedTestModel.GetAllHasDogFunctionOverloadsForPeople().Single(s => s.Parameters.Count() == 2);
                    (orderByClause.Expression.ShouldBeSingleValueFunctionCallQueryNode(expectedFunc).Parameters.Last() as NamedFunctionParameterNode).Value.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    aliasNodes["@p1"].ShouldBeConstantQueryNode(true);
                });
        }

        [Fact]
        public void ParseOrderby_AliasInBinaryOp()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$orderby=ID mul @p1 asc&@p1=3 div 2"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    orderByClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Multiply).Right.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetInt32(false));
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Divide).Right.ShouldBeConstantQueryNode(2);
                });
        }
        #endregion

        #region expand
        [Fact]
        public void ParseExpandOrderby_AliasInFunction()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People?$expand=MyPet2Set($orderby=concat(Color, @p1 )asc )&@p1='abc'"),
                (oDataPath, filterClause, orderByClause, selectExpandClause, aliasNodes) =>
                {
                    SingleValueFunctionCallNode node = (selectExpandClause.SelectedItems.First() as ExpandedNavigationSelectItem).OrderByOption.Expression as SingleValueFunctionCallNode;
                    node.Parameters.Last().ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetString(true));
                    aliasNodes["@p1"].ShouldBeConstantQueryNode("abc");
                });
        }
        #endregion

        #region private methods
        private void ParseUriAndVerify(
            Uri uri,
            Action<ODataPath, FilterClause, OrderByClause, SelectExpandClause, IDictionary<string, SingleValueNode>> verifyAction)
        {
            // run 2 test passes:
            // 1. low level api - ODataUriParser instance methods
            {
                List<CustomQueryOptionToken> queries = Microsoft.OData.UriParser.QueryOptionUtils.ParseQueryOptions(uri);
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbledygook/"), uri);

                ODataPath path = parser.ParsePath();
                IEdmNavigationSource entitySource = ResolveEntitySource(path);
                IEdmEntitySet entitySet = entitySource as IEdmEntitySet;

                var dic = queries.ToDictionary(customQueryOptionToken => customQueryOptionToken.Name, customQueryOptionToken => queries.GetQueryOptionValue(customQueryOptionToken.Name));
                ODataQueryOptionParser queryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, entitySet?.EntityType, entitySet, dic)
                {
                    Configuration = { ParameterAliasValueAccessor = parser.ParameterAliasValueAccessor }
                };

                FilterClause filterClause = queryOptionParser.ParseFilter();
                SelectExpandClause selectExpandClause = queryOptionParser.ParseSelectAndExpand();
                OrderByClause orderByClause = queryOptionParser.ParseOrderBy();

                // Two parser should share same ParameterAliasNodes
                verifyAction(path, filterClause, orderByClause, selectExpandClause, parser.ParameterAliasNodes);
                verifyAction(path, filterClause, orderByClause, selectExpandClause, queryOptionParser.ParameterAliasNodes);
            }

            //2. high level api - ParseUri
            {
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbledygook/"), uri);
                verifyAction(parser.ParsePath(), parser.ParseFilter(), parser.ParseOrderBy(), parser.ParseSelectAndExpand(), parser.ParameterAliasNodes);
            }
        }

        private static IEdmNavigationSource ResolveEntitySource(ODataPath oDataPath)
        {
            IEdmNavigationSource navigationSource = null;
            foreach (ODataPathSegment segment in oDataPath)
            {
                navigationSource = segment.TranslateWith(DetermineNavigationSourceTranslator.Instance) ?? navigationSource;
            }

            return navigationSource;
        }
        #endregion
    }
}
