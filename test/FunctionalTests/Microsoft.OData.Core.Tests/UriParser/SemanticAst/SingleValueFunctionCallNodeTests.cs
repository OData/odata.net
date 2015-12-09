//---------------------------------------------------------------------
// <copyright file="SingleValueFunctionCallNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the SingleValueFunctionCallNode class
    /// </summary>
    public class SingleValueFunctionCallNodeTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new SingleValueFunctionCallNode(null, null, EdmCoreModel.Instance.GetInt32(true));
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void ReturnTypeCanBeNull()
        {
            Action createWithNullReturnType = () => new SingleValueFunctionCallNode("stuff", null, null);
            createWithNullReturnType.ShouldNotThrow();
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.Name.Should().Be("stuff");
        }

        [Fact]
        public void ArgumentsAreSetCorrectly()
        {
            QueryNode[] args = new QueryNode[]
                {
                    new ConstantNode(1),
                    new ConstantNode(2),
                    new ConstantNode(3),
                    new ConstantNode(4),
                    new ConstantNode(5) 
                };
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", args, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.Parameters.Should().BeEquivalentTo(args);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.TypeReference.FullName().Should().Be(EdmCoreModel.Instance.GetInt32(true).FullName());
        }

        [Fact]
        public void TypeReferenceShouldNotBeCollection()
        {
            Action createWithCollectionReturnType = () => new SingleValueFunctionCallNode("stuff", null, new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true)).ToTypeReference().AsCollection());
            createWithCollectionReturnType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }

        [Fact]
        public void TypeReferenceShouldNotBeEntity()
        {
            Action createWithCollectionReturnType = () => new SingleValueFunctionCallNode("stuff", null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity());
            createWithCollectionReturnType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }

        [Fact]
        public void KindIsSingleValueFunctionCall()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.InternalKind.Should().Be(InternalQueryNodeKind.SingleValueFunctionCall);
        }

        [Fact]
        public void ArgumentsBeConvertedToEmptyCollection()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.Parameters.Should().BeEmpty();
        }
    }
}
