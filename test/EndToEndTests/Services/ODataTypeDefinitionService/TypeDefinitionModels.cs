//---------------------------------------------------------------------
// <copyright file="TypeDefinitionModels.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace microsoft.odata.sampleService.models.typedefinition
{
    using Microsoft.Test.OData.Services.ODataWCFService;
    using System;
    using System.Collections.ObjectModel;

    public class Address : ClrObject
    {
        public string Road { get; set; }
        public string City { get; set; }
    }

    public class Person : ClrObject
    {
        public int PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Address Address { get; set; }

        public Collection<string> Descriptions { get; set; }
    }

    public class NumberCombo : ClrObject
    {
        public UInt16 Small { get; set; }
        public UInt32 Middle { get; set; }
        public UInt64 Large { get; set; }
    }

    public class Product : ClrObject
    {
        public UInt16 ProductId { get; set; }

        public UInt32 Quantity { get; set; }

        public UInt32? NullableUInt32 { get; set; }

        public UInt64 LifeTimeInSeconds { get; set; }

        public NumberCombo TheCombo { get; set; }

        public Collection<UInt64> LargeNumbers { get; set; }
    }
}
