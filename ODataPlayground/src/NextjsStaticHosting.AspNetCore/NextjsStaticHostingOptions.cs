using System;

namespace NextjsStaticHosting.AspNetCore
{
    /// <summary>
    /// Options for hosting exported Next.js client-side applications on ASP .NET Core.
    /// </summary>
    public class NextjsStaticHostingOptions
    {
        /// <summary>
        /// Relative path from the app's content path where the Next.js client app binaries are stored.
        /// </summary>
        /// <remarks>
        /// Usually you should not provide a leading slash in this value.
        /// This is used with <see cref="System.IO.Path.Combine(string, string)"/>
        /// with <see cref="Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath"/>
        /// as the base path.
        /// Specifying a rooted path here (e.g. <c>/foo</c>) is usually undesirable and may lead to security issues
        /// (i.e., the resulting physical path would be <c>DRIVE:\foo</c> because of the leading slash).
        /// </remarks>
        public string RootPath { get; set; }

        /// <summary>
        /// Uri to the Next.js dev server. Only applicable when <see cref="ProxyToDevServer"/> is true.
        /// </summary>
        public string DevServer { get; set; }

        /// <summary>
        /// Whether to proxy to a Next.js dev server instead of hosting static exported filed.
        /// </summary>
        public bool ProxyToDevServer { get; set; }
    }
}
