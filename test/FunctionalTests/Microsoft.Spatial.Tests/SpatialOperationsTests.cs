//---------------------------------------------------------------------
// <copyright file="SpatialOperationsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class SpatialOperationsTests
    {
        #region test data initialization

        private readonly List<GeometryPoint> geomPoints = new List<GeometryPoint>();
        private readonly List<GeometryLineString> geomLines = new List<GeometryLineString>();
        private readonly List<GeometrySurface> geomSurfaces = new List<GeometrySurface>();

        private readonly List<GeographyPoint> geogPoints = new List<GeographyPoint>();
        private readonly List<GeographyLineString> geogLines = new List<GeographyLineString>();
        private readonly List<GeographySurface> geogSurfaces = new List<GeographySurface>();

        private readonly List<double> numbers = new List<double>();
        private TestSpatialOperations implementation;

        public SpatialOperationsTests()
        {
            // We want to create the objects with a different spatial implementation, so that that one has
            // registered ops and the singleton one does not. This makes sure that the extension methods use
            // the backref to find their implementations.
            this.implementation = new TestSpatialOperations();
            var oldImplementation = SpatialImplementation.CurrentImplementation;
            SpatialImplementation.CurrentImplementation = new DataServicesSpatialImplementation {Operations = this.implementation};

            this.geomPoints.Add(GeometryFactory.Point());
            this.geomPoints.Add(GeometryFactory.Point(1, 2));
            this.geomPoints.Add(GeometryFactory.Point(3, 4));
            this.geomPoints.Add(GeometryFactory.Point(5, 6));

            this.geomLines.Add(GeometryFactory.LineString(1, 2).LineTo(3, 4));
            this.geomLines.Add(GeometryFactory.LineString(5, 6).LineTo(7, 8));
            this.geomLines.Add(GeometryFactory.LineString(9, 10).LineTo(11, 12));
            this.geomLines.Add(GeometryFactory.LineString(13, 14).LineTo(15, 16));

            this.geomSurfaces.Add(GeometryFactory.Polygon());
            this.geomSurfaces.Add(GeometryFactory.Polygon());
            this.geomSurfaces.Add(GeometryFactory.Polygon());
            this.geomSurfaces.Add(GeometryFactory.Polygon());

            this.geogPoints.Add(GeographyFactory.Point());
            this.geogPoints.Add(GeographyFactory.Point(1, 2));
            this.geogPoints.Add(GeographyFactory.Point(3, 4));
            this.geogPoints.Add(GeographyFactory.Point(5, 6));

            this.geogLines.Add(GeographyFactory.LineString(1, 2).LineTo(3, 4));
            this.geogLines.Add(GeographyFactory.LineString(5, 6).LineTo(7, 8));
            this.geogLines.Add(GeographyFactory.LineString(9, 10).LineTo(11, 12));
            this.geogLines.Add(GeographyFactory.LineString(13, 14).LineTo(15, 16));

            this.geogSurfaces.Add(GeographyFactory.Polygon());
            this.geogSurfaces.Add(GeographyFactory.Polygon());
            this.geogSurfaces.Add(GeographyFactory.Polygon());
            this.geogSurfaces.Add(GeographyFactory.Polygon());

            this.numbers.Add(1.23);
            this.numbers.Add(3.45);
            this.numbers.Add(5.67);
            this.numbers.Add(8.90);
            this.numbers.Add(-1.23);
            this.numbers.Add(-3.45);

            SpatialImplementation.CurrentImplementation = oldImplementation;
        }

        #endregion

        [Fact]
        public void SpatialOperationsThrowNotImplemented()
        {
            var operations = new BaseSpatialOperations();
            var message = new NotImplementedException().Message;
            foreach (var method in typeof(SpatialOperations).GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.DeclaringType != typeof(object)))
            {
                SpatialTestUtils.VerifyExceptionThrown<NotImplementedException>(() => this.InvokeOperation(operations, method), message);
            }
        }

        [Fact]
        public void DistanceGeometryPropagatesNull()
        {
            this.implementation.callback = (methodName, arguments) =>
            {
                Assert.True(false, "shouldn't call the implementation");
            };
            var result = GeometryOperationsExtensions.Distance(null, geomPoints[0]);
            Assert.Null(result);

            result = GeometryOperationsExtensions.Distance(geomPoints[0], null);
            Assert.Null(result);
        }

        [Fact]
        public void DistanceGeographyPropagatesNull()
        {
            this.implementation.callback = (methodName, arguments) =>
                                               {
                                                   Assert.True(false, "shouldn't call the implementation");
                                               };
            var result = GeographyOperationsExtensions.Distance(null, geogPoints[0]);
            Assert.Null(result);

            result = GeographyOperationsExtensions.Distance(geogPoints[0], null);
            Assert.Null(result);
        }

        [Fact]
        public void DistanceGeometryCallsRegisteredImplementation()
        {
            
            this.implementation.callback = (methodName, arguments) =>
                                               {
                                                   Assert.Equal("Distance", methodName);
                                                   Assert.Same(this.geomPoints[0], arguments[0]);
                                                   Assert.Same(this.geomPoints[1], arguments[1]);
                                               };
            this.implementation.getReturnValue = (t) =>
                                                     {
                                                         Assert.True(typeof(double) == t, "we should only see double");
                                                         return TestSpatialOperations.DefaultDistanceReturn;
                                                     };
            var result = this.geomPoints[0].Distance(this.geomPoints[1]);
            Assert.Equal(TestSpatialOperations.DefaultDistanceReturn, result);
        }

        [Fact]
        public void DistanceGeographyCallsRegisteredImplementation()
        {
            
            this.implementation.callback = (methodName, arguments) =>
                                               {
                                                   Assert.Equal("Distance", methodName);
                                                   Assert.Same(this.geogPoints[0], arguments[0]);
                                                   Assert.Same(this.geogPoints[1], arguments[1]);
                                               };
            this.implementation.getReturnValue = (t) =>
                                                     {
                                                         Assert.True(typeof(double) == t, "we should only see double");
                                                         return TestSpatialOperations.DefaultDistanceReturn;
                                                     };
            var result = this.geogPoints[0].Distance(this.geogPoints[1]);
            Assert.Equal(TestSpatialOperations.DefaultDistanceReturn, result);
        }
      
        #region test helper methods

        private object InvokeOperation(object instance, MethodInfo operation)
        {
            Assert.False(operation.IsStatic);

            try
            {
                return operation.Invoke(instance, this.MakeUpValueForParams(operation));
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        private object[] MakeUpValueForParams(MethodInfo method)
        {
            var geomPointQueue = new Queue<GeometryPoint>(this.geomPoints.Skip(1));
            var geomLineQueue = new Queue<GeometryLineString>(this.geomLines.Skip(1));
            var geomSurfaceQueue = new Queue<GeometrySurface>(this.geomSurfaces.Skip(1));

            var geogPointQueue = new Queue<GeographyPoint>(this.geogPoints.Skip(1));
            var geogLineQueue = new Queue<GeographyLineString>(this.geogLines.Skip(1));
            var geogSurfaceQueue = new Queue<GeographySurface>(this.geogSurfaces.Skip(1));

            var doubleQueue = new Queue<double>(this.numbers.Skip(1));

            var parameters = new List<object>();
            var types = method.GetParameters().Select(p => p.ParameterType).ToArray();
            foreach (var t in types)
            {
                var type = t;
                if (type.IsByRef)
                {
                    type = type.GetElementType();
                }

                if (type.IsAssignableFrom(typeof(double)))
                {
                    parameters.Add(doubleQueue.Dequeue());
                }
                else if (type.IsAssignableFrom(typeof(GeometrySurface)))
                {
                    parameters.Add(geomSurfaceQueue.Dequeue());
                }
                else if (type.IsAssignableFrom(typeof(GeometryCurve)))
                {
                    parameters.Add(geomLineQueue.Dequeue());
                }
                else if (type.IsAssignableFrom(typeof(Geometry)))
                {
                    parameters.Add(geomPointQueue.Dequeue());
                }
                else if (type.IsAssignableFrom(typeof(GeographySurface)))
                {
                    parameters.Add(geogSurfaceQueue.Dequeue());
                }
                else if (type.IsAssignableFrom(typeof(GeographyCurve)))
                {
                    parameters.Add(geogLineQueue.Dequeue());
                }
                else if (type.IsAssignableFrom(typeof(Geography)))
                {
                    parameters.Add(geogPointQueue.Dequeue());
                }
                else
                {
                    Assert.True(false, "Unsupported type: " + type);
                }
            }

            return parameters.ToArray();
        }

        #endregion

        /// <summary>
        ///   Non-abstract version of the base class which does not override any methods
        /// </summary>
        private class BaseSpatialOperations : SpatialOperations
        {
        }

        /// <summary>
        ///   Test implementation which fires delegates when any operation is invoked
        /// </summary>
        private class TestSpatialOperations : SpatialOperations
        {
            public const int MinX = 1;
            public const int MinY = 2;
            public const int MaxX = 3;
            public const int MaxY = 4;
            public const double DefaultDistanceReturn = 1.1;

            public Action<string, object[]> callback;
            public Func<Type, object> getReturnValue;

            public TestSpatialOperations() : this((m, a) => { }, t => 3)
            {
            }

            public TestSpatialOperations(Action<string, object[]> callback, Func<Type, object> getReturnValue)
            {
                this.callback = callback;
                this.getReturnValue = getReturnValue;
                this.Called = false;
            }

            public bool Called { get; set; }

            #region Operation overloads

            public override double Distance(Geometry operand1, Geometry operand2)
            {
                return this.Invoke(() => this.Distance(operand1, operand2));
            }

            public override double Distance(Geography operand1, Geography operand2)
            {
                return this.Invoke(() => this.Distance(operand1, operand2));
            }

            #endregion

            private TReturn Invoke<TReturn>(Expression<Func<TReturn>> expression)
            {
                this.Called = true;
                var methodCall = (MethodCallExpression)expression.Body;
                var arguments = methodCall.Arguments.Cast<MemberExpression>().Select(m => ((FieldInfo)m.Member).GetValue(((ConstantExpression)m.Expression).Value)).ToArray();
                this.callback(methodCall.Method.Name, arguments);
                return (TReturn)this.getReturnValue(typeof(TReturn));
            }
        }
    }
}