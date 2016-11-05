//---------------------------------------------------------------------
// <copyright file="EndToEndTestBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client;
using Microsoft.Test.OData.Framework.Client;
using Microsoft.Test.OData.Framework.Server;
using Microsoft.Test.OData.Services.TestServices;
using Microsoft.Test.OData.Services.TestServices.PrimitiveKeysServiceReference;
using Xunit;

namespace Microsoft.OData.Profile111.Tests
{
    /// <summary>
    /// Base class for OData End to End Tests.
    /// </summary>
    public class EndToEndTestBase
    {
        private readonly ServiceDescriptor serviceDescriptor;
        private readonly IServiceWrapper serviceWrapper;

        static EndToEndTestBase()
        {
            TestServiceUtil.ServiceUriGenerator = ServiceGeneratorFactory.CreateServiceUriGenerator();
        }

        /// <summary>
        /// Initializes a new instance of the EndToEndTestBase class.
        /// </summary>
        /// <param name="serviceDescriptor">Descriptor for the service that these tests will target.</param>
        protected EndToEndTestBase(ServiceDescriptor serviceDescriptor)
        {
            this.serviceDescriptor = serviceDescriptor;
            this.serviceWrapper = new ExternalHostedServiceWrapper(this.serviceDescriptor);
            this.TestCompleted = false;
            this.ResetDataSource();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the test is completed.
        /// </summary>
        public bool TestCompleted { get; set; }

        /// <summary>
        /// Gets or sets the MSTest Test Context.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Gets the URI for the test's OData Service.
        /// </summary>
        protected Uri ServiceUri
        {
            get { return this.serviceWrapper == null ? null : this.serviceWrapper.ServiceUri; }
        }

        private void ResetDataSource()
        {
            try
            {
                var context = this.serviceDescriptor.CreateDataServiceContext(this.ServiceUri);
                var ar = context.BeginExecute(new Uri("ResetDataSource/", UriKind.Relative), null, null, "POST");
                ar.AsyncWaitHandle.WaitOne();
            }
            catch (Exception)
            {
                // the reason why this is an empty catch is because the reset call may return a 404 page not found and in that case the we need to
                // catch it instead of having the test fail. If it does return 404 its fine this is just a reset call which makes the Datasource remak itself 
            }
        }

        /// <summary>
        /// Creates a wrapped DataServiceContext for the OData Service.
        /// </summary>
        /// <typeparam name="TContext">The context type being wrapped.</typeparam>
        /// <returns>A wrapped instance of the specified DataServiceContext type.</returns>
        internal virtual DataServiceContextWrapper<TContext> CreateWrappedContext<TContext>() where TContext : DataServiceContext
        {
            var context = this.serviceDescriptor.CreateDataServiceContext(this.serviceWrapper.ServiceUri) as TContext;
            Assert.NotNull(context);

            var contextWrapper = new DataServiceContextWrapper<TContext>(context)
            {
                UrlConventions = DataServiceUrlConventions.Default
            };

            return contextWrapper;
        }
    }
}