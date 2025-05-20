//---------------------------------------------------------------------
// <copyright file="ConstructibleModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.E2E.Tests.StubEdm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class ConstructibleModelTests : EdmLibTestCaseBase
{
    EdmCoreModel m_coreModel;

    public ConstructibleModelTests()
    {
        this.m_coreModel = EdmCoreModel.Instance;
    }

    [Fact]
    public void ConstructEntityWithVariousPrimitiveTypeProperties_ShouldValidateProperties()
    {
        // Arrange
        EdmModel model = new EdmModel();
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmStructuralProperty p1 = t1.AddStructuralProperty("P1", m_coreModel.GetBoolean(false));
        EdmStructuralProperty p2 = t1.AddStructuralProperty("P2", m_coreModel.GetDecimal(1, 1, false));
        EdmStructuralProperty p3 = t1.AddStructuralProperty("P3", m_coreModel.GetTemporal(EdmPrimitiveTypeKind.Duration, 1, false));
        EdmStructuralProperty p4 = t1.AddStructuralProperty("P4", m_coreModel.GetBinary(false, 4, false));
        EdmStructuralProperty p5 = t1.AddStructuralProperty("P5", m_coreModel.GetBinary(false));
        EdmStructuralProperty p6 = t1.AddStructuralProperty("P6", m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Stream, true));
        EdmStructuralProperty p7 = t1.AddStructuralProperty("P7", m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Stream, false));

        // Act
        IEdmStructuralProperty q1 = (IEdmStructuralProperty)t1.FindProperty("P1");
        IEdmStructuralProperty q2 = (IEdmStructuralProperty)t1.FindProperty("P2");
        IEdmStructuralProperty q3 = (IEdmStructuralProperty)t1.FindProperty("P3");
        IEdmStructuralProperty q4 = (IEdmStructuralProperty)t1.FindProperty("P4");
        IEdmStructuralProperty q5 = (IEdmStructuralProperty)t1.FindProperty("P5");
        IEdmStructuralProperty q6 = (IEdmStructuralProperty)t1.FindProperty("P6");
        IEdmStructuralProperty q7 = (IEdmStructuralProperty)t1.FindProperty("P7");

        // Assert
        Assert.Equal(p1, q1);
        Assert.Equal(p2, q2);
        Assert.Equal(p3, q3);
        Assert.Equal(p4, q4);
        Assert.Equal(p5, q5);
        Assert.Equal(p6, q6);
        Assert.Equal(p7, q7);

        Assert.Equal(EdmPrimitiveTypeKind.Boolean, q1.Type.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Decimal, q2.Type.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Duration, q3.Type.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Binary, q4.Type.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Binary, q5.Type.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Stream, q6.Type.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Stream, q7.Type.PrimitiveKind());

        Assert.False(q1.Type.IsNullable);
        Assert.False(q2.Type.IsNullable);
        Assert.False(q3.Type.IsNullable);
        Assert.False(q4.Type.IsNullable);
        Assert.False(q5.Type.IsNullable);
        Assert.True(q6.Type.IsNullable);
        Assert.False(q7.Type.IsNullable);

        Assert.Equal(1, q2.Type.AsDecimal().Scale);
        Assert.Equal(1, q2.Type.AsDecimal().Precision);
        Assert.Equal(1, q3.Type.AsTemporal().Precision);
        Assert.False(q4.Type.AsBinary().IsUnbounded);
        Assert.Equal(4, q4.Type.AsBinary().MaxLength);
    }

    [Fact]
    public void ConstructEntityTypesWithInheritance_ShouldValidateHierarchy()
    {
        // Arrange
        var model = new ModelWithRemovableElements<EdmModel>(new EdmModel());
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
        EdmEntityType t3 = new EdmEntityType("Bunk", "T3", t2);
        EdmEntityType t4 = new EdmEntityType("Bunk", "T4", t3);
        EdmEntityType t5 = new EdmEntityType("Bunk", "T5");

        model.WrappedModel.AddElement(t1);
        model.WrappedModel.AddElements(new EdmEntityType[] { t2, t3, t4, t5 });

        // Act & Assert
        Assert.Equal(5, model.SchemaElements.Count());
        Assert.Equal(model.SchemaElements.First(), t1);
        Assert.Equal(model.FindType("Bunk.T2"), t2);
        Assert.Equal(model.SchemaElements.Last(), t5);
        Assert.Equal(model.FindType("Bunk.T5"), t5);

        // Act
        model.RemoveElement(t5);

        // Assert
        Assert.Equal(4, model.SchemaElements.Count());
        Assert.Equal(model.SchemaElements.First(), t1);
        Assert.Equal(model.FindType("Bunk.T2"), t2);
        Assert.Equal(model.SchemaElements.Last(), t4);
        Assert.Null(model.FindType("Bunk.T5"));

        // Arrange
        EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", EdmPrimitiveTypeKind.String, false);
        EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", EdmPrimitiveTypeKind.Int32, false);
        EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", m_coreModel.GetInt32(false));

        EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetInt32(false));

        EdmStructuralProperty f31 = t3.AddStructuralProperty("F31", m_coreModel.GetInt32(false));
        EdmStructuralProperty f32 = t3.AddStructuralProperty("F32", m_coreModel.GetInt32(false));

        t1.AddKeys(f12);
        t2.AddKeys(f32);

        // Act
        t2 = new EdmEntityType("Bunk", "T2");
        model.WrappedModel.AddElement(t2);

        // Assert
        Assert.Equal(2, t3.DeclaredProperties.Count());
        Assert.Empty(t4.DeclaredProperties);
        Assert.Equal(3, t4.Properties().Count());
        Assert.Null(t4.FindProperty("F11"));
        Assert.Equal(t4.FindProperty("F21"), f21);
        Assert.Equal(t4.Key().Single(), f32);
        Assert.Equal(t4.FindProperty("F32"), f32);

        // Arrange
        model.SetAnnotationValue(f11, "Grumble", "Stumble", new EdmStringConstant(m_coreModel.GetString(false), "Rumble"));
        model.SetAnnotationValue(f11, "Grumble", "Tumble", new EdmStringConstant(m_coreModel.GetString(false), "Bumble"));
        model.SetAnnotationValue<Boxed<int>>(f11, new Boxed<int>(66));
        model.SetAnnotationValue<Boxed<string>>(f11, new Boxed<string>("Goop"));

        // Assert
        Assert.Equal(4, model.DirectValueAnnotations(f11).Count());
        Assert.Equal("Rumble", ((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Stumble")).Value);
        Assert.Equal("Bumble", ((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Tumble")).Value);
        Assert.Equal(66, model.GetAnnotationValue<Boxed<int>>(f11).Value);
        Assert.Equal("Goop", model.GetAnnotationValue<Boxed<string>>(f11).Value);

        // Act
        model.SetAnnotationValue(f11, "Grumble", "Tumble", new EdmStringConstant(m_coreModel.GetString(false), "Fumble"));

        // Assert
        Assert.Equal("Fumble", ((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Tumble")).Value);

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => model.GetAnnotationValue<Boxed<int>>(f11, "Grumble", "Tumble"));

        // Assert
        Assert.True(exception.Message.Contains("Boxed") && exception.Message.Contains("String"));

        // Act
        model.SetAnnotationValue<Boxed<string>>(f11, null);

        // Assert
        Assert.Equal(3, model.DirectValueAnnotations(f11).Count());
        Assert.Equal("Rumble", ((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Stumble")).Value);
        Assert.Equal("Fumble", ((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Tumble")).Value);
        Assert.Equal(66, model.GetAnnotationValue<Boxed<int>>(f11).Value);
        Assert.Null(model.GetAnnotationValue<Boxed<string>>(f11));
        Assert.Null(model.GetAnnotationValue<Boxed<bool>>(f11));

        // Act
        model.SetAnnotationValue<Boxed<int>>(f11, null);
        model.SetAnnotationValue(f11, "Grumble", "Stumble", null);
        model.SetAnnotationValue(f11, "Grumble", "Tumble", null);

        // Assert
        Assert.Empty(model.DirectValueAnnotations(f11));
        Assert.Null(model.GetAnnotationValue(f11, "Grumble", "Stumble"));
        Assert.Null(model.GetAnnotationValue(f11, "Grumble", "Tumble"));
        Assert.Null(model.GetAnnotationValue<Boxed<int>>(f11));
        Assert.Null(model.GetAnnotationValue<Boxed<string>>(f11));
        Assert.Null(model.GetAnnotationValue<Boxed<bool>>(f11));
    }

    [Fact]
    public void ConstructCyclicStructuredTypes_ShouldDetectAndReportCycles()
    {
        EdmModel model = new EdmModel();
        StubEdmEntityType t1 = new StubEdmEntityType("Bunk", "T1");
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2", t1);
        t1.BaseType = t2;
        StubEdmComplexType c1 = new StubEdmComplexType("Bunk", "T1");
        EdmComplexType c2 = new EdmComplexType("Bunk", "T2", c1, false);
        c1.BaseType = c2;

        model.AddElements(new IEdmSchemaElement[] { t1, t2 });
        model.AddElements(new IEdmSchemaElement[] { c1, c2 });

        Assert.True(t1.BaseEntityType().IsBad());
        Assert.Equal(EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, t1.BaseEntityType().Errors().Single().ErrorCode);
        Assert.True(t2.BaseEntityType().IsBad());
        Assert.Equal(EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, t2.BaseEntityType().Errors().Single().ErrorCode);

        Assert.True(c1.BaseComplexType().IsBad());
        Assert.Equal(EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, c1.BaseComplexType().Errors().Single().ErrorCode);
        Assert.True(c2.BaseComplexType().IsBad());
        Assert.Equal(EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, c2.BaseComplexType().Errors().Single().ErrorCode);

        IEnumerable<string> csdls = this.GetSerializerResult(model, out IEnumerable<EdmError> serializingErrors);
        Assert.Empty(csdls);
        Assert.True(serializingErrors.Count() > 0);
    }

    [Fact]
    public void AddPropertyToEntityType_ShouldHandleDuplicateAndInvalidProperties()
    {
        EdmModel model = new EdmModel();
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2");

        model.AddElement(t1);
        model.AddElement(t1);

        EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", m_coreModel.GetString(false));
        EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetInt32(false));

        // This adds a dup property, but it's Ok - model is semantically invalid, however structurally it's fine.
        t1.AddProperty(f11);
        Assert.Throws<InvalidOperationException>(() => t1.AddProperty(f21));
    }

    [Fact]
    public void ConstructComplexTypesWithInheritance_ShouldValidatePropertiesAndHierarchy()
    {
        var model = new ModelWithRemovableElements<EdmModel>(new EdmModel());
        EdmComplexType t1 = new EdmComplexType("Bunk", "T1");
        EdmComplexType t2 = new EdmComplexType("Bunk", "T2");
        EdmComplexType t3 = new EdmComplexType("Bunk", "T3", t1, false);
        EdmComplexType t4 = new EdmComplexType("Bunk", "T4", t3, false);
        EdmComplexType t5 = new EdmComplexType("Bunk", "T5");

        model.WrappedModel.AddElement(t1);
        model.WrappedModel.AddElements(new EdmComplexType[] { t2, t3, t4, t5 });

        Assert.Equal(5, model.SchemaElements.Count());
        Assert.Equal(model.SchemaElements.First(), t1);
        Assert.Equal(model.FindType("Bunk.T2"), t2);
        Assert.Equal(model.SchemaElements.Last(), t5);
        Assert.Equal(model.FindType("Bunk.T5"), t5);

        model.RemoveElement(t5);

        Assert.Equal(4, model.SchemaElements.Count());
        Assert.Equal(model.SchemaElements.First(), t1);
        Assert.Equal(model.FindType("Bunk.T2"), t2);
        Assert.Equal(model.SchemaElements.Last(), t4);
        Assert.Null(model.FindType("Bunk.T5"));

        EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", m_coreModel.GetString(false));

        EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", m_coreModel.GetInt32(false));
        EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", m_coreModel.GetInt32(false));

        EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetInt32(false));

        EdmStructuralProperty f31 = t3.AddStructuralProperty("F31", m_coreModel.GetInt32(false));
        EdmStructuralProperty f32 = t3.AddStructuralProperty("F32", m_coreModel.GetInt32(false));

        Assert.Empty(t4.DeclaredProperties);
        Assert.True(t4.Properties().Count() == t3.Properties().Count());
        Assert.Equal(t4.FindProperty("F11"), f11);
        Assert.Null(t4.FindProperty("F21"));

        t3 = new EdmComplexType("Bunk", "T3", t2, false);
        f31 = t3.AddStructuralProperty("F31", m_coreModel.GetString(false));
        model.WrappedModel.AddElement(t3);

        Assert.True(t4.DeclaredProperties.Count() == 0);
        Assert.True(t4.Properties().Count() == 5);
        Assert.True(t3.Properties().Count() == 2);
        Assert.Equal(t4.FindProperty("F11"), f11);
        Assert.Null(t4.FindProperty("F21"));
        Assert.Null(t3.FindProperty("F11"));
        Assert.Equal(t3.FindProperty("F21"), f21);

        t2 = new EdmComplexType("Bunk", "T2");
        model.WrappedModel.AddElement(t2);

        Assert.True(t3.Properties().Count() == 2);
        Assert.True(t2.Properties().Count() == 0);
        Assert.True(t4.Properties().Count() == 5);
        Assert.Equal(t4.FindProperty("F32"), f32);
        Assert.NotEqual(t4.FindProperty("F31"), f31);
        Assert.Null(t4.FindProperty("F21"));

        EdmStructuralProperty f41 = t4.AddStructuralProperty("F41", m_coreModel.GetInt32(false));
        Assert.True(t4.DeclaredProperties.Count() == 1);
        Assert.True(t4.Properties().Count() == 6);
        Assert.Equal(t4.FindProperty("F41"), f41);
    }

    [Fact]
    public void ConstructOneToOneNavigationProperties_ShouldValidateNavigationRelationships()
    {
        EdmModel model = new EdmModel();
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
        model.AddElement(t1);
        model.AddElement(t2);

        EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", m_coreModel.GetString(false));
        EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", m_coreModel.GetInt32(false));
        EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", m_coreModel.GetInt32(false));

        EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetString(false));
        EdmStructuralProperty f22 = t2.AddStructuralProperty("F22", m_coreModel.GetInt32(false));
        EdmStructuralProperty f23 = t2.AddStructuralProperty("F23", m_coreModel.GetInt32(false));

        t1.AddKeys(f11, f12);
        t2.AddKeys(f23);

        EdmNavigationProperty p101 = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "P101", Target = t2, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f13 }, PrincipalProperties = t2.Key() },
            new EdmNavigationPropertyInfo() { Name = "P201", Target = t1, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
        t1.AddProperty(p101);
        t2.AddProperty(p101.Partner);
        EdmNavigationProperty p102 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P102", Target = t2, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
            new EdmNavigationPropertyInfo() { Name = "P202", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f21, f22 }, PrincipalProperties = t1.Key() });
        EdmNavigationProperty p103 = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "P103", Target = t2, TargetMultiplicity = EdmMultiplicity.One, OnDelete = EdmOnDeleteAction.Cascade },
            new EdmNavigationPropertyInfo() { Name = "P203", Target = t1, TargetMultiplicity = EdmMultiplicity.One });
        t1.AddProperty(p103);
        t2.AddProperty(p103.Partner);
        EdmNavigationProperty p104 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P104", Target = t2, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "P204", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

        Assert.True(t1.Properties().Count() == 7);
        Assert.Equal(t1.FindProperty("P103"), p103);

        Assert.True(p101.PropertyKind == EdmPropertyKind.Navigation);
        Assert.True(p102.OnDelete == EdmOnDeleteAction.None);
        Assert.True(p103.OnDelete == EdmOnDeleteAction.Cascade);

        var p202 = p102.Partner;
        var p201 = p101.Partner;
        var p203 = p103.Partner;
        var p204 = p104.Partner;
        Assert.Equal(p201.Partner, p101);
        Assert.Equal(p202.Partner, p102);
        Assert.Equal(p203.Partner, p103);
        Assert.Equal("P104", p204.Partner.Name);

        Assert.Equal(((IEdmNavigationProperty)p101).ToEntityType(), t2);
        Assert.Equal(((IEdmNavigationProperty)p102).ToEntityType(), t2);
        Assert.Equal(((IEdmNavigationProperty)p103).ToEntityType(), t2);
        Assert.Equal(((IEdmNavigationProperty)p104).ToEntityType(), t2);
        Assert.False(((IEdmNavigationProperty)p104).ContainsTarget);
        Assert.Equal(((IEdmNavigationProperty)p201).ToEntityType(), t1);
        Assert.True(((IEdmNavigationProperty)p201).ContainsTarget);
        Assert.Equal(((IEdmNavigationProperty)p202).ToEntityType(), t1);
        Assert.Equal(((IEdmNavigationProperty)p203).ToEntityType(), t1);
        Assert.Equal(((IEdmNavigationProperty)p204).ToEntityType(), t1);
    }

    [Fact]
    public void ConstructManyToManyNavigationProperties_ShouldValidateNavigationRelationships()
    {
        EdmModel model = new EdmModel();
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
        model.AddElement(t1);
        model.AddElement(t2);

        EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", m_coreModel.GetString(false));
        EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", m_coreModel.GetInt32(false));
        EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", m_coreModel.GetInt32(false));

        EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetString(false));
        EdmStructuralProperty f22 = t2.AddStructuralProperty("F22", m_coreModel.GetInt32(false));
        EdmStructuralProperty f23 = t2.AddStructuralProperty("F23", m_coreModel.GetInt32(false));

        t1.AddKeys(f11, f12);
        t2.AddKeys(f23);

        EdmNavigationProperty p101 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P101", Target = t2, TargetMultiplicity = EdmMultiplicity.Many, DependentProperties = new[] { f13 }, PrincipalProperties = t2.Key() },
            new EdmNavigationPropertyInfo() { Name = "P201", TargetMultiplicity = EdmMultiplicity.Many });
        EdmNavigationProperty p102 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P102", Target = t2, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "P202", TargetMultiplicity = EdmMultiplicity.Many, DependentProperties = new[] { f21, f22 }, PrincipalProperties = t1.Key() });
        EdmNavigationProperty p103 = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "P103", Target = t2, TargetMultiplicity = EdmMultiplicity.Many, OnDelete = EdmOnDeleteAction.Cascade },
            new EdmNavigationPropertyInfo() { Name = "P203", Target = t1, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
        t2.AddProperty(p101.Partner);
        t1.AddProperty(p103);
        t2.AddProperty(p103.Partner);

        EdmNavigationProperty p104 = t1.AddUnidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P104", Target = t2, TargetMultiplicity = EdmMultiplicity.Many });
        EdmNavigationProperty p104_partner = t2.AddUnidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P104Partner", Target = t1, TargetMultiplicity = EdmMultiplicity.Many });
        Assert.Null(p104.Partner);
        Assert.Null(p104_partner.Partner);

        Assert.True(t1.Properties().Count() == 7);
        Assert.Equal(t1.FindProperty("P103"), p103);

        Assert.True(p101.PropertyKind == EdmPropertyKind.Navigation);
        Assert.True(p102.OnDelete == EdmOnDeleteAction.None);
        Assert.True(p103.OnDelete == EdmOnDeleteAction.Cascade);

        var p201 = p101.Partner;
        var p202 = p102.Partner;
        var p203 = p103.Partner;
        var p204 = p104_partner;

        Assert.Equal(p201.Partner, p101);
        Assert.Equal(p202.Partner, p102);
        Assert.Equal(p203.Partner, p103);
        Assert.Equal(p204.Name, p104.Name + "Partner");

        Assert.Equal(((IEdmNavigationProperty)p101).ToEntityType(), t2);
        Assert.Equal(((IEdmNavigationProperty)p102).ToEntityType(), t2);
        Assert.Equal(((IEdmNavigationProperty)p103).ToEntityType(), t2);
        Assert.Equal(((IEdmNavigationProperty)p104).ToEntityType(), t2);
        Assert.Equal(((IEdmNavigationProperty)p201).ToEntityType(), t1);
        Assert.Equal(((IEdmNavigationProperty)p202).ToEntityType(), t1);
        Assert.Equal(((IEdmNavigationProperty)p203).ToEntityType(), t1);
        Assert.Equal(((IEdmNavigationProperty)p204).ToEntityType(), t1);

        p202 = p103.Partner;

        Assert.Equal(p103.Partner, p202);
        Assert.Equal(p203.Partner.Name, p103.Name);
        Assert.Equal("P202", p102.Partner.Name);
    }

    [Fact]
    public void AddNavigationPropertyToInvalidTypes_ShouldThrowInvalidOperationException()
    {
        EdmModel model = new EdmModel();
        EdmEntityType t3 = new EdmEntityType("Bunk", "T3");
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1", t3);
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
        model.AddElement(t1);
        model.AddElement(t2);
        model.AddElement(t3);

        EdmNavigationProperty p101 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P101", Target = t2, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "P201", TargetMultiplicity = EdmMultiplicity.Many });
        EdmNavigationProperty p102 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P102", Target = t2, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "P202", TargetMultiplicity = EdmMultiplicity.Many });
        EdmNavigationProperty p103 = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            new EdmNavigationPropertyInfo() { Name = "P103", Target = t2, TargetMultiplicity = EdmMultiplicity.Many, OnDelete = EdmOnDeleteAction.Cascade },
            new EdmNavigationPropertyInfo() { Name = "P203", Target = t1, TargetMultiplicity = EdmMultiplicity.Many });
        EdmNavigationProperty p104 = t1.AddUnidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P104", Target = t2, TargetMultiplicity = EdmMultiplicity.Many });
        t1.AddProperty(p103);
        t2.AddProperty(p103.Partner);

        Assert.Throws<InvalidOperationException>(() => t3.AddProperty(p103));
        Assert.Throws<InvalidOperationException>(() => t3.AddProperty(p103.Partner));
        Assert.Throws<InvalidOperationException>(() => t3.AddProperty(p101));
        Assert.Throws<InvalidOperationException>(() => t3.AddProperty(p101.Partner));
        Assert.Throws<InvalidOperationException>(() => t3.AddProperty(p102));
        Assert.Throws<InvalidOperationException>(() => t3.AddProperty(p102.Partner));
    }

    [Fact]
    public void ConstructEntitySets_ShouldValidateEntitySetConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
        model.AddElement(t1);
        model.AddElement(t2);

        EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", m_coreModel.GetString(false));
        EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", m_coreModel.GetInt32(false));
        EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", m_coreModel.GetInt32(false));

        EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetString(false));
        EdmStructuralProperty f22 = t2.AddStructuralProperty("F22", m_coreModel.GetInt32(false));
        EdmStructuralProperty f23 = t2.AddStructuralProperty("F23", m_coreModel.GetInt32(false));

        t1.AddKeys(f11, f12);
        t2.AddKeys(f23);

        EdmNavigationProperty p101 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P101", Target = t2, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f13 }, PrincipalProperties = t2.Key() },
            new EdmNavigationPropertyInfo() { Name = "P201", TargetMultiplicity = EdmMultiplicity.One });
        EdmNavigationProperty p102 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P102", Target = t2, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "P202", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f21, f22 }, PrincipalProperties = t1.Key() });
        EdmNavigationProperty p103 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P103", Target = t2, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "P203", TargetMultiplicity = EdmMultiplicity.One });

        IEdmNavigationProperty p201 = p101.Partner;
        IEdmNavigationProperty p203 = p103.Partner;

        EdmEntityContainer c1 = new EdmEntityContainer("Bunk", "Gunk");
        model.AddElement(c1);
        EdmEntitySet E1 = c1.AddEntitySet("E1", t1);
        EdmEntitySet E2 = new EdmEntitySet(c1, "E2", t2);
        c1.AddElement(E2);

        E1.AddNavigationTarget(p102, E2);
        E1.AddNavigationTarget(p103, E2);

        E2.AddNavigationTarget(p201, E1);
        E2.AddNavigationTarget(p203, E1);

        Assert.True(E1.EntityType == t1);
        Assert.True(E2.EntityType == t2);
    }

    [Fact]
    public void ConstructSingletons_ShouldValidateSingletonConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
        model.AddElement(t1);
        model.AddElement(t2);

        EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", m_coreModel.GetString(false));
        EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", m_coreModel.GetInt32(false));
        EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", m_coreModel.GetInt32(false));

        EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetString(false));
        EdmStructuralProperty f22 = t2.AddStructuralProperty("F22", m_coreModel.GetInt32(false));
        EdmStructuralProperty f23 = t2.AddStructuralProperty("F23", m_coreModel.GetInt32(false));

        t1.AddKeys(f11, f12);
        t2.AddKeys(f23);

        EdmNavigationProperty p101 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P101", Target = t2, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f13 }, PrincipalProperties = t2.Key() },
            new EdmNavigationPropertyInfo() { Name = "P201", TargetMultiplicity = EdmMultiplicity.One });
        EdmNavigationProperty p102 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P102", Target = t2, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "P202", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f21, f22 }, PrincipalProperties = t1.Key() });
        EdmNavigationProperty p103 = t1.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "P103", Target = t2, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "P203", TargetMultiplicity = EdmMultiplicity.One });

        IEdmNavigationProperty p201 = p101.Partner;
        IEdmNavigationProperty p203 = p103.Partner;

        EdmEntityContainer c1 = new EdmEntityContainer("Bunk", "Gunk");
        model.AddElement(c1);
        EdmEntitySet E1 = c1.AddEntitySet("E1", t2);
        EdmSingleton S1 = c1.AddSingleton("S1", t1);
        EdmSingleton S2 = new EdmSingleton(c1, "S2", t2);
        c1.AddElement(S2);

        S1.AddNavigationTarget(p102, S2);
        S1.AddNavigationTarget(p103, E1);

        S2.AddNavigationTarget(p201, E1);
        S2.AddNavigationTarget(p203, E1);

        E1.AddNavigationTarget(p201, S1);
        E1.AddNavigationTarget(p203, S1);

        Assert.True(S1.EntityType == t1);
        Assert.True(S2.EntityType == t2);
    }

    [Fact]
    public void SerializeConstructedModel_ShouldGenerateValidCsdl()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        EdmEntityType orderType = new EdmEntityType("Westwind", "Order");
        model.AddElements(new IEdmSchemaElement[] { customerType, orderType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", m_coreModel.GetString(true));

        model.SetAnnotationValue(customerName, "Grumble", "Stumble", new EdmStringConstant(m_coreModel.GetString(false), "Rumble"));
        model.SetAnnotationValue(customerName, "Grumble", "Tumble", new EdmStringConstant(m_coreModel.GetString(false), "Bumble"));
        model.SetAnnotationValue<Boxed<int>>(customerName, new Boxed<int>(66));

        EdmStructuralProperty orderID = orderType.AddStructuralProperty("IDO", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderCustomerID = orderType.AddStructuralProperty("CustomerID", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderItem = orderType.AddStructuralProperty("Item", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderQuantity = orderType.AddStructuralProperty("Quantity", m_coreModel.GetInt32(false));

        customerType.AddKeys(customerID);
        orderType.AddKeys(orderID);

        EdmNavigationProperty customerOrders = customerType.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderCustomerID }, PrincipalProperties = new[] { customerID } });

        EdmEntityContainer container = new EdmEntityContainer("Westwind", "Eastwind");
        model.AddElement(container);
        EdmEntitySet customers = container.AddEntitySet("Customers", customerType);
        EdmEntitySet orders = container.AddEntitySet("Orders", orderType);

        customers.AddNavigationTarget(customerOrders, orders);

        this.CompareEdmModelToCsdl(
            model,
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Westwind"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""IDC"" />
    </Key>
    <Property Name=""IDC"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" p3:Stumble=""Rumble"" p3:Tumble=""Bumble"" xmlns:p3=""Grumble"" />
    <Property Name=""Address"" Type=""Edm.String"" />
    <NavigationProperty Name=""Orders"" Type=""Collection(Westwind.Order)"" Partner=""Customer"" />
  </EntityType>
  <EntityType Name=""Order"">
    <Key>
      <PropertyRef Name=""IDO"" />
    </Key>
    <Property Name=""IDO"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""CustomerID"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Item"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Quantity"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Customer"" Type=""Westwind.Customer"" Nullable=""false"" Partner=""Orders"">
      <ReferentialConstraint Property=""CustomerID"" ReferencedProperty=""IDC"" />
    </NavigationProperty>
  </EntityType>
  <EntityContainer Name=""Eastwind"">
    <EntitySet Name=""Customers"" EntityType=""Westwind.Customer"">
      <NavigationPropertyBinding Path=""Orders"" Target=""Orders"" />
    </EntitySet>
    <EntitySet Name=""Orders"" EntityType=""Westwind.Order""/>
  </EntityContainer>
</Schema>"
            );
    }

    [Fact]
    public void NavigationPropertyWithoutPartner_ShouldNotGeneratePartnerProperty()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        EdmEntityType orderType = new EdmEntityType("Westwind", "Order");
        model.AddElements(new IEdmSchemaElement[] { customerType, orderType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", EdmPrimitiveTypeKind.String);

        EdmStructuralProperty orderID = orderType.AddStructuralProperty("IDO", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderCustomerID = orderType.AddStructuralProperty("CustomerID", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderItem = orderType.AddStructuralProperty("Item", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderQuantity = orderType.AddStructuralProperty("Quantity", m_coreModel.GetInt32(false));

        customerType.AddKeys(customerID);
        orderType.AddKeys(orderID);

        EdmNavigationProperty customerOrders = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many });

        Assert.Null(customerOrders.Partner);
        Assert.True(this.GetSerializerResult(model).Count() > 0);
    }

    [Fact]
    public void InferAssociationTypeName_WhenNavigationPropertyNameMatchesEntityTypeName_ShouldValidateUniqueness()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        EdmEntityType orderType = new EdmEntityType("Westwind", "Order");
        model.AddElements(new IEdmSchemaElement[] { customerType, orderType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", m_coreModel.GetString(true));

        EdmStructuralProperty orderID = orderType.AddStructuralProperty("IDO", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderCustomerID = orderType.AddStructuralProperty("CustomerID", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderItem = orderType.AddStructuralProperty("Item", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderQuantity = orderType.AddStructuralProperty("Quantity", m_coreModel.GetInt32(false));

        customerType.AddKeys(customerID);
        orderType.AddKeys(orderID);

        customerType.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.One });

        Assert.Equal(model.SchemaElements.Count(), model.SchemaElements.Distinct(new EdmNamedElementNameComparer()).Count());

        Assert.True(this.GetSerializerResult(model).Count() > 0);
    }

    [Fact]
    public void SelfReferencingNavigationProperty_ShouldValidateSelfAssociation()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        model.AddElements(new IEdmSchemaElement[] { customerType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", m_coreModel.GetString(true));
        customerType.AddKeys(customerID);
        EdmNavigationProperty dependentCustomer = customerType.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Dependents", Target = customerType, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Primary", TargetMultiplicity = EdmMultiplicity.One });
        IEdmNavigationProperty primaryCustomer = dependentCustomer.Partner;

        EdmEntityContainer container = new EdmEntityContainer("Westwind", "Eastwind");
        model.AddElement(container);
        EdmEntitySet customers = container.AddEntitySet("Customers", customerType);
        customers.AddNavigationTarget(dependentCustomer, customers);
        customers.AddNavigationTarget(primaryCustomer, customers);

        Assert.True(this.GetSerializerResult(model).Count() > 0);
    }

    [Fact]
    public void NavigationPropertyWithSelfReferencingPartner_ShouldValidateConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        EdmEntityType orderType = new EdmEntityType("Westwind", "Order");
        model.AddElements(new IEdmSchemaElement[] { customerType, orderType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", m_coreModel.GetString(true));

        EdmStructuralProperty orderID = orderType.AddStructuralProperty("IDO", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderCustomerID = orderType.AddStructuralProperty("CustomerID", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderItem = orderType.AddStructuralProperty("Item", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderQuantity = orderType.AddStructuralProperty("Quantity", m_coreModel.GetInt32(false));

        customerType.AddKeys(customerID);
        orderType.AddKeys(orderID);

        StubEdmNavigationProperty customerOrders = new StubEdmNavigationProperty("Orders")
        {
            DeclaringType = customerType,
            Type = new EdmEntityTypeReference(orderType, true)
        };
        customerType.AddProperty(customerOrders);

        StubEdmNavigationProperty orderCustomer = new StubEdmNavigationProperty("Customers")
        {
            DeclaringType = orderType,
            Type = new EdmEntityTypeReference(customerType, true)
        };
        orderType.AddProperty(orderCustomer);

        customerOrders.Partner = customerOrders;
        orderCustomer.Partner = orderCustomer;

        Assert.Equal(2, model.SchemaElements.Count());
        this.GetSerializerResult(model, out IEnumerable<EdmError> errors);
        Assert.Equal(2, errors.Count());
    }

    [Fact]
    public void NavigationPropertyWithSelfTargetTypeWithReferencingPartner_ShouldValidateConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        model.AddElements(new IEdmSchemaElement[] { customerType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", m_coreModel.GetString(true));

        customerType.AddKeys(customerID);

        StubEdmNavigationProperty BestFriend = new StubEdmNavigationProperty("BestFriend")
        {
            DeclaringType = customerType,
            Type = new EdmEntityTypeReference(customerType, true)
        };

        BestFriend.Partner = BestFriend;

        Assert.Single(model.SchemaElements);
        this.GetSerializerResult(model, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);
    }

    [Fact]
    public void NavigationPropertyWithSelfCollectionTarget_ShouldValidateConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        model.AddElements(new IEdmSchemaElement[] { customerType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", m_coreModel.GetString(true));

        customerType.AddKeys(customerID);

        StubEdmNavigationProperty Friends = new StubEdmNavigationProperty("Friends")
        {
            DeclaringType = customerType,
            Type = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(customerType, true)))
        };

        Friends.Partner = Friends;

        Assert.Single(model.SchemaElements);
        this.GetSerializerResult(model, out IEnumerable<EdmError> errors);
        Assert.Empty(errors);
    }

    [Fact]
    public void DoubleNavigationProperty_ShouldValidateMultipleNavigationRelationships()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        EdmEntityType orderType = new EdmEntityType("Westwind", "Order");
        model.AddElements(new IEdmSchemaElement[] { customerType, orderType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", m_coreModel.GetString(true));

        EdmStructuralProperty orderID = orderType.AddStructuralProperty("IDO", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderCustomerID = orderType.AddStructuralProperty("CustomerID", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderItem = orderType.AddStructuralProperty("Item", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderQuantity = orderType.AddStructuralProperty("Quantity", m_coreModel.GetInt32(false));

        customerType.AddKeys(customerID);
        orderType.AddKeys(orderID);

        EdmNavigationProperty customerOrders = customerType.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.One });
        IEdmNavigationProperty orderCustomer = customerOrders.Partner;

        EdmNavigationProperty specialCustomerOrders = customerType.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "SpecialOrders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "SpecialCustomer", TargetMultiplicity = EdmMultiplicity.One });
        IEdmNavigationProperty specialOrderCustomer = specialCustomerOrders.Partner;

        EdmEntityContainer container = new EdmEntityContainer("Westwind", "Eastwind");
        model.AddElement(container);
        EdmEntitySet customers = container.AddEntitySet("Customers", customerType);
        EdmEntitySet orders = container.AddEntitySet("Orders", orderType);
        customers.AddNavigationTarget(customerOrders, orders);
        orders.AddNavigationTarget(orderCustomer, customers);
        customers.AddNavigationTarget(specialCustomerOrders, orders);
        orders.AddNavigationTarget(specialOrderCustomer, customers);

        Assert.True(this.GetSerializerResult(model).Count() > 0);
    }

    [Fact]
    public void EntityContainerWithoutNavigationTarget_ShouldValidateEntityContainerConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customerType = new EdmEntityType("Westwind", "Customer");
        EdmEntityType orderType = new EdmEntityType("Westwind", "Order");
        model.AddElements(new IEdmSchemaElement[] { customerType, orderType });

        EdmStructuralProperty customerID = customerType.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        EdmStructuralProperty customerName = customerType.AddStructuralProperty("Name", m_coreModel.GetString(false));
        EdmStructuralProperty customerAddress = customerType.AddStructuralProperty("Address", m_coreModel.GetString(true));

        EdmStructuralProperty orderID = orderType.AddStructuralProperty("IDO", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderCustomerID = orderType.AddStructuralProperty("CustomerID", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderItem = orderType.AddStructuralProperty("Item", m_coreModel.GetInt32(false));
        EdmStructuralProperty orderQuantity = orderType.AddStructuralProperty("Quantity", m_coreModel.GetInt32(false));

        customerType.AddKeys(customerID);
        orderType.AddKeys(orderID);

        EdmNavigationProperty customerOrders = customerType.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "Orders", Target = orderType, TargetMultiplicity = EdmMultiplicity.Many },
            new EdmNavigationPropertyInfo() { Name = "Customers", TargetMultiplicity = EdmMultiplicity.One });

        EdmEntityContainer container = new EdmEntityContainer("Westwind", "Eastwind");
        model.AddElement(container);
        EdmEntitySet customers = container.AddEntitySet("Customers", customerType);
        EdmEntitySet orders = container.AddEntitySet("Orders", orderType);
        Assert.True(this.GetSerializerResult(model).Count() > 0);
    }

    [Fact]
    public void ValidatePrimitiveTypeReferenceModelShortcuts_ShouldVerifyTypeProperties()
    {
        EdmModel model = new EdmModel();
        IEdmPrimitiveTypeReference binaryRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Binary, false);
        IEdmPrimitiveTypeReference boolRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Boolean, false);
        IEdmPrimitiveTypeReference byteRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Byte, false);
        IEdmPrimitiveTypeReference doubleRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Double, false);
        IEdmPrimitiveTypeReference guidRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Guid, false);
        IEdmPrimitiveTypeReference int16Ref = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Int16, false);
        IEdmPrimitiveTypeReference int32Ref = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Int32, false);
        IEdmPrimitiveTypeReference int64Ref = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Int64, false);
        IEdmPrimitiveTypeReference sbyteRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.SByte, false);
        IEdmPrimitiveTypeReference singleRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Single, false);
        IEdmPrimitiveTypeReference streamRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Stream, false);
        IEdmPrimitiveTypeReference stringRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.String, false);
        IEdmPrimitiveTypeReference decimalRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Decimal, false);
        IEdmPrimitiveTypeReference datetimeoffsetRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.DateTimeOffset, false);
        IEdmPrimitiveTypeReference timeRef = m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Duration, false);

        Assert.Equal(EdmPrimitiveTypeKind.Binary, binaryRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Boolean, boolRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Byte, byteRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Double, doubleRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Guid, guidRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Int16, int16Ref.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Int32, int32Ref.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Int64, int64Ref.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.SByte, sbyteRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Single, singleRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Stream, streamRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.String, stringRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Decimal, decimalRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.DateTimeOffset, datetimeoffsetRef.PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Duration, timeRef.PrimitiveKind());

        Assert.Null(decimalRef.AsDecimal().Precision);
        Assert.Null(decimalRef.AsDecimal().Scale);

        Assert.Equal(0, timeRef.AsTemporal().Precision);
    }

    [Fact]
    public void ConstructSpatialPrimitiveTypeReferences_ShouldValidateSpatialTypeConfiguration()
    {
        EdmModel model = new EdmModel();
        Assert.Equal(4326, m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Geography, false).AsSpatial().SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Geometry, false).AsSpatial().SpatialReferenceIdentifier);
        Assert.Equal(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.Geography, false).SpatialReferenceIdentifier);
        Assert.Equal(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false).SpatialReferenceIdentifier);
        Assert.Equal(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, false).SpatialReferenceIdentifier);
        Assert.Equal(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, false).SpatialReferenceIdentifier);
        Assert.Equal(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyCollection, false).SpatialReferenceIdentifier);
        Assert.Equal(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, false).SpatialReferenceIdentifier);
        Assert.Equal(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, false).SpatialReferenceIdentifier);
        Assert.Equal(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, false).SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.Geometry, false).SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, false).SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, false).SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, false).SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, false).SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPolygon, false).SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiLineString, false).SpatialReferenceIdentifier);
        Assert.Equal(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPoint, false).SpatialReferenceIdentifier);

        Assert.Equal(1337, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.Geometry, 1337, false).SpatialReferenceIdentifier);
        Assert.Null(m_coreModel.GetSpatial(EdmPrimitiveTypeKind.Geometry, null, false).SpatialReferenceIdentifier);
    }

    [Fact]
    public void ConstructOperations_ShouldValidateOperationConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customer = new EdmEntityType("Westwind", "Customer");
        EdmStructuralProperty customerID = customer.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        customer.AddKeys(customerID);
        model.AddElement(customer);

        EdmEntityContainer container = new EdmEntityContainer("Westwind", "Gunk");
        model.AddElement(container);
        EdmEntitySet customers = container.AddEntitySet("Customers", customer);

        EdmOperation foo = new EdmFunction("Westwind", "Foo", m_coreModel.GetInt32(true));
        foo.AddParameter(new EdmOperationParameter(foo, "Fred", m_coreModel.GetInt32(false)));
        foo.AddParameter("Barney", m_coreModel.GetInt32(false));
        model.AddElement(foo);

        EdmOperation bar = new EdmAction("Westwind", "Bar", m_coreModel.GetInt32(false));
        model.AddElement(bar);

        EdmAction bazAction = new EdmAction("Westwind", "baz", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, false)));
        EdmOperationParameter wilma = new EdmOperationParameter(bazAction, "Wilma", m_coreModel.GetInt32(false));
        bazAction.AddParameter(wilma);
        model.AddElement(bazAction);
        EdmActionImport baz = container.AddActionImport("baz", bazAction, new EdmPathExpression(customers.Name));
        Assert.Equal(container, baz.Container);

        this.CompareEdmModelToCsdl(
            model,
@"<Schema Namespace=""Westwind"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""IDC"" />
    </Key>
    <Property Name=""IDC"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <Function Name=""Foo"">
    <Parameter Name=""Barney"" Nullable=""false"" Type=""Edm.Int32"" />
    <Parameter Name=""Fred"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Action Name=""Bar"">
    <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
  </Action>
  <Action Name=""baz"">
    <Parameter Name=""Wilma"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Collection(Westwind.Customer)"" Nullable=""false"" />
  </Action>
  <EntityContainer Name=""Gunk"">
    <EntitySet EntityType=""Westwind.Customer"" Name=""Customers"" />
    <ActionImport Action=""Westwind.baz"" EntitySet=""Customers"" Name=""baz"" />
  </EntityContainer>
</Schema>"
        );

        Assert.Equal(bazAction.FindParameter("Wilma"), wilma);
        Assert.Null(bazAction.FindParameter("Betty"));

        bar = new EdmFunction("Westwind", "Barre", m_coreModel.GetString(false));
        model.AddElement(bar);

        foo = new EdmAction("Westwind", "Foo", m_coreModel.GetInt32(true));
        foo.AddParameter(new EdmOperationParameter(foo, "Fred", m_coreModel.GetInt32(false)));
        model.AddElement(foo);

        bazAction = new EdmAction("Westwind", "baz", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, false)));
        bazAction.AddParameter(wilma);
        bazAction.AddParameter("Betty", m_coreModel.GetString(false));
        model.AddElement(bazAction);
        container.AddActionImport("baz", bazAction, null);

        Assert.Equal(bazAction.FindParameter("Betty").Name, "Betty");
        Assert.Null(bazAction.FindParameter("Barney"));

        this.CompareEdmModelToCsdl(
            model,
@"<Schema Namespace=""Westwind"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""IDC"" />
    </Key>
    <Property Name=""IDC"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <Function Name=""Foo"">
    <Parameter Name=""Barney"" Nullable=""false"" Type=""Edm.Int32"" />
    <Parameter Name=""Fred"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Action Name=""Foo"">
    <Parameter Name=""Fred"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Action>
  <Action Name=""Bar"">
    <ReturnType Type=""Edm.Int32"" Nullable=""false"" />
  </Action>
  <Action Name=""baz"">
    <Parameter Name=""Wilma"" Nullable=""false"" Type=""Edm.Int32"" />
    <ReturnType Type=""Collection(Westwind.Customer)"" Nullable=""false"" />
  </Action>
  <Action Name=""baz"">
    <Parameter Name=""Wilma"" Nullable=""false"" Type=""Edm.Int32"" />
    <Parameter Name=""Betty"" Nullable=""false"" Type=""Edm.String"" />
    <ReturnType Type=""Collection(Westwind.Customer)"" Nullable=""false"" />
  </Action>
  <EntityContainer Name=""Gunk"">
    <EntitySet EntityType=""Westwind.Customer"" Name=""Customers"" />
    <ActionImport Action=""Westwind.baz"" EntitySet=""Customers"" Name=""baz"" />
    <ActionImport Action=""Westwind.baz"" Name=""baz"" />
  </EntityContainer>
  <Function Name=""Barre"">
    <ReturnType Type=""Edm.String"" Nullable=""false"" />
  </Function>
</Schema>"
        );
    }

    [Fact]
    public void ConstructEnums_ShouldValidateEnumConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEnumType colors = new EdmEnumType("Foo", "Colors");
        var red = new EdmEnumMember(colors, "Red", new EdmEnumMemberValue(1));
        colors.AddMember(red);
        colors.AddMember("Blue", new EdmEnumMemberValue(2));
        colors.AddMember("Green", new EdmEnumMemberValue(3));
        colors.AddMember("Orange", new EdmEnumMemberValue(4));

        EdmEnumType gender = new EdmEnumType("Foo", "Gender", EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
        gender.AddMember("Male", new EdmEnumMemberValue(1));
        gender.AddMember("Female", new EdmEnumMemberValue(2));

        Assert.Equal(4, colors.Members.Count());
        Assert.Equal(2, gender.Members.Count());

        Assert.Equal("Foo", colors.Namespace);
        Assert.Equal("Colors", colors.Name);
        Assert.False(colors.IsFlags);

        Assert.Equal("Foo", gender.Namespace);
        Assert.Equal("Gender", gender.Name);
        Assert.True(gender.IsFlags);

        Assert.Equal("Male", gender.Members.First().Name);
        Assert.Equal(1, gender.Members.First().Value.Value);
        Assert.Equal("Female", gender.Members.ElementAt(1).Name);
        Assert.Equal(2, gender.Members.ElementAt(1).Value.Value);

        model.AddElement(colors);
        model.AddElement(gender);

        this.CompareEdmModelToCsdl(
            model,
            @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EnumType Name=""Colors"">
    <Member Name=""Blue"" Value=""2"" /> 
    <Member Name=""Green"" Value=""3"" /> 
    <Member Name=""Orange"" Value=""4"" /> 
    <Member Name=""Red"" Value=""1"" /> 
  </EnumType>
  <EnumType IsFlags=""true"" Name=""Gender"">
    <Member Name=""Female"" Value=""2"" /> 
    <Member Name=""Male"" Value=""1"" /> 
  </EnumType>
</Schema>");
    }

    [Fact]
    public void EnumRoundTrip_ShouldValidateSerializationAndDeserialization()
    {
        var csdl = @"
<Schema Namespace=""Foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EnumType Name=""ÁËìôťŽš"">
    <Member Name=""Blue"" Value=""2"" /> 
    <Member Name=""ボ施"" Value=""3"" /> 
    <Member Name=""Orange"" Value=""4"" /> 
    <Member Name=""šЁσςk"" Value=""1"" /> 
  </EnumType>
  <EnumType IsFlags=""true"" Name=""Gender"">
    <Member Name=""Female"" Value=""2"" /> 
    <Member Name=""ÁËìôťŽš"" Value=""1"" /> 
  </EnumType>
</Schema>";
        var expectCsdls = new XElement[] { XElement.Parse(csdl) };
        var isParsed = SchemaReader.TryParse(expectCsdls.Select(e => e.CreateReader()), out var model, out IEnumerable<EdmError> errors);
        Assert.True(isParsed);
        Assert.False(errors.Any());

        model.Validate(out errors);
        Assert.Empty(errors);

        var enums = model.SchemaElements.OfType<IEdmEnumType>();
        var colors = enums.Where(n => n.Name.Equals("ÁËìôťŽš")).First();
        var gender = enums.Where(n => n.Name.Equals("Gender")).First();

        Assert.Equal(4, colors.Members.Count());
        Assert.Equal(2, gender.Members.Count());

        Assert.Equal("Foo", colors.Namespace);
        Assert.Equal("ÁËìôťŽš", colors.Name);
        Assert.False(colors.IsFlags);

        Assert.Equal("ÁËìôťŽš", gender.Members.ElementAt(1).Name);
        Assert.Equal(1, (gender.Members.ElementAt(1).Value).Value);
        Assert.Equal("Female", gender.Members.ElementAt(0).Name);
        Assert.Equal(2, (gender.Members.ElementAt(0).Value).Value);

        var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

        XElement expectedContainers = ExtractElementByName(expectCsdls.ToList(), "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualCsdls.ToList(), "EntityContainer");
    }

    [Fact]
    public void ConstructFunctionImports_ShouldValidateFunctionImportConfiguration()
    {
        var customer = new EdmEntityType("Westwind", "Customer");
        var container = new EdmEntityContainer("Westwind", "Gunk");

        Func<EdmModel> createBaseModel = () =>
        {
            EdmModel newModel = new EdmModel();
            EdmStructuralProperty customerID = customer.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
            customer.AddKeys(customerID);
            newModel.AddElement(customer);

            newModel.AddElement(container);
            return newModel;
        };
        var model = createBaseModel();
        EdmEntitySet customers = container.AddEntitySet("Customers", customer);

        var fooAction = new EdmAction("Westwind", "Foo", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, false)));
        model.AddElement(fooAction);
        var foo = new EdmActionImport(container, "Foo", fooAction);
        container.AddElement(foo);

        var fooFunction = new EdmFunction("Westwind", "Foo", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, false)), true /*isBound*/, null, true /*isComposable*/);
        model.AddElement(fooFunction);
        fooFunction.AddParameter("Fred", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, false)));
        container.AddFunctionImport("Foo", fooFunction, new EdmPathExpression(customers.Name));

        this.CompareEdmModelToCsdl(
            model,
@"<Schema Namespace=""Westwind"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""IDC"" />
    </Key>
    <Property Name=""IDC"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <Action Name=""Foo"">
    <ReturnType Type=""Collection(Westwind.Customer)"" Nullable=""false"" />
  </Action>
  <Function Name=""Foo"" IsBound=""true"" IsComposable=""true"">
    <Parameter Name=""Fred"" Type=""Collection(Westwind.Customer)"" Nullable=""false"" />
    <ReturnType Type=""Collection(Westwind.Customer)"" Nullable=""false"" />
  </Function>
  <EntityContainer Name=""Gunk"">
    <EntitySet Name=""Customers"" EntityType=""Westwind.Customer"" />
    <ActionImport Name=""Foo"" Action=""Westwind.Foo"" />
    <FunctionImport Name=""Foo"" Function=""Westwind.Foo"" EntitySet=""Customers"" />
  </EntityContainer>
</Schema>");

        customer = new EdmEntityType("Eastwind", "Customer");
        container = new EdmEntityContainer("Eastwind", "Gunk");
        customers = container.AddEntitySet("Customers", customer);
        model = createBaseModel();

        var fooFunction1 = new EdmFunction("Eastwind", "Foo", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, false)), false, null, true);
        model.AddElement(fooFunction1);
        container.AddFunctionImport("Foo", fooFunction1, new EdmPathExpression(customers.Name));

        var foo2Action = new EdmAction("Eastwind", "Foo", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, false)), true /*isBound*/, null);
        foo2Action.AddParameter(new EdmOperationParameter(foo2Action, "Fred", EdmCoreModel.GetCollection(new EdmEntityTypeReference(customer, false))));
        model.AddElement(foo2Action);
        var foo2 = new EdmActionImport(container, "Foo", foo2Action, new EdmPathExpression(customers.Name));
        container.AddElement(foo2);

        this.CompareEdmModelToCsdl(
            model,
@"<Schema Namespace=""Eastwind"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""IDC"" />
    </Key>
    <Property Name=""IDC"" Nullable=""false"" Type=""Edm.Int32"" />
  </EntityType>
  <Function IsComposable=""true"" Name=""Foo"">
    <ReturnType Type=""Collection(Eastwind.Customer)"" Nullable=""false"" />
  </Function>
  <Action IsBound=""true"" Name=""Foo"">
    <Parameter Name=""Fred"" Nullable=""false"" Type=""Collection(Eastwind.Customer)"" />
    <ReturnType Type=""Collection(Eastwind.Customer)"" Nullable=""false"" />
  </Action>
  <EntityContainer Name=""Gunk"">
    <EntitySet EntityType=""Eastwind.Customer"" Name=""Customers"" />
    <FunctionImport EntitySet=""Customers"" Function=""Eastwind.Foo"" Name=""Foo"" />
    <ActionImport Action=""Eastwind.Foo"" EntitySet=""Customers"" Name=""Foo"" />
  </EntityContainer>
</Schema>");
    }

    [Fact]
    public void AddSameObjectMultipleTimes_ShouldDetectAndReportDuplicates()
    {
        var model = new EdmModel();
        var complexType = new EdmComplexType("MyNamespace", "MyComplexType");
        model.AddElement(complexType);
        model.AddElement(complexType);


        IEnumerable<EdmError> edmErrors;
        model.Validate(out edmErrors);

        Assert.Single(edmErrors);

        Assert.Equal(EdmErrorCode.AlreadyDefined, edmErrors.ElementAt(0).ErrorCode);
        Assert.Equal("(MyNamespace.MyComplexType)", edmErrors.ElementAt(0).ErrorLocation.ToString());
    }

    // [EdmLib] Constructible NavProps should not take types that are invalid for a nav prop
    [Fact]
    public void CreateNavigationPropertyWithInvalidType_ShouldThrowArgumentException()
    {
        EdmModel model = new EdmModel();
        EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
        EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
        model.AddElement(t1);
        model.AddElement(t2);

        EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", EdmCoreModel.Instance.GetInt32(false));

        EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", EdmCoreModel.Instance.GetString(false));
        EdmStructuralProperty f22 = t2.AddStructuralProperty("F22", EdmCoreModel.Instance.GetInt32(false));
        EdmStructuralProperty f23 = t2.AddStructuralProperty("F23", EdmCoreModel.Instance.GetInt32(false));

        t1.AddKeys(f11, f12);
        t2.AddKeys(f23);

        var exception1 = Assert.Throws<ArgumentException>(() => EdmNavigationProperty.CreateNavigationPropertyWithPartner(
            "qqq", EdmCoreModel.Instance.GetInt32(false /*isNullable*/), null /*dependentProperties*/, null /*principalProperties*/, false /*containsTarget*/, EdmOnDeleteAction.None,
            "ppp", new EdmEntityTypeReference(t1, true /*isNullable*/), null /*partnerDependentProperties*/, null /*partnerPrincipalProperties*/, false /*partnerContainsTarget*/, EdmOnDeleteAction.None));
        Assert.Equal("propertyType", exception1.ParamName);

        var exception2 = Assert.Throws<ArgumentException>(() => EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                "qqq", new EdmEntityTypeReference(t1, true /*isNullable*/), null /*dependentProperties*/, null /*principalProperties*/, false /*containsTarget*/, EdmOnDeleteAction.None,
                "ppp", EdmCoreModel.Instance.GetInt32(false /*isNullable*/), null /*partnerDependentProperties*/, null /*partnerPrincipalProperties*/, false /*partnerContainsTarget*/, EdmOnDeleteAction.None));
        Assert.Equal("partnerPropertyType", exception2.ParamName);
    }

    [Fact]
    // [EdmLib] Adding the same element twice should throw an exception.
    public void ValidateEdmCoreModel_ShouldVerifyCoreModelConfiguration()
    {
        Assert.Equal("Edm", EdmCoreModel.Namespace);
        Assert.Equal(40, EdmCoreModel.Instance.SchemaElements.Count());
        Assert.Empty(EdmCoreModel.Instance.VocabularyAnnotations);
        Assert.Empty(EdmCoreModel.Instance.ReferencedModels);
        Assert.Null(EdmCoreModel.Instance.EntityContainer);
        Assert.Null(EdmCoreModel.Instance.FindTerm("Edm.Int32"));
        Assert.Empty(EdmCoreModel.Instance.FindOperations("Edm.Int32"));
        Assert.Null(EdmCoreModel.Instance.FindEntityContainer("Edm.Int32"));
        Assert.Empty(EdmCoreModel.Instance.FindVocabularyAnnotations(new EdmEntityType("Foo", "Bar")));
        Assert.Equal(EdmPrimitiveTypeKind.SByte, EdmCoreModel.Instance.GetSByte(false).PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Stream, EdmCoreModel.Instance.GetStream(false).PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Stream, EdmCoreModel.Instance.GetStream(false).PrimitiveKind());
        Assert.Equal(EdmPrimitiveTypeKind.Single, EdmCoreModel.Instance.GetSingle(false).PrimitiveKind());
    }

    [Fact]
    public void ConstructElementAnnotations_ShouldValidateAnnotationsConfiguration()
    {
        EdmModel model = new EdmModel();
        EdmEntityType customer = new EdmEntityType("Westwind", "Customer");
        EdmStructuralProperty customerID = customer.AddStructuralProperty("IDC", m_coreModel.GetInt32(false));
        customer.AddKeys(customerID);
        model.AddElement(customer);

        EdmEntityContainer container = new EdmEntityContainer("Westwind", "Gunk");
        model.AddElement(container);
        EdmEntitySet customers = container.AddEntitySet("Customers", customer);

        XElement contacts =
            new XElement("{http://msn}Contacts",
                new XElement("{http://msn}Contact",
                    new XElement("{http://msn}Name", "Patrick Hines"),
                    new XElement("{http://msn}Phone", "206-555-0144"),
                    new XElement("{http://msn}Address",
                        new XElement("{http://msn}Street1", "123 Main St"),
                        new XElement("{http://msn}City", "Mercer Island"),
                        new XElement("{http://msn}State", "WA"),
                        new XElement("{http://msn}Postal", "68042")
                    )
                )
            );

        var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), contacts.ToString());
        annotation.SetIsSerializedAsElement(model, true);
        model.SetAnnotationValue(customer, "http://msn", "Contacts", annotation);

        this.CompareEdmModelToCsdl(
            model,
@"<Schema Namespace=""Westwind"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""Gunk"">
    <EntitySet EntityType=""Westwind.Customer"" Name=""Customers"" />
  </EntityContainer>
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""IDC"" />
    </Key>
    <Property Name=""IDC"" Nullable=""false"" Type=""Edm.Int32"" />
    <Contacts xmlns=""http://msn"">
      <Contact>
        <Name>Patrick Hines</Name>
        <Phone>206-555-0144</Phone>
        <Address>
          <Street1>123 Main St</Street1>
          <City>Mercer Island</City>
          <State>WA</State>
          <Postal>68042</Postal>
        </Address>
      </Contact>
    </Contacts>
  </EntityType>
</Schema>"
            );
    }

    [Fact]
    public void GetAssociationName_ShouldValidateAssociationNameGeneration()
    {
        var model = new EdmModel();

        var entityA = new EdmEntityType("foo.bar", "A");
        var entityAId = entityA.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        entityA.AddKeys(entityAId);
        model.AddElement(entityA);

        var entityB = new EdmEntityType("foo.bar", "B");
        var entityBId = entityB.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        entityB.AddKeys(entityBId);
        model.AddElement(entityB);

        EdmNavigationProperty aToB = entityA.AddBidirectionalNavigation(
            new EdmNavigationPropertyInfo() { Name = "ToB", Target = entityB, TargetMultiplicity = EdmMultiplicity.One },
            new EdmNavigationPropertyInfo() { Name = "ToA", TargetMultiplicity = EdmMultiplicity.One });
        IEdmNavigationProperty bToA = aToB.Partner;

        model.Validate(out IEnumerable<EdmError> errors);
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateNullNameInConstructibleApi_ShouldThrowArgumentNullException()
    {
        var model = new EdmModel();
        Assert.Throws<ArgumentNullException>(() => model.AddElement(new EdmFunction(null, "ValidName", EdmCoreModel.Instance.GetInt64(true))));
        Assert.Throws<ArgumentNullException>(() => model.AddElement(new EdmAction("ValidNamespace", null, EdmCoreModel.Instance.GetInt32(true))));
    }

    [Fact]
    public void AddInvalidEntityContainerElement_ShouldThrowInvalidOperationException()
    {
        var model = new EdmModel();
        var entityContainer = new EdmEntityContainer("NS", "Container");
        var badElement = new CustomEntityContainerElement(entityContainer, "Element", (EdmContainerElementKind)36);
        Assert.Throws<InvalidOperationException>(() => entityContainer.AddElement(badElement));
    }

    [Fact]
    public void AddEntityContainerElementWithNoneKind_ShouldThrowInvalidOperationException()
    {
        var model = new EdmModel();
        var entityContainer = new EdmEntityContainer("NS", "Container");
        var badElement = new CustomEntityContainerElement(entityContainer, "Element", EdmContainerElementKind.None);
        Assert.Throws<InvalidOperationException>(() => entityContainer.AddElement(badElement));
    }

    [Theory]
    [InlineData(EdmVersion.V40)]
    [InlineData(EdmVersion.V401)]
    public void AddDuplicateKeyToEntityType_ShouldDetectAndReportDuplicates(EdmVersion edmVersion)
    {
        var model = new EdmModel();

        var entityType = new EdmEntityType("NS", "Entity");
        var entityId = entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
        entityType.AddKeys(entityId, entityId);
        Assert.Equal(2, entityType.DeclaredKey.Count());

        entityType.AddKeys(entityId);
        Assert.Equal(3, entityType.DeclaredKey.Count());

        var foo = new EdmStructuralProperty(new EdmEntityType("Foo", "Bar"), "FooId", EdmCoreModel.Instance.GetString(false));
        entityType.AddKeys(foo);
        model.AddElement(entityType);
        var validationRuleSet = ValidationRuleSet.GetEdmModelRuleSet(this.GetProductVersion(edmVersion));

        var validationResult = model.Validate(validationRuleSet, out IEnumerable<EdmError>? actualErrors);
        Assert.Equal(3, actualErrors.Count());

        Assert.Equal(EdmErrorCode.KeyPropertyMustBelongToEntity, actualErrors.ElementAt(0).ErrorCode);
        Assert.Equal("The key property 'FooId' must belong to the entity 'Entity'.", actualErrors.ElementAt(0).ErrorMessage);
        Assert.Equal("(NS.Entity)", actualErrors.ElementAt(0).ErrorLocation.ToString());

        Assert.Equal(EdmErrorCode.DuplicatePropertySpecifiedInEntityKey, actualErrors.ElementAt(1).ErrorCode);
        Assert.Equal("The key specified in entity type 'Entity' is not valid. Property 'Id' is referenced more than once in the key element.", actualErrors.ElementAt(1).ErrorMessage);
        Assert.Equal("(Microsoft.OData.Edm.EdmStructuralProperty)", actualErrors.ElementAt(1).ErrorLocation.ToString());

        Assert.Equal(EdmErrorCode.DuplicatePropertySpecifiedInEntityKey, actualErrors.ElementAt(2).ErrorCode);
        Assert.Equal("The key specified in entity type 'Entity' is not valid. Property 'Id' is referenced more than once in the key element.", actualErrors.ElementAt(2).ErrorMessage);
        Assert.Equal("(Microsoft.OData.Edm.EdmStructuralProperty)", actualErrors.ElementAt(2).ErrorLocation.ToString());

        var serializedCsdls = GetSerializerResult(model, edmVersion, out IEnumerable<EdmError> serializationErrors).Select(n => XElement.Parse(n));
        Assert.True(serializedCsdls.Any());
        Assert.False(serializationErrors.Any());

        // if the original test model is not valid, the serializer should still generate CSDLs that parser can handle, but the round trip-ability is not guaranteed.
        var isWellFormed = SchemaReader.TryParse(serializedCsdls.Select(e => e.CreateReader()), out IEdmModel? roundtrippedModel, out IEnumerable<EdmError>? parserErrors);
        Assert.True(isWellFormed);

        var roundTripCsdls = this.GetSerializerResult(roundtrippedModel).Select(n => XElement.Parse(n));

        var expectXElements = serializedCsdls.ToList();
        var actualXElements = roundTripCsdls.ToList();

        Assert.Equal(expectXElements.Count(), actualXElements.Count());

        // extract EntityContainers into one place
        XElement expectedContainers = ExtractElementByName(expectXElements, "EntityContainer");
        XElement actualContainers = ExtractElementByName(actualXElements, "EntityContainer");

        // compare just the EntityContainers
        CsdlXElementComparer.Compare(expectedContainers, actualContainers);

        foreach (var expectXElement in expectXElements)
        {
            var schemaNamespace = expectXElement.Attribute("Namespace") == null ? string.Empty : expectXElement.Attribute("Namespace").Value;
            var actualXElement = actualXElements.FirstOrDefault(e => schemaNamespace == (e.Attribute("Namespace") == null ? string.Empty : e.Attribute("Namespace").Value));

            Assert.NotNull(actualXElement);

            CsdlXElementComparer.Compare(expectXElement, actualXElement);
        }
    }

    #region Private

    private void CompareEdmModelToCsdl(IEdmModel model, string expected)
    {
        string outputText = this.GetSerializerResult(model).Single();
        (new CsdlXElementComparer()).Compare(XElement.Parse(expected), XElement.Parse(outputText));

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(outputText)) }, out IEdmModel parsedModel, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        outputText = this.GetSerializerResult(parsedModel).Single();
        (new CsdlXElementComparer()).Compare(XElement.Parse(outputText), XElement.Parse(expected));
    }

    private class EdmNamedElementNameComparer : IEqualityComparer<IEdmSchemaElement>, IEqualityComparer<IEdmNamedElement>
    {
        public bool Equals(IEdmSchemaElement x, IEdmSchemaElement y)
        {
            if (x.FullName() == y.FullName())
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(IEdmSchemaElement obj)
        {
            return obj.Name.GetHashCode();
        }

        public bool Equals(IEdmNamedElement x, IEdmNamedElement y)
        {
            if (x.Name == y.Name)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(IEdmNamedElement obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    private sealed class CustomEntityContainerElement : IEdmEntityContainerElement
    {
        private readonly IEdmEntityContainer container;
        private readonly EdmContainerElementKind containerElementKind;
        private readonly string name;

        public CustomEntityContainerElement(IEdmEntityContainer container, string name, EdmContainerElementKind containerElementKind)
        {
            this.container = container;
            this.containerElementKind = containerElementKind;
            this.name = name;
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return this.containerElementKind; }
        }

        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }

        public string Name
        {
            get { return this.name; }
        }
    }

    class Boxed<T>
    {
        public readonly T Value;

        public Boxed(T value)
        {
            Value = value;
        }
    }

    #endregion
}
