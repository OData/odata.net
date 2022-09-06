//---------------------------------------------------------------------
// <copyright file="IOracleBasedDataSynchronizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;

    /// <summary>
    /// Internal contract for use in the context of a particular implementation of IAsyncDataSynchronizer which uses a data oracle
    /// Does not have an implementation name because it is not intended to be used externally
    /// </summary>
    public interface IOracleBasedDataSynchronizer
    {
        /// <summary>
        /// Synchronizes an entity set given the output from a data oracle
        /// </summary>
        /// <param name="entitySetName">The name of the set to synchronize</param>
        /// <param name="allEntitiesInSet">All of the entities that exist in the set</param>
        void SynchronizeEntireEntitySet(string entitySetName, IEnumerable<SerializableEntity> allEntitiesInSet);

        /// <summary>
        /// Synchronizes an entity and its related subgraph given the output from a data oracle
        /// </summary>
        /// <param name="entity">The entity returned from the oracle</param>
        void SynchronizeEntityInstanceGraph(SerializableEntity entity);
    }
}