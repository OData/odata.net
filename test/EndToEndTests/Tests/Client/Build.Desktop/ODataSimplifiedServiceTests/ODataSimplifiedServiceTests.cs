//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedServiceQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices.ODataSimplifiedServiceReference;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for ODataSimplified service
    /// </summary>
    // [Ignore] // Issues: #623
    // [TestClass] // github issuse: #896
    public class ODataSimplifiedServiceQueryTests : ODataWCFServiceTestsBase<ODataSimplifiedService>
    {
        public ODataSimplifiedServiceQueryTests()
            : base(ServiceDescriptors.ODataSimplifiedServiceDescriptor)
        {
        }

        [TestMethod]
        public void TestODataSimplifiedServiceQueryEntities()
        {
            TestClientContext.EnableWritingODataAnnotationWithoutPrefix = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

            Assert.IsTrue(TestClientContext.People.ToList().Count >= 5);
            Assert.AreEqual(5, TestClientContext.Products.ToList().Count);
        }

        [TestMethod]
        public void TestODataSimplifiedServiceQueryNavigationProperty()
        {
            TestClientContext.EnableWritingODataAnnotationWithoutPrefix = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

            Assert.AreEqual(2, TestClientContext.People.ByKey(new Dictionary<string, object> { { "PersonId", 1 } }).Products.ToList().Count);
        }

        [TestMethod]
        public void TestODataSimplifiedServiceQueryExpandedEntities()
        {
            TestClientContext.EnableWritingODataAnnotationWithoutPrefix = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

            var person = TestClientContext.People.Expand("Products").Where(p => p.PersonId == 1).ToList();
            Assert.AreEqual(2, person[0].Products.ToList().Count);
        }

        [TestMethod]
        public void TestODataSimplifiedServiceCreateEntity()
        {
            TestClientContext.EnableWritingODataAnnotationWithoutPrefix = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

            var person = new Person
            {
                PersonId = 99,
                FirstName = "Jack",
                LastName = "Jones",
                Address = new Address
                {
                    Road = "Ziyue Road",
                    City = "Shanghai"
                },
                Descriptions = new ObservableCollection<string> { "Kindly" }
            };
            TestClientContext.AddToPeople(person);
            TestClientContext.SaveChanges();

            TestClientContext.Detach(person);
            var personReturned = TestClientContext.People.ByKey(new Dictionary<string, object> { { "PersonId", person.PersonId } }).GetValue();
            Assert.AreEqual(person.PersonId, personReturned.PersonId);
        }
    }
}
