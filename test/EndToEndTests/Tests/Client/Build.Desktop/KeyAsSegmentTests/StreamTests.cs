//---------------------------------------------------------------------
// <copyright file="StreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices.KeyAsSegmentServiceReference;
    using Xunit.Abstractions;
    using Xunit;

    public class StreamTests : KeyAsSegmentTest
    {
        public StreamTests(ITestOutputHelper helper)
            :base(helper)
        {
        }
        private static readonly byte[] binaryTestData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

        [Fact]
        public void GetReadStreamFromMle()
        {
            var contextWrapper = this.CreateWrappedContext();
            var customerInfoQuery = contextWrapper.CreateQuery<CustomerInfo>("CustomerInfo");

            foreach (var customerInfo in customerInfoQuery)
            {
                var streamResponse = contextWrapper.GetReadStream(customerInfo);
                Assert.NotNull(streamResponse);
                VerifyStreamReadable(streamResponse.Stream);
            }
        }

        [Fact]
        public void GetReadStreamUriFromMle()
        {
            var contextWrapper = this.CreateWrappedContext();
            var customerInfoQuery = contextWrapper.CreateQuery<CustomerInfo>("CustomerInfo");

            foreach (var customerInfo in customerInfoQuery)
            {
                var streamResponseUri = contextWrapper.GetReadStreamUri(customerInfo);
                Assert.NotNull(streamResponseUri);
                VerifyUriDoesNotContainParentheses(streamResponseUri);
            }
        }

        [Fact]
        public void GetReadStreamFromNamedStreamProperty()
        {
            var contextWrapper = this.CreateWrappedContext();
            var firstCar = contextWrapper.CreateQuery<Car>("Car").Take(1).Single();

            var streamResponse = contextWrapper.GetReadStream(firstCar, "Photo", new DataServiceRequestArgs());
            VerifyStreamReadable(streamResponse.Stream);
        }

        [Fact]
        public void SetSaveStreamOnMle()
        {
            var contextWrapper = this.CreateWrappedContext();
            var customerInfo = contextWrapper.Context.CustomerInfo.Take(1).Single();

            var memoryStream = new MemoryStream(binaryTestData);

            contextWrapper.SetSaveStream(customerInfo, memoryStream, true, "application/binary", "var1");
            contextWrapper.SaveChanges();
        }

        [Fact]
        public void SetSaveStreamOnNamedStreamProperty()
        {
            var contextWrapper = this.CreateWrappedContext();
            var firstCar = contextWrapper.CreateQuery<Car>("Car").Take(1).Single();
            
            var memoryStream = new MemoryStream(binaryTestData);
            contextWrapper.SetSaveStream(firstCar, "Video", memoryStream, true, new DataServiceRequestArgs { ContentType = "application/binary" });
            contextWrapper.SaveChanges();
        }

        private static void VerifyStreamReadable(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                reader.ReadToEnd(); 
            }
        }
    }
}
