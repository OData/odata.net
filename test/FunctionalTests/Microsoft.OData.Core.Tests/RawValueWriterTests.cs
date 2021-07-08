//---------------------------------------------------------------------
// <copyright file="RawValueWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
            Assert.Empty(this.StreamAsString(target));
        }

        [Fact]
        public void StartWithJsonPadding()
        {
            this.settings.JsonPCallback = "foo";
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.Start();
            Assert.Equal("foo(", this.StreamAsString(target));
        }

        [Fact]
        public void EndDoesNothingNormally()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.End();
            Assert.Empty(this.StreamAsString(target));
        }

        [Fact]
        public void EndWithJsonPadding()
        {
            this.settings.JsonPCallback = "foo";
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.End();
            Assert.Equal(")", this.StreamAsString(target));
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
            Assert.Equal(value, this.StreamAsString(target));
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
            Assert.Equal("2014-09-18", this.StreamAsString(target));
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
            Assert.Equal("09:47:05.9000000", this.StreamAsString(target));
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
            Assert.Equal(@"{""type"":""Point"",""coordinates"":[22.2,22.2],""crs"":{""type"":""name"",""properties"":{""name"":""EPSG:4326""}}}", this.StreamAsString(target));
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
            Assert.Equal(@"{""type"":""Point"",""coordinates"":[1.2,3.16],""crs"":{""type"":""name"",""properties"":{""name"":""EPSG:0""}}}", this.StreamAsString(target));
        }

        [Fact]
        public async Task RawValueWriterStartAsync()
        {
            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.StartAsync());

            Assert.Equal("", result);
        }

        [Fact]
        public async Task RawValueWriterStartAsyncWithJsonPadding()
        {
            this.settings.JsonPCallback = "foo";

            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.StartAsync());

            Assert.Equal("foo(", result);
        }

        [Fact]
        public async Task RawValueWriterEndAsync()
        {
            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.EndAsync());

            Assert.Equal("", result);
        }

        [Fact]
        public async Task RawValueWriterEndAsyncWithJsonPadding()
        {
            this.settings.JsonPCallback = "foo";

            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.EndAsync());

            Assert.Equal(")", result);
        }

        [Fact]
        public async Task WriteRawStringValueAsync()
        {
            var value = "foobar";

            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.WriteRawValueAsync(value));

            Assert.Equal("foobar", result);
        }

        [Fact]
        public async Task WriteRawDateValueAsync()
        {
            var value = new Date(2014, 9, 18);

            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.WriteRawValueAsync(value));

            Assert.Equal("2014-09-18", result);
        }

        [Fact]
        public async Task WriteRawTimeOfDayValueAsync()
        {
            var value = new TimeOfDay(9, 47, 5, 900);

            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.WriteRawValueAsync(value));

            Assert.Equal("09:47:05.9000000", result);
        }

        [Fact]
        public async Task WriteRawGeographyValueAsync()
        {
            var value = GeographyPoint.Create(22.2, 22.2);

            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.WriteRawValueAsync(value));

            Assert.Equal(
                "{\"type\":\"Point\",\"coordinates\":[22.2,22.2],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                result);
        }

        [Fact]
        public async Task WriteRawGeometryValuesync()
        {
            var value = GeometryPoint.Create(1.2, 3.16);

            var result = await SetupRawValueWriterAndRunTestAsync(
                (rawValueWriter) => rawValueWriter.WriteRawValueAsync(value));

            Assert.Equal(
                "{\"type\":\"Point\",\"coordinates\":[1.2,3.16],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}",
                result);
        }

        private async Task<string> SetupRawValueWriterAndRunTestAsync(Func<RawValueWriter, Task> func)
        {
            var rawValueWriter = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());

            await func(rawValueWriter);

            await rawValueWriter.FlushAsync();
            await this.stream.FlushAsync();
            
            this.stream.Position = 0;
            return await new StreamReader(this.stream).ReadToEndAsync();
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
