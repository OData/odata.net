//---------------------------------------------------------------------
// <copyright file="SyntacticTreeVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Visitors
{
    public class SyntacticTreeVisitorTests
    {
        private class FakeVisitor : SyntacticTreeVisitor<string>
        {
        }

        [Fact]
        public void AllNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitAllToken = () => visitor.Visit(new AllToken(null, null, null));
            visitAllToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void AnyNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitAnyToken = () => visitor.Visit(new AnyToken(null, null, null));
            visitAnyToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void BinaryOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitBinaryOperatorToken = () => visitor.Visit(new BinaryOperatorToken(BinaryOperatorKind.Equal, new LiteralToken(1), new LiteralToken(1)));
            visitBinaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void CastNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitCastToken = () => visitor.Visit(new DottedIdentifierToken("stuff", null));
            visitCastToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void ExpandNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitExpandToken = () => visitor.Visit(new ExpandToken(null));
            visitExpandToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void ExpandTermNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitExpandTermToken = () => visitor.Visit(new ExpandTermToken(new NonSystemToken("stuff", null, null), 
                                                                                  null /*selectOption*/,
                                                                                  null /*expandOption*/));
            visitExpandTermToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void FunctionCallNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitFunctionCallToken = () => visitor.Visit(new FunctionCallToken("stuff", null));
            visitFunctionCallToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void LiteralNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitLiteralToken = () => visitor.Visit(new LiteralToken(1));
            visitLiteralToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void NonRootSegmentNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitNonRootSegmentToken = () => visitor.Visit(new InnerPathToken("stuff", null, null));
            visitNonRootSegmentToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void OrderByNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitOrderByToken = () => visitor.Visit(new OrderByToken(new LiteralToken(1), OrderByDirection.Ascending));
            visitOrderByToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void PropertyAccessNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitPropertyAccessToken = () => visitor.Visit(new EndPathToken("stuff", null));
            visitPropertyAccessToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void QueryOptionNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitQueryOptionToken = () => visitor.Visit(new CustomQueryOptionToken("stuff", "stuff"));
            visitQueryOptionToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void RangeVariableNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitRangeVariableToken = () => visitor.Visit(new RangeVariableToken("stuff"));
            visitRangeVariableToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void SelectNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSelectToken = () => visitor.Visit(new SelectToken(null));
            visitSelectToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void UnaryOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(new UnaryOperatorToken(UnaryOperatorKind.Negate, new LiteralToken(1)));
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void AggregateOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(new AggregateToken(new List<AggregateExpressionToken>()));
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void AggregateExpressionOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(new AggregateExpressionToken(new EndPathToken("Identifier", null), AggregationMethodDefinition.Sum, "Alias"));
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void GroupByOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(new GroupByToken(new List<EndPathToken>(), null));
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [Fact]
        public void ComputeNotImplemented()
        {
            ComputeToken token = new ComputeToken(new List<ComputeExpressionToken>());
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(token);
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
            Action acceptToken = () => token.Accept<string>(visitor);
            acceptToken.ShouldThrow<NotImplementedException>();

            ComputeVisitor computer = new ComputeVisitor();
            token.Accept<string>(computer).ShouldBeEquivalentTo(typeof(ComputeToken).ToString());
        }

        [Fact]
        public void ComputeExpressionNotImplemented()
        {
            ComputeExpressionToken token = new ComputeExpressionToken(new EndPathToken("Identifier", null), "Id");
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(token);
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
            Action acceptToken = () => token.Accept<string>(visitor);
            acceptToken.ShouldThrow<NotImplementedException>();

            ComputeVisitor computer = new ComputeVisitor();
            token.Accept<string>(computer).ShouldBeEquivalentTo(typeof(ComputeExpressionToken).ToString());
        }

        private class ComputeVisitor : SyntacticTreeVisitor<string>
        {
            public override string Visit(ComputeToken tokenIn)
            {
                return tokenIn.ToString();
            }

            public override string Visit(ComputeExpressionToken tokenIn)
            {
                return tokenIn.ToString();
            }
        }
    }
}
