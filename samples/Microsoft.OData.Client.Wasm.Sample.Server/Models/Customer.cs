//---------------------------------------------------------------------
// <copyright file="Customer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Wasm.Sample.Server.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Order> Orders { get; set; } = new();
    public Stream Photo { get; set; } = Stream.Null;
}
