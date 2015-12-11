﻿//---------------------------------------------------------------------
// <copyright file="SelectExpandBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        /// The entity type at the current level expand.
        /// </summary>
        private readonly IEdmStructuredType edmType;

        /// <summary>
        /// Build the ExpandOption variant of an SelectExpandBinder
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="edmType">The type of the top level expand item.</param>
        /// <param name="navigationSource">The navigation source of the top level expand item.</param>
        public SelectExpandBinder(ODataUriParserConfiguration configuration, IEdmStructuredType edmType, IEdmNavigationSource navigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(edmType, "edmType");

            this.configuration = configuration;
            this.edmType = edmType;
            this.navigationSource = navigationSource;
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
        /// Bind the top level expand.
        /// </summary>
        /// <param name="tokenIn">the token to visit</param>
        /// <returns>a SelectExpand clause based on this ExpandToken</returns>
        public SelectExpandClause Bind(ExpandToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");

            // the top level represents $it then has only select populated..
            List<SelectItem> expandedTerms = new List<SelectItem>();
            if (tokenIn.ExpandTerms.Single().ExpandOption != null)
            {
                expandedTerms.AddRange(tokenIn.ExpandTerms.Single().ExpandOption.ExpandTerms.Select(this.GenerateExpandItem).Where(expandedNavigationSelectItem => expandedNavigationSelectItem != null));
            }

            // if there are any select items at this level then allSelected is false, otherwise it's true.
            bool isAllSelected = tokenIn.ExpandTerms.Single().SelectOption == null;
            SelectExpandClause topLevelExpand = new SelectExpandClause(expandedTerms, isAllSelected);
            if (!isAllSelected)
            {
                SelectBinder selectBinder = new SelectBinder(this.Model, this.EdmType, this.Configuration.Settings.SelectExpandLimit, topLevelExpand, this.configuration.Resolver);
                selectBinder.Bind(tokenIn.ExpandTerms.Single().SelectOption);
            }

            return topLevelExpand;
        }

        /// <summary>
        /// Bind a sub level expand
        /// </summary>
        /// <param name="tokenIn">the token to visit</param>
        /// <returns>a SelectExpand clause based on this ExpandToken</returns>
        private SelectExpandClause BindSubLevel(ExpandToken tokenIn)
        {
            List<SelectItem> expandTerms = tokenIn.ExpandTerms.Select(this.GenerateExpandItem).Where(expandedNavigationSelectItem => expandedNavigationSelectItem != null).ToList();

            return new SelectExpandClause(expandTerms, true);
        }

        /// <summary>
        /// Generate a SubExpand based on the current nav property and the curren token
        /// </summary>
        /// <param name="currentNavProp">the current navigation property</param>
        /// <param name="tokenIn">the current token</param>
        /// <returns>a new SelectExpand clause bound to the current token and nav prop</returns>
        private SelectExpandClause GenerateSubExpand(IEdmNavigationProperty currentNavProp, ExpandTermToken tokenIn)
        {
            SelectExpandBinder nextLevelBinder = new SelectExpandBinder(this.Configuration, currentNavProp.ToEntityType(), this.NavigationSource != null ? this.NavigationSource.FindNavigationTarget(currentNavProp) : null);
            return nextLevelBinder.BindSubLevel(tokenIn.ExpandOption);
        }

        /// <summary>
        /// Decorate an expand tree using a select token.
        /// </summary>
        /// <param name="subExpand">the already built sub expand</param>
        /// <param name="currentNavProp">the current navigation property</param>
        /// <param name="select">the select token to use</param>
        /// <returns>A new SelectExpand clause decorated with the select token.</returns>
        private SelectExpandClause DecorateExpandWithSelect(SelectExpandClause subExpand, IEdmNavigationProperty currentNavProp, SelectToken select)
        {
            SelectBinder selectBinder = new SelectBinder(this.Model, currentNavProp.ToEntityType(), this.Settings.SelectExpandLimit, subExpand, this.configuration.Resolver);
            return selectBinder.Bind(select);
        }

        /// <summary>
        /// Build a expand clause for a nested expand.
        /// </summary>
        /// <returns>A new SelectExpandClause.</returns>
        private SelectExpandClause BuildDefaultSubExpand()
        {
            return new SelectExpandClause(new Collection<SelectItem>(), true);
        }

        /// <summary>
        /// Generate an expand item (and a select item for the implicit nav prop if necessary) based on an ExpandTermToken
        /// </summary>
        /// <param name="tokenIn">the expandTerm token to visit</param>
        /// <returns>the expand item for this expand term token.</returns>
        private SelectItem GenerateExpandItem(ExpandTermToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");

            // ensure that we're always dealing with proper V4 syntax
            if (tokenIn.PathToNavProp.NextToken != null && !tokenIn.PathToNavProp.IsNamespaceOrContainerQualified())
            {
                if (tokenIn.PathToNavProp.NextToken.Identifier != UriQueryConstants.RefSegment || tokenIn.PathToNavProp.NextToken.NextToken != null)
                {
                    throw new ODataException(ODataErrorStrings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
                }
            }

            PathSegmentToken currentToken = tokenIn.PathToNavProp;

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
            if (currentNavProp == null)
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_PropertyIsNotANavigationProperty(currentToken.Identifier, currentLevelEntityType.FullTypeName()));
            }

            bool isRef = false;
            if (firstNonTypeToken.NextToken != null)
            {
                // lastly... make sure that, since we're on a NavProp, that the next token isn't null.
                if (firstNonTypeToken.NextToken.Identifier == UriQueryConstants.RefSegment)
                {
                    isRef = true;
                }
                else
                {
                    throw new ODataException(ODataErrorStrings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
                }
            }

            pathSoFar.Add(new NavigationPropertySegment(currentNavProp, /*entitySet*/null));
            ODataExpandPath pathToNavProp = new ODataExpandPath(pathSoFar);

            IEdmNavigationSource targetNavigationSource = null;
            if (this.NavigationSource != null)
            {
                targetNavigationSource = this.NavigationSource.FindNavigationTarget(currentNavProp);
            }

            // call MetadataBinder to build the filter clause
            FilterClause filterOption = null;
            if (tokenIn.FilterOption != null)
            {
                MetadataBinder binder = this.BuildNewMetadataBinder(targetNavigationSource);
                FilterBinder filterBinder = new FilterBinder(binder.Bind, binder.BindingState);
                filterOption = filterBinder.BindFilter(tokenIn.FilterOption);
            }

            // call MetadataBinder again to build the orderby clause
            OrderByClause orderbyOption = null;
            if (tokenIn.OrderByOptions != null)
            {
                MetadataBinder binder = this.BuildNewMetadataBinder(targetNavigationSource);
                OrderByBinder orderByBinder = new OrderByBinder(binder.Bind);
                orderbyOption = orderByBinder.BindOrderBy(binder.BindingState, tokenIn.OrderByOptions);
            }

            SearchClause searchOption = null;
            if (tokenIn.SearchOption != null)
            {
                MetadataBinder binder = this.BuildNewMetadataBinder(targetNavigationSource);
                SearchBinder searchBinder = new SearchBinder(binder.Bind);
                searchOption = searchBinder.BindSearch(tokenIn.SearchOption);
            }

            if (isRef)
            {
                return new ExpandedReferenceSelectItem(pathToNavProp, targetNavigationSource, filterOption, orderbyOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.CountQueryOption, searchOption);
            }

            SelectExpandClause subSelectExpand;
            if (tokenIn.ExpandOption != null)
            {
                subSelectExpand = this.GenerateSubExpand(currentNavProp, tokenIn);
            }
            else
            {
                subSelectExpand = BuildDefaultSubExpand();
            }

            subSelectExpand = this.DecorateExpandWithSelect(subSelectExpand, currentNavProp, tokenIn.SelectOption);

            LevelsClause levelsOption = this.ParseLevels(tokenIn.LevelsOption, currentLevelEntityType, currentNavProp);
            return new ExpandedNavigationSelectItem(pathToNavProp, targetNavigationSource, subSelectExpand, filterOption, orderbyOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.CountQueryOption, searchOption, levelsOption);
        }

        /// <summary>
        /// Parse from levelsOption token to LevelsClause.
        /// Negative value would be treated as max.
        /// </summary>
        /// <param name="levelsOption">The levelsOption for current expand.</param>
        /// <param name="sourceType">The type of current level navigation source.</param>
        /// <param name="property">Navigation property for current expand.</param>
        /// <returns>The LevelsClause parsed, null if levelsOption is null</returns>
        private LevelsClause ParseLevels(long? levelsOption, IEdmType sourceType, IEdmNavigationProperty property)
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

        /// <summary>
        /// Build a new MetadataBinder to use for expand options.
        /// </summary>
        /// <param name="targetNavigationSource">The navigation source being expanded.</param>
        /// <returns>A new MetadataBinder ready to bind a Filter or Orderby clause.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        private MetadataBinder BuildNewMetadataBinder(IEdmNavigationSource targetNavigationSource)
        {
            BindingState state = new BindingState(this.configuration)
            {
                ImplicitRangeVariable =
                    NodeFactory.CreateImplicitRangeVariable(targetNavigationSource.EntityType().ToTypeReference(), targetNavigationSource)
            };
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            return new MetadataBinder(state);
        }
    }
}