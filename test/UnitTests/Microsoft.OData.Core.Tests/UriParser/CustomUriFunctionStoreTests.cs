//---------------------------------------------------------------------
// <copyright file="CustomUriFunctionsStoreTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser
{
    public class CustomUriFunctionsStoreTests
    {
        private static FunctionSignatureWithReturnType SigDouble(params IEdmTypeReference[] args)
            => new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetDouble(false), args);

        private static FunctionSignatureWithReturnType SigBool(params IEdmTypeReference[] args)
            => new FunctionSignatureWithReturnType(EdmCoreModel.Instance.GetBoolean(false), args);

        private static IEdmTypeReference EdmTInt32 => EdmCoreModel.Instance.GetInt32(false);
        private static IEdmTypeReference EdmTString => EdmCoreModel.Instance.GetString(false);
        private static IEdmTypeReference EdmTDate => EdmCoreModel.Instance.GetDate(false);

        private static IEdmModel NewModel() => new EdmModel();

        [Fact]
        public void GetOrCreate_CreatesAndCachesPerModel()
        {
            var model1 = NewModel();
            var model2 = NewModel();

            var store1a = CustomUriFunctionsStore.GetOrCreate(model1);
            var store1b = CustomUriFunctionsStore.GetOrCreate(model1);
            var store2 = CustomUriFunctionsStore.GetOrCreate(model2);

            Assert.Same(store1a, store1b);
            Assert.NotSame(store1a, store2);
        }

        [Fact]
        public async Task GetOrCreate_IsThreadSafe_ReturnsSameInstance()
        {
            var model = NewModel();
            var bag = new ConcurrentBag<CustomUriFunctionsStore>();

            await Task.WhenAll(Enumerable.Range(0, Environment.ProcessorCount * 4).Select(_ => Task.Run(() =>
            {
                bag.Add(CustomUriFunctionsStore.GetOrCreate(model));
            })));

            Assert.Single(bag.Distinct());
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(null, true)]
        [InlineData("", true)]
        public void TryGet_ArgumentValidation(string functionName, bool ignoreCase)
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("functionName", () =>
                store.TryGet(functionName, ignoreCase, out _));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Add_ArgumentValidation_FunctionName(string functionName)
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("functionName", () =>
                store.Add(functionName, SigDouble(EdmTInt32)));
        }

        [Fact]
        public void Add_ArgumentValidation_FunctionSignature()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("functionSignature", () =>
                store.Add("f", null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Remove_ArgumentValidation_ByName_FunctionName(string functionName)
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("functionName", () =>
                store.Remove(functionName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Remove_ArgumentValidation_ByNameAndSignature_FunctionName(string functionName)
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("functionName", () =>
                store.Remove(functionName, SigDouble(EdmTInt32)));
        }

        [Fact]
        public void Remove_ArgumentValidation_ByNameAndSignature_FunctionSignature()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());

            Assert.Throws<ArgumentNullException>("functionSignature", () =>
                store.Remove("f", null));
        }

        [Fact]
        public void Add_ThenTryGet_CaseSensitive_Succeeds_AndIsReadOnly()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var fn = "myFunc";
            var signature1 = SigDouble(EdmTInt32);

            store.Add(fn, signature1);

            Assert.True(store.TryGet(fn, ignoreCase: false, out var signatures));
            Assert.Single(signatures);
            Assert.Same(signature1, signatures[0]);

            // Ensure returned list is read-only
            var asIList = (IList<FunctionSignatureWithReturnType>)signatures;
            Assert.Throws<NotSupportedException>(() => asIList.Add(SigDouble(EdmTInt32)));
        }

        [Fact]
        public void TryGet_CaseInsensitive_FindsDifferentCasing()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var signature1 = SigDouble(EdmTString);

            store.Add("HelloWorld", signature1);

            Assert.False(store.TryGet("helloworld", ignoreCase: false, out _));
            Assert.True(store.TryGet("helloworld", ignoreCase: true, out var signatures));
            Assert.Single(signatures);
            Assert.Same(signature1, signatures[0]);
        }

        [Fact]
        public void TryGet_NotFound_SetsOutParamToNull()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());

            Assert.False(store.TryGet("doesNotExist", ignoreCase: false, out var signatures));
            Assert.Null(signatures);
        }

        [Fact]
        public void Add_SameInstanceTwice_DoesNotDuplicate()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var fn = "f";
            var signature1 = SigBool(EdmTInt32);

            store.Add(fn, signature1);
            store.Add(fn, signature1); // same reference

            Assert.True(store.TryGet(fn, false, out var signatures));
            Assert.Single(signatures);
            Assert.Same(signature1, signatures[0]);
        }

        [Fact]
        public void Remove_ByName_RemovesAllOverloads()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var fn = "f";
            store.Add(fn, SigDouble(EdmTInt32));
            store.Add(fn, SigDouble(EdmTString));

            Assert.True(store.Remove(fn));
            Assert.False(store.TryGet(fn, false, out _));
            Assert.False(store.Remove(fn)); // idempotent
        }

        [Fact]
        public void Remove_ByNameAndSignature_RemovesOnlyOneOverload()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var fn = "f";
            var signature1 = SigDouble(EdmTInt32);
            var signature2 = SigDouble(EdmTString);
            var signature3 = SigDouble(EdmTDate);

            store.Add(fn, signature1);
            store.Add(fn, signature2);
            store.Add(fn, signature3);

            Assert.True(store.Remove(fn, signature2));

            Assert.True(store.TryGet(fn, false, out var remaining));
            Assert.Equal(2, remaining.Count);
            Assert.Contains(signature1, remaining);
            Assert.Contains(signature3, remaining);
            Assert.DoesNotContain(signature2, remaining);
        }

        [Fact]
        public void Remove_ByNameAndSignature_WithStructurallyEqualButDifferentInstance_Fails()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var fn = "f";
            var signatureStored = SigDouble(EdmTInt32, EdmTString);
            var signatureDifferentReferenceSameShape = SigDouble(EdmTInt32, EdmTString);

            store.Add(fn, signatureStored);

            // Store compares by reference, so removal with different instance should fail.
            Assert.False(store.Remove(fn, signatureDifferentReferenceSameShape));

            Assert.True(store.TryGet(fn, false, out var signatures));
            Assert.Single(signatures);
            Assert.Same(signatureStored, signatures[0]);
        }

        [Fact]
        public void Remove_ByNameAndSignature_WhenNameMissing_ReturnsFalse()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            Assert.False(store.Remove("missing", SigDouble(EdmTInt32)));
        }

        [Fact]
        public void Snapshot_ReturnsReadOnlyLists_AndStableView()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var signatureA1 = SigDouble(EdmTInt32);
            var signatureB1 = SigDouble(EdmTString);

            store.Add("A", signatureA1);
            store.Add("B", signatureB1);

            var snapshot1 = store.Snapshot();
            Assert.Equal(2, snapshot1.Count);
            Assert.True(snapshot1.ContainsKey("A"));
            Assert.True(snapshot1.ContainsKey("B"));
            Assert.Single(snapshot1["A"]);
            Assert.Single(snapshot1["B"]);

            // Lists are read-only
            var listA = (IList<FunctionSignatureWithReturnType>)snapshot1["A"];
            Assert.Throws<NotSupportedException>(() => listA.Add(SigDouble(EdmTDate)));

            // Mutate store after snapshot
            var signatureA2 = SigDouble(EdmTDate);
            store.Add("A", signatureA2);
            store.Add("C", SigBool()); // new key

            // Existing snapshot remains stable
            Assert.Equal(2, snapshot1.Count);
            Assert.Single(snapshot1["A"]); // not affected by later add
            Assert.False(snapshot1.ContainsKey("C"));

            // Fresh snapshot reflects new state
            var snapshot2 = store.Snapshot();
            Assert.Equal(3, snapshot2.Count);
            Assert.Equal(2, snapshot2["A"].Count);
            Assert.Contains(signatureA1, snapshot2["A"]);
            Assert.Contains(signatureA2, snapshot2["A"]);
            Assert.Single(snapshot2["C"]);
        }

        [Fact]
        public async Task Concurrency_AddManyUniqueOverloads_IsSafeAndComplete()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var fn = "f";
            int count = 200;
            var signatures = Enumerable.Range(0, count)
                .Select(i => SigDouble(EdmCoreModel.Instance.GetInt32(false)))
                .ToArray();

            await Task.WhenAll(signatures.Select(signature => Task.Run(() => store.Add(fn, signature))));

            Assert.True(store.TryGet(fn, false, out var functionSignatures));
            Assert.Equal(count, functionSignatures.Count);

            // Verify all references are present
            foreach (var s in signatures)
            {
                Assert.Contains(s, functionSignatures);
            }
        }

        [Fact]
        public async Task Concurrency_AddSameReferenceManyTimes_ResultsInSingleEntry()
        {
            var store = CustomUriFunctionsStore.GetOrCreate(NewModel());
            var fn = "f";
            var signature = SigBool(EdmTString, EdmTInt32);

            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => Task.Run(() => store.Add(fn, signature))));

            Assert.True(store.TryGet(fn, false, out var signatures));
            Assert.Single(signatures);
            Assert.Same(signature, signatures[0]);
        }
    }
}
