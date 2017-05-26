//---------------------------------------------------------------------
// <copyright file="InnerPathTokenBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit tests for InnerPathTokenBinder.
    /// </summary>
    public class InnerPathTokenBinderTests
    {
        private static readonly ODataUriParserConfiguration Configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private static readonly ODataUriResolver DefaultUriResolver = ODataUriResolver.GetUriResolver(null);

        #region Short-span integration tests on BindInnerPathSegment
        [Fact]
        public void CollectionNavigationPropertyShouldCreateMatchingNode()
        {
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASingleDog, state);
            var token = new InnerPathToken("MyPeople", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp()).
                And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet().FindNavigationTarget(HardCodedTestModel.GetDogMyPeopleNavProp()));
        }

        [Fact]
        public void MissingPropertyShouldThrow()
        {
            const string MissingPropertyName = "ThisPropertyDoesNotExist";
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASingleDog, state);
            var token = new InnerPathToken(MissingPropertyName, new DummyToken(), null /*namedValues*/);

            Action bind = () => binder.BindInnerPathSegment(token);

            string expectedMessage =
                ODataErrorStrings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetDogType().FullTypeName(), MissingPropertyName);
            bind.ShouldThrow<ODataException>(expectedMessage);
        }

        [Fact]
        public void DeclaredPropertyOnOpenTypeShouldCreateMatchingNode()
        {
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePainting, state);
            var token = new InnerPathToken("Colors", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPaintingColorsProperty());
        }

        [Fact]
        public void OpenPropertyShouldCreateMatchingNode()
        {
            const string OpenPropertyName = "Emotions";
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePainting, state);
            var token = new InnerPathToken(OpenPropertyName, new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
        }

        [Fact]
        public void PrimitiveCollectionPropertyShouldCreateMatchingNode()
        {
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASingleDog, state);
            var token = new InnerPathToken("Nicknames", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetDogNicknamesProperty());
        }

        [Fact]
        public void ComplexCollectionPropertyShouldCreateMatchingNode()
        {
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePerson, state);
            var token = new InnerPathToken("PreviousAddresses", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionComplexNode(HardCodedTestModel.GetPersonPreviousAddressesProp());
        }

        [Fact]
        public void CollectionOfDateTimeOffsetShouldCreateMatchingNode()
        {
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASingleLion, state);
            var token = new InnerPathToken("AttackDates", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetLionAttackDatesProp());
        }

        [Fact]
        public void CollectionOfDateShouldCreateMatchingNode()
        {
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePerson, state);
            var token = new InnerPathToken("MyDates", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDatesProp());
        }

        [Fact]
        public void CollectionOfTimeOfDayShouldCreateMatchingNode()
        {
            var state = new BindingState(Configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePerson, state);
            var token = new InnerPathToken("MyTimeOfDays", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDaysProp());
        }

        [Fact]
        public void KeyLookupOnNavPropIntegrationTest()
        {
            var state = new BindingState(Configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            var metadataBinder = new MetadataBinder(state);
            var binder = new InnerPathTokenBinder(metadataBinder.Bind, state);
            var token = new InnerPathToken("MyPeople", null, new[] { new NamedValue(null, new LiteralToken(123)) });

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeKeyLookupQueryNode();
        }

        [Fact]
        public void InnerPathTokenBinderShouldFailIfPropertySourceIsNotASingleValue()
        {
            var state = new BindingState(Configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            var metadataBinder = new MetadataBinder(state);
            var binder = new InnerPathTokenBinder(metadataBinder.Bind, state);
            var token = new InnerPathToken("MyDog", new InnerPathToken("MyPeople", null, null), null);

            Action bind = () => binder.BindInnerPathSegment(token);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyAccessSourceNotSingleValue("MyDog"));
        }
        #endregion

        #region Unit tests on helper methods

        [Fact]
        public void BindPropertyShouldReturnCorrectPropertyIfFoundForEntity()
        {
            var result = InnerPathTokenBinder.BindProperty(HardCodedTestModel.GetPersonTypeReference(), "Shoe", DefaultUriResolver);
            result.Should().BeSameAs(HardCodedTestModel.GetPersonShoeProp());
        }

        [Fact]
        public void BindPropertyShouldReturnCorrectPropertyIfFoundForComplex()
        {
            var result = InnerPathTokenBinder.BindProperty(HardCodedTestModel.GetPersonAddressProp().Type, "MyNeighbors", DefaultUriResolver);
            result.Should().BeSameAs(HardCodedTestModel.GetAddressMyNeighborsProperty());
        }

        [Fact]
        public void BindPropertyShouldBeCaseSensitive()
        {
            var result = InnerPathTokenBinder.BindProperty(HardCodedTestModel.GetPersonTypeReference(), "shoe", DefaultUriResolver);
            result.Should().BeNull();
        }

        [Fact]
        public void BindPropertyShouldReturnNullIfNotFound()
        {
            var result = InnerPathTokenBinder.BindProperty(HardCodedTestModel.GetPersonTypeReference(), "missing", DefaultUriResolver);
            result.Should().BeNull();
        }

        [Fact]
        public void BindPropertyShouldReturnNullIfTypeNotStructured()
        {
            var result = InnerPathTokenBinder.BindProperty(EdmCoreModel.Instance.GetDecimal(false), "NotStructured", DefaultUriResolver);
            result.Should().BeNull();
        }

        [Fact]
        public void EnsureParentIsEntityForNavPropReturnsSameObjectAsPassedOnOnSuccess()
        {
            var parent = new SingleNavigationNode((IEdmEntitySet)null, HardCodedTestModel.GetPersonMyDogNavProp(), new EdmPathExpression("MyDog"));
            var result = InnerPathTokenBinder.EnsureParentIsResourceForNavProp(parent);
            result.Should().BeSameAs(parent);
        }

        [Fact]
        public void EnsureParentIsEntityForNavPropThrowsIfNotEntity()
        {
            var parent = new ConstantNode(null);
            Action targetMethod = () => InnerPathTokenBinder.EnsureParentIsResourceForNavProp(parent);
            targetMethod.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NavigationPropertyNotFollowingSingleEntityType);
        }

        [Fact]
        public void GetNavigationNodeCreatesSingleNavigationNodeForSingleMultiplicityProperty()
        {
            IEdmNavigationProperty property = HardCodedTestModel.GetPersonMyDogNavProp();
            IEdmNavigationSource navigationSource;
            SingleResourceNode parent = new SingleResourceCastNode(null, HardCodedTestModel.GetDogType());
            BindingState state = new BindingState(Configuration);
            KeyBinder keyBinder = new KeyBinder(FakeBindMethods.BindMethodReturningASingleDog);

            var result = InnerPathTokenBinder.GetNavigationNode(property, parent, null, state, keyBinder, out navigationSource);
            result.ShouldBeSingleNavigationNode(property);
        }

        [Fact]
        public void GetNavigationNodeCreatesCollectionNavigationNodeForManyMultiplicityProperty()
        {
            IEdmNavigationSource navigationSource;
            IEdmNavigationProperty property = HardCodedTestModel.GetDogMyPeopleNavProp();
            SingleResourceNode parent = new SingleResourceCastNode(null, HardCodedTestModel.GetDogType());
            BindingState state = new BindingState(Configuration);
            KeyBinder keyBinder = new KeyBinder(FakeBindMethods.BindMethodReturningASingleDog);

            var result = InnerPathTokenBinder.GetNavigationNode(property, parent, null, state, keyBinder, out navigationSource);
            result.ShouldBeCollectionNavigationNode(property);
        }

        #endregion
    }
}
