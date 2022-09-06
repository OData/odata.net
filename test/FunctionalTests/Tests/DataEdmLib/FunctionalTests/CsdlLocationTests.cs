//---------------------------------------------------------------------
// <copyright file="CsdlLocationTests.cs" company="Microsoft">
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
    public class CsdlLocationTests : EdmLibTestCaseBase
    {
        [TestMethod]
        public void LocateEntityContainer()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""PetId"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityContainer Name=""Wild"">
    <EntitySet Name=""People"" EntityType=""Hot.Person"" />
    <EntitySet Name=""Pets"" EntityType=""Hot.Pet"" />
  </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("Hot.Person");
            IEdmProperty personId = person.FindProperty("Id");
            IEdmProperty personAddress = person.FindProperty("Address");
            IEdmEntityType pet = (IEdmEntityType)model.FindType("Hot.Pet");
            IEdmProperty petId = pet.FindProperty("Id");
            IEdmProperty ownerId = pet.FindProperty("OwnerId");

            IEdmEntityContainer wild = model.EntityContainer;
            IEdmEntitySet people = model.EntityContainer.FindEntitySet("People");
            IEdmEntitySet pets = model.EntityContainer.FindEntitySet("Pets");

            AssertCorrectLocation((CsdlLocation)person.Location(), csdl, @"EntityType Name=""Person""");
            AssertCorrectLocation((CsdlLocation)personId.Location(), csdl, @"Property Name=""Id"" Type=""Int32"" Nullable=""false""");
            AssertCorrectLocation((CsdlLocation)personAddress.Location(), csdl, @"Property Name=""Address"" Type=""String"" MaxLength=""100"" ");
            AssertCorrectLocation((CsdlLocation)pet.Location(), csdl, @"EntityType Name=""Pet""");
            AssertCorrectLocation((CsdlLocation)petId.Location(), csdl, @"Property Name=""Id"" Type=""Int32"" Nullable=""false"" ");
            AssertCorrectLocation((CsdlLocation)ownerId.Location(), csdl, @"Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" ");
            AssertCorrectLocation((CsdlLocation)wild.Location(), csdl, @"EntityContainer Name=""Wild""");
            AssertCorrectLocation((CsdlLocation)people.Location(), csdl, @"EntitySet Name=""People"" EntityType=""Hot.Person"" ");
            AssertCorrectLocation((CsdlLocation)pets.Location(), csdl, @"EntitySet Name=""Pets"" EntityType=""Hot.Pet"" ");
        }

        [TestMethod]
        public void LocateTypes()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
  </EntityType>
  <ComplexType Name=""Smod"">
    <Property Name=""Id"" Type=""Edm.Int32"" />
    <Property Name=""Collection"" Type=""Collection(Edm.Int32)"" />
    <Property Name=""ref"" Type=""Ref(Grumble.Person)"" />
  </ComplexType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> error;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out error);
            Assert.IsTrue(parsed, "parsed");

            IEdmComplexType complexType = (IEdmComplexType)model.FindType("Grumble.Smod");
            IEdmTypeReference primitive = complexType.FindProperty("Id").Type;
            IEdmTypeReference collection = complexType.FindProperty("Collection").Type;
            IEdmTypeReference entityReference = complexType.FindProperty("ref").Type;

            AssertCorrectLocation((CsdlLocation)primitive.Location(), csdl, @"Property Name=""Id"" Type=""Edm.Int32"" ");
            AssertCorrectLocation((CsdlLocation)collection.Location(), csdl, @"Property Name=""Collection"" Type=""Collection(Edm.Int32)"" ");
            AssertCorrectLocation((CsdlLocation)entityReference.Location(), csdl, @"Property Name=""ref"" Type=""Ref(Grumble.Person)"" ");
        }

        [TestMethod]
        public void NoSourceOfErrorLocation()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""MyNS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name1=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""String"" />
  </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> error;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out error);
            Assert.IsFalse(parsed, "parsed");

            var location = (CsdlLocation)error.First().ErrorLocation;
            Assert.AreEqual(string.Empty, location.Source);
        }

        [TestMethod]
        public void TestSourcesOfErrorLocationInReferencedModels()
        {
            const string edmxUri = @"http://host/schema/Customer.xml";
            const string edmxRef1Uri = @"http://host/schema/VipCustomer.xml";
            const string edmxRef2Uri = @"http://host/schema/VipCard.xml";

            const string edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCustomer.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""VPCT"" />
  </edmx:Reference>
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VPCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Alias=""CT"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <ComplexType Name=""Address"" >
            <Property Type=""String"" Name=""StreetAddress"" Nullable=""false"" />
            <Property Type=""Int32"" Name=""ZipCode"" Nullable=""false"" />
        </ComplexType>
        <EntityContainer Name=""C1"">
            <EntitySet Name=""Customers"" EntityType=""CT.Customer"" />
            <EntitySet Name=""VipCustomers"" EntityType=""VPCT.VipCustomer"" />
            <EntitySet Name=""VipCards"" EntityType=""VPCD.VipCard"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            const string edmxRef1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"" ToProduceError>
        <Key>
        <PropertyRef Name=""VipCustomerID"" />
        </Key>
        <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""VipAddress"" Type=""NS1.Address"" Nullable=""false"" />
        <NavigationProperty Name=""VipCards"" Type=""Collection(VCD.VipCard)"" />
        <NavigationProperty Name=""BadTypeVipCards"" Type=""Collection(VPCD.VipCard)"" />
    </EntityType>
    <EntityContainer Name=""Vip_C1"">
        <EntitySet Name=""VipCustomers"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            const string edmxRef2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCard"">
        <Key>
        <PropertyRef Name=""VipCardID"" />
        </Key>
        <Property Name=""VipCardID"" Type=""Edm.Int32"" Nullable1=""false"" />
    </EntityType>
    <EntityContainer Name=""Vip_C2"">
        <EntitySet Name=""VipCustomers2"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
    <Schema Namespace=""NS.Ref2_NotIncluded"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCard"">
        <Key>
        <PropertyRef Name=""VipCardID"" />
        </Key>
        <Property Name=""VipCardID"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""Vip_C2"">
        <EntitySet Name=""VipCustomers2"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            Func<Uri, XmlReader> getReferencedSchemaFunc = uri =>
            {
                if (uri.AbsoluteUri.Equals(edmxRef1Uri))
                {
                    return XmlReader.Create(new StringReader(edmxRef1));
                }

                if (uri.AbsoluteUri.Equals(edmxRef2Uri))
                {
                    return XmlReader.Create(new StringReader(edmxRef2));
                }

                return null;
            };

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx), new XmlReaderSettings(), edmxUri), getReferencedSchemaFunc, out model, out errors);
            Assert.IsFalse(parsed, "parsed");

            var errorList = errors.ToArray();

            var location1 = (CsdlLocation)errorList[0].ErrorLocation;
            Assert.AreEqual(edmxRef1Uri, location1.Source);

            var location2 = (CsdlLocation)errorList[1].ErrorLocation;
            Assert.AreEqual(edmxRef2Uri, location2.Source);
        }

        public void AssertCorrectLocation(CsdlLocation location, string sourceText, string comparisonText)
        {
            string[] lines = sourceText.Split('\n');
            string resultString = lines[location.LineNumber - 1].Substring(location.LinePosition - 1);
            Assert.IsTrue(resultString.StartsWith(comparisonText), "Expected text was not found at the given location.");
        }
    }
}
