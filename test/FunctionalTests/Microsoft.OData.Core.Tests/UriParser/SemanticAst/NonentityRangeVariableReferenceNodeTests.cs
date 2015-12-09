//---------------------------------------------------------------------
// <copyright file="NonentityRangeVariableReferenceNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the NonEntityRangeVariableReferenceNode class
    /// </summary>
    public class NonentityRangeVariableReferenceNodeTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            Action createWithNullName = () => new NonentityRangeVariableReferenceNode(null, rangeVariable);
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void RangeVariableCannotBeNull()
        {
            Action createWithNullRangeVariable = () => new NonentityRangeVariableReferenceNode("suff", null);
            createWithNullRangeVariable.ShouldThrow<Exception>(Error.ArgumentNull("rangeVariable").ToString());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.Name.Should().Be("stuff");
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.TypeReference.FullName().Should().Be(EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true).FullName());
        }

        [Fact]
        public void RangeVariableIsSetCorrectly()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.RangeVariable.ShouldBeNonentityRangeVariable("stuff");
        }

        [Fact]
        public void KindIsNonEntityRangeVariableReferenceNode()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.InternalKind.Should().Be(InternalQueryNodeKind.NonentityRangeVariableReference);
        }
    }
}
