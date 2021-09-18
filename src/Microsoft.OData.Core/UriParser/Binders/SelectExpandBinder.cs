//---------------------------------------------------------------------
// <copyright file="SelectExpandBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.OData.Metadata;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Aggregation;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// ExpandOption variant of an SelectExpandBinder, where the default selection item for a given level is based on the select at that level
    /// instead of the top level select clause. If nothing is selected for a given expand in the ExpandOption syntax, then we by default
    /// select all from that item, instead of selecting nothing (and therefore pruning the expand off of the tree).
    /// </summary>
    internal sealed class SelectExpandBinder
    {
        /// <summary>
        /// The configuration used for binding.
        /// </summary>
        private readonly ODataUriParserConfiguration configuration;

        /// <summary>
        /// The navigation source at the current level expand.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// The navigation source at the resource path.
        /// </summary>
        private readonly IEdmNavigationSource resourcePathNavigationSource;

        /// <summary>
        /// The entity type at the current level expand.
        /// </summary>
        private readonly IEdmStructuredType edmType;

        /// <summary>
        /// The segments parsed in path and query option.
        /// </summary>
        private List<ODataPathSegment> parsedSegments = new List<ODataPathSegment>();

        private BindingState state;

        public SelectExpandBinder(ODataUriParserConfiguration configuration, ODataPathInfo odataPathInfo, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(odataPathInfo.TargetStructuredType, "edmType");

            this.configuration = configuration;
            this.edmType = odataPathInfo.TargetStructuredType;
            this.navigationSource = odataPathInfo.TargetNavigationSource;
            this.parsedSegments = odataPathInfo.Segments.ToList();
            this.state = state;

            if (this.state != null)
            {
                this.resourcePathNavigationSource = this.state.ResourcePathNavigationSource;
            }
            else
            {
                this.resourcePathNavigationSource = odataPathInfo.TargetNavigationSource;
            }
        }

        /// <summary>
        /// The model used for binding.
        /// </summary>
        public IEdmModel Model
        {
            get { return this.configuration.Model; }
        }

        /// <summary>
        /// The top level type.
        /// </summary>
        public IEdmStructuredType EdmType
        {
            get { return this.edmType; }
        }

        /// <summary>
        /// The top level navigation source for this level.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// The navigation source at the resource path.
        /// </summary>
        public IEdmNavigationSource ResourcePathNavigationSource
        {
            get { return this.resourcePathNavigationSource; }
        }

        /// <summary>
        /// The settings to use when binding.
        /// </summary>
        private ODataUriParserSettings Settings
        {
            get
            {
                return this.configuration.Settings;
            }
        }

        /// <summary>
        /// The configuration used for binding.
        /// </summary>
        private ODataUriParserConfiguration Configuration
        {
            get
            {
                return this.configuration;
            }
        }

        /// <summary>
        /// Bind the $expand <see cref="ExpandToken"/> and $select <see cref="SelectToken"/> at this level.
        /// </summary>
        /// <param name="expandToken">the expand token to visit.</param>
        /// <param name="selectToken">the select token to visit.</param>
        /// <returns>a SelectExpand clause based on <see cref="ExpandToken"/> and <see cref="SelectToken"/>.</returns>
        public SelectExpandClause Bind(ExpandToken expandToken, SelectToken selectToken)
        {
            List<SelectItem> selectExpandItems = new List<SelectItem>();

            // $expand=
            if (expandToken != null && expandToken.ExpandTerms.Any())
            {
                selectExpandItems.AddRange(expandToken.ExpandTerms.Select(this.GenerateExpandItem).Where(s => s != null));
            }

            // $select=
            bool isAllSelected = true;
            if (selectToken != null && selectToken.SelectTerms.Any())
            {
                // if there are any select items at this level then allSelected is false, otherwise it's true.
                isAllSelected = false;

                foreach (SelectTermToken selectTermToken in selectToken.SelectTerms)
                {
                    SelectItem selectItem = GenerateSelectItem(selectTermToken);
                    PathSelectItem selectPathItem = selectItem as PathSelectItem;
                    bool duplicate = false;

                    if (selectPathItem != null)
                    {
                        // It's not allowed to have multiple select clause with the same path.
                        // For example: $select=abc($top=2),abc($skip=2) is not allowed by design.
                        // Customer should combine them together, for example: $select=abc($top=2;$skip=2).
                        // The logic is different with ExpandTreeNormalizer. We should change the logic in ExpandTreeNormalizer
                        // in next breaking change version.
                        // For backward compatibility with previous versions of OData Library, we only validate
                        // if one of the select items has options.
                        foreach (PathSelectItem existingItem in selectExpandItems.OfType<PathSelectItem>())
                        {
                            if ((selectPathItem.HasOptions && OverLaps(selectPathItem, existingItem)) || (existingItem.HasOptions && OverLaps(existingItem, selectPathItem)))
                            {
                                throw new ODataException(ODataErrorStrings.SelectTreeNormalizer_MultipleSelecTermWithSamePathFound(ToPathString(selectTermToken.PathToProperty)));
                            }

                            // two items without options are identical -- for backward compat just ignore the new one
                            if (selectPathItem.SelectedPath.Equals(existingItem.SelectedPath))
                            {
                                duplicate = true;
                            }
                        }
                    }

                    if (!duplicate)
                    {
                        AddToSelectedItems(selectItem, selectExpandItems);
                    }
                }
            }

            // It's better to return "null" if both expand and select are null.
            // However, in order to be consistent, we returns empty "SelectExpandClause" with AllSelected = true.
            return new SelectExpandClause(selectExpandItems, isAllSelected);
        }

        /// <summary>
        /// Get the path string for a path segment token.
        /// </summary>
        /// <param name="head">The head of the path</param>
        /// <returns>The path string.</returns>
        internal static string ToPathString(PathSegmentToken head)
        {
            StringBuilder sb = new StringBuilder();
            PathSegmentToken curr = head;
            while (curr != null)
            {
                sb.Append(curr.Identifier);

                NonSystemToken nonSystem = curr as NonSystemToken;
                if (nonSystem != null && nonSystem.NamedValues != null)
                {
                    sb.Append("(");
                    bool first = true;
                    foreach (var item in nonSystem.NamedValues)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(",");
                        }

                        sb.Append(item.Name).Append("=").Append(item.Value.Value);
                    }

                    sb.Append(")");
                }

                curr = curr.NextToken;
                if (curr != null)
                {
                    sb.Append("/");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the first path is entirely contained in the second path.
        /// </summary>
        /// <param name="firstPath">First path item</param>
        /// <param name="secondPath">Second path item</param>
        /// <returns>The boolean value.</returns>
        private static bool OverLaps(PathSelectItem firstPath, PathSelectItem secondPath)
        {
            IEnumerator<ODataPathSegment> first = firstPath.SelectedPath.GetEnumerator();
            IEnumerator<ODataPathSegment> second = secondPath.SelectedPath.GetEnumerator();

            bool completed;
            while ((completed = first.MoveNext()) && second.MoveNext() && first.Current.Identifier == second.Current.Identifier)
            {
            }

            return !completed;
        }

        /// <summary>
        /// Generate a select item <see cref="SelectItem"/> based on a <see cref="SelectTermToken"/>.
        /// for example:  abc/efg($count=true;$filter=....;$top=1)
        /// </summary>
        /// <param name="tokenIn">the select term token to visit</param>
        /// <returns>the select item for this select term token.</returns>
        private SelectItem GenerateSelectItem(SelectTermToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");
            ExceptionUtils.CheckArgumentNotNull(tokenIn.PathToProperty, "pathToProperty");

            VerifySelectedPath(tokenIn);

            SelectItem newSelectItem;
            if (ProcessWildcardTokenPath(tokenIn, out newSelectItem))
            {
                return newSelectItem;
            }

            IList<ODataPathSegment> selectedPath = ProcessSelectTokenPath(tokenIn.PathToProperty);
            Debug.Assert(selectedPath.Count > 0);

            // Navigation property should be the last segment in select path.
            if (VerifySelectedNavigationProperty(selectedPath, tokenIn))
            {
                return new PathSelectItem(new ODataSelectPath(selectedPath));
            }

            IEdmNavigationSource targetNavigationSource = null;
            ODataPathSegment lastSegment = selectedPath.Last();
            IEdmType targetElementType = lastSegment.TargetEdmType;
            IEdmCollectionType collection = targetElementType as IEdmCollectionType;
            if (collection != null)
            {
                targetElementType = collection.ElementType.Definition;
            }

            IEdmTypeReference elementTypeReference = targetElementType.ToTypeReference();

            // When Creating Range Variables, we only need a Navigation Source when the elementTypeReference is a StructuredTypeReference.
            // When the elementTypeReference is NOT StructuredTypeReference, We will create a NonResourceRangeVariable which don't require a Navigation Source.
            if (elementTypeReference != null && elementTypeReference.IsStructured())
            {
                // We should use the "NavigationSource" at this level for the next level binding.
                targetNavigationSource = this.NavigationSource;
            }

            // $compute
            ComputeClause compute = BindCompute(tokenIn.ComputeOption, this.ResourcePathNavigationSource, targetNavigationSource, elementTypeReference);
            HashSet<EndPathToken> generatedProperties = GetGeneratedProperties(compute, null);

            // $filter
            FilterClause filter = BindFilter(tokenIn.FilterOption, this.ResourcePathNavigationSource, targetNavigationSource, elementTypeReference, generatedProperties);

            // $orderby
            OrderByClause orderBy = BindOrderby(tokenIn.OrderByOptions, this.ResourcePathNavigationSource, targetNavigationSource, elementTypeReference, generatedProperties);

            // $search
            SearchClause search = BindSearch(tokenIn.SearchOption, this.ResourcePathNavigationSource, targetNavigationSource, elementTypeReference);

            // $select
            List<ODataPathSegment> parsedPath = new List<ODataPathSegment>(this.parsedSegments);
            parsedPath.AddRange(selectedPath);
            SelectExpandClause selectExpand = BindSelectExpand(null, tokenIn.SelectOption, parsedPath, this.ResourcePathNavigationSource, targetNavigationSource, elementTypeReference, generatedProperties);

            return new PathSelectItem(new ODataSelectPath(selectedPath),
                targetNavigationSource,
                selectExpand,
                filter,
                orderBy,
                tokenIn.TopOption,
                tokenIn.SkipOption,
                tokenIn.CountQueryOption,
                search,
                compute);
        }

        /// <summary>
        /// Generate an expand item (and a select item for the implicit nav prop if necessary) based on an ExpandTermToken
        /// </summary>
        /// <param name="tokenIn">the expandTerm token to visit</param>
        /// <returns>the expand item for this expand term token.</returns>
        private SelectItem GenerateExpandItem(ExpandTermToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");

            PathSegmentToken currentToken = tokenIn.PathToNavigationProp;

            IEdmStructuredType currentLevelEntityType = this.EdmType;
            List<ODataPathSegment> pathSoFar = new List<ODataPathSegment>();
            PathSegmentToken firstNonTypeToken = currentToken;

            if (currentToken.IsNamespaceOrContainerQualified())
            {
                pathSoFar.AddRange(SelectExpandPathBinder.FollowTypeSegments(currentToken, this.Model, this.Settings.SelectExpandLimit, this.configuration.Resolver, ref currentLevelEntityType, out firstNonTypeToken));
            }

            IEdmProperty edmProperty = this.configuration.Resolver.ResolveProperty(currentLevelEntityType, firstNonTypeToken.Identifier);
            if (edmProperty == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(currentLevelEntityType.FullTypeName(), currentToken.Identifier));
            }

            IEdmNavigationProperty currentNavProp = edmProperty as IEdmNavigationProperty;
            IEdmStructuralProperty currentComplexProp = edmProperty as IEdmStructuralProperty;
            if (currentNavProp == null && currentComplexProp == null)
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty(currentToken.Identifier, currentLevelEntityType.FullTypeName()));
            }

            if (currentComplexProp != null)
            {
                currentNavProp = ParseComplexTypesBeforeNavigation(currentComplexProp, ref firstNonTypeToken, pathSoFar);
            }

            bool isRef = false;
            bool isCount = false;
            bool hasDerivedTypeSegment = false;
            IEdmType derivedType = null;

            // Handle $expand=Customer/Fully.Qualified.Namespace.VipCustomer
            // The deriveTypeToken is Fully.Qualified.Namespace.VipCustomer
            if (firstNonTypeToken.NextToken != null && firstNonTypeToken.NextToken.IsNamespaceOrContainerQualified())
            {
                hasDerivedTypeSegment = true;
                derivedType = UriEdmHelpers.FindTypeFromModel(this.Model, firstNonTypeToken.NextToken.Identifier, this.configuration.Resolver);

                if (derivedType == null)
                {
                    // Exception example: The type Fully.Qualified.Namespace.UndefinedType is not defined in the model.
                    throw new ODataException(ODataErrorStrings.ExpandItemBinder_CannotFindType(firstNonTypeToken.NextToken.Identifier));
                }

                // In this example: $expand=Customer/Fully.Qualified.Namespace.VipCustomer
                // We validate that the derived type Fully.Qualified.Namespace.VipCustomer is related to Navigation property Customer.
                UriEdmHelpers.CheckRelatedTo(currentNavProp.ToEntityType(), derivedType);
            }

            // ensure that we're always dealing with proper V4 syntax
            if (firstNonTypeToken?.NextToken?.NextToken != null && !hasDerivedTypeSegment)
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
            }

            if ((firstNonTypeToken.NextToken != null && !hasDerivedTypeSegment) || 
                (firstNonTypeToken?.NextToken?.NextToken != null && hasDerivedTypeSegment))
            {
                PathSegmentToken nextToken = hasDerivedTypeSegment ? firstNonTypeToken.NextToken.NextToken : firstNonTypeToken.NextToken;

                // lastly... make sure that, since we're on a NavProp, that the next token isn't null.
                if (nextToken.Identifier == UriQueryConstants.RefSegment)
                {
                    isRef = true;
                }
                else if (nextToken.Identifier == UriQueryConstants.CountSegment)
                {
                    isCount = true;
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
                }
            }

            // Add the segments in select and expand to parsed segments
            List<ODataPathSegment> parsedPath = new List<ODataPathSegment>(this.parsedSegments);
            parsedPath.AddRange(pathSoFar);
            IEdmNavigationSource targetNavigationSource = null;
            if (this.NavigationSource != null)
            {
                IEdmPathExpression bindingPath;
                targetNavigationSource = this.NavigationSource.FindNavigationTarget(currentNavProp, BindingPathHelper.MatchBindingPath, parsedPath, out bindingPath);
            }

            NavigationPropertySegment navSegment = new NavigationPropertySegment(currentNavProp, targetNavigationSource);
            pathSoFar.Add(navSegment);
            parsedPath.Add(navSegment); // Add the navigation property segment to parsed segments for future usage.

            if (hasDerivedTypeSegment)
            {
                TypeSegment derivedTypeSegment = new TypeSegment(derivedType, targetNavigationSource);
                pathSoFar.Add(derivedTypeSegment);
                parsedPath.Add(derivedTypeSegment);
            }

            ODataExpandPath pathToNavProp = new ODataExpandPath(pathSoFar);

            // $apply
            ApplyClause applyOption = BindApply(tokenIn.ApplyOptions, this.ResourcePathNavigationSource, targetNavigationSource);

            // $compute
            ComputeClause computeOption = BindCompute(tokenIn.ComputeOption, this.ResourcePathNavigationSource, targetNavigationSource);

            var generatedProperties = GetGeneratedProperties(computeOption, applyOption);
            bool collapsed = applyOption?.Transformations.Any(t => t.Kind == TransformationNodeKind.Aggregate || t.Kind == TransformationNodeKind.GroupBy) ?? false;

            // $filter
            FilterClause filterOption = BindFilter(tokenIn.FilterOption, this.ResourcePathNavigationSource, targetNavigationSource, null, generatedProperties, collapsed);

            // $orderby
            OrderByClause orderbyOption = BindOrderby(tokenIn.OrderByOptions, this.ResourcePathNavigationSource, targetNavigationSource, null, generatedProperties, collapsed);

            // $search
            SearchClause searchOption = BindSearch(tokenIn.SearchOption, this.ResourcePathNavigationSource, targetNavigationSource, null);

            if (isRef)
            {
                return new ExpandedReferenceSelectItem(pathToNavProp, targetNavigationSource, filterOption, orderbyOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.CountQueryOption, searchOption, computeOption, applyOption);
            }

            if (isCount)
            {
                return new ExpandedCountSelectItem(pathToNavProp, targetNavigationSource, filterOption, searchOption);
            }

            // $select & $expand
            SelectExpandClause subSelectExpand = BindSelectExpand(tokenIn.ExpandOption, tokenIn.SelectOption, parsedPath, this.ResourcePathNavigationSource, targetNavigationSource, null, generatedProperties, collapsed);

            // $levels
            LevelsClause levelsOption = ParseLevels(tokenIn.LevelsOption, currentLevelEntityType, currentNavProp);

            return new ExpandedNavigationSelectItem(pathToNavProp,
                targetNavigationSource, subSelectExpand, filterOption, orderbyOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.CountQueryOption, searchOption, levelsOption, computeOption, applyOption);
        }

        /// <summary>
        /// Bind the apply clause <see cref="ApplyClause"/> at this level.
        /// </summary>
        /// <param name="applyToken">The apply tokens to visit.</param>
        /// <param name="resourcePathNavigationSource">The navigation source at the resource path.</param>
        /// <param name="targetNavigationSource">The target navigation source at the current level.</param>
        /// <returns>The null or the built apply clause.</returns>
        private ApplyClause BindApply(IEnumerable<QueryToken> applyToken, IEdmNavigationSource resourcePathNavigationSource, IEdmNavigationSource targetNavigationSource)
        {
            if (applyToken != null && applyToken.Any())
            {
                MetadataBinder binder = BuildNewMetadataBinder(this.Configuration, resourcePathNavigationSource, targetNavigationSource, null);
                ApplyBinder applyBinder = new ApplyBinder(binder.Bind, binder.BindingState);
                return applyBinder.BindApply(applyToken);
            }

            return null;
        }

        /// <summary>
        /// Bind the compute clause <see cref="ComputeToken"/> at this level.
        /// </summary>
        /// <param name="computeToken">The compute token to visit.</param>
        /// <param name="resourcePathNavigationSource">The navigation source at the resource path.</param>
        /// <param name="targetNavigationSource">The target navigation source at the current level.</param>
        /// <param name="elementType">The target element type.</param>
        /// <returns>The null or the built compute clause.</returns>
        private ComputeClause BindCompute(ComputeToken computeToken, IEdmNavigationSource resourcePathNavigationSource, IEdmNavigationSource targetNavigationSource, IEdmTypeReference elementType = null)
        {
            if (computeToken != null)
            {
                MetadataBinder binder = BuildNewMetadataBinder(this.Configuration, resourcePathNavigationSource, targetNavigationSource, elementType);
                ComputeBinder computeBinder = new ComputeBinder(binder.Bind);
                return computeBinder.BindCompute(computeToken);
            }

            return null;
        }

        /// <summary>
        /// Bind the filter clause <see cref="FilterClause"/> at this level.
        /// </summary>
        /// <param name="filterToken">The filter token to visit.</param>
        /// <param name="resourcePathNavigationSource">The navigation source at the resource path.</param>
        /// <param name="targetNavigationSource">The target navigation source at the current level.</param>
        /// <param name="elementType">The Edm element type.</param>
        /// <param name="generatedProperties">The generated properties.</param>
        /// <param name="collapsed">The collapsed boolean value.</param>
        /// <returns>The null or the built filter clause.</returns>
        private FilterClause BindFilter(QueryToken filterToken, IEdmNavigationSource resourcePathNavigationSource, IEdmNavigationSource targetNavigationSource,
            IEdmTypeReference elementType, HashSet<EndPathToken> generatedProperties, bool collapsed = false)
        {
            if (filterToken != null)
            {
                MetadataBinder binder = BuildNewMetadataBinder(this.Configuration, resourcePathNavigationSource, targetNavigationSource, elementType, generatedProperties, collapsed);
                FilterBinder filterBinder = new FilterBinder(binder.Bind, binder.BindingState);
                return filterBinder.BindFilter(filterToken);
            }

            return null;
        }

        /// <summary>
        /// Bind the orderby clause <see cref="OrderByClause"/> at this level.
        /// </summary>
        /// <param name="orderByToken">The orderby token to visit.</param>
        /// <param name="resourcePathNavigationSource">The navigation source at the resource path.</param>
        /// <param name="targetNavigationSource">The target navigation source at the current level.</param>
        /// <param name="elementType">The Edm element type.</param>
        /// <param name="generatedProperties">The generated properties.</param>
        /// <param name="collapsed">The collapsed boolean value.</param>
        /// <returns>The null or the built filter clause.</returns>
        /// <returns>The null or the built orderby clause.</returns>
        private OrderByClause BindOrderby(IEnumerable<OrderByToken> orderByToken, IEdmNavigationSource resourcePathNavigationSource, IEdmNavigationSource targetNavigationSource,
            IEdmTypeReference elementType, HashSet<EndPathToken> generatedProperties, bool collapsed = false)
        {
            if (orderByToken != null && orderByToken.Any())
            {
                MetadataBinder binder = BuildNewMetadataBinder(this.Configuration, resourcePathNavigationSource, targetNavigationSource, elementType, generatedProperties, collapsed);
                OrderByBinder orderByBinder = new OrderByBinder(binder.Bind);
                return orderByBinder.BindOrderBy(binder.BindingState, orderByToken);
            }

            return null;
        }

        /// <summary>
        /// Bind the search clause <see cref="SearchClause"/> at this level.
        /// </summary>
        /// <param name="searchToken">The search token to visit.</param>
        /// <param name="resourcePathNavigationSource">The navigation source at the resource path.</param>
        /// <param name="targetNavigationSource">The target navigation source at the current level.</param>
        /// <param name="elementType">The Edm element type.</param>
        /// <returns>The null or the built search clause.</returns>
        private SearchClause BindSearch(QueryToken searchToken, IEdmNavigationSource resourcePathNavigationSource, IEdmNavigationSource targetNavigationSource, IEdmTypeReference elementType)
        {
            if (searchToken != null)
            {
                MetadataBinder binder = BuildNewMetadataBinder(this.Configuration, resourcePathNavigationSource, targetNavigationSource, elementType);
                SearchBinder searchBinder = new SearchBinder(binder.Bind);
                return searchBinder.BindSearch(searchToken);
            }

            return null;
        }

        /// <summary>
        /// Bind the select and expand clause <see cref="SelectExpandClause"/> at this level.
        /// </summary>
        /// <param name="expandToken">The expand token to visit.</param>
        /// <param name="selectToken">The select token to visit.</param>
        /// <param name="segments">The parsed segments to visit.</param>
        /// <param name="resourcePathNavigationSource">The navigation source at the resource path.</param>
        /// <param name="targetNavigationSource">The target navigation source at the current level.</param>
        /// <param name="elementType">The Edm element type.</param>
        /// <param name="generatedProperties">The generated properties.</param>
        /// <param name="collapsed">The collapsed boolean value.</param>
        /// <returns>The null or the built select and expand clause.</returns>
        private SelectExpandClause BindSelectExpand(ExpandToken expandToken, SelectToken selectToken,
            IList<ODataPathSegment> segments, IEdmNavigationSource resourcePathNavigationSource, IEdmNavigationSource targetNavigationSource, IEdmTypeReference elementType,
            HashSet<EndPathToken> generatedProperties = null, bool collapsed = false)
        {
            if (expandToken != null || selectToken != null)
            {
                BindingState binding = CreateBindingState(this.Configuration, resourcePathNavigationSource, targetNavigationSource, elementType, generatedProperties, collapsed);

                SelectExpandBinder selectExpandBinder = new SelectExpandBinder(this.Configuration,
                new ODataPathInfo(new ODataPath(segments)), binding);

                return selectExpandBinder.Bind(expandToken, selectToken);
            }
            else
            {
                // It's better to return null for both Expand and Select are null.
                // However, in order to be consistent, we returns the empty SelectExpandClause with AllSelected = true.
                return new SelectExpandClause(new Collection<SelectItem>(), true);
            }
        }

        /// <summary>
        /// Process a <see cref="SelectTermToken"/> to identify whether it's a Wildcard path.
        /// </summary>
        /// <param name="selectToken">the select token to process.</param>
        /// <param name="newSelectItem">the built select item to out.</param>
        /// <returns>A boolean value indicates the result of processing wildcard token path.</returns>
        private bool ProcessWildcardTokenPath(SelectTermToken selectToken, out SelectItem newSelectItem)
        {
            newSelectItem = null;
            if (selectToken == null || selectToken.PathToProperty == null)
            {
                return false;
            }

            PathSegmentToken pathToken = selectToken.PathToProperty;
            if (SelectPathSegmentTokenBinder.TryBindAsWildcard(pathToken, this.Model, out newSelectItem))
            {
                // * or Namespace.*
                if (pathToken.NextToken != null)
                {
                    throw new ODataException(ODataErrorStrings.SelectExpandBinder_InvalidIdentifierAfterWildcard(pathToken.NextToken.Identifier));
                }

                VerifyNoQueryOptionsNested(selectToken, pathToken.Identifier);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Process a <see cref="PathSegmentToken"/> following any type segments if necessary.
        /// </summary>
        /// <param name="tokenIn">the path token to process.</param>
        /// <returns>The processed OData segments.</returns>
        private List<ODataPathSegment> ProcessSelectTokenPath(PathSegmentToken tokenIn)
        {
            Debug.Assert(tokenIn != null, "tokenIn != null");

            List<ODataPathSegment> pathSoFar = new List<ODataPathSegment>();
            IEdmStructuredType currentLevelType = this.edmType;

            // first, walk through all type segments in a row, converting them from tokens into segments.
            if (tokenIn.IsNamespaceOrContainerQualified() && !UriParserHelper.IsAnnotation(tokenIn.Identifier))
            {
                PathSegmentToken firstNonTypeToken;
                pathSoFar.AddRange(SelectExpandPathBinder.FollowTypeSegments(tokenIn, this.Model, this.Settings.SelectExpandLimit, this.configuration.Resolver, ref currentLevelType, out firstNonTypeToken));
                Debug.Assert(firstNonTypeToken != null, "Did not get last token.");
                tokenIn = firstNonTypeToken as NonSystemToken;
                if (tokenIn == null)
                {
                    throw new ODataException(ODataErrorStrings.SelectExpandBinder_SystemTokenInSelect(firstNonTypeToken.Identifier));
                }
            }

            // next, create a segment for the first non-type segment in the path.
            ODataPathSegment lastSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(tokenIn, this.Model, currentLevelType, this.configuration.Resolver, this.state);

            // next, create an ODataPath and add the segments to it.
            if (lastSegment != null)
            {
                pathSoFar.Add(lastSegment);

                // try create a complex type property path.
                while (true)
                {
                    // no need to go on if the current property is not of complex type or collection of complex type,
                    // unless the segment is a primitive type cast or a property on an open complex property.
                    currentLevelType = lastSegment.EdmType as IEdmStructuredType;
                    IEdmCollectionType collectionType = lastSegment.EdmType as IEdmCollectionType;
                    IEdmPrimitiveType primitiveType = lastSegment.EdmType as IEdmPrimitiveType;
                    DynamicPathSegment dynamicPath = lastSegment as DynamicPathSegment;
                    if ((currentLevelType == null || currentLevelType.TypeKind != EdmTypeKind.Complex)
                        && (collectionType == null || collectionType.ElementType.TypeKind() != EdmTypeKind.Complex)
                        && (primitiveType == null || primitiveType.TypeKind != EdmTypeKind.Primitive)
                        && (dynamicPath == null || tokenIn.NextToken == null))
                    {
                        break;
                    }

                    NonSystemToken nextToken = tokenIn.NextToken as NonSystemToken;
                    if (nextToken == null)
                    {
                        break;
                    }

                    if (UriParserHelper.IsAnnotation(nextToken.Identifier))
                    {
                        lastSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(nextToken, this.Model,
                            currentLevelType, this.configuration.Resolver, null);
                    }
                    else if (primitiveType == null && dynamicPath == null)
                    {
                        // This means last segment a collection of complex type,
                        // current segment can only be type cast and cannot be property name.
                        if (currentLevelType == null)
                        {
                            currentLevelType = collectionType.ElementType.Definition as IEdmStructuredType;
                        }

                        // If there is no collection type in the path yet, will try to bind property for the next token
                        // first try bind the segment as property.
                        lastSegment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(nextToken, this.Model,
                            currentLevelType, this.configuration.Resolver, null);
                    }
                    else
                    {
                        // determine whether we are looking at a type cast or a dynamic path segment.
                        EdmPrimitiveTypeKind nextTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(nextToken.Identifier);
                        IEdmPrimitiveType castType = EdmCoreModel.Instance.GetPrimitiveType(nextTypeKind);
                        if (castType != null)
                        {
                            lastSegment = new TypeSegment(castType, castType, null);
                        }
                        else if (dynamicPath != null)
                        {
                            lastSegment = new DynamicPathSegment(nextToken.Identifier);
                        }
                        else
                        {
                            throw new ODataException(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
                        }
                    }

                    // then try bind the segment as type cast.
                    if (lastSegment == null)
                    {
                        IEdmStructuredType typeFromNextToken =
                            UriEdmHelpers.FindTypeFromModel(this.Model, nextToken.Identifier, this.configuration.Resolver) as
                                IEdmStructuredType;

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

            // non-navigation cases do not allow further segments in $select.
            if (tokenIn.NextToken != null)
            {
                throw new ODataException(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
            }

            // Later, we can consider to create a "DynamicOperationSegment" to handle this.
            // But now, Let's throw exception.
            if (lastSegment == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_InvalidIdentifierInQueryOption(tokenIn.Identifier));
            }

            // navigation property is not allowed to append sub path in the selection.
            NavigationPropertySegment navPropSegment = pathSoFar.LastOrDefault() as NavigationPropertySegment;
            if (navPropSegment != null && tokenIn.NextToken != null)
            {
                throw new ODataException(ODataErrorStrings.SelectBinder_MultiLevelPathInSelect);
            }

            return pathSoFar;
        }

        private static HashSet<EndPathToken> GetGeneratedProperties(ComputeClause computeOption, ApplyClause applyOption)
        {
            HashSet<EndPathToken> generatedProperties = null;

            if (applyOption != null)
            {
                generatedProperties = applyOption.GetLastAggregatedPropertyNames();
            }

            if (computeOption != null)
            {
                var computedProperties = new HashSet<EndPathToken>(computeOption.ComputedItems.Select(i => new EndPathToken(i.Alias, null)));
                if (generatedProperties == null)
                {
                    generatedProperties = computedProperties;
                }
                else
                {
                    generatedProperties.UnionWith(computedProperties);
                }
            }

            return generatedProperties;
        }

        private IEdmNavigationProperty ParseComplexTypesBeforeNavigation(IEdmStructuralProperty edmProperty, ref PathSegmentToken currentToken, List<ODataPathSegment> pathSoFar)
        {
            pathSoFar.Add(new PropertySegment(edmProperty));

            if (currentToken.NextToken == null)
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty(currentToken.Identifier, edmProperty.DeclaringType.FullTypeName()));
            }

            currentToken = currentToken.NextToken;

            IEdmType complexType = edmProperty.Type.Definition;

            IEdmCollectionType collectionType = complexType as IEdmCollectionType;
            if (collectionType != null)
            {
                complexType = collectionType.ElementType.Definition;
            }

            IEdmStructuredType currentType = complexType as IEdmStructuredType;
            if (currentType == null)
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_InvaidSegmentInExpand(currentToken.Identifier));
            }

            if (currentToken.IsNamespaceOrContainerQualified())
            {
                pathSoFar.AddRange(SelectExpandPathBinder.FollowTypeSegments(currentToken, this.Model, this.Settings.SelectExpandLimit, this.configuration.Resolver, ref currentType, out currentToken));
            }

            IEdmProperty property = this.configuration.Resolver.ResolveProperty(currentType, currentToken.Identifier);
            if (edmProperty == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(currentType.FullTypeName(), currentToken.Identifier));
            }

            IEdmStructuralProperty complexProp = property as IEdmStructuralProperty;
            if (complexProp != null)
            {
                property = ParseComplexTypesBeforeNavigation(complexProp, ref currentToken, pathSoFar);
            }

            IEdmNavigationProperty navProp = property as IEdmNavigationProperty;
            if (navProp != null)
            {
                return navProp;
            }
            else
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_PropertyIsNotANavigationPropertyOrComplexProperty(currentToken.Identifier, currentType.FullTypeName()));
            }
        }

        /// <summary>
        /// Parse from levelsOption token to LevelsClause.
        /// Negative value would be treated as max.
        /// </summary>
        /// <param name="levelsOption">The levelsOption for current expand.</param>
        /// <param name="sourceType">The type of current level navigation source.</param>
        /// <param name="property">Navigation property for current expand.</param>
        /// <returns>The LevelsClause parsed, null if levelsOption is null</returns>
        private static LevelsClause ParseLevels(long? levelsOption, IEdmType sourceType, IEdmNavigationProperty property)
        {
            if (!levelsOption.HasValue)
            {
                return null;
            }

            IEdmType relatedType = property.ToEntityType();

            if (sourceType != null && relatedType != null && !UriEdmHelpers.IsRelatedTo(sourceType, relatedType))
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_LevelsNotAllowedOnIncompatibleRelatedType(property.Name, relatedType.FullTypeName(), sourceType.FullTypeName()));
            }

            return new LevelsClause(levelsOption.Value < 0, levelsOption.Value);
        }

        private static MetadataBinder BuildNewMetadataBinder(ODataUriParserConfiguration config,
            IEdmNavigationSource resourcePathNavigationSource,
            IEdmNavigationSource targetNavigationSource,
            IEdmTypeReference elementType,
            HashSet<EndPathToken> generatedProperties = null,
            bool collapsed = false)
        {
            BindingState state = CreateBindingState(config, resourcePathNavigationSource, targetNavigationSource, elementType, generatedProperties, collapsed);
            return new MetadataBinder(state);
        }

        private static BindingState CreateBindingState(ODataUriParserConfiguration config,
            IEdmNavigationSource resourcePathNavigationSource, IEdmNavigationSource targetNavigationSource,
            IEdmTypeReference elementType, HashSet<EndPathToken> generatedProperties = null,
            bool collapsed = false)
        {
            if (targetNavigationSource == null && elementType == null)
            {
                return null;
            }

            // For example if we have https://url/Books?$expand=Authors($filter=Name eq $it/Name)
            // $filter=Name will reference Authors(the expanded entity).
            // $it/Name will reference Books(the resource identified by the path).
            // The BindingState ImplicitRangeVariable property will store the $it that references the expanded/selected item (The Implicit Range Variable).
            // We add to the Stack, the $it that references the resource identified by the path (The Explicit Range Variable).

            BindingState state = new BindingState(config)
            {
                ImplicitRangeVariable =
                    NodeFactory.CreateImplicitRangeVariable(elementType != null ? elementType :
                        targetNavigationSource.EntityType().ToTypeReference(), targetNavigationSource)
            };

            state.AggregatedPropertyNames = generatedProperties;
            state.IsCollapsed = collapsed;
            state.ResourcePathNavigationSource = resourcePathNavigationSource;

            if (resourcePathNavigationSource != null)
            {
                // This $it rangeVariable will be added the Stack.
                // We are adding a rangeVariable whose navigationSource is the resource path entity set.
                // ODATA spec: Example 106 http://docs.oasis-open.org/odata/odata/v4.01/csprd05/part2-url-conventions/odata-v4.01-csprd05-part2-url-conventions.html#sec_it
                RangeVariable explicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(
                    resourcePathNavigationSource.EntityType().ToTypeReference(), resourcePathNavigationSource);
                state.RangeVariables.Push(explicitRangeVariable);
            }

            // Create $this rangeVariable and add it to the Stack.
            RangeVariable dollarThisRangeVariable = NodeFactory.CreateDollarThisRangeVariable(
                elementType != null ? elementType : targetNavigationSource.EntityType().ToTypeReference(), targetNavigationSource);
            state.RangeVariables.Push(dollarThisRangeVariable);

            return state;
        }

        private static void VerifySelectedPath(SelectTermToken selectedToken)
        {
            PathSegmentToken current = selectedToken.PathToProperty;
            while (current != null)
            {
                if (current is SystemToken)
                {
                    // It's not allowed to set a system token in a select clause.
                    throw new ODataException(ODataErrorStrings.SelectExpandBinder_SystemTokenInSelect(current.Identifier));
                }

                current = current.NextToken;
            }
        }

        private static bool VerifySelectedNavigationProperty(IList<ODataPathSegment> selectedPath, SelectTermToken tokenIn)
        {
            NavigationPropertySegment navPropSegment = selectedPath.LastOrDefault() as NavigationPropertySegment;
            if (navPropSegment != null)
            {
                // After navigation property, it's not allowed to nest query options
                VerifyNoQueryOptionsNested(tokenIn, navPropSegment.Identifier);

                return true;
            }

            return false;
        }

        private static void VerifyNoQueryOptionsNested(SelectTermToken selectToken, string identifier)
        {
            if (selectToken != null)
            {
                if (selectToken.ComputeOption != null ||
                    selectToken.FilterOption != null ||
                    selectToken.OrderByOptions != null ||
                    selectToken.SearchOption != null ||
                    selectToken.CountQueryOption != null ||
                    selectToken.SelectOption != null ||
                    selectToken.TopOption != null ||
                    selectToken.SkipOption != null)
                {
                    throw new ODataException(ODataErrorStrings.SelectExpandBinder_InvalidQueryOptionNestedSelection(identifier));
                }
            }
        }

        private static void AddToSelectedItems(SelectItem itemToAdd, List<SelectItem> selectItems)
        {
            if (itemToAdd == null)
            {
                return;
            }

            // ignore all property selection if there's a wildcard select item.
            WildcardSelectItem wildcardSelectItem = selectItems.OfType<WildcardSelectItem>().FirstOrDefault();
            if (wildcardSelectItem != null && IsStructuralOrNavigationPropertySelectionItem(itemToAdd))
            {
                wildcardSelectItem.AddSubsumed(itemToAdd);
                return;
            }

            // if the selected item is a nav prop, then see if its already there before we add it.
            PathSelectItem pathSelectItem = itemToAdd as PathSelectItem;
            if (pathSelectItem != null)
            {
                NavigationPropertySegment trailingNavPropSegment = pathSelectItem.SelectedPath.LastSegment as NavigationPropertySegment;
                if (trailingNavPropSegment != null)
                {
                    if (selectItems.OfType<PathSelectItem>().Any(i => i.SelectedPath.Equals(pathSelectItem.SelectedPath)))
                    {
                        return;
                    }
                }
            }

            // if the selected item is "*", filter the existing property selection.
            wildcardSelectItem = itemToAdd as WildcardSelectItem;
            if (wildcardSelectItem != null)
            {
                List<SelectItem> shouldFilter = selectItems.Where(s => IsStructuralSelectionItem(s)).ToList();
                wildcardSelectItem.AddSubsumed(shouldFilter);
                foreach (var filterItem in shouldFilter)
                {
                    selectItems.Remove(filterItem);
                }
            }

            selectItems.Add(itemToAdd);
        }

        private static bool IsStructuralOrNavigationPropertySelectionItem(SelectItem selectItem)
        {
            PathSelectItem pathSelectItem = selectItem as PathSelectItem;
            return pathSelectItem != null &&
                (pathSelectItem.SelectedPath.LastSegment is NavigationPropertySegment ||
                pathSelectItem.SelectedPath.LastSegment is PropertySegment);
        }

        private static bool IsStructuralSelectionItem(SelectItem selectItem)
        {
            PathSelectItem pathSelectItem = selectItem as PathSelectItem;
            return pathSelectItem != null && (pathSelectItem.SelectedPath.LastSegment is PropertySegment);
        }
    }
}