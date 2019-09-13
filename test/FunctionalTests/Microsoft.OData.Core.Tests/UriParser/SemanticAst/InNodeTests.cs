//---------------------------------------------------------------------
// <copyright file="InNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
            Assert.Same(left, inNode.Left);
        }

        [Fact]
        public void RightShouldBeSetCorrectly()
        {
            ConstantNode left = new ConstantNode("YungPupper");
            CollectionPropertyAccessNode right = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            InNode inNode = new InNode(left, right);
            Assert.Same(right, inNode.Right);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectlyFromOperands()
        {
            ConstantNode left = new ConstantNode("Cat");
            CollectionPropertyAccessNode right = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            InNode inNode = new InNode(left, right);
            Assert.Equal("Edm.Boolean", inNode.TypeReference.FullName());
        }

        [Fact]
        public void KindIsBinaryOperatorNode()
        {
            ConstantNode left = new ConstantNode("Scooby");
            CollectionPropertyAccessNode right = new CollectionPropertyAccessNode(this.fakeDogSource, HardCodedTestModel.GetDogNicknamesProperty());
            InNode inNode = new InNode(left, right);
            Assert.Equal(InternalQueryNodeKind.In, inNode.InternalKind);
        }
    }
}
