//---------------------------------------------------------------------
// <copyright file="SimpleQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using global::Xunit;
    using Microsoft.OData;
    using Microsoft.Xunit.Performance;

    public class SimpleQueryTests : IClassFixture<TestServiceFixture<SimpleQueryTests>>
    {
        TestServiceFixture<SimpleQueryTests> serviceFixture;

        public SimpleQueryTests(TestServiceFixture<SimpleQueryTests> serviceFixture)
        {
            this.serviceFixture = serviceFixture;
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void QuerySimpleEntitySet()
        {
            int RequestsPerIteration = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < RequestsPerIteration; i++)
                    {
                        QueryAndVerify("SimplePeopleSet", "odata.maxpagesize=10");                        
                    }
                }
            }
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void QueryLargeEntitySet()
        {
            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    QueryAndVerify("LargePeopleSet", "odata.maxpagesize=1000");
                }
            }
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void QuerySingleEntity()
        {
            int RequestsPerIteration = 100;

            foreach (var iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < RequestsPerIteration; i++)
                    {
                        QueryAndVerify("SimplePeopleSet(1)", null);
                    }
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
