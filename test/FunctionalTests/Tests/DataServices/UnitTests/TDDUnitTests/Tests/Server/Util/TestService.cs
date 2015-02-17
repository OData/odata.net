//---------------------------------------------------------------------
// <copyright file="TestService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server.Util
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Linq;

    public class TestService : DataService<SimpleContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.UseVerboseErrors = true;
        }
    }

    public class SimpleContext
    {
        public IQueryable<Customer> Customers { get { return new List<Customer>{new Customer{Id=1,Address=new Address{Street="Redmond Way"},Emails=new List<string>(){"bob@live.com"}}}.AsQueryable(); } }
    }

    [Key("Id")]
    public class Customer
    {
        public int Id { get; set; }
        public Address Address { get; set; }
        public IList<string> Emails { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
    }
}