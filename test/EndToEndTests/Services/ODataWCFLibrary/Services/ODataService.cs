//---------------------------------------------------------------------
// <copyright file="ODataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.IO;
    using System.ServiceModel.Web;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;
    using Microsoft.Test.OData.Services.ODataWCFService.Handlers;

    /// <summary>
    /// The base class of OData service
    /// </summary>
    public abstract class ODataService<TDataSource> : IODataService
        where TDataSource : class, IODataDataSource, new()
    {

        public virtual RootRequestHandler CreateRootRequestHandler(HttpMethod method, TDataSource dataSource)
        {
            return new RootRequestHandler(method, dataSource);
        }

        public virtual Stream ExecuteGetQuery()
        {
            DataSourceManager.EnsureCurrentDataSource<TDataSource>();
            var dataSource = DataSourceManager.GetCurrentDataSource<TDataSource>();
            return this.CreateRootRequestHandler(HttpMethod.GET, dataSource).Process(null);
        }

        public Stream ExecutePostRequest(Stream requestStream)
        {
            DataSourceManager.EnsureCurrentDataSource<TDataSource>();

            // TODO: [layliu] This is not perfect here. How many cases of X-HTTP-Method will be? We should figure it out.
            // For Portable platform, UsePostTunneling is set to true, Then Update/Delete request will use "POST", the real operation is stored in X-HTTP-Method
            string methodName = WebOperationContext.Current.IncomingRequest.Headers.Get("X-HTTP-Method");
            HttpMethod httpMethod = methodName == null ? HttpMethod.Unknown : Utility.CreateHttpMethod(methodName);
            var dataSource = DataSourceManager.GetCurrentDataSource<TDataSource>();

            if (httpMethod == HttpMethod.PATCH || httpMethod == HttpMethod.PUT || httpMethod == HttpMethod.DELETE)
            {
                return this.CreateRootRequestHandler(httpMethod, dataSource).Process(requestStream);
            }
            else
            {
                return this.CreateRootRequestHandler(HttpMethod.POST, dataSource).Process(requestStream);
            }
        }

        public Stream ExecutePatchRequest(Stream requestStream)
        {
            DataSourceManager.EnsureCurrentDataSource<TDataSource>();
            var dataSource = DataSourceManager.GetCurrentDataSource<TDataSource>();
            return this.CreateRootRequestHandler(HttpMethod.PATCH, dataSource).Process(requestStream);
        }

        public Stream ExecutePutRequest(Stream messageBody)
        {
            DataSourceManager.EnsureCurrentDataSource<TDataSource>();
            var dataSource = DataSourceManager.GetCurrentDataSource<TDataSource>();
            return this.CreateRootRequestHandler(HttpMethod.PUT, dataSource).Process(messageBody);
        }

        public Stream ExecuteDeleteRequest()
        {
            DataSourceManager.EnsureCurrentDataSource<TDataSource>();
            var dataSource = DataSourceManager.GetCurrentDataSource<TDataSource>();
            return this.CreateRootRequestHandler(HttpMethod.DELETE, dataSource).Process(null);
        }
    }
}
