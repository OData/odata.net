//---------------------------------------------------------------------
// <copyright file="ReadOnlyEnumerableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests
{
    public class ReadOnlyEnumerableTests
    {
        [Fact]
        public void EmptyOfTShouldAlwaysReturnTheSameInstance()
        {
            ReadOnlyEnumerable<ODataAction>.Empty().As<object>().Should().BeSameAs(ReadOnlyEnumerable<ODataAction>.Empty());
            ReadOnlyEnumerable<ODataAction>.Empty().As<object>().Should().NotBeSameAs(ReadOnlyEnumerable<ODataFunction>.Empty());
        }

        [Fact]
        public void AddToSourceListShouldThrowForEmptySource()
        {
            Action test = () => ReadOnlyEnumerable<int>.Empty().AddToSourceList(1);
            test.ShouldThrow<NotSupportedException>().WithMessage("Collection is read-only.");
        }

        [Fact]
        public void AddToSourceListShouldAddToTheSourceList()
        {
            ReadOnlyEnumerable<int> enumerable = new ReadOnlyEnumerable<int>();
            enumerable.Should().BeEmpty();
            enumerable.AddToSourceList(1);
            enumerable.Count().Should().Be(1);
            enumerable.Should().OnlyContain(i => i == 1);
        }

        [Fact]
        public void IsEmptyReadOnlyEnumerableShouldFailForNullSource()
        {
            IEnumerable<int> enumerable = null;
            enumerable.IsEmptyReadOnlyEnumerable().Should().BeFalse();
        }

        [Fact]
        public void IsEmptyReadOnlyEnumerableShouldFailForNewReadOnlyEnumerable()
        {
            (new ReadOnlyEnumerable<int>()).IsEmptyReadOnlyEnumerable().Should().BeFalse();
        }

        [Fact]
        public void IsEmptyReadOnlyEnumerableShouldPassForReadOnlyEnumerableDotEmpty()
        {
            ReadOnlyEnumerable<int>.Empty().IsEmptyReadOnlyEnumerable().Should().BeTrue();
        }

        [Fact]
        public void ToReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<int> enumerable = null;
            Action test = () => enumerable.ToReadOnlyEnumerable("Integers");
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [Fact]
        public void ToReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => (new List<int>()).ToReadOnlyEnumerable("Integers");
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [Fact]
        public void ToReadOnlyEnumerableShouldNotThrowForReadOnlyEnumerableSource()
        {
            (new ReadOnlyEnumerable<int>().ToReadOnlyEnumerable("Integers")).Should().BeEmpty();
        }

        [Fact]
        public void GetOrCreateReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<int> enumerable = null;
            Action test = () => enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [Fact]
        public void GetOrCreateReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => (new List<int>()).GetOrCreateReadOnlyEnumerable("Integers");
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [Fact]
        public void GetOrCreateReadOnlyEnumerableShouldCreateForEmptyReadOnlyEnumerableSource()
        {
            var enumerable = ReadOnlyEnumerable<int>.Empty();
            var enumerable2 = enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            enumerable2.As<object>().Should().NotBeSameAs(ReadOnlyEnumerable<int>.Empty());
            enumerable2.Should().NotBeNull();
        }

        [Fact]
        public void GetOrCreateReadOnlyEnumerableShouldGetForReadOnlyEnumerableSource()
        {
            var enumerable = new ReadOnlyEnumerable<int>();
            var enumerable2 = enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            enumerable2.As<object>().Should().NotBeSameAs(ReadOnlyEnumerable<int>.Empty());
            enumerable2.As<object>().Should().BeSameAs(enumerable);
        }

        [Fact]
        public void ConcatToReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<ODataProperty> enumerable = null;
            Action test = () => enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Properties"));
        }

        [Fact]
        public void ConcatToReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => new List<ODataProperty>().ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Properties"));
        }

        [Fact]
        public void ConcatToReadOnlyEnumerableShouldCreateReadOnlyEnumerableAndAddForEmptySource()
        {
            var enumerable = ReadOnlyEnumerable<ODataProperty>.Empty();
            var enumerable2 = enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            enumerable2.As<object>().Should().NotBeSameAs(enumerable);
            enumerable2.Count().Should().Be(1);
        }

        [Fact]
        public void ConcatToReadOnlyEnumerableShouldAddForReadOnlyEnumerableSource()
        {
            var enumerable = new ReadOnlyEnumerable<ODataProperty>();
            enumerable.Count().Should().Be(0);
            enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            enumerable.Count().Should().Be(1);
        }

        [Fact]
        public void AddActionShouldAddActionToEntry()
        {
            ODataEntry entry = ReaderUtils.CreateNewEntry();
            entry.Actions.Count().Should().Be(0);
            entry.AddAction(new ODataAction());
            entry.Actions.Count().Should().Be(1);
        }

        [Fact]
        public void AddFunctionShouldAddFunctionToEntry()
        {
            ODataEntry entry = ReaderUtils.CreateNewEntry();
            entry.Functions.Count().Should().Be(0);
            entry.AddFunction(new ODataFunction());
            entry.Functions.Count().Should().Be(1);
        }

        [Fact]
        public void AddActionShouldAddActionToFeed()
        {
            ODataFeed feed = new ODataFeed();
            feed.Actions.Count().Should().Be(0);
            feed.AddAction(new ODataAction());
            feed.Actions.Count().Should().Be(1);
            feed.AddAction(new ODataAction());
            feed.Actions.Count().Should().Be(2);
        }

        [Fact]
        public void AddFunctionShouldAddFunctionToFeed()
        {
            ODataFeed feed = new ODataFeed();
            feed.Functions.Count().Should().Be(0);
            feed.AddFunction(new ODataFunction());
            feed.Functions.Count().Should().Be(1);
            feed.AddFunction(new ODataFunction());
            feed.Functions.Count().Should().Be(2);
        }
    }
}
