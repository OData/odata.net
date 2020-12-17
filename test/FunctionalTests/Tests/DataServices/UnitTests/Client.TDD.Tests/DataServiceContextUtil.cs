using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Client.TDDUnitTests.Tests;

namespace Microsoft.OData.Client.TDDUnitTests
{
    public static class DataServiceContextUtil
    {
        private const string ValidEdmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        public static DataServiceContext ReConfigureForNetworkLoadingTests(this DataServiceContext context)
        {
            context.Format.InjectMetadataHttpNetworkRequest = InjectFakeEdmxRequest;
            return context;
        }
        internal static HttpClientRequestMessage InjectFakeEdmxRequest()
        {
            return new CustomizedHttpWebRequestMessage(
                new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri("http://temp.org/"),
                    false,
                    false,
                    new Dictionary<string, string>()),
                ValidEdmx,
                new Dictionary<string, string>());
        }
    }
 
}
