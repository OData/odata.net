// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Timers;

namespace DataSourceManager.DataStoreManager
{
    /// <summary>
    /// Default resource management class to manage resources.
    /// Use a dictionary to easily access the resource by <see cref="TKey"/> and make a constraint on the total number of resources.
    /// Use a timer for each resource, when the resource live longer than <see cref="MaxDataStoreInstanceLifeTime"/>, it will be destroyed automatically.
    /// </summary>
    public class DefaultDataStoreManager<TKey, TDataStoreType> :IDataStoreManager<TKey, TDataStoreType> where TDataStoreType : class, new()
    {
        /// <summary>
        /// The max capacity of the resource container, this is a constraint for memory cost.
        /// </summary>
        public int MaxDataStoreInstanceCapacity { get; set; }

        /// <summary>
        /// The max life time of each resource. When the resource lives longer than that, it will be destroyed automatically.
        /// Besides, when the resource container is full, the resource live longest will be destroyed.
        /// </summary>
        public TimeSpan MaxDataStoreInstanceLifeTime { get; set; }

        private Dictionary<TKey, DataStoreInstanceWrapper> _dataStoreDict = new Dictionary<TKey, DataStoreInstanceWrapper>();

        public DefaultDataStoreManager()
        {
            MaxDataStoreInstanceCapacity = 1000;
            MaxDataStoreInstanceLifeTime = new TimeSpan(0, 15, 0);
        }

        public TDataStoreType ResetDataStoreInstance(TKey key)
        {
            if (_dataStoreDict.ContainsKey(key))
            {
                _dataStoreDict[key] = new DataStoreInstanceWrapper(key, MaxDataStoreInstanceLifeTime.TotalMilliseconds, ResouceTimeoutHandler);
            }
            else
            {
                AddDataStoreInstance(key);
            }

            return _dataStoreDict[key].DataStore;
        }

        public TDataStoreType GetDataStoreInstance(TKey key)
        {
            if (_dataStoreDict.ContainsKey(key))
            {
                _dataStoreDict[key].UpdateLastUsedDateTime();
            }
            else
            {
                AddDataStoreInstance(key);
            }

            return _dataStoreDict[key].DataStore;
        }

        private TDataStoreType AddDataStoreInstance(TKey key)
        {
            if (_dataStoreDict.Count >= MaxDataStoreInstanceCapacity)
            {
                // No resource lives longer than maxLifeTime, find the one lives longest and remove it.
                var minLastUsedTime = DateTime.Now;
                TKey minKey = default(TKey);

                foreach (var val in _dataStoreDict)
                {
                    var resourceLastUsedTime = val.Value.DataStoreLastUsedDateTime;
                    if (resourceLastUsedTime < minLastUsedTime)
                    {
                        minLastUsedTime = resourceLastUsedTime;
                        minKey = val.Key;
                    }
                }

                DeleteDataStoreInstance(minKey);
            }

            System.Diagnostics.Trace.TraceInformation("The resouce dictionary size right now is {0}", _dataStoreDict.Count);
            _dataStoreDict.Add(key, new DataStoreInstanceWrapper(key, MaxDataStoreInstanceLifeTime.TotalMilliseconds, ResouceTimeoutHandler));
            return _dataStoreDict[key].DataStore;
        }

        private DefaultDataStoreManager<TKey, TDataStoreType> DeleteDataStoreInstance(TKey key)
        {
            if (_dataStoreDict.ContainsKey(key))
            {
                _dataStoreDict[key].StopTimer();
                _dataStoreDict.Remove(key);
            }

            return this;
        }

        private void ResouceTimeoutHandler(object source, EventArgs e)
        {
            var resouceUnit = source as DataStoreInstanceWrapper;
            if (resouceUnit != null)
            {
                System.Diagnostics.Trace.TraceInformation(resouceUnit.DatastoreKey + " timeout occured, now destroy it!");
                DeleteDataStoreInstance(resouceUnit.DatastoreKey);
            }
        }

        private class DataStoreInstanceWrapper
        {
            public TKey DatastoreKey { get; private set; }

            public TDataStoreType DataStore { get; private set; }

            public DateTime DataStoreLastUsedDateTime { get; private set; }

            private Timer DataStoreTimer { get; set; }

            private double _dataStoreLifeTime;

            private EventHandler _timerTimeoutHandler;

            public DataStoreInstanceWrapper(TKey key, double dataStoreLifeTime, EventHandler dataStoreTimeoutHandler)
            {
                DatastoreKey = key;
                DataStore = new TDataStoreType();
                DataStoreLastUsedDateTime = DateTime.Now;
                _dataStoreLifeTime = dataStoreLifeTime;
                _timerTimeoutHandler += dataStoreTimeoutHandler;
                InitTimer();
            }

            public DataStoreInstanceWrapper UpdateLastUsedDateTime()
            {
                UpdateTimer();
                DataStoreLastUsedDateTime = DateTime.Now;
                return this;
            }

            public void StopTimer()
            {
                DataStoreTimer.Stop();
            }

            private Timer InitTimer()
            {
                DataStoreTimer = new Timer(_dataStoreLifeTime);
                DataStoreTimer.Elapsed += (sender, args) =>
                {
                    if (_timerTimeoutHandler != null)
                    {
                        _timerTimeoutHandler.Invoke(this, args);
                    }
                };
                DataStoreTimer.Start();
                return DataStoreTimer;
            }

            private void UpdateTimer()
            {
                DataStoreTimer.Stop();
                DataStoreTimer = InitTimer();
            }
        };
    }
}