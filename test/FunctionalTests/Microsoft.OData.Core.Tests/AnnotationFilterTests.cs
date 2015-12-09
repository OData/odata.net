//---------------------------------------------------------------------
// <copyright file="AnnotationFilterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class AnnotationFilterTests
    {
        [Fact]
        public void CreateWithNullOrEmptyShouldCreateExcludeAllFilter()
        {
            foreach (string filterString in new string[] { null, "" })
            {
                var filter = AnnotationFilter.Create(filterString);
                filter.Matches("any.any").Should().BeFalse();
            }
        }

        [Fact]
        public void CreateWithExcludeAllPatternShouldCreateExcludeAllFilter()
        {
            foreach (string filterString in new string[] { "*,-*", "-ns.name,-*,*" })
            {
                var filter = AnnotationFilter.Create(filterString);
                filter.Matches("any.any").Should().BeFalse();
                filter.Matches("ns.name").Should().BeFalse();
            }
        }

        [Fact]
        public void CreateWithNoIncludePatternShouldCreateExcludeAllFilter()
        {
            var filter = AnnotationFilter.Create("-ns1.name,-ns2.*");
            filter.Matches("any.any").Should().BeFalse();
            filter.Matches("ns1.name").Should().BeFalse();
            filter.Matches("ns2.any").Should().BeFalse();
        }

        [Fact]
        public void CreateWithWildCardShouldCreateIncludeAllFilter()
        {
            var filter = AnnotationFilter.Create("*");
            filter.Matches("any.any").Should().BeTrue();
        }

        [Fact]
        public void TestCreateInclueAllFilter()
        {
            var filter = AnnotationFilter.CreateInclueAllFilter();
            filter.Matches("any.any").Should().BeTrue();

            filter.GetType().Should().Equals(AnnotationFilter.Create("*").GetType());
        }

        [Fact]
        public void CreateWithWildCardAndNoExcludePatternShouldCreateIncludeAllFilter()
        {
            var filter = AnnotationFilter.Create("ns.name,*");
            filter.Matches("any.any").Should().BeTrue();
            filter.Matches("ns.name").Should().BeTrue();
        }

        [Fact]
        public void ExcludeAllMatchesShouldThrowOnNullAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("-*").Matches(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("annotationName", ComparisonMode.Substring);
        }

        [Fact]
        public void ExcludeAllMatchesShouldThrowOnEmptyAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("-*").Matches("");
            action.ShouldThrow<ArgumentNullException>().WithMessage("annotationName", ComparisonMode.Substring);
        }

        [Fact]
        public void IncludeAllMatchesShouldThrowOnNullAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("*").Matches(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("annotationName", ComparisonMode.Substring);
        }

        [Fact]
        public void IncludeAllMatchesShouldThrowOnEmptyAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("*").Matches("");
            action.ShouldThrow<ArgumentNullException>().WithMessage("annotationName", ComparisonMode.Substring);
        }

        [Fact]
        public void MatchesShouldThrowOnNullAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("ns.name").Matches(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("annotationName", ComparisonMode.Substring);
        }

        [Fact]
        public void MatchesShouldThrowOnEmptyAnnotationName()
        {
            Action action = () => AnnotationFilter.Create("ns.name").Matches("");
            action.ShouldThrow<ArgumentNullException>().WithMessage("annotationName", ComparisonMode.Substring);
        }

        [Fact]
        public void IncludeExactMatchPatternShouldWork()
        {
            var filter = AnnotationFilter.Create("ns.name");
            filter.Matches("any.any").Should().BeFalse();
            filter.Matches("ns.name2").Should().BeFalse();
            filter.Matches("ns.name").Should().BeTrue();
        }

        [Fact]
        public void ExcludeExactMatchPatternShouldWork()
        {
            var filter = AnnotationFilter.Create("*,-ns.name");
            filter.Matches("any.any").Should().BeTrue();
            filter.Matches("ns.name2").Should().BeTrue();
            filter.Matches("ns.name").Should().BeFalse();
        }

        [Fact]
        public void IncludeStartsWithPatternShouldWork()
        {
            var filter = AnnotationFilter.Create("ns.*,NS.SubNS.*");
            filter.Matches("ns.any").Should().BeTrue();
            filter.Matches("ns.ns2.ns3.any").Should().BeTrue();
            filter.Matches("NS.any").Should().BeFalse();
            filter.Matches("NS.SubNS.any").Should().BeTrue();
            filter.Matches("any.any").Should().BeFalse();
        }

        [Fact]
        public void ExcludeStartsWithPatternShouldWork()
        {
            var filter = AnnotationFilter.Create("*,-ns.*,-NS.SubNS.*");
            filter.Matches("ns.any").Should().BeFalse();
            filter.Matches("ns.ns2.ns3.any").Should().BeFalse();
            filter.Matches("NS.any").Should().BeTrue();
            filter.Matches("NS.SubNS.any").Should().BeFalse();
            filter.Matches("any.any").Should().BeTrue();
        }

        [Fact]
        public void MultipleIncludesShouldWork()
        {
            var filter = AnnotationFilter.Create("ns.name, ns2.*");
            filter.Matches("any.any").Should().BeFalse();
            filter.Matches("ns.name").Should().BeTrue();
            filter.Matches("ns2.any").Should().BeTrue();
        }

        [Fact]
        public void MultipleExcludesShouldWork()
        {
            var filter = AnnotationFilter.Create("*,-ns.name, -ns2.*");
            filter.Matches("any.any").Should().BeTrue();
            filter.Matches("ns.name").Should().BeFalse();
            filter.Matches("ns2.any").Should().BeFalse();
        }

        [Fact]
        public void ExcludesShouldWinOverIncludes()
        {
            var filter = AnnotationFilter.Create("*,ns.name, -ns.name,ns2.*, -ns2.*");
            filter.Matches("any.any").Should().BeTrue();
            filter.Matches("ns.name").Should().BeFalse();
            filter.Matches("ns2.any").Should().BeFalse();
        }

        [Fact]
        public void TheExcludePatternShouldWinWhenBothTheIncludeAndExcludePatternsMatch()
        {
            var filter = AnnotationFilter.Create("ns.name,-ns.name");
            filter.Matches("ns.name").Should().BeFalse();
        }

        [Fact]
        public void TheMoreSpecificPatternInTheFilterShouldWinWhenMultiplePatternsMatch()
        {
            var filter = AnnotationFilter.Create("-*,ns.*,-ns.sub1.*,ns.sub1.sub2.*,-ns.sub1.sub2.sub3.*,ns.sub1.sub2.sub3.match");
            filter.Matches("ns.sub1.sub2.sub3.match").Should().BeTrue();
            filter.Matches("ns.sub1.sub2.sub3.notmatch").Should().BeFalse();
            filter.Matches("ns.sub1.sub2.match").Should().BeTrue();
            filter.Matches("ns.sub1.notmatch").Should().BeFalse();
            filter.Matches("ns.match").Should().BeTrue();
            filter.Matches("notmatch.any").Should().BeFalse();
        }
    }
}
