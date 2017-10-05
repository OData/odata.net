//---------------------------------------------------------------------
// <copyright file="WeakKeyDictioaryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WeakKeyDictioaryTests
    {
        WeakKeyComparer<KeyObject> comparer = WeakKeyComparer<KeyObject>.Default;
        Random rand = new Random();

        [TestMethod]
        public void TestAddItemIntoDict()
        {
            //Used to make sure all keys are alive in current scope
            List<KeyObject> keys;
            var weakDict = CreateDict(out keys);
            Assert.AreEqual(10, weakDict.Count);
        }

        [TestMethod]
        public void TestDictContainsKey()
        {
            List<KeyObject> keys;
            var weakDict = CreateDict(out keys);

            foreach (var key in keys)
            {
                Assert.IsTrue(weakDict.ContainsKey(key), "The dictionary should contains the key {0}", key);
            }
        }

        [TestMethod]
        public void TestDictIndex()
        {
            List<KeyObject> keys;
            var weakDict = CreateDict(out keys);

            //Get
            for (int i = 0; i < weakDict.Count; i++)
            {
                Assert.AreEqual(i, weakDict[keys[i]]);
            }

            //Set
            for (int i = 0; i < weakDict.Count; i++)
            {
                weakDict[keys[i]] = i + 1;
            }

            for (int i = 0; i < weakDict.Count; i++)
            {
                Assert.AreEqual(i + 1, weakDict[keys[i]]);
            }
        }

        [TestMethod]
        public void TestDictRemovesKey()
        {
            List<KeyObject> keys;
            var weakDict = CreateDict(out keys);

            int count = weakDict.Count;
            foreach (var key in keys)
            {
                weakDict.Remove(key);
                Assert.AreEqual(--count, weakDict.Count);
                Assert.IsFalse(weakDict.ContainsKey(key), "The dictionary should contains the key {0}", key);
            }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        // Net Core 1.0 is missing GC APIs
        [TestMethod]
        public void TestRemoveDeadItem()
        {
            WeakDictionary<KeyObject, int> weakDict = new WeakDictionary<KeyObject, int>(comparer, 5);
            for (int i = 0; i < 5; i++)
            {
                AddAnItem(weakDict, i);
            }

            // Force gc to collect dead items
            GC.Collect();
            if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
            {
                Assert.AreEqual(5, weakDict.Count);

                // Since the referesh interval is 5, so when adding the sixth item, it should clear the 5 dead items first.
                weakDict.RemoveCollectedEntries();
                Assert.AreEqual(0, weakDict.Count);
            }
        }

        [TestMethod]
        public void TestRemoveDeadItemWhenAddMoreThanRefreshIntervalItems()
        {
            WeakDictionary<KeyObject, int> weakDict = new WeakDictionary<KeyObject, int>(this.comparer, 5);
            for (int i = 0; i < 5; i++)
            {
                AddAnItem(weakDict, i);
            }

            // Force gc to collect dead items
            GC.Collect();
            if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
            {
                var key = new KeyObject(6);
                weakDict.Add(key, 6);

                // The countLimit after the first clean is 6 now.
                Assert.AreEqual(1, weakDict.Count);
            }
        }

        [TestMethod]
        public void TestRemoveDeadItemWhenAddMoreThanCurrentRefreshIntervalItems()
        {
            WeakDictionary<KeyObject, int> weakDict = new WeakDictionary<KeyObject, int>(this.comparer, 5);
            for (int i = 0; i < 4; i++)
            {
                AddAnItem(weakDict, i);
            }

            // Force gc to collect dead items
            GC.Collect();
            if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
            {
                var key1 = new KeyObject(11);
                weakDict.Add(key1, 11);

                var key2 = new KeyObject(12);

                // Weak dictionary will remove dead items during this adding operation.
                // Then the currentRefreshInterval will be 1(still alive)+5=6.
                weakDict.Add(key2, 12);

                Assert.AreEqual(2, weakDict.Count);

                for (int i = 0; i < 3; i++)
                {
                    AddAnItem(weakDict, i);
                }

                // Force gc to collect dead items
                GC.Collect();
                if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
                {
                    // Add the six item to dictionary.
                    var key6 = new KeyObject(16);

                    // Weak dictionary will not clean dead entities.
                    weakDict.Add(key6, 16);
                    Assert.AreEqual(6, weakDict.Count);

                    var key14 = new KeyObject(114);

                    // Weak dictionary will remove dead items during this adding operation.
                    // Then the currentRefreshInterval will be 3(still alive)+5=8.
                    weakDict.Add(key14, 114);
                    Assert.AreEqual(4, weakDict.Count);
                }
            }
        }
#endif

        private void AddAnItem(WeakDictionary<KeyObject, int> dict, int id)
        {
            KeyObject key = new KeyObject(id);
            dict.Add(key, rand.Next());
        }

        [TestMethod]
        public void TestAddANullKeyThrowsException()
        {
            WeakDictionary<KeyObject, int> weakDict = new WeakDictionary<KeyObject, int>(this.comparer, 5);
            Action testAction = () => { weakDict.Add(null, 10); };
            testAction.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void TestAddSameKeyThrowsException()
        {
            WeakDictionary<KeyObject, int> weakDict = new WeakDictionary<KeyObject, int>(this.comparer, 5);
            KeyObject key = new KeyObject(10);
            weakDict.Add(key, 10);

            Action testAction = () => { weakDict.Add(key, 10); };
            testAction.ShouldThrow<ArgumentException>();
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void TestWeakKeyComparerForRef()
        {
            var array = Enum.GetValues(typeof(RefType));

            foreach (RefType left in array)
            {
                foreach (RefType right in array)
                {
                    TestWeakKeyComparerForRefImplement(left, right);
                }
            }
        }

        // Net Core 1.0 is missing GC APIs
        private void TestWeakKeyComparerForRefImplement(RefType leftRefType, RefType rightRefType)
        {
            KeyObject obj = new KeyObject(1);
            var left = CreateObj(obj, leftRefType);
            var right = CreateObj(obj, rightRefType);

            if (leftRefType != RefType.DeadWeak && rightRefType != RefType.DeadWeak)
            {
                Assert.IsTrue(this.comparer.Equals(left, right));
            }
            else
            {
                GC.Collect();
                if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
                {
                    Assert.IsFalse(this.comparer.Equals(left, right));
                }
            }
        }

        [TestMethod]
        public void TestWeakKeyComparerForSameRef()
        {
            var array = Enum.GetValues(typeof(RefType));

            foreach (RefType refType in array)
            {
                KeyObject keyObj = new KeyObject(1);
                var obj = CreateObj(keyObj, refType);
                GC.Collect();
                if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
                {
                    Assert.IsTrue(this.comparer.Equals(obj, obj));
                }
            }
        }
#endif

        private WeakDictionary<object, int> CreateDict(out List<KeyObject> keys)
        {
            WeakDictionary<object, int> weakDict = new WeakDictionary<object, int>(this.comparer, refreshInterval: 5);
            keys = new List<KeyObject>();
            for (int i = 0; i < 10; i++)
            {
                KeyObject key = new KeyObject(i);
                keys.Add(key);
                weakDict.Add(key, i);
            }
            return weakDict;
        }

        private object CreateObj(KeyObject obj, RefType valueType)
        {
            switch (valueType)
            {
                case RefType.StrongRef:
                    return obj;
                case RefType.AliveWeak:
                    return new WeakKeyReference<KeyObject>(obj, this.comparer);
                default:
                    return new WeakKeyReference<KeyObject>(this.CreateObj(obj.ID), this.comparer);
            }
        }

        private KeyObject CreateObj(int id)
        {
            return new KeyObject(id);
        }
    }

    internal enum RefType
    {
        StrongRef = 0,
        AliveWeak = 1,
        DeadWeak = 2
    }

    internal class KeyObject
    {
        public KeyObject(int id)
        {
            this.ID = id;
        }
        public int ID { get; set; }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ID == ((KeyObject)obj).ID;
        }
    }
}
