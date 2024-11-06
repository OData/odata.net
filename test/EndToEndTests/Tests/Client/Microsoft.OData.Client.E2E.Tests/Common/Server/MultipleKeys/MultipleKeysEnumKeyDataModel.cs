//-----------------------------------------------------------------------------
// <copyright file="MultipleKeysEnumKeyDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.Client.E2E.Tests.Common.Server.MultipleKeys
{
    public class EmployeeWithEnumKey
    {
        [EfKey]
        public int EmployeeNumber { get; set; }
        [EfKey]
        public EmployeeType EmployeeType { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Organization")]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }

    public enum EmployeeType
    {
        FullTime,
        PartTime,
        Contractor
    }

    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<EmployeeWithEnumKey> Employees { get; set; }   
    }
}
