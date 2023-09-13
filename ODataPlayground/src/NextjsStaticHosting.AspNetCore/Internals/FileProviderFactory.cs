using Microsoft.Extensions.FileProviders;

namespace NextjsStaticHosting.AspNetCore.Internals
{
    internal class FileProviderFactory
    {
        public virtual IFileProvider CreateFileProvider(string physicalRoot) => new PhysicalFileProvider(physicalRoot);
    }
}
