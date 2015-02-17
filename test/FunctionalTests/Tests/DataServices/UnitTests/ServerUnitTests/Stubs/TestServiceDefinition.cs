//---------------------------------------------------------------------
// <copyright file="TestServiceDefinition.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Diagnostics;
    using test = System.Data.Test.Astoria;
    using ocs = AstoriaUnitTests.ObjectContextStubs;

    /// <summary>Base class for a service definition</summary>
    /// <remarks>The idea is that each service also defines its own definition class which allows the configuration of the service.
    /// Tests will then instantiate the definition class, tweak any configuration options they need and then will call CreateFor... method
    /// to get a TestWebRequest instance against such service.
    /// It's up to the definition class to setup the service with the required configuration and then later release all resources associated
    /// with the service once the TestWebRequest object is disposed.
    /// This should allow for caching of read-only services and so on.
    /// There's an assumption that only one TestWebRequest is created at any given time in the tests. This is needed as some services use
    /// static variables to configure the service instance being created.</remarks>
    public class TestServiceDefinition
    {
        /// <summary>The type of the data service to instantiate. By default this is the TestWebRequest.DataServiceType.
        /// Derived definitions may augment the behavior of this, or preinitialize with some default value.</summary>
        public Type DataServiceType { get; set; }

        /// <summary>The type of the service to instantiate. By default this is the TestWebRequest.ServiceType.
        /// Derived definitions may augment the behavior of this, or preinitialize with some default value.</summary>
        public Type ServiceType { get; set; }

        /// <summary>The type of the host interface to use (if this can be influenced).
        /// Leave it null for the default IDataServiceHost.</summary>
        public Type HostInterfaceType { get; set; }

        /// <summary>Constructor</summary>
        public TestServiceDefinition()
        {
            this.HostInterfaceType = typeof(Microsoft.OData.Service.IDataServiceHost);
        }

        /// <summary>Starts the service using in-proc hosting and returns a new TestWebRequest pointing to that service.</summary>
        /// <returns>The newly created TestWebRequest.</returns>
        public TestWebRequest CreateForInProcess()
        {
            return this.CreateForLocation(test.WebServerLocation.InProcess);
        }

        /// <summary>Starts the service using in-proc WCF hosting and returns a new TestWebRequest pointing to that service.</summary>
        /// <returns>The newly created TestWebRequest.</returns>
        public TestWebRequest CreateForInProcessWcf()
        {
            return this.CreateForLocation(test.WebServerLocation.InProcessWcf);
        }

        /// <summary>Starts the service for the specified location and returns a new TestWebRequest pointing to that service.</summary>
        /// <param name="location">The location where to start the service.</param>
        /// <returns>The newly created TestWebRequest.</returns>
        public TestWebRequest CreateForLocation(test.WebServerLocation location)
        {
            Debug.Assert(current == null, "Can't have two instances of TestWebRequest against any service definition at any given time.");
            Debug.Assert(
                (this.ServiceType == null || this.DataServiceType == null) && (this.DataServiceType != null || this.ServiceType != null),
                "Either DataServiceType or ServiceType must not be null, but only one can be specified.");

            TestWebRequest request = TestWebRequest.CreateForLocation(location);
            if (this.DataServiceType != null)
            {
                request.DataServiceType = this.DataServiceType;
            }
            else
            {
                request.ServiceType = this.ServiceType;
            }

            current = this;
            current.InitializeService(request);
            request.RegisterForDispose(() => { current = null; });

            return request;
        }

        /// <summary>If a TestWebRequest was created this static variable holds a reference to the service definition used to create the request.</summary>
        private static TestServiceDefinition current;

        /// <summary>If a TestWebRequest was created this static variable holds a reference to the service definition used to create the request.</summary>
        /// <remarks>This should not be used by the test themselves. This is meant for the services to get their configuration from is necessary.</remarks>
        internal static TestServiceDefinition Current { get { return current; } }

        /// <summary>Called to initialize the service on a given request.</summary>
        /// <param name="request">The request which was not yet used for the service to initialize on.</param>
        virtual protected void InitializeService(TestWebRequest request) 
        {
            request.RegisterForDispose(System.Data.Test.Astoria.TestUtil.RestoreStaticValueOnDispose(typeof(System.Data.Test.Astoria.BaseTestWebRequest), "HostInterfaceType"));
            System.Data.Test.Astoria.BaseTestWebRequest.HostInterfaceType = this.HostInterfaceType;
        }
    }

    /// <summary>Service definition for CustomObjectContext data context. That is the EF based Custom model.</summary>
    public class CustomObjectContextServiceDefinition : TestServiceDefinition
    {
        /// <summary>Constructor.</summary>
        public CustomObjectContextServiceDefinition()
        {
            this.DataServiceType = typeof(ocs.CustomObjectContext);
        }

        /// <summary>Called to initialize the service on a given request.</summary>
        /// <param name="request">The request which was not yet used for the service to initialize on.</param>
        protected override void InitializeService(TestWebRequest request)
        {
            // Create the table and populate it with data. Register for cleanup as well.
            request.RegisterForDispose(ocs.PopulateData.CreateTableAndPopulateData());
        }
    }
}
