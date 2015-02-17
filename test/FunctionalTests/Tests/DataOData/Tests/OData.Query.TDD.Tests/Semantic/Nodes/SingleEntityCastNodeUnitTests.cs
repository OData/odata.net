//---------------------------------------------------------------------
// <copyright file="SingleEntityCastNodeUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the SingleEntityCastNode class
    /// </summary>
    [TestClass]
    public class SingleEntityCastNodeUnitTests
    {
        private readonly EntityRangeVariableReferenceNode singleEntityNode = new EntityRangeVariableReferenceNode("a", new EntityRangeVariable("a", new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false), HardCodedTestModel.GetPeopleSet()));

        [TestMethod]
        public void EntityTypeCannotBeNull()
        {
            Action createWithNullEntityType = () => new SingleEntityCastNode(this.singleEntityNode, null);
            createWithNullEntityType.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [TestMethod]
        public void SourceIsSetCorrectly()
        {
            SingleEntityCastNode singleEntityCast = new SingleEntityCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.Source.Should().Be(this.singleEntityNode);
        }

        [TestMethod]
        public void TypeReferenceIsSetCorrectly()
        {
            SingleEntityCastNode singleEntityCast = new SingleEntityCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.TypeReference.FullName().Should().Be(HardCodedTestModel.GetPersonType().FullName());
        }

        [TestMethod]
        public void EntityTypeReferenceIsSameAsTypeReference()
        {
            SingleEntityCastNode singleEntityCast = new SingleEntityCastNode(this.singleEntityNode, HardCodedTestModel.GetPersonType());
            singleEntityCast.EntityTypeReference.Should().BeSameAs(singleEntityCast.TypeReference);
        }

        [TestMethod]
        public void KindIsSingleEntityCastNode()
        {
            SingleEntityCastNode singleEntityCast = new SingleEntityCastNode(this.singleEntityNode, HardCodedTestModel.GetDogType());
            singleEntityCast.InternalKind.Should().Be(InternalQueryNodeKind.SingleEntityCast);
        }
    }
}
