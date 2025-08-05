//---------------------------------------------------------------------
// <copyright file="CsdlLocationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class CsdlLocationTests : EdmLibTestCaseBase
{
    [Fact]
    public void LocateEntityContainerAndVerifyLocations()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        IEdmEntityType person = (IEdmEntityType)model.FindType("Hot.Person");
        IEdmProperty personId = person.FindProperty("Id");
        IEdmProperty personAddress = person.FindProperty("Address");
        IEdmEntityType pet = (IEdmEntityType)model.FindType("Hot.Pet");
        IEdmProperty petId = pet.FindProperty("Id");
        IEdmProperty ownerId = pet.FindProperty("OwnerId");

        IEdmEntityContainer wild = model.EntityContainer;
        IEdmEntitySet people = model.EntityContainer.FindEntitySet("People");
        IEdmEntitySet pets = model.EntityContainer.FindEntitySet("Pets");

        Assert.StartsWith(@"EntityType Name=""Person""", GetLocationString((CsdlLocation)person.Location(), csdl));
        Assert.StartsWith(@"Property Name=""Id"" Type=""Int32"" Nullable=""false""", GetLocationString((CsdlLocation)personId.Location(), csdl));
        Assert.StartsWith(@"Property Name=""Address"" Type=""String"" MaxLength=""100""", GetLocationString((CsdlLocation)personAddress.Location(), csdl));
        Assert.StartsWith(@"EntityType Name=""Pet""", GetLocationString((CsdlLocation)pet.Location(), csdl));
        Assert.StartsWith(@"Property Name=""Id"" Type=""Int32"" Nullable=""false""", GetLocationString((CsdlLocation)petId.Location(), csdl));
        Assert.StartsWith(@"Property Name=""OwnerId"" Type=""Int32"" Nullable=""false""", GetLocationString((CsdlLocation)ownerId.Location(), csdl));
        Assert.StartsWith(@"EntityContainer Name=""Wild""", GetLocationString((CsdlLocation)wild.Location(), csdl));
        Assert.StartsWith(@"EntitySet Name=""People"" EntityType=""Hot.Person""", GetLocationString((CsdlLocation)people.Location(), csdl));
        Assert.StartsWith(@"EntitySet Name=""Pets"" EntityType=""Hot.Pet""", GetLocationString((CsdlLocation)pets.Location(), csdl));
    }

    [Fact]
    public void LocateComplexTypePropertiesAndVerifyLocations()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> error);
        Assert.True(parsed);

        IEdmComplexType complexType = (IEdmComplexType)model.FindType("Grumble.Smod");
        IEdmTypeReference primitive = complexType.FindProperty("Id").Type;
        IEdmTypeReference collection = complexType.FindProperty("Collection").Type;
        IEdmTypeReference entityReference = complexType.FindProperty("ref").Type;

        Assert.StartsWith(@"Property Name=""Id"" Type=""Edm.Int32"" ", GetLocationString((CsdlLocation)primitive.Location(), csdl));
        Assert.StartsWith(@"Property Name=""Collection"" Type=""Collection(Edm.Int32)"" ", GetLocationString((CsdlLocation)collection.Location(), csdl));
        Assert.StartsWith(@"Property Name=""ref"" Type=""Ref(Grumble.Person)"" ", GetLocationString((CsdlLocation)entityReference.Location(), csdl));
    }

    [Fact]
    public void VerifyErrorLocationWhenSourceIsMissing()
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

        bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out IEdmModel model, out IEnumerable<EdmError> error);
        Assert.False(parsed);

        var location = (CsdlLocation)error.First().ErrorLocation;
        Assert.Equal(string.Empty, location.Source);
    }

    [Fact]
    public void VerifyErrorLocationsInReferencedModels()
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

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx), new XmlReaderSettings(), edmxUri), getReferencedSchemaFunc, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);

        var errorList = errors.ToArray();

        var location1 = (CsdlLocation)errorList[0].ErrorLocation;
        Assert.Equal(edmxRef1Uri, location1.Source);

        var location2 = (CsdlLocation)errorList[1].ErrorLocation;
        Assert.Equal(edmxRef2Uri, location2.Source);
    }

    public static string GetLocationString(CsdlLocation location, string sourceText)
    {
        string[] lines = sourceText.Split('\n');
        return lines[location.LineNumber - 1].Substring(location.LinePosition - 1);
    }
}
