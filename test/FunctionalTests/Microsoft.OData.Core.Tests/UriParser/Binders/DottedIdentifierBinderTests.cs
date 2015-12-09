//---------------------------------------------------------------------
// <copyright file="DottedIdentifierBinderTests.cs" company="Microsoft">
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
using Microsoft.OData.Core.UriParser.Visitors;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    public class DottedIdentifierBinderTests
    {
        private readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);
        private DottedIdentifierBinder dottedIdentifierBinder;
        private BindingState bindingState;

        public DottedIdentifierBinderTests()
        {
            IEdmEntitySet set = HardCodedTestModel.GetPeopleSet();
            EntityCollectionNode entityCollectionNode = new EntitySetNode(set);
            var implicitParameter = new EntityRangeVariable(ExpressionConstants.It, HardCodedTestModel.GetPersonTypeReference(), entityCollectionNode);
            this.bindingState = new BindingState(this.configuration) { ImplicitRangeVariable = implicitParameter };
            this.bindingState.RangeVariables.Push(new BindingState(this.configuration) { ImplicitRangeVariable = implicitParameter }.ImplicitRangeVariable);
            this.dottedIdentifierBinder = new DottedIdentifierBinder(FakeBindMethods.BindMethodReturningASinglePerson, this.bindingState);
        }

        [Fact]
        public void CastOnImplicitParameterShouldResultInSingleCastNode()
        {
            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.Employee", null);
            var resultNode = this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            resultNode.ShouldBeSingleCastNode(HardCodedTestModel.GetEmployeeTypeReference())
                .And.Source.ShouldBeEntityRangeVariableReferenceNode(ExpressionConstants.It);
        }

        [Fact]
        public void CastOnDerivedTypeSingleValueParentTokenShouldResultInSingleCastNode()
        {
            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.Employee", new DummyToken());
            var resultNode = this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            resultNode.ShouldBeSingleCastNode(HardCodedTestModel.GetEmployeeTypeReference())
                .And.Source.Should().BeSameAs(FakeBindMethods.FakePersonNode);
        }

        [Fact]
        public void CastOnSingleValueParentTokenShouldResultInSingleCastNode()
        {
            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.Person", new DummyToken());
            var resultNode = this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            resultNode.ShouldBeSingleCastNode(HardCodedTestModel.GetPersonTypeReference())
                .And.Source.Should().BeSameAs(FakeBindMethods.FakePersonNode);
        }

        [Fact]
        public void CastOnDerivedTypeEntityCollectionParentTokenShouldResultInCollectionCastNode()
        {
            this.dottedIdentifierBinder = new DottedIdentifierBinder(FakeBindMethods.BindMethodThatReturnsEntitySetNode, this.bindingState);

            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.Employee", new DummyToken());
            var resultNode = this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            resultNode.ShouldBeCollectionCastNode(HardCodedTestModel.GetEmployeeTypeReference())
                .And.Source.Should().BeSameAs(FakeBindMethods.FakeEntityCollectionNode);
        }

        [Fact]
        public void CastOnEntityCollectionParentTokenShouldResultInCollectionCastNode()
        {
            this.dottedIdentifierBinder = new DottedIdentifierBinder(FakeBindMethods.BindMethodThatReturnsEntitySetNode, this.bindingState);

            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.Person", new DummyToken());
            var resultNode = this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            resultNode.ShouldBeCollectionCastNode(HardCodedTestModel.GetPersonTypeReference())
                .And.Source.Should().BeSameAs(FakeBindMethods.FakeEntityCollectionNode);
        }

        [Fact]
        public void CastWithComplexTypeShouldWork()
        {
            this.dottedIdentifierBinder = new DottedIdentifierBinder(FakeBindMethods.BindSingleValueProperty, this.bindingState);
            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.HomeAddress", new DummyToken());
            var resultNode = this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            resultNode.ShouldBeSingleValueCastNode(HardCodedTestModel.GetHomeAddressReference());
        }

        [Fact]
        public void CastWithCollectionComplexTypeShouldWork()
        {
            this.dottedIdentifierBinder = new DottedIdentifierBinder(FakeBindMethods.BindCollectionProperty, this.bindingState);
            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.HomeAddress", new DummyToken());
            var resultNode = this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            resultNode.ShouldBeCollectionPropertyCastNode(HardCodedTestModel.GetHomeAddressReference());
        }

        [Fact]
        public void OR_NonFlagEnumValues_Error()
        {
            this.dottedIdentifierBinder = new DottedIdentifierBinder(FakeBindMethods.BindMethodThatReturnsEntitySetNode, this.bindingState);

            // NonFlagShape can't OR 2 member values 'Rectangle,foursquare'
            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.NonFlagShape'Rectangle,foursquare'", new DummyToken());
            Action parse = () => this.dottedIdentifierBinder.BindDottedIdentifier(castToken);
            parse.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.Binder_IsNotValidEnumConstant("Fully.Qualified.Namespace.NonFlagShape'Rectangle,foursquare'"));
        }

        [Fact]
        public void CastWithNonChildTypeShouldThrow()
        {
            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.Lion", null);
            Action bind = () => this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            bind.ShouldThrow<ODataException>().
                WithMessage((Strings.MetadataBinder_HierarchyNotFollowed(HardCodedTestModel.GetLionType().FullName(), HardCodedTestModel.GetPersonType().FullName())));

        }

        [Fact]
        public void CastWithOpenTypeShouldThrow()
        {
            this.dottedIdentifierBinder = new DottedIdentifierBinder(FakeBindMethods.BindMethodReturningASingleOpenProperty, this.bindingState);

            var castToken = new DottedIdentifierToken("Fully.Qualified.Namespace.Lion", new EndPathToken("Critics", new DummyToken()));
            Action bind = () => this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            bind.ShouldThrow<ODataException>().
                WithMessage((Strings.MetadataBinder_HierarchyNotFollowed(HardCodedTestModel.GetLionType().FullName(), "<null>")));

        }

        [Fact]
        public void CastOnMissingTypeShouldThrow()
        {
            var castToken = new DottedIdentifierToken("Missing.Type", null);
            Action bind = () => this.dottedIdentifierBinder.BindDottedIdentifier(castToken);

            bind.ShouldThrow<ODataException>().
                WithMessage(Strings.CastBinder_ChildTypeIsNotEntity("Missing.Type"));
        }

        [Fact]
        public void NamespaceQualifiedFunctionIsBoundAsSingleValueFunctionCallNode()
        {
            var dottedIdentifierToken = new DottedIdentifierToken("Fully.Qualified.Namespace.HasJob", null);
            var boundFunctionNode = this.dottedIdentifierBinder.BindDottedIdentifier(dottedIdentifierToken);
            boundFunctionNode.ShouldBeSingleValueFunctionCallQueryNode(HardCodedTestModel.GetHasJobFunction());
        }
    }

    internal class DummyToken : QueryToken
    {
        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { throw new System.NotImplementedException(); }
        }

        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}
