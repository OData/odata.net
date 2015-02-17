//---------------------------------------------------------------------
// <copyright file="IAsyncDataSynchronizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for any component used to synchronize entity or entity-set data asynchronously
    /// </summary>
    [ImplementationSelector("AsyncDataSynchronizer", DefaultImplementation = "Default")]
    public interface IAsyncDataSynchronizer
    {
        /// <summary>
        /// Synchronizes the data for the entity set with the given name
        /// </summary>
        /// <param name="continuation">The asynchronous continuation to report failure/completion on</param>
        /// <param name="entitySetName">The name of the entity set to refresh</param>
        void SynchronizeEntireEntitySet(IAsyncContinuation continuation, string entitySetName);

        /// <summary>
        /// Synchronizes the data for the entity with the given set name and key values
        /// </summary>
        /// <param name="continuation">The asynchronous continuation to report failure/completion on</param>
        /// <param name="entitySetName">The entity set the entity belongs to</param>
        /// <param name="keyValues">The key values of the entity</param>
        void SynchronizeEntityInstanceGraph(IAsyncContinuation continuation, string entitySetName, IEnumerable<NamedValue> keyValues);
    }
}