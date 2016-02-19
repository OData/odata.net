//---------------------------------------------------------------------
// <copyright file="SelectPropertyVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Visitors
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        /// The type for this level of the select
        /// </summary>
        private readonly IEdmStructuredType edmType;

        /// <summary>
        /// Resolver for uri parser.
        /// </summary>
        private readonly ODataUriResolver resolver;

        /// <summary>
        /// Build a property visitor to visit the select tree and decorate a SelectExpandClause
        /// </summary>
        /// <param name="model">The model used for binding.</param>
        /// <param name="edmType">The entity type that the $select is being applied to.</param>
        /// <param name="maxDepth">the maximum recursive depth.</param>
        /// <param name="expandClauseToDecorate">The already built expand clause to decorate</param>
        /// <param name="resolver">Resolver for uri parser.</param>
        public SelectPropertyVisitor(IEdmModel model, IEdmStructuredType edmType, int maxDepth, SelectExpandClause expandClauseToDecorate, ODataUriResolver resolver)
        {
            this.model = model;
            this.edmType = edmType;
            this.maxDepth = maxDepth;
            this.expandClauseToDecorate = expandClauseToDecorate;
            this.resolver = resolver ?? ODataUriResolver.Default;
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
                    this.expandClauseToDecorate.AddToSelectedItems(newSelectItem);
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
            IEdmStructuredType currentLevelType = this.edmType;

            // first, walk through all type segments in a row, converting them from tokens into segments.
            if (tokenIn.IsNamespaceOrContainerQualified())
            {
                PathSegmentToken firstNonTypeToken;
                pathSoFar.AddRange(SelectExpandPathBinder.FollowTypeSegments(tokenIn, this.model, this.maxDepth, this.resolver, ref currentLevelType, out firstNonTypeToken));
                Debug.Assert(firstNonTypeToken != null, "Did not get last token.");
                tokenIn = firstNonTypeToken as NonSystemToken;
                if (tokenIn == null)
                {
                    throw new ODataException(ODataErrorStrings.SelectPropertyVisitor_SystemTokenInSelect(firstNonTypeToken.Identifier));
                }  
            }

            // next, create a segment for the first non-type segment in the path.
            ODataPathSegment lastSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(tokenIn, this.model, currentLevelType, resolver);

            // next, create an ODataPath and add the segments to it.
            if (lastSegment != null)
            {
                pathSoFar.Add(lastSegment);

                bool hasCollectionInPath = false;

                // try create a complex type property path.
                while (true)
                {
                    // no need to go on if the current property is not of complex type or collection of complex type.
                    currentLevelType = lastSegment.EdmType as IEdmStructuredType;
                    var collectionType = lastSegment.EdmType as IEdmCollectionType;
                    if ((currentLevelType == null || currentLevelType.TypeKind != EdmTypeKind.Complex) 
                        && (collectionType == null || collectionType.ElementType.TypeKind() != EdmTypeKind.Complex))
                    {
                        break;
                    }

                    NonSystemToken nextToken = tokenIn.NextToken as NonSystemToken;
                    if (nextToken == null)
                    {
                        break;
                    }

                    lastSegment = null;

                    // This means last segment a collection of complex type,
                    // current segment can only be type cast and cannot be proprty name.
                    if (currentLevelType == null)
                    {
                        currentLevelType = collectionType.ElementType.Definition as IEdmStructuredType;
                        hasCollectionInPath = true;
                    }
                    else if (!hasCollectionInPath)
                    {
                        // If there is no collection type in the path yet, will try to bind property for the next token
                        // first try bind the segment as property.
                        lastSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(nextToken, this.model,
                            currentLevelType, resolver);
                    }

                    // then try bind the segment as type cast.
                    if (lastSegment == null)
                    {
                        IEdmStructuredType typeFromNextToken = UriEdmHelpers.FindTypeFromModel(this.model, nextToken.Identifier, this.resolver) as IEdmStructuredType;

                        if (typeFromNextToken.IsOrInheritsFrom(currentLevelType))
                        {
                            lastSegment = new TypeSegment(typeFromNextToken, /*entitySet*/null);
                        }
                    }

                    // type cast failed too.
                    if (lastSegment == null)
                    {
                        break;
                    }

                    // try move to and add next path segment.
                    tokenIn = nextToken;
                    pathSoFar.Add(lastSegment);
                }
            }

            ODataSelectPath selectedPath = new ODataSelectPath(pathSoFar);

            var selectionItem = new PathSelectItem(selectedPath);

            // non-navigation cases do not allow further segments in $select.
            if (tokenIn.NextToken != null)
            {
                throw new ODataException(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
            }

            // if the selected item is a nav prop, then see if its already there before we add it.
            NavigationPropertySegment trailingNavPropSegment = selectionItem.SelectedPath.LastSegment as NavigationPropertySegment;
            if (trailingNavPropSegment != null)
            {
                if (this.expandClauseToDecorate.SelectedItems.Any(x => x is PathSelectItem &&
                    ((PathSelectItem)x).SelectedPath.Equals(selectedPath)))
                {
                    return;
                }
            }

            this.expandClauseToDecorate.AddToSelectedItems(selectionItem);         
        }
    }
}