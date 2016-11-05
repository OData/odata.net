//---------------------------------------------------------------------
// <copyright file="OrderbyQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.QueryOptionTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OrderbyQueryTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public OrderbyQueryTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        private QueryOptionTestsHelper TestsHelper
        {
            get
            {
                return new QueryOptionTestsHelper(ServiceBaseUri, Model);
            }
        }

        [TestMethod]
        public void OrderbyQueryTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                // $count collection of primitive type
                List<ODataEntry> details = this.TestsHelper.QueryFeed("People?$orderby=Emails/$count", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Jill", details.First().Properties.Single(p => p.Name == "FirstName").Value);
                }

                // $count collection of primitive type, descending
                details = this.TestsHelper.QueryFeed("People?$orderby=Emails/$count desc", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Elmo", details.First().Properties.Single(p => p.Name == "FirstName").Value);
                }

                // $count collection of enum type
                details = this.TestsHelper.QueryFeed("Products?$orderby=CoverColors/$count", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Apple", details.First().Properties.Single(p => p.Name == "Name").Value);
                }

                // $count collection of complex type
                details = this.TestsHelper.QueryFeed("People?$orderby=Addresses/$count", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Jill", details.First().Properties.Single(p => p.Name == "FirstName").Value);
                }

                // $count collection of entity type
                details = this.TestsHelper.QueryFeed("Customers?$orderby=Orders/$count", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual("Bob", details.First().Properties.Single(p => p.Name == "FirstName").Value);
                }
            }
        }
    }
}
