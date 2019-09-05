//---------------------------------------------------------------------
// <copyright file="AnnotationFilterPatternTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class AnnotationFilterPatternTests
    {
        [Fact]
        public void CreatePatternShouldThrowOnNullPattern()
        {
            Action test = () => AnnotationFilterPattern.Create(null);
            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void CreatePatternShouldThrowOnEmptyPattern()
        {
            Action test = () => AnnotationFilterPattern.Create("");
            Assert.Throws<ArgumentNullException>(test);
        }

        [Fact]
        public void CreateWithStarShouldReturnIncludeAllPattern()
        {
            Assert.Same(AnnotationFilterPattern.IncludeAllPattern, AnnotationFilterPattern.Create("*"));
        }

        [Fact]
        public void IncludeAllPatternShouldNotBeExcludePattern()
        {
            Assert.False(AnnotationFilterPattern.IncludeAllPattern.IsExclude);
        }

        [Fact]
        public void IncludeAllPatternShouldMatchArbitraryInstanceAnnotation()
        {
            Assert.True(AnnotationFilterPattern.IncludeAllPattern.Matches("any.any"));
        }

        [Fact]
        public void CreateWithMinusStartShouldReturnExcludeAllPattern()
        {
            Assert.Same(AnnotationFilterPattern.ExcludeAllPattern, AnnotationFilterPattern.Create("-*"));
        }

        [Fact]
        public void ExcludeAllPatternShouldBeTrue()
        {
            Assert.True(AnnotationFilterPattern.ExcludeAllPattern.IsExclude);
        }

        [Fact]
        public void ExcludeAllPatternShouldMatchArbitraryInstanceAnnotation()
        {
            Assert.True(AnnotationFilterPattern.ExcludeAllPattern.Matches("any.any"));
        }

        [Theory]
        [InlineData("name")]
        [InlineData("-name")]
        public void InvalidPatternMissingDotShouldThrow(string pattern)
        {
            Action test = () => AnnotationFilterPattern.Create(pattern);
            test.Throws<ArgumentException>(Strings.AnnotationFilterPattern_InvalidPatternMissingDot(pattern));
        }

        [Theory]
        [InlineData("namespace.")]
        [InlineData(".name")]
        [InlineData("-namespace.")]
        [InlineData("-.name")]
        [InlineData("-.")]
        [InlineData("-.*")]
        public void InvalidPatternEmptySegmentShouldThrow(string pattern)
        {
            Action test = () => AnnotationFilterPattern.Create(pattern);
            test.Throws<ArgumentException>(Strings.AnnotationFilterPattern_InvalidPatternEmptySegment(pattern));
        }

        [Theory]
        [InlineData("namespace*.name")]
        [InlineData("namespace.*name")]
        [InlineData("namespce.foo*bar")]
        [InlineData("-namespace*.name")]
        [InlineData("-namespace.*name")]
        [InlineData("-namespce.foo*bar")]
        public void InvalidPatternWildCardInSegmentShouldThrow(string pattern)
        {
            Action test = () => AnnotationFilterPattern.Create(pattern);
            test.Throws<ArgumentException>(Strings.AnnotationFilterPattern_InvalidPatternWildCardInSegment(pattern));
        }

        [Theory]
        [InlineData("namespace.*.name")]
        [InlineData("*.name")]
        [InlineData("*.namespce.name")]
        [InlineData("-namespace.*.name")]
        [InlineData("-*.name")]
        [InlineData("-*.namespce.name")]
        public void InvalidPatternWildCardNotInLastSegmentShouldThrow(string pattern)
        {
            Action test = () => AnnotationFilterPattern.Create(pattern);
            test.Throws<ArgumentException>(Strings.AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment(pattern));
        }

        [Fact]
        public void CreateIncludeStartsWithFilterShouldPass()
        {
            AnnotationFilterPattern startsWithPattern = AnnotationFilterPattern.Create("namespace.*");
            Assert.False(startsWithPattern.IsExclude);
            Assert.False(startsWithPattern.Matches("any.any"));
            Assert.True(startsWithPattern.Matches("namespace.any"));
        }

        [Fact]
        public void CreateExcludeStartsWithFilterShouldPass()
        {
            AnnotationFilterPattern startsWithPattern = AnnotationFilterPattern.Create("-namespace.*");
            Assert.True(startsWithPattern.IsExclude);
            Assert.False(startsWithPattern.Matches("any.any"));
            Assert.True(startsWithPattern.Matches("namespace.any"));
        }

        [Fact]
        public void CreateIncludeExactMatchFilterShouldPass()
        {
            AnnotationFilterPattern exactMatchPattern = AnnotationFilterPattern.Create("namespace.name");
            Assert.False(exactMatchPattern.IsExclude);
            Assert.False(exactMatchPattern.Matches("any.any"));
            Assert.True(exactMatchPattern.Matches("namespace.name"));
        }

        [Fact]
        public void CreateExcludeExactMatchFilterShouldPass()
        {
            AnnotationFilterPattern exactMatchPattern = AnnotationFilterPattern.Create("-namespace.name");
            Assert.True(exactMatchPattern.IsExclude);
            Assert.False(exactMatchPattern.Matches("any.any"));
            Assert.True(exactMatchPattern.Matches("namespace.name"));
        }

        [Fact]
        public void ExcludeShouldHaveHigherPriorityThanIncludeIfThePatternsHaveTheSamePriority()
        {
            Assert.Equal(-1, AnnotationFilterPattern.Create("-*").CompareTo(AnnotationFilterPattern.Create("*")));
            Assert.Equal(-1, AnnotationFilterPattern.Create("-ns.*").CompareTo(AnnotationFilterPattern.Create("ns.*")));
            Assert.Equal(-1, AnnotationFilterPattern.Create("-ns.name").CompareTo(AnnotationFilterPattern.Create("ns.name")));
            Assert.Equal(-1, AnnotationFilterPattern.Create("-ns.*").CompareTo(AnnotationFilterPattern.Create("ns1.*")));
            Assert.Equal(-1, AnnotationFilterPattern.Create("-ns.sub1.*").CompareTo(AnnotationFilterPattern.Create("ns.sub2.*")));
        }

        [Fact]
        public void IdenticalPatternsShouldBeOfSamePriority()
        {
            Assert.Equal(0, AnnotationFilterPattern.Create("*").CompareTo(AnnotationFilterPattern.Create("*")));
            Assert.Equal(0, AnnotationFilterPattern.Create("ns.*").CompareTo(AnnotationFilterPattern.Create("ns.*")));
            Assert.Equal(0, AnnotationFilterPattern.Create("ns.name").CompareTo(AnnotationFilterPattern.Create("ns.name")));
            Assert.Equal(0, AnnotationFilterPattern.Create("-*").CompareTo(AnnotationFilterPattern.Create("-*")));
            Assert.Equal(0, AnnotationFilterPattern.Create("-ns.*").CompareTo(AnnotationFilterPattern.Create("-ns.*")));
            Assert.Equal(0, AnnotationFilterPattern.Create("-ns.name").CompareTo(AnnotationFilterPattern.Create("-ns.name")));
        }

        [Fact]
        public void WildCardShouldHaveLowerPriorityThanNoneWildCard()
        {
            Assert.Equal(1, AnnotationFilterPattern.Create("*").CompareTo(AnnotationFilterPattern.Create("foo.*")));
            Assert.Equal(-1, AnnotationFilterPattern.Create("-ns.*").CompareTo(AnnotationFilterPattern.Create("*")));
        }

        [Fact]
        public void IfPattern1StartsWithPattern2Pattern1ShouldBeGivenHigherPriorityThanPattern2()
        {
            Assert.Equal(-1, AnnotationFilterPattern.Create("ns.name").CompareTo(AnnotationFilterPattern.Create("ns.*")));
            Assert.Equal(-1, AnnotationFilterPattern.Create("ns.sub1.*").CompareTo(AnnotationFilterPattern.Create("ns.sub")));
        }

        [Fact]
        public void IfPattern2StartsWithPattern1Pattern2ShouldBeGivenHigherPriorityThanPattern1()
        {
            Assert.Equal(1, AnnotationFilterPattern.Create("ns.*").CompareTo(AnnotationFilterPattern.Create("ns.name")));
            Assert.Equal(1, AnnotationFilterPattern.Create("ns.sub").CompareTo(AnnotationFilterPattern.Create("ns.sub1.*")));
        }

        [Fact]
        public void PatternsUnderDifferentNamespacesShouldBeOfSamePriority()
        {
            Assert.Equal(0, AnnotationFilterPattern.Create("ns.*").CompareTo(AnnotationFilterPattern.Create("ns1.*")));
            Assert.Equal(0, AnnotationFilterPattern.Create("ns.sub1.name").CompareTo(AnnotationFilterPattern.Create("ns.sub2.name")));
            Assert.Equal(0, AnnotationFilterPattern.Create("ns.sub1.*").CompareTo(AnnotationFilterPattern.Create("ns.sub12.*")));
            Assert.Equal(0, AnnotationFilterPattern.Create("ns1.name").CompareTo(AnnotationFilterPattern.Create("ns2.name")));
        }

        [Fact]
        public void SortShouldOrderPatternsFromHigherPriorityToLowerPriority()
        {
            AnnotationFilterPattern pattern1 = AnnotationFilterPattern.Create("*");
            AnnotationFilterPattern pattern2 = AnnotationFilterPattern.Create("-*");
            AnnotationFilterPattern pattern3 = AnnotationFilterPattern.Create("ns.*");
            AnnotationFilterPattern pattern4 = AnnotationFilterPattern.Create("ns.name");
            AnnotationFilterPattern pattern5 = AnnotationFilterPattern.Create("-ns.name");
            AnnotationFilterPattern[] patternsToSort = new[] {pattern1, pattern2, pattern3, pattern4, pattern5};
            AnnotationFilterPattern.Sort(patternsToSort);
            Assert.Equal(pattern5, patternsToSort[0]);
            Assert.Equal(pattern4, patternsToSort[1]);
            Assert.Equal(pattern3, patternsToSort[2]);
            Assert.Equal(pattern2, patternsToSort[3]);
            Assert.Equal(pattern1, patternsToSort[4]);
        }
    }
}
