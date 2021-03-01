//---------------------------------------------------------------------
// <copyright file="UriParameterWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Writer.JsonLight
{
    public class UriParameterWriterIntegrationTests
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

        public UriParameterWriterIntegrationTests()
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

        [Fact]
        public void WriteComplexParameter_WithModel()
        {
            var resource = CreateAddress(1);
            WriteParameter(resource, true, (actual) => Assert.Equal("{\"@odata.type\":\"#NS.Address\",\"City\":{\"CityName\":\"City1\",\"AreaCode\":\"AreaCode1\"}}", actual));
        }

        [Fact]
        public void WriteComplexParameter_WithoutModel()
        {
            var resource = CreateAddress(1);
            WriteParameter(resource, false, (actual) => Assert.Equal(
                "{" +
                    "\"@odata.type\":\"#NS.Address\"," +
                    "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City1\",\"AreaCode\":\"AreaCode1\"}" +
                "}",
                actual));
        }

        [Fact]
        public void WriteComplexColParameter_WithModel()
        {
            WriteResourceSetParameter(true, false,
                (actual) =>
                    Assert.Equal("[{\"@odata.type\":\"#NS.Address\",\"City\":{\"CityName\":\"City0\",\"AreaCode\":\"AreaCode0\"}},{\"@odata.type\":\"#NS.CompanyAddress\",\"CompanyName\":\"MS\",\"City\":{\"CityName\":\"City1\",\"AreaCode\":\"AreaCode1\"}}]", actual));
        }

        [Fact]
        public void WriteComplexColParameter_WithoutModel()
        {
            WriteResourceSetParameter(false, false,
                (actual) => Assert.Equal(actual,
                    "[" +
                        "{" +
                            "\"@odata.type\":\"#NS.Address\"," +
                            "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City0\",\"AreaCode\":\"AreaCode0\"}" +
                        "}," +
                        "{" +
                            "\"@odata.type\":\"#NS.CompanyAddress\"," +
                            "\"CompanyName\":\"MS\"," +
                            "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City1\",\"AreaCode\":\"AreaCode1\"}" +
                        "}" +
                    "]"));
        }

        [Fact]
        public void WriteComplexColParameter_WithEmpty()
        {
            ResourceWrapper resource = new ResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = "NS.OtherInfo"
                }
            };
            WriteParameter(resource, true, (actual) => Assert.Equal("{\"@odata.type\":\"#NS.OtherInfo\"}", actual));
            WriteParameter(resource, false, (actual) => Assert.Equal("{\"@odata.type\":\"#NS.OtherInfo\"}", actual));
        }


        [Fact]
        public void WriteComplexParameter_WithNestedResource()
        {
            var resource = new ResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = "NS.OtherInfo",
                    Properties = new List<ODataProperty>()
                    {
                        new ODataProperty(){ Name = "Hight", Value = 12 },
                    }
                },
                NestedResourceInfos = new List<NestedResourceInfoWrapper>()
                {
                    new NestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "City",
                            IsCollection = false
                        },
                        nestedResource = new ResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = "NS.City",
                                Properties = new List<ODataProperty>()
                                {
                                    new ODataProperty(){ Name = "CityName", Value = "City0"},
                                    new ODataProperty(){ Name = "AreaCode", Value = "AreaCode0"},
                                }
                            }
                        }
                    }
                }
            };

            WriteParameter(resource, true,
                (actual) => Assert.Equal("{\"@odata.type\":\"#NS.OtherInfo\",\"Hight\":12,\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City0\",\"AreaCode\":\"AreaCode0\"}}", actual));
        }

        [Fact]
        public void WriteComplexColParameter_WithEmptyCollection()
        {
            ResourceSetWrapper resourceSet = new ResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet()
            };
            WriteParameter(resourceSet, true, (actual) => Assert.Equal("[]", actual));
            WriteParameter(resourceSet, false, (actual) => Assert.Equal("[]", actual));
        }

        [Fact]
        public void WriteComplexParameter_UnknownTypeName()
        {
            var resource = CreateAddress(0);
            resource.Resource.TypeName = "Person";
            Action action = () => WriteParameter(resource, true, null);
            action.Throws<ODataException>(Strings.ValidationUtils_UnrecognizedTypeName("Person"));
        }

        [Fact]
        public void WriteComplexParameter_WithNull()
        {
            var resource = CreateCompanyAddress(0, true);
            WriteParameter(resource, true, (actual) => Assert.Equal("{\"@odata.type\":\"#NS.CompanyAddress\",\"City\":null,\"CompanyName\":null}", actual));
        }

        [Fact]
        public void WriteEntityParameter_WithModel()
        {
            var resource = CreatePerson(0);
            WriteParameter(resource, true, (actual) => Assert.Equal(
                "{" +
                    "\"@odata.type\":\"#NS.Person\"," +
                    "\"Id\":0," +
                    "\"Address\":{\"City\":{\"CityName\":\"City0\",\"AreaCode\":\"AreaCode0\"}}," +
                    "\"CompanyAddress\":{\"CompanyName\":\"MS\",\"City\":{\"CityName\":\"City1\",\"AreaCode\":\"AreaCode1\"}}," +
                    "\"BillingAddresses\":" +
                    "[" +
                        "{\"City\":{\"CityName\":\"City2\",\"AreaCode\":\"AreaCode2\"}}," +
                        "{" +
                            "\"@odata.type\":\"#NS.CompanyAddress\"," +
                            "\"CompanyName\":\"MS\"," +
                            "\"City\":{\"CityName\":\"City3\",\"AreaCode\":\"AreaCode3\"}" +
                        "}" +
                    "]" +
                "}",
                actual));
        }

        [Fact]
        public void WriteEntityParameter_WithoutModel()
        {
            var resource = CreatePerson(0);
            WriteParameter(resource, false, (actual) => Assert.Equal(
                "{" +
                    "\"@odata.type\":\"#NS.Person\"," +
                    "\"Id\":0," +
                    "\"Address\":{\"@odata.type\":\"#NS.Address\",\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City0\",\"AreaCode\":\"AreaCode0\"}}," +
                    "\"CompanyAddress\":" +
                    "{" +
                        "\"@odata.type\":\"#NS.CompanyAddress\"," +
                        "\"CompanyName\":\"MS\"," +
                        "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City1\",\"AreaCode\":\"AreaCode1\"}" +
                    "}," +
                    "\"BillingAddresses\":" +
                    "[" +
                        "{\"@odata.type\":\"#NS.Address\",\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City2\",\"AreaCode\":\"AreaCode2\"}}," +
                        "{" +
                            "\"@odata.type\":\"#NS.CompanyAddress\"," +
                            "\"CompanyName\":\"MS\"," +
                            "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City3\",\"AreaCode\":\"AreaCode3\"}" +
                        "}" +
                    "]" +
                "}",
                actual));
        }

        [Fact]
        public void WriteEntityCollectionParameter_WithModel()
        {
            WriteResourceSetParameter(true, true,
                (actual) =>
                    Assert.Equal(
                    "[" +
                        "{" +
                            "\"@odata.type\":\"#NS.Person\"," +
                            "\"Id\":0," +
                            "\"Address\":{\"City\":{\"CityName\":\"City0\",\"AreaCode\":\"AreaCode0\"}}," +
                            "\"CompanyAddress\":{\"CompanyName\":\"MS\",\"City\":{\"CityName\":\"City1\",\"AreaCode\":\"AreaCode1\"}}," +
                            "\"BillingAddresses\":" +
                            "[" +
                                "{\"City\":{\"CityName\":\"City2\",\"AreaCode\":\"AreaCode2\"}}," +
                                "{" +
                                    "\"@odata.type\":\"#NS.CompanyAddress\"," +
                                    "\"CompanyName\":\"MS\"," +
                                    "\"City\":{\"CityName\":\"City3\",\"AreaCode\":\"AreaCode3\"}" +
                                "}" +
                            "]" +
                        "}," +
                        "{" +
                            "\"@odata.type\":\"#NS.Person\"," +
                            "\"Id\":1," +
                            "\"Address\":{\"City\":{\"CityName\":\"City10\",\"AreaCode\":\"AreaCode10\"}}," +
                            "\"CompanyAddress\":{\"CompanyName\":\"MS\",\"City\":{\"CityName\":\"City11\",\"AreaCode\":\"AreaCode11\"}}," +
                            "\"BillingAddresses\":" +
                            "[" +
                                "{\"City\":{\"CityName\":\"City12\",\"AreaCode\":\"AreaCode12\"}}," +
                                "{" +
                                    "\"@odata.type\":\"#NS.CompanyAddress\"," +
                                    "\"CompanyName\":\"MS\"," +
                                    "\"City\":{\"CityName\":\"City13\",\"AreaCode\":\"AreaCode13\"}" +
                                "}" +
                            "]" +
                        "}" +
                    "]",
                    actual));
        }

        [Fact]
        public void WriteEntityCollectionParameter_WithoutModel()
        {
            WriteResourceSetParameter(false, true,
                (actual) => Assert.Equal(
                    "[" +
                        "{" +
                            "\"@odata.type\":\"#NS.Person\"," +
                            "\"Id\":0," +
                            "\"Address\":{\"@odata.type\":\"#NS.Address\",\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City0\",\"AreaCode\":\"AreaCode0\"}}," +
                            "\"CompanyAddress\":" +
                            "{" +
                                "\"@odata.type\":\"#NS.CompanyAddress\"," +
                                "\"CompanyName\":\"MS\"," +
                                "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City1\",\"AreaCode\":\"AreaCode1\"}" +
                            "}," +
                            "\"BillingAddresses\":" +
                            "[" +
                                "{\"@odata.type\":\"#NS.Address\",\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City2\",\"AreaCode\":\"AreaCode2\"}}," +
                                "{" +
                                    "\"@odata.type\":\"#NS.CompanyAddress\"," +
                                    "\"CompanyName\":\"MS\"," +
                                    "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City3\",\"AreaCode\":\"AreaCode3\"}" +
                                "}" +
                            "]" +
                        "}," +
                        "{" +
                            "\"@odata.type\":\"#NS.Person\"," +
                            "\"Id\":1," +
                            "\"Address\":{\"@odata.type\":\"#NS.Address\",\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City10\",\"AreaCode\":\"AreaCode10\"}}," +
                            "\"CompanyAddress\":" +
                            "{" +
                                "\"@odata.type\":\"#NS.CompanyAddress\"," +
                                "\"CompanyName\":\"MS\"," +
                                "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City11\",\"AreaCode\":\"AreaCode11\"}" +
                            "}," +
                            "\"BillingAddresses\":" +
                            "[" +
                                "{\"@odata.type\":\"#NS.Address\",\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City12\",\"AreaCode\":\"AreaCode12\"}}," +
                                "{" +
                                    "\"@odata.type\":\"#NS.CompanyAddress\"," +
                                    "\"CompanyName\":\"MS\"," +
                                    "\"City\":{\"@odata.type\":\"#NS.City\",\"CityName\":\"City13\",\"AreaCode\":\"AreaCode13\"}" +
                                "}" +
                            "]" +
                        "}" +
                    "]",
                    actual));
        }

        private void WriteResourceSetParameter(bool withModel, bool isEntity, Action<string> validate)
        {
            var resourceSet = CreateResourceSet(isEntity);

            WriteParameter(resourceSet, withModel, validate);
        }

        private ResourceSetWrapper CreateResourceSet(bool isEntity)
        {
            var resourceSet = new ResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet(),
                Resources = new List<ResourceWrapper>()
            };

            if (isEntity)
            {
                for (int i = 0; i < 2; i++)
                {
                    resourceSet.Resources.Add(CreatePerson(i));
                }
            }
            else
            {
                resourceSet.Resources.Add(CreateAddress(0));
                resourceSet.Resources.Add(CreateCompanyAddress(1, false));
            }

            return resourceSet;
        }

        private ResourceWrapper CreatePerson(int id)
        {
            return new ResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = "NS.Person",
                    Properties = new List<ODataProperty>()
                    {
                        new ODataProperty()
                        {
                            Name = "Id",
                            Value = id
                        }
                    }
                },
                NestedResourceInfos = new List<NestedResourceInfoWrapper>
                {
                    new NestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "Address",
                            IsCollection = false
                        },
                        nestedResource = CreateAddress(id * 10)
                    },
                    new NestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "CompanyAddress",
                            IsCollection = false
                        },
                        nestedResource = CreateCompanyAddress(id * 10 + 1, false)
                    },
                    new NestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "BillingAddresses",
                            IsCollection = true
                        },
                        nestedResource = new ResourceSetWrapper()
                        {
                            ResourceSet = new ODataResourceSet(),
                            Resources = new List<ResourceWrapper>()
                            {
                                CreateAddress(id * 10 + 2),
                                CreateCompanyAddress(id * 10 + 3, false)
                            }
                        }
                    }
                }
            };
        }

        private ResourceWrapper CreateCompanyAddress(int id, bool withNullProperty)
        {
            var resource = CreateAddress(id);
            resource.Resource.TypeName = "NS.CompanyAddress";
            var nested = resource.NestedResourceInfos[0].nestedResource as ResourceWrapper;
            if (withNullProperty)
            {
                resource.NestedResourceInfos.Clear();
                resource.Resource.Properties = new[]
                {
                    new ODataProperty() {Name = "City", Value = null},
                    new ODataProperty() { Name = "CompanyName", Value = null }
                };
            }
            else
            {
                resource.Resource.Properties = new[] { new ODataProperty() { Name = "CompanyName", Value = "MS" } };
            }

            return resource;
        }

        private ResourceWrapper CreateAddress(int id)
        {
            return new ResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = "NS.Address",
                },
                NestedResourceInfos = new List<NestedResourceInfoWrapper>()
                {
                    new NestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "City",
                            IsCollection = false
                        },
                        nestedResource = new ResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = "NS.City",
                                Properties = new List<ODataProperty>()
                                {
                                    new ODataProperty(){ Name = "CityName", Value = "City" + id},
                                    new ODataProperty(){ Name = "AreaCode", Value = "AreaCode" + id},
                                }
                            }
                        }
                    }
                }
            };

        }

        private void WriteParameter(ItemWrapper wrapper, bool withModel, Action<string> validate)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var message = new InMemoryMessage { Stream = stream };
                var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };

                using (ODataMessageWriter messageWriter = new ODataMessageWriter((IODataRequestMessage)message, settings, withModel ? model : null))
                {
                    ODataWriter writer;
                    ResourceWrapper resource = wrapper as ResourceWrapper;
                    if (resource != null)
                    {
                        writer = messageWriter.CreateODataUriParameterResourceWriter(null, null);
                        WriteResource(writer, resource);
                    }
                    else
                    {
                        writer = messageWriter.CreateODataUriParameterResourceSetWriter(null, null);
                        WriteResourceSet(writer, wrapper as ResourceSetWrapper);
                    }

                    writer.Flush();
                }

                var actual = Encoding.UTF8.GetString(stream.ToArray());
                validate(actual);
            }
        }

        private void WriteResource(ODataWriter odataWriter, ResourceWrapper resource)
        {
            odataWriter.WriteStart(resource.Resource);
            var nestedResourceInfos = resource.NestedResourceInfos;
            if (nestedResourceInfos != null && nestedResourceInfos.Count > 0)
            {
                foreach (var nested in nestedResourceInfos)
                {
                    WriteNestedResourceInfo(odataWriter, nested);
                }
            }

            odataWriter.WriteEnd();
        }

        private void WriteNestedResourceInfo(ODataWriter odataWriter, NestedResourceInfoWrapper nestedResourceInfo)
        {
            odataWriter.WriteStart(nestedResourceInfo.NestedResourceInfo);
            var nestedResource = nestedResourceInfo.nestedResource;
            if (nestedResource != null)
            {
                if (nestedResource is ResourceWrapper)
                {
                    WriteResource(odataWriter, nestedResource as ResourceWrapper);
                }
                else
                {
                    WriteResourceSet(odataWriter, nestedResource as ResourceSetWrapper);
                }
            }

            odataWriter.WriteEnd();
        }

        private void WriteResourceSet(ODataWriter odataWriter, ResourceSetWrapper resourceSet)
        {
            odataWriter.WriteStart(resourceSet.ResourceSet);
            var resources = resourceSet.Resources;
            if (resources != null && resources.Count > 0)
            {
                foreach (var resource in resources)
                {
                    WriteResource(odataWriter, resource);
                }
            }

            odataWriter.WriteEnd();
        }

        public class ItemWrapper
        {
            public virtual ODataItem CurrentItem { get; set; }
        }

        public class ResourceSetWrapper : ItemWrapper
        {
            public ODataResourceSet ResourceSet { get; set; }
            public List<ResourceWrapper> Resources { get; set; }

            public override ODataItem CurrentItem
            {
                get
                {
                    return ResourceSet;
                }

                set
                {
                    ResourceSet = (ODataResourceSet)value;
                }
            }
        }

        public class ResourceWrapper : ItemWrapper
        {
            public ODataResource Resource { get; set; }
            public List<NestedResourceInfoWrapper> NestedResourceInfos { get; set; }

            public override ODataItem CurrentItem
            {
                get
                {
                    return Resource;
                }

                set
                {
                    Resource = (ODataResource)value;
                }
            }

        }

        public class NestedResourceInfoWrapper : ItemWrapper
        {
            public ODataNestedResourceInfo NestedResourceInfo { get; set; }
            public ItemWrapper nestedResource { get; set; }

            public override ODataItem CurrentItem
            {
                get
                {
                    return NestedResourceInfo;
                }

                set
                {
                    NestedResourceInfo = (ODataNestedResourceInfo)value;
                }
            }
        }
    }
}
