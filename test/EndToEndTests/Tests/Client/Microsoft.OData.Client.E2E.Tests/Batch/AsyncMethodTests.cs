//-----------------------------------------------------------------------------
// <copyright file="AsyncMethodTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Batch.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Edm.Csdl;
using System.Xml;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.Batch
{
    public class AsyncMethodTests : EndToEndTestBase<AsyncMethodTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(BanksController), typeof(MetadataController));
                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
            }
        }

        public AsyncMethodTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };
        }

        [Fact]
        public async Task JsonBatchSequencingSingeChangeSetTest()
        {
            // Create new BankAccounts object
            var bank = new Bank
            {
                Id = 45,
                Name = "Test Bank",
                Location = "KE",
                BankAccounts = new List<BankAccount>()
            };

            // Create new BankAccount object
            var bankAccount = new BankAccount
            {
                Id = 890,
                AccountNumber = "4567890",
                BankId = bank.Id,
                Bank = bank
            };

            // Establish the relationship between BankAccounts and BankAccount
            bank.BankAccounts.Add(bankAccount);

            // Add the BankAccounts entity to the context
            _context.AddObject("Banks", bank);

            // Add the related BankAccount entity
            _context.AddRelatedObject(bank, "BankAccounts", bankAccount);

            // Save both entities in a single batch request using JSON
            var response = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);
            Assert.Equal(2, response.Count());

            var bankResponse = response.First() as ChangeOperationResponse;
            var bankAccountResponse = response.Last() as ChangeOperationResponse;

            Assert.NotNull(bankResponse);
            Assert.NotNull(bankAccountResponse);

            Assert.Equal(201, bankResponse.StatusCode);
            Assert.Equal(201, bankAccountResponse.StatusCode);
        }
    }

    class Container : DataServiceContext
    {
        public Container(Uri serviceRoot) :
            base(serviceRoot, ODataProtocolVersion.V4)
        {
            Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(@"
            <edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
              <edmx:DataServices>
                <Schema Namespace=""ODataService"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                  <EntityType Name=""Bank"">
                    <Key>
                      <PropertyRef Name=""Id"" />
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
                    <Property Name=""Name"" Type=""Edm.String"" />
                    <Property Name=""Location"" Type=""Edm.String"" />
                    <NavigationProperty Name=""BankAccounts"" Type=""Collection(ODataService.BankAccount)"" />
                  </EntityType>
                  <EntityType Name=""BankAccount"">
                    <Key>
                      <PropertyRef Name=""Id"" />
                    </Key>
                    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
                    <Property Name=""AccountNumber"" Type=""Edm.String"" />
                    <NavigationProperty Name=""Bank"" Type=""ODataService.Bank"" />
                  </EntityType>
                </Schema>
                <Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                  <EntityContainer Name=""Container"">
                    <EntitySet Name=""Banks"" EntityType=""ODataService.Bank"">
                      <NavigationPropertyBinding Path=""BankAccounts"" Target=""BankAccounts"" />
                    </EntitySet>
                    <EntitySet Name=""BankAccounts"" EntityType=""ODataService.BankAccount"">
                      <NavigationPropertyBinding Path=""Bank"" Target=""Banks"" />
                    </EntitySet>
                  </EntityContainer>
                </Schema>
              </edmx:DataServices>
            </edmx:Edmx>
                ")));
            Format.UseJson();
            Banks = base.CreateQuery<Bank>("Banks");
            BankAccounts = base.CreateQuery<BankAccount>("BankAccounts");
        }

        public DataServiceQuery<Bank> Banks { get; private set; }
        public DataServiceQuery<BankAccount> BankAccounts { get; private set; }
    }
}

