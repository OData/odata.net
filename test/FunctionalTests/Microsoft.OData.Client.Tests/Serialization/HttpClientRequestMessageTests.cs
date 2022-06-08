using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Client.Tests.Serialization
{
    public class HttpClientRequestMessageTests
    {
        [Fact]
        public void UnwrapAggregateException()
        {
            var msg = new HttpClientRequestMessage(new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>()));
            var task = new Task<HttpResponseMessage>(() => throw new Exception("single exception"));
            task.RunSynchronously();

            var exception = Assert.Throws<DataServiceTransportException>(() => msg.EndGetResponse(task));
            Assert.StartsWith("System.Exception: single exception\r\n", exception.Message);
        }

#if NETCOREAPP2_0_OR_GREATER
        [Fact]
        public void SetCertificateValidationCallback()
        {
            var httpClientRequestMessage = new HttpClientRequestMessage(
                new DataServiceClientRequestMessageArgs("GET", new Uri("http://localhost"), false, false, new Dictionary<string, string>()));

            httpClientRequestMessage.ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true;
            Assert.True(httpClientRequestMessage.ServerCertificateCustomValidationCallback(null, null, null, SslPolicyErrors.None));

            httpClientRequestMessage.ServerCertificateCustomValidationCallback = (_, __, ___, ____) => false;
            Assert.False(httpClientRequestMessage.ServerCertificateCustomValidationCallback(null, null, null, SslPolicyErrors.None));

            httpClientRequestMessage.ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true;
            Assert.True(httpClientRequestMessage.ServerCertificateCustomValidationCallback(null, null, null, SslPolicyErrors.None));
            httpClientRequestMessage.ServerCertificateCustomValidationCallback += (_, __, ___, ____) => false;
            Assert.False(httpClientRequestMessage.ServerCertificateCustomValidationCallback(null, null, null, SslPolicyErrors.None));
        }
#endif
    }
}
