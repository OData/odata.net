//-----------------------------------------------------------------------------
// <copyright file="ComplexTypeTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.ComplexTypes;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;
using Microsoft.Spatial;

namespace Microsoft.OData.Core.E2E.Tests.ComplexTypeTests
{
    public class ComplexTypeTests : EndToEndTestBase<ComplexTypeTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;
        private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.Default.";

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

            Uri requestUri = new(_baseUri.AbsoluteUri + "People(1)/HomeAddress/Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress/FamilyName", UriKind.Absolute);

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

            Uri requestUri = new(_baseUri.AbsoluteUri + "People?$filter=HomeAddress/Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress/FamilyName eq 'Cats'", UriKind.Absolute);

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
            (ODataResource entry, List<ODataResource> complexTypeResources) = await QueryEntryAsync("People(1)");

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
            (entry, complexTypeResources) = await QueryEntryAsync("People(1)");
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

            (ODataResource entry, List<ODataResource> complexTypeResources) = await QueryEntryAsync("People(101)");

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
            (deletedEntry, complexTypeResources) = await QueryEntryAsync("People(101)", 404);

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
                Assert.Equal("Hood", accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value);

                var colorODataEnumValue = Assert.IsType<ODataEnumValue>(accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FavoriteColor").Value);
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
                Assert.Equal("Hood", accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value);

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
                (ODataResource entry, List<ODataResource> complexTypeResources) = await QueryEntryAsync("Accounts(101)");

                var accountInfo = complexTypeResources.Single(a => a.TypeName.EndsWith("AccountInfo"));
                Assert.Equal(middleNames[i], accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value);

                // update
                bool isPersonalAccount = i % 2 == 0;
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
                (entry, complexTypeResources) = await QueryEntryAsync("Accounts(101)");
                var updatedAccountInfo = complexTypeResources.Single(a => a.TypeName.EndsWith("AccountInfo"));
                Assert.Equal("FN", updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
                Assert.Equal(middleNames[i + 1], updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value);
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
            (entry, List<ODataResource> complexTypeResources) = await QueryEntryAsync("Accounts(10086)");
            Assert.Equal(10086, entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "AccountID").Value);

            var accountInfo = complexTypeResources.Single(a => a.TypeName.EndsWith("AccountInfo"));
            Assert.Equal("Peter", accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
            Assert.Equal("#999, ZiXing Road", accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "ShippingAddress").Value);

            // delete the entry
            var deleteRequestUri = new Uri(_baseUri + "Accounts(10086)");

            var deleteRequestMessage = new TestHttpClientRequestMessage(deleteRequestUri, base.Client)
            {
                Method = "DELETE"
            };

            var deleteResponseMessage = await deleteRequestMessage.GetResponseAsync();

            // verify the delete
            Assert.Equal(204, deleteResponseMessage.StatusCode);
            (ODataResource deletedEntry, List<ODataResource> complexProperties) = await QueryEntryAsync("Accounts(10086)", 404);
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
                Assert.Equal("Hood", accountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value);
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
            (ODataResource entry, List<ODataResource> complexProperties) = await QueryEntryAsync("Accounts(101)");
            var updatedAccountInfo = complexProperties.Single(r => r.TypeName.EndsWith("AccountInfo"));

            Assert.Equal("FN", updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
            Assert.Equal("Green", updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "LastName").Value);
            Assert.Equal("Hood", updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "MiddleName").Value);

            var favoriteFood = updatedAccountInfo.Properties.OfType<ODataProperty>().Single(p => p.Name == "FavoriteFood").Value as ODataCollectionValue;

            // validate items of favoriteFood.
            string[] expectedFavoriteFood = ["meat", "sea food", "apple"];
            int index = 0;

            Assert.NotNull(favoriteFood);
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

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "complextype/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
