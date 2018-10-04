//---------------------------------------------------------------------
// <copyright file="SetBasedOperationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    // $each set-based operations tests
    public class SetBasedOperationTests
    {
        #region tests
        [Fact]
        public void SetBasedOperations_EachSegmentAsFirstSegment_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_EachOnRoot);
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnSingleton_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/Boss/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.ShouldThrow<ODataException>().WithMessage(
                ODataErrorStrings.RequestUriProcessor_CannotApplyEachOnSingleEntities("Boss"));
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnSingleEntity_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.ShouldThrow<ODataException>().WithMessage(
                ODataErrorStrings.RequestUriProcessor_CannotApplyEachOnSingleEntities("People"));
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnEntityCollection_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(2);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnEntityCollectionWithFilterQuery_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each?$filter=ID eq 42"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(2);

                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode(42);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnEntityCollectionWithFilterSegment_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$each?@p1=ID eq 42"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(3);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    filterSegments.Count.Should().Be(1);

                    filterSegments[0].ParameterAlias.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    filterSegments[0].TargetEdmType.ToString().ShouldBeEquivalentTo(HardCodedTestModel.GetPersonType().ToString());
                    filterSegments[0].SingleResult.Should().BeFalse();

                    filterClause.Should().BeNull();
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode(42);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnEntityCollectionWithFilterSegmentAndFilterQuery_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$each?$filter=SSN eq 'num'&@p1=ID eq 42"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(3);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    filterSegments.Count.Should().Be(1);

                    filterSegments[0].ParameterAlias.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    filterSegments[0].TargetEdmType.ToString().ShouldBeEquivalentTo(HardCodedTestModel.GetPersonType().ToString());
                    filterSegments[0].SingleResult.Should().BeFalse();

                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode("num");
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode(42);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnTypeCastedMembers_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(3);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                    eachSegment.TargetEdmType.Should().Be(HardCodedTestModel.GetEmployeeType());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnTypeCastedMembersWithFilterSegment_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$filter(@p1)/$each?@p1=WorkEmail eq 'example@contoso.com'"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(4);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    filterSegments.Count.Should().Be(1);
                    filterSegments[0].ParameterAlias.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(true));
                    filterSegments[0].TargetEdmType.ToString().ShouldBeEquivalentTo(HardCodedTestModel.GetEmployeeType().ToString());
                    filterSegments[0].SingleResult.Should().BeFalse();

                    filterClause.Should().BeNull();
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode("example@contoso.com");

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                    eachSegment.TargetEdmType.Should().Be(HardCodedTestModel.GetEmployeeType());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnTypeCastedMembersWithFilterQuery_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$each?$filter=ID eq 42"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(3);

                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode(42);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                    eachSegment.TargetEdmType.Should().Be(HardCodedTestModel.GetEmployeeType());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnTypeCastedMembersWithFilterSegmentAndFilterQuery_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$filter(@p1)/$each?$filter=ID eq 42&@p1=WorkEmail eq 'example@contoso.com'"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(4);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    filterSegments.Count.Should().Be(1);
                    filterSegments[0].ParameterAlias.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(true));
                    filterSegments[0].TargetEdmType.ToString().ShouldBeEquivalentTo(HardCodedTestModel.GetEmployeeType().ToString());
                    filterSegments[0].SingleResult.Should().BeFalse();

                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode(42);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode("example@contoso.com");

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                    eachSegment.TargetEdmType.Should().Be(HardCodedTestModel.GetEmployeeType());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnNavigationProperty_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/MyFriendsDogs/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(4);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
                    eachSegment.TargetEdmType.Should().Be(HardCodedTestModel.GetDogType());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnBoundFunctionResults_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.GetPeopleWhoHaveDogs/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(3);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                    eachSegment.TargetEdmType.Should().Be(HardCodedTestModel.GetPersonType());
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnFilteredBoundFunctionResults_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/Fully.Qualified.Namespace.GetPeopleWhoHaveDogs/$each?@p1=ID eq 1"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(4);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    filterSegments.Count.Should().Be(1);
                    filterSegments[0].ParameterAlias.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    filterSegments[0].TargetEdmType.ToString().ShouldBeEquivalentTo(HardCodedTestModel.GetPersonType().ToString());
                    filterSegments[0].SingleResult.Should().BeFalse();

                    filterClause.Should().BeNull();
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode(1);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    eachSegment.Should().NotBeNull();
                    eachSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                    eachSegment.TargetEdmType.Should().Be(HardCodedTestModel.GetPersonType());
                });
        }

        [Fact]
        public void SetBasedOperations_ApplySetBasedActionOnCollection_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each/Fully.Qualified.Namespace.SummonPuppies"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(3);

                    List<EachSegment> eachSegments = oDataPath.OfType<EachSegment>().ToList();
                    eachSegments.Count.Should().Be(1);
                    eachSegments[0].TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                    eachSegments[0].TargetEdmType.Should().Be(HardCodedTestModel.GetPersonType());
                });
        }

        [Fact]
        public void SetBasedOperations_ApplySetBasedActionOnFilteredCollection_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$each/Fully.Qualified.Namespace.SummonPuppies?@p1=ID eq 1"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    oDataPath.Count.Should().Be(4);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    filterSegments.Count.Should().Be(1);
                    filterSegments[0].ParameterAlias.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    filterSegments[0].TargetEdmType.ToString().ShouldBeEquivalentTo(HardCodedTestModel.GetPersonType().ToString());
                    filterSegments[0].SingleResult.Should().BeFalse();

                    filterClause.Should().BeNull();
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Right.ShouldBeConstantQueryNode(1);

                    List<EachSegment> eachSegments = oDataPath.OfType<EachSegment>().ToList();
                    eachSegments.Count.Should().Be(1);
                    eachSegments[0].TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPeopleSet());
                    eachSegments[0].TargetEdmType.Should().Be(HardCodedTestModel.GetPersonType());
                });
        }

        // NOTE: Per OData 4.01 spec, GET operation and functions may proceed $each, but we are limiting the scope of that feature
        // by permitting only ONE action segment to follow $each.
        [Fact]
        public void SetBasedOperations_EachSegmentOnSetBasedActionOnCollectionResults_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each/Fully.Qualified.Namespace.SummonPuppies/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.SummonPuppies"));
        }

        // NOTE: Per OData 4.01 spec, GET operation and functions may proceed $each, but we are limiting the scope of that feature
        // by permitting only ONE action segment to follow $each.
        [Fact]
        public void SetBasedOperations_FunctionAfterEachSegment_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each/Fully.Qualified.Namespace.AllMyFriendsDogs"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_OnlySingleActionCanProceedEachPathSegment);
        }

        // NOTE: Per OData 4.01 spec, GET operation and functions may proceed $each, but we are limiting the scope of that feature
        // by permitting only ONE action segment to follow $each.
        [Fact]
        public void SetBasedOperations_NonActionPathSegmentAfterEachSegment_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_OnlySingleActionCanProceedEachPathSegment);
        }

        [Fact]
        public void SetBasedOperations_EachSegmentWithParenthesisExpression_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each(macrohard)"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.ShouldThrow<ODataException>().WithMessage(Strings.RequestUriProcessor_SyntaxError);
        }
        #endregion

        #region private methods
        private void ParseUriAndVerify(
            Uri uri,
            Action<ODataPath, FilterClause, IDictionary<string, SingleValueNode>> verifyAction)
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
                ODataQueryOptionParser queryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, entitySet.EntityType(), entitySet, dic)
                {
                    Configuration = { ParameterAliasValueAccessor = parser.ParameterAliasValueAccessor }
                };

                FilterClause filterClause = queryOptionParser.ParseFilter();

                // Two parser should share same ParameterAliasNodes
                verifyAction(path, filterClause, parser.ParameterAliasNodes);
                verifyAction(path, filterClause, queryOptionParser.ParameterAliasNodes);
            }

            //2. high level api - ParseUri
            {
                ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbledygook/"), uri);
                verifyAction(parser.ParsePath(), parser.ParseFilter(), parser.ParameterAliasNodes);
            }
        }

        private static IEdmNavigationSource ResolveEntitySource(ODataPath oDataPath)
        {
            IEdmNavigationSource navigationSource = null;
            foreach (ODataPathSegment segment in oDataPath)
            {
                navigationSource = segment.TranslateWith(new DetermineNavigationSourceTranslator()) ?? navigationSource;
            }

            return navigationSource;
        }
        #endregion
    }
}
