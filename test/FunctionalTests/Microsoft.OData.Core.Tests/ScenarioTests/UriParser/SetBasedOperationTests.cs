//---------------------------------------------------------------------
// <copyright file="SetBasedOperationTests.cs" company="Microsoft">
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
            parse.Throws<ODataUnrecognizedPathException>(ODataErrorStrings.RequestUriProcessor_EachOnRoot);
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnSingleton_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/Boss/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });

            parse.Throws<ODataException>(ODataErrorStrings.RequestUriProcessor_CannotApplyEachOnSingleEntities("Boss"));
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnSingleEntity_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(ODataErrorStrings.RequestUriProcessor_CannotApplyEachOnSingleEntities("People"));
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnEntityCollection_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnPrimitiveTypeCollection_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/RelatedIDs/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Primitive, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnComplexTypeCollection_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/PreviousAddresses/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnEntityCollectionWithFilterQuery_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each?$filter=ID eq 42"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(2, oDataPath.Count);

                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(42);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnEntityCollectionWithFilterSegment_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$each?@p1=ID eq 42"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(42);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnEntityCollectionWithFilterSegmentAndFilterQuery_ReturnsSuccess()
        {
            // Scenario of this test case:
            // A client is sending a PATCH request to update all people with ID eq 42 and return the updated people
            // whose SSN eq 'num'.
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$each?$filter=SSN eq 'num'&@p1=ID eq 42"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);

                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode("num");
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(42);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnTypeCastedMembers_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetEmployeeType(), eachSegment.TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnTypeCastedMembersWithFilterSegment_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$filter(@p1)/$each?@p1=WorkEmail eq 'example@contoso.com'"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);
                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(true));
                    Assert.Equal(HardCodedTestModel.GetEmployeeType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode("example@contoso.com");

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetEmployeeType(), eachSegment.TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnTypeCastedMembersWithFilterQuery_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$each?$filter=ID eq 42"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(42);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetEmployeeType(), eachSegment.TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnTypeCastedMembersWithFilterSegmentAndFilterQuery_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.Employee/$filter(@p1)/$each?$filter=ID eq 42&@p1=WorkEmail eq 'example@contoso.com'"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);
                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(true));
                    Assert.Equal(HardCodedTestModel.GetEmployeeType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(42);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode("example@contoso.com");

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetEmployeeType(), eachSegment.TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnNavigationProperty_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/1/MyFriendsDogs/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetDogsSet(), eachSegment.TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetDogType(), eachSegment.TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnBoundFunctionResults_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/Fully.Qualified.Namespace.GetPeopleWhoHaveDogs/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetPersonType(), eachSegment.TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnFilteredBoundFunctionResults_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/Fully.Qualified.Namespace.GetPeopleWhoHaveDogs/$each?@p1=ID eq 1"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);
                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(1);

                    EachSegment eachSegment = oDataPath.Last() as EachSegment;
                    Assert.NotNull(eachSegment);
                    Assert.Equal(RequestTargetKind.Resource, eachSegment.TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegment.TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetPersonType(), eachSegment.TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_ApplySetBasedActionOnCollection_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each/Fully.Qualified.Namespace.SummonPuppies"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<EachSegment> eachSegments = oDataPath.OfType<EachSegment>().ToList();
                    Assert.Single(eachSegments);
                    Assert.Equal(RequestTargetKind.Resource, eachSegments[0].TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegments[0].TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetPersonType(), eachSegments[0].TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_ApplySetBasedActionOnFilteredCollection_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$filter(@p1)/$each/Fully.Qualified.Namespace.SummonPuppies?@p1=ID eq 1"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(4, oDataPath.Count);

                    List<FilterSegment> filterSegments = oDataPath.OfType<FilterSegment>().ToList();
                    Assert.Single(filterSegments);
                    filterSegments[0].Expression.ShouldBeParameterAliasNode("@p1", EdmCoreModel.Instance.GetBoolean(false));
                    Assert.Equal(HardCodedTestModel.GetPersonType().ToString(), filterSegments[0].TargetEdmType.ToString());
                    Assert.False(filterSegments[0].SingleResult);

                    Assert.Null(filterClause);
                    aliasNodes["@p1"].ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).Right.ShouldBeConstantQueryNode(1);

                    List<EachSegment> eachSegments = oDataPath.OfType<EachSegment>().ToList();
                    Assert.Single(eachSegments);
                    Assert.Equal(RequestTargetKind.Resource, eachSegments[0].TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegments[0].TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetPersonType(), eachSegments[0].TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_EachSegmentOnSetBasedActionOnCollectionResults_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each/Fully.Qualified.Namespace.SummonPuppies/$each"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.SummonPuppies"));
        }

        [Fact]
        public void SetBasedOperations_FunctionAfterEachSegment_ReturnsSuccess()
        {
            ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each/Fully.Qualified.Namespace.AllMyFriendsDogs"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                    Assert.Equal(3, oDataPath.Count);

                    List<EachSegment> eachSegments = oDataPath.OfType<EachSegment>().ToList();
                    Assert.Single(eachSegments);
                    Assert.Equal(RequestTargetKind.Resource, eachSegments[0].TargetKind);
                    Assert.Same(HardCodedTestModel.GetPeopleSet(), eachSegments[0].TargetEdmNavigationSource);
                    Assert.Same(HardCodedTestModel.GetPersonType(), eachSegments[0].TargetEdmType);
                });
        }

        [Fact]
        public void SetBasedOperations_NonActionPathSegmentAfterEachSegment_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each/$filter(@p1)?@p1=true"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(ODataErrorStrings.RequestUriProcessor_OnlySingleOperationCanFollowEachPathSegment);
        }

        [Fact]
        public void SetBasedOperations_EachSegmentWithParenthesisExpression_ThrowsException()
        {
            Action parse = () => ParseUriAndVerify(
                new Uri("http://gobbledygook/People/$each(macrohard)"),
                (oDataPath, filterClause, aliasNodes) =>
                {
                });
            parse.Throws<ODataException>(Strings.RequestUriProcessor_SyntaxError);
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
                ODataQueryOptionParser queryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, entitySet?.EntityType, entitySet, dic)
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
                navigationSource = segment.TranslateWith(DetermineNavigationSourceTranslator.Instance) ?? navigationSource;
            }

            return navigationSource;
        }
        #endregion
    }
}
