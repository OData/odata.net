using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Client.Tests.Tracking;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    public class CountDistinctTests
    {
        private readonly DataServiceContext context;
        private string serviceUri = "http://example.com";

        #region TestEDMX 

        private const string Edmx =
            @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Microsoft.OData.Client.Tests.ALinq"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
     
      <EntityType Name=""Document"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
        <Property Name=""FileLength"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
    </Schema>
    <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""Documents"" EntityType="" Microsoft.OData.Client.Tests.ALinq.Document"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        #endregion

        public CountDistinctTests()
        {
            context = new DataServiceContext(new Uri(serviceUri));
            context.Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(Edmx)));
            context.Format.UseJson();
        }

        private void InterceptRequestAndMockResponse(DataServiceContext context, string aggregateAlias, object aggregateValue)
        {
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var contentTypeHeader = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";

                var mockedResponse = string.Format(
                    "{{\"@odata.context\":\"{0}/$metadata#{1}({2})\",\"value\":[{{\"@odata.id\":null,\"{2}\":{3}}}]}}",
                    serviceUri,
                    "Documents",
                    aggregateAlias,
                    aggregateValue);

                return new CustomizedRequestMessage(
                    args,
                    mockedResponse,
                    new Dictionary<string, string>()
                    {
                        {"Content-Type", contentTypeHeader},
                        {"OData-Version", "4.0"},
                    });
            };
        }

        [Fact]
        public void CountDistinctGetsGeneratedCorrectlyWithSelector()
        {
            int aggregateValue = 100;

            var queryable = context.CreateQuery<Document>("Documents");
            InterceptRequestAndMockResponse(context, "CountDistinctName", aggregateValue);
            var count = queryable.Select(x => x.Name).Distinct().Count();
            Assert.Equal(aggregateValue, count);
        }

        [Fact]
        public void DistinctFailsWithoutCorrectSelector()
        {
            int aggregateValue = 100;

            var queryable = context.CreateQuery<Document>("Documents");
            InterceptRequestAndMockResponse(context, "CountDistinctName", aggregateValue);
            Action act = () => queryable.Distinct().Count();
            Assert.Throws<NotSupportedException>(act);
        }

    }

    [Key("Id")]
    internal class Document : BaseEntityType
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int FileLength { get; set; }
    }
}
