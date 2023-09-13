using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace NextjsStaticHosting.AspNetCore.Internals
{
    internal class NextjsEndpointDataSource : EndpointDataSource
    {
        /// <summary>
        /// Use zero by default so that any conflicts with other default-order ASP .NET Core routes are caught during startup.
        /// </summary>
        private const int DefaultEndpointOrder = 0;

        /// <summary>
        /// Supports dyamic page segments of the form <c>"[param]"</c> and <c>"[...param]"</c>.
        /// This does not currently work with dynamic segments of the form <c>"[[...slug]]"</c>
        /// <see href="https://nextjs.org/docs/routing/dynamic-routes#optional-catch-all-routes">Optional catch all routes</see>.
        /// </summary>
        private static readonly Regex slugRegex = new Regex(@"^\[([^\[\]]+?)\]$");

        private readonly IEndpointRouteBuilder endpointRouteBuilder;
        private readonly StaticFileOptionsProvider staticFileOptionsProvider;
        private readonly List<Action<EndpointBuilder>> conventions;
        private IReadOnlyList<Endpoint> endpoints;

        internal NextjsEndpointDataSource(IEndpointRouteBuilder endpointRouteBuilder, StaticFileOptionsProvider staticFileOptionsProvider)
        {
            this.endpointRouteBuilder = endpointRouteBuilder ?? throw new ArgumentNullException(nameof(endpointRouteBuilder));
            this.staticFileOptionsProvider = staticFileOptionsProvider ?? throw new ArgumentNullException(nameof(staticFileOptionsProvider));

            this.conventions = new List<Action<EndpointBuilder>>();
            this.DefaultBuilder = new ConventionBuilder(this.conventions);
        }

        /// <summary>
        /// Gets a <see cref="IChangeToken"/> used to signal invalidation of cached <see cref="Endpoint"/>
        /// instances.
        /// </summary>
        /// <returns>The <see cref="IChangeToken"/>.</returns>
        public override IChangeToken GetChangeToken() => NullChangeToken.Singleton;

        /// <summary>
        /// Returns a read-only collection of <see cref="Endpoint"/> instances.
        /// </summary>
        public override IReadOnlyList<Endpoint> Endpoints
        {
            get
            {
                this.EnsureInitialized();
                return this.endpoints;
            }
        }

        public IEndpointConventionBuilder DefaultBuilder { get; }

        private void EnsureInitialized()
        {
            if (this.endpoints == null)
            {
                this.endpoints = this.Update();
            }
        }

        private IReadOnlyList<Endpoint> Update()
        {
            const string HtmlExtension = ".html";
            const string CatchAllSlugPrefix = "...";

            var staticFileOptions = this.staticFileOptionsProvider.StaticFileOptions;
            var requestDelegate = CreateRequestDelegate(this.endpointRouteBuilder, staticFileOptions);
            var endpoints = new List<Endpoint>();
            foreach (var filePath in TraverseFiles(staticFileOptions.FileProvider))
            {
                if (!filePath.EndsWith(HtmlExtension))
                {
                    continue;
                }

                var fileWithoutHtml = filePath.Substring(0, filePath.Length - HtmlExtension.Length);
                var patternSegments = new List<RoutePatternPathSegment>();
                var segments = fileWithoutHtml.Split('/');

                // NOTE: Start at 1 because paths here always have a leading slash
                for (int i = 1; i < segments.Length; i++)
                {
                    var segment = segments[i];
                    if (i == segments.Length - 1 && segment == "index")
                    {
                        // Skip `index` segment, match whatever we got so far.
                        // This is so that e.g. file `/a/b/index.html` is served at path `/a/b`, as desired.

                        // TODO: Should we also serve the same file at `/a/b/index`? Note that `/a/b/index.html` will already work
                        // via the UseStaticFiles middleware added by `NextjsStaticHostingExtensions.UseNextjsStaticHosting`.
                        break;
                    }

                    var match = slugRegex.Match(segment);
                    if (match.Success)
                    {
                        string slugName = match.Groups[1].Value;
                        if (slugName.StartsWith(CatchAllSlugPrefix))
                        {
                            // Catch all route -- see: https://nextjs.org/docs/routing/dynamic-routes#catch-all-routes
                            var parameterName = slugName.Substring(CatchAllSlugPrefix.Length);
                            patternSegments.Add(
                                RoutePatternFactory.Segment(
                                    RoutePatternFactory.ParameterPart(parameterName, null, RoutePatternParameterKind.CatchAll)));
                        }
                        else
                        {
                            // Dynamic route -- see: https://nextjs.org/docs/routing/dynamic-routes
                            patternSegments.Add(
                                RoutePatternFactory.Segment(
                                    RoutePatternFactory.ParameterPart(slugName)));
                        }
                    }
                    else
                    {
                        // Literal match
                        patternSegments.Add(
                            RoutePatternFactory.Segment(
                                RoutePatternFactory.LiteralPart(segment)));
                    }
                }
                var endpointBuilder = new RouteEndpointBuilder(requestDelegate, RoutePatternFactory.Pattern(patternSegments), order: DefaultEndpointOrder);

                endpointBuilder.Metadata.Add(new StaticFileEndpointMetadata(filePath));
                endpointBuilder.DisplayName = $"Next.js {filePath}";
                foreach (var convention in this.conventions)
                {
                    convention(endpointBuilder);
                }

                var endpoint = endpointBuilder.Build();
                endpoints.Add(endpoint);
            }

            return endpoints;
        }

        private static IEnumerable<string> TraverseFiles(IFileProvider fileProvider)
        {
            _ = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));

            var outstandingPaths = new Stack<string>();
            outstandingPaths.Push("");

            while (outstandingPaths.Count > 0)
            {
                var path = outstandingPaths.Pop();
                foreach (var entry in fileProvider.GetDirectoryContents(path))
                {
                    var entryPath = path + "/" + entry.Name;
                    if (entry.IsDirectory)
                    {
                        outstandingPaths.Push(entryPath);
                    }
                    else
                    {
                        yield return entryPath;
                    }
                }
            }
        }

        private static RequestDelegate CreateRequestDelegate(IEndpointRouteBuilder endpoints, StaticFileOptions options)
        {
            var app = endpoints.CreateApplicationBuilder();
            app.Use(next => context =>
            {
                var endpoint = context.GetEndpoint();
                var metadata = endpoint?.Metadata.GetMetadata<StaticFileEndpointMetadata>();
                if (metadata == null)
                {
                    throw new InvalidOperationException("Endpoint is missing metadata");
                }

                context.Request.Path = metadata.Path;

                // Set endpoint to null so the static files middleware will handle the request.
                context.SetEndpoint(null);

                return next(context);
            });

            app.UseStaticFiles(options);

            return app.Build();
        }

        internal sealed class StaticFileEndpointMetadata
        {
            public StaticFileEndpointMetadata(string path)
            {
                this.Path = path ?? throw new ArgumentNullException(nameof(path));
            }

            public string Path { get; }
        }

        private class ConventionBuilder : IEndpointConventionBuilder
        {
            private readonly List<Action<EndpointBuilder>> conventions;

            public ConventionBuilder(List<Action<EndpointBuilder>> conventions)
            {
                this.conventions = conventions;
            }

            public void Add(Action<EndpointBuilder> convention)
            {
                this.conventions.Add(convention);
            }
        }
    }
}
