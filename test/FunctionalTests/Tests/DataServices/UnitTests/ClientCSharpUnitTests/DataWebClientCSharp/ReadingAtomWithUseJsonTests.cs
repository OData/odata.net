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
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;
    using System.IO;
    #endregion

    /// <summary>
    /// Test ReadingAtom when useJson on the Context is selected
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    // [TestClass]
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

        [Ignore] // Remove Atom
        [TestMethod]
        public void ReadComplexCollectionPropertyDsvV3()
        {
            var context = CreateTransportLayerContext(ComplexCollectionPropertyPayload, MimeApplicationXml);
            var uri = new Uri("/Suppliers(1)/Addresses", UriKind.Relative);

            // Expected valid results that is not occuring
            var results = context.Execute<SimpleNorthwind.Address>(uri, "GET", false).ToList();
            ValidateMultipleAddresses(results);
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ReadServiceOperationComplexValuesDsvV3()
        {
            this.ReadValidateServiceOperationReturningCollectionComplexValues();
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ReadServiceOperationCollectionValuesDsv3()
        {
            var context = CreateTransportLayerContext(serviceOperationComplexCollectionPayload, MimeApplicationXml);
            var uri = new Uri("/GetAddresses", UriKind.Relative);
            var results = context.Execute<Collection<SimpleNorthwind.Address>>(uri, "GET", true).Single().ToList();
            ValidateMultipleAddresses(results);
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
