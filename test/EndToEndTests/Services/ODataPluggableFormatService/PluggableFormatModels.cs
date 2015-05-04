//---------------------------------------------------------------------
// <copyright file="PluggableFormatModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
    using System.Collections.Generic;
    using Microsoft.Test.OData.Services.ODataWCFService;

    public class ContactInfo : OpenClrObject
    {
        public string N { get; set; }
    }

    /// <summary>
    /// The class represents the Person model type.
    /// </summary>
    public class Person : ClrObject
    {
        public int Id { get; set; }

        public byte[] Picture { get; set; }

        public List<int> Numbers { get; set; }

        public ContactInfo BusinessCard { get; set; }
    }

    public class Product : ClrObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ProductInfo Info { get; set; }
    }

    public class ProductInfo : ClrObject
    {
        public string Site { get; set; }
        
        public long Serial { get; set; }
    }
}
