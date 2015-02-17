//---------------------------------------------------------------------
// <copyright file="WritingPipelineShortIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Common;
    using System.IO;
    using System.Text;
    using AstoriaUnitTests.ClientExtensions;
    using AstoriaUnitTests.Tests;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test ReadingAtom when useJson on the Context is selected
    /// </summary>
    [TestClass]
    public class WritingPipelineShortIntegrationTests
    {
        [TestMethod]
        public void BindTwoEntitiesSaveChanges()
        {
            var odataRequestMessage = new ODataTestMessage();
            var odataResponseMessage = new ODataTestMessage(){StatusCode = 204};
            DataServiceContextWithCustomTransportLayer context = new DataServiceContextWithCustomTransportLayer(DataServiceProtocolVersion.V3, () => odataRequestMessage, () => odataResponseMessage);

            WritingEntityReferenceLinkArgs args = null;
            context.Configurations.RequestPipeline.OnEntityReferenceLink((r => args = r));

            SimpleNorthwind.Product product = new SimpleNorthwind.Product() {ID = 1};
            SimpleNorthwind.Supplier supplier = new SimpleNorthwind.Supplier() {ID = 2};

            context.AttachTo("Products", product);
            context.AttachTo("Suppliers", supplier);

            context.SetLink(product, "Supplier", supplier);
            context.SaveChanges();

            // Note writing entity links is not triggered here as its not in the payload
            args.Should().BeNull();
            odataRequestMessage.Method.Should().Be("PUT");
            odataRequestMessage.Url.AbsoluteUri.Should().Be("http://somedummyuri/myService.svc/Products(1)/$links/Supplier");
        }

        [TestMethod]
        public void AttachEntityAddEntityAndBindSaveChanges()
        {
            var odataRequestMessage = new ODataTestMessage();
            var odataResponseMessage = new ODataTestMessage()
            {
                StatusCode = 202,
                MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes""?>
<entry xml:base=""http://services.odata.org/OData/OData.svc/"" xmlns:d=""http://schemas.microsoft.com/ado/2007/08/dataservices"" xmlns:m=""http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"" xmlns=""http://www.w3.org/2005/Atom"">
    <id>http://services.odata.org/OData/OData.svc/Suppliers(2)</id>
    <category term=""ODataDemo.Product"" scheme=""http://schemas.microsoft.com/ado/2007/08/dataservices/scheme"" />
    <link rel=""edit"" title=""AnnotationTests_Product"" href=""Products(2)"" />
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
  </entry>")) };
            odataResponseMessage.SetHeader("DataServiceId", "http://service/foo/Products(2)");
            odataResponseMessage.SetHeader("Location", "http://service/foo/Products(2)");
            odataResponseMessage.SetHeader("Content-Type", "application/atom+xml");

            DataServiceContextWithCustomTransportLayer context = new DataServiceContextWithCustomTransportLayer(DataServiceProtocolVersion.V3, () => odataRequestMessage, () => odataResponseMessage);

            WritingNavigationLinkArgs startingNavigLinkArgs = null;
            WritingNavigationLinkArgs endingNavigLinkArgs = null;
            WritingEntityReferenceLinkArgs args = null;
            context.Configurations.RequestPipeline.OnNavigationLinkStarting((r => startingNavigLinkArgs = r));
            context.Configurations.RequestPipeline.OnNavigationLinkEnding((r => endingNavigLinkArgs = r));
            context.Configurations.RequestPipeline.OnEntityReferenceLink((r => args = r));

            SimpleNorthwind.Product product = new SimpleNorthwind.Product() { ID = 1 };
            SimpleNorthwind.Supplier supplier = new SimpleNorthwind.Supplier() { ID = 2 };

            context.AddObject("Products", product);
            context.AttachTo("Suppliers", supplier);

            context.SetLink(product, "Supplier", supplier);
            context.SaveChanges();

            VerifyWritingNavigationLinkArgs(startingNavigLinkArgs, null, product, supplier, "Supplier");
            VerifyWritingNavigationLinkArgs(endingNavigLinkArgs, null, product, supplier, "Supplier");

            args.Should().NotBeNull();
            args.EntityReferenceLink.Url.AbsoluteUri.Should().Be("http://somedummyuri/myService.svc/Suppliers(2)");
        }

        private static void VerifyWritingNavigationLinkArgs(WritingNavigationLinkArgs args, string associationLinkUrl, object source, object target, string linkName)
        {
            args.Should().NotBeNull();
            args.Link.AssociationLinkUrl.Should().Be(associationLinkUrl);
            args.Source.Should().BeSameAs(source);
            args.Target.Should().BeSameAs(target);
            args.Link.Name.Should().Be(linkName);
        }
    }
}