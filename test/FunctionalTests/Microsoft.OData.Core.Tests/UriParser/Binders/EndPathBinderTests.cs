//---------------------------------------------------------------------
// <copyright file="EndPathBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit tests for the PropertyAccessBinder.
    /// </summary>
    public class EndPathBinderTests
    {
        private readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private EndPathBinder propertyBinder;
        private BindingState bindingState;

        public EndPathBinderTests()
        {
            this.bindingState = GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            this.propertyBinder = new EndPathBinder(BindMethod, this.bindingState);
        }

        // BindEndPath Tests
        [Fact]
        public void ImplicitPropertyAccessShouldCreatePropertyAccessQueryNode()
        {
            var token = new EndPathToken("Shoe", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp()).
                And.Source.ShouldBeResourceRangeVariableReferenceNode(ExpressionConstants.It);
        }

        [Fact]
        public void ExplicitPropertyAccessShouldCreatePropertyAccessQueryNode()
        {
            var token = new EndPathToken("Color", new RangeVariableToken("a"));
            CollectionResourceNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("a", HardCodedTestModel.GetDogTypeReference(), entityCollectionNode));
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetDogColorProp()).
                And.Source.ShouldBeResourceRangeVariableReferenceNode("a");
        }

        [Fact]
        public void ShouldThrowIfParameterIsNotInScope()
        {
            var token = new EndPathToken("Color", new RangeVariableToken("notInScope"));
            Action bind = () => this.propertyBinder.BindEndPath(token);

            bind.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_ParameterNotInScope("notInScope"));
        }

        [Fact]
        public void ManyParametersInScopeShouldNotInterfere()
        {
            var token = new EndPathToken("Shoe", null);

            CollectionResourceNode dogsEntityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            CollectionResourceNode peopleEntityCollectionNode = new EntitySetNode(HardCodedTestModel.GetPeopleSet());

            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("a", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("b", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("c", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("d", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("e", HardCodedTestModel.GetDogTypeReference(), dogsEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("f", HardCodedTestModel.GetPersonTypeReference(), peopleEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("g", HardCodedTestModel.GetPersonTypeReference(), peopleEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("h", HardCodedTestModel.GetPersonTypeReference(), peopleEntityCollectionNode));
            this.bindingState.RangeVariables.Push(new ResourceRangeVariable("i", HardCodedTestModel.GetPersonTypeReference(), peopleEntityCollectionNode));

            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonShoeProp()).
                And.Source.ShouldBeResourceRangeVariableReferenceNode(ExpressionConstants.It);
        }

        [Fact]
        public void PropertyAccessOnDateWorks()
        {
            var token = new EndPathToken("MyDate", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDateProp());
        }

        [Fact]
        public void PropertyAccessOnDateCollectionWorks()
        {
            var token = new EndPathToken("MyDates", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyDatesProp());
        }

        [Fact]
        public void PropertyAccessOnDateTimeWorks()
        {
            var token = new EndPathToken("FavoriteDate", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonFavoriteDateProp());
        }

        [Fact]
        public void PropertyAccessOnDateTimeOffsetWorks()
        {
            var token = new EndPathToken("Birthdate", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonBirthdateProp());
        }

        [Fact]
        public void PropertyAccessOnTimeSpanWorks()
        {
            var token = new EndPathToken("TimeEmployed", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonTimeEmployedProp());
        }

        [Fact]
        public void PropertyAccessOnTimeOfDayWorks()
        {
            var token = new EndPathToken("MyTimeOfDay", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValuePropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDayProp());
        }

        [Fact]
        public void PropertyAccessOnTimeOfDayCollectionWorks()
        {
            var token = new EndPathToken("MyTimeOfDays", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetPersonMyTimeOfDaysProp());
        }

        [Fact]
        public void BoundFunctionWorks()
        {
            var token = new EndPathToken("Fully.Qualified.Namespace.HasJob", null);
            var result = this.propertyBinder.BindEndPath(token);

            result.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetFunctionForHasJob());
        }

        [Fact]
        public void OpenPropertyIsNotBoundToAFunction()
        {
            var token = new EndPathToken("SomeOpenProperty", null);
            BindingState state = GetBindingStateForTest(HardCodedTestModel.GetPaintingTypeReference(), HardCodedTestModel.GetPaintingsSet());
            EndPathBinder binder = new EndPathBinder(BindMethod, state);
            var result = binder.BindEndPath(token);
            result.ShouldBeSingleValueOpenPropertyAccessQueryNode("SomeOpenProperty");
        }

        //CreateParentFromImplicitParameterTests

        [Fact]
        public void CreateParentShouldThrowIfBindingStateWithoutImplicitParameter()
        {
            var state = new BindingState(this.configuration);
            Action createparent = () => EndPathBinder.CreateParentFromImplicitRangeVariable(state);
            createparent.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyAccessWithoutParentParameter);
        }

        [Fact]
        public void CreateParentShouldProduceParentUsingParameters()
        {
            SingleValueNode parent = EndPathBinder.CreateParentFromImplicitRangeVariable(this.bindingState);
            ResourceRangeVariableReferenceNode entityRangeVariableReferenceNode = (ResourceRangeVariableReferenceNode)parent;
            entityRangeVariableReferenceNode.RangeVariable.Should().BeSameAs(this.bindingState.ImplicitRangeVariable);
        }

        [Fact]
        public void GenerateQueryNodeShouldWorkIfPropertyIsPrimitiveCollection()
        {
            var property = HardCodedTestModel.GetDogNicknamesProperty();
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("Color", HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            QueryNode result = EndPathBinder.GeneratePropertyAccessQueryNode(new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable), property, new BindingState(configuration));
            result.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetDogNicknamesProperty());
        }

        [Fact]
        public void GenerateQueryNodeShouldReturnQueryNode()
        {
            var property = HardCodedTestModel.GetDogColorProp();
            CollectionResourceNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            ResourceRangeVariable rangeVariable = new ResourceRangeVariable("Color", HardCodedTestModel.GetDogTypeReference(), entityCollectionNode);
            var result = EndPathBinder.GeneratePropertyAccessQueryNode(
                new ResourceRangeVariableReferenceNode(rangeVariable.Name, rangeVariable),
                property, new BindingState(configuration));

            result.ShouldBeSingleValuePropertyAccessQueryNode(property);
        }

        // GeneratePropertyAccessQueryNodeForOpenTypes tests

        [Fact]
        public void ShouldThrowNotImplementedIfTypeIsOpen()
        {
            const string OpenPropertyName = "Style";
            var token = new EndPathToken(OpenPropertyName, new RangeVariableToken("a"));
            CollectionResourceNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetPaintingsSet());
            SingleValueNode parentNode = new ResourceRangeVariableReferenceNode("a", new ResourceRangeVariable("a", HardCodedTestModel.GetPaintingTypeReference(), entityCollectionNode));
            
            var state = new BindingState(this.configuration);
            var metadataBinder = new MetadataBinder(state);
            var endPathBinder = new EndPathBinder(metadataBinder.Bind, state);
            var propertyNode = endPathBinder.GeneratePropertyAccessQueryForOpenType(
                token, parentNode);

            propertyNode.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
        }

        [Fact]
        public void ShouldThrowIfTypeNotOpen()
        {
            var token = new EndPathToken("Color", new RangeVariableToken("a"));
            CollectionResourceNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            SingleValueNode parentNode = new ResourceRangeVariableReferenceNode("a", new ResourceRangeVariable("a", HardCodedTestModel.GetPersonTypeReference(), entityCollectionNode));

            var state = new BindingState(this.configuration);
            var metadataBinder = new MetadataBinder(state);
            var endPathBinder = new EndPathBinder(metadataBinder.Bind, state);
            Action getQueryNode = () => endPathBinder.GeneratePropertyAccessQueryForOpenType(
                token, parentNode);

            getQueryNode.ShouldThrow<ODataException>().WithMessage(
                Strings.MetadataBinder_PropertyNotDeclared(parentNode.GetEdmTypeReference().FullName(),
                                                                                    token.Identifier));
        }

        [Fact]
        public void ShouldNotThrowIfTypeNotOpenButAggregateApplied()
        {
            var token = new EndPathToken("Color", new RangeVariableToken("a"));
            CollectionResourceNode entityCollectionNode = new EntitySetNode(HardCodedTestModel.GetDogsSet());
            SingleValueNode parentNode = new ResourceRangeVariableReferenceNode("a", new ResourceRangeVariable("a", HardCodedTestModel.GetPersonTypeReference(), entityCollectionNode));

            var state = new BindingState(this.configuration);
            state.AggregatedPropertyNames = new List<string> { "Color" };
            var metadataBinder = new MetadataBinder(state);
            var endPathBinder = new EndPathBinder(metadataBinder.Bind, state);
            var propertyNode = endPathBinder.GeneratePropertyAccessQueryForOpenType(
                token, parentNode);

            propertyNode.ShouldBeSingleValueOpenPropertyAccessQueryNode("Color");
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
            CollectionResourceNode entityCollectionNode = new EntitySetNode(type);
            var implicitRangeVariable = new ResourceRangeVariable(ExpressionConstants.It, typeReference, entityCollectionNode);
            var state = new BindingState(this.configuration) { ImplicitRangeVariable = implicitRangeVariable };
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            return state;
        }
    }
}
