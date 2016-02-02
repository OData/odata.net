//---------------------------------------------------------------------
// <copyright file="ODataEntriesEntityMaterializerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using AstoriaUnitTests.Tests;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the ODataEntriesEntityMaterializerUnitTests class.
    /// </summary>
    [TestClass]
    public class ODataEntriesEntityMaterializerUnitTests
    {
        [TestMethod]
        public void ShortIntegrationTestToValidateEntryShouldBeRead()
        {
            var odataEntry = new ODataEntry() { Id = new Uri("http://services.odata.org/OData/OData.svc/Customers(0)") };
            odataEntry.Properties = new ODataProperty[] { new ODataProperty() { Name = "ID", Value = 0 }, new ODataProperty() { Name = "Description", Value = "Simple Stuff" } };

            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            var context = new DataServiceContext();
            MaterializerEntry.CreateEntry(odataEntry, ODataFormat.Atom, true, clientEdmModel);
            var materializerContext = new TestMaterializerContext() {Model = clientEdmModel, Context = context};
            var adapter = new EntityTrackingAdapter(new TestEntityTracker(), MergeOption.OverwriteChanges, clientEdmModel, context);
            QueryComponents components = new QueryComponents(new Uri("http://foo.com/Service"), new Version(4, 0), typeof(Customer), null, new Dictionary<Expression, Expression>());

            var entriesMaterializer = new ODataEntriesEntityMaterializer(new ODataEntry[] { odataEntry }, materializerContext, adapter, components, typeof(Customer), null, ODataFormat.Atom);
            
            var customersRead = new List<Customer>();

            // This line will call ODataEntityMaterializer.ReadImplementation() which will reconstruct the entity, and will get non-public setter called.
            while (entriesMaterializer.Read())
            {
                customersRead.Add(entriesMaterializer.CurrentValue as Customer);
            }

            customersRead.Should().HaveCount(1);
            customersRead[0].ID.Should().Be(0);
            customersRead[0].Description.Should().Be("Simple Stuff");
        }
        
        public class Customer
        {
            public int ID { get; set; }

            // Mark setter to be non-public to test entity can still be constructed.
            public string Description { get; private set; }

            public Customer()
            {
            }

            public Customer(int id, string desc)
            {
                this.ID = id;
                this.Description = desc;
            }
        }
    }
}
