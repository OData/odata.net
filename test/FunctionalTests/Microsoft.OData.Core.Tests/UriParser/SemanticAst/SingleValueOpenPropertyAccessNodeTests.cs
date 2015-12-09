//---------------------------------------------------------------------
// <copyright file="SingleValueOpenPropertyAccessNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Tests for SingleValueOpenPropertyAccessNode class. 
    /// </summary>
    public class SingleValueOpenPropertyAccessNodeTests
    {
        private const string ExpectedPropertyName = "ThisIsAnOpenProperty";

        private SingleValueOpenPropertyAccessNode node;
        private SingleValueNode sourceNode;

        public SingleValueOpenPropertyAccessNodeTests()
        {
            this.sourceNode = new ConstantNode(null);
            this.node = new SingleValueOpenPropertyAccessNode(this.sourceNode, ExpectedPropertyName);
        }

        [Fact]
        public void GetKindExpectCorrectKind()
        {
            this.node.InternalKind.Should().Be(InternalQueryNodeKind.SingleValueOpenPropertyAccess);
        }

        [Fact]
        public void GetNameExpectInitialName()
        {
            this.node.Name.Should().Be(ExpectedPropertyName);
        }

        [Fact]
        public void GetTypeReferenceExpectNull()
        {
            this.node.TypeReference.Should().BeNull();
        }

        [Fact]
        public void CreateWithNullNameExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(this.sourceNode,  null);
            ctor.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithEmptyNameExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(this.sourceNode, String.Empty);
            ctor.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetSourceExpectInitialSource()
        {
            this.node.Source.Should().BeSameAs(this.sourceNode);
        }

        [Fact]
        public void CreateWithNullSourceExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(null, ExpectedPropertyName);
            ctor.ShouldThrow<ArgumentNullException>();
        }
    }
}
