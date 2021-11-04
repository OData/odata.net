//---------------------------------------------------------------------
// <copyright file="ConstructibleModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.FunctionalUtilities;
    using EdmLibTests.StubEdm;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConstructibleModelTests : EdmLibTestCaseBase
    {
        EdmCoreModel m_coreModel;

        public ConstructibleModelTests()
        {
            this.m_coreModel = EdmCoreModel.Instance;
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void ConstructSimpleEntityWithPrimitiveTypes()
        {
            EdmModel model = new EdmModel();
            EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
            EdmStructuralProperty p1 = t1.AddStructuralProperty("P1", m_coreModel.GetBoolean(false));
            EdmStructuralProperty p2 = t1.AddStructuralProperty("P2", m_coreModel.GetDecimal(1, 1, false));
            EdmStructuralProperty p3 = t1.AddStructuralProperty("P3", m_coreModel.GetTemporal(EdmPrimitiveTypeKind.Duration, 1, false));
            EdmStructuralProperty p4 = t1.AddStructuralProperty("P4", m_coreModel.GetBinary(false, 4, false));
            EdmStructuralProperty p5 = t1.AddStructuralProperty("P5", m_coreModel.GetBinary(false));
            EdmStructuralProperty p6 = t1.AddStructuralProperty("P6", m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Stream, true));
            EdmStructuralProperty p7 = t1.AddStructuralProperty("P7", m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Stream, false));
            IEdmStructuralProperty q1 = (IEdmStructuralProperty)t1.FindProperty("P1");
            IEdmStructuralProperty q2 = (IEdmStructuralProperty)t1.FindProperty("P2");
            IEdmStructuralProperty q3 = (IEdmStructuralProperty)t1.FindProperty("P3");
            IEdmStructuralProperty q4 = (IEdmStructuralProperty)t1.FindProperty("P4");
            IEdmStructuralProperty q5 = (IEdmStructuralProperty)t1.FindProperty("P5");
            IEdmStructuralProperty q6 = (IEdmStructuralProperty)t1.FindProperty("P6");
            IEdmStructuralProperty q7 = (IEdmStructuralProperty)t1.FindProperty("P7");

            Assert.AreEqual(p1, q1, "Property 1 found");
            Assert.AreEqual(p2, q2, "Property 2 found");
            Assert.AreEqual(p3, q3, "Property 3 found");
            Assert.AreEqual(p4, q4, "Property 4 found");
            Assert.AreEqual(p5, q5, "Property 5 found");
            Assert.AreEqual(p6, q6, "Property 6 found");
            Assert.AreEqual(p7, q7, "Property 7 found");

            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, q1.Type.PrimitiveKind(), "Property 1 correct Type");
            Assert.AreEqual(EdmPrimitiveTypeKind.Decimal, q2.Type.PrimitiveKind(), "Property 2 correct Type");
            Assert.AreEqual(EdmPrimitiveTypeKind.Duration, q3.Type.PrimitiveKind(), "Property 3 correct Type");
            Assert.AreEqual(EdmPrimitiveTypeKind.Binary, q4.Type.PrimitiveKind(), "Property 4 correct Type");
            Assert.AreEqual(EdmPrimitiveTypeKind.Binary, q5.Type.PrimitiveKind(), "Property 5 correct Type");
            Assert.AreEqual(EdmPrimitiveTypeKind.Stream, q6.Type.PrimitiveKind(), "Property 6 correct Type");
            Assert.AreEqual(EdmPrimitiveTypeKind.Stream, q7.Type.PrimitiveKind(), "Property 7 correct Type");

            Assert.IsFalse(q1.Type.IsNullable, "Property 1 correct Nullability");
            Assert.IsFalse(q2.Type.IsNullable, "Property 2 correct Nullability");
            Assert.IsFalse(q3.Type.IsNullable, "Property 3 correct Nullability");
            Assert.IsFalse(q4.Type.IsNullable, "Property 4 correct Nullability");
            Assert.IsFalse(q5.Type.IsNullable, "Property 5 correct Nullability");
            Assert.IsTrue(q6.Type.IsNullable, "Property 6 correct Nullability");
            Assert.IsFalse(q7.Type.IsNullable, "Property 7 correct Nullability");

            Assert.AreEqual(1, q2.Type.AsDecimal().Scale, "Property 2 correct Scale");
            Assert.AreEqual(1, q2.Type.AsDecimal().Precision, "Property 2 correct Precision");
            Assert.AreEqual(1, q3.Type.AsTemporal().Precision, "Property 3 correct Precision");
            Assert.AreEqual(false, q4.Type.AsBinary().IsUnbounded, "Property 4 note maxmax");
            Assert.AreEqual(4, q4.Type.AsBinary().MaxLength, "Property 4 correct MaxLength");
        }

        [TestMethod]
        public void ConstructSimpleEntityTypes()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(new EdmModel());
            EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
            EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
            EdmEntityType t3 = new EdmEntityType("Bunk", "T3", t2);
            EdmEntityType t4 = new EdmEntityType("Bunk", "T4", t3);
            EdmEntityType t5 = new EdmEntityType("Bunk", "T5");

            model.WrappedModel.AddElement(t1);
            model.WrappedModel.AddElements(new EdmEntityType[] { t2, t3, t4, t5 });

            Assert.IsTrue(model.SchemaElements.Count() == 5, "ModelElementCount");
            Assert.AreEqual(model.SchemaElements.First(), t1, "FirstModelElement");
            Assert.AreEqual(model.FindType("Bunk.T2"), t2, "FoundSchemaElement");
            Assert.AreEqual(model.SchemaElements.Last(), t5, "LastModelElement");
            Assert.AreEqual(model.FindType("Bunk.T5"), t5, "FoundSchemaElement");

            model.RemoveElement(t5);

            Assert.IsTrue(model.SchemaElements.Count() == 4, "ModelElementCount");
            Assert.AreEqual(model.SchemaElements.First(), t1, "FirstModelElement");
            Assert.AreEqual(model.FindType("Bunk.T2"), t2, "FoundSchemaElement");
            Assert.AreEqual(model.SchemaElements.Last(), t4, "LastModelElement");
            Assert.AreEqual(model.FindType("Bunk.T5"), null, "UnfoundSchemaElement");

            EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", EdmPrimitiveTypeKind.String, false);
            EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", EdmPrimitiveTypeKind.Int32, false);
            EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", m_coreModel.GetInt32(false));

            EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetInt32(false));

            EdmStructuralProperty f31 = t3.AddStructuralProperty("F31", m_coreModel.GetInt32(false));
            EdmStructuralProperty f32 = t3.AddStructuralProperty("F32", m_coreModel.GetInt32(false));

            t1.AddKeys(f12);
            t2.AddKeys(f32);

            t2 = new EdmEntityType("Bunk", "T2");
            model.WrappedModel.AddElement(t2);

            Assert.IsTrue(t3.DeclaredProperties.Count() == 2, "Declared property count");
            Assert.IsTrue(t4.DeclaredProperties.Count() == 0, "Declared property count");
            Assert.IsTrue(t4.Properties().Count() == 3, "Property count");
            Assert.AreEqual(t4.FindProperty("F11"), null, "Not finding F11");
            Assert.AreEqual(t4.FindProperty("F21"), f21, "Finding F21");
            Assert.AreEqual(t4.Key().Single(), f32, "Key");
            Assert.AreEqual(t4.FindProperty("F32"), f32, "Finding F32");

            model.SetAnnotationValue(f11, "Grumble", "Stumble", new EdmStringConstant(m_coreModel.GetString(false), "Rumble"));
            model.SetAnnotationValue(f11, "Grumble", "Tumble", new EdmStringConstant(m_coreModel.GetString(false), "Bumble"));
            model.SetAnnotationValue<Boxed<int>>(f11, new Boxed<int>(66));
            model.SetAnnotationValue<Boxed<string>>(f11, new Boxed<string>("Goop"));

            Assert.IsTrue(model.DirectValueAnnotations(f11).Count() == 4, "Annotations count");
            Assert.IsTrue(((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Stumble")).Value == "Rumble", "Find annotation value.");
            Assert.IsTrue(((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Tumble")).Value == "Bumble", "Find annotation value.");
            Assert.IsTrue(model.GetAnnotationValue<Boxed<int>>(f11).Value == 66, "Find anonymous annotation value.");
            Assert.IsTrue(model.GetAnnotationValue<Boxed<string>>(f11).Value == "Goop", "Find anonymous annotation value.");

            // Test overwriting an existing annotation.

            model.SetAnnotationValue(f11, "Grumble", "Tumble", new EdmStringConstant(m_coreModel.GetString(false), "Fumble"));
            Assert.IsTrue(((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Tumble")).Value == "Fumble", "Find annotation value.");

            // Test adding something to the Documentation namespace that is not documentation.

            bool caught = false;
            try
            {
                model.GetAnnotationValue<Boxed<int>>(f11, "Grumble", "Tumble");
            }
            catch (InvalidOperationException e)
            {
                if (e.Message.Contains("Boxed") && e.Message.Contains("String"))
                {
                    caught = true;
                }
            }

            Assert.IsTrue(caught, "Annotation pun.");

            // Test removing an annotation.

            model.SetAnnotationValue<Boxed<string>>(f11, null);

            Assert.IsTrue(model.DirectValueAnnotations(f11).Count() == 3, "Annotations count.");
            Assert.IsTrue(((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Stumble")).Value == "Rumble", "Find annotation value.");
            Assert.IsTrue(((IEdmStringValue)model.GetAnnotationValue(f11, "Grumble", "Tumble")).Value == "Fumble", "Find annotation value.");
            Assert.IsTrue(model.GetAnnotationValue<Boxed<int>>(f11).Value == 66, "Find anonymous annotation value.");
            Assert.IsNull(model.GetAnnotationValue<Boxed<string>>(f11), "Find anonymous annotation value.");
            Assert.IsNull(model.GetAnnotationValue<Boxed<bool>>(f11), "Find anonymous annotation value.");

            model.SetAnnotationValue<Boxed<int>>(f11, null);
            model.SetAnnotationValue(f11, "Grumble", "Stumble", null);
            model.SetAnnotationValue(f11, "Grumble", "Tumble", null);

            Assert.IsTrue(model.DirectValueAnnotations(f11).Count() == 0, "Annotations count");
            Assert.IsNull(model.GetAnnotationValue(f11, "Grumble", "Stumble"), "Find annotation value.");
            Assert.IsNull(model.GetAnnotationValue(f11, "Grumble", "Tumble"), "Find annotation value.");
            Assert.IsNull(model.GetAnnotationValue<Boxed<int>>(f11), "Find anonymous annotation value.");
            Assert.IsNull(model.GetAnnotationValue<Boxed<string>>(f11), "Find anonymous annotation value.");
            Assert.IsNull(model.GetAnnotationValue<Boxed<bool>>(f11), "Find anonymous annotation value.");
        }

        [TestMethod]
        public void ConstructCyclicStructuredTypes()
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

            Assert.IsTrue(t1.BaseEntityType().IsBad(), "Cyclic base type is bad.");
            Assert.AreEqual(t1.BaseEntityType().Errors().Single().ErrorCode, EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, "Correct error code");
            Assert.IsTrue(t2.BaseEntityType().IsBad(), "Cyclic base type is bad.");
            Assert.AreEqual(t2.BaseEntityType().Errors().Single().ErrorCode, EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, "Correct error code");

            Assert.IsTrue(c1.BaseComplexType().IsBad(), "Cyclic base type is bad.");
            Assert.AreEqual(c1.BaseComplexType().Errors().Single().ErrorCode, EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, "Correct error code");
            Assert.IsTrue(c2.BaseComplexType().IsBad(), "Cyclic base type is bad.");
            Assert.AreEqual(c2.BaseComplexType().Errors().Single().ErrorCode, EdmErrorCode.InterfaceCriticalCycleInTypeHierarchy, "Correct error code");

            IEnumerable<EdmError> serializingErrors;
            IEnumerable<string> csdls = this.GetSerializerResult(model, out serializingErrors);
            Assert.IsTrue(csdls.Count() == 0, "Invalid csdl count.");
            Assert.IsTrue(serializingErrors.Count() > 0, "Invalid serializing error count.");
        }

        [TestMethod]
        public void AddPropertySmokeTest()
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

            try
            {
                // this should not be allowed as it adds property with DeclaringType mismatching the type it is being added to.
                t1.AddProperty(f21);
                Assert.Fail("InvalidOperationException is expected.");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is InvalidOperationException, "InvalidOperationException is expected.");
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

        [TestMethod]
        public void ConstructSimpleComplexTypes()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(new EdmModel());
            EdmComplexType t1 = new EdmComplexType("Bunk", "T1");
            EdmComplexType t2 = new EdmComplexType("Bunk", "T2");
            EdmComplexType t3 = new EdmComplexType("Bunk", "T3", t1, false);
            EdmComplexType t4 = new EdmComplexType("Bunk", "T4", t3, false);
            EdmComplexType t5 = new EdmComplexType("Bunk", "T5");

            model.WrappedModel.AddElement(t1);
            model.WrappedModel.AddElements(new EdmComplexType[] { t2, t3, t4, t5 });

            Assert.IsTrue(model.SchemaElements.Count() == 5, "ModelElementCount");
            Assert.AreEqual(model.SchemaElements.First(), t1, "FirstModelElement");
            Assert.AreEqual(model.FindType("Bunk.T2"), t2, "FoundSchemaElement");
            Assert.AreEqual(model.SchemaElements.Last(), t5, "LastModelElement");
            Assert.AreEqual(model.FindType("Bunk.T5"), t5, "FoundSchemaElement");

            model.RemoveElement(t5);

            Assert.IsTrue(model.SchemaElements.Count() == 4, "ModelElementCount");
            Assert.AreEqual(model.SchemaElements.First(), t1, "FirstModelElement");
            Assert.AreEqual(model.FindType("Bunk.T2"), t2, "FoundSchemaElement");
            Assert.AreEqual(model.SchemaElements.Last(), t4, "LastModelElement");
            Assert.AreEqual(model.FindType("Bunk.T5"), null, "UnfoundSchemaElement");

            EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", m_coreModel.GetString(false));

            EdmStructuralProperty f12 = t1.AddStructuralProperty("F12", m_coreModel.GetInt32(false));
            EdmStructuralProperty f13 = t1.AddStructuralProperty("F13", m_coreModel.GetInt32(false));

            EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", m_coreModel.GetInt32(false));

            EdmStructuralProperty f31 = t3.AddStructuralProperty("F31", m_coreModel.GetInt32(false));
            EdmStructuralProperty f32 = t3.AddStructuralProperty("F32", m_coreModel.GetInt32(false));

            Assert.IsTrue(t4.DeclaredProperties.Count() == 0, "Declared property count");
            Assert.IsTrue(t4.Properties().Count() == t3.Properties().Count(), "Property count");
            Assert.AreEqual(t4.FindProperty("F11"), f11, "finding F11");
            Assert.AreEqual(t4.FindProperty("F21"), null, "Not finding F21");

            t3 = new EdmComplexType("Bunk", "T3", t2, false);
            f31 = t3.AddStructuralProperty("F31", m_coreModel.GetString(false));
            model.WrappedModel.AddElement(t3);

            Assert.IsTrue(t4.DeclaredProperties.Count() == 0, "Declared property count");
            Assert.IsTrue(t4.Properties().Count() == 5, "Property count");
            Assert.IsTrue(t3.Properties().Count() == 2, "Property count");
            Assert.AreEqual(t4.FindProperty("F11"), f11, "finding F11");
            Assert.AreEqual(t4.FindProperty("F21"), null, "Not finding F21");
            Assert.AreEqual(t3.FindProperty("F11"), null, "Not finding F11");
            Assert.AreEqual(t3.FindProperty("F21"), f21, "Finding F21");

            t2 = new EdmComplexType("Bunk", "T2");
            model.WrappedModel.AddElement(t2);

            Assert.IsTrue(t3.Properties().Count() == 2, "Property count");
            Assert.IsTrue(t2.Properties().Count() == 0, "Property count");
            Assert.IsTrue(t4.Properties().Count() == 5, "Property count");
            Assert.AreEqual(t4.FindProperty("F32"), f32, "Finding F32");
            Assert.AreNotEqual(t4.FindProperty("F31"), f31, "Finding F31");
            Assert.AreEqual(t4.FindProperty("F21"), null, "Not Finding F21");

            EdmStructuralProperty f41 = t4.AddStructuralProperty("F41", m_coreModel.GetInt32(false));
            Assert.IsTrue(t4.DeclaredProperties.Count() == 1, "Declared property count");
            Assert.IsTrue(t4.Properties().Count() == 6, "Property count");
            Assert.AreEqual(t4.FindProperty("F41"), f41, "Finding F41");
        }

        [TestMethod]
        public void ConstructOneToOneNavigationProperties()
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
                new EdmNavigationPropertyInfo() { Name = "P101", Target = t2, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f13 }, PrincipalProperties = t2.Key()},
                new EdmNavigationPropertyInfo() { Name = "P201", Target = t1, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            t1.AddProperty(p101);
            t2.AddProperty(p101.Partner);
            EdmNavigationProperty p102 = t1.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "P102", Target = t2, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "P202", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f21, f22 }, PrincipalProperties = t1.Key()});
            EdmNavigationProperty p103 = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "P103", Target = t2, TargetMultiplicity = EdmMultiplicity.One, OnDelete = EdmOnDeleteAction.Cascade },
                new EdmNavigationPropertyInfo() { Name = "P203", Target = t1, TargetMultiplicity = EdmMultiplicity.One });
            t1.AddProperty(p103);
            t2.AddProperty(p103.Partner);
            EdmNavigationProperty p104 = t1.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "P104", Target = t2, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "P204", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            Assert.IsTrue(t1.Properties().Count() == 7, "Properties count");
            Assert.AreEqual(t1.FindProperty("P103"), p103, "Navigation property lookup");

            Assert.IsTrue(p101.PropertyKind == EdmPropertyKind.Navigation, "Property Kind");
            Assert.IsTrue(p102.OnDelete == EdmOnDeleteAction.None, "OnDelete");
            Assert.IsTrue(p103.OnDelete == EdmOnDeleteAction.Cascade, "OnDelete");

            var p202 = p102.Partner;
            var p201 = p101.Partner;
            var p203 = p103.Partner;
            var p204 = p104.Partner;
            Assert.AreEqual(p201.Partner, p101, "Partners");
            Assert.AreEqual(p202.Partner, p102, "Partners");
            Assert.AreEqual(p203.Partner, p103, "Partners");
            Assert.AreEqual(p204.Partner.Name, "P104", "Partners");

            Assert.AreEqual(((IEdmNavigationProperty)p101).ToEntityType(), t2, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p102).ToEntityType(), t2, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p103).ToEntityType(), t2, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p104).ToEntityType(), t2, "ToEntityType");
            Assert.IsFalse(((IEdmNavigationProperty)p104).ContainsTarget, "p104.ContainsTarget");
            Assert.AreEqual(((IEdmNavigationProperty)p201).ToEntityType(), t1, "ToEntityType");
            Assert.IsTrue(((IEdmNavigationProperty)p201).ContainsTarget, "p201.ContainsTarget");
            Assert.AreEqual(((IEdmNavigationProperty)p202).ToEntityType(), t1, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p203).ToEntityType(), t1, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p204).ToEntityType(), t1, "ToEntityType");
        }

        [TestMethod]
        public void ConstructManyToManyNavigationProperties()
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
                new EdmNavigationPropertyInfo() { Name = "P101", Target = t2, TargetMultiplicity = EdmMultiplicity.Many, DependentProperties = new[] { f13 }, PrincipalProperties = t2.Key()},
                new EdmNavigationPropertyInfo() { Name = "P201", TargetMultiplicity = EdmMultiplicity.Many });
            EdmNavigationProperty p102 = t1.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "P102", Target = t2, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "P202", TargetMultiplicity = EdmMultiplicity.Many, DependentProperties = new[] { f21, f22 }, PrincipalProperties = t1.Key()});
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
            Assert.IsNull(p104.Partner, "1-way navigation should not have parter.");
            Assert.IsNull(p104_partner.Partner, "1-way navigation should not have parter.");

            Assert.IsTrue(t1.Properties().Count() == 7, "Properties count");
            Assert.AreEqual(t1.FindProperty("P103"), p103, "Navigation property lookup");

            Assert.IsTrue(p101.PropertyKind == EdmPropertyKind.Navigation, "Property Kind");
            Assert.IsTrue(p102.OnDelete == EdmOnDeleteAction.None, "OnDelete");
            Assert.IsTrue(p103.OnDelete == EdmOnDeleteAction.Cascade, "OnDelete");

            var p201 = p101.Partner;
            var p202 = p102.Partner;
            var p203 = p103.Partner;
            var p204 = p104_partner;

            Assert.AreEqual(p201.Partner, p101, "Partners");
            Assert.AreEqual(p202.Partner, p102, "Partners");
            Assert.AreEqual(p203.Partner, p103, "Partners");
            Assert.AreEqual(p204.Name, p104.Name + "Partner", "Partners");

            Assert.AreEqual(((IEdmNavigationProperty)p101).ToEntityType(), t2, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p102).ToEntityType(), t2, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p103).ToEntityType(), t2, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p104).ToEntityType(), t2, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p201).ToEntityType(), t1, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p202).ToEntityType(), t1, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p203).ToEntityType(), t1, "ToEntityType");
            Assert.AreEqual(((IEdmNavigationProperty)p204).ToEntityType(), t1, "ToEntityType");

            p202 = p103.Partner;

            Assert.AreEqual(p103.Partner, p202, "Partners");
            Assert.AreEqual(p203.Partner.Name, p103.Name, "Partners");
            Assert.AreEqual(p102.Partner.Name, "P202", "Partners");
        }

        [TestMethod]
        public void AddNavigationPropertyToIrrelevantTypes()
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

            this.VerifyThrowsException(typeof(InvalidOperationException), () => t3.AddProperty(p103));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => t3.AddProperty(p103.Partner));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => t3.AddProperty(p101));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => t3.AddProperty(p101.Partner));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => t3.AddProperty(p102));
            this.VerifyThrowsException(typeof(InvalidOperationException), () => t3.AddProperty(p102.Partner));
        }

        [TestMethod]
        public void ConstructEntitySets()
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
                new EdmNavigationPropertyInfo() { Name = "P101", Target = t2, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { f13 } , PrincipalProperties = t2.Key() },
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

            Assert.IsTrue(E1.EntityType() == t1, "EntitySet element type");
            Assert.IsTrue(E2.EntityType() == t2, "EntitySet element type");
        }

        [TestMethod]
        public void ConstructSingletons()
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

            Assert.IsTrue(S1.EntityType() == t1, "EntitySet element type");
            Assert.IsTrue(S2.EntityType() == t2, "EntitySet element type");
        }

        [TestMethod]
        public void SerializeConstructedModel()
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
                new EdmNavigationPropertyInfo() { Name = "Customer", TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { orderCustomerID }, PrincipalProperties = new[]{ customerID } });

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

        [TestMethod]
        public void NavigationPropertyWithoutPartnerSetForAssociationTypeTest()
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
            
            Assert.IsNull(customerOrders.Partner, "EdmNavigationProperty.Partner property should not be synthesized when it is not explicitly");
            Assert.IsNotNull(this.GetSerializerResult(model).Count() > 0, "The CsdlWriter should be able to serialize stock models.");
        }

        [TestMethod]
        public void AssociationTypeNameInferenceWhenNavigationPropertyNameEqualToEntityTypeNameTest()
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

            Assert.AreEqual(model.SchemaElements.Count(), model.SchemaElements.Distinct(new EdmNamedElementNameComparer()).Count(), "All types generated should have unique names");

            Assert.IsNotNull(this.GetSerializerResult(model).Count() > 0, "The CsdlWriter should be able to serialize stock models.");
        }

        [TestMethod]
        public void SelfAssociationNavigationPropertyTest()
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

            Assert.IsNotNull(this.GetSerializerResult(model).Count() > 0, "The CsdlWriter should be able to serialize stock models.");
        }

        [TestMethod]
        public void NavigationPropertyWithPartnerSelfReferenceingTest()
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

            Assert.AreEqual(2, model.SchemaElements.Count(), "The number of the generated schema elements should be 2");
            IEnumerable<EdmError> errors;
            this.GetSerializerResult(model, out errors);
            Assert.AreEqual(2, errors.Count(), "Self-Referencing navigation properties generate error.");
        }

        [TestMethod]
        public void NavigationPropertyWithSelfTargetTypeWithPartnerSelfReferenceingTest()
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

            Assert.AreEqual(1, model.SchemaElements.Count(), "The number of the generated schema elements should be 1");
            IEnumerable<EdmError> errors;
            this.GetSerializerResult(model, out errors);
            Assert.AreEqual(0, errors.Count(), "Self-Referencing navigation properties generate error.");
        }

        [TestMethod]
        public void NavigationPropertyWithSelfCollectionTargetTypeWithPartnerSelfReferenceingTest()
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

            Assert.AreEqual(1, model.SchemaElements.Count(), "The number of the generated schema elements should be 1");
            IEnumerable<EdmError> errors;
            this.GetSerializerResult(model, out errors);
            Assert.AreEqual(0, errors.Count(), "Self-Referencing navigation properties generate error.");
        }

        [TestMethod]
        public void DoubleNavigationPropertyTest()
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

            Assert.IsNotNull(this.GetSerializerResult(model).Count() > 0, "The CsdlWriter should be able to serialize stock models.");
        }

        [TestMethod]
        public void EntityContainerWithoutAddingNavigationTargetTest()
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
            Assert.IsNotNull(this.GetSerializerResult(model).Count() > 0, "The CsdlWriter should be able to serialize stock models.");
        }

        [TestMethod]
        public void PrimitiveTypeReferenceModelShortcutTest()
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

            Assert.AreEqual(EdmPrimitiveTypeKind.Binary, binaryRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Boolean, boolRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Byte, byteRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Double, doubleRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Guid, guidRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Int16, int16Ref.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Int32, int32Ref.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Int64, int64Ref.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.SByte, sbyteRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Single, singleRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Stream, streamRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.String, stringRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Decimal, decimalRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.DateTimeOffset, datetimeoffsetRef.PrimitiveKind(), "Correct primitive kind");
            Assert.AreEqual(EdmPrimitiveTypeKind.Duration, timeRef.PrimitiveKind(), "Correct primitive kind");

            Assert.IsNull(decimalRef.AsDecimal().Precision, "Decimal precision null when created with shortcut");
            Assert.AreEqual(0, decimalRef.AsDecimal().Scale, "Decimal scale 0 when created with shortcut");

            Assert.AreEqual(0, timeRef.AsTemporal().Precision, "Duration precision equals to 0 when created with shortcut");
        }

        [TestMethod]
        public void ConstructSpatialPrimitiveTypeReferences()
        {
            EdmModel model = new EdmModel();
            Assert.AreEqual(4326, m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Geography, false).AsSpatial().SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(0, m_coreModel.GetPrimitive(EdmPrimitiveTypeKind.Geometry, false).AsSpatial().SpatialReferenceIdentifier, "Correct Geometry Srid");
            Assert.AreEqual(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.Geography, false).SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false).SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, false).SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, false).SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyCollection, false).SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, false).SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, false).SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(4326, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, false).SpatialReferenceIdentifier, "Correct Geography Srid");
            Assert.AreEqual(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.Geometry, false).SpatialReferenceIdentifier, "Correct Geometry Srid");
            Assert.AreEqual(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, false).SpatialReferenceIdentifier, "Correct Geometry Srid");
            Assert.AreEqual(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, false).SpatialReferenceIdentifier, "Correct Geometry Srid");
            Assert.AreEqual(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, false).SpatialReferenceIdentifier, "Correct Geometry Srid");
            Assert.AreEqual(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, false).SpatialReferenceIdentifier, "Correct Geometry Srid");
            Assert.AreEqual(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPolygon, false).SpatialReferenceIdentifier, "Correct Geometry Srid");
            Assert.AreEqual(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiLineString, false).SpatialReferenceIdentifier, "Correct Geometry Srid");
            Assert.AreEqual(0, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPoint, false).SpatialReferenceIdentifier, "Correct Geometry Srid");

            Assert.AreEqual(1337, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.Geometry, 1337, false).SpatialReferenceIdentifier, "Correct Manual Srid");
            Assert.AreEqual(null, m_coreModel.GetSpatial(EdmPrimitiveTypeKind.Geometry, null, false).SpatialReferenceIdentifier, "Correct Manual Srid");
        }

        [TestMethod]
        public void ConstructOperations()
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
            Assert.AreEqual(container, baz.Container, "ActionImport with container has container.");

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

            Assert.AreEqual(bazAction.FindParameter("Wilma"), wilma, "Finding Wilma");
            Assert.IsNull(bazAction.FindParameter("Betty"), "Not finding Betty");

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

            Assert.AreEqual(bazAction.FindParameter("Betty").Name, "Betty", "Finding Betty");
            Assert.IsNull(bazAction.FindParameter("Barney"), "Not finding Barney");

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

        [TestMethod]
        public void ConstructEnums()
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

            Assert.AreEqual(colors.Members.Count(), 4, "Correct number of members");
            Assert.AreEqual(gender.Members.Count(), 2, "Correct number of members");

            Assert.AreEqual(colors.Namespace, "Foo", "Correct namespace");
            Assert.AreEqual(colors.Name, "Colors", "Correct name");
            Assert.AreEqual(colors.IsFlags, false, "correct treat as bits");

            Assert.AreEqual(gender.Namespace, "Foo", "Correct namespace");
            Assert.AreEqual(gender.Name, "Gender", "Correct name");
            Assert.AreEqual(gender.IsFlags, true, "correct treat as bits");

            Assert.AreEqual(gender.Members.First().Name, "Male", "Correct member name");
            Assert.AreEqual((gender.Members.First().Value.Value), 1, "Correct member value");
            Assert.AreEqual(gender.Members.ElementAt(1).Name, "Female", "Correct member name");
            Assert.AreEqual((gender.Members.ElementAt(1).Value.Value), 2, "Correct member value");

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

        [TestMethod]
        public void EnumRoundTripTest()
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
            var model = this.GetParserResult(expectCsdls);
            IEnumerable<EdmError> errors;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Invalid error count.");
            
            var enums = model.SchemaElements.OfType<IEdmEnumType>();
            var colors = enums.Where(n => n.Name.Equals("ÁËìôťŽš")).First();
            var gender = enums.Where(n => n.Name.Equals("Gender")).First();

            Assert.AreEqual(colors.Members.Count(), 4, "Correct number of members");
            Assert.AreEqual(gender.Members.Count(), 2, "Correct number of members");

            Assert.AreEqual(colors.Namespace, "Foo", "Correct namespace");
            Assert.AreEqual(colors.Name, "ÁËìôťŽš", "Correct name");
            Assert.AreEqual(colors.IsFlags, false, "correct treat as bits");

            Assert.AreEqual(gender.Namespace, "Foo", "Correct namespace");
            Assert.AreEqual(gender.Name, "Gender", "Correct name");
            Assert.AreEqual(gender.IsFlags, true, "correct treat as bits");

            Assert.AreEqual(gender.Members.ElementAt(1).Name, "ÁËìôťŽš", "Correct member name");
            Assert.AreEqual((gender.Members.ElementAt(1).Value).Value, 1, "Correct member value");
            Assert.AreEqual(gender.Members.ElementAt(0).Name, "Female", "Correct member name");
            Assert.AreEqual((gender.Members.ElementAt(0).Value).Value, 2, "Correct member value");

            var actualCsdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));
            new ConstructiveApiCsdlXElementComparer().Compare(expectCsdls.ToList(), actualCsdls.ToList());
        }

        [TestMethod]
        public void ConstructFunctionImports()
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

        [TestMethod]
        public void ConstructAddSameObjectMultipleTimes()
        {
            var model = new EdmModel();
            var complexType = new EdmComplexType("MyNamespace", "MyComplexType");
            model.AddElement(complexType);
            model.AddElement(complexType);


            IEnumerable<EdmError> edmErrors;
            model.Validate(out edmErrors);

            var expectedErrors = new EdmLibTestErrors()
            {
                { "(MyNamespace.MyComplexType)", EdmErrorCode.AlreadyDefined },
            };
            this.CompareErrors(edmErrors, expectedErrors);
        }

        // [EdmLib] Constructible NavProps should not take types that are invalid for a nav prop
        [TestMethod]
        public void CreateNavigationPropertyWithInvalidType()
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

            try
            {
                EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                    "qqq", EdmCoreModel.Instance.GetInt32(false /*isNullable*/), null /*dependentProperties*/, null /*principalProperties*/, false /*containsTarget*/, EdmOnDeleteAction.None,
                    "ppp", new EdmEntityTypeReference(t1, true /*isNullable*/), null /*partnerDependentProperties*/, null /*partnerPrincipalProperties*/, false /*partnerContainsTarget*/, EdmOnDeleteAction.None);
                Assert.Fail("exception expected");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("propertyType", e.ParamName, "exception");
            }

            try
            {
                EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                    "qqq", new EdmEntityTypeReference(t1, true /*isNullable*/), null /*dependentProperties*/, null /*principalProperties*/, false /*containsTarget*/, EdmOnDeleteAction.None,
                    "ppp", EdmCoreModel.Instance.GetInt32(false /*isNullable*/), null /*partnerDependentProperties*/, null /*partnerPrincipalProperties*/, false /*partnerContainsTarget*/, EdmOnDeleteAction.None);
                Assert.Fail("exception expected");
            }
            catch (ArgumentException e)
            {
                Assert.AreEqual("partnerPropertyType", e.ParamName, "exception");
            }
        }

        [TestMethod]
        // [EdmLib] Adding the same element twice should throw an exception.
        public void EdmCoreModelTests()
        {
            Assert.AreEqual("Edm", EdmCoreModel.Namespace, "Correct Namespace");
#if !ORCAS
            Assert.AreEqual(40, EdmCoreModel.Instance.SchemaElements.Count(), "Core model has one of every type except none.");
#endif
            Assert.AreEqual(0, EdmCoreModel.Instance.VocabularyAnnotations.Count(), "Core model has no annotations.");
            Assert.AreEqual(0, EdmCoreModel.Instance.ReferencedModels.Count(), "Core model has no references.");
            Assert.IsNull(EdmCoreModel.Instance.EntityContainer, "Core model has no containers.");
            Assert.IsNull(EdmCoreModel.Instance.FindTerm("Edm.Int32"), "Find term returns null");
            Assert.AreEqual(0, EdmCoreModel.Instance.FindOperations("Edm.Int32").Count(), "Find functions returns empty enumerable");
            Assert.IsNull(EdmCoreModel.Instance.FindEntityContainer("Edm.Int32"), "Find entity container returns null");
            Assert.AreEqual(0, EdmCoreModel.Instance.FindVocabularyAnnotations(new EdmEntityType("Foo", "Bar")).Count(), "Find vocabulary annoatations returns empty enumerable");
            Assert.AreEqual(EdmPrimitiveTypeKind.SByte, EdmCoreModel.Instance.GetSByte(false).PrimitiveKind(), "Can get SByte.");
            Assert.AreEqual(EdmPrimitiveTypeKind.Stream, EdmCoreModel.Instance.GetStream(false).PrimitiveKind(), "Can get SByte.");
            Assert.AreEqual(EdmPrimitiveTypeKind.Stream, EdmCoreModel.Instance.GetStream(false).PrimitiveKind(), "Can get Stream.");
            Assert.AreEqual(EdmPrimitiveTypeKind.Single, EdmCoreModel.Instance.GetSingle(false).PrimitiveKind(), "Can get Single.");
        }

        [TestMethod]
        public void ConstructElementAnnotations()
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

        [TestMethod]
        public void ConstructibleGettingAssociationName()
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

            IEnumerable<EdmError> errors = null;
            model.Validate(out errors);
            Assert.AreEqual(0, errors.Count(), "Unexpected errors.");
        }

        [TestMethod]
        public void ConstructibleApiNullNameCheck()
        {
            var model = new EdmModel();
            this.VerifyThrowsException(typeof(ArgumentNullException), () => model.AddElement(new EdmFunction(null, "ValidName", EdmCoreModel.Instance.GetInt64(true))));
            this.VerifyThrowsException(typeof(ArgumentNullException), () => model.AddElement(new EdmAction("ValidNamespace", null, EdmCoreModel.Instance.GetInt32(true))));
        }

        [TestMethod]
        public void ConstructibleApiEntityContainerElementWithInvalidElementKind()
        {
            var model = new EdmModel();
            var entityContainer = new EdmEntityContainer("NS", "Container");
            var badElement = new CustomEntityContainerElement(entityContainer, "Element", (EdmContainerElementKind)36);
            this.VerifyThrowsException(typeof(InvalidOperationException), () => entityContainer.AddElement(badElement));
        }

        [TestMethod]
        public void ConstructibleApiEntityContainerElementWithNoneElementKind()
        {
            var model = new EdmModel();
            var entityContainer = new EdmEntityContainer("NS", "Container");
            var badElement = new CustomEntityContainerElement(entityContainer, "Element", EdmContainerElementKind.None);
            this.VerifyThrowsException(typeof(InvalidOperationException), () => entityContainer.AddElement(badElement));
        }

        [TestMethod]
        public void EntityTypeAddingDuplicateKey()
        {
            var model = new EdmModel();

            var entityType = new EdmEntityType("NS", "Entity");
            var entityId = entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddKeys(entityId, entityId);
            Assert.AreEqual(2, entityType.DeclaredKey.Count(), "Invalid key count.");

            entityType.AddKeys(entityId);
            Assert.AreEqual(3, entityType.DeclaredKey.Count(), "Invalid key count.");

            var foo = new EdmStructuralProperty(new EdmEntityType("Foo", "Bar"), "FooId", EdmCoreModel.Instance.GetString(false));
            entityType.AddKeys(foo);
            model.AddElement(entityType);

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicatePropertySpecifiedInEntityKey },
                { null, null, EdmErrorCode.DuplicatePropertySpecifiedInEntityKey },
                { null, null, EdmErrorCode.KeyPropertyMustBelongToEntity }
            };
            this.VerifySemanticValidation(model, expectedErrors);

            var csdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

            var roundTripModel = this.GetParserResult(csdls);

            expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.DuplicatePropertySpecifiedInEntityKey },
                { null, null, EdmErrorCode.DuplicatePropertySpecifiedInEntityKey },
                { null, null, EdmErrorCode.BadUnresolvedProperty }
            };
            this.VerifySemanticValidation(roundTripModel, expectedErrors);

            var roundTripCsdls = this.GetSerializerResult(roundTripModel).Select(n => XElement.Parse(n));

            new ConstructiveApiCsdlXElementComparer().Compare(csdls.ToList(), roundTripCsdls.ToList());
        }

        //[TestMethod, Variation(Id = 110, SkipReason = @"[EdmLib] InvalidOperationException occurs when doing a round trip on model with duplication property that is being used as a key -- postponed")]
        public void EntityTypeDuplicateKeyProperty()
        {
            var model = new EdmModel();

            var entityType = new EdmEntityType("NS", "Entity");
            var entityId = entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddProperty(entityId);
            entityType.AddKeys(entityId);
            model.AddElement(entityType);
            Assert.AreEqual(2, entityType.Properties().Count(), "Invalid key count.");

            var expectedErrors = new EdmLibTestErrors()
            {
                { null, null, EdmErrorCode.AlreadyDefined }
            };

            this.VerifySemanticValidation(model, expectedErrors);

            var csdls = this.GetSerializerResult(model).Select(n => XElement.Parse(n));

            var roundTripModel = this.GetParserResult(csdls);
            this.VerifySemanticValidation(roundTripModel, expectedErrors);

            var roundTripCsdls = this.GetSerializerResult(roundTripModel).Select(n => XElement.Parse(n));

            new ConstructiveApiCsdlXElementComparer().Compare(csdls.ToList(), roundTripCsdls.ToList());
        }

        private void CompareEdmModelToCsdl(IEdmModel model, string expected)
        {
            string outputText = this.GetSerializerResult(model).Single();
            (new CsdlXElementComparer()).Compare(XElement.Parse(expected), XElement.Parse(outputText));

            IEdmModel parsedModel;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(outputText)) }, out parsedModel, out errors);
            Assert.IsTrue(parsed, "Parsing serialized model");
            Assert.IsTrue(errors.Count() == 0, "No errors");

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
    }
}
