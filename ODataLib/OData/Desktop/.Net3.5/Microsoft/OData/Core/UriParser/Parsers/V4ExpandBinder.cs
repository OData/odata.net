//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Metadata;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// ExpandOption variant of an ExpandBinder, where the default selection item for a given level is based on the select at that level
    /// instead of the top level select clause. If nothing is selected for a given expand in the ExpandOption syntax, then we by default
    /// select all from that item, instead of selecting nothing (and therefore pruning the expand off of the tree).
    /// </summary>
    //// TODO 1466134 Rename this to SelectExpandBinder when we're only using V4
    internal sealed class V4ExpandBinder : ExpandBinder
    {
        /// <summary>
        /// Build the ExpandOption variant of an ExpandBinder
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="edmType">The type of the top level expand item.</param>
        /// <param name="entitySet">The entity set of the top level expand item.</param>
        public V4ExpandBinder(ODataUriParserConfiguration configuration, IEdmStructuredType edmType, IEdmEntitySet entitySet)
            : base(configuration, edmType, entitySet)
        { 
        }

        /// <summary>
        /// Visit an ExpandToken
        /// </summary>
        /// <param name="tokenIn">the token to visit</param>
        /// <returns>a SelectExpand clause based on this ExpandToken</returns>
        public override SelectExpandClause Bind(ExpandToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");
            if (tokenIn.ExpandTerms.Any() && 
                tokenIn.ExpandTerms.All(x => x.PathToNavProp.Identifier == ExpressionConstants.It))
            {
                // if the path to nav prop is $it then this is our special top level case
                // that has only select populated..
                List<SelectItem> expandedTerms = new List<SelectItem>();
                if (tokenIn.ExpandTerms.Single().ExpandOption != null)
                {
                    expandedTerms.AddRange(tokenIn.ExpandTerms.Single().ExpandOption.ExpandTerms.Select(this.GenerateExpandItem).Where(expandedNavigationSelectItem => expandedNavigationSelectItem != null).Cast<SelectItem>());
                }
                
                // for each expanded nav prop select item we've generated we also need to add a path selection item pointing to its nav prop 
                // to explicitly call out that every nav prop is selected.
                IEnumerable<SelectItem> selectedItemsForThisLevel = this.AddNavPropLinksForThisLevel(expandedTerms);

                // if there are any select items at this level then allSelected is false, otherwise it's true.
                bool isAllSelected = tokenIn.ExpandTerms.Single().SelectOption == null;
                SelectExpandClause topLevelExpand = new SelectExpandClause(selectedItemsForThisLevel, isAllSelected);
                if (!isAllSelected)
                {
                    ISelectBinder selectBinder = SelectBinderFactory.Create(this.Model, this.EdmType, this.Configuration.Settings, topLevelExpand);
                    selectBinder.Bind(tokenIn.ExpandTerms.Single().SelectOption);
                }

                return topLevelExpand;
            }

            List<SelectItem> expandTerms = tokenIn.ExpandTerms.Select(this.GenerateExpandItem).Where(expandedNavigationSelectItem => expandedNavigationSelectItem != null).Cast<SelectItem>().ToList();

            IEnumerable<SelectItem> selectItemsForThisLevel = this.AddNavPropLinksForThisLevel(expandTerms);

            return new SelectExpandClause(selectItemsForThisLevel, true);
        }

        /// <summary>
        /// Generate a SubExpand based on the current nav property and the curren token
        /// </summary>
        /// <param name="currentNavProp">the current navigation property</param>
        /// <param name="tokenIn">the current token</param>
        /// <returns>a new SelectExpand clause bound to the current token and nav prop</returns>
        protected override SelectExpandClause GenerateSubExpand(IEdmNavigationProperty currentNavProp, ExpandTermToken tokenIn)
        {
            V4ExpandBinder nextLevelBinder = new V4ExpandBinder(this.Configuration, currentNavProp.ToEntityType(), this.EntitySet != null ? this.EntitySet.FindNavigationTarget(currentNavProp) : null);
            return nextLevelBinder.Bind(tokenIn.ExpandOption);
        }

        /// <summary>
        /// Decorate an expand tree using a select token.
        /// </summary>
        /// <param name="subExpand">the already built sub expand</param>
        /// <param name="currentNavProp">the current navigation property</param>
        /// <param name="select">the select token to use</param>
        /// <returns>A new SelectExpand clause decorated with the select token.</returns>
        protected override SelectExpandClause DecorateExpandWithSelect(SelectExpandClause subExpand, IEdmNavigationProperty currentNavProp, SelectToken select)
        {
            ISelectBinder selectBinder = SelectBinderFactory.Create(this.Model, currentNavProp.ToEntityType(), this.Settings, subExpand);
            return selectBinder.Bind(select);
        }

        /// <summary>
        /// Build a expand clause for a nested expand.
        /// </summary>
        /// <returns>A new SelectExpandClause.</returns>
        protected override SelectExpandClause BuildDefaultSubExpand()
        {
            return new SelectExpandClause(new Collection<SelectItem>(), true);
        }

        /// <summary>
        /// Generate an expand item based on an ExpandTermToken
        /// </summary>
        /// <param name="tokenIn">the expandTerm token to visit</param>
        /// <returns>the expand item for this expand term token.</returns>
        protected override ExpandedNavigationSelectItem GenerateExpandItem(ExpandTermToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");

            // ensure that we're always dealing with proper V4 syntax
            if (tokenIn.PathToNavProp.NextToken != null && !tokenIn.PathToNavProp.IsNamespaceOrContainerQualified())
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_TraversingMultipleNavPropsInTheSamePath);
            }

            PathSegmentToken currentToken = tokenIn.PathToNavProp;

            IEdmStructuredType currentLevelEntityType = this.EdmType;
            List<ODataPathSegment> pathSoFar = new List<ODataPathSegment>();
            PathSegmentToken firstNonTypeToken = currentToken;

            if (currentToken.IsNamespaceOrContainerQualified())
            {
                pathSoFar.AddRange(SelectExpandPathBinder.FollowTypeSegments(currentToken, this.Model, this.Settings.SelectExpandLimit, ref currentLevelEntityType, out firstNonTypeToken));
            }

            IEdmProperty edmProperty = currentLevelEntityType.FindProperty(firstNonTypeToken.Identifier);
            if (edmProperty == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(currentLevelEntityType.ODataFullName(), currentToken.Identifier));
            }

            IEdmNavigationProperty currentNavProp = edmProperty as IEdmNavigationProperty;
            if (currentNavProp == null)
            {
                // the server allowed non-navigation, non-stream properties to be expanded, but then ignored them.
                if (this.Settings.UseWcfDataServicesServerBehavior && !edmProperty.Type.IsStream())
                {
                    return null;
                }

                throw new ODataException(ODataErrorStrings.ExpandItemBinder_PropertyIsNotANavigationProperty(currentToken.Identifier, currentLevelEntityType.ODataFullName()));
            }

            pathSoFar.Add(new NavigationPropertySegment(currentNavProp, /*entitySet*/null));
            ODataExpandPath pathToNavProp = new ODataExpandPath(pathSoFar);

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

            IEdmEntitySet targetEntitySet = null;
            if (this.EntitySet != null)
            {
                targetEntitySet = this.EntitySet.FindNavigationTarget(currentNavProp);
            }

            // call MetadataBinder to build the filter clause
            FilterClause filterOption = null;
            if (tokenIn.FilterOption != null)
            {
                MetadataBinder binder = this.BuildNewMetadataBinder(targetEntitySet);
                FilterBinder filterBinder = new FilterBinder(binder.Bind, binder.BindingState);
                filterOption = filterBinder.BindFilter(tokenIn.FilterOption);
            }

            // call MetadataBinder again to build the orderby clause
            OrderByClause orderbyOption = null;
            if (tokenIn.OrderByOption != null)
            {
                MetadataBinder binder = this.BuildNewMetadataBinder(targetEntitySet);
                OrderByBinder orderByBinder = new OrderByBinder(binder.Bind);
                orderbyOption = orderByBinder.BindOrderBy(binder.BindingState, new OrderByToken[] { tokenIn.OrderByOption });
            }

            return new ExpandedNavigationSelectItem(pathToNavProp, targetEntitySet, filterOption, orderbyOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.CountQueryOption, subSelectExpand);
        }

        /// <summary>
        /// Generate the NavPropSelectionItem for an ExpandTerm
        /// </summary>
        /// <param name="expandItem">the expand term to reference</param>
        /// <returns>A new PathSelectionItem referencing the same nav prop as the expandItem</returns>
        private PathSelectItem GenerateNavPropSelectionItemForExpandTerm(SelectItem expandItem)
        {
            ExpandedNavigationSelectItem expandedNavigationSelectItem = expandItem as ExpandedNavigationSelectItem;
            return expandedNavigationSelectItem != null ? new PathSelectItem(expandedNavigationSelectItem.PathToNavigationProperty.ToSelectPath()) : null;
        }

        /// <summary>
        /// Add any nav prop links for the expands at this level.
        /// </summary>
        /// <param name="expandedTerms">the existing list of expanded terms.</param>
        /// <returns>A new list of selectItems containing the nav prop links for each expanded term.</returns>
        private IEnumerable<SelectItem> AddNavPropLinksForThisLevel(List<SelectItem> expandedTerms)
        {
            List<SelectItem> selectedItemsForThisLevel = new List<SelectItem>();
            selectedItemsForThisLevel.AddRange(expandedTerms);
            selectedItemsForThisLevel.AddRange(expandedTerms.Cast<ExpandedNavigationSelectItem>().Select(this.GenerateNavPropSelectionItemForExpandTerm).Where(pathSelectItem => pathSelectItem != null).Cast<SelectItem>());
            return selectedItemsForThisLevel;
        }
    }
}
