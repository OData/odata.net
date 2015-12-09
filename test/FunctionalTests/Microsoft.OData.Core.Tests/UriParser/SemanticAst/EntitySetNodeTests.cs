//---------------------------------------------------------------------
// <copyright file="EntitySetNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class EntitySetNodeTests
    {
        [Fact]
        public void ItemTypeReturnsEdmEntityTypeReference()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            node.ItemType.ShouldBeEquivalentTo(HardCodedTestModel.GetLionTypeReference());
        }

        [Fact]
        public void EntityItemTypeIsSameAsItemType()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            node.EntityItemType.Should().BeSameAs(node.ItemType);
        }

        [Fact]
        public void EntitySetComesFromSource()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            node.NavigationSource.Should().Be(HardCodedTestModel.GetLionSet());
        }

        [Fact]
        public void KindIsEntitySet()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            node.InternalKind.Should().Be(InternalQueryNodeKind.EntitySet);
        }
    }
}
