//---------------------------------------------------------------------
// <copyright file="SimpleQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using Microsoft.OData;
    using BenchmarkDotNet.Attributes;
    using global::Xunit;

    /// <summary>
    /// Performance tests on making queries to a service.
    /// These tests make simple queries (without query options) to the service
    /// and verify the status code.
    /// </summary>
    [MemoryDiagnoser]
    public class SimpleQueryTests : IClassFixture<TestServiceFixture<SimpleQueryTests>>
    {
        TestServiceFixture<SimpleQueryTests> serviceFixture;

        [GlobalSetup]
        public void LaunchService()
        {
            serviceFixture = new TestServiceFixture<SimpleQueryTests>();
        }

        [GlobalCleanup]
        public void KillService()
        {
            serviceFixture.Dispose();
        }

        [Benchmark]
        public void QuerySimpleEntitySet()
        {
            int RequestsPerIteration = 100;

            for (int i = 0; i < RequestsPerIteration; i++)
            {
                QueryAndVerify("SimplePeopleSet", "odata.maxpagesize=10");
            }
        }

        [Benchmark]
        public void QueryLargeEntitySet()
        {
            QueryAndVerify("LargePeopleSet", "odata.maxpagesize=1000");
        }

        [Benchmark]
        public void QuerySingleEntity()
        {
            int RequestsPerIteration = 100;

            for (int i = 0; i < RequestsPerIteration; i++)
            {
                QueryAndVerify("SimplePeopleSet(1)", null);
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
