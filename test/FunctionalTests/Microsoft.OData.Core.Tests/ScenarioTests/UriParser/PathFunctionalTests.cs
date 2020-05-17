//---------------------------------------------------------------------
// <copyright file="PathFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Tests.ScenarioTests.UriBuilder;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Test code for end-to-end parsing of the Path.
    /// TODO: add more tests for OperationImportSegment and OperationSegment
    /// </summary>
    public class PathFunctionalTests
    {
        [Fact]
        public void SimpleEntitySet()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs");
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
            Assert.Same(HardCodedTestModel.GetDogsSet(), path.NavigationSource());
        }

        [Fact]
        public void SimpleSingleton()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Boss");
            path.LastSegment.ShouldBeSingletonSegment(HardCodedTestModel.GetBossSingleton());
            Assert.Same(HardCodedTestModel.GetBossSingleton(), path.NavigationSource());
        }

        [Fact]
        public void SimpleServiceOperation()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetCoolPeople");
            Assert.Same(HardCodedTestModel.GetPeopleSet(),
                path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolPeople()).EntitySet);
            Assert.Same(HardCodedTestModel.GetPeopleSet(), path.NavigationSource());
        }

        [Fact]
        public void SimpleNavigationPropertyLinkSegment()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(7)/MyDog/$ref");
            path.LastSegment.ShouldBeNavigationPropertyLinkSegment(HardCodedTestModel.GetPersonMyDogNavProp());
            Assert.Same(HardCodedTestModel.GetDogsSet(), path.NavigationSource());
        }

        [Fact]
        public void SimpleActionImport()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("ResetAllData");
            var result = path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForResetAllData()).
                EntitySet;
            Assert.Null(result);
        }

        [Fact]
        public void VoidServiceOperationIsNotComposable()
        {
            Action parsePath = () => PathFunctionalTestsUtil.RunParsePath("GetNothing/foo");
            parsePath.Throws<ODataException>(ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("GetNothing"));
        }

        [Fact]
        public void VoidServiceOperationShouldAllowButIgnoreEmptyParens()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetNothing()");
            path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetVoidServiceOperation());
        }

        [Fact]
        public void PrimitiveServiceOperationIsComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetSomeNumber/$value");
            Assert.IsType<OperationImportSegment>(path.FirstSegment);
            path.LastSegment.ShouldBeValueSegment();
        }

        [Fact]
        public void PrimitiveServiceOperationShouldAllowButIgnoreEmptyParens()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetSomeNumber()");
            Assert.IsType<OperationImportSegment>(path.LastSegment);
        }

        [Fact]
        public void PrimitiveServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            Action parsePath = () => PathFunctionalTestsUtil.RunParsePath("GetSomeNumber/foo");
            parsePath.Throws<ODataUnrecognizedPathException>(ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("GetSomeNumber", "foo"));
        }

        [Fact]
        public void ComplexServiceOperationIsComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetSomeAddress/City");
            Assert.IsType<OperationImportSegment>(path.FirstSegment);
            path.LastSegment.ShouldBePropertySegment(HardCodedTestModel.GetAddressCityProperty());
        }

        [Fact]
        public void ComplexServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            Action parsePath = () => PathFunctionalTestsUtil.RunParsePath("GetSomeAddress/foo");
            parsePath.Throws<ODataUnrecognizedPathException>(ODataErrorStrings.RequestUriProcessor_ResourceNotFound("foo"));
        }

        [Fact]
        public void EntityServiceOperationIsComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetCoolestPerson/Fully.Qualified.Namespace.Employee");
            Assert.IsType<OperationImportSegment>(path.FirstSegment);
            path.LastSegment.ShouldBeTypeSegment(HardCodedTestModel.GetEmployeeType());
        }

        [Fact]
        public void EntityServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("GetCoolestPerson/foo", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("foo"));
        }

        [Fact]
        public void EntitySetServiceOperationIsComposable()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetCoolPeople(1)");
            Assert.IsType<OperationImportSegment>(path.FirstSegment);
            var ns = path.LastSegment.ShouldBeSimpleKeySegment(1).NavigationSource;
            Assert.Same(ns, HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void EntitySetServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetCoolPeople/foo", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void PrimitiveCollectionOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetSomeNumbers/foo", ODataErrorStrings.RequestUriProcessor_CannotQueryCollections("GetSomeNumbers"));
        }

        [Fact]
        public void ComplexCollectionServiceOperationThrowsRightErrorWhenFollowedByUnrecognizedSegment()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetSomeAddresses/foo", ODataErrorStrings.RequestUriProcessor_CannotQueryCollections("GetSomeAddresses"));
        }

        [Fact]
        public void SimpleKeyLookup()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)");
            Assert.Same(HardCodedTestModel.GetDogsSet(), path.LastSegment.ShouldBeSimpleKeySegment(1).NavigationSource);
        }

        [Fact]
        public void SimpleKeyLookupWithKeysAsSegments()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Dogs/1")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash }.ParsePath();
            path.LastSegment.ShouldBeSimpleKeySegment(1);
        }

        [Fact]
        public void SimpleKeyLookupWithKeysAsSegmentsFollowedByNavigation()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/People/1/Birthdate")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash }.ParsePath();
            path.LastSegment.ShouldBePropertySegment(HardCodedTestModel.GetPersonBirthdateProp());
        }

        [Fact]
        public void UseEscapeMarkerWithTypeSegmentInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/People/$/Fully.Qualified.Namespace.Employee")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash }.ParsePath();
            path.LastSegment.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetEmployeeType(), false)));
        }

        [Fact]
        public void SimpleTemplatedKeyLookupWithKeysAsSegments()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Dogs/{k1}")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash, EnableUriTemplateParsing = true }.ParsePath();
            var keySegment = Assert.IsType<KeySegment>(path.LastSegment);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);

            var uriExpression = Assert.IsType<UriTemplateExpression>(keypair.Value);
            Assert.Equal("{k1}", uriExpression.LiteralText);
            var keyType = (keySegment.EdmType as IEdmEntityType).DeclaredKey.Single().Type;
            Assert.Same(keyType, uriExpression.ExpectedType);
        }

        [Fact]
        public void UseTemplatedKeyWithPathTemplateSegmentInKeyAsSegmentShouldWork()
        {
            var paths = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Dogs/{k1}/{k2}")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash, EnableUriTemplateParsing = true }.ParsePath().ToList();
            var keySegment = Assert.IsType<KeySegment>(paths[1]);
            KeyValuePair<string, object> keypair = keySegment.Keys.Single();
            Assert.Equal("ID", keypair.Key);

            var uriExpression = Assert.IsType<UriTemplateExpression>(keypair.Value);
            Assert.Equal("{k1}", uriExpression.LiteralText);
            var keyType = (keySegment.EdmType as IEdmEntityType).DeclaredKey.Single().Type;
            Assert.Same(keyType, uriExpression.ExpectedType);

            var pathTemplateSegment = Assert.IsType<PathTemplateSegment>(paths[2]);
            Assert.Equal("{k2}", pathTemplateSegment.LiteralText);
        }

        [Fact]
        public void UseEscapeMarkerWithTemplatedKeyTypeSegmentInKeyAsSegmentShouldBeParsedAsPathTemplateSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Dogs/$/{k1}")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash, EnableUriTemplateParsing = true }.ParsePath();
            var pathTemplateSegment = Assert.IsType<PathTemplateSegment>(path.LastSegment);
            Assert.Equal("{k1}", pathTemplateSegment.LiteralText);
        }

        [Fact]
        public void RelativeFullUriShouldBeParsed()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/service.svc"), new Uri("Dogs", UriKind.Relative));
            var path = parser.ParsePath();
            Assert.Equal(new Uri("http://gobbldygook/service.svc/"), parser.ServiceRoot);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void TrailingEscapeMarkerShouldBeIgnoredInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/People/$")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash }.ParsePath();
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void DontUseEscapeSequenceInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Users/Fully.Qualified.Namespace.User")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash }.ParsePath();
            path.LastSegment.ShouldBeSimpleKeySegment("Fully.Qualified.Namespace.User");
        }

        [Fact]
        public void UseMultipleEscapeSequencesWithCountInKeyAsSegment()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$/$/People/1/$/$/MyDog/$/$/MyPeople/$/$/$count/$/$")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash }.ParsePath();
            path.LastSegment.ShouldBeCountSegment();
            Assert.Null(path.NavigationSource());
        }

        [Fact]
        public void PathThatIsOnlyEscapeSegments()
        {
            var path = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$/$/$")) { UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash }.ParsePath();
            Assert.Empty(path);
        }

        [Fact]
        public void AutoComputeForeignKeyReversalInKeyAsSegment()
        {
            var path = new ODataUriParser(
                HardCodedTestModel.TestModel,
                new Uri("http://gobbldygook/"),
                new Uri("http://gobbldygook/Dogs/1/LionsISaw/2"))
            {
                UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash
            }.ParsePath();
            path.LastSegment.ShouldBeKeySegment(
                new KeyValuePair<string, object>("ID1", 1),
                new KeyValuePair<string, object>("ID2", 2));
        }

        [Fact]
        public void AutoComputeForeignKeyInKeyAsSegment()
        {
            var path = new ODataUriParser(
                HardCodedTestModel.TestModel,
                new Uri("http://gobbldygook/"),
                new Uri("http://gobbldygook/Dogs/1/LionsISaw/2/Friends/3"))
            {
                UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash
            }.ParsePath();
            path.LastSegment.ShouldBeKeySegment(
                new KeyValuePair<string, object>("ID1", 2),
                new KeyValuePair<string, object>("ID2", 3));
        }

        [Fact]
        public void BoundActionOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)/Fully.Qualified.Namespace.Walk");
            Assert.Null(path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetDogWalkAction()).EntitySet);
        }

        [Fact]
        public void BoundActionThatReturnsEntitiesShouldHaveSetComputedCorrectly()
        {
            // Paint action returns a collection of Paintings that the Person created
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.Paint");
            Assert.Same(path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetPersonPaintAction()).
                EntitySet, HardCodedTestModel.GetPaintingsSet());
            Assert.Same(path.NavigationSource(), HardCodedTestModel.GetPaintingsSet());
        }

        [Fact]
        public void BoundActionFromNonDefaultContainerOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.Move");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetPersonMoveAction());
        }

        [Fact]
        public void ActionOnEntityCollection()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.FireAll");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFireAllAction());
        }

        [Fact]
        public void FunctionWithoutEntitySetPath()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs_NoSet()");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetAllMyFriendsDogs_NoSet());
        }

        [Fact]
        public void CannotAddParametersToActions()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Context.MoveEveryone(streetAddress='stuff')", ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Context.MoveEveryone"));
        }

        [Fact]
        public void NothingCanComeAfterAnAction()
        {
            // TODO: We can improve error message drastically when we refactor path parsing
            // The 'Walk' action returns an Address
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/Fully.Qualified.Namespace.Walk/City", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.Walk"));
        }

        #region Functions

        [Fact]
        public void BoundFunctionOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.HasJob");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForHasJob());
        }

        [Fact]
        public void OnlyExactlyMatchingFunctionWithMultipleOverloadsIsReturned()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.HasDog");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetHasDogOverloadWithOneParameter());
        }

        [Fact]
        public void FunctionWithNamedParameter()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.HasDog(inOffice=true)");
            var opSegment = path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForEmployeeHasDogWithTwoParameters());
            opSegment.ShouldHaveParameterCount(1);
            var source = Assert.IsType<ConvertNode>(opSegment.Parameters.SingleOrDefault(p => p.Name == "inOffice").Value).Source;
            var constantNode = Assert.IsType<ConstantNode>(source);
            Assert.Equal(true, constantNode.Value);
        }

        [Fact]
        public void FunctionWithMultipleNamedParameters()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.HasDog(inOffice=true, name='Fido')");
            var opSegment = path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters());
            opSegment.ShouldHaveParameterCount(2);
            //.And.ShouldHaveConstantParameter("inOffice", true)
            opSegment.ShouldHaveConstantParameter("name", "Fido");
            var source = Assert.IsType<ConvertNode>(opSegment.Parameters.SingleOrDefault(p => p.Name == "inOffice").Value).Source;
            var constantNode = Assert.IsType<ConstantNode>(source);
            Assert.Equal(true, constantNode.Value);
        }

        [Fact]
        public void FunctionWithAliasedParameters()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.HasDog(inOffice=@x, name=@y)");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetHasDogOverloadForPeopleWithThreeParameters())
                .ShouldHaveParameterCount(2)
                .ShouldHaveSegmentOfParameterAliasNode("inOffice", "@x")
                .ShouldHaveSegmentOfParameterAliasNode("name", "@y");
        }

        [Fact]
        public void FunctionWithPositionalParmeterShouldThrow()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.Employee/Fully.Qualified.Namespace.HasDog(true)", ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Fully.Qualified.Namespace.HasDog"));
        }

        [Fact]
        public void FunctionWithMultiplePositionalParametersShouldThrow()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.HasDog(true, 'Fido')", ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("Fully.Qualified.Namespace.HasDog"));
        }

        [Fact]
        public void FunctionsOnCollectionTypesWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.AllHaveDog");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForAllHaveDogWithOneParameter());
        }

        [Fact]
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

        [Fact]
        public void FunctionWithBogusBracketsThrows()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={}})", ODataErrorStrings.ExpressionLexer_InvalidCharacter("}", "10", "address={}}"));
        }

        [Fact]
        public void FunctionBoundToPrimitiveCannotBeInvoked()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/ID/IsPrime()", ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes());
            parse.Throws<ODataUnrecognizedPathException>(ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("ID", "IsPrime()"));
        }

        [Fact]
        public void FunctionBoundToComplexWorks()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var path = PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/Color/Test.IsDark()", model);

            Assert.Equal(4, path.Count);
            var operationSegment = path.LastSegment.ShouldBeOperationSegment(model.FindOperations("Test.IsDark").Single());
            Assert.Single(operationSegment.Operations);
        }

        [Fact]
        public void FunctionBoundToComplexAreComposable()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var path = PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/Color/Test.GetMostPopularVegetableWithThisColor/ID", model);

            var entityType = model.FindType("Test.Vegetable") as IEdmEntityType;
            Assert.NotNull(entityType);
            Assert.Equal(5, path.Count);
            var propertySegment = path.LastSegment.ShouldBePropertySegment(entityType.Properties().Single(p => p.Name == "ID"));
        }

        [Fact]
        public void FunctionBoundToComplexWithParametersWorks()
        {
            var model = ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes();
            var path = PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/Color/Test.IsDarkerThan(other={\"Blue\":128})", model);

            Assert.Equal(4, path.Count);
            var isDarkerThanFunction = model.FindOperations("Test.IsDarkerThan").ToList().FirstOrDefault();
            var operationSegment = path.LastSegment.ShouldBeOperationSegment(isDarkerThanFunction);
            Assert.Single(operationSegment.Operations);
        }

        // JSON (light) literals
        [Fact]
        public void FunctionWithComplexParameterThatUsesSingleQuotesInsteadOfDoubleWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={'Street' : 'stuff', 'City' : 'stuff'})");
            var parameters = result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddress()).Parameters;
            var parameter = Assert.Single(parameters);
            var convertNode = Assert.IsType<ConvertNode>(parameter.Value);
            Assert.Equal("Fully.Qualified.Namespace.Address", convertNode.TypeReference.FullName());
            var constNode = Assert.IsType<ConstantNode>(convertNode.Source);
            Assert.Equal("{'Street' : 'stuff', 'City' : 'stuff'}", constNode.Value);
        }

        [Fact]
        public void FunctionWithComplexParameterInJsonWithTypeNameWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={\"@odata.type\":\"Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"})");
            var parameters = result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddress()).Parameters;
            var parameter = Assert.Single(parameters);
            var convertNode = Assert.IsType<ConvertNode>(parameter.Value);
            Assert.Equal("Fully.Qualified.Namespace.Address", convertNode.TypeReference.FullName());

            var constNode = Assert.IsType<ConstantNode>(convertNode.Source);
            Assert.Equal("{\"@odata.type\":\"Fully.Qualified.Namespace.Address\",\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"}", constNode.Value);
        }

        [Fact]
        public void FunctionWithComplexParameterInJsonWithNoTypeNameWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddress(address={\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"})");

            var parameters = result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddress()).Parameters;
            var parameter = Assert.Single(parameters);
            var convertNode = Assert.IsType<ConvertNode>(parameter.Value);
            Assert.Equal("Fully.Qualified.Namespace.Address", convertNode.TypeReference.FullName());

            var constNode = Assert.IsType<ConstantNode>(convertNode.Source);
            Assert.Equal("{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"}", constNode.Value);
        }
        
        [Fact]
        public void FunctionWithCollectionParameterInJsonWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.OwnsTheseDogs(dogNames=[\"Barky\",\"Junior\"])");
            var parameters = result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForOwnsTheseDogs()).Parameters;
            var parameter = Assert.Single(parameters);
            var constNode = Assert.IsType<ConstantNode>(parameter.Value);

            var collectionValue = Assert.IsType<ODataCollectionValue>(constNode.Value);
            Assert.Equal("[\"Barky\",\"Junior\"]", constNode.LiteralText);
            Assert.Contains("Barky", collectionValue.Items);
            Assert.Contains("Junior", collectionValue.Items);
        }

        [Fact]
        public void FunctionWithCollectionOfComplexParameterInJsonWorks()
        {
            var result = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.CanMoveToAddresses(addresses=[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}])");

            var parameters = result.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForCanMoveToAddresses()).Parameters;
            var parameter = Assert.Single(parameters);
            var segmentParameter = Assert.IsType<OperationSegmentParameter>(parameter);
            var innerParameterNode = Assert.IsType<ConvertNode>(segmentParameter.Value);
            var constNode = Assert.IsType<ConstantNode>(innerParameterNode.Source);
            Assert.Equal("[{\"Street\":\"NE 24th St.\",\"City\":\"Redmond\"},{\"Street\":\"Pine St.\",\"City\":\"Seattle\"}]", constNode.Value);
            Assert.Equal("Collection(Fully.Qualified.Namespace.Address)", innerParameterNode.TypeReference.FullName());
        }

        [Fact]
        public void AmbiguousFunctionCallThrows()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/Test.Foo(p2='1')", ModelBuildingHelpers.GetModelWithFunctionOverloadsWithSameParameterNames());
            parse.Throws<ODataException>(ODataErrorStrings.FunctionOverloadResolver_NoSingleMatchFound("Test.Foo", "p2"));
        }

        [Fact]
        public void ActionBoundToComplexTypeWorks()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyAddress/Fully.Qualified.Namespace.ChangeState");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetChangeStateAction());
        }

        [Fact]
        public void ActionBoundToPrimitiveThrows()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Vegetables(0)/ID/Subtract", ModelBuildingHelpers.GetModelFunctionsOnNonEntityTypes());
            parse.Throws<ODataUnrecognizedPathException>(ODataErrorStrings.RequestUriProcessor_ValueSegmentAfterScalarPropertySegment("ID", "Subtract"));
        }

        [Fact]
        public void FunctionWithExpressionParameterThrows()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.OwnsTheseDogs(dogNames=Dogs(0))", ODataErrorStrings.MetadataBinder_UnknownFunction("Dogs"));
        }

        [Fact]
        public void FunctionWithMultipleParametersWithTheSameNameThrows()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Foo(p2='stuff', p2='1')", ModelBuildingHelpers.GetModelWithFunctionWithDuplicateParameterNames());
            parse.Throws<ODataException>(ODataErrorStrings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
        }

        #endregion

        [Fact]
        public void StructuralPropertyOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Birthdate");
            var segment = path.LastSegment.ShouldBePropertySegment(HardCodedTestModel.GetPersonBirthdateProp());
            Assert.Same(HardCodedTestModel.GetPersonBirthdateProp().Type.Definition, segment.EdmType);
        }

        [Fact]
        public void OpenPropertyOnOpenEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(1)/SomeOpenProp");
            var segment = path.LastSegment.ShouldBeDynamicPathSegment("SomeOpenProp");
            Assert.Null(segment.EdmType);
        }

        [Fact]
        public void OpenPropertyOnOpenComplex()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyOpenAddress/SomeOpenProp");
            var segment = path.LastSegment.ShouldBeDynamicPathSegment("SomeOpenProp");
            Assert.Null(segment.EdmType);
        }

        [Fact]
        public void NavigationPropertyOnEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyDog");
            var segment = path.LastSegment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp());
            Assert.Same(HardCodedTestModel.GetDogsSet(), segment.NavigationSource);
            Assert.Same(HardCodedTestModel.GetDogsSet(), path.NavigationSource());
        }

        [Fact]
        public void CollectionNavigationPropertyWithMissingEntitySetShouldNotThrow()
        {
            var model = ModelBuildingHelpers.GetTestModelForNavigationPropertyBinding();
            var path = new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Vegetables(1)/GenesModified")).ParsePath();
            Assert.Equal("GenesModified", path.LastSegment.Identifier);
        }

        [Fact]
        public void SingletonNonNullableNavigationPropertyWithMissingEntitySetShouldThrow()
        {
            var model = ModelBuildingHelpers.GetTestModelForNavigationPropertyBinding();
            Action parse = () => new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Vegetables(1)/KeyGene")).ParsePath();
            parse.Throws<ODataException>("The target Entity Set of Navigation Property 'KeyGene' could not be found. This is most likely an error in the IEdmModel.");
        }

        [Fact]
        public void SingletonNullableNavigationPropertyWithMissingEntitySetShouldNotThrow()
        {
            var model = ModelBuildingHelpers.GetTestModelForNavigationPropertyBinding();
            var path = new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/Vegetables(1)/DefectiveGene")).ParsePath();
            Assert.Equal("DefectiveGene", path.LastSegment.Identifier);
        }

        [Fact]
        public void ActionImportWithNoReturnEntitySet()
        {
            var model = ModelBuildingHelpers.GetModelWithActionWithMissingReturnSet();

            // We want a descriptive error message, and do NOT want a ODataUriParserException so the service implementor does not blindly surface this to users
            var path = new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/ActionWithMissingReturnSet")).ParsePath();
            path.LastSegment.ShouldBeOperationImportSegment(model.FindDeclaredOperationImports("ActionWithMissingReturnSet").Single());
        }

        [Fact]
        public void ServiceOperationWithNoReturnEntitySet()
        {
            var model = ModelBuildingHelpers.GetModelWithServiceOperationWithMissingReturnSet();

            // We want a descriptive error message, and do NOT want a ODataUriParserException so the service implementor does not blindly surface this to users
            var path = new ODataUriParser(model, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/GetVegetableWithMissingSet")).ParsePath();
            path.LastSegment.ShouldBeOperationImportSegment(model.FindDeclaredOperationImports("GetVegetableWithMissingSet").Single());
        }

        [Fact]
        public void CastShouldBeAllowedOnSingleEntity()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(2)/Fully.Qualified.Namespace.Employee");
            var typeSegment = path.LastSegment.ShouldBeTypeSegment(HardCodedTestModel.GetEmployeeType());
            Assert.Same(HardCodedTestModel.GetPeopleSet(), typeSegment.NavigationSource);
            Assert.Same(HardCodedTestModel.GetPeopleSet(), path.NavigationSource());
        }

        [Fact]
        public void CastShouldBeAllowedOnEntityCollection()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.Employee");
            var typeSegment = path.LastSegment.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetEmployeeType(), false)));
            Assert.Same(HardCodedTestModel.GetPeopleSet(), typeSegment.NavigationSource);
        }

        [Fact]
        public void CastShouldBeAllowedOnSingleComplexValueCollection()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyAddress/Fully.Qualified.Namespace.HomeAddress");
            path.LastSegment.ShouldBeTypeSegment(HardCodedTestModel.GetHomeAddressType());
        }

        [Fact]
        public void CastShouldBeAllowedOnComplexValueCollection()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/PreviousAddresses/Fully.Qualified.Namespace.HomeAddress");
            path.LastSegment.ShouldBeTypeSegment(new EdmCollectionType(new EdmComplexTypeReference(HardCodedTestModel.GetHomeAddressType(), false)));
        }

        [Fact]
        public void InvalidCastShouldNotBeAllowedOnSingleComplex()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/MyAddress/Fully.Qualified.Namespace.OpenAddress", ODataErrorStrings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType("Fully.Qualified.Namespace.OpenAddress", HardCodedTestModel.GetAddressType().FullName()));
        }

        [Fact]
        public void InvalidCastShouldNotBeAllowedOnComplexCollection()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/PreviousAddresses/Fully.Qualified.Namespace.OpenAddress", ODataErrorStrings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType("Fully.Qualified.Namespace.OpenAddress", HardCodedTestModel.GetAddressType().FullName()));
        }

        [Fact]
        public void InvalidCastShouldNotBeAllowedOnSingleEntity()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(2)/Fully.Qualified.Namespace.Person", ODataErrorStrings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType("Fully.Qualified.Namespace.Person", HardCodedTestModel.GetDogType().FullName()));
        }

        [Fact]
        public void InvalidCastShouldNotBeAllowedOnEntityCollection()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs/Fully.Qualified.Namespace.Person", ODataErrorStrings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType("Fully.Qualified.Namespace.Person", HardCodedTestModel.GetDogType().FullName()));
        }

        [Fact]
        public void KeyLookupsCanBeOnCollectionNavigationProperties()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs(1)/MyPeople(2)");
            var keySegment = path.LastSegment.ShouldBeSimpleKeySegment(2);
            Assert.Same(HardCodedTestModel.GetPeopleSet(), keySegment.NavigationSource);
            Assert.Same(HardCodedTestModel.GetPeopleSet(), path.NavigationSource());
        }

        [Fact]
        public void MultipartKeyLookup()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Lions(ID1=32, ID2=64)");
            path.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetLionSet());
            path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 32), new KeyValuePair<string, object>("ID2", 64));
        }

        [Fact]
        public void KeysExpressionsCanHaveWhitespace()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs( ID  =  1 )");
            path.LastSegment.ShouldBeSimpleKeySegment(1);
        }

        [Fact]
        public void KeysDuplicatedError()
        {
            Action action = () => PathFunctionalTestsUtil.RunParsePath("Dogs( ID  =  1, ID  =  1)");
            action.Throws<ODataException>(ODataErrorStrings.FunctionCallParser_DuplicateParameterOrEntityKeyName);
        }

        [Fact]
        public void TypeCastOnCollectionHasCollectionType()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.Employee");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                s => s.ShouldBeTypeSegment(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetEmployeeType(), false))),
            });
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void MultipartKeysExpressionsCanHaveWhitespace()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Lions( ID1 = 32  ,   ID2   =    64    )");
            path.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetLionSet());
            path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID1", 32), new KeyValuePair<string, object>("ID2", 64));
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void IfKeyIsExplicitlySetToValueOfImplicitKeyThrowError()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(32)/MyLions(ID1=64)", ODataErrorStrings.BadRequest_KeyCountMismatch(HardCodedTestModel.GetLionType().FullName()));
        }

        [Fact]
        public void KeyLookupCannotAppearAfterBatchReference()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$42(notgood)"));
            parser.BatchReferenceCallback = contentId => new BatchReferenceSegment(contentId, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            var batchReferenceCallbackClone = parser.BatchReferenceCallback;
            Assert.Equal(parser.BatchReferenceCallback, batchReferenceCallbackClone);

            Action parse = () => parser.ParsePath();

            parse.Throws<ODataException>(ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void EntityReferenceCannotAppearAfterBatchReference()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$42/$ref"));
            parser.BatchReferenceCallback = contentId =>
            {
                Assert.Equal("$42", contentId);
                return new BatchReferenceSegment(contentId, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            };

            var path = parser.ParsePath();
            path.LastSegment.ShouldBeReferenceSegment(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
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

#if !NETCOREAPP3_1
        [Fact]
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
#endif

        [Fact]
        public void FunctionParameterWithOpertianalSuffix()
        {
            // long
            PathFunctionalTestsUtil.RunParsePath("GetPet1(id=102)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet1())
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter("id", 102L);
            PathFunctionalTestsUtil.RunParsePath("GetPet1(id=102L)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet1())
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter("id", 102L);

            // float
            PathFunctionalTestsUtil.RunParsePath("GetPet2(id=102)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet2())
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter("id", 102F);
            PathFunctionalTestsUtil.RunParsePath("GetPet2(id=102F)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet2())
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter("id", 102F);

            // double
            PathFunctionalTestsUtil.RunParsePath("GetPet3(id=102.0)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet3())
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter("id", 102D);
            PathFunctionalTestsUtil.RunParsePath("GetPet3(id=102D)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet3())
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter("id", 102D);

            // decimal
            PathFunctionalTestsUtil.RunParsePath("GetPet4(id=102.0)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4())
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter("id", 102M);
            PathFunctionalTestsUtil.RunParsePath("GetPet4(id=102M)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4())
                .ShouldHaveParameterCount(1)
                .ShouldHaveConstantParameter("id", 102M);
        }

        [Fact]
        public void FunctionParameterDoublePrecision()
        {
            PathFunctionalTestsUtil.RunParsePath("GetPet3(id=1.0099999904632568)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet3()).ShouldHaveParameterCount(1).ShouldHaveConstantParameter("id", 1.0099999904632568D);
        }

#if !NETCOREAPP3_1
        [Fact]
        public void FunctionParameterSinglePrecision()
        {
            PathFunctionalTestsUtil.RunParsePath("GetPet2(id=-3.40282347E+38)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet2()).ShouldHaveParameterCount(1).ShouldHaveConstantParameter("id", -3.40282347E+38F);
        }

        [Fact]
        public void FunctionParameterDecimalBound()
        {
            PathFunctionalTestsUtil.RunParsePath("GetPet4(id=79228162514264337593543950335)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).ShouldHaveParameterCount(1).ShouldHaveConstantParameter("id", 79228162514264337593543950335M);
            PathFunctionalTestsUtil.RunParsePath("GetPet4(id=-79228162514264337593543950335)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet4()).ShouldHaveParameterCount(1).ShouldHaveConstantParameter("id", -79228162514264337593543950335M);
        }
#endif

        [Fact]
        public void FunctionParameterBooleanTrue()
        {
            PathFunctionalTestsUtil.RunParsePath("GetPet5(id=true)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet5()).ShouldHaveParameterCount(1).ShouldHaveConstantParameter("id", true);

            PathFunctionalTestsUtil.RunParsePath("GetPet5(id=false)").LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet5()).ShouldHaveParameterCount(1).ShouldHaveConstantParameter("id", false);

            PathFunctionalTestsUtil.RunParseErrorPath("GetPet5(id=1)", ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.Int32", "Edm.Boolean"));
        }

        [Fact]
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
            Assert.Throws<OverflowException>(parse);
        }

        [Fact]
        public void KeyLookupCannotAppearTwiceInARow()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)(2)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void KeyLookupCannotAppearAfterStructuralProperty()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/Color(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void KeyLookupCannotAppearAfterMetadata()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("$metadata(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void KeyLookupCannotAppearAfterCount()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/MyPeople/$count(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void KeyLookupCannotAppearAfterNamedStream()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs(1)/NamedStream(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void KeyLookupCannotAppearAfterVoidServiceOperation()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("GetNothing(1)", ODataErrorStrings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates("GetNothing"));
        }

        [Fact]
        public void KeyLookupCannotAppearAfterNonComposableFunctionWithoutParameters()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogsNonComposable(1)", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("Fully.Qualified.Namespace.AllMyFriendsDogsNonComposable"));
        }

        [Fact]
        public void KeyLookupCanAppearAfterComposableFunctionWithoutParameters()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetCoolPeople(1)");
            path.FirstSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolPeople());
        }

        [Fact]
        public void KeyLookupCannotAppearAfterFunctionWithParameters()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs(inOffice=true)(1)", ODataErrorStrings.ExpressionLexer_SyntaxError(14, "inOffice=true)(1"));
        }

        [Fact]
        public void BatchRequest()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("$batch");
            path.LastSegment.ShouldBeBatchSegment();
            Assert.Null(path.NavigationSource());
        }

        [Fact]
        public void BatchCannotAppearAfterSomethingElse()
        {
            // TODO: Error message isn't great, could improve
            PathFunctionalTestsUtil.RunParseErrorPath("Dogs/$batch", ODataErrorStrings.RequestUriProcessor_CannotQueryCollections("Dogs"));
        }

        [Fact]
        public void CountOnEntitySetIsValid()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Dogs/$count");
            path.LastSegment.ShouldBeCountSegment();
        }

        [Fact]
        public void CountOnCollectionReturnedByFunctionIsValid()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(0)/Fully.Qualified.Namespace.AllMyFriendsDogs/$count");
            path.LastSegment.ShouldBeCountSegment();
        }

        [Fact]
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

        [Fact]
        public void CountCannotAppearOnSingleNavigationProperties()
        {
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("People(1)/MyDog/$count",
                "The request URI is not valid. $count cannot be applied to the segment 'MyDog' since $count can only follow an entity set, a collection navigation property, a structural property of collection type, an operation returning collection type or an operation import returning collection type.");
        }

        [Fact]
        public void ValidMetadataRequest()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("$metadata");
            path.LastSegment.ShouldBeMetadataSegment();
            Assert.Null(path.NavigationSource());
        }

        [Fact]
        public void NothingCanAppearAfterMetadata()
        {
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("$metadata/Dogs", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$metadata"));
        }

        [Fact]
        public void MetadataCannotAppearAfterAnotherSegment()
        {
            // TODO: We can improve error message drastically when we refactor path parsing
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("People(1)/$metadata", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("$metadata"));
        }

        [Fact]
        public void KeyLookupOnSingleTypeCastIsInvalid()
        {
            PathFunctionalTestsUtil.RunParseErrorPath("People(1)/Fully.Qualified.Namespace.Employee(1)", ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void ComplexTypesCanBeCasted()
        {
            //PathFunctionalTestsUtil.RunParseErrorPath("People(1)/MyAddress/Fully.Qualified.Namespace.Address", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("Fully.Qualified.Namespace.Address"));
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyAddress/Fully.Qualified.Namespace.Address");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet()),
                s => s.ShouldBeSimpleKeySegment(1),
                s => s.ShouldBePropertySegment(HardCodedTestModel.GetPersonAddressProp()),
                s => s.ShouldBeTypeSegment(HardCodedTestModel.GetAddressType())
            });
        }

        [Fact]
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

        [Fact]
        public void ValueRequestOnComplexPropertyIsValid()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/MyAddress/$value");
            path.LastSegment.ShouldBeValueSegment();
            Assert.Null(path.NavigationSource());
        }

        [Fact]
        public void ValueRequestOnServiceRootIsInvalid()
        {
            // TODO: improve error message wehn refactoring / cleaning up code
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("$value", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("$value"));
        }

        [Fact]
        public void NothingCanAppearAfterValue()
        {
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("People(1)/MyAddress/$value/City", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$value"));
        }

        [Fact]
        public void NothingCanAppearAfterEnumValue()
        {
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("Pet2Set(1)/PetColorPattern/$value/City", ODataErrorStrings.RequestUriProcessor_MustBeLeafSegment("$value"));
        }

        [Fact]
        public void ReservedWordsAreCaseSensitive()
        {
            // TODO: Should the error message talk about $ being special? Would this be OK if there was a $metaDATA EntitySet? Is that allowed?
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("$metaDATA", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("$metaDATA"));
        }

        [Fact]
        public void DirectValueServiceOperationWithKeyLookupIsInvalid()
        {
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("DirectValuePrimitiveServiceOperation(ID='Bob')", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("DirectValuePrimitiveServiceOperation"));
        }

        [Fact]
        public void SystemQueryOptionsThatDoNotBelongInPathAreBlocked()
        {
            // TODO: Should the error message talk about $ being special? 
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("$top", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("$top"));
        }

        [Fact]
        public void BatchReferenceCallbackIsUsed()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldygook/"), new Uri("http://gobbldygook/$42/MyPeople"));
            parser.BatchReferenceCallback = contentId =>
            {
                Assert.Equal("$42", contentId);
                return new BatchReferenceSegment(contentId, HardCodedTestModel.GetDogType(), HardCodedTestModel.GetDogsSet());
            };
            var path = parser.ParsePath();

            Assert.Equal("$42", path.FirstSegment.ShouldBeBatchReferenceSegment(HardCodedTestModel.GetDogType()).ContentId);
            path.LastSegment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetDogMyPeopleNavProp());
            Assert.Same(HardCodedTestModel.GetDogsSet(), path.FirstSegment.TranslateWith(new DetermineNavigationSourceTranslator()));
        }

        //[Fact(Skip = "#833: Throw exception when $value appears after a stream.")]
        //public void ValueOnEntityThatisNotMediaLinkEntryShouldThrow()
        //{
        //    // From 4.7 of the spec (http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part2-url-conventions.html)
        //    // Properties of type Edm.Stream already return the raw value of the media stream
        //    // and do not support appending the $value segment.
        //    Action parse = () => PathFunctionalTestsUtil.RunParsePath("Dogs(1)/$value");
        //
        //    parse.ShouldThrow<ODataException>().WithMessage("TODO");
        //}

        [Fact]
        public void ValueOnCollectionShouldThrow()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Dogs(1)/Nicknames/$value");

            parse.Throws<ODataException>(ODataErrorStrings.PathParser_CannotUseValueOnCollection);
        }

        [Fact]
        public void ValueOnEntityCollectionShouldThrow()
        {
            Action parse = () => PathFunctionalTestsUtil.RunParsePath("Dogs/$value");

            parse.Throws<ODataException>(ODataErrorStrings.PathParser_CannotUseValueOnCollection);
        }

        [Fact]
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

        [Fact]
        public void PropertiesOnOpenPropertiesBecomeOpenProperties()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(-415)/OpenOne/OpenTwo/OpenThree");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPaintingsSet()),
                s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", -415)),
                s => s.ShouldBeDynamicPathSegment("OpenOne"),
                s => s.ShouldBeDynamicPathSegment("OpenTwo"),
                s => s.ShouldBeDynamicPathSegment("OpenThree")
            });
        }

        [Fact]
        public void PropertyWithPeriodsInNameIsFound()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Prop.With.Periods");
            path.LastSegment.ShouldBePropertySegment(HardCodedTestModel.GetPersonPropWithPeriods());
            Assert.Null(path.NavigationSource());
        }

        [Fact]
        public void SegmentWithPeriodsOnOpenTypeIsAProperty()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(1)/Not.A.Type.Or.Operation");
            path.LastSegment.ShouldBeDynamicPathSegment("Not.A.Type.Or.Operation");
            Assert.Null(path.NavigationSource());
        }

        [Fact]
        public void SegmentsWithPeriodsAfterOpenPropertyIsAnOpenProperty()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(-415)/OpenProperty/Not.A.Type.Or.Operation");
            path.LastSegment.ShouldBeDynamicPathSegment("Not.A.Type.Or.Operation");
        }

        /// <summary>
        /// Note that in V4 we will want to change the behavior so type casts win since there are open navigations
        /// and inheritance of complex types.
        /// </summary>
        [Fact]
        public void InV3WeCreateOpenPropertiesOverTypeSegments()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(-415)/OpenProperty/Fully.Qualified.Namespace.Person");
            VerificationHelpers.VerifyPath(path, new Action<ODataPathSegment>[]
            {
                s => s.ShouldBeEntitySetSegment(HardCodedTestModel.GetPaintingsSet()),
                s => s.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", -415)),
                s => s.ShouldBeDynamicPathSegment("OpenProperty"),
                s => s.ShouldBeDynamicPathSegment("Fully.Qualified.Namespace.Person"),
            });
        }

        [Fact]
        public void UriOverloadSmokeTest()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("https://www.tomatosoup.com:1234/OData/V3/", UriKind.Absolute), new Uri("https://www.tomatosoup.com:1234/OData/V3/Dogs"));
            var path = parser.ParsePath();

            Assert.Single(path);
            path.LastSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetDogsSet());
        }

        [Fact]
        public void UriOverloadWithDifferentHostShouldBeOk()
        {
            var parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("https://www.tomatosoup.com:1234/OData/V3/", UriKind.Absolute), new Uri("https://www.differentwebsite.com:1234/OData/V3/Dogs"));
            Action parse = () => parser.ParsePath();

            parse.DoesNotThrow();
        }

        [Fact]
        public void UriOverloadWithBadUriShouldThrow()
        {
            var serviceRoot = new Uri("https://www.tomatosoup.com:1234/OData/V3/", UriKind.Absolute);
            var path = new Uri("https://www.tomatosoup.com:1234/NoData/SomethingElse/Dogs");

            var parser = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, path);
            Action parse = () => parser.ParsePath();

            parse.Throws<ODataException>(ODataErrorStrings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(path, serviceRoot));
        }

        [Fact]
        public void UriOverloadWithoutServiceRootShouldThrow()
        {
            Action parse = () => new ODataUriParser(HardCodedTestModel.TestModel, null, new Uri("https://www.tomatosoup.com:1234/OData/V3/Dogs"));

            parse.Throws<ODataException>(ODataErrorStrings.UriParser_NeedServiceRootForThisOverload);
        }

        [Fact]
        public void TrailingDollarSegmentIsIgnored()
        {
            // regression test for:[UriParser] Trailing $ lost
            var serviceRoot = new Uri("https://www.blah.org/OData/");
            var path = new Uri("https://www.blah.org/OData/People/$");
            var parsedPath = new ODataUriParser(HardCodedTestModel.TestModel, serviceRoot, path).ParsePath();

            parsedPath.Single().ShouldBeEntitySetSegment(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void FunctionsOnCollectionsWithParametersWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("People/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)");
            path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetFunctionForAllHaveDogWithTwoParameters());
        }

        [Fact]
        public void CannotExplicitlyAddBindingParameterToFunction()
        {
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("People(1)/Fully.Qualified.Namespace.HasDog(person=$it)", Strings.RequestUriProcessor_ResourceNotFound("Fully.Qualified.Namespace.HasDog"));
        }

        [Fact]
        public void CannotAddEntityAsBindingParameterToFunction()
        {
            // bindable functions don't require the first parameter be specified, since its already implied in the path.
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("People(1)/Fully.Qualified.Namespace.HasDog(person=People(1))", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("Fully.Qualified.Namespace.HasDog"));
        }

        [Fact]
        public void LongFunctionChain()
        {
            // in this case all I really care about is that it doesn't throw... baselining this is overkill.
            var path = PathFunctionalTestsUtil.RunParsePath("People(1)/Fully.Qualified.Namespace.AllMyFriendsDogs()/Fully.Qualified.Namespace.OwnerOfFastestDog()/MyDog/MyPeople/Fully.Qualified.Namespace.AllHaveDog(inOffice=true)");
            Assert.Equal(7, path.Count);
        }

        [Fact]
        public void FunctionBindingFailsIfParameterNameIsWronglyCased()
        {
            PathFunctionalTestsUtil.RunParseErrorPath<ODataUnrecognizedPathException>("Fully.Qualified.Namespace.HasDog(inOfFiCe=true)", ODataErrorStrings.RequestUriProcessor_ResourceNotFound("Fully.Qualified.Namespace.HasDog"));
        }

        [Fact]
        public void FunctioImportnWithoutBindingParameterShouldWorkInPath()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("FindMyOwner(dogsName='fido')");
            path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForFindMyOwner());
        }

        [Fact]
        public void GeometryAndNullParameterValuesShouldWorkInPath()
        {
            var point = GeometryPoint.Create(1, 2);
            var path = PathFunctionalTestsUtil.RunParsePath("Paintings(0)/Fully.Qualified.Namespace.GetColorAtPosition(position=geometry'" + SpatialHelpers.WriteSpatial(point) + "',includeAlpha=null)");
            var operationSegment = path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetColorAtPositionFunction());
            operationSegment.ShouldHaveConstantParameter("position", point);
            operationSegment.ShouldHaveConstantParameter("includeAlpha", (object)null);
        }

        [Fact]
        public void GeographyAndNullParameterValuesShouldWorkInPath()
        {
            var point = GeographyPoint.Create(1, 2);
            var path = PathFunctionalTestsUtil.RunParsePath("People(0)/Fully.Qualified.Namespace.GetNearbyPriorAddresses(currentLocation=geography'" + SpatialHelpers.WriteSpatial(point) + "',limit=null)");
            var operationSegment = path.LastSegment.ShouldBeOperationSegment(HardCodedTestModel.GetNearbyPriorAddressesFunction());
            operationSegment.ShouldHaveConstantParameter("currentLocation", point);
            operationSegment.ShouldHaveConstantParameter("limit", (object)null);
        }

        [Fact]
        public void ExceptionShouldThrowForInvalidParameter()
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldgook/"), new Uri("http://gobbldgook/GetCoolPeople(id=test, limit=1)"));
            Action action = () => parser.ParsePath();
            action.Throws<ODataException>(ODataErrorStrings.MetadataBinder_ParameterNotInScope("id=test"));
        }

