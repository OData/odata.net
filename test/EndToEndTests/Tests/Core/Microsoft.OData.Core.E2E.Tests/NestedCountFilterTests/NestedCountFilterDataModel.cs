//---------------------------------------------------------------------
// <copyright file="NestedCountFilterDataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.E2E.Tests.NestedCountFilterTests;

public class StoreCustomer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<StoreOrder> Orders { get; set; } = new();
}

public class StoreOrder
{
    public int Id { get; set; }
    public List<StoreOrderItem> Items { get; set; } = new();
}

public class StoreOrderItem
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public StoreProduct Product { get; set; } = null!;
}

public class StoreProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<StoreReview> Reviews { get; set; } = new();
}

public class StoreReview
{
    public int Id { get; set; }
    public int Rating { get; set; }
}

