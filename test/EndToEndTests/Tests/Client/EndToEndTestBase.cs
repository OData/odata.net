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
    using Microsoft.Test.OData.Tests.Client.Common;
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
    using Microsoft.Test.DataDriven;
#endif
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Framework.Server;
    using Microsoft.Test.OData.Services.TestServices;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;
    using System.Reflection;

    /// <summary>
    /// Base class for OData End to End Tests.
    /// </summary>
    public class EndToEndTestBase
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
 : DataDrivenTest
#endif
        ,IDisposable
    {

        public bool TestCompleted { get; set; }
        private readonly ServiceDescriptor serviceDescriptor;
        private IServiceWrapper serviceWrapper;
        private string testClassName = "";
        private string testName = "";

        /// <summary>
        /// Initializes a new instance of the EndToEndTestBase class.
        /// </summary>
        public EndToEndTestBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EndToEndTestBase class.
        /// Initialization for the tests, called prior to running each test in this class.
        /// </summary>
        /// <param name="serviceDescriptor">Descriptor for the service that these tests will target.</param>
        protected EndToEndTestBase(ServiceDescriptor serviceDescriptor, ITestOutputHelper output)
        {
            TestServiceUtil.ServiceUriGenerator = ServiceGeneratorFactory.CreateServiceUriGenerator();
            this.serviceDescriptor = serviceDescriptor;
            this.serviceWrapper = new DefaultServiceWrapper(this.serviceDescriptor);
            this.TestCompleted = false;
            this.serviceWrapper.StartService();

            this.ResetDataSource();
            this.CustomTestInitialize();

            var helper = (TestOutputHelper)output;
            ITest test = (ITest)helper.GetType().GetField("test", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(helper);
            testClassName = test.TestCase.TestMethod.TestClass.Class.ToString();
            testName = test.TestCase.TestMethod.Method.Name;
        }

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
                //the reason why this is an empty catch is because the reset call may return a 404 page not found and in that case the we need to
                // catch it instead of having the test fail. If it does return 404 its fine this is just a reset call which makes the Datasource remak itself 
            }
        }
        /// <summary>
        /// Cleanup for the tests, called after running each test in this class.
        /// </summary>
        public void Dispose()
        {
            this.serviceWrapper.StopService();
        }
        /// <summary>
        /// Custom test initialization for derived classes.
        /// </summary>
        public virtual void CustomTestInitialize()
        {
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
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
            //Failed to cast DataServiceContext to specified type '{0}', typeof(TContext).Name)
            Assert.NotNull(context);

            var contextWrapper = new DataServiceContextWrapper<TContext>(context);

            contextWrapper.BuildingRequest += (s, e) => e.Headers.Add("TestName", string.Format("{0}.{1}", testClassName, testName));

            // Override any url conventions that may be baked into the context by codegen
            contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            return contextWrapper;
        }

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
    }
}