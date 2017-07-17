//---------------------------------------------------------------------
// <copyright file="CaseInsensitiveResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Tests.UriParser.Binders;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Metadata
{
    /// <summary>
    /// Unit tests for CaseInsensitiveODataUriResolver
    /// </summary>
    public class CaseInsensitiveResolverTests : ExtensionTestBase
    {
        #region type cases
        // TODO: Does not work with model ref, as model ref's SchemaElements is incomplete.
        [Fact]
        public void CaseInsensitiveTypeCastInPath()
        {
            this.TestCaseInsensitive(
                "People/TestNS.Person",
                "People/TesTNS.PERsON",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(PersonType, false))),
                Strings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void CaseInsensitiveTypeCastOnComplexTypeInPath()
        {
            this.TestCaseInsensitive(
                "People(1)/Addr/TestNS.Address",
                "People(1)/Addr/TestNS.ADDress",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeTypeSegment(AddrType),
                Strings.RequestUriProcessor_ResourceNotFound("TestNS.ADDress"));
        }

        [Fact]
        public void CaseInsensitiveTypeCastNameConflictInPath()
        {
            this.TestCaseInsensitiveConflict(
                "People(1)/Pen2/TestNS.StarPencil",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeTypeSegment(StarPencil),
                "More than one types match the name 'TestNS.StarPencil' were found in model.");
        }

        [Fact]
        public void CaseInsensitiveTypeCastNameNonexistInPath()
        {
            this.TestCaseInsensitiveNotExist(
                "People/NS.WHY",
                parser => parser.ParsePath(),
                Strings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void CaseInsensitiveTypeCastInSelectExpand()
        {
            this.TestCaseInsensitive(
                "People?$select=TestNS.Person/Name",
                "People?$select=TesTNS.PERsON/Name",
                parser => parser.ParseSelectAndExpand(),
                clause => clause.SelectedItems.Single().ShouldBePathSelectionItem(new ODataSelectPath(
                    new ODataPathSegment[]
                {
                    new TypeSegment(PersonType, PeopleSet),
                    new PropertySegment(PersonNameProp),
                })),
               Strings.ExpandItemBinder_CannotFindType("TesTNS.PERsON"));
        }

        [Fact]
        public void CaseInsensitiveTypeCastOnComplexTypeInSelectExpand()
        {
            this.TestCaseInsensitive(
               "People?$select=Addr/TestNS.Address",
               "People?$select=Addr/TesTNS.Address",
               parser => parser.ParseSelectAndExpand(),
               clause => clause.SelectedItems.Single().ShouldBePathSelectionItem(new ODataSelectPath(
                    new ODataPathSegment[]
                {
                    new PropertySegment(AddrProperty),
                    new TypeSegment(AddrType, null),
                })),
               Strings.SelectBinder_MultiLevelPathInSelect);
        }

        [Fact]
        public void CaseInsensitiveTypeCastNameConflictInSelectExpand()
        {
            this.TestCaseInsensitiveConflict(
                "People(1)/Pen2?$select=TestNS.StarPencil/Id",
                parser => parser.ParseSelectAndExpand(),
                clause => clause.SelectedItems.Single().ShouldBePathSelectionItem(new ODataSelectPath(
                      new ODataPathSegment[]
                {
                    new TypeSegment(StarPencil, PencilSet),
                    new PropertySegment(PencilId),
                })),
                "More than one types match the name 'TestNS.StarPencil' were found in model.");
        }

        [Fact]
        public void CaseInsensitiveTypeCastNonexistTypeInSelectExpand()
        {
            this.TestCaseInsensitiveNotExist(
                "People?$select=NS.WHY/Supername",
                parser => parser.ParseSelectAndExpand(),
                Strings.ExpandItemBinder_CannotFindType("NS.WHY"));
        }

        [Fact]
        public void CaseInsensitiveTypeCastInOrderBy()
        {
            this.TestCaseInsensitive(
               "People?$orderby=TestNS.Person/Name",
               "People?$orderby=TesTNS.PERsON/Name",
               (parser) => parser.ParseOrderBy(),
               (clause) => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PersonNameProp),
               Strings.CastBinder_ChildTypeIsNotEntity("TesTNS.PERsON"));
        }

        [Fact]
        public void CaseInsensitiveTypeCastOnComplexTypeInFilter()
        {
            this.TestCaseInsensitive(
               "People?$filter=TestNS.Person/Addr/TestNS.Address/ZipCode eq '550'",
               "People?$filter=TestNS.Person/Addr/TestNS.AddrESS/ZipCode eq '550'",
               parser => parser.ParseFilter(),
               clause => clause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal).And.Left.ShouldBeSingleValuePropertyAccessQueryNode(ZipCodeProperty),
               Strings.CastBinder_ChildTypeIsNotEntity("TestNS.AddrESS"));
        }

        //[Fact(Skip = "#582: Do not support built-in type name case insensitive. EnumValue's type name case insensitve also not supported.")]
        //public void CaseInsensitiveTypeCastInQueryOptionOrderBy2()
        //{
        //    var uriParser = new ODataUriParser(
        //        Model,
        //        ServiceRoot,
        //        new Uri("http://host/People?$orderby=cast(null, Edm.StRing)"))
        //    {
        //        Resolver = new ODataUriResolver() { EnableCaseInsensitive = true }
        //    };
        //
        //    var clause = uriParser.ParseOrderBy();
        //    clause.Expression.ShouldBeSingleValueFunctionCallQueryNode("cast", EdmCoreModel.Instance.GetString(false));
        //}

        [Fact]
        public void CaseInsensitiveTypeCastTypeNameConflictsInOrderby()
        {
            this.TestCaseInsensitiveConflict(
                "People?$orderby=Pen2/TestNS.StarPencil/Id",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PencilId),
                "More than one types match the name 'TestNS.StarPencil' were found in model.");
        }

        [Fact]
        public void CaseInsensitiveTypeCastNonexistTypeInOrderby()
        {
            this.TestCaseInsensitiveNotExist(
                "People?$orderby=NS.WHY/Supername",
                parser => parser.ParseOrderBy(),
                Strings.CastBinder_ChildTypeIsNotEntity("NS.WHY"));
        }
        #endregion

        #region navigation source cases
        [Fact]
        public void CaseInsensitiveEntitySetName()
        {
            this.TestCaseInsensitive(
               "People",
               "PeoplE",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeEntitySetSegment(PeopleSet),
               Strings.RequestUriProcessor_ResourceNotFound("PeoplE"));
        }

        [Fact]
        public void CaseInsensitiveEntitySetNameConflicts()
        {
            this.TestCaseInsensitiveConflict(
               "PencilSet",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeEntitySetSegment(PencilSet),
               "More than one navigation sources match the name 'PencilSet' were found in model.");
        }

        [Fact]
        public void CaseInsensitiveSingletonName()
        {
            this.TestCaseInsensitive(
               "Boss",
               "BOSS",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeSingletonSegment(Boss),
               Strings.RequestUriProcessor_ResourceNotFound("BOSS"));
        }

        [Fact]
        public void CaseInsensitiveSingletonNameConflicts()
        {
            this.TestCaseInsensitiveConflict(
               "BAJIE",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeSingletonSegment(BajieUpper),
               "More than one navigation sources match the name 'BAJIE' were found in model.");
        }

        [Fact]
        public void CaseInsensitiveNonExistNavigationSource()
        {
            this.TestCaseInsensitiveNotExist(
               "WUKONG",
               parser => parser.ParsePath(),
               Strings.RequestUriProcessor_ResourceNotFound("WUKONG"));
        }


        [Fact]
        public void CaseInsensitiveEntitySetKeyName()
        {
            this.TestCaseInsensitive(
               "PetSet(key1=1, key2='stm')",
               "PetSet(key1=1, KEY2='stm')",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("key1", 1), new KeyValuePair<string, object>("key2", "stm")),
               Strings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void CaseInsensitiveEntitySetKeyNameConflicts()
        {
            this.TestCaseInsensitiveConflict(
               "PetSetCon(key=1, KEY='stm')",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("key", 1), new KeyValuePair<string, object>("KEY", "stm")),
               "More than one keys match the name 'key' were found.");
        }

        [Fact]
        public void CaseInsensitiveEntitySetKeyNameNonexist()
        {
            this.TestCaseInsensitiveNotExist(
               "PetSet(key1=1, key3='stm')",
               parser => parser.ParsePath(),
               Strings.RequestUriProcessor_SyntaxError);
        }
        #endregion

        #region property name cases
        [Fact]
        public void CaseInsensitivePropertyNameInPath()
        {
            this.TestCaseInsensitive(
               "People(1)/Name",
               "People(1)/nAmE",
               (parser) => parser.ParsePath(),
               (path) => path.LastSegment.ShouldBePropertySegment(PersonNameProp),
               Strings.RequestUriProcessor_ResourceNotFound("nAmE"));
        }

        [Fact]
        public void CaseInsensitivePropertyNameNonexistNameInPath()
        {
            this.TestCaseInsensitiveNotExist(
                "People(1)/Name1",
                parser => parser.ParsePath(),
                Strings.RequestUriProcessor_ResourceNotFound("Name1"));
        }

        [Fact]
        public void CaseInsensitivePropertyNameConflictsInPath()
        {
            this.TestCaseInsensitiveConflict(
                "People(1)/Pen",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBeNavigationPropertySegment(PersonNavPen),
                Strings.UriParserMetadata_MultipleMatchingPropertiesFound("Pen", "TestNS.Person"));

            this.TestCaseInsensitiveConflict(
                "People(1)/pen",
                parser => parser.ParsePath(),
                path => path.LastSegment.ShouldBePropertySegment(PersonPen),
                Strings.UriParserMetadata_MultipleMatchingPropertiesFound("pen", "TestNS.Person"));
        }

        [Fact]
        public void CaseInsensitivePropertyNameForComplexTypeInPath()
        {
            this.TestCaseInsensitive(
               "People(1)/Addr/ZipCode",
               "People(1)/addr/zipcode",
               (parser) => parser.ParsePath(),
               (path) => path.LastSegment.ShouldBePropertySegment(ZipCodeProperty),
               Strings.RequestUriProcessor_ResourceNotFound("addr"));
        }

        [Fact]
        public void CaseInsensitivePropertyNameInSelectExpand()
        {
            this.TestCaseInsensitive(
                "People?$select=Addr/ZipCode",
                "People?$select=ADDR/ZIPCODE",
                parser => parser.ParseSelectAndExpand(),
                clause => clause.SelectedItems.Single().ShouldBePathSelectionItem(new ODataSelectPath(
                    new ODataPathSegment[]
                    {
                        new PropertySegment(AddrProperty),
                        new PropertySegment(ZipCodeProperty),
                    })),
                Strings.MetadataBinder_PropertyNotDeclared("TestNS.Person", "ADDR"));
        }

        [Fact]
        public void CaseInsensitivePropertyNameNonexistInSelectExpand()
        {
            this.TestCaseInsensitiveNotExist(
                "People?$select=Name1",
                parser => parser.ParseSelectAndExpand(),
                Strings.MetadataBinder_PropertyNotDeclared("TestNS.Person", "Name1"));
        }

        [Fact]
        public void CaseInsensitivePropertyNameConflictsInSelectExpand()
        {
            this.TestCaseInsensitiveConflict(
                "People(1)?$expand=Pen",
                parser => parser.ParseSelectAndExpand(),
                clause => clause.SelectedItems.Single().ShouldBeExpansionFor(PersonNavPen),
                Strings.UriParserMetadata_MultipleMatchingPropertiesFound("Pen", "TestNS.Person"));
        }

        [Fact]
        public void CaseInsensitivePropertyNameInOrderBy()
        {
            this.TestCaseInsensitive(
                "People?$orderby=Name",
                "People?$orderby=nAmE",
                (parser) => parser.ParseOrderBy(),
                (clause) => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PersonNameProp),
                Strings.MetadataBinder_PropertyNotDeclared("TestNS.Person", "nAmE"));
        }

        [Fact]
        public void CaseInsensitivePropertyNonexistNameInOrderBy()
        {
            this.TestCaseInsensitiveNotExist(
                "People?$orderby=Nam2e",
                (parser) => parser.ParseOrderBy(),
                Strings.MetadataBinder_PropertyNotDeclared("TestNS.Person", "Nam2e"));
        }

        [Fact]
        public void CaseInsensitivePropertyNameConflictsInOrderBy()
        {
            this.TestCaseInsensitiveConflict(
                "People?$orderby=pen",
                parser => parser.ParseOrderBy(),
                clause => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PersonPen),
                "More than one properties match the name 'pen' were found in type 'TestNS.Person'.");
        }

        [Fact]
        public void CaseInsensitivePropertyNameForComplexTypeInFilter()
        {
            this.TestCaseInsensitive(
                "People?$filter=startswith(Addr/ZipCode,'2')",
                "People?$filter=startswith(ADDR/zipCODE,'2')",
                parser => parser.ParseFilter(), filter =>
                {
                    IList<QueryNode> parameters = filter.Expression
                       .ShouldBeSingleValueFunctionCallQueryNode("startswith", EdmCoreModel.Instance.GetBoolean(false))
                       .And.Parameters.ToList();
                    parameters.Count.Should().Be(2);
                    parameters[0].ShouldBeSingleValuePropertyAccessQueryNode(ZipCodeProperty);
                    parameters[1].ShouldBeConstantQueryNode("2");
                },
                Strings.MetadataBinder_PropertyNotDeclared("TestNS.Person", "ADDR"));
        }
        #endregion

        #region bound operation
        [Fact]
        public void CaseInsensitiveBoundOperationNameInPath()
        {
            this.TestCaseInsensitive(
               "People(1)/TestNS.FindPencil",
               "People(1)/TestNS.FindPENCIL",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeOperationSegment(FindPencil1P),
               Strings.RequestUriProcessor_ResourceNotFound("TestNS.FindPENCIL"));
        }

        [Fact]
        public void CaseInsensitiveBoundOperationNameConflictsInPath()
        {
            this.TestCaseInsensitiveConflict(
               "People(1)/TestNS.FindPencilsCon",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeOperationSegment(FindPencilsCon),
               Strings.FunctionOverloadResolver_NoSingleMatchFound("TestNS.FindPencilsCon", ""));
        }

        [Fact]
        public void CaseInsensitiveNonExistBoundOperationInPath()
        {
            this.TestCaseInsensitiveNotExist(
               "People(1)/TestNS.FindPencilEx",
               parser => parser.ParsePath(),
               Strings.RequestUriProcessor_ResourceNotFound("TestNS.FindPencilEx"));
        }

        [Fact]
        public void CaseInsensitiveBoundOperationWithParameterNameInPath()
        {
            this.TestCaseInsensitive(
               "People(1)/TestNS.FindPencil(pid=5)",
               "People(1)/TestNS.FindPENCIL(piD=5)",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeOperationSegment(FindPencil2P),
               Strings.RequestUriProcessor_ResourceNotFound("TestNS.FindPENCIL"));
        }

        [Fact]
        public void CaseInsensitiveBoundOperationWithParameterNameConflictsInPath()
        {
            this.TestCaseInsensitiveConflict(
               "People(1)/TestNS.FindPencilCon(pid=5, PID=6)",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeOperationSegment(FindPencilCon),
               "More than one parameters match the name 'pid' were found.");
        }

        [Fact]
        public void CaseInsensitiveBoundOperationWithParameterNameNonExistInPath()
        {
            this.TestCaseInsensitiveNotExist(
                "People(1)/TestNS.FindPencil(piT=5)",
                parser => parser.ParsePath(),
                Strings.RequestUriProcessor_ResourceNotFound("TestNS.FindPencil"));
        }

        [Fact]
        public void CaseInsensitiveBoundOperationWithParameterNameInOrderBy()
        {
            this.TestCaseInsensitive(
               "People?$orderby=TestNS.FindPencil(pid=5)/Id",
               "People?$orderby=TestNS.FindPencil(PID=5)/Id",
               parser => parser.ParseOrderBy(),
               clause => clause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(PencilId),
               Strings.MetadataBinder_UnknownFunction("TestNS.FindPencil"));
        }
        #endregion

        #region operation imports
        [Fact]
        public void CaseInsensitiveOperationImportName()
        {
            this.TestCaseInsensitive(
               "Feed",
               "FEED",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeOperationImportSegment(FeedImport),
               Strings.RequestUriProcessor_ResourceNotFound("FEED"));
        }

        [Fact]
        public void CaseInsensitiveOperationImportNameConflicts()
        {
            this.TestCaseInsensitiveConflict(
               "FeedCon",
               parser => parser.ParsePath(),
               path => path.LastSegment.ShouldBeOperationImportSegment(FeedConImport),
               Strings.FunctionOverloadResolver_MultipleActionImportOverloads("FeedCon"));
        }

        [Fact]
        public void CaseInsensitiveNonExistOperationImport()
        {
            this.TestCaseInsensitiveNotExist(
               "FeedSheep",
               parser => parser.ParsePath(),
               Strings.RequestUriProcessor_ResourceNotFound("FeedSheep"));
        }
        #endregion

        #region help methods
        private void TestCaseInsensitive<TResult>(string originalStr, string caseInsensitiveStr, Func<ODataUriParser, TResult> parse, Action<TResult> verify, string errorMessage)
        {
            TestUriParserExtension(originalStr, caseInsensitiveStr, parse, verify, errorMessage, Model, parser => parser.Resolver = new ODataUriResolver() { EnableCaseInsensitive = true });
        }

        private void TestCaseInsensitiveConflict<TResult>(string originalStr, Func<ODataUriParser, TResult> parse, Action<TResult> verify, string conflictMessage)
        {
            this.TestConflict(originalStr, parse, verify, conflictMessage, Model, new ODataUriResolver() { EnableCaseInsensitive = true });
        }

        private void TestCaseInsensitiveNotExist<TResult>(string originalStr, Func<ODataUriParser, TResult> parse, string message)
        {
            this.TestNotExist(originalStr, parse, message, Model, parser => parser.Resolver = new ODataUriResolver() { EnableCaseInsensitive = true });
        }
        #endregion
    }
}
