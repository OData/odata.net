//---------------------------------------------------------------------
// <copyright file="EdmVersionTests.cs" company="Microsoft">
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
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EdmVersionTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void CheckCsdlV4RoundTrip()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            VerifyRoundTrip(inputText, EdmConstants.EdmVersion4);
        }

        [TestMethod]
        public void CheckMultiVersionCsdlsRoundtrip()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Real4"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex4"" BaseType=""Real4.Complex1"">
    <Property Name=""Prop4"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            List<StringWriter> outStrings = new List<StringWriter>();
            List<XmlReader> readers = new List<XmlReader>();
            List<XmlWriter> writers = new List<XmlWriter>();

            foreach (string s in new string[] { inputText })
            {
                readers.Add(XmlReader.Create(new StringReader(s)));
            }

            for (int i = 0; i < readers.Count; i++)
            {
                StringWriter sw = new StringWriter();
                outStrings.Add(sw);
                writers.Add(XmlWriter.Create(sw, settings));
            }

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(readers, out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Version check");

            IEnumerator<XmlWriter> writerEnumerator = writers.GetEnumerator();
            model.TryWriteSchema(s => { writerEnumerator.MoveNext(); return writerEnumerator.Current; }, out errors);

            foreach (XmlWriter xw in writers)
            {
                xw.Flush();
                xw.Close();
            }

            IEnumerator<StringWriter> swEnumerator = outStrings.GetEnumerator();

            foreach (string input in new string[] { inputText })
            {
                swEnumerator.MoveNext();
                Assert.IsTrue(swEnumerator.Current.ToString() == input, "Input = Output");
            }
        }

        [TestMethod]
        public void CheckDefaultVersionSerialization()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Version check");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);

            // Make sure it is possible to remove version from the model.
            model.SetEdmVersion(null);
            Assert.IsNull(model.GetEdmVersion(), "Version is null");

            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            string outputText = sw.ToString();

            parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(outputText)) }, out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed 2");
            Assert.IsTrue(errors.Count() == 0, "No errors 2");
            Assert.AreEqual(EdmConstants.EdmVersionDefault, model.GetEdmVersion(), "Version check 2");
        }

        [TestMethod]
        public void CheckNewModelHasNoVersion()
        {
            EdmModel model = new EdmModel();
            EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
            EdmStructuralProperty p1 = t1.AddStructuralProperty("P1", EdmCoreModel.Instance.GetBoolean(false));
            EdmStructuralProperty p2 = t1.AddStructuralProperty("P2", EdmCoreModel.Instance.GetDecimal(1, 1, false));
            EdmStructuralProperty p3 = t1.AddStructuralProperty("P3", EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, 1, false));
            EdmStructuralProperty p4 = t1.AddStructuralProperty("P4", EdmCoreModel.Instance.GetBinary(false, 4, false));
            EdmStructuralProperty p5 = t1.AddStructuralProperty("P5", EdmCoreModel.Instance.GetBinary(false));
            IEdmStructuralProperty q1 = (IEdmStructuralProperty)t1.FindProperty("P1");
            IEdmStructuralProperty q2 = (IEdmStructuralProperty)t1.FindProperty("P2");
            IEdmStructuralProperty q3 = (IEdmStructuralProperty)t1.FindProperty("P3");
            IEdmStructuralProperty q4 = (IEdmStructuralProperty)t1.FindProperty("P4");
            IEdmStructuralProperty q5 = (IEdmStructuralProperty)t1.FindProperty("P5");
            model.AddElement(t1);

            Assert.IsNull(model.GetEdmVersion(), "Version is null");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            IEnumerable<EdmError> errors;
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            string outputText = sw.ToString();

            IEdmModel iEdmModel;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(outputText)) }, out iEdmModel, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            Assert.AreEqual(EdmConstants.EdmVersionDefault, iEdmModel.GetEdmVersion(), "Version check");
        }

        [TestMethod]
        public void CheckEdmxVersionRoundtrip()
        {
            const string edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
          <Key>
            <PropertyRef Name=""CustomerID"" />
          </Key>
          <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No Errors");
            Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Model version check 1");
            Assert.AreEqual(CsdlConstants.EdmxVersion4, model.GetEdmxVersion(), "EDMX version check 1");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            using (XmlWriter xw = XmlWriter.Create(sw, settings))
            {
                CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
                xw.Close();

                parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(sw.ToString())), out model, out errors);
                Assert.IsTrue(parsed, "Model Parsed");
                Assert.IsTrue(errors.Count() == 0, "No Errors");
                Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Model version check 2");
                Assert.AreEqual(CsdlConstants.EdmxVersion4, model.GetEdmxVersion(), "EDMX version check 2");
            }
        }

        [TestMethod]
        public void CheckEdmxVersionChange()
        {
            const string edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
          <Key>
            <PropertyRef Name=""CustomerID"" />
          </Key>
          <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No Errors");
            Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Model version check 1");
            Assert.AreEqual(CsdlConstants.EdmxVersion4, model.GetEdmxVersion(), "EDMX version check 1");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            using (XmlWriter xw = XmlWriter.Create(sw, settings))
            {
                model.SetEdmxVersion(CsdlConstants.EdmxVersion4);
                CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
                xw.Close();

                parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(sw.ToString())), out model, out errors);
                Assert.IsTrue(parsed, "Model Parsed");
                Assert.IsTrue(errors.Count() == 0, "No Errors");
                Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Model version check 2");
                Assert.AreEqual(CsdlConstants.EdmxVersion4, model.GetEdmxVersion(), "EDMX version check 2");
            }
        }

        [TestMethod]
        public void CheckEdmxVersionInference()
        {
            const string edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
          <Key>
            <PropertyRef Name=""CustomerID"" />
          </Key>
          <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No Errors");
            Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Model version check 1");
            Assert.AreEqual(CsdlConstants.EdmxVersion4, model.GetEdmxVersion(), "EDMX version check 1");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            using (XmlWriter xw = XmlWriter.Create(sw, settings))
            {
                model.SetEdmxVersion(CsdlConstants.EdmxVersion4);
                CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
                xw.Close();

                parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(sw.ToString())), out model, out errors);
                Assert.IsTrue(parsed, "Model Parsed");
                Assert.IsTrue(errors.Count() == 0, "No Errors");
                Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Model version check 2");
                Assert.AreEqual(CsdlConstants.EdmxVersion4, model.GetEdmxVersion(), "EDMX version check 2");
            }
        }
        
        [TestMethod]
        public void CheckEdmxVersionMismatch()
        {
            // Edmx namespace is for v 2.0 != Version="4.0"
            const string edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""2.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
          <Key>
            <PropertyRef Name=""CustomerID"" />
          </Key>
          <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out errors);
            Assert.IsFalse(parsed, "Model failed to parse");
            Assert.IsTrue(errors.Count() == 1, "1 Error");
            Assert.AreEqual(EdmErrorCode.InvalidVersionNumber, errors.First().ErrorCode, "Error code check");
        }

        [TestMethod]
        public void CheckEdmxUnsupportedVersion()
        {
            const string edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
          <Key>
            <PropertyRef Name=""CustomerID"" />
          </Key>
          <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No Errors");
            Assert.AreEqual(EdmConstants.EdmVersion4, model.GetEdmVersion(), "Model version check 1");
            Assert.AreEqual(CsdlConstants.EdmxVersion4, model.GetEdmxVersion(), "EDMX version check 1");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            using (XmlWriter xw = XmlWriter.Create(sw, settings))
            {
                model.SetEdmxVersion(new Version(1, 123));
                try
                {
                    CsdlWriter.TryWriteCsdl(model, xw, CsdlTarget.OData, out errors);
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e is InvalidOperationException, "e is InvalidOperationException");
                }
            }
        }

        void VerifyRoundTrip(string inputText, Version expectedEdmVersion)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            Assert.AreEqual(expectedEdmVersion, model.GetEdmVersion(), "Version check");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            string outputText = sw.ToString();
            Assert.AreEqual(inputText, outputText, "Input = Output");
        }
    }
}
