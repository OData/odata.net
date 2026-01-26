//---------------------------------------------------------------------
// <copyright file="Order.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Wasm.Sample.Server.Models;

public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public Customer Customer { get; set; } = null!;
}
