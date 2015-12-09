//---------------------------------------------------------------------
// <copyright file="SpatialTestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public static class SpatialTypeVerifier
    {
        public static void VerifyAsCollection(this Geometry data, params Action<Geometry>[] verifier)
        {
            VerifyTypedInstance<GeometryCollection>(data, verifier == null, (actual) =>
                                                                                {
                                                                                    Assert.Equal(actual.Geometries.Count, verifier.Length);
                                                                                    for (int i = 0; i < actual.Geometries.Count; ++i)
                                                                                    {
                                                                                        verifier[i](actual.Geometries[i]);
                                                                                    }
                                                                                });
        }

        public static void VerifyAsCollection(this Geography data, params Action<Geography>[] verifier)
        {
            VerifyTypedInstance<GeographyCollection>(data, verifier == null, (actual) =>
                                                                                 {
                                                                                     Assert.Equal(actual.Geographies.Count, verifier.Length);
                                                                                     for (int i = 0; i < actual.Geographies.Count; ++i)
                                                                                     {
                                                                                         verifier[i](actual.Geographies[i]);
                                                                                     }
                                                                                 });
        }

        public static void VerifyAsLineString(this Geometry data, params PositionData[] expected)
        {
            VerifyTypedInstance<GeometryLineString>(data, expected == null, (actual) =>
                                                                                 {
                                                                                     Assert.Equal(actual.Points.Count, expected.Length);

                                                                                     for (int i = 0; i < actual.Points.Count; ++i)
                                                                                     {
                                                                                         VerifyAsPoint(actual.Points[i], expected[i]);
                                                                                     }
                                                                                 });
        }

        public static void VerifyAsLineString(this Geography data, params PositionData[] expected)
        {
            VerifyTypedInstance<GeographyLineString>(data, expected == null, (actual) =>
                                                                                  {
                                                                                      Assert.Equal(actual.Points.Count, expected.Length);

                                                                                      for (int i = 0; i < actual.Points.Count; ++i)
                                                                                      {
                                                                                          VerifyAsPoint(actual.Points[i], expected[i]);
                                                                                      }
                                                                                  });
        }

        public static void VerifyAsMultiLineString(this Geometry data, params PositionData[][] expected)
        {
            VerifyTypedInstance<GeometryMultiLineString>(data, expected == null, (actual) =>
                                                                                      {
                                                                                          Assert.Equal(actual.LineStrings.Count, expected.Length);

                                                                                          for (int i = 0; i < actual.LineStrings.Count; ++i)
                                                                                          {
                                                                                              actual.LineStrings[i].VerifyAsLineString(expected[i]);
                                                                                          }
                                                                                      });
        }

        public static void VerifyAsMultiLineString(this Geography data, params PositionData[][] expected)
        {
            VerifyTypedInstance<GeographyMultiLineString>(data, expected == null, (actual) =>
                                                                                       {
                                                                                           Assert.Equal(actual.LineStrings.Count, expected.Length);

                                                                                           for (int i = 0; i < actual.LineStrings.Count; ++i)
                                                                                           {
                                                                                               actual.LineStrings[i].VerifyAsLineString(expected[i]);
                                                                                           }
                                                                                       });
        }

        public static void VerifyAsMultiPoint(this Geometry data, params PositionData[] expected)
        {
            VerifyTypedInstance<GeometryMultiPoint>(data, expected == null, (actual) =>
                                                                                 {
                                                                                     Assert.Equal(actual.Points.Count, expected.Length);

                                                                                     for (int i = 0; i < actual.Points.Count; ++i)
                                                                                     {
                                                                                         VerifyAsPoint(actual.Points[i], expected[i]);
                                                                                     }
                                                                                 });
        }

        public static void VerifyAsMultiPoint(this Geography data, params PositionData[] expected)
        {
            VerifyTypedInstance<GeographyMultiPoint>(data, expected == null, (actual) =>
                                                                                  {
                                                                                      Assert.Equal(actual.Points.Count, expected.Length);

                                                                                      for (int i = 0; i < actual.Points.Count; ++i)
                                                                                      {
                                                                                          VerifyAsPoint(actual.Points[i], expected[i]);
                                                                                      }
                                                                                  });
        }

        public static void VerifyAsMultiPolygon(this Geometry data, params PositionData[][][] expected)
        {
            VerifyTypedInstance<GeometryMultiPolygon>(data, expected == null, (actual) =>
                                                                                   {
                                                                                       Assert.Equal(actual.Polygons.Count, expected.Length);

                                                                                       for (int i = 0; i < actual.Polygons.Count; ++i)
                                                                                       {
                                                                                           // expected[i] can be null
                                                                                           actual.Polygons[i].VerifyAsPolygon(expected[i]);
                                                                                       }
                                                                                   });
        }

        public static void VerifyAsMultiPolygon(this Geography data, params PositionData[][][] expected)
        {
            VerifyTypedInstance<GeographyMultiPolygon>(data, expected == null, (actual) =>
                                                                                    {
                                                                                        Assert.Equal(actual.Polygons.Count, expected.Length);

                                                                                        for (int i = 0; i < actual.Polygons.Count; ++i)
                                                                                        {
                                                                                            // expected[i] can be null
                                                                                            actual.Polygons[i].VerifyAsPolygon(expected[i]);
                                                                                        }
                                                                                    });
        }

        public static void VerifyAsPoint(this Geometry data, PositionData expected)
        {
            VerifyTypedInstance<GeometryPoint>(data, expected == null, (actual) => VerifyPosition(expected, actual));
        }

        public static void VerifyAsPoint(this Geography data, PositionData expected)
        {
            VerifyTypedInstance<GeographyPoint>(data, expected == null, (actual) => VerifyPosition(expected, actual));
        }

        public static void VerifyAsPolygon(this Geometry data, params PositionData[][] expected)
        {
            VerifyTypedInstance<GeometryPolygon>(data, expected == null, (actual) =>
                                                                              {
                                                                                  Assert.Equal(actual.Rings.Count, expected.Length);

                                                                                  for (int i = 0; i < actual.Rings.Count; ++i)
                                                                                  {
                                                                                      // expected[i] can be null
                                                                                      actual.Rings[i].VerifyAsLineString(expected[i]);
                                                                                  }
                                                                              });
        }

        public static void VerifyAsPolygon(this Geography data, params PositionData[][] expected)
        {
            VerifyTypedInstance<GeographyPolygon>(data, expected == null, (actual) =>
                                                                               {
                                                                                   Assert.Equal(actual.Rings.Count, expected.Length);

                                                                                   for (int i = 0; i < actual.Rings.Count; ++i)
                                                                                   {
                                                                                       // expected[i] can be null
                                                                                       actual.Rings[i].VerifyAsLineString(expected[i]);
                                                                                   }
                                                                               });
        }

        private static void VerifyPosition(PositionData expected, GeometryPoint actual)
        {
            Assert.Equal(expected.Latitude, actual.X);
            Assert.Equal(expected.Longitude, actual.Y);
            Assert.Equal(expected.Z, actual.Z);
            Assert.Equal(expected.M, actual.M);
        }

        private static void VerifyPosition(PositionData expected, GeographyPoint actual)
        {
            Assert.Equal(expected.Latitude, actual.Latitude);
            Assert.Equal(expected.Longitude, actual.Longitude);
            Assert.Equal(expected.Z, actual.Z);
            Assert.Equal(expected.M, actual.M);
        }

        private static void VerifyTypedInstance<T>(ISpatial instance, bool expectEmpty, Action<T> verifier) where T : class
        {
            var typedGeography = instance as T;
            Assert.NotNull(typedGeography);

            if (expectEmpty)
            {
                Assert.True(instance.IsEmpty);
            }
            else
            {
                verifier(typedGeography);
            }
        }
    }

    public static class SpatialTestUtils
    {
        public static IDisposable RegisterOperations(SpatialOperations operations)
        {
            var originalValue = SpatialImplementation.CurrentImplementation.Operations;
            SpatialImplementation.CurrentImplementation.Operations = operations;

            return new ActionOnDispose(() => 
            {
                SpatialImplementation.CurrentImplementation.Operations = originalValue;
            });
        }

        public static void VerifyXPaths(XPathNavigator navigator, IXmlNamespaceResolver namespaceResolver, params string[] xpaths)
        {
            Assert.NotNull(navigator);
            Assert.NotNull(xpaths);
            foreach (string xpath in xpaths)
            {
                bool isTrue;
                if (xpath.StartsWith("count") || xpath.StartsWith("boolean"))
                {
                    isTrue = (bool)navigator.Evaluate(xpath, namespaceResolver);
                }
                else
                {
                    XPathNodeIterator iterator = navigator.Select(xpath, namespaceResolver);
                    isTrue = iterator.Count > 0;
                }


                Assert.True(isTrue, "The expression " + xpath + " did not find elements. The document has just been traced.");
            }
        }

        /// <summary>Runs the specified action and catches any thrown exception.</summary>
        /// <param name="action">Action to run.</param>
        /// <typeparam name="T">The exception type</typeparam>
        /// <returns>Caught exception; null if none was thrown, or the thrown exception is of the wrong type</returns>
        public static T RunCatching<T>(Action action)
            where T : class
        {
            Exception ex = RunCatching(action);
            if (ex == null)
            {
                return null;
            }
            if (ex is T)
            {
                return ex as T;
            }
            Assert.True(false, "Exception of the wrong type thrown - expected " + typeof(T).Name + " but got " + ex.GetType().Name);
            return null;
        }

        /// <summary>Runs the specified action and catches any thrown exception.</summary>
        /// <param name="action">Action to run.</param>
        /// <returns>Caught exception; null if none was thrown.</returns>
        public static Exception RunCatching(Action action)
        {
            Debug.Assert(action != null, "action != null");

            Exception result = null;
            try
            {
                action();
            }
            catch (Exception exception)
            {
                result = exception;
            }

            return result;
        }

        public static void AssertEqualContents<TElement>(IEnumerable<TElement> expected, IEnumerable<TElement> actual, IEqualityComparer<TElement> comparison = null)
        {
            AssertEqualContents(expected.ToList(), actual.ToList());
        }

        public static void AssertEqualContents<TElement>(IList<TElement> expected, IList<TElement> actual, IEqualityComparer<TElement> comparison = null)
        {
            if (expected.Count != actual.Count || Enumerable.Zip(expected, actual, (e, a) => AreEqualValues(e, a, comparison)).Any(r => !r))
            {
                var expectedBuilder = new StringBuilder("Expected: ");
                var actualBuilder = new StringBuilder("Actual:   ");
                for (int i = 0; i < Math.Min(expected.Count, actual.Count); i++)
                {
                    expectedBuilder.Append(GetReadableString(expected[i]));
                    expectedBuilder.Append(' ');

                    actualBuilder.Append(GetReadableString(actual[i]));
                    actualBuilder.Append(' ');

                    while (expectedBuilder.Length < actualBuilder.Length)
                    {
                        expectedBuilder.Append(' ');
                    }

                    while (actualBuilder.Length < expectedBuilder.Length)
                    {
                        actualBuilder.Append(' ');
                    }
                }

                for (int i = Math.Min(expected.Count, actual.Count); i < expected.Count; i++)
                {
                    expectedBuilder.Append(GetReadableString(expected[i]));
                    expectedBuilder.Append(' ');
                }

                for (int i = Math.Min(expected.Count, actual.Count); i < actual.Count; i++)
                {
                    actualBuilder.Append(GetReadableString(actual[i]));
                    actualBuilder.Append(' ');
                }

                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("Contents are not equal");
                messageBuilder.AppendLine(expectedBuilder.ToString());
                messageBuilder.AppendLine(actualBuilder.ToString());

                var message = messageBuilder.ToString();
                Assert.True(false, message);
            }
        }

        public static void AssertEqualValues<TValue>(TValue expected, TValue actual)
        {
            Assert.True(AreEqualValues(expected, actual));
        }

        private static bool AreEqualValues<TValue>(TValue expected, TValue actual, IEqualityComparer<TValue> comparison = null)
        {
            Func<object, object, bool> comp;
            if (comparison == null)
            {
                comp = (lhs, rhs) => lhs.Equals(rhs);
            }
            else
            {
                comp = (lhs, rhs) => comparison.Equals((TValue)lhs, (TValue)rhs);
            }
            return AreEqualValues(expected, actual, comp);
        }

        private static bool AreEqualValues(object expected, object actual, Func<object, object, bool> comparison)
        {
            if (expected == null)
            {
                return actual == null;
            }

            if (expected.GetType() != actual.GetType())
            {
                return false;
            }

            var type = expected.GetType();

            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                type = nullableType;
            }

            if (type.IsArray)
            {
                var expectedArray = (Array)expected;
                var actualArray = (Array)actual;

                if (expectedArray.Length != actualArray.Length)
                {
                    return false;
                }

                for (int i = 0; i < expectedArray.Length; i++)
                {
                    if (!AreEqualValues(expectedArray.GetValue(i), actualArray.GetValue(i)))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var keyProperty = type.GetProperty("Key");
                var valueProperty = type.GetProperty("Value");
                return AreEqualValues(keyProperty.GetValue(expected, null), keyProperty.GetValue(actual, null))
                    && AreEqualValues(valueProperty.GetValue(expected, null), valueProperty.GetValue(actual, null));
            }

            return comparison(expected, actual);
        }

        private static string GetReadableString(object value)
        {
            if (value == null)
            {
                return "null";
            }

            var type = value.GetType();
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                type = nullableType;
            }

            if (type.IsArray)
            {
                var array = (Array)value;
                var builder = new StringBuilder();
                builder.Append('{');

                for (int i = 0; i < array.Length; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(' ');
                    }

                    builder.Append(GetReadableString(array.GetValue(i)));
                }

                builder.Append('}');
                return builder.ToString();
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var keyProperty = type.GetProperty("Key");
                var valueProperty = type.GetProperty("Value");

                var key = keyProperty.GetValue(value, null);
                value = valueProperty.GetValue(value, null);

                var builder = new StringBuilder();
                builder.Append('{');
                builder.Append(GetReadableString(key));
                builder.Append(',');
                builder.Append(GetReadableString(value));
                builder.Append('}');
                return builder.ToString();
            }

            return value.ToString();
        }

        public static void VerifyExceptionThrown<T>(Action action, string expectedMessage)
            where T : Exception
        {
            var exception = RunCatching<T>(action);
            Assert.True(exception != null, "An exception was expected but none was thrown.");
            Assert.True(expectedMessage == exception.Message, "The exception did not contain the expected message.");
        }

        public static void VerifyXPaths(XPathNavigator navigator, params string[] xpaths)
        {
            Assert.NotNull(navigator);
            Assert.NotNull(xpaths);
            foreach (string xpath in xpaths)
            {
                bool isTrue;
                if (xpath.StartsWith("count") || xpath.StartsWith("boolean"))
                {
                    isTrue = (bool)navigator.Evaluate(xpath);
                }
                else
                {
                    XPathNodeIterator iterator = navigator.Select(xpath);
                    isTrue = iterator.Count > 0;
                }

                Assert.True(isTrue, "The expression " + xpath + " did not find elements. The document has just been traced.");
            }
        }
    }

    /// <summary>
    /// Turn spatial object into a set of positions
    /// </summary>
    public class SpatialToPositionPipeline : SpatialPipeline
    {
        private CoordinateSystem coordinateSystem;
        private readonly List<PositionData> coordinates = new List<PositionData>();

        private class GeographyPipe : GeographyPipeline
        {
            private readonly SpatialToPositionPipeline parent;

            public GeographyPipe(SpatialToPositionPipeline parent)
            {
                this.parent = parent;
            }

            public override void LineTo(GeographyPosition position)
            {
                this.parent.coordinates.Add(new PositionData(position.Latitude, position.Longitude, position.Z, position.M));
            }

            public override void BeginFigure(GeographyPosition position)
            {
                this.parent.coordinates.Add(new PositionData(position.Latitude, position.Longitude, position.Z, position.M));
            }

            public override void BeginGeography(SpatialType type)
            {
            }

            public override void EndFigure()
            {
            }

            public override void EndGeography()
            {
            }

            public override void Reset()
            {
            }

            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
                this.parent.coordinateSystem = coordinateSystem;
            }
        }

        private class GeometryPipe : GeometryPipeline
        {
            private readonly SpatialToPositionPipeline parent;

            public GeometryPipe(SpatialToPositionPipeline parent)
            {
                this.parent = parent;
            }

            public override void LineTo(GeometryPosition position)
            {
                this.parent.coordinates.Add(new PositionData(position.X, position.Y, position.Z, position.M));
            }

            public override void BeginFigure(GeometryPosition position)
            {
                this.parent.coordinates.Add(new PositionData(position.X, position.Y, position.Z, position.M));
            }

            public override void BeginGeometry(SpatialType type)
            {
            }

            public override void EndFigure()
            {
            }

            public override void EndGeometry()
            {
            }

            public override void Reset()
            {
            }

            public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
            {
                this.parent.coordinateSystem = coordinateSystem;
            }
        }

        public CoordinateSystem CoordinateSystem
        {
            get { return this.coordinateSystem; }
        }

        public List<PositionData> Coordinates
        {
            get { return this.coordinates; }
        }

        public override GeographyPipeline GeographyPipeline
        {
            get { return new GeographyPipe(this); }
        }

        public override GeometryPipeline GeometryPipeline
        {
            get { return new GeometryPipe(this); }
        }
    }
}
