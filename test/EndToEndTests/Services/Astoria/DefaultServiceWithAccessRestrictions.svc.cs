//---------------------------------------------------------------------
// <copyright file="DefaultServiceWithAccessRestrictions.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.AstoriaDefaultService
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Linq;
    using System.ServiceModel.Web;
    using System.Text;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts.DataOracle;
    using Microsoft.Test.OData.Framework.TestProviders.OptionalProviders;

    [System.ServiceModel.ServiceBehaviorAttribute(IncludeExceptionDetailInFaults = true)]
    public class ServiceWithAccessRestrictions : DataService<DefaultContainer>, IDataServiceDataSourceCreator, System.IServiceProvider
    {
        private static bool usePrePopulatedStreamsData = false;
        protected static bool streamDataInitialized = false;
        protected static object lockObject = new object();

        public static void InitializeService(DataServiceConfiguration config)
        {
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.AcceptSpatialLiteralsInQuery = false;
            config.DataServiceBehavior.MaxProtocolVersion = Microsoft.OData.Client.ODataProtocolVersion.V4;
            config.SetEntitySetAccessRule("*", Microsoft.OData.Service.EntitySetRights.All);
            config.SetEntitySetAccessRule("MappedEntityType", EntitySetRights.None);
            config.SetEntitySetAccessRule("Message", EntitySetRights.AllRead);

            config.SetServiceActionAccessRule("*", Microsoft.OData.Service.ServiceActionRights.Invoke);
            config.SetServiceOperationAccessRule("*", Microsoft.OData.Service.ServiceOperationRights.All);

            config.SetEntitySetPageSize("Customer", 2);
            config.SetEntitySetPageSize("Order", 2);

            config.RegisterKnownType(typeof(ComplexWithAllPrimitiveTypes));
            config.EnableTypeAccess("*");
        }

        public virtual object GetService(System.Type serviceType)
        {
            if (((serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceStreamProvider2)) || (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceStreamProvider))))
            {
                var inMemoryProvider = new InMemoryStreamProvider<ReferenceEqualityComparer>();
                if (usePrePopulatedStreamsData)
                {
                    this.EnsureStreamDataIsInitialized(inMemoryProvider);
                }

                return inMemoryProvider;
            }

            if (((serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider)) || 
                 (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider2)) ||
                 (serviceType == typeof(Microsoft.OData.Service.IUpdatable))))
            {
                return this.CurrentDataSource;
            }

            if (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceActionProvider))
            {
                return new AstoriaDefaultActionProvider(this);
            }

            return null;
        }

        #region Populate Stream Data
        protected void EnsureStreamDataIsInitialized(InMemoryStreamProvider<ReferenceEqualityComparer> inMemoryProvider)
        {
            System.Threading.Monitor.Enter(ServiceWithAccessRestrictions.lockObject);
            try
            {
                if (ServiceWithAccessRestrictions.streamDataInitialized == false)
                {
                    PopulateStreamData(this.CurrentDataSource, inMemoryProvider);
                    ServiceWithAccessRestrictions.streamDataInitialized = true;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(ServiceWithAccessRestrictions.lockObject);
            }
        }
        
        protected static void PopulateStreamData(DefaultContainer container, InMemoryStreamProvider<ReferenceEqualityComparer> inMemoryProvider)
        {
            PopulateCustomerInfoStreams(container, inMemoryProvider);
            PopulateCustomerStreams(container, inMemoryProvider);
            PopulateCarStreams(container, inMemoryProvider);
            PopulateOrderLineStreams(container, inMemoryProvider);
            PopulateProductStreams(container, inMemoryProvider);
        }

        private static void PopulateCustomerInfoStreams(DefaultContainer container, InMemoryStreamProvider<ReferenceEqualityComparer> inMemoryProvider)
        {
            var entities = container.CustomerInfo.ToList();
            inMemoryProvider.InitializeEntityMediaStream(entities[0], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 0"));
            inMemoryProvider.InitializeEntityMediaStream(entities[1], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 1"));
            inMemoryProvider.InitializeEntityMediaStream(entities[2], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 2"));
            inMemoryProvider.InitializeEntityMediaStream(entities[3], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 3"));
            inMemoryProvider.InitializeEntityMediaStream(entities[4], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 4"));
            inMemoryProvider.InitializeEntityMediaStream(entities[5], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 5"));
            inMemoryProvider.InitializeEntityMediaStream(entities[6], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 6"));
            inMemoryProvider.InitializeEntityMediaStream(entities[7], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 7"));
            inMemoryProvider.InitializeEntityMediaStream(entities[8], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 8"));
            inMemoryProvider.InitializeEntityMediaStream(entities[9], null, false, "text/plain", Encoding.UTF8.GetBytes("Test stream data 9"));

            inMemoryProvider.SaveStreams();
        }

        private static void PopulateCustomerStreams(DefaultContainer container, InMemoryStreamProvider<ReferenceEqualityComparer> inMemoryProvider)
        {
            var entities = container.Customer.ToList();

            inMemoryProvider.InitializeEntityNamedStream(entities[0], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("abcagtagag"));
            inMemoryProvider.InitializeEntityNamedStream(entities[1], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aahahah"));
            inMemoryProvider.InitializeEntityNamedStream(entities[2], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aghawrararh"));
            inMemoryProvider.InitializeEntityNamedStream(entities[3], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("itlf,u5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[4], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("cbvnrystj46w"));
            inMemoryProvider.InitializeEntityNamedStream(entities[5], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("rhwh3qw5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[6], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("ajh5uw35635"));
            inMemoryProvider.InitializeEntityNamedStream(entities[7], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("4y24y24yq"));
            inMemoryProvider.InitializeEntityNamedStream(entities[8], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("y422y424y24y"));
            inMemoryProvider.InitializeEntityNamedStream(entities[9], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("9t75p89lk"));

            inMemoryProvider.InitializeEntityNamedStream(entities[0], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("abcagtagag"));
            inMemoryProvider.InitializeEntityNamedStream(entities[1], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aahahah"));
            inMemoryProvider.InitializeEntityNamedStream(entities[2], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aghawrararh"));
            inMemoryProvider.InitializeEntityNamedStream(entities[3], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("itlf,u5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[4], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("cbvnrystj46w"));
            inMemoryProvider.InitializeEntityNamedStream(entities[5], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("rhwh3qw5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[6], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("ajh5uw35635"));
            inMemoryProvider.InitializeEntityNamedStream(entities[7], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("4y24y24yq"));
            inMemoryProvider.InitializeEntityNamedStream(entities[8], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("y422y424y24y"));
            inMemoryProvider.InitializeEntityNamedStream(entities[9], "Thumbnail", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("9t75p89lk"));

            inMemoryProvider.SaveStreams();
        }

        private static void PopulateProductStreams(DefaultContainer container, InMemoryStreamProvider<ReferenceEqualityComparer> inMemoryProvider)
        {
            var entities = container.Product.ToList();

            inMemoryProvider.InitializeEntityNamedStream(entities[0], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("abcagtagag"));
            inMemoryProvider.InitializeEntityNamedStream(entities[1], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aahahah"));
            inMemoryProvider.InitializeEntityNamedStream(entities[2], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aghawrararh"));
            inMemoryProvider.InitializeEntityNamedStream(entities[3], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("itlf,u5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[4], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("cbvnrystj46w"));
            inMemoryProvider.InitializeEntityNamedStream(entities[5], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("rhwh3qw5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[6], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("ajh5uw35635"));
            inMemoryProvider.InitializeEntityNamedStream(entities[7], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("4y24y24yq"));
            inMemoryProvider.InitializeEntityNamedStream(entities[8], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("y422y424y24y"));
            inMemoryProvider.InitializeEntityNamedStream(entities[9], "Picture", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("9t75p89lk"));

            inMemoryProvider.SaveStreams();
        }

        private static void PopulateOrderLineStreams(DefaultContainer container, InMemoryStreamProvider<ReferenceEqualityComparer> inMemoryProvider)
        {
            var entities = container.OrderLine.ToList();

            inMemoryProvider.InitializeEntityNamedStream(entities[0], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("abcagtagag"));
            inMemoryProvider.InitializeEntityNamedStream(entities[1], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aahahah"));
            inMemoryProvider.InitializeEntityNamedStream(entities[2], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aghawrararh"));
            inMemoryProvider.InitializeEntityNamedStream(entities[3], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("itlf,u5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[4], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("cbvnrystj46w"));
            inMemoryProvider.InitializeEntityNamedStream(entities[5], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("rhwh3qw5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[6], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("ajh5uw35635"));
            inMemoryProvider.InitializeEntityNamedStream(entities[7], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("4y24y24yq"));
            inMemoryProvider.InitializeEntityNamedStream(entities[8], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("y422y424y24y"));
            inMemoryProvider.InitializeEntityNamedStream(entities[9], "OrderLineStream", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("9t75p89lk"));

            inMemoryProvider.SaveStreams();
        }

        private static void PopulateCarStreams(DefaultContainer container, InMemoryStreamProvider<ReferenceEqualityComparer> inMemoryProvider)
        {
            var entities = container.Car.ToList();

            inMemoryProvider.InitializeEntityMediaStream(entities[0], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("abcagtagag"));
            inMemoryProvider.InitializeEntityMediaStream(entities[1], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aahahah"));
            inMemoryProvider.InitializeEntityMediaStream(entities[2], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aghawrararh"));
            inMemoryProvider.InitializeEntityMediaStream(entities[3], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("itlf,u5"));
            inMemoryProvider.InitializeEntityMediaStream(entities[4], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("cbvnrystj46w"));
            inMemoryProvider.InitializeEntityMediaStream(entities[5], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("rhwh3qw5"));
            inMemoryProvider.InitializeEntityMediaStream(entities[6], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("ajh5uw35635"));
            inMemoryProvider.InitializeEntityMediaStream(entities[7], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("4y24y24yq"));
            inMemoryProvider.InitializeEntityMediaStream(entities[8], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("y422y424y24y"));
            inMemoryProvider.InitializeEntityMediaStream(entities[9], null, false, "application/octet-stream", Encoding.UTF8.GetBytes("9t75p89lk"));

            inMemoryProvider.InitializeEntityNamedStream(entities[0], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("abcagtagag"));
            inMemoryProvider.InitializeEntityNamedStream(entities[1], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aahahah"));
            inMemoryProvider.InitializeEntityNamedStream(entities[2], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aghawrararh"));
            inMemoryProvider.InitializeEntityNamedStream(entities[3], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("itlf,u5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[4], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("cbvnrystj46w"));
            inMemoryProvider.InitializeEntityNamedStream(entities[5], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("rhwh3qw5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[6], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("ajh5uw35635"));
            inMemoryProvider.InitializeEntityNamedStream(entities[7], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("4y24y24yq"));
            inMemoryProvider.InitializeEntityNamedStream(entities[8], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("y422y424y24y"));
            inMemoryProvider.InitializeEntityNamedStream(entities[9], "Video", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("9t75p89lk"));

            inMemoryProvider.InitializeEntityNamedStream(entities[0], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("abcagtagag"));
            inMemoryProvider.InitializeEntityNamedStream(entities[1], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aahahah"));
            inMemoryProvider.InitializeEntityNamedStream(entities[2], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("aghawrararh"));
            inMemoryProvider.InitializeEntityNamedStream(entities[3], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("itlf,u5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[4], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("cbvnrystj46w"));
            inMemoryProvider.InitializeEntityNamedStream(entities[5], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("rhwh3qw5"));
            inMemoryProvider.InitializeEntityNamedStream(entities[6], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("ajh5uw35635"));
            inMemoryProvider.InitializeEntityNamedStream(entities[7], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("4y24y24yq"));
            inMemoryProvider.InitializeEntityNamedStream(entities[8], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("y422y424y24y"));
            inMemoryProvider.InitializeEntityNamedStream(entities[9], "Photo", null, false, "application/octet-stream", Encoding.UTF8.GetBytes("9t75p89lk"));

            inMemoryProvider.SaveStreams();
        }
        #endregion

        // 
        // Service Operations
        //
        [WebGet()]
        public string GetPrimitiveString()
        {
            return "Foo";
        }

        [WebGet()]
        public IEnumerable<Customer> GetSpecificCustomer(string Name)
        {
            return this.CurrentDataSource.Customer.Where(p1 => (p1.Name == Name));
        }

        [WebGet()]
        public int GetCustomerCount()
        {
            return this.CurrentDataSource.Customer.Count();
        }

        [WebGet()]
        public int GetArgumentPlusOne(int arg1)
        {
            return (arg1 + 1);
        }

        [WebGet()]
        public IEnumerable<ContactDetails> EntityProjectionReturnsCollectionOfComplexTypes()
        {
            return this.CurrentDataSource.Customer.Select(p2 => p2.PrimaryContactInfo);
        }

        [WebInvoke()]
        public void ResetDataSource()
        {
            this.CurrentDataSource.ClearData();
        }

        // A service operation that returns in-stream error in the response payload.
        [WebGet()]
        public IEnumerable<Customer> InStreamErrorGetCustomer()
        {
            return this.CurrentDataSource.Customer.AsEnumerable<Customer>().Where(c => c.CustomerId < 0 && this.ThrowForSpecificCustomer(c));
        }

        private bool ThrowForSpecificCustomer(Customer c)
        {
            if (c.CustomerId == -9)
            {
                throw new DataServiceException("InStreamErrorGetCustomer ThrowForSpecificCustomer error");
            }

            return c.CustomerId < -5;
        }

        //
        // Actions
        //
        public void IncreaseSalaries(IEnumerable<Employee> employees, int n)
        {
            foreach (var employee in employees)
            {
                employee.Salary += n;
            }
        }

        public void Sack(Employee employee)
        {
            employee.Salary = 0;
            employee.Title += "[Sacked]";
        }

        public void ResetComputerDetailsSpecifications(ComputerDetail computerDetail, IEnumerable<string> specifications, DateTimeOffset purchaseTime)
        {
            computerDetail.SpecificationsBag.Clear();
            computerDetail.SpecificationsBag.AddRange(specifications);
            computerDetail.PurchaseDate = purchaseTime;
        }

        object IDataServiceDataSourceCreator.CreateDataSource()
        {
            return this.CreateDataSource();
        }
    }
}
