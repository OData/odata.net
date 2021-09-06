//---------------------------------------------------------------------
// <copyright file="ConstructibleExpressionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstructibleExpressionsTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void EdmBinaryConstant()
        {
            var e = new EdmBinaryConstant(new byte[] { 1, 2, 3 });
            Assert.AreEqual(EdmExpressionKind.BinaryConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(3, e.Value[2], "e.Value[2]");

            e = new EdmBinaryConstant(EdmCoreModel.Instance.GetBinary(true, null, true), new byte[] { 3, 2, 1 });
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual(true, e.Type.AsBinary().IsUnbounded, "e.Type.AsBinary().isUnbounded");
            Assert.AreEqual(1, e.Value[2], "e.Value[2]");

            e = new EdmBinaryConstant(null, new byte[] { 3, 2, 1 });
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");

            try
            {
                new EdmBinaryConstant(null);
                Assert.Fail("exception expected.");
            }
            catch (Exception ex1)
            {
                Assert.AreEqual(typeof(ArgumentNullException), ex1.GetType(), "ArgumentNullException expected");
            }
            try
            {
                new EdmBinaryConstant(EdmCoreModel.Instance.GetBinary(true), null);
                Assert.Fail("exception expected.");
            }
            catch (Exception ex2)
            {
                Assert.AreEqual(typeof(ArgumentNullException), ex2.GetType(), "ArgumentNullException expected");
            }
        }

        [TestMethod]
        public void EdmBooleanConstant()
        {
            var e = new EdmBooleanConstant(true);
            Assert.AreEqual(EdmExpressionKind.BooleanConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(true, e.Value, "e.Value");

            e = new EdmBooleanConstant(EdmCoreModel.Instance.GetBinary(true, null, true), false);
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual(true, e.Type.AsBinary().IsUnbounded, "e.Type.AsBinary().isUnbounded");
            Assert.AreEqual(false, e.Value, "e.Value");

            e = new EdmBooleanConstant(null, true);
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmDurationConstant()
        {
            var e = new EdmDurationConstant(new TimeSpan(1, 2, 3));
            Assert.AreEqual(EdmExpressionKind.DurationConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(new TimeSpan(1, 2, 3), e.Value, "e.Value");

            e = new EdmDurationConstant(EdmCoreModel.Instance.GetDuration(true), new TimeSpan(3, 2, 1));
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual(new TimeSpan(3, 2, 1), e.Value, "e.Value");

            e = new EdmDurationConstant(null, new TimeSpan(2, 4, 8));
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmDateTimeOffsetConstant()
        {
            var e = new EdmDateTimeOffsetConstant(new DateTimeOffset(2011, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)));
            Assert.AreEqual(EdmExpressionKind.DateTimeOffsetConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(new DateTimeOffset(2011, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)), e.Value, "e.Value");

            e = new EdmDateTimeOffsetConstant(EdmCoreModel.Instance.GetDateTimeOffset(true), new DateTimeOffset(2211, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)));
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual(new DateTimeOffset(2211, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)), e.Value, "e.Value");

            e = new EdmDateTimeOffsetConstant(null, new DateTimeOffset(2011, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)));
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmDateConstant()
        {
            var e = new EdmDateConstant(new Date(2014, 8, 8));
            Assert.AreEqual(EdmExpressionKind.DateConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(new Date(2014, 8, 8), e.Value, "e.Value");

            e = new EdmDateConstant(EdmCoreModel.Instance.GetDate(true), new Date(2014, 8, 8));
            Assert.AreEqual(e.Type.AsPrimitive().PrimitiveKind(), EdmPrimitiveTypeKind.Date);
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual(new Date(2014, 8, 8), e.Value, "e.Value");

            e = new EdmDateConstant(null, new Date(2014, 8, 8));
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmDecimalConstant()
        {
            var e = new EdmDecimalConstant((decimal)11.22);
            Assert.AreEqual(EdmExpressionKind.DecimalConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual((decimal)11.22, e.Value, "e.Value");

            e = new EdmDecimalConstant(EdmCoreModel.Instance.GetDecimal(true), (decimal)33.22);
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual((decimal)33.22, e.Value, "e.Value");

            e = new EdmDecimalConstant(null, (decimal)11.22);
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmFloatingConstant()
        {
            var e = new EdmFloatingConstant(11.22);
            Assert.AreEqual(EdmExpressionKind.FloatingConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(11.22, e.Value, "e.Value");

            e = new EdmFloatingConstant(EdmCoreModel.Instance.GetDouble(true), 33.22);
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual(33.22, e.Value, "e.Value");

            e = new EdmFloatingConstant(null, 11.22);
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmGuidConstant()
        {
            var e = new EdmGuidConstant(new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
            Assert.AreEqual(EdmExpressionKind.GuidConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }), e.Value, "e.Value");

            e = new EdmGuidConstant(EdmCoreModel.Instance.GetGuid(true), new Guid(new byte[] { 100, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual(new Guid(new byte[] { 100, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }), e.Value, "e.Value");

            e = new EdmGuidConstant(null, new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmIntegerConstant()
        {
            var e = new EdmIntegerConstant(2);
            Assert.AreEqual(EdmExpressionKind.IntegerConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(2, e.Value, "e.Value");

            e = new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(true), 3);
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual(3, e.Value, "e.Value");

            e = new EdmIntegerConstant(null, 2);
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmStringConstant()
        {
            var e = new EdmStringConstant("qqq");
            Assert.AreEqual(EdmExpressionKind.StringConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual("qqq", e.Value, "e.Value");

            e = new EdmStringConstant(EdmCoreModel.Instance.GetString(true), "aaa");
            Assert.AreEqual(true, e.Type.IsNullable, "e.Type.IsNullable");
            Assert.AreEqual("aaa", e.Value, "e.Value");

            e = new EdmStringConstant(null, "qqq");
            Assert.IsNull(e.Type, "e.Type");

            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");

            try
            {
                new EdmStringConstant(null);
                Assert.Fail("exception expected.");
            }
            catch (Exception ex1)
            {
                Assert.AreEqual(typeof(ArgumentNullException), ex1.GetType(), "ArgumentNullException expected");
            }
            try
            {
                new EdmStringConstant(EdmCoreModel.Instance.GetString(true), null);
                Assert.Fail("exception expected.");
            }
            catch (Exception ex2)
            {
                Assert.AreEqual(typeof(ArgumentNullException), ex2.GetType(), "ArgumentNullException expected");
            }
        }

        [TestMethod]
        public void EdmTimeOfDayConstant()
        {
            var e = new EdmTimeOfDayConstant(new TimeOfDay(12, 30, 50, 0));
            Assert.AreEqual(EdmExpressionKind.TimeOfDayConstant, e.ExpressionKind, "e.ExpressionKind");
            Assert.IsNull(e.Type, "e.Type");
            Assert.AreEqual(new TimeOfDay(12, 30, 50, 0), e.Value, "e.Value");

            e = new EdmTimeOfDayConstant(EdmCoreModel.Instance.GetTimeOfDay(true), new TimeOfDay(1, 5, 10, 10));
            Assert.AreEqual(e.Type.AsPrimitive().PrimitiveKind(), EdmPrimitiveTypeKind.TimeOfDay);
            Assert.AreEqual(new TimeOfDay(1, 5, 10, 10), e.Value, "e.Value");

            e = new EdmTimeOfDayConstant(null, new TimeOfDay(23, 50, 11, 999));
            Assert.IsNull(e.Type, "e.Type");
            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");
        }

        [TestMethod]
        public void EdmCastExpression()
        {
            var e = new EdmCastExpression(new EdmStringConstant("qwerty"), EdmCoreModel.Instance.GetBoolean(false));
            Assert.AreEqual(EdmExpressionKind.Cast, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual("qwerty", ((IEdmStringValue)e.Operand).Value, "((IEdmStringValue)e.Operand).Value");
            Assert.AreEqual("Edm.Boolean", e.Type.FullName(), "e.Type.FullName()");
            Assert.IsFalse(e.IsBad(), "e good");

            try
            {
                new EdmCastExpression(null, EdmCoreModel.Instance.GetBoolean(false));
                Assert.Fail("exception expected.");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new EdmCastExpression(new EdmStringConstant("qwerty"), null);
                Assert.Fail("exception expected.");
            }
            catch (ArgumentNullException)
            {
            }

            var ee = new MutableCastExpression();
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(2, ee.Errors().Count(), "Expression has 2 errors");
        }

        private sealed class MutableCastExpression : IEdmCastExpression
        {
            public IEdmExpression Operand
            {
                get;
                set;
            }

            public IEdmTypeReference Type
            {
                get;
                set;
            }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.Cast; }
            }
        }

        [TestMethod]
        public void EdmLabeledExpression()
        {
            var e = new EdmLabeledExpression("l1", new EdmStringConstant("qwerty"));
            Assert.AreEqual("l1", e.Name, "e.Label");
            Assert.AreEqual("qwerty", ((IEdmStringValue)e.Expression).Value, ((IEdmStringValue)e.Expression).Value);
            Assert.IsFalse(e.IsBad(), "e good");

            try
            {
                new EdmLabeledExpression(null, new EdmStringConstant("qwerty"));
                Assert.Fail("exception expected.");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new EdmLabeledExpression("l1", null);
                Assert.Fail("exception expected.");
            }
            catch (ArgumentNullException)
            {
            }

            var ee = new MutableLabeledExpression();
            Assert.IsNull(ee.Name, "ee.Label");
            Assert.IsNull(ee.Expression, "ee.Expression");
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(2, ee.Errors().Count(), "Expression has errors");
        }

        private sealed class MutableLabeledExpression : IEdmLabeledExpression
        {
            public IEdmExpression Expression
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.Labeled; }
            }
        }

        [TestMethod]
        public void EdmCollectionExpression()
        {
            var e = new EdmCollectionExpression(
                new EdmLabeledExpression("l1", new EdmStringConstant("qwerty")),
                new EdmLabeledExpression("l2", new EdmStringConstant("qwerty2")));
            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(EdmExpressionKind.Collection, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual(2, e.Elements.Count(), "e.Elements.Count()");
            Assert.AreEqual("l2", ((EdmLabeledExpression)e.Elements.ElementAt(1)).Name, "((EdmLabeledElement)e.Elements.ElementAt(1)).Label");
            var l1 = e.Elements.First();

            e = new EdmCollectionExpression();
            Assert.IsNull(e.DeclaredType, "e.DeclaredType");
            Assert.AreEqual(0, e.Elements.Count(), "e.Elements.Count()");
            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");

            try
            {
                new EdmCollectionExpression((IEdmLabeledExpression[])null);
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void EdmLabeledExpressionReferenceExpression()
        {
            var e = new EdmLabeledExpressionReferenceExpression(new EdmLabeledExpression("qq", EdmNullExpression.Instance));
            Assert.IsFalse(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(EdmExpressionKind.LabeledExpressionReference, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual("qq", ((EdmLabeledExpression)e.ReferencedLabeledExpression).Name, "((EdmLabeledExpression)e.ReferencedLabeledExpression).Name");

            try
            {
                e.ReferencedLabeledExpression = new EdmLabeledExpression("qqq", EdmNullExpression.Instance);
                Assert.Fail("exception expected");
            }
            catch (InvalidOperationException)
            {
            }

            e = new EdmLabeledExpressionReferenceExpression();
            Assert.IsTrue(e.IsBad(), "Expression not bad.");
            Assert.AreEqual(1, e.Errors().Count(), "Expression has errors");

            try
            {
                e.ReferencedLabeledExpression = null;
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            e.ReferencedLabeledExpression = new EdmLabeledExpression("qqq", EdmNullExpression.Instance);
            Assert.IsFalse(e.IsBad(), "Expression not bad.");

            try
            {
                e.ReferencedLabeledExpression = new EdmLabeledExpression("qqq", EdmNullExpression.Instance);
                Assert.Fail("exception expected");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void EdmApplyExpression()
        {
            var arguments = new IEdmExpression[] { new EdmIntegerConstant(1) };
            var operation = new EdmFunction("NS", "function", new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true));
            var e = new EdmApplyExpression(operation, arguments);
            Assert.AreEqual(EdmExpressionKind.FunctionApplication, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual(operation, e.AppliedFunction, "e.AppliedFunction");
            Assert.AreEqual(arguments, e.Arguments, "e.AppliedFunction");
            Assert.IsFalse(e.IsBad(), "e good");

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmApplyExpression(null, arguments));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmApplyExpression(null, arguments.AsEnumerable()));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmApplyExpression(operation, null));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmApplyExpression(operation, (IEnumerable<IEdmExpression>)null));

            var ee = new MutableEdmApplyExpression();
            Assert.IsNull(ee.AppliedFunction, "ee.AppliedFunction");
            Assert.IsNull(ee.Arguments, "ee.Arguments");
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(2, ee.Errors().Count(), "Expression has no errors");
        }

        private sealed class MutableEdmApplyExpression : IEdmApplyExpression
        {
            public IEdmFunction AppliedFunction
            {
                get;
                set;
            }

            public IEnumerable<IEdmExpression> Arguments
            {
                get;
                set;
            }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.FunctionApplication; }
            }
        }

        [TestMethod]
        public void EdmEnumMemberExpression()
        {
            var et = new EdmEnumType("NS", "Spicy");
            var em = new EdmEnumMember(et, "Hot", new EdmEnumMemberValue(5));
            var e = new EdmEnumMemberExpression(em);
            Assert.AreEqual(EdmExpressionKind.EnumMember, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual("Hot", e.EnumMembers.Single().Name, "e.EnumMembers");
            Assert.IsFalse(e.IsBad(), "e good");

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmEnumMemberExpression(null));

            var ee = new MutableEdmEnumMemberExpression();
            Assert.IsNull(ee.EnumMembers, "e.EnumMembers");
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(1, ee.Errors().Count(), "Expression has errors");
        }

        private sealed class MutableEdmEnumMemberExpression : IEdmEnumMemberExpression
        {
            public IEnumerable<IEdmEnumMember> EnumMembers { get; set; }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.EnumMember; }
            }
        }

        [TestMethod]
        public void EdmNullExpressionTest()
        {
            var e = EdmNullExpression.Instance;
            Assert.AreEqual(EdmExpressionKind.Null, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual(EdmValueKind.Null, e.ValueKind, "e.ValueKind");
            Assert.IsFalse(e.IsBad(), "e good");
        }

        [TestMethod]
        public void EdmLabeledExpressionReferenceExpressionTest()
        {
            var label = new EdmLabeledExpression("Label", new EdmStringConstant("foo"));
            var e = new EdmLabeledExpressionReferenceExpression(label);
            Assert.AreEqual(EdmExpressionKind.LabeledExpressionReference, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual("Label", e.ReferencedLabeledExpression.Name, "e.ReferencedLabeledExpression");
            Assert.IsFalse(e.IsBad(), "e good");
            this.VerifyThrowsException(typeof(InvalidOperationException), () => e.ReferencedLabeledExpression = label);

            e = new EdmLabeledExpressionReferenceExpression();
            e.ReferencedLabeledExpression = label;
            Assert.AreEqual(EdmExpressionKind.LabeledExpressionReference, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual("Label", e.ReferencedLabeledExpression.Name, "e.ReferencedLabeledExpression");
            Assert.IsFalse(e.IsBad(), "e good");
            this.VerifyThrowsException(typeof(InvalidOperationException), () => e.ReferencedLabeledExpression = label);

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmLabeledExpressionReferenceExpression(null));

            var ee = new MutableEdmLabeledExpressionReferenceExpression();
            Assert.IsNull(ee.ReferencedLabeledExpression, "e.ReferencedLabeledExpression");
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(1, ee.Errors().Count(), "Expression has errors");
        }

        private sealed class MutableEdmLabeledExpressionReferenceExpression : IEdmLabeledExpressionReferenceExpression
        {
            public IEdmLabeledExpression ReferencedLabeledExpression
            {
                get;
                set;
            }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.LabeledExpressionReference; }
            }
        }

        [TestMethod]
        public void EdmIfExpression()
        {
            var e = new EdmIfExpression(new EdmStringConstant("if"), new EdmStringConstant("then"), new EdmStringConstant("else"));
            Assert.AreEqual(EdmExpressionKind.If, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual("if", ((IEdmStringValue)e.TestExpression).Value, "e.TestExpression");
            Assert.AreEqual("then", ((IEdmStringValue)e.TrueExpression).Value, "e.TrueExpression");
            Assert.AreEqual("else", ((IEdmStringValue)e.FalseExpression).Value, "e.FalseExpression");
            Assert.IsFalse(e.IsBad(), "e good");

            try
            {
                new EdmIfExpression(null, new EdmStringConstant("then"), new EdmStringConstant("else"));
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new EdmIfExpression(new EdmStringConstant("if"), null, new EdmStringConstant("else"));
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new EdmIfExpression(new EdmStringConstant("if"), new EdmStringConstant("then"), null);
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            var ee = new MutableIfExpression();
            Assert.IsNull(ee.TestExpression, "ee.TestExpression");
            Assert.IsNull(ee.TrueExpression, "ee.TrueExpression");
            Assert.IsNull(ee.FalseExpression, "ee.FalseExpression");
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(3, ee.Errors().Count(), "Expression has no errors");
        }

        private sealed class MutableIfExpression : IEdmIfExpression
        {
            public IEdmExpression TestExpression
            {
                get;
                set;
            }

            public IEdmExpression TrueExpression
            {
                get;
                set;
            }

            public IEdmExpression FalseExpression
            {
                get;
                set;
            }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.If; }
            }
        }

        [TestMethod]
        public void EdmIsTypeExpression()
        {
            var e = new EdmIsTypeExpression(new EdmStringConstant("qwerty"), EdmCoreModel.Instance.GetBoolean(false));
            Assert.AreEqual(EdmExpressionKind.IsType, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual("qwerty", ((IEdmStringValue)e.Operand).Value, "((IEdmStringValue)e.Operand).Value");
            Assert.AreEqual("Edm.Boolean", e.Type.FullName(), "e.Type.FullName()");
            Assert.IsFalse(e.IsBad(), "e good");

            try
            {
                new EdmIsTypeExpression(null, EdmCoreModel.Instance.GetBoolean(false));
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new EdmIsTypeExpression(new EdmStringConstant("qwerty"), null);
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            var ee = new MutableIsTypeExpression();
            Assert.IsNull(ee.Operand, "ee.Operand");
            Assert.IsNull(ee.Type, "ee.Type");
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(2, ee.Errors().Count(), "Expression has no errors");
        }

        private sealed class MutableIsTypeExpression : IEdmIsTypeExpression
        {
            public IEdmExpression Operand
            {
                get;
                set;
            }

            public IEdmTypeReference Type
            {
                get;
                set;
            }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.IsType; }
            }
        }

        [TestMethod]
        public void EdmPathExpression()
        {
            var e = new EdmPathExpression("x", "y");
            Assert.IsFalse(e.IsBad(), "e good");
            Assert.AreEqual(EdmExpressionKind.Path, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual(2, e.PathSegments.Count(), "e.Path.Count()");
            var s1 = e.PathSegments.First();
            Assert.AreEqual("x", s1, "s1");
            Assert.AreEqual("y", e.PathSegments.Last(), "e.Path.Last()");

            try
            {
                new EdmPathExpression((string[])null);
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            var ee = new MutablePathExpression();
            Assert.IsNull(ee.PathSegments, "ee.Path");
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(1, ee.Errors().Count(), "Expression has no errors");
        }

        private sealed class MutablePathExpression : IEdmPathExpression
        {
            public IEnumerable<string> PathSegments
            {
                get;
                set;
            }

            public string Path { get; set; }

            public EdmExpressionKind ExpressionKind
            {
                get { return EdmExpressionKind.Path; }
            }
        }

        [TestMethod]
        public void EdmPropertyConstructor()
        {
            var e = new EdmPropertyConstructor("n1", new EdmStringConstant("qwerty"));
            Assert.AreEqual("n1", e.Name, "e.Name");
            Assert.AreEqual("qwerty", ((IEdmStringValue)e.Value).Value, ((IEdmStringValue)e.Value).Value);
            Assert.IsFalse(e.IsBad(), "e good");

            try
            {
                new EdmPropertyConstructor(null, new EdmStringConstant("qwerty"));
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new EdmPropertyConstructor("n1", null);
                Assert.Fail("exception expected");
            }
            catch (ArgumentNullException)
            {
            }

            var ee = new MutablePropertyConstructor();
            Assert.IsNull(ee.Name, "ee.Name");
            Assert.IsNull(ee.Value, "ee.Expression");
            Assert.IsTrue(ee.IsBad(), "Expression is bad.");
            Assert.AreEqual(2, ee.Errors().Count(), "Expression has errors");
        }

        private sealed class MutablePropertyConstructor : IEdmPropertyConstructor
        {
            public string Name
            {
                get;
                set;
            }

            public IEdmExpression Value
            {
                get;
                set;
            }
        }

        [TestMethod]
        public void EdmRecordExpression()
        {
            var e = new EdmRecordExpression(EdmCoreModel.Instance.GetBoolean(true).AsStructured(),
                new EdmPropertyConstructor("p1", new EdmStringConstant("qwerty")),
                new EdmPropertyConstructor("p2", new EdmStringConstant("qwerty2")));
            Assert.AreEqual(EdmExpressionKind.Record, e.ExpressionKind, "e.ExpressionKind");
            Assert.AreEqual("Edm.Boolean", e.DeclaredType.FullName(), "e.DeclaredType");
            Assert.IsTrue(e.IsBad(), "e is bad because it has a bad declared type");
            Assert.AreEqual(1, e.Errors().Count(), "Expression has errors");

            e = new EdmRecordExpression();
            Assert.IsNull(e.DeclaredType, "e.DeclaredType");
            Assert.AreEqual(0, e.Properties.Count(), "e.Properties.Count()");
            Assert.IsFalse(e.IsBad(), "e is good");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");

            e = new EdmRecordExpression(new EdmEntityTypeReference(new EdmEntityType("", ""), false),
                new EdmPropertyConstructor("p1", new EdmStringConstant("qwerty")),
                new EdmPropertyConstructor("p2", new EdmStringConstant("qwerty2")));
            Assert.IsFalse(e.IsBad(), "e is good");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");

            e = new EdmRecordExpression((IEdmStructuredTypeReference)null);
            Assert.IsNull(e.DeclaredType, "e.DeclaredType");
            Assert.AreEqual(0, e.Properties.Count(), "e.Properties.Count()");
            Assert.IsFalse(e.IsBad(), "e is good");
            Assert.AreEqual(0, e.Errors().Count(), "Expression has no errors");

            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmPropertyConstructor(null, new EdmStringConstant("qwerty")));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => new EdmPropertyConstructor("p1", null));
        }
    }
}
