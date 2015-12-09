//---------------------------------------------------------------------
// <copyright file="NonentityRangeVariableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the NonentityRangeVariable class
    /// </summary>
    public class NonentityRangeVariableTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new NonentityRangeVariable(null, EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            NonentityRangeVariable nonentityRangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            nonentityRangeVariable.Name.Should().Be("stuff");
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            var typeReference = EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true);
            NonentityRangeVariable nonentityRangeVariable = new NonentityRangeVariable("stuff", typeReference, null);
            nonentityRangeVariable.TypeReference.Should().BeSameAs(typeReference);
        }

        [Fact]
        public void DissallowEntityType()
        {
            var entityTypeRef = HardCodedTestModel.GetPersonTypeReference();
            Action ctor = () => new NonentityRangeVariable("abc", entityTypeRef, null);
            ctor.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_NonentityParameterQueryNodeWithEntityType(entityTypeRef.FullName()));
        }

        [Fact]
        public void KindIsNonEntityRangeVariable()
        {
            NonentityRangeVariable nonentityRangeVariable = new NonentityRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            nonentityRangeVariable.Kind.Should().Be(RangeVariableKind.Nonentity);
        }
    }
}
