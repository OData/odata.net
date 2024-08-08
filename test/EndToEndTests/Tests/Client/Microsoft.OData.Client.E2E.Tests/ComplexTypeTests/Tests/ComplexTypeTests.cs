//-----------------------------------------------------------------------------
// <copyright file="ComplexTypeTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.ComplexTypeTests.Server;
using Microsoft.OData.Edm;
using System.Reflection;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ComplexTypeTests.Tests
{
    public class ComplexTypeTests : EndToEndTestBase<ComplexTypeTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private IEdmModel _model = null;
        private const string NameSpacePrefix = "Microsoft.OData.Client.E2E.Tests.Common.Server.Default.";

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ComplexTypeTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }
        public ComplexTypeTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
            _model = DefaultEdmModel.GetEdmModel();
        }

        protected readonly string[] mimeTypes =
        [
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
        ];

        [Fact]
        public void QueryingEntityWithDerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            foreach (var mimeType in mimeTypes)
            {
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri(_baseUri.AbsoluteUri + "People(1)", UriKind.Absolute),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                    {
                        var reader = messageReader.CreateODataResourceReader();
                        bool startHomeAddress = false;
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.NestedResourceInfoStart)
                            {
                                ODataNestedResourceInfo navigation = reader.Item as ODataNestedResourceInfo;
                                if (navigation != null && navigation.Name == "HomeAddress")
                                {
                                    startHomeAddress = true;
                                }
                            }
                            else if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                if (startHomeAddress)
                                {
                                    Assert.NotNull(entry);
                                    string typeName = NameSpacePrefix + "HomeAddress";
                                    Assert.Equal(typeName, entry.TypeName);
                                    Assert.Equal("Cats", entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);
                                    startHomeAddress = false;
                                }
                            }
                        }

                        Assert.Equal(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [Fact]
        public void QueryingADerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
            {
                BaseUri = _baseUri,
                Validations = ValidationKinds.All & ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
            };

            foreach (var mimeType in mimeTypes)
            {
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri(_baseUri.AbsoluteUri + "People(1)/HomeAddress", UriKind.Absolute),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                    {
                        var resourceReader = messageReader.CreateODataResourceReader();
                        while (resourceReader.Read())
                        {
                            if (resourceReader.State == ODataReaderState.ResourceEnd)
                            {
                                var homeAddress = resourceReader.Item as ODataResource;
                                Assert.NotNull(homeAddress);
                                Assert.Equal("Tokyo", homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "City").Value);
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void QueryPropertyUnderDerivedComplexTypeProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            foreach (var mimeType in mimeTypes)
            {
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri(_baseUri.AbsoluteUri + "People(1)/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.HomeAddress/FamilyName", UriKind.Absolute),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                    {
                        ODataProperty property = messageReader.ReadProperty();
                        Assert.NotNull(property);
                        Assert.Equal("Cats", property.Value);
                    }
                }
            }
        }

        [Fact]
        public void FilterByPropertyUnderDerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            foreach (var mimeType in mimeTypes)
            {
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri(_baseUri.AbsoluteUri + "People?$filter=HomeAddress/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.HomeAddress/FamilyName eq 'Cats'", UriKind.Absolute),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();
                        bool startHomeAddress = false;
                        int depth = 0;
                        while (reader.Read())
                        {
                            switch (reader.State)
                            {
                                case ODataReaderState.NestedResourceInfoStart:
                                    depth++;
                                    ODataNestedResourceInfo navigation = reader.Item as ODataNestedResourceInfo;
                                    if (navigation != null && navigation.Name == "HomeAddress")
                                    {
                                        startHomeAddress = true;
                                    }
                                    break;
                                case ODataReaderState.NestedResourceInfoEnd:
                                    depth--;
                                    break;
                                case ODataReaderState.ResourceEnd:
                                    {
                                        ODataResource entry = reader.Item as ODataResource;
                                        if (entry != null)
                                        {
                                            if (startHomeAddress)
                                            {
                                                Assert.NotNull(entry);
                                                string typeName = NameSpacePrefix + "HomeAddress";
                                                Assert.Equal(typeName, entry.TypeName);
                                                Assert.Equal("Cats", entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);
                                                startHomeAddress = false;
                                            }
                                            if (depth == 0)
                                            {
                                                Assert.Equal(1, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "PersonID").Value);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }

                        Assert.Equal(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [Fact]
        public void UpdatingDerivedComplexTypeProperty_UpdatesSuccessfully()
        {
            string[] familyNames = { "Cats", "Tigers", "Lions", "Finals" };
            for (int i = 0; i < mimeTypes.Length; i++)
            {
                ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

                List<ODataResource> complexTypeResources = null;
                ODataResource entry = this.QueryEntry("People(1)", out complexTypeResources);
                var homeAddress = complexTypeResources.Last(a => a.TypeName.EndsWith("HomeAddress"));
                Assert.Equal(familyNames[i], homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);

                //update
                var entryWrapper = new ODataResourceWrapper()
                {
                    Resource = new ODataResource() { TypeName = NameSpacePrefix + "Person" },
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
                                            Value = familyNames[i+1]
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                var settings = new ODataMessageWriterSettings();
                settings.BaseUri = _baseUri;
                var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
                var personSet = _model.EntityContainer.FindEntitySet("People");

                var args = new DataServiceClientRequestMessageArgs(
                    "PATCH",
                    new Uri(_baseUri + "People(1)"),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Content-Type", mimeTypes[i]);
                requestMessage.SetHeader("Accept", mimeTypes[i]);

                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(personSet, personType);
                    ODataWriterHelper.WriteResource(odataWriter, entryWrapper);
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(204, responseMessage.StatusCode);

                // verify updated value
                entry = this.QueryEntry("People(1)", out complexTypeResources);
                var updatedHomeAddress = complexTypeResources.Last(a => a.TypeName.EndsWith("HomeAddress"));
                Assert.Equal("Chengdu", updatedHomeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "City").Value);
                Assert.Equal(familyNames[i + 1], updatedHomeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);
            }
        }

        [Fact]
        public void InsertDeleteEntityWithDerivedComplexTypePropertyClientTest()
        {
            _context.Format.UseJson(_model);

            // create an an account entity and a contained PI entity
            Common.Client.Default.Person newPerson = new Common.Client.Default.Person()
            {
                PersonID = 10001,
                FirstName = "New",
                LastName = "Person",
                MiddleName = "Guy",
                //Home = GeographyPoint.Create(32.1, 23.1),
                /*HomeAddress = new Common.Client.Default.HomeAddress
                {
                    City = "Shanghai",
                    Street = "Zixing Rd",
                    PostalCode = "200241",
                    FamilyName = "New"
                }*/
            };
            _context.AddToPeople(newPerson);
            _context.SaveChanges();

            var queryable0 = _context.People.Where(person => person.PersonID == 10001);
            var personResult = queryable0.First();
            Assert.NotNull(personResult);
            var homeAddress = personResult.HomeAddress as Common.Client.Default.HomeAddress;
            Assert.NotNull(homeAddress);
            Assert.Equal("New", homeAddress.FamilyName);

            // delete
            var personToDelete = _context.People.Where(person => person.PersonID == 10001).Single();
            _context.DeleteObject(personToDelete);
            _context.SaveChanges();
            var People = _context.People.ToList();
            var queryDeletedPerson = People.Where(person => person.PersonID == 10001);
            Assert.Equal(0, queryDeletedPerson.Count());
        }

        [Fact]
        public void FunctionReturnDerivedComplexType()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            foreach (var mimeType in mimeTypes)
            {
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri(_baseUri.AbsoluteUri + "People(1)/Default.GetHomeAddress", UriKind.Absolute),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Accept", mimeType);

                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                    {
                        var odataReader = messageReader.CreateODataResourceReader();
                        while (odataReader.Read())
                        {
                            if (odataReader.State == ODataReaderState.ResourceEnd)
                            {
                                var homeAddress = odataReader.Item as ODataResource;
                                Assert.NotNull(homeAddress);
                                Assert.Equal("Tokyo", homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "City").Value);
                                Assert.Equal("Cats", homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);
                            }
                        }
                    }
                }

                requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Accept", mimeType);

                responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                    {
                        var odataReader = messageReader.CreateODataResourceReader();
                        while (odataReader.Read())
                        {
                            if (odataReader.State == ODataReaderState.ResourceEnd)
                            {
                                var homeAddress = odataReader.Item as ODataResource;
                                Assert.NotNull(homeAddress);
                                Assert.Equal("Sydney", homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "City").Value);
                                Assert.Equal("Zips", homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void QueryEntityWithOpenComplexTypeProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            foreach (var mimeType in mimeTypes)
            {
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                    new Uri(_baseUri.AbsoluteUri + "Accounts(101)", UriKind.Absolute),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                    {
                        var reader = messageReader.CreateODataResourceReader();

                        ODataResource account = null;
                        ODataResource accountInfo = null;
                        ODataResource addressComplex1 = null;
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceStart)
                            {
                                if (account == null)
                                {
                                    account = reader.Item as ODataResource;
                                }
                                else if (accountInfo == null)
                                {
                                    accountInfo = reader.Item as ODataResource;
                                }
                                else if (addressComplex1 == null)
                                {
                                    addressComplex1 = reader.Item as ODataResource;
                                }
                            }
                        }

                        Assert.NotNull(accountInfo);
                        string typeName = NameSpacePrefix + "AccountInfo";
                        Assert.Equal(typeName, accountInfo.TypeName);
                        Assert.Equal("\"Hood\"", (accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value as ODataUntypedValue).RawValue);

                        var colorODataEnumValue = accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FavoriteColor").Value as ODataEnumValue;
                        string colorTypeName = NameSpacePrefix + "Color";
                        Assert.Equal(colorTypeName, colorODataEnumValue.TypeName);
                        Assert.Equal("Red", colorODataEnumValue.Value);
                        Assert.Equal(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [Fact]
        public void QueryingOpenPropertyUnderOpenComplexType_ExecutesSuccessfully()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            foreach (var mimeType in mimeTypes)
            {
                var args = new DataServiceClientRequestMessageArgs(
                    "GET",
                   new Uri(_baseUri.AbsoluteUri + "Accounts(101)/AccountInfo/MiddleName", UriKind.Absolute),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
                    {
                        ODataProperty middleNameProperty = messageReader.ReadProperty();
                        Assert.Equal("Hood", middleNameProperty.Value);
                    }
                }
            }
        }

        [Fact]
        public void TopLevelQueryDerivedComplexTypePropertyClientTest()
        {
            _context.Format.UseJson(_model);

            var address = _context.People.Where(p => p.PersonID == 1).Select(p => p.HomeAddress).Single();

            var homeAddress = address as Common.Client.Default.HomeAddress;
            Assert.NotNull(homeAddress);
            Assert.Equal("Cats", homeAddress.FamilyName);
        }

        [Fact]
        public void QueryOptionOnDerivedComplexTypePropertyClientTest()
        {
            _context.Format.UseJson(_model);

            var queryable0 = _context.People.Where(p => (p.HomeAddress as Common.Client.Default.HomeAddress).FamilyName == "Cats");

            Common.Client.Default.Person personResult = queryable0.Single();
            Assert.NotNull(personResult);
            Common.Client.Default.HomeAddress homeAddress = personResult.HomeAddress as Common.Client.Default.HomeAddress;
            Assert.NotNull(homeAddress);
            Assert.Equal(1, personResult.PersonID);
            Assert.Equal("Cats", homeAddress.FamilyName);
        }

        [Fact]
        public void UpdateDerivedComplexTypeClientTest()
        {
           // _context.Format.UseJson(_model);

            _context.MergeOption = MergeOption.OverwriteChanges;
            var people = _context.People.ToList();
            var person = _context.People.Where(p => p.PersonID == 1).Single();
            Assert.NotNull(person);

           var homeAddress = person.HomeAddress as Common.Client.Default.HomeAddress;
            Assert.NotNull(homeAddress);
            Assert.Equal("Cats", homeAddress.FamilyName);
            homeAddress.City = "Shanghai";
            homeAddress.FamilyName = "Tigers";

            _context.UpdateObject(person);
            _context.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);

            var updatedPerson = _context.People.Where(p => p.PersonID == 1).Single();
            var updatedHomeAddress = updatedPerson.HomeAddress as Common.Client.Default.HomeAddress;
            Assert.NotNull(updatedHomeAddress);
            Assert.Equal("Shanghai", updatedHomeAddress.City);
            Assert.Equal("Tigers", updatedHomeAddress.FamilyName);
        }

        private ODataResource QueryEntry(string uri, out List<ODataResource> complexProperties, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            var args = new DataServiceClientRequestMessageArgs(
                "GET",
                new Uri(_baseUri.AbsoluteUri + uri, UriKind.Absolute),
                usePostTunneling: false,
                new Dictionary<string, string>(),
                HttpClientFactory);

            var queryRequestMessage = new HttpClientRequestMessage(args);
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.Equal(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataResource entry = null;
            complexProperties = new List<ODataResource>();
            if (expectedStatusCode == 200)
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, _model))
                {
                    var reader = messageReader.CreateODataResourceReader();
                    while (reader.Read())
                    {
                        switch (reader.State)
                        {
                            case ODataReaderState.NestedResourceInfoStart:
                                break;
                            case ODataReaderState.NestedResourceInfoEnd:
                                break;
                            case ODataReaderState.ResourceSetStart:
                                break;
                            case ODataReaderState.ResourceStart:
                                {
                                    if (entry == null)
                                    {
                                        entry = reader.Item as ODataResource;
                                    }
                                    else
                                    {
                                        complexProperties.Add(reader.Item as ODataResource);
                                    }

                                    break;
                                }
                            default:
                                break;
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }

            return entry;
        }
    }
}
