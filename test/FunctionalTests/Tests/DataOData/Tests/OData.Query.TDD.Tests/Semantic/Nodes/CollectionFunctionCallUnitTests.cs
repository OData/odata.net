//---------------------------------------------------------------------
// <copyright file="CollectionFunctionCallUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Unit tests for the CollectionFunctionCallNode class
    /// </summary>
    [TestClass]
    public class CollectionFunctionCallNodeUnitTests
    {
        private readonly IEdmTypeReference itemTypeReference;
        private readonly IEdmCollectionTypeReference collectionTypeReference;

        public CollectionFunctionCallNodeUnitTests()
        {
            itemTypeReference = EdmCoreModel.Instance.GetString(true);
            collectionTypeReference = new EdmCollectionType(itemTypeReference).ToTypeReference().AsCollection();
        }

        [TestMethod]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new CollectionFunctionCallNode(null, null, new QueryNode[] { }, collectionTypeReference, null);
            createWithNullName.ShouldThrow<ArgumentNullException>().WithMessage("name", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void CollectionTypeReferenceCannotBeNull()
        {
            Action createWithNullType = () => new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { }, null, null);
            createWithNullType.ShouldThrow<ArgumentNullException>().WithMessage("collectionType", ComparisonMode.EquivalentSubstring);
        }

        [TestMethod]
        public void FunctionImportsAreSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", HardCodedTestModel.GetHasDogOverloads(), null, collectionTypeReference, null);
            collectionFunctionCallNode.Functions.Should().ContainExactly(HardCodedTestModel.GetHasDogOverloads());
        }

        [TestMethod]
        public void NameIsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", HardCodedTestModel.GetHasDogOverloads(), null, collectionTypeReference, null);
            collectionFunctionCallNode.Name.Should().Be("stuff");
        }

        [TestMethod]
        public void ArgumentsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            collectionFunctionCallNode.Parameters.Should().HaveCount(1);
            collectionFunctionCallNode.Parameters.ElementAt(0).ShouldBeConstantQueryNode(1);
        }

        [TestMethod]
        public void ItemTypeSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            collectionFunctionCallNode.ItemType.Should().BeSameAs(this.itemTypeReference);
        }

        [TestMethod]
        public void KindIsSetCorrectly()
        {
            var collectionFunctionCallNode = new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, collectionTypeReference, null);
            collectionFunctionCallNode.Kind.Should().Be(QueryNodeKind.CollectionFunctionCall);
        }

        [TestMethod]
        public void ItemTypeMustBePrimitiveOrComplex()
        {
            Action createWithEntityCollectionType = () => new CollectionFunctionCallNode("stuff", new List<IEdmFunction>() { HardCodedTestModel.GetFunctionForHasJob() }, new QueryNode[] { new ConstantNode(1) }, new EdmCollectionTypeReference(new EdmCollectionType(ModelBuildingHelpers.BuildValidEntityType().ToTypeReference())), null);
            createWithEntityCollectionType.ShouldThrow<ArgumentException>().WithMessage(ODataErrorStrings.Nodes_CollectionFunctionCallNode_ItemTypeMustBePrimitiveOrComplexOrEnum);
        }
    }
}
