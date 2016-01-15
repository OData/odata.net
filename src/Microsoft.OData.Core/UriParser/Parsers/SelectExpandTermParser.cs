//---------------------------------------------------------------------
// <copyright file="SelectExpandTermParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Sub-parser that <see cref="SelectExpandParser"/> uses to parse a single select or expand term.
    /// Uses a provided Lexer, which must be positioned at the term, to parse the term.
    /// </summary>
    internal sealed class SelectExpandTermParser
    {
        /// <summary>
        /// Lexer provided by <see cref="SelectExpandParser"/>.
        /// </summary>
        private readonly ExpressionLexer lexer;

        /// <summary>
        /// Max length of a select or expand path.
        /// </summary>
        private readonly int maxPathLength;

        /// <summary>
        /// True if we are parsing select, false if we are parsing expand.
        /// </summary>
        private readonly bool isSelect;

        /// <summary>
        /// Constructs a term parser.
        /// </summary>
        /// <param name="lexer">Lexer to use for parsing the term. Should be position at the term to parse.</param>
        /// <param name="maxPathLength">Max length of a select or expand path.</param>
        /// <param name="isSelect">True if we are parsing select, false if we are parsing expand.</param>
        internal SelectExpandTermParser(ExpressionLexer lexer, int maxPathLength, bool isSelect)
        {
            this.lexer = lexer;
            this.maxPathLength = maxPathLength;
            this.isSelect = isSelect;
        }

        /// <summary>
        /// Parses a select or expand term into a PathSegmentToken.
        /// Assumes the lexer is positioned at the beginning of the term to parse.
        /// When done, the lexer will be positioned at whatever is after the identifier. 
        /// </summary>
        /// <param name="allowRef">Whether the $ref operation is valid in this token.</param>
        /// <returns>parsed query token</returns>
        internal PathSegmentToken ParseTerm(bool allowRef = false)
        {
            int pathLength;
            PathSegmentToken token = this.ParseSegment(null, allowRef);
            if (token != null)
            {
                pathLength = 1;
            }
            else
            {
                return null;
            }

            this.CheckPathLength(pathLength);

            // If this property was a path, walk that path. e.g. SomeComplex/SomeInnerComplex/SomeNavProp
            while (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Slash)
            {
                // Move from '/' to the next segment
                this.lexer.NextToken();

                // TODO: Could remove V4 if we don't want to allow a trailing '/' character
                // Allow a single trailing slash for backwards compatibility with the WCF DS Server parser.
                if (pathLength > 1 && this.lexer.CurrentToken.Kind == ExpressionTokenKind.End)
                {
                    break;
                }

                token = this.ParseSegment(token, allowRef);
                if (token != null)
                {
                    this.CheckPathLength(++pathLength);
                }
            }

            return token;
        }

        /// <summary>
        /// Check that the current path length is less than the maximum path length
        /// </summary>
        /// <param name="pathLength">the current path length</param>
        private void CheckPathLength(int pathLength)
        {
            if (pathLength > this.maxPathLength)
            {
                throw new ODataException(ODataErrorStrings.UriQueryExpressionParser_TooDeep);
            }
        }

        /// <summary>
        /// Uses the ExpressionLexer to visit the next ExpressionToken, and delegates parsing of segments, type segments, identifiers, 
        /// and the star token to other methods.
        /// </summary>
        /// <param name="previousSegment">Previously parsed PathSegmentToken, or null if this is the first token.</param>
        /// <param name="allowRef">Whether the $ref operation is valid in this token.</param>
        /// <returns>A parsed PathSegmentToken representing the next segment in this path.</returns>
        private PathSegmentToken ParseSegment(PathSegmentToken previousSegment, bool allowRef)
        {
            // TODO $count is defined in specification for expand, it is not supported now. Also note $count is not supported with star as expand option.
            if (this.lexer.CurrentToken.Text.StartsWith("$", StringComparison.Ordinal) && (!allowRef || this.lexer.CurrentToken.Text != UriQueryConstants.RefSegment))
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_SystemTokenInSelectExpand(this.lexer.CurrentToken.Text, this.lexer.ExpressionText));
            }

            // Some check here to throw exception, both prop1/*/prop2 and */$ref/prop will throw exception, both are for $expand cases
            if (!isSelect)
            {
                if (previousSegment != null && previousSegment.Identifier == UriQueryConstants.Star && this.lexer.CurrentToken.GetIdentifier() != UriQueryConstants.RefSegment)
                {
                    // Star can only be followed with $ref
                    throw new ODataException(ODataErrorStrings.ExpressionToken_OnlyRefAllowWithStarInExpand);
                }
                else if (previousSegment != null && previousSegment.Identifier == UriQueryConstants.RefSegment)
                {
                    // $ref should not have more property followed.
                    throw new ODataException(ODataErrorStrings.ExpressionToken_NoPropAllowedAfterRef);
                }
            }

            string propertyName;

            if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Dot)
            {
                propertyName = this.lexer.ReadDottedIdentifier(this.isSelect);
            }
            else if (this.lexer.CurrentToken.Kind == ExpressionTokenKind.Star)
            {
                // "*/$ref" is supported in expand
                if (this.lexer.PeekNextToken().Kind == ExpressionTokenKind.Slash && isSelect)
                {
                    throw new ODataException(ODataErrorStrings.ExpressionToken_IdentifierExpected(this.lexer.Position));
                }
                else if (previousSegment != null && !isSelect)
                {
                    // expand option like "customer?$expand=VIPCUstomer/*" is not allowed as specification does not allowed any property before *.
                    throw new ODataException(ODataErrorStrings.ExpressionToken_NoSegmentAllowedBeforeStarInExpand);
                }
                
                propertyName = this.lexer.CurrentToken.Text;
                this.lexer.NextToken();
            }
            else
            {
                propertyName = this.lexer.CurrentToken.GetIdentifier();
                this.lexer.NextToken();
            }

            return new NonSystemToken(propertyName, null, previousSegment);
        }
    }
}