//---------------------------------------------------------------------
// <copyright file="UriParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using global::Xunit;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Xunit.Performance;

    public class UriParserTests
    {
        private static readonly IEdmModel Model = TestUtils.GetAdventureWorksModel();
        private static readonly Uri ServiceRoot = new Uri(@"http://odata.org/Perf.svc/");

        [Benchmark]
        public void ParseUri()
        {
            string query = "Employee(1)/PurchaseOrderHeader/Vendor/ProductVendor/Product?" +
                           "$filter=totaloffsetminutes(DiscontinuedDate) gt 10 and LuckeyNumbers/any(a: a lt 100) and contains(Name, 'aaaa')" +
                           "&$orderby=StandardCost,Size desc,Weight asc" +
                           "&$top=10" +
                           "&$skip=10" +
                           "&$select=BillOfMaterials,ProductModel,ProductSubcategory,ProductCostHistory,ProductListPriceHistory" +
                           "&$expand=ProductInventory,WorkOrder($expand=Product($select=BillOfMaterials,ProductModel;$orderby=StandardCost;$top=4;$skip=3;$search=NOT clothing))" +
                           "&$search=(mountain OR bike) AND NOT clothing" +
                           "&$count=true";

            int roundPerIteration = 1000;

            TestExecution(query, roundPerIteration, parser => parser.ParseUri());
        }

        [Benchmark]
        public void ParsePath()
        {
            string query = "Employee(1)/PurchaseOrderHeader/Vendor/ProductVendor/Product/ProductInventory(ProductID = 1,LocationID = 1)/Location/WorkOrderRouting/WorkOrder/Product/BillOfMaterials(1)/UnitMeasure/ModifiedDate";
            
            int roundPerIteration = 5000;

            TestExecution(query, roundPerIteration, parser => parser.ParsePath());
        }

        [Benchmark]
        public void ParseFilter()
        {
            string query = "Product?$filter=contains(Name, 'aaaa') and startswith(ProductNumber, '000') " +
                           "and MakeFlag and concat(Color, SizeUnitMeasureCode) gt 'lll' and fractionalseconds(SellStartDate) eq 0 " +
                           "and round(StandardCost) mod ListPrice lt 10 and TimeZones/any(a: a/Offset eq maxdatetime())";
            
            int roundPerIteration = 5000;

            TestExecution(query, roundPerIteration, parser => parser.ParseFilter());
        }

        [Benchmark]
        public void ParseOrderBy()
        {
            string query = "Product?$orderby=Name,ProductNumber,MakeFlag,Color,SizeUnitMeasureCode desc,SellStartDate desc,ListPrice,StandardCost asc,ModifiedDate asc";

            int roundPerIteration = 5000;

            TestExecution(query, roundPerIteration, parser => parser.ParseOrderBy());
        }

        [Benchmark]
        public void ParseSelectAndExpand()
        {
            string query = "Product?$select=BillOfMaterials,ProductModel,ProductSubcategory,ProductCostHistory,ProductListPriceHistory" +
                           "&$expand=ProductInventory,WorkOrder($expand=Product($select=BillOfMaterials,ProductModel;$orderby=StandardCost;$top=4;$skip=3;$search=NOT clothing))";

            int roundPerIteration = 5000;

            TestExecution(query, roundPerIteration, parser => parser.ParseSelectAndExpand());
        }

        private void TestExecution(string query, int roundPerIteration, Action<ODataUriParser> parseAction)
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < roundPerIteration; i++)
                    {
                        ODataUriParser parser = new ODataUriParser(Model, ServiceRoot, new Uri(ServiceRoot, query));
                        parseAction(parser);
                    }
                }
            }
        }
    }
}
