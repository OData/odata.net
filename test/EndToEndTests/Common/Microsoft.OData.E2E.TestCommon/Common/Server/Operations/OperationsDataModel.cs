//---------------------------------------------------------------------
// <copyright file="OperationsDataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.Operations;

public class Address : AbstractEntity
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Address addressObj)
        {
            return false;
        }

        return this.Street == addressObj.Street && this.City == addressObj.City &&
               this.PostalCode == addressObj.PostalCode;
    }

    public override int GetHashCode()
    {
        return (this.Street?.GetHashCode() ?? 0) ^ (this.City?.GetHashCode() ?? 0) ^ (this.PostalCode?.GetHashCode() ?? 0);
    }
}

public class HomeAddress : Address
{
    public string? FamilyName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not HomeAddress addressObj)
        {
            return false;
        }

        return this.FamilyName == addressObj.FamilyName && base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return (this.FamilyName?.GetHashCode() ?? 0) ^ base.GetHashCode();
    }
}

public class CompanyAddress : Address
{
    public string? CompanyName { get; set; }
}

public enum CustomerLevel
{
    Common = 0,
    Silver = 1,
    Gold = 2,
}

public class Customer : AbstractEntity
{
    private readonly Collection<Order> orders = new Collection<Order>();

    [EfKey]
    public int CustomerID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Collection<string>? Emails { get; set; }
    public Address? Address { get; set; }
    public CustomerLevel Level { get; set; }

    public Collection<Order> Orders
    {
        get
        {
            return this.orders;
        }
    }
}

public class OrderDetail : AbstractEntity
{
    public int Quantity { get; set; }
    public float UnitPrice { get; set; }
}

public class InfoFromCustomer : AbstractEntity
{
    public string? CustomerMessage { get; set; }
}

public class Order : AbstractEntity
{
    [EfKey]
    public int OrderID { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public Customer? Customer { get; set; }
    public List<string>? Notes { get; set; }
    public List<OrderDetail>? OrderDetails { get; set; }
    public InfoFromCustomer? InfoFromCustomer { get; set; }
}

public abstract class AbstractEntity
{
    public DateTimeOffset UpdatedTime { get; set; }
}
