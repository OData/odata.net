//---------------------------------------------------------------------
// <copyright file="RawValueWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// This is a test class for RawValueWriter and is intended
    /// to contain all RawValueWriter Unit Tests
    ///</summary>
    public class RawValueWriterTests
    {
        private MemoryStream stream;
        private ODataMessageWriterSettings settings;

        public RawValueWriterTests()
        {
            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings();
        }

        [Fact]
        public void StartDoesNothingNormally()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.Start();
            this.StreamAsString(target).Should().BeEmpty();
        }

        [Fact]
        public void StartWithJsonPadding()
        {
            this.settings.JsonPCallback = "foo";
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.Start();
            this.StreamAsString(target).Should().Be("foo(");
        }

        [Fact]
        public void EndDoesNothingNormally()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.End();
            this.StreamAsString(target).Should().BeEmpty();
        }

        [Fact]
        public void EndWithJsonPadding()
        {
            this.settings.JsonPCallback = "foo";
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.End();
            this.StreamAsString(target).Should().Be(")");
        }

        /// <summary>
        ///A test for WriteRawValue
        ///</summary>
        [Fact]
        public void WriteRawValueWritesRawString()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            const string value = "string value";
            target.WriteRawValue(value);
            this.StreamAsString(target).Should().Be(value);
        }

        // <summary>
        ///A test for writing raw Date value
        ///</summary>
        [Fact]
        public void WriteRawValueWritesDate()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            Date value = new Date(2014, 9, 18);
            target.WriteRawValue(value);
            this.StreamAsString(target).Should().Be("2014-09-18");
        }

        // <summary>
        ///A test for writing raw Date value
        ///</summary>
        [Fact]
        public void WriteRawValueWritesTimeOfDay()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            TimeOfDay value = new TimeOfDay(9, 47, 5, 900);
            target.WriteRawValue(value);
            this.StreamAsString(target).Should().Be("09:47:05.9000000");
        }

        /// <summary>
        ///A test for writing raw Geography value
        ///</summary>
        [Fact]
        public void WriteRawValueWritesGeographyValue()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            var value = GeographyPoint.Create(22.2, 22.2);
            target.WriteRawValue(value);
            this.StreamAsString(target).Should().Be(@"{""type"":""Point"",""coordinates"":[22.2,22.2],""crs"":{""type"":""name"",""properties"":{""name"":""EPSG:4326""}}}");
        }

        /// <summary>
        ///A test for writing raw Geometry value
        ///</summary>
        [Fact]
        public void WriteRawValueWritesGeometryValue()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            var value = GeometryPoint.Create(1.2, 3.16);
            target.WriteRawValue(value);
            this.StreamAsString(target).Should().Be(@"{""type"":""Point"",""coordinates"":[1.2,3.16],""crs"":{""type"":""name"",""properties"":{""name"":""EPSG:0""}}}");
        }

        private string StreamAsString(RawValueWriter target)
        {
            if (target.TextWriter != null)
            {
                target.TextWriter.Flush();
            }
            this.stream.Flush();
            this.stream.Position = 0;
            return new StreamReader(this.stream).ReadToEnd();
        }
    }
}
