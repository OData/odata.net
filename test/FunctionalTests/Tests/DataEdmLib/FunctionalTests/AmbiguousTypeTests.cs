//---------------------------------------------------------------------
// <copyright file="AmbiguousTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using EdmLibTests.StubEdm;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EdmLibTests.FunctionalTests
{
    [TestClass]
    public class AmbiguousTypeTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void AmbiguousTermTest()
        {
            EdmModel model = new EdmModel();

            IEdmTerm term1 = new EdmTerm("Foo", "Bar", EdmPrimitiveTypeKind.Byte);
            IEdmTerm term2 = new EdmTerm("Foo", "Bar", EdmPrimitiveTypeKind.Decimal);
            IEdmTerm term3 = new EdmTerm("Foo", "Bar", EdmCoreModel.Instance.GetPrimitive(EdmPrimitiveTypeKind.Double, false));

            model.AddElement(term1);
            Assert.AreEqual(term1, model.FindTerm("Foo.Bar"), "Correct item.");

            model.AddElement(term2);
            model.AddElement(term3);

            IEdmTerm ambiguous = model.FindTerm("Foo.Bar");
            Assert.IsTrue(ambiguous.IsBad(), "Ambiguous binding is bad");

            Assert.AreEqual(EdmSchemaElementKind.Term, ambiguous.SchemaElementKind, "Correct schema element kind.");
            Assert.AreEqual("Foo", ambiguous.Namespace, "Correct Namespace");
            Assert.AreEqual("Bar", ambiguous.Name, "Correct Name");
            Assert.IsTrue(ambiguous.Type.IsBad(), "Type is bad.");
        }

        [TestMethod]
        public void AmbiguousEntitySetTest()
        {
            EdmEntityContainer container = new EdmEntityContainer("NS1", "Baz");

            IEdmEntitySet set1 = new StubEdmEntitySet("Foo", container);
            IEdmEntitySet set2 = new StubEdmEntitySet("Foo", container);
            IEdmEntitySet set3 = new StubEdmEntitySet("Foo", container);

            container.AddElement(set1);
            Assert.AreNotEqual(set3, container.FindEntitySet("Foo"), "Checking the object equality.");
            Assert.AreEqual(set3.Name, container.FindEntitySet("Foo").Name, "Checking the object equality.");

            container.AddElement(set2);
            container.AddElement(set3);

            IEdmEntitySet ambiguous = container.FindEntitySet("Foo");
            Assert.IsTrue(ambiguous.IsBad(), "Ambiguous binding is bad");

            Assert.AreEqual(EdmContainerElementKind.EntitySet, ambiguous.ContainerElementKind, "Correct container element kind");
            Assert.AreEqual("NS1.Baz", ambiguous.Container.FullName(), "Correct container name");
            Assert.AreEqual("Foo", ambiguous.Name, "Correct Name");
            Assert.IsTrue(ambiguous.EntityType().IsBad(), "Association is bad.");
        }

        [TestMethod]
        public void AmbiguousTypeTest()
        {
            EdmModel model = new EdmModel();

            IEdmSchemaType type1 = new StubEdmComplexType("Foo", "Bar");
            IEdmSchemaType type2 = new StubEdmComplexType("Foo", "Bar");
            IEdmSchemaType type3 = new StubEdmComplexType("Foo", "Bar");

            model.AddElement(type1);
            Assert.AreEqual(type1, model.FindType("Foo.Bar"), "Correct item.");

            model.AddElement(type2);
            model.AddElement(type3);

            IEdmSchemaType ambiguous = model.FindType("Foo.Bar");
            Assert.IsTrue(ambiguous.IsBad(), "Ambiguous binding is bad");

            Assert.AreEqual(EdmSchemaElementKind.TypeDefinition, ambiguous.SchemaElementKind, "Correct schema element kind.");
            Assert.AreEqual("Foo", ambiguous.Namespace, "Correct Namespace");
            Assert.AreEqual("Bar", ambiguous.Name, "Correct Name");
            Assert.AreEqual(EdmTypeKind.None, ambiguous.TypeKind, "Correct type kind.");
        }

        [TestMethod]
        public void AmbiguousPropertyTest()
        {
            EdmComplexType complex = new EdmComplexType("Bar", "Foo");

            StubEdmStructuralProperty prop1 = new StubEdmStructuralProperty("Foo");
            StubEdmStructuralProperty prop2 = new StubEdmStructuralProperty("Foo");
            StubEdmStructuralProperty prop3 = new StubEdmStructuralProperty("Foo");

            prop1.DeclaringType = complex;
            prop2.DeclaringType = complex;
            prop3.DeclaringType = complex;

            complex.AddProperty(prop1);
            complex.AddProperty(prop2);
            complex.AddProperty(prop3);

            IEdmProperty ambiguous = complex.FindProperty("Foo");
            Assert.IsTrue(ambiguous.IsBad(), "Ambiguous binding is bad");

            Assert.AreEqual(EdmPropertyKind.None, ambiguous.PropertyKind, "No property kind.");
            Assert.AreEqual(complex, ambiguous.DeclaringType, "Correct declaring type.");
            Assert.IsTrue(ambiguous.Type.IsBad(), "Type is bad.");

            complex = new EdmComplexType("Bar", "Foo");
            prop3 = new StubEdmStructuralProperty("Foo");
            prop3.DeclaringType = complex;
            complex.AddProperty(prop3);
            Assert.AreEqual(prop3, complex.FindProperty("Foo"), "Correct last item remaining.");
        }
    }
}
