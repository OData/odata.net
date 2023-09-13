using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace NextjsStaticHosting.AspNetCore.Internals
{
    /// <summary>
    /// Caches an instance of <see cref="StaticFileOptions"/>
    /// so that we can re-use the same <see cref="IFileProvider"/>
    /// in <see cref="NextjsEndpointDataSource"/> and
    /// <see cref="NextjsStaticHostingExtensions.UseNextjsStaticHosting(IApplicationBuilder)"/>.
    /// </summary>
    internal class StaticFileOptionsProvider
    {
        public StaticFileOptionsProvider(IWebHostEnvironment env, FileProviderFactory fileProviderFavtory, IOptions<NextjsStaticHostingOptions> options)
        {
            _ = env ?? throw new ArgumentNullException(nameof(env));
            _ = fileProviderFavtory ?? throw new ArgumentNullException(nameof(fileProviderFavtory));
            var o = options?.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(o.RootPath))
            {
                throw new InvalidOperationException($"{nameof(NextjsStaticHostingOptions)}.{nameof(NextjsStaticHostingOptions.RootPath)} must be specified.");
            }

            string physicalRoot = Path.Combine(env.ContentRootPath, o.RootPath);
            var fileProvider = fileProviderFavtory.CreateFileProvider(physicalRoot);
            this.StaticFileOptions = new StaticFileOptions
            {
                FileProvider = fileProvider,
            };
        }

        public StaticFileOptions StaticFileOptions { get; }
    }
}
