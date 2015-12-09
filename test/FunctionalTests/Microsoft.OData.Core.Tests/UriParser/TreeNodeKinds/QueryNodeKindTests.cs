//---------------------------------------------------------------------
// <copyright file="QueryNodeKindTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.TreeNodeKinds
{
    public class QueryNodeKindTests
    {
        [Fact]
        public void QueryNodeKindMustBeASubsetOfInternalQueryNodeKind()
        {
            //make sure there aren't any query node kinds that aren't also internal query node kinds
            var queryNodeKindFields = typeof(QueryNodeKind).GetFields();
            var internalQueryNodeKindFields = typeof(InternalQueryNodeKind).GetFields();

            var queryNodesThatAreInternalQueryNodes =
                queryNodeKindFields.Select(f => internalQueryNodeKindFields.Contains(f));

            queryNodesThatAreInternalQueryNodes.Should().HaveCount(queryNodeKindFields.Count());
        }

        [Fact]
        public void InternalQueryNodeKindMustBeCastableToQueryNodeKind()
        {
            //make sure the field names are the exact same order.
            var queryNodeKindFields = typeof(QueryNodeKind).GetFields();
            var internalQueryNodeKindFields = typeof(InternalQueryNodeKind).GetFields();

            int i = 0;
            foreach (var queryNodeKind in queryNodeKindFields)
            {
                internalQueryNodeKindFields[i].Name.Should().Be(queryNodeKind.Name);
                i++;
            }
        }
    }
}
