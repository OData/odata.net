//---------------------------------------------------------------------
// <copyright file="NonentityRangeVariableUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the NonentityRangeVariable class
    /// </summary>
    [TestClass]
    public class NonentityRangeVariableUnitTests
    {
        [TestMethod]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new NonentityRangeVariable(null, EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [TestMethod]
        public void NameIsSetCorrectly()
        {
            NonentityRangeVariable nonentityRangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            nonentityRangeVariable.Name.Should().Be("stuff");
        }

        [TestMethod]
        public void TypeReferenceIsSetCorrectly()
        {
            var typeReference = EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true);
            NonentityRangeVariable nonentityRangeVariable = new NonentityRangeVariable("stuff", typeReference, null);
            nonentityRangeVariable.TypeReference.Should().BeSameAs(typeReference);
        }

        [TestMethod]
        public void DissallowEntityType()
        {
            var entityTypeRef = HardCodedTestModel.GetPersonTypeReference();
            Action ctor = () => new NonentityRangeVariable("abc", entityTypeRef, null);
            ctor.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_NonentityParameterQueryNodeWithEntityType(entityTypeRef.FullName()));
        }

        [TestMethod]
        public void KindIsNonEntityRangeVariable()
        {
            NonentityRangeVariable nonentityRangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            nonentityRangeVariable.Kind.Should().Be(RangeVariableKind.Nonentity);
        }
    }
}
