//---------------------------------------------------------------------
// <copyright file="OperationServiceModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.OData;
using Microsoft.Test.OData.Services.ODataWCFService;
using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

namespace Microsoft.Test.OData.Services.ODataOperationService
{
    public class Address : ClrObject
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }

        public override bool Equals(object obj)
        {
            Address addressObj = obj as Address;
            if (addressObj == null)
            {
                return false;
            }

            return this.Street == addressObj.Street && this.City == addressObj.City &&
                   this.PostalCode == addressObj.PostalCode;
        }

        public override int GetHashCode()
        {
            return this.Street.GetHashCode() ^ this.City.GetHashCode() ^ this.PostalCode.GetHashCode();
        }
    }

    public class HomeAddress : Address
    {
        public string FamilyName { get; set; }

        public override bool Equals(object obj)
        {
            HomeAddress addressObj = obj as HomeAddress;
            if (addressObj == null)
            {
                return false;
            }

            return this.FamilyName == addressObj.FamilyName && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.FamilyName.GetHashCode() ^ base.GetHashCode();
        }
    }

    public class CompanyAddress : Address
    {
        public string CompanyName { get; set; }
    }

    public enum CustomerLevel
    {
        Common = 0,
        Silver = 1,
        Gold = 2,
    }

    public class Customer : ClrObject
    {
        private readonly EntityCollection<Order> orders = new EntityCollection<Order>(DataSourceManager.GetCurrentDataSource<OperationServiceDataSource>().Orders);

        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Collection<string> Emails { get; set; }
        public Address Address { get; set; }
        public CustomerLevel Level { get; set; }

        public EntityCollection<Order> Orders
        {
            get
            {
                return this.orders.Cleanup();
            }
        }
    }

    public class OrderDetail : ClrObject
    {
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
    }

    public class InfoFromCustomer : ClrObject
    {
        public string CustomerMessage { get; set; }
    }

    public class Order : ClrObject
    {
        public int ID { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public Customer Customer { get; set; }
        public List<string> Notes { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public InfoFromCustomer InfoFromCustomer { get; set; }
    }
}
