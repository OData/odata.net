//---------------------------------------------------------------------
// <copyright file="EntitySetNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class EntitySetNodeTests
    {
        [Fact]
        public void ItemTypeReturnsEdmEntityTypeReference()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            Assert.True(node.ItemType.IsEquivalentTo(HardCodedTestModel.GetLionTypeReference()));
        }

        [Fact]
        public void EntityItemTypeIsSameAsItemType()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            Assert.Same(node.ItemType, node.EntityItemType);
        }

        [Fact]
        public void EntitySetComesFromSource()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            Assert.Same(HardCodedTestModel.GetLionSet(), node.NavigationSource);
        }

        [Fact]
        public void KindIsEntitySet()
        {
            var node = new EntitySetNode(HardCodedTestModel.GetLionSet());
            Assert.Equal(InternalQueryNodeKind.EntitySet, node.InternalKind);
        }
    }
}
