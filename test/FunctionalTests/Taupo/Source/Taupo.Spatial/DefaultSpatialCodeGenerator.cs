//---------------------------------------------------------------------
// <copyright file="DefaultSpatialCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Spatial.Contracts;

    /// <summary>
    /// Implements custom initialization code generator for spatial types
    /// </summary>
    [ImplementationName(typeof(ICustomInitializationCodeGenerator), "Default")]
    public class DefaultSpatialCodeGenerator : ICustomInitializationCodeGenerator
    {
        /// <summary>
        /// Gets or sets the well-known-text formatter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IWellKnownTextSpatialFormatter WellKnownTextFormatter { get; set; }

        /// <summary>
        /// Generates a code expression to initialize an object.
        /// </summary>
        /// <param name="value">The object to generate an expression for.</param>
        /// <param name="initializationExpression">An initialize expression for the specified object.</param>
        /// <returns>True if initialization expression was generated, false otherwise.</returns>
        public bool TryGenerateInitialization(object value, out CodeExpression initializationExpression)
        {
            initializationExpression = null;

            var spatialInstance = value as ISpatial; 
            if (spatialInstance == null)
            {
                return false;
            }

            if (!spatialInstance.IsEmpty)
            {
                if (TryCallGeographyPointCreate(spatialInstance, out initializationExpression) || TryCallGeometryPointCreate(spatialInstance, out initializationExpression))
                {
                    return true;
                }
            }

            ExceptionUtilities.CheckAllRequiredDependencies(this);

            string asWellKnownText = this.WellKnownTextFormatter.Convert(spatialInstance);

            //// WellKnownTextSqlFormatter.Create().Parse<Geography>(new StringReader("<WKT>"));
            initializationExpression =
                Code.Type(typeof(WellKnownTextSqlFormatter).FullName)
                .Call("Create")
                .Call(
                    "Read",
                    new[] { Code.TypeRef(spatialInstance.GetType().GetBaseType()) },
                    Code.New(typeof(StringReader), Code.Primitive(asWellKnownText)));

            return true;
        }

        private static bool TryCallGeographyPointCreate(ISpatial spatialInstance, out CodeExpression initializationExpression)
        {
            return TryCallPointCreate<GeographyPoint>(spatialInstance, CoordinateSystem.DefaultGeography, "Geography", p => p.Latitude, p => p.Longitude, p => p.Z, p => p.M, out initializationExpression);
        }

        private static bool TryCallGeometryPointCreate(ISpatial spatialInstance, out CodeExpression initializationExpression)
        {
            return TryCallPointCreate<GeometryPoint>(spatialInstance, CoordinateSystem.DefaultGeometry, "Geometry", p => p.X, p => p.Y, p => p.Z, p => p.M, out initializationExpression);
        }

        private static bool TryCallPointCreate<TPoint>(
            ISpatial spatialInstance, 
            CoordinateSystem defaultCoordinateSystem,
            string coordinateSystemMethodName,
            Func<TPoint, double> getFirstCoord, 
            Func<TPoint, double> getSecondCoord,
            Func<TPoint, double?> getThirdCoord,
            Func<TPoint, double?> getFourthCoord, 
            out CodeExpression initializationExpression) where TPoint : class, ISpatial
        {
            var point = spatialInstance as TPoint;
            if (point == null)
            {
                initializationExpression = null;
                return false;
            }

            List<CodeExpression> parameters = new List<CodeExpression>();

            bool coordinateSystemSpecified = point.CoordinateSystem != defaultCoordinateSystem;
            if (coordinateSystemSpecified)
            {
                ////CoordinateSystem.Geography(1234)
                parameters.Add(Code.Type(typeof(CoordinateSystem).FullName).Call(coordinateSystemMethodName, Code.Primitive(point.CoordinateSystem.EpsgId)));
            }

            parameters.Add(Code.Primitive(getFirstCoord(point)));
            parameters.Add(Code.Primitive(getSecondCoord(point)));

            var thirdCoordinate = getThirdCoord(point);
            var fourthCoordinate = getFourthCoord(point);

            if (thirdCoordinate.HasValue || fourthCoordinate.HasValue || coordinateSystemSpecified)
            {
                parameters.Add(Code.Primitive(thirdCoordinate));
            }

            if (fourthCoordinate.HasValue || coordinateSystemSpecified)
            {
                parameters.Add(Code.Primitive(fourthCoordinate));
            }

            ////GeographyPoint.Create(1, 2);
            initializationExpression = Code.Type(typeof(TPoint).FullName).Call("Create", parameters.ToArray());
            return true;
        }
    }
}
