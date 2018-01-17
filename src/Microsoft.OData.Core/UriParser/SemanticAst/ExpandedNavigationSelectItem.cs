//---------------------------------------------------------------------
// <copyright file="ExpandedNavigationSelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// This represents one level of expansion for a particular expansion tree.
    /// </summary>
    public sealed class ExpandedNavigationSelectItem : ExpandedReferenceSelectItem
    {
        /// <summary>
        /// Create an Expand item using a nav prop, its entity set and a SelectExpandClause
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="navigationSource">the navigation source for this ExpandItem</param>
        /// <param name="selectExpandOption">This level select and any sub expands for this expand item.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedNavigationSelectItem(ODataExpandPath pathToNavigationProperty, IEdmNavigationSource navigationSource, SelectExpandClause selectExpandOption)
            : this(pathToNavigationProperty, navigationSource, selectExpandOption, null, null, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Create an expand item, using a navigationProperty, its entity set, and any expand options.
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="navigationSource">the navigation source for this expand level.</param>
        /// <param name="selectAndExpand">This level select and any sub expands for this expand item.</param>
        /// <param name="filterOption">A filter clause for this expand (can be null)</param>
        /// <param name="orderByOption">An Orderby clause for this expand (can be null)</param>
        /// <param name="topOption">A top clause for this expand (can be null)</param>
        /// <param name="skipOption">A skip clause for this expand (can be null)</param>
        /// <param name="countOption">An query count clause for this expand (can be null)</param>
        /// <param name="searchOption">An levels clause for this expand (can be null)</param>
        /// <param name="levelsOption">An levels clause for this expand (can be null)</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedNavigationSelectItem(
             ODataExpandPath pathToNavigationProperty,
             IEdmNavigationSource navigationSource,
             SelectExpandClause selectAndExpand,
             FilterClause filterOption,
             OrderByClause orderByOption,
             long? topOption,
             long? skipOption,
             bool? countOption,
             SearchClause searchOption,
             LevelsClause levelsOption)
            : base(pathToNavigationProperty, navigationSource, filterOption, orderByOption, topOption, skipOption, countOption, searchOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavigationProperty, "pathToNavigationProperty");

            this.SelectAndExpand = selectAndExpand;
            this.LevelsOption = levelsOption;
        }

        /// <summary>
        /// Create an expand item, using a navigationProperty, its entity set, and any expand options.
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="navigationSource">the navigation source for this expand level.</param>
        /// <param name="selectAndExpand">This level select and any sub expands for this expand item.</param>
        /// <param name="filterOption">A filter clause for this expand (can be null)</param>
        /// <param name="orderByOption">An Orderby clause for this expand (can be null)</param>
        /// <param name="topOption">A top clause for this expand (can be null)</param>
        /// <param name="skipOption">A skip clause for this expand (can be null)</param>
        /// <param name="countOption">An query count clause for this expand (can be null)</param>
        /// <param name="searchOption">An levels clause for this expand (can be null)</param>
        /// <param name="levelsOption">An levels clause for this expand (can be null)</param>
        /// <param name="computeOption">A compute clause for this expand (can be null)</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedNavigationSelectItem(
             ODataExpandPath pathToNavigationProperty,
             IEdmNavigationSource navigationSource,
             SelectExpandClause selectAndExpand,
             FilterClause filterOption,
             OrderByClause orderByOption,
             long? topOption,
             long? skipOption,
             bool? countOption,
             SearchClause searchOption,
             LevelsClause levelsOption,
             ComputeClause computeOption)
            : base(pathToNavigationProperty, navigationSource, filterOption, orderByOption, topOption, skipOption, countOption, searchOption, computeOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavigationProperty, "pathToNavigationProperty");

            this.SelectAndExpand = selectAndExpand;
            this.LevelsOption = levelsOption;
        }

        /// <summary>
        /// The select and expand clause for this expanded navigation.
        /// </summary>
        public SelectExpandClause SelectAndExpand
        {
            get; private set;
        }

        /// <summary>
        /// Gets the levels clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public LevelsClause LevelsOption
        {
           get; private set;
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
