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
    using Xunit;

    /// <summary>
    /// Tests for ODataSimplified service
    /// </summary>
    // [Ignore] // Issues: #623
    // [TestClass] // github issuse: #896
    //The whole of this class was ignored in MsTest tests that is why am skipping all the tests in this class. 
    public class ODataSimplifiedServiceQueryTests : ODataWCFServiceTestsBase<ODataSimplifiedService>
    {
        public ODataSimplifiedServiceQueryTests()
            : base(ServiceDescriptors.ODataSimplifiedServiceDescriptor)
        {
        }

        [Fact(Skip ="Ignore")]
        public void TestODataSimplifiedServiceQueryEntities()
        {
            TestClientContext.EnableWritingODataAnnotationWithoutPrefix = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

            Assert.True(TestClientContext.People.ToList().Count >= 5);
            Assert.Equal(5, TestClientContext.Products.ToList().Count);
        }

        [Fact(Skip ="Ignore")]
        public void TestODataSimplifiedServiceQueryNavigationProperty()
        {
            TestClientContext.EnableWritingODataAnnotationWithoutPrefix = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

            Assert.Equal(2, TestClientContext.People.ByKey(new Dictionary<string, object> { { "PersonId", 1 } }).Products.ToList().Count);
        }

        [Fact(Skip ="Ignore")]
        public void TestODataSimplifiedServiceQueryExpandedEntities()
        {
            TestClientContext.EnableWritingODataAnnotationWithoutPrefix = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

            var person = TestClientContext.People.Expand("Products").Where(p => p.PersonId == 1).ToList();
            Assert.Equal(2, person[0].Products.ToList().Count);
        }

        [Fact(Skip ="Ignore")]
        public void TestODataSimplifiedServiceCreateEntity()
        {
            TestClientContext.EnableWritingODataAnnotationWithoutPrefix = true;
            TestClientContext.SendingRequest2 += (sender, eventArgs) => ((Microsoft.OData.Client.HttpClientRequestMessage)eventArgs.RequestMessage).SetHeader("Accept", "application/json;odata.metadata=full");

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
            Assert.Equal(person.PersonId, personReturned.PersonId);
        }
    }
}
