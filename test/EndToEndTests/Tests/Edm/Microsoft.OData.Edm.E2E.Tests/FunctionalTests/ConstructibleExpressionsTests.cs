//---------------------------------------------------------------------
// <copyright file="ConstructibleExpressionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ConstructibleExpressionsTests : EdmLibTestCaseBase
{
    [Fact]
    public void EdmBinaryConstant_ShouldCreateBinaryConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmBinaryConstant(new byte[] { 1, 2, 3 });

        Assert.Equal(EdmExpressionKind.BinaryConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal(3, e.Value[2]);

        e = new EdmBinaryConstant(EdmCoreModel.Instance.GetBinary(true, null, true), new byte[] { 3, 2, 1 });
        Assert.True(e.Type.IsNullable);
        Assert.True(e.Type.AsBinary().IsUnbounded);
        Assert.Equal(1, e.Value[2]);

        e = new EdmBinaryConstant(null, new byte[] { 3, 2, 1 });
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());

        Assert.Throws<ArgumentNullException>(() => new EdmBinaryConstant(null));
        Assert.Throws<ArgumentNullException>(() => new EdmBinaryConstant(EdmCoreModel.Instance.GetBinary(true), null));
    }

    [Fact]
    public void EdmBooleanConstant_ShouldCreateBooleanConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmBooleanConstant(true);
        Assert.Equal(EdmExpressionKind.BooleanConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.True(e.Value);

        e = new EdmBooleanConstant(EdmCoreModel.Instance.GetBinary(true, null, true), false);
        Assert.True(e.Type.IsNullable);
        Assert.True(e.Type.AsBinary().IsUnbounded);
        Assert.False(e.Value);

        e = new EdmBooleanConstant(null, true);
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmDurationConstant_ShouldCreateDurationConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmDurationConstant(new TimeSpan(1, 2, 3));
        Assert.Equal(EdmExpressionKind.DurationConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal(new TimeSpan(1, 2, 3), e.Value);

        e = new EdmDurationConstant(EdmCoreModel.Instance.GetDuration(true), new TimeSpan(3, 2, 1));
        Assert.True(e.Type.IsNullable);
        Assert.Equal(new TimeSpan(3, 2, 1), e.Value);

        e = new EdmDurationConstant(null, new TimeSpan(2, 4, 8));
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmDateTimeOffsetConstant_ShouldCreateDateTimeOffsetConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmDateTimeOffsetConstant(new DateTimeOffset(2011, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)));
        Assert.Equal(EdmExpressionKind.DateTimeOffsetConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal(new DateTimeOffset(2011, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)), e.Value);

        e = new EdmDateTimeOffsetConstant(EdmCoreModel.Instance.GetDateTimeOffset(true), new DateTimeOffset(2211, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)));
        Assert.True(e.Type.IsNullable);
        Assert.Equal(new DateTimeOffset(2211, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)), e.Value);

        e = new EdmDateTimeOffsetConstant(null, new DateTimeOffset(2011, 9, 8, 0, 0, 0, new TimeSpan(1, 2, 0)));
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmDateConstant_ShouldCreateDateConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmDateConstant(new Date(2014, 8, 8));
        Assert.Equal(EdmExpressionKind.DateConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal(new Date(2014, 8, 8), e.Value);

        e = new EdmDateConstant(EdmCoreModel.Instance.GetDate(true), new Date(2014, 8, 8));
        Assert.Equal(EdmPrimitiveTypeKind.Date, e.Type.AsPrimitive().PrimitiveKind());
        Assert.True(e.Type.IsNullable);
        Assert.Equal(new Date(2014, 8, 8), e.Value);

        e = new EdmDateConstant(null, new Date(2014, 8, 8));
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmDecimalConstant_ShouldCreateDecimalConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmDecimalConstant((decimal)11.22);
        Assert.Equal(EdmExpressionKind.DecimalConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal((decimal)11.22, e.Value);

        e = new EdmDecimalConstant(EdmCoreModel.Instance.GetDecimal(true), (decimal)33.22);
        Assert.True(e.Type.IsNullable);
        Assert.Equal((decimal)33.22, e.Value);

        e = new EdmDecimalConstant(null, (decimal)11.22);
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmFloatingConstant_ShouldCreateFloatingPointConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmFloatingConstant(11.22);
        Assert.Equal(EdmExpressionKind.FloatingConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal(11.22, e.Value);

        e = new EdmFloatingConstant(EdmCoreModel.Instance.GetDouble(true), 33.22);
        Assert.True(e.Type.IsNullable);
        Assert.Equal(33.22, e.Value);

        e = new EdmFloatingConstant(null, 11.22);
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmGuidConstant_ShouldCreateGuidConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmGuidConstant(new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
        Assert.Equal(EdmExpressionKind.GuidConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal(new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }), e.Value);

        e = new EdmGuidConstant(EdmCoreModel.Instance.GetGuid(true), new Guid(new byte[] { 100, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
        Assert.True(e.Type.IsNullable);
        Assert.Equal(new Guid(new byte[] { 100, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }), e.Value);

        e = new EdmGuidConstant(null, new Guid(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmIntegerConstant_ShouldCreateIntegerConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmIntegerConstant(2);
        Assert.Equal(EdmExpressionKind.IntegerConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal(2, e.Value);

        e = new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(true), 3);
        Assert.True(e.Type.IsNullable);
        Assert.Equal(3, e.Value);

        e = new EdmIntegerConstant(null, 2);
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmStringConstant_ShouldCreateStringConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmStringConstant("qqq");
        Assert.Equal(EdmExpressionKind.StringConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal("qqq", e.Value);

        e = new EdmStringConstant(EdmCoreModel.Instance.GetString(true), "aaa");
        Assert.True(e.Type.IsNullable);
        Assert.Equal("aaa", e.Value);

        e = new EdmStringConstant(null, "qqq");
        Assert.Null(e.Type);

        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());

        Assert.Throws<ArgumentNullException>(() => new EdmStringConstant(null));
        Assert.Throws<ArgumentNullException>(() => new EdmStringConstant(EdmCoreModel.Instance.GetString(true), null));
    }

    [Fact]
    public void EdmTimeOfDayConstant_ShouldCreateTimeOfDayConstantExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmTimeOfDayConstant(new TimeOfDay(12, 30, 50, 0));
        Assert.Equal(EdmExpressionKind.TimeOfDayConstant, e.ExpressionKind);
        Assert.Null(e.Type);
        Assert.Equal(new TimeOfDay(12, 30, 50, 0), e.Value);

        e = new EdmTimeOfDayConstant(EdmCoreModel.Instance.GetTimeOfDay(true), new TimeOfDay(1, 5, 10, 10));
        Assert.Equal(EdmPrimitiveTypeKind.TimeOfDay, e.Type.AsPrimitive().PrimitiveKind());
        Assert.Equal(new TimeOfDay(1, 5, 10, 10), e.Value);

        e = new EdmTimeOfDayConstant(null, new TimeOfDay(23, 50, 11, 999));
        Assert.Null(e.Type);
        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());
    }

    [Fact]
    public void EdmCastExpression_ShouldCreateCastExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmCastExpression(new EdmStringConstant("qwerty"), EdmCoreModel.Instance.GetBoolean(false));
        Assert.Equal(EdmExpressionKind.Cast, e.ExpressionKind);
        Assert.Equal("qwerty", ((IEdmStringValue)e.Operand).Value);
        Assert.Equal("Edm.Boolean", e.Type.FullName());
        Assert.False(e.IsBad());

        Assert.Throws<ArgumentNullException>(() => new EdmCastExpression(null, EdmCoreModel.Instance.GetBoolean(false)));
        Assert.Throws<ArgumentNullException>(() => new EdmCastExpression(new EdmStringConstant("qwerty"), null));

        var ee = new MutableCastExpression();
        Assert.True(ee.IsBad());
        Assert.Equal(2, ee.Errors().Count());
    }

    [Fact]
    public void EdmLabeledExpression_ShouldCreateLabeledExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmLabeledExpression("l1", new EdmStringConstant("qwerty"));
        Assert.Equal("l1", e.Name);
        Assert.Equal("qwerty", ((IEdmStringValue)e.Expression).Value);
        Assert.False(e.IsBad());

        Assert.Throws<ArgumentNullException>(() => new EdmLabeledExpression(null, new EdmStringConstant("qwerty")));
        Assert.Throws<ArgumentNullException>(() => new EdmLabeledExpression("l1", null));

        var ee = new MutableLabeledExpression();
        Assert.Null(ee.Name);
        Assert.Null(ee.Expression);
        Assert.True(ee.IsBad());
        Assert.Equal(2, ee.Errors().Count());
    }

    [Fact]
    public void EdmCollectionExpression_ShouldCreateCollectionExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmCollectionExpression(
            new EdmLabeledExpression("l1", new EdmStringConstant("qwerty")),
            new EdmLabeledExpression("l2", new EdmStringConstant("qwerty2")));
        Assert.False(e.IsBad());
        Assert.Equal(EdmExpressionKind.Collection, e.ExpressionKind);
        Assert.Equal(2, e.Elements.Count());
        Assert.Equal("l2", ((EdmLabeledExpression)e.Elements.ElementAt(1)).Name);
        var l1 = e.Elements.First();

        e = new EdmCollectionExpression();
        Assert.Null(e.DeclaredType);
        Assert.Empty(e.Elements);
        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());

        Assert.Throws<ArgumentNullException>(() => new EdmCollectionExpression((IEdmLabeledExpression[])null));
    }

    [Fact]
    public void EdmLabeledExpressionReferenceExpression_ShouldCreateLabeledExpressionReferenceAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmLabeledExpressionReferenceExpression(new EdmLabeledExpression("qq", EdmNullExpression.Instance));
        Assert.False(e.IsBad());
        Assert.Equal(EdmExpressionKind.LabeledExpressionReference, e.ExpressionKind);
        Assert.Equal("qq", ((EdmLabeledExpression)e.ReferencedLabeledExpression).Name);

        Assert.Throws<InvalidOperationException>(() => e.ReferencedLabeledExpression = new EdmLabeledExpression("qqq", EdmNullExpression.Instance));

        e = new EdmLabeledExpressionReferenceExpression();
        Assert.True(e.IsBad());
        Assert.Single(e.Errors());

        Assert.Throws<ArgumentNullException>(() => e.ReferencedLabeledExpression = null);

        e.ReferencedLabeledExpression = new EdmLabeledExpression("qqq", EdmNullExpression.Instance);
        Assert.False(e.IsBad());

        Assert.Throws<InvalidOperationException>(() => e.ReferencedLabeledExpression = new EdmLabeledExpression("qqq", EdmNullExpression.Instance));
    }

    [Fact]
    public void EdmLabeledExpression_ShouldCreateLabeledExpressionReferenceAndValidateProperties()
    {
        // Arrange & Act & Assert
        var label = new EdmLabeledExpression("Label", new EdmStringConstant("foo"));
        var e = new EdmLabeledExpressionReferenceExpression(label);
        Assert.Equal(EdmExpressionKind.LabeledExpressionReference, e.ExpressionKind);
        Assert.Equal("Label", e.ReferencedLabeledExpression.Name);
        Assert.False(e.IsBad());
        Assert.Throws<InvalidOperationException>(() => e.ReferencedLabeledExpression = label);

        e = new EdmLabeledExpressionReferenceExpression();
        e.ReferencedLabeledExpression = label;
        Assert.Equal(EdmExpressionKind.LabeledExpressionReference, e.ExpressionKind);
        Assert.Equal("Label", e.ReferencedLabeledExpression.Name);
        Assert.False(e.IsBad());
        Assert.Throws<InvalidOperationException>(() => e.ReferencedLabeledExpression = label);

        Assert.Throws<ArgumentNullException>(() => new EdmLabeledExpressionReferenceExpression(null));

        var ee = new MutableEdmLabeledExpressionReferenceExpression();
        Assert.Null(ee.ReferencedLabeledExpression);
        Assert.True(ee.IsBad());
        Assert.Single(ee.Errors());
    }

    [Fact]
    public void EdmApplyExpression_ShouldCreateApplyExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var arguments = new IEdmExpression[] { new EdmIntegerConstant(1) };
        var operation = new EdmFunction("NS", "function", new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true));
        var e = new EdmApplyExpression(operation, arguments);
        Assert.Equal(EdmExpressionKind.FunctionApplication, e.ExpressionKind);
        Assert.Equal(operation, e.AppliedFunction);
        Assert.Equal(arguments, e.Arguments);
        Assert.False(e.IsBad());

        Assert.Throws<ArgumentNullException>(() => new EdmApplyExpression(null, arguments));
        Assert.Throws<ArgumentNullException>(() => new EdmApplyExpression(null, arguments));
        Assert.Throws<ArgumentNullException>(() => new EdmApplyExpression(operation, null));
        Assert.Throws<ArgumentNullException>(() => new EdmApplyExpression(operation, (IEnumerable<IEdmExpression>)null));

        var ee = new MutableEdmApplyExpression();
        Assert.Null(ee.AppliedFunction);
        Assert.Null(ee.Arguments);
        Assert.True(ee.IsBad());
        Assert.Equal(2, ee.Errors().Count());
    }

    [Fact]
    public void EdmEnumMemberExpression_ShouldCreateEnumMemberExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var et = new EdmEnumType("NS", "Spicy");
        var em = new EdmEnumMember(et, "Hot", new EdmEnumMemberValue(5));
        var e = new EdmEnumMemberExpression(em);
        Assert.Equal(EdmExpressionKind.EnumMember, e.ExpressionKind);
        Assert.Equal("Hot", e.EnumMembers.Single().Name);
        Assert.False(e.IsBad());

        Assert.Throws<ArgumentNullException>(() => new EdmEnumMemberExpression(null));

        var ee = new MutableEdmEnumMemberExpression();
        Assert.Null(ee.EnumMembers);
        Assert.True(ee.IsBad());
        Assert.Single(ee.Errors());
    }

    [Fact]
    public void EdmNullExpression_ShouldCreateNullExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = EdmNullExpression.Instance;
        Assert.Equal(EdmExpressionKind.Null, e.ExpressionKind);
        Assert.Equal(EdmValueKind.Null, e.ValueKind);
        Assert.False(e.IsBad());
    }

    [Fact]
    public void EdmIfExpression_ShouldCreateIfExpressionAndValidateProperties()
    {
        var e = new EdmIfExpression(new EdmStringConstant("if"), new EdmStringConstant("then"), new EdmStringConstant("else"));
        Assert.Equal(EdmExpressionKind.If, e.ExpressionKind);
        Assert.Equal("if", ((IEdmStringValue)e.TestExpression).Value);
        Assert.Equal("then", ((IEdmStringValue)e.TrueExpression).Value);
        Assert.Equal("else", ((IEdmStringValue)e.FalseExpression).Value);
        Assert.False(e.IsBad());

        Assert.Throws<ArgumentNullException>(() => new EdmIfExpression(null, new EdmStringConstant("then"), new EdmStringConstant("else")));
        Assert.Throws<ArgumentNullException>(() => new EdmIfExpression(new EdmStringConstant("if"), null, new EdmStringConstant("else")));
        Assert.Throws<ArgumentNullException>(() => new EdmIfExpression(new EdmStringConstant("if"), new EdmStringConstant("then"), null));

        var ee = new MutableIfExpression();
        Assert.Null(ee.TestExpression);
        Assert.Null(ee.TrueExpression);
        Assert.Null(ee.FalseExpression);
        Assert.True(ee.IsBad());
        Assert.Equal(3, ee.Errors().Count());
    }

    [Fact]
    public void EdmIsOfExpression_ShouldCreateIsOfExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmIsOfExpression(new EdmStringConstant("qwerty"), EdmCoreModel.Instance.GetBoolean(false));
        Assert.Equal(EdmExpressionKind.IsOf, e.ExpressionKind);
        Assert.Equal("qwerty", ((IEdmStringValue)e.Operand).Value);
        Assert.Equal("Edm.Boolean", e.Type.FullName());
        Assert.False(e.IsBad());

        Assert.Throws<ArgumentNullException>(() => new EdmIsOfExpression(null, EdmCoreModel.Instance.GetBoolean(false)));
        Assert.Throws<ArgumentNullException>(() => new EdmIsOfExpression(new EdmStringConstant("qwerty"), null));

        var ee = new MutableIsTypeExpression();
        Assert.Null(ee.Operand);
        Assert.Null(ee.Type);
        Assert.True(ee.IsBad());
        Assert.Equal(2, ee.Errors().Count());
    }

    [Fact]
    public void EdmPathExpression_ShouldCreatePathExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmPathExpression("x", "y");
        Assert.False(e.IsBad());
        Assert.Equal(EdmExpressionKind.Path, e.ExpressionKind);
        Assert.Equal(2, e.PathSegments.Count());
        var s1 = e.PathSegments.First();
        Assert.Equal("x", s1);
        Assert.Equal("y", e.PathSegments.Last());

        Assert.Throws<ArgumentNullException>(() => new EdmPathExpression((string[])null));

        var ee = new MutablePathExpression();
        Assert.Null(ee.PathSegments);
        Assert.True(ee.IsBad());
        Assert.Single(ee.Errors());
    }

    [Fact]
    public void EdmPropertyConstructor_ShouldCreatePropertyConstructorAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmPropertyConstructor("n1", new EdmStringConstant("qwerty"));
        Assert.Equal("n1", e.Name);
        Assert.Equal("qwerty", ((IEdmStringValue)e.Value).Value);
        Assert.False(e.IsBad());

        Assert.Throws<ArgumentNullException>(() => new EdmPropertyConstructor(null, new EdmStringConstant("qwerty")));
        Assert.Throws<ArgumentNullException>(() => new EdmPropertyConstructor("n1", null));

        var ee = new MutablePropertyConstructor();
        Assert.Null(ee.Name);
        Assert.Null(ee.Value);
        Assert.True(ee.IsBad());
        Assert.Equal(2, ee.Errors().Count());
    }

    [Fact]
    public void EdmRecordExpression_ShouldCreateRecordExpressionAndValidateProperties()
    {
        // Arrange & Act & Assert
        var e = new EdmRecordExpression(EdmCoreModel.Instance.GetBoolean(true).AsStructured(),
            new EdmPropertyConstructor("p1", new EdmStringConstant("qwerty")),
            new EdmPropertyConstructor("p2", new EdmStringConstant("qwerty2")));

        Assert.Equal(EdmExpressionKind.Record, e.ExpressionKind);
        Assert.Equal("Edm.Boolean", e.DeclaredType.FullName());
        Assert.True(e.IsBad());
        Assert.Single(e.Errors());

        e = new EdmRecordExpression();
        Assert.Null(e.DeclaredType);
        Assert.Empty(e.Properties);
        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());

        e = new EdmRecordExpression(new EdmEntityTypeReference(new EdmEntityType("", ""), false),
            new EdmPropertyConstructor("p1", new EdmStringConstant("qwerty")),
            new EdmPropertyConstructor("p2", new EdmStringConstant("qwerty2")));
        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());

        e = new EdmRecordExpression((IEdmStructuredTypeReference)null);
        Assert.Null(e.DeclaredType);
        Assert.Empty(e.Properties);
        Assert.False(e.IsBad());
        Assert.Empty(e.Errors());

        Assert.Throws<ArgumentNullException>(() => new EdmPropertyConstructor(null, new EdmStringConstant("qwerty")));
        Assert.Throws<ArgumentNullException>(() => new EdmPropertyConstructor("p1", null));
    }

    #region Private

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

    private sealed class MutableEdmEnumMemberExpression : IEdmEnumMemberExpression
    {
        public IEnumerable<IEdmEnumMember> EnumMembers { get; set; }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EnumMember; }
        }
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

    private sealed class MutableIsTypeExpression : IEdmIsOfExpression
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
            get { return EdmExpressionKind.IsOf; }
        }
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

    #endregion
}
