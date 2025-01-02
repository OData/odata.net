//---------------------------------------------------------------------
// <copyright file="ContextUrlWriterReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Tests;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Roundtrip
{
    public partial class ContextUrlWriterReaderTests
    {
        private static readonly ODataVersion[] Versions = new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 };

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

        [Fact]
        public async Task ExpandedEntity_Async()
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

            using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Async = true }))
            {
                var (success, errors) = await CsdlWriter.TryWriteCsdlAsync(this.model, writer, CsdlTarget.OData).ConfigureAwait(false);
                if (!success)
                {
                    Assert.True(false, "Serialization was unsuccessful");
                }
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
                string.Format("\"{0}$metadata#Employees(AssociatedCompany,AssociatedCompany())/$entity\"", TestBaseUri),
                out payload, out contentType);

                this.ReadPayload(payload, contentType, model, omReader =>
                {
                    var reader = omReader.CreateODataResourceReader(this.employeeSet, this.employeeType);
                    while (reader.Read()) { }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                });
            }
        }

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
                Assert.Contains(expectedUri, payload);
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
    }
}
