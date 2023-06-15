namespace Microsoft.OData.ODataService
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class EmptyODataService : IODataService
    {
        private EmptyODataService()
        {
        }

        public static EmptyODataService Instance { get; } = new EmptyODataService();

        public async Task<ODataResponse> GetAsync(ODataRequest request)
        {
            var stream = new MemoryStream(); //// TODO error handling
            await stream.WriteAsync(Encoding.UTF8.GetBytes("TODO this portion of the OData standard has not been implemented")); //// TODO is this the right encoding?
            stream.Position = 0;
            return new ODataResponse(501, Enumerable.Empty<string>(), stream);
        }
    }
}
