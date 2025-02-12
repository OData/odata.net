//-----------------------------------------------------------------------------
// <copyright file="ContainmentTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Containment;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;
using Xunit;
using ClientDefaultModel = Microsoft.OData.E2E.TestCommon.Common.Client.Default;

namespace Microsoft.OData.Client.E2E.Tests.ContainmentTests
{
    public class ContainmentTests : EndToEndTestBase<ContainmentTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ContainmentTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public ContainmentTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDefaultDataSource();
        }

        [Fact]
        public async Task QueryingAContainedEntityFromODataClient_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            var queryable = _context.CreateQuery<ClientDefaultModel.GiftCard>("Accounts(101)/MyGiftCard");
            Assert.EndsWith("Accounts(101)/MyGiftCard", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

            List<ClientDefaultModel.GiftCard> result = (await queryable.ExecuteAsync()).ToList();
            Assert.Single(result);
            Assert.Equal(301, result[0].GiftCardID);
            Assert.Equal("AAA123A", result[0].GiftCardNO);
        }

        [Fact]
        public async Task QueryingAContainedEntitySetFromODataClient_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            var queryable = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments");
            Assert.EndsWith("Accounts(103)/MyPaymentInstruments", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

            List<ClientDefaultModel.PaymentInstrument> result = (await queryable.ExecuteAsync()).ToList();
            Assert.Equal(4, result.Count);
            Assert.Equal(103902, result[1].PaymentInstrumentID);
            Assert.Equal("103 second PI", result[1].FriendlyName);
        }

        [Fact]
        public async Task QueryingASpecificEntityInAContainedEntitySetFromODataClient_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            var queryable = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments(103902)");
            Assert.EndsWith("Accounts(103)/MyPaymentInstruments(103902)", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

            List<ClientDefaultModel.PaymentInstrument> result = (await queryable.ExecuteAsync()).ToList();
            Assert.Single(result);
            Assert.Equal(103902, result[0].PaymentInstrumentID);
            Assert.Equal("103 second PI", result[0].FriendlyName);
        }

        [Fact]
        public async Task QueryingAnIndividualPropertyOfAContainedEntityFromODataClient_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            var queryable = _context.CreateQuery<int>("Accounts(103)/MyPaymentInstruments(103902)/PaymentInstrumentID");
            Assert.EndsWith("Accounts(103)/MyPaymentInstruments(103902)/PaymentInstrumentID", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

            List<int> result = (await queryable.ExecuteAsync()).ToList();
            Assert.Single(result);
            Assert.Equal(103902, result[0]);
        }

        [Fact]
        public async Task LinqUriTranslationTest_WorkCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            //translate to key
            var q1 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.PaymentInstrumentID == 103901);
            ClientDefaultModel.PaymentInstrument q1Result = (await ((DataServiceQuery<ClientDefaultModel.PaymentInstrument>)q1).ExecuteAsync()).Single();
            Assert.Equal(103901, q1Result.PaymentInstrumentID);

            //$filter
            var q2 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.CreatedDate > new DateTimeOffset(new DateTime(2013, 10, 1)));
            ClientDefaultModel.PaymentInstrument q2Result = (await ((DataServiceQuery<ClientDefaultModel.PaymentInstrument>)q2).ExecuteAsync()).Single();
            Assert.Equal(103905, q2Result.PaymentInstrumentID);

            //$orderby
            var q3 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").OrderBy(pi => pi.CreatedDate).ThenByDescending(pi => pi.FriendlyName);
            List<ClientDefaultModel.PaymentInstrument> q3Result = (await ((DataServiceQuery<ClientDefaultModel.PaymentInstrument>)q3).ExecuteAsync()).ToList();
            Assert.Equal(103902, q3Result[0].PaymentInstrumentID);

            //$expand
            var q4 = _context.Accounts.Expand(account => account.MyPaymentInstruments).Where(account => account.AccountID == 103);
            ClientDefaultModel.Account q4Result = (await ((DataServiceQuery<ClientDefaultModel.Account>)q4).ExecuteAsync()).Single();
            Assert.NotNull(q4Result.MyPaymentInstruments);

            var q5 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Expand(pi => pi.BillingStatements).Where(pi => pi.PaymentInstrumentID == 103901);
            ClientDefaultModel.PaymentInstrument q5Result = (await ((DataServiceQuery<ClientDefaultModel.PaymentInstrument>)q5).ExecuteAsync()).Single();
            Assert.NotNull(q5Result.BillingStatements);

            //$top
            var q6 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Take(1);
            var q6Result = (await ((DataServiceQuery<ClientDefaultModel.PaymentInstrument>)q6).ExecuteAsync()).ToList();

            //$count
            var q7 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Count();

            //$count=true
            var q8 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").IncludeCount();
            var q8Result = (await q8.ExecuteAsync()).ToList();

            //projection
            var q9 = _context.Accounts.Where(a => a.AccountID == 103).Select(a => new ClientDefaultModel.Account() { AccountID = a.AccountID, MyGiftCard = a.MyGiftCard });
            var q9Result = (await ((DataServiceQuery<ClientDefaultModel.Account>)q9).ExecuteAsync()).Single();
            Assert.NotNull(q9Result.MyGiftCard);

            var q10 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.PaymentInstrumentID == 103901).Select(p => new ClientDefaultModel.PaymentInstrument()
            {
                PaymentInstrumentID = p.PaymentInstrumentID,
                BillingStatements = p.BillingStatements
            });
            var q10Result = (await ((DataServiceQuery<ClientDefaultModel.PaymentInstrument>)q10).ExecuteAsync()).ToList();
        }

        [Fact]
        public async Task CallingFunctionFromODataClientThatReturnsAContainedEntity_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            ClientDefaultModel.PaymentInstrument result = (await _context.ExecuteAsync<ClientDefaultModel.PaymentInstrument>(new Uri(_baseUri.AbsoluteUri +
                "Accounts(101)/Default.GetDefaultPI()", UriKind.Absolute), "GET", true)).Single();
            Assert.Equal(101901, result.PaymentInstrumentID);

            result.FriendlyName = "Random Name";
            _context.UpdateObject(result);
            await _context.SaveChangesAsync();

            result = (await _context.ExecuteAsync<ClientDefaultModel.PaymentInstrument>(new Uri(_baseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101901)", UriKind.Absolute), "GET", true)).Single();
            Assert.Equal("Random Name", result.FriendlyName);
        }

        [Fact]
        public async Task InvokingAnActionFromODataClientThatReturnsAContainedEntity_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            ClientDefaultModel.PaymentInstrument result = (await _context.ExecuteAsync<ClientDefaultModel.PaymentInstrument>(new Uri(_baseUri.AbsoluteUri +
                "Accounts(101)/Default.RefreshDefaultPI", UriKind.Absolute), "POST", true,
                new BodyOperationParameter("newDate", new DateTimeOffset(DateTime.Now)))).Single();
            Assert.Equal(101901, result.PaymentInstrumentID);

            result.FriendlyName = "Random Name";
            _context.UpdateObject(result);
            await _context.SaveChangesAsync();

            result = (await _context.ExecuteAsync<ClientDefaultModel.PaymentInstrument>(new Uri(_baseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101901)", UriKind.Absolute), "GET")).Single();
            Assert.Equal("Random Name", result.FriendlyName);
        }

        [Fact]
        public async Task CreatingAContainedEntityFromODataClientUsingAddRelatedObject_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            // create an an account entity and a contained PI entity
            ClientDefaultModel.Account newAccount = new ClientDefaultModel.Account()
            {
                AccountID = 110,
                CountryRegion = "CN",
                AccountInfo = new ClientDefaultModel.AccountInfo()
                {
                    FirstName = "New",
                    LastName = "Guy"
                }
            };
            ClientDefaultModel.PaymentInstrument newPI = new ClientDefaultModel.PaymentInstrument()
            {
                PaymentInstrumentID = 110901,
                FriendlyName = "110's first PI",
                CreatedDate = new DateTimeOffset(new DateTime(2012, 12, 10))
            };

            _context.AddToAccounts(newAccount);
            _context.AddRelatedObject(newAccount, "MyPaymentInstruments", newPI);
            await _context.SaveChangesAsync();

            var queryable0 = _context.Accounts.ByKey(110);
            ClientDefaultModel.Account accountResult = await queryable0.GetValueAsync();
            Assert.Equal("Guy", accountResult.AccountInfo.LastName);

            var queryable1 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(110)/MyPaymentInstruments").ByKey(110901);
            ClientDefaultModel.PaymentInstrument piResult = await queryable1.GetValueAsync();
            Assert.Equal("110's first PI", piResult.FriendlyName);
        }

        [Fact]
        public async Task UpdatingAContainedEntityFromODataClientUsingUpdateObject_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            // Get a contained PI entity
            var queryable1 = _context.CreateQuery<ClientDefaultModel.PaymentInstrument>("Accounts(101)/MyPaymentInstruments").ByKey(101901);
            ClientDefaultModel.PaymentInstrument piResult = await queryable1.GetValueAsync();

            piResult.FriendlyName = "Michael's first PI";
            _context.UpdateObject(piResult);
            await _context.SaveChangesAsync();

            piResult = await queryable1.GetValueAsync();
            Assert.Equal("Michael's first PI", piResult.FriendlyName);
        }

        [Fact]
        public async Task CreateContainedNonCollectionEntityFromODataClientUsingUpdateRelatedObject()
        {
            _context.Format.UseJson(_model);

            // create an an account entity and a contained PI entity
            ClientDefaultModel.Account newAccount = new ClientDefaultModel.Account()
            {
                AccountID = 120,
                CountryRegion = "GB",
                AccountInfo = new ClientDefaultModel.AccountInfo()
                {
                    FirstName = "Diana",
                    LastName = "Spencer"
                }
            };

            ClientDefaultModel.GiftCard giftCard = new ClientDefaultModel.GiftCard()
            {
                GiftCardID = 320,
                GiftCardNO = "XX120ABCDE",
                Amount = 76,
                ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
            };

            _context.AddToAccounts(newAccount);
            _context.UpdateRelatedObject(newAccount, "MyGiftCard", giftCard);
            await _context.SaveChangesAsync();

            var queryable1 = _context.CreateQuery<ClientDefaultModel.GiftCard>("Accounts(120)/MyGiftCard");
            List<ClientDefaultModel.GiftCard> giftCardResult = (await queryable1.ExecuteAsync()).ToList();

            Assert.Single(giftCardResult);
            Assert.Equal(76, giftCardResult[0].Amount);
        }

        public static IEnumerable<object[]> SelectQueryTestData()
        {
            yield return new object[] { "Accounts(101)/MyGiftCard?$select=GiftCardID,GiftCardNO", 2, "application/json;odata.metadata=full" };
            yield return new object[] { "Accounts(101)/MyGiftCard?$select=GiftCardID,GiftCardNO", 2, "application/json;odata.metadata=minimal" };
            yield return new object[] { "Accounts(101)/MyGiftCard?$select=GiftCardID,GiftCardNO", 2, "application/json;odata.metadata=none" };
            yield return new object[] { "Accounts(101)/MyPaymentInstruments(101901)?$select=PaymentInstrumentID,FriendlyName,CreatedDate", 3, "application/json;odata.metadata=full" };
            yield return new object[] { "Accounts(101)/MyPaymentInstruments(101901)?$select=PaymentInstrumentID,FriendlyName,CreatedDate", 3, "application/json;odata.metadata=minimal" };
            yield return new object[] { "Accounts(101)/MyPaymentInstruments(101901)?$select=PaymentInstrumentID,FriendlyName,CreatedDate", 3, "application/json;odata.metadata=none" };
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "containmenttests/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
