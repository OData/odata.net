//---------------------------------------------------------------------
// <copyright file="SingleValueFunctionCallNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.Test.OData.Utils.Metadata;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
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
            Assert.Throws<ArgumentNullException>("name", createWithNullName);
        }

        [Fact]
        public void ReturnTypeCanBeNull()
        {
            Action createWithNullReturnType = () => new SingleValueFunctionCallNode("stuff", null, null);
            createWithNullReturnType.DoesNotThrow();
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            Assert.Equal("stuff", singleValueFunction.Name);
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

            Assert.Equal(5, singleValueFunction.Parameters.Count());
            int index = 1;
            foreach (var parameter in singleValueFunction.Parameters)
            {
                var constantNode = Assert.IsType<ConstantNode>(parameter);
                Assert.Equal(index++, constantNode.Value);
            }
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            Assert.Equal(singleValueFunction.TypeReference.FullName(), EdmCoreModel.Instance.GetInt32(true).FullName());
        }

        [Fact]
        public void TypeReferenceShouldNotBeCollection()
        {
            Action createWithCollectionReturnType = () => new SingleValueFunctionCallNode("stuff", null, new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true)).ToTypeReference().AsCollection());
            createWithCollectionReturnType.Throws<ArgumentException>(ODataErrorStrings.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }

        [Fact]
        public void TypeReferenceShouldNotBeEntity()
        {
            Action createWithCollectionReturnType = () => new SingleValueFunctionCallNode("stuff", null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity());
            createWithCollectionReturnType.Throws<ArgumentException>(ODataErrorStrings.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }

        [Fact]
        public void KindIsSingleValueFunctionCall()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            Assert.Equal(InternalQueryNodeKind.SingleValueFunctionCall, singleValueFunction.InternalKind);
        }

        [Fact]
        public void ArgumentsBeConvertedToEmptyCollection()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            Assert.Empty(singleValueFunction.Parameters);
        }
    }
}
