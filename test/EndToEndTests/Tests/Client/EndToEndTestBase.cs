//---------------------------------------------------------------------
// <copyright file="EndToEndTestBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
#if !WIN8 && !SILVERLIGHT && !PORTABLELIB && !WINDOWSPHONE
    using Microsoft.Test.OData.Tests.Client.Common;
#endif
#if SILVERLIGHT
    using System.Threading;
    using Microsoft.Silverlight.Testing;
#endif
#if !WIN8 && !SILVERLIGHT && !PORTABLELIB && !(NETCOREAPP1_0 || NETCOREAPP2_0)
    using Microsoft.Test.DataDriven;
#endif
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Framework.Server;
    using Microsoft.Test.OData.Services.TestServices;
#if WIN8 || WINDOWSPHONE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    /// Base class for OData End to End Tests.
    /// </summary>
    [TestClass]
#if !WIN8 && !WINDOWSPHONE
    [DeploymentItem(@"EntityFramework.dll")]
#endif
#if !PORTABLELIB
    [DeploymentItem(@"Microsoft.VisualStudio.QualityTools.Common.dll")]
    [DeploymentItem(@"Microsoft.VisualStudio.TeamSystem.Licensing.dll")]
#endif
    public class EndToEndTestBase
#if !WIN8 && !PORTABLELIB && !(NETCOREAPP1_0 || NETCOREAPP2_0)
 : DataDrivenTest
#endif
    {
        public bool TestCompleted { get; set; }

        private readonly ServiceDescriptor serviceDescriptor;
        private IServiceWrapper serviceWrapper;

        /// <summary>
        /// Initializes a new instance of the EndToEndTestBase class.
        /// </summary>
        public EndToEndTestBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EndToEndTestBase class.
        /// </summary>
        /// <param name="serviceDescriptor">Descriptor for the service that these tests will target.</param>
        protected EndToEndTestBase(ServiceDescriptor serviceDescriptor)
        {
            TestServiceUtil.ServiceUriGenerator = ServiceGeneratorFactory.CreateServiceUriGenerator();
            this.serviceDescriptor = serviceDescriptor;
        }

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

        /// <summary>
        /// Initialization for the tests, called prior to running each test in this class.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
#if WIN8 || PORTABLELIB
            this.serviceWrapper = new ExternalHostedServiceWrapper(this.serviceDescriptor);
#else
            this.serviceWrapper = new DefaultServiceWrapper(this.serviceDescriptor);
#endif
            this.TestCompleted = false;
            this.serviceWrapper.StartService();

            this.ResetDataSource();
            this.CustomTestInitialize();
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
                //the reason why this is an empty catch is because the reset call may return a 404 page not found and in that case the we need to
                // catch it instead of having the test fail. If it does return 404 its fine this is just a reset call which makes the Datasource remak itself 
            }
        }

        /// <summary>
        /// Custom test initialization for derived classes.
        /// </summary>
        public virtual void CustomTestInitialize()
        {
        }

        /// <summary>
        /// Cleanup for the tests, called after running each test in this class.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.serviceWrapper.StopService();
        }

#if !WIN8 && !PORTABLELIB && !(NETCOREAPP1_0 || NETCOREAPP2_0)
        /// <summary>
        /// Exposes the protected single parameter DataDrivenTest.Invoke method.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="action">The test action to invoke.</param>
        /// <param name="parameterData">The parameters for the invocation.</param>
        /// <remarks>
        /// We expose this method so that it can be used in extension methods to avoid cluttering the class.
        /// This assumes that no constraints are being applied to the parameters.
        /// </remarks>
        public void InvokeDataDrivenTest<T>(Action<T> action, ParameterData<T> parameterData)
        {
            this.Invoke(action, parameterData, new Constraint<T>[0]);
        }
#endif

        /// <summary>
        /// Creates a wrapped DataServiceContext for the OData Service.
        /// </summary>
        /// <typeparam name="TContext">The context type being wrapped.</typeparam>
        /// <returns>A wrapped instance of the specified DataServiceContext type.</returns>
        internal virtual DataServiceContextWrapper<TContext> CreateWrappedContext<TContext>() where TContext : DataServiceContext
        {
            var context = this.serviceDescriptor.CreateDataServiceContext(this.serviceWrapper.ServiceUri) as TContext;
            Assert.IsNotNull(context, "Failed to cast DataServiceContext to specified type '{0}'", typeof(TContext).Name);

            var contextWrapper = new DataServiceContextWrapper<TContext>(context);

#if !WINDOWSPHONE
            var testClassName = this.TestContext.FullyQualifiedTestClassName;
            contextWrapper.BuildingRequest += (s, e) => e.Headers.Add("TestName", string.Format("{0}.{1}", testClassName, this.TestContext.TestName));

            // Override any url conventions that may be baked into the context by codegen
            contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
#endif
            return contextWrapper;
        }

#if !WIN8 && !PORTABLELIB && !WINDOWSPHONE
        /// <summary>
        /// Get the metadata document from the test service as an IEdmModel
        /// </summary>
        protected IEdmModel RetrieveServiceEdmModel()
        {
            Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage message = new Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage(new Uri(this.ServiceUri.AbsoluteUri + "$metadata", UriKind.Absolute));
            message.SetHeader("Accept", MimeTypes.ApplicationXml);

            using (var messageReader = new ODataMessageReader(message.GetResponse()))
            {
                return messageReader.ReadMetadataDocument();
            }
        }
#endif
    }
}