//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralPrefixesStoreTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser
{
    public class CustomUriLiteralPrefixesStoreTests
    {
        private static IEdmModel NewModel() => new EdmModel();

        private static IEdmTypeReference EdmTString(bool nullable = false) => EdmCoreModel.Instance.GetString(nullable);
        private static IEdmTypeReference EdmTInt32(bool nullable = false) => EdmCoreModel.Instance.GetInt32(nullable);
        private static IEdmTypeReference EdmTBoolean(bool nullable = false) => EdmCoreModel.Instance.GetBoolean(nullable);

        [Fact]
        public void GetOrCreate_CreatesAndCachesPerModel()
        {
            var model1 = NewModel();
            var model2 = NewModel();

            var store1a = CustomUriLiteralPrefixesStore.GetOrCreate(model1);
            var store1b = CustomUriLiteralPrefixesStore.GetOrCreate(model1);
            var store2 = CustomUriLiteralPrefixesStore.GetOrCreate(model2);

            Assert.Same(store1a, store1b);
            Assert.NotSame(store1a, store2);
        }

        [Fact]
        public async Task GetOrCreate_IsThreadSafe_ReturnsSameInstance()
        {
            var model = NewModel();
            var bag = new ConcurrentBag<CustomUriLiteralPrefixesStore>();

            await Task.WhenAll(Enumerable.Range(0, Environment.ProcessorCount * 4).Select(_ => Task.Run(() =>
            {
                bag.Add(CustomUriLiteralPrefixesStore.GetOrCreate(model));
            })));

            Assert.Single(bag.Distinct());
        }

        [Fact]
        public void GetOrCreate_NullModel_Throws()
        {
            Assert.Throws<ArgumentNullException>("model", () => CustomUriLiteralPrefixesStore.GetOrCreate(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void TryGet_ArgumentValidation(string literalPrefix)
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("literalPrefix", () =>
                store.TryGet(literalPrefix, out _));
        }

        [Fact]
        public void TryGet_NotFound_ReturnsFalseAndOutNull()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());

            Assert.False(store.TryGet("unknown", out var t));
            Assert.Null(t);
        }

        [Fact]
        public void TryGet_Found_ReturnsTrueAndType()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var prefix = "str";
            var type = EdmTString();

            Assert.True(store.Add(prefix, type));

            Assert.True(store.TryGet(prefix, out var resolved));
            Assert.Same(type, resolved);
        }

        [Fact]
        public void TryGet_IsCaseSensitive()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var prefix = "MyPrefix";
            var type = EdmTInt32();

            Assert.True(store.Add(prefix, type));
            Assert.True(store.TryGet("MyPrefix", out var resolved));
            Assert.Same(type, resolved);

            Assert.False(store.TryGet("myprefix", out _)); // different casing should not match
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Add_ArgumentValidation_LiteralPrefix(string literalPrefix)
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("literalPrefix", () =>
                store.Add(literalPrefix, EdmTString()));
        }

        [Theory]
        [InlineData("p")]
        [InlineData("prefix")]
        public void Add_ArgumentValidation_Type(string literalPrefix)
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("edmType", () =>
                store.Add(literalPrefix, null));
        }

        [Fact]
        public void Add_AddsNewPrefixOnce_ReturnsTrue()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var prefix = "p";
            var type = EdmTString();

            Assert.True(store.Add(prefix, type));
            Assert.True(store.TryGet(prefix, out var resolved));
            Assert.Same(type, resolved);
        }

        [Fact]
        public void Add_Duplicate_ReturnsFalse_AndDoesNotReplaceExistingType()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var prefix = "dup";
            var type1 = EdmTString();
            var type2 = EdmTInt32();

            Assert.True(store.Add(prefix, type1));
            Assert.False(store.Add(prefix, type2)); // duplicate key, should not change type

            Assert.True(store.TryGet(prefix, out var resolved));
            Assert.Same(type1, resolved);
        }

        [Fact]
        public void Add_CaseSensitive_AllowsDifferentCasingAsDistinctKeys()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var type = EdmTBoolean();

            Assert.True(store.Add("abc", type));
            Assert.True(store.Add("ABC", type)); // distinct key due to case-sensitivity

            Assert.True(store.TryGet("abc", out var t1));
            Assert.True(store.TryGet("ABC", out var t2));
            Assert.Same(type, t1);
            Assert.Same(type, t2);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Remove_ArgumentValidation(string literalPrefix)
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("literalPrefix", () =>
                store.Remove(literalPrefix));
        }

        [Fact]
        public void Remove_NotExisting_ReturnsFalse()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());

            Assert.False(store.Remove("missing"));
        }

        [Fact]
        public void Remove_Existing_ReturnsTrue_AndRemoves()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var prefix = "del";
            var type = EdmTString();

            Assert.True(store.Add(prefix, type));

            Assert.True(store.Remove(prefix));
            Assert.False(store.TryGet(prefix, out _));
            Assert.False(store.Remove(prefix)); // idempotent second remove
        }

        [Fact]
        public void Remove_IsCaseSensitive_DoesNotRemoveDifferentCasing()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var prefix = "CaseKey";
            var type = EdmTString();

            Assert.True(store.Add(prefix, type));
            Assert.False(store.Remove("casekey"));
            Assert.True(store.TryGet(prefix, out var resolved));
            Assert.Same(type, resolved);
        }

        [Fact]
        public void Snapshot_ReturnsPointInTimeImmutableView()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var aType = EdmTString();
            var bType = EdmTInt32();

            Assert.True(store.Add("A", aType));
            Assert.True(store.Add("B", bType));

            var snapshot1 = store.Snapshot();
            Assert.Equal(2, snapshot1.Count);
            Assert.True(snapshot1.ContainsKey("A"));
            Assert.True(snapshot1.ContainsKey("B"));
            Assert.Same(aType, snapshot1["A"]);
            Assert.Same(bType, snapshot1["B"]);

            // Using the concrete type's Add returns a new dictionary and should not affect the store
            var concrete = Assert.IsAssignableFrom<ImmutableDictionary<string, IEdmTypeReference>>(snapshot1);
            var mutatedView = concrete.Add("C", EdmTBoolean());
            Assert.Equal(3, mutatedView.Count);
            Assert.Equal(2, snapshot1.Count); // original snapshot unchanged

            // Mutate the store after taking the snapshot
            Assert.True(store.Remove("A"));
            Assert.True(store.Add("C", EdmTBoolean()));

            // snapshot1 is stable (point-in-time)
            Assert.Equal(2, snapshot1.Count);
            Assert.True(snapshot1.ContainsKey("A"));
            Assert.False(snapshot1.ContainsKey("C"));

            // new snapshot reflects updates
            var snapshot2 = store.Snapshot();
            Assert.False(snapshot2.ContainsKey("A"));
            Assert.True(snapshot2.ContainsKey("C"));
        }

        [Fact]
        public async Task Concurrency_AddManyUniquePrefixes_IsSafeAndComplete()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            int count = 200;
            var type = EdmTString();

            await Task.WhenAll(Enumerable.Range(0, count).Select(i => Task.Run(() =>
            {
                store.Add("p_" + i, type);
            })));

            var snapshot = store.Snapshot();
            Assert.True(Enumerable.Range(0, count).All(i => snapshot.ContainsKey("p_" + i)));
            Assert.Equal(count, snapshot.Count);
        }

        [Fact]
        public async Task Concurrency_AddSamePrefixManyTimes_EndsWithSingleEntry()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var prefix = "dup";
            var type = EdmTInt32();

            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => Task.Run(() =>
            {
                store.Add(prefix, type);
            })));

            Assert.True(store.TryGet(prefix, out var resolved));
            Assert.Same(type, resolved);

            // Ensure only one key exists with that name
            var snapshot = store.Snapshot();
            Assert.True(snapshot.ContainsKey(prefix));
            Assert.Single(snapshot.Where(kv => kv.Key == prefix));
        }

        [Fact]
        public async Task Concurrency_RemoveManyTimes_AfterSingleAdd_ResultIsRemoved()
        {
            var store = CustomUriLiteralPrefixesStore.GetOrCreate(NewModel());
            var prefix = "toRemove";
            Assert.True(store.Add(prefix, EdmTString()));

            await Task.WhenAll(Enumerable.Range(0, 64).Select(_ => Task.Run(() =>
            {
                store.Remove(prefix);
            })));

            Assert.False(store.TryGet(prefix, out _));
        }
    }
}
