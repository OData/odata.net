namespace Microsoft.OData.ODataService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for <see cref="IODataService"/>
    /// </summary>
    public static class ODataExtensions
    {
        public static async Task<Stream> GetAsync(this IODataService odata, string url, IEnumerable<string> headers, string body) //// TODO this extension method is probably not the "ultimate" in structured typing, feel free to throw it away or add other overloads (for example, response codes aren't part of this method signature)
        {
            if (odata == null)
            {
                throw new ArgumentNullException(nameof(odata));
            }

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, -1, true)) //// TODO is this encoding an odata requirement?
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
                return await odata.GetAsync(url, stream).ConfigureAwait(false);
            }
        }
    }
}
