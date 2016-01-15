//---------------------------------------------------------------------
// <copyright file="SelectExpandParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Object that knows how to parse a select or expand expression. That is, a path to a property, 
    /// a wildcard, operation name, etc, including nested expand options.
    /// </summary>
    internal sealed class SelectExpandParser
    {
        /// <summary>
        /// The URI resolver which will resolve different kinds of Uri parsing context
        /// </summary>
        private readonly ODataUriResolver resolver;

        /// <summary>
        /// The parent entity type for expand option in case expand option is star, get all parent entity navigation properties, it is an IEdmStructuredType.
        /// </summary>
        private readonly IEdmStructuredType parentEntityType;

        /// <summary>
        /// The maximum allowable recursive depth.
        /// </summary>
        private readonly int maxRecursiveDepth;

        /// <summary>
        /// Object to handle the parsing of any nested expand options that we discover.
        /// </summary>
        private ExpandOptionParser expandOptionParser;

        /// <summary>
        /// Lexer used to parse an the $select or $expand string.
        /// </summary>
        private ExpressionLexer lexer;

        /// <summary>
        /// True if we are we parsing $select.
        /// </summary>
        private bool isSelect;

        /// <summary>
        /// Whether to allow case insensitive for builtin identifier.
        /// </summary>
        private bool enableCaseInsensitiveBuiltinIdentifier;

        /// <summary>
        /// Build the SelectOption strategy.
        /// TODO: Really should not take the clauseToParse here. Instead it should be provided with a call to ParseSelect() or ParseExpand().
        /// </summary>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <param name="maxRecursiveDepth">max recursive depth</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        public SelectExpandParser(string clauseToParse, int maxRecursiveDepth, bool enableCaseInsensitiveBuiltinIdentifier = false)
        {
            this.maxRecursiveDepth = maxRecursiveDepth;

            // Set max recursive depth for path, $filter, $orderby and $search to maxRecursiveDepth in case they were not be be specified.
            this.MaxPathDepth = maxRecursiveDepth;
            this.MaxFilterDepth = maxRecursiveDepth;
            this.MaxOrderByDepth = maxRecursiveDepth;
            this.MaxSearchDepth = maxRecursiveDepth;

            // Sets up our lexer. We don't turn useSemicolonDelimiter on since the parsing code for expand options, 
            // which is the only thing that needs it, is in a different class that uses it's own lexer.
            this.lexer = clauseToParse != null ? new ExpressionLexer(clauseToParse, false /*moveToFirstToken*/, false /*useSemicolonDelimiter*/) : null;

            this.enableCaseInsensitiveBuiltinIdentifier = enableCaseInsensitiveBuiltinIdentifier;
        }

        /// <summary>
        /// Build the ExpandOption strategy (SelectOption build does not need resolover and parentEntityType now).
        /// </summary>
        /// <param name="resolver">the URI resolver which will resolve different kinds of Uri parsing context</param>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <param name="parentEntityType">the parent entity type for expand option</param>
        /// <param name="maxRecursiveDepth">max recursive depth</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        public SelectExpandParser(ODataUriResolver resolver, string clauseToParse, IEdmStructuredType parentEntityType, int maxRecursiveDepth, bool enableCaseInsensitiveBuiltinIdentifier = false)
            : this(clauseToParse, maxRecursiveDepth, enableCaseInsensitiveBuiltinIdentifier)
        {
            this.resolver = resolver;
            this.parentEntityType = parentEntityType;
        }
        
        /// <summary>
        /// The maximum depth for path nested in $expand.
        /// </summary>
        internal int MaxPathDepth { get; set; }

        /// <summary>
        /// The maximum depth for $filter nested in $expand.
        /// </summary>
        internal int MaxFilterDepth { get; set; }

        /// <summary>
        /// The maximum depth for $orderby nested in $expand.
        /// </summary>
        internal int MaxOrderByDepth { get; set; }

        /// <summary>
        /// The maximum depth for $search nested in $expand.
        /// </summary>
        internal int MaxSearchDepth { get; set; }

        /// <summary>
        /// Parses a full $select expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public SelectToken ParseSelect()
        {
            this.isSelect = true;
            return this.ParseCommaSeperatedSelectList(termTokens => new SelectToken(termTokens), this.ParseSingleSelectTerm);
        }

        /// <summary>
        /// Parses a full $expand expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public ExpandToken ParseExpand()
        {
            this.isSelect = false;
            return this.ParseCommaSeperatedExpandList(termTokens => new ExpandToken(termTokens), this.ParseSingleExpandTerm);
        }

        /// <summary>
        /// Parses a single term in a comma separated list of things to select.
        /// </summary>
        /// <returns>A token representing thing to select.</returns>
        private PathSegmentToken ParseSingleSelectTerm()
        {
            this.isSelect = true;

            var termParser = new SelectExpandTermParser(this.lexer, this.MaxPathDepth, this.isSelect);
            return termParser.ParseTerm();
        }

        /// <summary>
        /// Parses a single term in a comma separated list of things to expand.
        /// </summary>
        /// <returns>A token list representing thing to expand, the expand option star will have more than one items in the list.</returns>
        private List<ExpandTermToken> ParseSingleExpandTerm()
        {
            this.isSelect = false;

            var termParser = new SelectExpandTermParser(this.lexer, this.MaxPathDepth, this.isSelect);
            PathSegmentToken pathToken = termParser.ParseTerm(allowRef: true);

            string optionsText = null;
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            {
                optionsText = this.lexer.AdvanceThroughBalancedParentheticalExpression();

                // Move lexer to what is after the parenthesis expression. Now CurrentToken will be the next thing.
                this.lexer.NextToken();
            }

            if (expandOptionParser == null)
            {
                expandOptionParser = new ExpandOptionParser(this.resolver, this.parentEntityType, this.maxRecursiveDepth, enableCaseInsensitiveBuiltinIdentifier)
                {
                    MaxFilterDepth = MaxFilterDepth,
                    MaxOrderByDepth = MaxOrderByDepth,
                    MaxSearchDepth = MaxSearchDepth
                };
            }

            return this.expandOptionParser.BuildExpandTermToken(pathToken, optionsText);
        }

        /// <summary>
        /// Parsed a comma separated list of $expand terms.
        /// </summary>
        /// <param name="ctor">A method to construct the final token from the term tokens.</param>
        /// <param name="termParsingFunc">A method to parse each individual term.</param>
        /// <returns>A token representing the entire $expand clause syntactically.</returns>
        private ExpandToken ParseCommaSeperatedExpandList(Func<IEnumerable<ExpandTermToken>, ExpandToken> ctor, Func<List<ExpandTermToken>> termParsingFunc)
        {
            List<ExpandTermToken> termTokens = new List<ExpandTermToken>();

            List<ExpandTermToken> starTermTokens = new List<ExpandTermToken>();

            // TODO Per specification, "A navigation property MUST NOT appear in more than one expandItem.", so should be some check to make sure explicitly specified property does not show multiple times.

            // This happens if we were passed a null string
            if (this.lexer == null)
            {
                return ctor(termTokens);
            }

            // Move to the first token
            this.lexer.NextToken();

            // This happens if it was just whitespace. e.g. fake.svc/Customers?$expand=     &$filter=IsCool&$orderby=ID
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.End)
            {
                return ctor(termTokens);
            }

            // Process first term
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
            {
                starTermTokens = termParsingFunc();
            }
            else
            {
                termTokens.AddRange(termParsingFunc());
            }

            // If it was a list of terms, then commas will be separating them
            while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
            {
                // Move over the ',' to the next term
                this.lexer.NextToken();
                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End && this.lexer.CurrentToken.Kind != ExpressionTokenKind.Star)
                {
                    termTokens.AddRange(termParsingFunc());
                }
                else if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
                {
                    // Multiple stars is not allowed here.
                    if (starTermTokens.Count > 0)
                    {
                        throw new ODataException(ODataErrorStrings.UriExpandParser_TermWithMultipleStarNotAllowed(this.lexer.ExpressionText));
                    }

                    starTermTokens = termParsingFunc();
                }
                else
                {
                    break;
                }
            }

            // If there is * with other property, per specification, other propreties will take precedence over the star operator
            if (starTermTokens.Count > 0)
            {
                List<string> explicitedTokens = new List<string>();
                foreach (var tmpTokens in termTokens)
                {
                    var pathToNav = tmpTokens.PathToNavProp;
                    if (pathToNav.Identifier != UriQueryConstants.RefSegment)
                    {
                        explicitedTokens.Add(pathToNav.Identifier);
                    }
                    else
                    {
                        explicitedTokens.Add(pathToNav.NextToken.Identifier);
                    }
                }

                // Add navigation path if it is not in list yet
                foreach (var tmpTokens in starTermTokens)
                {
                    var pathToNav = tmpTokens.PathToNavProp;
                    if (pathToNav.Identifier != UriQueryConstants.RefSegment && !explicitedTokens.Contains(pathToNav.Identifier))
                    {
                        termTokens.Add(tmpTokens);
                    }
                    else if (pathToNav.Identifier == UriQueryConstants.RefSegment && !explicitedTokens.Contains(pathToNav.NextToken.Identifier))
                    {
                        termTokens.Add(tmpTokens);
                    }
                }
            }

            // If there isn't a comma, then we must be done. Otherwise there is a syntax error
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            return ctor(termTokens);
        }

        /// <summary>
        /// Parsed a comma separated list of $select terms.
        /// </summary>
        /// <param name="ctor">A method to construct the final token from the term tokens.</param>
        /// <param name="termParsingFunc">A method to parse each individual term.</param>
        /// <returns>A token representing the entire $select clause syntactically.</returns>
        private SelectToken ParseCommaSeperatedSelectList(Func<IEnumerable<PathSegmentToken>, SelectToken> ctor, Func<PathSegmentToken> termParsingFunc)
        {
            List<PathSegmentToken> termTokens = new List<PathSegmentToken>();

            // This happens if we were passed a null string
            if (this.lexer == null)
            {
                return ctor(termTokens);
            }

            // Move to the first token
            this.lexer.NextToken();

            // This happens if it was just whitespace. e.g. fake.svc/Customers?$expand=     &$filter=IsCool&$orderby=ID
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.End)
            {
                return ctor(termTokens);
            }

            // Process first term
            termTokens.Add(termParsingFunc());

            // If it was a list of terms, then commas will be separating them
            while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Comma)
            {
                // Move over the ',' to the next term
                this.lexer.NextToken();
                if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
                {
                    termTokens.Add(termParsingFunc());
                }
                else
                {
                    break;
                }
            }

            // If there isn't a comma, then we must be done. Otherwise there is a syntax error
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            return ctor(termTokens);
        }
    }
}