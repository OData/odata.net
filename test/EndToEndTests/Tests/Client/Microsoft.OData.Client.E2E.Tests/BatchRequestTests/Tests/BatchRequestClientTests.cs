//---------------------------------------------------------------------
// <copyright file="BatchRequestClientTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.BatchRequestTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Xunit;
using Account = Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Account;
using AccountInfo = Microsoft.OData.Client.E2E.Tests.Common.Client.Default.AccountInfo;
using PaymentInstrument = Microsoft.OData.Client.E2E.Tests.Common.Client.Default.PaymentInstrument;
using Statement = Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Statement;

namespace Microsoft.OData.Client.E2E.Tests.BatchRequestTests.Tests;

public class BatchRequestClientTests : EndToEndTestBase<BatchRequestClientTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(BatchRequestTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
            {
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), new DefaultODataBatchHandler());
                opt.RouteOptions.EnableNonParenthesisForEmptyParameterFunction = true;
            });
        }
    }

    public BatchRequestClientTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDefaultDataSource();
    }

    [Fact]
    public async Task AddAndModifiedBatchRequestsTest()
    {
        // POST Requests
        var account = new Account
        {
            AccountID = 110,
            CountryRegion = "US"
        };
        _context.AddToAccounts(account);

        var paymentInstrument = new PaymentInstrument
        {
            PaymentInstrumentID = 102910,
            FriendlyName = "102 batch new PI",
            CreatedDate = new DateTimeOffset(new DateTime(2013, 12, 29, 11, 11, 57))
        };
        _context.AddRelatedObject(account, "MyPaymentInstruments", paymentInstrument);

        var billingStatement = new Statement
        {
            StatementID = 102910010,
            TransactionDescription = "Digital goods: PC",
            Amount = 1000
        };
        _context.AddRelatedObject(paymentInstrument, "BillingStatements", billingStatement);

        // PATCH Request
        var accountToUpdate = _context.Accounts.Where(a => a.AccountID == 102).Single();
        Assert.NotNull(accountToUpdate);
        var now = DateTimeOffset.Now;
        accountToUpdate.UpdatedTime = now;
        accountToUpdate.AccountInfo = new AccountInfo
        {
            FirstName = "John",
            LastName = "Doe"
        };
        _context.UpdateObject(accountToUpdate);

        var response = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseRelativeUri);

        // Assert
        Assert.Equal(4, response.Count());

        foreach (ChangeOperationResponse changeResponse in response.Cast<ChangeOperationResponse>())
        {
            Assert.True(changeResponse.StatusCode == 200 || changeResponse.StatusCode == 201);

            var descriptor = changeResponse.Descriptor as EntityDescriptor;
            Assert.NotNull(descriptor);

            if (descriptor.Entity is Account createdAccount && createdAccount?.AccountID == 110)
            {
                Assert.NotNull(createdAccount);
                Assert.Equal(110, createdAccount.AccountID);
                Assert.Equal("US", createdAccount.CountryRegion);
            }
            else if (descriptor.Entity is Account modifiedAccount && modifiedAccount?.UpdatedTime != null && modifiedAccount?.AccountID == 102)
            {
                Assert.NotNull(modifiedAccount);
                Assert.Equal(102, modifiedAccount.AccountID);
                Assert.Equal("GB", modifiedAccount.CountryRegion);
                Assert.Equal(now, modifiedAccount.UpdatedTime);
                Assert.Equal("John", modifiedAccount.AccountInfo.FirstName);
                Assert.Equal("Doe", modifiedAccount.AccountInfo.LastName);
            }
            else if (descriptor.Entity is PaymentInstrument createdPaymentInstrument)
            {
                Assert.NotNull(createdPaymentInstrument);
                Assert.Equal(102910, createdPaymentInstrument.PaymentInstrumentID);
                Assert.Equal("102 batch new PI", createdPaymentInstrument.FriendlyName);
            }
            else if (descriptor.Entity is Statement createdStatement)
            {
                Assert.NotNull(createdStatement);
                Assert.Equal(102910010, createdStatement.StatementID);
                Assert.Equal(1000, createdStatement.Amount);
                Assert.Equal("Digital goods: PC", createdStatement.TransactionDescription);
            }
        }
    }

    [Fact]
    public async Task QueryBatchRequestsTest()
    {
        // GET Requests
        var batchRequest = new DataServiceRequest[]
        {
            new DataServiceRequest<Account>(new Uri(_baseUri + "Accounts(102)")),
            new DataServiceRequest<PaymentInstrument>(new Uri(_baseUri + "Accounts(101)/MyPaymentInstruments")),
            new DataServiceRequest<Statement>(new Uri(_baseUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"))
        };

        var response = await _context.ExecuteBatchAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseRelativeUri, batchRequest);

        foreach (var operationResponse in response)
        {
            if (operationResponse is QueryOperationResponse<PaymentInstrument> paymentInstrumentResponse)
            {
                Assert.Equal(200, paymentInstrumentResponse.StatusCode);
                var paymentInstruments = paymentInstrumentResponse.ToList();
                Assert.Equal(3, paymentInstruments.Count);
            }

            else if (operationResponse is QueryOperationResponse<Statement> statementResponse)
            {
                Assert.Equal(200, statementResponse.StatusCode);
                var statements = statementResponse.ToList();
                Assert.Single(statements);
                Assert.Equal(103901001, statements[0].StatementID);
                Assert.Equal(100, statements[0].Amount);
                Assert.Equal("Digital goods: App", statements[0].TransactionDescription);
            }

            else if (operationResponse is QueryOperationResponse<Account> accountResponse)
            {
                Assert.Equal(200, accountResponse.StatusCode);
                var accounts = accountResponse.ToList();
                Assert.Single(accounts);
                Assert.Equal(102, accounts[0].AccountID);
                Assert.Equal("GB", accounts[0].CountryRegion);
            }
        }
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "batchrequests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
