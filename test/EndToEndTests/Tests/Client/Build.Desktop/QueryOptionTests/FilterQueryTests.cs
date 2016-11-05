//---------------------------------------------------------------------
// <copyright file="FilterQueryTests.cs" company="Microsoft">
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
    public class FilterQueryTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public FilterQueryTests()
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
        public void FilterQueryTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                // $count collection of primitive type
                List<ODataEntry> details = this.TestsHelper.QueryFeed("People?$filter=Emails/$count lt 2", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(4, details.Count);
                }

                // $count collection of enum type
                details = this.TestsHelper.QueryFeed("Products?$filter=CoverColors/$count lt 2", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(1, details.Count);
                }

                // $count collection of complex type
                details = this.TestsHelper.QueryFeed("People?$filter=Addresses/$count eq 2", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(2, details.Count);
                }

                // $count collection of entity type
                details = this.TestsHelper.QueryFeed("Customers?$filter=Orders/$count lt 2", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(1, details.Count);
                }
            }
        }
    }
}
