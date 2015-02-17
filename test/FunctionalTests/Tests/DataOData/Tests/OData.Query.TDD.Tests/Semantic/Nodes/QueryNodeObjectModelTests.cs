//---------------------------------------------------------------------
// <copyright file="QueryNodeObjectModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using FluentAssertions;
    using Microsoft.OData.Edm.Library.Expressions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for classes in the semantic tree object model that we have not fully unit tested yet.
    /// </summary>
    [TestClass]
    public class QueryNodeObjectModelTests
    {
        [TestMethod]
        public void ResolveEntitySetFromExpressionCanResolveStaticEntitySets()
        {
            var expression = new EdmEntitySetReferenceExpression(HardCodedTestModel.GetLionSet());
            var result = EntitySetExpressionResolver.ResolveEntitySetFromExpression(expression);
            result.Should().Be(HardCodedTestModel.GetLionSet());
        }

        [TestMethod]
        public void ResolveEntitySetFromExpressionResolvesNullToNull()
        {
            var result = EntitySetExpressionResolver.ResolveEntitySetFromExpression(null);
            result.Should().BeNull();
        }
    }
}
