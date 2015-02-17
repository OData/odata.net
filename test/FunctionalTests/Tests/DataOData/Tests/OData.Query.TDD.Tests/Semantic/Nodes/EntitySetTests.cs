//---------------------------------------------------------------------
// <copyright file="EntitySetTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Nodes
{
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;

    [TestClass]
    public class EntitySetTests
    {
        [TestMethod]
        public void ItemTypeReturnsEdmEntityTypeReference()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            node.ItemType.ShouldBeEquivalentTo(HardCodedTestModel.GetLionTypeReference());
        }

        [TestMethod]
        public void EntityItemTypeIsSameAsItemType()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            node.EntityItemType.Should().BeSameAs(node.ItemType);
        }

        [TestMethod]
        public void EntitySetComesFromSource()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            node.NavigationSource.Should().Be(HardCodedTestModel.GetLionSet());
        }

        [TestMethod]
        public void KindIsEntitySet()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            node.InternalKind.Should().Be(InternalQueryNodeKind.EntitySet);
        }
    }
}
