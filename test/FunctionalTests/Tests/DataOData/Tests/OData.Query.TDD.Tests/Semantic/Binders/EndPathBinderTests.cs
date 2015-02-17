//---------------------------------------------------------------------
// <copyright file="EndPathBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the PropertyAccessBinder.
    /// </summary>
    [TestClass]
    public class EndPathBinderTests
    {
        private readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private EndPathBinder propertyBinder;
        private BindingState bindingState;

        [TestInitialize]
        public void Init()
        {
            this.bindingState = GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            this.propertyBinder = new EndPathBinder(BindMethod, this.bindingState);
        }

        // BindEndPath Tests
        [TestMethod]
        public void ImplicitPropertyAccessShouldCreatePropertyAccessQueryNode()
        {
            var token = new EndPathToken("Shoe", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp()).
                And.Source.ShouldBeEntityRangeVariableReferenceNode(ExpressionConstants.It);
        }

        [TestMethod]
        public void ExplicitPropertyAccessShouldCreatePropertyAccessQueryNode()
        {
            var token = new EndPathToken("Color", new RangeVariableToken("a"));
            EntityCollectionNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("a", HardCodedTestModel.GetDogTypeReference(), entityCollectionNode));
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                And.Source.ShouldBeEntityRangeVariableReferenceNode("a");
        }

        [TestMethod]
        public void ShouldThrowIfParameterIsNotInScope()
        {
            var token = new EndPathToken("Color", new RangeVariableToken("notInScope"));
            Action bind = () => this.propertyBinder.BindEndPath(token);

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_ParameterNotInScope("notInScope"));
        }

        [TestMethod]
        public void ManyParametersInScopeShouldNotInterfere()
        {
            var token = new EndPathToken("Shoe", null);

            EntityCollectionNode dogsEntityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityCollectionNode peopleEntityCollectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());

            this.bindingState.RangeVariables.Push(new EntityRangeVariable("a", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("b", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("c", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("d", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("e", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("f", HardCodedTestModel.GetPersonTypeReference(), peopleEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("g", HardCodedTestModel.GetPersonTypeReference(), peopleEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("h", HardCodedTestModel.GetPersonTypeReference(), peopleEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new EntityRangeVariable("i", HardCodedTestModel.GetPersonTypeReference(), peopleEntityCollectionNode));

            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp()).
                And.Source.ShouldBeEntityRangeVariableReferenceNode(ExpressionConstants.It);
        }

        [TestMethod]
        public void PropertyAccessOnDateWorks()
        {
            var token = new EndPathToken("MyDate", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
        }

        [TestMethod]
        public void PropertyAccessOnDateCollectionWorks()
        {
            var token = new EndPathToken("MyDates", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDatesProp());
        }

        [TestMethod]
        public void PropertyAccessOnDateTimeWorks()
        {
            var token = new EndPathToken("FavoriteDate", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonFavoriteDateProp());
        }

        [TestMethod]
        public void PropertyAccessOnDateTimeOffsetWorks()
        {
            var token = new EndPathToken("Birthdate", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonBirthdateProp());
        }

        [TestMethod]
        public void PropertyAccessOnTimeSpanWorks()
        {
            var token = new EndPathToken("TimeEmployed", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
        }

        [TestMethod]
        public void PropertyAccessOnTimeOfDayWorks()
        {
            var token = new EndPathToken("MyTimeOfDay", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDayProp());
        }

        [TestMethod]
        public void PropertyAccessOnTimeOfDayCollectionWorks()
        {
            var token = new EndPathToken("MyTimeOfDays", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDaysProp());
        }

        [TestMethod]
        public void BoundFunctionWorks()
        {
            var token = new EndPathToken("Fully.Qualified.Namespace.HasJob", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForHasJob());
        }

        [TestMethod]
        public void OpenPropertyIsNotBoundToAFunction()
        {
            var token = new EndPathToken("SomeOpenProperty", null);
            BindingState state = GetBindingStateForTest(HardCodedTestModel.GetPaintingTypeReference(), HardCodedTestModel.GetPaintingsSet());
            EndPathBinder binder = new EndPathBinder(BindMethod, state);
            var result = binder.BindEndPath(token);
            result.ShouldBeSingleValueOpenPropertyAccessQueryNode("SomeOpenProperty");
        }

        //CreateParentFromImplicitParameterTests

        [TestMethod]
        public void CreateParentShouldThrowIfBindingStateWithoutImplicitParameter()
        {
            var state = new BindingState(this.configuration);
            Action createparent = () => EndPathBinder.CreateParentFromImplicitRangeVariable(state);
            createparent.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyAccessWithoutParentParameter);
        }

        [TestMethod]
        public void CreateParentShouldProduceParentUsingParameters()
        {
            SingleValueNode parent = EndPathBinder.CreateParentFromImplicitRangeVariable(this.bindingState);
            EntityRangeVariableReferenceNode entityRangeVariableReferenceNode = (EntityRangeVariableReferenceNode)parent;
            entityRangeVariableReferenceNode.RangeVariable.Should().BeSameAs(this.bindingState.ImplicitRangeVariable);
        }

        [TestMethod]
        public void GenerateQueryNodeShouldWorkIfPropertyIsPrimitiveCollection()
        {
            var property = HardCodedTestModel.GetDogNicknamesProperty();
            EntityRangeVariable rangeVariable = new EntityRangeVariable("Color", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            QueryNode result = EndPathBinder.GeneratePropertyAccessQueryNode(new EntityRangeVariableReferenceNode(rangeVariable.Name, rangeVariable), property);
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetDogNicknamesProperty());
        }

        [TestMethod]
        public void GenerateQueryNodeShouldReturnQueryNode()
        {
            var property = HardCodedTestModel.GetDogColorProp();
            EntityCollectionNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            EntityRangeVariable rangeVariable = new EntityRangeVariable("Color", HardCodedTestModel.GetDogTypeReference(), entityCollectionNode);
            var result = EndPathBinder.GeneratePropertyAccessQueryNode(
                new EntityRangeVariableReferenceNode(rangeVariable.Name, rangeVariable),
                property);

            result.ShouldBeSingleValuePropertyAccessQueryNode(property);
        }

        // GeneratePropertyAccessQueryNodeForOpenTypes tests

        [TestMethod]
        public void ShouldThrowNotImplementedIfTypeIsOpen()
        {
            const string OpenPropertyName = "Style";
            var token = new EndPathToken(OpenPropertyName, new RangeVariableToken("a"));
            EntityCollectionNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetPaintingsSet());
            SingleValueNode parentNode = new EntityRangeVariableReferenceNode("a", new EntityRangeVariable("a", HardCodedTestModel.GetPaintingTypeReference(), entityCollectionNode));
            var propertyNode = EndPathBinder.GeneratePropertyAccessQueryForOpenType(
                token, parentNode);

            propertyNode.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
        }

        [TestMethod]
        public void ShouldThrowIfTypeNotOpen()
        {
            var token = new EndPathToken("Color", new RangeVariableToken("a"));
            EntityCollectionNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            SingleValueNode parentNode = new EntityRangeVariableReferenceNode("a", new EntityRangeVariable("a", HardCodedTestModel.GetPersonTypeReference(), entityCollectionNode));
            Action getQueryNode = () => EndPathBinder.GeneratePropertyAccessQueryForOpenType(
                token, parentNode);

            getQueryNode.ShouldThrow<ODataException>().WithMessage(
                Strings.MetadataBinder_PropertyNotDeclared(parentNode.GetEdmTypeReference().ODataFullName(),
                                                                                    token.Identifier));

        }

        /// <summary>
        /// We substitute this method for the MetadataBinder.Bind method to keep the tests from growing too large in scope.
        /// In practice this does the same thing.
        /// </summary>
        internal SingleValueNode BindMethod(QueryToken queryToken)
        {
            queryToken.Should().BeOfType<RangeVariableToken>();
            return RangeVariableBinder.BindRangeVariableToken(queryToken.As<RangeVariableToken>(), this.bindingState) as SingleValueNode;
        }

        /// <summary>
        /// Gets a BindingState for the test to use.
        /// </summary>
        /// <param name="type">Optional type for the implicit parameter.</param>
        /// <returns></returns>
        private BindingState GetBindingStateForTest(IEdmEntityTypeReference typeReference, IEdmEntitySet type)
        {
            type.Should().NotBeNull();
            EntityCollectionNode entityCollectionNode = new EntitySetNode(type);
            var implicitRangeVariable = new EntityRangeVariable(ExpressionConstants.It, typeReference, entityCollectionNode);
            var state = new BindingState(this.configuration) { ImplicitRangeVariable = implicitRangeVariable };
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            return state;
        }
    }
}
