//---------------------------------------------------------------------
// <copyright file="QueryTestData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Test.Taupo.OData.Query.Tests.Common.DataServiceProvider;
    using Microsoft.Test.Taupo.OData.Query.Tests.DataClasses;
    #endregion Namespaces

    /// <summary>
    /// Helper class to define test data based on the test metadata model.
    /// </summary>
    public static class QueryTestData
    {
        private const int customerCount = 7;
        private const int addressCount = 6;
        private const int multiKeyCount = 4;

        internal static DSPContext BuildTestData()
        {
            DSPContext data = new DSPContext();

            // create an entity set for 'Customer' and fill it
            IList<object> customers = data.GetEntitySetEntities("Customers");
            CreateCustomers(customers);

            // create an entity set for 'MultiKeys' and fill it
            IList<object> multiKeys = data.GetEntitySetEntities("MultiKeys");
            CreateMultiKeys(multiKeys);

            // create a resource 
            // add the service operations
            CreateServiceOperations(data.ServiceOperations);

            return data;
        }

        internal static Customer[] CreateCustomers()
        {
            return new Customer[]
            {
                CreateCustomer(1),
                CreateCustomer(2),
                CreateCustomer(3),
                CreateCustomer(4),
                CreateCustomer(5),
                CreateCustomer(6),
                CreateCustomer(7),
            };
        }

        internal static Customer CreateCustomer(int id)
        {
            switch (id)
            {
                case 1:
                    Address address1 = CreateAddress(1);
                    List<string> emails1 = new List<string>() { "bill.meyer@odata.org", "b.meyer@odata.org" };
                    return new Customer { ID = 1, Name = "Bill Meyer", Emails = emails1, Address = address1 };

                case 2:
                    Address address2 = CreateAddress(2);
                    List<string> emails2 = new List<string>() { "john.deer@odata.org", "j.deer@odata.org" };
                    return new Customer { ID = 2, Name = "John Deer", Emails = emails2, Address = address2 };

                case 3:
                    Address address3 = CreateAddress(3);
                    List<string> emails3 = new List<string>() { "john.doe@odata.org", "j.doe@odata.org" };
                    return new Customer { ID = 3, Name = "John Doe", Emails = emails3, Address = address3 };

                case 4:
                    Address address4 = CreateAddress(4);
                    List<string> emails4 = new List<string>() { "hannah.hannah@odata.org", "h.hannah@odata.org", "h.h@odata.org" };
                    return new Customer { ID = 4, Name = "hannah hannah", Emails = emails4, Address = address4 };

                case 5:
                    Address address5 = CreateAddress(5);
                    List<string> emails5 = new List<string>() { "john.doe@somewhere.org" };
                    return new Customer { ID = 5, Name = "John Doe", Emails = emails5, Address = address5 };

                case 6:
                    Address address6 = CreateAddress(6);
                    List<string> emails6 = new List<string>() { "j.d@mycompany.com" };
                    return new Customer { ID = 6, Name = "John Doe", Emails = emails6, Address = address6 };

                case 7:
                    // NOTE: this customer has the same address as customer #6
                    Address address7 = CreateAddress(6);
                    List<string> emails7 = new List<string>() { "anne@home.at" };
                    return new Customer { ID = 6, Name = "Anne Miller", Emails = emails7, Address = address7 };

                default:
                    throw new NotSupportedException();
            }
        }

        internal static Address CreateAddress(int id)
        {
            switch (id)
            {
                case 1:
                    return new Address { City = "Vienna", Zip = 1120 };
                case 2:
                    return new Address { City = "Prague", Zip = 1234 };
                case 3:
                    return new Address { City = "Redmond", Zip = 98052 };
                case 4:
                    return new Address { City = "Seattle", Zip = 94004 };
                case 5:
                    return new Address { City = "Seattle", Zip = 94005 };
                case 6:
                    return new Address { City = "Bellevue", Zip = 98008 };
                default:
                    throw new NotSupportedException();
            }

        }

        internal static MultiKey CreateMultiKey(int id)
        {
            switch (id)
            {
                case 1:
                    return new MultiKey() { Keya = 42, KeyA = "foo", KeyB = 42.2, NonKey = "NonKey" };

                case 2:
                    return new MultiKey() { Keya = 42, KeyA = "foo", KeyB = 42, NonKey = "NonKey" };

                case 3:
                    return new MultiKey() { Keya = 42, KeyA = "foo", KeyB = 42.42, NonKey = "NonKey" };

                case 4:
                    return new MultiKey() { Keya = 42, KeyA = "foo", KeyB = 0.0, NonKey = "foo" };

                default:
                    throw new NotSupportedException();
            }
        }

        private static void CreateCustomers(IList<object> customers)
        {
            Debug.Assert(customers != null && customers.Count == 0);

            for (int i = 1; i <= customerCount; ++i)
            {
                customers.Add(CreateCustomer(i));
            }
        }

        private static void CreateMultiKeys(IList<object> multiKeys)
        {
            Debug.Assert(multiKeys != null && multiKeys.Count == 0);

            for (int i = 1; i <= multiKeyCount; ++i)
            {
                multiKeys.Add(CreateMultiKey(i));
            }
        }

        private static void CreateServiceOperations(IDictionary<string, Func<object[], object>> serviceOperations)
        {
            Debug.Assert(serviceOperations != null && serviceOperations.Count == 0);

            // TODO: how to model void service operations?
            // metadata.AddServiceOperation("VoidServiceOperation", ServiceOperationResultKind.Void, null, null, "GET", defaultParams);

            // metadata.AddServiceOperation("DirectValuePrimitiveServiceOperation", ServiceOperationResultKind.DirectValue, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", defaultParams);
            serviceOperations.Add("DirectValuePrimitiveServiceOperation", (args) => { return (int)args[0]; });

            // metadata.AddServiceOperation("DirectValueComplexServiceOperation", ServiceOperationResultKind.DirectValue, addressType, null, "GET", defaultParams);
            serviceOperations.Add("DirectValueComplexServiceOperation", (args) => { return CreateAddress((int)args[0]); });

            // metadata.AddServiceOperation("DirectValueEntityServiceOperation", ServiceOperationResultKind.DirectValue, customerType, metadata.ResourceSet("Customers"), "GET", defaultParams);
            serviceOperations.Add("DirectValueEntityServiceOperation", (args) => { return CreateCustomer((int)args[0]); });

            // metadata.AddServiceOperation("EnumerationPrimitiveServiceOperation", ServiceOperationResultKind.Enumeration, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", defaultParams);
            serviceOperations.Add("EnumerationPrimitiveServiceOperation", (args) => { return args.Cast<int>(); });

            // metadata.AddServiceOperation("EnumerationComplexServiceOperation", ServiceOperationResultKind.Enumeration, addressType, null, "GET", defaultParams);
            serviceOperations.Add("EnumerationComplexServiceOperation", (args) => { return args.Cast<int>().Select(id => CreateAddress(id)); });

            // metadata.AddServiceOperation("EnumerationEntityServiceOperation", ServiceOperationResultKind.Enumeration, customerType, metadata.ResourceSet("Customers"), "GET", defaultParams);
            serviceOperations.Add("EnumerationEntityServiceOperation", (args) => { return args.Cast<int>().Select(id => CreateCustomer(id)); });

            // metadata.AddServiceOperation("QuerySinglePrimitiveServiceOperation", ServiceOperationResultKind.QueryWithSingleResult, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", defaultParams);
            serviceOperations.Add("QuerySinglePrimitiveServiceOperation", (args) => { return new List<int>() { (int)args[0] }.AsQueryable(); });

            // metadata.AddServiceOperation("QuerySingleComplexServiceOperation", ServiceOperationResultKind.QueryWithSingleResult, addressType, null, "GET", defaultParams);
            serviceOperations.Add("QuerySingleComplexServiceOperation", (args) => { return new List<Address>() { CreateAddress((int)args[0]) }.AsQueryable(); });

            // metadata.AddServiceOperation("QuerySingleEntityServiceOperation", ServiceOperationResultKind.QueryWithSingleResult, customerType, metadata.ResourceSet("Customers"), "GET", defaultParams);
            serviceOperations.Add("QuerySingleEntityServiceOperation", (args) => { return new List<Customer>() { CreateCustomer((int)args[0]) }.AsQueryable(); });

            // metadata.AddServiceOperation("QueryMultiplePrimitiveServiceOperation", ServiceOperationResultKind.QueryWithMultipleResults, ResourceType.GetPrimitiveResourceType(typeof(int)), null, "GET", defaultParams);
            serviceOperations.Add("QueryMultiplePrimitiveServiceOperation", (args) => { int i = (int)args[0]; return new List<int>() { i, i + 1 }.AsQueryable(); });

            // metadata.AddServiceOperation("QueryMultipleComplexServiceOperation", ServiceOperationResultKind.QueryWithMultipleResults, addressType, null, "GET", defaultParams);
            serviceOperations.Add("QueryMultipleComplexServiceOperation", (args) => { int i = (int)args[0]; return new List<Address>() { CreateAddress(i), CreateAddress(i + 1) }.AsQueryable(); });

            // metadata.AddServiceOperation("QueryMultipleEntityServiceOperation", ServiceOperationResultKind.QueryWithMultipleResults, customerType, metadata.ResourceSet("Customers"), "GET", defaultParams);
            serviceOperations.Add("QueryMultipleEntityServiceOperation", (args) => { int i = (int)args[0]; return new List<Customer>() { CreateCustomer(i), CreateCustomer(i + 1) }.AsQueryable(); });

            // metadata.AddServiceOperation("ServiceOperationWithNoParameters", ServiceOperationResultKind.QueryWithMultipleResults, customerType, metadata.ResourceSet("Customers"), "GET", null);
            serviceOperations.Add("ServiceOperationWithNoParameters", (args) => { Debug.Assert(args == null || args.Length == 0); return new List<Customer>() { CreateCustomer(1), CreateCustomer(2) }.AsQueryable(); });

            // metadata.AddServiceOperation("ServiceOperationWithMultipleParameters", ServiceOperationResultKind.QueryWithMultipleResults, customerType, metadata.ResourceSet("Customers"), "GET",
            //    new[] { 
            //        new ServiceOperationParameter("paramInt", ResourceType.GetPrimitiveResourceType(typeof(int))), 
            //        new ServiceOperationParameter("paramString", ResourceType.GetPrimitiveResourceType(typeof(string))), 
            //        new ServiceOperationParameter("paramNullableBool", ResourceType.GetPrimitiveResourceType(typeof(bool?)))
            //    }
            serviceOperations.Add("ServiceOperationWithMultipleParameters", (args) => 
            {
                Debug.Assert(args != null, "args != null");
                Debug.Assert(args.Length == 3, "args.Length == 3");
                Debug.Assert(args[0] is int, "args[0] is int");
                Debug.Assert(args[1] is string, "args[1] is string");
                Debug.Assert(args[2] == null || args[2] is bool?, "args[2] is bool?"); 
                return new List<Customer>() { CreateCustomer(1), CreateCustomer(2) }.AsQueryable(); 
            });
        }
    }
}
