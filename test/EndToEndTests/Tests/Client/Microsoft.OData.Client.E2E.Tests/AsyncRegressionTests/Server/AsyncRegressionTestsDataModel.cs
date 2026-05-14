//-----------------------------------------------------------------------------
// <copyright file="AsyncRegressionTestsDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.OData.ModelBuilder;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRegressionTests.Server;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Order> Orders { get; set; } = new();
    public Stream Photo { get; set; } = Stream.Null;
}

public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public Customer Customer { get; set; } = null!;
}

[MediaType]
public class MediaAsset
{
    public int Id { get; set; }
}
