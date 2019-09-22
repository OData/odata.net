//---------------------------------------------------------------------
// <copyright file="ReadOnlyEnumerableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests
{
    public class ReadOnlyEnumerableTests
    {
        [Fact]
        public void EmptyOfTShouldAlwaysReturnTheSameInstance()
        {
            Assert.Same(ReadOnlyEnumerable<ODataAction>.Empty(), ReadOnlyEnumerable<ODataAction>.Empty());
            Assert.NotSame(ReadOnlyEnumerable<ODataAction>.Empty(), ReadOnlyEnumerable<ODataFunction>.Empty());
        }

        [Fact]
        public void AddToSourceListShouldThrowForEmptySource()
        {
            Action test = () => ReadOnlyEnumerable<int>.Empty().AddToSourceList(1);
            test.Throws<NotSupportedException>("Collection is read-only.");
        }

        [Fact]
        public void AddToSourceListShouldAddToTheSourceList()
        {
            ReadOnlyEnumerable<int> enumerable = new ReadOnlyEnumerable<int>();
            Assert.Empty(enumerable);
            enumerable.AddToSourceList(1);
            int value = Assert.Single(enumerable);
            Assert.Equal(1, value);
        }

        [Fact]
        public void IsEmptyReadOnlyEnumerableShouldFailForNullSource()
        {
            IEnumerable<int> enumerable = null;
            var result = enumerable.IsEmptyReadOnlyEnumerable();
            Assert.False(result);
        }

        [Fact]
        public void IsEmptyReadOnlyEnumerableShouldFailForNewReadOnlyEnumerable()
        {
            var result = (new ReadOnlyEnumerable<int>()).IsEmptyReadOnlyEnumerable();
            Assert.False(result);
        }

        [Fact]
        public void IsEmptyReadOnlyEnumerableShouldPassForReadOnlyEnumerableDotEmpty()
        {
            var result = ReadOnlyEnumerable<int>.Empty().IsEmptyReadOnlyEnumerable();
            Assert.True(result);
        }

        [Fact]
        public void ToReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<int> enumerable = null;
            Action test = () => enumerable.ToReadOnlyEnumerable("Integers");
            test.Throws<ODataException>(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [Fact]
        public void ToReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => (new List<int>()).ToReadOnlyEnumerable("Integers");
            test.Throws<ODataException>(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [Fact]
        public void ToReadOnlyEnumerableShouldNotThrowForReadOnlyEnumerableSource()
        {
            var result = new ReadOnlyEnumerable<int>().ToReadOnlyEnumerable("Integers");
            Assert.Empty(result);
        }

        [Fact]
        public void GetOrCreateReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<int> enumerable = null;
            Action test = () => enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            test.Throws<ODataException>(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [Fact]
        public void GetOrCreateReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => (new List<int>()).GetOrCreateReadOnlyEnumerable("Integers");
            test.Throws<ODataException>(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [Fact]
        public void GetOrCreateReadOnlyEnumerableShouldCreateForEmptyReadOnlyEnumerableSource()
        {
            var enumerable = ReadOnlyEnumerable<int>.Empty();
            var enumerable2 = enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            Assert.NotSame(enumerable2, ReadOnlyEnumerable<int>.Empty());
            Assert.NotNull(enumerable2);
        }

        [Fact]
        public void GetOrCreateReadOnlyEnumerableShouldGetForReadOnlyEnumerableSource()
        {
            var enumerable = new ReadOnlyEnumerable<int>();
            var enumerable2 = enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            Assert.NotSame(enumerable2, ReadOnlyEnumerable<int>.Empty());
            Assert.Same(enumerable2, enumerable);
        }

        [Fact]
        public void ConcatToReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<ODataProperty> enumerable = null;
            Action test = () => enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            test.Throws<ODataException>(ErrorStrings.ReaderUtils_EnumerableModified("Properties"));
        }

        [Fact]
        public void ConcatToReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => new List<ODataProperty>().ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            test.Throws<ODataException>(ErrorStrings.ReaderUtils_EnumerableModified("Properties"));
        }

        [Fact]
        public void ConcatToReadOnlyEnumerableShouldCreateReadOnlyEnumerableAndAddForEmptySource()
        {
            var enumerable = ReadOnlyEnumerable<ODataProperty>.Empty();
            var enumerable2 = enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            Assert.NotSame(enumerable2, enumerable);
            Assert.Single(enumerable2);
        }

        [Fact]
        public void ConcatToReadOnlyEnumerableShouldAddForReadOnlyEnumerableSource()
        {
            var enumerable = new ReadOnlyEnumerable<ODataProperty>();
            Assert.Empty(enumerable);
            enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            Assert.Single(enumerable);
        }

        [Fact]
        public void AddActionShouldAddActionToEntry()
        {
            ODataResource entry = ReaderUtils.CreateNewResource();
            Assert.Empty(entry.Actions);
            entry.AddAction(new ODataAction());
            Assert.Single(entry.Actions);
        }

        [Fact]
        public void AddFunctionShouldAddFunctionToEntry()
        {
            ODataResource entry = ReaderUtils.CreateNewResource();
            Assert.Empty(entry.Functions);
            entry.AddFunction(new ODataFunction());
            Assert.Single(entry.Functions);
        }

        [Fact]
        public void AddActionShouldAddActionToFeed()
        {
            ODataResourceSet feed = new ODataResourceSet();
            Assert.Empty(feed.Actions);
            feed.AddAction(new ODataAction());
            Assert.Single(feed.Actions);
            feed.AddAction(new ODataAction());
            Assert.Equal(2, feed.Actions.Count());
        }

        [Fact]
        public void AddFunctionShouldAddFunctionToFeed()
        {
            ODataResourceSet feed = new ODataResourceSet();
            Assert.Empty(feed.Functions);
            feed.AddFunction(new ODataFunction());
            Assert.Single(feed.Functions);
            feed.AddFunction(new ODataFunction());
            Assert.Equal(2, feed.Functions.Count());
        }
    }
}
