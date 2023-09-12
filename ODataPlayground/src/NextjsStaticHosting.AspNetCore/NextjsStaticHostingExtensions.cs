using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NextjsStaticHosting.AspNetCore.Internals;

namespace NextjsStaticHosting.AspNetCore
{
    /// <summary>
    /// Extension method to add support for hosting a Next.js statically generated client-side app on ASP .NET Core.
    /// </summary>
    public static class NextjsStaticHostingExtensions
    {
        /// <summary>
        /// Adds necessary dependencies to the DI container.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// public void ConfigureServices(IServiceCollection services)
        /// {
        ///     services.AddNextjsStaticHosting(options => options.RootPath = "wwwroot/clientapp");
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static void AddNextjsStaticHosting(this IServiceCollection services, Action<NextjsStaticHostingOptions> configureAction = null)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));

            if (configureAction != null)
            {
                services.Configure(configureAction);
            }

            services.AddSingleton<FileProviderFactory>();
            services.AddSingleton<StaticFileOptionsProvider>();

            // Add YARP (used only when `NextjsStaticHostingOptions.ProxyToDevServer` is true).
            // See also: `ProxyToDevServerMiddleware`.
            services.AddHttpForwarder();
        }

        /// <summary>
        /// Registers endpoints for Next.js pages found in the configured <see cref="NextjsStaticHostingOptions.RootPath"/>.
        /// This ensures that the correct static Next.js pages will be served at the right paths.
        /// <para>
        ///   For example, say your client application is composed of the following files:
        ///   <list type="bullet">
        ///     <item><c>/post/create.html</c></item>
        ///     <item><c>/post/[pid].html</c></item>
        ///     <item><c>/post/[...slug].html</c></item>
        ///   </list>
        /// </para>
        /// <para>
        ///   The following routes will be created accordingly:
        ///   <list type="bullet">
        ///     <item><c>post/create</c>, serving file <c>/post/create.html</c></item>
        ///     <item><c>post/{pid}</c>, serving file <c>/post/[pid].html</c></item>
        ///     <item><c>post/{*slug}</c>, serving file <c>/post/[...slug].html</c></item>
        ///   </list>
        /// </para>
        /// <para>
        /// ASP .NET Core Endpoint Routing built-in route precedence rules ensure the same semantics as Next.js expects will apply.
        /// E.g., <c>post/create</c> has higher precedence than <c>post/{pid}</c>, which in turn has higher precedence than <c>post/{*slug}</c>.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// public void Configure(IApplicationBuilder app)
        /// {
        ///     app.UseRouting();
        ///     app.UseEndpoints(endpoints =>
        ///     {
        ///         endpoints.MapNextjsStaticHtmls();
        ///     });
        /// 
        ///     app.UseNextjsStaticHosting();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IEndpointConventionBuilder MapNextjsStaticHtmls(this IEndpointRouteBuilder endpoints)
        {
            _ = endpoints ?? throw new ArgumentNullException(nameof(endpoints));

            var options = endpoints.ServiceProvider.GetRequiredService<IOptions<NextjsStaticHostingOptions>>();
            if (options.Value.ProxyToDevServer)
            {
                var logger = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("NextjsStaticHostingExtensions");
                logger.LogInformation($"{nameof(NextjsStaticHostingExtensions)} was configured with {nameof(NextjsStaticHostingOptions.ProxyToDevServer)}, skipping dynamic endpoint configuration.");
                return new NullEndpointConventionBuilder();
            }

            var staticFileOptionsProvider = endpoints.ServiceProvider.GetRequiredService<StaticFileOptionsProvider>();
            var dataSource = new NextjsEndpointDataSource(endpoints, staticFileOptionsProvider);
            endpoints.DataSources.Add(dataSource);
            return dataSource.DefaultBuilder;
        }

        /// <summary>
        /// Registers a static files middleware that will serve files from the configured <see cref="NextjsStaticHostingOptions.RootPath"/>.
        /// This is required to serve non-html files including JavaScript, CSS, and any other artifacts produced by Next.js export outputs.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// public void Configure(IApplicationBuilder app)
        /// {
        ///     app.UseRouting();
        ///     app.UseEndpoints(endpoints =>
        ///     {
        ///         endpoints.MapNextjsStaticHtmls();
        ///     });
        /// 
        ///     app.UseNextjsStaticHosting();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static void UseNextjsStaticHosting(this IApplicationBuilder app)
        {
            _ = app ?? throw new ArgumentNullException(nameof(app));

            var options = app.ApplicationServices.GetRequiredService<IOptions<NextjsStaticHostingOptions>>();
            if (options.Value.ProxyToDevServer)
            {
                app.UseMiddleware<ProxyToDevServerMiddleware>();
                return;
            }

            var staticFileOptionsProvider = app.ApplicationServices.GetRequiredService<StaticFileOptionsProvider>();
            app.UseStaticFiles(staticFileOptionsProvider.StaticFileOptions);
        }

        private class NullEndpointConventionBuilder : IEndpointConventionBuilder
        {
            public void Add(Action<EndpointBuilder> convention)
            {
            }
        }
    }
}
