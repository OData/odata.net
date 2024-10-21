//---------------------------------------------------------------------
// <copyright file="ODataWriterValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataWriterValidationTests
    {
        private readonly EdmModel model;
        private readonly EdmComplexType addressComplexType;
        private readonly EdmComplexType regionalAddressComplexType;
        private readonly EdmComplexType globalAddressComplexType;
        private readonly EdmComplexType cityComplexType;
        private readonly EdmEnumType colorEnumType;
        private readonly EdmEntityType customerEntityType;
        private readonly EdmEntityType orderEntityType;
        private readonly EdmEntityType orderItemEntityType;
        private readonly EdmEntityType productEntityType;
        private readonly EdmEntitySet customersEntitySet;
        private readonly EdmEntitySet ordersEntitySet;
        private readonly EdmEntitySet productsEntitySet;
        private readonly EdmAction sendOffersAction;
        private readonly ODataResource customerResource;
        private readonly ODataResource orderResource;
        private readonly ODataResource shippingAddressResource;
        private readonly ODataResource locationResource;
        private readonly ODataResource productResource;
        private readonly ODataNestedResourceInfo ordersNestedResourceInfo;
        private readonly ODataNestedResourceInfo officeLocationsNestedResourceInfo;
        private readonly ODataNestedResourceInfo customerNestedResourceInfo;
        private readonly ODataNestedResourceInfo shippingAddressNestedResourceInfo;
        private readonly ODataMessageWriterSettings messageWriterSettings;
        private readonly Stream stream;

        public ODataWriterValidationTests()
        {
            this.model = new EdmModel();

            this.addressComplexType = this.model.AddComplexType("NS", "Address");
            this.addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);

            this.regionalAddressComplexType = this.model.AddComplexType("NS", "RegionalAddress", this.addressComplexType);

            this.globalAddressComplexType = this.model.AddComplexType("NS", "GlobalAddress", this.addressComplexType);
            this.globalAddressComplexType.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);

            this.cityComplexType = this.model.AddComplexType("NS", "City");
            this.cityComplexType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            this.colorEnumType = new EdmEnumType("NS", "Color");
            this.colorEnumType.AddMember("Black", new EdmEnumMemberValue(1));
            this.colorEnumType.AddMember("White", new EdmEnumMemberValue(2));
            this.model.AddElement(this.colorEnumType);

            this.customerEntityType = this.model.AddEntityType("NS", "Customer");
            this.customerEntityType.AddKeys(this.customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            this.customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            this.customerEntityType.AddStructuralProperty("Photo", EdmPrimitiveTypeKind.Stream);
            this.customerEntityType.AddStructuralProperty(
                "Emails",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(
                        EdmCoreModel.Instance.GetString(isNullable: false))));
            this.customerEntityType.AddStructuralProperty(
                "OfficeLocations",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(
                        new EdmComplexTypeReference(this.addressComplexType, isNullable: false))));

            this.orderEntityType = this.model.AddEntityType("NS", "Order");
            this.orderEntityType.AddKeys(this.orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            this.orderEntityType.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Decimal, isNullable: false);
            this.orderEntityType.AddStructuralProperty(
                "ShippingAddress",
                new EdmComplexTypeReference(this.addressComplexType, isNullable: false));

            this.orderItemEntityType = this.model.AddEntityType("NS", "OrderItem");
            this.orderItemEntityType.AddKeys(this.orderItemEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            this.orderItemEntityType.AddStructuralProperty("Description", EdmPrimitiveTypeKind.String);

            this.productEntityType = this.model.AddEntityType("NS", "Product", baseType: null, isAbstract: false, isOpen: true, hasStream: true);
            this.productEntityType.AddKeys(this.productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            this.productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);

            this.customerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Orders",
                    TargetMultiplicity = EdmMultiplicity.Many,
                    Target = this.orderEntityType,
                    ContainsTarget = false
                });
            this.orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Customer",
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    Target = this.customerEntityType,
                    ContainsTarget = false
                });
            this.orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Items",
                    TargetMultiplicity = EdmMultiplicity.Many,
                    Target = this.orderItemEntityType,
                    ContainsTarget = true
                });

            var entityContainer = this.model.AddEntityContainer("Default", "Container");
            this.customersEntitySet = entityContainer.AddEntitySet("Customers", this.customerEntityType);
            this.ordersEntitySet = entityContainer.AddEntitySet("Orders", this.orderEntityType);
            this.productsEntitySet = entityContainer.AddEntitySet("Products", this.productEntityType);

            this.sendOffersAction = new EdmAction("Default", "SendOffers", returnType: null, isBound: true, entitySetPathExpression: null);
            this.sendOffersAction.AddParameter("bindingParameter", new EdmEntityTypeReference(this.productEntityType, isNullable: false));
            this.sendOffersAction.AddParameter("emails", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(false)));
            this.model.AddElement(this.sendOffersAction);

            var shippingAddressProperty = this.orderEntityType.FindProperty("ShippingAddress");
            var derivedTypeConstraintAnnotation = new EdmVocabularyAnnotation(
                shippingAddressProperty,
                ValidationVocabularyModel.DerivedTypeConstraintTerm,
                new EdmCollectionExpression(
                    new EdmStringConstant("NS.RegionalAddress")));
            this.model.AddVocabularyAnnotation(derivedTypeConstraintAnnotation);

            this.customerResource = new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Name", Value = "Sue" }
                }
            };

            this.orderResource = new ODataResource
            {
                TypeName = "NS.Order",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Amount", Value = 130M }
                }
            };

            this.ordersNestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Orders",
                IsCollection = true,
                Url = new Uri("http://tempuri.org/Customers(1)/Orders")
            };

            this.locationResource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Street", Value = "Waiyaki Way" }
                }
            };

            this.officeLocationsNestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "OfficeLocations",
                IsCollection = true,
                IsComplex = true
            };

            this.customerNestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri("http://tempuri.org/Orders(1)/Customer")
            };

            this.shippingAddressNestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "ShippingAddress",
                IsCollection = false,
                IsComplex = true
            };

            this.shippingAddressResource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Street", Value = "Wood Avenue" }
                }
            };

            var productPhotoMediaResource = new ODataStreamReferenceValue
            {
                EditLink = new Uri($"http://tempuri.org/Products(1)/Photo"),
                ReadLink = new Uri($"http://tempuri.org/Products(1)/Photo"),
                ContentType = "image/png",
                ETag = "media-etag"
            };

            this.productResource = new ODataResource
            {
                TypeName = "NS.Product",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Name", Value = "Lenovo ThinkPad P53s" }
                },
                MediaResource = new ODataStreamReferenceValue()
            };

            this.productResource.MediaResource.SetMetadataBuilder(
                new TempEntityMetadataBuilder(
                    resourceId: this.productResource.Id,
                    mediaResource: productPhotoMediaResource,
                    eTag: null),
                "Photo");

            this.messageWriterSettings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
                ODataUri = new ODataUri { ServiceRoot = new Uri("http://tempuri.org") },
            };

            this.stream = new MemoryStream();
        }

        public static IEnumerable<object[]> GetODataVersions()
        {
            yield return new object[] { ODataVersion.V4 };
            yield return new object[] { ODataVersion.V401 };
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraintAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempGlobalShippingAddressResource = new ODataResource
                    {
                        TypeName = "NS.GlobalAddress",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Street", Value = "One Microsoft Way" },
                            new ODataProperty { Name = "Country", Value = "USA" }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(this.shippingAddressNestedResourceInfo);
                    await resourceWriter.WriteStartAsync(tempGlobalShippingAddressResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint(
                        "NS.GlobalAddress",
                        "property",
                        "ShippingAddress"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_UnrecognizedTypeNameAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Str)",
                                    Items = new [] { "sue@temp.com" }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_UnrecognizedTypeName("Collection(Str)"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_UnsupportedPrimitiveTypeAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Edm.String)",
                                    Items = new object [] { new object() }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_UnsupportedPrimitiveType("System.Object"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_IncorrectTypeKindAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Id = new Uri("http://tempuri.org/Customers(1)"),
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Sue" },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Edm.Int32",
                                    Items = new [] { "sue@this.temp.com" }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_IncorrectTypeKind("Edm.Int32", "Collection", "Primitive"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_IncompatibleTypeAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Id = new Uri("http://tempuri.org/Customers(1)"),
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Edm.Int32)",
                                    Items = new [] { "sue@this.temp.com" }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_IncompatibleType("Collection(Edm.Int32)", "Collection(Edm.String)"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollectionAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = false,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(tempCustomerNestedResourceInfo);
                    // Set nested resource info IsCollection to null to cause subsequent WriteStartAsync(ODataResource) call to fail
                    tempCustomerNestedResourceInfo.IsCollection = null;
                    await resourceWriter.WriteStartAsync(this.customerResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection("Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ODataMessageWriter_CannotWriteTopLevelNullAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(() => messageWriter.WritePropertyAsync(new ODataProperty
                {
                    Name = "Name",
                    Value = null
                }));

                Assert.Equal(
                    Strings.ODataMessageWriter_CannotWriteTopLevelNull,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_IncompatiblePrimitiveItemTypeAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempOrderResource = new ODataResource
                    {
                        TypeName = "NS.Order",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(tempOrderResource);
                    await resourceWriter.WriteStartAsync(new ODataProperty
                    {
                        Name = "Amount",
                        Value = 3.124d
                    });
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_IncompatiblePrimitiveItemType("Edm.Double", "False", "Edm.Decimal", "False"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyTypeAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(this.customerNestedResourceInfo);
                    await resourceWriter.WriteStartAsync(this.productResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("NS.Product", "NS.Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_NonPrimitiveTypeForPrimitiveValueAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Emails", Value = "sue@temp.com" } // Emails is an collection property
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_NonPrimitiveTypeForPrimitiveValue("Collection(Edm.String)"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_PropertiesMustNotContainReservedCharsAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Id = new Uri("http://tempuri.org/Customers(1)"),
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Customer.Name", Value = "Sue" },
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_PropertiesMustNotContainReservedChars("Customer.Name", "':', '.', '@'"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [InlineData("", ODataVersion.V4)]
        [InlineData(null, ODataVersion.V4)]
        [InlineData("", ODataVersion.V401)]
        [InlineData(null, ODataVersion.V401)]
        public async Task WriterValidationUtils_PropertiesMustHaveNonEmptyNameAsync(string propertyName, ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = propertyName, Value = "Houston, we have a problem" } // Property name empty
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_PropertiesMustHaveNonEmptyName,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_PropertyDoesNotExistOnTypeAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "NonExistentProperty", Value = "!" }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_PropertyDoesNotExistOnType("NonExistentProperty", "NS.Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_CollectionPropertiesMustNotHaveNullValueAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Emails", Value = null }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);

                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue("Emails"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_StreamPropertiesMustNotHaveNullValueAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Photo", Value = null }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);

                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_StreamPropertiesMustNotHaveNullValue("Photo"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_StreamPropertyInRequestAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Sue" }
                        }
                    };

                    var photoProperty = new ODataStreamPropertyInfo
                    {
                        Name = "Photo",
                        EditLink = new Uri("http://tempuri.org/Customers(1)/Photo"),
                        ReadLink = new Uri("http://tempuri.org/Customers(1)/Photo"),
                        ContentType = "text/plain"
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteStartAsync(photoProperty);
                    await resourceWriter.WritePrimitiveAsync(new ODataPrimitiveValue(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_StreamPropertyInRequest("Photo"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_MismatchPropertyKindForStreamPropertyAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 }
                        }
                    };

                    var photoProperty = new ODataStreamPropertyInfo
                    {
                        Name = "Name",
                        EditLink = new Uri("http://tempuri.org/Customers(1)/Name"),
                        ReadLink = new Uri("http://tempuri.org/Customers(1)/Name"),
                        ContentType = "text/plain"
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteStartAsync(photoProperty);
                    await resourceWriter.WritePrimitiveAsync(new ODataPrimitiveValue(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_MismatchPropertyKindForStreamProperty("Name"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadataAsync(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            this.messageWriterSettings.ODataUri.Path = new ODataUriParser(this.model, new Uri("Orders", UriKind.Relative)).ParsePath();

            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempOrderItemsNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Items",
                        IsCollection = true,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Items")
                    };

                    var tempOrderItemResource = new ODataResource
                    {
                        TypeName = "NS.OrderItem",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Description", Value = "Pencil" },
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(tempOrderItemsNestedResourceInfo);
                    // Change nested resource info name to cause subsequent WriteStartAsync(ODataResource) call to fail
                    tempOrderItemsNestedResourceInfo.Name = "Customer";
                    await resourceWriter.WriteStartAsync(new ODataResourceSet());
                    await resourceWriter.WriteStartAsync(tempOrderItemResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadata("http://tempuri.org/Orders(1)/Items"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadataAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = false,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(tempCustomerNestedResourceInfo);
                    // Change nested resource info name to cause subsequent WriteStartAsync(ODataResource) call to fail
                    tempCustomerNestedResourceInfo.Name = "Items";
                    await resourceWriter.WriteStartAsync(this.customerResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadata("http://tempuri.org/Orders(1)/Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContentAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = false,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(tempCustomerNestedResourceInfo);
                    // To force the subsequent WriteStartAsync(ODataResource) call to fail
                    tempCustomerNestedResourceInfo.IsCollection = true;
                    await resourceWriter.WriteStartAsync(this.customerResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent("http://tempuri.org/Orders(1)/Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContentAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempOrdersNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Orders",
                        IsCollection = true,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Customers(1)/Orders")
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(this.customerResource);
                    await resourceWriter.WriteStartAsync(tempOrdersNestedResourceInfo);
                    tempOrdersNestedResourceInfo.IsCollection = false;
                    await resourceWriter.WriteStartAsync(new ODataResourceSet());
                    // To force subsequent WriteStartAsync(ODataResource) call to fail
                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent("http://tempuri.org/Customers(1)/Orders"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadataAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = true,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(tempCustomerNestedResourceInfo);
                    await resourceWriter.WriteStartAsync(this.customerResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata("http://tempuri.org/Orders(1)/Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadataAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 7 },
                            new ODataProperty { Name = "Name", Value = "Sue" }
                        }
                    };

                    var tempOrdersNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Orders",
                        IsCollection = false,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Customers(7)/Orders")
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteStartAsync(tempOrdersNestedResourceInfo);
                    await resourceWriter.WriteStartAsync(new ODataResourceSet());
                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata("http://tempuri.org/Customers(7)/Orders"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_NullCollectionItemForNonNullableTypeAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var collectionStart = new ODataCollectionStart
                    {
                        SerializationInfo = new ODataCollectionStartSerializationInfo
                        {
                            CollectionTypeName = "Collection(Edm.String)"
                        }
                    };

                    var parameterWriter = await messageWriter.CreateODataParameterWriterAsync(this.sendOffersAction);

                    await parameterWriter.WriteStartAsync();
                    var collectionWriter = await parameterWriter.CreateCollectionWriterAsync("emails");
                    await collectionWriter.WriteStartAsync(collectionStart);
                    await collectionWriter.WriteItemAsync("sue@test.com");
                    await collectionWriter.WriteItemAsync(null);
                    await collectionWriter.WriteEndAsync();
                    await parameterWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_NullCollectionItemForNonNullableType("Edm.String"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValueAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = null }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue("Name", "Edm.String"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNullAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = false
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(tempCustomerNestedResourceInfo);
                    await resourceWriter.WriteEntityReferenceLinkAsync(
                        new ODataEntityReferenceLink
                        {
                            Url = null
                        });
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNullAsync(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new AsyncStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.Default,
                IsResponse = true,
                IsAsync = true,
                Model = model
            };

            await using (var jsonOutputContext = new ODataJsonOutputContext(messageInfo, this.messageWriterSettings))
            {
                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => jsonOutputContext.WriteEntityReferenceLinksAsync(
                        new ODataEntityReferenceLinks
                        {
                            Links = new List<ODataEntityReferenceLink> { null }
                        }));

                Assert.Equal(
                    Strings.WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull,
                    exception.Message);
            }
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_NextPageLinkInRequestAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempOrdersResourceSet = new ODataResourceSet
                    {
                        TypeName = "Collection(NS.Order)",
                        NextPageLink = new Uri("http://tempuri.org/Orders?$skiptoken=Id-2")
                    };

                    var tempResourceSetWriter = await messageWriter.CreateODataResourceSetWriterAsync(this.ordersEntitySet, this.orderEntityType);

                    await tempResourceSetWriter.WriteStartAsync(tempOrdersResourceSet);
                    await tempResourceSetWriter.WriteStartAsync(this.orderResource);
                    await tempResourceSetWriter.WriteEndAsync();
                    await tempResourceSetWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_NextPageLinkInRequest,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_OperationInRequestAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Lenovo ThinkPad P53s" }
                        }
                    };

                    tempProductResource.AddAction(
                        new ODataAction
                        {
                            Title = "SendOffers",
                            Target = new Uri("http://tempuri.org/Products(1)/Default.SendOffers"),
                            Metadata = new Uri("http://tempuri.org/$metadata#Default.SendOffers")
                        });

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.productsEntitySet, this.productEntityType);

                    await resourceWriter.WriteStartAsync(tempProductResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_OperationInRequest("http://tempuri.org/$metadata#Default.SendOffers"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_EnumerableContainsANullItemAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Lenovo ThinkPad P53s" }
                        }
                    };

                    tempProductResource.MetadataBuilder = new NullActionTempEntityMetadataBuilder(tempProductResource.Id);

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.productsEntitySet, this.productEntityType);

                    await resourceWriter.WriteStartAsync(tempProductResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_EnumerableContainsANullItem("ODataResource.Actions"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_ActionsAndFunctionsMustSpecifyMetadataAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Lenovo ThinkPad P53s" }
                        }
                    };

                    tempProductResource.AddAction(
                        new ODataAction
                        {
                            Title = "SendOffers",
                            Target = new Uri("http://tempuri.org/Products(1)/Default.SendOffers")
                        });

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.productsEntitySet, this.productEntityType);

                    await resourceWriter.WriteStartAsync(tempProductResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata("ODataAction"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_StreamReferenceValuesNotSupportedInCollectionsAsync(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = this.stream };

            await using (var messageWriter = new ODataMessageWriter(requestMessage, this.messageWriterSettings))
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Features",
                                Value = new ODataCollectionValue
                                {
                                    Items = new List<object>
                                    {
                                        new ODataStreamReferenceValue
                                        {
                                            EditLink = new Uri($"http://tempuri.org/Products(1)/Photo"),
                                            ReadLink = new Uri($"http://tempuri.org/Products(1)/Photo"),
                                            ContentType = "image/png",
                                            ETag = "media-etag"
                                        }
                                    }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync();

                    await resourceWriter.WriteStartAsync(tempProductResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_StreamReferenceValuesNotSupportedInCollections,
                    exception.Message);
            }
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_NestedCollectionsAreNotSupportedAsync(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = this.stream };

            await using (var messageWriter = new ODataMessageWriter(requestMessage, this.messageWriterSettings))
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Features",
                                Value = new ODataCollectionValue
                                {
                                    Items = new List<object>
                                    {
                                        new ODataCollectionValue
                                        {
                                            Items = new List<object>
                                            {
                                                3.142857142
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync();

                    await resourceWriter.WriteStartAsync(tempProductResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_NestedCollectionsAreNotSupported,
                    exception.Message);
            }
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_NonNullableCollectionElementsMustNotBeNullAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Edm.String)",
                                    Items = new [] { (string)null }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(tempCustomerResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_NonNullableCollectionElementsMustNotBeNull,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_RecursionDepthLimitReachedAsync(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 1;

            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var odataError = new ODataError
                {
                    Code = "OEC1",
                    Message = "OEM1",
                    Target = "OET1",
                    InnerError = new ODataInnerError(
                        new Dictionary<string, ODataValue>
                        {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                        })
                    {
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                    { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM2") },
                                    { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET2") },
                                    { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES2") }
                            })
                        {
                            InnerError = new ODataInnerError(
                                new Dictionary<string, ODataValue>
                                {
                                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM3") },
                                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET3") },
                                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES3") }
                                })
                        }
                    }
                };

                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => messageWriter.WriteErrorAsync(odataError, includeDebugInformation: true));

                Assert.Equal(
                    Strings.ValidationUtils_RecursionDepthLimitReached(1),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_MissingTypeNameWithMetadataAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Features",
                                Value = new ODataCollectionValue
                                {
                                    Items = new List<object>
                                    {
                                        3.142857142
                                    }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.productsEntitySet, this.productEntityType);

                    await resourceWriter.WriteStartAsync(tempProductResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_MissingTypeNameWithMetadata,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_TypeNameMustNotBeEmptyAsync(ODataVersion odataVersion)
        {
            await SetupRequestMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Features",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "",
                                    Items = new List<object>
                                    {
                                        3.142857142
                                    }
                                }
                            }
                        }
                    };

                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.productsEntitySet, this.productEntityType);

                    await resourceWriter.WriteStartAsync(tempProductResource);
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_TypeNameMustNotBeEmpty,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_ResourceMustSpecifyNameAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var serviceDocument = new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo>
                    {
                        new ODataEntitySetInfo
                        {
                            Title = "Products",
                            Url = new Uri("Products", UriKind.Relative)
                        }
                    }
                };

                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => messageWriter.WriteServiceDocumentAsync(serviceDocument));
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_ResourceMustSpecifyUrlAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var serviceDocument = new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo>
                    {
                        new ODataEntitySetInfo
                        {
                            Name = "Products",
                            Title = "Products"
                        }
                    }
                };

                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => messageWriter.WriteServiceDocumentAsync(serviceDocument));

                Assert.Equal(
                    Strings.ValidationUtils_ResourceMustSpecifyUrl,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_WorkspaceResourceMustNotContainNullItemAsync(ODataVersion odataVersion)
        {
            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var serviceDocument = new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo>
                    {
                        null
                    }
                };

                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => messageWriter.WriteServiceDocumentAsync(serviceDocument));

                Assert.Equal(
                    Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task ValidationUtils_MaxDepthOfNestedEntriesExceededAsync(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 2;

            await SetupResponseMessageWriterAndRunTestAsync(async (messageWriter) =>
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.customersEntitySet, this.customerEntityType);

                    await resourceWriter.WriteStartAsync(this.customerResource);
                    await resourceWriter.WriteStartAsync(this.ordersNestedResourceInfo);
                    await resourceWriter.WriteStartAsync(new ODataResourceSet());
                    await resourceWriter.WriteStartAsync(this.orderResource);
                    await resourceWriter.WriteStartAsync(this.shippingAddressNestedResourceInfo);
                    await resourceWriter.WriteStartAsync(this.shippingAddressResource);
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                    await resourceWriter.WriteEndAsync();
                });

                Assert.Equal(
                    Strings.ValidationUtils_MaxDepthOfNestedEntriesExceeded(2),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsoluteAsync(ODataVersion odataVersion)
        {
            var exception = await Assert.ThrowsAsync<ODataException>(async () =>
            {
                this.messageWriterSettings.BaseUri = new Uri("/", UriKind.Relative);

                await SetupResponseMessageWriterAndRunTestAsync((messageWriter) => Task.CompletedTask, odataVersion);
            });

            Assert.Equal(
            Strings.WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute('/'),
            exception.Message);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public async Task WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessageAsync(ODataVersion odataVersion)
        {
            var exception = await Assert.ThrowsAsync<ODataException>(async () =>
            {
                this.messageWriterSettings.JsonPCallback = "callback";

                await SetupRequestMessageWriterAndRunTestAsync((messageWriter) => Task.CompletedTask, odataVersion);
            });

            Assert.Equal(
                Strings.WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage,
                exception.Message);
        }

        #region Synchronous Tests

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_UnsupportedPrimitiveType(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Edm.String)",
                                    Items = new object [] { new object() }
                                }
                            }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_UnsupportedPrimitiveType("System.Object"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_IncorrectTypeKind(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Id = new Uri("http://tempuri.org/Customers(1)"),
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Sue" },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Edm.Int32",
                                    Items = new [] { "sue@this.temp.com" }
                                }
                            }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_IncorrectTypeKind("Edm.Int32", "Collection", "Primitive"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_IncompatibleType(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Id = new Uri("http://tempuri.org/Customers(1)"),
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Edm.Int32)",
                                    Items = new [] { "sue@this.temp.com" }
                                }
                            }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_IncompatibleType("Collection(Edm.Int32)", "Collection(Edm.String)"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = false,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.ordersEntitySet, this.orderEntityType);

                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteStart(tempCustomerNestedResourceInfo);
                    // Set nested resource info IsCollection to null to cause subsequent WriteStart(ODataResource) call to fail
                    tempCustomerNestedResourceInfo.IsCollection = null;
                    resourceWriter.WriteStart(this.customerResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_NestedResourceInfoMustSpecifyIsCollection("Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ODataMessageWriter_CannotWriteTopLevelNull(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() => messageWriter.WriteProperty(new ODataProperty
                {
                    Name = "Name",
                    Value = null
                }));

                Assert.Equal(
                    Strings.ODataMessageWriter_CannotWriteTopLevelNull,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_IncompatiblePrimitiveItemType(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempOrderResource = new ODataResource
                    {
                        TypeName = "NS.Order",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.ordersEntitySet, this.orderEntityType);

                    resourceWriter.WriteStart(tempOrderResource);
                    resourceWriter.WriteStart(new ODataProperty
                    {
                        Name = "Amount",
                        Value = 3.124d
                    });
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_IncompatiblePrimitiveItemType("Edm.Double", "False", "Edm.Decimal", "False"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.ordersEntitySet, this.orderEntityType);

                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteStart(this.customerNestedResourceInfo);
                    resourceWriter.WriteStart(this.productResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("NS.Product", "NS.Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_NonPrimitiveTypeForPrimitiveValue(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Emails", Value = "sue@temp.com" } // Emails is an collection property
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_NonPrimitiveTypeForPrimitiveValue("Collection(Edm.String)"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_PropertiesMustNotContainReservedChars(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Id = new Uri("http://tempuri.org/Customers(1)"),
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Customer.Name", Value = "Sue" },
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_PropertiesMustNotContainReservedChars("Customer.Name", "':', '.', '@'"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [InlineData("", ODataVersion.V4)]
        [InlineData(null, ODataVersion.V4)]
        [InlineData("", ODataVersion.V401)]
        [InlineData(null, ODataVersion.V401)]
        public void WriterValidationUtils_PropertiesMustHaveNonEmptyName(string propertyName, ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = propertyName, Value = "Houston, we have a problem" } // Property name empty
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_PropertiesMustHaveNonEmptyName,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_PropertyDoesNotExistOnType(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "NonExistentProperty", Value = "!" }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_PropertyDoesNotExistOnType("NonExistentProperty", "NS.Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Emails", Value = null }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);

                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue("Emails"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_StreamPropertiesMustNotHaveNullValue(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Photo", Value = null }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);

                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_StreamPropertiesMustNotHaveNullValue("Photo"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_StreamPropertyInRequest(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Sue" }
                        }
                    };

                    var photoProperty = new ODataStreamPropertyInfo
                    {
                        Name = "Photo",
                        EditLink = new Uri("http://tempuri.org/Customers(1)/Photo"),
                        ReadLink = new Uri("http://tempuri.org/Customers(1)/Photo"),
                        ContentType = "text/plain"
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteStart(photoProperty);
                    resourceWriter.WritePrimitive(new ODataPrimitiveValue(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_StreamPropertyInRequest("Photo"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_MismatchPropertyKindForStreamProperty(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 }
                        }
                    };

                    var photoProperty = new ODataStreamPropertyInfo
                    {
                        Name = "Name",
                        EditLink = new Uri("http://tempuri.org/Customers(1)/Name"),
                        ReadLink = new Uri("http://tempuri.org/Customers(1)/Name"),
                        ContentType = "text/plain"
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteStart(photoProperty);
                    resourceWriter.WritePrimitive(new ODataPrimitiveValue(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }));
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_MismatchPropertyKindForStreamProperty("Name"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadata(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            this.messageWriterSettings.ODataUri.Path = new ODataUriParser(this.model, new Uri("Orders", UriKind.Relative)).ParsePath();

            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempOrderItemsNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Items",
                        IsCollection = true,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Items")
                    };

                    var tempOrderItemResource = new ODataResource
                    {
                        TypeName = "NS.OrderItem",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Description", Value = "Pencil" },
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.ordersEntitySet, this.orderEntityType);

                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteStart(tempOrderItemsNestedResourceInfo);
                    // Change nested resource info name to cause subsequent WriteStart(ODataResource) call to fail
                    tempOrderItemsNestedResourceInfo.Name = "Customer";
                    resourceWriter.WriteStart(new ODataResourceSet());
                    resourceWriter.WriteStart(tempOrderItemResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkWithResourceSetPayloadAndResourceMetadata("http://tempuri.org/Orders(1)/Items"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadata(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = false,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.ordersEntitySet, this.orderEntityType);

                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteStart(tempCustomerNestedResourceInfo);
                    // Change nested resource info name to cause subsequent WriteStart(ODataResource) call to fail
                    tempCustomerNestedResourceInfo.Name = "Items";
                    resourceWriter.WriteStart(this.customerResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkWithResourcePayloadAndResourceSetMetadata("http://tempuri.org/Orders(1)/Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = false,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.ordersEntitySet, this.orderEntityType);

                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteStart(tempCustomerNestedResourceInfo);
                    // To force the subsequent WriteStart(ODataResource) call to fail
                    tempCustomerNestedResourceInfo.IsCollection = true;
                    resourceWriter.WriteStart(this.customerResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceContent("http://tempuri.org/Orders(1)/Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempOrdersNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Orders",
                        IsCollection = true,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Customers(1)/Orders")
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(this.customerResource);
                    resourceWriter.WriteStart(tempOrdersNestedResourceInfo);
                    tempOrdersNestedResourceInfo.IsCollection = false;
                    resourceWriter.WriteStart(new ODataResourceSet());
                    // To force subsequent WriteStart(ODataResource) call to fail
                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetContent("http://tempuri.org/Customers(1)/Orders"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = true,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.ordersEntitySet, this.orderEntityType);

                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteStart(tempCustomerNestedResourceInfo);
                    resourceWriter.WriteStart(this.customerResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithResourceMetadata("http://tempuri.org/Orders(1)/Customer"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 7 },
                            new ODataProperty { Name = "Name", Value = "Sue" }
                        }
                    };

                    var tempOrdersNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Orders",
                        IsCollection = false,
                        IsComplex = false,
                        Url = new Uri("http://tempuri.org/Customers(7)/Orders")
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteStart(tempOrdersNestedResourceInfo);
                    resourceWriter.WriteStart(new ODataResourceSet());
                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithResourceSetMetadata("http://tempuri.org/Customers(7)/Orders"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_NullCollectionItemForNonNullableType(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var collectionStart = new ODataCollectionStart
                    {
                        SerializationInfo = new ODataCollectionStartSerializationInfo
                        {
                            CollectionTypeName = "Collection(Edm.String)"
                        }
                    };

                    var parameterWriter = messageWriter.CreateODataParameterWriter(this.sendOffersAction);

                    parameterWriter.WriteStart();
                    var collectionWriter = parameterWriter.CreateCollectionWriter("emails");
                    collectionWriter.WriteStart(collectionStart);
                    collectionWriter.WriteItem("sue@test.com");
                    collectionWriter.WriteItem(null);
                    collectionWriter.WriteEnd();
                    parameterWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_NullCollectionItemForNonNullableType("Edm.String"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = null }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue("Name", "Edm.String"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        IsCollection = false
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.ordersEntitySet, this.orderEntityType);

                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteStart(tempCustomerNestedResourceInfo);
                    resourceWriter.WriteEntityReferenceLink(
                        new ODataEntityReferenceLink
                        {
                            Url = null
                        });
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.Default,
                IsResponse = true,
                IsAsync = false,
                Model = model
            };

            using (var jsonOutputContext = new ODataJsonOutputContext(messageInfo, this.messageWriterSettings))
            {
                var exception = Assert.Throws<ODataException>(
                    () => jsonOutputContext.WriteEntityReferenceLinks(
                        new ODataEntityReferenceLinks
                        {
                            Links = new List<ODataEntityReferenceLink> { null }
                        }));

                Assert.Equal(
                    Strings.WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull,
                    exception.Message);
            }
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_NextPageLinkInRequest(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempOrdersResourceSet = new ODataResourceSet
                    {
                        TypeName = "Collection(NS.Order)",
                        NextPageLink = new Uri("http://tempuri.org/Orders?$skiptoken=Id-2")
                    };

                    var tempResourceSetWriter = messageWriter.CreateODataResourceSetWriter(this.ordersEntitySet, this.orderEntityType);

                    tempResourceSetWriter.WriteStart(tempOrdersResourceSet);
                    tempResourceSetWriter.WriteStart(this.orderResource);
                    tempResourceSetWriter.WriteEnd();
                    tempResourceSetWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_NextPageLinkInRequest,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_OperationInRequest(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Lenovo ThinkPad P53s" }
                        }
                    };

                    tempProductResource.AddAction(
                        new ODataAction
                        {
                            Title = "SendOffers",
                            Target = new Uri("http://tempuri.org/Products(1)/Default.SendOffers"),
                            Metadata = new Uri("http://tempuri.org/$metadata#Default.SendOffers")
                        });

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.productsEntitySet, this.productEntityType);

                    resourceWriter.WriteStart(tempProductResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_OperationInRequest("http://tempuri.org/$metadata#Default.SendOffers"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_EnumerableContainsANullItem(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Lenovo ThinkPad P53s" }
                        }
                    };

                    tempProductResource.MetadataBuilder = new NullActionTempEntityMetadataBuilder(tempProductResource.Id);

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.productsEntitySet, this.productEntityType);

                    resourceWriter.WriteStart(tempProductResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_EnumerableContainsANullItem("ODataResource.Actions"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty { Name = "Name", Value = "Lenovo ThinkPad P53s" }
                        }
                    };

                    tempProductResource.AddAction(
                        new ODataAction
                        {
                            Title = "SendOffers",
                            Target = new Uri("http://tempuri.org/Products(1)/Default.SendOffers")
                        });

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.productsEntitySet, this.productEntityType);

                    resourceWriter.WriteStart(tempProductResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata("ODataAction"),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_StreamReferenceValuesNotSupportedInCollections(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = this.stream };

            using (var messageWriter = new ODataMessageWriter(requestMessage, this.messageWriterSettings))
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Features",
                                Value = new ODataCollectionValue
                                {
                                    Items = new List<object>
                                    {
                                        new ODataStreamReferenceValue
                                        {
                                            EditLink = new Uri($"http://tempuri.org/Products(1)/Photo"),
                                            ReadLink = new Uri($"http://tempuri.org/Products(1)/Photo"),
                                            ContentType = "image/png",
                                            ETag = "media-etag"
                                        }
                                    }
                                }
                            }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter();

                    resourceWriter.WriteStart(tempProductResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_StreamReferenceValuesNotSupportedInCollections,
                    exception.Message);
            }
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_NestedCollectionsAreNotSupported(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = this.stream };

            using (var messageWriter = new ODataMessageWriter(requestMessage, this.messageWriterSettings))
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Features",
                                Value = new ODataCollectionValue
                                {
                                    Items = new List<object>
                                    {
                                        new ODataCollectionValue
                                        {
                                            Items = new List<object>
                                            {
                                                3.142857142
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter();

                    resourceWriter.WriteStart(tempProductResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_NestedCollectionsAreNotSupported,
                    exception.Message);
            }
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_NonNullableCollectionElementsMustNotBeNull(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempCustomerResource = new ODataResource
                    {
                        TypeName = "NS.Customer",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Emails",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "Collection(Edm.String)",
                                    Items = new [] { (string)null }
                                }
                            }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(tempCustomerResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_NonNullableCollectionElementsMustNotBeNull,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_RecursionDepthLimitReached(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 1;

            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var odataError = new ODataError
                {
                    Code = "OEC1",
                    Message = "OEM1",
                    Target = "OET1",
                    InnerError = new ODataInnerError(
                        new Dictionary<string, ODataValue>
                        {
                                { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM1") },
                                { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET1") },
                                { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES1") }
                        })
                    {
                        InnerError = new ODataInnerError(
                            new Dictionary<string, ODataValue>
                            {
                                    { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM2") },
                                    { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET2") },
                                    { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES2") }
                            })
                        {
                            InnerError = new ODataInnerError(
                                new Dictionary<string, ODataValue>
                                {
                                        { JsonConstants.ODataErrorInnerErrorMessageName, new ODataPrimitiveValue("OIEM3") },
                                        { JsonConstants.ODataErrorInnerErrorTypeNameName, new ODataPrimitiveValue("OIET3") },
                                        { JsonConstants.ODataErrorInnerErrorStackTraceName, new ODataPrimitiveValue("OIES3") }
                                })
                        }
                    }
                };

                var exception = Assert.Throws<ODataException>(
                    () => messageWriter.WriteError(odataError, includeDebugInformation: true));

                Assert.Equal(
                    Strings.ValidationUtils_RecursionDepthLimitReached(1),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_MissingTypeNameWithMetadata(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Features",
                                Value = new ODataCollectionValue
                                {
                                    Items = new List<object>
                                    {
                                        3.142857142
                                    }
                                }
                            }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.productsEntitySet, this.productEntityType);

                    resourceWriter.WriteStart(tempProductResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.WriterValidationUtils_MissingTypeNameWithMetadata,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_TypeNameMustNotBeEmpty(ODataVersion odataVersion)
        {
            SetupRequestMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var tempProductResource = new ODataResource
                    {
                        TypeName = "NS.Product",
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Id", Value = 1 },
                            new ODataProperty
                            {
                                Name = "Features",
                                Value = new ODataCollectionValue
                                {
                                    TypeName = "",
                                    Items = new List<object>
                                    {
                                        3.142857142
                                    }
                                }
                            }
                        }
                    };

                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.productsEntitySet, this.productEntityType);

                    resourceWriter.WriteStart(tempProductResource);
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_TypeNameMustNotBeEmpty,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_ResourceMustSpecifyName(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var serviceDocument = new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo>
                    {
                        new ODataEntitySetInfo
                        {
                            Title = "Products",
                            Url = new Uri("Products", UriKind.Relative)
                        }
                    }
                };

                var exception = Assert.Throws<ODataException>(
                    () => messageWriter.WriteServiceDocument(serviceDocument));
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_ResourceMustSpecifyUrl(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var serviceDocument = new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo>
                    {
                        new ODataEntitySetInfo
                        {
                            Name = "Products",
                            Title = "Products"
                        }
                    }
                };

                var exception = Assert.Throws<ODataException>(
                    () => messageWriter.WriteServiceDocument(serviceDocument));

                Assert.Equal(
                    Strings.ValidationUtils_ResourceMustSpecifyUrl,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_WorkspaceResourceMustNotContainNullItem(ODataVersion odataVersion)
        {
            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var serviceDocument = new ODataServiceDocument
                {
                    EntitySets = new List<ODataEntitySetInfo>
                    {
                        null
                    }
                };

                var exception = Assert.Throws<ODataException>(
                    () => messageWriter.WriteServiceDocument(serviceDocument));

                Assert.Equal(
                    Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem,
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_MaxDepthOfNestedEntriesExceeded(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            this.messageWriterSettings.MessageQuotas.MaxNestingDepth = 2;

            SetupResponseMessageWriterAndRunTest((messageWriter) =>
            {
                var exception = Assert.Throws<ODataException>(() =>
                {
                    var resourceWriter = messageWriter.CreateODataResourceWriter(this.customersEntitySet, this.customerEntityType);

                    resourceWriter.WriteStart(this.customerResource);
                    resourceWriter.WriteStart(this.ordersNestedResourceInfo);
                    resourceWriter.WriteStart(new ODataResourceSet());
                    resourceWriter.WriteStart(this.orderResource);
                    resourceWriter.WriteStart(this.shippingAddressNestedResourceInfo);
                    resourceWriter.WriteStart(this.shippingAddressResource);
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                    resourceWriter.WriteEnd();
                });

                Assert.Equal(
                    Strings.ValidationUtils_MaxDepthOfNestedEntriesExceeded(2),
                    exception.Message);
            }, odataVersion);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute(ODataVersion odataVersion)
        {
            var exception = Assert.Throws<ODataException>(() =>
            {
                this.messageWriterSettings.BaseUri = new Uri("/", UriKind.Relative);

                SetupResponseMessageWriterAndRunTest((messageWriter) => { }, odataVersion);
            });

            Assert.Equal(
            Strings.WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute('/'),
            exception.Message);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage(ODataVersion odataVersion)
        {
            var exception = Assert.Throws<ODataException>(() =>
            {
                this.messageWriterSettings.JsonPCallback = "callback";

                SetupRequestMessageWriterAndRunTest((messageWriter) => { }, odataVersion);
            });

            Assert.Equal(
                Strings.WriterValidationUtils_MessageWriterSettingsJsonPaddingOnRequestMessage,
                exception.Message);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_ResourceWithoutMediaResourceAndMLETypeAsync(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;

            var exception = Assert.Throws<ODataException>(() =>
            {
                var tempProductResource = new ODataResource
                {
                    TypeName = "NS.Product",
                    Id = new Uri("http://tempuri.org/Products(1)"),
                    Properties = new List<ODataProperty>
                    {
                        new ODataProperty { Name = "Id", Value = 1 },
                        new ODataProperty { Name = "Name", Value = "Lenovo ThinkPad P53s" }
                    }
                };

                var writerValidator = new WriterValidator(this.messageWriterSettings);

                writerValidator.ValidateMetadataResource(tempProductResource, this.productEntityType);
            });

            Assert.Equal(
                Strings.ValidationUtils_ResourceWithoutMediaResourceAndMLEType("NS.Product"),
                exception.Message);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_ResourceWithMediaResourceAndNonMLETypeAsync(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;

            var exception = Assert.Throws<ODataException>(() =>
            {
                var tempCustomerResource = new ODataResource
                {
                    TypeName = "NS.Customer",
                    Id = new Uri("http://tempuri.org/Customers(1)"),
                    Properties = new List<ODataProperty>
                    {
                        new ODataProperty { Name = "Id", Value = 1 },
                        new ODataProperty { Name = "Name", Value = "Sue" }
                    },
                    MediaResource = new ODataStreamReferenceValue
                    {
                        EditLink = new Uri($"http://tempuri.org/Customers(1)/Photo"),
                        ReadLink = new Uri($"http://tempuri.org/Customers(1)/Photo"),
                        ContentType = "image/png",
                        ETag = "media-etag"
                    }
                };

                var writerValidator = new WriterValidator(this.messageWriterSettings);

                writerValidator.ValidateMetadataResource(tempCustomerResource, this.customerEntityType);
            });

            Assert.Equal(
                Strings.ValidationUtils_ResourceWithMediaResourceAndNonMLEType("NS.Customer"),
                exception.Message);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void ValidationUtils_LinkMustSpecifyName(ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;

            var exception = Assert.Throws<ODataException>(() =>
            {
                var tempCustomerNestedResourceInfo = new ODataNestedResourceInfo
                {
                    // Name = "Customer", // Name not specified
                    IsCollection = false,
                    IsComplex = false,
                    Url = new Uri("http://tempuri.org/Orders(1)/Customer")
                };

                new WriterValidator(this.messageWriterSettings).ValidateNestedResourceInfo(
                    tempCustomerNestedResourceInfo,
                    this.orderEntityType,
                    expandedPayloadKind: null);
            });

            Assert.Equal(
                Strings.ValidationUtils_LinkMustSpecifyName,
                exception.Message);
        }

        [Theory]
        [MemberData(nameof(GetODataVersions))]
        public void WriterValidationUtils_MessageWriterSettingsServiceDocumentUriMustBeNullOrAbsolute(ODataVersion odataVersion)
        {
            var exception = Assert.Throws<ODataException>(() =>
            {
                var tempMessageWriterSettings = new ODataMessageWriterSettings
                {
                    EnableMessageStreamDisposal = false,
                    Version = odataVersion,
                    ODataUri = new ODataUri { ServiceRoot = new Uri("/", UriKind.Relative) }
                };
            });

            Assert.Equal(
                Strings.WriterValidationUtils_MessageWriterSettingsServiceDocumentUriMustBeNullOrAbsolute('/'),
                exception.Message);
        }

        #endregion Synchronous Tests

        #region Helper Methods

        private async Task SetupResponseMessageWriterAndRunTestAsync(Func<ODataMessageWriter, Task> func, ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            IODataResponseMessage responseMessage = new InMemoryMessage { Stream = this.stream };

            await using (var messageWriter = new ODataMessageWriter(responseMessage, this.messageWriterSettings, this.model))
            {
                await func(messageWriter);
            }
        }

        private async Task SetupRequestMessageWriterAndRunTestAsync(Func<ODataMessageWriter, Task> func, ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = this.stream };

            await using (var messageWriter = new ODataMessageWriter(requestMessage, this.messageWriterSettings, this.model))
            {
                await func(messageWriter);
            }
        }

        private void SetupResponseMessageWriterAndRunTest(Action<ODataMessageWriter> action, ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            IODataResponseMessage responseMessage = new InMemoryMessage { Stream = this.stream };

            using (var messageWriter = new ODataMessageWriter(responseMessage, this.messageWriterSettings, this.model))
            {
                action(messageWriter);
            }
        }

        private void SetupRequestMessageWriterAndRunTest(Action<ODataMessageWriter> action, ODataVersion odataVersion)
        {
            this.messageWriterSettings.Version = odataVersion;
            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = this.stream };

            using (var messageWriter = new ODataMessageWriter(requestMessage, this.messageWriterSettings, this.model))
            {
                action(messageWriter);
            }
        }

        #endregion Helper Methods

        internal class TempEntityMetadataBuilder : ODataResourceMetadataBuilder
        {
            private readonly Uri resourceId;
            private ODataStreamReferenceValue mediaResource;
            private readonly string eTag;

            internal TempEntityMetadataBuilder(Uri resourceId, ODataStreamReferenceValue mediaResource, string eTag)
            {
                this.resourceId = resourceId;
                this.mediaResource = mediaResource;
                this.eTag = eTag;
            }

            internal override Uri GetEditLink()
            {
                return this.resourceId;
            }

            internal override string GetETag()
            {
                return this.eTag;
            }

            internal override Uri GetId()
            {
                return this.resourceId;
            }

            internal override Uri GetReadLink()
            {
                return this.resourceId;
            }

            internal override bool TryGetIdForSerialization(out Uri id)
            {
                id = this.resourceId;

                return true;
            }

            internal override ODataStreamReferenceValue GetMediaResource()
            {
                return this.mediaResource;
            }
        }

        internal class NullActionTempEntityMetadataBuilder : TempEntityMetadataBuilder
        {
            internal NullActionTempEntityMetadataBuilder(Uri resourceId)
                : base(resourceId, null, null)
            {
            }

            internal override IEnumerable<ODataAction> GetActions()
            {
                return new List<ODataAction>
                {
                    null
                };
            }
        }
    }
}