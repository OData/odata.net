//---------------------------------------------------------------------
// <copyright file="AnnotationFilterPatternTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class AnnotationFilterPatternTests
    {
        [Fact]
        public void CreatePatternShouldThrowOnNullPattern()
        {
            Action test = () => AnnotationFilterPattern.Create(null);
            test.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreatePatternShouldThrowOnEmptyPattern()
        {
            Action test = () => AnnotationFilterPattern.Create("");
            test.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithStarShouldReturnIncludeAllPattern()
        {
            AnnotationFilterPattern.Create("*").As<object>().Should().BeSameAs(AnnotationFilterPattern.IncludeAllPattern);            
        }

        [Fact]
        public void IncludeAllPatternShouldNotBeExcludePattern()
        {
            AnnotationFilterPattern.IncludeAllPattern.IsExclude.Should().BeFalse();
        }

        [Fact]
        public void IncludeAllPatternShouldMatchArbitraryInstanceAnnotation()
        {
            AnnotationFilterPattern.IncludeAllPattern.Matches("any.any").Should().BeTrue();
        }

        [Fact]
        public void CreateWithMinusStartShouldReturnExcludeAllPattern()
        {
            AnnotationFilterPattern.Create("-*").As<object>().Should().BeSameAs(AnnotationFilterPattern.ExcludeAllPattern);
        }

        [Fact]
        public void ExcludeAllPatternShouldBeTrue()
        {
            AnnotationFilterPattern.ExcludeAllPattern.IsExclude.Should().BeTrue();
        }

        [Fact]
        public void ExcludeAllPatternShouldMatchArbitraryInstanceAnnotation()
        {
            AnnotationFilterPattern.ExcludeAllPattern.Matches("any.any").Should().BeTrue();
        }

        [Fact]
        public void InvalidPatternMissingDotShouldThrow()
        {
            foreach (string pattern in new[] { "name", "-name" })
            {
                Action test = () => AnnotationFilterPattern.Create(pattern);
                test.ShouldThrow<ArgumentException>().WithMessage(Strings.AnnotationFilterPattern_InvalidPatternMissingDot(pattern));
            }
        }

        [Fact]
        public void InvalidPatternEmptySegmentShouldThrow()
        {
            foreach (string pattern in new[] { "namespace.", ".name", ".", "-namespace.", "-.name", "-.", "-.*" })
            {
                Action test = () => AnnotationFilterPattern.Create(pattern);
                test.ShouldThrow<ArgumentException>().WithMessage(Strings.AnnotationFilterPattern_InvalidPatternEmptySegment(pattern));
            }
        }

        [Fact]
        public void InvalidPatternWildCardInSegmentShouldThrow()
        {
            foreach (string pattern in new[] { "namespace*.name", "namespace.*name", "namespce.foo*bar", "-namespace*.name", "-namespace.*name", "-namespce.foo*bar" })
            {
                Action test = () => AnnotationFilterPattern.Create(pattern);
                test.ShouldThrow<ArgumentException>().WithMessage(Strings.AnnotationFilterPattern_InvalidPatternWildCardInSegment(pattern));
            }
        }

        [Fact]
        public void InvalidPatternWildCardNotInLastSegmentShouldThrow()
        {
            foreach (string pattern in new[] { "namespace.*.name", "*.name", "*.namespce.name", "-namespace.*.name", "-*.name", "-*.namespce.name" })
            {
                Action test = () => AnnotationFilterPattern.Create(pattern);
                test.ShouldThrow<ArgumentException>().WithMessage(Strings.AnnotationFilterPattern_InvalidPatternWildCardMustBeInLastSegment(pattern));
            }
        }

        [Fact]
        public void CreateIncludeStartsWithFilterShouldPass()
        {
            AnnotationFilterPattern startsWithPattern = AnnotationFilterPattern.Create("namespace.*");
            startsWithPattern.IsExclude.Should().BeFalse();
            startsWithPattern.Matches("any.any").Should().BeFalse();
            startsWithPattern.Matches("namespace.any").Should().BeTrue();
        }

        [Fact]
        public void CreateExcludeStartsWithFilterShouldPass()
        {
            AnnotationFilterPattern startsWithPattern = AnnotationFilterPattern.Create("-namespace.*");
            startsWithPattern.IsExclude.Should().BeTrue();
            startsWithPattern.Matches("any.any").Should().BeFalse();
            startsWithPattern.Matches("namespace.any").Should().BeTrue();
        }

        [Fact]
        public void CreateIncludeExactMatchFilterShouldPass()
        {
            AnnotationFilterPattern exactMatchPattern = AnnotationFilterPattern.Create("namespace.name");
            exactMatchPattern.IsExclude.Should().BeFalse();
            exactMatchPattern.Matches("any.any").Should().BeFalse();
            exactMatchPattern.Matches("namespace.name").Should().BeTrue();
        }

        [Fact]
        public void CreateExcludeExactMatchFilterShouldPass()
        {
            AnnotationFilterPattern exactMatchPattern = AnnotationFilterPattern.Create("-namespace.name");
            exactMatchPattern.IsExclude.Should().BeTrue();
            exactMatchPattern.Matches("any.any").Should().BeFalse();
            exactMatchPattern.Matches("namespace.name").Should().BeTrue();
        }

        [Fact]
        public void ExcludeShouldHaveHigherPriorityThanIncludeIfThePatternsHaveTheSamePriority()
        {
            AnnotationFilterPattern.Create("-*").CompareTo(AnnotationFilterPattern.Create("*")).Should().Be(-1);
            AnnotationFilterPattern.Create("-ns.*").CompareTo(AnnotationFilterPattern.Create("ns.*")).Should().Be(-1);
            AnnotationFilterPattern.Create("-ns.name").CompareTo(AnnotationFilterPattern.Create("ns.name")).Should().Be(-1);
            AnnotationFilterPattern.Create("-ns.*").CompareTo(AnnotationFilterPattern.Create("ns1.*")).Should().Be(-1);
            AnnotationFilterPattern.Create("-ns.sub1.*").CompareTo(AnnotationFilterPattern.Create("ns.sub2.*")).Should().Be(-1);
        }

        [Fact]
        public void IdenticalPatternsShouldBeOfSamePriority()
        {
            AnnotationFilterPattern.Create("*").CompareTo(AnnotationFilterPattern.Create("*")).Should().Be(0);
            AnnotationFilterPattern.Create("ns.*").CompareTo(AnnotationFilterPattern.Create("ns.*")).Should().Be(0);
            AnnotationFilterPattern.Create("ns.name").CompareTo(AnnotationFilterPattern.Create("ns.name")).Should().Be(0);
            AnnotationFilterPattern.Create("-*").CompareTo(AnnotationFilterPattern.Create("-*")).Should().Be(0);
            AnnotationFilterPattern.Create("-ns.*").CompareTo(AnnotationFilterPattern.Create("-ns.*")).Should().Be(0);
            AnnotationFilterPattern.Create("-ns.name").CompareTo(AnnotationFilterPattern.Create("-ns.name")).Should().Be(0);
        }

        [Fact]
        public void WildCardShouldHaveLowerPriorityThanNoneWildCard()
        {
            AnnotationFilterPattern.Create("*").CompareTo(AnnotationFilterPattern.Create("foo.*")).Should().Be(1);
            AnnotationFilterPattern.Create("-ns.*").CompareTo(AnnotationFilterPattern.Create("*")).Should().Be(-1);
        }

        [Fact]
        public void IfPattern1StartsWithPattern2Pattern1ShouldBeGivenHigherPriorityThanPattern2()
        {
            AnnotationFilterPattern.Create("ns.name").CompareTo(AnnotationFilterPattern.Create("ns.*")).Should().Be(-1);
            AnnotationFilterPattern.Create("ns.sub1.*").CompareTo(AnnotationFilterPattern.Create("ns.sub")).Should().Be(-1);
        }

        [Fact]
        public void IfPattern2StartsWithPattern1Pattern2ShouldBeGivenHigherPriorityThanPattern1()
        {
            AnnotationFilterPattern.Create("ns.*").CompareTo(AnnotationFilterPattern.Create("ns.name")).Should().Be(1);
            AnnotationFilterPattern.Create("ns.sub").CompareTo(AnnotationFilterPattern.Create("ns.sub1.*")).Should().Be(1);
        }

        [Fact]
        public void PatternsUnderDifferentNamespacesShouldBeOfSamePriority()
        {
            AnnotationFilterPattern.Create("ns.*").CompareTo(AnnotationFilterPattern.Create("ns1.*")).Should().Be(0);
            AnnotationFilterPattern.Create("ns.sub1.name").CompareTo(AnnotationFilterPattern.Create("ns.sub2.name")).Should().Be(0);
            AnnotationFilterPattern.Create("ns.sub1.*").CompareTo(AnnotationFilterPattern.Create("ns.sub12.*")).Should().Be(0);
            AnnotationFilterPattern.Create("ns1.name").CompareTo(AnnotationFilterPattern.Create("ns2.name")).Should().Be(0);
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
            patternsToSort[0].Should().Be(pattern5);
            patternsToSort[1].Should().Be(pattern4);
            patternsToSort[2].Should().Be(pattern3);
            patternsToSort[3].Should().Be(pattern2);
            patternsToSort[4].Should().Be(pattern1);
        }
    }
}