#region enum property in path
        [Fact]
        public void EnumPropertyOfEntity()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("Pet2Set(1)/PetColorPattern");
            Assert.Equal(3, path.Count);
            List<ODataPathSegment> segments = path.ToList();
            Assert.Equal(RequestTargetKind.Enum, segments[2].TargetKind);
            Assert.Same(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern"), segments[2].EdmType);
        }

        [Fact]
        public void EnumPropertyValueOfEntity()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("Pet2Set(1)/PetColorPattern/$value");
            Assert.Equal(4, path.Count);
            List<ODataPathSegment> segments = path.ToList();
            Assert.Equal(RequestTargetKind.Enum, segments[2].TargetKind);
            Assert.Same(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern"), segments[2].EdmType);
            Assert.Equal(RequestTargetKind.EnumValue, segments[3].TargetKind);
            Assert.Same(HardCodedTestModel.TestModel.FindType("Fully.Qualified.Namespace.ColorPattern"), segments[3].EdmType);
        }
#endregion

#region enum parameter in path
        [Fact]
        public void ParsePath_NullableEnumInFunction()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("GetPetCountNullable(colorPattern=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')");
            var segment = Assert.Single(path);
            var parameter = Assert.IsType<OperationImportSegment>(segment).Parameters.Single();
            var paramNode = Assert.IsType<OperationSegmentParameter>(parameter);
            Assert.Equal("colorPattern", paramNode.Name);
            var constNode = Assert.IsType<ConstantNode>(paramNode.Value);
            var enumValue = Assert.IsType<ODataEnumValue>(constNode.Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
            Assert.Equal("22", enumValue.Value); // BlueYellowStriped
        }

        [Fact]
        public void ParsePath_EnumInFunction()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("GetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')");
            var segment = Assert.Single(path);
            var parameter = Assert.IsType<OperationImportSegment>(segment).Parameters.Single();
            var paramNode = Assert.IsType<OperationSegmentParameter>(parameter);
            Assert.Equal("colorPattern", paramNode.Name);

            var constNode = Assert.IsType<ConstantNode>(paramNode.Value);
            var enumValue = Assert.IsType<ODataEnumValue>(constNode.Value);

            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
            Assert.Equal("22", enumValue.Value); // BlueYellowStriped
        }

        [Fact]
        public void ParsePath_EnumInFunction_undefined()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("GetPetCount(colorPattern=Fully.Qualified.Namespace.ColorPattern'99999222')");
            var segment = Assert.Single(path);
            var parameter = Assert.IsType<OperationImportSegment>(segment).Parameters.Single();
            var paramNode = Assert.IsType<OperationSegmentParameter>(parameter);
            Assert.Equal("colorPattern", paramNode.Name);

            var constNode = Assert.IsType<ConstantNode>(paramNode.Value);
            var enumValue = Assert.IsType<ODataEnumValue>(constNode.Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
            Assert.Equal("99999222", enumValue.Value);
        }
