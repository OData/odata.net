//---------------------------------------------------------------------
// <copyright file="WellKnownBinaryReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Reads and Converts a Well-Known Binary (WKB) byte data to a spatial data.
    /// It supports Extended WKB format meanwhile, for compatiblilty, suppport ISO WKB formatter.
    /// </summary>
    internal class WellKnownBinaryReader : SpatialReader<Stream>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownBinaryReader"/> class.
        /// </summary>
        /// <param name="destination">The spatial pipelien destination.</param>
        public WellKnownBinaryReader(SpatialPipeline destination)
            : base(destination)
        {
        }

        /// <summary>
        /// Parses some serialized format that represents one or more Geography spatial values, passing the first one down the pipeline.
        /// </summary>
        /// <param name="input">The input stream.</param>
        protected override void ReadGeographyImplementation(Stream input)
        {
            // Geography in WKB has lat/long reversed, should be y (long), x (lat), z, m
            TypeWashedPipeline pipeline = new TypeWashedToGeographyLongLatPipeline(Destination);
            Read(pipeline, input, CoordinateSystem.DefaultGeography);
        }

        /// <summary>
        /// Parses some serialized format that represents one or more Geometry spatial values, passing the first one down the pipeline.
        /// </summary>
        /// <param name="input">The input stream.</param>
        protected override void ReadGeometryImplementation(Stream input)
        {
            TypeWashedPipeline pipeline = new TypeWashedToGeometryPipeline(Destination);
            Read(pipeline, input, CoordinateSystem.DefaultGeometry);
        }

        /// <summary>
        /// Reads a <see cref="Geometry"/> or <see cref="Geography"/> in binary WKB format from an <see cref="Stream"/>.
        /// </summary>
        /// <param name="pipeline">The spatial pipeline to read from.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="defaultCoordinate">The default coordinate.</param>
        private static void Read(TypeWashedPipeline pipeline, Stream stream, CoordinateSystem defaultCoordinate)
        {
            Debug.Assert(pipeline != null);

            using (var reader = new BinaryReader(stream))
            {
                try
                {
                    ReadSpatial(pipeline, reader, defaultCoordinate);
                }
                catch (FormatException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw new FormatException($"Reading the spatial type failed, see details in inner exception.", ex);
                }
            }
        }

        private static void ReadSpatial(TypeWashedPipeline pipeline, BinaryReader reader, CoordinateSystem defaultCoordinate)
        {
            ByteOrder byteOrder = ReadByteOrder(reader);
            Header header = ReadHeader(reader, byteOrder);
            if (header.Srid >= 0)
            {
                pipeline.SetCoordinateSystem(header.Srid);
            }
            else if (defaultCoordinate != null)
            {
                pipeline.SetCoordinateSystem(defaultCoordinate?.EpsgId);
            }

            header.ByteOrder = byteOrder;
            switch (header.Type)
            {
                case SpatialType.Point:
                    ReadPoint(pipeline, reader, header);
                    break;

                case SpatialType.LineString:
                    ReadLineString(pipeline, reader, header, true);
                    break;

                case SpatialType.Polygon:
                    ReadPolygon(pipeline, reader, header);
                    break;

                case SpatialType.MultiPoint:
                    ReadMultiPoint(pipeline, reader, header);
                    break;

                case SpatialType.MultiLineString:
                    ReadMultiLineString(pipeline, reader, header);
                    break;

                case SpatialType.MultiPolygon:
                    ReadMultiPolygon(pipeline, reader, header);
                    break;

                case SpatialType.Collection:
                    ReadCollection(pipeline, reader, header);
                    break;

                default:
                    throw new FormatException($"Spatial type {header.Type} not recognized.");
            }
        }

        private static void ReadPoint(TypeWashedPipeline pipeline, BinaryReader reader, Header header)
        {
            (double x, double y, double? z, double? m) = ReadPoint(reader, header);

            pipeline.BeginGeo(SpatialType.Point);

            if (!IsEmptyPoint(x, y))
            {
                pipeline.BeginFigure(x, y, z, m);
                pipeline.EndFigure();
            }

            pipeline.EndGeo();
        }

        private static void ReadLineString(TypeWashedPipeline pipeline, BinaryReader reader, Header header, bool hasGeo = true)
        {
            // Read the num of points in the LineString
            int num = reader.ReadInt32(header.ByteOrder);

            if (hasGeo)
            {
                pipeline.BeginGeo(SpatialType.LineString);
            }

            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    (double x, double y, double? z, double? m) = ReadPoint(reader, header);
                    if (i == 0)
                    {
                        pipeline.BeginFigure(x, y, z, m);
                    }
                    else
                    {
                        pipeline.LineTo(x, y, z, m);
                    }
                }

                pipeline.EndFigure();
            }

            if (hasGeo)
            {
                pipeline.EndGeo();
            }
        }

        private static void ReadPolygon(TypeWashedPipeline pipeline, BinaryReader reader, Header header)
        {
            // Read the num of Rings in Polygon
            int num = reader.ReadInt32(header.ByteOrder);

            pipeline.BeginGeo(SpatialType.Polygon);

            for (int i = 0; i < num; i++)
            {
                ReadLineString(pipeline, reader, header, false);
            }

            pipeline.EndGeo();
        }

        private static void ReadMultiPoint(TypeWashedPipeline pipeline, BinaryReader reader, Header header)
        {
            // read the number of the points
            int num = reader.ReadInt32(header.ByteOrder);

            pipeline.BeginGeo(SpatialType.MultiPoint);

            for (int i = 0; i < num; i++)
            {
                ReadSpatial(pipeline, reader, null);
            }

            pipeline.EndGeo();
        }

        private static void ReadMultiLineString(TypeWashedPipeline pipeline, BinaryReader reader, Header header)
        {
            // read the number of the LineStrings
            int num = reader.ReadInt32(header.ByteOrder);

            pipeline.BeginGeo(SpatialType.MultiLineString);

            for (int i = 0; i < num; i++)
            {
                ReadSpatial(pipeline, reader, null);
            }

            pipeline.EndGeo();
        }

        private static void ReadMultiPolygon(TypeWashedPipeline pipeline, BinaryReader reader, Header header)
        {
            // read the number of the Polygon
            int num = reader.ReadInt32(header.ByteOrder);

            pipeline.BeginGeo(SpatialType.MultiPolygon);

            for (int i = 0; i < num; i++)
            {
                ReadSpatial(pipeline, reader, null);
            }

            pipeline.EndGeo();
        }

        private static void ReadCollection(TypeWashedPipeline pipeline, BinaryReader reader, Header header)
        {
            // read the number of the items in the collection
            int num = reader.ReadInt32(header.ByteOrder);

            pipeline.BeginGeo(SpatialType.Collection);

            for (int i = 0; i < num; i++)
            {
                ReadSpatial(pipeline, reader, null);
            }

            pipeline.EndGeo();
        }

        /// <summary>
        /// Function to read a coordinate sequence.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="header">The header information.</param>
        /// <returns>The read coordinate.</returns>
        private static (double, double, double?, double?) ReadPoint(BinaryReader reader, Header header)
        {
            double x = reader.ReadDouble(header.ByteOrder);
            double y = reader.ReadDouble(header.ByteOrder);

            double? z = null;
            if (header.HasZ)
            {
                double zV = reader.ReadDouble(header.ByteOrder);
                if (!double.IsNaN(zV))
                {
                    z = zV;
                }
            }

            double? m = null;
            if (header.HasM)
            {
                double mV = reader.ReadDouble(header.ByteOrder);
                if (!double.IsNaN(mV))
                {
                    m = mV;
                }
            }

            return (x, y, z, m);
        }

        private static bool IsEmptyPoint(double x, double y)
        {
            return double.IsNaN(x) || double.IsNaN(y);
        }

        private static ByteOrder ReadByteOrder(BinaryReader reader)
        {
            byte byteOrder = reader.ReadByte();
            if (byteOrder == 0)
            {
                return ByteOrder.BigEndian;
            }
            else if (byteOrder == 1)
            {
                return ByteOrder.LittleEndian;
            }

            throw new FormatException(Error.Format(SRResources.WellKnownBinary_UnknownByteOrder, byteOrder));
        }

        private static Header ReadHeader(BinaryReader reader, ByteOrder byteOrder)
        {
            uint type = reader.ReadUInt32(byteOrder);

            Header header = new Header();
            header.HasZ = false;
            header.HasM = false;

            // Determine Z, M, SRID existed for extended WKB
            if ((type & (0x80000000 | 0x40000000)) == (0x80000000 | 0x40000000))
            {
                header.HasZ = true;
                header.HasM = true;
            }
            else if ((type & 0x80000000) == 0x80000000)
            {
                header.HasZ = true;
            }
            else if ((type & 0x40000000) == 0x40000000)
            {
                header.HasM = true;
            }

            // Has SRID
            header.Srid = (type & 0x20000000) != 0 ? reader.ReadInt32(byteOrder) : -1;

            // Support TopologySuit
            uint ordinate = (type & 0xffff) / 1000;
            switch (ordinate)
            {
                case 1:
                    header.HasZ = true;
                    header.HasM = false;
                    break;
                case 2:
                    header.HasZ = false;
                    header.HasM = true;
                    break;
                case 3:
                    header.HasZ = true;
                    header.HasM = true;
                    break;
            }

            header.Type = (SpatialType)((type & 0xffff) % 1000);
            return header;
        }

        private class Header
        {
            public ByteOrder ByteOrder { get; set; }

            public SpatialType Type { get; set; }

            public bool HasZ { get; set; }

            public bool HasM { get; set; }

            public int Srid { get; set; }
        }
    }
}
