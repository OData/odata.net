//---------------------------------------------------------------------
// <copyright file="NavigationPropertyOnComplexTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            Assert.Equal(4, paths.Count());
            paths[0].ShouldBeEntitySetSegment(EntitySet);
            paths[1].ShouldBeKeySegment(new KeyValuePair<string, object>("UserName", "abc"));
            paths[2].ShouldBePropertySegment(addressProperty);
            paths[3].ShouldBeNavigationPropertySegment(city);

            uri = new Uri(@"http://host/People('abc')/DefaultNs.Employee/Address/DefaultNs.WorkAddress/City");
            paths = new ODataUriParser(Model, ServiceRoot, uri).ParsePath().ToList();
            Assert.Equal(6, paths.Count());
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
            parse.Throws<ODataException>(Strings.RequestUriProcessor_CannotQueryCollections("Addresses"));
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

            IEdmEntitySet cities = Model.EntityContainer.FindEntitySet("City");

            // test
            Uri uri = new Uri(@"http://host/People('abc')/Address?$expand=City&$select=City");
            var selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            Assert.Equal(2, selectAndExpandClause.SelectedItems.Count());
            var items = selectAndExpandClause.SelectedItems.ToList();
            Assert.IsType<ExpandedNavigationSelectItem>(items[0]);
            Assert.IsType<PathSelectItem>(items[1]);

            var expandItem = items[0] as ExpandedNavigationSelectItem;
            expandItem.PathToNavigationProperty.First().ShouldBeNavigationPropertySegment(city);
            Assert.Same(cities, expandItem.NavigationSource);

            var selectItem = items[1] as PathSelectItem;
            selectItem.SelectedPath.FirstOrDefault().ShouldBeNavigationPropertySegment(city);

            // test
            uri = new Uri(@"http://host/People('abc')/Addresses?$expand=City&$select=City");
            selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            Assert.Equal(2, selectAndExpandClause.SelectedItems.Count());
            items = selectAndExpandClause.SelectedItems.ToList();
            Assert.IsType<ExpandedNavigationSelectItem>(items[0]);
            Assert.IsType<PathSelectItem>(items[1]);

            expandItem = items[0] as ExpandedNavigationSelectItem;
            expandItem.PathToNavigationProperty.First().ShouldBeNavigationPropertySegment(city);
            Assert.Same(cities, expandItem.NavigationSource);

            selectItem = items[1] as PathSelectItem;
            selectItem.SelectedPath.FirstOrDefault().ShouldBeNavigationPropertySegment(city);

            // test
            uri = new Uri(@"http://host/People('abc')?$expand=Address/WorkAddress/DefaultNs.WorkAddress/City2");
            selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            var item = Assert.Single(selectAndExpandClause.SelectedItems);
            expandItem = Assert.IsType<ExpandedNavigationSelectItem>(item);
            var paths = expandItem.PathToNavigationProperty.ToList();
            paths[0].ShouldBePropertySegment(addressProperty);
            paths[1].ShouldBePropertySegment(workAddressProperty);
            paths[2].ShouldBeTypeSegment(workAddressType, workAddressType);
            paths[3].ShouldBeNavigationPropertySegment(city2);

            // test
            uri = new Uri(@"http://host/People('abc')?$expand=Addresses/City");
            selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            item = Assert.Single(selectAndExpandClause.SelectedItems);
            expandItem = Assert.IsType<ExpandedNavigationSelectItem>(item);
            paths = expandItem.PathToNavigationProperty.ToList();
            paths[0].ShouldBePropertySegment(addressesProperty);
            paths[1].ShouldBeNavigationPropertySegment(city);

            // test
            uri = new Uri(@"http://host/People('abc')?$expand=Address/City&$select=Address/City");
            selectAndExpandClause = new ODataUriParser(Model, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            Assert.Equal(2, selectAndExpandClause.SelectedItems.Count());
            items = selectAndExpandClause.SelectedItems.ToList();
            Assert.IsType<ExpandedNavigationSelectItem>(items[0]);
            Assert.IsType<PathSelectItem>(items[1]);

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
            Assert.Equal(5, paths.Count());
            paths[0].ShouldBeEntitySetSegment(entitySet);
            paths[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", "abc"));
            paths[2].ShouldBePropertySegment(complexProperty);
            paths[3].ShouldBeNavigationPropertySegment(navProperty);
            paths[4].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", "def"));

            uri = new Uri(@"http://host/Entities('abc')/Complex/CollectionOfNav/$ref");
            paths = new ODataUriParser(CollectionModel, ServiceRoot, uri).ParsePath().ToList();
            Assert.Equal(4, paths.Count());
            paths[0].ShouldBeEntitySetSegment(entitySet);
            paths[1].ShouldBeKeySegment(new KeyValuePair<string, object>("ID", "abc"));
            paths[2].ShouldBePropertySegment(complexProperty);
            paths[3].ShouldBeNavigationPropertyLinkSegment(navProperty);

            // test
            uri = new Uri(@"http://host/Entities('abc')?$expand=Complex/CollectionOfNav&$select=Complex/CollectionOfNav");
            var selectAndExpandClause = new ODataUriParser(CollectionModel, ServiceRoot, uri).ParseSelectAndExpand();

            // verify
            Assert.Equal(2, selectAndExpandClause.SelectedItems.Count());
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
            Assert.Equal(2, selectAndExpandClause.SelectedItems.Count());
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

            var entryLists = ReadPayload(payload, Model, EntitySet, EntityType).OfType<ODataResource>().ToList();

            Assert.Equal("http://host/City(111)", entryLists[0].Id.OriginalString);
            Assert.Equal("DefaultNs.Address", entryLists[1].TypeName);
            Assert.Equal("http://host/People('abc')", entryLists[2].Id.OriginalString);
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

            var entryLists = ReadPayload(payload, Model, EntitySet, EntityType).OfType<ODataResource>().ToList();

            Assert.Equal("http://host/City(111)", entryLists[0].Id.OriginalString);
            Assert.Equal("DefaultNs.Address", entryLists[1].TypeName);
            Assert.Equal("Zixing", entryLists[1].Properties.FirstOrDefault(s => s.Name == "Road").Value);
            Assert.Equal("http://host/City(222)", entryLists[2].Id.OriginalString);
            Assert.Equal("DefaultNs.WorkAddress", entryLists[3].TypeName);
            Assert.Equal("Ziyue", entryLists[3].Properties.FirstOrDefault(s => s.Name == "Road").Value);
            Assert.Equal("http://host/People('abc')", entryLists[4].Id.OriginalString);
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

            var entryLists = ReadPayload(payload, Model, EntitySet, EntityType).OfType<ODataResource>().ToList();

            Assert.Equal("http://host/City(111)", entryLists[0].Id.OriginalString);
            Assert.Equal("DefaultNs.WorkAddress", entryLists[1].TypeName);
            Assert.Equal("workplace", entryLists[1].Properties.FirstOrDefault(s => s.Name == "Road").Value);
            Assert.Equal("Zixing", entryLists[2].Properties.FirstOrDefault(s => s.Name == "Road").Value);
            Assert.Equal("http://host/People('abc')", entryLists[3].Id.OriginalString);
        }

        [Fact]
        public void WriteNavigationPropertyOnComplex()
        {
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')?$expand=Address/City($select=ZipCode)"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "Address" };
            ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "City", IsCollection = false };
            ODataResource nestednav = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 111 } } };

            string actual = WriteJsonLightEntry(Model, EntitySet, EntityType, odataUri, (writer) =>
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

            string expected = "{\"@odata.context\":\"http://host/$metadata#People(Address/City(ZipCode))/$entity\",\"UserName\":\"abc\",\"Address\":{\"Road\":\"Zixing\",\"City\":{\"ZipCode\":111}}}";

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void WriteNavigationPropertyOnComplexFullMetaData()
        {
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')?$expand=Address/City($select=ZipCode)"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "Address" };
            ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "City", IsCollection = false };
            ODataResource nestednav = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 111 } } };

            string actual = WriteJsonLightEntry(Model, EntitySet, EntityType, odataUri, (writer) =>
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
            },isFullMetadata:true);

            string expected = "{\"@odata.context\":\"http://host/$metadata#People(Address/City(ZipCode))/$entity\"," +
                "\"@odata.id\":\"People('abc')\",\"@odata.editLink\":\"People('abc')\",\"UserName\":\"abc\"," +
                "\"Address\":{\"Road\":\"Zixing\",\"City@odata.associationLink\"" +
                ":\"http://host/People('abc')/Address/City/$ref\",\"City@odata.navigationLink\"" +
                ":\"http://host/People('abc')/Address/City\",\"City\":{\"@odata.id\":\"City(111)\",\"@odata.editLink\":\"City(111)\",\"ZipCode\":111}}}";

            Assert.Equal(actual, expected);
        }

        [Theory]
        [InlineData(ODataVersion.V4)]
        [InlineData(ODataVersion.V401)]
        public void WriteNavigationPropertyOnDeepComplexFullMetaData(ODataVersion version)
        {
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People?$expand=Address/WorkAddress/DefaultNs.WorkAddress/City2&$select=UserName"), null);
            var odataUri = uriParser.ParseUri();

            ODataResourceSet peopleInfo = new ODataResourceSet();
            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
            ODataNestedResourceInfo addressInfo = new ODataNestedResourceInfo() { Name = "Address" };
            ODataResource address = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo workAddressInfo = new ODataNestedResourceInfo() { Name = "WorkAddress" };
            ODataResource workAddress = new ODataResource() { TypeName = "DefaultNs.WorkAddress", Properties = new[] { new ODataProperty { Name = "Road", Value = "Ziyue" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "City2", IsCollection = false };
            ODataResource nestednav = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 222 } } };

            string actual = WriteJsonLightEntry(Model, EntitySet, EntityType, odataUri, (writer) =>
            {
                writer.WriteStart(peopleInfo);
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
                writer.WriteEnd();
            }, true,isFullMetadata:true, version: version);

            string expected = version == ODataVersion.V4 ?
                //OData V4.0
                "{\"@odata.context\":\"http://host/$metadata#People(UserName,Address/WorkAddress/DefaultNs.WorkAddress/City2())\",\"value\":[{\"@odata.id\":\"People('abc')\",\"@odata.editLink\":\"People('abc')\",\"UserName\":\"abc\",\"Address\":{\"Road\":\"Zixing\",\"WorkAddress\":{\"@odata.type\":\"#DefaultNs.WorkAddress\",\"Road\":\"Ziyue\",\"City2@odata.associationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City2/$ref\",\"City2@odata.navigationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City2\",\"City2\":{\"@odata.id\":\"City(222)\",\"@odata.editLink\":\"City(222)\",\"ZipCode\":222}}}}]}" :
                //OData V4.01
                "{\"@context\":\"http://host/$metadata#People(UserName,Address/WorkAddress/DefaultNs.WorkAddress/City2())\",\"value\":[{\"@id\":\"People('abc')\",\"@editLink\":\"People('abc')\",\"UserName\":\"abc\",\"Address\":{\"Road\":\"Zixing\",\"WorkAddress\":{\"@type\":\"#DefaultNs.WorkAddress\",\"Road\":\"Ziyue\",\"City2@associationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City2/$ref\",\"City2@navigationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City2\",\"City2\":{\"@id\":\"City(222)\",\"@editLink\":\"City(222)\",\"ZipCode\":222}}}}]}";

            Assert.Equal(actual, expected);

            var entryList = ReadPayload(expected, Model, EntitySet, EntityType, true, version: version).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/City(222)"), entryList[0].Id);
            Assert.Equal("DefaultNs.City", entryList[0].TypeName);

            Assert.Null(entryList[1].Id);
            Assert.Equal("DefaultNs.WorkAddress", entryList[1].TypeName);

            Assert.Null(entryList[2].Id);
            Assert.Equal("DefaultNs.Address", entryList[2].TypeName);

            Assert.Equal(new Uri("http://host/People('abc')"), entryList[3].Id);
            Assert.Equal("DefaultNs.Person", entryList[3].TypeName);
        }


        [Theory]
        [InlineData(ODataVersion.V4)]
        [InlineData(ODataVersion.V401)]
        public void WriteNavigationPropertyOnDeepComplex(ODataVersion version)
        {
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People?$expand=Address/WorkAddress/DefaultNs.WorkAddress/City2&$select=UserName"), null);
            var odataUri = uriParser.ParseUri();

            ODataResourceSet peopleInfo = new ODataResourceSet();
            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
            ODataNestedResourceInfo addressInfo = new ODataNestedResourceInfo() { Name = "Address" };
            ODataResource address = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo workAddressInfo = new ODataNestedResourceInfo() { Name = "WorkAddress" };
            ODataResource workAddress = new ODataResource() { TypeName = "DefaultNs.WorkAddress", Properties = new[] { new ODataProperty { Name = "Road", Value = "Ziyue" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "City2", IsCollection = false };
            ODataResource nestednav = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 222 } } };

            string actual = WriteJsonLightEntry(Model, EntitySet, EntityType, odataUri, (writer) =>
                {
                    writer.WriteStart(peopleInfo);
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
                    writer.WriteEnd();
                }, true, version: version);

            string expected = version == ODataVersion.V4 ?
                //OData V4.01
                "{\"@odata.context\":\"http://host/$metadata#People(UserName,Address/WorkAddress/DefaultNs.WorkAddress/City2())\"," +
                    "\"value\":[" +
                    "{" +
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
                    "}]}" :
                //OData V4.01
                "{\"@context\":\"http://host/$metadata#People(UserName,Address/WorkAddress/DefaultNs.WorkAddress/City2())\"," +
                    "\"value\":[" +
                    "{" +
                        "\"UserName\":\"abc\"," +
                        "\"Address\":{" +
                            "\"Road\":\"Zixing\"," +
                            "\"WorkAddress\":{" +
                                "\"@type\":\"#DefaultNs.WorkAddress\"," +
                                "\"Road\":\"Ziyue\"," +
                                "\"City2\":{" +
                                    "\"ZipCode\":222" +
                                "}" +
                            "}" +
                        "}" +
                    "}]}";

            Assert.Equal(actual, expected);

            var entryList = ReadPayload(expected, Model, EntitySet, EntityType, true, version: version).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/City(222)"), entryList[0].Id);
            Assert.Equal("DefaultNs.City", entryList[0].TypeName);

            Assert.Null(entryList[1].Id);
            Assert.Equal("DefaultNs.WorkAddress", entryList[1].TypeName);

            Assert.Null(entryList[2].Id);
            Assert.Equal("DefaultNs.Address", entryList[2].TypeName);

            Assert.Equal(new Uri("http://host/People('abc')"), entryList[3].Id);
            Assert.Equal("DefaultNs.Person", entryList[3].TypeName);
        }

        private const string v4MinimalMetadataPayload =
                                               "{\"@odata.context\":\"http://host/$metadata#Entities(ID,Complex/CollectionOfNav())/$entity\"," +
                                                       "\"ID\":\"abc\"," +
                                                        "\"Complex\":{" +
                                                        "\"Prop1\":123," +
                                                        "\"CollectionOfNav@odata.associationLink\":\"http://host/Entities('abc')/WrittenLinks/$ref\"," +
                                                        "\"CollectionOfNav@odata.navigationLink\":\"http://host/Entities('abc')/WrittenLinks\"," +
                                                        "\"CollectionOfNav\":[" +
                                                            "{\"ID\":\"aaa\"}," +
                                                            "{\"ID\":\"bbb\"}" +
                                                        "]" +
                                                    "}" +
                                                "}";
        private const string v4FullMetadataPayload =
                                         "{\"@odata.context\":\"http://host/$metadata#Entities(ID,Complex/CollectionOfNav())/$entity\"," +
                                             "\"@odata.id\":\"Entities('abc')\"," +
                                             "\"@odata.editLink\":\"Entities('abc')\"," +
                                             "\"ID\":\"abc\"," +
                                             "\"Complex\":{" +
                                                 "\"Prop1\":123," +
                                                 "\"CollectionOfNav@odata.associationLink\":\"http://host/Entities('abc')/WrittenLinks/$ref\"," +
                                                 "\"CollectionOfNav@odata.navigationLink\":\"http://host/Entities('abc')/WrittenLinks\"," +
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
        private const string v401MinimalMetadataPayload =
                                               "{\"@context\":\"http://host/$metadata#Entities(ID,Complex/CollectionOfNav())/$entity\"," +
                                                    "\"ID\":\"abc\"," +
                                                    "\"Complex\":{" +
                                                        "\"Prop1\":123," +
                                                        "\"CollectionOfNav@associationLink\":\"http://host/Entities('abc')/WrittenLinks/$ref\"," +
                                                        "\"CollectionOfNav@navigationLink\":\"http://host/Entities('abc')/WrittenLinks\"," +
                                                        "\"CollectionOfNav\":[" +
                                                            "{\"ID\":\"aaa\"}," +
                                                            "{\"ID\":\"bbb\"}" +
                                                        "]" +
                                                    "}" +
                                                "}";
        private const string v401FullMetadataPayload =
                                        "{\"@context\":\"http://host/$metadata#Entities(ID,Complex/CollectionOfNav())/$entity\"," +
                                             "\"@id\":\"Entities('abc')\"," +
                                             "\"@editLink\":\"Entities('abc')\"," +
                                             "\"ID\":\"abc\"," +
                                             "\"Complex\":{" +
                                                 "\"Prop1\":123," +
                                                 "\"CollectionOfNav@associationLink\":\"http://host/Entities('abc')/WrittenLinks/$ref\"," +
                                                 "\"CollectionOfNav@navigationLink\":\"http://host/Entities('abc')/WrittenLinks\"," +
                                                 "\"CollectionOfNav\":[" +
                                                 "{" +
                                                     "\"@id\":\"NavEntities('aaa')\"," +
                                                     "\"@editLink\":\"NavEntities('aaa')\"," +
                                                     "\"ID\":\"aaa\"" +
                                                 "}," +
                                                 "{" +
                                                     "\"@id\":\"NavEntities('bbb')\"," +
                                                     "\"@editLink\":\"NavEntities('bbb')\"," +
                                                     "\"ID\":\"bbb\"" +
                                                 "}" +
                                                 "]" +
                                             "}" +
                                             "}";

        [Theory]
        [InlineData(false, v4MinimalMetadataPayload, ODataVersion.V4)]
        [InlineData(false, v401MinimalMetadataPayload, ODataVersion.V401)]
        [InlineData(true, v4FullMetadataPayload, ODataVersion.V4)]
        [InlineData(true, v401FullMetadataPayload, ODataVersion.V401)]
        public void WriteAndReadCollectionOfNavigationPropertyOnComplex(bool isFullMetadata, string expectedPayload, ODataVersion version)
        {
            var entitySet = CollectionModel.EntityContainer.FindEntitySet("Entities");
            var entityType = CollectionModel.FindType("DefaultNs.EntityType") as IEdmStructuredType;

            var uriParser = new ODataUriParser(CollectionModel, ServiceRoot, new Uri("http://host/Entities('abc')?$expand=Complex/CollectionOfNav&$select=ID"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "abc" } } };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "Complex" };
            ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Prop1", Value = 123 } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "CollectionOfNav", IsCollection = true, Url = new Uri("http://host/Entities('abc')/WrittenLinks"), AssociationLinkUrl = new Uri("http://host/Entities('abc')/WrittenLinks/$ref") };
            ODataResourceSet nestedResourceSet = new ODataResourceSet();
            ODataResource nestednav1 = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "aaa" } } };
            ODataResource nestednav2 = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "bbb" } } };

            string output = WriteJsonLightEntry(CollectionModel, entitySet, entityType, odataUri, (writer) =>
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
                }, false, isFullMetadata: isFullMetadata, version: version);

            Assert.Equal(expectedPayload, output);

            var entryList = ReadPayload(output, CollectionModel, entitySet, entityType, version: version).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/NavEntities('aaa')"), entryList[0].Id);
            Assert.Equal("DefaultNs.NavEntityType", entryList[0].TypeName);

            Assert.Equal(new Uri("http://host/NavEntities('bbb')"), entryList[1].Id);
            Assert.Equal("DefaultNs.NavEntityType", entryList[1].TypeName);

            Assert.Null(entryList[2].Id);
            Assert.Equal("DefaultNs.ComplexType", entryList[2].TypeName);

            Assert.Equal(new Uri("http://host/Entities('abc')"), entryList[3].Id);
            Assert.Equal("DefaultNs.EntityType", entryList[3].TypeName);
        }

        [Fact]
        public void WriteAndReadTopLevelSingleComplexWithCollectionNavigation()
        {
            var complexType = CollectionModel.FindType("DefaultNs.ComplexType") as IEdmComplexType;

            var uriParser = new ODataUriParser(CollectionModel, ServiceRoot, new Uri("http://host/Entities('abc')/Complex?$expand=CollectionOfNav($select=ID)"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Prop1", Value = 123 } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "CollectionOfNav", IsCollection = true };
            ODataResourceSet nestedResourceSet = new ODataResourceSet();
            ODataResource nestednav1 = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "aaa" } } };
            ODataResource nestednav2 = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "bbb" } } };

            string output = WriteJsonLightEntry(CollectionModel, null, complexType, odataUri, (writer) =>
            {
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
            });

            const string expectedPayload =
                "{\"@odata.context\":\"http://host/$metadata#Entities('abc')/Complex(CollectionOfNav(ID))\",\"Prop1\":123,\"CollectionOfNav\":[{\"ID\":\"aaa\"},{\"ID\":\"bbb\"}]}";

            Assert.Equal(expectedPayload, output);

            var entryList = ReadPayload(expectedPayload, CollectionModel, null, complexType).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/NavEntities('aaa')"), entryList[0].Id);
            Assert.Equal("DefaultNs.NavEntityType", entryList[0].TypeName);

            Assert.Equal(new Uri("http://host/NavEntities('bbb')"), entryList[1].Id);
            Assert.Equal("DefaultNs.NavEntityType", entryList[1].TypeName);

            Assert.Null(entryList[2].Id);
            Assert.Equal("DefaultNs.ComplexType", entryList[2].TypeName);
        }

        [Fact]
        public void WriteAndReadTopLevelCollectionComplexWithSingleNavigation()
        {
            var complexType = CollectionModel.FindType("DefaultNs.ComplexType") as IEdmComplexType;

            var uriParser = new ODataUriParser(CollectionModel, ServiceRoot, new Uri("http://host/Entities('abc')/CollectionOfComplex?$expand=SingleOfNav($select=ID)"), null);
            var odataUri = uriParser.ParseUri();

            ODataResourceSet nestedResourceSet = new ODataResourceSet();
            ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Prop1", Value = 123 } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "SingleOfNav", IsCollection = false };
            ODataResource nestednav1 = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "aaa" } } };

            string output = WriteJsonLightEntry(CollectionModel, null, complexType, odataUri, (writer) =>
            {
                writer.WriteStart(nestedResourceSet);
                writer.WriteStart(nestedComplex);
                writer.WriteStart(nestedResInfo);
                writer.WriteStart(nestednav1);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, true);

            const string expectedPayload =
                "{\"@odata.context\":\"http://host/$metadata#Entities('abc')/CollectionOfComplex(SingleOfNav(ID))\",\"value\":[{\"Prop1\":123,\"SingleOfNav\":{\"ID\":\"aaa\"}}]}";

            Assert.Equal(expectedPayload, output);

            var entryList = ReadPayload(expectedPayload, CollectionModel, null, complexType, true).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/NavEntities('aaa')"), entryList[0].Id);
            Assert.Equal("DefaultNs.NavEntityType", entryList[0].TypeName);

            Assert.Null(entryList[1].Id);
            Assert.Equal("DefaultNs.ComplexType", entryList[1].TypeName);
        }

        [Theory]
        [InlineData(ODataVersion.V4)]
        [InlineData(ODataVersion.V401)]
        public void WriteAndReadNavUnderComplexWithTypeCast(ODataVersion version)
        {
            var complexType = Model.FindType("DefaultNs.Address") as IEdmStructuredType;
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')/Address/WorkAddress?$expand=DefaultNs.WorkAddress/City2($expand=Region;$select=ZipCode)"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource workAddress = new ODataResource() { TypeName = "DefaultNs.WorkAddress", Properties = new[] { new ODataProperty { Name = "Road", Value = "Ziyue" } } };
            ODataNestedResourceInfo nestedCityInfo = new ODataNestedResourceInfo() { Name = "City2", IsCollection = false };
            ODataResource city = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 222 } } };
            ODataNestedResourceInfo nestedInfo = new ODataNestedResourceInfo() { Name = "Region", IsCollection = false };
            ODataResource region = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Name", Value = "Land" } } };

            string output = WriteJsonLightEntry(Model, null, complexType, odataUri, (writer) =>
            {
                writer.WriteStart(workAddress);
                writer.WriteStart(nestedCityInfo);
                writer.WriteStart(city);
                writer.WriteStart(nestedInfo);
                writer.WriteStart(region);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, version: version);

            string expectedPayload = version == ODataVersion.V4 ?
                // OData version 4.0
                "{\"@odata.context\":\"http://host/$metadata#People('abc')/Address/WorkAddress(DefaultNs.WorkAddress/City2(ZipCode,Region()))\"," +
                "\"@odata.type\":\"#DefaultNs.WorkAddress\"," +
                "\"Road\":\"Ziyue\"," +
                "\"City2\":{" +
                    "\"ZipCode\":222," +
                    "\"Region\":{" +
                        "\"Name\":\"Land\"" +
                    "}" +
                    "}" +
                "}" :
               // OData version 4.01
               "{\"@context\":\"http://host/$metadata#People('abc')/Address/WorkAddress(DefaultNs.WorkAddress/City2(ZipCode,Region()))\"," +
                "\"@type\":\"#DefaultNs.WorkAddress\"," +
                "\"Road\":\"Ziyue\"," +
                "\"City2\":{" +
                    "\"ZipCode\":222," +
                    "\"Region\":{" +
                        "\"Name\":\"Land\"" +
                    "}" +
                    "}" +
                "}";

            Assert.Equal(expectedPayload, output);

            var entryList = ReadPayload(expectedPayload, Model, null, complexType, version: version).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/Regions('Land')"), entryList[0].Id);
            Assert.Equal("DefaultNs.Region", entryList[0].TypeName);

            Assert.Equal(new Uri("http://host/City(222)"), entryList[1].Id);
            Assert.Equal("DefaultNs.City", entryList[1].TypeName);

            Assert.Null(entryList[2].Id);
            Assert.Equal("DefaultNs.WorkAddress", entryList[2].TypeName);
        }

        [Theory]
        [InlineData(ODataVersion.V4)]
        [InlineData(ODataVersion.V401)]
        public void WriteAndReadNavUnderComplexWithTypeCastWithFullMetadata(ODataVersion version)
        {
            var complexType = Model.FindType("DefaultNs.Address") as IEdmStructuredType;
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')/Address/WorkAddress?$expand=DefaultNs.WorkAddress/City2($expand=Region)"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource workAddress = new ODataResource() { TypeName = "DefaultNs.WorkAddress", Properties = new[] { new ODataProperty { Name = "Road", Value = "Ziyue" } } };
            ODataNestedResourceInfo nestedCityInfo = new ODataNestedResourceInfo() { Name = "City2", IsCollection = false };
            ODataResource city = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 222 } } };
            ODataNestedResourceInfo nestedInfo = new ODataNestedResourceInfo() { Name = "Region", IsCollection = false };
            ODataResource region = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Name", Value = "China" } } };

            string output = WriteJsonLightEntry(Model, null, complexType, odataUri, (writer) =>
            {
                writer.WriteStart(workAddress);
                writer.WriteStart(nestedCityInfo);
                writer.WriteStart(city);
                writer.WriteStart(nestedInfo);
                writer.WriteStart(region);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, false, true, version);

            string expectedPayload = version == ODataVersion.V4 ?
                // OData V4 Version
                "{\"@odata.context\":\"http://host/$metadata#People('abc')/Address/WorkAddress(DefaultNs.WorkAddress/City2(Region()))\"," +
                "\"@odata.type\":\"#DefaultNs.WorkAddress\"," +
                "\"Road\":\"Ziyue\"," +
                "\"City2\":{" +
                    "\"@odata.id\":\"City(222)\"," +
                    "\"@odata.editLink\":\"City(222)\"," +
                    "\"ZipCode\":222," +
                    "\"Region@odata.associationLink\":\"http://host/City(222)/Region/$ref\"," +
                    "\"Region@odata.navigationLink\":\"http://host/City(222)/Region\"," +
                    "\"Region\":{" +
                        "\"@odata.id\":\"Regions('China')\"," +
                        "\"@odata.editLink\":\"Regions('China')\"," +
                        "\"Name\":\"China\"}}}" :
                // OData Version 4.01
                "{\"@context\":\"http://host/$metadata#People('abc')/Address/WorkAddress(DefaultNs.WorkAddress/City2(Region()))\"," +
                    "\"@type\":\"#DefaultNs.WorkAddress\"," +
                    "\"Road\":\"Ziyue\"," +
                    "\"City2\":{" +
                        "\"@id\":\"City(222)\"," +
                        "\"@editLink\":\"City(222)\"," +
                        "\"ZipCode\":222," +
                        "\"Region@associationLink\":\"http://host/City(222)/Region/$ref\"," +
                        "\"Region@navigationLink\":\"http://host/City(222)/Region\"," +
                        "\"Region\":{" +
                            "\"@id\":\"Regions('China')\"," +
                            "\"@editLink\":\"Regions('China')\"," +
                            "\"Name\":\"China\"}}}";

            Assert.Equal(expectedPayload, output);

            var entryList = ReadPayload(expectedPayload, Model, null, complexType, version: version).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/Regions('China')"), entryList[0].Id);
            Assert.Equal("DefaultNs.Region", entryList[0].TypeName);

            Assert.Equal(new Uri("http://host/City(222)"), entryList[1].Id);
            Assert.Equal("DefaultNs.City", entryList[1].TypeName);

            Assert.Null(entryList[2].Id);
            Assert.Equal("DefaultNs.WorkAddress", entryList[2].TypeName);
        }

        [Theory]
        [InlineData(ODataVersion.V4)]
        [InlineData(ODataVersion.V401)]
        public void WriteAndReadNavUnderComplexWithSplitBindingPath(ODataVersion version)
        {
            var complexType = Model.FindType("DefaultNs.Address") as IEdmStructuredType;
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')/Address?$expand=WorkAddress/DefaultNs.WorkAddress/City2($expand=Region;$select=ZipCode)"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource address = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo workAddressInfo = new ODataNestedResourceInfo() { Name = "WorkAddress" };
            ODataResource workAddress = new ODataResource() { TypeName = "DefaultNs.WorkAddress", Properties = new[] { new ODataProperty { Name = "Road", Value = "Ziyue" } } };
            ODataNestedResourceInfo nestedCityInfo = new ODataNestedResourceInfo() { Name = "City2", IsCollection = false };
            ODataResource city = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 222 } } };
            ODataNestedResourceInfo nestedInfo = new ODataNestedResourceInfo() { Name = "Region", IsCollection = false };
            ODataResource region = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Name", Value = "Land" } } };

            string output = WriteJsonLightEntry(Model, null, complexType, odataUri, (writer) =>
                {
                    writer.WriteStart(address);
                    writer.WriteStart(workAddressInfo);
                    writer.WriteStart(workAddress);
                    writer.WriteStart(nestedCityInfo);
                    writer.WriteStart(city);
                    writer.WriteStart(nestedInfo);
                    writer.WriteStart(region);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                }, version: version);

            string expectedPayload = version == ODataVersion.V4 ?
                // OData version 4.0
                "{\"@odata.context\":\"http://host/$metadata#People('abc')/Address(WorkAddress/DefaultNs.WorkAddress/City2(ZipCode,Region()))\"," +
                "\"Road\":\"Zixing\"," +
                "\"WorkAddress\":{" +
                    "\"@odata.type\":\"#DefaultNs.WorkAddress\"," +
                    "\"Road\":\"Ziyue\"," +
                    "\"City2\":{" +
                        "\"ZipCode\":222," +
                        "\"Region\":{" +
                            "\"Name\":\"Land\"" +
                            "}" +
                        "}" +
                    "}" +
                "}" :
                // OData version 4.01
                "{\"@context\":\"http://host/$metadata#People('abc')/Address(WorkAddress/DefaultNs.WorkAddress/City2(ZipCode,Region()))\"," +
                "\"Road\":\"Zixing\"," +
                "\"WorkAddress\":{" +
                    "\"@type\":\"#DefaultNs.WorkAddress\"," +
                    "\"Road\":\"Ziyue\"," +
                    "\"City2\":{" +
                        "\"ZipCode\":222," +
                        "\"Region\":{" +
                            "\"Name\":\"Land\"" +
                            "}" +
                        "}" +
                    "}" +
                "}";

            Assert.Equal(expectedPayload, output);

            var entryList = ReadPayload(expectedPayload, Model, null, complexType, version: version).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/Regions('Land')"), entryList[0].Id);
            Assert.Equal("DefaultNs.Region", entryList[0].TypeName);

            Assert.Equal(new Uri("http://host/City(222)"), entryList[1].Id);
            Assert.Equal("DefaultNs.City", entryList[1].TypeName);

            Assert.Null(entryList[2].Id);
            Assert.Equal("DefaultNs.WorkAddress", entryList[2].TypeName);

            Assert.Null(entryList[3].Id);
            Assert.Equal("DefaultNs.Address", entryList[3].TypeName);
        }
        #endregion

        #region auto compute navigation link
        [Fact]
        public void AutoComputeNavigationLinkForNavUnderSingleComplexInWriter()
        {
            var entitySet = CollectionModel.EntityContainer.FindEntitySet("Entities");
            var entityType = CollectionModel.FindType("DefaultNs.EntityType") as IEdmStructuredType;

            var uriParser = new ODataUriParser(CollectionModel, ServiceRoot, new Uri("http://host/Entities('abc')"));
            var odataUri = uriParser.ParseUri();

            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "abc" } } };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "Complex" };
            ODataResource nestedComplex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Prop1", Value = 123 } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "CollectionOfNav", IsCollection = true };

            string output = WriteJsonLightEntry(CollectionModel, entitySet, entityType, odataUri, (writer) =>
            {
                writer.WriteStart(res);
                writer.WriteStart(nestedComplexInfo);
                writer.WriteStart(nestedComplex);
                writer.WriteStart(nestedResInfo);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, false, isFullMetadata: true);

            string expected = "{\"@odata.context\":\"http://host/$metadata#Entities/$entity\"," +
                              "\"@odata.id\":\"Entities('abc')\"," +
                              "\"@odata.editLink\":\"Entities('abc')\"," +
                              "\"ID\":\"abc\"," +
                              "\"Complex\":{" +
                                  "\"Prop1\":123," +
                                  "\"CollectionOfNav@odata.associationLink\":\"http://host/Entities('abc')/Complex/CollectionOfNav/$ref\"," +
                                  "\"CollectionOfNav@odata.navigationLink\":\"http://host/Entities('abc')/Complex/CollectionOfNav\"," +
                                  "\"SingleOfNav@odata.associationLink\":\"http://host/Entities('abc')/Complex/SingleOfNav/$ref\"," +
                                  "\"SingleOfNav@odata.navigationLink\":\"http://host/Entities('abc')/Complex/SingleOfNav\"" +
                                "}" +
                              "}";

            Assert.Equal(expected, output);

            var itemsList = ReadPayload(expected, CollectionModel, entitySet, entityType).ToList();
            var nestedInfo = itemsList[0] as ODataNestedResourceInfo;
            Assert.Equal(new Uri("http://host/Entities('abc')/Complex/CollectionOfNav/$ref"), nestedInfo.AssociationLinkUrl);
            Assert.Equal(new Uri("http://host/Entities('abc')/Complex/CollectionOfNav"), nestedInfo.Url);

            // itemsList[1] is nested info of SingleOfNav, skip the validation.

            var resource = itemsList[2] as ODataResource;
            Assert.Equal("DefaultNs.ComplexType", resource.TypeName);

            nestedInfo = itemsList[3] as ODataNestedResourceInfo;
            Assert.Equal("Complex", nestedInfo.Name);
            Assert.Null(nestedInfo.AssociationLinkUrl);
            Assert.Null(nestedInfo.Url);

            resource = itemsList[4] as ODataResource;
            Assert.Equal(new Uri("http://host/Entities('abc')"), resource.Id);
        }

        [Fact]
        public void AutoComputeNavigationLinkForDeepNavigationUnderSingleComplexInWriter()
        {
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
            ODataNestedResourceInfo addressInfo = new ODataNestedResourceInfo() { Name = "Address" };
            ODataResource address = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo workAddressInfo = new ODataNestedResourceInfo() { Name = "WorkAddress" };
            ODataResource workAddress = new ODataResource() { TypeName = "DefaultNs.WorkAddress", Properties = new[] { new ODataProperty { Name = "Road", Value = "Ziyue" } } };
            ODataNestedResourceInfo nestedCityInfo = new ODataNestedResourceInfo() { Name = "City2", IsCollection = false };
            ODataResource city = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 222 } } };
            ODataNestedResourceInfo nestedInfo = new ODataNestedResourceInfo() { Name = "Region", IsCollection = false };
            ODataResource region = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Name", Value = "Land" } } };

            string output = WriteJsonLightEntry(Model, EntitySet, EntityType, odataUri, (writer) =>
            {
                writer.WriteStart(res);
                writer.WriteStart(addressInfo);
                writer.WriteStart(address);
                writer.WriteStart(workAddressInfo);
                writer.WriteStart(workAddress);
                writer.WriteStart(nestedCityInfo);
                writer.WriteStart(city);
                writer.WriteStart(nestedInfo);
                writer.WriteStart(region);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, false, true);

            string expected = "{\"@odata.context\":\"http://host/$metadata#People/$entity\"," +
                              "\"@odata.id\":\"People('abc')\"," +
                              "\"@odata.editLink\":\"People('abc')\"," +
                              "\"UserName\":\"abc\"," +
                              "\"Address\":{" +
                                  "\"Road\":\"Zixing\"," +
                                  "\"WorkAddress\":{" +
                                      "\"@odata.type\":\"#DefaultNs.WorkAddress\"," +
                                      "\"Road\":\"Ziyue\"," +
                                      "\"City2@odata.associationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City2/$ref\"," +
                                      "\"City2@odata.navigationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City2\"," +
                                      "\"City2\":{" +
                                          "\"@odata.id\":\"City(222)\"," +
                                          "\"@odata.editLink\":\"City(222)\"," +
                                          "\"ZipCode\":222," +
                                          "\"Region@odata.associationLink\":\"http://host/City(222)/Region/$ref\"," +
                                          "\"Region@odata.navigationLink\":\"http://host/City(222)/Region\"," +
                                          "\"Region\":{" +
                                              "\"@odata.id\":\"Regions('Land')\"," +
                                              "\"@odata.editLink\":\"Regions('Land')\"," +
                                              "\"Name\":\"Land\"" +
                                              "}}," +
                                      "\"City@odata.associationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City/$ref\"," +
                                      "\"City@odata.navigationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City\"," +
                                      "\"City3@odata.associationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City3/$ref\"," +
                                      "\"City3@odata.navigationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City3\"," +
                                      "\"City4@odata.associationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City4/$ref\"," +
                                      "\"City4@odata.navigationLink\":\"http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City4\"" +
                                      "}," +
                                      "\"City@odata.associationLink\":\"http://host/People('abc')/Address/City/$ref\"," +
                                      "\"City@odata.navigationLink\":\"http://host/People('abc')/Address/City\"," +
                                      "\"City3@odata.associationLink\":\"http://host/People('abc')/Address/City3/$ref\"," +
                                      "\"City3@odata.navigationLink\":\"http://host/People('abc')/Address/City3\"" +
                              "}" +
                              "}";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void NotAbleToComputeNavigationLinkUnderCollectionOfComplexInFullMetadata()
        {
            var uriParser = new ODataUriParser(Model, ServiceRoot, new Uri("http://host/People('abc')"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "UserName", Value = "abc" } } };
            ODataNestedResourceInfo addressInfo = new ODataNestedResourceInfo() { Name = "Addresses", IsCollection = true };
            ODataResourceSet resSet = new ODataResourceSet();
            ODataResource address = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Road", Value = "Zixing" } } };
            ODataNestedResourceInfo nestedCityInfo = new ODataNestedResourceInfo() { Name = "City", IsCollection = false };
            ODataResource city = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ZipCode", Value = 222 } } };

            var output = WriteJsonLightEntry(Model, EntitySet, EntityType, odataUri, (writer) =>
            {
                writer.WriteStart(res);
                writer.WriteStart(addressInfo);
                writer.WriteStart(resSet);
                writer.WriteStart(address);
                writer.WriteStart(nestedCityInfo);
                writer.WriteStart(city);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, false, isFullMetadata: true);

            string expected = "{" +
                              "\"@odata.context\":\"http://host/$metadata#People/$entity\"," +
                              "\"@odata.id\":\"People('abc')\"," +
                              "\"@odata.editLink\":\"People('abc')\"," +
                              "\"UserName\":\"abc\"," +
                              "\"Addresses\":[" +
                                  "{\"Road\":\"Zixing\"," +
                                      "\"City\":{" +
                                          "\"@odata.id\":\"City(222)\"," +
                                          "\"@odata.editLink\":\"City(222)\"," +
                                          "\"ZipCode\":222," +
                                          "\"Region@odata.associationLink\":\"http://host/City(222)/Region/$ref\"," +
                                          "\"Region@odata.navigationLink\":\"http://host/City(222)/Region\"}" +
                                      "}" +
                              "]" +
                              "}";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void AutoComputeNavigationLinkForNavUnderSingleComplexInReader()
        {
            var entitySet = CollectionModel.EntityContainer.FindEntitySet("Entities");
            var entityType = CollectionModel.FindType("DefaultNs.EntityType") as IEdmStructuredType;

            string payload = "{\"@odata.context\":\"http://host/$metadata#Entities/$entity\"," +
                              "\"ID\":\"abc\"," +
                              "\"Complex\":{" +
                                  "\"Prop1\":123" +
                                "}" +
                              "}";

            var itemsList = ReadPayload(payload, CollectionModel, entitySet, entityType).OfType<ODataNestedResourceInfo>().ToList();
            var nestedInfo = itemsList[0];
            Assert.Equal(new Uri("http://host/Entities('abc')/Complex/CollectionOfNav/$ref"), nestedInfo.AssociationLinkUrl);
            Assert.Equal(new Uri("http://host/Entities('abc')/Complex/CollectionOfNav"), nestedInfo.Url);

            nestedInfo = itemsList[1];
            Assert.Equal(new Uri("http://host/Entities('abc')/Complex/SingleOfNav/$ref"), nestedInfo.AssociationLinkUrl);
            Assert.Equal(new Uri("http://host/Entities('abc')/Complex/SingleOfNav"), nestedInfo.Url);
        }

        [Fact]
        public void AutoComputeIdForNavUnderComplex()
        {
            string expected = "{\"@odata.context\":\"http://host/$metadata#People/$entity\"," +
                              "\"UserName\":\"abc\"," +
                              "\"Address\":{" +
                                  "\"Road\":\"Zixing\"," +
                                  "\"WorkAddress\":{" +
                                      "\"@odata.type\":\"#DefaultNs.WorkAddress\"," +
                                      "\"Road\":\"Ziyue\"," +
                                      "\"City2\":{" +
                                          "\"ZipCode\":222," +
                                          "\"Region\":{" +
                                              "\"Name\":\"Land\"" +
                                              "}" +
                                      "}" +
                                  "}" +
                              "}" +
                              "}";

            var itemsList = ReadPayload(expected, Model, EntitySet, EntityType).OfType<ODataNestedResourceInfo>().ToList();

            // Region under City2
            var nestInfo = itemsList[0];
            Assert.Equal(new Uri("http://host/City(222)/Region"), nestInfo.Url);

            // City2 under WorkAddress
            nestInfo = itemsList[1];
            Assert.Equal(new Uri("http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City2"), nestInfo.Url);

            // City under WorkAddress
            nestInfo = itemsList[2];
            Assert.Equal(new Uri("http://host/People('abc')/Address/WorkAddress/DefaultNs.WorkAddress/City"), nestInfo.Url);

            // itemsList[3] and itemsList[4] are the nested info for complex property WorkAddress and itemsList[5] is the complex property . so skip the validation.

            // City under Address
            nestInfo = itemsList[6];
            Assert.Equal(new Uri("http://host/People('abc')/Address/City"), nestInfo.Url);
        }
        #endregion

        #region Containment under complex
        [Fact]
        public void WriteContainmentUnderComplex()
        {
            var model = ContainmentComplexModel();
            var entityType = model.FindType("NS.EntityType") as IEdmEntityType;
            var entitySet = model.EntityContainer.FindEntitySet("Entities1");
            var uriParser = new ODataUriParser(model, ServiceRoot, new Uri("http://host/Entities1('abc')"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource topEntity = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "abc" } } };
            ODataNestedResourceInfo complexInfo = new ODataNestedResourceInfo() { Name = "Complex" };
            ODataResource complex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Prop1", Value = 123 } } };
            ODataNestedResourceInfo containedInfo = new ODataNestedResourceInfo() { Name = "ContainedUnderComplex", IsCollection = true };
            ODataResourceSet set = new ODataResourceSet();
            ODataResource contained = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "def" } } };
            ODataNestedResourceInfo navInfo = new ODataNestedResourceInfo() { Name = "NavUnderContained", IsCollection = true };
            ODataResource nav = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "efg" } } };

            string output = WriteJsonLightEntry(model, entitySet, entityType, odataUri, (writer) =>
            {
                writer.WriteStart(topEntity);
                writer.WriteStart(complexInfo);
                writer.WriteStart(complex);
                writer.WriteStart(containedInfo);
                writer.WriteStart(set);
                writer.WriteStart(contained);
                writer.WriteStart(navInfo);
                writer.WriteStart(set);
                writer.WriteStart(nav);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, false, isFullMetadata:true);

            string expected = "{" +
                "\"@odata.context\":\"http://host/$metadata#Entities1/$entity\"," +
                "\"@odata.id\":\"Entities1('abc')\"," +
                "\"@odata.editLink\":\"Entities1('abc')\"," +
                "\"ID\":\"abc\"," +
                "\"Complex\":{" +
                    "\"Prop1\":123," +
                    "\"ContainedUnderComplex@odata.associationLink\":\"http://host/Entities1('abc')/Complex/ContainedUnderComplex/$ref\"," +
                    "\"ContainedUnderComplex@odata.navigationLink\":\"http://host/Entities1('abc')/Complex/ContainedUnderComplex\"," +
                    "\"ContainedUnderComplex\":[{" +
                        "\"@odata.id\":\"Entities1('abc')/Complex/ContainedUnderComplex('def')\"," +
                        "\"@odata.editLink\":\"Entities1('abc')/Complex/ContainedUnderComplex('def')\"," +
                        "\"ID\":\"def\"," +
                        "\"NavUnderContained@odata.associationLink\":\"http://host/Entities1('abc')/Complex/ContainedUnderComplex('def')/NavUnderContained/$ref\"," +
                        "\"NavUnderContained@odata.navigationLink\":\"http://host/Entities1('abc')/Complex/ContainedUnderComplex('def')/NavUnderContained\"," +
                        "\"NavUnderContained\":[{\"@odata.id\":\"Entities2('efg')\",\"@odata.editLink\":\"Entities2('efg')\",\"ID\":\"efg\"}]}]}}";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void WriteContainmentUnderComplexWithTypeCast()
        {
            var model = ContainmentComplexModel();
            var entityType = model.FindType("NS.EntityType") as IEdmEntityType;
            var entitySet = model.EntityContainer.FindEntitySet("Entities1");
            var uriParser = new ODataUriParser(model, ServiceRoot, new Uri("http://host/Entities1('abc')"), null);
            var odataUri = uriParser.ParseUri();

            ODataResource topEntity = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "abc" } } };
            ODataNestedResourceInfo complexInfo = new ODataNestedResourceInfo() { Name = "Complex" };
            ODataResource complex = new ODataResource() { TypeName = "NS.DerivedComplexType", Properties = new[] { new ODataProperty { Name = "Prop1", Value = 123 } } };
            ODataNestedResourceInfo containedInfo = new ODataNestedResourceInfo() { Name = "ContainedUnderDerivedComplex", IsCollection = false };
            ODataResource contained = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "def" } } };

            string output = WriteJsonLightEntry(model, entitySet, entityType, odataUri, (writer) =>
            {
                writer.WriteStart(topEntity);
                writer.WriteStart(complexInfo);
                writer.WriteStart(complex);
                writer.WriteStart(containedInfo);
                writer.WriteStart(contained);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, false, isFullMetadata: true);

            Assert.Contains("\"ContainedUnderDerivedComplex@odata.associationLink\":\"http://host/Entities1('abc')/Complex/NS.DerivedComplexType/ContainedUnderDerivedComplex/$ref\"," +
                "\"ContainedUnderDerivedComplex@odata.navigationLink\":\"http://host/Entities1('abc')/Complex/NS.DerivedComplexType/ContainedUnderDerivedComplex\"", output);
        }

        [Fact]
        public void WriteTopLevelComplexWithContainment()
        {
            var model = ContainmentComplexModel();
            var complexType = model.FindType("NS.ComplexType") as IEdmStructuredType;
            var uriParser = new ODataUriParser(model, ServiceRoot, new Uri("http://host/Entities1('abc')/Complex"), null);
            var odataUri = uriParser.ParseUri();
            var entitySet = model.EntityContainer.FindEntitySet("Entities1");

            ODataResource complex = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Prop1", Value = 123 } } };
            ODataNestedResourceInfo containedInfo = new ODataNestedResourceInfo() { Name = "ContainedUnderComplex", IsCollection = true };
            ODataResourceSet set = new ODataResourceSet();
            ODataResource contained = new ODataResource() { Properties = new[] { new ODataProperty { Name = "ID", Value = "def" } } };

            string output = WriteJsonLightEntry(model, null, complexType, odataUri, (writer) =>
            {
                writer.WriteStart(complex);
                writer.WriteStart(containedInfo);
                writer.WriteStart(set);
                writer.WriteStart(contained);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            }, false, isFullMetadata: true);

            // Verify the id of contained entity
            Assert.Contains("\"@odata.id\":\"Entities1('abc')/Complex/ContainedUnderComplex('def')\"", output);
        }

        [Fact]
        public void ReadTopLevelComplexWithContainment()
        {
            var model = ContainmentComplexModel();
            var complexType = model.FindType("NS.ComplexType") as IEdmComplexType;
            string payload = "{" +
               "\"@odata.context\":\"http://host/$metadata#Entities1('abc')/Complex\"," +
                   "\"Prop1\":123," +
                   "\"ContainedUnderComplex\":[{" +
                       "\"ID\":\"def\"," +
                       "\"NavUnderContained\":[{\"ID\":\"efg\"}]}]}";

            var itemsList = ReadPayload(payload, model, null, complexType).OfType<ODataResource>().ToList();
            Assert.Equal(new Uri("http://host/Entities2('efg')"), itemsList[0].Id);
            Assert.Equal(new Uri("http://host/Entities1('abc')/Complex/ContainedUnderComplex('def')"), itemsList[1].Id);
        }

        #endregion

        #region Private help method
        private List<ODataItem> ReadPayload(string payload, IEdmModel model, IEdmEntitySet entitySet, IEdmStructuredType entityType, bool isResouceSet = false, bool isFullMetadata = false, ODataVersion version = ODataVersion.V4)
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

            List<ODataItem> itemsList = new List<ODataItem>();

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings
            {
                Version = version
            };

            using (var messageReader = new ODataMessageReader((IODataResponseMessage)message, settings, model))
            {
                ODataReader reader;

                if (isResouceSet)
                {
                    reader = messageReader.CreateODataResourceSetReader(entitySet, entityType);
                }
                else
                {
                    reader = messageReader.CreateODataResourceReader(entitySet, entityType);
                }
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            itemsList.Add(reader.Item);
                            break;
                        case ODataReaderState.NestedResourceInfoEnd:
                            itemsList.Add(reader.Item);
                            break;
                    }
                }
            }

            return itemsList;
        }

        private static string WriteJsonLightEntry(IEdmModel model, IEdmEntitySet entitySet, IEdmStructuredType resourceType, ODataUri odataUri, Action<ODataWriter> writeAction, bool isResourceSet = false, bool isFullMetadata = false, ODataVersion version = ODataVersion.V4)
        {
            var stream = new MemoryStream();
            var message = new InMemoryMessage { Stream = stream };

            var settings = new ODataMessageWriterSettings { Version = version };
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
            if (isResourceSet)
            {
                writer = messageWriter.CreateODataResourceSetWriter(entitySet, resourceType);
            }
            else
            {
                writer = messageWriter.CreateODataResourceWriter(entitySet, resourceType);
            }

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

            var city3 = new EdmEntityType("DefaultNs", "City3");
            var cityId3 = city3.AddStructuralProperty("ZipCode3", EdmCoreModel.Instance.GetInt32(false));
            city3.AddKeys(cityId3);

            var region = new EdmEntityType("DefaultNs", "Region");
            var regionId = region.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            region.AddKeys(regionId);

            var cityRegion = city.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Region",
                Target = region,
                TargetMultiplicity = EdmMultiplicity.One,
            });

            var complex = new EdmComplexType("DefaultNs", "Address");
            complex.AddStructuralProperty("Road", EdmCoreModel.Instance.GetString(false));
            var navP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "City",
                    Target = city,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            var navP3 = complex.AddUnidirectionalNavigation(
               new EdmNavigationPropertyInfo()
               {
                   Name = "City3",
                   Target = city3,
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

            var navP4 = derivedComplex.AddUnidirectionalNavigation(
            new EdmNavigationPropertyInfo()
            {
                Name = "City4",
                Target = city,
                TargetMultiplicity = EdmMultiplicity.One,
            });

            complex.AddStructuralProperty("WorkAddress", new EdmComplexTypeReference(complex, false));

            person.AddStructuralProperty("Address", new EdmComplexTypeReference(complex, false));
            person.AddStructuralProperty("Addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complex, false))));
            person.AddStructuralProperty("NewAddresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complex, true))));

            model.AddElement(person);
            model.AddElement(employee);
            model.AddElement(city);
            model.AddElement(city3);
            model.AddElement(region);
            model.AddElement(complex);
            model.AddElement(derivedComplex);

            var entityContainer = new EdmEntityContainer("DefaultNs", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet people = new EdmEntitySet(entityContainer, "People", person);
            EdmEntitySet cities = new EdmEntitySet(entityContainer, "City", city);
            EdmEntitySet cities3 = new EdmEntitySet(entityContainer, "City", city3);
            EdmEntitySet regions = new EdmEntitySet(entityContainer, "Regions", region);
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("Address/City"));
            people.AddNavigationTarget(navP3, cities3, new EdmPathExpression("Address/City3"));
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("Addresses/City"));
            people.AddNavigationTarget(navP2, cities, new EdmPathExpression("Address/WorkAddress/DefaultNs.WorkAddress/City2"));
            people.AddNavigationTarget(navP4, cities, new EdmPathExpression("Address/WorkAddress/DefaultNs.WorkAddress/City4"));
            cities.AddNavigationTarget(cityRegion, regions);
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

            var collectionNav = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "CollectionOfNav",
                    Target = navEntity,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            var singleNav = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "SingleOfNav",
                    Target = navEntity,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                });

            entity.AddStructuralProperty("Complex", new EdmComplexTypeReference(complex, false));
            entity.AddStructuralProperty("CollectionOfComplex", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complex, false))));

            model.AddElement(entity);
            model.AddElement(navEntity);
            model.AddElement(complex);

            var entityContainer = new EdmEntityContainer("DefaultNs", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet entites = new EdmEntitySet(entityContainer, "Entities", entity);
            EdmEntitySet navEntities = new EdmEntitySet(entityContainer, "NavEntities", navEntity);
            entites.AddNavigationTarget(collectionNav, navEntities, new EdmPathExpression("Complex/CollectionOfNav"));
            entites.AddNavigationTarget(collectionNav, navEntities, new EdmPathExpression("CollectionOfComplex/CollectionOfNav"));
            entites.AddNavigationTarget(singleNav, navEntities, new EdmPathExpression("Complex/SingleOfNav"));
            entites.AddNavigationTarget(singleNav, navEntities, new EdmPathExpression("CollectionOfComplex/SingleOfNav"));
            entityContainer.AddElement(entites);
            entityContainer.AddElement(navEntities);

            return model;
        }


        private static IEdmModel ContainmentComplexModel()
        {
            var model = new EdmModel();

            var entity = new EdmEntityType("NS", "EntityType");
            var entityId = entity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            entity.AddKeys(entityId);

            var containedEntity = new EdmEntityType("NS", "ContainedEntityType");
            var containedEntityId = containedEntity.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            containedEntity.AddKeys(containedEntityId);

            var complex = new EdmComplexType("NS", "ComplexType");
            complex.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetInt32(false));

            var derivedComplex = new EdmComplexType("NS", "DerivedComplexType", complex);

            var containedUnderComplex = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "ContainedUnderComplex",
                    Target = containedEntity,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    ContainsTarget = true
                });

            var containedUnderDerivedComplex = derivedComplex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "ContainedUnderDerivedComplex",
                    Target = containedEntity,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    ContainsTarget = true
                });

            var navUnderContained = containedEntity.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "NavUnderContained",
                    Target = entity,
                    TargetMultiplicity = EdmMultiplicity.Many
                });

            entity.AddStructuralProperty("Complex", new EdmComplexTypeReference(complex, false));

            model.AddElement(entity);
            model.AddElement(containedEntity);
            model.AddElement(complex);
            model.AddElement(derivedComplex);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet entites1 = new EdmEntitySet(entityContainer, "Entities1", entity);
            EdmEntitySet entites2 = new EdmEntitySet(entityContainer, "Entities2", entity);
            entites1.AddNavigationTarget(navUnderContained, entites2, new EdmPathExpression("Complex/ContainedUnderComplex/NavUnderContained"));
            entityContainer.AddElement(entites1);
            entityContainer.AddElement(entites2);

            return model;
        }

        #endregion
        [Fact]
        public void NullValidationErrorMessageForCollectionsofComplexTypes()
        {
            string expectedErrorMessage = "A null value was found for the property named 'Addresses', which has the expected type 'Collection(DefaultNs.Address)[Nullable=False]'. The expected type 'Collection(DefaultNs.Address)[Nullable=False]' does not allow null values.";
            string actualErrorMessage = "";
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://host/$metadata#People/$entity\"," +
                    "\"UserName\":\"abc\"," +
                    "\"Addresses\":null" +
                "}";

            try
            {
                ReadPayload(payload, Model, EntitySet, EntityType).OfType<ODataResource>().ToList();
            }
            catch (Exception e)
            {
                actualErrorMessage = e.Message;               
            }
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }

        /// <summary>
        /// A nullable collection cannot be null but it can have null values.
        /// </summary>
        [Fact]
        public void NullValidationErrorMessageForANullableCollectionsofComplexTypes()
        {
            string expectedErrorMessage = "A null value was found for the property named 'NewAddresses', which has the expected type 'Collection(DefaultNs.Address)[Nullable=True]'. The expected type 'Collection(DefaultNs.Address)[Nullable=True]' cannot be null but it can have null values.";
            string actualErrorMessage = "";
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://host/$metadata#People/$entity\"," +
                    "\"UserName\":\"abc\"," +
                    "\"NewAddresses\":null" +
                "}";

            try
            {
                ReadPayload(payload, Model, EntitySet, EntityType).OfType<ODataResource>().ToList();
            }
            catch (Exception e)
            {
                actualErrorMessage = e.Message;
            }
            Assert.Equal(expectedErrorMessage, actualErrorMessage);
        }
    }
}
