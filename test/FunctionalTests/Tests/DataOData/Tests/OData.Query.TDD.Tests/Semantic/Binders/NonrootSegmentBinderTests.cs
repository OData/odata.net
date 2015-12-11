﻿//---------------------------------------------------------------------
// <copyright file="NonrootSegmentBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Unit tests for InnerPathTokenBinder.
    /// </summary>
    [TestClass]
    public class InnerPathTokenBinderTests
    {
        private static readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        #region Short-span integration tests on BindInnerPathSegment
        [TestMethod]
        public void CollectionNavigationPropertyShouldCreateMatchingNode()
        {
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASingleDog, state);
            var token = new InnerPathToken("MyPeople", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionNavigationNode(HardCodedTestModel.GetDogMyPeopleNavProp()).
                And.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet().FindNavigationTarget(HardCodedTestModel.GetDogMyPeopleNavProp()));
        }

        [TestMethod]
        public void MissingPropertyShouldThrow()
        {
            const string MissingPropertyName = "ThisPropertyDoesNotExist";
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASingleDog, state);
            var token = new InnerPathToken(MissingPropertyName, new DummyToken(), null /*namedValues*/);

            Action bind = () => binder.BindInnerPathSegment(token);

            string expectedMessage =
                ODataErrorStrings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetDogType().FullTypeName(), MissingPropertyName);
            bind.ShouldThrow<ODataException>(expectedMessage);
        }

        [TestMethod]
        public void DeclaredPropertyOnOpenTypeShouldCreateMatchingNode()
        {
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePainting, state);
            var token = new InnerPathToken("Colors", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPaintingColorsProperty());
        }

        [TestMethod]
        public void OpenPropertyShouldCreateMatchingNode()
        {
            const string OpenPropertyName = "Emotions";
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePainting, state);
            var token = new InnerPathToken(OpenPropertyName, new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
        }

        [TestMethod]
        public void PrimitiveCollectionPropertyShouldCreateMatchingNode()
        {
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASingleDog, state);
            var token = new InnerPathToken("Nicknames", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetDogNicknamesProperty());
        }

        [TestMethod]
        public void ComplexCollectionPropertyShouldCreateMatchingNode()
        {
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePerson, state);
            var token = new InnerPathToken("PreviousAddresses", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonPreviousAddressesProp());
        }

        [TestMethod]
        public void CollectionOfDateTimeOffsetShouldCreateMatchingNode()
        {
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASingleLion, state);
            var token = new InnerPathToken("AttackDates", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetLionAttackDatesProp());
        }

        [TestMethod]
        public void CollectionOfDateShouldCreateMatchingNode()
        {
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePerson, state);
            var token = new InnerPathToken("MyDates", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDatesProp());
        }

        [TestMethod]
        public void CollectionOfTimeOfDayShouldCreateMatchingNode()
        {
            var state = new BindingState(configuration);
            var binder = new InnerPathTokenBinder(FakeBindMethods.BindMethodReturningASinglePerson, state);
            var token = new InnerPathToken("MyTimeOfDays", new DummyToken(), null /*namedValues*/);

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDaysProp());
        }

        [TestMethod]
        public void KeyLookupOnNavPropIntegrationTest()
        {
            var state = new BindingState(configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            var metadataBinder = new MetadataBinder(state);
            var binder = new InnerPathTokenBinder(metadataBinder.Bind, state);
            var token = new InnerPathToken("MyPeople", null, new[] { new NamedValue(null, new LiteralToken(123)) });

            var result = binder.BindInnerPathSegment(token);
            result.ShouldBeKeyLookupQueryNode();
        }

        [TestMethod]
        public void InnerPathTokenBinderShouldFailIfPropertySourceIsNotASingleValue()
        {
            var state = new BindingState(configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            var metadataBinder = new MetadataBinder(state);
            var binder = new InnerPathTokenBinder(metadataBinder.Bind, state);
            var token = new InnerPathToken("MyDog", new InnerPathToken("MyPeople", null, null), null);

            Action bind = () => binder.BindInnerPathSegment(token);
            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyAccessSourceNotSingleValue("MyDog"));
        }
        #endregion

        #region Unit tests on helper methods

        [TestMethod]
        public void BindPropertyShouldReturnCorrectPropertyIfFoundForEntity()
        {
            var result = InnerPathTokenBinder.BindProperty(HardCodedTestModel.GetPersonTypeReference(), "Shoe");
            result.Should().BeSameAs(HardCodedTestModel.GetPersonShoeProp());
        }

        [TestMethod]
        public void BindPropertyShouldReturnCorrectPropertyIfFoundForComplex()
        {
            var result = InnerPathTokenBinder.BindProperty(HardCodedTestModel.GetPersonAddressProp().Type, "MyNeighbors");
            result.Should().BeSameAs(HardCodedTestModel.GetAddressMyNeighborsProperty());
        }

        [TestMethod]
        public void BindPropertyShouldBeCaseSensitive()
        {
            var result = InnerPathTokenBinder.BindProperty(HardCodedTestModel.GetPersonTypeReference(), "shoe");
            result.Should().BeNull();
        }

        [TestMethod]
        public void BindPropertyShouldReturnNullIfNotFound()
        {
            var result = InnerPathTokenBinder.BindProperty(HardCodedTestModel.GetPersonTypeReference(), "missing");
            result.Should().BeNull();
        }

        [TestMethod]
        public void BindPropertyShouldReturnNullIfTypeNotStructured()
        {
            var result = InnerPathTokenBinder.BindProperty(EdmCoreModel.Instance.GetDecimal(false), "NotStructured");
            result.Should().BeNull();
        }

        [TestMethod]
        public void EnsureParentIsEntityForNavPropReturnsSameObjectAsPassedOnOnSuccess()
        {
            var parent = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), (IEdmEntitySet)null);
            var result = InnerPathTokenBinder.EnsureParentIsEntityForNavProp(parent);
            result.Should().BeSameAs(parent);
        }

        [TestMethod]
        public void EnsureParentIsEntityForNavPropThrowsIfNotEntity()
        {
            var parent = new ConstantNode(null);
            Action targetMethod = () => InnerPathTokenBinder.EnsureParentIsEntityForNavProp(parent);
            targetMethod.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_NavigationPropertyNotFollowingSingleEntityType);
        }

        [TestMethod]
        public void GetNavigationNodeCreatesSingleNavigationNodeForSingleMultiplicityProperty()
        {
            IEdmNavigationProperty property = HardCodedTestModel.GetPersonMyDogNavProp();
            SingleEntityNode parent = new SingleEntityCastNode(null, HardCodedTestModel.GetDogType());
            BindingState state = new BindingState(configuration);
            KeyBinder keyBinder = new KeyBinder(FakeBindMethods.BindMethodReturningASingleDog);

            var result = InnerPathTokenBinder.GetNavigationNode(property, parent, null, state, keyBinder);
            result.ShouldBeSingleNavigationNode(property);
        }

        [TestMethod]
        public void GetNavigationNodeCreatesCollectionNavigationNodeForManyMultiplicityProperty()
        {
            IEdmNavigationProperty property = HardCodedTestModel.GetDogMyPeopleNavProp();
            SingleEntityNode parent = new SingleEntityCastNode(null, HardCodedTestModel.GetDogType());
            BindingState state = new BindingState(configuration);
            KeyBinder keyBinder = new KeyBinder(FakeBindMethods.BindMethodReturningASingleDog);

            var result = InnerPathTokenBinder.GetNavigationNode(property, parent, null, state, keyBinder);
            result.ShouldBeCollectionNavigationNode(property);
        }

        #endregion
    }
}
