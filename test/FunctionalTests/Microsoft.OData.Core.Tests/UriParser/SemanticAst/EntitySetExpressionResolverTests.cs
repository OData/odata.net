//---------------------------------------------------------------------
// <copyright file="EntitySetExpressionResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm.Library.Expressions;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Tests for classes in the semantic tree object model that we have not fully unit tested yet.
    /// </summary>
    public class EntitySetExpressionResolverTests
    {
        [Fact]
        public void ResolveEntitySetFromExpressionCanResolveStaticEntitySets()
        {
            var expression = new EdmEntitySetReferenceExpression(HardCodedTestModel.GetLionSet());
            var result = EntitySetExpressionResolver.ResolveEntitySetFromExpression(expression);
            result.Should().Be(HardCodedTestModel.GetLionSet());
        }

        [Fact]
        public void ResolveEntitySetFromExpressionResolvesNullToNull()
        {
            var result = EntitySetExpressionResolver.ResolveEntitySetFromExpression(null);
            result.Should().BeNull();
        }
    }
}
