//---------------------------------------------------------------------
// <copyright file="QueryOptionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.IO;
    using global::Xunit;
    using Microsoft.OData;
    using BenchmarkDotNet.Attributes;

    /// <summary>
    /// Performance tests making queries with different query options
    /// to am OData service.
    /// </summary>
    [MemoryDiagnoser]
    public class QueryOptionTests : IClassFixture<TestServiceFixture<QueryOptionTests>>
    {
        TestServiceFixture<QueryOptionTests> serviceFixture;

        [GlobalSetup]
        public void SetupService()
        {
            serviceFixture = new TestServiceFixture<QueryOptionTests>();
        }

        [GlobalCleanup]
        public void KillService()
        {
            serviceFixture.Dispose();
        }

        [Benchmark]
        public void QueryOptionsWithoutExpand()
        {
            int RequestsPerIteration = 20;

            for (int i = 0; i < RequestsPerIteration; i++)
            {
                QueryAndVerify("CompanySet?$filter=Revenue gt 500&$select=Name&$orderby=Revenue", "odata.maxpagesize=100");
            }
        }

        [Benchmark]
        public void QueryOptionsWithExpand()
        {
            QueryAndVerify("CompanySet?$filter=Revenue gt 500&$select=Name&$orderby=Revenue&$expand=Employees", "odata.maxpagesize=100");
        }

        [Benchmark]
        public void NestedQueryOptionsWithExpand()
        {
            QueryAndVerify("CompanySet?$orderby=Name&$expand=Employees($filter=Age gt 40;$select=FirstName,Age;$orderby=Age)", "odata.maxpagesize=1000");
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
