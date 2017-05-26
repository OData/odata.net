//---------------------------------------------------------------------
// <copyright file="NonentityRangeVariableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the NonResourceRangeVariable class
    /// </summary>
    public class NonResourceRangeVariableTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new NonResourceRangeVariable(null, EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [Fact]
        public void NameIsSetCorrectly()
        {
            NonResourceRangeVariable nonentityRangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            nonentityRangeVariable.Name.Should().Be("stuff");
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            var typeReference = EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true);
            NonResourceRangeVariable nonentityRangeVariable = new NonResourceRangeVariable("stuff", typeReference, null);
            nonentityRangeVariable.TypeReference.Should().BeSameAs(typeReference);
        }

        [Fact]
        public void DissallowEntityType()
        {
            var entityTypeRef = HardCodedTestModel.GetPersonTypeReference();
            Action ctor = () => new NonResourceRangeVariable("abc", entityTypeRef, null);
            ctor.ShouldThrow<ArgumentException>().WithMessage(Strings.Nodes_NonentityParameterQueryNodeWithEntityType(entityTypeRef.FullName()));
        }

        [Fact]
        public void KindIsNonEntityRangeVariable()
        {
            NonResourceRangeVariable nonentityRangeVariable = new NonResourceRangeVariable("stuff", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, true), null);
            nonentityRangeVariable.Kind.Should().Be(RangeVariableKind.NonResource);
        }
    }
}
