namespace Microsoft.OData.ODataService
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: ? <threadsafety static="true" instance="true"/>
    /// </summary>
    /// <remarks>
    /// The <see href="https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_Introduction">OData standard</see> defines the protocol as being RESTful using HTTP:
    /// > ...REST-based data services...using simple HTTP messages
    /// </remarks>
    public interface IODataService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">url or request</exception>
        Task<ODataResponse> GetAsync(ODataRequest request);
    }

    public sealed class ODataRequest
    {
        public ODataRequest(string url, IEnumerable<string> headers, Stream body)
        {
            this.Url = url;
            this.Headers = headers;
            this.Body = body;
        }

        public string Url { get; }

        public IEnumerable<string> Headers { get; }

        public Stream Body { get; }
    }

    public sealed class ODataResponse
    {
        public ODataResponse(int statusCode, IEnumerable<string> headers, Stream body)
        {
            this.StatusCode = statusCode;
            this.Headers = headers;
            this.Body = body;
        }

        public int StatusCode { get; }

        public IEnumerable<string> Headers { get; }

        public Stream Body { get; }
    }
}
