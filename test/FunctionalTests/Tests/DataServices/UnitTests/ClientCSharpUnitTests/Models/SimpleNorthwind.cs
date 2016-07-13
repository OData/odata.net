//---------------------------------------------------------------------
// <copyright file="SimpleNorthwind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System; 
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;

    /// <summary>
    /// Contains a simplified northwind model used in transport layer client integration tests
    /// </summary>
    public static class SimpleNorthwind
    {
        private const string SimpleNorthwindModel = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
    <Schema Namespace=""ODataDemo"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Product"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" m:FC_TargetPath=""SyndicationTitle"" m:FC_ContentKind=""text"" m:FC_KeepInContent=""false"" />
        <Property Name=""Description"" Type=""Edm.String"" Nullable=""true"" m:FC_TargetPath=""SyndicationSummary"" m:FC_ContentKind=""text"" m:FC_KeepInContent=""false"" />
        <Property Name=""ReleaseDate"" Type=""Edm.DateTimeOffset"" Nullable=""false"" />
        <Property Name=""DiscontinuedDate"" Type=""Edm.DateTimeOffset"" Nullable=""true"" />
        <Property Name=""Rating"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Price"" Type=""Edm.Decimal"" Nullable=""false"" />
        <Property Name=""PrimaryEmail"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""Emails"" Type=""Collection(Edm.String)"" Nullable=""true"" />
        <NavigationProperty Name=""Category"" Type=""ODataDemo.Category"" Partner=""Products"" />
        <NavigationProperty Name=""Supplier"" Type=""ODataDemo.Supplier"" Partner=""Products"" />
      </EntityType>
      <EntityType Name=""Category"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" m:FC_TargetPath=""SyndicationTitle"" m:FC_ContentKind=""text"" m:FC_KeepInContent=""true"" />
        <NavigationProperty Name=""Products"" Type=""Collection(ODataDemo.Product)"" Partner=""Category"" />
      </EntityType>
      <EntityType Name=""Supplier"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""true"" m:FC_TargetPath=""SyndicationTitle"" m:FC_ContentKind=""text"" m:FC_KeepInContent=""true"" />
        <Property Name=""PrimaryAddress"" Type=""ODataDemo.Address"" Nullable=""false"" m:FC_TargetPath=""City"" m:FC_NsUri=""http://mynamespace"" m:FC_NsPrefix=""e"" m:FC_SourcePath=""City"" m:FC_KeepInContent=""true"" />
        <Property Name=""Addresses"" Type=""Collection(ODataDemo.Address)"" Nullable=""false"" />
        <Property Name=""Concurrency"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Products"" Type=""Collection(ODataDemo.Product)"" Partner=""Supplier"" />
      </EntityType>
      <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""State"" Type=""Edm.String"" Nullable=""true"" />
        <Property Name=""ZipCode"" Type=""Edm.String"" Nullable=""true"" />
      </ComplexType>
      <Function Name=""GetAddresses"">
        <ReturnType Type=""Collection(ODataDemo.Address)"" />
      </Function>
      <Function Name=""GetPrimitiveString"">
        <ReturnType Type=""Edm.String"" />
      </Function>
      <Function Name=""GetProductsByRating"">
        <ReturnType Type=""Collection(ODataDemo.Product)"" />
        <Parameter Name=""rating"" Type=""Edm.Int32"" />
      </Function>
      <EntityContainer Name=""DemoService"">
        <EntitySet Name=""Products"" EntityType=""ODataDemo.Product"">
          <NavigationPropertyBinding Path=""Category"" Target=""Categories"" />
          <NavigationPropertyBinding Path=""Supplier"" Target=""Suppliers"" />
        </EntitySet>
        <EntitySet Name=""Categories"" EntityType=""ODataDemo.Category"">
          <NavigationPropertyBinding Path=""Products"" Target=""Products"" />
        </EntitySet>
        <EntitySet Name=""Suppliers"" EntityType=""ODataDemo.Supplier"">
          <NavigationPropertyBinding Path=""Products"" Target=""Products"" />
        </EntitySet>
        <FunctionImport Name=""GetAddresses"" Function=""ODataDemo.GetAddresses"" m:HttpMethod=""GET"" />
        <FunctionImport Name=""GetPrimitiveString"" Function=""ODataDemo.GetPrimitiveString"" m:HttpMethod=""GET"" />
        <FunctionImport Name=""GetProductsByRating"" EntitySet=""Products"" Function=""ODataDemo.GetProductsByRating"" m:HttpMethod=""GET"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        public static IEdmModel BuildSimplifiedNorthwindModel()
        {
            return CsdlReader.Parse(XmlTextReader.Create(new StringReader(SimpleNorthwindModel)));
        }

        public class Product
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTimeOffset ReleaseDate { get; set; }
            public DateTimeOffset DiscontinuedDate { get; set; }
            public int Rating { get; set; }
            public decimal Price { get; set; }
            public Category Category { get; set; }
            public Supplier Supplier { get; set; }
        }

        public class Category
        {
            public Category()
            {
                this.Products = new List<Product>();
            }

            public int ID { get; set; }
            public string Name { get; set; }
            public List<Product> Products { get; private set; }
        }

        public class Supplier
        {
            public Supplier()
            {
                this.Products = new List<Product>();
                this.Emails = new List<string>();
                this.Addresses = new List<Address>();
            }

            public int ID { get; set; }
            public string Name { get; set; }
            public string PrimaryEmail { get; set; }
            public List<string> Emails { get; private set; }
            public Address PrimaryAddress { get; set; }
            public List<Address> Addresses { get; private set; }
            public int Concurrency { get; set; }
            public List<Product> Products { get; private set; }
        }

        public class Address
        {
            public string ZipCode { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
        }
    }
}