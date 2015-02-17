//---------------------------------------------------------------------
// <copyright file="SingleValueOpenPropertyAccessNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for SingleValueOpenPropertyAccessNode class. 
    /// </summary>
    [TestClass]
    public class SingleValueOpenPropertyAccessNodeTests
    {
        private const string ExpectedPropertyName = "ThisIsAnOpenProperty";

        private SingleValueOpenPropertyAccessNode node;
        private SingleValueNode sourceNode;

        [TestInitialize]
        public void TestInitialize()
        {
            this.sourceNode = new ConstantNode(null);
            this.node = new SingleValueOpenPropertyAccessNode(this.sourceNode, ExpectedPropertyName);
        }

        [TestMethod]
        public void GetKindExpectCorrectKind()
        {
            this.node.InternalKind.Should().Be(InternalQueryNodeKind.SingleValueOpenPropertyAccess);
        }

        [TestMethod]
        public void GetNameExpectInitialName()
        {
            this.node.Name.Should().Be(ExpectedPropertyName);
        }

        [TestMethod]
        public void GetTypeReferenceExpectNull()
        {
            this.node.TypeReference.Should().BeNull();
        }

        [TestMethod]
        public void CreateWithNullNameExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(this.sourceNode,  null);
            ctor.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void CreateWithEmptyNameExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(this.sourceNode, String.Empty);
            ctor.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        public void GetSourceExpectInitialSource()
        {
            this.node.Source.Should().BeSameAs(this.sourceNode);
        }

        [TestMethod]
        public void CreateWithNullSourceExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(null, ExpectedPropertyName);
            ctor.ShouldThrow<ArgumentNullException>();
        }
    }
}
