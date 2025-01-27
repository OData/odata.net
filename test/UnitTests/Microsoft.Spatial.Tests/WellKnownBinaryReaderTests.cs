//---------------------------------------------------------------------
// <copyright file="WellKnownBinaryReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class WellKnownBinaryReaderTests
    {
        private readonly WellKnownBinaryFormatter _formatter = new WellKnownBinaryFormatterImplementation(new DataServicesSpatialImplementation(), new WellKnownBinaryWriterSettings());

        [Fact]
        public void ReadWKBPoint_Works()
        {
            // Empty Point
            ReadAndVerify<GeographyPoint, GeometryPoint>("0101000020E6100000000000000000F8FF000000000000F8FF",
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                });

            // X, Y
            ReadAndVerify<GeographyPoint, GeometryPoint>("0101000020E610000000000000000034400000000000002440",
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    Assert.False(p.IsEmpty);
                    Assert.Equal(10, p.Latitude);
                    Assert.Equal(20, p.Longitude);
                    Assert.Null(p.Z);
                    Assert.Null(p.M);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    Assert.False(p.IsEmpty);
                    Assert.Equal(20, p.X);
                    Assert.Equal(10, p.Y);
                    Assert.Null(p.Z);
                    Assert.Null(p.M);
                });

            // X, Y, Z, M
            ReadAndVerify<GeographyPoint, GeometryPoint>("01B90B00E0E6100000000000000000344000000000000024400000000000003E400000000000004440",
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    Assert.False(p.IsEmpty);
                    Assert.Equal(10, p.Latitude);
                    Assert.Equal(20, p.Longitude);
                    Assert.Equal(30, p.Z.Value);
                    Assert.Equal(40, p.M.Value);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    Assert.False(p.IsEmpty);
                    Assert.Equal(20, p.X);
                    Assert.Equal(10, p.Y);
                    Assert.Equal(30, p.Z.Value);
                    Assert.Equal(40, p.M.Value);
                });
        }

        [Theory]
        [InlineData("01BA0B00E01A00000000000000")] // Little Endian (HasZ, HasM)
        [InlineData("00E0000BBA0000001A00000000")] // Big Endian (HasZ, HasM)
        [InlineData("01020000201A00000000000000")] // Little Endian
        [InlineData("00200000020000001A00000000")] // Big Endian
        public void ReadWKBLineString_ForEmpty_Works(string value)
        {
            ReadAndVerify<GeographyLineString, GeometryLineString>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(26, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(26, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                });
        }

        [Theory]
        [InlineData("01020000207E000000020000000000000000002440000000000000344033333333333324409A99999999193440")] // Little Endian
        [InlineData("00200000020000007E00000002402400000000000040340000000000004024333333333333403419999999999A")] // Big Endian
        [InlineData("01020000A07E0000000200000000000000000024400000000000003440000000000000F8FF33333333333324409A99999999193440000000000000F8FF")] // Little Endian (HasZ)
        [InlineData("01020000E07E0000000200000000000000000024400000000000003440000000000000F8FF000000000000F8FF33333333333324409A99999999193440000000000000F8FF000000000000F8FF")] // Little Endian (HasZ, HasM)
        public void ReadWKBLineString_WithTwoPoints_NoIsoWKB_Works(string value)
        {
            ReadAndVerify<GeographyLineString, GeometryLineString>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(126, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsLineString(new PositionData(20, 10, null, null), new PositionData(20.1, 10.1, null, null));
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(126, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsLineString(new PositionData(10, 20, null, null), new PositionData(10.1, 20.1, null, null));
                });
        }


        [Theory]
        [InlineData("01BA0B00E02C00000002000000000000000000244000000000000034400000000000003E40000000000000444000000000000044400000000000003E4000000000000034400000000000002440")] // Little Endian
        [InlineData("00E0000BBA0000002C0000000240240000000000004034000000000000403E00000000000040440000000000004044000000000000403E00000000000040340000000000004024000000000000")] // Big Endian
        public void ReadWKBLineString_WithTwoPointsWithZAndM_Works(string value)
        {
            ReadAndVerify<GeographyLineString, GeometryLineString>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(44, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsLineString(new PositionData(20, 10, 30, 40), new PositionData(30, 40, 20, 10));
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(44, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(40, 30, 20, 10));
                });
        }

        [Theory]
        [InlineData("010300000000000000")] // Little Endian
        [InlineData("000000000300000000")] // Big Endian
        public void ReadWKBPolygon_ForEmpty_StandardWKB_Works(string value)
        {
            ReadAndVerify<GeographyPolygon, GeometryPolygon>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(CoordinateSystem.DefaultGeography.EpsgId, p.CoordinateSystem.EpsgId);
                    Assert.True(p.IsEmpty);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(CoordinateSystem.DefaultGeometry.EpsgId, p.CoordinateSystem.EpsgId);
                    Assert.True(p.IsEmpty);
                });
        }

        [Theory]
        [InlineData("01030000000400000004000000000000000000244000000000000034400000000000002E40000000000000394000000000000034400000000000003E4000000000000024400000000000003440040000000000000000002E40000000000000394000000000000034400000000000003E40000000000000394000000000008041400000000000002E400000000000003940000000000400000000000000000014400000000000001440000000000000184000000000000018400000000000001C400000000000001C4000000000000014400000000000001440")]
        [InlineData("0000000003000000040000000440240000000000004034000000000000402E00000000000040390000000000004034000000000000403E0000000000004024000000000000403400000000000000000004402E00000000000040390000000000004034000000000000403E00000000000040390000000000004041800000000000402E000000000000403900000000000000000000000000044014000000000000401400000000000040180000000000004018000000000000401C000000000000401C00000000000040140000000000004014000000000000")]
        public void ReadWKBPolygon_WithRings_StardardWKB_Works(string value)
        {
            // Be noted: the Polygon WKB contains 4 rings, but the third rings is empty (0 points), when reading this empty point ring, it's ignore and therefore there's only 3 rings in the polygon.
            // It's same as WKT logic.
            ReadAndVerify<GeographyPolygon, GeometryPolygon>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(CoordinateSystem.DefaultGeography.EpsgId, p.CoordinateSystem.EpsgId);
                    p.VerifyAsPolygon(
                        [new PositionData(20, 10), new PositionData(25, 15), new PositionData(30, 20), new PositionData(20, 10)],
                        [new PositionData(25, 15), new PositionData(30, 20), new PositionData(35, 25), new PositionData(25, 15)],
                        [new PositionData(5, 5), new PositionData(6, 6), new PositionData(7, 7), new PositionData(5, 5)]);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(CoordinateSystem.DefaultGeometry.EpsgId, p.CoordinateSystem.EpsgId);
                    p.VerifyAsPolygon(
                        [new PositionData(10, 20), new PositionData(15, 25), new PositionData(20, 30), new PositionData(10, 20)],
                        [new PositionData(15, 25), new PositionData(20, 30), new PositionData(25, 35), new PositionData(15, 25)],
                        [new PositionData(5, 5), new PositionData(6, 6), new PositionData(7, 7), new PositionData(5, 5)]);
                });
        }

        [Theory]
        [InlineData("0104000020E610000000000000")] // Little Endian (X, Y)
        [InlineData("0020000004000010E600000000")] // Big Endian (X, Y)
        [InlineData("01BC0B00E0E610000000000000")] // Little Endian (HasZ, HasM)
        [InlineData("00E0000BBC000010E600000000")] // Big Endian (HasZ, HasM)
        public void ReadWKBMultiPoints_ForEmpty_Works(string value)
        {
            ReadAndVerify<GeographyMultiPoint, GeometryMultiPoint>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                });
        }

        [Theory]
        [InlineData("0104000020E6100000020000000101000000000000000000F8FF000000000000F8FF0101000000000000000000F8FF000000000000F8FF")] // Little Endian
        [InlineData("0020000004000010E6000000020000000001FFF8000000000000FFF80000000000000000000001FFF8000000000000FFF8000000000000")] // Big Endian
        [InlineData("01BC0B00E0E61000000200000001B90B00C0000000000000F8FF000000000000F8FF000000000000F8FF000000000000F8FF01B90B00C0000000000000F8FF000000000000F8FF000000000000F8FF000000000000F8FF")]
        public void ReadMultiPoints_WithTwoEmptyPoints_Works(string value)
        {
            ReadAndVerify<GeographyMultiPoint, GeometryMultiPoint>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsMultiPoint(null, null);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsMultiPoint(null, null);
                });
        }

        [Theory]
        [InlineData("0104000020E6100000030000000101000000000000000000244000000000000034400101000000000000000000F8FF000000000000F8FF01010000000000000000003E400000000000004440")] // Little Endian
        [InlineData("0020000004000010E6000000030000000001402400000000000040340000000000000000000001FFF8000000000000FFF80000000000000000000001403E0000000000004044000000000000")] // Big Endian
        [InlineData("01BC0B00E0E61000000300000001B90B00C000000000000024400000000000003440000000000000F8FF000000000000F8FF01B90B00C0000000000000F8FF000000000000F8FF000000000000F8FF000000000000F8FF01B90B00C00000000000003E400000000000004440000000000000F8FF000000000000F8FF")]
        public void ReadMultiPoints_WithPoints_Works(string value)
        {
            ReadAndVerify<GeographyMultiPoint, GeometryMultiPoint>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsMultiPoint(new PositionData(20, 10), null, new PositionData(40, 30));
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(4326, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsMultiPoint(new PositionData(10, 20), null, new PositionData(30, 40));
                });
        }

        [Theory]
        [InlineData("01BD0B00E00F00000000000000")] // Little Endian (X, Y, HasZ, HasM)
        [InlineData("00E0000BBD0000000F00000000")] // Big Endian (X, Y, HasZ, HasM)
        [InlineData("01050000200F00000000000000")] // Little Endian
        [InlineData("00200000050000000F00000000")] // Big Endian
        public void ReadWKBMultiLineString_ForEmpty_Works(string value)
        {
            ReadAndVerify<GeographyMultiLineString, GeometryMultiLineString>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(15, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(15, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                });
        }

        [Theory]
        [InlineData("0105000020010000000300000001020000000200000000000000000024400000000000002440000000000000344000000000000034400102000000000000000102000000030000000000000000003E400000000000003E400000000000004440000000000000444000000000000049400000000000004940")] // Little Endian (X, Y)
        [InlineData("002000000500000001000000030000000002000000024024000000000000402400000000000040340000000000004034000000000000000000000200000000000000000200000003403E000000000000403E0000000000004044000000000000404400000000000040490000000000004049000000000000")] // Big Endian (X, Y)
        public void ReadWKBMultiLineString_WithLineStrings_Works(string value)
        {
            ReadAndVerify<GeographyMultiLineString, GeometryMultiLineString>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(1, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsMultiLineString(
                        [ new PositionData(10, 10), new PositionData(20, 20)],
                        null,
                        [ new PositionData(30, 30), new PositionData(40, 40), new PositionData(50, 50)]);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(1, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsMultiLineString(
                         [new PositionData(10, 10), new PositionData(20, 20)],
                         null,
                         [new PositionData(30, 30), new PositionData(40, 40), new PositionData(50, 50)]);
                });
        }

        [Theory]
        [InlineData("01BE0B00E01700000000000000")] // Little Endian
        [InlineData("00E0000BBE0000001700000000")] // Big Endia
        public void ReadWKBMultiPolygon_ForEmpty_Works(string value)
        {
            ReadAndVerify<GeographyMultiPolygon, GeometryMultiPolygon>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(23, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(23, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                });
        }

        [Theory]
        [InlineData("01060000201700000003000000010300000002000000040000000000000000002440000000000000344000000000000034400000000000003E400000000000003E40000000000000444000000000000024400000000000003440040000000000000000000840000000000000104000000000000010400000000000001440000000000000144000000000000018400000000000000840000000000000104001030000000000000001030000000100000004000000000000000000F03F00000000000000400000000000000040000000000000084000000000000008400000000000001040000000000000F03F0000000000000040")] // Little Endian (X, Y)
        [InlineData("0020000006000000170000000300000000030000000200000004402400000000000040340000000000004034000000000000403E000000000000403E0000000000004044000000000000402400000000000040340000000000000000000440080000000000004010000000000000401000000000000040140000000000004014000000000000401800000000000040080000000000004010000000000000000000000300000000000000000300000001000000043FF0000000000000400000000000000040000000000000004008000000000000400800000000000040100000000000003FF00000000000004000000000000000")] // Big Endian (X, Y)
        public void ReadWKBMultiPolygon_WithPolygons_Works(string value)
        {
            ReadAndVerify<GeographyMultiPolygon, GeometryMultiPolygon>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(23, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsMultiPolygon(
                        [[new PositionData(20, 10), new PositionData(30, 20), new PositionData(40, 30), new PositionData(20, 10)], [new PositionData(4, 3), new PositionData(5, 4), new PositionData(6, 5), new PositionData(4, 3)]],
                        null,
                        [[new PositionData(2, 1), new PositionData(3, 2), new PositionData(4, 3), new PositionData(2, 1)]]);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(23, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsMultiPolygon(
                        [[new PositionData(10, 20), new PositionData(20, 30), new PositionData(30, 40), new PositionData(10, 20)], [new PositionData(3, 4), new PositionData(4, 5), new PositionData(5, 6), new PositionData(3, 4)]],
                        null,
                        [[new PositionData(1, 2), new PositionData(2, 3), new PositionData(3, 4), new PositionData(1, 2)]]);
                });
        }

        [Theory]
        [InlineData("01BF0B00E02E00000000000000")] // Little Endian
        [InlineData("00E0000BBF0000002E00000000")] // Big Endia
        public void ReadWKBMultiCollection_ForEmpty_Works(string value)
        {
            ReadAndVerify<GeographyCollection, GeometryCollection>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(46, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(46, p.CoordinateSystem.EpsgId.Value);
                    Assert.True(p.IsEmpty);
                });
        }

        [Theory]
        [InlineData("01070000000400000001010000000000000000002440000000000000344001020000000200000000000000000034400000000000003E4000000000000044400000000000004940010300000000000000010700000001000000010100000000000000000010400000000000001440")] // Little Endian
        [InlineData("0000000007000000040000000001402400000000000040340000000000000000000002000000024034000000000000403E00000000000040440000000000004049000000000000000000000300000000000000000700000001000000000140100000000000004014000000000000")] // Big Endian
        public void ReadWKBCollection_WithSpatials_StandardWKB_Works(string value)
        {
            ReadAndVerify<GeographyCollection, GeometryCollection>(value,
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(CoordinateSystem.DefaultGeography.EpsgId, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsCollection(
                        (g) => g.VerifyAsPoint(new PositionData(20, 10)),
                        (g) => g.VerifyAsLineString(new PositionData(30, 20), new PositionData(50, 40)),
                        (g) => g.VerifyAsPolygon(null),
                        (g) => g.VerifyAsCollection(
                        (g1) => g1.VerifyAsPoint(new PositionData(5, 4))));
                },
                p =>
                {
                    Assert.NotNull(p.CoordinateSystem);
                    Assert.Equal(CoordinateSystem.DefaultGeometry.EpsgId, p.CoordinateSystem.EpsgId.Value);
                    p.VerifyAsCollection(
                        (g) => g.VerifyAsPoint(new PositionData(10, 20)),
                        (g) => g.VerifyAsLineString(new PositionData(20, 30), new PositionData(40, 50)),
                        (g) => g.VerifyAsPolygon(null),
                        (g) => g.VerifyAsCollection(
                        (g1) => g1.VerifyAsPoint(new PositionData(4, 5))));
                });
        }

        private void ReadAndVerify<TSpatial1, TSpatial2>(string bytes, Action<TSpatial1> verify1, Action<TSpatial2> verify2)
            where TSpatial1 : class, ISpatial
            where TSpatial2 : class, ISpatial
        {
            byte[] bs = HexToBytes(bytes);
            if (verify1 != null)
            {
                using (var stream1 = new MemoryStream(bs))
                {
                    var spatial1 = _formatter.Read<TSpatial1>(stream1);
                    verify1(spatial1);
                }
            }

            if (verify2 != null)
            {
                using (var stream2 = new MemoryStream(bs))
                {
                    var spatial2 = _formatter.Read<TSpatial2>(stream2);
                    verify2(spatial2);
                }
            }
        }

        private static byte[] HexToBytes(string hex)
        {
            int byteLen = hex.Length / 2;
            byte[] bytes = new byte[byteLen];

            for (int i = 0; i < hex.Length / 2; i++)
            {
                int i2 = 2 * i;
                if (i2 + 1 > hex.Length)
                    throw new ArgumentException("Hex string has odd length");

                int nib1 = HexToInt(hex[i2]);
                int nib0 = HexToInt(hex[i2 + 1]);
                bytes[i] = (byte)((nib1 << 4) + (byte)nib0);
            }
            return bytes;
        }

        private static int HexToInt(char hex)
        {
            switch (hex)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return hex - '0';
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return hex - 'A' + 10;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    return hex - 'a' + 10;
            }
            throw new ArgumentException("Invalid hex digit: " + hex);
        }
    }
}
