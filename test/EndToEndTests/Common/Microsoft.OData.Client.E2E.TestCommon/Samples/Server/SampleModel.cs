//-----------------------------------------------------------------------------
// <copyright file="SampleModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using EfKey = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Microsoft.OData.Client.E2E.TestCommon.Samples.Server
{
    public class Product
    {
        [EfKey]
        public int ProductId { get; set; }
        public string? Description { get; set; }
        public string? BaseConcurrency { get; set; }
        public Product()
        {
        }
    }

    public partial class OrderLine
    {
        [EfKey]
        public int OrderId { get; set; }
        [EfKey]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }

        public OrderLine()
        {
        }
    }
}
