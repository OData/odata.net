//---------------------------------------------------------------------
// <copyright file="NonNullablePropertiesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Net;
    using System.Transactions;
    using System.Text;
    using test = System.Data.Test.Astoria;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NonNullablePropertiesTests
    {
        private static DSPUnitTestServiceDefinition service;
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            DSPDataProviderKind providerKind = DSPDataProviderKind.EF;
                                    
            // set up the service
            DSPMetadata metadata = GetModel(false /*openType*/, false /*namedStreams*/);
            DSPContext defaultData = GetDefaultData(metadata);

            DSPUnitTestServiceDefinition.ModifyGeneratedCode = (currentCode) => 
            {
                // Add namespace which includes Code First Attributes
                currentCode.Insert(currentCode.ToString().IndexOf("public class PeopleType"), "using System.ComponentModel.DataAnnotations;\n\n");

                // Add the required attributes
                currentCode.Insert(currentCode.ToString().IndexOf("public string Name { get; set; }"), "[Required()]\n");
                currentCode.Insert(currentCode.ToString().IndexOf("public byte[] Body { get; set; }"), "[Required()]\n");
                currentCode.Insert(currentCode.ToString().IndexOf("public int? Age { get; set; }"), "[Required()]\n");

                return currentCode;
            };

            service = new DSPUnitTestServiceDefinition(metadata, providerKind, defaultData);
            service.Writable = true;
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/875
        [Ignore] // Remove Atom
        // [TestCategory("Partition2"), TestMethod]
        public void NonNullableComplexPropertyTest()
        {
            test.TestUtil.RunCombinations(
                new DSPUnitTestServiceDefinition[] { service },
                new string[] { UnitTestsUtil.AtomFormat },
                ServiceVersion.ValidVersions, // requestDSV
                ServiceVersion.ValidVersions, // requestMDSV
                ServiceVersion.ValidVersions, // maxProtocolVersion
                (localService, format, requestDSV, requestMDSV, maxProtocolVersion) =>
                {
                    if (maxProtocolVersion == null)
                    {
                        return;
                    }

                    localService.DataServiceBehavior.MaxProtocolVersion = maxProtocolVersion.ToProtocolVersion();
                    using (TestWebRequest request = localService.CreateForInProcess())
                    {
                        if (requestDSV != null && maxProtocolVersion.ToProtocolVersion() < requestDSV.ToProtocolVersion())
                        {
                            return;
                        }

                        request.StartService();
                        request.HttpMethod = "POST";
                        request.RequestUriString = "/People";
                        request.Accept = format;
                        if (requestDSV != null) request.RequestVersion = requestDSV.ToString();
                        if (requestMDSV != null) request.RequestMaxVersion = requestMDSV.ToString();

                        request.RequestContentType = format;
                        request.SetRequestStreamAsText(@"<entry xml:base='http://host/' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
    <category term='#AstoriaUnitTests.Tests.PeopleType' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
      <m:properties>
        <d:ID m:type='Edm.Int32'>1</d:ID>
        <d:Name>bar</d:Name>
        <d:Body></d:Body>
        <d:Age>6</d:Age>
        <d:Office m:type='#AstoriaUnitTests.Tests.OfficeType' m:null='true'/>
      </m:properties>
    </content>
  </entry>");
                        request.HttpMethod = "POST";
                        var exception = test.TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(exception, "Exception is always expected.");
                        Assert.IsNotNull(exception.InnerException, "InnerException is always expected.");

                        // For v1 and v2, provider should throw.
                        Assert.AreEqual("EntityFramework", exception.InnerException.Source, "Source expected: EntityFramework, actual: " + exception.InnerException.Source);
                        Assert.AreEqual(500, request.ResponseStatusCode, "Status code expected: 500" + ", actual: " + request.ResponseStatusCode);
                    }
                });
        }

        //TODO: Add test cases for complex property ("/People(1)/Office")
        [TestCategory("Partition2"), TestMethod]
        public void NonNullableTopLevelPropertiesTest()
        {
            // Create a Test case
            var testCase = new NonNullablePropertiesTestCase()
            {
                RequestUris = new string[] { "/People(1)/Name", "/People(1)/Body", "/People(1)/Age" },
                HttpMethods = new string[] { "PUT"},
            };            

            // Run the variations
            test.TestUtil.RunCombinations(
                new DSPUnitTestServiceDefinition[]{ service },
                testCase.HttpMethods,
                testCase.RequestUris,
                new string[] { UnitTestsUtil.JsonLightMimeType },
                ServiceVersion.ValidVersions, // requestDSV
                ServiceVersion.ValidVersions, // requestMDSV
                ServiceVersion.ValidVersions, // maxProtocolVersion
                (localService, httpMethod, requestUri, format, requestDSV, requestMDSV, maxProtocolVersion) =>
                {
                    if (maxProtocolVersion == null)
                    {
                        return;
                    }

                    localService.DataServiceBehavior.MaxProtocolVersion = maxProtocolVersion.ToProtocolVersion();
                    using (TestWebRequest request = localService.CreateForInProcess())
                    {
                        if (requestDSV != null && maxProtocolVersion.ToProtocolVersion() < requestDSV.ToProtocolVersion())
                        {
                            return;
                        }
                        request.StartService();
                        request.HttpMethod = httpMethod;
                        request.RequestUriString = requestUri;
                        request.Accept = format;
                        if (requestDSV != null) request.RequestVersion = requestDSV.ToString();
                        if (requestMDSV != null) request.RequestMaxVersion = requestMDSV.ToString();                           
                           
                        request.RequestContentType = format;
                        // TODO: Change the payload of null top-level properties #645
                        request.SetRequestStreamAsText(@"{ ""value"" : null }");

                        IDisposable dispose = null;
                        if (httpMethod != "GET")
                        {
                            dispose = service.CreateChangeScope(GetDefaultData(service.Metadata));
                        }

                        var exception = test.TestUtil.RunCatching(request.SendRequest);
                        Assert.IsNotNull(exception, "Exception is always expected.");
                        Assert.IsNotNull(exception.InnerException, "InnerException is always expected.");

                        if (requestUri.Substring(requestUri.LastIndexOf("/") + 1) == "Office")
                        {
                            Assert.AreEqual("System.Data.Entity", exception.InnerException.Source, "Source expected: System.Data.Entity, actual: " + exception.InnerException.Source);
                            Assert.AreEqual(500, request.ResponseStatusCode, "Status code expected: 500" + ", actual: " + request.ResponseStatusCode);
                        }
                        else
                        {
                            // For primitive properties provider decides what to do. For EF, we get an exception with response code of 500.
                            Assert.AreEqual(500, request.ResponseStatusCode, "Status code expected: 500" + ", actual: " + request.ResponseStatusCode);
                        }

                        dispose.Dispose();
                    }
                });
        }

        private class NonNullablePropertiesTestCase
        {
            public NonNullablePropertiesTestCase()
            {
                this.ExpectedStatusCode = 500;
                this.Version = new ServiceVersions();
                this.HttpMethods = new string[] { "PUT" };
            }

            public DSPUnitTestServiceDefinition[] Services { get; set; }
            public string[] RequestUris { get; set; }
            public ServiceVersions Version { get; set; }
            public string[] XPaths { get; set; }
            public int ExpectedStatusCode { get; set; }
            public string[] HttpMethods { get; set; }            
        }

        private static DSPMetadata GetModel(bool openType, bool namedStreams, Action<DSPMetadata> metadataModifier = null)
        {
            #region Model Definition
            // Navigation Collection Property: Client - Entity, Server - NonEntity
            DSPMetadata metadata = new DSPMetadata("ModelWithNonNullableProperties", "AstoriaUnitTests.Tests");
                
            // Define people type having non-nullable properties
            var peopleType = metadata.AddEntityType("PeopleType", null, null, false);
            var officeType = metadata.AddComplexType("OfficeType", null, null, false);
            metadata.AddPrimitiveProperty(officeType, "Building", typeof(string));
            metadata.AddPrimitiveProperty(officeType, "OfficeNumber", typeof(int));

            peopleType.IsOpenType = openType;
            metadata.AddKeyProperty(peopleType, "ID", typeof(int));
            if (!openType)
            {
                metadata.AddPrimitiveProperty(peopleType, "Name", typeof(string));                
                metadata.AddPrimitiveProperty(peopleType, "Body", typeof(byte[]));
                metadata.AddPrimitiveProperty(peopleType, "Age", typeof(Nullable<int>));
                metadata.AddComplexProperty(peopleType, "Office", officeType);
            }

            var peopleSet = metadata.AddResourceSet("People", peopleType);

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
            var officeType = metadata.GetResourceType("OfficeType");

            #region Default Data for the Model
            var context = new DSPContext();

            DSPResource people1 = new DSPResource(peopleType);
            people1.SetValue("ID", 1);
            people1.SetValue("Name", "Foo");
            people1.SetValue("Body", Encoding.UTF8.GetBytes("a byte array"));          
            people1.SetValue("Age", 23);

            var office = new DSPResource(officeType);
            office.SetValue("Building", "Building 18");
            office.SetValue("OfficeNumber", 100);
            people1.SetValue("Office", office);

            var people = context.GetResourceSetEntities("People");
            people.Add( people1 );

            #endregion Default Data for the Model

            return context;
        }


    }
}
