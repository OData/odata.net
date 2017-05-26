//---------------------------------------------------------------------
// <copyright file="ClientOpenTypeUpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.OpenTypesServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Generic client update test cases.
    /// </summary>
    [TestClass]
    public class ClientOpenTypeUpdateTests : EndToEndTestBase
    {
        private DataServiceContextWrapper<DefaultContainer> contextWrapper;

        public ClientOpenTypeUpdateTests()
            : base(ServiceDescriptors.OpenTypesService)
        {
        }

        [TestMethod]
        public void UpdateOpenTypeWithUndeclaredProperties()
        {
            SetContextWrapper();
            contextWrapper.MergeOption = MergeOption.PreserveChanges;
            contextWrapper.Configurations.RequestPipeline.OnEntryStarting(ea => EntryStarting(ea));
            var row = contextWrapper.Context.Row.Where(r => r.Id == Guid.Parse("814d505b-6b6a-45a0-9de0-153b16149d56")).First();

            // In practice, transient property data would be mutated here in the partial companion to the client proxy.

            contextWrapper.UpdateObject(row);
            contextWrapper.SaveChanges();
            // No more check, this case is to make sure that client doesn't throw exception.
        }

        private void EntryStarting(WritingEntryArgs ea)
        {
            var odataProps = ea.Entry.Properties as List<ODataProperty>;

            var entityState = contextWrapper.Context.Entities.First(e => e.Entity == ea.Entity).State;

            // Send up an undeclared property on an Open Type.
            if (entityState == EntityStates.Modified || entityState == EntityStates.Added)
            {
                if (ea.Entity.GetType() == typeof(Row))
                {
                    // In practice, the data from this undeclared property would probably be stored in a transient property of the partial companion class to the client proxy.
                    var undeclaredOdataProperty = new ODataProperty() { Name = "dynamicPropertyKey", Value = "dynamicPropertyValue" };
                    odataProps.Add(undeclaredOdataProperty);
                }
            }
        }

        private void SetContextWrapper()
        {
            contextWrapper = this.CreateWrappedContext<DefaultContainer>();
        }
    }
}
