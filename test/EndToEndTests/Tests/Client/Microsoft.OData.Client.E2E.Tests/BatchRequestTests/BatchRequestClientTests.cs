//---------------------------------------------------------------------
// <copyright file="BatchRequestClientTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.BatchRequest;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Xunit;
using Account = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Account;
using AccountInfo = Microsoft.OData.E2E.TestCommon.Common.Client.Default.AccountInfo;
using PaymentInstrument = Microsoft.OData.E2E.TestCommon.Common.Client.Default.PaymentInstrument;
using Statement = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Statement;

namespace Microsoft.OData.Client.E2E.Tests.BatchRequestTests;

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
        // Arrange

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
        var accountToUpdate = _context.Accounts.Where(a => a.AccountID == 107).Single();
        Assert.NotNull(accountToUpdate);
        var now = DateTimeOffset.Now;
        accountToUpdate.UpdatedTime = now;
        accountToUpdate.AccountInfo = new AccountInfo
        {
            FirstName = "John",
            LastName = "Doe"
        };
        _context.UpdateObject(accountToUpdate);

        // Act
        var response = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseRelativeUri);

        // Assert
        Assert.Equal(4, response.Count());

        var changeResponses = response.Cast<ChangeOperationResponse>().ToList();

        Assert.All(changeResponses, changeResponse => Assert.True(changeResponse.StatusCode == 200 || changeResponse.StatusCode == 201));

        // Account 110 is created
        var accountCreatedResponse = ((EntityDescriptor)changeResponses[0].Descriptor).Entity as Account;
        Assert.NotNull(accountCreatedResponse);
        Assert.Equal(110, accountCreatedResponse.AccountID);
        Assert.Equal("US", accountCreatedResponse.CountryRegion);
        Assert.Null(accountCreatedResponse.AccountInfo);

        // Account 107 is updated
        var accountUpdatedResponse = ((EntityDescriptor)changeResponses[3].Descriptor).Entity as Account;
        Assert.NotNull(accountUpdatedResponse);
        Assert.Equal(107, accountUpdatedResponse.AccountID);
        Assert.Equal("FR", accountUpdatedResponse.CountryRegion);
        Assert.Equal(now, accountUpdatedResponse.UpdatedTime);
        Assert.Equal("John", accountUpdatedResponse.AccountInfo.FirstName);
        Assert.Equal("Doe", accountUpdatedResponse.AccountInfo.LastName);

        // PaymentInstrument 102910 is created
        var paymentInstrumentCreatedResponse = ((EntityDescriptor)changeResponses[1].Descriptor).Entity as PaymentInstrument;
        Assert.NotNull(paymentInstrumentCreatedResponse);
        Assert.NotNull(paymentInstrumentCreatedResponse);
        Assert.Equal(102910, paymentInstrumentCreatedResponse.PaymentInstrumentID);
        Assert.Equal("102 batch new PI", paymentInstrumentCreatedResponse.FriendlyName);

        // Statement 102910010 is created
        var statementCreatedResponse = ((EntityDescriptor)changeResponses[2].Descriptor).Entity as Statement;
        Assert.NotNull(statementCreatedResponse);
        Assert.NotNull(statementCreatedResponse);
        Assert.Equal(102910010, statementCreatedResponse.StatementID);
        Assert.Equal(1000, statementCreatedResponse.Amount);
        Assert.Equal("Digital goods: PC", statementCreatedResponse.TransactionDescription);
    }

    [Fact]
    public async Task QueryBatchRequestsTest()
    {
        // Arrange
        var batchRequest = new DataServiceRequest[]
        {
            new DataServiceRequest<Account>(new Uri(_baseUri + "Accounts(102)")),
            new DataServiceRequest<PaymentInstrument>(new Uri(_baseUri + "Accounts(101)/MyPaymentInstruments")),
            new DataServiceRequest<Statement>(new Uri(_baseUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"))
        };

        // Act
        var response = await _context.ExecuteBatchAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseRelativeUri, batchRequest);

        var operationResponses = response.ToList();

        // Assert
        var accountResponse = operationResponses[0] as QueryOperationResponse<Account>;
        Assert.NotNull(accountResponse);
        Assert.Equal(200, accountResponse.StatusCode);
        var accounts = accountResponse.ToList();
        Assert.Single(accounts);
        Assert.Equal(102, accounts[0].AccountID);
        Assert.Equal("GB", accounts[0].CountryRegion);

        var paymentInstrumentResponse = operationResponses[1] as QueryOperationResponse<PaymentInstrument>;
        Assert.NotNull(paymentInstrumentResponse);
        Assert.Equal(200, paymentInstrumentResponse.StatusCode);
        var paymentInstruments = paymentInstrumentResponse.ToList();
        Assert.Equal(3, paymentInstruments.Count);

        var statementResponse = operationResponses[2] as QueryOperationResponse<Statement>;
        Assert.NotNull(statementResponse);
        Assert.Equal(200, statementResponse.StatusCode);
        var statements = statementResponse.ToList();
        Assert.Single(statements);
        Assert.Equal(103901001, statements[0].StatementID);
        Assert.Equal(100, statements[0].Amount);
        Assert.Equal("Digital goods: App", statements[0].TransactionDescription);
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "batchrequests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
