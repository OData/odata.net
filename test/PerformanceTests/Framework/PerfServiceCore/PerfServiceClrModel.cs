//---------------------------------------------------------------------
// <copyright file="PerfServiceClrModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PerfService
{
    using Microsoft.Test.OData.Services.ODataWCFService;

    public class Person : ClrObject
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public int Age { get; set; }
    }

    public class Address : ClrObject
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }

    public class Company : ClrObject
    {
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public EntityCollection<Person> Employees { get; set; }
        public int Revenue { get; set; }
    }
}
