//---------------------------------------------------------------------
// <copyright file="EdmValueToClrValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EdmValueToClrValueTests
    {
        private readonly EdmBooleanConstant booleanValue = new EdmBooleanConstant(EdmCoreModel.Instance.GetBoolean(false), true);
        private readonly EdmBinaryConstant binaryValue = new EdmBinaryConstant(EdmCoreModel.Instance.GetBinary(false), new byte[] { 1, 2 });
        private readonly EdmDateTimeOffsetConstant dateTimeOffsetValue = new EdmDateTimeOffsetConstant(EdmCoreModel.Instance.GetDateTimeOffset(false), DateTimeOffset.MinValue);
        private readonly EdmIntegerConstant byteValue = new EdmIntegerConstant(EdmCoreModel.Instance.GetByte(false), 1);
        private readonly EdmDecimalConstant decimalValue = new EdmDecimalConstant(EdmCoreModel.Instance.GetDecimal(false), 2.1M);
        private readonly EdmFloatingConstant doubleValue = new EdmFloatingConstant(EdmCoreModel.Instance.GetDouble(false), 1.2);
        private readonly EdmGuidConstant guidValue = new EdmGuidConstant(EdmCoreModel.Instance.GetGuid(false), Guid.Empty);
        private readonly EdmIntegerConstant int16Value = new EdmIntegerConstant(EdmCoreModel.Instance.GetInt16(false), 5);
        private readonly EdmIntegerConstant int32Value = new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(false), 10);
        private readonly EdmIntegerConstant int64Value = new EdmIntegerConstant(EdmCoreModel.Instance.GetInt64(false), 15);
        private readonly EdmIntegerConstant sbyteValue = new EdmIntegerConstant(EdmCoreModel.Instance.GetSByte(false), -1);
        private readonly EdmFloatingConstant singleValue = new EdmFloatingConstant(EdmCoreModel.Instance.GetSingle(false), 2.3f);
        private readonly EdmStringConstant stringValue = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), "foo");
        private readonly EdmDurationConstant durationValue = new EdmDurationConstant(EdmCoreModel.Instance.GetDuration(false), TimeSpan.MinValue);
        private readonly EdmDateConstant dateValue = new EdmDateConstant(EdmCoreModel.Instance.GetDate(false), new Date(2014, 8, 14));
        private readonly EdmTimeOfDayConstant timeOfDayValue = new EdmTimeOfDayConstant(EdmCoreModel.Instance.GetTimeOfDay(false), new TimeOfDay(1, 12, 5, 3));

        [TestMethod]
        public void AllPrimitiveTypesShouldBeCovered()
        {
            var enumValues = Enum.GetValues(typeof(EdmPrimitiveTypeKind)).Cast<EdmPrimitiveTypeKind>().ToList();
            var expectedMethods = new List<string>();
            foreach (var field in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                var fieldName = field.Name;
                fieldName = fieldName.Substring(0, 1).ToUpperInvariant() + fieldName.Substring(1);
                var expectedMethodName = "Edm" + fieldName + "ShouldMatch";
                expectedMethods.Add(expectedMethodName);

                var value = (IEdmPrimitiveValue)field.GetValue(this);
                enumValues.Remove(value.Type.PrimitiveKind()).Should().BeTrue();
            }

            var testMethods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.IsDefined(typeof(TestMethodAttribute), false))
                .Select(m => m.Name);
            testMethods.Should().Contain(expectedMethods);

            enumValues.Select(e => e.ToString())
                .Should()
                .OnlyContain(k => k == "None"
                    || k == "Stream"
                    || k.StartsWith("Geometry")
                    || k.StartsWith("Geography"));
        }

        [TestMethod]
        public void EdmBooleanValueShouldMatch()
        {
            ClrValueShouldMatch(this.booleanValue, this.booleanValue.Value);
        }

        [TestMethod]
        public void EdmBinaryValueShouldMatch()
        {
            ClrValueShouldMatch(this.binaryValue, this.binaryValue.Value);
        }

        [TestMethod]
        public void EdmByteValueShouldMatch()
        {
            ClrValueShouldMatch(this.byteValue, (byte)this.byteValue.Value);
        }

        [TestMethod]
        public void EdmDateTimeOffsetValueShouldMatch()
        {
            ClrValueShouldMatch(this.dateTimeOffsetValue, this.dateTimeOffsetValue.Value);
        }

        [TestMethod]
        public void EdmDecimalValueShouldMatch()
        {
            ClrValueShouldMatch(this.decimalValue, this.decimalValue.Value);
        }

        [TestMethod]
        public void EdmDoubleValueShouldMatch()
        {
            ClrValueShouldMatch(this.doubleValue, this.doubleValue.Value);
        }

        [TestMethod]
        public void EdmGuidValueShouldMatch()
        {
            ClrValueShouldMatch(this.guidValue, this.guidValue.Value);
        }

        [TestMethod]
        public void EdmInt16ValueShouldMatch()
        {
            ClrValueShouldMatch(this.int16Value, (short)this.int16Value.Value);
        }

        [TestMethod]
        public void EdmInt32ValueShouldMatch()
        {
            ClrValueShouldMatch(this.int32Value, (int)this.int32Value.Value);
        }

        [TestMethod]
        public void EdmInt64ValueShouldMatch()
        {
            ClrValueShouldMatch(this.int64Value, this.int64Value.Value);
        }

        [TestMethod]
        public void EdmSbyteValueShouldMatch()
        {
            ClrValueShouldMatch(this.sbyteValue, (sbyte)this.sbyteValue.Value);
        }

        [TestMethod]
        public void EdmSingleValueShouldMatch()
        {
            ClrValueShouldMatch(this.singleValue, (float)this.singleValue.Value);
        }

        [TestMethod]
        public void EdmStringValueShouldMatch()
        {
            ClrValueShouldMatch(this.stringValue, this.stringValue.Value);
        }

        [TestMethod]
        public void EdmDurationValueShouldMatch()
        {
            ClrValueShouldMatch(this.durationValue, this.durationValue.Value);
        }

        [TestMethod]
        public void EdmDateValueShouldMatch()
        {
            ClrValueShouldMatch(this.dateValue, this.dateValue.Value);
        }

        [TestMethod]
        public void EdmTimeOfDayValueShouldMatch()
        {
            ClrValueShouldMatch(this.timeOfDayValue, this.timeOfDayValue.Value);
        }

        [TestMethod]
        public void UnsupportedTypeShouldThrow()
        {
            var invalidType = new EdmPrimitiveValueSimulator(EdmCoreModel.Instance.GetString(false), EdmValueKind.Structured);
            Action convertToClr = () => invalidType.ToClrValue();
            convertToClr.ShouldThrow<InvalidOperationException>().WithMessage(Strings.EdmValueUtils_CannotConvertTypeToClrValue(invalidType.ValueKind));
        }

        private static void ClrValueShouldMatch<TValue>(IEdmPrimitiveValue edmValue, TValue expected)
        {
            edmValue.ToClrValue().Should().Be(expected);
        }
    }
}