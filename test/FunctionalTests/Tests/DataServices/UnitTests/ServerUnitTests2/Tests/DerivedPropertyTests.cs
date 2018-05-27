//---------------------------------------------------------------------
// <copyright file="DerivedPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Net;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using test = System.Data.Test.Astoria;
    using ResourceAssociationSet = Microsoft.OData.Service.Providers.ResourceAssociationSet;
    using ResourceProperty = Microsoft.OData.Service.Providers.ResourceProperty;
    using ResourceType = Microsoft.OData.Service.Providers.ResourceType;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
    [Ignore] // Remove Atom
    // [TestClass]
    public class DerivedPropertyTests
    {
        private const int PeopleSetCount = 7;
        private const string PeopleTypeName = "AstoriaUnitTests.Tests.PeopleType";
        private const string EmployeeTypeName = "AstoriaUnitTests.Tests.EmployeeType";
        private const string ManagerTypeName = "AstoriaUnitTests.Tests.ManagerType";
        private const string CustomerTypeName = "AstoriaUnitTests.Tests.CustomerType";
        private const string CustomerAddressTypeName = "AstoriaUnitTests.Tests.CustomerAddressType";
        private const string EmployeeAddressTypeName = "AstoriaUnitTests.Tests.EmployeeAddressType";

        private const string RelNamespace = "http://docs.oasis-open.org/odata/ns/related/";
        private const string RelLinksNamespace = "http://docs.oasis-open.org/odata/ns/relatedlinks/";

        private const string ManagerNavigationPropertyRelValue = RelNamespace + "Manager";

        private const string ManagerNavigationLinkXPath = "/atom:link[@rel='" + ManagerNavigationPropertyRelValue + "' and @title='Manager' and contains(@href, '{0}/{1}/Manager')]";
        private const string DirectReportsNavigationLinkXPath = "/atom:link[@rel='" + RelNamespace + "DirectReports' and @title='DirectReports' and contains(@href, '{0}/" + ManagerTypeName + "/DirectReports')]";
        private const string AddressNavigationLinkXPath = "/atom:link[@rel='" + RelNamespace + "Address' and @title='Address' and contains(@href, '{0}/{1}/Address')]";

        private const string ManagerAssociationLinkXPath = "/atom:link[@rel='" + RelLinksNamespace + "Manager' and @title='Manager' and contains(@href, '{0}/{1}/Manager/$ref')]";
        private const string DirectReportsAssociationLinkXPath = "/atom:link[@rel='" + RelLinksNamespace + "DirectReports' and @title='DirectReports' and contains(@href, '{0}/" + ManagerTypeName + "/DirectReports/$ref')]";
        private const string AddressAssociationLinkXPath = "/atom:link[@rel='" + RelLinksNamespace + "Address' and @title='Address' and contains(@href, '{0}/{1}/Address/$ref')]";

        private static DSPUnitTestServiceDefinition[] services;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            string[] providers = typeof(DSPDataProviderKind).GetEnumNames();
            services = new DSPUnitTestServiceDefinition[providers.Length + 1];

            for (int i = 0; i < providers.Length; i++)
            {
                DSPDataProviderKind providerKind = (DSPDataProviderKind)Enum.Parse(typeof(DSPDataProviderKind), providers[i]);
                services[i] = GetUnitTestServiceDefinition(providerKind, false /*openType*/, false /*namedStreams*/);
            }

            services[services.Length - 1] = GetUnitTestServiceDefinition(DSPDataProviderKind.CustomProvider, true /*openType*/, false /*namedStreams*/);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedNavPropertyModelValidation()
        {
            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { "/$metadata" },
                    XPaths = new string[] { "//edmx:DataServices" },
                }
            };

            SendRequestAndVerifyXPaths(testCases, formats: new string[] { UnitTestsUtil.MimeApplicationXml });
        }

        [TestCategory("Partition2"), TestMethod]
        public void FilterEntitySetWithTypeIdentifier()
        {
            var testVersion = new ServiceVersions();
            string xPath = "/atom:entry[atom:category/@term='#" + ManagerTypeName + "' and atom:id='http://host/People(2)' and atom:content/adsm:properties/ads:ID='2']";

            DerivedPropertyTestCase[] testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People/{0}", PeopleTypeName) },
                    XPaths = GetLinks(
                        GetLinksForPeopleEntity("2", ManagerTypeName),
                        "count(/atom:feed/atom:entry)=7",
                        "/atom:feed" + xPath),
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People/{0}", ManagerTypeName)},
                    XPaths = new string[] { "count(/atom:feed/atom:entry)=2", "/atom:feed" + xPath },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People/{0}", EmployeeTypeName)},
                    XPaths = new string[] { "count(/atom:feed/atom:entry)=5", "/atom:feed" + xPath },
                    Version = testVersion,
                }
            };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void FilterSingleEntityWithTypeIdentifier()
        {
            string xPath = "/atom:entry[atom:category/@term='#" + ManagerTypeName + "' and atom:id='http://host/People(2)' and atom:content/adsm:properties/ads:ID='2']";

            DerivedPropertyTestCase[] testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People(2)/{0}", ManagerTypeName),
                        String.Format("/People(2)/{0}", EmployeeTypeName),
                        String.Format("/People(2)/{0}", PeopleTypeName) },
                    XPaths = new string[] { String.Format("count(/atom:entry/atom:category[@term='#{0}'])=1", ManagerTypeName), xPath },
                    Version = new ServiceVersions(),
                    MinProtocolVersion = ODataProtocolVersion.V4,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People/{0}(2)", ManagerTypeName),
                        String.Format("/People/{0}(2)", EmployeeTypeName),
                        String.Format("/People/{0}(2)", PeopleTypeName) },
                    XPaths = new string[] { String.Format("count(/atom:entry/atom:category[@term='#{0}'])=1", ManagerTypeName), xPath },
                    Version = new ServiceVersions(),
                },
            };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPrimitivePropertyTests()
        {
            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People(2)/{0}/FullName", EmployeeTypeName),
                        String.Format("/People(2)/{0}/FullName", ManagerTypeName) },
                    XPaths = new string[] { "count(/adsm:value[text()='Andy Conrad'])=1" },
                    MinProtocolVersion = ODataProtocolVersion.V4,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People/{0}(2)/FullName", EmployeeTypeName),
                        String.Format("/People/{0}(2)/FullName", ManagerTypeName) },
                    XPaths = new string[] { "count(/adsm:value[text()='Andy Conrad'])=1" },
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People(2)/{0}/Name", PeopleTypeName) },
                    XPaths = new string[] { "count(/adsm:value[text()='Andy'])=1" },
                    MinProtocolVersion = ODataProtocolVersion.V4,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People/{0}(2)/Name", PeopleTypeName) },
                    XPaths = new string[] { "count(/adsm:value[text()='Andy'])=1" },
                },
            };
            
            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            SendRequestAndVerifyXPaths(testCases, formats: new string[] { UnitTestsUtil.MimeApplicationXml });
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedNavigationPropertyTests()
        {
            var testVersion = new ServiceVersions();

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People(3)/{0}/Manager", EmployeeTypeName), 
                        String.Format("/People(3)/{0}/Manager/{1}", EmployeeTypeName, ManagerTypeName) },
                    XPaths = GetLinks(
                        GetLinksForPeopleEntity("2", ManagerTypeName),
                        "count(/atom:entry)=1",
                        "/atom:entry[atom:category/@term='#" + ManagerTypeName + "' and contains(atom:id, 'People(2)') and atom:content/adsm:properties/ads:ID='2']",
                        "count(/atom:entry/atom:link)=7",
                        "/atom:entry/atom:link[@rel='edit' and contains(@href, 'People(2)')]"),
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People(5)/{0}/DirectReports", ManagerTypeName) },
                    XPaths = GetLinks(
                        GetLinksForPeopleEntity("2", ManagerTypeName),
                        "count(/atom:feed/atom:entry)=2",
                        string.Format("/atom:feed[atom:id='http://host/People(5)/{0}/DirectReports']", ManagerTypeName),
                        "count(/atom:feed/atom:entry/atom:link)=12", // Andy has edit, Manager, DR and address link, Marcelo has edit, Manager, Address links
                        "/atom:feed/atom:entry/atom:link[@rel='edit' and contains(@href, 'People(2)')]", // BUG BUG : the title must be manager, since the instance is manager type 
                        String.Format("/atom:feed/atom:entry" + ManagerNavigationLinkXPath, "People(6)", EmployeeTypeName),
                        "/atom:feed/atom:entry/atom:link[@rel='edit' and contains(@href, 'People(6)')]" ),
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People(5)/{0}/DirectReports/{0}", ManagerTypeName) },
                    XPaths = GetLinks(
                        GetLinksForPeopleEntity("2", ManagerTypeName),
                        "count(/atom:feed/atom:entry)=1",
                        String.Format("/atom:feed[atom:id='http://host/People(5)/{0}/DirectReports/{0}']", ManagerTypeName),
                        "count(/atom:feed/atom:entry/atom:link)=7",
                        "/atom:feed/atom:entry/atom:link[@rel='edit' and contains(@href,'People(2)')]" // BUG BUG : In the above case, its writes out employee, but now its writes manager
                    ),
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People(2)/{0}/DirectReports(3)", ManagerTypeName),
                                   String.Format("/People(2)/{0}/DirectReports(3)/{1}", ManagerTypeName, EmployeeTypeName),
                                   String.Format("/People(2)/{0}/DirectReports/{1}(3)", ManagerTypeName, EmployeeTypeName) },
                    XPaths = new string[] { "count(/atom:entry)=1",
                                   "/atom:entry[atom:category/@term='#" + EmployeeTypeName + "' and atom:id='http://host/People(3)' and atom:content/adsm:properties/ads:ID='3']",
                                   "count(/atom:entry/atom:link)=5",
                                   "/atom:entry/atom:link[@rel='edit' and contains(@href,'People(3)')]",
                                   String.Format("/atom:entry" + ManagerNavigationLinkXPath, "People(3)", EmployeeTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People(3)/{0}/DirectReports", ManagerTypeName) },
                    XPaths = new string[] { "count(//atom:entry)=0" },
                    Version = testVersion,
                }
            };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPropertyExpandTest()
        {
            var testVersion = new ServiceVersions();

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People?$expand={0}/Manager", EmployeeTypeName),
                        String.Format("/People?$expand={0}/Manager,{1}/Manager", EmployeeTypeName, ManagerTypeName),
                        String.Format("/People?$expand={0}/Manager,{1}/Manager", ManagerTypeName, EmployeeTypeName),
                        String.Format("/People?$expand={0}/Manager", EmployeeTypeName)}, // all the employees must have the Manager property expanded
                    XPaths = new string[] { 
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@title='Manager']/adsm:inline)=3", EmployeeTypeName),
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@title='Manager']/adsm:inline)=2", ManagerTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$expand={0}/Manager", ManagerTypeName) }, // all the managers must have the Manager property expanded
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@title='Manager']/adsm:inline)=2", ManagerTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$expand={0}/DirectReports", ManagerTypeName) }, // all the managers must have the DirectReports property expanded
                    XPaths = new string[] { 
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@title='DirectReports']/adsm:inline/atom:feed)=2", ManagerTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$expand={0}/Manager,{1}/DirectReports", EmployeeTypeName, ManagerTypeName) },
                    XPaths = new string[] { 
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:link[@title='Manager']/adsm:inline)=5",
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@title='DirectReports']/adsm:inline/atom:feed)=2", ManagerTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$expand={0}/DirectReports($expand={1}/Manager,{0}/DirectReports)", ManagerTypeName, EmployeeTypeName) },
                    XPaths = new string[] { 
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:link[@title='Manager']/adsm:inline)=0",
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@title='DirectReports']/adsm:inline/atom:feed)=2", ManagerTypeName),
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@title='DirectReports']/adsm:inline/atom:feed/atom:entry/atom:link[@title='Manager'])>4", ManagerTypeName),
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@title='DirectReports']/adsm:inline/atom:feed/atom:entry/atom:link[@title='DirectReports']/adsm:inline)=1", ManagerTypeName)
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$expand={0}/Address", EmployeeTypeName) }, 
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}' or atom:category/@term='#{1}']/atom:link[@title='Address']/adsm:inline)=5", EmployeeTypeName, ManagerTypeName), // all the employees must have the address property expanded
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term!='#{0}' and atom:category/@term!='#{1}']/atom:link[@title='Address']/adsm:inline)=0", EmployeeTypeName, ManagerTypeName), // none employee should not have address expanded
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$expand={0}/Address, {1}/Address", EmployeeTypeName, CustomerTypeName) }, 
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}' or @term='#{1}']]/atom:link[@title='Address']/adsm:inline/atom:entry/atom:category[@term='#{2}'])=5", EmployeeTypeName, ManagerTypeName, EmployeeAddressTypeName), // all the employees must have the address property expanded
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}']]/atom:link[@title='Address']/adsm:inline/atom:entry/atom:category[@term='#{1}'])=1", CustomerTypeName, CustomerAddressTypeName), // all the customers must have the address property expanded
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term!='#{0}' and @term!='#{1}' and @term!='#{2}']]/atom:link[@title='Address']/adsm:inline)=0", EmployeeTypeName, ManagerTypeName, CustomerTypeName), // none employees and customers should not have address expanded
                    },
                    Version = testVersion,
                },
            };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPropertyProjectionTests()
        {
            var testVersion = new ServiceVersions();

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People?$select={0}/ID", ManagerTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        String.Format("count(//atom:entry[atom:category/@term!='#{0}']/adsm:properties[count(*)!=0])=0", ManagerTypeName), // non-manager type should have 0 properties projected
                        String.Format("count(//atom:entry[atom:category/@term='#{0}']/adsm:properties[count(*)!=1])=0", ManagerTypeName), // Manager types should have only one property projected
                        String.Format("count(//atom:entry[atom:category/@term='#{0}']/adsm:properties/*[local-name()!='ID'])=0", ManagerTypeName),  // Manager types should have only ID property projected
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID&$expand={0}/Manager", EmployeeTypeName) }, // all the employees must have the Manager link property
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(//adsm:properties/*[local-name()!='ID'])!=0", // only ID properties are projected for all entries
                        "count(//atom:entry[atom:category/@term='#AstoriaUnitTests.Tests.People']/atom:link[@rel!=edit])=0", // people type should only have edit links
                        String.Format("count(//atom:entry[atom:category/@term='#{0}' or atom:category/@term='#{1}']/atom:link)>15", EmployeeTypeName, ManagerTypeName) // there should be 5 employee or manager types with edit and manager navigation and association links
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID&$expand={0}/Manager", ManagerTypeName) }, // all the managers must have the Manager link property
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(//adsm:properties/*[local-name()!='ID'])!=0", // only ID properties are projected for all entries
                        "count(//atom:entry/atom:link[@title='Manager'])>4", // Total number of manager links in the entire payload must be 4
                        String.Format("count(//atom:entry[atom:category/@term='#{0}']/atom:link[@title='Manager'])>4", ManagerTypeName) // there should be 2 manager types with manager navigation and association links
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID&$expand={0}/Manager", EmployeeTypeName) }, // all the employees must have the Manager property expanded
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID'])=0", // only ID properties are projected for all entries
                        "count(/atom:feed/atom:entry[atom:category/@term='#AstoriaUnitTests.Tests.People']/atom:link[@rel!=edit])=0", // people type should only have edit links
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}' or atom:category/@term='#{1}']/atom:link)=15", EmployeeTypeName, ManagerTypeName), // there should be 5 employee or manager types with edit and manager links
                        String.Format("count(//atom:link[@title='Manager']/adsm:inline)=5") // all the manager links must be expanded
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID&$expand={0}/Manager($select=Name)", EmployeeTypeName) }, // all the employees must have the Manager property expanded
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID'])=0", // only ID properties are projected for all entries
                        "count(/atom:feed/atom:entry[atom:category/@term='#AstoriaUnitTests.Tests.People']/atom:link[@rel!=edit])=0", // people type should only have edit links
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}' or atom:category/@term='#{1}']/atom:link)=15", EmployeeTypeName, ManagerTypeName), // there should be 5 employee or manager types with edit and manager links
                        String.Format("count(//atom:link[@title='Manager']/adsm:inline)=5", EmployeeTypeName, ManagerTypeName), // all the manager links must be expanded
                        "count(//adsm:inline/atom:entry/atom:content/adsm:properties/*[local-name()!='Name'])=0", // expanded entries should have only property
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID,{0}/FullName", EmployeeTypeName) }, // project full name for employees
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID' and local-name()!=FullName])=0", // only ID and FullName properties must be projected for all entries
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term!='#{0}' and atom:category/@term!='#{1}']/atom:content/adsm:properties[count(*)!=1])=0", EmployeeTypeName, ManagerTypeName), // People and Customer should have only one property i.e. ID
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term!='#{0}' and atom:category/@term!='#{1}']/atom:content/adsm:properties/*[local-name()!='ID'])=0", EmployeeTypeName, ManagerTypeName), // People and Customershould have only one property i.e. ID
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}' or @term='#{1}']]/atom:content/adsm:properties[count(*)!=2])=0", EmployeeTypeName, ManagerTypeName), // Employee and Manager should have only 2 properties i.e. ID and FullName
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID,{0}/FullName,{1}/FullName", EmployeeTypeName, CustomerTypeName) }, // project full name for employees and customer
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID' and local-name()!=FullName])=0", // only ID and FullName properties must be projected for all entries
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:content/adsm:properties[count(*)!=1])=0", PeopleTypeName), // People should have only one property i.e. ID
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:content/adsm:properties/*[local-name()!='ID'])=0", PeopleTypeName), // People should have only one property i.e. ID
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}' or @term='3{1}' or @term='3{2}']]/atom:content/adsm:properties[count(*)!=2])=0", EmployeeTypeName, ManagerTypeName, CustomerTypeName), // Employee,Manager and Customer should have only 2 properties i.e. ID and FullName
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID,{0}/FullName,{1}/FullName", ManagerTypeName, CustomerTypeName) }, // project full name for manager and customer types
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID' and local-name()!=FullName])=0", // only ID and FullName properties must be projected for all entries
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}' or @term='#{1}']]/atom:content/adsm:properties[count(*)!=1])=0", PeopleTypeName, EmployeeTypeName), // People and employee should have only one property i.e. ID
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}' or @term='#{1}']]/atom:content/adsm:properties/*[local-name()!='ID'])=0", PeopleTypeName, EmployeeTypeName), // People and employee should have only one property i.e. ID
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}' or @term='#{1}']]/atom:content/adsm:properties[count(*)!=2])=0", ManagerTypeName, CustomerTypeName), // Manager and Customer should have only 2 properties i.e. ID and FullName
                    },
                    Version = testVersion,
                },
            };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPropertyOrderByTests()
        {
            var testVersion = new ServiceVersions();

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$orderby={0}/Manager/Name", EmployeeTypeName) },
                    XPaths = new string[] {
                        "/atom:feed/atom:entry[position()=1 and atom:id='http://host/People(1)']",
                        "/atom:feed/atom:entry[position()=2 and atom:id='http://host/People(5)']",
                        "/atom:feed/atom:entry[position()=3 and atom:id='http://host/People(7)']",
                        "/atom:feed/atom:entry[position()=4 and atom:id='http://host/People(3)']",
                        "/atom:feed/atom:entry[position()=5 and atom:id='http://host/People(4)']",
                        "/atom:feed/atom:entry[position()=6 and atom:id='http://host/People(2)']",
                        "/atom:feed/atom:entry[position()=7 and atom:id='http://host/People(6)']",
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People/{0}?$orderby={1}/Manager/Name", EmployeeTypeName, ManagerTypeName),
                    },
                    XPaths = new string[] {
                        "/atom:feed/atom:entry[position()=1 and atom:id='http://host/People(3)']",
                        "/atom:feed/atom:entry[position()=2 and atom:id='http://host/People(4)']",
                        "/atom:feed/atom:entry[position()=3 and atom:id='http://host/People(5)']",
                        "/atom:feed/atom:entry[position()=4 and atom:id='http://host/People(6)']",
                        "/atom:feed/atom:entry[position()=5 and atom:id='http://host/People(2)']",
                    },
                    Version = testVersion,
                },
             };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPropertyFilterTests()
        {
            var testVersion = new ServiceVersions();

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$filter={0}/Manager/Name eq 'Shyam'", EmployeeTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=2",
                        "/atom:feed/atom:entry[atom:id='http://host/People(2)']",
                        "/atom:feed/atom:entry[atom:id='http://host/People(6)']"
                    },
                    Version = testVersion,
                }
             };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPropertyFilterTestsWithAnyAll()
        {
            var testVersion = new ServiceVersions();

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$filter={0}/DirectReports/{0}/any()", ManagerTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=1",
                        "/atom:feed/atom:entry[atom:id='http://host/People(5)']",
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$filter={0}/DirectReports/{0}/any(m: m/DirectReports/{0}/any() eq false)", ManagerTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=1",
                        "/atom:feed/atom:entry[atom:id='http://host/People(5)']",
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$filter={0}/DirectReports/{0}/any(m: m/DirectReports/{0}/any() eq false and not($it/{0}/DirectReports/all(dr: isof(dr, '{0}'))))", ManagerTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=1",
                        "/atom:feed/atom:entry[atom:id='http://host/People(5)']",
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$filter={1}/Manager/DirectReports/{0}/any(m: m/DirectReports/{0}/any())", ManagerTypeName, EmployeeTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=0",
                    },
                    Version = testVersion,
                },
             };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void ExpandWithSDP()
        {
            foreach (var service in services)
            {
                service.PageSizeCustomizer = (config, serviceType) => { config.SetEntitySetPageSize("People", 2); };
            };
            var testVersion = new ServiceVersions();

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People/{0}", EmployeeTypeName) },
                    XPaths = new string[] {
                        "count(//atom:entry)=2",
                        String.Format("//atom:link[@rel='next' and @href='http://host/People/{0}?$skiptoken=3']", EmployeeTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People/{0}?$orderby=FullName,ID asc", EmployeeTypeName) },
                    XPaths = new string[] {
                        "count(//atom:entry)=2",
                        String.Format("//atom:link[@rel='next' and @href=\"http://host/People/{0}?$orderby=FullName,ID%20asc&$skiptoken='Jian%20Li',4\"]", EmployeeTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$orderby={0}/Manager/Name,ID asc", EmployeeTypeName) },
                    XPaths = new string[] {
                        "count(//atom:entry)=2",
                        String.Format("//atom:link[@rel='next' and @href=\"http://host/People?$orderby={0}/Manager/Name,ID%20asc&$skiptoken=null,5\"]", EmployeeTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People/{0}?$expand={1}/DirectReports", EmployeeTypeName, ManagerTypeName) },
                    XPaths = new string[] {
                        "count(//atom:entry)=4",
                        "count(/atom:feed/atom:entry)=2",
                        String.Format("/atom:feed/atom:link[@rel='next' and @href='http://host/People/{0}?$expand={1}/DirectReports&$skiptoken=3']", EmployeeTypeName, ManagerTypeName),
                        String.Format("/atom:feed/atom:entry/atom:link/adsm:inline/atom:feed/atom:link[@rel='next' and @href='http://host/People(2)/{0}/DirectReports?$skiptoken=4']", ManagerTypeName) },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$expand={0}/DirectReports", ManagerTypeName) },
                    XPaths = new string[] {
                        "count(//atom:entry)=4",
                        "count(/atom:feed/atom:entry)=2",
                        String.Format("/atom:feed/atom:link[@rel='next' and @href='http://host/People?$expand={0}/DirectReports&$skiptoken=2']", ManagerTypeName),
                        String.Format("/atom:feed/atom:entry/atom:link/adsm:inline/atom:feed/atom:link[@rel='next' and @href='http://host/People(2)/{0}/DirectReports?$skiptoken=4']", ManagerTypeName) },
                    Version = testVersion,
                }
            };

            try
            {
                SendRequestAndVerifyXPaths(testCases);
            }
            finally
            {
                foreach (var service in services)
                {
                    service.PageSizeCustomizer = null;
                };
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void InvalidUriPathTests()
        {
            var errorCases = new ErrorCase[] {
                    new ErrorCase() {
                        RequestUri = "/People(3)/" + ManagerTypeName,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", ManagerTypeName) },
                    new ErrorCase() {
                        RequestUri = "/People(3)/" + ManagerTypeName + "(3)",
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestUriProcessor_SyntaxError") },
                    new ErrorCase() { 
                        RequestUri = "/People(2)/DirectReports",
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "DirectReports") },
                    new ErrorCase() {
                        RequestUri = "/People(2)/" + EmployeeTypeName + "/DirectReports",
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "DirectReports") },
                    new ErrorCase() {
                        RequestUri = String.Format("/People/{0}/Manager/{0}", EmployeeTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestUriProcessor_CannotQueryCollections", EmployeeTypeName) },
                };

            VerifyInvalidRequest(errorCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void InvalidOtherUriTests()
        {
            var errorCases = new ErrorCase[] {
#region invalid expand scenarios
                    new ErrorCase() {
                        RequestUri = String.Format("/People?$expand={0}", EmployeeTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = ODataLibResourceUtil.GetString("MetadataBinder_PropertyNotDeclared", PeopleTypeName, EmployeeTypeName) },
#endregion
#region invalid orderby scenarios
                    new ErrorCase() {
                        RequestUri = String.Format("/People?$orderby={0}", EmployeeTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = ODataLibResourceUtil.GetString("MetadataBinder_OrderByExpressionNotSingleValue") },
                    new ErrorCase() {
                        RequestUri = String.Format("/People?$orderby={0}/Manager/{0}", EmployeeTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = ODataLibResourceUtil.GetString("MetadataBinder_OrderByExpressionNotSingleValue") },
                    new ErrorCase() {
                        RequestUri = String.Format("/People?$orderby={0}/Manager/{0}", ManagerTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = ODataLibResourceUtil.GetString("MetadataBinder_OrderByExpressionNotSingleValue") },
                    new ErrorCase() {
                        RequestUri = String.Format("/People?$orderby={0}/DirectReports/{0}/Manager", ManagerTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = ODataLibResourceUtil.GetString("MetadataBinder_PropertyAccessSourceNotSingleValue", "Manager") },
#endregion
#region invalid filter scenarios
                    //new ErrorCase() {
                    //    RequestUri = String.Format("/People?$filter={0} eq null", EmployeeTypeName),
                    //    StatusCode = HttpStatusCode.BadRequest,
                    //    ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryParametersPathCannotEndInTypeIdentifier", "$filter", EmployeeTypeName) },
                    //new ErrorCase() {
                    //    RequestUri = String.Format("/People?$filter={0}/Manager/{0} eq null", EmployeeTypeName),
                    //    StatusCode = HttpStatusCode.BadRequest,
                    //    ErrorMessage = DataServicesResourceUtil.GetString("RequestUriProcessor_InvalidTypeIdentifier_MustBeASubType", EmployeeTypeName, ManagerTypeName) },
                    //new ErrorCase() {
                    //    RequestUri = String.Format("/People?$filter={0}/Manager/{0} eq null", ManagerTypeName),
                    //    StatusCode = HttpStatusCode.BadRequest,
                    //    ErrorMessage = DataServicesResourceUtil.GetString("RequestQueryProcessor_QueryParametersPathCannotEndInTypeIdentifier", "$filter", ManagerTypeName) },
                    new ErrorCase() {
                        RequestUri = String.Format("/People?$filter={0}/DirectReports/Name eq 'Shyam'", ManagerTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = ODataLibResourceUtil.GetString("MetadataBinder_PropertyAccessSourceNotSingleValue", "Name") },
                    new ErrorCase() {
                        RequestUri = String.Format("/People?$filter={0}/DirectReports/{0}/{0}/any()", ManagerTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = DataServicesResourceUtil.GetString("RequestUriProcessor_TypeIdentifierCannotBeSpecifiedAfterTypeIdentifier", ManagerTypeName, ManagerTypeName) },
                    new ErrorCase() {
                        RequestUri = String.Format("/People?$filter={0}/DirectReports/{0}", ManagerTypeName),
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessage = ODataLibResourceUtil.GetString("MetadataBinder_FilterExpressionNotSingleValue") },
#endregion
#region invalid POST scenarios
                    // ODataLib was fixed and reports missing type name as an annotation.
                    new ErrorCase() { // Type Name must be specified if the resource type has derived types - Atom Format
                        RequestUri = "/People/" + EmployeeTypeName,
                        StatusCode = HttpStatusCode.BadRequest,
                        HttpMethodNames = new string[] { "POST" },
                        ErrorMessage = DataServicesResourceUtil.GetString("BadRequest_TypeInformationMustBeSpecifiedForInhertiance"),
                        Payload = PayloadGenerator.Generate(
                            new PayloadBuilder()
                                .AddProperty("ID", 10)
                                .AddProperty("Name", "Sarah"),
                            UnitTestsUtil.AtomFormat),
                        RequestContentType = UnitTestsUtil.AtomFormat },
                    // ODataLib was fixed and reports missing type name as an annotation.
                    new ErrorCase() { // Type Name must be specified if the resource type has derived types - Json Format
                        RequestUri = "/People/" + EmployeeTypeName,
                        StatusCode = HttpStatusCode.BadRequest,
                        HttpMethodNames = new string[] { "POST" },
                        ErrorMessage = DataServicesResourceUtil.GetString("BadRequest_TypeInformationMustBeSpecifiedForInhertiance"),
                        Payload = PayloadGenerator.Generate(
                            new PayloadBuilder()
                                .AddProperty("ID", 10)
                                .AddProperty("Name", "Sarah"),
                            UnitTestsUtil.JsonLightMimeType),
                        RequestContentType = UnitTestsUtil.JsonLightMimeType },
                    new ErrorCase() { // Type Name specified must be assignable to the type specified in the uri - Atom Format
                        RequestUri = "/People/" + ManagerTypeName,
                        StatusCode = HttpStatusCode.BadRequest,
                        HttpMethodNames = new string[] { "POST" },
                        ErrorMessage = DataServicesResourceUtil.GetString("BadRequest_InvalidTypeSpecified", EmployeeTypeName, ManagerTypeName),
                        Payload = PayloadGenerator.Generate(
                            new PayloadBuilder() { TypeName = EmployeeTypeName }
                                .AddProperty("ID", 10)
                                .AddProperty("Name", "Sarah"),
                            UnitTestsUtil.AtomFormat),
                        RequestContentType = UnitTestsUtil.AtomFormat },
                    new ErrorCase() { // Type Name specified must be assignable to the type specified in the uri - Json Format
                        RequestUri = "/People/" + ManagerTypeName,
                        StatusCode = HttpStatusCode.BadRequest,
                        HttpMethodNames = new string[] { "POST" },
                        ErrorMessage = DataServicesResourceUtil.GetString("BadRequest_InvalidTypeSpecified", EmployeeTypeName, ManagerTypeName),
                        Payload = PayloadGenerator.Generate(
                            new PayloadBuilder() { TypeName = EmployeeTypeName }
                                .AddProperty("ID", 10)
                                .AddProperty("Name", "Sarah"),
                            UnitTestsUtil.JsonLightMimeType),
                        RequestContentType = UnitTestsUtil.JsonLightMimeType },
#endregion
                };

            VerifyInvalidRequest(errorCases);
        }

        [TestCategory("Partition2"), TestMethod, Description("Having open and declared property with the same name in derived types was giving wrong results")]
        public void DerivedProperty_OpenAndDeclaredPropertyWithSameName()
        {
            var metadata = GetModel(true /*openType*/, false /*namedStreams*/, metadataModifier: (m) =>
            {
                var fullNameProperty = new ResourceProperty("FullName", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))) { CanReflectOnInstanceTypeProperty = false };
                m.GetResourceType("CustomerType").AddProperty(fullNameProperty);
            });
            var serviceWithOpenTypes = new DSPUnitTestServiceDefinition(metadata, DSPDataProviderKind.CustomProvider, GetDefaultData(metadata));
            var testVersion = new ServiceVersions();

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    Services = new DSPUnitTestServiceDefinition[] { serviceWithOpenTypes },
                    RequestUris = new string[] { String.Format("/People?$select=ID,{0}/FullName", EmployeeTypeName) }, // project full name for employees
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID' and local-name()!=FullName])=0", // only ID and FullName properties must be projected for all entries
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term!='#{0}' and atom:category/@term!='#{1}']/atom:content/adsm:properties[count(*)!=1])=0", EmployeeTypeName, ManagerTypeName), // People and Customer should have only one property i.e. ID
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term!='#{0}' and atom:category/@term!='#{1}']/atom:content/adsm:properties/*[local-name()!='ID'])=0", EmployeeTypeName, ManagerTypeName), // People and Customershould have only one property i.e. ID
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}' or @term='3{1}']]/atom:content/adsm:properties[count(*)!=2])=0", EmployeeTypeName, ManagerTypeName), // Employee and Manager should have only 2 properties i.e. ID and FullName
                    },
                    Version = testVersion,
                },
                new DerivedPropertyTestCase() {
                    Services = new DSPUnitTestServiceDefinition[] { serviceWithOpenTypes },
                    RequestUris = new string[] { "/People?$select=ID,FullName" },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID' and local-name()!=FullName])=0", // all entries should have FullName and ID projected out
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties[count(*)!=2])=0", // all entries should have only 2 properties
                    },
                    Version = testVersion,
                }
            };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPropertyInsertTests()
        {
            var testVersion = new ServiceVersions();

            #region POST to Top Level Entity Set (With Type Identifier and most derived type) - type name is optional
            {
                var payloadGenerators = new PayloadBuilder[] {
                    new PayloadBuilder()
                        .AddProperty("ID", 10)
                        .AddProperty("Name", "Anthony")
                        .AddNavigationReferenceProperty("Manager", new PayloadBuilder() { Uri = "/People(5)" }),
                    new PayloadBuilder() { TypeName = ManagerTypeName }
                        .AddProperty("ID", 10)
                        .AddProperty("Name", "Anthony")
                        .AddNavigationReferenceProperty("Manager", new PayloadBuilder() { Uri = "/People(5)" })
                };

                var testCases = new DerivedPropertyTestCase[] {
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] { String.Format("/People/{0}", ManagerTypeName), },
                        XPaths = new string[] { "/atom:entry[atom:category/@term='#" + ManagerTypeName + "' and contains(atom:id, 'People(10)') and atom:content/adsm:properties[ads:ID='10' and ads:Name='Anthony']]" },
                        HttpMethods = new string[] { "POST" },
                        StatusCode = 201,
                        Version = testVersion,
                    }
                };

                SendRequestAndVerifyXPaths(testCases, payloads: payloadGenerators);
            }
            #endregion POST to Top Level Entity Set (With Type Identifier and most derived type)

            #region POST to Top Level Entity Set (With Type Identifier and not most derived type) - type name is required
            {
                var payloadGenerators = new PayloadBuilder[] {
                    new PayloadBuilder() { TypeName = EmployeeTypeName }
                        .AddProperty("ID", 10)
                        .AddProperty("Name", "Sarah")
                        .AddNavigationReferenceProperty("Manager", new PayloadBuilder() { Uri = "/People(2)" })
                };

                var testCases = new DerivedPropertyTestCase[] {
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] { String.Format("/People/{0}", EmployeeTypeName), },
                        XPaths = new string[] { "/atom:entry[atom:category/@term='#" + EmployeeTypeName + "' and contains(atom:id, 'People(10)') and atom:content/adsm:properties[ads:ID='10' and ads:Name='Sarah']]" },
                        HttpMethods = new string[] { "POST" },
                        StatusCode = 201,
                        Version = testVersion,
                    }
                };

                SendRequestAndVerifyXPaths(testCases, payloads: payloadGenerators);
            }
            #endregion POST to Top Level Entity Set (With Type Identifier and not most derived type)

            #region POST to Navigation Collection Property
            {
                var payloadGenerators = new PayloadBuilder[] {
                    new PayloadBuilder() { TypeName = EmployeeTypeName }
                        .AddProperty("ID", 10)
                        .AddProperty("Name", "Sarah")
                        .AddNavigationReferenceProperty("Manager", new PayloadBuilder() { Uri = "/People(2)" }),
                    new PayloadBuilder() { TypeName = EmployeeTypeName }
                        .AddProperty("ID", 10)
                        .AddProperty("Name", "Sarah")
                        .AddNavigationReferenceProperty("Manager", new PayloadBuilder() { Uri = "/People(2)" })
                };

                var testCases = new DerivedPropertyTestCase[] {
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] {
                            String.Format("/People(2)/{0}/DirectReports", ManagerTypeName),
                            String.Format("/People(2)/{0}/DirectReports/{1}", ManagerTypeName, EmployeeTypeName) },
                        XPaths = new string[] { "/atom:entry[atom:category/@term='#" + EmployeeTypeName + "' and contains(atom:id, 'People(10)') and atom:content/adsm:properties[ads:ID='10' and ads:Name='Sarah']]" },
                        HttpMethods = new string[] { "POST" },
                        StatusCode = 201,
                        Version = testVersion,
                    }
                };

                SendRequestAndVerifyXPaths(testCases, payloads: payloadGenerators);
            }
            #endregion POST to Navigation Collection Property
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPropertyUpdateTests()
        {
            #region Update Requests to Top Level Entity (With Type Identifier and most derived type) - specifying the type name is optional
            {
                var payloadGenerators = new PayloadBuilder[] {
                    new PayloadBuilder()
                        .AddProperty("Name", "Anthony")
                        .AddProperty("Manager", new PayloadBuilder() { Uri = "/People(5)" }),
                    new PayloadBuilder() { TypeName = ManagerTypeName }
                        .AddProperty("Name", "Anthony")
                        .AddProperty("Manager", new PayloadBuilder() { Uri = "/People(5)" }),
                };

                var testCases = new DerivedPropertyTestCase[] {
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] { String.Format("/People(2)/{0}", ManagerTypeName) },
                        HttpMethods = new string[] { "PATCH" },
                        StatusCode = 204,
                        MinProtocolVersion = ODataProtocolVersion.V4,
                        VerifyXPathsByQuery = new DerivedPropertyTestCase[] {
                            new DerivedPropertyTestCase() {
                                RequestUris = new string [] { String.Format("/People(2)/{0}", ManagerTypeName) },
                                XPaths = new string[] { "/atom:entry[atom:category/@term='#" + ManagerTypeName + "' and contains(atom:id, 'People(2)') and atom:content/adsm:properties[ads:ID='2' and ads:Name='Anthony']]" } } }
                    },
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] { String.Format("/People(2)/{0}", ManagerTypeName) },
                        HttpMethods = new string[] { "PATCH" },
                        StatusCode = 204,
                        Version = new ServiceVersions(),
                        VerifyXPathsByQuery = new DerivedPropertyTestCase[] {
                            new DerivedPropertyTestCase() {
                                RequestUris = new string [] { String.Format("/People(2)/{0}", ManagerTypeName) },
                                XPaths = new string[] { "/atom:entry[atom:category/@term='#" + ManagerTypeName + "' and contains(atom:id, 'People(2)') and atom:content/adsm:properties[ads:ID='2' and ads:Name='Anthony']]" } } }
                    }
                };

                SendRequestAndVerifyXPaths(testCases, payloads: payloadGenerators);
            }
            #endregion Update Requests to Top Level Entity (With Type Identifier and most derived type)

            #region Update Requests to Top Level Entity (With Type Identifier and not most derived type) - must specify the type name
            {
                var payloadGenerators = new PayloadBuilder[] {
                    new PayloadBuilder() { TypeName = EmployeeTypeName }
                        .AddProperty("Name", "Sarah")
                        .AddProperty("Manager", new PayloadBuilder() { Uri = "/People(2)" })
                };

                var testCases = new DerivedPropertyTestCase[] {
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] { String.Format("/People(3)/{0}", EmployeeTypeName) },
                        HttpMethods = new string[] { "PATCH"},
                        StatusCode = 204,
                        MinProtocolVersion = ODataProtocolVersion.V4,
                        VerifyXPathsByQuery = new DerivedPropertyTestCase[] {
                            new DerivedPropertyTestCase() {
                                RequestUris = new string [] { String.Format("/People(3)/{0}", EmployeeTypeName) },
                                XPaths = new string[] { "/atom:entry[atom:category/@term='#" + EmployeeTypeName + "' and contains(atom:id, 'People(3)') and atom:content/adsm:properties[ads:ID='3' and ads:Name='Sarah']]" } } }
                    },
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] { String.Format("/People(3)/{0}", EmployeeTypeName) },
                        HttpMethods = new string[] { "PATCH" },
                        StatusCode = 204,
                        Version = new ServiceVersions(),
                        VerifyXPathsByQuery = new DerivedPropertyTestCase[] {
                            new DerivedPropertyTestCase() {
                                RequestUris = new string [] { String.Format("/People(3)/{0}", EmployeeTypeName) },
                                XPaths = new string[] { "/atom:entry[atom:category/@term='#" + EmployeeTypeName + "' and contains(atom:id, 'People(3)') and atom:content/adsm:properties[ads:ID='3' and ads:Name='Sarah']]" } } }
                    }
                };

                SendRequestAndVerifyXPaths(testCases, payloads: payloadGenerators);
            }
            #endregion Update Requests to Top Level Entity (With Type Identifier and not most derived type)

            #region Update Requests to Navigation Collection Property
            {
                var payloadGenerators = new PayloadBuilder[] {
                     new PayloadBuilder() { TypeName = EmployeeTypeName }
                        .AddProperty("Name", "Sarah")
                        .AddProperty("Manager", new PayloadBuilder() { Uri = "/People(2)" }),
                };

                var testCases = new DerivedPropertyTestCase[] {
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] {
                            String.Format("/People(2)/{0}/DirectReports(3)", ManagerTypeName),
                            String.Format("/People(2)/{0}/DirectReports(3)/{1}", ManagerTypeName, EmployeeTypeName) },
                        HttpMethods = new string[] { "PATCH" },
                        StatusCode = 204,
                        MinProtocolVersion = ODataProtocolVersion.V4,
                        VerifyXPathsByQuery = new DerivedPropertyTestCase[] {
                            new DerivedPropertyTestCase() {
                                RequestUris = new string [] { "/People(3)" },
                                XPaths = new string[] { "/atom:entry[atom:category/@term='#" + EmployeeTypeName + "' and contains(atom:id, 'People(3)') and atom:content/adsm:properties[ads:ID='3' and ads:Name='Sarah']]" } } }
                    },
                    new DerivedPropertyTestCase()
                    {
                        RequestUris = new string[] {
                            String.Format("/People(2)/{0}/DirectReports(3)", ManagerTypeName),
                            String.Format("/People(2)/{0}/DirectReports(3)/{1}", ManagerTypeName, EmployeeTypeName) },
                        HttpMethods = new string[] { "PATCH" },
                        StatusCode = 204,
                        Version = new ServiceVersions(),
                        VerifyXPathsByQuery = new DerivedPropertyTestCase[] {
                            new DerivedPropertyTestCase() {
                                RequestUris = new string [] { "/People(3)" },
                                XPaths = new string[] { "/atom:entry[atom:category/@term='#" + EmployeeTypeName + "' and contains(atom:id, 'People(3)') and atom:content/adsm:properties[ads:ID='3' and ads:Name='Sarah']]" } } }
                    }
                };

                SendRequestAndVerifyXPaths(testCases, payloads: payloadGenerators);
            }
            #endregion Update Requests to Top Level Entity (With Type Identifier and not most derived type)
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedPropertyDeleteTests()
        {
            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People(3)/{0}", EmployeeTypeName) },
                    HttpMethods = new string[] { "DELETE"},
                    StatusCode = 204,
                    VerifyXPathsByQuery = new DerivedPropertyTestCase[] {
                            new DerivedPropertyTestCase() {
                                RequestUris = new string [] { "/People" },
                                XPaths = new string[] { "count(/atom:feed/atom:entry)=6" } } },
                    MinProtocolVersion = ODataProtocolVersion.V4,
                }
            };

            SendRequestAndVerifyXPaths(testCases, formats: new string[] { "application/atom+xml,application/xml" });
        }

        [TestCategory("Partition2"), TestMethod]
        public void DerivedNamedStreamPropertyTests()
        {
            var testVersion = new ServiceVersions();
            var namedStreamService = GetUnitTestServiceDefinition(DSPDataProviderKind.CustomProvider, false, true);
            var namedStreamServiceWithSDP = GetUnitTestServiceDefinition(DSPDataProviderKind.CustomProvider, false, true);
            namedStreamServiceWithSDP.PageSizeCustomizer = (config, serviceType) => { config.SetEntitySetPageSize("People", 2); };

            var photoLinkRelValue = "http://docs.oasis-open.org/odata/ns/edit-media/Photo";

            var testCases = new DerivedPropertyTestCase[] {
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People?$select={0}/Photo", ManagerTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + 2,
                        String.Format("//atom:link[@rel='next' and @href='http://host/People?$select={0}/Photo&$skiptoken=2']", ManagerTypeName),
                    },
                    Version = testVersion,
                    Services = new DSPUnitTestServiceDefinition[] { namedStreamServiceWithSDP },
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] {
                        String.Format("/People?$select={0}/Photo", ManagerTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        String.Format("count(//atom:entry/adsm:properties[count(*)!=0])=0", ManagerTypeName), // none of the entries should have the any properties serialized
                        String.Format("count(//atom:entry[atom:category/@term='#{0}']/atom:link[@rel='{1}'])=2", ManagerTypeName, photoLinkRelValue), // Manager types should have named streams link serialized
                        String.Format("count(//atom:entry[atom:category/@term!='#{0}']/atom:link[@rel='{1}'])=0", ManagerTypeName, photoLinkRelValue),  // Non manager types should not have any named streams link serialized
                    },
                    Version = testVersion,
                    Services = new DSPUnitTestServiceDefinition[] { namedStreamService },
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID,{0}/Photo,{1}/Photo", CustomerTypeName, EmployeeTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=2",
                        String.Format("//atom:link[@rel='next' and @href='http://host/People?$select=ID,{0}/Photo,{1}/Photo&$skiptoken=2']", CustomerTypeName, EmployeeTypeName),
                    },
                    Version = testVersion,
                    Services = new DSPUnitTestServiceDefinition[] { namedStreamServiceWithSDP },
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID,{0}/Photo,{1}/Photo", CustomerTypeName, EmployeeTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID'])=0", // only ID properties are projected for all entries
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term='#{0}']/atom:link[@rel='{1}'])=0", PeopleTypeName, photoLinkRelValue), // people type should not have the named stream link serialized
                        String.Format("count(/atom:feed/atom:entry[atom:category/@term!='#{0}']/atom:link[@rel='{1}'])={2}", PeopleTypeName, photoLinkRelValue, PeopleSetCount - 1), // all non-people should have the named stream link serialized
                    },
                    Version = testVersion,
                    Services = new DSPUnitTestServiceDefinition[] { namedStreamService },
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID,{0}/Photo,{1}/Photo", CustomerTypeName, ManagerTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=2",
                        String.Format("//atom:link[@rel='next' and @href='http://host/People?$select=ID,{0}/Photo,{1}/Photo&$skiptoken=2']", CustomerTypeName, ManagerTypeName),
                    },
                    Version = testVersion,
                    Services = new DSPUnitTestServiceDefinition[] { namedStreamServiceWithSDP },
                },
                new DerivedPropertyTestCase() {
                    RequestUris = new string[] { String.Format("/People?$select=ID,{0}/Photo,{1}/Photo", CustomerTypeName, ManagerTypeName) },
                    XPaths = new string[] {
                        "count(/atom:feed/atom:entry)=" + PeopleSetCount,
                        "count(/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID'])=0", // only ID properties are projected for all entries
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term='#{0}' or @term='3{1}']]/atom:link[@rel='{2}'])=0", PeopleTypeName, EmployeeTypeName, photoLinkRelValue), // people and employee type should not have the named stream link serialized
                        String.Format("count(/atom:feed/atom:entry[atom:category[@term!='{0}' and @term!='{1}']]/atom:link[@rel='{2}'])=3", PeopleTypeName, EmployeeTypeName, photoLinkRelValue), // all non-people should have the named stream link serialized
                    },
                    Version = testVersion,
                    Services = new DSPUnitTestServiceDefinition[] { namedStreamService },
                },
            };

            SendRequestAndVerifyXPaths(testCases);
        }

        [TestCategory("Partition2"), TestMethod, Description("Verify that the OData-EntityId header is generated from the id, not the edit link. These two values differ under derived type scenarios where the edit link has a type segment.")]
        public void DataServiceIdShouldNotContainTypeSegment()
        {
            var payloadBuilder = new PayloadBuilder() { TypeName = ManagerTypeName }.AddProperty("ID", 10).AddProperty("Name", "Anthony");
            var service = services[(int)DSPDataProviderKind.CustomProvider];

            using (service.CreateChangeScope(GetDefaultData(service.Metadata)))
            using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType")) 
            using (TestWebRequest request = service.CreateForInProcess())
            {
                // Use IDataServiceHost2 so that we can we can set a header through the request.RequestHeaders dictionary
                TestWebRequest.HostInterfaceType = typeof(IDataServiceHost2);
                request.StartService();
                request.HttpMethod = "POST";
                request.RequestUriString = "/People";
                request.RequestContentType = UnitTestsUtil.AtomFormat;
                request.Accept = UnitTestsUtil.AtomFormat;
                request.RequestHeaders["Prefer"] = "return=minimal";
                request.SetRequestStreamAsText(PayloadGenerator.Generate(payloadBuilder, UnitTestsUtil.AtomFormat));
                request.SendRequest();

                string locationHeader = request.ResponseHeaders["Location"];
                string odataEntityIdHeader = request.ResponseHeaders["OData-EntityId"];

                // Some asserts to verify this test is set up correctly
                Assert.IsNotNull(locationHeader);
                Assert.IsNotNull(odataEntityIdHeader);
                Assert.IsTrue(locationHeader.EndsWith(ManagerTypeName), "Expected the location header (which should come from the edit link) to end with a type segment.");

                // The value of the "Location" header should come from the edit link (which, in this case, should have a type segment). The value of the "OData-EntityId" header should come from the ID, which will never have a type segment.
                // Verify that OData-EntityId isn't coming from the edit link by checking that OData-EntityId is not the same as Location when Location has a type segment.
                Assert.AreNotEqual(odataEntityIdHeader, locationHeader);
            }
        }

        [TestCategory("Partition2"), TestMethod, Description("Verify that the feed with derived types work with no metadata option")]
        public void NoMetadataOptionShouldWorkForDerivedTypes()
        {
            var service = services[(int)DSPDataProviderKind.CustomProvider];
            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.StartService();
                request.HttpMethod = "GET";
                request.RequestUriString = "/People";
                request.Accept = UnitTestsUtil.JsonLightMimeTypeNoMetadata;
                request.SendRequest();

                Assert.AreEqual(request.ResponseStatusCode, 200, "Request must succeed");
                var response = request.GetResponseStreamAsXmlDocument(UnitTestsUtil.JsonLightMimeType);
                UnitTestsUtil.VerifyXPathExists(response, String.Format("/{0}/value/{1}[count({0})=7]", JsonValidator.ObjectString, JsonValidator.ArrayString));
            }
        }

        private static void SendRequestAndVerifyXPaths(DerivedPropertyTestCase[] testCases, string[] formats = null, PayloadBuilder[] payloads = null)
        {
            var version3 = new[] { ODataProtocolVersion.V4 };
            test.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                test.TestUtil.RunCombinations(
                    testCase.Services ?? services,
                    version3,
                    testCase.HttpMethods,
                    testCase.RequestUris,
                    formats ?? UnitTestsUtil.ResponseFormats,
                    payloads ?? new PayloadBuilder[1] { null },
                    version3,
                    version3,
                    (service, protocolVersion, httpMethod, requestUri, format, payload, requestDSV, requestMDSV) =>
                    {
                        // Currently, IUpdatable is not implemented for ReflectionProvider, hence skipping it for now
                        if (httpMethod != "GET" && service.ProviderKind == DSPDataProviderKind.Reflection)
                        {
                            return;
                        }

                        if (testCase.MinProtocolVersion > protocolVersion)
                        {
                            return;
                        }

                        // If the MPV is V2, we need to ignore requests with request DSV or MDSV greater than v2
                        if (requestDSV > protocolVersion || requestMDSV > protocolVersion)
                        {
                            return;
                        }

                        JsonToAtomUtil jsonToAtomUtil = new JsonToAtomUtil(service.Metadata);
                        using (OpenWebDataServiceHelper.MaxProtocolVersion.Restore())
                        using (TestWebRequest request = service.CreateForInProcess())
                        {
                            OpenWebDataServiceHelper.MaxProtocolVersion.Value = protocolVersion;
                            request.StartService();
                            request.HttpMethod = httpMethod;
                            request.RequestUriString = requestUri;
                            request.Accept = format;
                            request.RequestVersion = new ServiceVersion(requestDSV).ToString();
                            request.RequestMaxVersion = new ServiceVersion(requestMDSV).ToString();
                            if (payload != null)
                            {
                                request.RequestContentType = format;
                                request.SetRequestStreamAsText(PayloadGenerator.Generate(payload, format));
                            }

                            IDisposable dispose = null;
                            if (httpMethod != "GET")
                            {
                                dispose = service.CreateChangeScope(GetDefaultData(service.Metadata));
                            }

                            try
                            {
                                Exception exception = System.Data.Test.Astoria.TestUtil.RunCatching(request.SendRequest);

                                if (IsValidScenario(request, protocolVersion, testCase.Version, new ServiceVersion(requestDSV), new ServiceVersion(requestMDSV), exception))
                                {
                                    if (testCase.XPaths != null)
                                    {
                                        var xPaths = testCase.XPaths;
                                        UnitTestsUtil.VerifyXPaths(UnitTestsUtil.GetResponseAsAtom(request, jsonToAtomUtil), xPaths);
                                    }

                                    Assert.AreEqual(testCase.StatusCode, request.ResponseStatusCode, "The status code is not as expected");
                                    if (testCase.VerifyXPathsByQuery != null)
                                    {
                                        foreach (var queryTestCase in testCase.VerifyXPathsByQuery)
                                            foreach (var queryUri in queryTestCase.RequestUris)
                                            {
                                                request.RequestUriString = queryUri;
                                                request.HttpMethod = "GET";
                                                request.Accept = format;
                                                request.RequestMaxVersion = "4.0";
                                                exception = test.TestUtil.RunCatching(request.SendRequest);
                                                Assert.IsNull(exception, "No exception expected, but exception was thrown");
                                                UnitTestsUtil.VerifyXPaths(UnitTestsUtil.GetResponseAsAtom(request, jsonToAtomUtil), queryTestCase.XPaths);
                                            }
                                    }
                                }
                            }
                            finally
                            {
                                if (dispose != null) dispose.Dispose();
                            }
                        }
                    });
            });
        }

        private static bool IsValidScenario(TestWebRequest request, ODataProtocolVersion protocolVersion, ServiceVersions versions, ServiceVersion requestDSV, ServiceVersion requestMDSV, Exception exception)
        {
            // figure out the default values of the version if they are null
            if (requestDSV == null && requestMDSV != null)
            {
                requestDSV = requestMDSV.Version > 20 ? requestMDSV : new ServiceVersion(40);
            }

            if (exception != null)
            {
                Assert.AreEqual(400, request.ResponseStatusCode, "Expecting bad request");
                List<string> expectedErrorMessages = new List<string>();
                if (requestDSV != null && requestDSV.Version < versions.MinRequestDSV.Version)
                {
                    expectedErrorMessages.Add(DataServicesResourceUtil.GetString("DataService_DSVTooLow", requestDSV.ToString(), versions.MinRequestDSV.Version / 10, versions.MinRequestDSV.Version % 10));
                }
                if (requestMDSV != null && requestMDSV.Version < versions.MinRequestMDSV.Version)
                {
                    expectedErrorMessages.Add(DataServicesResourceUtil.GetString("DataService_MaxDSVTooLow", requestMDSV.ToString(), versions.MinRequestMDSV.Version / 10, versions.MinRequestMDSV.Version % 10));
                }
                // Since enableRelationshipLinks is true, the response will be 3.0 if the MPV is set to 3.0. For V1/V2 servers, this setting is ignored
                if (protocolVersion == ODataProtocolVersion.V4 && requestMDSV != null && requestMDSV.ToProtocolVersion() < protocolVersion)
                {
                    expectedErrorMessages.Add(DataServicesResourceUtil.GetString("DataService_MaxDSVTooLow", requestMDSV.ToString(), 4, 0));
                }

                if (expectedErrorMessages.Count != 0)
                {
                    Assert.IsTrue(expectedErrorMessages.Contains(exception.InnerException.Message), "Didn't find the expected error message");
                }
                else
                {
                    Assert.Fail("Exception not expected, but exception was thrown");
                }

                return false;
            }

            Assert.IsTrue(requestDSV == null || requestDSV.Version >= versions.MinRequestDSV.Version, "Request DSV should be equal or greater than the min required DSV");
            Assert.IsTrue(requestMDSV == null || requestMDSV.Version >= versions.MinRequestMDSV.Version, "Request MDSV should be equal or greater than the min required MDSV");

            return true;
        }

        private static void VerifyInvalidRequest(ErrorCase[] errorCases)
        {
            using (TestWebRequest request = services[1].CreateForInProcess())
            {
                request.StartService();

                test.TestUtil.RunCombinations(errorCases, (error) =>
                {
                    string[] methodNames = error.HttpMethodNames == null ? new string[] { "GET" } : error.HttpMethodNames;

                    foreach (var httpMethodName in methodNames)
                    {
                        request.RequestUriString = error.RequestUri;
                        request.Accept = UnitTestsUtil.MimeAny;
                        request.HttpMethod = httpMethodName;
                        if (error.Payload != null)
                        {
                            request.SetRequestStreamAsText(error.Payload);
                            request.RequestContentType = error.RequestContentType;
                        }

                        Exception exception = test.TestUtil.RunCatching(request.SendRequest);
                        UnitTestsUtil.VerifyTestException(exception, (int)error.StatusCode, error.ErrorMessage);
                    }
                });
            }
        }

        private static DSPMetadata GetModel(bool openType, bool namedStreams, Action<DSPMetadata> metadataModifier = null)
        {
            #region Model Definition
            // Navigation Collection Property: Client - Entity, Server - NonEntity
            DSPMetadata metadata = new DSPMetadata("ModelWithDerivedNavProperties", "AstoriaUnitTests.Tests");

            var peopleType = metadata.AddEntityType("PeopleType", null, null, false);
            peopleType.IsOpenType = openType;
            metadata.AddKeyProperty(peopleType, "ID", typeof(int));
            if (!openType)
            {
                metadata.AddPrimitiveProperty(peopleType, "Name", typeof(string));
            }

            var peopleSet = metadata.AddResourceSet("People", peopleType);

            var employeeType = metadata.AddEntityType("EmployeeType", null, peopleType, false);
            if (namedStreams)
            {
                metadata.AddNamedStreamProperty(employeeType, "Photo");
            }

            ResourceProperty fullNameProperty = null;
            if (!openType)
            {
                fullNameProperty = new ResourceProperty("FullName", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string))) { CanReflectOnInstanceTypeProperty = false };
                employeeType.AddProperty(fullNameProperty);
            }

            var managerType = metadata.AddEntityType("ManagerType", null, employeeType, false);

            var customerType = metadata.AddEntityType("CustomerType", null, peopleType, false);
            if (namedStreams)
            {
                metadata.AddNamedStreamProperty(customerType, "Photo");
            }

            if (!openType)
            {
                Assert.IsTrue(fullNameProperty != null, "fullNameProperty != null");
                customerType.AddProperty(fullNameProperty);
            }

            var baseAddressType = metadata.AddEntityType("AddressType", null, null, false);
            baseAddressType.IsOpenType = openType;
            metadata.AddKeyProperty(baseAddressType, "ID", typeof(int));
            if (!openType)
            {
                metadata.AddPrimitiveProperty(baseAddressType, "Street", typeof(string));
                metadata.AddPrimitiveProperty(baseAddressType, "City", typeof(string));
                metadata.AddPrimitiveProperty(baseAddressType, "State", typeof(string));
                metadata.AddPrimitiveProperty(baseAddressType, "ZipCode", typeof(string));
            }

            var customerAddressType = metadata.AddEntityType("CustomerAddressType", null, baseAddressType, false);
            var employeeAddressType = metadata.AddEntityType("EmployeeAddressType", null, baseAddressType, false);

            var addressSet = metadata.AddResourceSet("Addresses", baseAddressType);

            var drProperty = metadata.AddResourceSetReferenceProperty(managerType, "DirectReports", employeeType);
            var managerProperty = metadata.AddResourceReferenceProperty(employeeType, "Manager", managerType);

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "Manager_DirectReports",
                new ResourceAssociationSetEnd(peopleSet, employeeType, managerProperty),
                new ResourceAssociationSetEnd(peopleSet, managerType, drProperty)));

            var customerAddressProperty = metadata.AddResourceReferenceProperty(customerType, "Address", customerAddressType);
            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "Customer_Address",
                new ResourceAssociationSetEnd(peopleSet, customerType, customerAddressProperty),
                new ResourceAssociationSetEnd(addressSet, customerAddressType, null)));

            var employeeAddressProperty = metadata.AddResourceReferenceProperty(employeeType, "Address", employeeAddressType);
            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "Employee_Address",
                new ResourceAssociationSetEnd(peopleSet, employeeType, employeeAddressProperty),
                new ResourceAssociationSetEnd(addressSet, employeeAddressType, null)));

            if (metadataModifier != null)
            {
                metadataModifier(metadata);
            }

            metadata.SetReadOnly();
            #endregion Model Definition

            return metadata;
        }

        private static DSPContext GetDefaultData(DSPMetadata metadata)
        {
            var peopleType = metadata.GetResourceType("PeopleType");
            var employeeType = metadata.GetResourceType("EmployeeType");
            var managerType = metadata.GetResourceType("ManagerType");
            var customerType = metadata.GetResourceType("CustomerType");
            var employeeAddressType = metadata.GetResourceType("EmployeeAddressType");
            var customerAddressType = metadata.GetResourceType("CustomerAddressType");

            #region Default Data for the Model
            var context = new DSPContext();

            DSPResource people1 = new DSPResource(peopleType);
            people1.SetValue("ID", 1);
            people1.SetValue("Name", "Foo");

            DSPResource andyAddress = new DSPResource(employeeAddressType);
            andyAddress.SetValue("ID", 1);
            andyAddress.SetValue("Street", "Andy's address");
            andyAddress.SetValue("City", "Sammamish");
            andyAddress.SetValue("State", "WA");

            DSPResource andy = new DSPResource(managerType);
            andy.SetValue("ID", 2);
            andy.SetValue("Name", "Andy");
            andy.SetValue("FullName", "Andy Conrad");
            andy.SetValue("Address", andyAddress);

            DSPResource pratikAddress = new DSPResource(employeeAddressType);
            pratikAddress.SetValue("ID", 2);
            pratikAddress.SetValue("Street", "pratik's address");
            pratikAddress.SetValue("City", "Bothell");
            pratikAddress.SetValue("State", "WA");

            DSPResource pratik = new DSPResource(employeeType);
            pratik.SetValue("ID", 3);
            pratik.SetValue("Name", "Pratik");
            pratik.SetValue("FullName", "Pratik Patel");
            pratik.SetValue("Manager", andy);
            pratik.SetValue("Address", pratikAddress);

            DSPResource jimmyAddress = new DSPResource(employeeAddressType);
            jimmyAddress.SetValue("ID", 3);
            jimmyAddress.SetValue("Street", "jimmy's address");
            jimmyAddress.SetValue("City", "somewhere in seattle");
            jimmyAddress.SetValue("State", "WA");

            DSPResource jimmy = new DSPResource(employeeType);
            jimmy.SetValue("ID", 4);
            jimmy.SetValue("Name", "Jimmy");
            jimmy.SetValue("FullName", "Jian Li");
            jimmy.SetValue("Manager", andy);
            jimmy.SetValue("Address", jimmyAddress);

            andy.SetValue("DirectReports", new List<DSPResource>() { pratik, jimmy });

            DSPResource shyamAddress = new DSPResource(employeeAddressType);
            shyamAddress.SetValue("ID", 4);
            shyamAddress.SetValue("Street", "shyam's address");
            shyamAddress.SetValue("City", "university district");
            shyamAddress.SetValue("State", "WA");

            DSPResource shyam = new DSPResource(managerType);
            shyam.SetValue("ID", 5);
            shyam.SetValue("Name", "Shyam");
            shyam.SetValue("FullName", "Shyam Pather");
            shyam.SetValue("Address", shyamAddress);

            DSPResource marceloAddress = new DSPResource(employeeAddressType);
            marceloAddress.SetValue("ID", 5);
            marceloAddress.SetValue("Street", "marcelo's address");
            marceloAddress.SetValue("City", "kirkland");
            marceloAddress.SetValue("State", "WA");

            DSPResource marcelo = new DSPResource(employeeType);
            marcelo.SetValue("ID", 6);
            marcelo.SetValue("Name", "Marcelo");
            marcelo.SetValue("FullName", "Marcelo Lopez Ruiz");
            marcelo.SetValue("Manager", shyam);
            marcelo.SetValue("Address", marceloAddress);

            andy.SetValue("Manager", shyam);

            shyam.SetValue("DirectReports", new List<DSPResource>() { andy, marcelo });

            DSPResource customer1Address = new DSPResource(customerAddressType);
            customer1Address.SetValue("ID", 6);
            customer1Address.SetValue("Street", "customer1's address");
            customer1Address.SetValue("City", "somewhere");
            customer1Address.SetValue("State", "WA");

            var customer1 = new DSPResource(customerType);
            customer1.SetValue("ID", 7);
            customer1.SetValue("FullName", "Customer FullName");
            customer1.SetValue("Address", customer1Address);

            var people = context.GetResourceSetEntities("People");
            people.AddRange(new object[] { people1, andy, pratik, jimmy, shyam, marcelo, customer1 });

            var addresses = context.GetResourceSetEntities("Addresses");
            addresses.AddRange(new object[] { andyAddress, pratikAddress, jimmyAddress, shyamAddress, marceloAddress, customer1Address });

            #endregion Default Data for the Model

            return context;
        }

        private class ErrorCase
        {
            public string RequestUri { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public string Payload { get; set; }
            public string RequestContentType { get; set; }
            public string ErrorMessage { get; set; }
            public Type ExceptionType { get; set; }
            public string[] HttpMethodNames { get; set; }
            public override string ToString()
            {
                return this.RequestUri;
            }
        }

        private class DerivedPropertyTestCase
        {
            public DerivedPropertyTestCase()
            {
                this.StatusCode = 200;
                this.Version = new ServiceVersions();
                this.HttpMethods = new string[] { "GET" };
                this.MinProtocolVersion = ODataProtocolVersion.V4;
            }

            public DSPUnitTestServiceDefinition[] Services { get; set; }
            public string[] RequestUris { get; set; }
            public ServiceVersions Version { get; set; }
            public string[] XPaths { get; set; }
            public int StatusCode { get; set; }
            public string[] HttpMethods { get; set; }
            public DerivedPropertyTestCase[] VerifyXPathsByQuery { get; set; }
            public ODataProtocolVersion MinProtocolVersion { get; set; }
        }

        private static string[] GetLinksForPeopleEntity(string keyValue, string typeName)
        {
            return new string[] {
                 "//atom:entry" + String.Format(ManagerNavigationLinkXPath, String.Format("People({0})", keyValue), typeName),
                 "//atom:entry" + String.Format(DirectReportsNavigationLinkXPath, String.Format("People({0})", keyValue)),
                 "//atom:entry" + String.Format(AddressNavigationLinkXPath, String.Format("People({0})", keyValue), typeName),
                 "//atom:entry" + String.Format(ManagerAssociationLinkXPath, String.Format("People({0})", keyValue), typeName),
                 "//atom:entry" + String.Format(DirectReportsAssociationLinkXPath, String.Format("People({0})", keyValue)),
                 "//atom:entry" + String.Format(AddressAssociationLinkXPath, String.Format("People({0})", keyValue), typeName) };
        }

        private static string[] GetLinks(string[] xPaths1, params string[] xPaths2)
        {
            string[] array = new string[xPaths1.Length + xPaths2.Length];
            xPaths1.CopyTo(array, 0);
            xPaths2.CopyTo(array, xPaths1.Length);
            return array;
        }

        private static DSPUnitTestServiceDefinition GetUnitTestServiceDefinition(DSPDataProviderKind providerKind, bool openType, bool namedStreams)
        {
            DSPMetadata metadata = GetModel(openType, namedStreams);
            DSPContext defaultData = GetDefaultData(metadata);

            var service = new DSPUnitTestServiceDefinition(metadata, providerKind, defaultData);
            service.DataServiceBehavior.IncludeRelationshipLinksInResponse = true;
            service.Writable = true;

            if (providerKind == DSPDataProviderKind.CustomProvider)
            {
                service.SupportNamedStream = true;
                service.MediaResourceStorage = new DSPMediaResourceStorage();
            }

            return service;
        }
    }
}
