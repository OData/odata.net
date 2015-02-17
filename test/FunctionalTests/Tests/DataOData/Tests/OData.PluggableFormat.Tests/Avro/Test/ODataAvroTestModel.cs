//---------------------------------------------------------------------
// <copyright file="ODataAvroTestModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro.Test
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Hadoop.Avro;

    [DataContract]
    internal class Product
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public float Weight { get; set; }

        public override string ToString()
        {
            return string.Format("Id:{0},Weight:{1}", Id, Weight);
        }

        public override bool Equals(object obj)
        {
            Product prd = obj as Product;
            if (prd == null)
            {
                return false;
            }

            return this.Id == prd.Id
                && TestHelper.FloatEqual(this.Weight, prd.Weight);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Weight.GetHashCode();
        }
    }

    [DataContract]
    internal class Address
    {
        [DataMember]
        public string Road { get; set; }

        [DataMember]
        public string ZipCode { get; set; }

        public override string ToString()
        {
            return string.Format("Road:{0},ZipCode:{1}", Road, ZipCode);
        }

        public override bool Equals(object obj)
        {
            Address addr = obj as Address;
            if (addr == null)
            {
                return false;
            }

            return this.Road == addr.Road
                && this.ZipCode == addr.ZipCode;
        }

        public override int GetHashCode()
        {
            return this.Road.GetHashCode() ^ this.Road.GetHashCode();
        }
    }

    [DataContract]
    internal class AddProductParameter
    {
        [DataMember]
        public Product Product { get; set; }

        [DataMember]
        public Address Location { get; set; }

        public override string ToString()
        {
            return string.Format("Product:{0},Location:{1}", Product, Location);
        }

        public override bool Equals(object obj)
        {
            AddProductParameter addr = obj as AddProductParameter;
            if (addr == null)
            {
                return false;
            }

            return this.Product == addr.Product
                && this.Location == addr.Location;
        }

        public override int GetHashCode()
        {
            return this.Product.GetHashCode() ^ this.Location.GetHashCode();
        }
    }

    [DataContract]
    internal class GetMaxIdParameter
    {
        [DataMember]
        [AvroUnion(typeof(AvroNull), typeof(List<Product>))]
        public object Products { get; set; }
    }

    [DataContract(Namespace = "OData")]
    internal class Error
    {
        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
#endif