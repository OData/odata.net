//---------------------------------------------------------------------
// <copyright file="ReadingAtomWithUseJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Client;
    using System.Linq;
    using AstoriaUnitTests.ClientExtensions;
    using AstoriaUnitTests.Tests;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;
    using System.IO;
    #endregion

    /// <summary>
    /// Test ReadingAtom when useJson on the Context is selected
    /// </summary>
    [TestClass]
    public class ReadingAtomWithJsonTests
    {
        /// <summary>MIME type for ATOM bodies (http://www.iana.org/assignments/media-types/application/).</summary>
        internal const string MimeApplicationAtom = "application/atom+xml";

        /// <summary>MIME type for XML bodies.</summary>
        internal const string MimeApplicationXml = "application/xml";

        private const string serviceOperationComplexCollectionPayload = @"<?xml version=""1.0"" encoding=""utf-8""?>
<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"" >
  <m:element m:type=""ODataDemo.Address"">
    <d:Street>NE 228th</d:Street>
    <d:City>Sammamish</d:City>
    <d:State>WA</d:State>
    <d:ZipCode>98074</d:ZipCode>
  </m:element>
  <m:element m:type=""ODataDemo.Address"">
    <d:Street>NE 223rd</d:Street>
    <d:City>Sammamish</d:City>
    <d:State>WA</d:State>
    <d:ZipCode>98073</d:ZipCode>
  </m:element>
</m:value>";

        private const string ComplexCollectionPropertyPayload = @"<?xml version=""1.0"" encoding=""utf-8""?>
<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"" m:type=""Collection(ODataDemo.Address)"">
  <m:element>
    <d:Street>NE 228th</d:Street>
    <d:City>Sammamish</d:City>
    <d:State>WA</d:State>
    <d:ZipCode>98074</d:ZipCode>
  </m:element>
  <m:element>
    <d:Street>NE 223rd</d:Street>
    <d:City>Sammamish</d:City>
    <d:State>WA</d:State>
    <d:ZipCode>98073</d:ZipCode>
  </m:element>
</m:value>";
        private static IEdmModel Model;
        private static Func<string, Type> ResolveType;
        private static Func<Type, string> ResolveName;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Model = SimpleNorthwind.BuildSimplifiedNorthwindModel();
            ResolveType = (string name) =>
            {
                string updatedName = name;
                updatedName = updatedName.Replace("ODataDemo.", "AstoriaUnitTests.SimpleNorthwind+");
                var foundType = typeof(SimpleNorthwind).GetNestedTypes().Single(t => t.FullName == updatedName);
                return foundType;
            };

            ResolveName = (Type t) =>
            {
                return t.FullName.Replace("AstoriaUnitTests.SimpleNorthwind+", "ODataDemo.");
            };
        }

        [TestMethod]
        public void ReadingEntitySet()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title type=""text"">Products</title>
  <id>http://services.odata.org/OData/OData.svc/Products</id>
  <updated>2012-11-14T00:15:37Z</updated>
  <link rel=""self"" title=""Products"" href=""Products"" />
  <entry>
    <id>http://services.odata.org/OData/OData.svc/Products(0)</id>
    <title type=""text""></title>
    <summary type=""text""></summary>
    <updated>2012-11-14T00:15:37Z</updated>
    <author>
      <name />
    </author>
    <link rel=""edit"" title=""Product"" href=""Products(0)"" />
    <link rel=""http://docs.oasis-open.org/odata/ns/related/Category"" type=""application/atom+xml;type=entry"" title=""Category"" href=""Products(0)/Category"" />
    <link rel=""http://docs.oasis-open.org/odata/ns/related/Supplier"" type=""application/atom+xml;type=entry"" title=""Supplier"" href=""Products(0)/Supplier"" />
    <category term=""#ODataDemo.Product"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">0</d:ID>
        <d:Name m:type=""Edm.String"">Bread</d:Name>
        <d:Description m:type=""Edm.String"">Whole grain bread</d:Description>
        <d:ReleaseDate m:type=""Edm.DateTimeOffset"">1992-01-01T00:00:00Z</d:ReleaseDate>
        <d:DiscontinuedDate m:type=""Edm.DateTimeOffset"">1993-01-01T00:00:00Z</d:DiscontinuedDate>
        <d:Rating m:type=""Edm.Int32"">4</d:Rating>
        <d:Price m:type=""Edm.Decimal"">2.5</d:Price>
      </m:properties>
    </content>
  </entry>
