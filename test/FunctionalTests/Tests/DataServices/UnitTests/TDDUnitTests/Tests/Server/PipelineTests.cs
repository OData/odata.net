//---------------------------------------------------------------------
// <copyright file="PipelineTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Server
{
    using System;
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    /// <summary>
    /// Tests for accessing the Pipeline off of a Validator implementation.
    /// </summary>
    [TestClass]
    public class PipelineTests
    {
        [TestMethod]
        public void UseValidatorsInParallel()
        {
            var validator = SpatialValidator.Create();
            var geographyPipeline = (GeographyPipeline)validator;
            geographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            var geometryPipeline = (GeometryPipeline)validator;
            geometryPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            geometryPipeline.BeginGeometry(SpatialType.Point);
            geographyPipeline.BeginGeography(SpatialType.Point);
            geometryPipeline.BeginFigure(new GeometryPosition(12, 12, null, null));
            geographyPipeline.BeginFigure(new GeographyPosition(45, 45, null, null));
            geometryPipeline.EndFigure();
            geometryPipeline.EndGeometry();
            geographyPipeline.EndFigure();
            geographyPipeline.EndGeography();
        }

        [TestMethod]
        public void ExpectFailureOnCrossingPipelines()
        {
            var validator5 = new SpatialValidatorImplementation();
            validator5.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            validator5.GeographyPipeline.BeginGeography(SpatialType.Point);
            Action test = () => validator5.GeometryPipeline.BeginFigure(new GeometryPosition(333, 3333333, 333, 333));
            test.ShouldThrow<FormatException>().WithMessage(Strings.Validator_UnexpectedCall("SetCoordinateSystem", "BeginFigure"));
        }
    }
}