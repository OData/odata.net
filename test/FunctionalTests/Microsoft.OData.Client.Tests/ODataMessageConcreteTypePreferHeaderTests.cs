using System;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class ODataMessageConcreteTypePreferHeaderTests
    {
        private const string MaxPageSizePreference = "odata.maxpagesize";
        private const string PreferHeaderName = "Prefer";
        private const string MimeType = "application/json";

        /// <summary>
        /// Follows the same logic used to create a request in the DataServiceContext.
        /// </summary>
        [Fact]
        public void HeaderShouldBeCaseInsensitive()
        {
            HeaderCollection headers = new HeaderCollection();
            headers.SetHeader("Accept", MimeType);
            headers.SetHeader("Content-Type", MimeType);

            BuildingRequestEventArgs requestEventArgs = new BuildingRequestEventArgs("GET", new Uri("http://localhost"), headers, null, HttpStack.Auto);
            DataServiceClientRequestMessageArgs args = new DataServiceClientRequestMessageArgs(
                requestEventArgs.Method,
                requestEventArgs.RequestUri,
                false,
                false,
                requestEventArgs.Headers);

            HttpClientRequestMessage request = new HttpClientRequestMessage(args);
            const int maxPageSize = 10;
            ODataPreferenceHeader preferHeader = new ODataPreferenceHeader(request);
            preferHeader.MaxPageSize = maxPageSize;

            Assert.Equal(maxPageSize, preferHeader.MaxPageSize);
            string expected = $"{MaxPageSizePreference}={maxPageSize}";

            Assert.Equal(expected, request.GetHeader("pReFer"));
            Assert.Equal(MimeType, request.GetHeader("accepT"));
        }
    }
}
