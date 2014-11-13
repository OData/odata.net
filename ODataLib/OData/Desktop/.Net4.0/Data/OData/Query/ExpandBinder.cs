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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Build a semantic tree for Expand based on an Expand syntactic tree.
    /// </summary>
    internal abstract class ExpandBinder
    {
        /// <summary>
        /// The configuration used for binding.
        /// </summary>
        private readonly ODataUriParserConfiguration configuration;

        /// <summary>
        /// The entity set at the current level expand.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The entity type at the current level expand.
        /// </summary>
        private readonly IEdmEntityType entityType;

        /// <summary>
        /// Constructs a new ExpandBinder.
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="entityType">The entity type of the top level expand item.</param>
        /// <param name="entitySet">The entity set of the top level expand item.</param>
        protected ExpandBinder(ODataUriParserConfiguration configuration, IEdmEntityType entityType, IEdmEntitySet entitySet)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            ExceptionUtils.CheckArgumentNotNull(entityType, "topLevelEntityType");

            this.configuration = configuration;
            this.entityType = entityType;
            this.entitySet = entitySet;
        }

        /// <summary>
        /// The model used for binding.
        /// </summary>
        public IEdmModel Model
        {
            get { return this.configuration.Model; }
        }

        /// <summary>
        /// The top level entity type.
        /// </summary>
        public IEdmEntityType EntityType
        {
            get { return this.entityType; }
        }
        
        /// <summary>
        /// The top level entity set for this level.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// The settings to use when binding.
        /// </summary>
        protected ODataUriParserSettings Settings
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.configuration.Settings;
            }
        }

        /// <summary>
        /// The configuration used for binding.
        /// </summary>
        protected ODataUriParserConfiguration Configuration
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.configuration;
            }
        }

        /// <summary>
        /// Visit an ExpandToken
        /// </summary>
        /// <param name="tokenIn">the token to visit</param>
        /// <returns>a SelectExpand clause based on this ExpandToken</returns>
        public SelectExpandClause Bind(ExpandToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");
            List<ExpandedNavigationSelectItem> expandTerms = tokenIn.ExpandTerms.Select(this.GenerateExpandItem).Where(i => i != null).ToList();
            return new SelectExpandClause(UnknownSelection.Instance, new Expansion(expandTerms));
        }

        /// <summary>
        /// Generate a SubExpand based on the current nav property and the curren token
        /// </summary>
        /// <param name="currentNavProp">the current navigation property</param>
        /// <param name="tokenIn">the current token</param>
        /// <returns>a new SelectExpand clause bound to the current token and nav prop</returns>
        protected abstract SelectExpandClause GenerateSubExpand(IEdmNavigationProperty currentNavProp, ExpandTermToken tokenIn);

        /// <summary>
        /// Decorate an expand tree using a select token.
        /// </summary>
        /// <param name="subExpand">the already built sub expand</param>
        /// <param name="currentNavProp">the current navigation property</param>
        /// <param name="select">the select token to use</param>
        /// <returns>A new SelectExpand clause decorated with the select token.</returns>
        protected abstract SelectExpandClause DecorateExpandWithSelect(SelectExpandClause subExpand, IEdmNavigationProperty currentNavProp, SelectToken select);

        /// <summary>
        /// Build a expand clause for a nested expand.
        /// </summary>
        /// <returns>A new SelectExpandClause.</returns>
        private static SelectExpandClause BuildDefaultSubExpand()
        {
            return new SelectExpandClause(UnknownSelection.Instance, new Expansion(new List<ExpandedNavigationSelectItem>()));
        }

        /// <summary>
        /// Generate an expand item based on an ExpandTermToken
        /// </summary>
        /// <param name="tokenIn">the expandTerm token to visit</param>
        /// <returns>the expand item for this expand term token.</returns>
        private ExpandedNavigationSelectItem GenerateExpandItem(ExpandTermToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");

            // ensure that we're always dealing with a normalized tree
            if (tokenIn.PathToNavProp.NextToken != null && !tokenIn.PathToNavProp.IsNamespaceOrContainerQualified())
            {
                throw new ODataException(ODataErrorStrings.ExpandItemBinder_TraversingANonNormalizedTree);
            }

            PathSegmentToken currentToken = tokenIn.PathToNavProp;
            IEdmEntityType currentLevelEntityType = this.entityType;
            List<ODataPathSegment> pathSoFar = new List<ODataPathSegment>();
            PathSegmentToken firstNonTypeToken = currentToken;

            if (currentToken.IsNamespaceOrContainerQualified())
            {
                pathSoFar.AddRange(SelectExpandPathBinder.FollowTypeSegments(currentToken, this.Model, this.Settings.SelectExpandLimit, ref currentLevelEntityType, out firstNonTypeToken));
            }

            IEdmProperty edmProperty = currentLevelEntityType.FindProperty(firstNonTypeToken.Identifier);
            if (edmProperty == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_PropertyNotDeclared(currentLevelEntityType.FullName(), currentToken.Identifier));
            }

            IEdmNavigationProperty currentNavProp = edmProperty as IEdmNavigationProperty;
            if (currentNavProp == null)
            {
                // the server allowed non-navigation, non-stream properties to be expanded, but then ignored them.
                if (this.Settings.UseWcfDataServicesServerBehavior && !edmProperty.Type.IsStream())
                {
                    return null;
                }

                throw new ODataException(ODataErrorStrings.ExpandItemBinder_PropertyIsNotANavigationProperty(currentToken.Identifier, currentLevelEntityType.FullName()));
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
            if (this.entitySet != null)
            {
                targetEntitySet = this.entitySet.FindNavigationTarget(currentNavProp);
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

            return new ExpandedNavigationSelectItem(pathToNavProp, targetEntitySet, filterOption, orderbyOption, tokenIn.TopOption, tokenIn.SkipOption, tokenIn.InlineCountOption, subSelectExpand);
        }

        /// <summary>
        /// Build a new MetadataBinder to use for expand options.
        /// </summary>
        /// <param name="targetEntitySet">The entity set being expanded.</param>
        /// <returns>A new MetadataBinder ready to bind a Filter or Orderby clause.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        private MetadataBinder BuildNewMetadataBinder(IEdmEntitySet targetEntitySet)
        {
            BindingState state = new BindingState(this.configuration)
                {
                    ImplicitRangeVariable =
                        NodeFactory.CreateImplicitRangeVariable(
                                                                targetEntitySet.ElementType.ToTypeReference(),
                                                                targetEntitySet)
                };
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            return new MetadataBinder(state);
        }
    }
}
