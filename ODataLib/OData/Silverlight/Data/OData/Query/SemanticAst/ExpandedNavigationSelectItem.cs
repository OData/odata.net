//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SemanticAst
{
    using Microsoft.Data.Edm;

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
        /// The entity set for this expansion level.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

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
        /// The inlinecount option for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        private readonly InlineCountKind? inlineCountOption;

        /// <summary>
        /// The select that applies to this level, and any sub expand levels below this one.
        /// </summary>
        private readonly SelectExpandClause selectAndExpand;

        /// <summary>
        /// Create an Expand item using a nav prop, its entity set and a SelectExpandClause 
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="entitySet">the entity set for this ExpandItem</param>
        /// <param name="selectExpandOption">This level select and any sub expands for this expand item.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedNavigationSelectItem(ODataExpandPath pathToNavigationProperty, IEdmEntitySet entitySet, SelectExpandClause selectExpandOption)
            : this(pathToNavigationProperty, entitySet, null, null, null, null, null, selectExpandOption)
        {
        }

        /// <summary>
        /// Create an expand item, using a navigationProperty, its entity set, and any expand options.
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="entitySet">the entity set for this expand level.</param>
        /// <param name="filterOption">A filter clause for this expand (can be null)</param>
        /// <param name="orderByOption">An Orderby clause for this expand (can be null)</param>
        /// <param name="topOption">A top clause for this expand (can be null)</param>
        /// <param name="skipOption">A skip clause for this expand (can be null)</param>
        /// <param name="inlineCountOption">An Inlinecount clause for this expand (can be null)</param>
        /// <param name="selectAndExpand">This level select and any sub expands for this expand item.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
       internal ExpandedNavigationSelectItem(
            ODataExpandPath pathToNavigationProperty, 
            IEdmEntitySet entitySet,
            FilterClause filterOption, 
            OrderByClause orderByOption, 
            long? topOption, 
            long? skipOption, 
            InlineCountKind? inlineCountOption, 
            SelectExpandClause selectAndExpand)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(pathToNavigationProperty, "navigationProperty");

            this.pathToNavigationProperty = pathToNavigationProperty;
            this.entitySet = entitySet;
            this.filterOption = filterOption;
            this.orderByOption = orderByOption;
            this.topOption = topOption;
            this.skipOption = skipOption;
            this.inlineCountOption = inlineCountOption;
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
        /// Gets the EntitySet for this level.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get
            {
                return this.entitySet;
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
        internal FilterClause FilterOption
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.filterOption;
            }
        }

        /// <summary>
        /// Gets the orderby clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        internal OrderByClause OrderByOption
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.orderByOption;
            }
        }

        /// <summary>
        /// Gets the top clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        internal long? TopOption
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.topOption;
            }
        }

        /// <summary>
        /// Gets the skip clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        internal long? SkipOption
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.skipOption;
            }
        }

        /// <summary>
        /// Gets the inlinecount clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        internal InlineCountKind? InlineCountOption
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.inlineCountOption;
            }
        }
    }
}
