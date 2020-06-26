//---------------------------------------------------------------------
// <copyright file="SelectExpandPathToStringVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client.ALinq.UriParser;
using Microsoft.OData;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Tests ClientEdmModel functionalities
    /// </summary>
    public class SelectExpandPathToStringVisitorTests
    {
        [Fact]
        public void SystemTokenThrows()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            SystemToken systemToken = new SystemToken(ExpressionConstants.It, null);
            Action visitSystemToken = () => systemToken.Accept(visitor);
            NotSupportedException exception = Assert.Throws<NotSupportedException>(visitSystemToken);
            Assert.Equal(Strings.ALinq_IllegalSystemQueryOption(ExpressionConstants.It), exception.Message);
        }

        [Fact]
        public void ExpandOnlyPathResultsInExpandOnlyTree()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            NonSystemToken path = new NonSystemToken("NavProp", null, new NonSystemToken("NavProp1", null, null));
            path.IsStructuralProperty = false;
            path.NextToken.IsStructuralProperty = false;
            Assert.Equal("NavProp($expand=NavProp1)", path.Accept(visitor));
        }

        [Fact]
        public void SingleExpandResultsInSingleExpandTree()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            NonSystemToken path = new NonSystemToken("NavProp", null, null);
            path.IsStructuralProperty = false;
            Assert.Equal("NavProp", path.Accept(visitor));
        }

        [Fact]
        public void SelectAtTheEndOfPathResultsInSelectQueryOption()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            NonSystemToken path = new NonSystemToken("NavProp", null, new NonSystemToken("StructuralProp", null, null));
            path.IsStructuralProperty = false;
            path.NextToken.IsStructuralProperty = true;
            Assert.Equal("NavProp($select=StructuralProp)", path.Accept(visitor));
        }
    }
}
