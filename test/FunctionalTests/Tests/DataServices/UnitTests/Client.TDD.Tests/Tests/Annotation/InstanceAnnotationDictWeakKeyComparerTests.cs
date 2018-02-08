//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationDictWeakKeyComparerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.TDDUnitTests.Tests.Annotation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Microsoft.OData.Client.Annotation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InstanceAnnotationDictWeakKeyComparerTests
    {
        InstanceAnnotationDictWeakKeyComparer comparer = InstanceAnnotationDictWeakKeyComparer.Default;
        MemberInfo memberInfo = typeof(KeyObject).GetMember("ID").First();
        Random rand = new Random();

        [TestMethod]
        public void TestAddItemIntoDict()
        {
            //Used to make sure all keys are alive in current scope
            List<object> keys;
            var weakDict = CreateDict(out keys);
            Assert.AreEqual(10, weakDict.Count);
        }

        [TestMethod]
        public void TestDictContainsKey()
        {
            List<object> keys;
            var weakDict = CreateDict(out keys);

            foreach (var key in keys)
            {
                Assert.IsTrue(weakDict.ContainsKey(key), "The dictionary should contains the key {0}", key);
            }
        }

        [TestMethod]
        public void TestDictIndex()
        {
            List<object> keys;

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
            List<object> keys;
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
            var weakDict = CreateDictWithoutItem();
            for (int i = 0; i < 2; i++)
            {
                AddKeyObject(weakDict, i);
            }

            for (int i = 0; i < 3; i++)
            {
                AddTupleObject(weakDict, i);
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
            WeakDictionary<object, int> weakDict = new WeakDictionary<object, int>(this.comparer, 5);
            for (int i = 0; i < 2; i++)
            {
                AddKeyObject(weakDict, i);
            }

            for (int i = 0; i < 3; i++)
            {
                AddTupleObject(weakDict, i);
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
            var weakDict = CreateDictWithoutItem();
            for (int i = 0; i < 2; i++)
            {
                AddKeyObject(weakDict, i);
            }

            for (int i = 0; i < 2; i++)
            {
                AddTupleObject(weakDict, i);
            }

            // Force gc to collect dead items
            GC.Collect();
            if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
            {
                var key1 = new Tuple<object, MemberInfo>(new KeyObject(11), memberInfo);
                weakDict.Add(key1, 11);

                var key2 = new KeyObject(12);
                //Weak dictionary will remove dead items during this adding operation.
                //Then the currentRefreshInterval will be 1(still alive)+5=6.
                weakDict.Add(key2, 12);

                Assert.AreEqual(2, weakDict.Count);

                for (int i = 0; i < 3; i++)
                {
                    AddTupleObject(weakDict, i);
                }

                // Force gc to collect dead items
                GC.Collect();
                if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
                {
                    //Add the six item to dictionary.
                    var key6 = new KeyObject(16);
                    //Weak dictionary will not clean dead entities.
                    weakDict.Add(key6, 16);
                    Assert.AreEqual(6, weakDict.Count);

                    var key14 = new Tuple<object, MemberInfo>(new KeyObject(114), memberInfo);
                    //Weak dictionary will remove dead items during this adding operation.
                    //Then the currentRefreshInterval will be 3(still alive)+5=8.
                    weakDict.Add(key14, 114);
                    Assert.AreEqual(4, weakDict.Count);
                }
            }
        }
#endif

        private void AddKeyObject(WeakDictionary<object, int> dict, int id)
        {
            KeyObject key = new KeyObject(id);
            dict.Add(key, rand.Next());
        }

        private void AddTupleObject(WeakDictionary<object, int> dict, int id)
        {
            KeyObject key = new KeyObject(id);
            var tuple = new Tuple<object, MemberInfo>(key, this.memberInfo);
            dict.Add(tuple, rand.Next());
        }

        [TestMethod]
        public void TestAddDiffTupleForSameObjectThrowsException()
        {
            WeakDictionary<object, int> weakDict = new WeakDictionary<object, int>(this.comparer, 5);
            var key = new Tuple<object, MemberInfo>(new KeyObject(10), this.memberInfo);
            weakDict.Add(key, 10);

            var key2 = new Tuple<object, MemberInfo>(key.Item1, this.memberInfo);
            Action testAction = () => { weakDict.Add(key2, 10); };
            testAction.ShouldThrow<ArgumentException>();
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        // Net Core 1.0 is missing GC APIs
        [TestMethod]
        public void TestCompareObjectRef()
        {
            TestCompareKindsOfRef(false);
        }

        [TestMethod]
        public void TestCompareTupleofObjectRef()
        {
            TestCompareKindsOfRef(true);
        }

        private void TestCompareKindsOfRef(bool isTupleObject)
        {
            var array = Enum.GetValues(typeof(RefType));

            foreach (RefType left in array)
            {
                foreach (RefType right in array)
                {
                    TestCompareRefImplement(left, right, isTupleObject);
                }
            }
        }

        private void TestCompareRefImplement(RefType leftRefType, RefType rightRefType, bool isTupleObject)
        {
            KeyObject obj = new KeyObject(1);
            var left = CreateObj(obj, leftRefType, isTupleObject);
            var right = CreateObj(obj, rightRefType, isTupleObject);

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
        public void TestCompareSameObjectRef()
        {
            TestCompareSameRef(false);
        }

        [TestMethod]
        public void TestCompareTupleofSameObject()
        {
            TestCompareSameRef(true);
        }

        private void TestCompareSameRef(bool isTupleObj)
        {
            var array = Enum.GetValues(typeof(RefType));

            foreach (RefType refType in array)
            {
                KeyObject keyObj = new KeyObject(1);
                var obj = CreateObj(keyObj, refType, isTupleObj);

                if (refType != RefType.DeadWeak)
                {
                    Assert.IsTrue(this.comparer.Equals(obj, obj));
                }
                else
                {
                    GC.Collect();
                    if (GC.WaitForFullGCComplete() == GCNotificationStatus.Succeeded)
                    {
                        Assert.IsTrue(this.comparer.Equals(obj, obj));
                    }
                }
            }
        }
#endif

        private object CreateObj(KeyObject obj, RefType RefType, bool isTupleObject)
        {
            return isTupleObject ? CreateTupleObj(obj, RefType) : CreateKeyObj(obj, RefType);
        }

        private object CreateTupleObj(KeyObject obj, RefType RefType)
        {
            switch (RefType)
            {
                case RefType.StrongRef:
                    return new Tuple<object, MemberInfo>(obj, this.memberInfo);
                case RefType.AliveWeak:
                    return this.comparer.CreateKey(new Tuple<object, MemberInfo>(obj, this.memberInfo));
                default:
                    return this.comparer.CreateKey(new Tuple<object, MemberInfo>(this.CreateKeyObj(obj.ID), this.memberInfo));
            }
        }

        private object CreateKeyObj(KeyObject obj, RefType RefType)
        {
            switch (RefType)
            {
                case RefType.StrongRef:
                    return obj;
                case RefType.AliveWeak:
                    return new WeakKeyReference<object>(obj, this.comparer);
                default:
                    return new WeakKeyReference<object>(this.CreateKeyObj(obj.ID), this.comparer);
            }
        }

        private KeyObject CreateKeyObj(int id)
        {
            return new KeyObject(id);
        }

        private WeakDictionary<object, int> CreateDict(out List<object> keys)
        {
            WeakDictionary<object, int> weakDict = CreateDictWithoutItem();

            keys = new List<object>();
            for (int i = 0; i < 5; i++)
            {
                KeyObject key = new KeyObject(i);
                keys.Add(key);
                weakDict.Add(key, 2 * i);
                var tuple = new Tuple<object, MemberInfo>(key, this.memberInfo);
                keys.Add(tuple);
                weakDict.Add(tuple, 2 * i + 1);
            }

            return weakDict;
        }

        private WeakDictionary<object, int> CreateDictWithoutItem()
        {
            return new WeakDictionary<object, int>(InstanceAnnotationDictWeakKeyComparer.Default, 5)
            {
                RemoveCollectedEntriesRules = new List<Func<object, bool>>
                {
                    InstanceAnnotationDictWeakKeyComparer.Default.RemoveRule
                },
                CreateWeakKey = InstanceAnnotationDictWeakKeyComparer.Default.CreateKey
            };
        }
    }
}
