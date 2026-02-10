//---------------------------------------------------------------------
// <copyright file="EdmExpressionEvaluatorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    public class EdmExpressionEvaluatorTests
    {
        private EdmExpressionEvaluator _evaluator;

        public EdmExpressionEvaluatorTests()
        {
            Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions = new Dictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>>();
            _evaluator = new EdmExpressionEvaluator(builtInFunctions);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EvaluateNotUnaryOperatorExpressionWorks(bool test)
        {
            IEdmBooleanConstantExpression operand = new EdmBooleanConstant(test);

            IEdmUnaryOperatorExpression unaryOperator = new EdmUnaryOperatorExpression(operand, EdmUnaryOperatorKind.Not);

            IEdmValue value = _evaluator.Evaluate(unaryOperator);
            VerifyBooleanValue(value, !test);
        }

        [Fact]
        public void EvaluateNotUnaryOperatorExpressionThrowsForNonBooleanOperand()
        {
            IEdmIntegerConstantExpression operand = new EdmIntegerConstant(42);

            IEdmUnaryOperatorExpression unaryOperator = new EdmUnaryOperatorExpression(operand, EdmUnaryOperatorKind.Not);

            Action test = () => _evaluator.Evaluate(unaryOperator);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal("Unary Expression with operator kind 'Not' cannot be evaluated on value of kind 'Integer'.", exception.Message);
        }

        [Fact]
        public void EvaluateNegUnaryOperatorExpressionWorks()
        {
            // Integer
            IEdmIntegerConstantExpression operand1 = new EdmIntegerConstant(42);
            IEdmUnaryOperatorExpression unaryOperator = new EdmUnaryOperatorExpression(operand1, EdmUnaryOperatorKind.Negate);

            IEdmValue result = _evaluator.Evaluate(unaryOperator);
            VerifyIntegerValue(result, -42);

            // Floating
            IEdmFloatingConstantExpression operand2 = new EdmFloatingConstant(42.05);
            unaryOperator = new EdmUnaryOperatorExpression(operand2, EdmUnaryOperatorKind.Negate);

            result = _evaluator.Evaluate(unaryOperator);
            VerifyFloatValue(result, -42.05);

            // Decimal
            IEdmDecimalConstantExpression operand3 = new EdmDecimalConstant(42.05m);
            unaryOperator = new EdmUnaryOperatorExpression(operand3, EdmUnaryOperatorKind.Negate);

            result = _evaluator.Evaluate(unaryOperator);
            VerifyDecimalValue(result, -42.05m);
        }

        [Fact]
        public void EvaluateNegUnaryOperatorExpressionThrowsForNonNumberOperand()
        {
            IEdmBooleanConstantExpression operand = new EdmBooleanConstant(false);

            IEdmUnaryOperatorExpression unaryOperator = new EdmUnaryOperatorExpression(operand, EdmUnaryOperatorKind.Negate);

            Action test = () => _evaluator.Evaluate(unaryOperator);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal("Unary Expression with operator kind 'Negate' cannot be evaluated on value of kind 'Boolean'.", exception.Message);
        }

        [Theory]
        [InlineData(true, true, EdmBinaryOperatorKind.And, true)]
        [InlineData(true, false, EdmBinaryOperatorKind.And, false)]
        [InlineData(false, true, EdmBinaryOperatorKind.And, false)]
        [InlineData(false, false, EdmBinaryOperatorKind.And, false)]
        [InlineData(true, true, EdmBinaryOperatorKind.Or, true)]
        [InlineData(true, false, EdmBinaryOperatorKind.Or, true)]
        [InlineData(false, true, EdmBinaryOperatorKind.Or, true)]
        [InlineData(false, false, EdmBinaryOperatorKind.Or, false)]
        public void EvaluateLogicalBinaryOperatorExpressionWorksOnBoolean(bool left, bool right, EdmBinaryOperatorKind kind, bool expected)
        {
            IEdmBooleanConstantExpression leftExp = new EdmBooleanConstant(left);
            IEdmBooleanConstantExpression rightExp = new EdmBooleanConstant(right);

            IEdmBinaryOperatorExpression unaryOperator = new EdmBinaryOperatorExpression(leftExp, rightExp, kind);

            IEdmValue value = _evaluator.Evaluate(unaryOperator);
            VerifyBooleanValue(value, expected);
        }

        [Fact]
        public void EvaluateEqualComparisonBinaryOperatorExpressionWorksForNull()
        {
            IEdmNullExpression nullExp = EdmNullExpression.Instance;
            IEdmIntegerConstantExpression intergerExp = new EdmIntegerConstant(42);

            // null == 42 ?
            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(nullExp, intergerExp, EdmBinaryOperatorKind.Eq);

            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, false);

            // 42 == null ?
            binaryOperator = new EdmBinaryOperatorExpression(intergerExp, nullExp, EdmBinaryOperatorKind.Eq);

            value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, false);

            // null == null ?
            binaryOperator = new EdmBinaryOperatorExpression(nullExp, nullExp, EdmBinaryOperatorKind.Eq);
            value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, true);
        }

        [Theory]
        [InlineData(EdmBinaryOperatorKind.Eq, true)]
        [InlineData(EdmBinaryOperatorKind.Ne, false)]
        public void EvaluateComparisonBinaryOperatorExpressionWorksForGuid(EdmBinaryOperatorKind kind, bool expected)
        {
            IEdmGuidConstantExpression leftGuid = new EdmGuidConstant(Guid.Parse("12345678-1234-1234-1234-1234567890ab"));
            IEdmGuidConstantExpression rightGuid = new EdmGuidConstant(Guid.Parse("12345678-1234-1234-1234-1234567890ab"));

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftGuid, rightGuid, kind);
            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, expected);
        }

        [Theory]
        [InlineData(EdmBinaryOperatorKind.Eq, false)]
        [InlineData(EdmBinaryOperatorKind.Ne, true)]
        [InlineData(EdmBinaryOperatorKind.Gt, false)]
        [InlineData(EdmBinaryOperatorKind.Ge, false)]
        [InlineData(EdmBinaryOperatorKind.Lt, true)]
        [InlineData(EdmBinaryOperatorKind.Le, true)]
        public void EvaluateComparisonBinaryOperatorExpressionWorksForDate(EdmBinaryOperatorKind kind, bool expected)
        {
            IEdmDateConstantExpression leftDate = new EdmDateConstant(DateOnly.Parse("2026-02-10"));
            IEdmDateConstantExpression rightDate = new EdmDateConstant(DateOnly.Parse("2026-02-11"));

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftDate, rightDate, kind);
            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, expected);
        }

        [Theory]
        [InlineData(EdmBinaryOperatorKind.Eq, true)]
        [InlineData(EdmBinaryOperatorKind.Ne, false)]
        [InlineData(EdmBinaryOperatorKind.Gt, false)]
        [InlineData(EdmBinaryOperatorKind.Ge, true)]
        [InlineData(EdmBinaryOperatorKind.Lt, false)]
        [InlineData(EdmBinaryOperatorKind.Le, true)]
        public void EvaluateComparisonBinaryOperatorExpressionWorksForIntegers(EdmBinaryOperatorKind kind, bool expected)
        {
            IEdmIntegerConstantExpression leftExp = new EdmIntegerConstant(42);
            IEdmIntegerConstantExpression rightExp = new EdmIntegerConstant(42);

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftExp, rightExp, kind);

            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, expected);
        }

        [Theory]
        [InlineData(1, 1, EdmBinaryOperatorKind.Add, 1 + 1)]
        [InlineData(15, 12, EdmBinaryOperatorKind.Sub, 15 - 12)]
        [InlineData(8, 3, EdmBinaryOperatorKind.Mul, 8 * 3)]
        [InlineData(9, 2, EdmBinaryOperatorKind.Div, 9 / 2)]
        [InlineData(9, 2, EdmBinaryOperatorKind.Mod, 9 % 2)]
        public void EvaluateBinaryOperatorExpressionWorksOnIntegers(long left, long right, EdmBinaryOperatorKind kind, long expected)
        {
            IEdmIntegerConstantExpression leftExp = new EdmIntegerConstant(left);
            IEdmIntegerConstantExpression rightExp = new EdmIntegerConstant(right);

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftExp, rightExp, kind);

            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyIntegerValue(value, expected);
        }

        [Theory]
        [InlineData(1.1, 2.1, EdmBinaryOperatorKind.Add, 1.1 + 2.1)]
        [InlineData(15.8, 12.4, EdmBinaryOperatorKind.Sub, 15.8 - 12.4)]
        [InlineData(15.9, 3.2, EdmBinaryOperatorKind.Mul, 15.9 * 3.2)]
        [InlineData(9, 2, EdmBinaryOperatorKind.Div, 9.0 / 2)]
        [InlineData(9, 2, EdmBinaryOperatorKind.DivBy, 9.0 / 2)]
        public void EvaluateBinaryOperatorExpressionWorksOnFloatings(double left, double right, EdmBinaryOperatorKind kind, double expected)
        {
            IEdmFloatingConstantExpression leftExp = new EdmFloatingConstant(left);
            IEdmFloatingConstantExpression rightExp = new EdmFloatingConstant(right);

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftExp, rightExp, kind);

            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyFloatValue(value, expected);
        }

        [Theory]
        [InlineData(1.1, 2.1, EdmBinaryOperatorKind.Add, 1.1 + 2.1)]
        [InlineData(15.8, 12.4, EdmBinaryOperatorKind.Sub, 15.8 - 12.4)]
        [InlineData(15.9, 3.2, EdmBinaryOperatorKind.Mul, 15.9 * 3.2)]
        [InlineData(9, 2, EdmBinaryOperatorKind.Div, 9.0 / 2)]
        [InlineData(9.0000000000001, 2, EdmBinaryOperatorKind.DivBy, 9.0000000000001 / 2)]
        public void EvaluateBinaryOperatorExpressionWorksOnDecimals(decimal left, decimal right, EdmBinaryOperatorKind kind, decimal expected)
        {
            IEdmDecimalConstantExpression leftExp = new EdmDecimalConstant(left);
            IEdmDecimalConstantExpression rightExp = new EdmDecimalConstant(right);

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftExp, rightExp, kind);

            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyDecimalValue(value, expected);
        }

        [Fact]
        public void EvaluateAddBinaryOperatorExpressionWorksForString()
        {
            IEdmStringConstantExpression leftExp = new EdmStringConstant("hello");
            IEdmStringConstantExpression rightExp = new EdmStringConstant(" world");

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftExp, rightExp, EdmBinaryOperatorKind.Add);

            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyStringValue(value, "hello world");
        }

        [Theory]
        [InlineData(EdmBinaryOperatorKind.Add)]
        [InlineData(EdmBinaryOperatorKind.Sub)]
        public void EvaluateAddBinaryOperatorExpressionWorksForDateTimeOffsetAndDuration(EdmBinaryOperatorKind kind)
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(2026, 2, 10, 11, 25, 24, TimeSpan.Zero);
            TimeSpan timeSpan = new TimeSpan(1, 2, 3, 4);

            IEdmDateTimeOffsetConstantExpression leftExp = new EdmDateTimeOffsetConstant(dateTimeOffset);
            IEdmDurationConstantExpression rightExp = new EdmDurationConstant(timeSpan);

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftExp, rightExp, kind);

            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            if (kind == EdmBinaryOperatorKind.Add)
            {
                VerifyDateTimeOffsetValue(value, dateTimeOffset + timeSpan);
            }
            else
            {
                VerifyDateTimeOffsetValue(value, dateTimeOffset - timeSpan);
            }
        }

        [Fact]
        public void EvaluateHasBinaryOperatorExpressionWorksForEnum()
        {
            var (enumType, red, green, yellow, blue) = BuildEnumType(true);

            IEdmEnumMemberExpression leftEnum = new EdmEnumMemberExpression(red, green, yellow);
            IEdmEnumMemberExpression rightEnum = new EdmEnumMemberExpression(green, yellow);

            // Red, Green, Yellow has Green, Yellow ?
            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftEnum, rightEnum, EdmBinaryOperatorKind.Has);
            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, true);

            // Red, Green, Yellow has Blue ?
             binaryOperator = new EdmBinaryOperatorExpression(rightEnum, new EdmEnumMemberExpression(blue), EdmBinaryOperatorKind.Has);
             value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, false);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(5, true)]
        [InlineData(17, true)]
        [InlineData(99, false)]
        public void EvaluateInBinaryOperatorExpressionWorksForCollection(long test, bool expected)
        {
            IEdmIntegerConstantExpression leftInteger = new EdmIntegerConstant(test);
            IEdmCollectionExpression rightCollection = new EdmCollectionExpression(
                new EdmIntegerConstant(2),
                new EdmIntegerConstant(3),
                new EdmIntegerConstant(5),
                new EdmIntegerConstant(7),
                new EdmIntegerConstant(11),
                new EdmIntegerConstant(13),
                new EdmIntegerConstant(17),
                new EdmIntegerConstant(19));

            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftInteger, rightCollection, EdmBinaryOperatorKind.In);
            IEdmValue value = _evaluator.Evaluate(binaryOperator);
            VerifyBooleanValue(value, expected);
        }

        [Fact]
        public void EvaluateBinaryOperatorExpressionThrowsForNonValidLeftAndRight()
        {
            IEdmBooleanConstantExpression leftBoolean = new EdmBooleanConstant(false);
            IEdmIntegerConstantExpression rightInteger = new EdmIntegerConstant(42);

            // Boolean + Integer is not valid.
            IEdmBinaryOperatorExpression binaryOperator = new EdmBinaryOperatorExpression(leftBoolean, rightInteger, EdmBinaryOperatorKind.Add);

            Action test = () => _evaluator.Evaluate(binaryOperator);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(test);
            Assert.Equal("Binary Expression with operator kind 'Add' cannot be evaluated on left value of kind 'Boolean' and right value of kind 'Integer'.", exception.Message);
        }

        private static (IEdmEnumType, IEdmEnumMember, IEdmEnumMember, IEdmEnumMember, IEdmEnumMember) BuildEnumType(bool isFlag)
        {
            var enumType = new EdmEnumType("TestNamespace", "TestColorType", isFlag);
            var red = new EdmEnumMember(enumType, "Red", new EdmEnumMemberValue(1));
            enumType.AddMember(red);

            var green = new EdmEnumMember(enumType, "Green", new EdmEnumMemberValue(2));
            enumType.AddMember(green);

            var yellow = new EdmEnumMember(enumType, "Yellow", new EdmEnumMemberValue(4));
            enumType.AddMember(yellow);

            var blue = new EdmEnumMember(enumType, "Blue", new EdmEnumMemberValue(8));
            enumType.AddMember(blue);

            return (enumType, red, green, yellow, blue);
        }

        private static void VerifyBooleanValue(IEdmValue value, bool expected)
        {
            Assert.NotNull(value);
            Assert.IsType<EdmBooleanConstant>(value);
            EdmBooleanConstant booleanValue = (EdmBooleanConstant)value;
            Assert.Equal(expected, booleanValue.Value);
        }

        private static void VerifyIntegerValue(IEdmValue value, long expected)
        {
            Assert.NotNull(value);
            IEdmIntegerValue integerValue = value as IEdmIntegerValue;
            Assert.NotNull(integerValue);
            Assert.Equal(expected, integerValue.Value);
        }

        private static void VerifyFloatValue(IEdmValue value, double expected)
        {
            Assert.NotNull(value);
            IEdmFloatingValue floatingValue = value as IEdmFloatingValue;
            Assert.NotNull(floatingValue);
            Assert.Equal(expected, floatingValue.Value);
        }

        private static void VerifyDecimalValue(IEdmValue value, decimal expected)
        {
            Assert.NotNull(value);
            IEdmDecimalValue decimalValue = value as IEdmDecimalValue;
            Assert.NotNull(decimalValue);
            Assert.Equal(expected, decimalValue.Value);
        }

        private static void VerifyStringValue(IEdmValue value, string expected)
        {
            Assert.NotNull(value);
            IEdmStringValue stringValue = value as IEdmStringValue;
            Assert.NotNull(stringValue);
            Assert.Equal(expected, stringValue.Value);
        }

        private static void VerifyDateTimeOffsetValue(IEdmValue value, DateTimeOffset expected)
        {
            Assert.NotNull(value);
            IEdmDateTimeOffsetValue dateTimeOffsetValue = value as IEdmDateTimeOffsetValue;
            Assert.NotNull(dateTimeOffsetValue);
            Assert.Equal(expected, dateTimeOffsetValue.Value);
        }
    }
}
