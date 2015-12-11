//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.SampleService.Models.ODataSimplified
{
    using Microsoft.Test.OData.Services.ODataWCFService;
    using System.Collections.ObjectModel;

    public class Address : ClrObject
    {
        public string Road { get; set; }
        public string City { get; set; }
    }

    public class Person : ClrObject
    {
        public int PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Address Address { get; set; }

        public Collection<string> Descriptions { get; set; }

        public EntityCollection<Product> Products { get; set; }
    }

    public class NumberCombo : ClrObject
    {
        public int Small { get; set; }
        public long Middle { get; set; }
        public decimal Large { get; set; }
    }

    public class Product : ClrObject
    {
        public int ProductId { get; set; }

        public long Quantity { get; set; }

        public decimal LifeTimeInSeconds { get; set; }

        public NumberCombo TheCombo { get; set; }

        public Collection<decimal> LargeNumbers { get; set; }
    }
}
