//---------------------------------------------------------------------
// <copyright file="InNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the InNode class
    /// </summary>
    public class InNodeTests
    {
        private FakeSingleEntityNode fakeDogSource = FakeSingleEntityNode.CreateFakeSingleEntityNodeForDog();

        [Fact]
        public void LeftShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode("Doggo");
            CollectionPropertyAccessNode right = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            InNode inNode = new InNode(left, right);
            inNode.Left.Should().Be(left);
        }

        [Fact]
        public void RightShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode("YungPupper");
            CollectionPropertyAccessNode right = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            InNode inNode = new InNode(left, right);
            inNode.Right.Should().Be(right);
        }

        [Fact]
        public void ItemTypeShouldBeSetCorrectlyFromOperands()
        {
            ConstantNode left = new ConstantNode("Doge");
            CollectionPropertyAccessNode right = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            InNode inNode = new InNode(left, right);
            inNode.ItemType.FullName().Should().Be("Edm.String");
        }

        [Fact]
        public void TypeReferenceIsSetCorrectlyFromOperands()
        {
            ConstantNode left = new ConstantNode("Cat");
            CollectionPropertyAccessNode right = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            InNode inNode = new InNode(left, right);
            inNode.TypeReference.FullName().Should().Be("Edm.Boolean");
        }

        [Fact]
        public void KindIsBinaryOperatorNode()
        {
            ConstantNode left = new ConstantNode("Scooby");
            CollectionPropertyAccessNode right = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            InNode inNode = new InNode(left, right);
            inNode.InternalKind.Should().Be(InternalQueryNodeKind.In);
        }
    }
}
