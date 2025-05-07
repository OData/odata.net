//---------------------------------------------------------------------
// <copyright file="AsynchronousQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Asynchronous;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Xunit;
using Computer = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Computer;
using ComputerDetail = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.ComputerDetail;
using ContactDetails = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.ContactDetails;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Customer;
using Driver = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Driver;
using Employee = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Employee;

namespace Microsoft.OData.Client.E2E.Tests.AsynchronousTests;

/// <summary>
/// Client query tests using asynchronous APIs
/// - AddQueryOption
/// </summary>
public class AsynchronousQueryTests : AsynchronousEndToEndTestBase<AsynchronousQueryTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(AsynchronousQueryTestsController), typeof(MetadataController));
            
            services.AddControllers().AddOData(opt =>
            {
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(),
                    configureServices: s =>
                    {
                        s.AddSingleton<ODataBatchHandler>(sp => new DefaultODataBatchHandler());
                        s.AddSingleton<ODataResourceSerializer, CustomODataResourceSerializer>();
                    });
            });
        }
    }

    public AsynchronousQueryTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

    /// <summary>
    /// Add $filter query option
    /// </summary>
    [Fact]
    public void AddQueryOption_WithFilter_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.AddQueryOption("$filter", "true");

        // Act & Assert
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar);
                Assert.Equal(10, customers.Count());
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// Add $select query option
    /// </summary>
    [Fact]
    public void AddQueryOption_WithSelect_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.AddQueryOption("$select", "CustomerId");

        // Act & Assert
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar);
                Assert.Equal(10, customers.Count());
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// Add $orderby query option
    /// </summary>
    [Fact]
    public void AddQueryOption_WithOrderBy_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.AddQueryOption("$orderby", "CustomerId desc");

        // Act & Assert
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar);
                Assert.Equal(10, customers.Count());
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// Add two query options
    /// </summary>
    [Fact]
    public void AddQueryOption_WithMultipleOptions_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers
            .AddQueryOption("$filter", "true")
            .AddQueryOption("$orderby", "CustomerId desc");

        // Act & Assert
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar);
                Assert.Equal(10, customers.Count());
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// Query Entity Set  With Server Driven Paging
    /// </summary>
    [Fact]
    public void QueryEntitySet_WithServerDrivenPaging()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

        // Act
        var query = _context.Customers.IncludeCount();
        var ar = query.BeginExecute(null, null).EnqueueWait(this);
        var response = query.EndExecute(ar) as QueryOperationResponse<Customer>;
        var totalCount = response?.Count();
        var count = totalCount;
        var continuation = response?.GetContinuation();

        while (continuation != null)
        {
            var ar2 = _context.BeginExecute(continuation, null, null).EnqueueWait(this);
            var response2 = _context.EndExecute<Customer>(ar2);

            var currentPageCount = (response2 as QueryOperationResponse<Customer>)?.Count();
            count += currentPageCount;
            continuation = (response2 as QueryOperationResponse<Customer>)?.GetContinuation();
        }

        // Assert
        Assert.EndsWith("/odata/Customers?$count=true", response?.Query.RequestUri.AbsoluteUri);
        Assert.Equal(totalCount, count);
        this.EnqueueTestComplete();
    }

    [Fact]
    public void PreferCustomInstanceAnnotationTest()
    {
        // Arrange
        var value = "";
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        _context.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Prefer", "odata.include-annotations=*");
        _context.Configurations.ResponsePipeline.OnEntryEnded((ReadingEntryArgs) => value = (ReadingEntryArgs.Entry.InstanceAnnotations)?.FirstOrDefault()?.Name);

        var query = _context.Computers.OrderBy(c => c.ComputerId) as DataServiceQuery<Computer>;

        // Act
        var ar1 = query?.BeginExecute(null, null).EnqueueWait(this);
        var response = (query?.EndExecute(ar1) as QueryOperationResponse<Computer>);

        // Assert
        Assert.NotNull(response);
        var computer = response?.FirstOrDefault();
        Assert.Equal("MyNamespace.CustomAnnotation1", value);
        value = "";

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// ExecuteBatch Requests
    /// </summary>
    [Fact]
    public void ExecuteBatch_WithMultipleRequests_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var countOfBatchParts = 0;
        var countOfTimesSenderCalled = 0;
        _context.SendingRequest2 += ((sender, args) =>
        {
            if (args.IsBatchPart)
            {
                countOfBatchParts++;
            }

            countOfTimesSenderCalled++;
        });

        // Act
        var arBatch = _context.BeginExecuteBatch(
            null,
            null,
            new DataServiceRequest[]
            {
                new DataServiceRequest<Customer>(((from c in _context.Customers where c.CustomerId == -8 select c) as DataServiceQuery<Customer>)?.RequestUri),
                new DataServiceRequest<Customer>(((from c in _context.Customers where c.CustomerId == -6 select c) as DataServiceQuery<Customer>)?.RequestUri),
                new DataServiceRequest<Driver>(((from c in _context.Drivers where c.Name == "1" select c) as DataServiceQuery<Driver>)?.RequestUri),
                new DataServiceRequest<Driver>(((from c in _context.Drivers where c.Name == "3" select c) as DataServiceQuery<Driver>)?.RequestUri)
            }).EnqueueWait(this);

        DataServiceResponse qr = _context.EndExecuteBatch(arBatch);
        string actualValues = "";
        foreach (var r in qr)
        {
            if (r is QueryOperationResponse<Customer>)
            {
                var customer = (r as QueryOperationResponse<Customer>)?.Single();
                Assert.NotNull(customer);
                actualValues += customer.CustomerId;
            }

            if (r is QueryOperationResponse<Driver>)
            {
                var driver = (r as QueryOperationResponse<Driver>)?.Single();
                Assert.NotNull(driver);
                actualValues += driver.Name;
            }
        }

        // Assert
        Assert.Equal("-8-613", actualValues);
        Assert.True(countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts) == 1, "countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts ) == 1");
        this.EnqueueTestComplete();
    }

    /// <summary>
    /// ExecuteBatch Requests
    /// </summary>
    [Fact]
    public void ExecuteBatch_WithSaveChangesOptions_Test()
    {
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var countOfBatchParts = 0;
        var countOfTimesSenderCalled = 0;
        _context.SendingRequest2 += ((sender, args) =>
        {
            if (args.IsBatchPart)
            {
                countOfBatchParts++;
            }

            countOfTimesSenderCalled++;
        });

        var arBatch = _context.BeginExecuteBatch(
            null, // callback
            null, // state
            SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseRelativeUri,
            new DataServiceRequest[]
            {
                    new DataServiceRequest<Customer>(((_context.Customers.Where(c => c.CustomerId == -8)) as DataServiceQuery<Customer>)?.RequestUri),
                    new DataServiceRequest<Customer>(((_context.Customers.Where(c => c.CustomerId == -6)) as DataServiceQuery<Customer>)?.RequestUri),
                    new DataServiceRequest<Driver>(((_context.Drivers.Where(c => c.Name == "1")) as DataServiceQuery<Driver>)?.RequestUri),
                    new DataServiceRequest<Driver>(((_context.Drivers.Where(c => c.Name == "3")) as DataServiceQuery<Driver>)?.RequestUri)
            }).EnqueueWait(this);

        DataServiceResponse qr = _context.EndExecuteBatch(arBatch);
        string actualValues = "";
        foreach (var r in qr)
        {
            if (r is QueryOperationResponse<Customer>)
            {
                var customer = (r as QueryOperationResponse<Customer>)?.Single();
                Assert.NotNull(customer);
                actualValues += customer.CustomerId;
            }

            if (r is QueryOperationResponse<Driver>)
            {
                var driver = (r as QueryOperationResponse<Driver>)?.Single();
                Assert.NotNull(driver);
                actualValues += driver.Name;
            }
        }

        // Assert
        Assert.Equal("-8-613", actualValues);
        Assert.True(countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts) == 1, "countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts ) == 1");
        this.EnqueueTestComplete();
    }

    /// <summary>
    /// Query Entity Set
    /// </summary>
    [Fact]
    public void QueryEntitySet_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers;

        // Act & Assert
        query.BeginExecute(
            ar =>
            {
                var customers = query.EndExecute(ar).ToList();
                Assert.Equal(10, customers.Count());
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// IncludeCount Test
    /// </summary>
    [Fact]
    public void IncludeCount_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

        // Act & Assert
        var query = _context.Computers.IncludeCount();
        query.BeginExecute(
            (ar) =>
            {
                var comps = query.EndExecute(ar) as QueryOperationResponse<Computer>;
                Assert.NotNull(comps);
                Assert.Equal(10, comps.Count);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// IncludeCount Test
    /// </summary>
    [Fact]
    public void IncludeCount_WithServerDrivenPaging_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

        // Act & Assert
        var query = _context.Customers.IncludeCount();
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar) as QueryOperationResponse<Customer>;
                Assert.NotNull(customers);
                Assert.Equal(10, customers.Count);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query with nested calls to All
    /// </summary>
    [Fact]
    public void Linq_All_WithNestedConditions_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.Where(c => c.Logins.All(l => l.Orders.All(o => o.OrderId > 0))) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query = query.IncludeCount();

        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar) as QueryOperationResponse<Customer>;
                Assert.NotNull(customers);
                Assert.Equal(4, customers.Count);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using All()
    /// </summary>
    [Fact]
    public void Linq_All_WithSingleCondition_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.Where(c => c.Orders.All(o => o.OrderId > 0)) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query = query.IncludeCount();
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar) as QueryOperationResponse<Customer>;
                Assert.NotNull(customers);
                Assert.Equal(6, customers.Count);
                this.EnqueueTestComplete();
            },
                null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using nested calls to Any()
    /// </summary>
    [Fact]
    public void Linq_Any_WithNestedConditions_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.Where(c => c.Logins.Any(l => l.Orders.Any())) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query = query.IncludeCount();
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar) as QueryOperationResponse<Customer>;
                Assert.NotNull(customers);
                Assert.Equal(4, customers.Count);
                this.EnqueueTestComplete();
            },
                null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using Any()
    /// </summary>
    [Fact]
    public void Linq_Any_WithSingleCondition_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.Where(c => c.Orders.Any(o => o.OrderId < 0)) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query = query.IncludeCount();

        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar) as QueryOperationResponse<Customer>;
                Assert.NotNull(customers);
                Assert.Equal(4, customers.Count);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using Expand()
    /// </summary>
    [Fact]
    public void Linq_Expand_WithNavigationProperty_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.Expand("Wife").Where(c => c.CustomerId == -10) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customerWithWife = query.EndExecute(ar).Single();
                Assert.NotNull(customerWithWife.Wife);
                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customerWithWife.Wife.Name);
                this.EnqueueTestComplete();
            },
        null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using OrderByDescending()
    /// </summary>
    [Fact]
    public void Linq_OrderByDescending_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.OrderByDescending(c => c.Name) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();
                Assert.Equal(10, customers.Count);
                Assert.Equal("versioningtaskspurgesizesminusdatarfcactivator", customers.First().Name);
                Assert.Equal("allocatedentitiescontentcontainercurrentsynchronously", customers.ElementAt(8).Name);
                Assert.Null(customers.Last().Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using OrderByDescending() and ThenByDescending()
    /// </summary>
    [Fact]
    public void Linq_OrderByDescending_ThenByDescending_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers
            .OrderByDescending(c => c.PrimaryContactInfo.HomePhone.Extension)
            .ThenByDescending(c => c.Name) as DataServiceQuery<Customer>;
        List<Customer>? customers = null;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                try
                {
                    customers = query.EndExecute(ar).ToList();
                }
                finally
                {
                    this.TestCompleted = true;
                }
            },
            null);

        this.EnqueueConditional(() => this.TestCompleted);
        this.EnqueueCallback(() => Assert.NotNull(customers));
        this.EnqueueCallback(() => Assert.Equal("namedpersonalabsentnegationbelowstructuraldeformattercreatebackupterrestrial", customers?.First().Name));
        this.EnqueueCallback(() => Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customers?.Last().Name));
        this.EnqueueTestComplete();
    }

    /// <summary>
    /// LINQ query using OrderByDescending() and ThenBy()
    /// </summary>
    [Fact]
    public void Linq_OrderByDescending_ThenBy_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers
            .OrderByDescending(c => c.PrimaryContactInfo.WorkPhone.PhoneNumber)
            .ThenBy(c => c.Name) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();
                Assert.Equal("forbuiltinencodedchnlsufficientexternal", customers.First().Name);
                Assert.Equal("enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart", customers.ElementAt(8).Name);
                Assert.Null(customers.Last().Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using OrderBy()
    /// </summary>
    [Fact]
    public void Linq_OrderBy_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.OrderBy(c => c.Name) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();
                Assert.Null(customers.First().Name);
                Assert.Equal("versioningtaskspurgesizesminusdatarfcactivator", customers.Last().Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using OrderBy() and ThenByDescending()
    /// </summary>
    [Fact]
    public void Linq_OrderBy_ThenByDescending_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers
            .Where(c => c.Name != null && c.PrimaryContactInfo != null && c.PrimaryContactInfo.WorkPhone != null && c.PrimaryContactInfo.WorkPhone.PhoneNumber != null)
            .OrderBy(c => c.PrimaryContactInfo.WorkPhone.PhoneNumber)
            .ThenByDescending(c => c.Name) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();
                Assert.Equal("enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart", customers.First().Name);
                Assert.Equal("forbuiltinencodedchnlsufficientexternal", customers.Last().Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using OrderBy() and ThenBy()
    /// </summary>
    [Fact]
    public void Linq_OrderBy_ThenBy_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers
            .OrderBy(c => c.PrimaryContactInfo.WorkPhone.PhoneNumber)
            .ThenBy(c => c.Name) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();
                Assert.Null(customers.First().Name);
                Assert.Equal("forbuiltinencodedchnlsufficientexternal", customers.Last().Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project a primitive property from a feed, materialized into a custom entity
    /// </summary>
    [Fact]
    public void Linq_ProjectIntoCustomEntity_FromEntitySet_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.Name != null
                     select new EntityCustomer { Name = c.Name }) as DataServiceQuery<EntityCustomer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();

                Assert.Equal(9, customers.Count);

                // Because the customer id is not projected, it will be 0
                Assert.All(customers, c => Assert.Equal(0, c.CustomerId));
                Assert.All(customers, c => Assert.NotNull(c.Name));

                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customers[0].Name);
                Assert.Equal("enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart", customers.ElementAt(1).Name);
                Assert.Equal("forbuiltinencodedchnlsufficientexternal", customers.ElementAt(5).Name);
                Assert.Equal("allocatedentitiescontentcontainercurrentsynchronously", customers.ElementAt(8).Name);

                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project a primitive property from a feed, materialized into a non entity object using the constructor
    /// </summary>
    [Fact]
    public void Linq_ProjectIntoNonEntity_UsingConstructor_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.Name != null
                     select new NonEntityCustomer2(1000, c.Name)) as DataServiceQuery<NonEntityCustomer2>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();

                Assert.Equal(9, customers.Count);

                // Because the customer id is not projected, and it is assigned to 1000
                Assert.All(customers, c => Assert.Equal(1000, c.CustomerId));
                Assert.All(customers, c => Assert.NotNull(c.Name));

                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customers[0].Name);
                Assert.Equal("enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart", customers.ElementAt(1).Name);
                Assert.Equal("forbuiltinencodedchnlsufficientexternal", customers.ElementAt(5).Name);
                Assert.Equal("allocatedentitiescontentcontainercurrentsynchronously", customers.ElementAt(8).Name);
                this.EnqueueTestComplete();
            },
        null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project a primitive property from a feed, materialized into a non entity object using property initializers
    /// </summary>
    [Fact]
    public void Linq_ProjectIntoNonEntity_UsingInitializers_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.Name != null
                     select new NonEntityCustomer { CustomerId = 1000, Name = c.Name }) as DataServiceQuery<NonEntityCustomer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();

                Assert.Equal(9, customers.Count);

                // Because the customer id is not projected, and it is assigned to 1000
                Assert.All(customers, c => Assert.Equal(1000, c.CustomerId));
                Assert.All(customers, c => Assert.NotNull(c.Name));

                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customers[0].Name);
                Assert.Equal("enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart", customers.ElementAt(1).Name);
                Assert.Equal("forbuiltinencodedchnlsufficientexternal", customers.ElementAt(5).Name);
                Assert.Equal("allocatedentitiescontentcontainercurrentsynchronously", customers.ElementAt(8).Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project a primitive property from a feed
    /// </summary>
    [Fact]
    public void Linq_ProjectSingleProperty_FromEntitySet_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.Name != null
                     select new Customer { Name = c.Name }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();

                Assert.Equal(9, customers.Count);

                // Because the customer id is not projected, it is null -> Customer { Id = null } -> can be null
                Assert.All(customers, c => Assert.Null(c.CustomerId));
                Assert.All(customers, c => Assert.NotNull(c.Name));

                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customers[0].Name);
                Assert.Equal("enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart", customers.ElementAt(1).Name);
                Assert.Equal("forbuiltinencodedchnlsufficientexternal", customers.ElementAt(5).Name);
                Assert.Equal("allocatedentitiescontentcontainercurrentsynchronously", customers.ElementAt(8).Name);

                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project two primitive property from a feed
    /// </summary>
    [Fact]
    public void Linq_ProjectTwoProperties_FromEntitySet_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.Name != null
                     select new Customer { Name = c.Name, CustomerId = c.CustomerId }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();
                Assert.Equal(9, customers.Count);

                Assert.All(customers, c => Assert.NotNull(c.CustomerId));
                Assert.All(customers, c => Assert.NotEqual(0, c.CustomerId));
                Assert.All(customers, c => Assert.NotNull(c.Name));

                Assert.Equal(-10, customers[0].CustomerId);
                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customers[0].Name);
                Assert.Equal(-9, customers.ElementAt(1).CustomerId);
                Assert.Equal("enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart", customers.ElementAt(1).Name);
                Assert.Equal(-4, customers.ElementAt(5).CustomerId);
                Assert.Equal("forbuiltinencodedchnlsufficientexternal", customers.ElementAt(5).Name);
                Assert.Equal(-1, customers.ElementAt(8).CustomerId);
                Assert.Equal("allocatedentitiescontentcontainercurrentsynchronously", customers.ElementAt(8).Name);

                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query Project properties from entity and expanded entity
    /// </summary>
    [Fact]
    public void Linq_ProjectProperties_FromEntityAndExpandedEntity_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Computers
                         where c.ComputerId == -10
                         select new Computer { ComputerId = c.ComputerId, ComputerDetail = new ComputerDetail { ComputerDetailId = c.ComputerDetail.ComputerDetailId } }
                     ) as DataServiceQuery<Computer>;

        // Act & Assert
        Assert.NotNull(query);
        var ar = query.BeginExecute(null, null).EnqueueWait(this);
        var c1 = query.EndExecute(ar).Single();
        Assert.Equal(-10, c1.ComputerId);
        Assert.Equal(-10, c1.ComputerDetail.ComputerDetailId);

        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task Linq_ProjectProperties_WithConditionalNullCheck_Test()
    {
        // Assert
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Computers.Where(c => c.ComputerId == -10)
                    .Select(c => new Computer
                    {
                        ComputerId = c.ComputerId,
                        // this contrived expression is to get the plan compiler to perform
                        // a null check against an expanded entity
                        ComputerDetail = c.ComputerDetail == null ? null : c.ComputerDetail,
                    }) as DataServiceQuery<Computer>;

        // Act & Assert
        Assert.NotNull(query);
        var result = await query.ExecuteAsync();

        var computer = result.First();
        Assert.Equal(-10, computer.ComputerId);
        Assert.Equal(-10, computer.ComputerDetail.ComputerDetailId);
    }

    [Fact]
    public async Task Linq_ProjectProperties_FromNestedExpandedEntity_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.ComputerDetails.Where(c => c.ComputerDetailId == -10)
            .Select(c => new Computer
            {
                ComputerId = c.Computer.ComputerId,
            }) as DataServiceQuery<Computer>;

        // Act & Assert
        Assert.NotNull(query);
        var result = await query.ExecuteAsync();

        var computer = result.First();
        Assert.Equal(-10, computer.ComputerId);
    }

    [Fact]
    public async Task Linq_ProjectProperties_FromNestedComplexType_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.Where(c => c.CustomerId == -10)
            .Select(c => new ContactDetails
            {
                HomePhone = c.PrimaryContactInfo.HomePhone
            }) as DataServiceQuery<ContactDetails>;

        // Act & Assert
        Assert.NotNull(query);
        var result = await query.ExecuteAsync();

        var contactDetails = result.First();
        Assert.Equal("jqjklhnnkyhujailcedbguyectpuamgbghreatqvobbtj", contactDetails.HomePhone.Extension);
    }

    /// <summary>
    /// LINQ query Project Name Stream Property
    /// </summary>
    [Fact]
    public void Linq_ProjectStreamProperty_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.CustomerId == -10
                     select new Customer { CustomerId = c.CustomerId, Video = c.Video }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
               (ar) =>
               {
                   var c1 = query.EndExecute(ar).SingleOrDefault();
                   Assert.Equal(-10, c1?.CustomerId);
                   Assert.NotNull(c1?.Video);
                   this.EnqueueTestComplete();
               },
               null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query Order By Canonical Functions
    /// </summary>
    [Fact]
    public void Linq_OrderBy_WithCanonicalFunction_String_Test()
    {
        // Arrange
        var name = "commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass";
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var q0 = (from c in _context.Customers
                  orderby c.Name.Contains(name) && c.Name.Length == name.Length
                  select new Customer { CustomerId = c.CustomerId, Name = c.Name, }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(q0);
        Assert.EndsWith("odata/Customers?$orderby=contains(Name,'commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass') and length(Name) eq 88&$select=CustomerId,Name", q0.ToString());
        var ar0 = q0.BeginExecute(null, null).EnqueueWait(this);
        var value0 = q0.EndExecute(ar0).ToList();
        Assert.Equal(10, value0.Count);
        Assert.Equal(-9, value0[1].CustomerId);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// LINQ query Order By Canonical Functions
    /// </summary>
    [Fact]
    public void Linq_OrderBy_WithCanonicalFunction_Math_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var q1 = (from c in _context.ComputerDetails
                  orderby System.Math.Ceiling(c.Dimensions.Depth) < 0
                  select c) as DataServiceQuery<ComputerDetail>;

        // Act & Assert
        Assert.NotNull(q1);
        Assert.EndsWith("odata/ComputerDetails?$orderby=ceiling(Dimensions/Depth) lt 0", q1.ToString());
        var ar1 = q1.BeginExecute(null, null).EnqueueWait(this);
        var value1 = q1.EndExecute(ar1).ToList();
        Assert.True(value1.First().Dimensions.Depth > value1.Last().Dimensions.Depth);
    }

    /// <summary>
    /// LINQ query Order By Canonical Functions
    /// </summary>
    [Fact]
    public void Linq_OrderBy_WithCanonicalFunction_DateTime_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var q2 = (from c in _context.ComputerDetails
                  orderby c.PurchaseDate.Day == 15 && c.PurchaseDate.Year == 2020
                  select c) as DataServiceQuery<ComputerDetail>;

        // Act & Assert
        Assert.NotNull(q2);
        Assert.EndsWith("odata/ComputerDetails?$orderby=day(PurchaseDate) eq 15 and year(PurchaseDate) eq 2020", q2.ToString());

        var ar2 = q2.BeginExecute(null, null).EnqueueWait(this);
        var value2 = q2.EndExecute(ar2).ToList();
        Assert.Equal(10, value2.Count);

        Assert.True(value2.First().ComputerDetailId == -9);
        Assert.True(value2.Last().ComputerDetailId == -10);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// LINQ query Order By Canonical Functions
    /// </summary>
    [Fact]
    public void Linq_OrderBy_WithCanonicalFunction_Int_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var q4 = (from c in _context.People.OfType<Employee>()
                  orderby c.ManagersPersonId
                  select c) as DataServiceQuery<Employee>;

        // Act & Assert
        Assert.NotNull(q4);
        var ar = q4.BeginExecute(null, null).EnqueueWait(this);
        var value3 = q4.EndExecute(ar).ToList();
        Assert.Equal(5309, value3.Last().ManagersPersonId);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// LINQ query Filter With Canonical Functions
    /// </summary>
    [Fact]
    public void Linq_Filter_WithCanonicalFunction_String_Test()
    {
        // Arrange
        var name = "commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass";
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var q0 = (from c in _context.Customers
                  where c.Name.Contains(name) && c.Name.Length == name.Length
                  select new Customer { CustomerId = c.CustomerId, Name = c.Name, }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(q0);
        Assert.EndsWith("odata/Customers?$filter=contains(Name,'commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass') and length(Name) eq 88&$select=CustomerId,Name", q0.ToString());
        var ar0 = q0.BeginExecute(null, null).EnqueueWait(this);
        var value0 = q0.EndExecute(ar0).Single();
        Assert.Equal(name, value0.Name);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// LINQ query Order By Canonical Functions
    /// </summary>
    [Fact]
    public void Linq_Filter_WithCanonicalFunction_Math_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var q1 = (from c in _context.ComputerDetails
                  where System.Math.Ceiling(c.Dimensions.Depth) < 0
                  select c) as DataServiceQuery<ComputerDetail>;

        // Act & Assert
        Assert.NotNull(q1);
        Assert.EndsWith("odata/ComputerDetails?$filter=ceiling(Dimensions/Depth) lt 0", q1.ToString());

        var ar1 = q1.BeginExecute(null, null).EnqueueWait(this);
        var value1 = q1.EndExecute(ar1).ToList();
        Assert.Equal(7, value1.Count);
    }

    /// <summary>
    /// LINQ query Order By Canonical Functions
    /// </summary>
    [Fact]
    public void Linq_Filter_WithCanonicalFunction_Date_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var q2 = (from c in _context.ComputerDetails
                  where c.PurchaseDate.Day == 15 && c.PurchaseDate.Year == 2020
                  select c) as DataServiceQuery<ComputerDetail>;

        // Act & Assert
        Assert.NotNull(q2);
        Assert.EndsWith("odata/ComputerDetails?$filter=day(PurchaseDate) eq 15 and year(PurchaseDate) eq 2020", q2.ToString());

        var ar2 = q2.BeginExecute(null, null).EnqueueWait(this);
        var value2 = q2.EndExecute(ar2).Single();
        Assert.Equal(-10, value2.ComputerDetailId);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// LINQ query Order By Canonical Functions
    /// </summary>
    [Fact]
    public void Linq_Filter_WithCanonicalFunction_Int_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var q4 = (from c in _context.People.OfType<Employee>()
                  where c.ManagersPersonId == 47
                  select c) as DataServiceQuery<Employee>;

        // Act & Assert
        Assert.NotNull(q4);
        Assert.EndsWith("odata/People/Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.Employee?$filter=ManagersPersonId eq 47", q4.ToString());

        var ar = q4.BeginExecute(null, null).EnqueueWait(this);
        var value3 = q4.EndExecute(ar).ToList();
        Assert.Single(value3);

        this.EnqueueTestComplete();
    }

    /// <summary>
    /// LINQ - project a primitive property from a single entity into a custom entity
    /// </summary>
    [Fact]
    public void Linq_ProjectIntoCustomEntity_FromSingleEntity_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.CustomerId == -10
                     select new EntityCustomer { Name = c.Name }) as DataServiceQuery<EntityCustomer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customer = query.EndExecute(ar).Single();

                Assert.Equal(0, customer.CustomerId);
                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customer.Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project an EPM property from a feed
    /// </summary>
    [Fact]
    public void Linq_ProjectMappedProperties_FromEntitySet_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.Name != null
                     select new Customer { CustomerId = c.CustomerId }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();

                Assert.All(customers, c => Assert.NotEqual(0, c.CustomerId));
                Assert.All(customers, c => Assert.Null(c.Name));

                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project a navigation property from a feed
    /// </summary>
    [Fact]
    public void Linq_ProjectNavigationProperty_FromEntitySet_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.Wife != null
                     select new Customer { Wife = c.Wife }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar).ToList();
                Assert.Equal(9, customers.Count);
                Assert.All(customers, c => Assert.NotNull(c.Wife));
                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customers.First().Name);
                Assert.Equal("allocatedentitiescontentcontainercurrentsynchronously", customers.Last().Name);

                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project a primitive property from a single entity
    /// </summary>
    [Fact]
    public void Linq_ProjectSingleProperty_FromSingleEntity_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.CustomerId == -10
                     select new Customer { Name = c.Name }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customer = query.EndExecute(ar).Single();

                Assert.Null(customer.CustomerId);
                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customer.Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    [Fact]
    public void Linq_ProjectTwoProperties_FromSingleEntity_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers
                     where c.CustomerId == -10
                     select new Customer { Name = c.Name, CustomerId = c.CustomerId }) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customer = query.EndExecute(ar).Single();

                Assert.Equal(-10, customer.CustomerId);
                Assert.Equal("commastartedtotalnormaloffsetsregisteredgroupcelestialexposureconventionsimportcastclass", customer.Name);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using Skip() an Take()
    /// </summary>
    [Fact]
    public void Linq_SkipAndTake_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Computers.OrderBy(c => c.ComputerId).Skip(1).Take(3) as DataServiceQuery<Computer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var computers = query.EndExecute(ar).ToList();
                Assert.Equal(-9, computers.First().ComputerId);
                Assert.Equal(-7, computers.Last().ComputerId);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using Skip()
    /// </summary>
    [Fact]
    public void Linq_Skip_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Computers.OrderBy(c => c.ComputerId).Skip(1) as DataServiceQuery<Computer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var computers = query.EndExecute(ar).ToList();
                Assert.Equal(-9, computers.First().ComputerId);
                Assert.Equal(-1, computers.Last().ComputerId);

                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ query using Take()
    /// </summary>
    [Fact]
    public void Linq_Take_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Computers.OrderBy(c => c.ComputerId).Take(2) as DataServiceQuery<Computer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var computers = query.EndExecute(ar).ToList();
                Assert.Equal(-10, computers.First().ComputerId);
                Assert.Equal(-9, computers.Last().ComputerId);
                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// LINQ - project two primitive property from a single entity
    /// </summary>
    [Fact]
    public void Linq_Where_WithGreaterThanCondition_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = (from c in _context.Customers where c.CustomerId > 0 select c) as DataServiceQuery<Customer>;

        // Act & Assert
        Assert.NotNull(query);
        query.BeginExecute(
            (ar) =>
            {
                var customers = query.EndExecute(ar);
                Assert.All(customers, c => Assert.True(c.CustomerId > 0));

                this.EnqueueTestComplete();
            },
            null);

        this.WaitForTestToComplete();
    }

    /// <summary>
    /// An expression that compares only the key property, will generate a $filter query option.
    /// </summary>
    [Fact]
    public void Linq_Where_GeneratesFilter_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var query = _context.Customers.Where(c => c.CustomerId == -10);

        // Act & Assert
        var uri = query.ToString();

        Assert.EndsWith("$filter=CustomerId eq -10", uri);
    }

    /// <summary>
    /// Using ByKey, An expression that compares only the key property.
    /// KeyComparisonGeneratesFilterQuery was deprecated in favor of ByKey
    /// </summary>
    [Fact]
    public void Linq_ByKey_GeneratesKeySegment_Test()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

        // Act & Assert
        var query = _context.Customers.ByKey(-10);
        var uri = query.Query.ToString();

        Assert.EndsWith("Customers(-10)", uri);
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "asynchronousquerytests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}



/// <summary>
/// Custom Data Service Entity
/// </summary>
[KeyAttribute("CustomerId")]
public class EntityCustomer
{
    public int CustomerId { get; set; }
    public string? Name { get; set; }
}

/// <summary>
/// A Non Entity Class
/// </summary>
public class NonEntityCustomer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
}

/// <summary>
/// A Non Entity Class
/// </summary>
public class NonEntityCustomer2
{
    public NonEntityCustomer2(int customerId, string name)
    {
        this.CustomerId = customerId;
        this.Name = name;
    }

    public int CustomerId { get; private set; }
    public string Name { get; private set; }
}