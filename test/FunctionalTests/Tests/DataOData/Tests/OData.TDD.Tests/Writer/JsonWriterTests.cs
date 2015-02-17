//---------------------------------------------------------------------
// <copyright file="JsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer
{
    using System;
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the JsonWriter class.
    /// TODO: write unit tests for the remaining functions on JsonWriter.
    /// </summary>
    [TestClass]
    public class JsonWriterTests
    {
        private StringBuilder builder;
        private IJsonWriter writer;

        [TestInitialize]
        public void Initialize()
        {
            this.builder = new StringBuilder();
            this.writer = new JsonWriter(new StringWriter(builder), false /*indent*/, ODataFormat.Json, isIeee754Compatible: true);
        }

        [TestMethod]
        public void StartPaddingFunctionScopeWritesParenthesis()
        {
            this.writer.StartPaddingFunctionScope();
            this.builder.ToString().Should().Be("(");
        }

        [TestMethod]
        public void EndPaddingFunctionScopeWritesParenthesis()
        {
            this.writer.StartPaddingFunctionScope();
            this.writer.EndPaddingFunctionScope();
            this.builder.ToString().Should().Be("()");
        }

        [TestMethod]
        public void WritePaddingFunctionNameWritesName()
        {
            this.writer.WritePaddingFunctionName("example");
            this.builder.ToString().Should().Be("example");
        }

        #region WritePrimitiveValue

        [TestMethod]
        public void WritePrimitiveValueBoolean()
        {
            this.VerifyWritePrimitiveValue(false, "false");
        }

        [TestMethod]
        public void WritePrimitiveValueByte()
        {
            this.VerifyWritePrimitiveValue((byte)4, "4");
        }

        [TestMethod]
        public void WritePrimitiveValueDecimalWithIeee754CompatibleTrue()
        {
            this.VerifyWriterPrimitiveValueWithIeee754Compatible(42.2m, "\"42.2\"", isIeee754Compatible: true);
        }

        [TestMethod]
        public void WritePrimitiveValueDecimalWithIeee754CompatibleFalse()
        {
            this.VerifyWriterPrimitiveValueWithIeee754Compatible(42.2m, "42.2", isIeee754Compatible: false);
        }

        [TestMethod]
        public void WritePrimitiveValueDouble()
        {
            this.VerifyWritePrimitiveValue(42.2d, "42.2");
        }

        [TestMethod]
        public void WritePrimitiveValueInt16()
        {
            this.VerifyWritePrimitiveValue((short)876, "876");
        }

        [TestMethod]
        public void WritePrimitiveValueInt32()
        {
            this.VerifyWritePrimitiveValue((int)876, "876");
        }

        [TestMethod]
        public void WritePrimitiveValueInt64WithIeee754CompatibleTrue()
        {
            this.VerifyWriterPrimitiveValueWithIeee754Compatible((long)876, "\"876\"", isIeee754Compatible: true);
        }

        [TestMethod]
        public void WritePrimitiveValueInt64WithIeee754CompatibleFalse()
        {
            this.VerifyWriterPrimitiveValueWithIeee754Compatible((long)876, "876", isIeee754Compatible: false);
        }

        [TestMethod]
        public void WritePrimitiveValueSByte()
        {
            this.VerifyWritePrimitiveValue((sbyte)4, "4");
        }

        [TestMethod]
        public void WritePrimitiveValueSingle()
        {
            this.VerifyWritePrimitiveValue((Single)876, "876");
        }

        [TestMethod]
        public void WritePrimitiveValueString()
        {
            this.VerifyWritePrimitiveValue("string", "\"string\"");
        }

        [TestMethod]
        public void WritePrimitiveValueByteArray()
        {
            this.VerifyWritePrimitiveValue(new byte[] { 0 }, "\"" + Convert.ToBase64String(new byte[] { 0 }) + "\"");
        }

        [TestMethod]
        public void WritePrimitiveValueDateTimeOffset()
        {
            this.VerifyWritePrimitiveValue(new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, new TimeSpan(1, 2, 0)), "\"0001-02-03T04:05:06.007+01:02\"");
        }

        [TestMethod]
        public void WritePrimitiveValueGuid()
        {
            this.VerifyWritePrimitiveValue(new Guid("00000012-0000-0000-0000-012345678900"), "\"00000012-0000-0000-0000-012345678900\"");
        }

        [TestMethod]
        public void WritePrimitiveValueTimeSpan()
        {
            this.VerifyWritePrimitiveValue(new TimeSpan(1, 2, 3, 4, 5), "\"P1DT2H3M4.005S\"");
        }

        [TestMethod]
        public void WritePrimitiveValueDate()
        {
            this.VerifyWritePrimitiveValue(new Date(2014, 12, 31), "\"2014-12-31\"");
        }

        [TestMethod]
        public void WritePrimitiveValueTimeOfDay()
        {
            this.VerifyWritePrimitiveValue(new TimeOfDay(12, 30, 5, 10), "\"12:30:05.0100000\"");
        }

        private void VerifyWritePrimitiveValue<T>(T parameter, string expected)
        {
            this.writer.WritePrimitiveValue(parameter);
            this.builder.ToString().Should().Be(expected);
        }

        private void VerifyWriterPrimitiveValueWithIeee754Compatible<T>(T parameter, string expected, bool isIeee754Compatible)
        {
            this.writer = new JsonWriter(new StringWriter(builder), false, ODataFormat.Json, isIeee754Compatible);
            this.writer.WritePrimitiveValue(parameter);
            this.builder.ToString().Should().Be(expected);
        }

        #endregion
    }
}
