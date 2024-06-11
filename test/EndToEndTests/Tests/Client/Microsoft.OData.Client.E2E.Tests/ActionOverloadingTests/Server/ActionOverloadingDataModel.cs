//-----------------------------------------------------------------------------
// <copyright file="ActionOverloadingDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server
{
    public class Product
    {
        [EfKey]
        public int ProductId { get; set; }
        public string? Description { get; set; }
        public string? BaseConcurrency { get; set; }
        public Product()
        {
        }
    }

    public partial class OrderLine
    {
        [EfKey]
        public int OrderId { get; set; }
        [EfKey]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }

        public OrderLine()
        {
        }
    }

    public partial class Person
    {
        public int PersonId { get; set; }
        public string? Name { get; set; }
        public List<PersonMetadata> PersonMetadata { get; set; }

        public Person()
        {
            this.PersonMetadata = [];
        }
    }

    public partial class PersonMetadata
    {
        public int PersonMetadataId { get; set; }
        public int PersonId { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyValue { get; set; }
        public Person? Person { get; set; }

        public PersonMetadata()
        {
        }
    }

    public partial class Employee : Person
    {
        public int ManagersPersonId { get; set; }
        public int Salary { get; set; }
        public string? Title { get; set; }
        public Employee? Manager { get; set; }

        public Employee()
        {
        }
    }

    public partial class SpecialEmployee : Employee
    {
        public int CarsVIN { get; set; }
        public int Bonus { get; set; }
        public bool IsFullyVested { get; set; }
        public SpecialEmployee()
        {
        }
    }

    public partial class Contractor : Person
    {
        public int ContratorCompanyId { get; set; }
        public int BillingRate { get; set; }
        public int TeamContactPersonId { get; set; }
        public string? JobDescription { get; set; }

        public Contractor()
        {
        }
    }
}
