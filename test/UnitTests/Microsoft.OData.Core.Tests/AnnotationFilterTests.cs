//---------------------------------------------------------------------
// <copyright file="AnnotationFilterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class AnnotationFilterTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CreateWithNullOrEmptyShouldCreateExcludeAllFilter(string filterString)
        {
            var filter = AnnotationFilter.Create(filterString);
            Assert.False(filter.Matches("any.any"));
        }

        [Theory]
        [InlineData("*,-*")]
        [InlineData("-ns.name,-*,*")]
        public void CreateWithExcludeAllPatternShouldCreateExcludeAllFilter(string filterString)
        {
            var filter = AnnotationFilter.Create(filterString);
            Assert.False(filter.Matches("any.any"));
            Assert.False(filter.Matches("ns.name"));
        }

        [Fact]
        public void CreateWithNoIncludePatternShouldCreateExcludeAllFilter()
        {
            var filter = AnnotationFilter.Create("-ns1.name,-ns2.*");
            Assert.False(filter.Matches("any.any"));
            Assert.False(filter.Matches("ns1.name"));
            Assert.False(filter.Matches("ns2.any"));
        }

        [Fact]
        public void CreateWithWildCardShouldCreateIncludeAllFilter()
        {
            var filter = AnnotationFilter.Create("*");
            Assert.True(filter.Matches("any.any"));
        }

        [Fact]
        public void TestCreateIncludeAllFilter()
        {
            var filter = AnnotationFilter.CreateIncludeAllFilter();
            Assert.True(filter.Matches("any.any"));
            Assert.Equal(filter.GetType(), AnnotationFilter.Create("*").GetType());
        }

        [Fact]
        public void CreateWithWildCardAndNoExcludePatternShouldCreateIncludeAllFilter()
        {
            var filter = AnnotationFilter.Create("ns.name,*");
            Assert.True(filter.Matches("any.any"));
            Assert.True(filter.Matches("ns.name"));
        }

        [Fact]
        public void ExcludeAllMatchesShouldThrowOnNullAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("-*").Matches(null);
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("annotationName", exception.Message);
        }

        [Fact]
        public void ExcludeAllMatchesShouldThrowOnEmptyAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("-*").Matches("");
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("annotationName", exception.Message);
        }

        [Fact]
        public void IncludeAllMatchesShouldThrowOnNullAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("*").Matches(null);
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("annotationName", exception.Message);
        }

        [Fact]
        public void IncludeAllMatchesShouldThrowOnEmptyAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("*").Matches("");
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("annotationName", exception.Message);
        }

        [Fact]
        public void MatchesShouldThrowOnNullAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("ns.name").Matches(null);
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("annotationName", exception.Message);
        }

        [Fact]
        public void MatchesShouldThrowOnEmptyAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("ns.name").Matches("");
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("annotationName", exception.Message);
        }

        [Fact]
        public void IncludeExactMatchPatternShouldWork()
        {
            var filter = AnnotationFilter.Create("ns.name");
            Assert.False(filter.Matches("any.any"));
            Assert.False(filter.Matches("ns.name2"));
            Assert.True(filter.Matches("ns.name"));
        }

        [Fact]
        public void ExcludeExactMatchPatternShouldWork()
        {
            var filter = AnnotationFilter.Create("*,-ns.name");
            Assert.True(filter.Matches("any.any"));
            Assert.True(filter.Matches("ns.name2"));
            Assert.False(filter.Matches("ns.name"));
        }

        [Fact]
        public void IncludeStartsWithPatternShouldWork()
        {
            var filter = AnnotationFilter.Create("ns.*,NS.SubNS.*");
            Assert.True(filter.Matches("ns.any"));
            Assert.True(filter.Matches("ns.ns2.ns3.any"));
            Assert.False(filter.Matches("NS.any"));
            Assert.True(filter.Matches("NS.SubNS.any"));
            Assert.False(filter.Matches("any.any"));
        }

        [Fact]
        public void ExcludeStartsWithPatternShouldWork()
        {
            var filter = AnnotationFilter.Create("*,-ns.*,-NS.SubNS.*");
            Assert.False(filter.Matches("ns.any"));
            Assert.False(filter.Matches("ns.ns2.ns3.any"));
            Assert.True(filter.Matches("NS.any"));
            Assert.False(filter.Matches("NS.SubNS.any"));
            Assert.True(filter.Matches("any.any"));
        }

        [Fact]
        public void MultipleIncludesShouldWork()
        {
            var filter = AnnotationFilter.Create("ns.name, ns2.*");
            Assert.False(filter.Matches("any.any"));
            Assert.True(filter.Matches("ns.name"));
            Assert.True(filter.Matches("ns2.any"));
        }

        [Fact]
        public void MultipleExcludesShouldWork()
        {
            var filter = AnnotationFilter.Create("*,-ns.name, -ns2.*");
            Assert.True(filter.Matches("any.any"));
            Assert.False(filter.Matches("ns.name"));
            Assert.False(filter.Matches("ns2.any"));
        }

        [Fact]
        public void ExcludesShouldWinOverIncludes()
        {
            var filter = AnnotationFilter.Create("*,ns.name, -ns.name,ns2.*, -ns2.*");
            Assert.True(filter.Matches("any.any"));
            Assert.False(filter.Matches("ns.name"));
            Assert.False(filter.Matches("ns2.any"));
        }

        [Fact]
        public void TheExcludePatternShouldWinWhenBothTheIncludeAndExcludePatternsMatch()
        {
            var filter = AnnotationFilter.Create("ns.name,-ns.name");
            Assert.False(filter.Matches("ns.name"));
        }

        [Fact]
        public void TheMoreSpecificPatternInTheFilterShouldWinWhenMultiplePatternsMatch()
        {
            var filter = AnnotationFilter.Create("-*,ns.*,-ns.sub1.*,ns.sub1.sub2.*,-ns.sub1.sub2.sub3.*,ns.sub1.sub2.sub3.match");
            Assert.True(filter.Matches("ns.sub1.sub2.sub3.match"));
            Assert.False(filter.Matches("ns.sub1.sub2.sub3.notmatch"));
            Assert.True(filter.Matches("ns.sub1.sub2.match"));
            Assert.False(filter.Matches("ns.sub1.notmatch"));
            Assert.True(filter.Matches("ns.match"));
            Assert.False(filter.Matches("notmatch.any"));
        }
    }
}
