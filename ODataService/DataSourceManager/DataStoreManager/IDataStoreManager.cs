// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataSourceManager.DataStoreManager
{
    /// <summary>
    /// This is a data store management interface to manage the datastore with its key.
    /// </summary>
    /// <typeparam name="TKey">Type of the key to identify the data store.</typeparam>
    /// <typeparam name="TDataStoreType">Type of the real data store.</typeparam>
    public interface IDataStoreManager<TKey, TDataStoreType>
    {
        /// <summary>
        /// Get the data store instance by its key.
        /// </summary>
        /// <param name="key">The key to identify the data store.</param>
        /// <returns>Return the data store with key.</returns>
        TDataStoreType GetDataStoreInstance(TKey key);

        /// <summary>
        /// Find the resource by key and reset it to origin.
        /// </summary>
        /// <param name="key">The key to identify the data store.</param>
        /// <returns>Return the data store after reseting.</returns>
        TDataStoreType ResetDataStoreInstance(TKey key);
    }
}