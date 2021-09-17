//---------------------------------------------------------------------
// <copyright file="SelectExpandFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.Tests.UriParser.Binders;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;
using Microsoft.OData.UriParser.Aggregation;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// URI Parser functional tests for V4 $select and $expand.
    /// </summary>
    public class SelectExpandFunctionalTests
    {
        #region $select with no $expand
        [Fact]
        public void SelectSingleDeclaredPropertySucceeds()
        {
            var result = RunParseSelectExpandAndAssertPaths(
                "Name",
                null,
                "Name",
                null,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet());

            result.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp())));
            Assert.False(result.AllSelected);
        }

        [Fact]
        public void SelectWithEmptyStringMeansEverything()
        {
            var result = RunParseSelectExpandAndAssertPaths(
                "",
                null,
                "",
                null,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet());

            Assert.Empty(result.SelectedItems);
            Assert.True(result.AllSelected);
        }

        [Fact]
        public void SelectPropertiesWithRefOperationThrows()
        {
            Action readResult = () => RunParseSelectExpand("MyLions/$ref", null, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            readResult.Throws<ODataException>(ODataErrorStrings.UriSelectParser_SystemTokenInSelectExpand("$ref", "MyLions/$ref"));
        }

        [Fact]
        public void SelectPropertiesWithDollarCountOperationThrows()
        {
            Action readResult = () => RunParseSelectExpand("MyLions/$count", null, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            readResult.Throws<ODataException>(ODataErrorStrings.ExpressionToken_DollarCountNotAllowedInSelect);
        }

        [Fact]
        public void SelectWithAsteriskMeansWildcard()
        {
            ParseSingleSelectForPerson("*").ShouldBeWildcardSelectionItem();
        }

        [Fact]
        public void WildcardPreemptsAllStructuralProperties()
        {
            var results = RunParseSelectExpand("Name, *, MyAddress", null, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            Assert.False(results.AllSelected);
            results.SelectedItems.Single().ShouldBeWildcardSelectionItem();

            AssertSelectString("*", results);
        }

        [Fact]
        public void SelectEnumStructuralProperty()
        {
            var result = RunParseSelectExpand("PetColorPattern", null, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            result.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPet2PetColorPatternProperty())));
            Assert.False(result.AllSelected);
        }

        [Fact]
        public void SelectEnumStructuralPropertyWildcard()
        {
            var results = RunParseSelectExpand("PetColorPattern, *", null, HardCodedTestModel.GetPet2Type(), HardCodedTestModel.GetPet2Set());
            Assert.False(results.AllSelected);
            results.SelectedItems.Single().ShouldBeWildcardSelectionItem();
            AssertSelectString("*", results);
        }

        [Fact]
        public void SelectNavigationPropertyWithoutExpandMeansSelectLink()
        {
            ParseSingleSelectForPerson("MyDog").ShouldBePathSelectionItem(new ODataPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null)));
        }

        [Fact]
        public void SelectComplexProperty()
        {
            var selectItem = ParseSingleSelectForPerson("MyAddress");
            selectItem.ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp())));
        }

        [Fact]
        public void SelectComplexPropertyWithCast()
        {
            var selectItem = ParseSingleSelectForPerson("MyAddress/Fully.Qualified.Namespace.HomeAddress");
            ODataPathSegment[] segments = new ODataPathSegment[2];
            segments[0] = new PropertySegment(HardCodedTestModel.GetPersonAddressProp());
            segments[1] = new TypeSegment(HardCodedTestModel.GetHomeAddressType(), null);
            selectItem.ShouldBePathSelectionItem(new ODataPath(segments));
        }

        [Fact]
        public void SelectComplexPropertyWithWrongCast()
        {
            Action parse = () => ParseSingleSelectForPerson("MyAddress/Fully.Qualified.Namespace.OpenAddress");
            parse.Throws<ODataException>(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
        }

        [Fact]
        public void SelectComplexCollectionProperty()
        {
            var selectItem = ParseSingleSelectForPerson("PreviousAddresses");
            selectItem.ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp())));
        }

        [Fact]
        public void SelectComplexCollectionPropertyWrongSubProp()
        {
            Action parse = () => ParseSingleSelectForPerson("PreviousAddresses/WrongProp");
            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Address", "WrongProp"));
        }

        [Fact]
        public void SelectComplexCollectionPropertyWithCast()
        {
            var selectItem = ParseSingleSelectForPerson("PreviousAddresses/Fully.Qualified.Namespace.HomeAddress");
            ODataPathSegment[] segments = new ODataPathSegment[2];
            segments[0] = new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp());
            segments[1] = new TypeSegment(HardCodedTestModel.GetHomeAddressType(), null);
            selectItem.ShouldBePathSelectionItem(new ODataPath(segments));
        }

        [Fact]
        public void SelectComplexCollectionPropertyWithWrongCast()
        {
            Action parse = () => ParseSingleSelectForPerson("PreviousAddresses/Fully.Qualified.Namespace.OpenAddress");
            parse.Throws<ODataException>(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
        }

        [Fact]
        public void SelectWithCastProperty()
        {
            SelectExpandClause select = RunParseSelectExpand("Artist/Edm.String", null, HardCodedTestModel.GetPaintingType(), HardCodedTestModel.GetPaintingsSet());
            var item = Assert.Single(select.SelectedItems);

            ODataPathSegment[] segments = new ODataPathSegment[2];
            segments[0] = new PropertySegment(HardCodedTestModel.GetPaintingArtistProp());
            segments[1] = new TypeSegment(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), null);
            item.ShouldBePathSelectionItem(new ODataPath(segments));
        }

        [Fact]
        public void SelectWithCastOpenProperty()
        {
            SelectExpandClause select = RunParseSelectExpand("Assistant/Edm.String", null, HardCodedTestModel.GetPaintingType(), HardCodedTestModel.GetPaintingsSet());
            var item = Assert.Single(select.SelectedItems);

            ODataPathSegment[] segments = new ODataPathSegment[2];
            segments[0] = new DynamicPathSegment("Assistant");
            segments[1] = new TypeSegment(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), null);
            item.ShouldBePathSelectionItem(new ODataPath(segments));
        }

        [Fact]
        public void SelectWithCastOpenComplexProperty()
        {
            SelectExpandClause select = RunParseSelectExpand("Exhibit/Location/Edm.String", null, HardCodedTestModel.GetPaintingType(), HardCodedTestModel.GetPaintingsSet());
            var item = Assert.Single(select.SelectedItems);

            ODataPathSegment[] segments = new ODataPathSegment[3];
            segments[0] = new DynamicPathSegment("Exhibit");
            segments[1] = new DynamicPathSegment("Location");
            segments[2] = new TypeSegment(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), null);
            item.ShouldBePathSelectionItem(new ODataPath(segments));
        }

        [Fact]
        public void SelectActionMeansOperation()
        {
            ParseSingleSelectForDog("Fully.Qualified.Namespace.Walk", "Fully.Qualified.Namespace.Walk").ShouldBePathSelectionItem(new ODataPath(new OperationSegment(new List<IEdmOperation>() { HardCodedTestModel.GetDogWalkAction() }, null)));
        }

        [Fact]
        public void SelectFunctionWithOverloads()
        {
            ParseSingleSelectForPerson("Fully.Qualified.Namespace.HasDog", "Fully.Qualified.Namespace.HasDog").ShouldBePathSelectionItem(new ODataPath(new OperationSegment(HardCodedTestModel.GetAllHasDogFunctionOverloadsForPeople(), null)));
        }

        [Fact]
        public void SelectFunctionWithDerivedOverloads()
        {
            ParseSingleSelectForEmployee("Fully.Qualified.Namespace.HasDog", "Fully.Qualified.Namespace.HasDog").ShouldBePathSelectionItem(new ODataPath(new OperationSegment(HardCodedTestModel.GetHasDogOverloadForEmployee(), null)));
        }

        [Fact]
        public void SelectWorksWithoutEntitySet()
        {
            var item = ParseSingleSelect("Name", HardCodedTestModel.GetPersonType(), null /*entitySet*/);

            item.ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp())));
        }

        [Fact]
        public void MultipleSelectionsWorkWithoutEntitySet()
        {
            const string select = "Name, MyDog";
            var results = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                select,
                null,
                HardCodedTestModel.GetPersonType(),
                null);

            Assert.False(results.AllSelected);
            Assert.Equal(2, results.SelectedItems.Count());
        }

        [Fact]
        public void CallingAFunctionIsNotRecognizedInSelect()
        {
            Action test = () => ParseSingleSelectForPerson("HasDog(inOffice=true)");
            test.Throws<ODataException>(ODataErrorStrings.UriSelectParser_TermIsNotValid("(inOffice=true)"));
        }

        [Fact]
        public void SelectSupportsTypeSegments()
        {
            var item = ParseSingleSelectForPerson("Fully.Qualified.Namespace.Employee/PaintingsInOffice");
            item.ShouldBePathSelectionItem(new ODataPath(
                new TypeSegment(HardCodedTestModel.GetEmployeeType(), null),
                new NavigationPropertySegment(HardCodedTestModel.GetEmployeePaintingsInOfficeNavProp(), null)
            ));
        }

        [Fact]
        public void UnneededTypeSegmentInSelectIsOk()
        {
            var item = ParseSingleSelectForPerson("Fully.Qualified.Namespace.Employee/Name");
            item.ShouldBePathSelectionItem(new ODataPath(
                new TypeSegment(HardCodedTestModel.GetEmployeeType(), null),
                new PropertySegment(HardCodedTestModel.GetPersonNameProp())
            ));
        }

        [Fact]
        public void TypeSegmentForVeryDerivedTypeAndSelectPropertyOfMiddleDerivedType()
        {
            const string select = "Fully.Qualified.Namespace.Manager/WorkEmail";
            var results = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                select,
                null,
                HardCodedTestModel.GetPersonType(),
                null);

            Assert.False(results.AllSelected);
            results.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(
                new TypeSegment(HardCodedTestModel.GetManagerType(), null),
                new PropertySegment(HardCodedTestModel.GetEmployeeWorkEmailProp())
                ));
        }

        [Fact]
        public void SelectNavigationPropertyOnDerivedType()
        {
            var item = ParseSingleSelectForPerson("Fully.Qualified.Namespace.Manager/PaintingsInOffice");

            item.ShouldBePathSelectionItem(new ODataPath(
                new TypeSegment(HardCodedTestModel.GetManagerType(), HardCodedTestModel.GetPeopleSet()),
                new NavigationPropertySegment(HardCodedTestModel.GetEmployeePaintingsInOfficeNavProp(), null)));
        }

        [Fact]
        public void SelectOpenPropertyOnDerivedType()
        {
            const string select = "Fully.Qualified.Namespace.FramedPainting/OpenProp";
            var results = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                select,
                null,
                HardCodedTestModel.GetPaintingType(), null);

            Assert.False(results.AllSelected);
            results.SelectedItems.Single().ShouldBePathSelectionItem(new ODataSelectPath(
                    new TypeSegment(HardCodedTestModel.GetFramedPaintingType(), HardCodedTestModel.GetPaintingsSet()),
                    new DynamicPathSegment("OpenProp")));
        }

        [Fact]
        public void SelectOpenPropertyOnDerivedTypeWhereBaseTypeIsNotOpen()
        {
            var item = ParseSingleSelectForPerson("Fully.Qualified.Namespace.OpenEmployee/OpenProp");

            item.ShouldBePathSelectionItem(new ODataSelectPath(
                    new TypeSegment(HardCodedTestModel.GetOpenEmployeeType(), HardCodedTestModel.GetPeopleSet()),
                    new DynamicPathSegment("OpenProp")));
        }

        [Fact]
        public void SelectFunctionWithOverloadsScopedByTypeSegment()
        {
            ParseSingleSelectForPerson("Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.HasDog", "Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.HasDog").ShouldBePathSelectionItem(new ODataPath(new List<ODataPathSegment>() { new TypeSegment(HardCodedTestModel.GetEmployeeType(), null), new OperationSegment(HardCodedTestModel.GetHasDogOverloadForEmployee(), null) }));
        }

        [Fact]
        public void SelectActionWithOverloads()
        {
            ParseSingleSelectForPerson("Fully.Qualified.Namespace.Move", "Fully.Qualified.Namespace.Move").ShouldBePathSelectionItem(new ODataPath(new OperationSegment(HardCodedTestModel.GetMoveOverloadForPerson(), null)));
        }

        [Fact]
        public void SelectActionWithOverloadsScopedByTypeSegment()
        {
            ParseSingleSelectForPerson("Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Move", "Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Move").ShouldBePathSelectionItem(new ODataPath(new List<ODataPathSegment>() { new TypeSegment(HardCodedTestModel.GetEmployeeType(), null), new OperationSegment(new List<IEdmOperation>() { HardCodedTestModel.GetMoveOverloadForEmployee() }, null) }));
        }

        [Fact]
        public void NamespaceQualifiedActionNameShouldWork()
        {
            ParseSingleSelectForPerson("Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Move").ShouldBePathSelectionItem(new ODataPath(new List<ODataPathSegment>() { new TypeSegment(HardCodedTestModel.GetEmployeeType(), null), new OperationSegment(HardCodedTestModel.GetMoveOverloadForEmployee(), null) }));
        }

        [Fact]
        public void NamespaceQualifiedActionNameShouldWork2()
        {
            var selectItem = ParseSingleSelectForPerson("Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Move", "Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.Move") as PathSelectItem;
            Assert.NotNull(selectItem);
            selectItem.SelectedPath.FirstSegment.ShouldBeTypeSegment(HardCodedTestModel.GetEmployeeType());
            selectItem.SelectedPath.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForEmployee());
        }

        [Fact]
        public void UnqualifiedActionNameOnOpenTypeShouldBeInterpretedAsAnOperation()
        {
            ParseSingleSelectForPainting("Restore").ShouldBePathSelectionItem(new ODataPath(new DynamicPathSegment("Restore")));
        }

        [Fact]
        public void NamespaceQualifiedActionNameOnOpenTypeShouldBeInterpretedAsAnOperation()
        {
            ParseSingleSelectForPainting("Fully.Qualified.Namespace.Restore").ShouldBeSelectedItemOfType<PathSelectItem>().SelectedPath.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetRestoreAction());
        }

        [Fact]
        public void CanSelectSubPropertyOfComplexType()
        {
            const string select = "MyAddress/City";
            var result = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                select,
                null,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet());

            result.SelectedItems.Single().ShouldBePathSelectionItem(new ODataSelectPath(
                    new PropertySegment(HardCodedTestModel.GetPersonAddressProp()),
                    new PropertySegment(HardCodedTestModel.GetAddressCityProperty())));
            Assert.False(result.AllSelected);
        }

        [Fact]
        public void CanSelectSubPropertyOfComplexCollection()
        {
            const string select = "PreviousAddresses/City";
            var result = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                select,
                null,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet());

            result.SelectedItems.Single().ShouldBePathSelectionItem(new ODataSelectPath(
                    new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp()),
                    new PropertySegment(HardCodedTestModel.GetAddressCityProperty())));
            Assert.False(result.AllSelected);
        }

        [Fact]
        public void SelectManyDeclaredPropertiesSucceeds()
        {
            const string select = " Shoe, Birthdate,GeographyPoint,    TimeEmployed, \tPreviousAddresses";
            var result = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                select,
                null,
                HardCodedTestModel.GetPersonType(),
                                              HardCodedTestModel.GetPeopleSet());

            var items = result.SelectedItems.ToArray();
            items[0].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonShoeProp())));
            items[1].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonBirthdateProp())));
            items[2].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonGeographyPointProp())));
            items[3].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonTimeEmployedProp())));
            items[4].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp())));

            Assert.False(result.AllSelected);
        }

        [Fact]
        public void SelectOpenPropertySucceeds()
        {
            const string select = "SomeOpenProperty";
            var result = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                select,
                null,
                HardCodedTestModel.GetPaintingType(),
                HardCodedTestModel.GetPaintingsSet());

            result.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new DynamicPathSegment("SomeOpenProperty")));
            Assert.False(result.AllSelected);
        }

        [Fact]
        public void SelectMissingPropertyFailsOnNotOpenType()
        {
            Action parse = () => RunParseSelectExpand("SomeOpenProperty", null, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            parse.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType(), "SomeOpenProperty"));
        }

        [Fact]
        public void SelectMixedOpenAndDeclaredPropertiesSucceeds()
        {
            const string select = "Artist, SomeOpenProperty";
            var result = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                select,
                null,
                HardCodedTestModel.GetPaintingType(),
                HardCodedTestModel.GetPaintingsSet());

            var items = result.SelectedItems.ToArray();
            items[0].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPaintingArtistProp())));
            items[1].ShouldBePathSelectionItem(new ODataPath(new DynamicPathSegment("SomeOpenProperty")));
            Assert.False(result.AllSelected);
        }

        [Fact]
        public void SelectPropertyThroughNavPropWithoutExpandFails()
        {
            Action parse = () => RunParseSelectExpand("MyDog/Color", null, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            parse.Throws<ODataException>(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
        }

        [Fact]
        public void NonPathExpressionThrowsInSelect()
        {
            Action parseWithExpressionInSelect = () => RunParseSelectExpand("Name eq 'Name'", null, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithExpressionInSelect.Throws<ODataException>(ODataErrorStrings.UriSelectParser_TermIsNotValid("Name eq 'Name'"));
        }

        [Fact]
        public void SelectingNamespaceQualifiedWildcardsShouldWork()
        {
            var item = ParseSingleSelectForPerson("Fully.Qualified.Namespace.*");

            Assert.Equal("Fully.Qualified.Namespace", item.ShouldBeSelectedItemOfType<NamespaceQualifiedWildcardSelectItem>()
                .Namespace);
        }

        [Fact]
        public void NullExpandAndNonExistingSelectThrowsUsefulErrorMessage()
        {
            // regression coverage for: [URIParser] ArgumentNullException instead of Incorrect Type
            Action parseWithNullExpand = () => RunParseSelectExpand("NonExistingProperty", null, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithNullExpand.Throws<ODataException>(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "NonExistingProperty"));
        }

        [Fact]
        public void InvalidPropertyWithDollarSignThrowsUsefulErrorMessage()
        {
            // regression test for: [Fuzz] UriParser NulRefs in Select and Expand
            Action parseInvalidWithDollarSign = () => RunParseSelectExpand("Name$(comma)", null, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseInvalidWithDollarSign.Throws<ODataException>(ODataErrorStrings.UriSelectParser_TermIsNotValid("Name$(comma)"));
        }

        [Fact]
        public void ShouldIgnoreCommaAtEndofSelect()
        {
            const string select = "MyDog,";
            const string expectedSelect = "MyDog";
            var results = RunParseSelectExpandAndAssertPaths(
                select,
                null,
                expectedSelect,
                null,
                HardCodedTestModel.GetPersonType(),
                null);
            var selectItem = Assert.Single(results.SelectedItems);
            selectItem.ShouldBeSelectedItemOfType<PathSelectItem>()
                .SelectedPath.LastSegment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        #endregion

        #region $expand with no $select

        [Fact]
        public void ExpandWithoutSelectShouldDefaultToAllSelections()
        {
            // This helper method always checks that AllSelected.Should().BeTrue() on the resulting SelectExpandClause
            var item = ParseSingleExpandForPerson("MyDog");

            var result = item.ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).
                SelectAndExpand.AllSelected;
            Assert.True(result);
        }

        [Fact]
        public void ExpandedNavPropShouldntShowUpAsNavPropSelectionItemIfSelectIsntAlreadyPopulated()
        {
            var result = RunParseSelectExpand("", "MyDog", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            Assert.True(result.AllSelected);
            Assert.Empty(result.SelectedItems.Where(x => x is PathSelectItem));
        }

        [Fact]
        public void ExpandCannotGoThroughNavigationProperties()
        {
            Action parse = () => RunParseSelectExpand(null, "MyDog/MyPeople", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            parse.Throws<ODataException>(ODataErrorStrings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
        }

        [Fact]
        public void MultipleNestedQueryOptionsMustBeSeparatedBySemiColon()
        {
            Action parseWithNonSemiColonTerminatedQueryOptions = () => RunParseSelectExpand(null, "MyDog($select=Color,$expand=MyPeople)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithNonSemiColonTerminatedQueryOptions.Throws<ODataException>(ODataErrorStrings.UriSelectParser_SystemTokenInSelectExpand("$expand", "Color,$expand=MyPeople"));
        }

        [Fact]
        public void ExpandPropertiesWithRefOperationInNonTopLevel()
        {
            var result = RunParseSelectExpand(null, "MyDog($expand=MyPeople/$ref)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString("MyDog($expand=MyPeople/$ref)", result);
        }

        [Fact]
        public void ExpandNavigationWithNavigationAfterRefOperationThrows()
        {
            const string expandClauseText = "MyDog/$ref/MyPeople";
            Action readResult = () => RunParseSelectExpand(null, expandClauseText, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            readResult.Throws<ODataException>(ODataErrorStrings.ExpressionToken_NoPropAllowedAfterRef);
        }

        [Fact]
        public void ExpandNavigationWithNavigationAfterDollarCountOperationThrows()
        {
            const string expandClauseText = "MyDog/$count/MyPeople";
            Action readResult = () => RunParseSelectExpand(null, expandClauseText, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            readResult.Throws<ODataException>(ODataErrorStrings.ExpressionToken_NoPropAllowedAfterDollarCount);
        }

        [Fact]
        public void ExpandNavigationWithNestedQueryOptionOnRef()
        {
            const string expandWithOrderby = "MyPet2Set/$ref($orderby=PetColorPattern desc)";
            var results = RunParseSelectExpand(null, expandWithOrderby, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            Assert.Single(results.SelectedItems);
            Assert.True(results.AllSelected);

            SelectItem expandItem = results.SelectedItems.Single(x => x.GetType() == typeof(ExpandedReferenceSelectItem));
            var orderbyClause = expandItem.ShouldBeExpansionWithRefFor(HardCodedTestModel.GetPersonMyPet2SetNavProp()).OrderByOption;
            orderbyClause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            Assert.Equal(OrderByDirection.Descending, orderbyClause.Direction);
        }

        [Fact]
        public void LastEmbeddedQueryOptionDoesNotRequireSemiColon()
        {
            var result = RunParseSelectExpand(null, "MyDog($expand=MyPeople)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString("MyDog($expand=MyPeople)", result);
        }

        [Fact]
        public void BasicNestedExpansionsShouldWork()
        {
            var topLeveItem = RunParseSelectExpand(null, "MyDog($expand=MyPeople($expand=MyPaintings))", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            var myDogItem = topLeveItem.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());
            Assert.True(myDogItem.SelectAndExpand.AllSelected);
            var myPeopleItem = myDogItem.SelectAndExpand.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp());
            Assert.True(myPeopleItem.SelectAndExpand.AllSelected);
            var myPaintingsItem = myPeopleItem.SelectAndExpand.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyPaintingsNavProp());
            Assert.True(myPaintingsItem.SelectAndExpand.AllSelected);
            Assert.Empty(myPaintingsItem.SelectAndExpand.SelectedItems);
        }

        [Fact]
        public void MultipleExpansionsShouldWork()
        {
            string expand = "MyDog, MyPaintings, MyFavoritePainting";
            var results = RunParseSelectExpandAndAssertPaths(
                null,
                expand,
                "",
                expand,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet());

            Assert.Equal(3, results.SelectedItems.Count());
            var expansions = results.SelectedItems.ToArray();
            expansions[0].ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());
            expansions[1].ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyPaintingsNavProp());
            expansions[2].ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyFavoritePaintingNavProp());
            Assert.True(results.AllSelected);
        }

        [Fact]
        public void NonPathExpressionThrowsInExpand()
        {
            Action parseWithExpressionInExpand = () => RunParseSelectExpand(null, "Name eq 'Name'", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parseWithExpressionInExpand.Throws<ODataException>(ODataErrorStrings.UriSelectParser_TermIsNotValid("Name eq 'Name'"));
        }

        [Fact]
        public void MultipleExpandsOnTheSamePropertyAreCollapsed()
        {
            var results = RunParseSelectExpand(null, "MyDog, MyDog($expand=MyPeople)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            AssertSelectString("", results);
            AssertExpandString("MyDog($expand=MyPeople)", results);

            var items = results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp())
                   .SelectAndExpand.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp())
                   .SelectAndExpand.SelectedItems;
            Assert.Empty(items);
        }

        [Fact]
        public void ExpandWorksWithoutAnEntitySet()
        {
            var item = ParseSingleExpand("MyDog", HardCodedTestModel.GetPersonType(), null);

            item.ShouldBeExpansionFor(new ODataExpandPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), null /*entitySet*/)));
        }

        [Fact]
        public void ExpandNavigationPropertyOnDerivedType()
        {
            var item = ParseSingleExpandForPerson("Fully.Qualified.Namespace.Manager/PaintingsInOffice");

            item.ShouldBeExpansionFor(new ODataExpandPath(
                new TypeSegment(HardCodedTestModel.GetManagerType(), HardCodedTestModel.GetPeopleSet()),
                new NavigationPropertySegment(HardCodedTestModel.GetEmployeePaintingsInOfficeNavProp(), HardCodedTestModel.GetPaintingsSet())
                ));
        }

        [Fact]
        public void DeepExpandShouldBeMerged()
        {
            const string expand = "MyDog($expand=MyPeople($expand=MyDog($expand=MyPeople($expand=MyPaintings)))), MyDog($expand=MyPeople($expand=MyDog($expand=MyPeople)))";
            const string expectedExpand = "MyDog($expand=MyPeople($expand=MyDog($expand=MyPeople($expand=MyPaintings))))";
            var results = RunParseSelectExpand(null, expand, HardCodedTestModel.GetPersonType(), null);

            Assert.Single(results.SelectedItems);
            Assert.True(results.AllSelected);
            AssertExpandString(expectedExpand, results);
        }

        [Fact]
        public void ExpandWithEnumSelect()
        {
            const string expand = "MyPeople($expand=MyPet2Set($select=PetColorPattern,Color))"; // test 'PetColorPattern' which is enum type property
            const string expectedExpand = "MyPeople($expand=MyPet2Set($select=PetColorPattern,Color))";
            var results = RunParseSelectExpand(null, expand, HardCodedTestModel.GetDogType(), null);

            Assert.Single(results.SelectedItems);
            Assert.True(results.AllSelected);
            AssertExpandString(expectedExpand, results);
        }

        [Fact]
        public void ParseEnumPropertyOrderByWithinExpand()
        {
            const string expandWithOrderby = "MyPet2Set($orderby=PetColorPattern desc)";
            var results = RunParseSelectExpand(null, expandWithOrderby, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            Assert.Single(results.SelectedItems);
            Assert.True(results.AllSelected);

            SelectItem expandItem = results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem);
            var orderbyClause = expandItem.ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyPet2SetNavProp()).OrderByOption;
            orderbyClause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPet2PetColorPatternProperty());
            Assert.Equal(OrderByDirection.Descending, orderbyClause.Direction);
        }

        [Fact]
        public void RepeatedExpandWithTypeSegmentsShouldBeMerged()
        {
            const string expand = "Fully.Qualified.Namespace.Manager/DirectReports, Fully.Qualified.Namespace.Manager/DirectReports";
            var results = RunParseSelectExpand(null, expand, HardCodedTestModel.GetPersonType(), null);

            Assert.Single(results.SelectedItems);
            Assert.True(results.AllSelected);
            AssertExpandString("Fully.Qualified.Namespace.Manager/DirectReports", results);
        }

        [Fact]
        public void ExpandDifferentNavigationsOnSameTypeShouldNotBeMerged()
        {
            const string expand = "Fully.Qualified.Namespace.Manager/PaintingsInOffice, Fully.Qualified.Namespace.Manager/DirectReports";
            var results = RunParseSelectExpandAndAssertPaths(
                null,
                expand,
                "",
                "Fully.Qualified.Namespace.Manager/PaintingsInOffice, Fully.Qualified.Namespace.Manager/DirectReports",
                HardCodedTestModel.GetPersonType(),
                null);

            Assert.Equal(2, results.SelectedItems.Count());
            Assert.True(results.AllSelected);
        }

        [Fact]
        public void DeepExpandWithDifferentTypeSegmentsShouldNotBeMerged()
        {
            const string expand = "Fully.Qualified.Namespace.Manager/DirectReports,   Fully.Qualified.Namespace.Employee/Manager";
            var results = RunParseSelectExpand(null, expand, HardCodedTestModel.GetPersonType(), null);

            const string expectedExpand = "Fully.Qualified.Namespace.Manager/DirectReports,   Fully.Qualified.Namespace.Employee/Manager";
            AssertExpandString(expectedExpand, results);
        }

        [Fact]
        public void ShouldIgnoreCommaAtEndofExpand()
        {
            const string expectedExpand = "MyDog";
            var results = RunParseSelectExpandAndAssertPaths(
                null,
                "MyDog,",
                "",
                expectedExpand,
                HardCodedTestModel.GetPersonType(),
                null);

            var item = Assert.Single(results.SelectedItems);
            var expandedItem = Assert.IsType<ExpandedNavigationSelectItem>(item);
            expandedItem.PathToNavigationProperty.LastSegment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [Fact]
        public void MaxExpandDepthSettingShouldBeEnforced()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host/"), new Uri("http://host/People?$expand=MyDog($expand=MyPeople;)"));
            parser.Settings.MaximumExpansionDepth = 1;
            Action parse = () => parser.ParseSelectAndExpand();
            parse.Throws<ODataException>(ODataErrorStrings.UriParser_ExpandDepthExceeded(2, 1));
        }

        [Fact]
        public void MaxExpandCountSettingShouldBeEnforced()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://host/"), new Uri("http://host/People?$expand=MyDog,MyLions"));
            parser.Settings.MaximumExpansionCount = 1;
            Action parse = () => parser.ParseSelectAndExpand();
            parse.Throws<ODataException>(ODataErrorStrings.UriParser_ExpandCountExceeded(2, 1));
        }

        #endregion

        #region Interesting $expand with other options scenarios

        [Fact]
        public void SelectPropertyThroughNavPropWithExpandFails()
        {
            Action parse = () => RunParseSelectExpand("MyPeople/Name", "MyPeople", HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());

            parse.Throws<ODataException>(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
        }

        [Fact]
        public void NestedSelectPropertyWithJustNavPropAtParentLevelMeansJustOnePropertyAtInnerLevel()
        {
            var results = RunParseSelectExpand("MyPeople", "MyPeople($select=Name)", HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());

            Assert.False(results.AllSelected);
            var myPeople = results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp()).SelectAndExpand;
            myPeople.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp())));
            Assert.False(myPeople.AllSelected);

            AssertSelectString("MyPeople", results);
            AssertExpandString("MyPeople($select=Name)", results);
        }

        [Fact]
        public void NestedSelectPropertyWithNothingSelectedAtParentLevelMeansAllAtTopLevelAndJustOnePropertyAtInnerLevel()
        {
            var results = RunParseSelectExpand(null, "MyPeople($select=Name)", HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());

            Assert.True(results.AllSelected);
            var myPeople = results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp()).SelectAndExpand;
            myPeople.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp())));
            Assert.False(myPeople.AllSelected);

            AssertSelectString("", results);
            AssertExpandString("MyPeople($select=Name)", results);
        }

        [Fact]
        public void ExpandsDoNotHaveToAppearInSelectToBeSelected()
        {
            var results = RunParseSelectExpand("MyAddress", "MyDog, MyFavoritePainting", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            Assert.Equal(3, results.SelectedItems.Count());
            Assert.Equal(2, results.SelectedItems.OfType<ExpandedNavigationSelectItem>().Count());
            Assert.True(results.SelectedItems.OfType<ExpandedNavigationSelectItem>().ElementAt(0).SelectAndExpand.AllSelected);
            Assert.True(results.SelectedItems.OfType<ExpandedNavigationSelectItem>().ElementAt(1).SelectAndExpand.AllSelected);
            Assert.False(results.AllSelected);

            AssertSelectString("MyAddress", results);
            AssertExpandString("MyDog,MyFavoritePainting", results);
        }

        [Fact]
        public void SomeExpandedNavPropsCanAppearInSelectAndAreRetainedAsNavPropLinks()
        {
            var results = RunParseSelectExpand("MyAddress, MyDog", "MyDog, MyFavoritePainting", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            Assert.Equal(4, results.SelectedItems.Count());
            Assert.Equal(2, results.SelectedItems.OfType<ExpandedNavigationSelectItem>().Count());
            Assert.True(results.SelectedItems.OfType<ExpandedNavigationSelectItem>().ElementAt(0).SelectAndExpand.AllSelected);
            Assert.True(results.SelectedItems.OfType<ExpandedNavigationSelectItem>().ElementAt(1).SelectAndExpand.AllSelected);
            results.SelectedItems.OfType<PathSelectItem>().ElementAt(0).ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp())));
            results.SelectedItems.OfType<PathSelectItem>().ElementAt(1).ShouldBePathSelectionItem(new ODataPath(new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetPeopleSet())));
            Assert.False(results.AllSelected);

            AssertSelectString("MyAddress,MyDog", results);
            AssertExpandString("MyDog,MyFavoritePainting", results);
        }

        [Fact]
        public void ExpandOnDerivedTypeWorks()
        {
            var results = RunParseSelectExpand("FirstName, MyAddress", "Fully.Qualified.Namespace.Employee/OfficeDog", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            results.SelectedItems.OfType<PathSelectItem>().ElementAt(0).ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonFirstNameProp())));
            results.SelectedItems.OfType<PathSelectItem>().ElementAt(1).ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp())));
            var expand = Assert.Single(results.SelectedItems.OfType<ExpandedNavigationSelectItem>());
            Assert.True(expand.SelectAndExpand.AllSelected);

            AssertSelectString("FirstName,MyAddress", results);
            AssertExpandString("Fully.Qualified.Namespace.Employee/OfficeDog", results);
        }

        [Fact]
        public void ExpandOnDerivedWithSelectTypeWorks()
        {
            var results = RunParseSelectExpand("FirstName, MyAddress", "Fully.Qualified.Namespace.Employee/OfficeDog($select=Color)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            results.SelectedItems.OfType<PathSelectItem>().ElementAt(0).ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonFirstNameProp())));
            results.SelectedItems.OfType<PathSelectItem>().ElementAt(1).ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp())));
            var expand = Assert.Single(results.SelectedItems.OfType<ExpandedNavigationSelectItem>());
            Assert.False(expand.SelectAndExpand.AllSelected);
            expand.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetDogColorProp())));
            Assert.False(results.AllSelected);

            AssertSelectString("FirstName,MyAddress", results);
            AssertExpandString("Fully.Qualified.Namespace.Employee/OfficeDog($select=Color)", results);
        }

        [Fact]
        public void MultipleDeepLevelExpansionsAndSelectionsShouldWork()
        {
            const string select = "MyDog, MyFavoritePainting";
            const string expand = "MyDog($expand=MyPeople($select=Name)), MyFavoritePainting($select=Artist)";
            const string expectedExpand = "MyDog($expand=MyPeople($select=Name)),MyFavoritePainting($select=Artist)";

            var results = RunParseSelectExpandAndAssertPaths(select,
                                                             expand,
                                                             select,
                                                             expectedExpand,
                                                              HardCodedTestModel.GetPersonType(),
                                                              HardCodedTestModel.GetPeopleSet());

            var items = results.SelectedItems.ToList();
            Assert.Equal(4, items.Count());
            Assert.False(results.AllSelected);

            SelectExpandClause myDog = items[0].ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).SelectAndExpand;
            myDog.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetDogMyPeopleNavProp())
                    .SelectAndExpand.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp())));
            Assert.True(myDog.AllSelected);

            SelectExpandClause myFavoritePainting = items[1].ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyFavoritePaintingNavProp()).SelectAndExpand;
            myFavoritePainting.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetPaintingArtistProp())));
            Assert.False(myFavoritePainting.AllSelected);
        }

        [Fact]
        public void SimpleExpandAndOnlySelectIt()
        {
            const string selectedExpand = "MyDog";
            var results = RunParseSelectExpandAndAssertPaths(
                selectedExpand,
                selectedExpand,
                selectedExpand,
                selectedExpand,
                HardCodedTestModel.GetPersonType(),
                HardCodedTestModel.GetPeopleSet());

            Assert.False(results.AllSelected);
            Assert.True(results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).
                SelectAndExpand.AllSelected);
        }

        [Fact]
        public void ExpandSupportsTypeSegments()
        {
            // PaintingsInOffice is defined on Employee, not Person
            var paintingsInOffice = ParseSingleExpandForPerson("Fully.Qualified.Namespace.Employee/PaintingsInOffice");

            paintingsInOffice.ShouldBeExpansionFor(new ODataExpandPath(
                new TypeSegment(HardCodedTestModel.GetEmployeeType(), HardCodedTestModel.GetPeopleSet()),
                new NavigationPropertySegment(HardCodedTestModel.GetEmployeePaintingsInOfficeNavProp(), HardCodedTestModel.GetPaintingsSet())
            ));
        }

        [Fact]
        public void UnneededTypeSegmentOnSelectButNotExpandIsIgnored()
        {
            var result = RunParseSelectExpand("Fully.Qualified.Namespace.Employee/MyDog", "MyDog", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            result.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(new ODataExpandPath(
                new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetDogsSet())
                ));
        }

        [Fact]
        public void UnneededTypeOnExpandButNotSelectIsKept()
        {
            var result = RunParseSelectExpand("MyDog", "Fully.Qualified.Namespace.Employee/MyDog", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            result.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(new ODataExpandPath(
                new TypeSegment(HardCodedTestModel.GetEmployeeType(), HardCodedTestModel.GetPeopleSet()),
                new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetDogsSet())
                ));
        }

        [Fact]
        public void SelectAndExpandWithDifferentTypesWorks()
        {
            var result = RunParseSelectExpand("Fully.Qualified.Namespace.Employee/MyDog", "Fully.Qualified.Namespace.Manager/MyDog", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            Assert.Equal(2, result.SelectedItems.Count());
            result.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp());
            result.SelectedItems.Single(x => x is PathSelectItem).ShouldBePathSelectionItem(new ODataPath(new TypeSegment(HardCodedTestModel.GetEmployeeType(), HardCodedTestModel.GetPeopleSet()), new NavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp(), HardCodedTestModel.GetPeopleSet())));
        }

        [Fact]
        public void ExpandSamePropertyOnTwoDifferentTypesWithoutASelectExpandsNavPropOnBothTypes()
        {
            const string selectAndExpand = "Fully.Qualified.Namespace.Employee/MyDog, Fully.Qualified.Namespace.Manager/MyDog";
            var results = RunParseSelectExpandAndAssertPaths(
                null,
                selectAndExpand,
                "",
                selectAndExpand,
                HardCodedTestModel.GetPersonType(),
                null);
            Assert.True(results.AllSelected);
        }

        [Fact]
        public void WildCardOnExpandedNavigationProperty()
        {
            const string select = "MyPaintings";
            const string expand = "MyPaintings($select=*)";
            var results = RunParseSelectExpandAndAssertPaths(
                select,
                expand,
                select,
                expand,
                HardCodedTestModel.GetPersonType(), null);

            Assert.False(results.AllSelected);
            var myPaintings = results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyPaintingsNavProp()).SelectAndExpand;
            var item = Assert.Single(myPaintings.SelectedItems);
            Assert.IsType<WildcardSelectItem>(item);
            Assert.False(myPaintings.AllSelected);
        }

        [Fact]
        public void WildCardOnExpandedNavigationPropertyAfterTypeSegment()
        {
            const string select = "Fully.Qualified.Namespace.Manager/MyPaintings";
            const string expand = "Fully.Qualified.Namespace.Manager/MyPaintings($select=*)";
            var results = RunParseSelectExpandAndAssertPaths(
                select,
                expand,
                select,
                expand,
                HardCodedTestModel.GetPersonType(),
                null);

            Assert.False(results.AllSelected);
            results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(new ODataExpandPath(
                new TypeSegment(HardCodedTestModel.GetManagerType(), HardCodedTestModel.GetPeopleSet()),
                new NavigationPropertySegment(HardCodedTestModel.GetPersonMyPaintingsNavProp(), HardCodedTestModel.GetPaintingsSet())));
        }

        [Fact]
        public void WildCardOnExpandedNavigationPropertyOnDerivedType()
        {
            const string select = "Fully.Qualified.Namespace.Manager/PaintingsInOffice";
            const string expand = "Fully.Qualified.Namespace.Manager/PaintingsInOffice($select=*)";
            var results = RunParseSelectExpandAndAssertPaths(
                select,
                expand,
                select,
                expand,
                HardCodedTestModel.GetPersonType(),
                null);

            Assert.False(results.AllSelected);
            results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(new ODataExpandPath(
                new TypeSegment(HardCodedTestModel.GetManagerType(), HardCodedTestModel.GetPeopleSet()),
                new NavigationPropertySegment(HardCodedTestModel.GetEmployeePaintingsInOfficeNavProp(), HardCodedTestModel.GetPaintingsSet())));
        }

        [Fact]
        public void ExpandSyntacticErrorMessageSpecifiesExpandAsWellAsSelect()
        {
            // regression coverage for: [UriParser] Error message wrong when term not valid in expand part of select expand
            // regression coverage for: [URIParser] Change UriSelectParser_TermIsNotValid error message for expand
            Action createWithExpandSyntaxError = () => RunParseSelectExpand(null, "Microsoft.Test.Taupo.OData.WCFService.Customer/Orders('id')", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            createWithExpandSyntaxError.Throws<ODataException>(Strings.UriSelectParser_TermIsNotValid("('id')"));
        }

        [Fact]
        public void MixOfSelectionTypesShouldWork()
        {
            const string select = "Name,Birthdate,MyAddress,Fully.Qualified.Namespace.*,MyLions";
            const string expand = "MyDog";
            var results = RunParseSelectExpandAndAssertPaths(
                select,
                expand,
                select,
                expand,
                HardCodedTestModel.GetPersonType(),
                null);

            Assert.Equal(6, results.SelectedItems.Count());
            Assert.False(results.AllSelected);
        }

        [Fact]
        public void SelectingANavPropIsNotRecursiveAllSelection()
        {
            // In V3 Selecting MyDog meant that everything below it is recursively selected (AllSelection = true).
            // In V4 There is no recursive all selection.
            var results = RunParseSelectExpand("MyDog", "MyDog($expand=MyPeople($select=*))", HardCodedTestModel.GetPersonType(), null);

            Assert.Equal(2, results.SelectedItems.Count());
            Assert.False(results.AllSelected);

            AssertSelectString("MyDog", results);
            AssertExpandString("MyDog($expand=MyPeople($select=*))", results);

            var clauseForMyDog = results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyDogNavProp()).SelectAndExpand;
            Assert.Single(clauseForMyDog.SelectedItems);
            Assert.True(clauseForMyDog.AllSelected);
            var clauseForMyPeople = clauseForMyDog.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>().SelectAndExpand;
            clauseForMyPeople.SelectedItems.Single().ShouldBeSelectedItemOfType<WildcardSelectItem>();
            Assert.False(clauseForMyPeople.AllSelected);
        }

        [Fact]
        public void SelectOnComplexTypeWorks()
        {
            var results = RunParseSelectExpand("City", null, HardCodedTestModel.GetAddressType(), null);
            var item = Assert.Single(results.SelectedItems);
            item.ShouldBeSelectedItemOfType<PathSelectItem>()
                .SelectedPath.Single().ShouldBePropertySegment(HardCodedTestModel.GetAddressCityProperty());
        }

        [Fact]
        public void SelectOnEnumTypeWorks()
        {
            var results = RunParseSelectExpand("PetColorPattern", null, HardCodedTestModel.GetPet2Type(), null);
            var item = Assert.Single(results.SelectedItems);
            item.ShouldBeSelectedItemOfType<PathSelectItem>()
                .SelectedPath.Single().ShouldBePropertySegment(HardCodedTestModel.GetPet2PetColorPatternProperty());
        }

        //[Fact(Skip = "#622: support NavProps in complex types, make sure we can select and expand them.")]
        //public void ExpandOnComplexTypeWorks()
        //{
        //    var results = RunParseSelectExpand(null, "MyFavoriteNeighbor", HardCodedTestModel.GetAddressType(), null);
        //    results.SelectedItems.Should().HaveCount(2);
        //    results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>()
        //        .And.PathToNavigationProperty.Single().ShouldBeNavigationPropertySegment(HardCodedTestModel.GetAddressMyFavoriteNeighborNavProp());
        //}

        //[Fact(Skip = "#622: support NavProps in complex types, make sure we can select and expand them.")]
        //public void SelectAndExpandWorkOnComplexTypes()
        //{
        //    var results = RunParseSelectExpand("City", "MyFavoriteNeighbor($select=Name)", HardCodedTestModel.GetAddressType(), null);
        //    results.SelectedItems.Should().HaveCount(2);
        //    results.SelectedItems.First().ShouldBeSelectedItemOfType<PathSelectItem>()
        //        .And.SelectedPath.First().ShouldBePropertySegment(HardCodedTestModel.GetAddressCityProperty());
        //    var myNeighborsExpansion = results.SelectedItems.Last().ShouldBeSelectedItemOfType<ExpandedNavigationSelectItem>().And;
        //    myNeighborsExpansion.PathToNavigationProperty.Single().ShouldBePropertySegment(HardCodedTestModel.GetAddressMyNeighborsProperty());
        //    myNeighborsExpansion.SelectAndExpand.SelectedItems.Single().ShouldBeSelectedItemOfType<PathSelectItem>()
        //        .And.SelectedPath.Single().ShouldBePropertySegment(HardCodedTestModel.GetPersonNameProp());
        //}

        //[Fact(Skip = "#622: Get nested $filter working")]
        //public void BasicNestedFilterClauseWorks()
        //{
        //    const string expectedSelect = "MyPaintings";
        //    const string expand = "MyPaintings($filter=true)";
        //    var results = RunParseSelectExpandAndAssertPaths(
        //        null,
        //        expand,
        //        expectedSelect,
        //        expand,
        //        HardCodedTestModel.GetPersonType(),
        //        HardCodedTestModel.GetPeopleSet());
        //    var filterClause = results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyPaintingsNavProp()).And.FilterOption;
        //    filterClause.ItemType.FullName().Should().Be(HardCodedTestModel.GetPaintingType().FullName());
        //    filterClause.RangeVariable.Kind.Should().Be(RangeVariableKind.Resource);
        //    filterClause.RangeVariable.Name.Should().Be("$it");
        //    filterClause.Expression.ShouldBeConstantQueryNode(true);
        //}

        //[Fact(Skip = "#622: Get nested $orderby working.")]
        //public void BasicNestedOrderbyClauseWorks()
        //{
        //    const string expectedSelect = "MyPaintings";
        //    const string expand = "MyPaintings($orderby=Value)";
        //    var results = RunParseSelectExpandAndAssertPaths(
        //        null,
        //        expand,
        //        expectedSelect,
        //        expand,
        //        HardCodedTestModel.GetPersonType(),
        //        HardCodedTestModel.GetPeopleSet());
        //    var orderbyClause = results.SelectedItems.Single(x => x is ExpandedNavigationSelectItem).ShouldBeExpansionFor(HardCodedTestModel.GetPersonMyPaintingsNavProp()).And.OrderByOption;
        //    orderbyClause.ItemType.FullName().Should().Be(HardCodedTestModel.GetPaintingType().FullName());
        //    orderbyClause.RangeVariable.Kind.Should().Be(RangeVariableKind.Resource);
        //    orderbyClause.RangeVariable.Name.Should().Be("$it");
        //    orderbyClause.Expression.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPaintingValueProperty());
        //}

        [Fact]
        public void NestedOptionsWithoutClosingParenthesisThrows()
        {
            Action parse = () => RunParseSelectExpand(null, "MyPaintings($filter=true", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPaintingsSet());
            parse.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_UnbalancedBracketExpression);
        }

        [Fact]
        public void ShouldBeAbleToSelectOnANonEntityType()
        {
            var results = RunParseSelectExpand("City", null, HardCodedTestModel.GetAddressType(), null);
            results.SelectedItems.Single().ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty())));
        }

        [Fact]
        public void MultipleSelectsOnTheSameExpandItem()
        {
            var results = RunParseSelectExpand("", "MyDog($select=Color,Breed)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString("MyDog($select=Color,Breed)", results);
        }

        [Fact]
        public void RedundantExpandsWithUniqueSelectsArePropertyCollapsed()
        {
            var results = RunParseSelectExpand("", "MyDog($select=Color), MyDog($select=Breed)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString("MyDog($select=Breed,Color)", results);
        }

        [Fact]
        public void TypeSegmentsWorkOnSubExpands()
        {
            var results = RunParseSelectExpand("", "MyPeople($select=Fully.Qualified.Namespace.Employee/Name)", HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            AssertExpandString("MyPeople($select=Fully.Qualified.Namespace.Employee/Name)", results);
        }

        [Fact]
        public void ExplicitNavPropIsNotAddedIfNeededAtDeeperLevels()
        {
            const string expandClauseText = "MyDog($select=Color;$expand=MyPeople)";
            const string selectClauseText = "";
            var results = RunParseSelectExpand(selectClauseText, expandClauseText, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertSelectString(selectClauseText, results);
            AssertExpandString(expandClauseText, results);
        }

        [Fact]
        public void DuplicatePropertiesWithNoOptionsReturnsSingle()
        {
            const string expandClauseText = "";
            const string selectClauseText = "Name, Name";
            const string expectedSelect = "Name";

            var results = RunParseSelectExpand(selectClauseText, expandClauseText, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertSelectString(expectedSelect, results);
        }

        [Fact]
        public void SelectAndExpandShouldWorkOnSelectComplexProperties()
        {
            const string expectedExpand = "MyDog($select=Color)";
            const string expectedSelect = "Name,MyAddress/City,MyDog";

            var results = RunParseSelectExpand("Name,MyAddress/City,MyDog", "MyDog($select=Color)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString(expectedExpand, results);
            AssertSelectString(expectedSelect, results);
        }

        [Fact]
        public void SelectAndExpandShouldWorkOnSelectComplexPropertiesWithTypeCast()
        {
            const string expectedExpand = "MyDog($select=Color)";
            const string expectedSelect = "Name,MyAddress/Fully.Qualified.Namespace.HomeAddress/HomeNO,MyDog";

            var results = RunParseSelectExpand("Name,MyAddress/Fully.Qualified.Namespace.HomeAddress/HomeNO,MyDog", "MyDog($select=Color)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString(expectedExpand, results);
            AssertSelectString(expectedSelect, results);
        }

        [Fact]
        public void SelectAndExpandShouldWorkOnSelectComplexPropertiesWithMultipleTypeCasts()
        {
            const string expectedExpand = "MyDog($select=Color)";
            const string expectedSelect = "Name,MyAddress/Fully.Qualified.Namespace.HomeAddress/NextHome/Fully.Qualified.Namespace.HomeAddress/HomeNO,MyDog";

            var results = RunParseSelectExpand("Name,MyAddress/Fully.Qualified.Namespace.HomeAddress/NextHome/Fully.Qualified.Namespace.HomeAddress/HomeNO,MyDog", "MyDog($select=Color)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString(expectedExpand, results);
            AssertSelectString(expectedSelect, results);
        }

        [Fact]
        public void SelectAndExpandShouldWorkOnSelectComplexPropertiesRecursively()
        {
            const string expectedExpand = "MyDog($select=Color)";
            const string expectedSelect = "Name,MyAddress/NextHome/NextHome/City,MyDog";

            var results = RunParseSelectExpand("Name,MyAddress/NextHome/NextHome/City,MyDog", "MyDog($select=Color)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString(expectedExpand, results);
            AssertSelectString(expectedSelect, results);
        }

        [Fact]
        public void SelectAndExpandShouldWorkOnSelectOpenProperty()
        {
            const string expectedExpand = "MyDog($select=Color)";
            const string expectedSelect = "Name,MyOpenAddress/Test,MyDog";

            var results = RunParseSelectExpand("Name,MyOpenAddress/Test,MyDog", "MyDog($select=Color)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            AssertExpandString(expectedExpand, results);
            AssertSelectString(expectedSelect, results);
        }

        [Fact]
        public void DollarItinFilterInsideExpandShouldReferenceQueriedEntity()
        {
            // $it/ID references PersonType since the resource is a People EntitySet.
            IEdmStructuredType expectedRight = (IEdmStructuredType)HardCodedTestModel.GetPersonType();

            // $filter=ID references MyDog whose type is DogType.
            IEdmStructuredType expectedLeft = (IEdmStructuredType)HardCodedTestModel.GetDogType();
            SelectExpandClause clause = RunParseSelectExpand("", "MyDog($filter=ID eq $it/ID)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            IEdmStructuredType right =
                (
                    (
                        (
                            clause.SelectedItems.First() as ExpandedNavigationSelectItem // $expand=MyDog(...)
                        ).FilterOption.Expression as BinaryOperatorNode // $filter=ID eq $it/ID
                    ).Right as SingleValuePropertyAccessNode // $it/ID
                ).Property.DeclaringType;

            IEdmStructuredType left =
                (
                    (
                        (
                            clause.SelectedItems.First() as ExpandedNavigationSelectItem // $expand=MyDog(...)
                        ).FilterOption.Expression as BinaryOperatorNode // $filter=ID eq $it/ID
                    ).Left as SingleValuePropertyAccessNode // $filter=ID
                ).Property.DeclaringType;

            Assert.Equal(expectedRight, right);
            Assert.Equal(expectedLeft, left);
        }

        [Fact]
        public void DollarItinFilterInsideNestedExpandShouldReferenceResourcePathEntity()
        {
            // $it/ID references PersonType since the resource is a People EntitySet.
            IEdmStructuredType expectedRight = (IEdmStructuredType)HardCodedTestModel.GetPersonType();

            // $filter=ID1 references LionsISaw whose type in LionType.
            IEdmStructuredType expectedLeft = (IEdmStructuredType)HardCodedTestModel.GetLionType();
            SelectExpandClause clause = RunParseSelectExpand("", "MyDog($select=Color;$expand=LionsISaw($filter=ID1 eq $it/ID))", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            IEdmStructuredType right =
                (
                    (
                        (
                            (
                                clause.SelectedItems.First() as ExpandedNavigationSelectItem // $expand=MyDog(...)
                            ).SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem // $Select=Color;$expand=LionISaw(...)
                        ).FilterOption.Expression as BinaryOperatorNode // $filter=ID1 eq $it/ID
                    ).Right as SingleValuePropertyAccessNode // $it/ID
                ).Property.DeclaringType;

            IEdmStructuredType left =
                (
                    (
                        (
                            (
                                clause.SelectedItems.First() as ExpandedNavigationSelectItem // $expand=MyDog(...)
                            ).SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem // $Select=Color;$expand=LionISaw(...)
                        ).FilterOption.Expression as BinaryOperatorNode // $filter=ID1 eq $it/ID
                    ).Left as SingleValuePropertyAccessNode // $filter=ID1
                ).Property.DeclaringType;

            Assert.Equal(expectedRight, right);
            Assert.Equal(expectedLeft, left);
        }

        [Fact]
        public void DollarItinFilterInsideMultiNestedExpandShouldReferenceResourcePathEntity()
        {
            // $it/ID references PersonType since the resource is a People EntitySet
            IEdmStructuredType expectedRight = (IEdmStructuredType)HardCodedTestModel.GetPersonType();

            // $filter=ID references MyPaintings whose type in PaintingType.
            IEdmStructuredType expectedLeft = (IEdmStructuredType)HardCodedTestModel.GetPaintingType();
            SelectExpandClause clause = RunParseSelectExpand("", "MyDog($select=Color;$expand=MyPeople($expand=MyPaintings($filter=ID eq $it/ID)))", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            IEdmStructuredType right =
                (
                    (
                        (
                            (
                                (
                                clause.SelectedItems.First() as ExpandedNavigationSelectItem // $expand=MyDog(...)
                                ).SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem // $select=Color;$expand=MyPeople(...)
                            ).SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem // $expand=MyPaintings(...)
                        ).FilterOption.Expression as BinaryOperatorNode // $filter=ID eq $it/ID
                    ).Right as SingleValuePropertyAccessNode // $it/ID
                ).Property.DeclaringType;

            IEdmStructuredType left =
                (
                    (
                        (
                            (
                                (
                                clause.SelectedItems.First() as ExpandedNavigationSelectItem // $expand=MyDog(...)
                                ).SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem // $select=Color;$expand=MyPeople(...)
                            ).SelectAndExpand.SelectedItems.First() as ExpandedNavigationSelectItem // $expand=MyPaintings(...)
                        ).FilterOption.Expression as BinaryOperatorNode // $filter=ID eq $it/ID
                    ).Left as SingleValuePropertyAccessNode // $filter=ID
                ).Property.DeclaringType;

            Assert.Equal(expectedRight, right);
            Assert.Equal(expectedLeft, left);
        }

        [Fact]
        public void DollarItinFilterInsideSelectShouldReferenceResourcePathEntity()
        {
            // $it/ID references PersonType since the resource is a People EntitySet
            IEdmStructuredType expectedRight = (IEdmStructuredType)HardCodedTestModel.GetPersonType();

            // $filter=Street references PreviousAddresses whose type in AddressType.
            IEdmStructuredType expectedLeft = (IEdmStructuredType)HardCodedTestModel.GetAddressType();

            SelectExpandClause clause = RunParseSelectExpand("PreviousAddresses($filter=Street eq $it/Name)", "", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            IEdmStructuredType right =
                (
                    (
                        (
                            clause.SelectedItems.First() as PathSelectItem // $select=PreviousAddresses(...)
                        ).FilterOption.Expression as BinaryOperatorNode // $filter=Street eq $it/Name
                    ).Right as SingleValuePropertyAccessNode // $it/Name
                ).Property.DeclaringType;

            IEdmStructuredType left =
                (
                    (
                        (
                            clause.SelectedItems.First() as PathSelectItem // $select=PreviousAddresses(...)
                        ).FilterOption.Expression as BinaryOperatorNode // $filter=Street eq $it/Name
                    ).Left as SingleValuePropertyAccessNode // $filter=Street
                ).Property.DeclaringType;

            Assert.Equal(expectedRight, right);
            Assert.Equal(expectedLeft, left);
        }

        [Theory]
        [InlineData("RelatedSSNs($orderby=$this)", OrderByDirection.Ascending)]
        [InlineData("RelatedSSNs($orderby=$this asc)", OrderByDirection.Ascending)]
        [InlineData("RelatedSSNs($orderby=$this desc)", OrderByDirection.Descending)]
        public void DollarThisinOrderByPrimitiveCollectionInsideSelectShouldReferenceSelectedItem(string queryString, OrderByDirection orderByDirection)
        {
            // Arrange & Act
            // People?$select=RelatedSSNs($orderby=$this)
            // People?$select=RelatedSSNs($orderby=$this asc)
            // People?$select=RelatedSSNs($orderby=$this desc)
            SelectExpandClause clause = RunParseSelectExpand(queryString, "", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            OrderByClause orderByClause =
                (
                    clause.SelectedItems.First() as PathSelectItem // $select=RelatedSSNs(...)
                ).OrderByOption;

            IEdmTypeReference typeReference = (orderByClause.Expression as NonResourceRangeVariableReferenceNode).TypeReference;

            // Assert
            orderByClause.Expression.ShouldBeNonResourceRangeVariableReferenceNode(ExpressionConstants.This);
            Assert.Equal(orderByDirection, orderByClause.Direction);
            Assert.Equal("Edm.String", typeReference.Definition.FullTypeName()); // RelatedSSNs is a collection of strings.
        }

        [Fact]
        public void DollarThisinFilterInsideSelectShouldReferenceSelectedItemPrimitiveType()
        {
            // Arrange & Act
            // People?$select=RelatedSSNs($filter=endswith($this,'xyz'))
            SelectExpandClause clause = RunParseSelectExpand("RelatedSSNs($filter=endswith($this,'xyz'))", "", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            // Assert
            PathSelectItem selectItem = (PathSelectItem) Assert.Single(clause.SelectedItems);
            Assert.NotNull(selectItem.FilterOption);
            selectItem.FilterOption.Expression.ShouldBeSingleValueFunctionCallQueryNode("endswith");

            SingleValueFunctionCallNode singleValueFunctionCallNode = (SingleValueFunctionCallNode)selectItem.FilterOption.Expression;
            Assert.Equal(2, singleValueFunctionCallNode.Parameters.Count());

            ConvertNode convertNode = (ConvertNode) singleValueFunctionCallNode.Parameters.First();

            // $this references RelatedSSNs which is a collection of primitives, that's why we have a NonResourceRangeVariableReferenceNode
            convertNode.Source.ShouldBeNonResourceRangeVariableReferenceNode(ExpressionConstants.This);
            IEdmTypeReference typeReference = convertNode.Source.TypeReference;
            Assert.Equal("Edm.String", typeReference.Definition.FullTypeName()); // RelatedSSNs is a collection of strings.
        }

        [Fact]
        public void DollarThisinFilterInsideSelectShouldReferenceSelectedItemStructuredType()
        {
            // Arrange

            // $this/Street references PreviousAddresses which is a collection of Type Address.
            IEdmStructuredType expectedType = (IEdmStructuredType)HardCodedTestModel.GetAddressType();

            // Act
            // People?$select=PreviousAddresses($filter=endswith($this/Street,'xyz'))
            SelectExpandClause clause = RunParseSelectExpand("PreviousAddresses($filter=endswith($this/Street,'xyz'))", "", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            // Assert
            PathSelectItem selectItem = (PathSelectItem)Assert.Single(clause.SelectedItems);
            Assert.NotNull(selectItem.FilterOption);
            selectItem.FilterOption.Expression.ShouldBeSingleValueFunctionCallQueryNode("endswith");

            SingleValueFunctionCallNode singleValueFunctionCallNode = (SingleValueFunctionCallNode)selectItem.FilterOption.Expression; // endswith($this/Street,'xyz')
            Assert.Equal(2, singleValueFunctionCallNode.Parameters.Count());

            SingleValuePropertyAccessNode singleValuePropertyAccessNode = (SingleValuePropertyAccessNode)singleValueFunctionCallNode.Parameters.First(); // $this/Street

            // $this references Address which is a structured stype, that's why we have a ResourceRangeVariableReferenceNode.
            singleValuePropertyAccessNode.Source.ShouldBeResourceRangeVariableReferenceNode(ExpressionConstants.This);
            Assert.Equal("Street", singleValuePropertyAccessNode.Property.Name);
            Assert.Equal(expectedType, singleValuePropertyAccessNode.Property.DeclaringType); // Address is the DeclaringType of Street
        }

        [Fact]
        public void DollarThisinFilterInsideSelectInsideExpandShouldReferenceSelectedItem()
        {
            // Arrange & Act
            // People?$expand=MyDog($select=Nicknames($filter=startswith($this, 'blu')))
            SelectExpandClause clause = RunParseSelectExpand("", "MyDog($select=Nicknames($filter=startswith($this, 'blu')))", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            // Assert
            ExpandedNavigationSelectItem expandedSelectItem = (ExpandedNavigationSelectItem)Assert.Single(clause.SelectedItems); // $expand=MyDog(...)
            Assert.NotNull(expandedSelectItem.SelectAndExpand);
            SelectExpandClause innerClause = expandedSelectItem.SelectAndExpand; // $select=Nicknames(...)

            PathSelectItem selectItem = (PathSelectItem)Assert.Single(innerClause.SelectedItems);
            Assert.NotNull(selectItem.FilterOption);
            selectItem.FilterOption.Expression.ShouldBeSingleValueFunctionCallQueryNode("startswith");

            SingleValueFunctionCallNode singleValueFunctionCallNode = (SingleValueFunctionCallNode)selectItem.FilterOption.Expression;
            Assert.Equal(2, singleValueFunctionCallNode.Parameters.Count());

            ConvertNode convertNode = (ConvertNode)singleValueFunctionCallNode.Parameters.First();
            convertNode.Source.ShouldBeNonResourceRangeVariableReferenceNode(ExpressionConstants.This);
            IEdmTypeReference typeReference = convertNode.Source.TypeReference;
            Assert.Equal("Edm.String", typeReference.Definition.FullTypeName()); // Nicknames is a collection of strings.
        }

        [Fact]
        public void SelectAndExpandShouldFailOnSelectWrongComplexProperties()
        {
            Action parse = () => RunParseSelectExpand("Name,MyAddress/City/Street,MyDog", "MyDog($select=Color)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());
            parse.Throws<ODataException>(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
        }

        [Fact]
        public void SelectAndExpandShouldFailOnSelectComplexPropertiesWithWrongTypeCast()
        {
            Action parse = () => RunParseSelectExpand("Name,MyAddress/Fully.Qualified.Namespace.OpenAddress/HomeNO,MyDog", "MyDog($select=Color)", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet());

            parse.Throws<ODataException>(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
        }

        // TODO: Tests cases with query options

        #endregion

        #region $select with nested query options
        [Fact]
        public void SelectWithNestedFilterWorks()
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "PreviousAddresses($filter=Street eq 'abc')"}
                });

            // Act
            var selectClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectClause);
            Assert.False(selectClause.AllSelected);
            var pathSelectItem = Assert.IsType<PathSelectItem>(Assert.Single(selectClause.SelectedItems));
            pathSelectItem.SelectedPath.Equals(new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp())));

            Assert.NotNull(pathSelectItem.FilterOption);
            pathSelectItem.FilterOption.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
        }

        [Fact]
        public void SelectWithNestedTopSkipAndCountWorks()
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "PreviousAddresses($top=4;$skip=2;$count=true)"}
                });

            // Act
            var selectClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectClause);
            Assert.False(selectClause.AllSelected);
            var pathSelectItem = Assert.IsType<PathSelectItem>(Assert.Single(selectClause.SelectedItems));
            pathSelectItem.SelectedPath.Equals(new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp())));

            Assert.NotNull(pathSelectItem.TopOption);
            Assert.Equal(4, pathSelectItem.TopOption);

            Assert.NotNull(pathSelectItem.SkipOption);
            Assert.Equal(2, pathSelectItem.SkipOption);
        }

        // $expand=navProp/$count
        [Fact]
        public void ExpandWithNavigationPropCountWorks()
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", "MyPaintings/$count"}
                });

            // Act
            var selectExpandClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectExpandClause);
            ExpandedCountSelectItem expandedCountSelectItem = Assert.IsType<ExpandedCountSelectItem>(Assert.Single(selectExpandClause.SelectedItems));
            Assert.Null(expandedCountSelectItem.FilterOption);
            Assert.Null(expandedCountSelectItem.SearchOption);
        }

        // $expand=navProp/$count($filter=prop)
        [Fact]
        public void ExpandWithNavigationPropCountWithFilterOptionWorks()
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", "MyPaintings/$count($filter=Artist eq 'Artist One')"}
                });

            // Act
            var selectExpandClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectExpandClause);
            ExpandedCountSelectItem expandedCountSelectItem = Assert.IsType<ExpandedCountSelectItem>(Assert.Single(selectExpandClause.SelectedItems));
            Assert.NotNull(expandedCountSelectItem.FilterOption);
            Assert.Null(expandedCountSelectItem.SearchOption);
        }

        // $expand=navProp/$count($search=prop)
        [Fact]
        public void ExpandWithNavigationPropCountWithSearchOptionWorks()
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", "MyPaintings/$count($search=Blue)"}
                });

            // Act
            var selectExpandClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectExpandClause);
            ExpandedCountSelectItem expandedCountSelectItem = Assert.IsType<ExpandedCountSelectItem>(Assert.Single(selectExpandClause.SelectedItems));
            Assert.Null(expandedCountSelectItem.FilterOption);
            Assert.NotNull(expandedCountSelectItem.SearchOption);
        }

        // $expand=navProp/fully.qualified.type/$ref
        [Theory]
        [InlineData("MyPeople/Fully.Qualified.Namespace.Employee/$ref")]
        [InlineData("MyPeople/MainAlias.Employee/$ref")]
        public void ExpandWithNavigationPropRefWithFullyQualifiedTypeWorks(string query)
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", query}
                });

            // Act
            var selectExpandClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectExpandClause);
            ExpandedReferenceSelectItem expandedRefSelectItem = Assert.IsType<ExpandedReferenceSelectItem>(Assert.Single(selectExpandClause.SelectedItems));
            Assert.Same(HardCodedTestModel.GetPeopleSet(), expandedRefSelectItem.NavigationSource);
            Assert.Equal(2, expandedRefSelectItem.PathToNavigationProperty.Count);

            NavigationPropertySegment navPropSegment = Assert.IsType<NavigationPropertySegment>(expandedRefSelectItem.PathToNavigationProperty.Segments.First());
            TypeSegment typeSegment = Assert.IsType<TypeSegment>(expandedRefSelectItem.PathToNavigationProperty.Segments.Last());
            Assert.Equal("MyPeople", navPropSegment.Identifier);
            Assert.Equal("Collection(Fully.Qualified.Namespace.Person)", navPropSegment.EdmType.FullTypeName());
            Assert.Equal("Fully.Qualified.Namespace.Employee", typeSegment.EdmType.FullTypeName());
        }

        // $expand=navProp/fully.qualified.type/$count
        [Theory]
        [InlineData("MyPeople/Fully.Qualified.Namespace.Employee/$count")]
        [InlineData("MyPeople/MainAlias.Employee/$count")] // With schema alias
        public void ExpandWithNavigationPropCountWithFullyQualifiedTypeWorks(string query)
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", query}
                });

            // Act
            var selectExpandClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectExpandClause);
            ExpandedCountSelectItem expandedCountSelectItem = Assert.IsType<ExpandedCountSelectItem>(Assert.Single(selectExpandClause.SelectedItems));
            Assert.Same(HardCodedTestModel.GetPeopleSet(), expandedCountSelectItem.NavigationSource);
            Assert.Null(expandedCountSelectItem.FilterOption);
            Assert.Null(expandedCountSelectItem.SearchOption);
            Assert.Equal(2, expandedCountSelectItem.PathToNavigationProperty.Count);

            NavigationPropertySegment navPropSegment = Assert.IsType<NavigationPropertySegment>(expandedCountSelectItem.PathToNavigationProperty.Segments.First());
            TypeSegment typeSegment = Assert.IsType<TypeSegment>(expandedCountSelectItem.PathToNavigationProperty.Segments.Last());
            Assert.Equal("MyPeople", navPropSegment.Identifier);
            Assert.Equal("Collection(Fully.Qualified.Namespace.Person)", navPropSegment.EdmType.FullTypeName());
            Assert.Equal("Fully.Qualified.Namespace.Employee", typeSegment.EdmType.FullTypeName());
        }

        // $expand=navProp/fully.qualified.type/$count($filter=prop)
        [Theory]
        [InlineData("MyPeople/Fully.Qualified.Namespace.Employee/$count($filter=ID eq 1)")]
        [InlineData("MyPeople/MainAlias.Employee/$count($filter=ID eq 1)")] // With schema alias
        public void ExpandWithNavigationPropCountWithFilterAndFullyQualifiedTypeWorks(string query)
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", query}
                });

            // Act
            var selectExpandClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectExpandClause);
            ExpandedCountSelectItem expandedCountSelectItem = Assert.IsType<ExpandedCountSelectItem>(Assert.Single(selectExpandClause.SelectedItems));
            Assert.Same(HardCodedTestModel.GetPeopleSet(), expandedCountSelectItem.NavigationSource);
            Assert.NotNull(expandedCountSelectItem.FilterOption);
            Assert.Null(expandedCountSelectItem.SearchOption);
            Assert.Equal(2, expandedCountSelectItem.PathToNavigationProperty.Count);

            NavigationPropertySegment navPropSegment = Assert.IsType<NavigationPropertySegment>(expandedCountSelectItem.PathToNavigationProperty.Segments.First());
            TypeSegment typeSegment = Assert.IsType<TypeSegment>(expandedCountSelectItem.PathToNavigationProperty.Segments.Last());
            Assert.Equal("MyPeople", navPropSegment.Identifier);
            Assert.Equal("Collection(Fully.Qualified.Namespace.Person)", navPropSegment.EdmType.FullTypeName());
            Assert.Equal("Fully.Qualified.Namespace.Employee", typeSegment.EdmType.FullTypeName());
        }

        // $expand=navProp/fully.qualified.type/$count($search=prop)
        [Theory]
        [InlineData("MyPeople/Fully.Qualified.Namespace.Employee/$count($search=blue)")]
        [InlineData("MyPeople/MainAlias.Employee/$count($search=blue)")] // With schema alias
        public void ExpandWithNavigationPropCountWithSearchAndFullyQualifiedTypeWorks(string query)
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", query}
                });

            // Act
            var selectExpandClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectExpandClause);
            ExpandedCountSelectItem expandedCountSelectItem = Assert.IsType<ExpandedCountSelectItem>(Assert.Single(selectExpandClause.SelectedItems));
            Assert.Same(HardCodedTestModel.GetPeopleSet(), expandedCountSelectItem.NavigationSource);
            Assert.Null(expandedCountSelectItem.FilterOption);
            Assert.NotNull(expandedCountSelectItem.SearchOption);
            Assert.Equal(2, expandedCountSelectItem.PathToNavigationProperty.Count);

            NavigationPropertySegment navPropSegment = Assert.IsType<NavigationPropertySegment>(expandedCountSelectItem.PathToNavigationProperty.Segments.First());
            TypeSegment typeSegment = Assert.IsType<TypeSegment>(expandedCountSelectItem.PathToNavigationProperty.Segments.Last());
            Assert.Equal("MyPeople", navPropSegment.Identifier);
            Assert.Equal("Collection(Fully.Qualified.Namespace.Person)", navPropSegment.EdmType.FullTypeName());
            Assert.Equal("Fully.Qualified.Namespace.Employee", typeSegment.EdmType.FullTypeName());
        }

        [Theory]
        [InlineData("MyPeople/Fully.Qualified.Namespace.UndefinedType")]
        [InlineData("MyPeople/Fully.Qualified.Namespace.UndefinedType/$ref")]
        [InlineData("MyPeople/Fully.Qualified.Namespace.UndefinedType/$count")]
        [InlineData("MyPeople/Fully.Qualified.Namespace.UndefinedType/$count($search=blue)")]
        [InlineData("MyPeople/Fully.Qualified.Namespace.UndefinedType/$count($filter=ID eq 1)")]
        public void ExpandWithNavigationPropWithUndefinedTypeThrows(string query)
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", query}
                });

            // Act
            Action action = () => odataQueryOptionParser.ParseSelectAndExpand();

            // Assert

            // Exception: The type Fully.Qualified.Namespace.UndefinedType is not defined in the model.
            action.Throws<ODataException>(ODataErrorStrings.ExpandItemBinder_CannotFindType("Fully.Qualified.Namespace.UndefinedType"));
        }

        [Fact]
        public void SelectWithNestedSelectWorks()
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "MyAddress($select=City,MyNeighbors($count=true))"}
                });

            // Act
            var selectClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectClause);
            Assert.False(selectClause.AllSelected);
            var pathSelectItem = Assert.IsType<PathSelectItem>(Assert.Single(selectClause.SelectedItems));
            pathSelectItem.SelectedPath.Equals(new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonAddressProp())));

            SelectExpandClause subLevelSelect = pathSelectItem.SelectAndExpand;
            Assert.NotNull(subLevelSelect);
            Assert.False(subLevelSelect.AllSelected);
            Assert.Equal(2, subLevelSelect.SelectedItems.Count()); // City & MyNeighbors

            // City
            PathSelectItem subLevelPathSelect = Assert.IsType<PathSelectItem>(subLevelSelect.SelectedItems.First());
            subLevelPathSelect.SelectedPath.Equals(new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty())));
            Assert.Null(subLevelPathSelect.CountOption);

            // MyNeighbors
            subLevelPathSelect = Assert.IsType<PathSelectItem>(subLevelSelect.SelectedItems.Last());
            subLevelPathSelect.SelectedPath.Equals(new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetAddressMyNeighborsProperty())));
            Assert.NotNull(subLevelPathSelect.CountOption);
            Assert.True(subLevelPathSelect.CountOption);
        }

        [Fact]
        public void SelectWithNestedWildcardWorks()
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "PreviousAddresses($select=*)"}
                });

            // Act
            var selectClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectClause);
            Assert.False(selectClause.AllSelected);
            var pathSelectItem = Assert.IsType<PathSelectItem>(Assert.Single(selectClause.SelectedItems));
            pathSelectItem.SelectedPath.Equals(new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonPreviousAddressesProp())));

            SelectExpandClause subLevelSelect = pathSelectItem.SelectAndExpand;
            Assert.NotNull(subLevelSelect);
            Assert.False(subLevelSelect.AllSelected); // it's false not matter we select "*"
            Assert.IsType<WildcardSelectItem>(Assert.Single(subLevelSelect.SelectedItems));
        }

        [Fact]
        public void SelectWithNestedSelectGeneratedPropertiesWorks()
        {
            // Arrange
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "PreviousAddresses($compute=tolower(Street) as lowStreet;$select=City,lowStreet)"}
                });

            // Act
            var selectClause = odataQueryOptionParser.ParseSelectAndExpand();

            // Assert
            Assert.NotNull(selectClause);
            Assert.False(selectClause.AllSelected);
            var pathSelectItem = Assert.IsType<PathSelectItem>(Assert.Single(selectClause.SelectedItems));

            // $compute
            ComputeClause subCompute = pathSelectItem.ComputeOption;
            Assert.NotNull(subCompute);
            ComputeExpression computeExpr = Assert.Single(subCompute.ComputedItems);
            Assert.Equal("lowStreet", computeExpr.Alias);
            var functionCall = Assert.IsType<SingleValueFunctionCallNode>(computeExpr.Expression);
            Assert.Equal("tolower", functionCall.Name);
            Assert.Empty(functionCall.Functions);

            // $select
            SelectExpandClause subLevelSelect = pathSelectItem.SelectAndExpand;
            Assert.NotNull(subLevelSelect);
            Assert.False(subLevelSelect.AllSelected);
            Assert.Equal(2, subLevelSelect.SelectedItems.Count());

            var items = subLevelSelect.SelectedItems.ToArray();
            items[0].ShouldBePathSelectionItem(new ODataPath(new PropertySegment(HardCodedTestModel.GetAddressCityProperty())));
            items[1].ShouldBePathSelectionItem(new ODataPath(new DynamicPathSegment("lowStreet")));
        }

        #endregion

        #region $select and generated properties
        [Fact]
        public void DollarComputedPropertyTreatedAsOpenPropertyInSelect()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "DoubleTotal"},
                    {"$compute", "FavoriteNumber mul 2 as DoubleTotal"}
                });
            odataQueryOptionParser.ParseCompute();
            var selectClause = odataQueryOptionParser.ParseSelectAndExpand();
            AssertSelectString("DoubleTotal", selectClause);
        }

        [Fact]
        public void ApplyComputedPropertyTreatedAsOpenPropertyInSelect()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "DoubleTotal"},
                    {"$apply", "compute(FavoriteNumber mul 2 as DoubleTotal)"}
                });
            odataQueryOptionParser.ParseApply();
            var selectClause = odataQueryOptionParser.ParseSelectAndExpand();
            AssertSelectString("DoubleTotal", selectClause);
        }

        [Fact]
        public void SelectAfterApplyReferencingCollapsedPropertyThrows()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "FavoriteNumber"},
                    {"$apply", "aggregate(FavoriteNumber with sum as DoubleTotal)"},
                });

            odataQueryOptionParser.ParseApply();
            Action action = () => odataQueryOptionParser.ParseSelectAndExpand();

            action.Throws<ODataException>(ODataErrorStrings.ApplyBinder_GroupByPropertyNotPropertyAccessValue("FavoriteNumber"));
        }

        [Fact]
        public void ComputeAfterApplyPropertyTreatedAsOpenPropertyInSelect()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$select", "DoubleTotal, NewTotal"},
                    {"$apply", "aggregate(FavoriteNumber with sum as DoubleTotal)"},
                    {"$compute", "DoubleTotal as NewTotal"},
                });
            odataQueryOptionParser.ParseApply();
            odataQueryOptionParser.ParseCompute();
            var selectClause = odataQueryOptionParser.ParseSelectAndExpand();
            AssertSelectString("DoubleTotal, NewTotal", selectClause);
        }

        [Fact]
        public void ComputeInExpandPropertyTreatedAsOpenPropertyInSelect()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", "MyDog($compute=Color as ColorAlias;$select=ColorAlias)"},
                });
            var expandClause = odataQueryOptionParser.ParseSelectAndExpand();
            // TODO: Can't use AssertExpandString, because SelectExpandClauseExtensions doesn't support $compute,$filter,$apply etc.
            var expandedSelectionItem = expandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Single();
            Assert.NotNull(expandedSelectionItem.ComputeOption);
            Assert.Equal("ColorAlias", expandedSelectionItem.ComputeOption.ComputedItems.Single().Alias);
            Assert.Equal("ColorAlias", expandedSelectionItem.SelectAndExpand.SelectedItems.Single().ShouldBeSelectedItemOfType<PathSelectItem>().SelectedPath.LastSegment.Identifier);
        }

        [Fact]
        public void ComputeInExpandPropertyTreatedAsOpenPropertyInFilter()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", "MyDog($compute=Color as ColorAlias;$filter=ColorAlias eq 'Red')"},
                });
            var expandClause = odataQueryOptionParser.ParseSelectAndExpand();
            // TODO: Can't use AssertExpandString, because SelectExpandClauseExtensions doesn't support $compute,$filter,$apply etc.
            var expandedSelectionItem = expandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Single();
            Assert.NotNull(expandedSelectionItem.ComputeOption);
            Assert.Equal("ColorAlias", expandedSelectionItem.ComputeOption.ComputedItems.Single().Alias);
            var binaryOperatorNode = expandedSelectionItem.FilterOption.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            (binaryOperatorNode.Left as ConvertNode).Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("ColorAlias");
        }

        [Fact]
        public void ApplyInExpandPropertyTreatedAsOpenPropertyInSelect()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", "MyDog($apply=aggregate(Color with max as MaxColor);$select=MaxColor)"},
                });
            var expandClause = odataQueryOptionParser.ParseSelectAndExpand();
            // TODO: Can't use AssertExpandString, because SelectExpandClauseExtensions doesn't support $compute,$filter,$apply etc.
            var expandedSelectionItem = expandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Single();
            Assert.NotNull(expandedSelectionItem.ApplyOption);
            Assert.Equal("MaxColor", (expandedSelectionItem.ApplyOption.Transformations.Single() as AggregateTransformationNode).AggregateExpressions.Single().Alias);
            Assert.Equal("MaxColor", expandedSelectionItem.SelectAndExpand.SelectedItems.Single().ShouldBeSelectedItemOfType<PathSelectItem>().SelectedPath.LastSegment.Identifier);
        }

        [Fact]
        public void ApplyInExpandPropertyTreatedAsOpenPropertyInFilter()
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", "MyDog($apply=aggregate(Color with max as MaxColor);$filter=MaxColor eq 'Red')"},
                });
            var expandClause = odataQueryOptionParser.ParseSelectAndExpand();
            // TODO: Can't use AssertExpandString, because SelectExpandClauseExtensions doesn't support $compute,$filter,$apply etc.
            var expandedSelectionItem = expandClause.SelectedItems.OfType<ExpandedNavigationSelectItem>().Single();
            Assert.NotNull(expandedSelectionItem.ApplyOption);
            Assert.Equal("MaxColor", (expandedSelectionItem.ApplyOption.Transformations.Single() as AggregateTransformationNode).AggregateExpressions.Single().Alias);
            var binaryOperatorNode = expandedSelectionItem.FilterOption.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            (binaryOperatorNode.Left as ConvertNode).Source.ShouldBeSingleValueOpenPropertyAccessQueryNode("MaxColor");
        }

        [Theory]
        [InlineData("$apply=aggregate(Color with max as MaxColor);$filter=Color eq 'Red'")]
        [InlineData("$apply=aggregate(Color with max as MaxColor)/filter(Color eq 'Red')")]
        [InlineData("$apply=aggregate(Color with max as MaxColor);$select=Color")]
        [InlineData("$apply=aggregate(Color with max as MaxColor);$select=MaxColor,Color")]
        public void FilterAfterApplyReferencingCollapsedPropertyThrows(string nestedClause)
        {
            var odataQueryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(),
                new Dictionary<string, string>()
                {
                    {"$expand", $"MyDog({nestedClause})"},
                });

            Action action = () => odataQueryOptionParser.ParseSelectAndExpand();

            action.Throws<ODataException>(ODataErrorStrings.ApplyBinder_GroupByPropertyNotPropertyAccessValue("Color"));
        }

        #endregion

        #region Test Running Helpers

        private static SelectExpandClause RunParseSelectExpand(string select, string expand, IEdmStructuredType type, IEdmEntitySet entitySet)
        {
            return new ODataQueryOptionParser(HardCodedTestModel.TestModel, type, entitySet, new Dictionary<string, string> { { "$expand", expand }, { "$select", select } }).ParseSelectAndExpand();
        }

        private static SelectExpandClause RunParseSelectExpandAndAssertPaths(string select, string expand, string expectedSelect, string expectedExpand, IEdmEntityType type, IEdmEntitySet enitytSet)
        {
            var result = RunParseSelectExpand(select, expand, type, enitytSet);
            AssertSelectString(expectedSelect, result);
            AssertExpandString(expectedExpand, result);
            return result;
        }

        private static string TrimSpacesAroundSlash(string path)
        {
            return string.Join("/", path.Split(new char[] { '/' }).Select(p => p.Trim()));
        }

        private static SelectItem ParseSingleSelectForEmployee(string select, string expectedSelect = null)
        {
            return ParseSingleSelect(select, HardCodedTestModel.GetEmployeeType(), HardCodedTestModel.GetPeopleSet(), expectedSelect);
        }

        private static SelectItem ParseSingleSelectForPerson(string select, string expectedSelect = null)
        {
            return ParseSingleSelect(select, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), expectedSelect);
        }

        private static SelectItem ParseSingleSelectForDog(string select, string expectedSelect = null)
        {
            return ParseSingleSelect(select, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet(), expectedSelect);
        }

        private static SelectItem ParseSingleSelectForPainting(string select, string expectedSelect = null)
        {
            return ParseSingleSelect(select, HardCodedTestModel.GetPaintingType(), HardCodedTestModel.GetPaintingsSet(), expectedSelect);
        }

        private static SelectItem ParseSingleSelect(string select, IEdmEntityType entityType, IEdmEntitySet entitySet, string expectedSelect = null)
        {
            var result = RunParseSelectExpand(select, null, entityType, entitySet);
            var selectionItem = result.SelectedItems.Single();

            AssertSelectString(expectedSelect ?? select, result);
            Assert.True(String.IsNullOrEmpty(ConvertExpandToString(result)));

            var publicSelectItem = result.SelectedItems.Single();
            Assert.Same(selectionItem, publicSelectItem);

            if (!String.IsNullOrEmpty(select))
            {
                Assert.False(result.AllSelected);
            }

            return selectionItem;
        }

        private static SelectItem ParseSingleExpandForPerson(string expand, string expectedExpand = null)
        {
            return ParseSingleExpand(expand, HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), expectedExpand);
        }

        private static SelectItem ParseSingleExpand(string expand, IEdmEntityType entityType, IEdmEntitySet entitySet, string expectedExpand = null)
        {
            var result = RunParseSelectExpand(null, expand, entityType, entitySet);
            var expandedSelectionItem = result.SelectedItems.Single(x => x is ExpandedNavigationSelectItem);

            AssertSelectString("", result);
            AssertExpandString(expectedExpand ?? expand, result);

            Assert.True(result.AllSelected);

            return expandedSelectionItem;
        }
        #endregion

        #region Stringify Object Model Helpers

        private static void AssertExpandString(string expand, SelectExpandClause result)
        {
            string expectedExpand = expand ?? string.Empty;
            expectedExpand = string.Join(",", expectedExpand.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(TrimSpacesAroundSlash).Distinct());
            Assert.Equal(expectedExpand, ConvertExpandToString(result));
        }

        private static void AssertSelectString(string select, SelectExpandClause result)
        {
            string expectedSelect = select ?? string.Empty;
            expectedSelect = string.Join(",", expectedSelect.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(TrimSpacesAroundSlash).Distinct());
            Assert.Equal(expectedSelect, ConvertSelectToString(result));
        }

        // TODO: Need V4 String Builder
        private static string ConvertSelectToString(SelectExpandClause selectExpandClause)
        {
            string selectClause, expandClause;
            // todo: run this for each version
            selectExpandClause.GetSelectExpandPaths(ODataVersion.V4, out selectClause, out expandClause);
            return selectClause;
        }

        // TODO: Need V4 String Builder
        private static string ConvertExpandToString(SelectExpandClause selectExpandClause)
        {
            string selectClause, expandClause;
            // todo: run this for each OData version
            selectExpandClause.GetSelectExpandPaths(ODataVersion.V4, out selectClause, out expandClause);
            return expandClause;
        }

        #endregion
    }
}
