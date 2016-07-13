using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.Tests.UriParser;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class NavigationPropertyOnComplexTests
    {
        private readonly static Uri ServiceRoot = new Uri("http://host");
        private readonly static IEdmModel Model = GetModel();
        private readonly static IEdmEntitySet EntitySet = Model.EntityContainer.FindEntitySet("People");
        private readonly static IEdmEntityType EntityType = Model.FindType("DefaultNs.Person") as IEdmEntityType;

        private static readonly IEdmModel CollectionModel = GetCollectionModel();

        #region Uri parser
        [Fact]
        public void ParseNavOnComplex()
        {
            IEdmEntityType employeeType = Model.FindType("DefaultNs.Employee") as IEdmEntityType;
            IEdmStructuredType addressType = Model.FindType("DefaultNs.Address") as IEdmStructuredType;
            IEdmStructuredType workAddressType = Model.FindType("DefaultNs.WorkAddress") as IEdmStructuredType;

            IEdmProperty addressProperty = EntityType.FindProperty("Address");
            IEdmProperty addressesProperty = EntityType.FindProperty("Addresses");
            IEdmProperty workAddressProperty = addressType.FindProperty("WorkAddress");

            IEdmNavigationProperty city = addressType.FindProperty("City") as IEdmNavigationProperty;
            IEdmNavigationProperty city2 = workAddressType.FindProperty("City2") as IEdmNavigationProperty;

            Uri uri = new Uri(@"http://host/People('abc')/Address/City");
            var paths = new ODataUriParser(Model, ServiceRoot, uri).ParsePath().ToList();
            Assert.Equal(paths.Count(), 4);
            paths[0].ShouldBeEntitySetSegment(EntitySet);
            paths[1].ShouldBeKeySegment(new KeyValuePair<string, object>("UserName", "abc")) ;
            paths[2].ShouldBePropertySegment(addressProperty);
            paths[3].ShouldBeNavigationPropertySegment(city);

            uri = new Uri(@"http://host/People('abc')/DefaultNs.Employee/Address/DefaultNs.WorkAddress/City");
            paths = new ODataUriParser(Model, ServiceRoot, uri).ParsePath().ToList();
            Assert.Equal(paths.Count(), 6);
            paths[0].ShouldBeEntitySetSegment(EntitySet);
            paths[1].ShouldBeKeySegment(new KeyValuePair<string, object>("UserName", "abc"));
            paths[2].ShouldBeTypeSegment(employeeType);
            paths[3].ShouldBePropertySegment(addressProperty);
            paths[4].ShouldBeTypeSegment(workAddressType, addressType);
            paths[5].ShouldBeNavigationPropertySegment(city);

            uri = new Uri(@"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City2");
            paths = new ODataUriParser(Model, ServiceRoot, uri).ParsePath().ToList();
            paths[2].ShouldBePropertySegment(addressProperty);
            paths[3].ShouldBePropertySegment(workAddressProperty);
            paths[4].ShouldBeTypeSegment(workAddressType, addressType);
            paths[5].ShouldBeNavigationPropertySegment(city2);

            uri = new Uri(@"http://host/People('abc')/Addresses/$count");
            paths = new ODataUriParser(Model, ServiceRoot, uri).ParsePath().ToList();
            paths[2].ShouldBePropertySegment(addressesProperty);
            paths[3].ShouldBeCountSegment();

            uri = new Uri(@"http://host/People('abc')/Addresses/City");
            Action parse = () => new ODataUriParser(Model, ServiceRoot, uri).ParsePath().ToList();
            parse.ShouldThrow<ODataException>().WithMessage(Strings.RequestUriProcessor_CannotQueryCollections("Addresses"));
        }

        [Fact]
        public void ParseNavOfComplexInQueryOption()
        {
            IEdmStructuredType addressType = Model.FindType("DefaultNs.Address") as IEdmStructuredType;
            IEdmStructuredType workAddressType = Model.FindType("DefaultNs.WorkAddress") as IEdmStructuredType;

            IEdmProperty addressProperty = EntityType.FindProperty("Address");
            IEdmProperty addressesProperty = EntityType.FindProperty("Addresses");
            IEdmProperty workAddressProperty = addressType.FindProperty("WorkAddress");

            IEdmNavigationProperty city = addressType.FindProperty("City") as IEdmNavigationProperty;
            IEdmNavigationProperty city2 = workAddressType.FindProperty("City2") as IEdmNavigationProperty;

            // test
            Uri uri = new Uri(@"http://host/People('abc')/Address?$expand=City&$select=City");
            var selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            selectAndExpandClause.SelectedItems.Count().Should().Be(2);
            var items = selectAndExpandClause.SelectedItems.ToList();
            items[0].Should().BeOfType<ExpandedNavigationSelectItem>();
            items[1].Should().BeOfType<PathSelectItem>();

            var expandItem = items[0] as ExpandedNavigationSelectItem;
            expandItem.PathToNavigationProperty.First().ShouldBeNavigationPropertySegment(city);
            expandItem.NavigationSource.Name.Should().Be("City");

            var selectItem = items[1] as PathSelectItem;
            selectItem.SelectedPath.FirstOrDefault().ShouldBeNavigationPropertySegment(city);

            // test
            uri = new Uri(@"http://host/People('abc')/Addresses?$expand=City&$select=City");
            selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            selectAndExpandClause.SelectedItems.Count().Should().Be(2);
            items = selectAndExpandClause.SelectedItems.ToList();
            items[0].Should().BeOfType<ExpandedNavigationSelectItem>();
            items[1].Should().BeOfType<PathSelectItem>();

            expandItem = items[0] as ExpandedNavigationSelectItem;
            expandItem.PathToNavigationProperty.First().ShouldBeNavigationPropertySegment(city);
            expandItem.NavigationSource.Name.Should().Be("City");

            selectItem = items[1] as PathSelectItem;
            selectItem.SelectedPath.FirstOrDefault().ShouldBeNavigationPropertySegment(city);

            // test
            uri = new Uri(@"http://host/People('abc')?$expand=Address/WorkAddress/DefaultNs.WorkAddress/City2");
            selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            selectAndExpandClause.SelectedItems.Count().Should().Be(1);
            items = selectAndExpandClause.SelectedItems.ToList();
            items[0].Should().BeOfType<ExpandedNavigationSelectItem>();
            expandItem = items[0] as ExpandedNavigationSelectItem;
            var paths = expandItem.PathToNavigationProperty.ToList();
            paths[0].ShouldBePropertySegment(addressProperty);
            paths[1].ShouldBePropertySegment(workAddressProperty);
            paths[2].ShouldBeTypeSegment(workAddressType, addressType);
            paths[3].ShouldBeNavigationPropertySegment(city2);

            // test
            uri = new Uri(@"http://host/People('abc')?$expand=Addresses/City");
            selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            selectAndExpandClause.SelectedItems.Count().Should().Be(1);
            items = selectAndExpandClause.SelectedItems.ToList();
            items[0].Should().BeOfType<ExpandedNavigationSelectItem>();
            expandItem = items[0] as ExpandedNavigationSelectItem;
            paths = expandItem.PathToNavigationProperty.ToList();
            paths[0].ShouldBePropertySegment(addressesProperty);
            paths[1].ShouldBeNavigationPropertySegment(city);

            // test
            uri = new Uri(@"http://host/People('abc')?$expand=Address/City&$select=Address/City");
            selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            selectAndExpandClause.SelectedItems.Count().Should().Be(2);
            items = selectAndExpandClause.SelectedItems.ToList();
            items[0].Should().BeOfType<ExpandedNavigationSelectItem>();
            items[1].Should().BeOfType<PathSelectItem>();

            expandItem = items[0] as ExpandedNavigationSelectItem;
            paths = expandItem.PathToNavigationProperty.ToList();
            paths[0].ShouldBePropertySegment(addressProperty);
            paths[1].ShouldBeNavigationPropertySegment(city);

            selectItem = items[1] as PathSelectItem;
            paths = selectItem.SelectedPath.ToList();
            paths[0].ShouldBePropertySegment(addressProperty);
            paths[1].ShouldBeNavigationPropertySegment(city);
        }

        [Fact]
        public void ParseCollectionOfNavOnComplex()
        {
            var entitySet = CollectionModel.EntityContainer.FindEntitySet("Entities");
            var entityType = CollectionModel.FindType("DefaultNs.EntityType") as IEdmStructuredType;
            var complexProperty = entityType.FindProperty("Complex");
            var complexType = CollectionModel.FindType("DefaultNs.ComplexType") as IEdmStructuredType;
            var navProperty = complexType.FindProperty("CollectionOfNav") as IEdmNavigationProperty;

            Uri uri = new Uri(@"http://host/Entities('abc')/Complex/CollectionOfNav('def')");
            var paths = new ODataUriParser(CollectionModel, ServiceRoot, uri).ParsePath().ToList();
            Assert.Equal(paths.Count(), 5);
            paths[0].ShouldBeEntitySetSegment(entitySet);
            paths[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", "abc"));
            paths[2].ShouldBePropertySegment(complexProperty);
            paths[3].ShouldBeNavigationPropertySegment(navProperty);
            paths[4].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", "def"));

            uri = new Uri(@"http://host/Entities('abc')/Complex/CollectionOfNav/$ref");
            paths = new ODataUriParser(CollectionModel, ServiceRoot, uri).ParsePath().ToList();
            Assert.Equal(paths.Count(), 4);
            paths[0].ShouldBeEntitySetSegment(entitySet);
            paths[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", "abc"));
            paths[2].ShouldBePropertySegment(complexProperty);
            paths[3].ShouldBeNavigationPropertyLinkSegment(navProperty);

            // test
            uri = new Uri(@"http://host/Entities('abc')?$expand=Complex/CollectionOfNav&$select=Complex/CollectionOfNav");
            var selectAndExpandClause = new ODataUriParser(CollectionModel, ServiceRoot, uri).ParseSelectAndExpand();
            
            // verify
            selectAndExpandClause.SelectedItems.Count().Should().Be(2);
            var items = selectAndExpandClause.SelectedItems.ToList();

            var expandItem = items[0] as ExpandedNavigationSelectItem;
            var segments = expandItem.PathToNavigationProperty.ToList();
            segments[0].ShouldBePropertySegment(complexProperty);
            segments[1].ShouldBeNavigationPropertySegment(navProperty);

            var selectItem = items[1] as PathSelectItem;
            segments = selectItem.SelectedPath.ToList();
            segments[0].ShouldBePropertySegment(complexProperty);
            segments[1].ShouldBeNavigationPropertySegment(navProperty);

            // test
            uri = new Uri(@"http://host/Entities('abc')/Complex?$expand=CollectionOfNav&$select=CollectionOfNav");
            selectAndExpandClause = new ODataUriParser(CollectionModel, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            selectAndExpandClause.SelectedItems.Count().Should().Be(2);
            items = selectAndExpandClause.SelectedItems.ToList();

            expandItem = items[0] as ExpandedNavigationSelectItem;
            segments = expandItem.PathToNavigationProperty.ToList();
            segments[0].ShouldBeNavigationPropertySegment(navProperty);

            selectItem = items[1] as PathSelectItem;
            segments = selectItem.SelectedPath.ToList();
            segments[0].ShouldBeNavigationPropertySegment(navProperty);

            // test
            uri = new Uri(@"http://host/Entities('abc')?$expand=Complex/CollectionOfNav/$ref");
            selectAndExpandClause = new ODataUriParser(CollectionModel, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            var referenceItem = selectAndExpandClause.SelectedItems.FirstOrDefault() as ExpandedReferenceSelectItem;
            segments = referenceItem.PathToNavigationProperty.ToList();
            segments[0].ShouldBePropertySegment(complexProperty);
            segments[1].ShouldBeNavigationPropertySegment(navProperty);
        }
        #endregion

        #region Reader and Writer

        [Fact]
        public void ReadNavigationPropertyOnComplex()
        {
            const string payload =
                "{\"@odata.context\":\"http://host/$metadata#People/$entity\",\"UserName\":\"abc\",\"Address\":{\"Road\":\"Zixing\",\"City\":{\"ZipCode\":111}}}";

            var entryLists = ReadPayload(payload, Model, EntitySet, EntityType);

            entryLists[0].Id.Should().Be("http://host/City(111)");
            entryLists[1].TypeName.Should().Be("DefaultNs.Address");
            entryLists[2].Id.Should().Be("http://host/People('abc')");
        }

        [Fact]
        public void ReadNavigationPropertyOnCollectionOfComplex()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://host/$metadata#People/$entity\"," +
                    "\"UserName\":\"abc\"," +
                    "\"Addresses\":" +
                    "[{" +
                        "\"Road\":\"Zixing\"," +
                        "\"City\":" +
                            "{\"ZipCode\":111}" +
                    "}," +
                    "{" +
                        "\"@odata.type\":\"#DefaultNs.WorkAddress\"," +
                        "\"Road\":\"Ziyue\"," +
                        "\"City\":" +
                            "{\"ZipCode\":222}" +
                    "}]" +
                "}";

            var entryLists = ReadPayload(payload, Model, EntitySet, EntityType);

            entryLists[0].Id.Should().Be("http://host/City(111)");
            entryLists[1].TypeName.Should().Be("DefaultNs.Address");
            entryLists[1].Properties.FirstOrDefault(s => s.Name == "Road").Value.Should().Be("Zixing");
            entryLists[2].Id.Should().Be("http://host/City(222)");
            entryLists[3].TypeName.Should().Be("DefaultNs.WorkAddress");
            entryLists[3].Properties.FirstOrDefault(s=>s.Name == "Road").Value.Should().Be("Ziyue");
            entryLists[4].Id.Should().Be("http://host/People('abc')");
        }

        [Fact]
        public void ReadNavigationPropertyInNestedComplex()
        {
            const string payload =
                "{" +
                "\"@odata.context\":\"http://host/$metadata#People/$entity\"," +
                "\"UserName\":\"abc\"," +
                "\"Address\":{" +
                    "\"Road\":\"Zixing\"," +
                    "\"WorkAddress\":{" +
                        "\"@odata.type\":\"#DefaultNs.WorkAddress\"," +
                        "\"Road\":\"workplace\"," +
                        "\"City2\":{\"ZipCode\":111}" +
                    "}" +
                "}}";

            var entryLists = ReadPayload(payload, Model, EntitySet, EntityType);

            entryLists[0].Id.Should().Be("http://host/City(111)");
            entryLists[1].TypeName.Should().Be("DefaultNs.WorkAddress");
            entryLists[1].Properties.FirstOrDefault(s => s.Name == "Road").Value.Should().Be("workplace");
            entryLists[2].Properties.FirstOrDefault(s => s.Name == "Road").Value.Should().Be("Zixing");
            entryLists[3].Id.Should().Be("http://host/People('abc')");
        }

        [Fact]
        public void WriteNavigationPropertyOnComplex()
        {
            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "Address" };
            ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "City", IsCollection = false };
            ODataResource nestednav = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 111 } } };

            string actual = WriteJsonLightEntry(Model, EntitySet, (writer) =>
            {
                writer.WriteStart(res);
                writer.WriteStart(nestedComplexInfo);
                writer.WriteStart(nestedComplex);
                writer.WriteStart(nestedResInfo);
                writer.WriteStart(nestednav);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            string expected = "{\"@odata.context\":\"http://host/$metadata#People/$entity\",\"UserName\":\"abc\",\"Address\":{\"Road\":\"Zixing\",\"City\":{\"ZipCode\":111}}}";

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void WriteNavigationPropertyOnDeepComplex()
        {
            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
            ODataNestedResourceInfo addressInfo = new ODataNestedResourceInfo() { Name = "Address" };
            ODataResource address = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo workAddressInfo = new ODataNestedResourceInfo() { Name = "WorkAddress" };
            ODataResource workAddress = new ODataResource() { TypeName="DefaultNs.WorkAddress", Properties = new[] { new ODataProperty { Name = "Road", Value = "Ziyue" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "City2", IsCollection = false };
            ODataResource nestednav = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 222 } } };

            string actual = WriteJsonLightEntry(Model, EntitySet, (writer) =>
            {
                writer.WriteStart(res);
                writer.WriteStart(addressInfo);
                writer.WriteStart(address);
                writer.WriteStart(workAddressInfo);
                writer.WriteStart(workAddress);
                writer.WriteStart(nestedResInfo);
                writer.WriteStart(nestednav);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            string expected = "{" +
                              "\"@odata.context\":\"http://host/$metadata#People/$entity\"," +
                                  "\"UserName\":\"abc\"," +
                                  "\"Address\":{" +
                                      "\"Road\":\"Zixing\"," +
                                      "\"WorkAddress\":{" +
                                          "\"@odata.type\":\"#DefaultNs.WorkAddress\"," +
                                          "\"Road\":\"Ziyue\"," +
                                          "\"City2\":{" +
                                              "\"ZipCode\":222" +
                                            "}" +
                                        "}" +
                                "}" +
                              "}";

            Assert.Equal(actual, expected);
        }

        private const string minimalMetadataPayload = "{" +
                                                "\"@odata.context\":\"http://host/$metadata#Entities/$entity\"," +
                                                    "\"ID\":\"abc\"," +
                                                    "\"Complex\":{" +
                                                        "\"Prop1\":123," +
                                                        "\"CollectionOfNav\":[" +
                                                            "{\"ID\":\"aaa\"}," +
                                                            "{\"ID\":\"bbb\"}" +
                                                        "]" +
                                                    "}" +
                                                "}";
        private const string fullMetadataPayload = "{" +
                                             "\"@odata.context\":\"http://host/$metadata#Entities/$entity\"," +
                                             "\"@odata.id\":\"Entities('abc')\"," +
                                             "\"@odata.editLink\":\"Entities('abc')\"," +
                                             "\"ID\":\"abc\"," +
                                             "\"Complex\":{" +
                                                 "\"Prop1\":123," +
                                                 "\"CollectionOfNav\":[" +
                                                 "{" +
                                                     "\"@odata.id\":\"NavEntities('aaa')\"," +
                                                     "\"@odata.editLink\":\"NavEntities('aaa')\"," +
                                                     "\"ID\":\"aaa\"" +
                                                 "}," +
                                                 "{" +
                                                     "\"@odata.id\":\"NavEntities('bbb')\"," +
                                                     "\"@odata.editLink\":\"NavEntities('bbb')\"," +
                                                     "\"ID\":\"bbb\"" +
                                                 "}" +
                                                 "]" +
                                             "}" +
                                             "}";

        [Theory]
        [InlineData(false, minimalMetadataPayload)]
        [InlineData(true, fullMetadataPayload)]
        public void WriteAndReadCollectionOfNavigationPropertyOnComplex(bool isFullMetadata, string expectedPayload)
        {
            var entitySet = CollectionModel.EntityContainer.FindEntitySet("Entities");
            var entityType = CollectionModel.FindType("DefaultNs.EntityType") as IEdmStructuredType;

            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "abc" } } };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "Complex" };
            ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Prop1", Value = 123 } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "CollectionOfNav", IsCollection = true };
            ODataResourceSet nestedResourceSet = new ODataResourceSet();
            ODataResource nestednav1 = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "aaa" } } };
            ODataResource nestednav2 = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "bbb" } } };

            string output = WriteJsonLightEntry(CollectionModel, entitySet, (writer) =>
            {
                writer.WriteStart(res);
                writer.WriteStart(nestedComplexInfo);
                writer.WriteStart(nestedComplex);
                writer.WriteStart(nestedResInfo);
                writer.WriteStart(nestedResourceSet);
                writer.WriteStart(nestednav1);
                writer.WriteEnd();
                writer.WriteStart(nestednav2);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, isFullMetadata: isFullMetadata);

            Assert.Equal(expectedPayload, output);

            var entryList = ReadPayload(output, CollectionModel, entitySet, entityType);
            entryList[0].Id.Should().Be(new Uri("http://host/NavEntities('aaa')"));
            entryList[0].TypeName.Should().Be("DefaultNs.NavEntityType");

            entryList[1].Id.Should().Be(new Uri("http://host/NavEntities('bbb')"));
            entryList[1].TypeName.Should().Be("DefaultNs.NavEntityType");

            entryList[2].Id.Should().Be(null);
            entryList[2].TypeName.Should().Be("DefaultNs.ComplexType");

            entryList[3].Id.Should().Be(new Uri("http://host/Entities('abc')"));
            entryList[3].TypeName.Should().Be("DefaultNs.EntityType");
        }

        #endregion

        #region Private help method
        private List<ODataResource> ReadPayload(string payload, IEdmModel model, IEdmNavigationSource entitySet, IEdmStructuredType entityType, bool isFullMetadata = false)
        {
            InMemoryMessage message = new InMemoryMessage();
            if (isFullMetadata)
            {
                message.SetHeader("Content-Type", "application/json;odata.metadata=full");
            }
            else
            {
                message.SetHeader("Content-Type", "application/json;odata.metadata=minimal");
            }
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            List<ODataResource> entryList = new List<ODataResource>();

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, null, model))
            {
                var reader = messageReader.CreateODataResourceReader(entitySet, entityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            entryList.Add((ODataResource)reader.Item);
                            break;
                    }
                }
            }

            return entryList;
        }

        private static string WriteJsonLightEntry(IEdmModel model, IEdmEntitySet entitySet, Action<ODataWriter> writeAction, bool isFullMetadata = false)
        {
            var stream = new MemoryStream();
            var message = new InMemoryMessage { Stream = stream };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, AutoComputePayloadMetadata = true };
            var odataUri = new ODataUri { ServiceRoot = ServiceRoot };
            settings.ODataUri = odataUri;
            settings.SetServiceDocumentUri(ServiceRoot);

            settings.SetContentType(ODataFormat.Json);
            if (isFullMetadata)
            {
                settings.SetContentType("application/json;odata.metadata=full", null);
            }
            else
            {
                settings.SetContentType("application/json;odata.metadata=minimal", null);
            }

            var messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model);
            ODataWriter writer = null;
            writer = messageWriter.CreateODataResourceWriter(entitySet);

            if (writeAction != null)
            {
                writeAction(writer);
            }

            writer.Flush();

            var actual = Encoding.UTF8.GetString(stream.ToArray());
            return actual;
        }

        private static IEdmModel GetModel()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("DefaultNs", "Person");
            var entityId = person.AddStructuralProperty("UserName", EdmCoreModel.Instance.GetString(false));
            person.AddKeys(entityId);

            var employee = new EdmEntityType("DefaultNs", "Employee", person);

            var city = new EdmEntityType("DefaultNs", "City");
            var cityId = city.AddStructuralProperty("ZipCode", EdmCoreModel.Instance.GetInt32(false));
            city.AddKeys(cityId);

            var complex = new EdmComplexType("DefaultNs", "Address");
            complex.AddStructuralProperty("Road", EdmCoreModel.Instance.GetString(false));
            var navP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "City",
                    Target = city,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            var derivedComplex = new EdmComplexType("DefaultNs", "WorkAddress", complex);
            var navP2 = derivedComplex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "City2",
                    Target = city,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            complex.AddStructuralProperty("WorkAddress", new EdmComplexTypeReference(complex, false));

            person.AddStructuralProperty("Address", new EdmComplexTypeReference(complex, false));
            person.AddStructuralProperty("Addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complex, false))));

            model.AddElement(person);
            model.AddElement(employee);
            model.AddElement(city);
            model.AddElement(complex);
            model.AddElement(derivedComplex);

            var entityContainer = new EdmEntityContainer("DefaultNs", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet people = new EdmEntitySet(entityContainer, "People", person);
            EdmEntitySet cities = new EdmEntitySet(entityContainer, "City", city);
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("Address/City"));
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("Addresses/City"));
            people.AddNavigationTarget(navP2, cities, new EdmPathExpression("Address/WorkAddress/DefaultNs.WorkAddress/City2"));
            entityContainer.AddElement(people);
            entityContainer.AddElement(cities);

            return model;
        }

        private static IEdmModel GetCollectionModel()
        {
            var model = new EdmModel();

            var entity = new EdmEntityType("DefaultNs", "EntityType");
            var entityId = entity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            entity.AddKeys(entityId);

            var navEntity = new EdmEntityType("DefaultNs", "NavEntityType");
            var navEntityId = navEntity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            navEntity.AddKeys(navEntityId);

            var complex = new EdmComplexType("DefaultNs", "ComplexType");
            complex.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetInt32(false));

            var navP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "CollectionOfNav",
                    Target = navEntity,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            entity.AddStructuralProperty("Complex", new EdmComplexTypeReference(complex, false));

            model.AddElement(entity);
            model.AddElement(navEntity);
            model.AddElement(complex);

            var entityContainer = new EdmEntityContainer("DefaultNs", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet entites = new EdmEntitySet(entityContainer, "Entities", entity);
            EdmEntitySet navEntities = new EdmEntitySet(entityContainer, "NavEntities", navEntity);
            entites.AddNavigationTarget(navP, navEntities, new EdmPathExpression("Complex/CollectionOfNav"));
            entityContainer.AddElement(entites);
            entityContainer.AddElement(navEntities);

            return model;
        }

        #endregion
    }
}
