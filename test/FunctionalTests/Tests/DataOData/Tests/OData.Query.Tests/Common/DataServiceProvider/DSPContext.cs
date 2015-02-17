//---------------------------------------------------------------------
// <copyright file="DSPContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.Common.DataServiceProvider
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// The "context" for the DSP data provider. The context holds the actual data to be reported through the provider. 
    /// This implementation stores the data in a List and all of it is in-memory.
    /// </summary>
    /// <remarks>This class was copied from the product unit tests.</remarks>
    public class DSPContext
    {
        /// <summary>The actual data storage.</summary>
        /// <remarks>Dictionary where the key is the name of the entity set and the value is a list of resources.</remarks>
        private readonly Dictionary<string, List<object>> entitySetsStorage;
                        
        /// <summary>Constructor, creates a new empty context.</summary>
        public DSPContext()
        {
            this.entitySetsStorage = new Dictionary<string, List<object>>();
            this.ServiceOperations = new Dictionary<string, Func<object[], object>>();
        }

        /// <summary>
        /// A dictionary of service operations that can be invoked on this context
        /// </summary>
        public Dictionary<string, Func<Object[], Object>> ServiceOperations { get; set; }

        /// <summary>Gets a list of entities for the specified entity set.</summary>
        /// <param name="entitySetName">The name of the entity set to get resources for.</param>
        /// <returns>List of resources for the specified entity set. Note that if such entity set was not yet seen by this context
        /// it will get created (with empty list).</returns>
        public IList<object> GetEntitySetEntities(string entitySetName)
        {
            List<object> entities;
            if (!this.entitySetsStorage.TryGetValue(entitySetName, out entities))
            {
                entities = new List<object>();
                this.entitySetsStorage[entitySetName] = entities;
            }

            return entities;
        }
    }
}
