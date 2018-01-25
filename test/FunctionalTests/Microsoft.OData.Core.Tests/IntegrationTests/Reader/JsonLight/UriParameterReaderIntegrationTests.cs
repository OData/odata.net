//---------------------------------------------------------------------
// <copyright file="UriParameterReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Reader.JsonLight
{
    public class UriParameterReaderIntegrationTests
    {
        private readonly Uri metadataDocumentUri = new Uri("http://odata.org/test/$metadata");
        private readonly string ns = "NS";
        private readonly EdmModel model;
        private readonly EdmEntitySet peopleSet;
        private readonly EdmEntityType personT;
        private readonly EdmComplexType companyAddressT;
        private readonly EdmComplexType addressT;
        private readonly EdmComplexType cityT;
        private readonly EdmComplexType otherInfoT;
        private readonly EdmCollectionType billingAddressesT;

        public UriParameterReaderIntegrationTests()
        {
            model = new EdmModel();

            cityT = new EdmComplexType(ns, "City");
            cityT.AddStructuralProperty("CityName", EdmPrimitiveTypeKind.String, false);
            cityT.AddStructuralProperty("AreaCode", EdmPrimitiveTypeKind.String, true);
            model.AddElement(cityT);

            addressT = new EdmComplexType(ns, "Address");
            addressT.AddStructuralProperty("City", new EdmComplexTypeReference(cityT, true));
            model.AddElement(addressT);

            companyAddressT = new EdmComplexType(ns, "CompanyAddress", addressT, false);
            companyAddressT.AddStructuralProperty("CompanyName", EdmPrimitiveTypeKind.String);
            model.AddElement(companyAddressT);

            otherInfoT = new EdmComplexType(ns, "OtherInfo", null, false, true);
            otherInfoT.AddStructuralProperty("Hight", EdmPrimitiveTypeKind.Int32);
            model.AddElement(otherInfoT);

            personT = new EdmEntityType("NS", "Person", null, false, true);
            var Id = personT.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            personT.AddKeys(Id);
            personT.AddStructuralProperty("Address", new EdmComplexTypeReference(addressT, false));
            personT.AddStructuralProperty("CompanyAddress", new EdmComplexTypeReference(companyAddressT, true));
            billingAddressesT = new EdmCollectionType(new EdmComplexTypeReference(addressT, false));
            personT.AddStructuralProperty("BillingAddresses", new EdmCollectionTypeReference(billingAddressesT));
            model.AddElement(personT);

            EdmEntityContainer container = new EdmEntityContainer(ns, "Container");
            peopleSet = new EdmEntitySet(container, "people", personT);
            model.AddElement(container);
        }

        private void ReadSingletonParameter(string payload, IEdmStructuredType resourceType, out List<ODataResourceSet> resourceSets, out List<ODataResource> resources, out List<ODataNestedResourceInfo> nestedResourceInfos)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload))};
            var settings = new ODataMessageReaderSettings();
            using (var msgReader = new ODataMessageReader((IODataRequestMessage)message, settings, model))
            {
                var reader = msgReader.CreateODataUriParameterResourceReader((resourceType == null || resourceType is IEdmComplexType) ? null : peopleSet, resourceType);
                resources = new List<ODataResource>();
                nestedResourceInfos = new List<ODataNestedResourceInfo>();
                resourceSets = new List<ODataResourceSet>();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        resources.Add(reader.Item as ODataResource);
                    }
                    else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                    {
                        nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        resourceSets.Add(reader.Item as ODataResourceSet);
                    }
                }
            }
        }

        private void ReadCollectionParameter(string payload, IEdmStructuredType resourceType, out List<ODataResourceSet> resourceSets, out List<ODataResource> resources, out List<ODataNestedResourceInfo> nestedResourceInfos)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            var settings = new ODataMessageReaderSettings();
            using (var msgReader = new ODataMessageReader((IODataRequestMessage)message, settings, model))
            {
                var reader = msgReader.CreateODataUriParameterResourceSetReader((resourceType == null || resourceType is IEdmComplexType) ? null : peopleSet, resourceType);
                resources = new List<ODataResource>();
                nestedResourceInfos = new List<ODataNestedResourceInfo>();
                resourceSets = new List<ODataResourceSet>();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        resources.Add(reader.Item as ODataResource);
                    }
                    else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                    {
                        nestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        resourceSets.Add(reader.Item as ODataResourceSet);
                    }
                }
            }
        }

        private void ReadAndValidate(string payload, IEdmStructuredType resourceType, bool isSingleton, Action<List<ODataResource>, List<ODataNestedResourceInfo>, List<ODataResourceSet>> verification)
        {
            List<ODataResource> resources;
            List<ODataNestedResourceInfo> nestedResourceInfos;
            List<ODataResourceSet> resourceSets;

            if (isSingleton)
            {
                ReadSingletonParameter(payload, resourceType, out resourceSets, out resources, out nestedResourceInfos);
            }
            else
            {
                ReadCollectionParameter(payload, resourceType, out resourceSets, out resources, out nestedResourceInfos);
            }

            verification(resources, nestedResourceInfos, resourceSets);
        }

        [Fact]
        public void ReadComplexParameter()
        {
            var payload =
                "{" +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"Shanghai\"," +
                        "\"AreaCode\":\"021\"" +
                    "}" +
                "}";

            ReadAndValidate(payload, addressT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.First().Properties.First().Value.ShouldBeEquivalentTo("Shanghai");
                resources.First().Properties.Last().Value.ShouldBeEquivalentTo("021");
                nestedResourceInfos.First().Name.ShouldBeEquivalentTo("City");
            });
        }

        [Fact]
        public void ReadComplexParameter_NoExpectedType()
        {
            var payload =
                "{" +
                    "\"@odata.type\":\"#NS.CompanyAddress\"," +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"Shanghai\"," +
                        "\"AreaCode\":\"021\"" +
                    "}" +
                "}";

            ReadAndValidate(payload, null, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.First().Properties.First().Value.ShouldBeEquivalentTo("Shanghai");
                resources.First().Properties.Last().Value.ShouldBeEquivalentTo("021");
                resources.Last().TypeName.ShouldBeEquivalentTo("NS.CompanyAddress");
                nestedResourceInfos.First().Name.ShouldBeEquivalentTo("City");
            });
        }

        [Fact]
        public void ReadComplexParameter_Noodatatype_NoExpectedType()
        {
            var payload =
                "{" +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"Shanghai\"," +
                        "\"AreaCode\":\"021\"" +
                    "}" +
                "}";

            // Not supported.
            Exception ex = Assert.Throws<ODataException>(() =>
                ReadAndValidate(payload, null, true, (resources, nestedResourceInfos, resourceSets) =>
                {
                    resources.First().Properties.First().Value.ShouldBeEquivalentTo("Shanghai");
                    resources.First().Properties.Last().Value.ShouldBeEquivalentTo("021");
                    resources.Last().TypeName.ShouldBeEquivalentTo("NS.CompanyAddress");
                    nestedResourceInfos.First().Name.ShouldBeEquivalentTo("City");
                }));
            Assert.Contains("A resource without a type name was found", ex.Message);
        }

        [Fact]
        public void ReadComplexParameter_Inherit()
        {
            var payload =
                "{" +
                    "\"@odata.type\":\"#NS.CompanyAddress\"," +
                    "\"CompanyName\":\"MS\"," +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"Shanghai\"," +
                        "\"AreaCode\":\"021\"" +
                    "}" +
                "}";

            ReadAndValidate(payload, addressT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.First().Properties.First().Value.ShouldBeEquivalentTo("Shanghai");
                resources.First().Properties.Last().Value.ShouldBeEquivalentTo("021");
                resources.Last().Properties.Single().Value.ShouldBeEquivalentTo("MS");
                nestedResourceInfos.First().Name.ShouldBeEquivalentTo("City");
            });
        }

        [Fact]
        public void ReadComplexParameter_UndeclaredProperty()
        {
            var payload =
                "{" +
                    "\"Hight\":180," +
                    "\"City\":" +
                    "{" +
                        "\"@odata.type\":\"NS.City\"," +
                        "\"CityName\":\"Shanghai\"," +
                        "\"AreaCode\":\"021\"" +
                    "}" +
                "}";

            ReadAndValidate(payload, otherInfoT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.First().TypeName.ShouldBeEquivalentTo("NS.City");
                resources.First().Properties.Count().ShouldBeEquivalentTo(2);
                resources.Last().Properties.Single().Value.ShouldBeEquivalentTo(180);
                nestedResourceInfos.First().Name.ShouldBeEquivalentTo("City");
            });
        }

        [Fact]
        public void ReadComplexParameter_WithNullProperty()
        {
            var payload =
                "{" +
                    "\"@odata.type\":\"#NS.CompanyAddress\"," +
                    "\"CompanyName\":null," +
                    "\"City\":null" +
                "}";

            ReadAndValidate(payload, addressT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.First().Should().BeNull();
                resources.Last().Properties.Single().Value.Should().BeNull();
                resources.Last().TypeName.ShouldBeEquivalentTo("NS.CompanyAddress");
            });
        }

        [Fact]
        public void ReadComplexColParameter()
        {
            var payload =
                "[" +
                    "{" +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City1\"," +
                            "\"AreaCode\":\"AreaCode1\"" +
                        "}" +
                    "}," +
                    "{" +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City2\"," +
                            "\"AreaCode\":\"AreaCode2\"" +
                        "}" +
                    "}" +
                "]";

            ReadAndValidate(payload, addressT, false, (resources, nestedResourceInfos, resourceSets) =>
            {
                resourceSets.Count.ShouldBeEquivalentTo(1);
                resources.ElementAt(0).Properties.Count().ShouldBeEquivalentTo(2);
                resources.ElementAt(1).Properties.Count().ShouldBeEquivalentTo(0);
                resources.ElementAt(2).Properties.Count().ShouldBeEquivalentTo(2);
                resources.ElementAt(3).Properties.Count().ShouldBeEquivalentTo(0);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(2);
            });
        }

        [Fact]
        public void ReadComplexColParameter_NoExpectedType()
        {
            var payload =
                "[" +
                    "{" +
                        "\"@odata.type\":\"#NS.Address\"," +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City1\"," +
                            "\"AreaCode\":\"AreaCode1\"" +
                        "}" +
                    "}," +
                    "{" +
                        "\"@odata.type\":\"#NS.CompanyAddress\"," +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City2\"," +
                            "\"AreaCode\":\"AreaCode2\"" +
                        "}" +
                    "}" +
                "]";

            ReadAndValidate(payload, null, false, (resources, nestedResourceInfos, resourceSets) =>
            {
                resourceSets.Count.ShouldBeEquivalentTo(1);
                resources.ElementAt(0).Properties.Count().ShouldBeEquivalentTo(2);
                resources.ElementAt(1).TypeName.ShouldBeEquivalentTo("NS.Address");
                resources.ElementAt(2).Properties.Count().ShouldBeEquivalentTo(2);
                resources.ElementAt(3).TypeName.ShouldBeEquivalentTo("NS.CompanyAddress");
                nestedResourceInfos.Count.ShouldBeEquivalentTo(2);
            });
        }

        [Fact]
        public void ReadComplexColParameter_Inheirt()
        {
            var payload =
                "[" +
                    "{" +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City1\"," +
                            "\"AreaCode\":\"AreaCode1\"" +
                        "}" +
                    "}," +
                    "{" +
                        "\"@odata.type\":\"#NS.CompanyAddress\"," +
                        "\"CompanyName\":null," +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City2\"," +
                            "\"AreaCode\":\"AreaCode2\"" +
                        "}" +
                    "}" +
                "]";

            ReadAndValidate(payload, addressT, false, (resources, nestedResourceInfos, resourceSets) =>
            {
                resourceSets.Count.ShouldBeEquivalentTo(1);
                resources.ElementAt(0).Properties.Count().ShouldBeEquivalentTo(2);
                resources.ElementAt(1).Properties.Count().ShouldBeEquivalentTo(0);
                resources.ElementAt(2).Properties.Count().ShouldBeEquivalentTo(2);
                resources.ElementAt(3).Properties.Count().ShouldBeEquivalentTo(1);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(2);
            });
        }

        [Fact]
        public void ReadEntityParameter()
        {
            var payload =
                "{" +
                    "\"Id\":0," +
                    "\"CompanyAddress\":" +
                    "{" +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City0\"," +
                            "\"AreaCode\":\"AreaCode0\"" +
                        "}" +
                    "}," +
                    "\"BillingAddresses\":" +
                    "[" +
                        "{" +
                            "\"City\":" +
                            "{" +
                                "\"CityName\":\"City1\"," +
                                "\"AreaCode\":\"AreaCode1\"" +
                            "}" +
                        "}," +
                        "{" +
                            "\"@odata.type\":\"#NS.CompanyAddress\"," +
                            "\"CompanyName\":null," +
                            "\"City\":" +
                            "{" +
                                "\"CityName\":\"City2\"," +
                                "\"AreaCode\":\"AreaCode2\"" +
                            "}" +
                        "}" +
                    "]," +
                    "\"Address\":" +
                    "{" +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City3\"," +
                            "\"AreaCode\":\"AreaCode3\"" +
                        "}" +
                    "}" +
                "}";

            ReadAndValidate(payload, personT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.Count.ShouldBeEquivalentTo(9);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(7);
                
                nestedResourceInfos.ElementAt(1).Name.ShouldBeEquivalentTo("CompanyAddress");
                resources.ElementAt(1).TypeName.ShouldBeEquivalentTo("NS.CompanyAddress");

                resourceSets.Count.ShouldBeEquivalentTo(1);
                nestedResourceInfos.ElementAt(4).Name.ShouldBeEquivalentTo("BillingAddresses");
                resources.ElementAt(5).Properties.Single().Value.Should().BeNull();

                nestedResourceInfos.ElementAt(6).Name.ShouldBeEquivalentTo("Address");
                resources.ElementAt(7).TypeName.ShouldBeEquivalentTo("NS.Address");

                resources.ElementAt(8).Properties.Single().Value.ShouldBeEquivalentTo(0);
            });
        }

        [Fact]
        public void ReadEntityParameter_WithNullOrEmptyCollection()
        {
            var payload =
            "{" +
                "\"Id\":0," +
                "\"CompanyAddress\":null," +
                "\"BillingAddresses\":[]," +
                "\"Address\":" +
                "{" +
                    "\"City\":{}" +
                "}" +
            "}";

            ReadAndValidate(payload, personT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.Count.ShouldBeEquivalentTo(4);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(4);

                nestedResourceInfos.ElementAt(0).Name.ShouldBeEquivalentTo("CompanyAddress");
                resources.ElementAt(0).Should().BeNull();

                resourceSets.Count.ShouldBeEquivalentTo(1);
                nestedResourceInfos.ElementAt(1).Name.ShouldBeEquivalentTo("BillingAddresses");

                nestedResourceInfos.ElementAt(3).Name.ShouldBeEquivalentTo("Address");
                resources.ElementAt(1).Properties.Count().ShouldBeEquivalentTo(0);
            });

        }

        [Fact]
        public void ReadEntityParameter_WithUndeclaredResourceSet()
        {
            var payload =
            "{" +
                "\"Id\":0," +
                "\"Address\":" +
                "{" +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"City3\"," +
                        "\"AreaCode\":\"AreaCode3\"" +
                    "}" +
                "}," +
                "\"OtherAddresses@odata.type\":\"#Collection(NS.CompanyAddress)\"," +
                "\"OtherAddresses\":" +
                "[" +
                    "{" +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City1\"," +
                            "\"AreaCode\":\"AreaCode1\"" +
                        "}" +
                    "}," +
                    "{" +
                        "\"CompanyName\":null," +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City2\"," +
                            "\"AreaCode\":\"AreaCode2\"" +
                        "}" +
                    "}" +
                "]" +
            "}";

            ReadAndValidate(payload, personT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.Count.ShouldBeEquivalentTo(7);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(5);
                resourceSets.Count.ShouldBeEquivalentTo(1);

                nestedResourceInfos.ElementAt(4).Name.ShouldBeEquivalentTo("OtherAddresses");

                resources.ElementAt(3).TypeName.ShouldBeEquivalentTo("NS.CompanyAddress");
                resources.ElementAt(5).TypeName.ShouldBeEquivalentTo("NS.CompanyAddress");
            });

        }

        [Fact]
        public void ReadEntityParameter_WithUndeclaredEmptyResourceSet()
        {
            var payload =
            "{" +
                "\"Id\":0," +
                "\"Address\":" +
                "{" +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"City3\"," +
                        "\"AreaCode\":\"AreaCode3\"" +
                    "}" +
                "}," +
                "\"OtherAddresses@odata.type\":\"#Collection(NS.CompanyAddress)\"," +
                "\"OtherAddresses\":[]" +
            "}";

            ReadAndValidate(payload, personT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.Count.ShouldBeEquivalentTo(3);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(3);
                resourceSets.Count.ShouldBeEquivalentTo(1);

                nestedResourceInfos.ElementAt(2).Name.ShouldBeEquivalentTo("OtherAddresses");
                nestedResourceInfos.ElementAt(2).IsCollection.ShouldBeEquivalentTo(true);
            });
        }

        [Fact]
        public void ReadEntityParameter_WithUndeclaredResource()
        {
            var payload =
            "{" +
                "\"Id\":0," +
                "\"Address\":" +
                "{" +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"City1\"," +
                        "\"AreaCode\":\"AreaCode1\"" +
                    "}" +
                "}," +
                "\"OtherAddresses\":" +
                "{" +
                    "\"@odata.type\":\"#NS.CompanyAddress\"," +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"City2\"," +
                        "\"AreaCode\":\"AreaCode2\"" +
                    "}" +
                "}" +
            "}";

            ReadAndValidate(payload, personT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.Count.ShouldBeEquivalentTo(5);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(4);

                nestedResourceInfos.ElementAt(3).Name.ShouldBeEquivalentTo("OtherAddresses");
                nestedResourceInfos.ElementAt(3).IsCollection.ShouldBeEquivalentTo(false);

                resources.ElementAt(3).TypeName.ShouldBeEquivalentTo("NS.CompanyAddress");
            });
        }

        [Fact]
        public void ReadEntityParameter_WithUndeclaredResource_NoExpectedType()
        {
            var payload =
            "{" +
                "\"@odata.type\":\"NS.Person\"," +
                "\"Id\":0," +
                "\"Address\":" +
                "{" +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"City3\"," +
                        "\"AreaCode\":\"AreaCode3\"" +
                    "}" +
                "}," +
                "\"OtherAddresses@odata.type\":\"#Collection(NS.CompanyAddress)\"," +
                "\"OtherAddresses\":[]" +
            "}";

            ReadAndValidate(payload, personT, true, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.Count.ShouldBeEquivalentTo(3);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(3);
                resourceSets.Count.ShouldBeEquivalentTo(1);

                nestedResourceInfos.ElementAt(2).Name.ShouldBeEquivalentTo("OtherAddresses");
                nestedResourceInfos.ElementAt(2).IsCollection.ShouldBeEquivalentTo(true);
            });
        }

        [Fact]
        public void ReadEntityParameter_WithUndeclaredNullResource()
        {
            var payload =
            "{" +
                "\"Id\":0," +
                "\"Address\":" +
                "{" +
                    "\"City\":" +
                    "{" +
                        "\"CityName\":\"City3\"," +
                        "\"AreaCode\":\"AreaCode3\"" +
                    "}" +
                "}," +
                "\"OtherAddresses@odata.type\":\"#NS.CompanyAddress\"," +
                "\"OtherAddresses\":null" +
            "}";

            // Not supported.
            Exception ex = Assert.Throws<ODataException>(() =>
                ReadAndValidate(payload, personT, true, (resources, nestedResourceInfos, resourceSets) =>
                {
                    resources.Count.ShouldBeEquivalentTo(4);
                    nestedResourceInfos.Count.ShouldBeEquivalentTo(3);

                    nestedResourceInfos.ElementAt(2).Name.ShouldBeEquivalentTo("OtherAddresses");
                    nestedResourceInfos.ElementAt(2).IsCollection.ShouldBeEquivalentTo(false);

                    resources.ElementAt(2).Should().BeNull();
                }));
            Assert.Contains("A complex property with an 'odata.type' property annotation was found", ex.Message);
        }

        [Fact]
        public void ReadEntityColParameter()
        {
            var payload =
            "[" +
                "{" +
                    "\"Id\":0," +
                    "\"Address\":" +
                    "{" +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City1\"," +
                            "\"AreaCode\":\"AreaCode1\"" +
                        "}" +
                    "}" +
                 "}," +
                "{" +
                    "\"Id\":1," +
                    "\"Address\":" +
                    "{" +
                        "\"City\":" +
                        "{" +
                            "\"CityName\":\"City1\"," +
                            "\"AreaCode\":\"AreaCode1\"" +
                        "}" +
                    "}" +
                "}" +
            "]";

            ReadAndValidate(payload, personT, false, (resources, nestedResourceInfos, resourceSets) =>
            {
                resources.Count.ShouldBeEquivalentTo(6);
                nestedResourceInfos.Count.ShouldBeEquivalentTo(4);
            });
        }
    }
}
