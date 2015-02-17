//---------------------------------------------------------------------
// <copyright file="Northwind.NamedStreamPartial.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace NorthwindModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;

    [NamedStream("Stream1")]
    [NamedStream("Stream2")]
    public partial class Customers : global::System.Data.Objects.DataClasses.EntityObject
    {
    }

    [NamedStream("Stream3")]
    [NamedStream("Stream4")]
    public partial class Order_Details : global::System.Data.Objects.DataClasses.EntityObject
    {
    }

    [NamedStream("Stream1")]
    public partial class Orders : global::System.Data.Objects.DataClasses.EntityObject
    {
    }
}
