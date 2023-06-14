namespace Microsoft.OData.ODataService
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO: ? <threadsafety static="true" instance="true"/>
    /// </summary>
    public interface IODataService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">url or request</exception>
        Task<Stream> GetAsync(string url, Stream request); //// TODO it actually *must* be http, so *that* should be in the interface, and an extension for just a stream shuold be made
    }
}
