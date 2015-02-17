//---------------------------------------------------------------------
// <copyright file="ReadOnlyEnumerableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class ReadOnlyEnumerableTests
    {
        [TestMethod]
        public void EmptyOfTShouldAlwaysReturnTheSameInstance()
        {
            ReadOnlyEnumerable<ODataAction>.Empty().As<object>().Should().BeSameAs(ReadOnlyEnumerable<ODataAction>.Empty());
            ReadOnlyEnumerable<ODataAction>.Empty().As<object>().Should().NotBeSameAs(ReadOnlyEnumerable<ODataFunction>.Empty());
        }

        [TestMethod]
        public void AddToSourceListShouldThrowForEmptySource()
        {
            Action test = () => ReadOnlyEnumerable<int>.Empty().AddToSourceList(1);
            test.ShouldThrow<NotSupportedException>().WithMessage("Collection is read-only.");
        }

        [TestMethod]
        public void AddToSourceListShouldAddToTheSourceList()
        {
            ReadOnlyEnumerable<int> enumerable = new ReadOnlyEnumerable<int>();
            enumerable.Should().BeEmpty();
            enumerable.AddToSourceList(1);
            enumerable.Count().Should().Be(1);
            enumerable.Should().OnlyContain(i => i == 1);
        }

        [TestMethod]
        public void IsEmptyReadOnlyEnumerableShouldFailForNullSource()
        {
            IEnumerable<int> enumerable = null;
            enumerable.IsEmptyReadOnlyEnumerable().Should().BeFalse();
        }

        [TestMethod]
        public void IsEmptyReadOnlyEnumerableShouldFailForNewReadOnlyEnumerable()
        {
            (new ReadOnlyEnumerable<int>()).IsEmptyReadOnlyEnumerable().Should().BeFalse();
        }

        [TestMethod]
        public void IsEmptyReadOnlyEnumerableShouldPassForReadOnlyEnumerableDotEmpty()
        {
            ReadOnlyEnumerable<int>.Empty().IsEmptyReadOnlyEnumerable().Should().BeTrue();
        }

        [TestMethod]
        public void ToReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<int> enumerable = null;
            Action test = () => enumerable.ToReadOnlyEnumerable("Integers");
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [TestMethod]
        public void ToReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => (new List<int>()).ToReadOnlyEnumerable("Integers");
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [TestMethod]
        public void ToReadOnlyEnumerableShouldNotThrowForReadOnlyEnumerableSource()
        {
            (new ReadOnlyEnumerable<int>().ToReadOnlyEnumerable("Integers")).Should().BeEmpty();
        }

        [TestMethod]
        public void GetOrCreateReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<int> enumerable = null;
            Action test = () => enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [TestMethod]
        public void GetOrCreateReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => (new List<int>()).GetOrCreateReadOnlyEnumerable("Integers");
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Integers"));
        }

        [TestMethod]
        public void GetOrCreateReadOnlyEnumerableShouldCreateForEmptyReadOnlyEnumerableSource()
        {
            var enumerable = ReadOnlyEnumerable<int>.Empty();
            var enumerable2 = enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            enumerable2.As<object>().Should().NotBeSameAs(ReadOnlyEnumerable<int>.Empty());
            enumerable2.Should().NotBeNull();
        }

        [TestMethod]
        public void GetOrCreateReadOnlyEnumerableShouldGetForReadOnlyEnumerableSource()
        {
            var enumerable = new ReadOnlyEnumerable<int>();
            var enumerable2 = enumerable.GetOrCreateReadOnlyEnumerable("Integers");
            enumerable2.As<object>().Should().NotBeSameAs(ReadOnlyEnumerable<int>.Empty());
            enumerable2.As<object>().Should().BeSameAs(enumerable);
        }

        [TestMethod]
        public void ConcatToReadOnlyEnumerableShouldThrowForNullSource()
        {
            IEnumerable<ODataProperty> enumerable = null;
            Action test = () => enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Properties"));
        }

        [TestMethod]
        public void ConcatToReadOnlyEnumerableShouldThrowForListSource()
        {
            Action test = () => new List<ODataProperty>().ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ReaderUtils_EnumerableModified("Properties"));
        }

        [TestMethod]
        public void ConcatToReadOnlyEnumerableShouldCreateReadOnlyEnumerableAndAddForEmptySource()
        {
            var enumerable = ReadOnlyEnumerable<ODataProperty>.Empty();
            var enumerable2 = enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            enumerable2.As<object>().Should().NotBeSameAs(enumerable);
            enumerable2.Count().Should().Be(1);
        }

        [TestMethod]
        public void ConcatToReadOnlyEnumerableShouldAddForReadOnlyEnumerableSource()
        {
            var enumerable = new ReadOnlyEnumerable<ODataProperty>();
            enumerable.Count().Should().Be(0);
            enumerable.ConcatToReadOnlyEnumerable("Properties", new ODataProperty());
            enumerable.Count().Should().Be(1);
        }

        [TestMethod]
        public void AddActionShouldAddActionToEntry()
        {
            ODataEntry entry = ReaderUtils.CreateNewEntry();
            entry.Actions.Count().Should().Be(0);
            entry.AddAction(new ODataAction());
            entry.Actions.Count().Should().Be(1);
        }

        [TestMethod]
        public void AddFunctionShouldAddFunctionToEntry()
        {
            ODataEntry entry = ReaderUtils.CreateNewEntry();
            entry.Functions.Count().Should().Be(0);
            entry.AddFunction(new ODataFunction());
            entry.Functions.Count().Should().Be(1);
        }

        [TestMethod]
        public void AddActionShouldAddActionToFeed()
        {
            ODataFeed feed = new ODataFeed();
            feed.Actions.Count().Should().Be(0);
            feed.AddAction(new ODataAction());
            feed.Actions.Count().Should().Be(1);
            feed.AddAction(new ODataAction());
            feed.Actions.Count().Should().Be(2);
        }

        [TestMethod]
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
