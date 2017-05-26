//---------------------------------------------------------------------
// <copyright file="DataServiceSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using System.Linq;
using Microsoft.OData;

namespace AstoriaUnitTests.Tests.Server.Simulators
{
    internal class DataServiceSimulator : IDataService
    {
        public DataServiceConfiguration Configuration
        {
            get;
            set;
        }

        public DataServiceProviderWrapper Provider
        {
            get;
            set;
        }

        public object Instance
        {
            get { return this; }
        }

        public DataServiceOperationContext OperationContext
        {
            get;
            set;
        }

        public DataServiceProcessingPipeline ProcessingPipeline
        {
            get;
            set;
        }

        public UpdatableWrapper Updatable
        {
            get;
            set;
        }

        public DataServiceStreamProviderWrapper StreamProvider
        {
            get;
            set;
        }

        public DataServiceActionProviderWrapper ActionProvider
        {
            get;
            set;
        }

        public DataServiceExecutionProviderWrapper ExecutionProvider
        {
            get
            {
                return this.executionProvider ?? (this.executionProvider = new DataServiceExecutionProviderWrapper(this));
            }
            set
            {
                executionProvider = value;
            }
        }

        private DataServiceExecutionProviderWrapper executionProvider;

        public DataServicePagingProviderWrapper PagingProvider
        {
            get { return new DataServicePagingProviderWrapper(this); }
        }

        public void InternalHandleException(HandleExceptionArgs args)
        {
        }

        public SegmentInfo GetSegmentForContentId(string contentId)
        {
            return null;
        }

        public object GetResource(RequestDescription description, int segmentIndex, string typeFullName)
        {
            throw new NotImplementedException();
        }

        public void DisposeDataSource()
        {
        }

        public void InternalOnStartProcessingRequest(ProcessRequestArgs args)
        {
        }

        public void InternalOnRequestQueryConstructed(IQueryable query)
        {
        }

        public DataServiceODataWriter CreateODataWriterWrapper(ODataWriter odataWriter)
        {
            throw new NotImplementedException();
        }
    }
}
