//---------------------------------------------------------------------
// <copyright file="CsdlParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.E2E.Tests.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class CsdlParsingTests : EdmLibTestCaseBase
{
    [Fact]
    public void TryParse_WithNullReferencesParameter_ThrowsArgumentNullException()
    {
        string edmx =
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

        IEnumerable<IEdmModel>? refs = null;
        var exception = Assert.Throws<ArgumentNullException>(() => CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), refs, out IEdmModel model, out IEnumerable<EdmError> errors));
        Assert.Equal("Value cannot be null. (Parameter 'references')", exception.Message);

        var exception2 = Assert.Throws<ArgumentNullException>(() => CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), refs, new CsdlReaderSettings(), out IEdmModel model, out IEnumerable<EdmError> errors));
        Assert.Equal("Value cannot be null. (Parameter 'references')", exception2.Message);
    }

    [Fact]
    public void TryParse_SimpleEFEdmx_Succeeds()
    {
        var edmx =
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

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name);
    }

    [Fact]
    public void TryParse_CircularReferencedModelODataEdmx_Succeeds()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""Location"" />
    <edmx:Include Namespace=""Test.Location2"" Alias=""Location2"" ></edmx:Include>
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
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
            <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
            <EntitySet Name=""VipCustomers"" EntityType=""Location.VipCustomer"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        // the above EntitySet 'VipCustomers' will reference the below EntityType 'VipCustomer' ,
        // and the below NS1.Address reference the above complex type.
        var edmxRef1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Test.hdskfhewfhew"" Alias=""Location"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"">
        <Key>
        <PropertyRef Name=""VipCustomerID"" />
        </Key>
        <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""VipAddress"" Type=""NS1.Address"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""Vip_C1"">
        <EntitySet Name=""VipCustomers"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
        {
            if (uri.AbsoluteUri == "http://host/schema/Location.xml")
            {
                return XmlReader.Create(new StringReader(edmxRef1));
            }

            return null;
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), getReferencedModelReaderFunc, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.NotNull(model.FindType("NS.Ref1.VipCustomer"));
        IEdmEntitySet entitySet = model.EntityContainer.EntitySets().First<IEdmEntitySet>(s => s.Name == "VipCustomers");
        Assert.NotNull(entitySet);
        Assert.NotNull(entitySet.EntityType);

        // verify property type in ref1 model
        IEdmProperty referencedEntityTypeProp1 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCustomerID");
        Assert.Equal("Edm.String", referencedEntityTypeProp1.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.Primitive, referencedEntityTypeProp1.Type.Definition.TypeKind);

        // verify property type in main model
        IEdmProperty referencedEntityTypeProp2 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipAddress");
        Assert.Equal("NS1.Address", referencedEntityTypeProp2.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.Complex, referencedEntityTypeProp2.Type.Definition.TypeKind);

        Assert.True(parsed);
        Assert.True(errors.Count() == 0);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.First().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name);
    }

    [Fact]
    public void TryParse_CircularReferencedMultipleModels_WithNamespaceAlias_Succeeds()
    {
        // customer:
        var edmx =
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

        // the above EntitySet 'VipCustomers' will reference the below EntityType 'VipCustomer' ,
        // and the below NS1.Address reference the above complex type.
        // VipCustomer:
        var edmxRef1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"">
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

        // the below will be referenced by the above both.
        // VipCard:
        var edmxRef2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
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
            if (uri.AbsoluteUri == "http://host/schema/VipCustomer.xml")
            {
                return XmlReader.Create(new StringReader(edmxRef1));
            }

            if (uri.AbsoluteUri == "http://host/schema/VipCard.xml")
            {
                return XmlReader.Create(new StringReader(edmxRef2));
            }
            return null;
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), getReferencedSchemaFunc, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.NotNull(model.FindType("NS.Ref1.VipCustomer"));
        IEdmEntitySet entitySet = model.EntityContainer.EntitySets().First<IEdmEntitySet>(s => s.Name == "VipCustomers");
        Assert.NotNull(entitySet);
        Assert.NotNull(entitySet.EntityType);

        // verify property type in ref1 model
        IEdmProperty referencedEntityTypeProp1 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCustomerID");
        Assert.Equal("Edm.String", referencedEntityTypeProp1.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.Primitive, referencedEntityTypeProp1.Type.Definition.TypeKind);

        // verify property type in main model
        IEdmProperty referencedEntityTypeProp2 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipAddress");
        Assert.Equal("NS1.Address", referencedEntityTypeProp2.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.Complex, referencedEntityTypeProp2.Type.Definition.TypeKind);

        // verify property type in ref2 model
        IEdmProperty referencedEntityTypeProp3 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCards");
        Assert.True(referencedEntityTypeProp3.Type.IsCollection());
        IEdmEntityType VipCardEntityType = (IEdmEntityType)referencedEntityTypeProp3.Type.AsCollection().ElementType().Definition;
        Assert.Equal("NS.Ref2.VipCard", VipCardEntityType.FullName());
        Assert.Equal("Edm.Int32", VipCardEntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCardID").Type.FullName());
        Assert.Equal(EdmTypeKind.Collection, referencedEntityTypeProp3.Type.Definition.TypeKind);

        // verify bad property type referencing ref2 model
        IEdmProperty referencedEntityTypeProp4 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "BadTypeVipCards");
        IEdmEntityType badVipCardType = (IEdmEntityType)referencedEntityTypeProp4.Type.AsCollection().ElementType().Definition;
        Assert.Equal("VPCD.VipCard", badVipCardType.FullName());

        Assert.True(parsed);
        Assert.True(errors.Count() == 0);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.First().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name);

        // verify VipCard.xml
        IEdmEntitySet cardEntitySet = model.EntityContainer.EntitySets().First<IEdmEntitySet>(s => s.Name == "VipCards");

        // verify property type in ref2 model
        IEdmProperty cardEntitySetProp1 = cardEntitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCardID");
        Assert.Equal("Edm.Int32", cardEntitySetProp1.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.Primitive, cardEntitySetProp1.Type.Definition.TypeKind);
    }

    [Fact]
    public void TryParse_CircularReferencedMultipleModels_WithNamespaceAlias_ParseOneByOne()
    {
        // customer:
        var edmx =
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

        // the above EntitySet 'VipCustomers' will reference the below EntityType 'VipCustomer' ,
        // and the below NS1.Address reference the above complex type.
        // VipCustomer:
        var edmxRef1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"">
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

        // the below will be referenced by the above both.
        // VipCard:
        var edmxRef2 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
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
    <Schema Namespace=""NS.Ref2_NotIncluded"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCard"">
        <Key>
        <PropertyRef Name=""VipCardID"" />
        </Key>
        <Property Name=""VipCardID"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmxRef2)), (Func<Uri, XmlReader>)null, out IEdmModel ref2Model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmxRef2)), out ref2Model, out errors);
        Assert.True(parsed);

        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmxRef1)), (IEdmModel)ref2Model, out IEdmModel ref1Model, out errors);
        Assert.True(parsed);

        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), new IEdmModel[] { ref1Model, ref2Model }, out IEdmModel model, out errors);
        Assert.True(parsed);

        Assert.NotNull(model.FindType("NS.Ref1.VipCustomer"));
        IEdmEntitySet entitySet = model.EntityContainer.EntitySets().First<IEdmEntitySet>(s => s.Name == "VipCustomers");
        Assert.NotNull(entitySet);
        Assert.NotNull(entitySet.EntityType);

        // verify property type in ref1 model
        IEdmProperty referencedEntityTypeProp1 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCustomerID");
        Assert.Equal("Edm.String", referencedEntityTypeProp1.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.Primitive, referencedEntityTypeProp1.Type.Definition.TypeKind);

        // verify property type in main model (circle reference not resolved)
        IEdmProperty referencedEntityTypeProp2 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipAddress");
        Assert.Equal("NS1.Address", referencedEntityTypeProp2.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.None, referencedEntityTypeProp2.Type.Definition.TypeKind);

        // verify property type in ref2 model
        IEdmProperty referencedEntityTypeProp3 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCards");
        Assert.True(referencedEntityTypeProp3.Type.IsCollection());
        IEdmEntityType VipCardEntityType = (IEdmEntityType)referencedEntityTypeProp3.Type.AsCollection().ElementType().Definition;
        Assert.Equal("NS.Ref2.VipCard", VipCardEntityType.FullName());
        Assert.Equal("Edm.Int32", VipCardEntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCardID").Type.FullName());
        Assert.Equal(EdmTypeKind.Collection, referencedEntityTypeProp3.Type.Definition.TypeKind);

        // verify bad property type referencing ref2 model
        IEdmProperty referencedEntityTypeProp4 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "BadTypeVipCards");
        IEdmEntityType badVipCardType = (IEdmEntityType)referencedEntityTypeProp4.Type.AsCollection().ElementType().Definition;
        Assert.Equal("VPCD.VipCard", badVipCardType.FullName());

        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.First().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name);

        // verify VipCard.xml
        IEdmEntitySet cardEntitySet = model.EntityContainer.EntitySets().First<IEdmEntitySet>(s => s.Name == "VipCards");

        // verify property type in ref2 model
        IEdmProperty cardEntitySetProp1 = cardEntitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCardID");
        Assert.Equal("Edm.Int32", cardEntitySetProp1.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.Primitive, cardEntitySetProp1.Type.Definition.TypeKind);
    }

    [Fact]
    public void TryParse_CircularReferencedModelODataEdmx_LegacyApproach_Succeeds()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""Location"" />
    <edmx:Include Namespace=""Test.Location2"" Alias=""Location2"" ></edmx:Include>
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
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
            <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
            <EntitySet Name=""VipCustomers"" EntityType=""Location.VipCustomer"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        // the above EntitySet 'VipCustomers' will reference the below EntityType 'VipCustomer' ,
        // and the below NS1.Address reference the above complex type(will result in UnresolvedType)
        var edmxRef1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Test.hdskfhewfhew"" Alias=""Location"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"">
        <Key>
        <PropertyRef Name=""VipCustomerID"" />
        </Key>
        <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""VipAddress"" Type=""NS1.Address"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""Vip_C1"">
        <EntitySet Name=""VipCustomers"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmxRef1)), out IEdmModel refModel, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), new[] { refModel }, out IEdmModel model, out errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.NotNull(model.FindType("NS.Ref1.VipCustomer"));
        IEdmEntitySet entitySet = model.EntityContainer.EntitySets().First<IEdmEntitySet>(s => s.Name == "VipCustomers");
        Assert.NotNull(entitySet);
        Assert.NotNull(entitySet.EntityType);

        // verify property type in ref1 model
        IEdmProperty referencedEntityTypeProp1 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipCustomerID");
        Assert.Equal("Edm.String", referencedEntityTypeProp1.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.Primitive, referencedEntityTypeProp1.Type.Definition.TypeKind);

        // verify property type in main model
        IEdmProperty referencedEntityTypeProp2 = entitySet.EntityType.Properties().First<IEdmProperty>(s => s.Name == "VipAddress");
        Assert.Equal("NS1.Address", referencedEntityTypeProp2.Type.Definition.FullTypeName());

        //  Reverse reference does not work using old approach.
        Assert.Equal("NS1.Address", referencedEntityTypeProp2.Type.Definition.FullTypeName());
        Assert.Equal(EdmTypeKind.None, referencedEntityTypeProp2.Type.Definition.TypeKind);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.First().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name);
    }

    [Fact]
    public void TryParse_BadReferenceWithoutInclude_Fails()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedSchemaFunc = uri => null;

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), getReferencedSchemaFunc, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);
        var error = errors.Single();
        Assert.Equal("edmx:Reference must contain at least one edmx:Includes or edmx:IncludeAnnotations.", error.ErrorMessage);
    }

    [Fact]
    public void TryParse_BadReferenceWithoutInclude_SelfClosingElement_Fails()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"" />
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedSchemaFunc = uri => null;

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), getReferencedSchemaFunc, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);
        var error = errors.Single();
        Assert.Equal("edmx:Reference must contain at least one edmx:Includes or edmx:IncludeAnnotations.", error.ErrorMessage);
    }

    [Fact]
    public void TryParse_EdmxWithEFandODataBodies_Fails()
    {
        var edmx =
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

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);
        Assert.Single(errors);
    }

    [Fact]
    public void TryParse_SimpleEFEdmxWithGarbage_Succeeds()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <!-- EF Runtime content -->
  <edmx:Runtime>
    <!-- SSDL content -->
    <edmx:StorageModels>
      <Schema Namespace=""NorthwindModel.Store"" Alias=""Self"" Provider=""System.Data.SqlClient"" ProviderManifestToken=""2008"" xmlns:store=""http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator"" xmlns=""http://schemas.microsoft.com/ado/2009/02/edm/ssdl"">
        <EntityContainer Name=""NorthwindModelStoreContainer"">
          <EntitySet Name=""Categories"" EntityType=""NorthwindModel.Store.Categories"" store:Type=""Tables"" Schema=""dbo"" />
        </EntityContainer>
        <EntityType Name=""Categories"">
          <Key>
            <PropertyRef Name=""CategoryID"" />
          </Key>
          <Property Name=""CategoryID"" Type=""int"" Nullable=""false"" />
          <Property Name=""CategoryName"" Type=""nvarchar"" Nullable=""false"" MaxLength=""15"" />
          <Property Name=""Description"" Type=""ntext"" />
          <Property Name=""Picture"" Type=""image"" />
        </EntityType>
      </Schema>
    </edmx:StorageModels>
    <!-- CSDL content -->
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
    <!-- C-S mapping content -->
    <edmx:Mappings>
      <Mapping Space=""C-S"" xmlns=""http://schemas.microsoft.com/ado/2008/09/mapping/cs"">
        <EntityContainerMapping StorageEntityContainer=""NorthwindModelStoreContainer"" CdmEntityContainer=""NorthwindEntities"">
          <EntitySetMapping Name=""Territories"">
            <EntityTypeMapping TypeName=""NorthwindModel.Territory"">
              <MappingFragment StoreEntitySet=""Territories"">
                <ScalarProperty Name=""TerritoryID"" ColumnName=""TerritoryID"" />
                <ScalarProperty Name=""TerritoryDescription"" ColumnName=""TerritoryDescription"" />
                <ScalarProperty Name=""RegionID"" ColumnName=""RegionID"" />
              </MappingFragment>
            </EntityTypeMapping>
          </EntitySetMapping>
          <AssociationSetMapping Name=""EmployeeTerritories"" TypeName=""NorthwindModel.EmployeeTerritories"" StoreEntitySet=""EmployeeTerritories"">
            <EndProperty Name=""Employees"">
              <ScalarProperty Name=""EmployeeID"" ColumnName=""EmployeeID"" />
            </EndProperty>
            <EndProperty Name=""Territories"">
              <ScalarProperty Name=""TerritoryID"" ColumnName=""TerritoryID"" />
            </EndProperty>
          </AssociationSetMapping>
        </EntityContainerMapping>
      </Mapping>
    </edmx:Mappings>
  </edmx:Runtime>
  <!-- EF Designer content (DO NOT EDIT MANUALLY BELOW HERE) -->
  <Designer xmlns=""http://docs.oasis-open.org/odata/ns/edmx"">
    <Connection>
      <DesignerInfoPropertySet>
        <DesignerProperty Name=""MetadataArtifactProcessing"" Value=""EmbedInOutputAssembly"" />
      </DesignerInfoPropertySet>
    </Connection>
    <Options>
      <DesignerInfoPropertySet>
        <DesignerProperty Name=""ValidateOnBuild"" Value=""true"" />
        <DesignerProperty Name=""EnablePluralization"" Value=""True"" />
        <DesignerProperty Name=""IncludeForeignKeysInModel"" Value=""True"" />
      </DesignerInfoPropertySet>
    </Options>
    <!-- Diagram content (shape and connector positions) -->
    <Diagrams>
      <Diagram Name=""NorthwindEntities"">
        <EntityTypeShape EntityType=""NorthwindModel.Category"" Width=""1.5"" PointX=""3"" PointY=""19.5"" Height=""1.9802864583333317"" IsExpanded=""true"" />
        <AssociationConnector Association=""NorthwindModel.FK_Order_Details_Orders"" ManuallyRouted=""false"">
          <ConnectorPoint PointX=""6.75"" PointY=""3.3074446614583328"" />
          <ConnectorPoint PointX=""7.5"" PointY=""3.3074446614583328"" />
        </AssociationConnector>
        <AssociationConnector Association=""NorthwindModel.EmployeeTerritories"" ManuallyRouted=""false"">
          <ConnectorPoint PointX=""4.5"" PointY=""10.307571614583335"" />
          <ConnectorPoint PointX=""5.2447891666666671"" PointY=""10.307571614583333"" />
          <ConnectorPoint PointX=""5.5520833333333321"" PointY=""10.307571614583335"" />
          <ConnectorPoint PointX=""6.416666666666667"" PointY=""10.307571614583335"" />
          <ConnectorPoint PointX=""6.583333333333333"" PointY=""10.307571614583335"" />
          <ConnectorPoint PointX=""7.635416666666667"" PointY=""10.307571614583335"" />
          <ConnectorPoint PointX=""7.802083333333333"" PointY=""10.307571614583335"" />
          <ConnectorPoint PointX=""8"" PointY=""10.307571614583335"" />
        </AssociationConnector>
      </Diagram>
    </Diagrams>
  </Designer>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name);
    }

    [Fact]
    public void TryParse_EmptyEdmx_FailsWithXmlError()
    {
        var edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>";
        bool success = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(success, "Parse failed.");
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.XmlError, errors.First().ErrorCode);
    }

    [Fact]
    public void TryParse_EdmxWithEmptyVersion_FailsWithInvalidVersionNumber()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version="" "" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
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

        bool success = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(success);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.InvalidVersionNumber, errors.First().ErrorCode);
    }

    [Fact]
    public void TryParse_EdmxWithBadVersion_FailsWithInvalidVersionNumber()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""1.2.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
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

        bool success = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(success);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.InvalidVersionNumber, errors.First().ErrorCode);
    }

    [Fact]
    public void TryParse_EdmxWithNonIntegerVersion_FailsWithInvalidVersionNumber()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""1.WHARGARBLE"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
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

        bool success = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(success);
        Assert.Single(errors);
        Assert.Equal(EdmErrorCode.InvalidVersionNumber, errors.First().ErrorCode);
    }

    [Fact]
    public void TryParse_EdmxWithMultipleAttributes_Succeeds()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx OtherAttribute=""BorkBorkBork"" Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
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

        bool success = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(success);
        Assert.Empty(errors);
    }

    [Fact]
    public void TryParse_BadEdmx_ChecksErrorLocations()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!--Remark-->
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Org.OData.Core.V1"" Alias=""Core"">
      <UnExpected Term=""Core.Description"">
        <String>Core terms needed to write vocabularies</String>
      </UnExpected>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);
        Assert.Single(errors);

        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode);
        Assert.Equal("(6, 8)", errors.First().ErrorLocation.ToString());
        Assert.Equal("The schema element 'UnExpected' was not expected in the given context.", errors.First().ErrorMessage);
    }

    [Fact]
    public void TryParse_BadEdmxSingleLine_ChecksErrorLocations()
    {
        var edmx =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<!--Remark-->" +
            "<edmx:Edmx xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\" Version=\"4.0\">" +
            "<edmx:DataServices>" +
            "<Schema xmlns=\"http://docs.oasis-open.org/odata/ns/edm\" Namespace=\"Org.OData.Core.V1\" Alias=\"Core\">" +
            "<UnExpected Term=\"Core.Description\">" +
            "<String>Core terms needed to write vocabularies</String>" +
            "</UnExpected>" +
            "</Schema>" +
            "</edmx:DataServices>" +
            "</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);
        Assert.Single(errors);

        Assert.Equal(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode);
        Assert.Equal("(1, 250)", errors.First().ErrorLocation.ToString());
        Assert.Equal("The schema element 'UnExpected' was not expected in the given context.", errors.First().ErrorMessage);
    }

    [Fact]
    public void TryParse_DeclaredNamespacesAndTerm_Succeeds()
    {
        const string edmx = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
    <edmx:DataServices>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Test.NoAlias"">
            <Term Name=""Decimal_val"" Type=""Edm.Decimal"" />
            <Term Name=""GUID_val"" Type=""Edm.Guid"" /> 
            <ComplexType Name=""GeoLocation"" >
                <Property Name=""Lat"" Type=""Edm.Double"" />
                <Property Name=""Long"" Type=""Edm.Double"" />
                <Annotation Term=""Test.NoAlias.GUID_val"" Guid=""21EC2020-3AEA-1069-A2DD-08002B30309D"" />
                <Annotation Term=""Test.NoAlias.Decimal_val"" Decimal=""76.902"" />
            </ComplexType>
        </Schema>
        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Test.HasAlias"" Alias=""Self"">
            <ComplexType Name=""GeoLocation1"" >
                <Property Name=""Lat"" Type=""Edm.Double"" />
                <Property Name=""Long"" Type=""Edm.Double"" />
        </ComplexType>
        </Schema>
    </edmx:DataServices>
</edmx:Edmx>
";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);

        Assert.True(parsed);
        Assert.False(errors.Any());
        var declaredNamespaces = model.DeclaredNamespaces.ToList();
        Assert.Equal(2, declaredNamespaces.Count);
        Assert.Contains("Test.NoAlias", declaredNamespaces);
        Assert.Contains("Test.HasAlias", declaredNamespaces);

        // verify term's inline annotations:
        IEdmSchemaElement annoElement = model.SchemaElements.First(s => s.FullName().Contains("GeoLocation"));
        Guid guidVal = model.GetTermValue<Guid>(annoElement, "Test.NoAlias.GUID_val", new EdmToClrEvaluator(null));
        Assert.Equal(Guid.Parse("21EC2020-3AEA-1069-A2DD-08002B30309D"), guidVal);
        decimal decimalVal = model.GetTermValue<decimal>(annoElement, "Test.NoAlias.Decimal_val", new EdmToClrEvaluator(null));
        Assert.Equal(76.902m, decimalVal);
    }

    [Fact]
    public void TryParse_IgnoreUnexpectedAttributeAndElementInSchema_SucceedsWithWarnings()
    {
        const string edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"" UnexpectedAttribute=""true"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <UnexpectedElement>
            <NextedUnexpectedElement/>
        </UnexpectedElement>
        <EntityContainer Name=""C1"">
            <UnexpectedElement2/>
            <EntitySet Name=""Customers"" IncludeInServiceDocument=""false"" EntityType=""NS1.Customer"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), true, out IEdmModel model, out IEnumerable<EdmError> errors);

        Assert.True(parsed);
        Assert.Equal(3, errors.Count());
        Assert.Equal("C1", model.EntityContainer.Name);
        Assert.Equal("Customers", model.EntityContainer.Elements.Single().Name);
        Assert.Equal("NS1.Customer", model.SchemaElements.Single(e => e.FullName() == "NS1.Customer").FullName());
        Assert.Equal("CustomerID", ((IEdmEntityType)model.SchemaElements.Single(e => e.FullName() == "NS1.Customer")).DeclaredStructuralProperties().Single().Name);
    }

    [Fact]
    public void TryParse_IgnoreUnexpectedAttributeAndElementInReferencedSchema_SucceedsWithWarnings()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""Location"" />
  </edmx:Reference>
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
            <EntitySet Name=""VipCustomers"" EntityType=""Location.VipCustomer"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        // the above EntitySet 'VipCustomers' will reference the below EntityType 'VipCustomer' ,
        // and the below NS1.Address reference the above complex type.
        var edmxRef1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"">
        <Key>
        <PropertyRef Name=""VipCustomerID""  UnexpectedAttribute=""2""/>
        </Key>
        <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""Vip_C1"">
        <EntitySet Name=""VipCustomers"" UnexpectedAttribute=""1"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
        {
            if (uri.AbsoluteUri == "http://host/schema/Location.xml")
            {
                return XmlReader.Create(new StringReader(edmxRef1));
            }

            return null;
        };

        CsdlReaderSettings settings = new CsdlReaderSettings()
        {
            GetReferencedModelReaderFunc = getReferencedModelReaderFunc,
            IgnoreUnexpectedAttributesAndElements = true
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), Enumerable.Empty<IEdmModel>(), settings, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Equal(2, errors.Count());
        Assert.NotNull(model.FindType("NS.Ref1.VipCustomer"));
        IEdmEntitySet entitySet = model.EntityContainer.EntitySets().First<IEdmEntitySet>(s => s.Name == "VipCustomers");
        Assert.NotNull(entitySet);
        Assert.NotNull(entitySet.EntityType);
    }

    [Fact]
    public void TryParse_ReferenceModelWithFuncReturnsNull_FailsWithUnresolvedReferenceUri()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""Location"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        CsdlReaderSettings settings = new CsdlReaderSettings()
        {
            GetReferencedModelReaderFunc = (uri) => null,
            IgnoreUnexpectedAttributesAndElements = false
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), Enumerable.Empty<IEdmModel>(), settings, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.False(parsed);
        Assert.Single(errors);
        var error = errors.Single();

        Assert.Equal(EdmErrorCode.UnresolvedReferenceUriInEdmxReference, error.ErrorCode);
        //Assert.Equal(Strings.EdmxParser_UnresolvedReferenceUriInEdmxReference, error.ErrorMessage);
    }

    [Fact]
    public void TryParse_ReferenceModelWithFuncReturnsNull_WellKnownUri_Succeeds()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://docs.oasis-open.org/odata/odata/v4.0/os/vocabularies/Org.OData.Core.V1.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""Location"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        CsdlReaderSettings settings = new CsdlReaderSettings()
        {
            GetReferencedModelReaderFunc = (uri) => null,
            IgnoreUnexpectedAttributesAndElements = false
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), Enumerable.Empty<IEdmModel>(), settings, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        Assert.Equal(8, model.ReferencedModels.Count());
    }

    [Fact]
    public void TryParse_CsdlReaderSettings_ResolvesReferences()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""Location"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
            <Property Name=""Property1"" Type=""Location.C1"" Nullable=""false"" />
            <Property Name=""Property2"" Type=""NS.Ref2.C2"" Nullable=""false"" />
        </EntityType>
        <EntityContainer Name=""C1"">
            <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        var edmxRef1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""C1"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
        {
            if (uri.AbsoluteUri == "http://host/schema/Location.xml")
            {
                return XmlReader.Create(new StringReader(edmxRef1));
            }

            return null;
        };

        CsdlReaderSettings settings = new CsdlReaderSettings()
        {
            GetReferencedModelReaderFunc = getReferencedModelReaderFunc,
            IgnoreUnexpectedAttributesAndElements = true
        };

        EdmModel ref2 = new EdmModel();
        var c2Type = new EdmComplexType("NS.Ref2", "C2");
        ref2.AddElement(c2Type);

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), new[] { ref2 }, settings, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);
        var customerType = model.FindType("NS1.Customer") as IEdmEntityType;
        Assert.NotNull(customerType);
        var property1 = customerType.FindProperty("Property1");
        var c1Type = model.FindType("NS.Ref1.C1");
        Assert.NotNull(c1Type);
        Assert.Equal(c1Type, property1.Type.Definition);

        var property2 = customerType.FindProperty("Property2");
        Assert.NotNull(c2Type);
        Assert.Equal(c2Type, property2.Type.Definition);
    }

    [Fact]
    public void TryParse_EntityWithEnumKey_SucceedsAndValidates()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType IsFlags=""true"" Name=""Gender"">
        <Member Name=""Female"" Value=""2"" /> 
        <Member Name=""Male"" Value=""1"" /> 
      </EnumType>
        <EntityType Name=""Family"">
            <Key>
            <PropertyRef Name=""GenderVal"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
            <Property Name=""GenderVal"" Type=""NS1.Gender"" Nullable=""false"" />
        </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
        {
            return null;
        };

        CsdlReaderSettings settings = new CsdlReaderSettings()
        {
            GetReferencedModelReaderFunc = getReferencedModelReaderFunc,
            IgnoreUnexpectedAttributesAndElements = true
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), new IEdmModel[0], settings, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out IEnumerable<EdmError> validationResults);
        Assert.True(valid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TryParse_ReferenceModelWithAnnotationsInReference_RespectsIgnoreUnexpectedAttributesAndElements()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <Annotation Term=""Core.Description"">
      <String>Core terms needed to write vocabularies</String>
    </Annotation>
    <edmx:Include Namespace=""NS.Ref1"" Alias=""Location"" />
    <Annotation Term=""Core.Description"" />
    <Annotation Term=""Core.Description"">
    </Annotation>
  </edmx:Reference>
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
            <EntitySet Name=""VipCustomers"" EntityType=""Location.VipCustomer"" />
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        // the above EntitySet 'VipCustomers' will reference the below EntityType 'VipCustomer' ,
        // and the below NS1.Address reference the above complex type.
        var edmxRef1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS.Ref1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""VipCustomer"">
        <Key>
        <PropertyRef Name=""VipCustomerID""/>
        </Key>
        <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
    </EntityType>
    <EntityContainer Name=""Vip_C1"">
        <EntitySet Name=""VipCustomers"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
        {
            if (uri.AbsoluteUri == "http://host/schema/Location.xml")
            {
                return XmlReader.Create(new StringReader(edmxRef1));
            }

            return null;
        };

        CsdlReaderSettings settings = new CsdlReaderSettings()
        {
            GetReferencedModelReaderFunc = getReferencedModelReaderFunc,
            IgnoreUnexpectedAttributesAndElements = true
        };
        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), Enumerable.Empty<IEdmModel>(), settings, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Single(errors);
        Assert.NotNull(model.FindType("NS.Ref1.VipCustomer"));

        IEdmEntitySet entitySet = model.EntityContainer.EntitySets().First<IEdmEntitySet>(s => s.Name == "VipCustomers");
        Assert.NotNull(entitySet);
        Assert.NotNull(entitySet.EntityType);

        settings = new CsdlReaderSettings()
        {
            GetReferencedModelReaderFunc = getReferencedModelReaderFunc,
            IgnoreUnexpectedAttributesAndElements = false
        };

        parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), Enumerable.Empty<IEdmModel>(), settings, out model, out errors);
        Assert.False(parsed);
        Assert.Single(errors);
    }

    [Fact]
    public void TryParse_AbstractComplexType_ValidatesSuccessfully()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""SampleSchema"" Alias=""Self"">

      <ComplexType Name=""C1"" Abstract=""true"">
      </ComplexType>
      <ComplexType Name=""D1"" BaseType=""SampleSchema.C1"">
      </ComplexType>

    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out IEnumerable<EdmError> validationResults);
        Assert.True(valid);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void TryParse_EdmxWithAttributesAnnotationPathAndIncludeInServiceDocument_Succeeds()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
      <Schema Namespace=""com.test.gateway.default.iwbep.v4_gw_sample_basic.v0001"" Alias=""Test__self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""BusinessPartner"" HasStream=""false"">
            <Key>
              <PropertyRef Name=""BusinessPartnerID""/>
            </Key>
            <Property Name=""BusinessPartnerID"" Type=""Edm.String"" Nullable=""false"" MaxLength=""10""/>
            <Property Name=""BusinessPartnerRole"" Type=""Edm.String"" Nullable=""false"" MaxLength=""3""/>
        </EntityType>
        <EntityContainer Name=""DefaultContainer"">
            <EntitySet Name=""BusinessPartnerList1"" EntityType=""com.test.gateway.default.iwbep.v4_gw_sample_basic.v0001.BusinessPartner"" IncludeInServiceDocument=""false"">
            </EntitySet>
            <EntitySet Name=""BusinessPartnerList2"" EntityType=""com.test.gateway.default.iwbep.v4_gw_sample_basic.v0001.BusinessPartner"" IncludeInServiceDocument=""true"">
            </EntitySet>
            <EntitySet Name=""BusinessPartnerList3"" EntityType=""com.test.gateway.default.iwbep.v4_gw_sample_basic.v0001.BusinessPartner"">
            </EntitySet>
        </EntityContainer>
        <Annotations Target=""Test__self.BusinessPartner"">
            <Annotation Term=""com.test.vocabularies.UI.v1.Facets"">
              <Collection>
                <Record Type=""com.test.vocabularies.UI.v1.ReferenceFacet"">
                  <PropertyValue Property=""Label"" String=""Contacts""/>
                  <PropertyValue Property=""Target"" AnnotationPath=""BP_2_CONTACT/@com.sap.vocabularies.UI.v1.Badge""/>
                </Record>
             </Collection>
            </Annotation>
      </Annotations>
      </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        bool success = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(success);

        var entitiySets = model.EntityContainer.Elements.Where(e => e.ContainerElementKind == EdmContainerElementKind.EntitySet).Select(e => e as IEdmEntitySet);
        var bp1 = entitiySets.Where(e => e.Name == "BusinessPartnerList1").FirstOrDefault();
        Assert.False(bp1.IncludeInServiceDocument);
        var bp2 = entitiySets.Where(e => e.Name == "BusinessPartnerList2").FirstOrDefault();
        Assert.True(bp2.IncludeInServiceDocument);
        var bp3 = entitiySets.Where(e => e.Name == "BusinessPartnerList3").FirstOrDefault();
        Assert.True(bp3.IncludeInServiceDocument);

        var annotation = model.VocabularyAnnotations.FirstOrDefault();
        var collection = annotation.Value as IEdmCollectionExpression;
        var record = collection.Elements.FirstOrDefault() as IEdmRecordExpression;
        var propertyValue = record.Properties.Where(e => e.Name == "Target").FirstOrDefault();
        var expression = propertyValue.Value as IEdmExpression;
        Assert.Equal(EdmExpressionKind.AnnotationPath, expression.ExpressionKind);

        var pathExpression = expression as IEdmPathExpression;
        Assert.Equal("BP_2_CONTACT/@com.sap.vocabularies.UI.v1.Badge", string.Join("/", pathExpression.Path));
    }

    #region Edm.Untyped

    [Fact]
    public void TryParse_EdmUntypedType_SucceedsAndValidates()
    {
        var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EnumType IsFlags=""true"" Name=""Gender"">
        <Member Name=""Female"" Value=""2"" /> 
        <Member Name=""Male"" Value=""1"" /> 
      </EnumType>
        <EntityType Name=""Family"">
            <Key>
            <PropertyRef Name=""GenderVal"" />
            </Key>
            <Property Name=""GenderVal"" Type=""NS1.Gender"" Nullable=""false"" />
            <Property Name=""ExtendedInfo"" Type=""Edm.Untyped"" Nullable=""false"" />
        </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
        {
            return null;
        };

        CsdlReaderSettings settings = new CsdlReaderSettings()
        {
            GetReferencedModelReaderFunc = getReferencedModelReaderFunc,
            IgnoreUnexpectedAttributesAndElements = true
        };

        bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), new IEdmModel[0], settings, out IEdmModel model, out IEnumerable<EdmError> errors);
        Assert.True(parsed);
        Assert.Empty(errors);

        bool valid = model.Validate(out IEnumerable<EdmError> validationResults);
        Assert.True(valid);
        Assert.Empty(validationResults);
        IEdmEntityType familyType = (IEdmEntityType)model.FindType("NS1.Family");
        IEdmUntypedTypeReference untypedReference = (IEdmUntypedTypeReference)familyType.FindProperty("ExtendedInfo").Type;
        Assert.Equal(EdmTypeKind.Untyped, untypedReference.Definition.TypeKind);
    }
    #endregion
}
