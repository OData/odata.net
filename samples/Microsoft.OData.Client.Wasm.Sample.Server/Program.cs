//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.OData;
using Microsoft.OData.Client.Wasm.Sample.Server.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services);

var app = builder.Build();

ConfigureMiddleware(app);

app.Run();

static void ConfigureServices(IServiceCollection services)
{
    var model = BuildEdmModel();

    // Add services
    services.AddControllers().AddOData(options =>
    {
        options.EnableQueryFeatures().AddRouteComponents(
            routePrefix: "odata",
            model: model,
            configureServices: (nestedServices) =>
            {
                nestedServices.AddSingleton<ODataBatchHandler, DefaultODataBatchHandler>();
                nestedServices.AddSingleton(_ => new ODataMessageReaderSettings
                {
                    EnableMessageStreamDisposal = false,
                    Version = ODataVersion.V401,
                    MaxProtocolVersion = ODataVersion.V401
                });
            });
    });

    // Add CORS policy
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    });
}

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseODataRouteDebug();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    // Enable HTTPS redirection
    app.UseHttpsRedirection();

    // Serve Blazor WebAssembly static files
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    // Enable OData batching
    app.UseODataBatching();

    // Configure routing
    app.UseRouting();

    // Apply CORS policy
    app.UseCors("AllowAll");

    // Map API controllers
    app.MapControllers();

    // Fallback to Blazor SPA for client-side routing
    app.MapFallbackToFile("index.html");
}

static IEdmModel BuildEdmModel()
{
    var modelBuilder = new ODataConventionModelBuilder();

    var customersEntitySetConfiguration = modelBuilder.EntitySet<Customer>("Customers");
    customersEntitySetConfiguration.EntityType.Collection.Function("GetTopCustomer")
        .ReturnsFromEntitySet<Customer>("Customers");

    var ordersEntitySetConfiguration = modelBuilder.EntitySet<Order>("Orders");
    ordersEntitySetConfiguration.EntityType.Collection.Function("GetTop2Orders")
        .ReturnsCollectionFromEntitySet<Order>("Orders");
    var applyDiscountAction = ordersEntitySetConfiguration.EntityType.Action("ApplyDiscount");
    applyDiscountAction.Parameter<decimal>("discountPercentage");
    applyDiscountAction.Returns<decimal>();

    modelBuilder.EntitySet<MediaAsset>("Media");

    modelBuilder.Action("ResetDataSource");

    return modelBuilder.GetEdmModel();
}
