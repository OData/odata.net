//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Visit a Select property and use it to decorate a SelectExpand Tree
    /// </summary>
    internal sealed class SelectPropertyVisitor : PathSegmentTokenVisitor
    {
        /// <summary>
        /// The model used for binding.
        /// </summary>
        private readonly IEdmModel model;

        /// <summary>
        /// the maximum recursive depth.
        /// </summary>
        private readonly int maxDepth;

        /// <summary>
        /// The expand tree to decorate.
        /// </summary>
        private readonly SelectExpandClause expandClauseToDecorate;

        /// <summary>
        /// The entity type for this level of the select
        /// </summary>
        private readonly IEdmEntityType entityType;

        /// <summary>
        /// Build a property visitor to visit the select tree and decorate a SelectExpandClause
        /// </summary>
        /// <param name="model">The model used for binding.</param>
        /// <param name="entityType">The entity type that the $select is being applied to.</param>
        /// <param name="maxDepth">the maximum recursive depth.</param>
        /// <param name="expandClauseToDecorate">The already built expand clause to decorate</param>
        public SelectPropertyVisitor(IEdmModel model, IEdmEntityType entityType, int maxDepth, SelectExpandClause expandClauseToDecorate)
        {
            this.model = model;
            this.entityType = entityType;
            this.maxDepth = maxDepth;
            this.expandClauseToDecorate = expandClauseToDecorate;
        }

        /// <summary>
        /// The expand tree that we're decorating
        /// </summary>
        public SelectExpandClause DecoratedExpandClause
        {
            get { return this.expandClauseToDecorate; }
        }

        /// <summary>
        /// Visit a System Token
        /// </summary>
        /// <param name="tokenIn">the system token to visit</param>
        public override void Visit(SystemToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");
            throw new ODataException(ODataErrorStrings.SelectPropertyVisitor_SystemTokenInSelect(tokenIn.Identifier));
        }

        /// <summary>
        /// Visit a NonSystemToken
        /// </summary>
        /// <param name="tokenIn">the non sytem token to visit</param>
        public override void Visit(NonSystemToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");
            
            // before looking for type segments or paths, handle both of the wildcard cases.
            if (tokenIn.NextToken == null)
            {
                SelectItem newSelectItem;
                if (SelectPathSegmentTokenBinder.TryBindAsWildcard(tokenIn, this.model, out newSelectItem))
                {
                    this.expandClauseToDecorate.AddSelectItem(newSelectItem);
                    return;
                }
            }

            this.ProcessTokenAsPath(tokenIn);
        }

        /// <summary>
        /// process a nonsystemtoken as a path, following any type segments if necessary
        /// </summary>
        /// <param name="tokenIn">the token to process</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It makes sense to keep all of this logic in one place")]
        private void ProcessTokenAsPath(NonSystemToken tokenIn)
        {
            Debug.Assert(tokenIn != null, "tokenIn != null");

            List<ODataPathSegment> pathSoFar = new List<ODataPathSegment>();
            IEdmEntityType currentLevelEntityType = this.entityType;

            // first, walk through all type segments in a row, converting them from tokens into segments.
            if (tokenIn.IsNamespaceOrContainerQualified())
            {
                PathSegmentToken firstNonTypeToken;
                pathSoFar.AddRange(SelectExpandPathBinder.FollowTypeSegments(tokenIn, this.model, this.maxDepth, ref currentLevelEntityType, out firstNonTypeToken));
                Debug.Assert(firstNonTypeToken != null, "Did not get last token.");
                tokenIn = firstNonTypeToken as NonSystemToken;
                if (tokenIn == null)
                {
                    throw new ODataException(ODataErrorStrings.SelectPropertyVisitor_SystemTokenInSelect(firstNonTypeToken.Identifier));
                }  
            }

            // next, create a segment for the first non-type segment in the path.
            ODataPathSegment lastSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(tokenIn, this.model, currentLevelEntityType);
            Debug.Assert(lastSegment != null, "nextSegment != null");

            // next, create an ODataPath and add the segments to it.
            pathSoFar.Add(lastSegment);
            ODataSelectPath selectedPath = new ODataSelectPath(pathSoFar);

            var navigationSelection = new PathSelectItem(selectedPath);
                
            // next, create a selection item for the path, based on the last segment's type.
            // TODO: just have PathSelectItem
            if (lastSegment is NavigationPropertySegment)
            {
                bool foundExactExpand = false;
                bool foundDifferentTypeExpand = false;
                foreach (var subItem in this.expandClauseToDecorate.Expansion.ExpandItems)
                {
                    IEdmNavigationProperty subItemNavigationProperty = subItem.PathToNavigationProperty.GetNavigationProperty();
                    if (subItem.PathToNavigationProperty.Equals(navigationSelection.SelectedPath))
                    {
                        foundExactExpand = true;

                        if (tokenIn.NextToken == null)
                        {
                            subItem.SelectAndExpand.SetAllSelectionRecursively();
                        }
                        else
                        {
                            SelectPropertyVisitor nextLevelVisitor = new SelectPropertyVisitor(this.model, subItemNavigationProperty.ToEntityType(), this.maxDepth, subItem.SelectAndExpand);
                            tokenIn.NextToken.Accept(nextLevelVisitor);
                        }
                    }
                    else if (subItem.PathToNavigationProperty.LastSegment.Equals(navigationSelection.SelectedPath.LastSegment))
                    {
                        foundDifferentTypeExpand = true;
                    }
                }

                if (foundDifferentTypeExpand && !foundExactExpand)
                {
                    throw new ODataException(ODataErrorStrings.SelectPropertyVisitor_DisparateTypeSegmentsInSelectExpand);
                }

                if (!foundExactExpand)
                {
                    // if it has sub-properties selected, then require it to have been expanded.
                    if (tokenIn.NextToken != null)
                    {
                        throw new ODataException(ODataErrorStrings.SelectionItemBinder_NoExpandForSelectedProperty(tokenIn.Identifier));
                    }

                    // otherwise just add it to the partial selection.
                    this.expandClauseToDecorate.AddSelectItem(navigationSelection);
                }
                else
                {
                    this.expandClauseToDecorate.InitializeEmptySelection();
                }
            }
            else
            {
                // non-navigation cases do not allow further segments in $select.
                if (tokenIn.NextToken != null)
                {
                    throw new ODataException(ODataErrorStrings.SelectionItemBinder_NonNavigationPathToken);
                }

                this.expandClauseToDecorate.AddSelectItem(navigationSelection);
            }
        }
    }
}
