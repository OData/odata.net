//---------------------------------------------------------------------
// <copyright file="UriParserMicrobenchmarks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance.Microbenchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;

    /// <summary>
    /// Targets the URI parser tokenization hot path indirectly via the
    /// public <see cref="ODataUriParser"/>.  The internal lexer/parser
    /// allocations are exercised by ParseFilter-heavy queries.
    /// See REPORT.md findings #11, #12, #13, #14, #15, #16.
    /// </summary>
    [MemoryDiagnoser]
    public class UriParserMicrobenchmarks
    {
        private static readonly IEdmModel Model = TestUtils.GetAdventureWorksModel();
        private static readonly Uri ServiceRoot = new Uri("http://odata.org/Perf.svc/");

        // Heavy parameter aliases & numeric mix; exercises ExpressionLexer paths.
        private const string AliasHeavy = "Product?$filter=Name eq @p1 and Price lt @p2 and Cost gt @p3 and Foo eq @bar";
        private const string NumericHeavy = "Product?$filter=Price eq 1.5 and Cost lt 100 and Inf eq -INF and Other gt 1.234e10";
        private const string DeepExpand = "Product?$expand=ProductInventory($expand=Location($expand=WorkOrderRouting($expand=WorkOrder($expand=Product))))";

        [Benchmark]
        public object Filter_Aliases()
        {
            return new ODataUriParser(Model, ServiceRoot, new Uri(ServiceRoot, AliasHeavy)).ParseFilter();
        }

        [Benchmark]
        public object Filter_Numerics()
        {
            return new ODataUriParser(Model, ServiceRoot, new Uri(ServiceRoot, NumericHeavy)).ParseFilter();
        }

        [Benchmark]
        public object Expand_Deep()
        {
            return new ODataUriParser(Model, ServiceRoot, new Uri(ServiceRoot, DeepExpand)).ParseSelectAndExpand();
        }
    }
}