</feed>";

            var context = CreateTransportLayerContext(payload);
            var product = context.Execute<SimpleNorthwind.Product>(new Uri("/Product", UriKind.Relative)).SingleOrDefault();
            Assert.IsNotNull(product);
            product.ID.Should().Be(0);
            product.Name.Should().Be("Bread");
            product.Description.Should().Be("Whole grain bread");
        }

        [TestMethod]
        public void ReadingAtomEntry()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <id>http://services.odata.org/OData/OData.svc/Products(0)</id>
  <title type=""text""></title>
  <summary type=""text""></summary>
  <updated>2012-11-14T00:15:15Z</updated>
  <author>
    <name />
  </author>
  <link rel=""edit"" title=""Product"" href=""Products(0)"" />
  <link rel=""http://docs.oasis-open.org/odata/ns/related/Category"" type=""application/atom+xml;type=entry"" title=""Category"" href=""Products(0)/Category"" />
  <link rel=""http://docs.oasis-open.org/odata/ns/related/Supplier"" type=""application/atom+xml;type=entry"" title=""Supplier"" href=""Products(0)/Supplier"" />
  <category term=""#ODataDemo.Product"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
  <content type=""application/xml"">
    <m:properties>
      <d:Name m:type=""Edm.String"">Bread</d:Name>
      <d:Description m:type=""Edm.String"">Whole grain bread</d:Description>
      <d:ID m:type=""Edm.Int32"">0</d:ID>
      <d:ReleaseDate m:type=""Edm.DateTimeOffset"">1992-01-01T00:00:00Z</d:ReleaseDate>
      <d:DiscontinuedDate m:type=""Edm.DateTimeOffset"">1993-01-01T00:00:00Z</d:DiscontinuedDate>
      <d:Rating m:type=""Edm.Int32"">4</d:Rating>
      <d:Price m:type=""Edm.Decimal"">2.5</d:Price>
    </m:properties>
  </content>
</entry>";

            var context = CreateTransportLayerContext(payload);
            var product = context.Execute<SimpleNorthwind.Product>(new Uri("/Product(0)", UriKind.Relative)).SingleOrDefault();
            Assert.IsNotNull(product);
            product.ID.Should().Be(0);
            product.Name.Should().Be("Bread");
            product.Description.Should().Be("Whole grain bread");
        }

        [TestMethod]
        public void ReadFeedWithProjection()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<feed xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <title type=""text"">Products</title>
  <id>http://services.odata.org/OData/OData.svc/Products</id>
  <updated>2012-11-14T00:54:12Z</updated>
  <link rel=""self"" title=""Products"" href=""Products"" />
  <entry>
    <id>http://services.odata.org/OData/OData.svc/Products(0)</id>
    <title type=""text"">Bread</title>
    <summary type=""text"">Whole grain bread</summary>
    <updated>2012-11-14T00:54:12Z</updated>
    <author>
      <name />
    </author>
    <link rel=""edit"" title=""Product"" href=""Products(0)"" />
    <link rel=""http://docs.oasis-open.org/odata/ns/related/Category"" type=""application/atom+xml;type=entry"" title=""Category"" href=""Products(0)/Category"">
      <m:inline>
        <entry>
          <id>http://services.odata.org/OData/OData.svc/Categories(0)</id>
          <title type=""text"">Food</title>
          <updated>2012-11-14T00:54:12Z</updated>
          <author>
            <name />
          </author>
          <link rel=""edit"" title=""Category"" href=""Categories(0)"" />
          <link rel=""http://docs.oasis-open.org/odata/ns/related/Products"" type=""application/atom+xml;type=feed"" title=""Products"" href=""Categories(0)/Products"" />
          <category term=""#ODataDemo.Category"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
          <content type=""application/xml"">
            <m:properties>
              <d:ID m:type=""Edm.Int32"">0</d:ID>
              <d:Name>Food</d:Name>
            </m:properties>
          </content>
        </entry>
      </m:inline>
    </link>
    <link rel=""http://docs.oasis-open.org/odata/ns/related/Supplier"" type=""application/atom+xml;type=entry"" title=""Supplier"" href=""Products(0)/Supplier"" />
    <category term=""#ODataDemo.Product"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">0</d:ID>
        <d:ReleaseDate m:type=""Edm.DateTimeOffset"">1992-01-01T00:00:00Z</d:ReleaseDate>
        <d:DiscontinuedDate m:type=""Edm.DateTimeOffset"">1993-01-01T00:00:00Z</d:DiscontinuedDate>
        <d:Rating m:type=""Edm.Int32"">4</d:Rating>
        <d:Price m:type=""Edm.Decimal"">2.5</d:Price>
      </m:properties>
    </content>
  </entry>
