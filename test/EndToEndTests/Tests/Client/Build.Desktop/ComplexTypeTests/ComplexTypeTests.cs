//---------------------------------------------------------------------
// <copyright file="ComplexTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ComplexTypeTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataClient = Microsoft.OData.Client;

    [TestClass]
    public class ComplexTypeTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        private const string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public ComplexTypeTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        #region Test method

        #region Complex type inheritance

        [TestMethod]
        public void QueryEntityWithDerivedComplexTypeProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(1)", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                ODataComplexValue homeAddress = (entry.Properties.Single(p => p.Name == "HomeAddress").Value as ODataComplexValue);
                                Assert.IsNotNull(homeAddress);
                                string typeName = NameSpacePrefix + "HomeAddress";
                                Assert.AreEqual(typeName, homeAddress.TypeName);
                                Assert.AreEqual("Cats", homeAddress.Properties.Single(p => p.Name == "FamilyName").Value);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryDerivedComplexTypeProperty()
        {
            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in types)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + @"People(1)/HomeAddress", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty property = messageReader.ReadProperty();
                        ODataComplexValue homeAddress = property.Value as ODataComplexValue;
                        Assert.IsNotNull(homeAddress);
                        Assert.AreEqual("Tokyo", homeAddress.Properties.Single(p => p.Name == "City").Value);
                        Assert.AreEqual("Cats", homeAddress.Properties.Single(p => p.Name == "FamilyName").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryPropertyUnderDerivedComplexTypeProperty()
        {
            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in types)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + @"People(1)/HomeAddress/Microsoft.Test.OData.Services.ODataWCFService.HomeAddress/FamilyName", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty property = messageReader.ReadProperty();
                        Assert.IsNotNull(property);
                        Assert.AreEqual("Cats", property.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void FilterByPropertyUnderDerivedComplexTypeProperty()
        {
            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in types)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + @"People?$filter=HomeAddress/Microsoft.Test.OData.Services.ODataWCFService.HomeAddress/FamilyName eq 'Cats'", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataFeedReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.AreEqual(1, entry.Properties.Single(p => p.Name == "PersonID").Value);
                                ODataComplexValue homeAddress = (entry.Properties.Single(p => p.Name == "HomeAddress").Value as ODataComplexValue);
                                Assert.IsNotNull(homeAddress);
                                string typeName = NameSpacePrefix + "HomeAddress";
                                Assert.AreEqual(typeName, homeAddress.TypeName);
                                Assert.AreEqual("Cats", homeAddress.Properties.Single(p => p.Name == "FamilyName").Value);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void UpdateDerivedComplexTypeProperty()
        {
            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            string[] familyNames = { "Cats", "Tigers", "Lions", "Finals" };
            for (int i = 0; i < types.Length; i++)
            {
                ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

                ODataEntry entry = this.QueryEntry("People(1)");
                ODataComplexValue homeAddress = (entry.Properties.Single(p => p.Name == "HomeAddress").Value as ODataComplexValue);
                Assert.AreEqual(familyNames[i], homeAddress.Properties.Single(p => p.Name == "FamilyName").Value);

                //update
                entry = new ODataEntry() { TypeName = NameSpacePrefix + "Person" };
                entry.Properties = new[] 
                {
                    new ODataProperty
                    {
                        Name = "HomeAddress",
                        Value = new ODataComplexValue
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
                                    Value = familyNames[i+1]
                                }
                            }
                        }
                    }
                };
                var settings = new ODataMessageWriterSettings();
                settings.PayloadBaseUri = ServiceBaseUri;
                settings.AutoComputePayloadMetadataInJson = true;

                var personType = Model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
                var personSet = Model.EntityContainer.FindEntitySet("People");

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "People(1)"));
                requestMessage.SetHeader("Content-Type", types[i]);
                requestMessage.SetHeader("Accept", types[i]);
                requestMessage.Method = "PATCH";

                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(personSet, personType);
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(204, responseMessage.StatusCode);

                // verify updated value
                entry = this.QueryEntry("People(1)");
                ODataComplexValue updatedHomeAddress = (entry.Properties.Single(p => p.Name == "HomeAddress").Value as ODataComplexValue);
                Assert.AreEqual("Chengdu", updatedHomeAddress.Properties.Single(p => p.Name == "City").Value);
                Assert.AreEqual(familyNames[i + 1], updatedHomeAddress.Properties.Single(p => p.Name == "FamilyName").Value);
            }
        }

        [TestMethod]
        public void InsertDeleteEntityWithDerivedComplexTypeProperty()
        {
            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            string[] familyNames = { "Cats", "Tigers", "Lions", "Finals" };
            for (int i = 0; i < types.Length; i++)
            {
                // insert
                var entry = new ODataEntry() { TypeName = NameSpacePrefix + "Person" };
                entry.Properties = new[]
                {
                    new ODataProperty { Name = "PersonID", Value = 101 },
                    new ODataProperty { Name = "FirstName", Value = "Peter" },
                    new ODataProperty { Name = "LastName", Value = "Zhang" },
                    new ODataProperty
                    {
                        Name = "HomeAddress",
                        Value = new ODataComplexValue
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
                    },
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
                };

                var settings = new ODataMessageWriterSettings();
                settings.PayloadBaseUri = ServiceBaseUri;

                var personType = Model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
                var peopleSet = Model.EntityContainer.FindEntitySet("People");

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "People"));
                requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
                requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
                requestMessage.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(peopleSet, personType);
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                }

                var responseMessage = requestMessage.GetResponse();

                // verify the insert
                Assert.AreEqual(201, responseMessage.StatusCode);
                entry = this.QueryEntry("People(101)");
                Assert.AreEqual(101, entry.Properties.Single(p => p.Name == "PersonID").Value);

                ODataComplexValue homeAddress = (entry.Properties.Single(p => p.Name == "HomeAddress").Value as ODataComplexValue);
                Assert.AreEqual("Chengdu", homeAddress.Properties.Single(p => p.Name == "City").Value);
                Assert.AreEqual("Tigers", homeAddress.Properties.Single(p => p.Name == "FamilyName").Value);

                // delete the entry
                var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "People(101)"));
                deleteRequestMessage.Method = "DELETE";
                var deleteResponseMessage = deleteRequestMessage.GetResponse();

                // verify the delete
                Assert.AreEqual(204, deleteResponseMessage.StatusCode);
                ODataEntry deletedEntry = this.QueryEntry("People(101)", 204);
                Assert.IsNull(deletedEntry);
            }
        }

        [TestMethod]
        public void FunctionReturnDerivedComplexType()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            foreach (var mimeType in types)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + @"People(1)/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty property = messageReader.ReadProperty();
                        ODataComplexValue homeAddress = property.Value as ODataComplexValue;
                        Assert.IsNotNull(homeAddress);
                        Assert.AreEqual("Tokyo", homeAddress.Properties.Single(p => p.Name == "City").Value);
                        Assert.AreEqual("Cats", homeAddress.Properties.Single(p => p.Name == "FamilyName").Value);
                    }
                }

                requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + @"People(3)/Microsoft.Test.OData.Services.ODataWCFService.GetHomeAddress", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);

                responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty property = messageReader.ReadProperty();
                        ODataComplexValue homeAddress = property.Value as ODataComplexValue;
                        Assert.IsNotNull(homeAddress);
                        Assert.AreEqual("Sydney", homeAddress.Properties.Single(p => p.Name == "City").Value);
                        Assert.AreEqual("Zips", homeAddress.Properties.Single(p => p.Name == "FamilyName").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryPropertyUnderDerivedComplexTypeClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var familyName = TestClientContext.People.Where(p => p.PersonID == 1).Select(p => (p.HomeAddress as HomeAddress).FamilyName).Single();
            Assert.IsNotNull(familyName);
            Assert.AreEqual("Cats", familyName);
        }

        [TestMethod]
        public void TopLevelQueryDerivedComplexTypePropertyClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            var address = TestClientContext.People.Where(p => p.PersonID == 1).Select(p => p.HomeAddress).Single();

            var homeAddress = address as HomeAddress;
            Assert.IsNotNull(homeAddress);
            Assert.AreEqual("Cats", homeAddress.FamilyName);
        }

        [TestMethod]
        public void QueryOptionOnDerivedComplexTypePropertyClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            var queryable0 = TestClientContext.People.Where(p => (p.HomeAddress as HomeAddress).FamilyName == "Cats");

            Person personResult = queryable0.Single();
            Assert.IsNotNull(personResult);
            HomeAddress homeAddress = personResult.HomeAddress as HomeAddress;
            Assert.IsNotNull(homeAddress);
            Assert.AreEqual(1, personResult.PersonID);
            Assert.AreEqual("Cats", homeAddress.FamilyName);
        }

        [TestMethod]
        public void UpdateDerivedComplexTypeClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var person = TestClientContext.People.Where(p => p.PersonID == 1).Single();
            Assert.IsNotNull(person);

            var homeAddress = person.HomeAddress as Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.HomeAddress;
            Assert.IsNotNull(homeAddress);
            Assert.AreEqual("Cats", homeAddress.FamilyName);
            homeAddress.City = "Shanghai";
            homeAddress.FamilyName = "Tigers";

            TestClientContext.UpdateObject(person);
            TestClientContext.SaveChanges(Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate);

            var updatedPerson = TestClientContext.People.Where(p => p.PersonID == 1).Single();
            var updatedHomeAddress = updatedPerson.HomeAddress as Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.HomeAddress;
            Assert.IsNotNull(updatedHomeAddress);
            Assert.AreEqual("Shanghai", updatedHomeAddress.City);
            Assert.AreEqual("Tigers", updatedHomeAddress.FamilyName);
        }

        [TestMethod]
        public void InsertDeleteEntityWithDerivedComplexTypePropertyClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            // create an an account entity and a contained PI entity
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
            TestClientContext.SaveChanges();

            var queryable0 = TestClientContext.People.Where(person => person.PersonID == 10001);
            Person personResult = queryable0.First();
            Assert.IsNotNull(personResult);
            HomeAddress homeAddress = personResult.HomeAddress as HomeAddress;
            Assert.IsNotNull(homeAddress);
            Assert.AreEqual("New", homeAddress.FamilyName);

            // delete
            var personToDelete = TestClientContext.People.Where(person => person.PersonID == 10001).Single();
            TestClientContext.DeleteObject(personToDelete);
            TestClientContext.SaveChanges();
            var People = TestClientContext.People.ToList();
            var queryDeletedPerson = People.Where(person => person.PersonID == 10001);
            Assert.AreEqual(0, queryDeletedPerson.Count());
        }

        #endregion

        #region Open complex type

        [TestMethod]
        public void QueryEntityWithOpenComplexTypeProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                ODataComplexValue accountInfo = (entry.Properties.Single(p => p.Name == "AccountInfo").Value as ODataComplexValue);
                                Assert.IsNotNull(accountInfo);
                                string typeName = NameSpacePrefix + "AccountInfo";
                                Assert.AreEqual(typeName, accountInfo.TypeName);
                                Assert.AreEqual("Hood", accountInfo.Properties.Single(p => p.Name == "MiddleName").Value);

                                var colorODataEnumValue = accountInfo.Properties.Single(p => p.Name == "FavoriteColor").Value as ODataEnumValue;
                                string colorTypeName = NameSpacePrefix + "Color";
                                Assert.AreEqual(colorTypeName, colorODataEnumValue.TypeName);
                                Assert.AreEqual("Red", colorODataEnumValue.Value);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryOpenPropertyUnderOpenComplexType()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            foreach (var mimeType in types)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)/AccountInfo/MiddleName", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty middleNameProperty = messageReader.ReadProperty();
                        Assert.AreEqual("Hood", middleNameProperty.Value);
                    }
                }
            }
        }

        [TestMethod]
        public void FilterByAnOpenProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            foreach (var mimeType in types)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + @"Accounts?$filter=AccountInfo/MiddleName eq 'Hood'", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataFeedReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                int? accountID = entry.Properties.Single(p => p.Name == "AccountID").Value as int?;
                                Assert.AreEqual(101, accountID);
                                ODataComplexValue accountInfo = (entry.Properties.Single(p => p.Name == "AccountInfo").Value as ODataComplexValue);
                                Assert.IsNotNull(accountInfo);
                                string typeName = NameSpacePrefix + "AccountInfo";
                                Assert.AreEqual(typeName, accountInfo.TypeName);
                                Assert.AreEqual("Hood", accountInfo.Properties.Single(p => p.Name == "MiddleName").Value);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void UpdateOpenComplexTypeProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            string[] middleNames = { "Hood", "MN1", "MN2", "MN3" };
            for (int i = 0; i < types.Length; i++)
            {
                ODataEntry entry = this.QueryEntry("Accounts(101)");
                ODataComplexValue accountInfo = (entry.Properties.Single(p => p.Name == "AccountInfo").Value as ODataComplexValue);
                Assert.AreEqual(middleNames[i], accountInfo.Properties.Single(p => p.Name == "MiddleName").Value);

                // update
                bool isPersonalAccount = (i % 2 == 0);
                entry = new ODataEntry() { TypeName = NameSpacePrefix + "Account" };
                entry.Properties = new[] 
                {
                    new ODataProperty
                    {
                        Name = "AccountInfo",
                        Value = new ODataComplexValue
                        {
                            TypeName = NameSpacePrefix + "AccountInfo",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "FirstName",
                                    Value = "FN"
                                },
                                new ODataProperty
                                {
                                    Name = "LastName",
                                    Value = "LN"
                                },
                                new ODataProperty
                                {
                                    Name = "MiddleName",
                                    Value = middleNames[i+1]
                                },
                                new ODataProperty
                                {
                                    Name = "IsPersonalAccount",
                                    Value = isPersonalAccount
                                }
                            }
                        }
                    }
                };
                var settings = new ODataMessageWriterSettings();
                settings.PayloadBaseUri = ServiceBaseUri;
                settings.AutoComputePayloadMetadataInJson = true;

                var accountType = Model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
                var accountSet = Model.EntityContainer.FindEntitySet("Accounts");

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(101)"));
                requestMessage.SetHeader("Content-Type", types[i]);
                requestMessage.SetHeader("Accept", types[i]);
                requestMessage.Method = "PATCH";

                using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(accountSet, accountType);
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(204, responseMessage.StatusCode);

                // verify updated value
                entry = this.QueryEntry("Accounts(101)");
                var updatedAccountInfo = (entry.Properties.Single(p => p.Name == "AccountInfo").Value as ODataComplexValue);
                Assert.AreEqual("FN", updatedAccountInfo.Properties.Single(p => p.Name == "FirstName").Value);
                Assert.AreEqual(middleNames[i + 1], updatedAccountInfo.Properties.Single(p => p.Name == "MiddleName").Value);
                Assert.AreEqual(isPersonalAccount, updatedAccountInfo.Properties.Single(p => p.Name == "IsPersonalAccount").Value);
            }
        }

        [TestMethod]
        public void InsertDeleteEntityWithOpenComplexTypeProperty()
        {
            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            foreach (var mimetype in types)
            {
                var entry = new ODataEntry() { TypeName = NameSpacePrefix + "Account" };
                entry.Properties = new[]
                {
                    new ODataProperty { Name = "AccountID", Value = 10086 },
                    new ODataProperty { Name = "CountryRegion", Value = "CN" },                
                    new ODataProperty
                    {
                        Name = "AccountInfo",
                        Value = new ODataComplexValue
                        {
                            TypeName = NameSpacePrefix + "AccountInfo",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "FirstName",
                                    Value = "Peter"
                                },
                                new ODataProperty
                                {
                                    Name = "LastName",
                                    Value = "Andy"
                                },
                                new ODataProperty
                                {
                                    Name = "ShippingAddress",
                                    Value = "#999, ZiXing Road"
                                }
                            }
                        }
                    }
                };

                var settings = new ODataMessageWriterSettings();
                settings.PayloadBaseUri = ServiceBaseUri;

                var accountType = Model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
                var accountSet = Model.EntityContainer.FindEntitySet("Accounts");

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts"));
                requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
                requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
                requestMessage.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(accountSet, accountType);
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                }

                var responseMessage = requestMessage.GetResponse();

                // verify the insert
                Assert.AreEqual(201, responseMessage.StatusCode);
                entry = this.QueryEntry("Accounts(10086)");
                Assert.AreEqual(10086, entry.Properties.Single(p => p.Name == "AccountID").Value);

                ODataComplexValue accountInfo = (entry.Properties.Single(p => p.Name == "AccountInfo").Value as ODataComplexValue);
                Assert.AreEqual("Peter", accountInfo.Properties.Single(p => p.Name == "FirstName").Value);
                Assert.AreEqual("#999, ZiXing Road", accountInfo.Properties.Single(p => p.Name == "ShippingAddress").Value);

                // delete the entry
                var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(10086)"));
                deleteRequestMessage.Method = "DELETE";
                var deleteResponseMessage = deleteRequestMessage.GetResponse();

                // verify the delete
                Assert.AreEqual(204, deleteResponseMessage.StatusCode);
                ODataEntry deletedEntry = this.QueryEntry("Accounts(10086)", 204);
                Assert.IsNull(deletedEntry);
            }
        }

        [TestMethod]
        public void FunctionReturnOpenComplexType()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            foreach (var mimetype in types)
            {
                var mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + @"Accounts(101)/Microsoft.Test.OData.Services.ODataWCFService.GetAccountInfo", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty property = messageReader.ReadProperty();
                        ODataComplexValue accountInfo = property.Value as ODataComplexValue;
                        Assert.IsNotNull(accountInfo);
                        Assert.AreEqual("Alex", accountInfo.Properties.Single(p => p.Name == "FirstName").Value);
                        Assert.AreEqual("Hood", accountInfo.Properties.Single(p => p.Name == "MiddleName").Value);
                    }
                }

                requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + @"Accounts(103)/Microsoft.Test.OData.Services.ODataWCFService.GetAccountInfo", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);

                responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataProperty property = messageReader.ReadProperty();
                        ODataComplexValue accountInfo = property.Value as ODataComplexValue;
                        Assert.IsNotNull(accountInfo);
                        Assert.AreEqual("Adam", accountInfo.Properties.Single(p => p.Name == "FirstName").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryOpenComplexTypeClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var accountInfo = TestClientContext.Accounts.Where(a => a.AccountID == 101).Select(a => a.AccountInfo).Single();

            Assert.IsNotNull(accountInfo);
            Assert.AreEqual("Hood", accountInfo.MiddleName);

            var middleName = TestClientContext.Accounts.Where(a => a.AccountID == 101).Select(a => a.AccountInfo.MiddleName).Single();
            Assert.AreEqual("Hood", middleName);
        }

        [TestMethod]
        public void UpdateOpenComplexTypeClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var account = TestClientContext.Accounts.Where(a => a.AccountID == 101).Single();
            Assert.IsNotNull(account);

            var accountInfo = account.AccountInfo;
            Assert.IsNotNull(accountInfo);
            Assert.AreEqual("Hood", accountInfo.MiddleName);

            accountInfo.FirstName = "Peter";
            accountInfo.MiddleName = "White";

            // verify: open type enum property from client side should be serialized with odata.type
            accountInfo.FavoriteColor = Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.Color.Blue;
            accountInfo.Address = new Address
            {
                Street = "1",
                City = "2",
                PostalCode = "3"
            };

            TestClientContext.UpdateObject(account);
            TestClientContext.SaveChanges(Microsoft.OData.Client.SaveChangesOptions.ReplaceOnUpdate);

            var updatedAccount = TestClientContext.Accounts.Where(a => a.AccountID == 101).Single();
            Assert.IsNotNull(updatedAccount);

            var updatedAccountInfo = updatedAccount.AccountInfo;
            Assert.IsNotNull(updatedAccountInfo);
            Assert.AreEqual("Peter", updatedAccountInfo.FirstName);
            Assert.AreEqual("White", updatedAccountInfo.MiddleName);

            Assert.AreEqual(Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.Color.Blue, updatedAccountInfo.FavoriteColor);
            Assert.AreEqual("1", updatedAccountInfo.Address.Street);
            Assert.AreEqual("2", updatedAccountInfo.Address.City);
            Assert.AreEqual("3", updatedAccountInfo.Address.PostalCode);
        }

        [TestMethod]
        public void UpdateOpenComplexTypeWithUndeclaredPropertiesClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            TestClientContext.MergeOption = ODataClient.MergeOption.OverwriteChanges;
            TestClientContext.Configurations.RequestPipeline.OnEntryStarting(ea => EntryStarting(ea, TestClientContext));
            var account = TestClientContext.Accounts.Where(a => a.AccountID == 101).First();

            // In practice, transient property data would be mutated here in the partial companion to the client proxy.

            TestClientContext.UpdateObject(account);
            TestClientContext.SaveChanges();
            // No more check, this case is to make sure that client doesn't throw exception.
        }

        [TestMethod]
        public void InsertDeleteEntityWithOpenComplexTypePropertyClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            Account newAccount = new Account()
            {
                AccountID = 110,
                CountryRegion = "CN",
                AccountInfo = new AccountInfo()
                {
                    FirstName = "New",
                    LastName = "Guy",
                    MiddleName = "Howard",
                    //FavoriteColor = Color.Red
                }
            };
            TestClientContext.AddToAccounts(newAccount);
            TestClientContext.SaveChanges();

            var queryable0 = TestClientContext.Accounts.Where(account => account.AccountID == 110);
            Account accountResult = queryable0.Single();
            Assert.IsNotNull(accountResult);
            Assert.AreEqual("Howard", accountResult.AccountInfo.MiddleName);

            //Assert.AreEqual(Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.Color.Red,
            //    accountResult.AccountInfo.FavoriteColor);

            // delete
            var accountToDelete = TestClientContext.Accounts.Where(account => account.AccountID == 110).Single();
            TestClientContext.DeleteObject(accountToDelete);
            TestClientContext.SaveChanges();

            var accounts = TestClientContext.Accounts.ToList();
            var queryDeletedAccount = accounts.Where(account => account.AccountID == 110);
            Assert.AreEqual(0, queryDeletedAccount.Count());
        }

        [TestMethod]
        public void DeleteInsertEntityWithOpenComplexTypePropertyClientTest()
        {
            TestClientContext.Format.UseJson(Model);

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var account = TestClientContext.Accounts.Where(a => a.AccountID == 101).Single();

            // delete the entry
            var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(101)"));
            deleteRequestMessage.Method = "DELETE";
            var deleteResponseMessage = deleteRequestMessage.GetResponse();

            var entry = new ODataEntry() { TypeName = NameSpacePrefix + "Account" };
            entry.Properties = new[]
            {
                new ODataProperty { Name = "AccountID", Value = 101 },
                new ODataProperty { Name = "CountryRegion", Value = "CN" },                
                new ODataProperty
                {
                    Name = "AccountInfo",
                    Value = new ODataComplexValue
                    {
                        TypeName = NameSpacePrefix + "AccountInfo",
                        Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "FirstName",
                                Value = "Peter"
                            },
                            new ODataProperty
                            {
                                Name = "LastName",
                                Value = "Andy"
                            }
                        }
                    }
                }
            };

            var settings = new ODataMessageWriterSettings();

            var accountType = Model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
            var accountSet = Model.EntityContainer.FindEntitySet("Accounts");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "POST";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataEntryWriter(accountSet, accountType);
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();
            var updatedAccount = TestClientContext.Accounts.Where(a => a.AccountID == 101).Single();
            var info = updatedAccount.AccountInfo;
            Assert.AreEqual(null, info.MiddleName);

        }
        #endregion

        #region open collection property

        [TestMethod]
        public void OpenCollectionPropertyRoundTrip()
        {
            var mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;

            // update
            var accountInfo = new ODataProperty
            {
                Name = "AccountInfo",
                Value = new ODataComplexValue
                {
                    TypeName = NameSpacePrefix + "AccountInfo",
                    Properties = new[]
                    {
                        new ODataProperty
                        {
                            Name = "FirstName",
                            Value = "FN"
                        },
                        new ODataProperty
                        {
                            Name = "FavoriteFood",                            
                            Value = new ODataCollectionValue()
                            {
                                TypeName = "Collection(Edm.String)",
                                Items = new[] {"meat", "sea food", "apple"}
                            }
                        }
                    }
                }
            };

            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;
            settings.AutoComputePayloadMetadataInJson = true;

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(101)/AccountInfo"));
            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);
            requestMessage.Method = "PATCH";

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
            {
                messageWriter.WriteProperty(accountInfo);
            }

            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(204, responseMessage.StatusCode);

            // verify updated value
            var entry = this.QueryEntry("Accounts(101)");
            var updatedAccountInfo = (entry.Properties.Single(p => p.Name == "AccountInfo").Value as ODataComplexValue);
            Assert.AreEqual("FN", updatedAccountInfo.Properties.Single(p => p.Name == "FirstName").Value);
            Assert.AreEqual("Green", updatedAccountInfo.Properties.Single(p => p.Name == "LastName").Value);
            Assert.AreEqual("Hood", updatedAccountInfo.Properties.Single(p => p.Name == "MiddleName").Value);
            var favoriteFood = updatedAccountInfo.Properties.Single(p => p.Name == "FavoriteFood").Value as ODataCollectionValue;
            // validate items of favoriteFood.
            string[] expectedFavoriteFood = new[] { "meat", "sea food", "apple" };

            int index = 0;
            foreach (var item in favoriteFood.Items)
            {
                Assert.AreEqual(expectedFavoriteFood[index++], item);
            }
            Assert.AreEqual(3, index);
        }

        #endregion

        #endregion


        #region Helper

        private ODataEntry QueryEntry(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.AreEqual(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataEntry entry = null;
            if (expectedStatusCode == 200)
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataEntryReader();
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
                        {
                            entry = reader.Item as ODataEntry;
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }

            return entry;
        }

        private void EntryStarting(Microsoft.OData.Client.WritingEntryArgs ea, InMemoryEntities context)
        {
            var entityState = context.Entities.First(e => e.Entity == ea.Entity).State;

            // Send up an undeclared property on an Open Type.
            if (entityState == ODataClient.EntityStates.Modified || entityState == ODataClient.EntityStates.Added)
            {

                // Send up an undeclared property on an Open Type.
                if (ea.Entity.GetType() == typeof(Account))
                {
                    var accountInfoProperty = ea.Entry.Properties.FirstOrDefault(e => e.Name == "AccountInfo");
                    var accountInfoComplexValueProperties = ((ODataComplexValue)accountInfoProperty.Value).Properties as List<ODataProperty>;

                    // In practice, the data from this undeclared property would probably be stored in a transient property of the partial companion class to the client proxy.
                    var undeclaredOdataProperty = new ODataProperty() { Name = "dynamicPropertyKey", Value = "dynamicPropertyValue" };
                    accountInfoComplexValueProperties.Add(undeclaredOdataProperty);
                }
            }
        }

        #endregion
    }
}
