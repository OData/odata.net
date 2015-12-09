//---------------------------------------------------------------------
// <copyright file="PositionData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Data.Spatial;

namespace Microsoft.Spatial.Tests
{
    public class PositionData
    {
        public static readonly List<PositionData> PointData;

        public static readonly List<PositionData[]> RingFigureData;

        private readonly double x;
        private readonly double y;

        public PositionData(double x, double y, double? z, double? m)
        {
            this.x = x;
            this.y = y;
            this.Z = z;
            this.M = m;
        }

        public PositionData(double x, double y)
            : this(x, y, null, null)
        {
        }

        static PositionData()
        {
            PointData = new List<PositionData>
                            {
                                new PositionData(10, 20, 30, 40),
                                new PositionData(20, 30, null, 40),
                                new PositionData(30, 40, 10, null),
                                new PositionData(10, 30, null, null),
                                new PositionData(-10, -20, -30, -40),
                            };

            RingFigureData = new List<PositionData[]>
                                 {
                                     new[]
                                         {
                                             new PositionData(10, 20, 30, 40),
                                             new PositionData(20, 30, null, 40),
                                             new PositionData(30, 40, 10, null),
                                             new PositionData(10, 20, 30, 40),
                                         },
                                     new[]
                                         {
                                             new PositionData(20, 30, null, 40),
                                             new PositionData(30, 40, 10, null),
                                             new PositionData(10, 30, null, null),
                                             new PositionData(20, 30, null, 40),
                                         },
                                     new[]
                                         {
                                             new PositionData(-10, -20, -30, -40),
                                             new PositionData(30, 40, 10, null),
                                             new PositionData(10, 30, null, null),
                                             new PositionData(-10, -20, -30, -40),
                                         }
                                 };
        }

        public double Latitude
        {
            get { return this.x; }
        }

        public double Longitude
        {
            get { return this.y; }
        }

        public double? M { get; private set; }

        public double X
        {
            get { return this.x; }
        }

        public double Y
        {
            get { return this.y; }
        }

        public double? Z { get; private set; }

        public override bool Equals(object obj)
        {
            var p = obj as PositionData;
            if (p == null)
            {
                return false;
            }

            return this.x == p.x && this.y == p.y && this.Z == p.Z && this.M == p.M;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("({0} {1} {2} {3})",
                                 this.x,
                                 this.y,
                                 this.Z != null ? this.Z.Value.ToString() : "NULL",
                                 this.M != null ? this.M.Value.ToString() : "NULL");
        }

        public GeographyPosition ToGeographyPosition()
        {
            return new GeographyPosition(this.x, this.y, this.Z, this.M);
        }

        public GeometryPosition ToGeometryPosition()
        {
            return new GeometryPosition(this.x, this.y, this.Z, this.M);
        }

        public static void DrawLine(GeographyPipeline pipeline, PositionData[] line)
        {
            for (int i = 0; i < line.Length; ++i)
            {
                if (i == 0)
                {
                    pipeline.BeginFigure(new GeographyPosition(line[i].X, line[i].Y, line[i].Z, line[i].M));
                }
                else
                {
                    pipeline.LineTo(new GeographyPosition(line[i].X, line[i].Y, line[i].Z, line[i].M));
                }
            }

            pipeline.EndFigure();
        }

        public static void DrawLine(GeometryPipeline pipeline, PositionData[] line)
        {
            for (int i = 0; i < line.Length; ++i)
            {
                PositionData currentLine = line[i];
                if (i == 0)
                {
                    pipeline.BeginFigure(new GeometryPosition(currentLine.X, currentLine.Y, currentLine.Z, currentLine.M));
                }
                else
                {
                    pipeline.LineTo(new GeometryPosition(currentLine.X, currentLine.Y, currentLine.Z, currentLine.M));
                }
            }

            pipeline.EndFigure();
        }

        public static void DrawPoint(GeographyPipeline pipeline, PositionData point)
        {
            pipeline.BeginFigure(new GeographyPosition(point.X, point.Y, point.Z, point.M));
            pipeline.EndFigure();
        }

        public static void DrawPoint(GeometryPipeline pipeline, PositionData point)
        {
            pipeline.BeginFigure(new GeometryPosition(point.X, point.Y, point.Z, point.M));
            pipeline.EndFigure();
        }

        internal static void DrawLine(TypeWashedPipeline pipeline, PositionData[] line)
        {
            for (int i = 0; i < line.Length; ++i)
            {
                if (i == 0)
                {
                    pipeline.BeginFigure(line[i].X, line[i].Y, line[i].Z, line[i].M);
                }
                else
                {
                    pipeline.LineTo(line[i].X, line[i].Y, line[i].Z, line[i].M);
                }
            }

            pipeline.EndFigure();
        }

        internal static void DrawPoint(TypeWashedPipeline pipeline, PositionData point)
        {
            pipeline.BeginFigure(point.X, point.Y, point.Z, point.M);
            pipeline.EndFigure();
        }
    }
}