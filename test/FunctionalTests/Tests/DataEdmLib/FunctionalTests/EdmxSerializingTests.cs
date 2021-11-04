//---------------------------------------------------------------------
// <copyright file="EdmxSerializingTests.cs" company="Microsoft">
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
    public class EdmxSerializingTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void SerializeSimpleSchemaAsEFEdmx()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
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
</Schema>";

            const string expectedResult =
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

            VerifyResult(inputText, expectedResult, CsdlTarget.EntityFramework);
        }

        [TestMethod]
        public void SerializeMultipleCsdlInEFEdmx()
        {
            const string inputText1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
            const string inputText2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
  </ComplexType>
</Schema>";

            const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Smod"">
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
      </Schema>
      <Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
          <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
          <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
        </ComplexType>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

            VerifyResult(new string[] { inputText1, inputText2 }, expectedResult, CsdlTarget.EntityFramework);
        }

        [TestMethod]
        public void SerializeSimpleSchemaAsODataEdmx()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
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
</Schema>";

            const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
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
  </edmx:DataServices>
</edmx:Edmx>";

            VerifyResult(inputText, expectedResult, CsdlTarget.OData);
        }

        [TestMethod]
        public void SerializeMultipleCsdlInODataEdmx()
        {
            const string inputText1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
            const string inputText2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
  </ComplexType>
</Schema>";

            const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Smod"">
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
      </ComplexType>
    </Schema>
    <Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
        <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
      </ComplexType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            VerifyResult(new string[] { inputText1, inputText2 }, expectedResult, CsdlTarget.OData);
        }

        [TestMethod]
        public void TestEntityContainerSchemaNamespace()
        {
            const string inputText1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Smod"">
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
        <EntityType Name=""Blot"">
          <Key>
            <PropertyRef Name=""Id"" />
          </Key>
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </EntityType>
      </Schema>";

            const string inputText2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
      <Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
          <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
          <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
        </ComplexType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Blots"" EntityType=""Grumble.Blot"" />
        </EntityContainer>
      </Schema>";

            const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Smod"">
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
        <EntityType Name=""Blot"">
          <Key>
            <PropertyRef Name=""Id"" />
          </Key>
          <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        </EntityType>
      </Schema>
      <Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
          <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""1024"" />
          <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
        </ComplexType>
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Blots"" EntityType=""Grumble.Blot"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

            VerifyResult(new string[] { inputText1, inputText2 }, expectedResult, CsdlTarget.EntityFramework,
                model =>
                {
                    Assert.AreEqual("Mumble", model.EntityContainer.Namespace, "GetEntityContainerSchemaNamespace check");
                });
        }

        [TestMethod]
        public void TestEntityContainerPreservingNamespaceOfEmptySchema()
        {
            const string inputText1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Blots"" EntityType=""Grumble.Blot"" />
        </EntityContainer>
      </Schema>";

            const string expectedResult =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""C1"">
          <EntitySet Name=""Blots"" EntityType=""Grumble.Blot"" />
        </EntityContainer>
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";

            VerifyResult(new string[] { inputText1 }, expectedResult, CsdlTarget.EntityFramework,
                model =>
                {
                    Assert.AreEqual("Grumble", model.EntityContainer.Namespace, "GetEntityContainerSchemaNamespace check");
                });
        }

        [TestMethod]
        public void TestNullArgChecks()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
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
  </edmx:DataServices>
</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            CsdlReader.TryParse(XmlReader.Create(new StringReader(inputText)), out model, out errors);

            try
            {
                CsdlWriter.TryWriteCsdl(null, XmlWriter.Create(new StringWriter()), CsdlTarget.EntityFramework, out errors);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentNullException, "e is ArgumentNullException");
            }
            try
            {
                CsdlWriter.TryWriteCsdl(model, null, CsdlTarget.EntityFramework, out errors);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentNullException, "e is ArgumentNullException");
            }
        }

        void VerifyResult(string inputText, string expectedResult, CsdlTarget target)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No Errors");

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            CsdlWriter.TryWriteCsdl(model, xw, target, out errors);
            xw.Flush();
            xw.Close();
            string outputText = sw.ToString();
            Assert.AreEqual(expectedResult, outputText, "Expected Result = Output");
        }

        void VerifyResult(string[] inputText, string expectedResult, CsdlTarget target)
        {
            VerifyResult(inputText, expectedResult, target, null);
        }

        void VerifyResult(string[] inputText, string expectedResult, CsdlTarget target, Action<IEdmModel> visitModel)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            List<XmlReader> readers = new List<XmlReader>();
            foreach (string s in inputText)
            {
                readers.Add(XmlReader.Create(new StringReader(s)));
            }
            bool parsed = SchemaReader.TryParse(readers, out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No Errors");

            if (visitModel != null)
            {
                visitModel(model);
            }

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            CsdlWriter.TryWriteCsdl(model, xw, target, out errors);
            xw.Flush();
            xw.Close();
            string outputText = sw.ToString();
            Assert.AreEqual(expectedResult, outputText, "Expected Result = Output");
        }
    }
}
