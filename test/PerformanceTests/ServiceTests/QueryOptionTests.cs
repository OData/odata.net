//---------------------------------------------------------------------
// <copyright file="QueryOptionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using global::Xunit;
    using Microsoft.OData.Core;
    using Microsoft.Xunit.Performance;

    public class QueryOptionTests : IClassFixture<TestServiceFixture<QueryOptionTests>>
    {
        TestServiceFixture<QueryOptionTests> serviceFixture;

        public QueryOptionTests(TestServiceFixture<QueryOptionTests> serviceFixture)
        {
            this.serviceFixture = serviceFixture;
        }

        [Benchmark]
        public void QueryOptionsWithoutExpand_20()
        {
            int RequestsPerIteration = 20;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < RequestsPerIteration; i++)
                    {
                        QueryAndVerify("CompanySet?$filter=Revenue gt 500&$select=Name&$orderby=Revenue", "odata.maxpagesize=100");
                    }
                }
            }
        }

        [Benchmark]
        public void ExpandNavigationProperty()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    QueryAndVerify("CompanySet?$expand=Employees", "odata.maxpagesize=100");
                }
            }
        }

        [Benchmark]
        public void NestedQueryOptionsWithExpand()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    QueryAndVerify("CompanySet?$orderby=Name&$expand=Employees($filter=Age gt 40;$select=FirstName,Age;$orderby=Age)", "odata.maxpagesize=1000");
                }
            }
        }
        private void QueryAndVerify(string query, string preferHeader)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = serviceFixture.ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(serviceFixture.ServiceBaseUri.AbsoluteUri + query, UriKind.Absolute));
            if (!String.IsNullOrEmpty(preferHeader))
            {
                requestMessage.SetHeader("Prefer", preferHeader);
            }
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);
        }
    }
}
