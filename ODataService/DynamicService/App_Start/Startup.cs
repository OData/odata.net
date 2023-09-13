// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Restier.Core;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Restier.AspNetCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Restier.Core.Submit;
using Microsoft.Restier.Core.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.OData.Edm;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNet.OData;

namespace DynamicService
{
    public class Startup
    {
        private const string routeName = "DynamicApi";
        private const string corsPolicy = "_allowAllCORS";
        internal const string serviceName = "ourService";

        /// <summary>
        /// The application configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configures the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddScoped<IEdmModel>(sp => 
            //    ((DynamicModelBuilder)sp.GetRequiredService<IModelBuilder>()).GetModel(null)
            //);
            services.AddRestier((builder) =>
            {
                // This delegate is executed after OData is added to the container.
                // Add replacement services here.
                builder.AddRestierApi<DynamicApi>(routeServices =>
                {
                    routeServices
                        .AddSingleton(new ODataValidationSettings
                        {
                            MaxAnyAllExpressionDepth = 10,
                            MaxExpansionDepth = 10,
                        })
                        .AddScoped<ODataMessageWriterSettings>((sp) =>
                        new ODataMessageWriterSettings
                        {
                            Version = ODataVersion.V401,
                            BaseUri = new Uri("http://" + serviceName, UriKind.Absolute),
                        })

                        // omit @odata prefixes
                        .AddSingleton<ODataSimplifiedOptions>((sp) =>
                        {
                            ODataSimplifiedOptions simplifiedOptions = new ODataSimplifiedOptions();
                            simplifiedOptions.SetOmitODataPrefix(true);
                            return simplifiedOptions;
                        })
                        .AddScoped<ODataUriResolver>((serviceProvider) =>
                            new UnqualifiedODataUriResolver
                            {
                                EnableNoDollarQueryOptions = true,
                                EnableCaseInsensitive = true,
                            })
                        .AddSingleton<IChangeSetInitializer, DataSourceManager.Submit.ChangeSetInitializer<DynamicApi>>()
                        .AddSingleton<ISubmitExecutor, DataSourceManager.Submit.SubmitExecutor>()
                        .AddScoped<IEdmModel>(sp =>
                           ((DynamicModelBuilder)sp.GetRequiredService<IModelBuilder>()).GetModel(null)
                        )
                        .AddChainedService<IModelBuilder, DynamicModelBuilder>()
                      //                    .AddSingleton<DataSourceManager.DataStoreManager.IDataStoreManager<string, DynamicApi>>(new DataSourceManager.DataStoreManager.SingleDataStoreManager<string, DynamicApi>())
                        ;
                });
            });

            services.AddControllers(options => options.EnableEndpointRouting = false);

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = false;
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: corsPolicy,
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    });
            });
        }

        /// <summary>
        /// Configures the application and the HTTP Request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(corsPolicy);

            app.UseSession();

//            app.UseDynamicServiceMiddleware();

            app.UseMvc(builder =>
            {
                builder.Select().Expand().Filter().OrderBy().MaxTop(100).Count().SetTimeZoneInfo(TimeZoneInfo.Utc);

                builder.MapRestier(builder =>
                {
                    builder.MapApiRoute<DynamicApi>(routeName, "", true);
                });
            });

            //todo: remove when we have a real storage location for csdl
            AppDomain.CurrentDomain.SetData("ContentRootPath", env.ContentRootPath + @"\App_Data");
        }
    }

    /// <summary>
    /// DynamicService request handling
    /// </summary>
    public class DynamicServiceMiddleware
    {
        private RequestDelegate _next;
        public DynamicServiceMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // set the host to be Jetsons
            context.Request.Host = new HostString(Startup.serviceName);

            // set the default format to be application/json
            string accepts = "application/json";
            StringValues acceptValues;
            if (context.Request.Headers.TryGetValue("Accept", out acceptValues)  && acceptValues.Count > 0)
            {
                accepts = acceptValues[0].Replace("*/*", "application/json").Replace("application/*", "application/json");
                foreach (string accept in accepts.Split(','))
                {
                    if (accept.StartsWith("application/json") | accept.StartsWith("application/xml"))
                    {
                        break;
                    }
                }
            }

            context.Request.Headers["Accept"] = accepts;

            // attempts at geting the service container to inject IEdmModel service
//            IPerRouteContainer perRouteContainer = context.RequestServices.GetRequiredService<IPerRouteContainer>();
//            var sp = perRouteContainer.GetODataRootContainer(routeName);
 
            await _next.Invoke(context);
        }
    }

    /// <summary>
    /// Extension method for registering DynamicServiceMiddleware
    /// </summary>    
    public static class DynamicServiceMiddlewareExtensions
    {
        public static IApplicationBuilder UseDynamicServiceMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DynamicServiceMiddleware>();
        }
    }
}
