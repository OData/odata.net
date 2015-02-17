//---------------------------------------------------------------------
// <copyright file="RawValueWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests
{
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;

    /// <summary>
    /// This is a test class for RawValueWriter and is intended
    /// to contain all RawValueWriter Unit Tests
    ///</summary>
    [TestClass()]
    public class RawValueWriterTests
    {
        private MemoryStream stream;
        private ODataMessageWriterSettings settings;

        [TestInitialize]
        public void Initialize()
        {
            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings();
        }

        [TestMethod()]
        public void StartDoesNothingNormally()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.Start();
            this.StreamAsString(target).Should().BeEmpty();
        }

        [TestMethod()]
        public void StartWithJsonPadding()
        {
            this.settings.JsonPCallback = "foo";
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.Start();
            this.StreamAsString(target).Should().Be("foo(");
        }

        [TestMethod()]
        public void EndDoesNothingNormally()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            target.End();
            this.StreamAsString(target).Should().BeEmpty();
        }

        [TestMethod()]
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
        [TestMethod()]
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
        [TestMethod()]
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
        [TestMethod()]
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
        [TestMethod()]
        public void WriteRawValueWritesGeographyValue()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            var value = GeographyPoint.Create(22.2, 22.2);
            target.WriteRawValue(value);
            this.StreamAsString(target).Should().Be("SRID=4326;POINT (22.2 22.2)");
        }

        /// <summary>
        ///A test for writing raw Geometry value
        ///</summary>
        [TestMethod()]
        public void WriteRawValueWritesGeometryValue()
        {
            RawValueWriter target = new RawValueWriter(this.settings, this.stream, new UTF32Encoding());
            var value2 = GeometryPoint.Create(1.2, 3.16);
            target.WriteRawValue(value2);
            this.StreamAsString(target).Should().Be("SRID=0;POINT (1.2 3.16)");
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
