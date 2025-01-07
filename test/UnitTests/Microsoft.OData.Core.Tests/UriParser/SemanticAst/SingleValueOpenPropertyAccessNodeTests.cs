//---------------------------------------------------------------------
// <copyright file="SingleValueOpenPropertyAccessNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
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
            Assert.Equal(InternalQueryNodeKind.SingleValueOpenPropertyAccess, this.node.InternalKind);
        }

        [Fact]
        public void GetNameExpectInitialName()
        {
            Assert.Equal(ExpectedPropertyName, this.node.Name);
        }

        [Fact]
        public void GetTypeReferenceExpectNull()
        {
            Assert.Null(this.node.TypeReference);
        }

        [Fact]
        public void CreateWithNullNameExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(this.sourceNode,  null);
            Assert.Throws<ArgumentNullException>("openPropertyName", ctor);
        }

        [Fact]
        public void CreateWithEmptyNameExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(this.sourceNode, String.Empty);
            Assert.Throws<ArgumentNullException>("openPropertyName", ctor);
        }

        [Fact]
        public void GetSourceExpectInitialSource()
        {
            Assert.Same(this.sourceNode, this.node.Source);
        }

        [Fact]
        public void CreateWithNullSourceExpectException()
        {
            Action ctor = () => new SingleValueOpenPropertyAccessNode(null, ExpectedPropertyName);
            Assert.Throws<ArgumentNullException>(ctor);
        }
    }
}
