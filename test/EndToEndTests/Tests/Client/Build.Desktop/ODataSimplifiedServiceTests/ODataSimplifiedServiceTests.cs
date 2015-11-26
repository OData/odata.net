//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedServiceQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices.ODataSimplifiedServiceReference;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for pluggable format service
    /// </summary>
    [TestClass]
    public class ODataSimplifiedServiceQueryTests : ODataWCFServiceTestsBase<ODataSimplifiedService>
    {
        public ODataSimplifiedServiceQueryTests()
            : base(ServiceDescriptors.ODataSimplifiedServiceDescriptor)
        {
        }

        [TestMethod]
        public void TestODataSimplifiedServiceQueryEntities()
        {
            TestClientContext.ODataSimplified = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpWebRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

            Assert.AreEqual(5, this.TestClientContext.People.ToList().Count);
            Assert.AreEqual(5, this.TestClientContext.Products.ToList().Count);
        }

        [TestMethod]
        public void TestODataSimplifiedServiceCreateEntity()
        {
            TestClientContext.ODataSimplified = true;
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
            var personReturned = TestClientContext.People.ByKey(new Dictionary<string,object>{{"PersonId", person.PersonId}}).GetValue();
            Assert.AreEqual(person.PersonId, personReturned.PersonId);
        }
    }
}
