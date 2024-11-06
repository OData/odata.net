//-----------------------------------------------------------------------------
// <copyright file="MultipleKeysEnumKeyDataSource.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------



namespace Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys
{
    public class MultipleKeysEnumKeyDataSource
    {
        public static MultipleKeysEnumKeyDataSource CreateInstance()
        {
            return new MultipleKeysEnumKeyDataSource();
        }

        public MultipleKeysEnumKeyDataSource()
        {
            ResetData();
            InitializeData();
        }

        private void InitializeData()
        {
            PopulateOrganizationsSet();
            PopulateEmployeesWithEnumAsKeySet();
        }

        private void ResetData()
        {
            Employees?.Clear();
            Organizations?.Clear();
        }

        public IList<EmployeeWithEnumKey>? Employees { get; private set; }
        public IList<Organization>? Organizations { get; private set; }

        private void PopulateEmployeesWithEnumAsKeySet()
        {
            this.Employees = new List<EmployeeWithEnumKey>
            {
                new()
                {
                    EmployeeNumber = 1,
                    EmployeeType = EmployeeType.FullTime,
                    Name = "Employee 1",
                    Salary = 10000,
                    OrganizationId = 1,
                    Organization = this.Organizations.First(a => a.Id == 1)
                },
                new()
                {
                    EmployeeNumber = 2,
                    EmployeeType = EmployeeType.PartTime,
                    Name = "Employee 2",
                    Salary = 20000,
                    OrganizationId = 2,
                    Organization = this.Organizations.First(a => a.Id == 2)
                },
                new()
                {
                    EmployeeNumber = 3,
                    EmployeeType = EmployeeType.Contractor,
                    Name = "Employee 3",
                    Salary = 30000,
                    OrganizationId = 3,
                    Organization = this.Organizations.First(a => a.Id == 3)
                },
                new()
                {
                    EmployeeNumber = 4,
                    EmployeeType = EmployeeType.FullTime,
                    Name = "Employee 4",
                    Salary = 40000,
                    OrganizationId = 3,
                    Organization = this.Organizations.First(a => a.Id == 3)
                },
            };

            foreach (var org in this.Organizations)
            {
                org.Employees = this.Employees.Where(e => e.OrganizationId == org.Id).ToList();
            }
        }

        private void PopulateOrganizationsSet()
        {
            this.Organizations = new List<Organization>
            {
                new() { Id = 1, Name = "Org 1" },
                new() { Id = 2, Name = "Org 2" },
                new() { Id = 3, Name = "Org 3" },
            };
        }
    }
}