</feed>";

            var context = CreateTransportLayerContext(payload);
            var product = context.Execute<SimpleNorthwind.Product>(new Uri("/Product(0)", UriKind.Relative)).SingleOrDefault();
            Assert.IsNotNull(product);
            product.ID.Should().Be(0);
            Assert.IsNotNull(product.Category);
            product.Category.ID.Should().Be(0);
        }

        [TestMethod]
        public void ReadEntryWithProjection()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <id>http://services.odata.org/OData/OData.svc/Categories(0)</id>
  <title type=""text"">Food</title>
  <updated>2012-11-14T00:56:37Z</updated>
  <author>
    <name />
  </author>
  <link rel=""edit"" title=""Category"" href=""Categories(0)"" />
  <link rel=""http://docs.oasis-open.org/odata/ns/related/Products"" type=""application/atom+xml;type=feed"" title=""Products"" href=""Categories(0)/Products"">
    <m:inline>
      <feed>
        <title type=""text"">Products</title>
        <id>http://services.odata.org/OData/OData.svc/Categories(0)/Products</id>
        <updated>2012-11-14T00:56:37Z</updated>
        <link rel=""self"" title=""Products"" href=""Categories(0)/Products"" />
        <entry>
          <id>http://services.odata.org/OData/OData.svc/Products(0)</id>
          <title type=""text"">Bread</title>
          <summary type=""text"">Whole grain bread</summary>
          <updated>2012-11-14T00:56:37Z</updated>
          <author>
            <name />
          </author>
          <link rel=""edit"" title=""Product"" href=""Products(0)"" />
          <link rel=""http://docs.oasis-open.org/odata/ns/related/Category"" type=""application/atom+xml;type=entry"" title=""Category"" href=""Products(0)/Category"" />
          <link rel=""http://docs.oasis-open.org/odata/ns/related/Supplier"" type=""application/atom+xml;type=entry"" title=""Supplier"" href=""Products(0)/Supplier"" />
          <category term=""#ODataDemo.Product"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
          <content type=""application/xml"">
            <m:properties>
              <d:ID m:type=""Edm.Int32"">0</d:ID>
              <d:ReleaseDate m:type=""Edm.DateTimeOffset"">1992-01-01T00:00:00Z</d:ReleaseDate>
              <d:DiscontinuedDate m:type=""Edm.DateTimeOffset"">1993-01-01T00:00:00Z</d:DiscontinuedDate>
              <d:Rating m:type=""Edm.Int32"">4</d:Rating>
              <d:Price m:type=""Edm.Decimal"">2.5</d:Price>
            </m:properties>
          </content>
        </entry>
      </feed>
    </m:inline>
  </link>
  <category term=""#ODataDemo.Category"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
  <content type=""application/xml"">
    <m:properties>
      <d:ID m:type=""Edm.Int32"">0</d:ID>
      <d:Name>Food</d:Name>
    </m:properties>
  </content>
