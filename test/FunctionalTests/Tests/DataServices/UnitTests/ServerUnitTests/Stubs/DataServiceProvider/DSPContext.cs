//---------------------------------------------------------------------
// <copyright file="DSPContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using Microsoft.OData.Service.Providers;
    using System.Collections.Generic;

    /// <summary>The "context" for the DSP data provider. The context holds the actual data to be reported through the provider.</summary>
    /// <remarks>This implementation stores the data in a List and all of it is in-memory.</remarks>
    public class DSPContext
    {
        /// <summary>The actual data storage.</summary>
        /// <remarks>Dictionary where the key is the name of the resource set and the value is a list of resources.</remarks>
        private Dictionary<string, List<object>> resourceSetsStorage;

        /// <summary>Constructor, creates a new empty context.</summary>
        public DSPContext()
        {
            this.resourceSetsStorage = new Dictionary<string, List<object>>();
            this.ServiceOperations = new Dictionary<string, Func<object[], object>>();
        }

        /// <summary>
        /// Returns the list of entity sets that are defined on this context.
        /// </summary>
        public IEnumerable<KeyValuePair<string, List<object>>> EntitySets
        {
            get { return this.resourceSetsStorage; }
        }

        /// <summary>
        /// A dictionary of service operations that can be invoked on this context
        /// </summary>
        public Dictionary<string, Func<Object[], Object>> ServiceOperations { get; set; }

        /// <summary>Gets a list of resources for the specified resource set.</summary>
        /// <param name="resourceSetName">The name of the resource set to get resources for.</param>
        /// <returns>List of resources for the specified resource set. Note that if such resource set was not yet seen by this context
        /// it will get created (with empty list).</returns>
        public List<object> GetResourceSetEntities(string resourceSetName)
        {
            List<object> entities;
            if (!this.resourceSetsStorage.TryGetValue(resourceSetName, out entities))
            {
                entities = new List<object>();
                this.resourceSetsStorage[resourceSetName] = entities;
            }

            return entities;
        }
    }
}
