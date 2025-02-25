//-----------------------------------------------------------------------------
// <copyright file="PayloadValueConverterDataModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.OData.Client.E2E.Tests.PayloadValueConverterTests.Server
{
    public class ContactInfo : OpenClrObject
    {
        public string? N { get; set; }
    }

    /// <summary>
    /// The class represents the Person model type.
    /// </summary>
    public class Person : ClrObject
    {
        public int Id { get; set; }

        public byte[]? Picture { get; set; }

        public List<int>? Numbers { get; set; }

        public ContactInfo? BusinessCard { get; set; }
    }

    public class Product : ClrObject
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public ProductInfo? Info { get; set; }
    }

    public class ProductInfo : ClrObject
    {
        public string? Site { get; set; }

        public long Serial { get; set; }
    }

    [Serializable]
    public abstract class ClrObject
    {
        protected ClrObject()
        {
            this.UpdatedTime = DateTime.Now;
        }

        public DateTime UpdatedTime { get; set; }
    }

    [Serializable]
    public abstract class OpenClrObject : ClrObject
    {
        //Open properties
        private readonly Dictionary<string, object> openProperties = new Dictionary<string, object>();

        public Dictionary<string, object> OpenProperties
        {
            get
            {
                return openProperties;
            }
        }
    }
}
