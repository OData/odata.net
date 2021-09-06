//---------------------------------------------------------------------
// <copyright file="OracleBasedAsyncDataSynchronizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Uses the data oracle service to synchronize data asyncronously
    /// </summary>
    [ImplementationName(typeof(IAsyncDataSynchronizer), "Default")]
    public class OracleBasedAsyncDataSynchronizer : IAsyncDataSynchronizer
    {
        /// <summary>
        /// Gets or sets the oracle service client
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataOracleService OracleServiceClient { get; set; }

        /// <summary>
        /// Gets or sets the synchronizer to invoke with the results from the oracle
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IOracleBasedDataSynchronizer UnderlyingSynchronizer { get; set; }

        /// <summary>
        /// Synchronizes the data for the entity set with the given name
        /// </summary>
        /// <param name="continuation">The asynchronous continuation to report failure/completion on</param>
        /// <param name="entitySetName">The name of the entity set to refresh</param>
        public void SynchronizeEntireEntitySet(IAsyncContinuation continuation, string entitySetName)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(entitySetName, "entitySetName");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.OracleServiceClient.BeginGetEntitySet(
                entitySetName,
                result => AsyncHelpers.CatchErrors(
                    continuation, 
                    delegate
                    {
                        string error;
                        var entities = this.OracleServiceClient.EndGetEntitySet(out error, result);
                        if (error != null)
                        {
                            continuation.Fail(new Exception(error));
                            return;
                        }

                        this.UnderlyingSynchronizer.SynchronizeEntireEntitySet(entitySetName, entities);
                        continuation.Continue();
                    }),
                null);
        }

        /// <summary>
        /// Synchronizes the data for the entity with the given set name and key values
        /// </summary>
        /// <param name="continuation">The asynchronous continuation to report failure/completion on</param>
        /// <param name="entitySetName">The entity set the entity belongs to</param>
        /// <param name="keyValues">The key values of the entity</param>
        public void SynchronizeEntityInstanceGraph(IAsyncContinuation continuation, string entitySetName, IEnumerable<NamedValue> keyValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(continuation, "continuation");
            ExceptionUtilities.CheckArgumentNotNull(entitySetName, "entitySetName");
            ExceptionUtilities.CheckCollectionNotEmpty(keyValues, "keyValues");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.OracleServiceClient.BeginGetEntity(
                entitySetName, 
                keyValues.Select(v => new SerializableNamedValue() { Name = v.Name, Value = v.Value }).ToList(),
                result => AsyncHelpers.CatchErrors(
                    continuation, 
                    delegate
                        {
                        string error;
                        var entity = this.OracleServiceClient.EndGetEntity(out error, result);
                        if (error != null)
                        {
                            continuation.Fail(new Exception(error));
                            return;
                        }

                        this.UnderlyingSynchronizer.SynchronizeEntityInstanceGraph(entity);
                        continuation.Continue();
                    }),
                null);
        }
    }
}