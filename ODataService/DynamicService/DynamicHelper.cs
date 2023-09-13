using DataSourceGenerator;
using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Restier.AspNetCore;
using Microsoft.Restier.AspNetCore.Batch;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Core.Query;
using Microsoft.Restier.Core.Submit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DynamicService
{
#if false
    public static class DynamicHelper
    {
        //private const string MapRestierRouteMethod = "MapRestierRoute";
        //private const string httpConfigurationExtensionsType = "System.Web.Http.HttpConfigurationExtensions";
        private const string DynamicApiType = "Microsoft.OData.Service.ApiAsAService.Api.DynamicApi`1";
        private const string DynamicHelperType = "Microsoft.OData.Service.ApiAsAService.DynamicHelper";
        //private static Type dynamicApiType = Assembly.GetExecutingAssembly().GetType(DynamicApiType);
        //private static Type dynamicHelperType = Assembly.GetExecutingAssembly().GetType(DynamicHelperType);

        //public static ApiBase CreateEntifyFrameworkApi(Type tApi, IServiceProvider serviceProvider)
        //{
        //    return (ApiBase)typeof(EntityFrameworkApi<>).MakeGenericType(tApi).GetConstructor(new Type[] { typeof(IServiceProvider) }).Invoke(new object[] { serviceProvider });
        //}

        /// <summary>
        /// Maps the API routes to the RestierController.
        /// </summary>
        /// <typeparam name="TApi">The user API.</typeparam>
        /// <param name="config">The <see cref="HttpConfiguration"/> instance.</param>
        /// <param name="routeName">The name of the route.</param>
        /// <param name="routePrefix">The prefix of the route.</param>
        /// <param name="batchHandler">The handler for batch requests.</param>
        /// <returns>The task object containing the resulted <see cref="ODataRoute"/> instance.</returns>
        public static Task<ODataRoute> MapRestierRoute<TApi>(
            HttpConfiguration config,
            string routeName,
            string routePrefix,
            RestierBatchHandler batchHandler = null)
            where TApi : ApiBase
        {
            // This will be added a service to callback stored in ApiConfiguration
            // Callback is called by ApiBase.AddApiServices method to add real services.
            ApiBase.AddPublisherServices(typeof(TApi), services =>
            {
                services.AddScoped<ApiFactory>(sp => new ApiFactory(sp));
                MapEfServices(services);
                services.AddODataServices<TApi>();
            });

            IContainerBuilder func() => new RestierContainerBuilder(typeof(TApi));
            config.UseCustomContainerBuilder(func);

            var conventions = CreateRestierRoutingConventions(config, routeName);
            if (batchHandler != null)
            {
                batchHandler.ODataRouteName = routeName;
            }

            void configureAction(IContainerBuilder builder) => builder
                .AddService<IEnumerable<IODataRoutingConvention>>(ServiceLifetime.Singleton, sp => conventions)
                .AddService<ODataBatchHandler>(ServiceLifetime.Singleton, sp => batchHandler);

            var route = GenerateODataServiceRoute(config, routeName, routePrefix, configureAction);
            return Task.FromResult(route);
        }

        /// <summary>
        /// Creates the default routing conventions.
        /// </summary>
        /// <param name="config">The <see cref="HttpConfiguration"/> instance.</param>
        /// <param name="routeName">The name of the route.</param>
        /// <returns>The routing conventions created.</returns>
        internal static IList<IODataRoutingConvention> CreateRestierRoutingConventions(
            HttpConfiguration config, string routeName)
        {
            var conventions = ODataRoutingConventions.CreateDefaultWithAttributeRouting(routeName, config);
            var index = 0;
            for (; index < conventions.Count; index++)
            {
                if (conventions[index] is AttributeRoutingConvention attributeRouting)
                {
                    break;
                }
            }

            conventions.Insert(index + 1, RestierRoutingConventionFactory.Create());
            return conventions;
        }

        /// <summary>
        /// Maps the specified OData route and the OData route attributes.
        /// </summary>
        /// <param name="configuration">The server configuration.</param>
        /// <param name="routeName">The name of the route to map.</param>
        /// <param name="routePrefix">The prefix to add to the OData route's path template.</param>
        /// <param name="configureAction">The configuring action to add the services to the root container.</param>
        /// <returns>The added <see cref="ODataRoute"/>.</returns>
        public static ODataRoute GenerateODataServiceRoute(HttpConfiguration configuration, string routeName,
            string routePrefix, Action<IContainerBuilder> configureAction)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (routeName == null)
            {
                throw new ArgumentNullException("routeName");
            }

            // 1) Build and configure the root container.
            IServiceProvider rootContainer = configuration.CreateODataRootContainer(routeName, configureAction);

            // 2) Resolve the path handler and set URI resolver to it.
            IODataPathHandler pathHandler = rootContainer.GetRequiredService<IODataPathHandler>();

            // if settings is not on local, use the global configuration settings.
            if (pathHandler != null && pathHandler.UrlKeyDelimiter == null)
            {
                ODataUrlKeyDelimiter urlKeyDelimiter = configuration.GetUrlKeyDelimiter();
                pathHandler.UrlKeyDelimiter = urlKeyDelimiter;
            }

            // 3) Resolve some required services and create the route constraint.
            ODataPathRouteConstraint routeConstraint = new ODataPathRouteConstraint(routeName);

            // Attribute routing must initialized before configuration.EnsureInitialized is called.
            rootContainer.GetServices<IODataRoutingConvention>();

            // 4) Resolve HTTP handler, create the OData route and register it.
            ODataRoute route;
            HttpRouteCollection routes = configuration.Routes;
            routePrefix = RemoveTrailingSlash(routePrefix);
            HttpMessageHandler messageHandler = rootContainer.GetService<HttpMessageHandler>();
            if (messageHandler != null)
            {
                route = new ODataRoute(
                    routePrefix,
                    routeConstraint,
                    defaults: null,
                    constraints: null,
                    dataTokens: null,
                    handler: messageHandler);
            }
            else
            {
                ODataBatchHandler batchHandler = rootContainer.GetService<ODataBatchHandler>();
                if (batchHandler != null)
                {
                    batchHandler.ODataRouteName = routeName;
                    string batchTemplate = String.IsNullOrEmpty(routePrefix) ? ODataRouteConstants.Batch
                        : routePrefix + '/' + ODataRouteConstants.Batch;
                    routes.MapHttpBatchRoute(routeName + "Batch", batchTemplate, batchHandler);
                }

                route = new ODataRoute(routePrefix, routeConstraint);
            }

            return route;
        }

        private static string RemoveTrailingSlash(string routePrefix)
        {
            if (!String.IsNullOrEmpty(routePrefix))
            {
                int prefixLastIndex = routePrefix.Length - 1;
                if (routePrefix[prefixLastIndex] == '/')
                {
                    // Remove the last trailing slash if it has one.
                    routePrefix = routePrefix.Substring(0, routePrefix.Length - 1);
                }
            }
            return routePrefix;
        }

        private static IServiceCollection MapEfServices(IServiceCollection services)
        {
            services.AddScoped<DbContext>(sp =>
            {
                var modelType = sp.GetRequiredService<ApiFactory>().ModelType;
                DbContext dbContext = Activator.CreateInstance(modelType) as DbContext;
#if EF7
                dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
#else
                dbContext.Configuration.ProxyCreationEnabled = false;
#endif
                return dbContext;
            });

            return services
                .AddService<IModelBuilder, ModelProducer>()
                .AddService<IModelMapper>((sp, next) => new ModelMapper(sp.GetRequiredService<ApiFactory>().ModelType))
                .MakeScoped<IModelMapper>()
                .AddService<IQueryExpressionSourcer, QueryExpressionSourcer>()
                .AddService<IQueryExecutor, QueryExecutor>()
                .AddService<IQueryExpressionProcessor, QueryExpressionProcessor>()
                .AddService<IChangeSetInitializer, ChangeSetInitializer>()
                .AddService<ISubmitExecutor, SubmitExecutor>()
                ;
        }

        public static Type GetDynamicDbContext(string csdlFile)
        {
            string name = csdlFile.Split('\\').Last().Replace(".xml", "");
            DbContextGenerator generator = new DbContextGenerator();
            return generator.GenerateDbContext(csdlFile);
        }

        public static Type GetDynamicDbContextInADifferentAppDomain(string csdlFile)
        {
            string name = csdlFile.Split('\\').Last().Replace(".xml", "");

            // Create a new AppDomain and execute the generation in that AppDomain
            AppDomain currentDomain = AppDomain.CurrentDomain;
            var newDomain = AppDomain.CreateDomain(name, null, AppDomain.CurrentDomain.SetupInformation);

            // Pass this app domain to the new one
            newDomain.SetData("domain", currentDomain);
            var program = new DbContextGenerator();

            // Set the name of the csdl file. Could also be done with newDomain.SetData()
            program.csdlFileName = csdlFile;

            // Create the Assembly in the new AppDomain
            newDomain.DoCallBack(new CrossAppDomainDelegate(program.GenerateDbContextInANewAppDomain));
 
            // Various attempts passing the type back to this app domain; none work
            // string assemblies = String.Concat(newDomain.GetAssemblies().Select(a=>a.GetName().Name));
            // Assembly assembly = newDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName.Name);
            //Type result = program.DbContextType;
            //return Result.DbContextType;
            //return program.DbContextType;
            object resultType = AppDomain.CurrentDomain.GetData("contextType");
            return resultType as Type;
        }
    }
#endif
}