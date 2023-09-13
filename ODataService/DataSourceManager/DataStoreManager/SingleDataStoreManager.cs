// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace DataSourceManager.DataStoreManager
{
    /// <summary>
    /// Data store manager for managing a single (i.e., read-only) datastore
    /// </summary>
    public class SingleDataStoreManager<TKey, TDataStoreType> : IDataStoreManager<TKey, TDataStoreType> where TDataStoreType : class, new()
    {
        private TDataStoreType dataStore;

        public SingleDataStoreManager()
        {
            dataStore = new TDataStoreType();
        }

        public TDataStoreType ResetDataStoreInstance(TKey key)
        {
            return dataStore = new TDataStoreType();
        }

        public TDataStoreType GetDataStoreInstance(TKey key)
        {
            return dataStore;
        }
    }
}