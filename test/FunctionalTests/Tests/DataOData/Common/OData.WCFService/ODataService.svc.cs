//---------------------------------------------------------------------
// <copyright file="ODataService.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;

    /// <summary>
    /// The class implements an OData service using WCF as the host
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class ODataService : IODataService
    {
        private IEdmModel model;
        private InMemoryDataSource currentDataSource;

        /// <summary>
        /// The EDM metadata model of the service.
        /// </summary>
        public IEdmModel Model
        {
            get { return this.model ?? (this.model = new InMemoryModel().GetModel()); }
        }

        /// <summary>
        /// The entity sets of the default container.
        /// </summary>
        public IEnumerable<IEdmEntitySet> EntitySets
        {
            get { return this.Model.EntityContainer.Elements.OfType<IEdmEntitySet>(); }
        }

        /// <summary>
        /// The default entity container (only a single container is supported).
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.model.EntityContainer; }
        }

        /// <summary>
        /// The current <see cref="ObjectContext"/> instance.
        /// </summary>
        public InMemoryDataSource CurrentDataSource
        {
            get { return this.currentDataSource ?? (this.currentDataSource = new InMemoryDataSource()); }
        }

        /// <summary>
        /// This is the entry point into the OData server which processes query requests for feed or entry data.
        /// </summary>
        /// <param name="requestUri">The request URI to process and return results for.</param>
        /// <returns>A stream containing the results of parsing the <paramref name="requestUri"/>against the data store and return results for the same.</returns>
        [WebGet(UriTemplate = "/OData/{requestUri}")]
        public Stream GetFeedOrEntry(string requestUri)
        {
            return new QueryHandler { Model = this.Model, DataContext = new DataContext(this.CurrentDataSource) }.ProcessGetQuery(requestUri);
        }

        /// <summary>
        /// This is the entry point into the OData server which processes query requests for property data.
        /// </summary>
        /// <param name="uriPart1">The entry part of the request URI.</param>
        /// <param name="uriPart2">The property part of the request URI.</param>
        /// <returns>A stream containing the results of parsing the query against the data store and return results for the same.</returns>
        [WebGet(UriTemplate = "/OData/{uriPart1}/{uriPart2}")]
        public Stream GetProperty(string uriPart1, string uriPart2)
        {
            return new QueryHandler { Model = this.Model, DataContext = new DataContext(this.CurrentDataSource) }.ProcessGetQuery(uriPart1 + "/" + uriPart2);
        }

        /// <summary>
        /// This is the entry point into the OData server which returns the Service Document.
        /// </summary>
        /// <returns>A stream containing the Service Document based on the sets exposed by the backing data store.</returns>
        [WebGet(UriTemplate = "/OData/")]
        public Stream GetServiceDocument()
        {
            return new ServiceDocumentHandler { DataContext = new DataContext(this.CurrentDataSource), Model = this.Model }.ProcessServiceDocumentRequest();
        }

        /// <summary>
        /// This is the entry point into the OData server which returns the OData Metadata document.
        /// </summary>
        /// <returns>A stream containing the Metadata document based on the sets and types exposed by the backing data store.</returns>
        [WebGet(UriTemplate = "/OData/$metadata")]
        public Stream GetMetadataDocument()
        {
            return new MetadataDocumentHandler { DataContext = new DataContext(this.CurrentDataSource), Model = this.Model }.ProcessMetadataRequest();
        }

        /// <summary>
        /// This is the entry point into the OData server for requests to create a new entry.
        /// </summary>
        /// <param name="messageBody">Stream containing the new entry to insert into the backing data store.</param>
        /// <param name="requestUri">The request URI to parse for the target entity set.</param>
        /// <returns>If successful, a stream containg the new entry. Otherwise, an error.</returns>
        [WebInvoke(UriTemplate = "/OData/{requestUri}", Method = "POST")]
        public Stream CreateEntry(Stream messageBody, string requestUri)
        {
            return new CreateHandler { Model = this.Model, DataContext = new DataContext(this.CurrentDataSource) }.ProcessCreateRequest(messageBody);
        }

        /// <summary>
        /// This is the entry point into the OData server for requests to update an existing entry.
        /// </summary>
        /// <param name="messageBody">The body of the message, containing the updated entry values.</param>
        /// <param name="requestUri">The request URI to parse for the target entry.</param>
        /// <returns>If successful, a stream containg the updated entry. Otherwise, an error.</returns>
        [WebInvoke(UriTemplate = "/OData/{requestUri}", Method = "PUT")]
        public Stream UpdateEntry(Stream messageBody, string requestUri)
        {
            return new UpdateHandler { DataContext = new DataContext(this.CurrentDataSource), Model = this.Model }.ProcessEntryUpdateRequest(messageBody);
        }

        /// <summary>
        /// This is the entry point into the OData server for requests to update a property value.
        /// </summary>
        /// <param name="messageBody">The body of the message, containing the updated property value.</param>
        /// <param name="uriPart1">The entry part of the request URI.</param>
        /// <param name="uriPart2">The property part of the request URI.</param>
        /// <returns>If successful, a stream containg the updated entry. Otherwise, an error.</returns>
        [WebInvoke(UriTemplate = "/OData/{uriPart1}/{uriPart2}", Method = "PUT")]
        public Stream UpdateProperty(Stream messageBody, string uriPart1, string uriPart2)
        {
            return new UpdateHandler { DataContext = new DataContext(this.CurrentDataSource), Model = this.Model }.ProcessTopLevelPropertyRequest(messageBody, uriPart2);
        }

        /// <summary>
        /// This is the entry point into the OData server for requests to delete an existing entry.
        /// </summary>
        /// <param name="requestUri">The request URI to parse for the target entry.</param>
        /// <returns>If successful, an empty stream. Otherwise, an error.</returns>
        [WebInvoke(UriTemplate = "/OData/{requestUri}", Method = "DELETE")]
        public Stream DeleteEntry(string requestUri)
        {
            return new DeleteHandler { DataContext = new DataContext(this.CurrentDataSource), Model = this.Model }.ProcessDeleteRequest();
        }
    }
}
