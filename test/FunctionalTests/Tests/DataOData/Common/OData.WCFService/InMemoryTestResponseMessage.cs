//---------------------------------------------------------------------
// <copyright file="InMemoryTestResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel.Web;
    using System.Threading.Tasks;
    using Microsoft.OData;

    /// <summary>
    /// Internal implementation of IODataResponseMessage which uses a memory stream to write out the results to.
    /// TODO: use an actual type inherited from Message instead of this class
    /// </summary>
    public class InMemoryTestResponseMessage : IODataResponseMessage
    {
        private Stream stream;

        /// <summary>
        /// Creates or Initializes an instance of the InMemoryTestResponseMessage Type
        /// </summary>
        /// <param name="memoryStream">the stream to write out the response to</param>
        /// <param name="statusCode">the status code of the current response</param>
        public InMemoryTestResponseMessage(Stream memoryStream, int statusCode)
        {
            this.stream = memoryStream;
            this.StatusCode = statusCode;
        }

        private WebHeaderCollection WCFHeaderCollection
        {
            get
            {
                return WebOperationContext.Current.OutgoingResponse.Headers;
            }
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                List<KeyValuePair<string, string>> headersLookup = new List<KeyValuePair<string, string>>();
                foreach (var header in this.WCFHeaderCollection.AllKeys)
                {
                    headersLookup.Add(new KeyValuePair<string, string>(header, this.WCFHeaderCollection[header]));
                }

                return headersLookup;
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        public string GetHeader(string headerName)
        {
            if (this.WCFHeaderCollection.AllKeys.Contains(headerName))
            {
                return this.WCFHeaderCollection[headerName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public Stream GetStream()
        {
            return this.stream;
        }

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            this.WCFHeaderCollection[headerName] = headerValue;
        }

        /// <summary>
        /// The result status code of the response message.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return (int)WebOperationContext.Current.OutgoingResponse.StatusCode;
            }
            set
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = (HttpStatusCode)value;
            }
        }

        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public Task<Stream> GetStreamAsync()
        {
            TaskCompletionSource<Stream> completionSource = new TaskCompletionSource<Stream>();
            completionSource.SetResult(this.stream);
            return completionSource.Task;
        }
    }

}