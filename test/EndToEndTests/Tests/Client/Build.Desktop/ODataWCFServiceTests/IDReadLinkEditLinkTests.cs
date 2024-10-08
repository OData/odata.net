﻿//---------------------------------------------------------------------
// <copyright file="IDReadLinkEditLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    public class IdReadLinkEditLinkTests : ODataWCFServiceTestsBase<InMemoryEntities>, IDisposable
    {
        private const string TestHeader = "Test_ODataEntryFieldToModify";

        public IdReadLinkEditLinkTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        [Fact]
        public void ClientShouldUseTheEditLinkGotFromPayloadToUpdateTheEntryInMinimalMetadataJson()
        {
            this.UpdateObject(MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
        }

        [Fact]
        public void ClientShouldUseTheEditLinkGotFromPayloadToUpdateTheEntryInMinimalFullJson()
        {
            this.UpdateObject(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata);
        }

        private void UpdateObject(string mimeType)
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader(TestHeader, "EditLink");

            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", mimeType);
            TestClientContext.Format.UseJson(Model);

            var person = TestClientContext.People.Where(p => p.PersonID == 2).SingleOrDefault();
            var stringOfErrorMessage = "";
            try
            {
                person.FirstName = "bla";
                TestClientContext.UpdateObject(person);
                //The EditLink in payload looks like http://potato987654321098, more infomation can be found in Microsoft.Test.OData.Services.ODataWCFService.ResponseWriter.WriteEntry()
                TestClientContext.SaveChanges();
            }
            catch (Exception ex)
            {
                stringOfErrorMessage = ex.InnerException.Message;
            }
            //The test is to make sure that the request is sent to server incorrectly, so it is supposed to throw exception.
            //TODO: This verification may fail in other languages.
            Assert.True(stringOfErrorMessage.Contains("No such host is known") //if fiddler is open
                          || stringOfErrorMessage.Contains("The remote name could not be resolved") //if fiddler is not open
                          || stringOfErrorMessage.Contains("Unable to connect to the remote server")
                          , String.Format("Exception message is expected to contain 'No such host is known' or 'The remote name could not be resolved', but actually not. The actual message is '{0}'.", stringOfErrorMessage));
        }

        //[Fact]
        //public void ItIsAbleToDeserializeObjectIfItIsTransientInAtom()
        //{
        //    this.QueryObjectWhenItIsTransient(MimeTypes.ApplicationAtomXml);
        //}

        [Fact]
        public void ItIsAbleToDeserializeObjectIfItIsTransientInFullMetadataJson()
        {
            this.QueryObjectWhenItIsTransient(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata);
        }

        [Fact]
        public void ItIsAbleToDeserializeObjectIfItIsTransientInMinimalMetadataJson()
        {
            this.QueryObjectWhenItIsTransient(MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
        }

        private void QueryObjectWhenItIsTransient(string mimeType)
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader(TestHeader, "IsTransient");

            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", mimeType);

            TestClientContext.Format.UseJson(Model);

            var person = TestClientContext.People.Where(p => p.PersonID == 2).SingleOrDefault();
            //String.Format("Expected PersonID is '2', but actually it is '{0}'", person.PersonID
            Assert.Equal(2, person.PersonID);
            var entityDescriptor = TestClientContext.GetEntityDescriptor(person);
            Assert.Null(entityDescriptor);
        }

        // [Fact] // github issuse: #896
        public void ItIsAbleToDeserializeObjectsIfTheyAreTransientInAtom()
        {
            this.QueryObjectsWhenTheyAreTransient(MimeTypes.ApplicationAtomXml);
        }

        [Fact]
        public void ItIsAbleToDeserializeObjectsIfTheyAreTransientInFullMetadataJson()
        {
            this.QueryObjectsWhenTheyAreTransient(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata);
        }

        [Fact]
        public void ItIsAbleToDeserializeObjectsIfTheyAreTransientInMinimalMetadataJson()
        {
            this.QueryObjectsWhenTheyAreTransient(MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
        }

        private void QueryObjectsWhenTheyAreTransient(string mimeType)
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader(TestHeader, "IsTransient");

            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", mimeType);
            TestClientContext.Format.UseJson(Model);

            var entryResults = TestClientContext.Execute<Person>(new Uri(ServiceBaseUri + "/People?$filter=PersonID eq 1")).ToArray();
            //Unexpected number of Products returned
            Assert.Equal(1, entryResults.Count());
            var entityDescriptor = TestClientContext.GetEntityDescriptor(entryResults[0]);
            Assert.Null(entityDescriptor);
        }

        [Fact]
        public void ClientShouldUseTheReadLinkGotFromPayloadToLoadPropertyInFullJson()
        {
            this.LoadProperty(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata);
        }

        [Fact]
        public void ClientShouldUseTheReadLinkGotFromPayloadToLoadPropertyInMinimalJson()
        {
            this.LoadProperty(MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
        }

        //[Fact]
        //public void ClientShouldUseTheReadLinkGotFromPayloadToLoadPropertyInAtom()
        //{
        //    this.LoadProperty(MimeTypes.ApplicationAtomXml);
        //}

        private void LoadProperty(string mimeType)
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader(TestHeader, "ReadLink");
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", mimeType);
            TestClientContext.Format.UseJson(Model);

            var person = TestClientContext.People.Where(p => p.PersonID == 2).SingleOrDefault();

            var stringOfErrorMessage = "";
            try
            {
                TestClientContext.LoadProperty(person, "FirstName");
            }
            catch (Exception ex)
            {
                stringOfErrorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
            }
            //The ReadLink in payload looks like http://potato987654321098, more infomation can be found in Microsoft.Test.OData.Services.ODataWCFService.ResponseWriter.WriteEntry()
            //The test is to make sure that the request is sent to server incorrectly, so it is supposed to throw exception.
            Assert.True(stringOfErrorMessage.Contains("No such host is known") //if fiddler is open
                          || stringOfErrorMessage.Contains("The remote name could not be resolved") //if fiddler is not open
                          || stringOfErrorMessage.Contains("Unable to connect to the remote server")
                          , String.Format("Exception message is expected to contain 'No such host is known' or 'The remote name could not be resolved', but actually not. The actual message is '{0}'.", stringOfErrorMessage));
        }

        //[Fact]
        //public void ShouldBeAbleToAddLinkBetweenEntitiesInAtom()
        //{
        //    this.AddDeleteLink(MimeTypes.ApplicationAtomXml);
        //}

        [Fact]
        public void ShouldBeAbleToAddLinkBetweenEntitiesInFullJson()
        {
            this.AddDeleteLink(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata);
        }

        [Fact]
        public void ShouldBeAbleToAddLinkBetweenEntitiesInMinimalJson()
        {
            this.AddDeleteLink(MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata);
        }

        public void AddDeleteLink(string mimeType)
        {
            TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", mimeType);
            TestClientContext.Format.UseJson(Model);

            //preparation
            //currently service does not support $expand, so need to query the navigation Orders first
            var uri = new Uri(ServiceBaseUri + "Products(5)/Details");
            var detailsInAProdct = TestClientContext.Execute<ProductDetail>(uri);
            var intOriginalOrderCount = detailsInAProdct.Count();
            var prodct = TestClientContext.Products.Where(c => c.ProductID == 5).SingleOrDefault();

            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader(TestHeader, "EditLink");//this is make sure EditLink is replaced with a random value.

            var productDetail = TestClientContext.ProductDetails.Where(o => o.ProductDetailID == 1 && o.ProductID == 6).SingleOrDefault();

            //add a link between customer and order
            TestClientContext.AddLink(prodct, "Details", productDetail);
            TestClientContext.SaveChanges();
            detailsInAProdct = TestClientContext.Execute<ProductDetail>(uri);
            var intOrderCountAfterAddLink = detailsInAProdct.Count();
            //The link is added
            Assert.Equal(intOriginalOrderCount + 1, intOrderCountAfterAddLink);

            //delete the added link
            TestClientContext.DeleteLink(prodct, "Details", productDetail);
            TestClientContext.SaveChanges();
            detailsInAProdct = TestClientContext.Execute<ProductDetail>(uri);
            var intOrderCountAfterDeleteLink = detailsInAProdct.Count();
            //The added link is deleted
            Assert.Equal(intOriginalOrderCount, intOrderCountAfterDeleteLink);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}