//---------------------------------------------------------------------
// <copyright file="DisableAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.DisableAtomTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DisableAtomTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        private const string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public DisableAtomTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        #region Test method

        [TestMethod]
        public void GetServiceDocumentUsingAtomShouldBeFailed()
        {
            VerifyAtomBeDisabledQueryTest(string.Empty);
        }

        [TestMethod]
        public void QueryEntitySetUsingAtomShouldBeFailed()
        {
            VerifyAtomBeDisabledQueryTest("People");
        }

        [TestMethod]
        public void CountUsingAtomShouldBeFailed()
        {
            VerifyAtomBeDisabledQueryTest("People/$count");
        }

        [TestMethod]
        public void QueryEntityUsingAtomShouldBeFailed()
        {
            VerifyAtomBeDisabledQueryTest("People(1)");
        }

        [TestMethod]
        public void QueryComplexPropertyUsingAtomShouldBeFailed()
        {
            VerifyAtomBeDisabledQueryTest("People(1)/HomeAddress");
        }

        [TestMethod]
        public void QueryPrimitivePropertyUsingAtomShouldBeFailed()
        {
            VerifyAtomBeDisabledQueryTest("People(1)/FirstName");
        }

        [TestMethod]
        public void FilterUsingAtomShouldBeFailed()
        {
            VerifyAtomBeDisabledQueryTest("People?$filter=FirstName eq 'Bob'");
        }

        [TestMethod]
        public void OrderByUsingAtomShouldBeFailed()
        {
            VerifyAtomBeDisabledQueryTest("People?$orderby=FirstName");
        }

        [TestMethod]
        public void UpdateUsingAtomShouldBeFailed()
        {
            var entryWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpacePrefix + "Person"
                },
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "HomeAddress",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + "HomeAddress",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "City",
                                        Value = "Chengdu"
                                    },
                                    new ODataProperty
                                    {
                                        Name = "FamilyName",
                                        Value = "Tigers"
                                    }
                                }
                            }
                        }
                    }
                }
            };
            
            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;
            var personType = Model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var personSet = Model.EntityContainer.FindEntitySet("People");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "People(1)"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationAtomXml);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationAtomXml);
            requestMessage.Method = "PATCH";

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
            {
                try
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(personSet, personType);
                    ODataWriterHelper.WriteResource(odataWriter, entryWrapper);
                }
                catch (Microsoft.OData.ODataContentTypeException)
                {
                    return;
                }
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void PostUsingAtomShouldBeFailed()
        {
            var entryWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpacePrefix + "Person",
                    Properties = new[]
                        {
                            new ODataProperty { Name = "PersonID", Value = 101 },
                            new ODataProperty { Name = "FirstName", Value = "Peter" },
                            new ODataProperty { Name = "LastName", Value = "Zhang" },
                            new ODataProperty
                            {
                                Name = "Home",
                                Value = GeographyPoint.Create(32.1, 23.1)
                            },
                            new ODataProperty
                            {
                                Name = "Numbers",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Edm.String)",
                                    Items = new string[] { "12345" }
                                }
                            },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Edm.String)",
                                    Items = new string[] { "a@b.cc" }
                                }
                            }
                        }
                },
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                    {
                        new ODataNestedResourceInfoWrapper()
                        {
                            NestedResourceInfo = new ODataNestedResourceInfo()
                            {
                                Name = "HomeAddress",
                                IsCollection = false
                            },
                            NestedResourceOrResourceSet = new ODataResourceWrapper()
                            {
                                Resource = new ODataResource()
                                {
                                    TypeName = NameSpacePrefix + "HomeAddress",
                                    Properties = new[]
                                    {
                                        new ODataProperty
                                        {
                                            Name = "Street",
                                            Value = "ZiXing Road"
                                        },
                                        new ODataProperty
                                        {
                                            Name = "City",
                                            Value = "Chengdu"
                                        },
                                        new ODataProperty
                                        {
                                            Name = "PostalCode",
                                            Value = "200241"
                                        },
                                        new ODataProperty
                                        {
                                            Name = "FamilyName",
                                            Value = "Tigers"
                                        }
                                    }
                                }
                            }
                        }
                    }
            };

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var personType = Model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var peopleSet = Model.EntityContainer.FindEntitySet("People");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "People"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationAtomXml);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationAtomXml);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
            {
                try
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(peopleSet, personType);
                    ODataWriterHelper.WriteResource(odataWriter, entryWrapper);
                }
                catch (Microsoft.OData.ODataContentTypeException)
                {
                    return;
                }
                Assert.IsTrue(false);
            }
        }

        // [Ignore] // Remove Atom
        // [TestMethod] // github issuse: #896
        public void QueryUsingAtomShouldBeFailedClientTest()
        {
            // TestClientContext.Format.UseAtom();

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            try
            {
                var person = TestClientContext.People.Where(p => p.PersonID == 1).Single();
            }
            catch (Microsoft.OData.Client.DataServiceQueryException e)
            {
                var statusCode = e.Response.StatusCode;
                Assert.AreEqual(415, statusCode);
                return;
            }
            Assert.IsTrue(false);
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        // [Ignore] // Remove Atom
        // [TestMethod] // github issuse: #896
        public void UpdateUsingAtomShouldBeFailedClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var person = TestClientContext.People.Where(p => p.PersonID == 1).Single();
            Assert.IsNotNull(person);

            var homeAddress = person.HomeAddress as Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.HomeAddress;
            Assert.IsNotNull(homeAddress);

            // TestClientContext.Format.UseAtom();
            homeAddress.City = "Shanghai";
            TestClientContext.UpdateObject(person);

            try
            {
                TestClientContext.SaveChanges(Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate);
            }
            catch (Microsoft.OData.Client.DataServiceRequestException e)
            {
                var message = e.InnerException.Message;
                Assert.IsTrue(message.StartsWith("A supported MIME type could not be found that matches the content type of the response."));
                return;
            }
            Assert.IsTrue(false);
        }

        // [Ignore] // Remove Atom
        // [TestMethod] // github issuse: #896
        public void InsertUsingAtomShouldBeFailedClientTest()
        {
            // TestClientContext.Format.UseAtom();
            
            Person newPerson = new Person()
            {
                PersonID = 10001,
                FirstName = "New",
                LastName = "Person",
                MiddleName = "Guy",
                Home = GeographyPoint.Create(32.1, 23.1),
                HomeAddress = new HomeAddress
                {
                    City = "Shanghai",
                    Street = "Zixing Rd",
                    PostalCode = "200241",
                    FamilyName = "New"
                }
            };
            TestClientContext.AddToPeople(newPerson);
            try
            {
                TestClientContext.SaveChanges();
            }
            catch (Microsoft.OData.Client.DataServiceRequestException e)
            {
                var message = e.InnerException.Message;
                Assert.IsTrue(message.StartsWith("A supported MIME type could not be found that matches the content type of the response."));
                return;
            }
        }
#endif

        #endregion

        private void VerifyAtomBeDisabledQueryTest(string uri)
        {
            var message = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            message.SetHeader("Accept", MimeTypes.ApplicationAtomXml);

            var responseMessage = message.GetResponse();

            Assert.AreEqual(415, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage))
            {
                var error = messageReader.ReadError();
                Assert.AreEqual(typeof(Microsoft.OData.ODataError), error.GetType());
                Assert.AreEqual("UnsupportedMediaType", error.ErrorCode);
            }
        }
    }
}