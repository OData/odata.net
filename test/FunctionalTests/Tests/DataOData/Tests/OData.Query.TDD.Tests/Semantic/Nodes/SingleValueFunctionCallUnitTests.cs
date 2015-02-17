//---------------------------------------------------------------------
// <copyright file="SingleValueFunctionCallUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Unit tests for the SingleValueFunctionCallNode class
    /// </summary>
    [TestClass]
    public class SingleValueFunctionCallUnitTests
    {
        [TestMethod]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new SingleValueFunctionCallNode(null, null, EdmCoreModel.Instance.GetInt32(true));
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [TestMethod]
        public void ReturnTypeCanBeNull()
        {
            Action createWithNullReturnType = () => new SingleValueFunctionCallNode("stuff", null, null);
            createWithNullReturnType.ShouldNotThrow();
        }

        [TestMethod]
        public void NameIsSetCorrectly()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.Name.Should().Be("stuff");
        }

        [TestMethod]
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

        [TestMethod]
        public void TypeReferenceIsSetCorrectly()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.TypeReference.FullName().Should().Be(EdmCoreModel.Instance.GetInt32(true).FullName());
        }

        [TestMethod]
        public void TypeReferenceShouldNotBeCollection()
        {
            Action createWithCollectionReturnType = () => new SingleValueFunctionCallNode("stuff", null, new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true)).ToTypeReference().AsCollection());
            createWithCollectionReturnType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }

        [TestMethod]
        public void TypeReferenceShouldNotBeEntity()
        {
            Action createWithCollectionReturnType = () => new SingleValueFunctionCallNode("stuff", null, ModelBuildingHelpers.BuildValidEntityType().ToTypeReference().AsEntity());
            createWithCollectionReturnType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_SingleValueFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }

        [TestMethod]
        public void KindIsSingleValueFunctionCall()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.InternalKind.Should().Be(InternalQueryNodeKind.SingleValueFunctionCall);
        }

        [TestMethod]
        public void ArgumentsBeConvertedToEmptyCollection()
        {
            SingleValueFunctionCallNode singleValueFunction = new SingleValueFunctionCallNode("stuff", null, EdmCoreModel.Instance.GetInt32(true));
            singleValueFunction.Parameters.Should().BeEmpty();
        }
    }
}
