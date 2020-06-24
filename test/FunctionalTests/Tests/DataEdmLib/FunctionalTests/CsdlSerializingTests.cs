//---------------------------------------------------------------------
// <copyright file="CsdlSerializingTests.cs" company="Microsoft">
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
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsdlSerializingTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void SerializeSimpleComplexTypeSchema()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeSimpleEntityTypeSchema()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Smod"" OpenType=""true"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeSimpleModel()
        {
            var inputText =
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
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeInheritanceComplexTypeSchema()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"" Abstract=""true"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
  <ComplexType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Other"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeInheritanceEntityTypeSchema()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Smod"" OpenType=""true"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Clod"" BaseType=""Grumble.Smod"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" MaxLength=""128"" Unicode=""false"" />
  </EntityType>
  <EntityType Name=""Blod"" BaseType=""Grumble.Clod"" OpenType=""true"">
    <Property Name=""Nameee"" Type=""Edm.String"" Nullable=""false"" MaxLength=""128"" Unicode=""false"" />
  </EntityType>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeAllPrimitiveTypesAndFacets()
        {
            //MATHEWL TODO: Inclusion of Default Facet causes parser to have errors. Check if this is intended behavior
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
    <Property Name=""myDate"" Type=""Edm.Date"" />
    <Property Name=""myBinary"" Type=""Edm.Binary"" MaxLength=""64"" />
    <Property Name=""myBinaryMax"" Type=""Edm.Binary"" MaxLength=""max"" />
    <Property Name=""myBoolean"" Type=""Edm.Boolean"" />
    <Property Name=""myDateTime"" Type=""Edm.DateTimeOffset"" Precision=""2"" />
    <Property Name=""myPrecision"" Type=""Edm.Duration"" Precision=""3"" />
    <Property Name=""myDateTimeOffset"" Type=""Edm.DateTimeOffset"" Precision=""1"" />
    <Property Name=""myDecimal"" Type=""Edm.Decimal"" DefaultValue=""3.5"" Precision=""3"" Scale=""2"" />
    <Property Name=""myFacetlessDecimal"" Type=""Edm.Decimal"" />
    <Property Name=""mySingle"" Type=""Edm.Single"" Nullable=""false"" />
    <Property Name=""myDouble"" Type=""Edm.Double"" Nullable=""false"" />
    <Property Name=""myGuid"" Type=""Edm.Guid"" />
    <Property Name=""mySByte"" Type=""Edm.SByte"" />
    <Property Name=""myInt16"" Type=""Edm.Int16"" />
    <Property Name=""myInt64"" Type=""Edm.Int64"" />
    <Property Name=""myByte"" Type=""Edm.Byte"" />
    <Property Name=""myStream"" Type=""Edm.Stream"" />
    <Property Name=""myString"" Type=""Edm.String"" DefaultValue=""BorkBorkBork"" MaxLength=""128"" Unicode=""false"" />
    <Property Name=""myStringMax"" Type=""Edm.String"" MaxLength=""max"" Unicode=""false"" />
    <Property Name=""myTimeOfDay"" Type=""Edm.TimeOfDay"" Precision=""4"" />
  </ComplexType>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeComplexCollectionProperty()
        {
            //MATHEWL TODO: Inclusion of Default Facet causes parser to have errors. Check if this is intended behavior
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Foo"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
  </ComplexType>
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
    <Property Name=""myBinary"" Type=""Collection(Edm.Binary)"" MaxLength=""64"" />
    <Property Name=""myBinaryMax"" Type=""Collection(Edm.Binary)"" MaxLength=""max"" />
    <Property Name=""myBoolean"" Type=""Collection(Edm.Boolean)"" />
    <Property Name=""myEnum"" Type=""Collection(Grumble.Color)"" />
    <Property Name=""myComplex"" Type=""Collection(Grumble.Foo)"" />
  </ComplexType>
  <EnumType Name=""Color"" IsFlags=""true"">
    <Member Name=""Red"" />
    <Member Name=""Green"" />
    <Member Name=""Blue"" />
    <Member Name=""Yellow"" />
  </EnumType>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeNonNullableCollectionProperty()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
    <Property Name=""myStrings"" Type=""Collection(Edm.String)"" Nullable=""false"" />
  </ComplexType>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeAllSpatialPrimitiveTypesAndFacets()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
    <Property Name=""myGeography"" Type=""Edm.Geography"" SRID=""1"" />
    <Property Name=""myPoint"" Type=""Edm.GeographyPoint"" SRID=""2"" />
    <Property Name=""myLineString"" Type=""Edm.GeographyLineString"" SRID=""3"" />
    <Property Name=""myPolygon"" Type=""Edm.GeographyPolygon"" SRID=""4"" />
    <Property Name=""myGeographyCollection"" Type=""Edm.GeographyCollection"" SRID=""5"" />
    <Property Name=""myMultiPolygon"" Type=""Edm.GeographyMultiPolygon"" SRID=""6"" />
    <Property Name=""myMultiLineString"" Type=""Edm.GeographyMultiLineString"" SRID=""7"" />
    <Property Name=""myMultiPoint"" Type=""Edm.GeographyMultiPoint"" SRID=""8"" />
    <Property Name=""myGeometry"" Type=""Edm.Geometry"" SRID=""9"" />
    <Property Name=""myGeometricPoint"" Type=""Edm.GeometryPoint"" SRID=""10"" />
    <Property Name=""myGeometricLineString"" Type=""Edm.GeometryLineString"" SRID=""11"" />
    <Property Name=""myGeometricPolygon"" Type=""Edm.GeometryPolygon"" SRID=""12"" />
    <Property Name=""myGeometryCollection"" Type=""Edm.GeometryCollection"" SRID=""13"" />
    <Property Name=""myGeometricMultiPolygon"" Type=""Edm.GeometryMultiPolygon"" SRID=""14"" />
    <Property Name=""myGeometricMultiLineString"" Type=""Edm.GeometryMultiLineString"" SRID=""15"" />
    <Property Name=""myGeometricMultiPoint"" Type=""Edm.GeometryMultiPoint"" SRID=""Variable"" />
  </ComplexType>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeEntityContainer()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""100"" />
    <NavigationProperty Name=""ToPet"" Type=""Hot.Pet"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Wild"">
    <EntitySet Name=""People"" EntityType=""Hot.Person"">
      <NavigationPropertyBinding Path=""Pet"" Target=""Pets"" />
    </EntitySet>
    <EntitySet Name=""Pets"" EntityType=""Hot.Pet"" />
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeSingleton()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""100"" />
    <NavigationProperty Name=""ToPet"" Type=""Hot.Pet"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Wild"">
    <EntitySet Name=""People"" EntityType=""Hot.Person"">
      <NavigationPropertyBinding Path=""Pet"" Target=""Pets"" />
    </EntitySet>
    <Singleton Name=""SingletonPeople"" Type=""Hot.Person"">
      <NavigationPropertyBinding Path=""Pet"" Target=""Pets"" />
    </Singleton>
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(inputText);
        }


        [TestMethod]
        public void SerializeNavigationProperties()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Cold"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Feckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""Ego"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Ego"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Bonus"" Type=""Edm.String"" MaxLength=""1001"" />
    <NavigationProperty Name=""MyReckless"" Type=""Cold.Reckless"" Nullable=""false"" Partner=""MyFecklesses"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
  </EntityType>
  <EntityType Name=""Reckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""AlterEgo"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""AlterEgo"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""MyFecklesses"" Type=""Collection(Cold.Feckless)"" Partner=""MyReckless"" ContainsTarget=""true"">
      <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
      <ReferentialConstraint Property=""AlterEgo"" ReferencedProperty=""Ego"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeMultipleFiles()
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
            VerifyRoundTrip(new string[] { inputText1, inputText2 });
        }

        [TestMethod]
        public void SerializeMultipleFilesTypesInProperties()
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
  <ComplexType Name=""Clod"">
    <Property Name=""Smod"" Type=""Grumble.Smod"" Nullable=""false"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
  </ComplexType>
</Schema>";
            VerifyRoundTrip(new string[] { inputText1, inputText2 });
        }

        [TestMethod]
        public void SerializeMultipleFilesDottedNamespace()
        {
            const string inputText1 = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble.Fumble"" Alias=""Fumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
            const string inputText2 = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Mumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Clod"">
    <Property Name=""Smod"" Type=""Fumble.Smod"" Nullable=""false"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""2048"" />
  </ComplexType>
</Schema>";
            VerifyRoundTrip(new string[] {inputText1, inputText2});
        }

        [TestMethod]
        public void SerializeFunctions()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Fred"">
    <Property Name=""FredProp"" Type=""Edm.Int32"" />
  </EntityType>
  <Function Name=""DoIt1"">
    <Parameter Name=""P1"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""DoIt2"">
    <Parameter Name=""P1"" Type=""Ref(Grumble.Fred)"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""DoIt3"">
    <ReturnType Type=""Ref(Grumble.Fred)"" />
  </Function>
  <Function Name=""DoIt6"">
    <Parameter Name=""P1"" Type=""Edm.Int32"" />
    <Parameter Name=""P2"" Type=""Edm.Int32"" />
    <Parameter Name=""P3"" Type=""Edm.Int32"" />
    <Parameter Name=""P4"" Type=""Edm.Int32"" />
    <Parameter Name=""P5"" Type=""Edm.Int32"" />
    <Parameter Name=""P6"" Type=""Edm.Int32"" />
    <Parameter Name=""P7"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""DoIt6"">
    <Parameter Name=""P1"" Type=""Edm.Int32"" />
    <Parameter Name=""P2"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeFunctionImports()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
    <NavigationProperty Name=""Orders"" Type=""Collection(Grumble.Order)"" />
  </EntityType>
  <EntityType Name=""Order"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
    <NavigationProperty Name=""Customer"" Type=""Grumble.Customer"" Nullable=""false"" />
  </EntityType>
  <Action Name=""GetCustomers"">
    <Parameter Name=""P1"" Type=""Edm.Int32"" />
    <Parameter Name=""P2"" Type=""Edm.Int32"" />
    <Parameter Name=""P3"" Type=""Edm.Int32"" Nullable=""false"" />
    <ReturnType Type=""Collection(Grumble.Customer)"" />
  </Action>
  <Action Name=""GetBestCustomerID"">
    <ReturnType Type=""Edm.Int32"" />
  </Action>
  <Function Name=""peopleWhoAreAwesome"" IsBound=""true"" EntitySetPath=""Persons/"" IsComposable=""true"">
    <Parameter Name=""Persons"" Type=""Collection(Grumble.Customer)"" />
    <ReturnType Type=""Collection(Grumble.Customer)"" />
  </Function>
  <Function Name=""peopleWhoAreAwesome"" IsComposable=""true"">
    <Parameter Name=""Persons"" Type=""Collection(Grumble.Customer)"" />
    <ReturnType Type=""Collection(Grumble.Order)"" />
  </Function>
  <Function Name=""peopleWhoAreAwesome"" IsBound=""true"" EntitySetPath=""Purchases/Customer"" IsComposable=""true"">
    <Parameter Name=""Purchases"" Type=""Collection(Grumble.Order)"" />
    <ReturnType Type=""Collection(Grumble.Customer)"" />
  </Function>
  <Function Name=""peopleWhoAreAwesome"" IsComposable=""true"">
    <Parameter Name=""Purchases"" Type=""Collection(Grumble.Order)"" />
    <ReturnType Type=""Collection(Grumble.Order)"" />
  </Function>
  <Function Name=""peopleWhoAreAwesome"">
    <Parameter Name=""Persons"" Type=""Collection(Grumble.Customer)"" />
    <ReturnType Type=""Collection(Grumble.Customer)"" />
  </Function>
  <EntityContainer Name=""C1"">
    <EntitySet Name=""Customers"" EntityType=""Grumble.Customer"">
      <NavigationPropertyBinding Path=""Orders"" Target=""Orders"" />
    </EntitySet>
    <EntitySet Name=""Orders"" EntityType=""Grumble.Order"">
      <NavigationPropertyBinding Path=""Customer"" Target=""Customers"" />
    </EntitySet>
    <ActionImport Name=""GetCustomers"" Action=""Grumble.GetCustomers"" EntitySet=""Customers"" />
    <ActionImport Name=""GetBestCustomerID"" Action=""Grumble.GetBestCustomerID"" />
    <FunctionImport Name=""peopleWhoAreAwesome"" Function=""Grumble.peopleWhoAreAwesome"" />
    <FunctionImport Name=""peopleWhoAreAwesome"" Function=""Grumble.peopleWhoAreAwesome"" EntitySet=""Customers"" />
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(inputText,
                model =>
                {
                    var pp = (model.EntityContainer.FindOperationImports("GetCustomers")).Single().Operation.Parameters.ToArray();
                    Assert.IsTrue(pp[0].Type.IsNullable, "P1 is nullable");
                    Assert.IsTrue(pp[1].Type.IsNullable, "P2 is nullable");
                    Assert.IsFalse(pp[2].Type.IsNullable, "P3 is not nullable");
                },
                () => inputText.Replace(@" Nullable=""true""", ""));
        }

        [TestMethod]
        public void SerializeWithoutSchemaElements()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""C1"">
    <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    <EntitySet Name=""People"" EntityType=""NS1.People"" />
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void FailSerializingTwoSchemasToOneFile()
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

            var inputText = new string[] { inputText1, inputText2 };
            List<StringWriter> outStrings = new List<StringWriter>();
            List<XmlReader> readers = new List<XmlReader>();
            foreach (string s in inputText)
            {
                readers.Add(XmlReader.Create(new StringReader(s)));
            }
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(readers, out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            using (XmlWriter xw = XmlWriter.Create(sw, settings))
            {
                IEnumerable<EdmError> expectedSerializationErrors = new EdmLibTestErrors() 
                {
                    {0, 0, EdmErrorCode.SingleFileExpected},
                };
                IEnumerable<EdmError> actualSerializationErrors;
                model.TryWriteSchema(xw, out actualSerializationErrors);
                CompareErrors(actualSerializationErrors, expectedSerializationErrors);
            }
        }

        [TestMethod]
        public void SerializeCollectionReturnTypesRegressionTest()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""MyEntityType"">
    <Key>
      <PropertyRef Name=""Property1"" />
    </Key>
    <Property Name=""Property1"" Type=""Edm.String"" Nullable=""false"" />
  </EntityType>
  <Function Name=""MyFunction"">
    <ReturnType Type=""Collection(MyNamespace.MyEntityType)"" />
  </Function>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void CyclicBaseTypeAliasingRegressionTest()
        {
            const string inputText1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Real1"" Alias=""Real1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex1"" BaseType=""Real2.Complex2"">
    <Property Name=""Prop1"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            const string inputText2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Real2"" Alias=""Real2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex2"" BaseType=""Real3.Complex3"">
    <Property Name=""Prop2"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";

            const string inputText3 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Real3"" Alias=""Real3"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex3"" BaseType=""Real1.Complex1"">
    <Property Name=""Prop3"" Type=""Edm.Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>";
            VerifyRoundTrip(new string[] { inputText1, inputText2, inputText3 });
        }


        [TestMethod]
        public void SerializeCollection()
        {
            // This would not pass validation, but still verifies that Collection() roundtrips properly
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
    <Property Name=""Collection"" Type=""Collection(Edm.Int32)"" />
  </ComplexType>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeEnumType()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""c1"" Type=""foo.Color"" />
    <Property Name=""c2"" Type=""foo.Color2"" Nullable=""false"" />
  </EntityType>
  <EnumType Name=""Color"" IsFlags=""true"">
    <Member Name=""Red"" />
    <Member Name=""Green"" />
    <Member Name=""Blue"" />
    <Member Name=""Yellow"" />
  </EnumType>
  <EnumType Name=""Color2"" UnderlyingType=""Edm.Int64"">
    <Member Name=""Red"" />
    <Member Name=""Green"" />
    <Member Name=""Blue"" />
    <Member Name=""Yellow"" />
  </EnumType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeEnumTypeWithValues()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""c1"" Type=""foo.Color"" />
    <Property Name=""c2"" Type=""foo.Color2"" Nullable=""false"" />
  </EntityType>
  <EnumType Name=""Color"" IsFlags=""true"">
    <Member Name=""Red"" Value=""1"" />
    <Member Name=""Green"" Value=""2"" />
    <Member Name=""Blue"" />
    <Member Name=""Yellow"" />
  </EnumType>
  <EnumType Name=""Color2"" UnderlyingType=""Edm.Int64"">
    <Member Name=""Red"" />
    <Member Name=""Green"" />
    <Member Name=""Blue"" Value=""15"" />
    <Member Name=""Yellow"" Value=""23"" />
  </EnumType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeTerm()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <Term Name=""Term1"" Type=""Edm.Int32"" Nullable=""false"" />
  <Term Name=""Term2"" Type=""Collection(Edm.Int32)"" />
  <Term Name=""Term3"" Type=""Ref(foo.Person)"" />
  <Term Name=""Term5"" Type=""Edm.String"" MaxLength=""128"" Unicode=""false"" />
</Schema>";

            VerifyRoundTrip(inputText);
        }
        
        [TestMethod]
        public void SerializeWithCoreVocabularyTerm()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Annotation Term=""Org.OData.Core.V1.OptimisticConcurrency"">
      <Collection>
        <PropertyPath>Concurrency</PropertyPath>
      </Collection>
    </Annotation>
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeTypeTerm()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""LyingPerson"" BaseType=""Edm.TypeTerm"">
    <Property Name=""Sobriquet"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""FictionalAge"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeVocabularyAnnotation()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
    <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
    <Annotation Term=""foo.Age"" Qualifier=""Best"" Int=""456"" />
    <Annotation Term=""Funk.Mage"" Int=""789"" />
  </EntityType>
  <Term Name=""Age"" Type=""Edm.Int32"" />
  <Term Name=""Subject"" Type=""foo.Person"" />
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeOutOfLineVocabularyAnnotation()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
  </EntityType>
  <Term Name=""Age"" Type=""Edm.Int32"" />
  <Term Name=""Subject"" Type=""foo.Person"" />
  <EntityContainer Name=""C1"">
    <EntitySet Name=""People"" EntityType=""foo.Person"" />
  </EntityContainer>
  <Annotations Target=""foo.Person"">
    <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
    <Annotation Term=""foo.Age"" Qualifier=""Best"" Int=""456"" />
    <Annotation Term=""Funk.Mage"" Int=""789"" />
  </Annotations>
  <Annotations Target=""foo.Person/Birthday"">
    <Annotation Term=""Funk.Mage"" Int=""101"" />
  </Annotations>
  <Annotations Target=""foo.C1/People"">
    <Annotation Term=""Funk.Mage"" Int=""117"" />
  </Annotations>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeOutOfLineVocabularyAnnotationOnExtendedTargets()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
  </EntityType>
  <Term Name=""Age"" Type=""Edm.Int32"" />
  <Term Name=""Subject"" Type=""foo.Person"" />
  <Function Name=""Func"">
    <Parameter Name=""P1"" Type=""Edm.Int32"" />
    <Parameter Name=""P2"" Type=""Edm.Int32"" />
    <Parameter Name=""P3"" Type=""Edm.Int32"" />
    <Parameter Name=""P4"" Type=""Edm.Int32"" />
    <Parameter Name=""P5"" Type=""Edm.Int32"" />
    <Parameter Name=""P6"" Type=""Edm.Int32"" />
    <Parameter Name=""P7"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Function Name=""Func"">
    <Parameter Name=""P1"" Type=""Edm.Int16"" />
    <Parameter Name=""P2"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.Int32"" />
  </Function>
  <Action Name=""GetCustomers"">
    <Parameter Name=""P1"" Type=""Edm.Int32"" />
    <Parameter Name=""P2"" Type=""Edm.Int32"" />
    <Parameter Name=""P3"" Type=""Edm.Int32"" Nullable=""false"" />
    <ReturnType Type=""Collection(Grumble.Customer)"" />
  </Action>
  <EntityContainer Name=""C1"">
    <EntitySet Name=""People"" EntityType=""foo.Person"" />
    <ActionImport Name=""GetCustomers"" Action=""foo.GetCustomers"" EntitySet=""People"" />
  </EntityContainer>
  <Annotations Target=""foo.Person"">
    <Annotation Term=""foo.Age"" Qualifier=""First"" Int=""123"" />
    <Annotation Term=""foo.Age"" Qualifier=""Best"" Int=""456"" />
    <Annotation Term=""Funk.Mage"" Int=""789"" />
  </Annotations>
  <Annotations Target=""foo.Person/Birthday"">
    <Annotation Term=""Funk.Mage"" Int=""101"" />
  </Annotations>
  <Annotations Target=""foo.C1/People"">
    <Annotation Term=""Funk.Mage"" Int=""117"" />
  </Annotations>
  <Annotations Target=""foo.Func(Edm.Int32, Edm.Int32, Edm.Int32, Edm.Int32, Edm.Int32, Edm.Int32, Edm.Int32)"">
    <Annotation Term=""Funk.Mage"" Int=""1337"" />
  </Annotations>
  <Annotations Target=""foo.Func(Edm.Int16, Edm.String)"">
    <Annotation Term=""Funk.Mage"" Int=""1338"" />
  </Annotations>
  <Annotations Target=""foo.Func(Edm.Int32, Edm.Int32, Edm.Int32, Edm.Int32, Edm.Int32, Edm.Int32, Edm.Int32)/P1"">
    <Annotation Term=""Funk.Mage"" Int=""1339"" />
  </Annotations>
  <Annotations Target=""foo.Func(Edm.Int16, Edm.String)/P2"">
    <Annotation Term=""Funk.Mage"" Int=""1340"" />
  </Annotations>
  <Annotations Target=""foo.C1/GetCustomers(Edm.Int32, Edm.Int32, Edm.Int32)/P1"">
    <Annotation Term=""Funk.Mage"" Int=""1341"" />
  </Annotations>
  <Annotations Target=""foo.C1/GetCustomers(Edm.Int32, Edm.Int32, Edm.Int32)"">
    <Annotation Term=""Funk.Mage"" Int=""1342"" />
  </Annotations>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeConstantExpression()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
    <Annotation Term=""Funk.IC"" Qualifier=""In"" Int=""1"" />
    <Annotation Term=""Funk.IC"" Qualifier=""Out"" Int=""2"" />
    <Annotation Term=""Funk.SC"" Qualifier=""In"" String=""Cat"" />
    <Annotation Term=""Funk.SC"" Qualifier=""Out"" String=""Dog"" />
    <Annotation Term=""Funk.BinC"" Qualifier=""In"" Binary=""00FF1122"" />
    <Annotation Term=""Funk.BinC"" Qualifier=""Out"" Binary=""0FF02113"" />
    <Annotation Term=""Funk.FC"" Qualifier=""In"" Float=""1.1"" />
    <Annotation Term=""Funk.FC"" Qualifier=""Out"" Float=""22000000000"" />
    <Annotation Term=""Funk.GC"" Qualifier=""In"" Guid=""4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2"" />
    <Annotation Term=""Funk.GC"" Qualifier=""Out"" Guid=""4ae71c81-c21a-40a2-8d53-f1a29ed4a2f3"" />
    <Annotation Term=""Funk.DC"" Qualifier=""In"" Decimal=""1.2"" />
    <Annotation Term=""Funk.DC"" Qualifier=""Out"" Decimal=""2.3"" />
    <Annotation Term=""Funk.BC"" Qualifier=""In"" Bool=""true"" />
    <Annotation Term=""Funk.BC"" Qualifier=""Out"" Bool=""false"" />
    <Annotation Term=""Funk.DTOfsC"" Qualifier=""In"" DateTimeOffset=""2001-10-26T19:32:52+01:00"" />
    <Annotation Term=""Funk.DTOfsC"" Qualifier=""Out"" DateTimeOffset=""2001-10-26T19:32:52+01:00"" />
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeRecordExpression()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
    <Annotation Term=""Funk.RC"">
      <Record>
        <PropertyValue Property=""X"" Int=""10"" />
        <PropertyValue Property=""Y"" Int=""20"" />
      </Record>
    </Annotation>
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeIfExpression()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
    <Annotation Term=""Funk.RC"">
      <If>
        <Bool>true</Bool>
        <Int>123</Int>
        <Float>3.14</Float>
      </If>
    </Annotation>
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeOutOfLineConstantExpressions()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
    <Annotation Term=""Funk.IC"" Qualifier=""In"">
      <If>
        <Bool>true</Bool>
        <Int>1</Int>
        <Int>2</Int>
      </If>
    </Annotation>
    <Annotation Term=""Funk.SC"" Qualifier=""In"">
      <If>
        <Bool>true</Bool>
        <String>Cat</String>
        <String>Dog</String>
      </If>
    </Annotation>
    <Annotation Term=""Funk.BinC"" Qualifier=""In"">
      <If>
        <Bool>true</Bool>
        <String>123456</String>
        <String>654321</String>
      </If>
    </Annotation>
    <Annotation Term=""Funk.FC"" Qualifier=""In"">
      <If>
        <Bool>true</Bool>
        <Float>1.1</Float>
        <Float>22000000000</Float>
      </If>
    </Annotation>
    <Annotation Term=""Funk.GC"" Qualifier=""In"">
      <If>
        <Bool>true</Bool>
        <Guid>4ae71c81-c21a-40a2-8d53-f1a29ed4a2f2</Guid>
        <Guid>4ae71c81-c21a-40a2-8d53-f1a29ed4a2f3</Guid>
      </If>
    </Annotation>
    <Annotation Term=""Funk.DC"" Qualifier=""In"">
      <If>
        <Bool>true</Bool>
        <Decimal>1.2</Decimal>
        <Decimal>2.3</Decimal>
      </If>
    </Annotation>
    <Annotation Term=""Funk.DTOfsC"" Qualifier=""In"">
      <If>
        <Bool>true</Bool>
        <DateTimeOffset>2001-10-26T19:32:52Z</DateTimeOffset>
        <DateTimeOffset>2001-10-26T19:32:52Z</DateTimeOffset>
      </If>
    </Annotation>
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializePathExpressions()
        {
            const string inputText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Age"" Type=""Edm.Int32"" />
    <Annotation Term=""Funk.Mage"" Path=""Age"" />
  </EntityType>
  <EntityType Name=""LyingPerson"" BaseType=""Edm.TypeTerm"">
    <Property Name=""Sobriquet"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""FictionalAge"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <Term Name=""Age"" Type=""Edm.Int32"" />
  <Term Name=""Subject"" Type=""foo.Person"" />
  <Annotations Target=""foo.Person"">
    <Annotation Term=""foo.Age"">
      <If>
        <Bool>true</Bool>
        <Path>Age</Path>
        <Path>Age</Path>
      </If>
    </Annotation>
  </Annotations>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeApplyFunctionExpressions()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""CoolPersonTerm"" Type=""Funk.CoolPerson"" />
  <Annotations Target=""foo.Person"">
    <Annotation Term=""foo.CoolPersonTerm"">
      <Record>
        <PropertyValue Property=""Street"">
          <Apply Function=""Functions.StringConcat"">
            <Apply Function=""Functions.StringConcat"">
              <Apply Function=""Functions.IntegerToString"">
                <Path>Address/Number</Path>
              </Apply>
              <String>/</String>
            </Apply>
            <Apply Function=""Functions.StringConcat"">
              <Apply Function=""Functions.StringConcat"">
                <Path>Address/Street</Path>
                <Apply Function=""Functions.StringConcat"">
                  <String>/</String>
                  <Path>Address/City</Path>
                </Apply>
              </Apply>
              <Apply Function=""Functions.StringConcat"">
                <String>/</String>
                <Path>Address/State</Path>
              </Apply>
            </Apply>
          </Apply>
        </PropertyValue>
      </Record>
    </Annotation>
  </Annotations>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeIsTypeExpressions()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""foo.Person"">
    <Annotation Term=""Funk.Punning"">
      <IsType Type=""Edm.Int32"">
        <Path>Living</Path>
      </IsType>
    </Annotation>
    <Annotation Term=""Funk.Clear"">
      <IsType Type=""foo.Address"">
        <Path>Address</Path>
      </IsType>
    </Annotation>
  </Annotations>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeCastExpressions()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""foo.Person"">
    <Annotation Term=""Funk.Punning"">
      <IsType Type=""Edm.Int32"">
        <Path>Living</Path>
      </IsType>
    </Annotation>
    <Annotation Term=""Funk.Clear"">
      <Cast Type=""foo.Address"">
        <Path>Address</Path>
      </Cast>
    </Annotation>
  </Annotations>
</Schema>";
            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void CheckArgumentValidation()
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

            try
            {
                model.TryWriteSchema((XmlWriter)null, out errors);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentNullException, "model.WriteCsdl((XmlWriter)null) => ArgumentNullException");
            }
            try
            {
                model.TryWriteSchema((Func<string, XmlWriter>)null, out errors);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentNullException, "model.WriteCsdl((Func<string, XmlWriter>)null) => ArgumentNullException");
            }
            try
            {
                model.TryWriteSchema(s => null, out errors);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is ArgumentNullException, "model.WriteCsdl(s => null) => ArgumentNullException");
            }
        }

        [TestMethod]
        public void SerializePathExpression()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Age"" Type=""Edm.Int32"" />
    <Annotation Term=""Funk.Mage"" Path=""Age"" />
  </EntityType>
  <EntityType Name=""LyingPerson"" BaseType=""Edm.TypeTerm"">
    <Property Name=""Sobriquet"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""FictionalAge"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <Term Name=""Age"" Type=""Edm.Int32"" />
  <Term Name=""Subject"" Type=""foo.Person"" />
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializePropertyPathExpression()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Age"" Type=""Edm.Int32"" />
    <Annotation Term=""Funk.Mage"" PropertyPath=""Age"" />
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializePropertyPathExpressions()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Age"" Type=""Edm.Int32"" />
    <Annotation Term=""Funk.Mage"">
      <Collection>
        <PropertyPath>Age</PropertyPath>
        <PropertyPath>Name</PropertyPath>
      </Collection>
    </Annotation>
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }


        [TestMethod]
        public void SerializeAssociationWithNoAssociationSet()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""Edm.String"" MaxLength=""100"" />
    <NavigationProperty Name=""ToPet"" Type=""Hot.Pet"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""PetId"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void RoundtripModelWithMultipleSilentMappings()
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Content"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""List"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Fields"" Type=""Collection(NS1.Field)"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Field"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Description"" Type=""NS1.Description"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Description"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""C1"">
    <EntitySet Name=""Contents"" EntityType=""NS1.Content"">
      <NavigationPropertyBinding Path=""Fields"" Target=""ContentFields"" />
    </EntitySet>
    <EntitySet Name=""Lists"" EntityType=""NS1.List"">
      <NavigationPropertyBinding Path=""Fields"" Target=""ListFields"" />
    </EntitySet>
    <EntitySet Name=""ContentFields"" EntityType=""NS1.Field"">
      <NavigationPropertyBinding Path=""Content"" Target=""Contents"" />
      <NavigationPropertyBinding Path=""Description"" Target=""Descriptions"" />
    </EntitySet>
    <EntitySet Name=""ListFields"" EntityType=""NS1.Field"">
      <NavigationPropertyBinding Path=""List"" Target=""Lists"" />
      <NavigationPropertyBinding Path=""Description"" Target=""Descriptions"" />
    </EntitySet>
    <EntitySet Name=""Descriptions"" EntityType=""NS1.Description"" />
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(csdl);
        }

        [TestMethod]
        public void SerializeNavigationPropertiesWithSharedAssociation()
        {
            const string inputText =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Cold"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Feckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""Ego"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Ego"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Bonus"" Type=""Edm.String"" MaxLength=""1001"" />
    <NavigationProperty Name=""MyReckless"" Type=""Cold.Reckless"" Nullable=""false"" Partner=""MyFecklesses"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
    <NavigationProperty Name=""MyReckless2"" Type=""Cold.Reckless"" Nullable=""false"" Partner=""MyFecklesses2"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
  </EntityType>
  <EntityType Name=""Reckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""AlterEgo"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""AlterEgo"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""MyFecklesses"" Type=""Collection(Cold.Feckless)"" Partner=""MyReckless"" ContainsTarget=""true"" />
    <NavigationProperty Name=""MyFecklesses2"" Type=""Collection(Cold.Feckless)"" Partner=""MyReckless2"" ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Recklesses"" EntityType=""Cold.Reckless"">
      <NavigationPropertyBinding Path=""MyFecklesses"" Target=""Fecklesses"" />
      <NavigationPropertyBinding Path=""MyFecklesses2"" Target=""Fecklesses"" />
    </EntitySet>
    <EntitySet Name=""Fecklesses"" EntityType=""Cold.Feckless"">
      <NavigationPropertyBinding Path=""MyReckless"" Target=""Recklesses"" />
      <NavigationPropertyBinding Path=""MyReckless2"" Target=""Recklesses"" />
    </EntitySet>
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(inputText);
        }

        [TestMethod]
        public void SerializeUsingAlias()
        {
            Func<IEnumerable<XElement>, string, XElement> GetCsdl = (csdls, namespaceValue) =>
                {
                    return csdls.Single(n => n.Attribute("Namespace").Value == namespaceValue);
                };

            var expectedCsdls = new[] {
                XElement.Parse(@"
              <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                <ComplexType Name=""SimpleType"">
                    <Property Name=""Data"" Type=""Edm.String"" />
                    <Property Name=""DataType"" Type=""Display.SimpleType"" />
                </ComplexType>
                <EntityContainer Name=""Container"" />
              </Schema>"),
                XElement.Parse(@"
              <Schema Namespace=""Org.OData.Display"" Alias=""Display"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
                <ComplexType Name=""SimpleType"">
                    <Property Name=""Data"" Type=""Edm.String"" />
                </ComplexType>
              </Schema>")};

            var actualCsdls = this.GetSerializerResult(this.GetParserResult(expectedCsdls)).Select(n=>XElement.Parse(n));
            Assert.IsTrue
                (
                    XElement.DeepEquals( GetCsdl(expectedCsdls, "DefaultNamespace"), GetCsdl(actualCsdls, "DefaultNamespace")) && XElement.DeepEquals( GetCsdl(expectedCsdls, "Org.OData.Display"), GetCsdl(actualCsdls, "Org.OData.Display")), 
                    "The CSDLs that the serializer generates is different from the original CSDLs"
                );
        }

        [TestMethod]
        public void RoundtripModelWithTypeDefinitions()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Int32"" />
  <TypeDefinition Name=""Width"" UnderlyingType=""Edm.Int32"">
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Centimeters"" />
  </TypeDefinition>
  <TypeDefinition Name=""Weight"" UnderlyingType=""Edm.Decimal"">
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Kilograms"" />
  </TypeDefinition>
  <TypeDefinition Name=""Address"" UnderlyingType=""Edm.String"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Weight"" Type=""MyNS.Weight"" />
    <Property Name=""Address"" Type=""MyNS.Address"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""MyNS.Person"" />
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(csdl);
        }

        [ TestMethod]
        public void RoundtripModelWithTypeDefinitionFacets()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Int32"" />
  <TypeDefinition Name=""Width"" UnderlyingType=""Edm.Int32"">
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Centimeters"" />
  </TypeDefinition>
  <TypeDefinition Name=""Weight"" UnderlyingType=""Edm.Decimal"">
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Kilograms"" />
  </TypeDefinition>
  <TypeDefinition Name=""Address"" UnderlyingType=""Edm.String"" />
  <TypeDefinition Name=""Point"" UnderlyingType=""Edm.GeographyPoint"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Weight"" Type=""MyNS.Weight"" Precision=""3"" Scale=""2"" />
    <Property Name=""Address"" Type=""MyNS.Address"" MaxLength=""10"" Unicode=""false"" />
    <Property Name=""Point"" Type=""MyNS.Point"" SRID=""123"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""MyNS.Person"" />
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(csdl);
        }

        [TestMethod]
        public void RoundtripModelWithTypeDefinitionInOperation()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Int32"" />
  <TypeDefinition Name=""Width"" UnderlyingType=""Edm.Int32"">
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Centimeters"" />
  </TypeDefinition>
  <TypeDefinition Name=""Weight"" UnderlyingType=""Edm.Decimal"">
    <Annotation Term=""Org.OData.Measurements.V1.Unit"" String=""Kilograms"" />
  </TypeDefinition>
  <TypeDefinition Name=""Address"" UnderlyingType=""Edm.String"" />
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Weight"" Type=""MyNS.Weight"" />
    <Property Name=""Address"" Type=""MyNS.Address"" />
  </EntityType>
  <Function Name=""GetWeightByLength"">
    <Parameter Name=""length"" Type=""MyNS.Length"" />
    <ReturnType Type=""MyNS.Weight"" />
  </Function>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""People"" EntityType=""MyNS.Person"" />
    <FunctionImport Name=""GetWeightByLength"" Function=""MyNS.GetWeightByLength"" />
  </EntityContainer>
</Schema>";

            VerifyRoundTrip(csdl);
        }

        [TestMethod]
        public void RoundtripDefaultValueAttributeOfTermElement()
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""ConventionalIDs"" Type=""Core.Tag"" DefaultValue=""True"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Entity-ids follow OData URL conventions"" />
  </Term>