</entry>";

            var context = CreateTransportLayerContext(payload);
            var category = context.Execute<SimpleNorthwind.Category>(new Uri("/People(1)", UriKind.Relative)).SingleOrDefault();

            Assert.IsNotNull(category);
            Assert.IsNotNull(category.Products);
            category.ID.Should().Be(0);
            Assert.AreEqual(1, category.Products.Count);
            category.Products[0].ID.Should().Be(0);
        }

        [TestMethod]
        public void ReadComplexCollectionPropertyDsvV3()
        {
            var context = CreateTransportLayerContext(ComplexCollectionPropertyPayload, MimeApplicationXml);
            var uri = new Uri("/Suppliers(1)/Addresses", UriKind.Relative);

            // Expected valid results that is not occuring
            var results = context.Execute<SimpleNorthwind.Address>(uri, "GET", false).ToList();
            ValidateMultipleAddresses(results);
        }

        [TestMethod]
        public void ReadPrimitiveCollectionProperty()
        {
            const string payload = @"<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" m:type=""Collection(Edm.Int32)"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element>3</m:element>
                        <m:element>2</m:element>
                        <m:element>1</m:element>
                    </m:value>";

            var context = CreateTransportLayerContext(payload, MimeApplicationXml);
            var uri = new Uri("/Suppliers(1)/Numbers", UriKind.Relative);

            var results = context.Execute<int>(uri, "GET", false).ToList();
            VerifyNumbers(results);
        }

        [TestMethod]
        public void ReadServiceOperationPrimitiveCollection()
        {
            const string payload = @"<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data""  xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                        <m:element m:type=""Edm.Int32"">3</m:element>
                        <m:element m:type=""Edm.Int32"">2</m:element>
                        <m:element m:type=""Edm.Int32"">1</m:element>
                    </m:value>";

            var context = CreateTransportLayerContext(payload, MimeApplicationXml);
            var uri = new Uri("GetNumbers", UriKind.Relative);

            var results = context.Execute<int>(uri, "GET", false).ToList();
            VerifyNumbers(results);
        }

        [TestMethod]
        public void ReadPrimitive()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">Exotic Liquids</m:value>";

            var context = CreateTransportLayerContext(payload, MimeApplicationXml);
            var uri = new Uri("/Product(1)/Name", UriKind.Relative);
            var nameValue = context.Execute<string>(uri, "GET", true).Single();
            Assert.IsNotNull(nameValue);
            nameValue.Should().Be("Exotic Liquids");
        }

        [TestMethod]
        public void ReadComplex()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<m:value m:type=""ODataDemo.Address"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"">
  <d:Street>NE 228th</d:Street>
  <d:City>Sammamish</d:City>
  <d:State>WA</d:State>
  <d:ZipCode>98074</d:ZipCode>
</m:value>";

            var context = CreateTransportLayerContext(payload, MimeApplicationXml);
            var uri = new Uri("Supplier(1)/PrimaryAddress", UriKind.Relative);
            var address = context.Execute<SimpleNorthwind.Address>(uri, "GET", true).Single();
            address.Should().NotBeNull();
            address.Street.Should().Be("NE 228th");
            address.State.Should().Be("WA");
            address.City.Should().Be("Sammamish");
            address.ZipCode.Should().Be("98074");
        }

        [TestMethod]
        public void ReadError()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<error xmlns=""http://docs.oasis-open.org/odata/ns/metadata"">
  <code></code>
  <message xml:lang=""en-US"">The URI 'http://services.odata.org/OData/OData.svc/Suppliers(0)/$value' is not valid. The segment before '$value' must be a Media Link Entry or a primitive property.</message>
