//---------------------------------------------------------------------
// <copyright file="ODataEntityMaterializerUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using FluentAssertions;
    using Microsoft.OData;
    using Xunit;

    /// <summary>
    /// Unit tests for the ODataEntityMaterializerUnitTests class.
    /// </summary>
    public class ODataEntityMaterializerUnitTests
    {
        private ClientEdmModel clientModel;
        private DataServiceContext context;

        public ODataEntityMaterializerUnitTests()
        {
            this.clientModel = new ClientEdmModel(ODataProtocolVersion.V4);
            this.context = new DataServiceContext();
        }

        [Fact]
        public void AfterEntryMaterializedShouldOccur()
        {
            foreach (ODataFormat format in new ODataFormat[] { ODataFormat.Json })
            {
                var entity = new SimpleEntity() { ID = 1 };
                var odataEntry = CreateEntryWithMaterializerEntry(format, entity);
                MaterializedEntityArgs found = null;
                this.context.Configurations.ResponsePipeline.OnEntityMaterialized((MaterializedEntityArgs materializedEntryEventArgs) => found = materializedEntryEventArgs);

                this.context.Configurations.ResponsePipeline.FireEndEntryEvents(MaterializerEntry.GetEntry(odataEntry));
                Assert.NotNull(found);
                found.Entity.Should().Be(entity);
                found.Entry.Should().Be(odataEntry);
            }
        }

        private ODataResource CreateEntryWithMaterializerEntry(ODataFormat format, object resolvedObject)
        {
            var entry = new ODataResource();
            entry.Id = new Uri("http://www.odata.org/Northwind.Svc/Customer(1)");
            entry.Properties = new ODataProperty[] { new ODataProperty() { Name = "ID", Value = 1 } };

            var materializerEntry = MaterializerEntry.CreateEntry(entry, format, true, this.clientModel);
            materializerEntry.ResolvedObject = resolvedObject;

            return entry;
        }

        internal class SimpleEntity
        {
            public int ID { get; set; }
        }
    }
}
