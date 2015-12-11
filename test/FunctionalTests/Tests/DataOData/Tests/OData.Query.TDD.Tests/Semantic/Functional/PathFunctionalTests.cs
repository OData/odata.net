//---------------------------------------------------------------------
// <copyright file="PathFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Functional
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    #endregion Namespaces

    /// <summary>
    /// Test code for end-to-end parsing of the Path.
    /// TODO: add more tests for OperationImportSegment and OperationSegment
    /// </summary>
    [TestClass]
    public class PathFunctionalTests
    {
        [TestMethod]
        public void SimpleEntitySet()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs");
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
            path.NavigationSource().Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void SimpleSingleton()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Boss");
            path.LastSegment.ShouldBeSingletonSegment(HardCodedTestModel.GetBossSingleton());
            path.NavigationSource().Should().Be(HardCodedTestModel.GetBossSingleton());
        }

        [TestMethod]
        public void SimpleServiceOperation()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetCoolPeople");
            path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolPeople()).
                And.EntitySet.Should().Be(HardCodedTestModel.GetPeopleSet());
            path.NavigationSource().Should().Be(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void SimpleNavigationPropertyLinkSegment()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(7)/MyDog/$ref");
            path.LastSegment.ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp());
            path.NavigationSource().Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void SimpleActionImport()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("ResetAllData");
            path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForResetAllData()).
                And.EntitySet.Should().BeNull();
        }

        [TestMethod]
        public void VoidServiceOperationIsNotComposable()
        {
            Action parsePath = () => PathFunctionalTestsUtil.RunParsePath("GetNothing/foo");
            parsePath.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("GetNothing"));
        }

        [TestMethod]
        public void VoidServiceOperationShouldAllowButIgnoreEmptyParens()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetNothing()");
            path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetVoidServiceOperation());
        }

        [TestMethod]
        public void PrimitiveServiceOperationIsComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetSomeNumber/$value");
            path.FirstSegment.Should().BeOfType<OperationImportSegment>();
            path.LastSegment.ShouldBeValueSegment();
        }

        [TestMethod]
        public void PrimitiveServiceOperationShouldAllowButIgnoreEmptyParens()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetSomeNumber()");
            path.LastSegment.Should().BeOfType<OperationImportSegment>();
        }

        [TestMethod]
        public void PrimitiveServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            Action parsePath = () => PathFunctionalTestsUtil.RunParsePath("GetSomeNumber/foo");
            parsePath.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("GetSomeNumber", "foo"));
        }

        [TestMethod]
        public void ComplexServiceOperationIsComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetSomeAddress/City");
            path.FirstSegment.Should().BeOfType<OperationImportSegment>();
            path.LastSegment.ShouldBePropertySegment(HardCodedTestModel.GetAddressCityProperty());
        }

        [TestMethod]
        [Ignore]
        public void ComplexServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            Action parsePath = () => PathFunctionalTestsUtil.RunParsePath("GetSomeAddress/foo");
            parsePath.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_ResourceNotFound("foo"));
        }

        [TestMethod]
        public void EntityServiceOperationIsComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetCoolestPerson/Fully.Qualified.Namespace.Employee");
            path.FirstSegment.Should().BeOfType<OperationImportSegment>();
            path.LastSegment.ShouldBeTypeSegment(HardCodedTestModel.GetEmployeeType());
        }

        [TestMethod]
        public void EntityServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetCoolestPerson/foo", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("foo"));
        }

        [TestMethod]
        public void EntitySetServiceOperationIsComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetCoolPeople(1)");
            path.FirstSegment.Should().BeOfType<OperationImportSegment>();
            path.LastSegment.ShouldBeSimpleKeySegment(1)
                .And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void EntitySetServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetCoolPeople/foo", ODataErrorStrings.RequestUriProcessor_CannotQueryCollections("GetCoolPeople"));
        }

        [TestMethod]
        public void PrimitiveCollectionOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetSomeNumbers/foo", ODataErrorStrings.RequestUriProcessor_CannotQueryCollections("GetSomeNumbers"));
        }

        [TestMethod]
        public void ComplexCollectionServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetSomeAddresses/foo", ODataErrorStrings.RequestUriProcessor_CannotQueryCollections("GetSomeAddresses"));
        }

        [TestMethod]
        public void SimpleKeyLookup()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)");
            path.LastSegment.ShouldBeSimpleKeySegment(1).And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void SimpleKeyLookupWithKeysAsSegments()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Dogs/1")) { UrlConventions = ODataUrlConventions.KeyAsSegment }.ParsePath();
            path.LastSegment.ShouldBeSimpleKeySegment(1);
        }

        [TestMethod]
        public void SimpleKeyLookupWithKeysAsSegmentsFollowedByNavigation()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/People/1/Birthdate")) { UrlConventions = ODataUrlConventions.KeyAsSegment }.ParsePath();
            path.LastSegment.ShouldBePropertySegment(HardCodedTestModel.GetPersonBirthdateProp());
        }

        [TestMethod]
        public void UseEscapeMarkerWithTypeSegmentInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/People/$/Fully.Qualified.Namespace.Employee")) { UrlConventions = ODataUrlConventions.KeyAsSegment }.ParsePath();
            path.LastSegment.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetEmployeeType(), false)));
        }

        [TestMethod]
        public void SimpleTemplatedKeyLookupWithKeysAsSegments()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Dogs/{k1}")) { UrlConventions = ODataUrlConventions.KeyAsSegment, EnableUriTemplateParsing = true }.ParsePath();
            var keySegment = path.LastSegment.As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{k1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
        }

        [TestMethod]
        public void UseTemplatedKeyWithPathTemplateSegmentInKeyAsSegmentShouldWork()
        {
            var paths = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Dogs/{k1}/{k2}")) { UrlConventions = ODataUrlConventions.KeyAsSegment, EnableUriTemplateParsing = true }.ParsePath().ToList();
            var keySegment = paths[1].As<KeySegment>();
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            keypair.Key.Should().Be("ID");
            keypair.Value.As<UriTemplateExpression>().ShouldBeEquivalentTo(new UriTemplateExpression() { LiteralText = "{k1}", ExpectedType = keySegment.EdmType.As<IEdmEntityType>().DeclaredKey.Single().Type });
            paths[2].As<PathTemplateSegment>().LiteralText.Should().Be("{k2}");
        }

        [TestMethod]
        public void UseEscapeMarkerWithTemplatedKeyTypeSegmentInKeyAsSegmentShouldBeParsedAsPathTemplateSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Dogs/$/{k1}")) { UrlConventions = ODataUrlConventions.KeyAsSegment, EnableUriTemplateParsing = true }.ParsePath();
            path.LastSegment.As<PathTemplateSegment>().LiteralText.Should().Be("{k1}");
        }

        [TestMethod]
        public void RelativeFullUriShouldBeParsed()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/service.svc"), new Uri("Dogs", UriKind.Relative));
            var path = parser.ParsePath();
            parser.ServiceRoot.ShouldBeEquivalentTo(new Uri("http://gobbldygook/service.svc/"));
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void TrailingEscapeMarkerShouldBeIgnoredInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/People/$")) { UrlConventions = ODataUrlConventions.KeyAsSegment }.ParsePath();
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void DontUseEscapeSequenceInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Users/Fully.Qualified.Namespace.User")) { UrlConventions = ODataUrlConventions.KeyAsSegment }.ParsePath();
            path.LastSegment.ShouldBeSimpleKeySegment("Fully.Qualified.Namespace.User");
        }

        [TestMethod]
        public void UseMultipleEscapeSequencesWithCountInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$/$/People/1/$/$/MyDog/$/$/MyPeople/$/$/$count/$/$")) { UrlConventions = ODataUrlConventions.KeyAsSegment }.ParsePath();
            path.LastSegment.ShouldBeCountSegment();
            path.NavigationSource().Should().BeNull();
        }

        [TestMethod]
        public void PathThatIsOnlyEscapeSegments()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$/$/$")) { UrlConventions = ODataUrlConventions.KeyAsSegment }.ParsePath();
            path.Should().BeEmpty();
        }

        [TestMethod]
        public void AutoComputeForeignKeyReversalInKeyAsSegment()
        {
            var path = new ODataUriParser(
                HardCodedTestModel.TestModel,
                new Uri("http://gobbldygook/"),
                new Uri("http://gobbldygook/Dogs/1/LionsISaw/2"))
            {
                UrlConventions = ODataUrlConventions.KeyAsSegment
            }.ParsePath();
            path.LastSegment.ShouldBeKeySegment(
                new KeyValuePair<string, object>("ID1", 1),
                new KeyValuePair<string, object>("ID2", 2));
        }

        [TestMethod]
        public void AutoComputeForeignKeyInKeyAsSegment()
        {
            var path = new ODataUriParser(
                HardCodedTestModel.TestModel,
                new Uri("http://gobbldygook/"),
                new Uri("http://gobbldygook/Dogs/1/LionsISaw/2/Friends/3"))
            {
                UrlConventions = ODataUrlConventions.KeyAsSegment
            }.ParsePath();
            path.LastSegment.ShouldBeKeySegment(
                new KeyValuePair<string, object>("ID1", 2),
                new KeyValuePair<string, object>("ID2", 3));
        }

        [TestMethod]
        public void BoundActionOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)/Fully.Qualified.Namespace.Walk");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetDogWalkAction()).And.EntitySet.Should().BeNull();
        }

        [TestMethod]
        public void BoundActionThatReturnsEntitiesShouldHaveSetComputedCorrectly()
        {
            // Paint action returns a collection of Paintings that the Person created
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.Paint");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetPersonPaintAction()).
                And.EntitySet.Should().BeSameAs(HardCodedTestModel.GetPaintingsSet());
            path.NavigationSource().Should().Be(HardCodedTestModel.GetPaintingsSet());
        }

        [TestMethod]
        public void BoundActionFromNonDefaultContainerOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.Move");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetPersonMoveAction());
        }

        [TestMethod]
        public void ActionOnEntityCollection()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.FireAll");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFireAllAction());
        }

        [TestMethod]
        public void FunctionWithoutEntitySetPath()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet()");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetAllMyFriendsDogs_NoSet());
        }

        [TestMethod]
        public void CannotAddParametersToActions()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Context.MoveEveryone(streetAddress='stuff')", ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Context.MoveEveryone"));
        }

        [TestMethod]
        public void NothingCanComeAfterAnAction()
        {
            // TODO: We can improve error message drastically when we refactor path parsing
            // The 'Walk' action returns an Address
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/Fully.Qualified.Namespace.Walk/City", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.Walk"));
        }

        #region Functions

        [TestMethod]
        public void BoundFunctionOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.HasJob");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForHasJob());
        }

        [TestMethod]
        public void OnlyExactlyMatchingFunctionWithMultipleOverloadsIsReturned()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.HasDog");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetHasDogOverloadWithOneParameter());
        }

        [TestMethod]
        public void FunctionWithNamedParameter()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.HasDog(inOffice=true)");
            var opSegment = path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForEmployeeHasDogWithTwoParameters())
                .And.ShouldHaveParameterCount(1);
            opSegment.And.Parameters.SingleOrDefault(p => p.Name == "inOffice").Value.As<ConvertNode>().Source.As<ConstantNode>().Value.Should().Be(true);
        }

        [TestMethod]
        public void FunctionWithMultipleNamedParameters()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.HasDog(inOffice=true, name='Fido')");
            var opSegment = path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters())
                  .And.ShouldHaveParameterCount(2);
            //.And.ShouldHaveConstantParameter("inOffice", true)
            opSegment.And.ShouldHaveConstantParameter("name", "Fido");
            opSegment.And.Parameters.SingleOrDefault(p => p.Name == "inOffice").Value.As<ConvertNode>().Source.As<ConstantNode>().Value.Should().Be(true);
        }

        [TestMethod]
        public void FunctionWithAliasedParameters()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.HasDog(inOffice=@x, name=@y)");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters())
                .And.ShouldHaveParameterCount(2)
                .And.ShouldHaveSegmentOfParameterAliasNode("inOffice", "@x")
                .And.ShouldHaveSegmentOfParameterAliasNode("name", "@y");
        }

        [TestMethod]
        public void FunctionWithPositionalParmeterShouldThrow()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.HasDog(true)", ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Fully.Qualified.Namespace.HasDog"));
        }

        [TestMethod]
        public void FunctionWithMultiplePositionalParametersShouldThrow()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.HasDog(true, 'Fido')", ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Fully.Qualified.Namespace.HasDog"));
        }

        [TestMethod]
        public void FunctionsOnCollectionTypesWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.AllHaveDog");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForAllHaveDogWithOneParameter());
        }

        [TestMethod]
        public void FunctionsAreComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.GetMyDog/Color");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
                {
                    s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                    s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 1)),
                    s => s.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForGetMyDog()),
                    s => s.ShouldBePropertySegment(HardCodedTestModel.GetDogColorProp())
                });
        }

        [TestMethod]
        public void FunctionWithBogusBracketsThrows()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={}})", ODataErrorStrings.ExpressionLexer_InvalidCharacter("}", "10", "address={}}"));
        }

        [TestMethod]
        public void FunctionBoundToPrimitiveCannotBeInvoked()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/ID/IsPrime()", ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("ID", "IsPrime()"));
        }

        [TestMethod]
        public void FunctionBoundToComplexWorks()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var path = PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/Color/Test.IsDark()", model);

            path.Should().HaveCount(4);
            path.LastSegment.ShouldBeOperationSegment(model.FindOperations("Test.IsDark").Single()).And.Operations.Should().HaveCount(1);
        }

        // Fix ODataUri parser to support composable Functions on Complex, it can also be supported when we don't track the entity set as well.
        [Ignore]
        [TestMethod]
        public void FunctionBoundToComplexAreComposable()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var path = PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/Color/GetMostPopularVegetableWithThisColor/ID", model);

            path.Should().HaveCount(5);
            path.LastSegment.ShouldBePropertySegment(model.FindType("Test.Vegetable").As<IEdmEntityType>().Properties().Single(p => p.Name == "ID"));
        }

        [TestMethod]
        public void FunctionBoundToComplexWithParametersWorks()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var path = PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/Color/Test.IsDarkerThan(other={\"Blue\":128})", model);

            path.Should().HaveCount(4);
            var isDarkerThanFunction = model.FindOperations("Test.IsDarkerThan").ToList().FirstOrDefault();
            path.LastSegment.ShouldBeOperationSegment(isDarkerThanFunction).And.Operations.Should().HaveCount(1);
        }

        // JSON (light) literals

        [TestMethod]
        public void FunctionWithComplexParameterThatUsesSingleQuotesInsteadOfDoubleWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={'Street' : 'stuff', 'City' : 'stuff'})");
            result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddress()).
                And.Parameters.Single().Value.As<ConstantNode>().Value.ShouldBeODataComplexValue().WithTypeName("Fully.Qualified.Namespace.Address").And.Properties.Should().HaveCount(2);
        }

        [TestMethod]
        public void FunctionWithComplexParameterInJsonWithTypeNameWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={\"@odata.type\":\"Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"})");
            result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddress()).
                And.Parameters.Single().Value.As<ConstantNode>().Value.ShouldBeODataComplexValue().WithTypeName("Fully.Qualified.Namespace.Address").And.Properties.Should().HaveCount(2);
        }

        [TestMethod]
        public void FunctionWithComplexParameterInJsonWithNoTypeNameWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"})");
            result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddress()).
                And.Parameters.Single().Value.As<ConstantNode>().Value.ShouldBeODataComplexValue().WithTypeName("Fully.Qualified.Namespace.Address").And.Properties.Should().HaveCount(2);
        }

        [TestMethod]
        public void FunctionWithComplexParameterInJsonWithBadPropertiesThrows()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={\"@odata.type\":\"Fully.Qualified.Namespace.Address\",\"NotTheRightProperty\":\"wut\"})");
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ValidationUtils_PropertyDoesNotExistOnType("NotTheRightProperty", "Fully.Qualified.Namespace.Address"));
        }

        [TestMethod]
        public void FunctionWithCollectionParameterInJsonWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.OwnsTheseDogs(dogNames=[\"Barky\",\"Junior\"])");
            var parameterValue = result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForOwnsTheseDogs()).And.Parameters.Single().Value;
            parameterValue.As<ConstantNode>().Value.Should().BeOfType<ODataCollectionValue>();
            parameterValue.As<ConstantNode>().LiteralText.Should().Be("[\"Barky\",\"Junior\"]");
            parameterValue.As<ConstantNode>().Value.As<ODataCollectionValue>().Items.Should().Contain("Barky").And.Contain("Junior");
        }

        [TestMethod]
        public void FunctionWithCollectionOfComplexParameterInJsonWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddresses(addresses=[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}])");
            var parameterValue = result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddresses()).And.Parameters.Single();
            parameterValue.As<OperationSegmentParameter>().Value.As<ConstantNode>().Value.Should().BeOfType<ODataCollectionValue>();
            parameterValue.As<OperationSegmentParameter>().Value.As<ConstantNode>().Value.As<ODataCollectionValue>().Items.Should().HaveCount(2);
        }

        [TestMethod]
        public void AmbiguousFunctionCallThrows()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/Test.Foo(p2='1')", ModelBuildingHelpers.GetModelWithFunctionOverloadsWithSameParameterNames());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound("Test.Foo", "p2"));
        }

        [TestMethod]
        public void ActionBoundToComplexTypeWorks()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyAddress/Fully.Qualified.Namespace.ChangeState");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetChangeStateAction());
        }

        [TestMethod]
        public void ActionBoundToPrimitiveThrows()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/ID/Subtract", ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("ID", "Subtract"));
        }

        [TestMethod]
        public void FunctionWithExpressionParameterThrows()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.OwnsTheseDogs(dogNames=Dogs(0))", ODataErrorStrings.MetadataBinder_UnknownFunction("Dogs"));
        }

        [TestMethod]
        public void FunctionWithMultipleParametersWithTheSameNameThrows()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Foo(p2='stuff', p2='1')", ModelBuildingHelpers.GetModelWithFunctionWithDuplicateParameterNames());
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
        }

        #endregion

        [TestMethod]
        public void StructuralPropertyOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Birthdate");
            path.LastSegment.ShouldBePropertySegment(HardCodedTestModel.GetPersonBirthdateProp()).
                And.EdmType.Should().BeSameAs(HardCodedTestModel.GetPersonBirthdateProp().Type.Definition);
        }

        [TestMethod]
        public void OpenPropertyOnOpenEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(1)/SomeOpenProp");
            path.LastSegment.ShouldBeOpenPropertySegment("SomeOpenProp")
                .And.EdmType.Should().BeNull();
        }

        [TestMethod]
        public void OpenPropertyOnOpenComplex()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyOpenAddress/SomeOpenProp");
            path.LastSegment.ShouldBeOpenPropertySegment("SomeOpenProp")
                .And.EdmType.Should().BeNull();
        }

        [TestMethod]
        public void NavigationPropertyOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyDog");
            path.LastSegment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp()).
                And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet());
            path.NavigationSource().Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void CollectionNavigationPropertyWithMissingEntitySetShouldNotThrow()
        {
            var model = ModelBuildingHelpers.GetTestModelForNavigationPropertyBinding();
            var path = new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Vegetables(1)/GenesModified")).ParsePath();
            Assert.AreEqual(path.LastSegment.Identifier, "GenesModified");
        }

        [TestMethod]
        public void SingletonNonNullableNavigationPropertyWithMissingEntitySetShouldThrow()
        {
            var model = ModelBuildingHelpers.GetTestModelForNavigationPropertyBinding();
            Action parse = () => new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Vegetables(1)/KeyGene")).ParsePath();
            parse.ShouldThrow<ODataException>().WithMessage("The target Entity Set of Navigation Property 'KeyGene' could not be found. This is most likely an error in the IEdmModel.").
                And.GetType().Should().Be<ODataException>();
        }

        [TestMethod]
        public void SingletonNullableNavigationPropertyWithMissingEntitySetShouldNotThrow()
        {
            var model = ModelBuildingHelpers.GetTestModelForNavigationPropertyBinding();
            var path = new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Vegetables(1)/DefectiveGene")).ParsePath();
            Assert.AreEqual(path.LastSegment.Identifier, "DefectiveGene");
        }

        [TestMethod]
        public void ActionImportWithNoReturnEntitySet()
        {
            var model = ModelBuildingHelpers.GetModelWithActionWithMissingReturnSet();

            // We want a descriptive error message, and do NOT want a ODataUriParserException so the service implementor does not blindly surface this to users
            var path = new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/ActionWithMissingReturnSet")).ParsePath();
            path.LastSegment.ShouldBeOperationImportSegment(model.FindDeclaredOperationImports("ActionWithMissingReturnSet").Single());
        }

        [TestMethod]
        public void ServiceOperationWithNoReturnEntitySet()
        {
            var model = ModelBuildingHelpers.GetModelWithServiceOperationWithMissingReturnSet();

            // We want a descriptive error message, and do NOT want a ODataUriParserException so the service implementor does not blindly surface this to users
            var path = new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/GetVegetableWithMissingSet")).ParsePath();
            path.LastSegment.ShouldBeOperationImportSegment(model.FindDeclaredOperationImports("GetVegetableWithMissingSet").Single());
        }

        [TestMethod]
        public void CastShouldBeAllowedOnSingleEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(2)/Fully.Qualified.Namespace.Employee");
            path.LastSegment.ShouldBeTypeSegment(HardCodedTestModel.GetEmployeeType()).
                And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
            path.NavigationSource().Should().Be(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void CastShouldBeAllowedOnEntityCollection()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.Employee");
            path.LastSegment.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetEmployeeType(), false))).
                And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void CastShouldBeAllowedOnSingleComplexValueCollection()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyAddress/Fully.Qualified.Namespace.HomeAddress");
            path.LastSegment.ShouldBeTypeSegment(HardCodedTestModel.GetHomeAddressType());
        }

        [TestMethod]
        public void CastShouldNotBeAllowedOnComplexValueCollection()
        {
            PathFunctionalTestsUtil.RunParseErrorPath(
                "People(1)/PreviousAddresses/Fully.Qualified.Namespace.HomeAddress",
                Strings.PathParser_TypeCastOnlyAllowedAfterEntityCollection("Fully.Qualified.Namespace.HomeAddress"));
        }

        [TestMethod]
        public void InvalidCastShouldNotBeAllowedOnSingleEntity()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(2)/Fully.Qualified.Namespace.Person", ODataErrorStrings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType("Fully.Qualified.Namespace.Person", HardCodedTestModel.GetDogType().FullName()));
        }

        [TestMethod]
        public void InvalidCastShouldNotBeAllowedOnEntityCollection()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs/Fully.Qualified.Namespace.Person", ODataErrorStrings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType("Fully.Qualified.Namespace.Person", HardCodedTestModel.GetDogType().FullName()));
        }

        [TestMethod]
        public void KeyLookupsCanBeOnCollectionNavigationProperties()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)/MyPeople(2)");
            path.LastSegment.ShouldBeSimpleKeySegment(2)
                .And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
            path.NavigationSource().Should().Be(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void MultipartKeyLookup()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Lions(ID1=32, ID2=64)");
            path.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetLionSet());
            path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 32), new KeyValuePair<string, object>("ID2", 64));
        }

        [TestMethod]
        public void KeysExpressionsCanHaveWhitespace()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs( ID  =  1 )");
            path.LastSegment.ShouldBeSimpleKeySegment(1);
        }

        [TestMethod]
        public void KeysDuplicatedError()
        {
            Action action = () => PathFunctionalTestsUtil.RunParsePath("Dogs( ID  =  1, ID  =  1)");
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
        }

        [TestMethod]
        public void TypeCastOnCollectionHasCollectionType()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.Employee");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                s => s.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetEmployeeType(), false))),
            });
        }

        [TestMethod]
        public void KeysExpressionsCanAppearOnTypeSegments()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.Employee(1)");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                s => s.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetEmployeeType(), false))),
                s => s.ShouldBeSimpleKeySegment(1)
            });
        }

        [TestMethod]
        public void DerivedPropertyAccessAfterKeysExpressionsOnTypeSegmentIsOk()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.Manager(1)/NumberOfReports");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                s => s.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetManagerType(), false))),
                s => s.ShouldBeSimpleKeySegment(1),
                s => s.ShouldBePropertySegment(HardCodedTestModel.GetManagerNumberOfReportsProp())
            });
        }

        [TestMethod]
        public void MultipartKeysExpressionsCanHaveWhitespace()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Lions( ID1 = 32  ,   ID2   =    64    )");
            path.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetLionSet());
            path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 32), new KeyValuePair<string, object>("ID2", 64));
        }

        [TestMethod]
        public void NumberOfKeyExpressionsCanBeLessThanNumberOfDeclaredKeysIfPreviousSegmentHasReferentialIntegrityConstraint()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(32)/MyLions(64)");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 32)),
                s => s.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyLionsNavProp()),
                s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 32), new KeyValuePair<string, object>("ID2", 64))
            });
        }

        [TestMethod]
        public void ExplicitKeysCanBeNamed()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(32)/MyLions(ID2=64)");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 32)),
                s => s.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyLionsNavProp()),
                s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 32), new KeyValuePair<string, object>("ID2", 64))
            });
        }

        [TestMethod]
        public void IfKeyIsExplicitlySetToValueOfImplicitKeyThrowError()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(32)/MyLions(ID1=64)", ODataErrorStrings.BadRequest_KeyCountMismatch(HardCodedTestModel.GetLionType().FullName()));
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterBatchReference()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$42(notgood)"));
            parser.BatchReferenceCallback = contentId => new BatchReferenceSegment(contentId, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            Action parse = () => parser.ParsePath();

            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void EntitySetKeyWithOpertionalSuffix()
        {
            // long
            PathFunctionalTestsUtil.RunParsePath("Pet1Set(102)/").LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 102L));
            PathFunctionalTestsUtil.RunParsePath("Pet1Set(102l)/").LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 102L));

            // single
            PathFunctionalTestsUtil.RunParsePath("Pet2Set(102.0)/").LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 102F));
            PathFunctionalTestsUtil.RunParsePath("Pet2Set(102.0F)/").LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 102f));

            // double
            PathFunctionalTestsUtil.RunParsePath("Pet3Set(12.0)/").LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 12d));
            PathFunctionalTestsUtil.RunParsePath("Pet3Set(12d)/").LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 12D));

            // decimal
            PathFunctionalTestsUtil.RunParsePath("Pet4Set(102.0)/").LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 102m));
            PathFunctionalTestsUtil.RunParsePath("Pet4Set(102m)/").LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 102M));
        }

        [TestMethod]
        public void EntitySetKeyWithUnmatchType()
        {
            // long
            PathFunctionalTestsUtil.RunParseErrorPath("Pet1Set(102F)/", Strings.RequestUriProcessor_SyntaxError);
            PathFunctionalTestsUtil.RunParseErrorPath("Pet1Set(9223372036854775808)/" /*bigger than long*/, Strings.RequestUriProcessor_SyntaxError);

            // single
            PathFunctionalTestsUtil.RunParseErrorPath("Pet2Set(102.0D)/", Strings.RequestUriProcessor_SyntaxError);
            PathFunctionalTestsUtil.RunParseErrorPath("Pet2Set(3402823000000000000000000000000000000000)/" /*bigger than Single*/, Strings.RequestUriProcessor_SyntaxError);

            // double
            PathFunctionalTestsUtil.RunParseErrorPath("Pet3Set(12M)/", Strings.RequestUriProcessor_SyntaxError);

            // decimal
            PathFunctionalTestsUtil.RunParseErrorPath("Pet4Set(102F)/", Strings.RequestUriProcessor_SyntaxError);
            PathFunctionalTestsUtil.RunParseErrorPath("Pet4Set(79228162514264337593543950336)/", Strings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void FunctionParameterWithOpertianalSuffix()
        {
            // long
            PathFunctionalTestsUtil.RunParsePath("GetPet1(id=102)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet1())
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter("id", 102L);
            PathFunctionalTestsUtil.RunParsePath("GetPet1(id=102L)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet1())
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter("id", 102L);

            // float
            PathFunctionalTestsUtil.RunParsePath("GetPet2(id=102)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet2())
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter("id", 102F);
            PathFunctionalTestsUtil.RunParsePath("GetPet2(id=102F)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet2())
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter("id", 102F);

            // double
            PathFunctionalTestsUtil.RunParsePath("GetPet3(id=102.0)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet3())
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter("id", 102D);
            PathFunctionalTestsUtil.RunParsePath("GetPet3(id=102D)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet3())
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter("id", 102D);

            // decimal
            PathFunctionalTestsUtil.RunParsePath("GetPet4(id=102.0)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4())
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter("id", 102M);
            PathFunctionalTestsUtil.RunParsePath("GetPet4(id=102M)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4())
                .And.ShouldHaveParameterCount(1)
                .And.ShouldHaveConstantParameter("id", 102M);
        }

        [TestMethod]
        public void FunctionParameterDoublePrecision()
        {
            PathFunctionalTestsUtil.RunParsePath("GetPet3(id=1.0099999904632568)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet3()).And.ShouldHaveParameterCount(1).And.ShouldHaveConstantParameter("id", 1.0099999904632568D);
        }

        [TestMethod]
        public void FunctionParameterSinglePrecision()
        {
            PathFunctionalTestsUtil.RunParsePath("GetPet2(id=-3.40282347E+38)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet2()).And.ShouldHaveParameterCount(1).And.ShouldHaveConstantParameter("id", -3.40282347E+38F);
        }

        [TestMethod]
        public void FunctionParameterDecimalBound()
        {
            PathFunctionalTestsUtil.RunParsePath("GetPet4(id=79228162514264337593543950335)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).And.ShouldHaveParameterCount(1).And.ShouldHaveConstantParameter("id", 79228162514264337593543950335M);
            PathFunctionalTestsUtil.RunParsePath("GetPet4(id=-79228162514264337593543950335)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).And.ShouldHaveParameterCount(1).And.ShouldHaveConstantParameter("id", -79228162514264337593543950335M);
        }

        [TestMethod]
        public void FunctionParameterBooleanTrue()
        {
            PathFunctionalTestsUtil.RunParsePath("GetPet5(id=true)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet5()).And.ShouldHaveParameterCount(1).And.ShouldHaveConstantParameter("id", true);

            PathFunctionalTestsUtil.RunParsePath("GetPet5(id=false)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet5()).And.ShouldHaveParameterCount(1).And.ShouldHaveConstantParameter("id", false);

            PathFunctionalTestsUtil.RunParseErrorPath("GetPet5(id=1)", ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.Int32", "Edm.Boolean"));
        }

        [TestMethod]
        public void FunctionParameterWithUnmatchType()
        {
            // long
            PathFunctionalTestsUtil.RunParseErrorPath("GetPet1(id=102F)", ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.Single", "Edm.Int64"));
            PathFunctionalTestsUtil.RunParseErrorPath("GetPet1(id=9223372036854775808)" /*bigger than long*/, ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.Decimal", "Edm.Int64"));

            // single
            PathFunctionalTestsUtil.RunParseErrorPath("GetPet2(id=102.0D)", ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.Double", "Edm.Single"));
            PathFunctionalTestsUtil.RunParseErrorPath("GetPet2(id=3402823000000000000000000000000000000000)" /*bigger than Single*/, ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.Double", "Edm.Single"));

            // double
            PathFunctionalTestsUtil.RunParseErrorPath("GetPet3(id=12M)", ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.Decimal", "Edm.Double"));

            // decimal
            // TODO: Whether different type should throw exception even when 102F can be promoted to 102M?
            //PathFunctionalTestsUtil.RunParseErrorPath("GetPet4(id=102F)", Strings.ODataUriUtils_ConvertFromUriLiteralTypeVerificationFailure("Edm.Decimal", "102"));

            Action parse = () => PathFunctionalTestsUtil.RunParsePath("GetPet4(id=79228162514264337593543950336)");
            parse.ShouldThrow<OverflowException>();
        }

        [TestMethod]
        public void KeyLookupCannotAppearTwiceInARow()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)(2)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterStructuralProperty()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/Color(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterMetadata()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("$metadata(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterCount()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/MyPeople/$count(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterNamedStream()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/NamedStream(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterVoidServiceOperation()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetNothing(1)", ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("GetNothing"));
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterNonComposableFunctionWithoutParameters()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogsNonComposable(1)", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.AllMyFriendsDogsNonComposable"));
        }

        [TestMethod]
        public void KeyLookupCanAppearAfterComposableFunctionWithoutParameters()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetCoolPeople(1)");
            path.FirstSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolPeople());
        }

        [TestMethod]
        public void KeyLookupCannotAppearAfterFunctionWithParameters()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs(inOffice=true)(1)", ODataErrorStrings.ExpressionLexer_SyntaxError(14, "inOffice=true)(1"));
        }

        [TestMethod]
        public void BatchRequest()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("$batch");
            path.LastSegment.ShouldBeBatchSegment();
            path.NavigationSource().Should().BeNull();
        }

        [TestMethod]
        public void BatchCannotAppearAfterSomethingElse()
        {
            // TODO: Error message isn't great, could improve
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs/$batch", ODataErrorStrings.RequestUriProcessor_CannotQueryCollections("Dogs"));
        }

        [TestMethod]
        public void CountOnEntitySetIsValid()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs/$count");
            path.LastSegment.ShouldBeCountSegment();
        }

        [TestMethod]
        public void CountOnCollectionReturnedByFunctionIsValid()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(0)/Fully.Qualified.Namespace.AllMyFriendsDogs/$count");
            path.LastSegment.ShouldBeCountSegment();
        }

        [TestMethod]
        public void CountCanAppearOnCollectionNavigationProperties()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)/MyPeople/$count");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet()),
                s => s.ShouldBeSimpleKeySegment(1),
                s => s.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp()),
                s => s.ShouldBeCountSegment()
            });
        }

        [TestMethod]
        public void CountCannotAppearOnSingleNavigationProperties()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/MyDog/$count",
                "The request URI is not valid. $count cannot be applied to the segment 'MyDog' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [TestMethod]
        public void ValidMetadataRequest()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("$metadata");
            path.LastSegment.ShouldBeMetadataSegment();
            path.NavigationSource().Should().BeNull();
        }

        [TestMethod]
        public void NothingCanAppearAfterMetadata()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("$metadata/Dogs", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$metadata"));
        }

        [TestMethod]
        public void MetadataCannotAppearAfterAnotherSegment()
        {
            // TODO: We can improve error message drastically when we refactor path parsing
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/$metadata", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("$metadata"));
        }

        [TestMethod]
        public void KeyLookupOnSingleTypeCastIsInvalid()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.Employee(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Ignore]
        [TestMethod]
        public void ComplexTypesCannotBeCasted()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/MyAddress/Fully.Qualified.Namespace.Address", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("Fully.Qualified.Namespace.Address"));
        }

        [TestMethod]
        public void ValueRequestOnPrimitivePropertyIsValid()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)/Color/$value");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet()),
                s => s.ShouldBeSimpleKeySegment(1),
                s => s.ShouldBePropertySegment(HardCodedTestModel.GetDogColorProp()),
                s => s.ShouldBeValueSegment()
            });
        }

        [TestMethod]
        public void ValueRequestOnComplexPropertyIsValid()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyAddress/$value");
            path.LastSegment.ShouldBeValueSegment();
            path.NavigationSource().Should().BeNull();
        }

        [TestMethod]
        public void ValueRequestOnServiceRootIsInvalid()
        {
            // TODO: improve error message wehn refactoring / cleaning up code
            PathFunctionalTestsUtil.RunParseErrorPath("$value", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("$value"));
        }

        [TestMethod]
        public void NothingCanAppearAfterValue()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/MyAddress/$value/City", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$value"));
        }

        [TestMethod]
        public void NothingCanAppearAfterEnumValue()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Pet2Set(1)/PetColorPattern/$value/City", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$value"));
        }

        [TestMethod]
        public void ReservedWordsAreCaseSensitive()
        {
            // TODO: Should the error message talk about $ being special? Would this be OK if there was a $metaDATA EntitySet? Is that allowed?
            PathFunctionalTestsUtil.RunParseErrorPath("$metaDATA", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("$metaDATA"));
        }

        [Ignore]
        [TestMethod]
        public void DirectValueServiceOperationWithKeyLookupIsInvalid()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("DirectValuePrimitiveServiceOperation(ID='Bob')", "TODO");
        }

        [TestMethod]
        public void SystemQueryOptionsThatDoNotBelongInPathAreBlocked()
        {
            // TODO: Should the error message talk about $ being special? 
            PathFunctionalTestsUtil.RunParseErrorPath("$top", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("$top"));
        }

        [TestMethod]
        public void BatchReferenceCallbackIsUsed()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$42/MyPeople"));
            parser.BatchReferenceCallback = contentId =>
            {
                contentId.Should().Be("$42");
                return new BatchReferenceSegment(contentId, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            };
            var path = parser.ParsePath();

            path.FirstSegment.ShouldBeBatchReferenceSegment(HardCodedTestModel.GetDogType())
                .And.ContentId.Should().Be("$42");
            path.LastSegment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp());
            path.FirstSegment.TranslateWith(new DetermineNavigationSourceTranslator()).Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [Ignore]
        [TestMethod]
        public void ValueOnEntityThatisNotMLEShouldThrow()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Dogs(1)/$value");

            parse.ShouldThrow<ODataException>().WithMessage("TODO");
        }

        [TestMethod]
        public void ValueOnCollectionShouldThrow()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Dogs(1)/Nicknames/$value");

            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.PathParser_CannotUseValueOnCollection);
        }

        [TestMethod]
        public void ValueOnEntityCollectionShouldThrow()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Dogs/$value");

            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.PathParser_CannotUseValueOnCollection);
        }

        [TestMethod]
        public void AccessNamedStream()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(-31)/NamedStream");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet()),
                s => s.ShouldBeSimpleKeySegment(-31),
                s => s.ShouldBePropertySegment(HardCodedTestModel.GetDogNamedStream())
            });
        }

        [TestMethod]
        public void PropertiesOnOpenPropertiesBecomeOpenProperties()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(-415)/OpenOne/OpenTwo/OpenThree");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPaintingsSet()),
                s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", -415)),
                s => s.ShouldBeOpenPropertySegment("OpenOne"),
                s => s.ShouldBeOpenPropertySegment("OpenTwo"),
                s => s.ShouldBeOpenPropertySegment("OpenThree")
            });
        }

        [TestMethod]
        public void PropertyWithPeriodsInNameIsFound()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Prop.With.Periods");
            path.LastSegment.ShouldBePropertySegment(HardCodedTestModel.GetPersonPropWithPeriods());
            path.NavigationSource().Should().BeNull();
        }

        [TestMethod]
        public void SegmentWithPeriodsOnOpenTypeIsAProperty()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(1)/Not.A.Type.Or.Operation");
            path.LastSegment.ShouldBeOpenPropertySegment("Not.A.Type.Or.Operation");
            path.NavigationSource().Should().BeNull();
        }

        [TestMethod]
        public void SegmentsWithPeriodsAfterOpenPropertyIsAnOpenProperty()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(-415)/OpenProperty/Not.A.Type.Or.Operation");
            path.LastSegment.ShouldBeOpenPropertySegment("Not.A.Type.Or.Operation");
        }

        /// <summary>
        /// Note that in V4 we will want to change the behavior so type casts win since there are open navigations
        /// and inheritance of complex types.
        /// </summary>
        [TestMethod]
        public void InV3WeCreateOpenPropertiesOverTypeSegments()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(-415)/OpenProperty/Fully.Qualified.Namespace.Person");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPaintingsSet()),
                s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", -415)),
                s => s.ShouldBeOpenPropertySegment("OpenProperty"),
                s => s.ShouldBeOpenPropertySegment("Fully.Qualified.Namespace.Person"),
            });
        }

        [TestMethod]
        public void UriOverloadSmokeTest()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("https://www.tomatosoup.com:1234/OData/V3/", UriKind.Absolute), new Uri("https://www.tomatosoup.com:1234/OData/V3/Dogs"));
            var path = parser.ParsePath();

            path.Should().HaveCount(1);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void UriOverloadWithDifferentHostShouldBeOk()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("https://www.tomatosoup.com:1234/OData/V3/", UriKind.Absolute), new Uri("https://www.differentwebsite.com:1234/OData/V3/Dogs"));
            Action parse = () => parser.ParsePath();

            parse.ShouldNotThrow();
        }

        [TestMethod]
        public void UriOverloadWithBadUriShouldThrow()
        {
            var serviceRoot = new Uri("https://www.tomatosoup.com:1234/OData/V3/", UriKind.Absolute);
            var path = new Uri("https://www.tomatosoup.com:1234/NoData/SomethingElse/Dogs");

            var parser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, path);
            Action parse = () => parser.ParsePath();

            parse.ShouldThrow<Exception>().WithMessage(ODataErrorStrings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(path, serviceRoot));
        }

        [TestMethod]
        public void UriOverloadWithoutServiceRootShouldThrow()
        {
            Action parse = () => new ODataUriParser(HardCodedTestModel.TestModel, null, new Uri("https://www.tomatosoup.com:1234/OData/V3/Dogs"));

            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.UriParser_NeedServiceRootForThisOverload);
        }

        [TestMethod]
        public void TrailingDollarSegmentIsIgnored()
        {
            // regression test for:[UriParser] Trailing $ lost
            var serviceRoot = new Uri("https://www.blah.org/OData/");
            var path = new Uri("https://www.blah.org/OData/People/$");
            var parsedPath = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, path).ParsePath();

            parsedPath.Single().ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void FunctionsOnCollectionsWithParametersWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForAllHaveDogWithTwoParameters());
        }

        [TestMethod]
        public void CannotExplicitlyAddBindingParameterToFunction()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.HasDog(person=$it)", Strings.RequestUriProcessor_ResourceNotFound("Fully.Qualified.Namespace.HasDog"));
        }

        [TestMethod]
        public void CannotAddEntityAsBindingParameterToFunction()
        {
            // bindable functions don't require the first parameter be specified, since its already implied in the path.
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.HasDog(person=People(1))", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("Fully.Qualified.Namespace.HasDog"));
        }

        [TestMethod]
        public void LongFunctionChain()
        {
            // in this case all I really care about is that it doesn't throw... baselining this is overkill.
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs()/Fully.Qualified.Namespace.OwnerOfFastestDog()/MyDog/MyPeople/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)");
            path.Count.Should().Be(7);
        }

        [TestMethod]
        public void FunctionBindingFailsIfParameterNameIsWronglyCased()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Fully.Qualified.Namespace.HasDog(inOfFiCe=true)", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("Fully.Qualified.Namespace.HasDog"));
        }

        [TestMethod]
        public void FunctioImportnWithoutBindingParameterShouldWorkInPath()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("FindMyOwner(dogsName='fido')");
            path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner());
        }

        [TestMethod]
        public void GeometryAndNullParameterValuesShouldWorkInPath()
        {
            var point = GeometryPoint.Create(1, 2);
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(0)/Fully.Qualified.Namespace.GetColorAtPosition(position=geometry'" + SpatialHelpers.WriteSpatial(point) + "',includeAlpha=null)");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetColorAtPositionFunction())
                .And.ShouldHaveConstantParameter("position", point)
                .And.ShouldHaveConstantParameter("includeAlpha", (object)null);
        }

        [TestMethod]
        public void GeographyAndNullParameterValuesShouldWorkInPath()
        {
            var point = GeographyPoint.Create(1, 2);
            var path = PathFunctionalTestsUtil.RunParsePath("People(0)/Fully.Qualified.Namespace.GetNearbyPriorAddresses(currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=null)");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetNearbyPriorAddressesFunction())
                .And.ShouldHaveConstantParameter("currentLocation", point)
                .And.ShouldHaveConstantParameter("limit", (object)null);
        }

        [TestMethod]
        public void ExceptionShouldThrowForInvalidParameter()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldgook/"), new Uri("http://gobbldgook/GetCoolPeople(id=test, limit=1)"));
            Action action = () => parser.ParsePath();
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_ParameterNotInScope("id=test"));
        }

        #region enum property in path
        [TestMethod]
        public void EnumPropertyOfEntity()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("Pet2Set(1)/PetColorPattern");
            path.Count.Should().Be(3);
            List<ODataPathSegment> segments = path.ToList();
            segments[2].TargetKind.Should().Be(Microsoft.OData.Core.UriParser.TreeNodeKinds.RequestTargetKind.Enum);
            segments[2].EdmType.Should().Be(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern"));
        }

        [TestMethod]
        public void EnumPropertyValueOfEntity()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("Pet2Set(1)/PetColorPattern/$value");
            path.Count.Should().Be(4);
            List<ODataPathSegment> segments = path.ToList();
            segments[2].TargetKind.Should().Be(Microsoft.OData.Core.UriParser.TreeNodeKinds.RequestTargetKind.Enum);
            segments[2].EdmType.Should().Be(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern"));
            segments[3].TargetKind.Should().Be(Microsoft.OData.Core.UriParser.TreeNodeKinds.RequestTargetKind.EnumValue);
            segments[3].EdmType.Should().Be(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern"));
        }
        #endregion

        #region enum parameter in path
        [TestMethod]
        public void ParsePath_NullableEnumInFunction()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("GetPetCountNullable(colorPattern=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')");
            path.Count.Should().Be(1);
            var paramNode = path.Single().As<OperationImportSegment>().Parameters.Single().As<OperationSegmentParameter>();
            paramNode.Name.Should().Be("colorPattern");
            paramNode.Value.As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
            paramNode.Value.As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("22"); // BlueYellowStriped
        }

        [TestMethod]
        public void ParsePath_EnumInFunction()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("GetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')");
            path.Count.Should().Be(1);
            var paramNode = path.Single().As<OperationImportSegment>().Parameters.Single().As<OperationSegmentParameter>();
            paramNode.Name.Should().Be("colorPattern");
            paramNode.Value.As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
            paramNode.Value.As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("22"); // BlueYellowStriped
        }

        [TestMethod]
        public void ParsePath_EnumInFunction_undefined()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("GetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'99999222')");
            path.Count.Should().Be(1);
            var paramNode = path.Single().As<OperationImportSegment>().Parameters.Single().As<OperationSegmentParameter>();
            paramNode.Name.Should().Be("colorPattern");
            paramNode.Value.As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
            paramNode.Value.As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("99999222");
        }
        #endregion

        #region enum as key
        [TestMethod]
        public void ParsePath_EnumAsKey()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("PetCategories(Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')");
            path.Count.Should().Be(2);
            var keyInfo = path.Last().As<KeySegment>().Keys.Single();
            keyInfo.Key.Should().Be("PetCategorysColorPattern");
            keyInfo.Value.As<ConstantNode>().Value.As<ODataEnumValue>().TypeName.Should().Be("Fully.Qualified.Namespace.ColorPattern");
            keyInfo.Value.As<ConstantNode>().Value.As<ODataEnumValue>().Value.Should().Be("22");
        }
        #endregion

        #region type definition

        [TestMethod]
        public void KeyOfTypeDefinitionShouldWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Pet6Set(5.1)");
            path.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPet6Set());
            path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 5.1));
        }

        [TestMethod]
        public void KeyOfIncompatibleTypeDefinitionShouldFail()
        {
            Action action = () => PathFunctionalTestsUtil.RunParsePath("Pet6Set(ID='abc')");
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [TestMethod]
        public void FunctionImportWithTypeDefinitionShouldWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetPet6(id=5.1)");
            path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet6());
        }

        [TestMethod]
        public void FunctionImportWithImcompatibleTypeDefinitionShouldFail()
        {
            Action action = () => PathFunctionalTestsUtil.RunParsePath("GetPet6(id='abc')");
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.String", "Fully.Qualified.Namespace.IdType"));
        }

        #endregion
    }
}
