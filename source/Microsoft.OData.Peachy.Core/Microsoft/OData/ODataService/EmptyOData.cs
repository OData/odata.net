namespace Microsoft.OData.ODataService
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public sealed partial class Peachy
    {
        public sealed class EmptyOData : IODataService
        {
            private EmptyOData()
            {
            }

            public static EmptyOData Instance { get; } = new EmptyOData();

            public async Task<Stream> GetAsync(string url, Stream request)
            {
                var stream = new MemoryStream(); //// TODO error handling
                await stream.WriteAsync(Encoding.UTF8.GetBytes("TODO this should be a 404")); //// TODO is this the right encoding?
                stream.Position = 0;
                return stream;
            }
        }
    }
}
