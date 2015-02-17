//---------------------------------------------------------------------
// <copyright file="ReflectionCompoundKeyContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace AstoriaUnitTests.CompoundKeyContext.ReflectionCompoundKeyContext
{
    using Microsoft.OData.Client;

    public class ReflectionCompoundKeyContext
    {
        public IQueryable<Customer> Customers { get { return new List<Customer>().AsQueryable(); } }
        public IQueryable<Order> Orders { get { return new List<Order>().AsQueryable(); } }
    }

    [Key("Id")]
    public class Customer
    {
        public int Id { get; set; }
        public List<Order> Orders { get; set; }
    }

    [Key("bKey", "AKey")]
    public class Order
    {
        public int bKey { get; set; }
        public int AKey { get; set; }
        public Customer Customer { get; set; }
    }
}
