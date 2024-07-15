//-----------------------------------------------------------------------------
// <copyright file="AnnotationTestsDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.ModelBuilder;
using Microsoft.Spatial;
using System.Collections.ObjectModel;

namespace Microsoft.OData.Client.E2E.Tests.AnnotationTests.Server
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public ODataInstanceAnnotationContainer InstanceAnnotations { get; set; }
    }

    public class HomeAddress : Address
    {
        public string FamilyName { get; set; }
    }

    public class CompanyAddress : Address
    {
        public string CompanyName { get; set; }
    }

    public class CityInformation
    {
        public string CountryRegion { get; set; }
        public bool IsCapital { get; set; }
    }

    public class Person
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public Collection<string> Numbers { get; set; }
        public Collection<string> Emails { get; set; }
        public Collection<Address> Addresses { get; set; }
        public Address HomeAddress { get; set; }
    }

    public class Customer : Person
    {
        public string City { get; set; }
        public DateTimeOffset Birthday { get; set; }
        public TimeSpan TimeBetweenLastTwoOrders { get; set; }
        public Company Company { get; set; }
    }

    public class Employee : Person
    {
        public DateTimeOffset DateHired { get; set; }
        public GeographyPoint Office { get; set; }
        public int CompanyID { get; set; }
        public Company Company { get; set; }
    }

    public class Company
    {
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public Int64 Revenue { get; set; }
        public Customer VipCustomer {  get; set; }
    }
}
