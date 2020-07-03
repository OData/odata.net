//---------------------------------------------------------------------
// <copyright file="CollectionNullableFacetTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CollectionTests
{
    using System;
using System.Linq;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Services.TestServices;
using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;


    /// <summary>
    /// Tests for collection property nullable facet
    /// </summary>
    public class CollectionNullableFacetTest : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        private static string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public CollectionNullableFacetTest() : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        /// <summary>
        /// Verify collection structrual property with nullable facet specified false cannot have null element
        /// And collection can be empty
        /// </summary>
        [Fact]
        public void CollectionNullableFalseInStructrualProperty()
        {
            var personToAdd = new ODataResource
            {
                TypeName = NameSpacePrefix + "Customer",
                Properties = new[] 
                {
                    new ODataProperty {Name = "Numbers", Value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)", Items = new[] {"222-222-221", null}}}, 
                    new ODataProperty {Name = "Emails", Value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)", Items = new string[] {}}}
                }
            };
            this.UpdateEntityWithCollectionContainsNull(personToAdd, "Numbers");
        }

        /// <summary>
        /// Verify collection in structrual property with nullable facet specified false can have null element
        /// And collection can be empty
        /// </summary>
        [Fact]
        public void CollectionNullableTrueInStructrualProperty()
        {
            var personToAdd = new ODataResource
            {
                TypeName = NameSpacePrefix + "Customer",
                Properties = new[] 
                {
                    new ODataProperty {Name = "Numbers", Value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)", Items = new string[] {}}},
                    new ODataProperty {Name = "Emails", Value = new ODataCollectionValue() {TypeName = "Collection(Edm.String)", Items = new[] {"a@a.b", "b@b.b", null}}}
                }
            };
            this.UpdateEntityWithCollectionContainsNull(personToAdd, "Emails");
        }


        #region private methods
        /// <summary>
        /// Update entity with null element in collection 
        /// testProperty is a structual property and its collection value contains null element 
        /// </summary>
        private void UpdateEntityWithCollectionContainsNull(ODataResource personToAdd, String testProperty)
        {
            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;
            var customerType = Model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;
            var customerSet = Model.EntityContainer.FindEntitySet("Customers");

            // get the IsNullable value of testProperty
            bool isNullable = true;
            foreach (IEdmStructuralProperty property in customerType.BaseEntityType().DeclaredStructuralProperties())
            {
                if (property.Name.Equals(testProperty))
                {
                    IEdmCollectionTypeReference typeRef = property.Type as IEdmCollectionTypeReference;
                    Assert.NotNull(typeRef);
                    isNullable = typeRef.IsNullable;
                }
            }

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Customers(1)"));
                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "PUT";

                try
                {
                    //write request message
                    using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                    {
                        var odataWriter = messageWriter.CreateODataResourceWriter(customerSet, customerType);
                        odataWriter.WriteStart(personToAdd);
                        odataWriter.WriteEnd();
                    }

                    // send the http request
                    var responseMessage = requestMessage.GetResponse();

                    // verify the update
                    Assert.Equal(204, responseMessage.StatusCode);
                    ODataResource updatedProduct = this.QueryEntityItem("Customers(1)") as ODataResource;
                    ODataCollectionValue testCollection = updatedProduct.Properties.Single(p => p.Name == testProperty).Value as ODataCollectionValue;
                    ODataCollectionValue expectValue = personToAdd.Properties.Single(p => p.Name == testProperty).Value as ODataCollectionValue;
                    var actIter = testCollection.Items.GetEnumerator();
                    var expIter = expectValue.Items.GetEnumerator();
                    while ((actIter.MoveNext()) && (expIter.MoveNext()))
                    {
                        Assert.Equal(actIter.Current, expIter.Current);
                    }
                }
                catch (Exception exception)
                {
                    if (!isNullable)
                    {
                        Assert.Equal(exception.Message, "A null value was detected in the items of a collection property value; non-nullable instances of collection types do not support null values as items.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private ODataItem QueryEntityItem(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.Equal(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataItem item = null;
            if (expectedStatusCode == 200)
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceReader();
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            item = reader.Item;
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }

            return item;
        }

        #endregion
    }
}
