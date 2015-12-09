//---------------------------------------------------------------------
// <copyright file="ExpandTreeNormalizerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    public class ExpandTreeNormalizerTests
    {
        [Fact]
        public void NormalizeAnExpandOptionSyntaxTreeResultsInUnchangedOutput()
        {
            // $expand=1($expand=2;)
            ExpandToken innerExpand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("2", null, null)) });
            ExpandToken expand = new ExpandToken(new ExpandTermToken[]{new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                       null /*selectOption*/,
                                                                       innerExpand)});
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            ExpandToken normalizedExpand = expandTreeNormalizer.NormalizeExpandTree(expand);
            normalizedExpand.ExpandTerms.Single().ShouldBeExpandTermToken("1", true)
                .And.ExpandOption.ExpandTerms.Single().ShouldBeExpandTermToken("2", true)
                .And.ExpandOption.Should().BeNull();
        }

        [Fact]
        public void NormalizeTreeWorksWhenPathsHaveArguments()
        {
            // $expand=1(name=value)/2
            ExpandToken expand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("1", new NamedValue[] { new NamedValue("name", new LiteralToken("value")) }, null)) });
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            ExpandToken normalizedExpand = expandTreeNormalizer.NormalizeExpandTree(expand);
            normalizedExpand.ExpandTerms.Single().ShouldBeExpandTermToken("1", true)
                .And.PathToNavProp.As<NonSystemToken>().NamedValues.Single().ShouldBeNamedValue("name", "value");
            normalizedExpand.ExpandTerms.Single().ExpandOption.Should().BeNull();
        }

        [Fact]
        public void CombineTermsWorksOnASingleTerm()
        {
            // $expand=stuff
            ExpandToken expand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("stuff", null, null)) });
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            ExpandToken combinedExpand = expandTreeNormalizer.CombineTerms(expand);
            combinedExpand.ExpandTerms.Single().ShouldBeExpandTermToken("stuff", false);
        }

        [Fact]
        public void CombineTermsWorksForMultipleTerms()
        {
            // $expand=1($expand=2), 1($expand=3)
            List<ExpandTermToken> expandTerms = new List<ExpandTermToken>();
            var token2 = new NonSystemToken("2", null, null);
            var token3 = new NonSystemToken("3", null, null);
            expandTerms.Add(new ExpandTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null), /*SelectToken*/null, new ExpandToken(new List<ExpandTermToken>() { new ExpandTermToken(token2) })));
            expandTerms.Add(new ExpandTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null), /*SelectToken*/null, new ExpandToken(new List<ExpandTermToken>() { new ExpandTermToken(token3) })));
            ExpandToken expand = new ExpandToken(expandTerms);
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            ExpandToken combinedExpand = expandTreeNormalizer.CombineTerms(expand);
            combinedExpand.ExpandTerms.Single().ShouldBeExpandTermToken("1", true);
            combinedExpand.ExpandTerms.ElementAt(0).ExpandOption.ExpandTerms.Should().Contain(t => t.PathToNavProp == token2);
            combinedExpand.ExpandTerms.ElementAt(0).ExpandOption.ExpandTerms.Should().Contain(t => t.PathToNavProp == token3);
        }

        [Fact]
        public void InvertPathsActuallyInvertsPaths()
        {
            // $expand=1/2
            ExpandToken expand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("2", null, new NonSystemToken("1", null, null))) });
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            ExpandToken invertedPaths = expandTreeNormalizer.NormalizePaths(expand);
            invertedPaths.ExpandTerms.Single().ShouldBeExpandTermToken("1", false)
                .And.PathToNavProp.NextToken.ShouldBeNonSystemToken("2");
        }

        [Fact]
        public void InvertPathsKeepsExpandOptionsInvariant()
        {
            //$expand=1($filter=filter, $orderby=orderby, $top=top, $skip=skip;)
            ExpandToken expand = new ExpandToken(
                                    new ExpandTermToken[] { 
                                        new ExpandTermToken(
                                            new NonSystemToken("1", null, null), 
                                            new LiteralToken("filter"), 
                                            new OrderByToken []{ new OrderByToken(new LiteralToken("orderby"), OrderByDirection.Descending)}, 
                                            1, 
                                            2, 
                                            false,
                                            3,
                                            new StringLiteralToken("searchme"), 
                                            new SelectToken(null), 
                                            new ExpandToken(null))
                                    }
                                );
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            ExpandToken invertedPaths = expandTreeNormalizer.NormalizePaths(expand);
            var invertedToken = invertedPaths.ExpandTerms.Single();
            invertedToken.ShouldBeExpandTermToken("1", true);
            invertedToken.FilterOption.ShouldBeLiteralQueryToken("filter");
            invertedToken.OrderByOptions.Single().Expression.ShouldBeLiteralQueryToken("orderby");
            invertedToken.OrderByOptions.Single().Direction.Should().Be(OrderByDirection.Descending);
            invertedToken.TopOption.Should().Be(1);
            invertedToken.SkipOption.Should().Be(2);
            invertedToken.CountQueryOption.Should().BeFalse();
            invertedToken.LevelsOption.Should().Be(3);
            invertedToken.SearchOption.ShouldBeStringLiteralToken("searchme");
            invertedToken.SelectOption.Properties.Should().BeEmpty();
            invertedToken.ExpandOption.ExpandTerms.Should().BeEmpty();
        }

        [Fact]
        public void AddTermsDoesNothingForIdenticalTrees()
        {
            // $expand=1($expand=2;)
            ExpandToken innerExpand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("2", null, null)) });
            ExpandTermToken outerExpandToken = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                   innerExpand);
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            var addedToken = expandTreeNormalizer.CombineTerms(outerExpandToken, outerExpandToken);
            addedToken.ShouldBeExpandTermToken("1", true).And.ExpandOption.ExpandTerms.Single().ShouldBeExpandTermToken("2", true);
        }

        [Fact]
        public void AddTermsWorksForOneLevelBelow()
        {
            // $expand=1($expand=2;), 1($expand=3;)
            ExpandTermToken innerExpandTerm1 = new ExpandTermToken(new NonSystemToken("2", null, null));
            ExpandToken innerExpand1 = new ExpandToken(new ExpandTermToken[] { innerExpandTerm1 });
            ExpandTermToken outerToken1 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                              null /*selectOption*/,
                                                              innerExpand1);
            ExpandTermToken innerExpandTerm2 = new ExpandTermToken(new NonSystemToken("3", null, null));
            ExpandToken innerExpand2 = new ExpandToken(new ExpandTermToken[] { innerExpandTerm2 });
            ExpandTermToken outerToken2 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                              null /*selectOption*/,
                                                              innerExpand2);
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            var addedToken = expandTreeNormalizer.CombineTerms(outerToken1, outerToken2);
            addedToken.ShouldBeExpandTermToken("1", true).And.ExpandOption.ExpandTerms.Should().Contain(innerExpandTerm2).And.Contain(innerExpandTerm1);
        }

        [Fact]
        public void AddTermsWorksAtDeepLevel()
        {
            // $expand=1($expand=2($expand=3;);), 1($expand=2($expand=0;);)

            ExpandTermToken innerInnerExpandTerm1 = new ExpandTermToken(new NonSystemToken("3", null, null));
            ExpandTermToken innerExpandTerm1 = new ExpandTermToken(new NonSystemToken("2", null, null),
                                                                   null /*selectOption*/,
                                                                   new ExpandToken(new ExpandTermToken[] { innerInnerExpandTerm1 }));
            ExpandTermToken outerExpandTerm1 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                   new ExpandToken(new ExpandTermToken[] { innerExpandTerm1 }));

            ExpandTermToken innerInnerExpandTerm2 = new ExpandTermToken(new NonSystemToken("0", null, null));
            ExpandTermToken innerExpandTerm2 = new ExpandTermToken(new NonSystemToken("2", null, null),
                                                                   null /*selectOption*/,
                                                                   new ExpandToken(new ExpandTermToken[] { innerInnerExpandTerm2 }));
            ExpandTermToken outerExpandTerm2 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                    new ExpandToken(new ExpandTermToken[] { innerExpandTerm2 }));
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            var addedToken = expandTreeNormalizer.CombineTerms(outerExpandTerm1, outerExpandTerm2);
            addedToken.ShouldBeExpandTermToken("1", true)
                .And.ExpandOption.ExpandTerms.Single().ShouldBeExpandTermToken("2", true)
                .And.ExpandOption.ExpandTerms.Should().Contain(innerInnerExpandTerm1)
                .And.Contain(innerInnerExpandTerm2);
        }

        [Fact]
        public void AddTermsWorksForNestedMultipleTerms()
        {
            // $expand=1($expand=2($expand=5;),4;), 1($expand=2($expand=0;);)
            ExpandTermToken innerExpandTerm1 = new ExpandTermToken(new NonSystemToken("2", null, null),
                                                       null /*selectOption*/,
                                                       new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("5", null, null)) }));
            ExpandTermToken innerExpandTerm2 = new ExpandTermToken(new NonSystemToken("4", null, null));
            ExpandTermToken outerExpandTerm1 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                   new ExpandToken(new ExpandTermToken[] { innerExpandTerm1, innerExpandTerm2 }));

            ExpandTermToken innerExpandTerm3 = new ExpandTermToken(new NonSystemToken("2", null, null),
                                                                   null /*selectOption*/,
                                                                   new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("0", null, null)) }));
            ExpandTermToken outerExpandTerm2 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                   new ExpandToken(new ExpandTermToken[] { innerExpandTerm3 }));
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            var addedToken = expandTreeNormalizer.CombineTerms(outerExpandTerm1, outerExpandTerm2);
            addedToken.ShouldBeExpandTermToken("1", true).And.ExpandOption.ExpandTerms.Should().Contain(innerExpandTerm2);
            ExpandTermToken twoToken = addedToken.ExpandOption.ExpandTerms.FirstOrDefault(x => x.PathToNavProp.Identifier == "2");
            twoToken.ShouldBeExpandTermToken("2", true);
            ExpandTermToken fiveToken = twoToken.ExpandOption.ExpandTerms.FirstOrDefault(x => x.PathToNavProp.Identifier == "5");
            ExpandTermToken zeroToken = twoToken.ExpandOption.ExpandTerms.FirstOrDefault(x => x.PathToNavProp.Identifier == "0");
            fiveToken.ShouldBeExpandTermToken("5", true);
            zeroToken.ShouldBeExpandTermToken("0", true);
        }

        [Fact]
        public void CombineChildNodesWorksForTwoEmptyNodes()
        {
            // $expand=1
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("1", null, null));
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            IEnumerable<ExpandTermToken> combinedChildren = expandTreeNormalizer.CombineChildNodes(expandTerm, expandTerm);
            combinedChildren.Count().Should().Be(0);
        }

        [Fact]
        public void CombineChildNodesWorksForSingleEmptyNode()
        {
            // $expand=1($expand=2;), 1
            ExpandToken innerExpand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("2", null, null)) });
            ExpandTermToken outerExpandTerm1 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                   innerExpand);
            ExpandTermToken outerExpandTerm2 = new ExpandTermToken(new NonSystemToken("1", null, null));
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            IEnumerable<ExpandTermToken> combinedChildren = expandTreeNormalizer.CombineChildNodes(outerExpandTerm1, outerExpandTerm2);
            combinedChildren.Single().ShouldBeExpandTermToken("2", false);
        }

        [Fact]
        public void CombineChildNodesWorksForTwoPopulatedNodes()
        {
            // $expand=1($expand=2), 1($expand=3)
            ExpandTermToken innerExpandTerm1 = new ExpandTermToken(new NonSystemToken("2", null, null));
            ExpandToken innerExpand1 = new ExpandToken(new ExpandTermToken[] { innerExpandTerm1 });
            ExpandTermToken outerExpandTerm1 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                   innerExpand1);
            ExpandTermToken innerExpandTerm2 = new ExpandTermToken(new NonSystemToken("3", null, null));
            ExpandToken innerExpand2 = new ExpandToken(new ExpandTermToken[] { innerExpandTerm2 });
            ExpandTermToken outerExpandTerm2 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                   innerExpand2);
            ExpandTreeNormalizer expandTreeNormalizer = new ExpandTreeNormalizer();
            IEnumerable<ExpandTermToken> combinedChildren = expandTreeNormalizer.CombineChildNodes(outerExpandTerm1, outerExpandTerm2);
            combinedChildren.Should().Contain(innerExpandTerm1).And.Contain(innerExpandTerm2);
        }
    }
}
