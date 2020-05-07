//---------------------------------------------------------------------
// <copyright file="LiteralFormatTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using System;
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices.KeyAsSegmentServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LiteralFormatTests : KeyAsSegmentTest
    {
        [TestMethod]
        public void PrimaryKeyValueBeginsWithDollarSign()
        {
            var contextWrapper = this.CreateWrappedContext();
            var dollarSignKeyValues = new[] { "$var1", "$$", "$$$", "$$$$", "$", "$orderby", "$filter", "$format", "$top", "$count", "$expand", "$select" };
            var customer = contextWrapper.Context.Customer.Take(1).Single();

            foreach (var dollarSignKeyValue in dollarSignKeyValues)
            {
                var newLogin = AddNewLoginForCustomer(contextWrapper.Context, customer, dollarSignKeyValue);

                var loginQuery = contextWrapper.CreateQuery<Login>("Login").Where(l => l.Username == dollarSignKeyValue).ToArray();
                Assert.IsTrue(newLogin == loginQuery.Single(), "Query result does not equal newly added login with key " + dollarSignKeyValue);

                var customerQuery = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri + "/Login/$" + dollarSignKeyValue + "/Customer")).ToArray();
                Assert.IsTrue(customer == customerQuery.Single(), "Execute query result does not equal associated customer");
            }
        }

        [TestMethod]
        public void PrimaryKeyValueContainsForwardSlash()
        {
            var contextWrapper = this.CreateWrappedContext();
            var keyValues = new[] { " /", "/ ", "var1/baz", "//var1", "var1//" };
            var customer = contextWrapper.Context.Customer.Take(1).Single();

            foreach (var keyValue in keyValues)
            {
                AddNewLoginForCustomer(contextWrapper.Context, customer, keyValue);
            }
        }

        [TestMethod]
        public void PrimaryKeyValueContainsWhitespace()
        {
            var contextWrapper = this.CreateWrappedContext();
            
            // Strings cannot have whitespace at the end.
            var keyValues = new[] { "var1 baz", "  var1" }; 
            var customer = contextWrapper.Context.Customer.Take(1).Single();

            foreach (var keyValue in keyValues)
            {
                var newLogin = AddNewLoginForCustomer(contextWrapper.Context, customer, keyValue);

                var loginQuery = contextWrapper.CreateQuery<Login>("Login").Where(l => l.Username == keyValue).ToArray();
                Assert.IsTrue(newLogin == loginQuery.Single(), "Query result does not equal newly added login with key " + keyValue);

                var customerQuery = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri + "/Login/" + keyValue + "/Customer")).ToArray();
                Assert.IsTrue(customer == customerQuery.Single(), "Execute query result does not equal associated customer");
            }
        }

        private Login AddNewLoginForCustomer(DefaultContainer context, Customer customer, string loginKeyValue)
        {
            var newLogin = new Login { Username = loginKeyValue, };

            context.AddToLogin(newLogin);
            context.SetLink(newLogin, "Customer", customer);
            context.AddLink(customer, "Logins", newLogin);
            context.SaveChanges();

            return newLogin;
        }
    }
}
