//---------------------------------------------------------------------
// <copyright file="ConstructibleVocabularyAnnotationsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class ExtentionMethods
    {
        public static IEdmEntityType FindEntityType(this IEdmModel model, string qualifiedName)
        {
            return model.FindType(qualifiedName) as IEdmEntityType;
        }
    }

    [TestClass]
    public class ConstructibleVocabularyAnnotationsTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void CreateSimpleTermAnnotation()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(CreateModel());

            var term = model.FindTerm("NS1.Title");
            var customer = model.FindEntityType("NS1.Customer");

            var annotation = new EdmVocabularyAnnotation(
                customer,
                term,
                "q1",
                new EdmStringConstant("Hello world!"));
            model.WrappedModel.AddVocabularyAnnotation(annotation);

            var annotation2 = new EdmVocabularyAnnotation(
                customer,
                term,
                "q2",
                new EdmStringConstant("Hello world2!"));
            model.WrappedModel.AddVocabularyAnnotation(annotation2);

            var annotations = customer.VocabularyAnnotations(model).ToList();
            Assert.AreEqual(2, annotations.Count, "customer.VocabularyAnnotations(model).Count");
            Assert.IsTrue(annotations.Contains(annotation), "annotations.Contains(annotation)");
            Assert.IsTrue(annotations.Contains(annotation2), "annotations.Contains(annotation)");

            IEnumerable<EdmError> errors;
            Assert.IsTrue(model.Validate(out errors), "validate");

            var sw = new StringWriter();
            var w = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            model.TryWriteSchema(w, out errors);
            w.Close();
            Assert.AreEqual(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <Term Name=""Title"" Type=""Edm.String"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
  </EntityContainer>
  <Annotations Target=""NS1.Customer"">
    <Annotation Term=""NS1.Title"" Qualifier=""q1"" String=""Hello world!"" />
    <Annotation Term=""NS1.Title"" Qualifier=""q2"" String=""Hello world2!"" />
  </Annotations>
</Schema>", sw.ToString(), "model.WriteCsdl(w)");

            model.RemoveVocabularyAnnotation(annotation);
            Assert.IsTrue(model.Validate(out errors), "validate2");
            sw = new StringWriter();
            w = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            model.TryWriteSchema(w, out errors);
            w.Close();
            Assert.AreEqual(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <Term Name=""Title"" Type=""Edm.String"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
  </EntityContainer>
  <Annotations Target=""NS1.Customer"">
    <Annotation Term=""NS1.Title"" Qualifier=""q2"" String=""Hello world2!"" />
  </Annotations>
</Schema>", sw.ToString(), "model.WriteCsdl(w) 2");
        }

        [TestMethod]
        public void CreateSimpleTermAnnotationWithMultiPartNamespace()
        {
            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(CreateModelWithSillyNamespace());

            var term = model.FindTerm("Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Title");
            var customer = model.FindEntityType("Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Customer");

            var annotation = new EdmVocabularyAnnotation(
                customer,
                term,
                "q1",
                new EdmStringConstant("Hello world!"));
            model.WrappedModel.AddVocabularyAnnotation(annotation);

            var annotation2 = new EdmVocabularyAnnotation(
                customer,
                term,
                "q2",
                new EdmStringConstant("Hello world2!"));
            model.WrappedModel.AddVocabularyAnnotation(annotation2);

            var secondterm = model.FindTerm("Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.integerId");
            var customerId = customer.FindProperty("CustomerID");

            var annotation3 = new EdmVocabularyAnnotation(
                customerId,
                secondterm,
                new EdmStringConstant("Hello world3!"));
            model.WrappedModel.AddVocabularyAnnotation(annotation3);

            var annotations = customer.VocabularyAnnotations(model).ToList();
            Assert.AreEqual(2, annotations.Count, "customer.VocabularyAnnotations(model).Count");
            Assert.IsTrue(annotations.Contains(annotation), "annotations.Contains(annotation)");
            Assert.IsTrue(annotations.Contains(annotation2), "annotations.Contains(annotation)");
            var idAnnotations = customerId.VocabularyAnnotations(model).ToList();
            Assert.AreEqual(1, idAnnotations.Count, "customerId.VocabularyAnnotations(model).Count");
            Assert.IsTrue(idAnnotations.Contains(annotation3), "customerId.Contains(annotation)");

            IEnumerable<EdmError> errors;
            Assert.IsTrue(model.Validate(out errors), "validate");

            var sw = new StringWriter();
            var w = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            model.TryWriteSchema(w, out errors);
            w.Close();
            Assert.AreEqual(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <Term Name=""Title"" Type=""Edm.String"" />
  <Term Name=""integerId"" Type=""Edm.String"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Customers"" EntityType=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Customer"" />
  </EntityContainer>
  <Annotations Target=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Customer"">
    <Annotation Term=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Title"" Qualifier=""q1"" String=""Hello world!"" />
    <Annotation Term=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Title"" Qualifier=""q2"" String=""Hello world2!"" />
  </Annotations>
  <Annotations Target=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Customer/CustomerID"">
    <Annotation Term=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.integerId"" String=""Hello world3!"" />
  </Annotations>
</Schema>", sw.ToString(), "model.WriteCsdl(w)");

            model.RemoveVocabularyAnnotation(annotation);
            Assert.IsTrue(model.Validate(out errors), "validate2");
            sw = new StringWriter();
            w = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            model.TryWriteSchema(w, out errors);
            w.Close();
            Assert.AreEqual(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <Term Name=""Title"" Type=""Edm.String"" />
  <Term Name=""integerId"" Type=""Edm.String"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Customers"" EntityType=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Customer"" />
  </EntityContainer>
  <Annotations Target=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Customer"">
    <Annotation Term=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Title"" Qualifier=""q2"" String=""Hello world2!"" />
  </Annotations>
  <Annotations Target=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.Customer/CustomerID"">
    <Annotation Term=""Really.Way.Too.Long.Namespace.With.Lots.Of.Dots.integerId"" String=""Hello world3!"" />
  </Annotations>
</Schema>", sw.ToString(), "model.WriteCsdl(w) 2");
        }

        [TestMethod]
        public void TestAnnotationsWithModelReferencesAnnotationsInTheModel()
        {
            var vocabulary = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(CreateModel());
            vocabulary.RemoveElement(vocabulary.EntityContainer);
            vocabulary.RemoveElement(vocabulary.FindEntityType("NS1.Customer"));
            IEnumerable<EdmError> errors;
            Assert.IsTrue(vocabulary.Validate(out errors), "validate vocabulary");

            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(CreateModel());
            model.RemoveElement(model.FindTerm("NS1.Title"));
            model.RemoveElement(model.FindEntityType("NS1.Person"));
            model.WrappedModel.AddReferencedModel(vocabulary);

            var vterm = vocabulary.FindTerm("NS1.Title");
            var tterm = vocabulary.FindEntityType("NS1.Person");
            var customer = model.FindEntityType("NS1.Customer");

            var vannotation = new EdmVocabularyAnnotation(
                customer,
                vterm,
                "q1",
                new EdmStringConstant("Hello world!"));
            model.WrappedModel.AddVocabularyAnnotation(vannotation);

            var sw = new StringWriter();
            var w = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            model.TryWriteSchema(w, out errors);
            w.Close();
            Assert.AreEqual(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
  </EntityContainer>
  <Annotations Target=""NS1.Customer"">
    <Annotation Term=""NS1.Title"" Qualifier=""q1"" String=""Hello world!"" />
  </Annotations>
</Schema>", sw.ToString(), "model.WriteCsdl(w)");
        }

        [TestMethod]
        public void TestAnnotationsWithModelReferencesAnnotationsInTheVocabulary()
        {
            var vocabulary = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(CreateModel());
            vocabulary.RemoveElement(vocabulary.EntityContainer);
            vocabulary.RemoveElement(vocabulary.FindEntityType("NS1.Customer"));

            var model = new FunctionalUtilities.ModelWithRemovableElements<EdmModel>(CreateModel());
            model.RemoveElement(model.FindTerm("NS1.Title"));
            model.RemoveElement(model.FindEntityType("NS1.Person"));

            vocabulary.WrappedModel.AddReferencedModel(model);
            model.WrappedModel.AddReferencedModel(vocabulary);

            IEnumerable<EdmError> errors;
            Assert.IsTrue(model.Validate(out errors), "validate model");

            var vterm = vocabulary.FindTerm("NS1.Title");
            var tterm = vocabulary.FindEntityType("NS1.Person");
            var customer = model.FindEntityType("NS1.Customer");

            var vannotation = new EdmVocabularyAnnotation(
                customer,
                vterm,
                "q1",
                new EdmStringConstant("Hello world!"));
            vocabulary.WrappedModel.AddVocabularyAnnotation( vannotation);

            Assert.AreEqual(0, model.VocabularyAnnotations.Count(), "model.VocabularyAnnotations.Count()");
            Assert.AreEqual(1, vocabulary.VocabularyAnnotations.Count(), "model.VocabularyAnnotations.Count()");
            var annotations = customer.VocabularyAnnotations(model).ToList();
            Assert.AreEqual(1, annotations.Count, "annotations.Count");
            Assert.IsTrue(annotations.Contains(vannotation), "annotations.Contains(vannotation)");

            var sw = new StringWriter();
            var w = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            model.TryWriteSchema(w, out errors);
            w.Close();
            Assert.AreEqual(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
  </EntityContainer>
</Schema>", sw.ToString(), "model.WriteCsdl(w)");

            sw = new StringWriter();
            w = XmlWriter.Create(sw, new XmlWriterSettings() { Indent = true });
            vocabulary.TryWriteSchema(w, out errors);
            w.Close();
            Assert.AreEqual(
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""Title"" Type=""Edm.String"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <Annotations Target=""NS1.Customer"">
    <Annotation Term=""NS1.Title"" Qualifier=""q1"" String=""Hello world!"" />
  </Annotations>
</Schema>", sw.ToString(), "vocabulary.WriteCsdl(w)");
        }

        private EdmModel CreateModel()
        {
            var model = new EdmModel();

            var customer = new EdmEntityType("NS1", "Customer");
            var customerId = new EdmStructuralProperty(customer, "CustomerID", EdmCoreModel.Instance.GetString(false));
            customer.AddProperty(customerId);
            customer.AddKeys(customerId);
            model.AddElement(customer);

            var title = new EdmTerm("NS1", "Title", EdmCoreModel.Instance.GetString(true));
            model.AddElement(title);

            var person = new EdmEntityType("NS1", "Person");
            var Id = person.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            person.AddKeys(Id);
            model.AddElement(person);

            var container = new EdmEntityContainer("NS1", "Container");
            container.AddElement(new EdmEntitySet(container, "Customers", customer));
            model.AddElement(container);

            IEnumerable<EdmError> errors;
            Assert.IsTrue(model.Validate(out errors), "validate");

            return model;
        }

        private EdmModel CreateModelWithSillyNamespace()
        {
            var model = new EdmModel();

            var customer = new EdmEntityType("Really.Way.Too.Long.Namespace.With.Lots.Of.Dots", "Customer");
            var customerId = customer.AddStructuralProperty("CustomerID", EdmCoreModel.Instance.GetString(false));
            customer.AddKeys(customerId);
            model.AddElement(customer);

            var title = new EdmTerm("Really.Way.Too.Long.Namespace.With.Lots.Of.Dots", "Title", EdmCoreModel.Instance.GetString(true));
            model.AddElement(title);

            var integerId = new EdmTerm("Really.Way.Too.Long.Namespace.With.Lots.Of.Dots", "integerId", EdmCoreModel.Instance.GetString(true));
            model.AddElement(integerId);

            var person = new EdmEntityType("Really.Way.Too.Long.Namespace.With.Lots.Of.Dots", "Person");
            var Id = person.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            person.AddKeys(Id);
            model.AddElement(person);

            var container = new EdmEntityContainer("Really.Way.Too.Long.Namespace.With.Lots.Of.Dots", "Container");
            container.AddEntitySet("Customers", customer);
            model.AddElement(container);

            IEnumerable<EdmError> errors;
            Assert.IsTrue(model.Validate(out errors), "validate");

            return model;
        }
    }
}
