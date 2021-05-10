//---------------------------------------------------------------------
// <copyright file="SelectExpandOptionParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Parser that knows how to parse expand options that could come after the path part of an expand term.
    /// Delegates to other parsing code as needed. I.E., when a nested $filter comes along, this will
    /// fire up the filter parsing code to figure that out. That code won't even know that it came from a nested location.
    /// </summary>
    internal sealed class SelectExpandOptionParser
    {
        /// <summary>
        /// The URI resolver which will resolve different kinds of Uri parsing context
        /// </summary>
        private readonly ODataUriResolver resolver;

        /// <summary>
        /// The parent structured type for select/expand option in case expand option is star, get all parent navigation properties
        /// </summary>
        private readonly IEdmStructuredType parentStructuredType;

        /// <summary>
        /// Max recursion depth. As we recurse, each new instance of this class will have this lowered by 1.
        /// </summary>
        private readonly int maxRecursionDepth;

        /// <summary>
        /// Whether to enable no dollar query options.
        /// </summary>
        private readonly bool enableNoDollarQueryOptions;

        /// <summary>
        /// Whether to allow case insensitive for builtin identifier.
        /// </summary>
        private readonly bool enableCaseInsensitiveBuiltinIdentifier;

        /// <summary>
        /// Lexer to parse over the optionsText for a single $expand term. This is NOT the same lexer used by <see cref="SelectExpandParser"/>
        /// to parse over the entirety of $select or $expand.
        /// </summary>
        private ExpressionLexer lexer;

        /// <summary>
        /// Creates an instance of this class to parse options.
        /// </summary>
        /// <param name="maxRecursionDepth">Max recursion depth left.</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        /// <param name="enableNoDollarQueryOptions">Whether to enable no dollar query options.</param>
        internal SelectExpandOptionParser(
            int maxRecursionDepth,
            bool enableCaseInsensitiveBuiltinIdentifier = false,
            bool enableNoDollarQueryOptions = false)
        {
            this.maxRecursionDepth = maxRecursionDepth;
            this.enableCaseInsensitiveBuiltinIdentifier = enableCaseInsensitiveBuiltinIdentifier;
            this.enableNoDollarQueryOptions = enableNoDollarQueryOptions;
        }

        /// <summary>
        /// Creates an instance of this class to parse options.
        /// </summary>
        /// <param name="resolver">The URI resolver which will resolve different kinds of Uri parsing context</param>
        /// <param name="parentStructuredType">The parent structured type for expand option</param>
        /// <param name="maxRecursionDepth">Max recursion depth left.</param>
        /// <param name="enableCaseInsensitiveBuiltinIdentifier">Whether to allow case insensitive for builtin identifier.</param>
        /// <param name="enableNoDollarQueryOptions">Whether to enable no dollar query options.</param>
        internal SelectExpandOptionParser(
            ODataUriResolver resolver,
            IEdmStructuredType parentStructuredType,
            int maxRecursionDepth,
            bool enableCaseInsensitiveBuiltinIdentifier = false,
            bool enableNoDollarQueryOptions = false)
            : this(maxRecursionDepth, enableCaseInsensitiveBuiltinIdentifier, enableNoDollarQueryOptions)
        {
            this.resolver = resolver;
            this.parentStructuredType = parentStructuredType;
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
        /// Building off a PathSegmentToken, continue parsing any select options (nested $filter, $expand, etc)
        /// to build up an SelectTermToken which fully represents the tree that makes up this select term.
        /// </summary>
        /// <param name="pathToken">The PathSegmentToken representing the parsed select path whose options we are now parsing.</param>
        /// <param name="optionsText">A string of the text between the parenthesis after a select option.</param>
        /// <returns>The select term token based on the path token, and all available select options.</returns>
        internal SelectTermToken BuildSelectTermToken(PathSegmentToken pathToken, string optionsText)
        {
            // Setup a new lexer for parsing the optionsText
            this.lexer = new ExpressionLexer(optionsText ?? "", true /*moveToFirstToken*/, true /*useSemicolonDelimiter*/);

            QueryToken filterOption = null;
            IEnumerable<OrderByToken> orderByOptions = null;
            long? topOption = null;
            long? skipOption = null;
            bool? countOption = null;
            QueryToken searchOption = null;
            SelectToken selectOption = null;
            ComputeToken computeOption = null;

            if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.OpenParen)
            {
                // advance past the '('
                this.lexer.NextToken();

                // Check for (), which is not allowed.
                if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.CloseParen)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_MissingSelectOption(pathToken.Identifier));
                }

                // Look for all the supported query options
                while (this.lexer.CurrentToken.Kind != ExpressionTokenKind.CloseParen)
                {
                    string text = this.enableCaseInsensitiveBuiltinIdentifier
                        ? this.lexer.CurrentToken.Text.ToLowerInvariant()
                        : this.lexer.CurrentToken.Text;

                    // Prepend '$' prefix if needed.
                    if (this.enableNoDollarQueryOptions && !text.StartsWith(UriQueryConstants.DollarSign, StringComparison.Ordinal))
                    {
                        text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", UriQueryConstants.DollarSign, text);
                    }

                    switch (text)
                    {
                        case ExpressionConstants.QueryOptionFilter: // inner $filter
                            filterOption = ParseInnerFilter();
                            break;

                        case ExpressionConstants.QueryOptionOrderby: // inner $orderby
                            orderByOptions = ParseInnerOrderBy();
                            break;

                        case ExpressionConstants.QueryOptionTop: // inner $top
                            topOption = ParseInnerTop();
                            break;

                        case ExpressionConstants.QueryOptionSkip: // innner $skip
                            skipOption = ParseInnerSkip();
                            break;

                        case ExpressionConstants.QueryOptionCount: // inner $count
                            countOption = ParseInnerCount();
                            break;

                        case ExpressionConstants.QueryOptionSearch: // inner $search
                            searchOption = ParseInnerSearch();
                            break;

                        case ExpressionConstants.QueryOptionSelect: // inner $select
                            selectOption = ParseInnerSelect(pathToken);
                            break;

                        case ExpressionConstants.QueryOptionCompute: // inner $compute
                            computeOption = ParseInnerCompute();
                            break;

                        default:
                            throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.lexer.ExpressionText));
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

            return new SelectTermToken(pathToken, filterOption, orderByOptions, topOption, skipOption, countOption, searchOption, selectOption, computeOption);
        }

        /// <summary>
        /// Building of a PathSegmentToken, continue parsing any expand options (nested $filter, $expand, etc)
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
            ComputeToken computeOption = null;
            IEnumerable<QueryToken> applyOptions = null;

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

                    // Prepend '$' prefix if needed.
                    if (this.enableNoDollarQueryOptions && !text.StartsWith(UriQueryConstants.DollarSign, StringComparison.Ordinal))
                    {
                        text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", UriQueryConstants.DollarSign, text);
                    }

                    switch (text)
                    {
                        case ExpressionConstants.QueryOptionFilter: // inner $filter
                            filterOption = ParseInnerFilter();
                            break;

                        case ExpressionConstants.QueryOptionOrderby: // inner $orderby
                            orderByOptions = ParseInnerOrderBy();
                            break;

                        case ExpressionConstants.QueryOptionTop: // inner $top
                            topOption = ParseInnerTop();
                            break;

                        case ExpressionConstants.QueryOptionSkip: // inner $skip
                            skipOption = ParseInnerSkip();
                            break;

                        case ExpressionConstants.QueryOptionCount: // inner $count
                            countOption = ParseInnerCount();
                            break;

                        case ExpressionConstants.QueryOptionSearch: // inner $search
                            searchOption = ParseInnerSearch();
                            break;

                        case ExpressionConstants.QueryOptionLevels: // inner $level
                            levelsOption = ParseInnerLevel();
                            break;

                        case ExpressionConstants.QueryOptionSelect: // inner $select
                            selectOption = ParseInnerSelect(pathToken);
                            break;

                        case ExpressionConstants.QueryOptionExpand: // inner $expand
                            expandOption = ParseInnerExpand(pathToken);
                            break;

                        case ExpressionConstants.QueryOptionCompute: // inner $compute
                            computeOption = ParseInnerCompute();
                            break;

                        case ExpressionConstants.QueryOptionApply: // inner $apply
                            applyOptions = ParseInnerApply();
                            break;

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
                skipOption, countOption, levelsOption, searchOption, selectOption, expandOption, computeOption, applyOptions);
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
            if (this.parentStructuredType == null)
            {
                throw new ODataException(ODataErrorStrings.UriExpandParser_ParentStructuredTypeIsNull(this.lexer.ExpressionText));
            }

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
                                    levelsOption = ParseInnerLevel();
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

            foreach (var navigationProperty in this.parentStructuredType.NavigationProperties())
            {
                var tmpPathToken = default(PathSegmentToken);

                // create path token for each navigation properties.
                if (pathToken.Identifier.Equals(UriQueryConstants.RefSegment, StringComparison.Ordinal))
                {
                    tmpPathToken = new NonSystemToken(navigationProperty.Name, null, pathToken.NextToken.NextToken);
                    tmpPathToken = new NonSystemToken(UriQueryConstants.RefSegment, null, tmpPathToken);
                }
                else
                {
                    tmpPathToken = new NonSystemToken(navigationProperty.Name, null, pathToken.NextToken);
                }

                ExpandTermToken currentToken = new ExpandTermToken(tmpPathToken, null, null,
                    null, null, null, levelsOption, null, null, null, null, null);
                expandTermTokenList.Add(currentToken);
            }

            return expandTermTokenList;
        }

        /// <summary>
        /// Parse the filter option in the select/expand option text.
        /// </summary>
        /// <returns>The filter option for select/expand</returns>
        private QueryToken ParseInnerFilter()
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string filterText = UriParserHelper.ReadQueryOption(this.lexer);

            UriQueryExpressionParser filterParser = new UriQueryExpressionParser(this.MaxFilterDepth, enableCaseInsensitiveBuiltinIdentifier);
            return filterParser.ParseFilter(filterText);
        }

        /// <summary>
        /// Parse the orderby option in the select/expand option text.
        /// </summary>
        /// <returns>The orderby option for select/expand</returns>
        private IEnumerable<OrderByToken> ParseInnerOrderBy()
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string orderByText = UriParserHelper.ReadQueryOption(this.lexer);

            UriQueryExpressionParser orderbyParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
            return orderbyParser.ParseOrderBy(orderByText);
        }

        /// <summary>
        /// Parse the top option in the select/expand option text.
        /// </summary>
        /// <returns>The top option for select/expand</returns>
        private long? ParseInnerTop()
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string topText = UriParserHelper.ReadQueryOption(this.lexer);

            // TryParse requires a non-nullable non-negative long.
            long top;
            if (!long.TryParse(topText, out top) || top < 0)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidTopOption(topText));
            }

            return top;
        }

        /// <summary>
        /// Parse the skip option in the select/expand option text.
        /// </summary>
        /// <returns>The skip option for select/expand</returns>
        private long? ParseInnerSkip()
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string skipText = UriParserHelper.ReadQueryOption(this.lexer);

            // TryParse requires a non-nullable non-negative long.
            long skip;
            if (!long.TryParse(skipText, out skip) || skip < 0)
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidSkipOption(skipText));
            }

            return skip;
        }

        /// <summary>
        /// Parse the count option in the select/expand option text.
        /// </summary>
        /// <returns>The count option for select/expand</returns>
        private bool? ParseInnerCount()
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string countText = UriParserHelper.ReadQueryOption(this.lexer);
            switch (countText)
            {
                case ExpressionConstants.KeywordTrue:
                    return true;

                case ExpressionConstants.KeywordFalse:
                    return false;

                default:
                    throw new ODataException(ODataErrorStrings.UriSelectParser_InvalidCountOption(countText));
            }
        }

        /// <summary>
        /// Parse the search option in the select/expand option text.
        /// </summary>
        /// <returns>The search option for select/expand</returns>
        private QueryToken ParseInnerSearch()
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string searchText = UriParserHelper.ReadQueryOption(this.lexer);

            SearchParser searchParser = new SearchParser(this.MaxSearchDepth);
            return searchParser.ParseSearch(searchText);
        }

        /// <summary>
        /// Parse the select option in the select/expand option text.
        /// </summary>
        /// <param name="pathToken">The path segment token</param>
        /// <returns>The select option for select/expand</returns>
        private SelectToken ParseInnerSelect(PathSegmentToken pathToken)
        {
            // advance to the equal sign
            this.lexer.NextToken();
            string selectText = UriParserHelper.ReadQueryOption(this.lexer);

            IEdmStructuredType targetStructuredType = null;
            if (this.resolver != null && this.parentStructuredType != null)
            {
                var parentProperty = this.resolver.ResolveProperty(parentStructuredType, pathToken.Identifier);

                // It is a property, need to find the type.
                // or for select query like: $select=Address($expand=City)
                if (parentProperty != null)
                {
                    targetStructuredType = parentProperty.Type.ToStructuredType();
                }
            }

            SelectExpandParser innerSelectParser = new SelectExpandParser(
                resolver,
                selectText,
                targetStructuredType,
                this.maxRecursionDepth - 1,
                this.enableCaseInsensitiveBuiltinIdentifier,
                this.enableNoDollarQueryOptions);

            return innerSelectParser.ParseSelect();
        }

        /// <summary>
        /// Parse the expand option in the select/expand option text.
        /// </summary>
        /// <param name="pathToken">The path segment token</param>
        /// <returns>The expand option for select/expand</returns>
        private ExpandToken ParseInnerExpand(PathSegmentToken pathToken)
        {
            // advance to the equal sign
            this.lexer.NextToken();

            string expandText = UriParserHelper.ReadQueryOption(this.lexer);

            IEdmStructuredType targetStructuredType = null;
            if (this.resolver != null && this.parentStructuredType != null)
            {
                var parentProperty = this.resolver.ResolveProperty(parentStructuredType, pathToken.Identifier);

                // it is a property, need to find the type.
                // Like $expand=Friends($expand=Trips($expand=*)), when expandText becomes "Trips($expand=*)",
                // find navigation property Trips of Friends, then get Entity type of Trips.
                // or for select query like: $select=Address($expand=City)
                if (parentProperty != null)
                {
                    targetStructuredType = parentProperty.Type.ToStructuredType();
                }
            }

            SelectExpandParser innerExpandParser = new SelectExpandParser(
                resolver,
                expandText,
                targetStructuredType,
                this.maxRecursionDepth - 1,
                this.enableCaseInsensitiveBuiltinIdentifier,
                this.enableNoDollarQueryOptions);

            return innerExpandParser.ParseExpand();
        }

        /// <summary>
        /// Parse the level option in the expand option text.
        /// </summary>
        /// <returns>The level option for expand in long type</returns>
        private long? ParseInnerLevel()
        {
            long? levelsOption = null;

            // advance to the equal sign
            this.lexer.NextToken();
            string levelsText = UriParserHelper.ReadQueryOption(this.lexer);
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
        /// Parse the compute option in the expand option text.
        /// </summary>
        /// <returns>The compute option for expand</returns>
        private ComputeToken ParseInnerCompute()
        {
            this.lexer.NextToken();
            string computeText = UriParserHelper.ReadQueryOption(this.lexer);

            UriQueryExpressionParser computeParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
            return computeParser.ParseCompute(computeText);
        }

        /// <summary>
        /// Parse the apply option in the expand option text.
        /// </summary>
        /// <returns>The apply option for expand</returns>
        private IEnumerable<QueryToken> ParseInnerApply()
        {
            this.lexer.NextToken();
            string applyText = UriParserHelper.ReadQueryOption(this.lexer);

            UriQueryExpressionParser applyParser = new UriQueryExpressionParser(this.MaxOrderByDepth, enableCaseInsensitiveBuiltinIdentifier);
            return applyParser.ParseApply(applyText);
        }
    }
}