</error>";

            var context = CreateTransportLayerContext(payload, MimeApplicationXml);
            Action test = () => context.Execute<SimpleNorthwind.Product>(new Uri("/People(1)", UriKind.Relative)).ToList();

            test.ShouldThrow<DataServiceQueryException>().WithInnerException<ODataErrorException>().WithMessage("An error occurred while processing this request.").WithInnerMessage("The URI 'http://services.odata.org/OData/OData.svc/Suppliers(0)/$value' is not valid. The segment before '$value' must be a Media Link Entry or a primitive property.");
        }

        [TestMethod]
        public void ReadServiceOperationPrimitiveValue()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8""?><m:value xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">Foo</m:value>";

            var context = CreateTransportLayerContext(payload, MimeApplicationXml);
            var uri = new Uri("/GetPrimitiveString", UriKind.Relative);
            var nameValue = context.Execute<string>(uri, "GET", true).Single();
            Assert.IsNotNull(nameValue);
            nameValue.Should().Be("Foo");
        }

        [TestMethod]
        public void ReadServiceOperationComplexValuesDsvV3()
        {
            this.ReadValidateServiceOperationReturningCollectionComplexValues();
        }

        [TestMethod]
        public void ReadServiceOperationCollectionValuesDsv3()
        {
            var context = CreateTransportLayerContext(serviceOperationComplexCollectionPayload, MimeApplicationXml);
            var uri = new Uri("/GetAddresses", UriKind.Relative);
            var results = context.Execute<Collection<SimpleNorthwind.Address>>(uri, "GET", true).Single().ToList();
            ValidateMultipleAddresses(results);
        }

        [TestMethod]
        public void LoadCategoryPropertyTest()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
  <id>http://services.odata.org/OData/OData.svc/Categories(0)</id>
  <title type=""text"">Food</title>
  <updated>2012-11-15T16:42:58Z</updated>
  <author>
    <name />
  </author>
  <link rel=""edit"" title=""Category"" href=""Categories(0)"" />
  <link rel=""http://docs.oasis-open.org/odata/ns/related/Products"" type=""application/atom+xml;type=feed"" title=""Products"" href=""Categories(0)/Products"" />
  <category term=""#ODataDemo.Category"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
  <content type=""application/xml"">
    <m:properties>
      <d:ID m:type=""Edm.Int32"">0</d:ID>
      <d:Name>Food</d:Name>
    </m:properties>
  </content>
</entry>";

            var context = CreateTransportLayerContext(payload);
            var p = new SimpleNorthwind.Product() { ID = 0 };
            context.AttachTo("Products", p);
            var operationResponse = context.LoadProperty(p, "Category");
            var enumerator = operationResponse.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            var category = (SimpleNorthwind.Category)enumerator.Current;
            Assert.IsNotNull(category);
            category.ID.Should().Be(0);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void LoadCategoryPropertyCollectionTest()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <id>http://services.odata.org/OData/OData.svc//Products</id>
  <title type=""text"">Products</title>
  <updated>2012-11-15T19:29:45Z</updated>
  <link rel=""self"" title=""Products"" href=""Products"" />
  <entry>
    <id>http://services.odata.org/OData/OData.svc/Suppliers(1)</id>
    <category term=""#ODataDemo.Product"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <link rel=""edit"" title=""AnnotationTests_Product"" href=""Products(1)"" />
    <title />
    <updated>2012-11-15T19:29:45Z</updated>
    <author>
      <name />
    </author>
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">1</d:ID>
      </m:properties>
    </content>
    <d:City xmlns:e=""http://mynamespace"">Seattle</d:City>
  </entry>
  <entry>
    <id>http://services.odata.org/OData/OData.svc/Suppliers(2)</id>
    <category term=""#ODataDemo.Product"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <link rel=""edit"" title=""AnnotationTests_Product"" href=""Products(2)"" />
    <title />
    <updated>2012-11-15T19:29:45Z</updated>
    <author>
      <name />
    </author>
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">2</d:ID>
      </m:properties>
    </content>
    <d:City xmlns:e=""http://mynamespace"">Tacoma</d:City>
  </entry>
