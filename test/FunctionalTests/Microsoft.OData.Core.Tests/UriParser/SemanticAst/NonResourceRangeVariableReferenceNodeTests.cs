//---------------------------------------------------------------------
// <copyright file="NonentityRangeVariableReferenceNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the NonEntityRangeVariableReferenceNode class
    /// </summary>
    public class NonResourceRangeVariableReferenceNodeTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            NonResourceRangeVariable rangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            Action createWithNullName = () => new NonResourceRangeVariableReferenceNode(null, rangeVariable);
            Assert.Throws<ArgumentNullException>("name", createWithNullName);
        }

        [Fact]
        public void RangeVariableCannotBeNull()
        {
            Action createWithNullRangeVariable = () => new NonResourceRangeVariableReferenceNode("suff", null);
            Assert.Throws<ArgumentNullException>("rangeVariable", createWithNullRangeVariable);
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            NonResourceRangeVariable rangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonResourceRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonResourceRangeVariableReferenceNode("stuff", rangeVariable);
            Assert.Equal("stuff", nonentityRangeVariableReferenceNode.Name);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            NonResourceRangeVariable rangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonResourceRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonResourceRangeVariableReferenceNode("stuff", rangeVariable);
            Assert.Equal(EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true).FullName(), nonentityRangeVariableReferenceNode.TypeReference.FullName());
        }

        [Fact]
        public void RangeVariableIsSetCorrectly()
        {
            NonResourceRangeVariable rangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonResourceRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonResourceRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.RangeVariable.ShouldBeNonentityRangeVariable("stuff");
        }

        [Fact]
        public void KindIsNonEntityRangeVariableReferenceNode()
        {
            NonResourceRangeVariable rangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonResourceRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonResourceRangeVariableReferenceNode("stuff", rangeVariable);
            Assert.Equal(InternalQueryNodeKind.NonResourceRangeVariableReference, nonentityRangeVariableReferenceNode.InternalKind);
        }
    }
}
