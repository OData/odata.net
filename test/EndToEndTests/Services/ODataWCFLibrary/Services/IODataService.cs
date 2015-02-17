//---------------------------------------------------------------------
// <copyright file="IODataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    /// <summary>
    /// Contract for the OData over WCF Service
    /// </summary>
    [ServiceContract]
    public interface IODataService
    {
        /// <summary>
        /// This is the entry point into the OData server which processes GET requests.
        /// </summary>
        /// <returns>A stream containing the results of the GET query.</returns>
        [WebGet(UriTemplate = "/*")]
        Stream ExecuteGetQuery();

        /// <summary>
        /// This is the entry point into the OData server for requests to create a new entry.
        /// </summary>
        /// <param name="requestStream">Stream containing the new entry to insert into the backing data store.</param>
        /// <returns>If successful, a stream containg the new entry. Otherwise, an error.</returns>
        [WebInvoke(UriTemplate = "/*", Method = "POST")]
        Stream ExecutePostRequest(Stream requestStream);

        /// <summary>
        /// This is the entry point into the OData server for requests to update an existing entry.
        /// </summary>
        /// <param name="requestStream">The body of the message, containing the updated entry values.</param>
        /// <returns>If successful, an empty stream. Otherwise, an error.</returns>
        [WebInvoke(UriTemplate = "/*", Method = "PATCH")]
        Stream ExecutePatchRequest(Stream requestStream);

        /// <summary>
        /// This is the entry point into the OData server for requests to update an existing entry.
        /// </summary>
        /// <param name="requestStream">The body of the message, containing the updated entry values.</param>
        /// <returns>If successful, an empty stream. Otherwise, an error.</returns>
        [WebInvoke(UriTemplate = "/*", Method = "PUT")]
        Stream ExecutePutRequest(Stream requestStream);

        /// <summary>
        /// This is the entry point into the OData server for requests to delete an existing entry.
        /// </summary>
        /// <returns>If successful, an empty stream. Otherwise, an error.</returns>
        [WebInvoke(UriTemplate = "/*", Method = "DELETE")]
        Stream ExecuteDeleteRequest();
    }
}
