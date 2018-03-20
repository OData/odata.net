﻿//---------------------------------------------------------------------
// <copyright file="ContextUrlWriterReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using System.Xml;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
    }

    public class FactoryAddress : Address
    {
        public string FactoryType { get; set; }
        public Collection<string> FactoryPhoneNumbers { get; set; }
    }

    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductBook : Product
    {
        public string Author { get; set; }
    }

    public class ProductCd : Product
    {
        public string Artist { get; set; }
    }

    [Flags]
    public enum AccessLevel
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4,
        ReadWrite = 3
    }

    public class ContextUrlWriterReaderTests
    {
        private const string TestNameSpace = "Microsoft.OData.Tests.ScenarioTests.Roundtrip";
        private const string TestContainerName = "InMemoryEntities";
        private const string TestBaseUri = "http://www.example.com/";

        private EdmModel model;
        private EdmEntityType personType;
        private EdmEntityType employeeType;
        private EdmEntityType companyType;
        private EdmEntityType workingGroupType;
        private EdmEntityType peopleWorkingGroupsType;
        private EdmEntityType productType;
        private EdmEntityType productBookType;
        private EdmEntityType productCdType;
        private EdmEntityType locationType;
        private EdmComplexType addressType;
        private EdmEnumType accessLevelType;

        private EdmEntitySet employeeSet;
        private EdmEntitySet companySet;
        private EdmEntitySet peopleSet;
        private EdmEntitySet workingGroupsSet;
        private EdmSingleton peopleWorkingGroupsSingleton;

        private Uri testServiceRootUri;

        protected readonly ODataFormat[] mimeTypes = new ODataFormat[]
        {
            ODataFormat.Json
        };

        public ContextUrlWriterReaderTests()
        {
            this.testServiceRootUri = new Uri(TestBaseUri);

            this.model = new EdmModel();

            var defaultContainer = new EdmEntityContainer(TestNameSpace, TestContainerName);
            this.model.AddElement(defaultContainer);

            productType = new EdmEntityType(TestNameSpace, "Product");
            var productIdProperty = new EdmStructuralProperty(productType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
            productType.AddProperty(productIdProperty);
            productType.AddKeys(new IEdmStructuralProperty[] { productIdProperty });
            productType.AddProperty(new EdmStructuralProperty(productType, "Name", EdmCoreModel.Instance.GetString(false)));
            this.model.AddElement(productType);

            productBookType = new EdmEntityType(TestNameSpace, "ProductBook", productType);
            productBookType.AddProperty(new EdmStructuralProperty(productBookType, "Author", EdmCoreModel.Instance.GetString(false)));
            this.model.AddElement(productBookType);

            productCdType = new EdmEntityType(TestNameSpace, "ProductCd", productType);
            productCdType.AddProperty(new EdmStructuralProperty(productCdType, "Artist", EdmCoreModel.Instance.GetString(false)));
            this.model.AddElement(productCdType);

            addressType = new EdmComplexType(TestNameSpace, "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(false)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "City", EdmCoreModel.Instance.GetString(false)));
            this.model.AddElement(addressType);

            var factoryAddressType = new EdmComplexType(TestNameSpace, "FactoryAddress", addressType, false);
            factoryAddressType.AddProperty(new EdmStructuralProperty(factoryAddressType, "FactoryType", EdmCoreModel.Instance.GetString(false)));
            factoryAddressType.AddProperty(new EdmStructuralProperty(factoryAddressType, "FactoryPhoneNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));
            this.model.AddElement(factoryAddressType);

            var manufactoryType = new EdmComplexType(TestNameSpace, "Manufactory");
            manufactoryType.AddProperty(new EdmStructuralProperty(manufactoryType, "ManufactoryAddress", new EdmComplexTypeReference(addressType, true)));
            manufactoryType.AddProperty(new EdmStructuralProperty(manufactoryType, "Name", EdmCoreModel.Instance.GetString(false)));
            manufactoryType.AddProperty(new EdmStructuralProperty(manufactoryType, "Numbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));
            this.model.AddElement(manufactoryType);

            var companyDivisionType = new EdmComplexType(TestNameSpace, "Division");
            companyDivisionType.AddProperty(new EdmStructuralProperty(companyDivisionType, "Name", EdmCoreModel.Instance.GetString(false)));
            companyDivisionType.AddProperty(new EdmStructuralProperty(companyDivisionType, "Manufactory", new EdmComplexTypeReference(manufactoryType, true)));
            this.model.AddElement(companyDivisionType);

            accessLevelType = new EdmEnumType(TestNameSpace, "AccessLevel", isFlags: true);
            accessLevelType.AddMember("None", new EdmEnumMemberValue(0));
            accessLevelType.AddMember("Read", new EdmEnumMemberValue(1));
            accessLevelType.AddMember("Write", new EdmEnumMemberValue(2));
            accessLevelType.AddMember("Execute", new EdmEnumMemberValue(4));
            accessLevelType.AddMember("ReadWrite", new EdmEnumMemberValue(3));
            this.model.AddElement(accessLevelType);

            personType = new EdmEntityType(TestNameSpace, "Person", null, false, /*IsOpen*/ true);
            var personIdProperty = new EdmStructuralProperty(personType, "PersonId", EdmCoreModel.Instance.GetInt32(false));
            personType.AddProperty(personIdProperty);
            personType.AddKeys(new IEdmStructuralProperty[] { personIdProperty });
            personType.AddProperty(new EdmStructuralProperty(personType, "Name", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Age", EdmCoreModel.Instance.GetInt32(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "HomeAddress", new EdmComplexTypeReference(addressType, true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "PhoneNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));
            personType.AddProperty(new EdmStructuralProperty(personType, "Addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressType, true)))));
            personType.AddProperty(new EdmStructuralProperty(personType, "UserAccess", new EdmEnumTypeReference(accessLevelType, true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "UserAccesses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEnumTypeReference(accessLevelType, true)))));

            this.model.AddElement(personType);

            employeeType = new EdmEntityType(TestNameSpace, "Employee", personType, false, true);
            employeeType.AddProperty(new EdmStructuralProperty(employeeType, "DateHired", EdmCoreModel.Instance.GetDateTimeOffset(true)));
            employeeType.AddProperty(new EdmStructuralProperty(employeeType, "WorkNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));
            this.model.AddElement(employeeType);

            companyType = new EdmEntityType(TestNameSpace, "Company");
            var companyIdProperty = new EdmStructuralProperty(companyType, "CompanyId", EdmCoreModel.Instance.GetInt32(false));
            companyType.AddProperty(companyIdProperty);
            companyType.AddKeys(new IEdmStructuralProperty[] { companyIdProperty });
            companyType.AddProperty(new EdmStructuralProperty(companyType, "Name", EdmCoreModel.Instance.GetString(false)));
            companyType.AddProperty(new EdmStructuralProperty(companyType, "Division", new EdmComplexTypeReference(companyDivisionType, true)));
            this.model.AddElement(companyType);

            locationType = new EdmEntityType(TestNameSpace, "Location");
            var locationId = new EdmStructuralProperty(locationType, "LocationId", EdmCoreModel.Instance.GetInt32(false));
            locationType.AddProperty(locationId);
            locationType.AddKeys(new IEdmStructuralProperty[] { locationId });
            locationType.AddProperty(new EdmStructuralProperty(locationType, "Name", EdmCoreModel.Instance.GetString(false)));
            this.model.AddElement(locationType);

            workingGroupType = new EdmEntityType(TestNameSpace, "WorkingGroup");
            var workingGroupIdProperty = new EdmStructuralProperty(workingGroupType, "WorkingGroupId", EdmCoreModel.Instance.GetInt32(false));
            workingGroupType.AddProperty(workingGroupIdProperty);
            workingGroupType.AddKeys(new IEdmStructuralProperty[] { workingGroupIdProperty });
            workingGroupType.AddProperty(new EdmStructuralProperty(workingGroupType, "Name", EdmCoreModel.Instance.GetString(false)));
            this.model.AddElement(workingGroupType);

            peopleWorkingGroupsType = new EdmEntityType(TestNameSpace, "PeopleWorkingGroupsRoot");
            this.model.AddElement(peopleWorkingGroupsType);

            this.peopleSet = new EdmEntitySet(defaultContainer, "People", personType);
            this.companySet = new EdmEntitySet(defaultContainer, "Companys", companyType);
            this.employeeSet = new EdmEntitySet(defaultContainer, "Employees", employeeType);
            this.workingGroupsSet = new EdmEntitySet(defaultContainer, "WorkingGroups", workingGroupType);
            defaultContainer.AddElement(this.employeeSet);
            defaultContainer.AddElement(this.peopleSet);
            defaultContainer.AddElement(this.companySet);
            defaultContainer.AddElement(this.workingGroupsSet);

            this.peopleWorkingGroupsSingleton = new EdmSingleton(defaultContainer, "PeopleWorkingGroups", peopleWorkingGroupsType);
            defaultContainer.AddElement(this.peopleWorkingGroupsSingleton);

            var associatedCompanyNavigation = employeeType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "AssociatedCompany",
                Target = companyType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            employeeSet.AddNavigationTarget(associatedCompanyNavigation, companySet);

            var peopleNavigation = peopleWorkingGroupsType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "People",
                Target = personType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true,
            });

            var workingGroupNavigation = peopleWorkingGroupsType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "WorkingGroups",
                Target = workingGroupType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true,
            });

            var associatedWorkingGroupsNavigation = personType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "AssociatedWorkingGroups",
                Target = workingGroupType,
                TargetMultiplicity = EdmMultiplicity.Many
            });

            var locationNavigation = companyType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Locations",
                Target = locationType,
                TargetMultiplicity = EdmMultiplicity.Many,
                ContainsTarget = true,
            });

            var propertyManagerNavigation = locationType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "PropertyManager",
                Target = personType,
                TargetMultiplicity = EdmMultiplicity.One,
            });

            var employeesNavigation = locationType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Employees",
                Target = personType,
                TargetMultiplicity = EdmMultiplicity.Many,
            });

            peopleWorkingGroupsSingleton.AddNavigationTarget(associatedWorkingGroupsNavigation, peopleWorkingGroupsSingleton);

            var locationNavSource = companySet.FindNavigationTarget(locationNavigation) as EdmNavigationSource;
            locationNavSource.AddNavigationTarget(propertyManagerNavigation, peopleSet);
            locationNavSource.AddNavigationTarget(employeesNavigation, peopleSet);
        }

        #region Cases

        #region Test ContextURL on Root
        // V4 Protocol Spec Chapters 10.1: Service Document
        // Sample Request: http://host/service.svc/
        // ContextUrl in Response: http://host/service.svc/$metadata
        [Fact]
        public void ServiceDocument()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;

                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    ODataServiceDocument serviceDocument = new ODataServiceDocument();
                    serviceDocument.EntitySets = new ODataEntitySetInfo[] { new ODataEntitySetInfo { Name = "People", Url = new Uri(TestBaseUri + "People") } };
                    omWriter.WriteServiceDocument(serviceDocument);
                }, string.Format("\"{0}$metadata\"", TestBaseUri), out payload, out contentType);
                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadServiceDocument());
            }
        }

        // V4 Protocol Spec Chapters 10.2: Not Contained Collection of Entities
        // Sample Rquest: http://host/service.svc/People
        // ContextUrl in Response: http://host/service.svc/$metadata#People
        [Fact]
        public void NotContainedCollectionOfEntities()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var writer = omWriter.CreateODataResourceSetWriter(this.peopleSet, this.personType);
                    var feed = new ODataResourceSet();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                }, string.Format("\"{0}$metadata#People\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader(this.peopleSet, this.personType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.2: Contained Collection of Entities
        // Sample Rquest: http://host/service.svc/PeopleWorkingGroups/People
        // ContextUrl in Response: http://host/service.svc/$metadata#PeopleWorkingGroups/People
        [Fact]
        public void ContainedCollectionOfEntities()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var writer = omWriter.CreateODataResourceSetWriter();
                    var feed = new ODataResourceSet
                    {
                        SerializationInfo = new ODataResourceSerializationInfo
                        {
                            NavigationSourceName = "PeopleWorkingGroups/People",
                            NavigationSourceEntityTypeName = this.personType.FullName(),
                            IsFromCollection = true
                        }
                    };
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                }, string.Format("\"{0}$metadata#PeopleWorkingGroups/People\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader();
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.3: Not Contained Entity
        // Sample Rquest: http://host/service.svc/People(1)
        // ContextUrl in Response: http://host/service.svc/$metadata#People/$entity
        [Fact]
        public void NotContainedEntity()
        {
            ODataResource entry = new ODataResource()
            {
                TypeName = TestNameSpace + ".Person",
                Properties = new[]
                {
                    new ODataProperty {Name = "PersonId", Value = 1},
                    new ODataProperty {Name = "Name", Value = "Test1"},
                    new ODataProperty {Name = "Age", Value = 20},
                    new ODataProperty {Name = "HomeAddress", Value = null},
                },
            };

            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var writer = omWriter.CreateODataResourceWriter(this.peopleSet, this.personType);
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                }, string.Format("\"{0}$metadata#People/$entity\"", TestBaseUri), out payload, out contentType);
                payload = payload.Replace(".People/$", ".Test/$");
                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.peopleSet, this.personType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.3: Entity
        //
        [Fact]
        public void ContainedEntity()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    omWriter.Settings.ODataUri = new ODataUriParser(model,new Uri("Companys(1)/Locations", UriKind.Relative)).ParseUri();
                    omWriter.Settings.ODataUri.ServiceRoot = new Uri(TestBaseUri);
                    omWriter.Settings.SetContentType("application/json;odata.metadata=full", "");
                    IEdmEntitySetBase target = this.companySet.FindNavigationTarget(this.companyType.FindProperty("Locations") as IEdmNavigationProperty) as IEdmEntitySetBase;
                    var writer = omWriter.CreateODataResourceSetWriter(target);
                    writer.WriteStart(new ODataResourceSet());
                    writer.WriteStart(new ODataResource {
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty {Name="LocationId", Value = 1 },
                            new ODataProperty {Name="Name", Value = "Burlington Factory" }
                        }
                    });
                    writer.WriteStart(new ODataNestedResourceInfo{ Name="Employees"});
                    writer.WriteStart(new ODataResourceSet());
                    writer.WriteStart(new ODataResource
                    {
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty {Name="PersonId", Value = 1 },
                            new ODataProperty {Name="Name", Value = "Fred" }
                        }
                    });
                    writer.WriteEnd(); // resource
                    writer.WriteEnd(); // nested resource set
                    writer.WriteEnd(); // nestedInfo
                    writer.WriteStart(new ODataNestedResourceInfo { Name = "PropertyManager" });
                    writer.WriteStart(new ODataResource
                    {
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty {Name="PersonId", Value = 1 },
                            new ODataProperty {Name="Name", Value = "Fred" }
                        }
                    });
                    writer.WriteEnd(); // resource
                    writer.WriteEnd(); // nestedInfo
                    writer.WriteEnd(); // resource
                    writer.WriteEnd(); // resource set
                }, string.Format("\"{0}$metadata#Companys(1)/Locations\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader();
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.4: Singleton
        // Sample Rquest: http://host/service.svc/PeopleWorkingGroups
        // ContextUrl in Response: http://host/service.svc/$metadata#PeopleWorkingGroups
        [Fact]
        public void Singleton()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var writer = omWriter.CreateODataResourceWriter(this.peopleWorkingGroupsSingleton, this.peopleWorkingGroupsType);
                    var feed = new ODataResource();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                }, string.Format("\"{0}$metadata#PeopleWorkingGroups\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.peopleWorkingGroupsSingleton, this.peopleWorkingGroupsType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.5: Entity Collection Derived Entities
        // Sample Request: http://host/service/People/Model.Employee
        // ContextUrl in Response: http://host/service/$metadata#People/Model.Employee
        [Fact]
        public void CollectionOfDerivedEntity()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var writer = omWriter.CreateODataResourceSetWriter(this.peopleSet, this.employeeType);
                    var feed = new ODataResourceSet();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                },
                string.Format("\"{0}$metadata#People/{1}.Employee\"", TestBaseUri, TestNameSpace),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader(this.peopleSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.6: Derived Entity
        // Sample Request: http://host/service/People(2)/Model.Employee
        // ContextUrl in Response: http://host/service/$metadata#People/Model.Employee/$entity
        [Fact]
        public void DerivedEntity()
        {
            ODataResource entry = new ODataResource()
            {
                TypeName = TestNameSpace + ".Employee",
                Properties = new[]
                {
                    new ODataProperty {Name = "PersonId", Value = 1},
                    new ODataProperty {Name = "Name", Value = "Test1"},
                    new ODataProperty {Name = "Age", Value = 20},
                    new ODataProperty {Name = "HomeAddress", Value = null},
                    new ODataProperty {Name = "DateHired", Value = null},
                },
            };

            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var writer = omWriter.CreateODataResourceWriter(this.peopleSet, this.employeeType);
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                },
                string.Format("\"{0}$metadata#People/{1}.Employee/$entity\"", TestBaseUri, TestNameSpace),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.peopleSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.7: Collection of Projected Entities (not on derived types)
        // Sample Request: http://host/service/People?$select=PersonId,Name
        // Context Url in Response: http://host/service/$metadata#People(PersonId,Name)
        [Fact]
        public void CollectionOfProjectedNotDerivedEntities()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.personType, this.peopleSet, new Dictionary<string, string> { { "$expand", "" }, { "$select", "PersonId,Name" } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };

                    var writer = omWriter.CreateODataResourceSetWriter(this.peopleSet, this.personType);
                    var feed = new ODataResourceSet();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                }, string.Format("\"{0}$metadata#People(PersonId,Name)", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader(this.peopleSet, this.personType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.7: Collection of Projected Entities (on derived types)
        // Sample Request: http://host/service/People/Model.Employee?$select=PersonId,Name
        // Context Url in Response: http://host/service/$metadata#People/Model.Employee(PersonId,Name)
        [Fact]
        public void CollectionOfProjectedDerivedEntities()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.personType, this.peopleSet, new Dictionary<string, string> { { "$expand", "" }, { "$select", "PersonId,Name" } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };

                    var writer = omWriter.CreateODataResourceSetWriter(this.peopleSet, this.employeeType);
                    var feed = new ODataResourceSet();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                },
                string.Format("\"{0}$metadata#People/{1}.Employee(PersonId,Name)\"", TestBaseUri, TestNameSpace),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader(this.peopleSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.8: Projected Entities (not on derived types)
        // Sample Request: http://host/service/People(1)?$select=PersonId,Name
        // Context Url in Response: http://host/service/$metadata#People(PersonId,Name)/$entity
        [Fact]
        public void ProjectedNotDerivedEntity()
        {
            ODataResource entry = new ODataResource()
            {
                TypeName = TestNameSpace + ".Person",
                Properties = new[]
                {
                    new ODataProperty {Name = "PersonId", Value = 1},
                    new ODataProperty {Name = "Name", Value = "Test1"},
                },
            };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.personType, this.peopleSet, new Dictionary<string, string> { { "$expand", "" }, { "$select", "PersonId,Name" } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };
                    var writer = omWriter.CreateODataResourceWriter(this.peopleSet, this.personType);
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                }, string.Format("\"{0}$metadata#People(PersonId,Name)/$entity\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.peopleSet, this.personType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapters 10.8: Projected Entities (on derived types)
        // Sample Request: http://host/service/People(1)/Model.Employee?$select=PersonId,Name
        // Context Url in Response: http://host/service/$metadata#People/Model.Employee(PersonId,Name)/$entity
        [Fact]
        public void ProjectedDerivedEntity()
        {
            ODataResource entry = new ODataResource()
            {
                TypeName = TestNameSpace + ".Employee",
                Properties = new[]
                {
                    new ODataProperty {Name = "PersonId", Value = 1},
                    new ODataProperty {Name = "Name", Value = "Test1"},
                },
            };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.personType, this.peopleSet, new Dictionary<string, string> { { "$expand", "" }, { "$select", "PersonId,Name" } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };
                    var writer = omWriter.CreateODataResourceWriter(this.peopleSet, this.employeeType);
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                },
                string.Format("\"{0}$metadata#People/{1}.Employee(PersonId,Name)/$entity\"", TestBaseUri, TestNameSpace),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.peopleSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapter 10.9: Collection of Projected Expanded Entities (contain select)
        // Sample Request: http://host/service/Employees?$expand=Company($select=CompanyId)
        // Context Url in Response: http://host/service/$metadata#Employees(Company(CompanyId))
        [Fact]
        public void CollectionOfProjectedExpandedEntities()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.employeeType, this.employeeSet, new Dictionary<string, string> { { "$expand", "AssociatedCompany($select=CompanyId)" }, { "$select", null } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };

                    var writer = omWriter.CreateODataResourceSetWriter(this.employeeSet, this.employeeType);
                    var feed = new ODataResourceSet();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                },
                string.Format("\"{0}$metadata#Employees(AssociatedCompany(CompanyId))\"", TestBaseUri),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader(this.employeeSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapter 10.9: Collection of Projected Expanded Entities (not contain select)
        // Sample Request: http://host/service/Employees?$expand=Company
        // Context Url in Response: http://host/service/$metadata#Employees(Company())
        [Theory]
        [InlineData(ODataVersion.V4, "\"{0}$metadata#Employees\"")]
        [InlineData(ODataVersion.V401, "\"{0}$metadata#Employees(AssociatedCompany())\"")]
        public void CollectionOfExpandedEntities(ODataVersion version, string contextString)
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.employeeType, this.employeeSet, new Dictionary<string, string> { { "$expand", "AssociatedCompany" }, { "$select", null } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };

                    omWriter.Settings.Version = version;

                    var writer = omWriter.CreateODataResourceSetWriter(this.employeeSet, this.employeeType);
                    var feed = new ODataResourceSet();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                }, string.Format(contextString, TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader(this.employeeSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }, version: version);
            }
        }

        // V4 Protocol Spec Chapter 10.9: Collection of Projected Expanded Entities (not contain select)
        // Sample Request: http://host/service/Employees?$expand=Company
        // Context Url in Response: http://host/service/$metadata#Employees(Company)
        [Fact]
        public void CollectionOfExpandedEntities_Version741AndBefore()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                // In 7.4.1 and before, this scenario resulted in the same 
                // context Uri as 10.2: http://host/service/$metadata#Employees
                var writerSettings = new ODataMessageWriterSettings
                {
                    //LibraryCompatibility = ODataLibraryCompatibility.Version7_4_1,
                };

                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, writerSettings, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.employeeType, this.employeeSet, new Dictionary<string, string> { { "$expand", "AssociatedCompany" }, { "$select", null } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };

                    var writer = omWriter.CreateODataResourceSetWriter(this.employeeSet, this.employeeType);
                    var feed = new ODataResourceSet();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                }, string.Format("\"{0}$metadata#Employees\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader(this.employeeSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapter 10.10: Projected Expanded Entities (contain select)
        // Sample Request: http://host/service/Employees(0)?$expand=Company($select=CompanyId)
        // Context Url in Response: http://host/service/$metadata#Employees(Company(CompanyId))/$entity
        [Fact]
        public void ProjectedExpandedEntity()
        {
            ODataResource entry = new ODataResource()
            {
                TypeName = TestNameSpace + ".Employee",
                Properties = new[]
                {
                    new ODataProperty {Name = "PersonId", Value = 1},
                    new ODataProperty {Name = "Name", Value = "Test1"},
                    new ODataProperty {Name = "Age", Value = 20},
                    new ODataProperty {Name = "HomeAddress", Value = null},
                    new ODataProperty {Name = "DateHired", Value = null},
                },
            };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.employeeType, this.employeeSet, new Dictionary<string, string> { { "$expand", "AssociatedCompany($select=CompanyId)" }, { "$select", null } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };
                    var writer = omWriter.CreateODataResourceWriter(this.employeeSet, this.employeeType);
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                },
                string.Format("\"{0}$metadata#Employees(AssociatedCompany(CompanyId))/$entity\"", TestBaseUri),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.employeeSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapter 10.10: Projected Expanded Entities (not contain select)
        // Sample Request: http://host/service/Employees(0)?$expand=Company
        // Context Url in Response: http://host/service/$metadata#Employees(Company)/$entity
        [Fact]
        public void ExpandedEntity()
        {
            ODataResource entry = new ODataResource()
            {
                TypeName = TestNameSpace + ".Employee",
                Properties = new[]
                {
                    new ODataProperty {Name = "PersonId", Value = 1},
                    new ODataProperty {Name = "Name", Value = "Test1"},
                    new ODataProperty {Name = "Age", Value = 20},
                    new ODataProperty {Name = "HomeAddress", Value = null},
                    new ODataProperty {Name = "DateHired", Value = null},
                },
            };

            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);

            using (var writer = XmlWriter.Create(stringWriter))
            {
                IEnumerable<EdmError> errors;
                CsdlWriter.TryWriteCsdl(this.model, writer, CsdlTarget.OData, out errors);
            }

            string modelAsString = stringBuilder.ToString();

            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.employeeType, this.employeeSet, new Dictionary<string, string> { { "$expand", "AssociatedCompany" }, { "$select", "AssociatedCompany" } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause
                    };
                    var writer = omWriter.CreateODataResourceWriter(this.employeeSet, this.employeeType);
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                },
                string.Format("\"{0}$metadata#Employees(AssociatedCompany)/$entity\"", TestBaseUri),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.employeeSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

        // V4 Protocol Spec Chapter 10.10: Projected Expanded Entities (not contain select)
        // Sample Request: http://host/service/Employees(0)?$expand=AssociatedCompany
        [Theory]
        [InlineData(ODataVersion.V4, "\"{0}$metadata#Employees(AssociatedCompany)/$entity\"")]
        [InlineData(ODataVersion.V401, "\"{0}$metadata#Employees(AssociatedCompany())/$entity\"")]
        public void ExpandedMultiSegmentEntity(ODataVersion version, string contextString)
        {
            ODataResource entry = new ODataResource()
            {
                TypeName = TestNameSpace + ".Employee",
                Properties = new[]
                {
                    new ODataProperty {Name = "PersonId", Value = 1},
                    new ODataProperty {Name = "Name", Value = "Test1"},
                    new ODataProperty {Name = "Age", Value = 20},
                    new ODataProperty {Name = "HomeAddress", Value = null},
                    new ODataProperty {Name = "DateHired", Value = null},
                },
            };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var selectExpandClause = new ODataQueryOptionParser(this.model, this.employeeType, this.employeeSet, new Dictionary<string, string> { { "$expand", "AssociatedCompany" }, { "$select", "AssociatedCompany" } }).ParseSelectAndExpand();

                    omWriter.Settings.ODataUri = new ODataUri()
                    {
                        ServiceRoot = this.testServiceRootUri,
                        SelectAndExpand = selectExpandClause,
                    };
                    omWriter.Settings.Version = version;
                    var writer = omWriter.CreateODataResourceWriter(this.employeeSet, this.employeeType);
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                },
                string.Format(contextString, TestBaseUri),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.employeeSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }, version: version);
            }
        }

        // V4 Protocol Spec Chapter 10.11: Collection of Entity References
        // Sample Request: http://host/service/Companys(0)/Employee/$ref
        // Context Url in Response: http://host/service/$metadata#Collection($ref)
        [Fact]
        public void CollectionOfEntityReferences()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri(TestBaseUri + "/Companys(1)") };
            ODataEntityReferenceLinks referenceLinks = new ODataEntityReferenceLinks { Links = new[] { referenceLink } };

            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter => omWriter.WriteEntityReferenceLinks(referenceLinks),
                    string.Format("\"{0}$metadata#Collection($ref)\"", TestBaseUri),
                    out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadEntityReferenceLinks());
            }
        }

        // V4 Protocol Spec Chapter 10.12: Entity References
        // Sample Request: http://host/service/Employee(0)/AssociatedCompany/$ref
        // Context Url in Response: http://host/service/$metadata#$ref
        [Fact]
        public void EntityReferences()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri(TestBaseUri + "/AssociatedCompany(1)") };

            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter => omWriter.WriteEntityReferenceLink(referenceLink),
                    string.Format("\"{0}$metadata#$ref\"", TestBaseUri),
                    out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadEntityReferenceLink());
            }
        }

        // V4 Protocol Spec Chapter 10.13: Individual Property With PrimitiveType
        // Sample Request: http://host/service/People(1)/Name
        // Context Url in Response: http://host/service/$metadata#People(1)/Name
        [Fact]
        public void IndividualPropertyWithPrimitiveType()
        {
            var property = new ODataProperty { Name = "propertyName", Value = "value" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + "People(1)/Name")).ParsePath()
                        };
                        omWriter.WriteProperty(property);
                    },
                    string.Format("\"{0}$metadata#People(1)/Name\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        // V4 Protocol Spec Chapter 10.13: Collection of individual property with primitive type
        // Sample Request: http://host/service/People(1)/PhoneNumbers
        // Context Url in Response: http://host/service/$metadata#People(1)/PhoneNumbers
        [Fact]
        public void CollectionOfPrimitiveTypeIndividualProperty()
        {
            var properties = new Collection<string>() { "TEST" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + "People(1)/PhoneNumbers")).ParsePath()
                        };
                        omWriter.WriteProperty(this.CreateODataProperty(properties, "PhoneNumbers"));
                    },
                    string.Format("\"{0}$metadata#People(1)/PhoneNumbers\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        // V4 Protocol Spec Chapter 10.13: Individual Property With ComplexType
        // Sample Request: http://host/service/People(1)/HomeAddress
        // Context Url in Response: http://host/service/$metadata#People(1)/HomeAddress
        [Fact]
        public void IndividualPropertyWithComplexType()
        {
            var address = new Address() { Street = "TestStreet", City = "TestCity" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + "People(1)/HomeAddress"));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        var odataWriter = omWriter.CreateODataResourceWriter();
                        var complexResource = this.CreateComplexResource(address, "HomeAddress");
                        odataWriter.WriteStart(complexResource);
                        odataWriter.WriteEnd();
                    },
                    string.Format("\"{0}$metadata#People(1)/HomeAddress\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model,
                omReader =>
                    {
                        var odatareader = omReader.CreateODataResourceReader();
                        while(odatareader.Read())
                        {
                            if(odatareader.State == ODataReaderState.ResourceEnd)
                            {
                                Assert.NotNull(odatareader.Item as ODataResource);
                            }
                        }
                    });
            }
        }

        // V4 Protocol Spec Chapter 10.13: Collection of individual property with complex type
        // Sample Request: http://host/service/People(1)/Addresses
        // Context Url in Response: http://host/service/$metadata#People(1)/Addresses
        [Fact]
        public void CollectionOfComplexTypeIndividualProperty()
        {
            var address = new Address() { Street = "TestStreet", City = "TestCity" };
            var addresses = new Collection<Address>() { address };

            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + "People(1)/Addresses"));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        var odataWriter = omWriter.CreateODataResourceSetWriter(null, addressType);
                        var complexResource = this.CreateComplexResource(address, "HomeAddress");
                        odataWriter.WriteStart(new ODataResourceSet());
                        odataWriter.WriteStart(complexResource);
                        odataWriter.WriteEnd();
                        odataWriter.WriteEnd();
                    },
                    string.Format("\"{0}$metadata#People(1)/Addresses\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model,
                    omReader =>
                    {
                        var odatareader = omReader.CreateODataResourceSetReader();
                        while (odatareader.Read())
                        {
                            if (odatareader.State == ODataReaderState.ResourceSetEnd)
                            {
                                Assert.NotNull(odatareader.Item as ODataResourceSet);
                            }
                            if (odatareader.State == ODataReaderState.ResourceEnd)
                            {
                                Assert.NotNull(odatareader.Item as ODataResource);
                            }
                        }
                    });
            }
        }

        // V4 Protocol Spec Chapter 10.13: Individual Property With EnumType
        // Sample Request: http://host/service/People(1)/UserAccess
        // Context Url in Response: http://host/service/$metadata#People(1)/UserAccess
        [Fact]
        public void IndividualPropertyWithEnumType()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + "People(1)/UserAccess"));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        omWriter.WriteProperty(this.CreateODataProperty(AccessLevel.Read, "UserAccess"));
                    },
                    string.Format("\"{0}$metadata#People(1)/UserAccess\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        // V4 Protocol Spec Chapter 10.13: Property
        // Sample Request: http://host/service/People(1)/UserAccesses
        // Context Url in Response: http://host/service/$metadata#People(1)/UserAccesses
        [Fact]
        public void CollectionOfEnumTypeIndividualProperty()
        {
            var properties = new Collection<AccessLevel>() { AccessLevel.Read };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + "People(1)/UserAccesses"));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        omWriter.WriteProperty(this.CreateODataProperty(properties, "UserAccesses"));
                    },
                    string.Format("\"{0}$metadata#People(1)/UserAccesses\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        // V4 Protocol Spec Chapter 10.14: Collection of Value with Complex Type
        // Response: http://host/service/$metadata#Collection(Model.Address)
        [Fact]
        public void CollectionOfComplexType()
        {
            var address = new Address() { Street = "TestStreet", City = "TestCity" };
            var addresses = new Collection<Address>() { address };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataWriter = omWriter.CreateODataResourceSetWriter(null, addressType);
                        var complexResource = this.CreateComplexResource(address, "HomeAddress");
                        odataWriter.WriteStart(new ODataResourceSet());
                        odataWriter.WriteStart(complexResource);
                        odataWriter.WriteEnd();
                        odataWriter.WriteEnd();
                    },
                    string.Format("\"{0}$metadata#Collection({1}.Address)\"", TestBaseUri, TestNameSpace),
                    out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var odatareader = omReader.CreateODataResourceSetReader();
                    while (odatareader.Read())
                    {
                        if (odatareader.State == ODataReaderState.ResourceSetEnd)
                        {
                            Assert.NotNull(odatareader.Item as ODataResourceSet);
                        }
                        if (odatareader.State == ODataReaderState.ResourceEnd)
                        {
                            Assert.NotNull(odatareader.Item as ODataResource);
                        }
                    }
                });
            }
        }

        // V4 Protocol Spec Chapter 10.14: Collection of Value with Primitive Type
        // Response: http://host/service/$metadata#Collection(Edm.String)
        [Fact]
        public void CollectionOfPromitiveType()
        {
            var properties = new Collection<string>() { "TEST" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter => omWriter.WriteProperty(this.CreateODataProperty(properties, "Test")),
                    string.Format("\"{0}$metadata#Collection(Edm.String)\"", TestBaseUri),
                    out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        // V4 Protocol Spec Chapter 10.14: Collection of Value with Enum Type
        // Response: http://host/service/$metadata#Collection(Model.AccessLevel)
        [Fact]
        public void CollectionOfEnumType()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter => omWriter.WriteProperty(this.CreateODataProperty(new Collection<AccessLevel> { AccessLevel.Read }, "AccessLevel")),
                    string.Format("\"{0}$metadata#Collection({1}.AccessLevel)\"", TestBaseUri, TestNameSpace),
                    out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        // V4 Protocol Spec Chapter 10.14: Collection of Value with Enum Type
        // Response: http://host/service/$metadata#Collection(Model.AccessLevel)
        [Fact]
        public void CollectionOfEnumTypeUsingODataCollectionReader()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                IEdmTypeReference typeReference = new EdmEnumTypeReference(accessLevelType, true);

                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        ODataCollectionStart collectionStart = new ODataCollectionStart();
                        collectionStart.SetSerializationInfo(new ODataCollectionStartSerializationInfo { CollectionTypeName = string.Format("Collection({0}.AccessLevel)", TestNameSpace) });
                        ODataEnumValue[] items =
                        {
                            new ODataEnumValue(AccessLevel.Read.ToString(), TestNameSpace),
                            new ODataEnumValue(AccessLevel.Write.ToString(), TestNameSpace)
                        };

                        ODataCollectionWriter collectionWriter = omWriter.CreateODataCollectionWriter(typeReference);
                        collectionWriter.WriteStart(collectionStart);
                        foreach (object item in items)
                        {
                            collectionWriter.WriteItem(item);
                        }

                        collectionWriter.WriteEnd();
                    },
                    string.Format("\"{0}$metadata#Collection({1}.AccessLevel)\"", TestBaseUri, TestNameSpace),
                    out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataCollectionReader(typeReference);
                    IList items = new ArrayList();
                    while (reader.Read())
                    {
                        if (ODataCollectionReaderState.Value == reader.State)
                        {
                            items.Add(reader.Item);
                        }
                    }

                    Assert.Equal(2, items.Count);
                });
            }
        }

        // V4 Protocol Spec Chapter 10.15: Value with Complex Type
        // Response: http://host/service/$metadata#Model.Address
        [Fact]
        public void ValueWithComplexType()
        {
            var address = new Address() { Street = "TestStreet", City = "TestCity" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataWriter = omWriter.CreateODataResourceWriter();
                        var complexResource = this.CreateComplexResource(address, "HomeAddress");
                        odataWriter.WriteStart(complexResource);
                        odataWriter.WriteEnd();
                    },
                    string.Format("\"{0}$metadata#{1}.Address\"", TestBaseUri, TestNameSpace), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                    {
                        var odatareader = omReader.CreateODataResourceReader();
                        while (odatareader.Read())
                        {
                            if (odatareader.State == ODataReaderState.ResourceEnd)
                            {
                                Assert.NotNull(odatareader.Item as ODataResource);
                            }
                        }
                    });
            }
        }

        // V4 Protocol Spec Chapter 10.15: Value with Primitive Type
        // Response: http://host/service/$metadata#Edm.String
        [Fact]
        public void ValueWithPrimitiveType()
        {
            var property = new ODataProperty { Name = "propertyName", Value = "value" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter => omWriter.WriteProperty(property),
                    string.Format("\"{0}$metadata#Edm.String\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        // V4 Protocol Spec Chapter 10.15: Value with Enum Type
        // Response: http://host/service/$metadata#Model.AccessLevel
        [Fact]
        public void ValueWithEnumType()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter => omWriter.WriteProperty(this.CreateODataProperty(AccessLevel.Read, "AccessLevel")),
                    string.Format("\"{0}$metadata#{1}.AccessLevel\"", TestBaseUri, TestNameSpace), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        [Fact]
        public void CollectionTypeIndividualPropertyInDerivedEntityType()
        {
            var properties = new Collection<string>() { "TEST" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + "People(1)/" + TestNameSpace + ".Employee/WorkNumbers"));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        omWriter.WriteProperty(this.CreateODataProperty(properties, "WorkNumbers"));
                    },
                    string.Format("\"{0}$metadata#People(1)/{1}.Employee/WorkNumbers\"", TestBaseUri, TestNameSpace), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        [Fact]
        public void IndividualPropertyInDerivedEntityType()
        {
            var property = new ODataProperty { Name = "DateHired", Value = new DateTimeOffset() };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + string.Format("People(1)/{0}.Employee/DateHired", TestNameSpace)));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        omWriter.WriteProperty(property);
                    },
                    string.Format("\"{0}$metadata#People(1)/{1}.Employee/DateHired\"", TestBaseUri, TestNameSpace), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        [Fact]
        public void DerivedComplexTypeFollowedbyMultipleLevelComplexType()
        {
            var factoryAddress = new FactoryAddress()
            {
                Street = "TestStreet",
                City = "TestCity",
                FactoryType = "TESTType",
                FactoryPhoneNumbers = new Collection<string>() { "TEST" },
            };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + string.Format("Companys(1)/Division/Manufactory/ManufactoryAddress/{0}.FactoryAddress", TestNameSpace)));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };

                        var odataWriter = omWriter.CreateODataResourceWriter();
                        var complexResource = this.CreateComplexResource(factoryAddress, "ManufactoryAddress");
                        odataWriter.WriteStart(complexResource);
                        odataWriter.WriteEnd();
                    },
                    string.Format("\"{0}$metadata#Companys(1)/Division/Manufactory/ManufactoryAddress/{1}.FactoryAddress\"", TestBaseUri, TestNameSpace), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var odatareader = omReader.CreateODataResourceReader();
                    while (odatareader.Read())
                    {
                        if (odatareader.State == ODataReaderState.ResourceEnd)
                        {
                            var resource = odatareader.Item as ODataResource;
                            Assert.NotNull(resource);
                            Assert.Equal(4, resource.Properties.Count());
                        }
                    }
                });
            }
        }

        [Fact]
        public void IndividualPropertyOfDerivedTypeFollowedbyMultipleLevelComplexType()
        {
            var property = new ODataProperty { Name = "Street", Value = "TestStreet" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + string.Format("Companys(1)/Division/Manufactory/ManufactoryAddress/{0}.FactoryAddress/FactoryType", TestNameSpace)));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        omWriter.WriteProperty(property);
                    },
                    string.Format("\"{0}$metadata#Companys(1)/Division/Manufactory/ManufactoryAddress/{1}.FactoryAddress/FactoryType\"", TestBaseUri, TestNameSpace), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        [Fact]
        public void CollectionTypeIndividualPropertyOfDerivedTypeFollowedByMultipleLevelComplexType()
        {
            var properties = new Collection<string>() { "TEST" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + string.Format("Companys(1)/Division/Manufactory/ManufactoryAddress/{0}.FactoryAddress/FactoryPhoneNumbers", TestNameSpace)));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        omWriter.WriteProperty(this.CreateODataProperty(properties, "FactoryPhoneNumbers"));
                    },
                    string.Format("\"{0}$metadata#Companys(1)/Division/Manufactory/ManufactoryAddress/{1}.FactoryAddress/FactoryPhoneNumbers\"", TestBaseUri, TestNameSpace), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        [Fact]
        public void IndividualPropertyFollowedbyMultipleLevelComplexType()
        {
            var property = new ODataProperty { Name = "Name", Value = "TestName" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + "Companys(1)/Division/Manufactory/Name"));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };
                        omWriter.WriteProperty(property);
                    },
                    string.Format("\"{0}$metadata#Companys(1)/Division/Manufactory/Name\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        [Fact]
        public void CollectionTypeIndividualTypeFollowedByMultipleLevelComplexType()
        {
            var properties = new Collection<string>() { "TEST" };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + string.Format("Companys(1)/Division/Manufactory/Numbers")));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };

                        omWriter.WriteProperty(this.CreateODataProperty(properties, "Numbers"));
                    },
                    string.Format("\"{0}$metadata#Companys(1)/Division/Manufactory/Numbers\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        [Fact]
        public void DerivedEntityWithOpenType()
        {
            var property = new ODataProperty { Name = "OpenType", Value = 123 };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + string.Format("People(1)/{0}.Employee/OpenType", TestNameSpace)));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };

                        omWriter.WriteProperty(property);
                    },
                    string.Format("\"{0}$metadata#People(1)/{1}.Employee/OpenType\"", TestBaseUri, TestNameSpace), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }

        [Fact]
        public void EntityTypeWithOpenType()
        {
            var property = new ODataProperty { Name = "OpenType", Value = 123 };
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model,
                    omWriter =>
                    {
                        var odataUriParser = new ODataUriParser(this.model, this.testServiceRootUri, new Uri(TestBaseUri + ("People(1)/OpenType")));
                        omWriter.Settings.ODataUri = new ODataUri()
                        {
                            ServiceRoot = this.testServiceRootUri,
                            Path = odataUriParser.ParsePath()
                        };

                        omWriter.WriteProperty(property);
                    },
                    string.Format("\"{0}$metadata#People(1)/OpenType\"", TestBaseUri), out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader => omReader.ReadProperty());
            }
        }
        #endregion

        #region Test ContextURL in request
        [Fact]
        public void MissmatchContextUrlInRequest()
        {
            foreach (ODataFormat mimeType in mimeTypes)
            {
                string payload, contentType;
                this.WriteAndValidateContextUri(mimeType, model, omWriter =>
                {
                    var writer = omWriter.CreateODataResourceSetWriter(this.peopleSet, this.employeeType);
                    var feed = new ODataResourceSet();
                    feed.Id = new Uri("urn:test");
                    writer.WriteStart(feed);
                    writer.WriteEnd();
                },
                string.Format("\"{0}$metadata#People/{1}.Employee\"", TestBaseUri, TestNameSpace),
                out payload, out contentType);

                payload = payload.Replace("$metadata#People/Microsoft.OData.Tests.Employee", "WrongURL");
                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceSetReader(this.peopleSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                },
                false);
            }
        }
        #endregion

        #endregion

        #region Helper Function
        private void WriteAndValidateContextUri(ODataFormat odataFormat, EdmModel edmModel, Action<ODataMessageWriter> test, string expectedUri, out string payload, out string contentType)
        {
            var writerSettings = new ODataMessageWriterSettings();
            WriteAndValidateContextUri(odataFormat, model, writerSettings, test, expectedUri, out payload, out contentType);
        }

        private void WriteAndValidateContextUri(ODataFormat odataFormat, EdmModel edmModel, ODataMessageWriterSettings writerSettings, Action<ODataMessageWriter> test, string expectedUri, out string payload, out string contentType)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream() };

            writerSettings.EnableMessageStreamDisposal = false;
            writerSettings.BaseUri = new Uri(TestBaseUri);
            writerSettings.SetServiceDocumentUri(new Uri(TestBaseUri));
            writerSettings.SetContentType(odataFormat);

            using (var msgWriter = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, edmModel))
            {
                test(msgWriter);
            }

            message.Stream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(message.Stream))
            {
                contentType = message.GetHeader("Content-Type");
                payload = reader.ReadToEnd();
                payload.Should().Contain(expectedUri);
            }
        }

        private void ReadPayload(string payload, string contentType, EdmModel edmModel, Action<ODataMessageReader> test, bool readingResponse = true, ODataVersion version = ODataVersion.V4)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", contentType);

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
            {
                BaseUri = new Uri(TestBaseUri + "$metadata"),
                EnableMessageStreamDisposal = true,
                Version = version,
            };

            using (var msgReader =
                readingResponse
                ? new ODataMessageReader((IODataResponseMessage)message, readerSettings, edmModel)
                : new ODataMessageReader((IODataRequestMessage)message, readerSettings, edmModel))
            {
                test(msgReader);
            }
        }

        private ODataResource CreateComplexResource(object value, string propertyName)
        {
            Type t = value.GetType();
            List<ODataProperty> properties = new List<ODataProperty>();
            foreach (var p in t.GetProperties())
            {
                properties.Add(CreateODataProperty(t.GetProperty(p.Name).GetValue(value, new object[] { }), p.Name));
            }

            return new ODataResource() { TypeName = t.FullName, Properties = properties, };
        }

        private ODataProperty CreateODataProperty(object value, string propertyName)
        {
            if (value != null)
            {
                Type t = value.GetType();
                if (t.IsGenericType())
                {
                    // Build a collection type property
                    string genericTypeName = t.GetGenericTypeDefinition().Name;
                    genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
                    genericTypeName += "(" + t.GetGenericArguments().Single().FullName.Replace("System.", "Edm.") + ")";

                    if (t.GetGenericArguments().Single().Namespace != "System" && !t.GetGenericArguments().Single().Namespace.StartsWith("Microsoft.Spatial"))
                    {
                        var items = new List<object>();
                        var values = value as IEnumerable;
                        foreach (var v in values)
                        {
                            items.Add(CreateODataProperty(v, t.GetGenericArguments().Single().FullName.Replace("System.", "Edm.")).Value);
                        }
                        return new ODataProperty { Name = propertyName, Value = new ODataCollectionValue() { TypeName = genericTypeName, Items = items } };
                    }
                    else
                    {
                        return new ODataProperty { Name = propertyName, Value = new ODataCollectionValue() { TypeName = genericTypeName, Items = (value as IEnumerable).Cast<object>() } };
                    }
                }
                else if (t.Namespace != "System" && !t.Namespace.StartsWith("Microsoft.Spatial"))
                {
                    if (t.IsEnum() == true)
                    {
                        return new ODataProperty { Name = propertyName, Value = new ODataEnumValue(value.ToString(), t.FullName) };
                    }
                }
            }

            return new ODataProperty { Name = propertyName, Value = value };
        }
        #endregion
    }
}
