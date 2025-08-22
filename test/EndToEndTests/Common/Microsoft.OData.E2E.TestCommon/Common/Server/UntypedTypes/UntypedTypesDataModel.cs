//---------------------------------------------------------------------
// <copyright file="UntypedTypesDataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.E2E.TestCommon.Common.Server.UntypedTypes;

public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public object? UntypedProperty { get; set; } = null;
    public List<object>? UntypedList { get; set; } = null;
    public List<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
}
