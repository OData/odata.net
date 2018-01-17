//---------------------------------------------------------------------
// <copyright file="AnnotationBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using AstoriaUnitTests.Stubs;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    [TestClass]
    public class AnnotationTests
    {
        private static int NumAnnotationBuilderCalls = 0;
        public class AnnotationBuilderContext
        {
            private static List<Customer> CustomersData;
            public IQueryable<Customer> Customers
            {
                get
                {
                    if (CustomersData == null)
                    {
                        CustomersData = new List<Customer>(new Customer[] { new Customer() { ID = 1 } });
                    }

                    return CustomersData.AsQueryable();
                }
            }
        }

        [Key("ID")]
        public class Customer
        {
            public int ID { get; set; }
            public Address Address { get; set; }
        }

        public class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
        }

        public class AnnotationsBuilderDataService : DataService<AnnotationBuilderContext>
        {
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.AnnotationsBuilder = BuildAnnotations;
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }

            private static IEnumerable<IEdmModel> BuildAnnotations(IEdmModel model)
            {
                NumAnnotationBuilderCalls++;
                var entityContainer = model.EntityContainer;
                Assert.IsNotNull(entityContainer);

                var entitySet = entityContainer.FindEntitySet("Customers");
                Assert.IsNotNull(entitySet);

                var customerType = model.FindDeclaredType("AstoriaUnitTests.Tests.AnnotationTests_Customer");
                Assert.IsNotNull(customerType);

                return new IEdmModel[] { model }.AsEnumerable();
            }
        }

        [TestCategory("Partition2"), TestMethod]
        public void EndToEndAnnotationTestEnsuringAnnotationBuilderCalledOnceAndMetadataBumpedToV3()
        {
            NumAnnotationBuilderCalls = 0;
            var testServiceDef = new TestServiceDefinition { DataServiceType = typeof(AnnotationsBuilderDataService) };
            using (TestWebRequest request = (testServiceDef).CreateForInProcessWcf())
            {               
                request.RequestUriString = request.BaseUri + "/$metadata";
                request.Accept = "application/xml";
                request.RequestMaxVersion = "4.0";
                request.SendRequest();
                var results = request.GetResponseStreamAsText();
                var returnedModel = CsdlReader.Parse(XmlTextReader.Create(new StringReader(results)));
                Assert.IsNotNull(returnedModel);

                Assert.AreEqual(1, NumAnnotationBuilderCalls);
            }
        }
        [Ignore] // Remove Atom
        // github: https://github.com/OData/odata.net/issues/868, same for other comment out test cases in this file.
        // [TestCategory("Partition2"), TestMethod]
        public void AnnotationBuilderShouldNotBeCalledOutsideMetadataUri()
        {
            NumAnnotationBuilderCalls = 0;
            var testServiceDef = new TestServiceDefinition { DataServiceType = typeof(AnnotationsBuilderDataService) };
            using (TestWebRequest request = (testServiceDef).CreateForInProcessWcf())
            {
                request.RequestUriString = request.BaseUri + "/Customers";
                request.Accept = "application/atom+xml";
                request.RequestMaxVersion = "4.0";
                request.SendRequest();
                request.GetResponseStreamAsText();
                Assert.AreEqual(0, NumAnnotationBuilderCalls);
            }
        }
    }
}

