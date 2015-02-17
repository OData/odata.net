//---------------------------------------------------------------------
// <copyright file="EdmAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.TestUtilities
{
    using FluentAssertions;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Fluent assertions for EDM objects.
    /// </summary>
    public static class EdmAssertions
    {
        public static AndConstraint<IEdmTypeReference> ShouldBeEquivalentTo(this IEdmTypeReference typeReference, IEdmTypeReference expectedTypeReference)
        {
            typeReference.IsEquivalentTo(expectedTypeReference).Should().BeTrue();
            ////typeReference.Should().BeSameAs(expectedTypeReference.Definition);
            ////typeReference.IsNullable.Should().Be(expectedTypeReference.IsNullable);
            return new AndConstraint<IEdmTypeReference>(typeReference);
        }

        public static AndConstraint<IEdmType> ShouldBeEquivalentTo(this IEdmType type, IEdmType expectedType)
        {
            type.IsEquivalentTo(expectedType).Should().BeTrue();
            ////typeReference.Should().BeSameAs(expectedTypeReference.Definition);
            ////typeReference.IsNullable.Should().Be(expectedTypeReference.IsNullable);
            return new AndConstraint<IEdmType>(type);
        }
    }
}
