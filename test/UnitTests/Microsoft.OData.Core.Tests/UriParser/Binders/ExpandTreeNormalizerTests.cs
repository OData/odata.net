//---------------------------------------------------------------------
// <copyright file="ExpandTreeNormalizerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class ExpandTreeNormalizerTests
    {
        [Fact]
        public void NormalizeAnExpandOptionSyntaxTreeResultsInUnchangedOutput()
        {
            // Arrange: $expand=1($expand=2;)
            ExpandToken innerExpand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("2", null, null)) });
            ExpandToken expand = new ExpandToken(new ExpandTermToken[]{new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                       null /*selectOption*/,
                                                                       innerExpand)});

            // Act
            ExpandToken normalizedExpand = ExpandTreeNormalizer.NormalizeExpandTree(expand);

            // Assert
            Assert.NotNull(normalizedExpand);
            ExpandTermToken term = Assert.Single(normalizedExpand.ExpandTerms);
            term.ShouldBeExpandTermToken("1", true);

            Assert.NotNull(term.ExpandOption);
            ExpandTermToken subTerm = Assert.Single(term.ExpandOption.ExpandTerms);
            subTerm.ShouldBeExpandTermToken("2", true);
            Assert.Null(subTerm.ExpandOption);
        }

        [Fact]
        public void NormalizeTreeWorksWhenPathsHaveArguments()
        {
            // Arrange: $expand=1(name=value)
            ExpandToken expand = new ExpandToken(new ExpandTermToken[]
            {
                new ExpandTermToken(new NonSystemToken("1", new NamedValue[] { new NamedValue("name", new LiteralToken("value")) }, null))
            });

            // Act
            ExpandToken normalizedExpand = ExpandTreeNormalizer.NormalizeExpandTree(expand);

            // Assert
            Assert.NotNull(normalizedExpand);
            ExpandTermToken term = Assert.Single(normalizedExpand.ExpandTerms).ShouldBeExpandTermToken("1", true);
            NonSystemToken token = Assert.IsType<NonSystemToken>(term.PathToNavigationProp);
            Assert.Single(token.NamedValues).ShouldBeNamedValue("name", "value");

            Assert.Null(term.ExpandOption);
        }

        [Fact]
        public void CombineTermsWorksOnASingleTerm()
        {
            // Arrange: $expand=stuff
            ExpandToken expand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("stuff", null, null)) });

            // Act
            ExpandToken combinedExpand = ExpandTreeNormalizer.CombineTerms(expand);

            // Assert
            Assert.NotNull(combinedExpand);
            Assert.Single(combinedExpand.ExpandTerms).ShouldBeExpandTermToken("stuff", false);
        }

        [Fact]
        public void CombineTermsWorksForMultipleTerms()
        {
            // Arrange: $expand=1($expand=2), 1($expand=3)
            List<ExpandTermToken> expandTerms = new List<ExpandTermToken>();
            var token2 = new NonSystemToken("2", null, null);
            var token3 = new NonSystemToken("3", null, null);
            expandTerms.Add(new ExpandTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null), /*SelectToken*/null, new ExpandToken(new List<ExpandTermToken>() { new ExpandTermToken(token2) })));
            expandTerms.Add(new ExpandTermToken(new NonSystemToken("1", /*namedValues*/null, /*nextToken*/null), /*SelectToken*/null, new ExpandToken(new List<ExpandTermToken>() { new ExpandTermToken(token3) })));
            ExpandToken expand = new ExpandToken(expandTerms);

            // Act
            ExpandToken combinedExpand = ExpandTreeNormalizer.CombineTerms(expand);

            // Assert
            Assert.NotNull(combinedExpand);
            ExpandTermToken term = Assert.Single(combinedExpand.ExpandTerms).ShouldBeExpandTermToken("1", true);
            Assert.Contains(term.ExpandOption.ExpandTerms, t => t.PathToNavigationProp == token2);
            Assert.Contains(term.ExpandOption.ExpandTerms, t => t.PathToNavigationProp == token3);
        }

        [Fact]
        public void InvertPathsActuallyInvertsPaths()
        {
            // Arrange: $expand=1/2
            ExpandToken expand = new ExpandToken(new ExpandTermToken[]
            {
                new ExpandTermToken(new NonSystemToken("2", null, new NonSystemToken("1", null, null)))
            });

            // Act
            ExpandToken invertedPaths = ExpandTreeNormalizer.NormalizePaths(expand);

            // Assert
            Assert.NotNull(invertedPaths);
            ExpandTermToken term = Assert.Single(invertedPaths.ExpandTerms).ShouldBeExpandTermToken("1", false);
            Assert.NotNull(term.PathToNavigationProp.NextToken);
            term.PathToNavigationProp.NextToken.ShouldBeNonSystemToken("2");
        }

        [Fact]
        public void InvertPathsKeepsExpandOptionsInvariant()
        {
            // Arrange: $expand=1($filter=filter, $orderby=orderby, $top=top, $skip=skip;)
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
                        new SelectToken(selectTerms: null),
                        new ExpandToken(null))
                }
            );

            // Act
            ExpandToken invertedPaths = ExpandTreeNormalizer.NormalizePaths(expand);

            // Assert
            Assert.NotNull(invertedPaths);
            ExpandTermToken invertedToken = Assert.Single(invertedPaths.ExpandTerms);
            invertedToken.ShouldBeExpandTermToken("1", true);
            invertedToken.FilterOption.ShouldBeLiteralQueryToken("filter");
            Assert.Single(invertedToken.OrderByOptions).Expression.ShouldBeLiteralQueryToken("orderby");
            Assert.Equal(OrderByDirection.Descending, Assert.Single(invertedToken.OrderByOptions).Direction);
            Assert.Equal(1, invertedToken.TopOption);
            Assert.Equal(2, invertedToken.SkipOption);
            Assert.False(invertedToken.CountQueryOption);
            Assert.Equal(3, invertedToken.LevelsOption);
            invertedToken.SearchOption.ShouldBeStringLiteralToken("searchme");
            Assert.Empty(invertedToken.SelectOption.SelectTerms);
            Assert.Empty(invertedToken.ExpandOption.ExpandTerms);
        }

        [Fact]
        public void AddTermsDoesNothingForIdenticalTrees()
        {
            // Arrange: $expand=1($expand=2;)
            ExpandToken innerExpand = new ExpandToken(new ExpandTermToken[]
            {
                new ExpandTermToken(new NonSystemToken("2", null, null))
            });
            ExpandTermToken outerExpandToken = new ExpandTermToken(new NonSystemToken("1", null, null),
                null /*selectOption*/,
                innerExpand);

            // Act
            ExpandTermToken addedToken = ExpandTreeNormalizer.CombineTerms(outerExpandToken, outerExpandToken);

            // Assert
            Assert.NotNull(addedToken);
            addedToken.ShouldBeExpandTermToken("1", true);
            Assert.Single(addedToken.ExpandOption.ExpandTerms).ShouldBeExpandTermToken("2", true);
        }

        [Fact]
        public void AddTermsWorksForOneLevelBelow()
        {
            // Arrange: $expand=1($expand=2;), 1($expand=3;)
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

            // Act
            ExpandTermToken addedToken = ExpandTreeNormalizer.CombineTerms(outerToken1, outerToken2);

            // Assert
            addedToken.ShouldBeExpandTermToken("1", true);
            Assert.Equal(2, addedToken.ExpandOption.ExpandTerms.Count());
            Assert.Contains(innerExpandTerm2, addedToken.ExpandOption.ExpandTerms);
            Assert.Contains(innerExpandTerm1, addedToken.ExpandOption.ExpandTerms);
        }

        [Fact]
        public void AddTermsWorksAtDeepLevel()
        {
            // Arrange: $expand=1($expand=2($expand=3;);), 1($expand=2($expand=0;);)
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

            // Act
            ExpandTermToken addedToken = ExpandTreeNormalizer.CombineTerms(outerExpandTerm1, outerExpandTerm2);

            // Assert
            addedToken.ShouldBeExpandTermToken("1", true);
            ExpandTermToken expandTerms = Assert.Single(addedToken.ExpandOption.ExpandTerms).ShouldBeExpandTermToken("2", true);
            Assert.Contains(innerInnerExpandTerm1, expandTerms.ExpandOption.ExpandTerms);
            Assert.Contains(innerInnerExpandTerm2, expandTerms.ExpandOption.ExpandTerms);
        }

        [Fact]
        public void AddTermsWorksForNestedMultipleTerms()
        {
            // Arrange: $expand=1($expand=2($expand=5;),4;), 1($expand=2($expand=0;);)
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

            // Act
            ExpandTermToken addedToken = ExpandTreeNormalizer.CombineTerms(outerExpandTerm1, outerExpandTerm2);

            // Assert
            addedToken.ShouldBeExpandTermToken("1", true);
            Assert.Contains(innerExpandTerm2, addedToken.ExpandOption.ExpandTerms);

            ExpandTermToken twoToken = addedToken.ExpandOption.ExpandTerms.FirstOrDefault(x => x.PathToNavigationProp.Identifier == "2");
            twoToken.ShouldBeExpandTermToken("2", true);

            ExpandTermToken fiveToken = twoToken.ExpandOption.ExpandTerms.FirstOrDefault(x => x.PathToNavigationProp.Identifier == "5");
            fiveToken.ShouldBeExpandTermToken("5", true);

            ExpandTermToken zeroToken = twoToken.ExpandOption.ExpandTerms.FirstOrDefault(x => x.PathToNavigationProp.Identifier == "0");
            zeroToken.ShouldBeExpandTermToken("0", true);
        }

        [Fact]
        public void CombineChildNodesWorksForTwoEmptyNodes()
        {
            // Arrange: $expand=1
            ExpandTermToken expandTerm = new ExpandTermToken(new NonSystemToken("1", null, null));

            // Act
            IEnumerable<SelectExpandTermToken> combinedChildren = ExpandTreeNormalizer.CombineChildNodes(expandTerm, expandTerm);

            // Assert
            Assert.Empty(combinedChildren);
        }

        [Fact]
        public void CombineChildNodesWorksForSingleEmptyNode()
        {
            // Arrange: $expand=1($expand=2;), 1
            ExpandToken innerExpand = new ExpandToken(new ExpandTermToken[] { new ExpandTermToken(new NonSystemToken("2", null, null)) });
            ExpandTermToken outerExpandTerm1 = new ExpandTermToken(new NonSystemToken("1", null, null),
                                                                   null /*selectOption*/,
                                                                   innerExpand);
            ExpandTermToken outerExpandTerm2 = new ExpandTermToken(new NonSystemToken("1", null, null));

            // Act
            IEnumerable<SelectExpandTermToken> combinedChildren = ExpandTreeNormalizer.CombineChildNodes(outerExpandTerm1, outerExpandTerm2);

            // Assert
            Assert.Single(combinedChildren).ShouldBeExpandTermToken("2", false);
        }

        [Fact]
        public void CombineChildNodesWorksForTwoPopulatedNodes()
        {
            // Arrange: $expand=1($expand=2), 1($expand=3)
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

            // Act
            IEnumerable<SelectExpandTermToken> combinedChildren = ExpandTreeNormalizer.CombineChildNodes(outerExpandTerm1, outerExpandTerm2);

            // Assert
            Assert.Contains(innerExpandTerm1, combinedChildren);
            Assert.Contains(innerExpandTerm2, combinedChildren);
        }
    }
}