</Schema>";

            VerifyRoundTrip(csdl);
        }

        void VerifyRoundTrip(string inputText)
        {
            VerifyRoundTrip(inputText, null, null);
        }

        void VerifyRoundTrip(string inputText, Action<IEdmModel> checkModel, Func<string> expectedText)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(inputText)) }, out model, out errors);
            Assert.IsTrue(parsed, "Model Parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            if (checkModel != null)
            {
                checkModel(model);
            }
            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            model.TryWriteSchema(xw, out errors);
            xw.Flush();
            xw.Close();
            string outputText = sw.ToString();
            var expectedTextStr = expectedText != null ? expectedText() : inputText;
            Assert.AreEqual(expectedTextStr, outputText, "Input = Output");
        }

        void VerifyRoundTrip(IEnumerable<string> inputText)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            List<StringWriter> outStrings = new List<StringWriter>();
            List<XmlReader> readers = new List<XmlReader>();
            List<XmlWriter> writers = new List<XmlWriter>();

            foreach (string s in inputText)
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

            IEnumerator<XmlWriter> writerEnumerator = writers.GetEnumerator();
            model.TryWriteSchema(s => { writerEnumerator.MoveNext(); return writerEnumerator.Current; }, out errors);

            foreach (XmlWriter xw in writers)
            {
                xw.Flush();
                xw.Close();
            }

            IEnumerator<StringWriter> swEnumerator = outStrings.GetEnumerator();

            foreach (string input in inputText)
            {
                swEnumerator.MoveNext();
                Assert.IsTrue(swEnumerator.Current.ToString() == input, "Input = Output");
            }
        }
    }
}
