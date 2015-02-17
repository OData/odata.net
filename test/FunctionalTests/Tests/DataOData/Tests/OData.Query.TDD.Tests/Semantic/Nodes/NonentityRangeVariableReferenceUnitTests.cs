//---------------------------------------------------------------------
// <copyright file="NonentityRangeVariableReferenceUnitTests.cs" company="Microsoft">
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
    /// Unit tests for the NonEntityRangeVariableReferenceNode class
    /// </summary>
    [TestClass]
    public class NonentityRangeVariableReferenceUnitTests
    {
        [TestMethod]
        public void NameCannotBeNull()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            Action createWithNullName = () => new NonentityRangeVariableReferenceNode(null, rangeVariable);
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [TestMethod]
        public void RangeVariableCannotBeNull()
        {
            Action createWithNullRangeVariable = () => new NonentityRangeVariableReferenceNode("suff", null);
            createWithNullRangeVariable.ShouldThrow<Exception>(Error.ArgumentNull("rangeVariable").ToString());
        }

        [TestMethod]
        public void NameIsSetCorrectly()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.Name.Should().Be("stuff");
        }

        [TestMethod]
        public void TypeReferenceIsSetCorrectly()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.TypeReference.FullName().Should().Be(EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true).FullName());
        }

        [TestMethod]
        public void RangeVariableIsSetCorrectly()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.RangeVariable.ShouldBeNonentityRangeVariable("stuff");
        }

        [TestMethod]
        public void KindIsNonEntityRangeVariableReferenceNode()
        {
            NonentityRangeVariable rangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            NonentityRangeVariableReferenceNode nonentityRangeVariableReferenceNode = new NonentityRangeVariableReferenceNode("stuff", rangeVariable);
            nonentityRangeVariableReferenceNode.InternalKind.Should().Be(InternalQueryNodeKind.NonentityRangeVariableReference);
        }
    }
}
