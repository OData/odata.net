//---------------------------------------------------------------------
// <copyright file="SyntacticTreeVisitorUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    [TestClass]
    public class SyntacticTreeVisitorUnitTests
    {
        private class FakeVisitor : SyntacticTreeVisitor<string>
        {
        }

        [TestMethod]
        public void AllNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitAllToken = () => visitor.Visit(new AllToken(null, null, null));
            visitAllToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void AnyNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitAnyToken = () => visitor.Visit(new AnyToken(null, null, null));
            visitAnyToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void BinaryOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitBinaryOperatorToken = () => visitor.Visit(new BinaryOperatorToken(BinaryOperatorKind.Equal, new LiteralToken(1), new LiteralToken(1)));
            visitBinaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void CastNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitCastToken = () => visitor.Visit(new DottedIdentifierToken("stuff", null));
            visitCastToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void ExpandNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitExpandToken = () => visitor.Visit(new ExpandToken(null));
            visitExpandToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void ExpandTermNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitExpandTermToken = () => visitor.Visit(new ExpandTermToken(new NonSystemToken("stuff", null, null), 
                                                                                  null /*selectOption*/,
                                                                                  null /*expandOption*/));
            visitExpandTermToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void FunctionCallNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitFunctionCallToken = () => visitor.Visit(new FunctionCallToken("stuff", null));
            visitFunctionCallToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void LiteralNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitLiteralToken = () => visitor.Visit(new LiteralToken(1));
            visitLiteralToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void NonRootSegmentNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitNonRootSegmentToken = () => visitor.Visit(new InnerPathToken("stuff", null, null));
            visitNonRootSegmentToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void OrderByNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitOrderByToken = () => visitor.Visit(new OrderByToken(new LiteralToken(1), OrderByDirection.Ascending));
            visitOrderByToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void PropertyAccessNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitPropertyAccessToken = () => visitor.Visit(new EndPathToken("stuff", null));
            visitPropertyAccessToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void QueryOptionNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitQueryOptionToken = () => visitor.Visit(new CustomQueryOptionToken("stuff", "stuff"));
            visitQueryOptionToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void RangeVariableNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitRangeVariableToken = () => visitor.Visit(new RangeVariableToken("stuff"));
            visitRangeVariableToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void SelectNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitSelectToken = () => visitor.Visit(new SelectToken(null));
            visitSelectToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void UnaryOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(new UnaryOperatorToken(UnaryOperatorKind.Negate, new LiteralToken(1)));
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void AggregateOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(new AggregateToken(new List<AggregateStatementToken>()));
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void AggregateStatementOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(new AggregateStatementToken(new EndPathToken("Identifier", null), AggregationVerb.Sum, "Alias"));
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void GroupByOperatorNotImplemented()
        {
            FakeVisitor visitor = new FakeVisitor();
            Action visitUnaryOperatorToken = () => visitor.Visit(new GroupByToken(new List<EndPathToken>(), null));
            visitUnaryOperatorToken.ShouldThrow<NotImplementedException>();
        }
    }
}
