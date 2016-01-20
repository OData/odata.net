//---------------------------------------------------------------------
// <copyright file="ExpandOptionParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Parser that knows how to parse expand options that could come after the path part of an expand term.
    /// Delegates to other parsing code as needed. I.E., when a nested $filter comes along, this will
    /// fire up the filter parsing code to figure that out. That code won't even know that it came from a nested location.
    /// </summary>
    internal sealed class ExpandOptionParser
    {
        /// <summary>
        /// The URI resolver which will resolve different kinds of Uri parsing context
        /// </summary>
        private readonly ODataUriResolver resolver;

        /// <summary>
        /// The parent entity type for expand option in case expand option is star, get all parent navigation properties
        /// </summary>
        private readonly IEdmStructuredType parentEntityType;
        
        /// <summary>
        /// Max recursion depth. As we recurse, each new instance of this class will have this lowered by 1.
        /// </summary>
        private readonly int maxRecursionDepth;

        /// <summary>
        /// Lexer to parse over the optionsText for a single $expand term. This is NOT the same lexer used by <see cref="SelectExpandParser"/>
        /// to parse over the entirety of $select or $expand. 
        /// </summary>
        private ExpressionLexer lexer;

        /// <summary>
        /// Whether to allow case insensitive for builtin identifier.
        /// </summary>
        private bool enableCaseInsensitiveBuiltinIdentifier;

        /// <summary>
        /// Creates an instance of this class to parse options.
        /// </summary>
        /// <param name="maxRecursionDepth">Max recursion depth left.</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        internal ExpandOptionParser(int maxRecursionDepth, bool enableCaseInsensitiveBuiltinIdentifier = false)
        {
            this.maxRecursionDepth = maxRecursionDepth;
            this.enableCaseInsensitiveBuiltinIdentifier = enableCaseInsensitiveBuiltinIdentifier;
        }

        /// <summary>
        /// Creates an instance of this class to parse options.
        /// </summary>
        /// <param name="resolver">The URI resolver which will resolve different kinds of Uri parsing context</param>
        /// <param name="parentEntityType">The parent entity type for expand option</param>
        /// <param name="maxRecursionDepth">Max recursion depth left.</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        internal ExpandOptionParser(ODataUriResolver resolver, IEdmStructuredType parentEntityType, int maxRecursionDepth, bool enableCaseInsensitiveBuiltinIdentifier = false)
            : this(maxRecursionDepth, enableCaseInsensitiveBuiltinIdentifier)
        {
            this.resolver = resolver;
            this.parentEntityType = parentEntityType;
        }

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
        /// Building off of a PathSegmentToken, continue parsing any expand options (nested $filter, $expand, etc)
        /// to build up an ExpandTermToken which fully represents the tree that makes up this expand term.
        /// </summary>
        /// <param name="pathToken">The PathSegmentToken representing the parsed expand path whose options we are now parsing.</param>
        /// <param name="optionsText">A string of the text between the parenthesis after an expand option.</param>
        /// <returns>The list of expand term tokens based on the path token, and all available expand options.</returns>
        internal List<ExpandTermToken> BuildExpandTermToken(PathSegmentToken pathToken, string optionsText)
        {
            // Setup a new lexer for parsing the optionsText
            this.lexer = new ExpressionLexer(optionsText ?? "", true /*moveToFirstToken*/, true /*useSemicolonDelimiter*/);

            // $expand option with star only support $ref option, $expand option property could be "*" or "*/$ref", special logic will be adopted.
            if (pathToken.Identifier == UriQueryConstants.Star || (pathToken.Identifier == UriQueryConstants.RefSegment && pathToken.NextToken.Identifier == UriQueryConstants.Star))
            {
                return BuildStarExpandTermToken(pathToken);
            }

            QueryToken filterOption = null;
            IEnumerable<OrderByToken> orderByOptions = null;
            long? topOption = null;
            long? skipOption = null;
            bool? countOption = null;
            long? levelsOption = null;
            QueryToken searchOption = null;
            SelectToken selectOption = null;
            ExpandToken expandOption = null;

            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            {
                // advance past the '('
                this.lexer.NextToken();

                // Check for (), which is not allowed.
                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_MissingExpandOption(pathToken.Identifier));
                }

                // Look for all the supported query options
                while (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
                {
                    string text = this.enableCaseInsensitiveBuiltinIdentifier
                        ? this.lexer.CurrentToken.Text.ToLowerInvariant()
                        : this.lexer.CurrentToken.Text;
                    switch (text)
                    {
                        case ExpressionConstants.QueryOptionFilter:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string filterText = this.ReadQueryOption();

                                UriQueryExpressionParser filterParser = new UriQueryExpressionParser(this.MaxFilterDepth, enableCaseInsensitiveBuiltinIdentifier);
                                filterOption = filterParser.ParseFilter(filterText);
                                break;
                            }

                        case ExpressionConstants.QueryOptionOrderby:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string orderByText = this.ReadQueryOption();

                                UriQueryExpressionParser orderbyParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
                                orderByOptions = orderbyParser.ParseOrderBy(orderByText);
                                break;
                            }

                        case ExpressionConstants.QueryOptionTop:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string topText = this.ReadQueryOption();

                                // TryParse requires a non-nullable non-negative long.
                                long top;
                                if (!long.TryParse(topText, out top) || top < 0)
                                {
                                    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidTopOption(topText));
                                }

                                topOption = top;
                                break;
                            }

                        case ExpressionConstants.QueryOptionSkip:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string skipText = this.ReadQueryOption();

                                // TryParse requires a non-nullable non-negative long.
                                long skip;
                                if (!long.TryParse(skipText, out skip) || skip < 0)
                                {
                                    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidSkipOption(skipText));
                                }

                                skipOption = skip;
                                break;
                            }

                        case ExpressionConstants.QueryOptionCount:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string countText = this.ReadQueryOption();
                                switch (countText)
                                {
                                    case ExpressionConstants.KeywordTrue:
                                        {
                                            countOption = true;
                                            break;
                                        }

                                    case ExpressionConstants.KeywordFalse:
                                        {
                                            countOption = false;
                                            break;
                                        }

                                    default:
                                        {
                                            throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidCountOption(countText));
                                        }
                                }

                                break;
                            }

                        case ExpressionConstants.QueryOptionLevels:
                            {
                                levelsOption = ResolveLevelOption();
                                break;
                            }

                        case ExpressionConstants.QueryOptionSearch:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string searchText = this.ReadQueryOption();

                                SearchParser searchParser = new SearchParser(this.MaxSearchDepth);
                                searchOption = searchParser.ParseSearch(searchText);

                                break;
                            }

                        case ExpressionConstants.QueryOptionSelect:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string selectText = this.ReadQueryOption();

                                SelectExpandParser innerSelectParser = new SelectExpandParser(selectText, this.maxRecursionDepth, enableCaseInsensitiveBuiltinIdentifier);
                                selectOption = innerSelectParser.ParseSelect();
                                break;
                            }

                        case ExpressionConstants.QueryOptionExpand:
                            {
                                // advance to the equal sign
                                this.lexer.NextToken();
                                string expandText = this.ReadQueryOption();

                                // As 2016/1/8, the navigation property is only supported in entity type, and will support in ComplexType in future. 
                                IEdmStructuredType targetEntityType = null;
                                if (this.resolver != null && this.parentEntityType != null)
                                {
                                    var parentProperty = this.resolver.ResolveProperty(parentEntityType, pathToken.Identifier) as IEdmNavigationProperty;

                                    // it is a navigation property, need to find the type. Like $expand=Friends($expand=Trips($expand=*)), when expandText becomes "Trips($expand=*)", find navigation property Trips of Friends, then get Entity type of Trips.
                                    if (parentProperty != null)
                                    { 
                                        targetEntityType = parentProperty.ToEntityType();
                                    }
                                }

                                SelectExpandParser innerExpandParser = new SelectExpandParser(resolver, expandText, targetEntityType, this.maxRecursionDepth - 1, enableCaseInsensitiveBuiltinIdentifier);
                                expandOption = innerExpandParser.ParseExpand();
                                break;
                            }

                        default:
                            {
                                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
                            }
                    }
                }

                // Move past the ')'
                this.lexer.NextToken();
            }

            // Either there was no '(' at all or we just read past the ')' so we should be at the end
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            // TODO, there should be some check here in case pathToken identifier is $ref, select, expand and levels options are not allowed.
            List<ExpandTermToken> expandTermTokenList = new List<ExpandTermToken>();
            ExpandTermToken currentToken = new ExpandTermToken(pathToken, filterOption, orderByOptions, topOption,
                skipOption, countOption, levelsOption, searchOption, selectOption, expandOption);
            expandTermTokenList.Add(currentToken);

            return expandTermTokenList;
        }


        /// <summary>
        /// Building off of a PathSegmentToken whose value is star, only nested level options is allowed.
        /// </summary>
        /// <param name="pathToken">The PathSegmentToken representing the parsed expand path whose options we are now parsing.</param>
        /// <returns>An expand term token based on the path token, and all available expand options.</returns>
        private List<ExpandTermToken> BuildStarExpandTermToken(PathSegmentToken pathToken)
        {
            List<ExpandTermToken> expandTermTokenList = new List<ExpandTermToken>();
            long? levelsOption = null;
            bool isRefExpand = (pathToken.Identifier == UriQueryConstants.RefSegment);

            // Based on the specification, 
            //   For star in expand, this will be supported,
            //   $expand=*
            //   $expand=EntitySet($expand=* )
            //   $expand=*/$ref 
            //   $expand=*,EntitySet
            //   $expand=EntitySet, *
            //   $expand=*/$ref,EntitySet
            //   Parenthesized set of expand options for star expand option supported are $level per specification.
            //   And this will throw exception,
            //   $expand= * /$count
            //   Parenthesized set of expand options for star expand option which will also cause exception are $filter, $select, $orderby, $skip, $top, $count, $search, and $expand per specification. 
            // And level is not supported with "*/$ref".

            // As 2016/1/8, the navigation property is only supported in entity type, and will support in ComplexType in future.
            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            {
                // advance past the '('
                this.lexer.NextToken();

                // Check for (), which is not allowed.
                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_MissingExpandOption(pathToken.Identifier));
                }

                // Only level option is supported by expand.
                while (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
                {
                    string text = this.enableCaseInsensitiveBuiltinIdentifier
                        ? this.lexer.CurrentToken.Text.ToLowerInvariant()
                        : this.lexer.CurrentToken.Text;
                    switch (text)
                    {
                        case ExpressionConstants.QueryOptionLevels:
                            {
                                if (!isRefExpand) 
                                { 
                                    levelsOption = ResolveLevelOption();
                                }
                                else
                                {
                                    // no option is allowed when expand with star per specification
                                    throw new ODataException(ODataErrorStrings.UriExpandParser_TermIsNotValidForStarRef(this.lexer.ExpressionText));
                                }

                                break;
                            }

                        default:
                            {
                                throw new ODataException(ODataErrorStrings.UriExpandParser_TermIsNotValidForStar(this.lexer.ExpressionText));
                            }
                    }
                }

                // Move past the ')'
                this.lexer.NextToken();
            }

            // Either there was no '(' at all or we just read past the ')' so we should be at the end
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.End)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            // As 2016/1/8, the navigation property is only supported in entity type, and will support in ComplexType in future.
            var entityType = parentEntityType as IEdmEntityType;
            if (entityType == null)
            {
                throw new ODataException(ODataErrorStrings.UriExpandParser_ParentEntityIsNull(this.lexer.ExpressionText));
            }

            foreach (var navigationProperty in entityType.NavigationProperties())
            {
                var tmpPathToken = default(PathSegmentToken);

                // create path token for each navigation properties.
                if (pathToken.Identifier.Equals(UriQueryConstants.RefSegment))
                {
                    tmpPathToken = new NonSystemToken(navigationProperty.Name, null, pathToken.NextToken.NextToken);
                    tmpPathToken = new NonSystemToken(UriQueryConstants.RefSegment, null, tmpPathToken);
                }
                else
                {
                    tmpPathToken = new NonSystemToken(navigationProperty.Name, null, pathToken.NextToken);
                }

                ExpandTermToken currentToken = new ExpandTermToken(tmpPathToken, null, null,
                    null, null, null, levelsOption, null, null, null);
                expandTermTokenList.Add(currentToken);
            }

            return expandTermTokenList;
        }

        /// <summary>
        /// Parse the level option in the expand option text.
        /// </summary>
        /// <returns>The level option for expand in long type</returns>
        private long? ResolveLevelOption()
        {
            long? levelsOption = null;

            // advance to the equal sign
            this.lexer.NextToken();
            string levelsText = this.ReadQueryOption();
            long level;

            if (string.Equals(
                ExpressionConstants.KeywordMax,
                levelsText,
                this.enableCaseInsensitiveBuiltinIdentifier ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                levelsOption = long.MinValue;
            }
            else if (!long.TryParse(levelsText, NumberStyles.None, CultureInfo.InvariantCulture, out level) || level < 0)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidLevelsOption(levelsText));
            }
            else
            {
                levelsOption = level;
            }

            return levelsOption;
        }

        /// <summary>
        /// Read a query option from the lexer.
        /// </summary>
        /// <returns>The query option as a string.</returns>
        private string ReadQueryOption()
        {
            if (this.lexer.CurrentToken.Kind != ExpressionTokenKind.Equal)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
            }

            // get the full text from the current location onward
            // there could be literals like 'A string literal; tricky!' in there, so we need to be careful.
            // Also there could be more nested (...) expressions that we ignore until we recurse enough times to get there.
            string expressionText = this.lexer.AdvanceThroughExpandOption();

            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.SemiColon)
            {
                // Move over the ';' seperator
                this.lexer.NextToken();
                return expressionText;
            }

            // If there wasn't a semicolon, it MUST be the last option. We must be at ')' in this case
            this.lexer.ValidateToken(ExpressionTokenKind.CloseParen);
            return expressionText;
        }
    }
}