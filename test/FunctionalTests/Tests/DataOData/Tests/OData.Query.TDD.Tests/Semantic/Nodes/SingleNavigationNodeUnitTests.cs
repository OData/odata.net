//---------------------------------------------------------------------
// <copyright file="SingleNavigationNodeUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Nodes
{
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;

    [TestClass]
    public class SingleNavigationNodeUnitTests
    {
        [TestMethod]
        public void SourceIsSet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.Source.Should().BeSameAs(source);
        }

        [TestMethod]
        public void TypeReferenceIsExactlyFromProperty()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.TypeReference.Should().BeSameAs(HardCodedTestModel.GetPersonMyDogNavProp().Type);
        }

        [TestMethod]
        public void EntityTypeIsSameAsType()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.EntityTypeReference.Should().BeSameAs(node.TypeReference);
        }

        [TestMethod]
        public void EntitySetIsCalculatedCorrectly()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.NavigationSource.Should().BeSameAs(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void SingleNavigationNodeHandlesNullEntitySetOnParentNode()
        {
            var source = new FakeSingleEntityNode(HardCodedTestModel.GetPersonTypeReference(), null);
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.NavigationSource.Should().BeNull();
        }

        [TestMethod]
        public void SingleNavigationNodeHandlesNullSourceSetParameter()
        {
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), (IEdmEntitySet)null);
            node.NavigationSource.Should().BeNull();
        }

        [TestMethod]
        public void KindIsEntitySet()
        {
            var source = FakeSingleEntityNode.CreateFakeSingleEntityNodeForPerson();
            var node = new SingleNavigationNode(HardCodedTestModel.GetPersonMyDogNavProp(), source);

            node.InternalKind.Should().Be(InternalQueryNodeKind.SingleNavigationNode);
        }
    }
}