</feed>";

            var context = CreateTransportLayerContext(payload);
            var c = new SimpleNorthwind.Category() { ID = 0 };
            context.AttachTo("Categories", c);
            var operationResponse = context.LoadProperty(c, "Products");
            var enumerator = operationResponse.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            var product = (SimpleNorthwind.Product)enumerator.Current;
            Assert.IsNotNull(product);
            product.ID.Should().Be(1);
            Assert.IsTrue(enumerator.MoveNext());
            var product2 = (SimpleNorthwind.Product)enumerator.Current;
            Assert.IsNotNull(product2);
            product2.ID.Should().Be(2);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void ALinqProjection()
        {
            const string payload = @"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <id>http://services.odata.org/OData/OData.svc//Products</id>
  <title type=""text"">Products</title>
  <updated>2012-11-15T19:29:45Z</updated>
  <link rel=""self"" title=""Products"" href=""Products"" />
  <entry>
    <id>http://services.odata.org/OData/OData.svc/Suppliers(1)</id>
    <category term=""#ODataDemo.Product"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <link rel=""edit"" title=""AnnotationTests_Product"" href=""Products(1)"" />
    <title />
    <updated>2012-11-15T19:29:45Z</updated>
    <author>
      <name />
    </author>
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">1</d:ID>
      </m:properties>
    </content>
    <d:City xmlns:e=""http://mynamespace"">Seattle</d:City>
  </entry>
</feed>";

            var context = CreateTransportLayerContext(payload);
            var query = from p in context.CreateQuery<SimpleNorthwind.Product>("Products")
                        select new
                        {
                            ID = p.ID
                        };

            var results = query.ToList();
            var enumerator = results.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
        }

        private static void VerifyNumbers(List<int> results)
        {
            Assert.IsNotNull(results);
            results.Should().HaveCount(3);
            results[0].Should().Be(3);
            results[1].Should().Be(2);
            results[2].Should().Be(1);
        }

        private void ReadValidateServiceOperationReturningCollectionComplexValues()
        {
            var context = this.CreateTransportLayerContext(serviceOperationComplexCollectionPayload, MimeApplicationXml);
            var uri = new Uri("/GetAddresses", UriKind.Relative);
            var results = context.Execute<SimpleNorthwind.Address>(uri, "GET", false).ToList();
            ValidateMultipleAddresses(results);
        }

        private static void ValidateMultipleAddresses(List<SimpleNorthwind.Address> results)
        {
            Assert.IsNotNull(results);
            results.Should().HaveCount(2);
            results[0].Street.Should().Be("NE 228th");
            results[1].Street.Should().Be("NE 223rd");
        }

        private DataServiceContextWithCustomTransportLayer CreateTransportLayerContext(string payload, string contentTypeHeaderValue = MimeApplicationAtom)
        {
            return CreateTransportLayerContext(payload, "4.0", ODataProtocolVersion.V4, contentTypeHeaderValue);
        }

        private DataServiceContextWithCustomTransportLayer CreateTransportLayerContext(string payload, string odataVersion, ODataProtocolVersion maxDataServiceVersion, string contentTypeHeaderValue = MimeApplicationAtom)
        {
            IODataRequestMessage requestMessage = new ODataTestMessage();
            var responseMessage = new ODataTestMessage();
            responseMessage.SetHeader("Content-Type", contentTypeHeaderValue);
            responseMessage.SetHeader("OData-Version", odataVersion);
            responseMessage.StatusCode = 200;
            responseMessage.WriteToStream(payload);
            responseMessage.SetHeader("Content-Length", responseMessage.MemoryStream.Length.ToString());

            var context = new DataServiceContextWithCustomTransportLayer(maxDataServiceVersion, requestMessage, responseMessage);
            //context.EnableAtom = true;
            context.ResolveName = ResolveName;
            context.ResolveType = ResolveType;
            context.Format.UseJson(Model);
            return context;
        }
    }
}
