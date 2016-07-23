//---------------------------------------------------------------------
// <copyright file="Data.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CustomInstanceAnnotationsTests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData;

    public static class Data<T>
    {
        private static readonly DateTimeOffset CachedNow = DateTimeOffset.Now;
        private static readonly DateTimeOffset CachedUtcNow = DateTimeOffset.UtcNow;
        private static CoordinateSystem UnitSphereCoordinateSystem = CoordinateSystem.Geography(104001);

        private static object[] rawValues =
        {
            // Binary
            new Byte[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 },

            // Boolean
            true,
            false,

            // Byte
            Byte.MinValue,
            Byte.MaxValue,
            (Byte)0,
            (Byte)1,

            // DateTimeOffset
            DateTimeOffset.MinValue,
            DateTimeOffset.MaxValue,
            CachedNow,
            CachedUtcNow,

            // Decimal
            Decimal.MaxValue,
            Decimal.MinusOne,
            Decimal.MinValue,
            Decimal.One,
            Decimal.Zero,

            // Double
            Double.Epsilon,
            Double.MaxValue,
            Double.MinValue,
            Double.NaN,
            Double.NegativeInfinity,
            Double.PositiveInfinity,
            -Double.Epsilon,
            (Double)0,

            //Int16
            (Int16)Byte.MinValue,
            (Int16)Byte.MaxValue,
            Int16.MaxValue,
            Int16.MinValue,
            (Int16)(-1),
            (Int16)0,
            (Int16)1,

            // Int32
            (Int32)Byte.MinValue,
            (Int32)Byte.MaxValue,
            (Int32)Int16.MaxValue,
            (Int32)Int16.MinValue,
            (Int32)Int32.MaxValue,
            (Int32)Int32.MinValue,
            (Int32)(-1),
            (Int32)0,
            (Int32)1,

            // Int64
            (Int64)Byte.MinValue,
            (Int64)Byte.MaxValue,
            (Int64)Int16.MaxValue,
            (Int64)Int16.MinValue,
            (Int64)Int32.MaxValue,
            (Int64)Int32.MinValue,
            (Int64)Int64.MaxValue,
            (Int64)Int64.MinValue,
            (Int64)(-1),
            (Int64)0,
            (Int64)1,

            // SByte
            SByte.MaxValue,
            SByte.MinValue,
            (SByte)(-1),
            (SByte)0,
            (SByte)1,

            // String
            String.Empty,
            Environment.NewLine,

            // Single
            Single.Epsilon,
            Single.MaxValue,
            Single.MinValue,
            Single.NaN,
            Single.NegativeInfinity,
            Single.PositiveInfinity,
            -Single.Epsilon,
            (Single)0,

            // Geography
            GeographyPoint.Create(0, 0),
            GeographyPoint.Create(0, 0, 0),
            GeographyPoint.Create(0, 0, 0, 0),
            GeographyPoint.Create(0, 0, 0, 0),
            GeographyPoint.Create(UnitSphereCoordinateSystem, 0, 0, 0, 0),
            GeographyPoint.Create(Double.Epsilon, Double.Epsilon),
            GeographyPoint.Create(Double.Epsilon, Double.Epsilon, Double.Epsilon),
            GeographyPoint.Create(Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon),
            GeographyPoint.Create(UnitSphereCoordinateSystem, Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon),

            // Geometry
            GeometryPoint.Create(0, 0),
            GeometryPoint.Create(0, 0, 0),
            GeometryPoint.Create(0, 0, 0, 0),
            GeometryPoint.Create(UnitSphereCoordinateSystem, 0, 0, 0, 0),
            GeometryPoint.Create(Double.Epsilon, Double.Epsilon),
            GeometryPoint.Create(Double.Epsilon, Double.Epsilon, Double.Epsilon),
            GeometryPoint.Create(Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon),
            GeometryPoint.Create(UnitSphereCoordinateSystem, Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon),
        };

        private static Dictionary<Type, string> CollectionTypeNames = new Dictionary<Type, string>()
        {
            { typeof(Byte[]), "Collection(Edm.Binary)" },
            { typeof(Boolean), "Collection(Edm.Boolean)" },
            { typeof(Byte), "Collection(Edm.Byte)" },
            { typeof(DateTimeOffset), "Collection(Edm.DateTimeOffset)" },
            { typeof(Decimal), "Collection(Edm.Decimal)" },
            { typeof(Double), "Collection(Edm.Double)" },
            { typeof(Int16), "Collection(Edm.Int16)" },
            { typeof(Int32), "Collection(Edm.Int32)" },
            { typeof(Int64), "Collection(Edm.Int64)" },
            { typeof(SByte), "Collection(Edm.SByte)" },
            { typeof(String), "Collection(Edm.String)" },
            { typeof(Single), "Collection(Edm.Single)" },
            { typeof(Geography), "Collection(Edm.Geography)" },
            { typeof(Geometry), "Collection(Edm.Geometry)" },
        };

        public static IEnumerable<T> Values = rawValues.OfType<T>();

        public static IEnumerable<ODataPrimitiveValue> PrimitiveValues = Values.Select(v => new ODataPrimitiveValue(v));

        public static ODataCollectionValue CollectionValue = new ODataCollectionValue { TypeName = CollectionTypeNames[typeof(T)], Items = Values.Cast<object>() };
    }
}
