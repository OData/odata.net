//---------------------------------------------------------------------
// <copyright file="EntityCollectionCastNodeUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Library;

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Nodes
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// TODO: test the rest of EntityCollectionCastNode
    /// </summary>
    [TestClass]
    public class EntityCollectionCastNodeUnitTests
    {
        private readonly EntityCollectionNode fakeSource = new EntitySetNode(HardCodedTestModel.GetPeopleSet());

        [TestMethod]
        public void ItemTypeReferenceShouldBeCreatedFromConstructorParameter()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.ItemType.Definition.Should().BeSameAs(HardCodedTestModel.GetEmployeeType());
            node.ItemType.IsNullable.Should().BeFalse();
        }

        [TestMethod]
        public void CollectionTypeShouldBeCreatedFromItemType()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.CollectionType.FullName().Should().Be("Collection(Fully.Qualified.Namespace.Employee)");
        }

        [TestMethod]
        public void CollectionTypeShouldBeSaved()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.CollectionType.Should().BeSameAs(node.CollectionType);
        }

        [TestMethod]
        public void ItemTypeShouldBeSaved()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.ItemType.Should().BeSameAs(node.ItemType);
        }

        [TestMethod]
        public void EntityItemTypeShouldBeSameAsItemType()
        {
            var node = new EntityCollectionCastNode(fakeSource, HardCodedTestModel.GetEmployeeType());
            node.EntityItemType.Should().BeSameAs(node.ItemType);
        }
    }
}
