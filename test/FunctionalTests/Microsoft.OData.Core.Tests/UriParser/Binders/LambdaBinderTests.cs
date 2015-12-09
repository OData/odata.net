//---------------------------------------------------------------------
// <copyright file="LambdaBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    /// <summary>
    /// Tests for the Lambda binder. (any/all)
    /// Note that [Any|All]() (empty parens) is handled at the syntactic level. It is treated like [Any|All](a : true)
    /// </summary>
    public class LambdaBinderTests
    {
        private ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        private QueryNode parentQueryNode;
        private QueryNode expressionQueryNode;
        private bool shouldReturnParent;

        public LambdaBinderTests()
        {
            this.shouldReturnParent = true;
            this.parentQueryNode = new EntitySetNode(this.configuration.Model.FindDeclaredEntitySet("People"));
            this.expressionQueryNode = new ConstantNode(true);
        }

        [Fact]
        public void AllTokenWithEntityCollectionParentConstantExpression()
        {
            var binder = new LambdaBinder(this.FakeBindMethod);
            var state = this.GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            var allToken = this.CreateTestAllQueryToken();

            var result = binder.BindLambdaToken(allToken, state);
            result.ShouldBeAllQueryNode().And.Source.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetPeopleSet());
            result.Body.ShouldBeConstantQueryNode(true);
        }

        [Fact]
        public void AllTokenWithNonEntityCollectionParentNonConstantExpression()
        {
            this.parentQueryNode = new CollectionPropertyAccessNode(new ConstantNode(null), HardCodedTestModel.GetDogNicknamesProperty());
            this.expressionQueryNode = new BinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual, new ConstantNode(1), new ConstantNode(5));
            var binder = new LambdaBinder(this.FakeBindMethod);
            var state = this.GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            var allToken = this.CreateTestAllQueryToken();

            var result = binder.BindLambdaToken(allToken, state);
            result.ShouldBeAllQueryNode().And.Source.ShouldBeCollectionPropertyAccessQueryNode(HardCodedTestModel.GetDogNicknamesProperty());
            result.Body.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual);
        }

        [Fact]
        public void AnyTokenWithEntityCollectionParentConstantExpression()
        {
            var binder = new LambdaBinder(this.FakeBindMethod);
            var state = this.GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            var anyToken = this.CreateTestAnyQueryToken();

            var result = binder.BindLambdaToken(anyToken, state);
            result.ShouldBeAnyQueryNode().And.Source.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetPeopleSet());
            result.Body.ShouldBeConstantQueryNode(true);
        }

        [Fact]
        public void AnyTokenWithNonConstantExpressionNullParameter()
        {
            this.expressionQueryNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new ConstantNode(false));
            var binder = new LambdaBinder(this.FakeBindMethod);
            var state = this.GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            var expression = new LiteralToken("foo");
            var parent = new LiteralToken("bar");
            var anyToken = new AnyToken(expression, null, parent);

            var result = binder.BindLambdaToken(anyToken, state);
            result.ShouldBeAnyQueryNode().And.Source.ShouldBeEntitySetQueryNode(HardCodedTestModel.GetPeopleSet());
            result.Body.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Negate);
        }

        [Fact]
        public void BindLambdaTokenShouldFailForNonCollectionParent()
        {
            this.parentQueryNode = new ConstantNode(true);
            var binder = new LambdaBinder(this.FakeBindMethod);
            var state = this.GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            var allToken = this.CreateTestAllQueryToken();

            Action bind = () => binder.BindLambdaToken(allToken, state);
            bind.ShouldThrow<ODataException>().
                WithMessage((Strings.MetadataBinder_LambdaParentMustBeCollection));
        }

        [Fact]
        public void BindLambdaTokenShouldPassForOpenPropertyParent()
        {
            this.parentQueryNode = new CollectionOpenPropertyAccessNode(new ConstantNode(null), "SomeCollectionProperty");
            var binder = new LambdaBinder(this.FakeBindMethod);
            var state = this.GetBindingStateForTest(HardCodedTestModel.GetPaintingTypeReference(), HardCodedTestModel.GetPaintingsSet());
            var allToken = this.CreateTestAllQueryToken();

            Action bind = () => binder.BindLambdaToken(allToken, state);
            bind.ShouldNotThrow();
        }

        [Fact]
        public void BindLambdaTokenShouldFailForNullExpression()
        {
            this.expressionQueryNode = null;
            var binder = new LambdaBinder(this.FakeBindMethod);
            var state = this.GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            var allToken = this.CreateTestAllQueryToken();

            Action bind = () => binder.BindLambdaToken(allToken, state);
            bind.ShouldThrow<ODataException>().
                WithMessage((Strings.MetadataBinder_AnyAllExpressionNotSingleValue));
        }

        [Fact]
        public void BindLambdaTokenShouldFailForNonBoolExpression()
        {
            this.expressionQueryNode = new ConstantNode(0);
            var binder = new LambdaBinder(this.FakeBindMethod);
            var state = this.GetBindingStateForTest(HardCodedTestModel.GetPersonTypeReference(), HardCodedTestModel.GetPeopleSet());
            var allToken = this.CreateTestAnyQueryToken();

            Action bind = () => binder.BindLambdaToken(allToken, state);
            bind.ShouldThrow<ODataException>().
                WithMessage((Strings.MetadataBinder_AnyAllExpressionNotSingleValue));
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
            var implicitParameter = new EntityRangeVariable(ExpressionConstants.It, typeReference, entityCollectionNode);
            var state = new BindingState(this.configuration) { ImplicitRangeVariable = implicitParameter };
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            return state;
        }

        private QueryNode FakeBindMethod(QueryToken token)
        {
            if (this.shouldReturnParent)
            {
                this.shouldReturnParent = false;
                return this.parentQueryNode;
            }
            else
            {
                return this.expressionQueryNode;
            }
        }

        private AllToken CreateTestAllQueryToken()
        {
            var expression = new LiteralToken("foo");
            var parent = new LiteralToken("bar");
            return new AllToken(expression, "p", parent);
        }

        private AnyToken CreateTestAnyQueryToken()
        {
            var expression = new LiteralToken("foo");
            var parent = new LiteralToken("bar");
            return new AnyToken(expression, "p", parent);
        }
    }
}
