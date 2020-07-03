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
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

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

        [Fact]
        public void OrderbyQueryTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                // $count collection of primitive type
                List<ODataResource> resources = this.TestsHelper.QueryFeed("People?$orderby=Emails/$count", mimeType);
                Func<List<ODataResource>, List<ODataResource>> getEntries = 
                    (res) => res.Where(r => r != null && (r.TypeName.Contains("Person") 
                        || r.TypeName.Contains("Customer")
                        || r.TypeName.Contains("Product")
                        || r.TypeName.Contains("Employee"))).ToList();
                var details = getEntries(resources);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal("Jill", details.First().Properties.Single(p => p.Name == "FirstName").Value);
                }

                // $count collection of primitive type, descending
                details = getEntries(this.TestsHelper.QueryFeed("People?$orderby=Emails/$count desc", mimeType));
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal("Elmo", details.First().Properties.Single(p => p.Name == "FirstName").Value);
                }

                // $count collection of enum type
                details = getEntries(this.TestsHelper.QueryFeed("Products?$orderby=CoverColors/$count", mimeType));
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal("Apple", details.First().Properties.Single(p => p.Name == "Name").Value);
                }

                // $count collection of complex type
                details = getEntries(this.TestsHelper.QueryFeed("People?$orderby=Addresses/$count", mimeType));
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal("Jill", details.First().Properties.Single(p => p.Name == "FirstName").Value);
                }

                // $count collection of entity type
                details = getEntries(this.TestsHelper.QueryFeed("Customers?$orderby=Orders/$count", mimeType));
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal("Bob", details.First().Properties.Single(p => p.Name == "FirstName").Value);
                }
            }
        }
    }
}
