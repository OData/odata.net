//---------------------------------------------------------------------
// <copyright file="SpatialReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class SpatialReaderTests
    {
        private const string HalfPoint = "P\r\n";
        private const string OnePoint = "P\r\nT\r\n";
        private const string OneAndAHalfPoints = OnePoint + HalfPoint;
        private const string TwoPoints = OnePoint + OnePoint;
        private const string MissingLineMessage = "Input should have had a T line. Instead, found ''.";
        private static readonly GeographyPoint ExpectedGeographyResult = TestData.PointG(45.345345, -127.92734, null, null, CoordinateSystem.DefaultGeography);
        private static readonly GeometryPoint ExpectedGeometryResult = TestData.PointM(3.1415, 2.718, null, null, CoordinateSystem.DefaultGeometry);
        private DataServicesSpatialImplementation implementation;

        public SpatialReaderTests() { this.implementation = new DataServicesSpatialImplementation(); }

        [Fact]
        public void FormatParsesOneShapeCorrectly() { ShouldFindOneShape(OnePoint, string.Empty); }

        [Fact]
        public void FormatParsesOneShapeCorrectlyWithExtraTestAvailable() { ShouldFindOneShape(OneAndAHalfPoints, HalfPoint); }

        [Fact]
        public void FormatParsesOneShapeCorrectlyFromMultiplePoints() { ShouldFindOneShape(TwoPoints, OnePoint); }

        [Fact]
        public void FormatParsesPartialDataCorrectly() { ShouldFindNothing(HalfPoint); }

        [Fact]
        public void ParseGivesValidationgPipeline()
        {
            // this test is bad, we have a task to fix the api to be testable
            var builder = new TrivialFormatter().GetValidatingBuilder();
            var pipe = builder.Key;
            AssertThrows<FormatException>(() => pipe.GeometryPipeline.EndFigure());
        }

        [Fact]
        public void ErrorOnNullDestinationInCtor()
        {
            Action act = () => new TrivialReader(null);
            SpatialTestUtils.VerifyExceptionThrown<ArgumentNullException>(act, "Value cannot be null.\r\nParameter name: destination");
        }

        [Fact]
        public void ErrorOnNullInputToReadMethods()
        {
            var reader = new TrivialReader(new CallSequenceLoggingPipeline());
            Action[] acts = { () => reader.ReadGeography(null), () => reader.ReadGeometry(null) };

            foreach (var act in acts)
            {
                SpatialTestUtils.VerifyExceptionThrown<ArgumentNullException>(act, "Value cannot be null.\r\nParameter name: input");
            }
        }

        private static void ShouldFindOneShape(string input, string expectedUnusedInput)
        {
            var testSubject = new TrivialFormatter();
            AssertParsesOneShape(testSubject, input, ExpectedGeographyResult, expectedUnusedInput);
            AssertParsesOneShape(testSubject, input, ExpectedGeometryResult, expectedUnusedInput);
        }

        private static void ShouldFindNothing(string input)
        {
            var testSubject = new TrivialFormatter();
            Assert.Equal(MissingLineMessage,
                AssertThrows<ParseErrorException>(() => testSubject.Read<GeographyPoint>(new StringReader(input))).Message);
            
            Assert.Equal(MissingLineMessage,
                AssertThrows<ParseErrorException>(() => testSubject.Read<GeometryPoint>(new StringReader(input))).Message);
        }

        private static TException AssertThrows<TException>(Action func) where TException : Exception
        {
            try
            {
                func();
            }
            catch (Exception ex)
            {
                if (typeof (TException).IsInstanceOfType(ex))
                    return (TException) ex;
                throw;
            }
            Assert.True(false, "Expected exception, but no exception was thrown.");
// ReSharper disable HeuristicUnreachableCode
            return default(TException);
// ReSharper restore HeuristicUnreachableCode
        }

        private static void AssertParsesOneShape<TExpectedType>(TrivialFormatter testSubject, string input, TExpectedType expected,
            string expectedUnusedInput) where TExpectedType : class, ISpatial
        {
            var reader = new StringReader(input);
            var actual = testSubject.Read<TExpectedType>(reader);
            Assert.Equal(expected, actual);
            Assert.Equal(expectedUnusedInput, reader.ReadToEnd());
        }
    }

    internal class TrivialFormatter : TestSpatialFormatter<TextReader, TextWriter>
    {
        public TrivialFormatter()
        {
            this.ReadGeographyAction = (s, p) => new TrivialReader(p).ReadGeography(s);
            this.ReadGeometryAction = (s, p) => new TrivialReader(p).ReadGeometry(s);
        }

        internal KeyValuePair<SpatialPipeline, IShapeProvider> GetValidatingBuilder()
        {
            return this.MakeValidatingBuilder();
        }
    }

    internal class TrivialReader : SpatialReader<TextReader>
    {
        private static readonly GeographyPoint ExpectedGeographyResult = TestData.PointG(45.345345, -127.92734, null, null);
        private static readonly GeometryPoint ExpectedGeometryResult = TestData.PointM(3.1415, 2.718, null, null);

        public TrivialReader(SpatialPipeline destination) : base(destination) { }

        protected override void ReadGeographyImplementation(TextReader input)
        {
            MatchLine(input, "P");

            GeographyPipeline destination = this.Destination;
            destination.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            destination.BeginGeography(SpatialType.Point);
            destination.BeginFigure(new GeographyPosition(ExpectedGeographyResult.Latitude,
                                                                         ExpectedGeographyResult.Longitude, null, null));

            MatchLine(input, "T");

            destination.EndFigure();
            destination.EndGeography();
        }

        protected override void ReadGeometryImplementation(TextReader input)
        {
            MatchLine(input, "P");

            GeometryPipeline destination = this.Destination;
            destination.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            destination.BeginGeometry(SpatialType.Point);
            destination.BeginFigure(new GeometryPosition(ExpectedGeometryResult.X,
                                                                         ExpectedGeometryResult.Y, null, null));

            MatchLine(input, "T");

            destination.EndFigure();
            destination.EndGeometry();
        }

        private static void MatchLine(TextReader input, string expectedLine)
        {
            var line = input.ReadLine();
            if (line == null || line.Trim() != expectedLine)
            {
                string errorMessage = string.Format("Input should have had a {1} line. Instead, found '{0}'.", line, expectedLine);
                throw new FormatException(errorMessage);
            }
        }
    }
}
