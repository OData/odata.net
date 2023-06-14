namespace Microsoft.OData.OData
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IOData"/>
    /// </summary>
    public static class ODataExtensions
    {
        public static async Task<Stream> SendAsync(this IOData odata, string url, IEnumerable<string> headers, string body)
        {
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8)) //// TODO is this encoding an odata requirement?
                {
                    foreach (var header in headers)
                    {
                        await streamWriter.WriteLineAsync(header).ConfigureAwait(false);
                    }

                    await streamWriter.WriteLineAsync().ConfigureAwait(false);
                    await streamWriter.WriteAsync(body).ConfigureAwait(false);
                }

                //// TODO why do we have to wait to write everything to the stream before sending? implement a different stream so we don't have to wait
                stream.Position = 0;
                return await odata.SendAsync(url, stream).ConfigureAwait(false);
            }
        }
    }
}
