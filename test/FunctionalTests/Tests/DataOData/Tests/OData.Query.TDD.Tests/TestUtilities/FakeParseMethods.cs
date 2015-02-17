//---------------------------------------------------------------------
// <copyright file="FakeParseMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.TestUtilities
{
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Class that contains fake parse methods to help unit syntactic tests from growing in scope.
    /// </summary>
    internal class FakeParseMethods
    {
        private ExpressionLexer lexer;

        public FakeParseMethods(ExpressionLexer lexer)
        {
            this.lexer = lexer;
        }
        public QueryToken ParseMethodReturningALiteral()
        {
            this.lexer.NextToken();
            return new LiteralToken("stuff");
        }
    }
}
