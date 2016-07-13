//---------------------------------------------------------------------
// <copyright file="EdmxModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class EdmxModelBuilder
    {
        public static string NonMatchingNamespaceAndVersionEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""1.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
</edmx:Edmx>";
        }

        public static string InvalidConceptualModelsEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema />
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";
        }

        public static string InvalidConceptualModelNamespaceEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Name=""DefaultNamespace"" xmlns=""http://foo"" />
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";
        }

        public static string InvalidDataServicesEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema />
  </edmx:DataServices>
</edmx:Edmx>";
        }

        public static string EmptyConceptualModelsEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels />
  </edmx:Runtime>
</edmx:Edmx>";
        }

        public static string SimpleConceptualModelsEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";
        }

        public static string EmptyDataServicesEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices />
</edmx:Edmx>";
        }

        public static string EmptyDataServicesWithOtherAttributesEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices m:DataServiceVersion=""2.31"" m:Other=""foo"" m:MaxDataServiceVersion=""2.5"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" />
</edmx:Edmx>";
        }

        public static string SimpleDataServicesEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        }

        public static IEdmModel SimpleEdmx()
        {
            var model = new EdmModel();

            EdmComplexType simpleType = new EdmComplexType("DefaultNamespace", "SimpleType");
            simpleType.AddProperty(new EdmStructuralProperty(simpleType, "Data", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(simpleType);

            EdmEntityContainer container = new EdmEntityContainer("DefaultNamespace", "Container");
            model.AddElement(container);

            return model;
        }

        public static string TwoDataServicesEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
  <edmx:DataServices>
    <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        }

        public static string TwoRuntimeEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";
        }

        public static string UsingAliasEdmx()
        {
            return @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Runtime>
    <edmx:ConceptualModels>
      <Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <ComplexType Name=""SimpleType"">
          <Property Name=""Data"" Type=""Edm.String"" />
        <Property Name=""DataType"" Type=""Display.SimpleType"" />
        </ComplexType>
        <EntityContainer Name=""Container"" />
      </Schema>
    <Schema Namespace=""Org.OData.Display"" Alias=""Display"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""SimpleType"">
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
    </Schema>
    </edmx:ConceptualModels>
  </edmx:Runtime>
</edmx:Edmx>";
        }

        #region referenced model
        public static bool GetReferencedModelEdmx(out IEdmModel model, out string mainEdmx, out string referencedEdmx1, out string referencedEdmx2)
        {
            // customer:
            mainEdmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/VipCustomer.xml"">
    <edmx:Include Namespace=""NS.Ref1"" Alias=""VPCT"" />
    <edmx:IncludeAnnotations TermNamespace="""" TargetNamespace=""NS.Ref1"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.validation"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.display"" Qualifier=""Tablet""/>
    <edmx:IncludeAnnotations TermNamespace=""org.example.hcm"" TargetNamespace=""com.contoso.Sales"" />
    <edmx:IncludeAnnotations TermNamespace=""org.example.hcm"" Qualifier=""Tablet"" TargetNamespace=""com.contoso.Person"" />
  </edmx:Reference>
  <edmx:Reference Uri=""http://host/schema/VipCard.xml"">
    <edmx:Include Namespace=""NS.Ref2"" Alias=""VPCD"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""NS1"" Alias=""CT"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Customer"">
            <Key>
            <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <ComplexType Name=""Address"" >
            <Property Name=""StreetAddress"" Type=""Edm.String"" Nullable=""false"" />
            <Property Name=""ZipCode"" Type=""Edm.Int32"" Nullable=""false"" />
        </ComplexType>
        <EntityContainer Name=""C1"" Extends=""Vip_C2"">
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
            referencedEdmx1 =
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
    </EntityType>
    <EntityContainer Name=""Vip_C1"">
        <EntitySet Name=""VipCustomers"" EntityType=""NS1.VipCustomer"" />
    </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            // the below will be referenced by the above both.
            // VipCard:
            referencedEdmx2 =
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

            string referencedEdmx1Tmp = referencedEdmx1;
            string referencedEdmx2Tmp = referencedEdmx2;
            Func<Uri, XmlReader> getReferencedModelReaderFunc = uri =>
            {
                if (uri.AbsoluteUri == "http://host/schema/VipCustomer.xml")
                {
                    return XmlReader.Create(new StringReader(referencedEdmx1Tmp));
                }

                if (uri.AbsoluteUri == "http://host/schema/VipCard.xml")
                {
                    return XmlReader.Create(new StringReader(referencedEdmx2Tmp));
                }

                return null;
            };

            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(mainEdmx)), getReferencedModelReaderFunc, out model, out errors);
            Assert.IsTrue(parsed);
            return parsed;
        }
        #endregion
    }
}
