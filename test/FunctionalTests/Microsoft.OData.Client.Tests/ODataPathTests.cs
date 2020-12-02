using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Client.Tests.Tracking;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class ODataPathTests
    {

        private static string EDMX = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""odata.tests.nested"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Level1"">
        <Key>
          <PropertyRef Name=""id"" />
        </Key>
        <Property Name=""id"" Type=""Edm.String"" Nullable=""false""></Property>
        <NavigationProperty Name=""Level2Prop"" Type=""Collection(odata.tests.nested.Level2)"" ContainsTarget=""true""></NavigationProperty>
      </EntityType>
      <EntityType Name=""Level2"">
        <Key>
          <PropertyRef Name=""id"" />
        </Key>
        <Property Name=""id"" Type=""Edm.String"" Nullable=""false""></Property>
        <NavigationProperty Name=""Level3Prop"" Type=""Collection(odata.tests.nested.Level3)"" ContainsTarget=""true""></NavigationProperty>
      </EntityType>
      <EntityType Name=""Level3"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.String"" Nullable=""false""></Property>
      </EntityType>
      <EntityContainer Name=""Default"">
        <EntitySet Name=""Level1"" EntityType=""odata.tests.nested.Level1""></EntitySet>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";


        private const string RESPONSE = @"{""@odata.context"":""https://localhost:44395/api/v1/$metadata#Level1('3C3C802D-2D04-49D9-8E33-C09A64530A9C')/Level2Prop('E63F816D-B7E7-4FC9-AE62-7E709AFF290E')/Level3Prop"",
    ""value"":
    [
    
        {
            ""Id"":""8679ff75-bbdf-4470-bb36-212e9199e9f8""
        }
    ]
}";

        private const string EXPECTED_URL = "https://localhost:44395/api/v1/Level1('3C3C802D-2D04-49D9-8E33-C09A64530A9C')/Level2Prop('E63F816D-B7E7-4FC9-AE62-7E709AFF290E')/Level3Prop('8679ff75-bbdf-4470-bb36-212e9199e9f8')";
        [Fact]
        public async Task Test_NavigationBidingUrlIsAsExpected()
        {
            var ctx = new DataServiceContext(new Uri("http://localhost"));
            ctx.Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(EDMX)));
            ctx.Format.UseJson();

            ctx.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated((settingsArgs) =>
            {
                settingsArgs.Settings.Validations = ValidationKinds.None;
            });

            ctx.Configurations.RequestPipeline.OnMessageCreating = args => new CustomizedRequestMessage(
                args,
                RESPONSE,
                new Dictionary<string, string>()
                {
                    {"Content-Type", "application/json;charset=utf-8"},
                });

            var query = ctx.CreateQuery<Level3>("Level3");

            var values = await query.ExecuteAsync();
            values = values.ToList();

            var first = values.FirstOrDefault();
            Assert.NotNull(first?.Id);
            Assert.NotNull(ctx.Entities);

            var firstEntityDescriptor = ctx.Entities.FirstOrDefault();
            Assert.Equal(EXPECTED_URL,firstEntityDescriptor?.Identity?.AbsoluteUri);

        }
    }



    [Microsoft.OData.Client.Key("Id")]
    public class Level3
    {
        public string Id { get; set; }
    }
}
