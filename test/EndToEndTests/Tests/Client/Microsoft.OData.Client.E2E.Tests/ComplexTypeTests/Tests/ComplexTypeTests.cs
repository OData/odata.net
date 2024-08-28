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
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.ComplexTypeTests.Server;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ComplexTypeTests.Tests
{
    public class ComplexTypeTests : EndToEndTestBase<ComplexTypeTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;
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

            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDefaultDataSource();
        }

        public static IEnumerable<object[]> MimeTypesData
        {
            get
            {
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
            }
        }

        #region Complex type inheritance

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAnEntityWithADerivedComplexTypeProperty_ExecutesSuccessfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            var requestUri = new Uri(_baseUri.AbsoluteUri + "People(1)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();
                bool startHomeAddress = false;

                while (await reader.ReadAsync())
                {
                    if (reader.State == ODataReaderState.NestedResourceInfoStart)
                    {
                        if (reader.Item is ODataNestedResourceInfo navigation && navigation.Name == "HomeAddress")
                        {
                            startHomeAddress = true;
                        }
                    }
                    else if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        ODataResource? entry = reader.Item as ODataResource;

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

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingADerivedComplexTypeProperty_ExecutesSuccessfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
            {
                BaseUri = _baseUri,
                Validations = ValidationKinds.All & ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
            };

            Uri requestUri = new(_baseUri.AbsoluteUri + "People(1)/HomeAddress", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var resourceReader = await messageReader.CreateODataResourceReaderAsync();

                while (await resourceReader.ReadAsync())
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

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAPropertyOfADerivedComplexTypeProperty_ExecutesSuccessfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            Uri requestUri = new(_baseUri.AbsoluteUri + "People(1)/HomeAddress/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.HomeAddress/FamilyName", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                ODataProperty property = await messageReader.ReadPropertyAsync();

                Assert.NotNull(property);
                Assert.Equal("Cats", property.Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task FilterByAPropertyOfADerivedComplexTypeProperty_ExecutesSuccessfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            Uri requestUri = new(_baseUri.AbsoluteUri + "People?$filter=HomeAddress/Microsoft.OData.Client.E2E.Tests.Common.Server.Default.HomeAddress/FamilyName eq 'Cats'", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceSetReaderAsync();
                bool startHomeAddress = false;
                int depth = 0;

                while (await reader.ReadAsync())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.NestedResourceInfoStart:
                            depth++;
                            if (reader.Item is ODataNestedResourceInfo navigation && navigation.Name == "HomeAddress")
                            {
                                startHomeAddress = true;
                            }
                            break;
                        case ODataReaderState.NestedResourceInfoEnd:
                            depth--;
                            break;
                        case ODataReaderState.ResourceEnd:
                            {
                                if (reader.Item is ODataResource entry)
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

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task UpdatingADerivedComplexTypeProperty_UpdatesSuccessfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };
            (ODataResource entry, List<ODataResource> complexTypeResources) = await this.QueryEntryAsync("People(1)");

            var homeAddress = complexTypeResources.Last(a => a.TypeName.EndsWith("HomeAddress"));

            Assert.Equal("Cats", homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);

            //update
            var entryWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource() { TypeName = NameSpacePrefix + "Person" },
                NestedResourceInfoWrappers =
                [
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
                                    Properties =
                                    [
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
                                    ]
                                }
                            }
                        }
                ]
            };

            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var personSet = _model.EntityContainer.FindEntitySet("People");

            var requestUrl = new Uri(_baseUri + "People(1)");

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, base.Client)
            {
                Method = "PATCH"
            };

            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(personSet, personType);
                await ODataWriterHelper.WriteResourceAsync(odataWriter, entryWrapper);
            }

            var responseMessage = await requestMessage.GetResponseAsync();
            Assert.Equal(204, responseMessage.StatusCode);

            // verify updated value
            (entry, complexTypeResources) = await this.QueryEntryAsync("People(1)");
            var updatedHomeAddress = complexTypeResources.Last(a => a.TypeName.EndsWith("HomeAddress"));
            Assert.Equal("Chengdu", updatedHomeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "City").Value);
            Assert.Equal("Tigers", updatedHomeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);
        }

        [Fact]
        public async Task InsertingAndDeletingAnEntityWithADerivedComplexTypeProperty_WorksCorrectly()
        {
            // insert
            var entryWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = NameSpacePrefix + "Person",
                    Properties =
                    [
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
                    ]
                },
                NestedResourceInfoWrappers =
                [
                    new()
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
                                Properties =
                                [
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
                                ]
                            }
                        }
                    }
                ]
            };

            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var peopleSet = _model.EntityContainer.FindEntitySet("People");

            var requestUrl = new Uri(_baseUri + "People");

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, base.Client)
            {
                Method = "POST"
            };

            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(peopleSet, personType);
                await ODataWriterHelper.WriteResourceAsync(odataWriter, entryWrapper);
            }

            var responseMessage = await requestMessage.GetResponseAsync();

            // verify the insert
            Assert.Equal(201, responseMessage.StatusCode);

            (ODataResource entry, List<ODataResource> complexTypeResources) = await this.QueryEntryAsync("People(101)");

            Assert.Equal(101, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "PersonID").Value);

            var homeAddress = complexTypeResources.Single(r => r.TypeName.EndsWith("Address"));

            Assert.Equal("Chengdu", homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "City").Value);
            Assert.Equal("Tigers", homeAddress.Properties.OfType<ODataProperty>().Single(p => p.Name == "FamilyName").Value);

            var deleteRequestUrl = new Uri(_baseUri + "People(101)");

            var deleteRequestMessage = new TestHttpClientRequestMessage(deleteRequestUrl, base.Client)
            {
                Method = "DELETE"
            };

            var deleteResponseMessage = await deleteRequestMessage.GetResponseAsync();

            // verify the delete
            Assert.Equal(204, deleteResponseMessage.StatusCode);

            ODataResource deletedEntry = null;
            (deletedEntry, complexTypeResources) = await this.QueryEntryAsync("People(101)", 404);

            Assert.Null(deletedEntry);
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task FunctionReturns_DerivedComplexType_Successfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            Uri requestUri = new(_baseUri.AbsoluteUri + "People(1)/Default.GetHomeAddress", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var odataReader = await messageReader.CreateODataResourceReaderAsync();

                while (await odataReader.ReadAsync())
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

            requestUri = new Uri(_baseUri.AbsoluteUri + "People(3)/Default.GetHomeAddress", UriKind.Absolute);

            requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var odataReader = await messageReader.CreateODataResourceReaderAsync();

                while (await odataReader.ReadAsync())
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

        [Fact]
        public async Task QueryingAPropertyOfADerivedComplexType_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            var familyName = await _context.People.ByKey(1).Select(p => (p.HomeAddress as Common.Client.Default.HomeAddress).FamilyName).GetValueAsync();

            Assert.NotNull(familyName);
            Assert.Equal("Cats", familyName);
        }

        [Fact]
        public async Task QueryingATopLevelDerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);

            var address = await _context.People.ByKey(1).GetValueAsync();
            var homeAddress = address.HomeAddress as Common.Client.Default.HomeAddress;

            Assert.NotNull(homeAddress);
            Assert.Equal("Cats", homeAddress.FamilyName);
        }

        [Fact]
        public async Task AFilterQueryOptionOnADerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);

            var queryable0 = _context.People.Where(p => (p.HomeAddress as Common.Client.Default.HomeAddress).FamilyName == "Cats");

            Common.Client.Default.Person personResult = (await ((DataServiceQuery<Common.Client.Default.Person>)queryable0).ExecuteAsync()).Single();

            Assert.NotNull(personResult);

            Common.Client.Default.HomeAddress homeAddress = personResult.HomeAddress as Common.Client.Default.HomeAddress;

            Assert.NotNull(homeAddress);
            Assert.Equal(1, personResult.PersonID);
            Assert.Equal("Cats", homeAddress.FamilyName);
        }

        [Fact]
        public async Task UpdatingADerivedComplexType_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            var people = (await _context.People.ExecuteAsync()).ToList();
            var person = await _context.People.ByKey(1).GetValueAsync();

            Assert.NotNull(person);

            var homeAddress = person.HomeAddress as Common.Client.Default.HomeAddress;

            Assert.NotNull(homeAddress);
            Assert.Equal("Cats", homeAddress.FamilyName);

            homeAddress.City = "Shanghai";
            homeAddress.FamilyName = "Tigers";

            _context.UpdateObject(person);
            await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

            var updatedPerson = await _context.People.ByKey(1).GetValueAsync();
            var updatedHomeAddress = updatedPerson.HomeAddress as Common.Client.Default.HomeAddress;

            Assert.NotNull(updatedHomeAddress);
            Assert.Equal("Shanghai", updatedHomeAddress.City);
            Assert.Equal("Tigers", updatedHomeAddress.FamilyName);
        }

        [Fact]
        public async Task InsertingAndDeletingAnEntityWithADerivedComplexTypeProperty_ExecutesSuccessfully()
        {
            _context.Format.UseJson(_model);

            // create an an account entity and a contained PI entity
            Common.Client.Default.Person newPerson = new Common.Client.Default.Person()
            {
                PersonID = 10001,
                FirstName = "New",
                LastName = "Person",
                MiddleName = "Guy",
                Home = GeographyPoint.Create(32.1, 23.1),
                HomeAddress = new Common.Client.Default.HomeAddress
                {
                    City = "Shanghai",
                    Street = "Zixing Rd",
                    PostalCode = "200241",
                    FamilyName = "New"
                }
            };

            _context.AddToPeople(newPerson);
            await _context.SaveChangesAsync();

            var queryable0 = _context.People.ByKey(10001);
            var personResult = await queryable0.GetValueAsync();

            Assert.NotNull(personResult);

            var homeAddress = personResult.HomeAddress as Common.Client.Default.HomeAddress;

            Assert.NotNull(homeAddress);
            Assert.Equal("New", homeAddress.FamilyName);

            // delete
            var personToDelete =await _context.People.ByKey(10001).GetValueAsync();

            _context.DeleteObject(personToDelete);
            await _context.SaveChangesAsync();

            var People = (await _context.People.ExecuteAsync()).ToList();
            var queryDeletedPerson = People.Where(person => person.PersonID == 10001);

            Assert.Empty(queryDeletedPerson);
        }
        #endregion

        #region Open complex type

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAnEntityWithAnOpenComplexTypeProperty_ExecutesSuccessfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            Uri requestUri = new(_baseUri.AbsoluteUri + "Accounts(101)", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();

                ODataResource account = null;
                ODataResource accountInfo = null;
                ODataResource addressComplex1 = null;

                while (await reader.ReadAsync())
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

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task QueryingAnOpenPropertyOfAnOpenComplexType_ExecutesSuccessfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            Uri requestUri = new(_baseUri.AbsoluteUri + "Accounts(101)/AccountInfo/MiddleName", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                ODataProperty middleNameProperty = messageReader.ReadProperty();
                Assert.Equal("Hood", middleNameProperty.Value);
            }
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task FilteringByAnOpenProperty_ExecutesSuccessfully(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            Uri requestUri = new(_baseUri.AbsoluteUri + "Accounts?$filter=AccountInfo/MiddleName eq 'Hood'", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client);
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceSetReaderAsync();

                ODataResource account = null;
                ODataResource accountInfo = null;
                ODataResource addressComplex1 = null;

                while (await reader.ReadAsync())
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
                        else addressComplex1 ??= reader.Item as ODataResource;
                    }
                }

                Assert.NotNull(account);
                Assert.NotNull(accountInfo);

                string typeName = NameSpacePrefix + "AccountInfo";

                Assert.Equal(typeName, accountInfo.TypeName);
                Assert.Equal("\"Hood\"", (accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value as ODataUntypedValue).RawValue);

                int? accountID = account.Properties.OfType<ODataProperty>().Single(p => p.Name == "AccountID").Value as int?;

                Assert.Equal(101, accountID);
                Assert.NotNull(addressComplex1);
                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        [Fact]
        public async Task UpdatingAnOpenComplexTypeProperty_ExecutesSuccessfully()
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            string[] middleNames = { "Hood", "MN1", "MN2", "MN3" };
            for (int i = 0; i < types.Length; i++)
            {
                (ODataResource entry, List<ODataResource> complexTypeResources) = await this.QueryEntryAsync("Accounts(101)");

                var accountInfo = complexTypeResources.Single(a => a.TypeName.EndsWith("AccountInfo"));
                Assert.Equal("\"" + middleNames[i] + "\"", (accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value as ODataUntypedValue).RawValue);

                // update
                bool isPersonalAccount = (i % 2 == 0);
                entry = new ODataResource() { TypeName = NameSpacePrefix + "Account" };
                var accountInfo_NestedInfo = new ODataNestedResourceInfo() { Name = "AccountInfo" };
                var accountInfo_Resource = new ODataResource()
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
                };

                var settings = new ODataMessageWriterSettings();
                settings.BaseUri = _baseUri;
                var accountType = _model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
                var accountSet = _model.EntityContainer.FindEntitySet("Accounts");

                var args = new DataServiceClientRequestMessageArgs(
                    "PATCH",
                    new Uri(_baseUri + "Accounts(101)"),
                    usePostTunneling: false,
                    new Dictionary<string, string>(),
                    HttpClientFactory);

                var requestMessage = new HttpClientRequestMessage(args);
                requestMessage.SetHeader("Content-Type", types[i]);
                requestMessage.SetHeader("Accept", types[i]);

                using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(accountSet, accountType);
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteStart(accountInfo_NestedInfo);
                    odataWriter.WriteStart(accountInfo_Resource);
                    odataWriter.WriteEnd();
                    odataWriter.WriteEnd();
                    odataWriter.WriteEnd();
                }

                var responseMessage = requestMessage.GetResponse();
                Assert.Equal(204, responseMessage.StatusCode);

                // verify updated value
                (entry, complexTypeResources) = await this.QueryEntryAsync("Accounts(101)");
                var updatedAccountInfo = complexTypeResources.Single(a => a.TypeName.EndsWith("AccountInfo"));
                Assert.Equal("FN", updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
                Assert.Equal("\"" + middleNames[i + 1] + "\"", (updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value as ODataUntypedValue).RawValue);
                Assert.Equal(isPersonalAccount, updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "IsPersonalAccount").Value);
            }
        }

        [Fact]
        public async Task InsertingAndDeletingAnEntityWithAnOpenComplexTypeProperty_ExecutesSuccessfully()
        {
            var entry = new ODataResource() { TypeName = NameSpacePrefix + "Account" };
            entry.Properties = new[]
            {
                    new ODataProperty { Name = "AccountID", Value = 10086 },
                    new ODataProperty { Name = "CountryRegion", Value = "CN" },
            };

            var accountInfo_NestedInfo = new ODataNestedResourceInfo() { Name = "AccountInfo", IsCollection = false };
            var accountInfoResource = new ODataResource
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
            };

            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var accountType = _model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
            var accountSet = _model.EntityContainer.FindEntitySet("Accounts");

            var requestUri = new Uri(_baseUri + "Accounts");

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "POST"
            };

            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(accountSet, accountType);
                await odataWriter.WriteStartAsync(entry);
                await odataWriter.WriteStartAsync(accountInfo_NestedInfo);
                await odataWriter.WriteStartAsync(accountInfoResource);
                await odataWriter.WriteEndAsync();
                await odataWriter.WriteEndAsync();
                await odataWriter.WriteEndAsync();
            }

            var responseMessage = await requestMessage.GetResponseAsync();

            // verify the insert
            Assert.Equal(201, responseMessage.StatusCode);
            (entry, List<ODataResource> complexTypeResources) = await this.QueryEntryAsync("Accounts(10086)");
            Assert.Equal(10086, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "AccountID").Value);

            var accountInfo = complexTypeResources.Single(a => a.TypeName.EndsWith("AccountInfo"));
            Assert.Equal("Peter", accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
            Assert.Equal("\"#999, ZiXing Road\"", (accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShippingAddress").Value as ODataUntypedValue).RawValue);

            // delete the entry
            var deleteRequestUri = new Uri(_baseUri + "Accounts(10086)");

            var deleteRequestMessage = new TestHttpClientRequestMessage(deleteRequestUri, base.Client)
            {
                Method = "DELETE"
            };

            var deleteResponseMessage = await deleteRequestMessage.GetResponseAsync();

            // verify the delete
            Assert.Equal(204, deleteResponseMessage.StatusCode);
            (ODataResource deletedEntry, List<ODataResource> complexProperties) = await this.QueryEntryAsync("Accounts(10086)", 404);
            Assert.Null(deletedEntry);
        }

        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public async Task FunctionReturnsAnOpenComplexType_Correctly(string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

            Uri requestUri = new(_baseUri + "Accounts(101)/Default.GetAccountInfo", UriKind.Absolute);

            var requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var odataReader = await messageReader.CreateODataResourceReaderAsync();
                ODataResource accountInfo = null;

                while (await odataReader.ReadAsync())
                {
                    if (odataReader.State == ODataReaderState.ResourceEnd)
                    {
                        accountInfo = odataReader.Item as ODataResource;
                    }
                }

                Assert.NotNull(accountInfo);
                Assert.Equal("Alex", accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
                Assert.Equal("\"Hood\"", (accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value as ODataUntypedValue).RawValue);
            }

            requestUri = new Uri(_baseUri.AbsoluteUri + "Accounts(103)/Default.GetAccountInfo", UriKind.Absolute);

            requestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "GET"
            };

            requestMessage.SetHeader("Accept", mimeType);
            responseMessage = await requestMessage.GetResponseAsync();

            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
                var odataReader = await messageReader.CreateODataResourceReaderAsync();
                
                while (await odataReader.ReadAsync())
                {
                    if (odataReader.State == ODataReaderState.ResourceEnd)
                    {
                        var accountInfo = odataReader.Item as ODataResource;

                        Assert.NotNull(accountInfo);
                        Assert.Equal("Adam", accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
                    }
                }
            }
        }

        [Fact]
        public async Task QueryingAnOpenComplexType_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;

            var accountInfo = await _context.Accounts.ByKey(101).Select(a => a.AccountInfo).GetValueAsync();
            Assert.NotNull(accountInfo);

            accountInfo.DynamicProperties.TryGetValue("MiddleName", out var middleName);
            Assert.Equal("Hood", middleName);

            accountInfo.DynamicProperties.TryGetValue("Address", out var address);
            Assert.NotNull(address);
        }

        [Fact]
        public async Task UpdatingAnOpenComplexType_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;
            
            var account = await _context.Accounts.ByKey(101).GetValueAsync();
            Assert.NotNull(account);

            var accountInfo = account.AccountInfo;
            Assert.NotNull(accountInfo);

            accountInfo.DynamicProperties.TryGetValue("MiddleName", out var middleName);
            Assert.Equal("Hood", middleName);

            accountInfo.FirstName = "Peter";
            accountInfo.DynamicProperties["MiddleName"] = "White";

            // verify: open type enum property from client side should be serialized with odata.type
            accountInfo.DynamicProperties["FavoriteColor"] = Common.Client.Default.Color.Blue;
            accountInfo.DynamicProperties["Address"] = new Common.Client.Default.Address
            {
                Street = "1",
                City = "2",
                PostalCode = "3"
            };

            _context.UpdateObject(account);
            await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

            var updatedAccount =await _context.Accounts.ByKey(101).GetValueAsync();
            Assert.NotNull(updatedAccount);

            var updatedAccountInfo = updatedAccount.AccountInfo;
            Assert.NotNull(updatedAccountInfo);
            Assert.Equal("Peter", updatedAccountInfo.FirstName);
            accountInfo.DynamicProperties.TryGetValue("MiddleName", out var updatedMiddleName);
            Assert.Equal("White", updatedMiddleName);

            accountInfo.DynamicProperties.TryGetValue("FavoriteColor", out var updatedFavoriteColor);
            Assert.Equal(Common.Client.Default.Color.Blue, updatedFavoriteColor);
            accountInfo.DynamicProperties.TryGetValue("Address", out var updatedAddress);
            Assert.Equal("2", (updatedAddress as Common.Client.Default.Address).City);
            Assert.Equal("1", (updatedAddress as Common.Client.Default.Address).Street);
            Assert.Equal("3", (updatedAddress as Common.Client.Default.Address).PostalCode);
        }

        [Fact]
        public async Task UpdatingAnOpenComplexTypeWithUndeclaredProperties_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;
            _context.Configurations.RequestPipeline.OnEntryStarting(ea => EntryStarting(ea));

            var account = await _context.Accounts.ByKey(101).GetValueAsync();
            Assert.Equal(3, account.AccountInfo.DynamicProperties.Count);

            // In practice, transient property data would be mutated here in the partial companion to the client proxy.

            _context.UpdateObject(account);
            await _context.SaveChangesAsync();

            var updatedAccount =await _context.Accounts.ByKey(101).GetValueAsync();
            Assert.Equal(4, account.AccountInfo.DynamicProperties.Count);

            account.AccountInfo.DynamicProperties.TryGetValue("dynamicPropertyKey", out var addedDynamicPropertyValue);
            Assert.Equal("dynamicPropertyValue", addedDynamicPropertyValue);
        }

        [Fact]
        public async Task UpdatingAndReadingAnOpenComplexTypeWithUndeclaredProperties_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;
            _context.Configurations.RequestPipeline.OnMessageWriterSettingsCreated(wsa =>
            {
                // writer supports ODataUntypedValue
                wsa.Settings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            });

            _context.Configurations.RequestPipeline.OnEntryStarting(ea =>
            {
                if (ea.Entity.GetType() == typeof(Common.Client.Default.AccountInfo))
                {
                    var undeclaredOdataProperty = new ODataProperty()
                    {
                        Name = "UndecalredOpenProperty1",
                        Value = new ODataUntypedValue() { RawValue = "{ \"sender\": \"RSS\", \"senderImage\": \"https://exchangelabs.live-int.com/connectors/content/images/feed-icon-128px.png?upn=admin%40tenant-EXHR-3837dom.EXTEST.MICROSOFT.COM\", \"summary\": \"RSS is now connected to your mailbox\", \"title\": null }" }
                    };
                    var accountInfoComplexValueProperties = ea.Entry.Properties as List<ODataProperty>;
                    accountInfoComplexValueProperties.Add(undeclaredOdataProperty);
                }
            });

            var account = await _context.Accounts.ByKey(101).GetValueAsync();
            _context.UpdateObject(account);
            await _context.SaveChangesAsync();

            _context.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated(rsa =>
            {
                // reader supports undeclared property
                rsa.Settings.Validations ^= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            });

            ODataUntypedValue undeclaredOdataPropertyValue = null;
            _context.Configurations.ResponsePipeline.OnEntityMaterialized(rea =>
            {
                if (rea.Entity.GetType() == typeof(Common.Client.Default.AccountInfo))
                {
                    var undeclaredOdataProperty = rea.Entry.Properties.OfType<ODataProperty>().FirstOrDefault(s => s.Name == "UndecalredOpenProperty1");
                    undeclaredOdataPropertyValue = (ODataUntypedValue)undeclaredOdataProperty.Value;
                }
            });

            //There is a bug in OData Client that need fixing.OData Client cannot materialize ODataUntypedValues.
            //var accountReturned = await _context.Accounts.ByKey(101).GetValueAsync();
            /* Assert.Equal<string>(
                 "{\"sender\":\"RSS\",\"senderImage\":\"https://exchangelabs.live-int.com/connectors/content/images/feed-icon-128px.png?upn=admin%40tenant-EXHR-3837dom.EXTEST.MICROSOFT.COM\",\"summary\":\"RSS is now connected to your mailbox\",\"title\":null}",
                 undeclaredOdataPropertyValue.RawValue);*/
        }

        [Fact]
        public async Task InsertingAndDeletingAnEntityWithAnOpenComplexTypeProperty_WorksCorrectly()
        {
            _context.Format.UseJson(_model);

            Common.Client.Default.Account newAccount = new Common.Client.Default.Account()
            {
                AccountID = 110,
                CountryRegion = "CN",
                AccountInfo = new Common.Client.Default.AccountInfo()
                {
                    FirstName = "New",
                    LastName = "Guy",
                    DynamicProperties = new Dictionary<string, object>()
                    {
                        { "MiddleName", "Howard" },
                        { "FavoriteColor", Common.Client.Default.Color.Red },
                        { "Address", new Common.Client.Default.Address()
                            {
                                City = "Shanghai",
                                PostalCode = "200001",
                                Street = "ZiXing"
                            }
                        }
                    }
                }
            };

            _context.AddToAccounts(newAccount);
            await _context.SaveChangesAsync();

            var queryable0 = _context.Accounts.ByKey(110);
            Common.Client.Default.Account accountResult = await queryable0.GetValueAsync();
            Assert.NotNull(accountResult);
            accountResult.AccountInfo.DynamicProperties.TryGetValue("MiddleName", out var middleName);
            accountResult.AccountInfo.DynamicProperties.TryGetValue("Address", out var address);
            accountResult.AccountInfo.DynamicProperties.TryGetValue("FavoriteColor", out var favoriteColor);
            Assert.Equal("Howard", middleName);
            Assert.NotNull(address);

            Assert.Equal(Common.Client.Default.Color.Red, favoriteColor);

            // delete
            var accountToDelete = await _context.Accounts.ByKey(110).GetValueAsync();
            _context.DeleteObject(accountToDelete);
            await _context.SaveChangesAsync();

            var accounts = (await _context.Accounts.ExecuteAsync()).ToList();
            var queryDeletedAccount = accounts.Where(account => account.AccountID == 110);
            Assert.Empty(queryDeletedAccount);
        }

        [Fact]
        public async Task DeletingAndInsertingAnEntityWithAnOpenComplexTypeProperty_WorksCorrectly()
        {
            _context.Format.UseJson(_model);
            _context.MergeOption = MergeOption.OverwriteChanges;
            _ = await _context.Accounts.ByKey(101).GetValueAsync();

            // delete the entry
            Uri requestUri = new(_baseUri + "Accounts(101)");

            var deleteRequestMessage = new TestHttpClientRequestMessage(requestUri, base.Client)
            {
                Method = "DELETE"
            };

            _ = await deleteRequestMessage.GetResponseAsync();

            var entry = new ODataResource() { TypeName = NameSpacePrefix + "Account" };

            entry.Properties = new[]
            {
                new ODataProperty { Name = "AccountID", Value = 101 },
                new ODataProperty { Name = "CountryRegion", Value = "CN" }
            };

            var accountInfo_NestedInfo = new ODataNestedResourceInfo() { Name = "AccountInfo", IsCollection = false };
            var accountInfo_Resource = new ODataResource
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
            };

            var settings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false
            };

            var accountType = _model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
            var accountSet = _model.EntityContainer.FindEntitySet("Accounts");

            var reqUrl = new Uri(_baseUri + "Accounts");

            var requestMessage = new TestHttpClientRequestMessage(reqUrl, base.Client)
            {
                Method = "POST"
            };

            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(accountSet, accountType);
                await odataWriter.WriteStartAsync(entry);
                await odataWriter.WriteStartAsync(accountInfo_NestedInfo);
                await odataWriter.WriteStartAsync(accountInfo_Resource);
                await odataWriter.WriteEndAsync();
                await odataWriter.WriteEndAsync();
                await odataWriter.WriteEndAsync();
            }

            _ = await requestMessage.GetResponseAsync();

            var updatedAccount = await _context.Accounts.ByKey(101).GetValueAsync();
            var info = updatedAccount.AccountInfo;
            info.DynamicProperties.TryGetValue("MiddleName", out var middleName);

            Assert.Null(middleName);
        }
        #endregion

        #region open collection property

        [Fact]
        public async Task OpenCollectionPropertyRoundTrip()
        {
            var mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;

            // update
            var accountInfo = new ODataResource
            {
                TypeName = NameSpacePrefix + "AccountInfo",
                Properties =
                [
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
                            Items = ["meat", "sea food", "apple"]
                        }
                    }
                ]
            };

            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };

            var requestUrl = new Uri(_baseUri + "Accounts(101)/AccountInfo");

            var requestMessage = new TestHttpClientRequestMessage(requestUrl, base.Client)
            {
                Method = "PATCH"
            };

            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
            {
                var odataWriter = await messageWriter.CreateODataResourceWriterAsync(null, null);
                await odataWriter.WriteStartAsync(accountInfo);
                await odataWriter.WriteEndAsync();
            }

            var responseMessage = await requestMessage.GetResponseAsync();
            Assert.Equal(204, responseMessage.StatusCode);

            // verify updated value
            (ODataResource entry, List<ODataResource> complexProperties) = await this.QueryEntryAsync("Accounts(101)");
            var updatedAccountInfo = complexProperties.Single(r => r.TypeName.EndsWith("AccountInfo"));

            Assert.Equal("FN", updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
            Assert.Equal("Green", updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "LastName").Value);
            Assert.Equal("\"Hood\"", (updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value as ODataUntypedValue).RawValue);

            var favoriteFood = updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FavoriteFood").Value as ODataCollectionValue;
            
            // validate items of favoriteFood.
            string[] expectedFavoriteFood = ["meat", "sea food", "apple"];
            int index = 0;

            foreach (var item in favoriteFood.Items)
            {
                Assert.Equal(expectedFavoriteFood[index++], item);
            }

            Assert.Equal(3, index);
        }
        #endregion

        private async Task<(ODataResource entry, List<ODataResource> complexProperties)> QueryEntryAsync(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            var requestUrl = new Uri(_baseUri.AbsoluteUri + uri, UriKind.Absolute);

            var queryRequestMessage = new TestHttpClientRequestMessage(requestUrl, base.Client)
            {
                Method = "GET"
            };

            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = await queryRequestMessage.GetResponseAsync();

            Assert.Equal(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataResource entry = null;
            List<ODataResource> complexProperties = [];

            if (expectedStatusCode == 200)
            {
                using var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, _model);
                var reader = await messageReader.CreateODataResourceReaderAsync();
                while (await reader.ReadAsync())
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

            return (entry, complexProperties);
        }

        private void EntryStarting(WritingEntryArgs ea)
        {
            if (ea.Entity.GetType() == typeof(Common.Client.Default.AccountInfo))
            {
                var properties = ea.Entry.Properties as List<ODataProperty>;
                var undeclaredOdataProperty = new ODataProperty() { Name = "dynamicPropertyKey", Value = "dynamicPropertyValue" };
                properties.Add(undeclaredOdataProperty);
                ea.Entry.Properties = properties;
            }
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "complextype/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