#endregion

#region enum as key
        [Fact]
        public void ParsePath_EnumAsKey()
        {
            ODataPath path = PathFunctionalTestsUtil.RunParsePath("PetCategories(Fully.Qualified.Namespace.ColorPattern'BlueYellowStriped')");
            Assert.Equal(2, path.Count);
            var keyInfo = Assert.IsType<KeySegment>(path.Last()).Keys.Single();
            Assert.Equal("PetCategorysColorPattern", keyInfo.Key);

            var enumValue = Assert.IsType<ODataEnumValue>(keyInfo.Value);
            Assert.Equal("Fully.Qualified.Namespace.ColorPattern", enumValue.TypeName);
            Assert.Equal("22", enumValue.Value);
        }
#endregion

#region type definition

        [Fact]
        public void KeyOfTypeDefinitionShouldWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("Pet6Set(5.1)");
            path.FirstSegment.ShouldBeEntitySetSegment(HardCodedTestModel.GetPet6Set());
            path.LastSegment.ShouldBeKeySegment(new KeyValuePair<string, object>("ID", 5.1));
        }

        [Fact]
        public void KeyOfIncompatibleTypeDefinitionShouldFail()
        {
            Action action = () => PathFunctionalTestsUtil.RunParsePath("Pet6Set(ID='abc')");
            action.Throws<ODataException>(ODataErrorStrings.RequestUriProcessor_SyntaxError);
        }

        [Fact]
        public void FunctionImportWithTypeDefinitionShouldWork()
        {
            var path = PathFunctionalTestsUtil.RunParsePath("GetPet6(id=5.1)");
            path.LastSegment.ShouldBeOperationImportSegment(HardCodedTestModel.GetFunctionImportForGetPet6());
        }

        [Fact]
        public void FunctionImportWithImcompatibleTypeDefinitionShouldFail()
        {
            Action action = () => PathFunctionalTestsUtil.RunParsePath("GetPet6(id='abc')");
            action.Throws<ODataException>(ODataErrorStrings.MetadataBinder_CannotConvertToType("Edm.String", "Fully.Qualified.Namespace.IdType"));
        }

#endregion
    }
}
