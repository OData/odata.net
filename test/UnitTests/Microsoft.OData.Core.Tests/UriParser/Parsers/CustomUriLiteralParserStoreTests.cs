//---------------------------------------------------------------------
// <copyright file="CustomUriLiteralParsersStoreTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    public class CustomUriLiteralParsersStoreTests
    {
        private static IEdmModel NewModel() => new EdmModel();

        private static IEdmTypeReference EdmTString(bool nullable = false) => EdmCoreModel.Instance.GetString(nullable);
        private static IEdmTypeReference EdmTInt32(bool nullable = false) => EdmCoreModel.Instance.GetInt32(nullable);
        private static IEdmTypeReference EdmTBoolean(bool nullable = false) => EdmCoreModel.Instance.GetBoolean(nullable);
        private static IEdmTypeReference EdmTDate(bool nullable = false) => EdmCoreModel.Instance.GetDate(nullable);

        private sealed class DummyUriLiteralParser : IUriLiteralParser
        {
            private readonly string _name;
            public DummyUriLiteralParser(string name) => _name = name;

            public object ParseUriStringToType(string text, IEdmTypeReference targetType, out UriLiteralParsingException parsingException)
            {
                parsingException = null;
                return $"{_name}:{text}:{targetType?.FullName()}";
            }

            public override string ToString() => $"DummyUriLiteralParser({_name})";
        }

        [Fact]
        public void GetOrCreate_CreatesAndCachesPerModel()
        {
            var model1 = NewModel();
            var model2 = NewModel();

            var store1a = CustomUriLiteralParsersStore.GetOrCreate(model1);
            var store1b = CustomUriLiteralParsersStore.GetOrCreate(model1);
            var store2 = CustomUriLiteralParsersStore.GetOrCreate(model2);

            Assert.Same(store1a, store1b);
            Assert.NotSame(store1a, store2);
        }

        [Fact]
        public async Task GetOrCreate_IsThreadSafe_ReturnsSameInstance()
        {
            var model = NewModel();
            var bag = new ConcurrentBag<CustomUriLiteralParsersStore>();

            await Task.WhenAll(Enumerable.Range(0, Environment.ProcessorCount * 4).Select(_ => Task.Run(() =>
            {
                bag.Add(CustomUriLiteralParsersStore.GetOrCreate(model));
            })));

            Assert.Single(bag.Distinct());
        }

        [Fact]
        public void GetOrCreate_NullModel_Throws()
        {
            Assert.Throws<ArgumentNullException>("model", () => CustomUriLiteralParsersStore.GetOrCreate(null));
        }

        [Fact]
        public void Contains_ArgumentValidation()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            Assert.Throws<ArgumentNullException>("parser", () => store.Contains(null));
        }

        [Fact]
        public void Contains_ReflectsUnboundParsers()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("p");

            Assert.False(store.Contains(parser));
            Assert.True(store.Add(parser));
            Assert.True(store.Contains(parser));
            Assert.True(store.Remove(parser));
            Assert.False(store.Contains(parser));
        }

        [Fact]
        public void TryGet_ArgumentValidation()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            Assert.Throws<ArgumentNullException>("edmTypeReference", () => store.TryGet(null, out _));
        }

        [Fact]
        public void TryGet_NotFound_ReturnsFalseAndOutNull()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            Assert.False(store.TryGet(EdmTString(), out var parser));
            Assert.Null(parser);
        }

        [Fact]
        public void TryGet_Found_ReturnsTrue_UsingSameReferenceKey()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var type = EdmTString(false);
            var parser = new DummyUriLiteralParser("stringParser");

            Assert.True(store.Add(type, parser));
            Assert.True(store.TryGet(type, out var resolved));
            Assert.Same(parser, resolved);
        }

        [Fact]
        public void TryGet_Found_ReturnsTrue_WithStructurallyEquivalentKey()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var keyA = EdmTString(false);
            var keyB = EdmTString(false); // equivalent but not necessarily same reference
            var parser = new DummyUriLiteralParser("stringParser");

            Assert.True(store.Add(keyA, parser));
            Assert.True(store.TryGet(keyB, out var resolved));
            Assert.Same(parser, resolved);
        }

        [Fact]
        public void Add_Unbound_ArgumentValidation()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            Assert.Throws<ArgumentNullException>("parser", () => store.Add(null));
        }

        [Fact]
        public void Add_Unbound_AddsOnce_AndReturnsFalseOnDuplicate()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("p");

            Assert.True(store.Add(parser));
            Assert.False(store.Add(parser)); // duplicate should be ignored

            var set = store.Snapshot();
            Assert.True(set.Contains(parser));
            Assert.Equal(1, set.Count);
        }

        [Theory]
        [InlineData(true, false)] // nullable, non-nullable are not equivalent
        [InlineData(false, true)]
        public void Add_TypeSpecific_NullabilityAffectsKeyEquivalence(bool nullableFirst, bool nullableSecond)
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("p");

            var type1 = EdmTString(nullableFirst);
            var type2 = EdmTString(nullableSecond);

            Assert.True(store.Add(type1, parser));
            // If not equivalent keys, second add should succeed; otherwise should fail
            bool equivalent = nullableFirst == nullableSecond;
            bool add2 = store.Add(type2, parser);
            Assert.Equal(!equivalent, add2);
        }

        [Fact]
        public void Add_TypeSpecific_ArgumentValidation()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var p = new DummyUriLiteralParser("p");

            Assert.Throws<ArgumentNullException>("edmTypeReference", () => store.Add(null, p));
            Assert.Throws<ArgumentNullException>("parser", () => store.Add(EdmTInt32(), null));
        }

        [Fact]
        public void Add_TypeSpecific_AddsOnce_AndDoesNotReplaceExisting()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var keyA = EdmTInt32(false);
            var parser1 = new DummyUriLiteralParser("p1");
            var parser2 = new DummyUriLiteralParser("p2");

            Assert.True(store.Add(keyA, parser1));

            // Equivalent key should be considered duplicate; value must not be replaced
            var equivalentKey = EdmTInt32(false);
            Assert.False(store.Add(equivalentKey, parser2));

            Assert.True(store.TryGet(keyA, out var resolved));
            Assert.Same(parser1, resolved);
        }

        [Fact]
        public void Remove_Unbound_ArgumentValidation()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            Assert.Throws<ArgumentNullException>("parser", () => store.Remove((IUriLiteralParser)null));
        }

        [Fact]
        public void Remove_Unbound_NotExisting_ReturnsFalse()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("p");

            Assert.False(store.Remove(parser));
        }

        [Fact]
        public void Remove_Unbound_Existing_ReturnsTrue_AndRemoves()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("p");

            Assert.True(store.Add(parser));
            Assert.True(store.Remove(parser));
            Assert.False(store.Snapshot().Contains(parser));
            Assert.False(store.Remove(parser)); // idempotent
        }

        [Fact]
        public void Remove_Unbound_AlsoRemovesAllTypeSpecificMappingsPointingToParser()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("shared");
            var type1 = EdmTString(false);
            var type2 = EdmTBoolean(false);

            Assert.True(store.Add(parser));             // unbound
            Assert.True(store.Add(type1, parser));         // bound
            Assert.True(store.Add(type2, parser));         // bound

            // Sanity
            Assert.True(store.Contains(parser));
            Assert.True(store.TryGet(type1, out var _));
            Assert.True(store.TryGet(type2, out var __));

            // Remove parser should remove all
            Assert.True(store.Remove(parser));

            Assert.False(store.Contains(parser));
            Assert.False(store.TryGet(type1, out _));
            Assert.False(store.TryGet(type2, out _));
        }

        [Fact]
        public void Remove_TypeSpecific_ArgumentValidation()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            Assert.Throws<ArgumentNullException>("edmTypeReference", () => store.Remove((IEdmTypeReference)null));
        }

        [Fact]
        public void Remove_TypeSpecific_NotExisting_ReturnsFalse()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            Assert.False(store.Remove(EdmTDate()));
        }

        [Fact]
        public void Remove_TypeSpecific_Existing_ReturnsTrue_AndDoesNotAffectUnbound()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var type = EdmTDate(false);
            var parser = new DummyUriLiteralParser("p");

            Assert.True(store.Add(parser));        // unbound
            Assert.True(store.Add(type, parser));     // type-specific

            Assert.True(store.Remove(type));     // remove mapping by type
            Assert.False(store.TryGet(type, out _));
            Assert.True(store.Contains(parser));   // unbound parser remains
        }

        [Fact]
        public void Remove_TypeSpecific_WithEquivalentTypeKey_Succeeds()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var keyA = EdmTInt32(false);
            var parser = new DummyUriLiteralParser("p");

            Assert.True(store.Add(keyA, parser));
            var equivalentKey = EdmTInt32(false);

            Assert.True(store.Remove(equivalentKey));
            Assert.False(store.TryGet(keyA, out _));
        }

        [Fact]
        public void Snapshot_SetsAndTypeMap_ArePointInTimeAndImmutable()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser1 = new DummyUriLiteralParser("p1");
            var parser2 = new DummyUriLiteralParser("p2");

            var typeA = EdmTString(false);
            var typeB = EdmTInt32(false);

            Assert.True(store.Add(parser1));
            Assert.True(store.Add(typeA, parser1));
            Assert.True(store.Add(typeB, parser2));

            var setSnapshot1 = store.Snapshot();
            var mapSnapshot1 = store.SnapshotByEdmType();

            Assert.Single(setSnapshot1);
            Assert.Equal(2, mapSnapshot1.Count);
            Assert.True(mapSnapshot1.ContainsKey(typeA));
            Assert.True(mapSnapshot1.ContainsKey(typeB));

            // Mutate store after snapshot
            Assert.True(store.Add(parser2));
            Assert.True(store.Remove(typeA));

            // Old snapshots unchanged
            Assert.Single(setSnapshot1);
            Assert.Equal(2, mapSnapshot1.Count);
            Assert.True(mapSnapshot1.ContainsKey(typeA));

            // New snapshots reflect updates
            var setSnapshot2 = store.Snapshot();
            var mapSnapshot2 = store.SnapshotByEdmType();
            Assert.Equal(2, setSnapshot2.Count);
            Assert.False(mapSnapshot2.ContainsKey(typeA));

            // Verify underlying are immutable collections
            var mapConcrete = Assert.IsAssignableFrom<ImmutableDictionary<IEdmTypeReference, IUriLiteralParser>>(mapSnapshot1);
            var setConcrete = Assert.IsAssignableFrom<IReadOnlySet<IUriLiteralParser>>(setSnapshot1);
            // ImmutableDictionary.Add returns a new instance (no mutation)
            var mapMutated = mapConcrete.Add(EdmTBoolean(false), parser1);
            Assert.Equal(3, mapMutated.Count);
            Assert.Equal(2, mapConcrete.Count);
            // IReadOnlySet cannot be mutated; just ensure we can enumerate
            Assert.Contains(parser1, setConcrete);
        }

        [Fact]
        public async Task Concurrency_AddManyUnbound_IsSafeAndComplete()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            int count = 200;
            var parsers = Enumerable.Range(0, count).Select(i => new DummyUriLiteralParser("p" + i)).ToArray();

            await Task.WhenAll(parsers.Select(p => Task.Run(() => store.Add(p))));

            var setSnap = store.Snapshot();
            Assert.Equal(count, setSnap.Count);
            foreach (var p in parsers)
            {
                Assert.True(setSnap.Contains(p));
            }
        }

        [Fact]
        public async Task Concurrency_AddSameUnboundManyTimes_EndsWithSingleEntry()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("dup");

            await Task.WhenAll(Enumerable.Range(0, 128).Select(_ => Task.Run(() => store.Add(parser))));

            var setSnap = store.Snapshot();
            Assert.Single(setSnap);
            Assert.Contains(parser, setSnap);
        }

        [Fact]
        public async Task Concurrency_AddManyTypeSpecific_IsSafeAndComplete()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("p");
            int count = 200;

            await Task.WhenAll(Enumerable.Range(0, count).Select(i => Task.Run(() =>
            {
                var type = EdmCoreModel.Instance.GetString(false); // equal but not necessarily same ref; use unique by adding suffix
                // Make unique by varying nullability every other key + use int as well
                var t = (i % 2 == 0) ? EdmCoreModel.Instance.GetString(false) : EdmCoreModel.Instance.GetInt32(false);
                // Also append collection variation by using string/int only; key comparer uses FullName+nullability; this creates two families of keys
                store.Add(new EdmCollectionTypeReference(new EdmCollectionType(t)), parser);
            })));

            var map = store.SnapshotByEdmType();
            Assert.True(map.Count >= 2); // at least two families of keys (string[] and int[])
        }

        [Fact]
        public async Task Concurrency_RemoveUnboundManyTimes_AfterSingleAdd_IsRemoved()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var parser = new DummyUriLiteralParser("toRemove");
            Assert.True(store.Add(parser));

            await Task.WhenAll(Enumerable.Range(0, 64).Select(_ => Task.Run(() => store.Remove(parser))));

            Assert.False(store.Snapshot().Contains(parser));
        }

        [Fact]
        public async Task Concurrency_RemoveTypeSpecificManyTimes_AfterSingleAdd_IsRemoved()
        {
            var store = CustomUriLiteralParsersStore.GetOrCreate(NewModel());
            var type = EdmTBoolean(false);
            var parser = new DummyUriLiteralParser("p");
            Assert.True(store.Add(type, parser));

            await Task.WhenAll(Enumerable.Range(0, 64).Select(_ => Task.Run(() => store.Remove(type))));

            Assert.False(store.TryGet(type, out _));
        }
    }
}
