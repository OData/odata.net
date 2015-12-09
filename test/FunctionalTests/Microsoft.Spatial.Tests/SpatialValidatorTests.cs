//---------------------------------------------------------------------
// <copyright file="SpatialValidatorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class SpatialValidatorTests
    {
        private const int NonDefaultEpsgId = 1234;
        private static readonly Dictionary<String, Action<TypeWashedPipeline>> Transitions = new Dictionary<string, Action<TypeWashedPipeline>>
            {
                { "SetCoordinateSystem", s=>s.SetCoordinateSystem(NonDefaultEpsgId) },
                { "BeginPoint", s=>s.BeginGeo(SpatialType.Point) },
                { "BeginLineString", s=>s.BeginGeo(SpatialType.LineString) },
                { "BeginPolygon", s=>s.BeginGeo(SpatialType.Polygon) },
                { "BeginMultiPoint", s=>s.BeginGeo(SpatialType.MultiPoint) },
                { "BeginMultiLineString", s=>s.BeginGeo(SpatialType.MultiLineString) },
                { "BeginMultiPolygon", s=>s.BeginGeo(SpatialType.MultiPolygon) },
                { "BeginCollection", s=>s.BeginGeo(SpatialType.Collection) },
                { "BeginFullGlobe", s=>s.BeginGeo(SpatialType.FullGlobe) },
                { "BeginFigure", s=>s.BeginFigure(5, 15, 25, 35) },
                { "AddLine", s=>s.LineTo(10,20,30,40) },
                { "EndFigure", s=>s.EndFigure() },
                { "EndGeo", s=>s.EndGeo() },
            };

        private readonly Func<TypeWashedPipeline>[] validators = { CreateGeographyValidator, CreateGeometryValidator };

        private static TypeWashedPipeline CreateGeometryValidator()
        {
            return new TypeWashedToGeometryPipeline(new SpatialValidatorImplementation());
        }

        private static TypeWashedPipeline CreateGeographyValidator()
        {
            return new TypeWashedToGeographyLatLongPipeline(new SpatialValidatorImplementation());
        }

        [Fact]
        public void SetCoordinateSystemState()
        {
            foreach (var v in validators)
            {
                RunStateValidatorTest(v, "SetCoordinateSystem");
            }
        }

        [Fact]
        public void BeginGeoState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                return v;
            };

            string[] commonStates =
                {
                    "BeginPoint",
                    "BeginLineString",
                    "BeginPolygon",
                    "BeginMultiPoint",
                    "BeginMultiLineString",
                    "BeginMultiPolygon",
                    "BeginCollection",
                };

            foreach (var v in validators)
            {
                var localValidator = v;
                var statesToCheck = commonStates;
                if (localValidator().IsGeography)
                {
                    statesToCheck = statesToCheck.Union(new[] { "BeginFullGlobe" }).ToArray();
                }
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    statesToCheck);
            }
        }

        [Fact]
        public void PointStartState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.Point);
                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "BeginFigure",
                    "EndGeo");
            }
        }

        [Fact]
        public void PointBuildingState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.Point);
                v.BeginFigure(10, 20, 30, 40);
                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "EndFigure");
            }
        }

        [Fact]
        public void PointEndState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.Point);
                v.BeginFigure(10, 20, 30, 40);
                v.EndFigure();
                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "EndGeo");
            }
        }

        [Fact]
        public void LineStringStartState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.LineString);

                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "BeginFigure",
                    "EndGeo");
            }
        }

        [Fact]
        public void LineBuildingStateWithOnePoint()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.LineString);
                v.BeginFigure(10, 20, 30, 40);

                return v;
            };


            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "AddLine");
            }
        }

        [Fact]
        public void LineBuildingStateWithTwoPoints()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.LineString);
                v.BeginFigure(10, 20, 30, 40);
                v.LineTo(10, 20, 30, 40);
                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "AddLine",
                    "EndFigure");
            }
        }

        [Fact]
        public void LineStringEndState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.LineString);
                v.BeginFigure(10, 20, 30, 40);
                v.LineTo(10, 20, 30, 40);
                v.EndFigure();

                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "EndGeo");
            }
        }

        [Fact]
        public void PolygonStartState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.Polygon);

                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "BeginFigure",
                    "EndGeo");
            }
        }

        [Fact]
        public void PolygonBuildingState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.Polygon);
                v.BeginFigure(10, 20, 30, 40);
                v.LineTo(20, 30, 40, 50);
                v.LineTo(20, 40, 40, 50);
                v.LineTo(20, 50, 40, 50);
                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "AddLine"
                    );
            }
        }

        [Fact]
        public void PolygonBuildingEndingState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.Polygon);
                v.BeginFigure(10, 20, 30, 40);
                v.LineTo(20, 30, 40, 50);
                v.LineTo(20, 40, 40, 50);
                v.LineTo(20, 50, 40, 50);
                v.LineTo(10, 20, 30, 40);
                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "AddLine",
                    "EndFigure"
                    );
            }
        }
        [Fact]
        public void MultiPointState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.MultiPoint);

                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "BeginPoint",
                    "SetCoordinateSystem",
                    "EndGeo");
            }
        }

        [Fact]
        public void MultiLineStringState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.MultiLineString);

                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "BeginLineString",
                    "SetCoordinateSystem",
                    "EndGeo");
            }
        }

        [Fact]
        public void MultiPolygonState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.MultiPolygon);

                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "BeginPolygon",
                    "SetCoordinateSystem",
                    "EndGeo");
            }
        }

        [Fact]
        public void CollectionState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.Collection);

                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "SetCoordinateSystem",
                    "BeginPoint",
                    "BeginLineString",
                    "BeginPolygon",
                    "BeginMultiPoint",
                    "BeginMultiLineString",
                    "BeginMultiPolygon",
                    "BeginCollection",
                    "EndGeo");
            }
        }

        [Fact]
        public void FullGlobeState()
        {
            Func<TypeWashedPipeline> stateSetup = () =>
            {
                var v = CreateGeographyValidator();
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.FullGlobe);

                return v;
            };

            RunStateValidatorTest(
                stateSetup,
                "EndGeo");
        }

        [Fact]
        public void RestartAtFinishedState()
        {
            Func<TypeWashedPipeline, TypeWashedPipeline> stateSetup = v =>
            {
                v.SetCoordinateSystem(NonDefaultEpsgId);
                v.BeginGeo(SpatialType.Point);
                v.BeginFigure(10, 20, 30, 40);
                v.EndFigure();
                v.EndGeo();

                return v;
            };

            foreach (var v in validators)
            {
                var localValidator = v;
                RunStateValidatorTest(
                    () => stateSetup(localValidator()),
                    "SetCoordinateSystem");
            }
        }

        [Fact]
        public void InvalidPointTest()
        {
            foreach (var v in validators)
            {
                var validator = v();
                validator.SetCoordinateSystem(NonDefaultEpsgId);
                validator.BeginGeo(SpatialType.Point);
                SpatialTestUtils.VerifyExceptionThrown<FormatException>(
                    () => validator.BeginFigure(10, 20, double.NaN, 40),
                    Strings.Validator_InvalidPointCoordinate(10, 20, double.NaN, 40));
            }
        }

        [Fact]
        public void MaxGeometryDepth()
        {
            foreach (var v in validators)
            {
                var validator = v();
                validator.SetCoordinateSystem(NonDefaultEpsgId);
                for (int i = 0; i < 28; i++)
                {
                    validator.BeginGeo(SpatialType.Collection);
                }

                SpatialTestUtils.VerifyExceptionThrown<FormatException>(
                   () => validator.BeginGeo(SpatialType.Point),
                   Strings.Validator_NestingOverflow(28));
            }
        }

        [Fact]
        public void CoordinateSystemReset()
        {
            foreach (var v in validators)
            {
                var validator = v();
                validator.SetCoordinateSystem(NonDefaultEpsgId);
                validator.BeginGeo(SpatialType.MultiPoint);
                SpatialTestUtils.VerifyExceptionThrown<FormatException>(
                   () => validator.SetCoordinateSystem(CoordinateSystem.DefaultGeography.EpsgId),
                   Strings.Validator_SridMismatch);
            }
        }

        [Fact]
        public void UnexpectedGeometry()
        {
            var validator1 = new SpatialValidatorImplementation();
            validator1.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            validator1.GeographyPipeline.BeginGeography(SpatialType.Point);
            SpatialTestUtils.VerifyExceptionThrown<FormatException>(
                 () => validator1.GeometryPipeline.BeginGeometry(SpatialType.Point),
                 Strings.Validator_UnexpectedCall("SetCoordinateSystem", "BeginPoint"));

            var validator2 = new SpatialValidatorImplementation();
            validator2.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            validator2.GeographyPipeline.BeginGeography(SpatialType.Point);
            validator2.GeographyPipeline.BeginFigure(new GeographyPosition(45, 180, null, null));
            validator2.GeographyPipeline.EndFigure();
            SpatialTestUtils.VerifyExceptionThrown<FormatException>(
                 () => validator2.GeometryPipeline.EndGeometry(),
                 Strings.Validator_UnexpectedCall("SetCoordinateSystem", "End"));

            var validator3 = new SpatialValidatorImplementation();
            validator3.GeometryPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            validator3.GeometryPipeline.BeginGeometry(SpatialType.Point);
            SpatialTestUtils.VerifyExceptionThrown<FormatException>(
                 () => validator3.GeographyPipeline.BeginGeography(SpatialType.Point),
                 Strings.Validator_UnexpectedCall("SetCoordinateSystem", "BeginPoint"));

            var validator4 = new SpatialValidatorImplementation();
            validator4.GeometryPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            validator4.GeometryPipeline.BeginGeometry(SpatialType.Point);
            validator4.GeometryPipeline.BeginFigure(new GeometryPosition(45, 180, null, null));
            validator4.GeometryPipeline.EndFigure();
            SpatialTestUtils.VerifyExceptionThrown<FormatException>(
                 () => validator4.GeographyPipeline.EndGeography(),
                 Strings.Validator_UnexpectedCall("SetCoordinateSystem", "End"));

            var validator5 = new SpatialValidatorImplementation();
            validator5.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            validator5.GeographyPipeline.BeginGeography(SpatialType.Point);

            SpatialTestUtils.VerifyExceptionThrown<FormatException>(
                  () => validator5.GeometryPipeline.BeginFigure(new GeometryPosition(333, 3333333, 333, 333)),
                  Strings.Validator_UnexpectedCall("SetCoordinateSystem", "BeginFigure"));
        }

        private static void RunStateValidatorTest(Func<TypeWashedPipeline> setup, params String[] validTransitions)
        {
            var v = setup();

            foreach (var t in Transitions)
            {
                if (!validTransitions.Contains(t.Key))
                {
                    var ex = SpatialTestUtils.RunCatching<FormatException>(() => t.Value(v));
                    Assert.NotNull(ex);
                }
                else
                {
                    t.Value(v);
                    v = setup();
                }
            }
        }
    }
}
