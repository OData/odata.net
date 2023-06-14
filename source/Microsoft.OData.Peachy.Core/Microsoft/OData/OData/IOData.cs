namespace Microsoft.OData.OData
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public interface IOData
    {
        Task<Stream> SendAsync(string url, Stream request);
    }
}
