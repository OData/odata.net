//---------------------------------------------------------------------
// <copyright file="MetadataBindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit test for the MetadataBinding class.
    /// </summary>
    public class MetadataBindingTests
    {
        private static readonly ODataUriParserConfiguration configuration = new ODataUriParserConfiguration(HardCodedTestModel.TestModel);

        #region Tests
        [Fact]
        public void MetadataBinderProcessSkipWithNullReturnsNull()
        {
            long? result = MetadataBinder.ProcessSkip(null);
            result.Should().Be(null);
        }

        [Fact]
        public void MetadataBinderProcessSkipWithNegativeNumberShouldThrow()
        {
            Action metadataBinderAction = () => MetadataBinder.ProcessSkip(-1);
            metadataBinderAction.ShouldThrow<ODataException>().WithMessage(
                ODataErrorStrings.MetadataBinder_SkipRequiresNonNegativeInteger("-1"));
        }

        [Fact]
        public void MetadataBinderProcessSkipWithPositiveNumberShouldReturnNumber()
        {
            long? result = MetadataBinder.ProcessSkip(1);
            result.Should().Be(1);
        }

        [Fact]
        public void MetadataBinderProcessTopWithNullReturnsNull()
        {
            long? result = MetadataBinder.ProcessTop(null);
            result.Should().Be(null);
        }

        [Fact]
        public void MetadataBinderProcessTopWithNegativeNumberShouldThrow()
        {
            Action metadataBinderAction = () => MetadataBinder.ProcessTop(-1);
            metadataBinderAction.ShouldThrow<ODataException>().WithMessage(
                ODataErrorStrings.MetadataBinder_TopRequiresNonNegativeInteger("-1"));
        }

        [Fact]
        public void MetadataBinderProcessTopWithPositiveNumberShouldReturnNumber()
        {
            long? result = MetadataBinder.ProcessTop(1);
            result.Should().Be(1);
        }

        [Fact]
        public void MetadataBinderProcessQueryOptionsWithNullBindStateShouldThrow()
        {
            BindInfo bindInfo = new BindInfo();

            // Test null bind state
            Action metadataBinderAction = () => MetadataBinder.ProcessQueryOptions(null, bindInfo.BindMethod);
            metadataBinderAction.ShouldThrow<ODataException>(
                ODataErrorStrings.MetadataBinder_QueryOptionsBindStateCannotBeNull);

            // Test bind state that doesn't have query options populated
            metadataBinderAction = () => MetadataBinder.ProcessQueryOptions(bindInfo.BindingState, bindInfo.BindMethod);
            metadataBinderAction.ShouldThrow<ODataException>(
                ODataErrorStrings.MetadataBinder_QueryOptionsBindStateCannotBeNull);
        }

        [Fact]
        public void MetadataBinderProcessQueryOptionsWithNullBindMethodShouldThrow()
        {
            BindInfo bindInfo = new BindInfo();

            Action metadataBinderAction = () => MetadataBinder.ProcessQueryOptions(bindInfo.BindingState, null);
            metadataBinderAction.ShouldThrow<ODataException>(
                ODataErrorStrings.MetadataBinder_QueryOptionsBindMethodCannotBeNull);
        }

        [Fact]
        public void MetadataBinderProcessQueryOptionsWithBindMethodThatReturnsNullShouldReturnEmptyList()
        {
            List<CustomQueryOptionToken> queryOptions = new List<CustomQueryOptionToken>();
            queryOptions.Add(new CustomQueryOptionToken(null, string.Empty));

            BindInfo bindInfo = new BindInfo(queryOptions, BindMethodReturnsNull);

            List<QueryNode> result = MetadataBinder.ProcessQueryOptions(bindInfo.BindingState, bindInfo.BindMethod);
            result.Count.Should().Be(0);
        }

        [Fact]
        public void MetadataBinderProcessQueryOptionsWithBindMethodThatReturnsNodeShouldReturnList()
        {
            List<CustomQueryOptionToken> queryOptions = new List<CustomQueryOptionToken>();
            queryOptions.Add(new CustomQueryOptionToken(null, string.Empty));

            BindInfo bindInfo = new BindInfo(queryOptions, BindMethodReturnsNode);

            List<QueryNode> result = MetadataBinder.ProcessQueryOptions(bindInfo.BindingState, bindInfo.BindMethod);
            result.Count.Should().Be(1);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// An object to hold info for binding
        /// </summary>
        private struct BindInfo
        {
            /// <summary>
            /// Encapsulates the state of the metadata binding.
            /// </summary>
            private BindingState bindingState;

            /// <summary>
            /// Encapsulates the bind method.
            /// </summary>
            private MetadataBinder.QueryTokenVisitor bindMethod;

            /// <summary>
            /// Encapsulates the binder.
            /// </summary>
            private MetadataBinder binder;

            /// <summary>
            /// Constructs a BindInfo with the given <paramref name="bindMethod"/>.
            /// </summary>
            /// <param name="bindingStateQueryOptions">The query options to be passed for the binding state. Null by default.</param>
            /// <param name="bindMethodDelegate">The bind method algorithm. By default, uses MetadataBinder method
            /// if this parameter isn't populated.</param>
            public BindInfo(List<CustomQueryOptionToken> bindingStateQueryOptions = null, MetadataBinder.QueryTokenVisitor bindMethod = null)
            {
                ResourceRangeVariable implicitRangeVariable = new ResourceRangeVariable(
                    ExpressionConstants.It,
                    HardCodedTestModel.GetPersonTypeReference(),
                    HardCodedTestModel.GetPeopleSet());
                this.bindingState = new BindingState(configuration) { ImplicitRangeVariable = implicitRangeVariable };
                this.bindingState.RangeVariables.Push(
                    new BindingState(configuration) { ImplicitRangeVariable = implicitRangeVariable }.ImplicitRangeVariable);
                
                if (bindingStateQueryOptions != null)
                {
                    this.bindingState.QueryOptions = bindingStateQueryOptions;
                }

                this.binder = new MetadataBinder(bindingState);
                this.bindMethod = bindMethod ?? binder.Bind;
            }

            /// <summary>
            /// Encapsulates the state of the metadata binding.
            /// </summary>
            public BindingState BindingState
            {
                get
                {
                    return this.bindingState;
                }
            }

            /// <summary>
            /// Encapsulates the state of the bind method.
            /// </summary>
            public MetadataBinder.QueryTokenVisitor BindMethod
            {
                get
                {
                    return this.bindMethod;
                }
            }
        }

        /// <summary>
        /// Helper bind method that returns null. Parameter is unused.
        /// </summary>
        private QueryNode BindMethodReturnsNull(QueryToken token)
        {
            return null;
        }

        /// <summary>
        /// Helper bind method that returns null. Parameter is unused.
        /// </summary>
        private QueryNode BindMethodReturnsNode(QueryToken token)
        {
            ResourceRangeVariable implicitRangeVariable = new ResourceRangeVariable(
                ExpressionConstants.It,
                HardCodedTestModel.GetPersonTypeReference(),
                HardCodedTestModel.GetPeopleSet());

            return NodeFactory.CreateRangeVariableReferenceNode(implicitRangeVariable);
        }
        #endregion
    }
}
