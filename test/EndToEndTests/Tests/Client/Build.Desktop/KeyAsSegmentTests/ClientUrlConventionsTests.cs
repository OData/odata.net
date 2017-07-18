//---------------------------------------------------------------------
// <copyright file="ClientUrlConventionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using System.Collections;
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices.KeyAsSegmentServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientUrlConventionsTests : KeyAsSegmentTest
    {
        [TestMethod]
        public void ClientChangesUrlConventionsBetweenQueries()
        {
            var contextWrapper = this.CreateWrappedContext();

            var queryWithAzureKeys = contextWrapper.CreateQuery<Customer>("Customer").OrderBy(c => c.CustomerId).ToList();

            contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryWithDefaultKeys = contextWrapper.CreateQuery<Customer>("Customer").OrderBy(c => c.CustomerId).ToList();

            CollectionAssert.AreEqual(queryWithAzureKeys, queryWithDefaultKeys, new SimpleCustomerComparer());

            contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            queryWithAzureKeys = contextWrapper.CreateQuery<Customer>("Customer").OrderBy(c => c.CustomerId).ToList();

            CollectionAssert.AreEqual(queryWithAzureKeys, queryWithDefaultKeys, new SimpleCustomerComparer());
        }

        // This is an unsupported scenario and does not currently work.
        // [TestMethod] // github issuse: #896
        public void ClientChangesUrlConventionsBetweenQueryAndUpdate()
        {
            var contextWrapper = this.CreateWrappedContext();

            var customersWithAzureKeys = contextWrapper.CreateQuery<Customer>("Customer").Take(1).Single();

            contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var order = new Order { OrderId = 98765 };
            contextWrapper.AddRelatedObject(customersWithAzureKeys, "Orders", order);
            contextWrapper.SaveChanges();
        }

        /// <summary>
        /// Simple high-level comparison of two Customer objects
        /// </summary>
        internal class SimpleCustomerComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null)
                {
                    Assert.IsNull(y);
                }
                else
                {
                    Customer c1 = (Customer)x;
                    Customer c2 = (Customer)y;

                    Assert.AreEqual(c1.CustomerId, c2.CustomerId);
                    Assert.AreEqual(c1.Name, c2.Name);
                    Assert.AreEqual(c1.BackupContactInfo.Count, c2.BackupContactInfo.Count);
                    this.Compare(c1.Husband, c2.Husband);
                    this.Compare(c1.Wife, c2.Wife);
                }

                return 0;
            }
        }
    }
}
