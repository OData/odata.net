//---------------------------------------------------------------------
// <copyright file="TypeWithPrimitiveProperties.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.DataClasses
{
    #region Namespaces
    using System;
    #endregion Namespaces

    internal sealed class TypeWithPrimitiveProperties
    {
        public int ID { get; set; }
        public string StringProperty { get; set; }
        public bool BoolProperty { get; set; }
        public bool? NullableBoolProperty { get; set; }
        public byte ByteProperty { get; set; }
        public byte? NullableByteProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public DateTime? NullableDateTimeProperty { get; set; }
        public decimal DecimalProperty { get; set; }
        public decimal? NullableDecimalProperty { get; set; }
        public double DoubleProperty { get; set; }
        public double? NullableDoubleProperty { get; set; }
        public Guid GuidProperty { get; set; }
        public Guid? NullableGuidProperty { get; set; }
        public Int16 Int16Property { get; set; }
        public Int16? NullableInt16Property { get; set; }
        public Int32 Int32Property { get; set; }
        public Int32? NullableInt32Property { get; set; }
        public Int64 Int64Property { get; set; }
        public Int64? NullableInt64Property { get; set; }
        public sbyte SByteProperty { get; set; }
        public sbyte? NullableSByteProperty { get; set; }
        public Single SingleProperty { get; set; }
        public Single? NullableSingleProperty { get; set; }
        public byte[] ByteArrayProperty { get; set; }
        // TODO: not adding streams here (they are also considered primitive though)

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            TypeWithPrimitiveProperties other = obj as TypeWithPrimitiveProperties;
            if (other == null) return false;

            return this.StringProperty == other.StringProperty &&
                this.BoolProperty == other.BoolProperty &&
                this.NullableBoolProperty == other.NullableBoolProperty &&
                this.ByteProperty == other.ByteProperty &&
                this.NullableByteProperty == other.NullableByteProperty &&
                this.DateTimeProperty == other.DateTimeProperty &&
                this.NullableDateTimeProperty == other.NullableDateTimeProperty &&
                this.DecimalProperty == other.DecimalProperty &&
                this.NullableDecimalProperty == other.NullableDecimalProperty &&
                this.DoubleProperty == other.DoubleProperty &&
                this.NullableDoubleProperty == other.NullableDoubleProperty &&
                this.GuidProperty == other.GuidProperty &&
                this.NullableGuidProperty == other.NullableGuidProperty &&
                this.Int16Property == other.Int16Property &&
                this.NullableInt16Property == other.NullableInt16Property &&
                this.Int32Property == other.Int32Property &&
                this.NullableInt32Property == other.NullableInt32Property &&
                this.Int64Property == other.Int64Property &&
                this.NullableInt64Property == other.NullableInt64Property &&
                this.SByteProperty == other.SByteProperty &&
                this.NullableSByteProperty == other.NullableSByteProperty &&
                this.SingleProperty == other.SingleProperty &&
                this.NullableSingleProperty == other.NullableSingleProperty &&
                this.ByteArrayProperty == other.ByteArrayProperty;
        }
    }
}
