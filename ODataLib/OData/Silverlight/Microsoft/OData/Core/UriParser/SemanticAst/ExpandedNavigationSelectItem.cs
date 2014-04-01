//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;

    /// <summary>
    /// This represents one level of expansion for a particular expansion tree.
    /// </summary>
    public sealed class ExpandedNavigationSelectItem : SelectItem
    {
        /// <summary>
        /// The Path for this expand level.
        /// This path includes zero or more type segments followed by exactly one Navigation Property.
        /// </summary>
        private readonly ODataExpandPath pathToNavigationProperty;

        /// <summary>
        /// The navigation source for this expansion level.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// The filter expand option for this expandItem. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        private readonly FilterClause filterOption;

        /// <summary>
        /// The orderby expand option for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        private readonly OrderByClause orderByOption;

        /// <summary>
        /// the top expand option for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        private readonly long? topOption;

        /// <summary>
        /// The skip option for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        private readonly long? skipOption;

        /// <summary>
        /// The query count option for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        private readonly bool? countOption;

        /// <summary>
        /// The levels option for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        private readonly LevelsClause levelsOption;

        /// <summary>
        /// The search option for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        private readonly SearchClause searchOption;

        /// <summary>
        /// The select that applies to this level, and any sub expand levels below this one.
        /// </summary>
        private readonly SelectExpandClause selectAndExpand;

        /// <summary>
        /// Create an Expand item using a nav prop, its entity set and a SelectExpandClause 
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="navigationSource">the navigation source for this ExpandItem</param>
        /// <param name="selectExpandOption">This level select and any sub expands for this expand item.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedNavigationSelectItem(ODataExpandPath pathToNavigationProperty, IEdmNavigationSource navigationSource, SelectExpandClause selectExpandOption)
            : this(pathToNavigationProperty, navigationSource, null, null, null, null, null, null, null, selectExpandOption)
        {
        }

        /// <summary>
        /// Create an expand item, using a navigationProperty, its entity set, and any expand options.
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="navigationSource">the navigation source for this expand level.</param>
        /// <param name="filterOption">A filter clause for this expand (can be null)</param>
        /// <param name="orderByOption">An Orderby clause for this expand (can be null)</param>
        /// <param name="topOption">A top clause for this expand (can be null)</param>
        /// <param name="skipOption">A skip clause for this expand (can be null)</param>
        /// <param name="countOption">An query count clause for this expand (can be null)</param>
        /// <param name="levelsOption">An levels clause for this expand (can be null)</param>
        /// <param name="searchOption">An levels clause for this expand (can be null)</param>
        /// <param name="selectAndExpand">This level select and any sub expands for this expand item.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        internal ExpandedNavigationSelectItem(
             ODataExpandPath pathToNavigationProperty,
             IEdmNavigationSource navigationSource,
             FilterClause filterOption,
             OrderByClause orderByOption,
             long? topOption,
             long? skipOption,
             bool? countOption,
             LevelsClause levelsOption,
             SearchClause searchOption,
             SelectExpandClause selectAndExpand)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavigationProperty, "navigationProperty");

            this.pathToNavigationProperty = pathToNavigationProperty;
            this.navigationSource = navigationSource;
            this.filterOption = filterOption;
            this.orderByOption = orderByOption;
            this.topOption = topOption;
            this.skipOption = skipOption;
            this.countOption = countOption;
            this.levelsOption = levelsOption;
            this.searchOption = searchOption;
            this.selectAndExpand = selectAndExpand;
        }

        /// <summary>
        /// Gets the Path for this expand level.
        /// This path includes zero or more type segments followed by exactly one Navigation Property.
        /// </summary>
        public ODataExpandPath PathToNavigationProperty
        {
            get
            {
                return this.pathToNavigationProperty;
            }
        }

        /// <summary>
        /// Gets the navigation source for this level.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get
            {
                return this.navigationSource;
            }
        }

        /// <summary>
        /// The select and expand clause for this expanded navigation.
        /// </summary>
        public SelectExpandClause SelectAndExpand
        {
            get
            {
                return this.selectAndExpand;
            }
        }

        /// <summary>
        /// The filter clause for this expand item
        /// </summary>
        public FilterClause FilterOption
        {
            get
            {
                return this.filterOption;
            }
        }

        /// <summary>
        /// Gets the levels clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public LevelsClause LevelsOption
        {
            get
            {
                return this.levelsOption;
            }
        }

        /// <summary>
        /// Gets the levels clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public SearchClause SearchOption
        {
            get
            {
                return this.searchOption;
            }
        }

        /// <summary>
        /// Gets the orderby clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public OrderByClause OrderByOption
        {
            get
            {
                return this.orderByOption;
            }
        }

        /// <summary>
        /// Gets the top clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public long? TopOption
        {
            get
            {
                return this.topOption;
            }
        }

        /// <summary>
        /// Gets the skip clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public long? SkipOption
        {
            get
            {
                return this.skipOption;
            }
        }

        /// <summary>
        /// Gets the count clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public bool? CountOption
        {
            get
            {
                return this.countOption;
            }
        }

        /// <summary>
        /// Translate using a <see cref="SelectItemTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this item.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T TranslateWith<T>(SelectItemTranslator<T> translator)
        {
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle using a <see cref="SelectItemHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(SelectItemHandler handler)
        {
            handler.Handle(this);
        }
    }
}
