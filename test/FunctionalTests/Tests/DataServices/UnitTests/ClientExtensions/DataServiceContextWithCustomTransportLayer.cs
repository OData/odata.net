//---------------------------------------------------------------------
// <copyright file="DataServiceContextWithCustomTransportLayer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.ClientExtensions
{
    #region Namespaces

    using System;
    using Microsoft.OData.Client;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.OData;

    #endregion Namespaces

    /// <summary>
    /// Provides support to make tests reusable across in-memory, local and remote web servers.
    /// </summary>
    public class DataServiceContextWithCustomTransportLayer : DataServiceContext
    {
        public static Uri DummyUri = new Uri("http://SomeDummyUri/myService.svc");
        private readonly Func<IODataResponseMessage> getResponseMessage;
        private readonly Func<IODataRequestMessage> getRequestMessage;

        public DataServiceContextWithCustomTransportLayer(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
            : this(ODataProtocolVersion.V4, requestMessage, responseMessage)
        {
        }

        public DataServiceContextWithCustomTransportLayer(ODataProtocolVersion protocolVersion, IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
            : this(protocolVersion, () => requestMessage, () => responseMessage)
        {
        }

        /// <summary>
        /// Initializes a new TestWebRequest instance with simple defaults 
        /// that have no side-effects on querying.
        /// </summary>
        public DataServiceContextWithCustomTransportLayer(ODataProtocolVersion maxProtocolVersion, Func<IODataRequestMessage> getRequestMessage, Func<IODataResponseMessage> getResponseMessage)
            : this(DummyUri, maxProtocolVersion, getRequestMessage, getResponseMessage)
        {
        }

        /// <summary>
        /// Initializes a new TestWebRequest instance with simple defaults 
        /// that have no side-effects on querying.
        /// </summary>
        public DataServiceContextWithCustomTransportLayer(Uri serviceUri, ODataProtocolVersion maxProtocolVersion, Func<IODataRequestMessage> getRequestMessage, Func<IODataResponseMessage> getResponseMessage)
            : base(serviceUri, maxProtocolVersion)
        {
            this.getRequestMessage = getRequestMessage;
            this.getResponseMessage = getResponseMessage;

            this.Configurations.RequestPipeline.OnMessageCreating = this.CreateMessage;
        }

        /// <summary>
        /// Creates an instance of the IODataRequestMessage 
        /// </summary>
        /// <param name="requestMessageArgs">Arguments for creating the request message.</param>
        /// <returns>an instance of the IODataRequestMessage </returns>
        internal DataServiceClientRequestMessage CreateMessage(DataServiceClientRequestMessageArgs requestMessageArgs)
        {
            var requestMessage = this.getRequestMessage();
            requestMessage.Url = requestMessageArgs.RequestUri;
            requestMessage.Method = requestMessageArgs.Method;
            
            foreach (var header in requestMessageArgs.Headers)
            {
                requestMessage.SetHeader(header.Key, header.Value);
            }

            return new TestDataServiceClientRequestMessage(requestMessage, getResponseMessage);
        }
    }
}
