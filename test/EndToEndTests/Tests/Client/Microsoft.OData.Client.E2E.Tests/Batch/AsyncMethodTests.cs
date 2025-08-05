//-----------------------------------------------------------------------------
// <copyright file="AsyncMethodTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Xml;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.Batch.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Microsoft.OData.Edm.Csdl;
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
                services.ConfigureControllers(typeof(BanksController), typeof(BankAccountsController), typeof(MetadataController));
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
        public async Task JsonBatchSequencingSingleChangeSetTest()
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

        [Fact]
        // Validating regression reported here: https://github.com/OData/odata.net/issues/3150
        public async Task BatchSequencingSingleChangeSetWithRelatedAlone()
        {
            // Create new Bank object
            var bank = new Bank
            {
                Id = 45,
                Name = "Test Bank",
                Location = "KE",
                BankAccounts = new List<BankAccount>()
            };

            // Add the Bank entity to the context
            _context.AddObject("Banks", bank);

            // Save bank
            var response = await _context.SaveChangesAsync();
            Assert.Single(response);

            var bankResponse = response.First() as ChangeOperationResponse;
            Assert.NotNull(bankResponse);
            Assert.Equal(201, bankResponse.StatusCode);

            // Create new BankAccount object
            var bankAccount = new BankAccount
            {
                Id = 890,
                AccountNumber = "4567890",
                BankId = bank.Id,
                Bank = bank
            };

            // Establish the relationship between Bank and BankAccount
            bank.BankAccounts.Add(bankAccount);

            // Add the related BankAccount entity
            _context.AddRelatedObject(bank, "BankAccounts", bankAccount);

            // Save bankAccount in a single batch request. 
            response = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);
            Assert.Single(response);

            var bankAccountResponse = response.Last() as ChangeOperationResponse;
            Assert.NotNull(bankAccountResponse);
            Assert.Equal(201, bankAccountResponse.StatusCode);
        }

        [Fact]
        public async Task JsonBatchSequencingSingeChangeSetTest_SetLink()
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
            };

            // Add the Bank and Account entities to the context
            _context.AddObject("Banks", bank);
            _context.AddObject("BankAccounts", bankAccount);

            // Set the link from BankAccount to Bank entity
            _context.SetLink(bankAccount, "Bank", bank);

            // Save both entities in a single batch request using JSON
            var response = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);
            Assert.Equal(3, response.Count()); // We get 2 POST's and a PUT for the link

            var bankResponse = response.ElementAt(0) as ChangeOperationResponse;
            var bankAccountResponse = response.ElementAt(1) as ChangeOperationResponse;
            var bankAccountBankResponse = response.ElementAt(2) as ChangeOperationResponse;

            Assert.NotNull(bankResponse);
            Assert.NotNull(bankAccountResponse);
            Assert.NotNull(bankAccountBankResponse);

            Assert.Equal(201, bankResponse.StatusCode);
            Assert.Equal(201, bankAccountResponse.StatusCode);
            Assert.Equal(204, bankAccountBankResponse.StatusCode);
        }

        [Theory]
        [InlineData(SaveChangesOptions.BatchWithSingleChangeset, 3001, 10001)]
        [InlineData(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch, 30001, 100001)]
        public async Task JsonBatchWithSingleChangesetWithAddLink(SaveChangesOptions saveChangesOptions, int bankId, int bankAccountId)
        {
            var bank = new Bank
            {
                Id = bankId,
                Name = $"Bank {bankId}",
                Location = "Loc 1"
            };
            var bankAccount = new BankAccount
            {
                Id = bankAccountId,
                AccountNumber = $"{bankAccountId}",
                BankId = bank.Id,
            };

            _context.AddObject("Banks", bank);
            _context.AddObject("BankAccounts", bankAccount);
            _context.AddLink(bank, "BankAccounts", bankAccount);

            var dataServiceResponse = await _context.SaveChangesAsync(saveChangesOptions);

            Assert.Equal(3, dataServiceResponse.Count()); // We get 2 POST's and a PUT for the link

            var operationResponseAt0 = dataServiceResponse.ElementAt(0) as ChangeOperationResponse;
            var operationResponseAt1 = dataServiceResponse.ElementAt(1) as ChangeOperationResponse;
            var operationResponseAt2 = dataServiceResponse.ElementAt(2) as ChangeOperationResponse;

            Assert.NotNull(operationResponseAt0);
            Assert.NotNull(operationResponseAt1);
            Assert.NotNull(operationResponseAt2);

            Assert.Equal(201, operationResponseAt0.StatusCode);
            Assert.Equal(201, operationResponseAt1.StatusCode);
            Assert.Equal(204, operationResponseAt2.StatusCode);
        }

        [Theory]
        [InlineData(SaveChangesOptions.BatchWithSingleChangeset, 3002, 10002)]
        [InlineData(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch, 30002, 100002)]
        public async Task JsonBatchWithSingleChangesetWithSetLink(SaveChangesOptions saveChangesOptions, int bankId, int bankAccountId)
        {
            var bank = new Bank
            {
                Id = bankId,
                Name = $"Bank {bankId}",
                Location = "Loc 1"
            };
            var bankAccount = new BankAccount
            {
                Id = bankAccountId,
                AccountNumber = $"{bankAccountId}",
                BankId = bank.Id,
            };

            _context.AddObject("Banks", bank);
            _context.AddObject("BankAccounts", bankAccount);
            _context.SetLink(bankAccount, "Bank", bank);

            var dataServiceResponse = await _context.SaveChangesAsync(saveChangesOptions);

            Assert.Equal(3, dataServiceResponse.Count()); // We get 2 POST's and a PUT for the link

            var operationResponseAt0 = dataServiceResponse.ElementAt(0) as ChangeOperationResponse;
            var operationResponseAt1 = dataServiceResponse.ElementAt(1) as ChangeOperationResponse;
            var operationResponseAt2 = dataServiceResponse.ElementAt(2) as ChangeOperationResponse;

            Assert.NotNull(operationResponseAt0);
            Assert.NotNull(operationResponseAt1);
            Assert.NotNull(operationResponseAt2);

            Assert.Equal(201, operationResponseAt0.StatusCode);
            Assert.Equal(201, operationResponseAt1.StatusCode);
            Assert.Equal(204, operationResponseAt2.StatusCode);
        }

        [Theory]
        [InlineData(SaveChangesOptions.BatchWithSingleChangeset, 3003, 10003)]
        [InlineData(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch, 30003, 100003)]
        public async Task JsonBatchWithSingleChangesetWithAddRelatedObject(SaveChangesOptions saveChangesOptions, int bankId, int bankAccountId)
        {
            var bank = new Bank
            {
                Id = bankId,
                Name = $"Bank {bankId}",
                Location = "Loc 1"
            };
            var bankAccount = new BankAccount
            {
                Id = bankAccountId,
                AccountNumber = $"{bankAccountId}",
                BankId = bank.Id,
            };

            _context.AddObject("Banks", bank);
            _context.AddRelatedObject(bank, "BankAccounts", bankAccount);

            var dataServiceResponse = await _context.SaveChangesAsync(saveChangesOptions);

            Assert.Equal(2, dataServiceResponse.Count()); // We get 2 POST's and a PUT for the link

            var operationResponseAt0 = dataServiceResponse.ElementAt(0) as ChangeOperationResponse;
            var operationResponseAt1 = dataServiceResponse.ElementAt(1) as ChangeOperationResponse;

            Assert.NotNull(operationResponseAt0);
            Assert.NotNull(operationResponseAt1);

            Assert.Equal(201, operationResponseAt0.StatusCode);
            Assert.Equal(201, operationResponseAt1.StatusCode);
        }

        [Theory]
        [InlineData(SaveChangesOptions.BatchWithSingleChangeset, 3004, 10004)]
        [InlineData(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch, 30004, 100004)]
        public async Task JsonBatchWithSingleChangesetWithSetRelatedObject(SaveChangesOptions saveChangesOptions, int bankId, int bankAccountId)
        {
            var bank = new Bank
            {
                Id = bankId,
                Name = $"Bank {bankId}",
                Location = "Loc 1"
            };
            var bankAccount = new BankAccount
            {
                Id = bankAccountId,
                AccountNumber = $"{bankAccountId}",
                BankId = bank.Id,
            };

            _context.AddObject("BankAccounts", bankAccount);
            _context.SetRelatedObject(bankAccount, "Bank", bank);

            var dataServiceResponse = await _context.SaveChangesAsync(saveChangesOptions);

            Assert.Equal(2, dataServiceResponse.Count());

            var operationResponseAt0 = dataServiceResponse.ElementAt(0) as ChangeOperationResponse;
            var operationResponseAt1 = dataServiceResponse.ElementAt(1) as ChangeOperationResponse;

            Assert.NotNull(operationResponseAt0);
            Assert.NotNull(operationResponseAt1);

            Assert.Equal(201, operationResponseAt0.StatusCode);
            Assert.Equal(201, operationResponseAt1.StatusCode);
        }

        [Fact]
        public async Task JsonBatchWithSingleChangesetWithDeleteLink()
        {
            var bank = _context.Banks.SingleOrDefault(d => d.Id == 302);
            var bankAccount = _context.BankAccounts.SingleOrDefault(d => d.Id == 1003);
            
            _context.DeleteLink(bank, "BankAccounts", bankAccount);
            
            var dataServiceResponse = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);
            
            var operationResponse = Assert.Single(dataServiceResponse) as ChangeOperationResponse;
            
            Assert.NotNull(operationResponse);
            Assert.Equal(204, operationResponse.StatusCode);
        }

        [Fact]
        public async Task JsonBatchWithSingleChangesetWithAddLinkToExisting()
        {
            var bank = _context.Banks.SingleOrDefault(d => d.Id == 302);
            var bankAccount = new BankAccount
            {
                Id = 10005,
                AccountNumber = "10005",
                BankId = bank.Id,
            };
            
            _context.AddObject("BankAccounts", bankAccount);
            _context.AddLink(bank, "BankAccounts", bankAccount);
            
            var dataServiceResponse = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);
            
            Assert.Equal(2, dataServiceResponse.Count());

            var operationResponseAt0 = dataServiceResponse.ElementAt(0) as ChangeOperationResponse;
            var operationResponseAt1 = dataServiceResponse.ElementAt(1) as ChangeOperationResponse;

            Assert.NotNull(operationResponseAt0);
            Assert.NotNull(operationResponseAt1);

            Assert.Equal(201, operationResponseAt0.StatusCode);
            Assert.Equal(204, operationResponseAt1.StatusCode);
        }

        [Fact]
        public async Task JsonBatchWithSingleChangesetWithSetLinkToExisting()
        {
            var bank = new Bank
            {
                Id = 3005,
                Name = "Bank 3005",
                Location = "Loc 1"
            };
            var bankAccount = _context.BankAccounts.SingleOrDefault(d => d.Id == 1005);

            _context.AddObject("Banks", bank);
            _context.SetLink(bankAccount, "Bank", bank);

            var dataServiceResponse = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);

            Assert.Equal(2, dataServiceResponse.Count());

            var operationResponseAt0 = dataServiceResponse.ElementAt(0) as ChangeOperationResponse;
            var operationResponseAt1 = dataServiceResponse.ElementAt(1) as ChangeOperationResponse;

            Assert.NotNull(operationResponseAt0);
            Assert.NotNull(operationResponseAt1);

            Assert.Equal(201, operationResponseAt0.StatusCode);
            Assert.Equal(204, operationResponseAt1.StatusCode);
        }

        [Fact]
        public async Task JsonBatchWithSingleChangesetWithSetLinkToNull()
        {
            var bankAccount = _context.BankAccounts.SingleOrDefault(d => d.Id == 1004);
            
            _context.SetLink(bankAccount, "Bank", null);

            var dataServiceResponse = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);

            var operationResponse = Assert.Single(dataServiceResponse) as ChangeOperationResponse;

            Assert.NotNull(operationResponse);
            Assert.Equal(204, operationResponse.StatusCode);
        }

        [Fact]
        public async Task JsonBatchWithSingleChangesetWithAddRelatedObjectToExisting()
        {
            var bank = _context.Banks.SingleOrDefault(d => d.Id == 303);
            var bankAccount = new BankAccount
            {
                Id = 10006,
                AccountNumber = "10006",
                BankId = bank.Id,
            };
            
            _context.AddRelatedObject(bank, "BankAccounts", bankAccount);
            
            var dataServiceResponse = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);


            var operationResponse = Assert.Single(dataServiceResponse) as ChangeOperationResponse;

            Assert.NotNull(operationResponse);
            Assert.Equal(201, operationResponse.StatusCode);
        }

        [Fact]
        public async Task JsonBatchWithSingleChangesetWithSetRelatedObjectToExisting()
        {
            var bank = new Bank
            {
                Id = 3006,
                Name = "Bank 3006",
                Location = "Loc 1"
            };
            var bankAccount = _context.BankAccounts.SingleOrDefault(d => d.Id == 1005);

            _context.SetRelatedObject(bankAccount, "Bank", bank);

            var dataServiceResponse = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);
            
            var operationResponse = Assert.Single(dataServiceResponse) as ChangeOperationResponse;
            Assert.NotNull(operationResponse);
            Assert.Equal(201, operationResponse.StatusCode);
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

