//---------------------------------------------------------------------
// <copyright file="ExpandedReferenceSelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// This represents one level of expansion for a particular expansion tree with $ref operation.
    /// </summary>
    public class ExpandedReferenceSelectItem : SelectItem
    {
        /// <summary>
        /// Create an Expand item using a nav prop, its entity set
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="navigationSource">the navigation source for this ExpandItem</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedReferenceSelectItem(ODataExpandPath pathToNavigationProperty, IEdmNavigationSource navigationSource)
            : this(pathToNavigationProperty, navigationSource, null, null, null, null, null, null)
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
        /// <param name="searchOption">A search clause for this expand (can be null)</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedReferenceSelectItem(
             ODataExpandPath pathToNavigationProperty,
             IEdmNavigationSource navigationSource,
             FilterClause filterOption,
             OrderByClause orderByOption,
             long? topOption,
             long? skipOption,
             bool? countOption,
             SearchClause searchOption)
            : this(pathToNavigationProperty, navigationSource, filterOption, orderByOption, topOption, skipOption, countOption, searchOption, null)
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
        /// <param name="searchOption">A search clause for this expand (can be null)</param>
        /// <param name="computeOption">A compute clause for this expand (can be null)</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedReferenceSelectItem(
             ODataExpandPath pathToNavigationProperty,
             IEdmNavigationSource navigationSource,
             FilterClause filterOption,
             OrderByClause orderByOption,
             long? topOption,
             long? skipOption,
             bool? countOption,
             SearchClause searchOption,
             ComputeClause computeOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavigationProperty, "pathToNavigationProperty");

            this.PathToNavigationProperty = pathToNavigationProperty;
            this.NavigationSource = navigationSource;
            this.FilterOption = filterOption;
            this.OrderByOption = orderByOption;
            this.TopOption = topOption;
            this.SkipOption = skipOption;
            this.CountOption = countOption;
            this.SearchOption = searchOption;
            this.ComputeOption = computeOption;
        }

        /// <summary>
        /// Gets the Path for this expand level.
        /// This path includes zero or more type segments followed by exactly one Navigation Property.
        /// </summary>
        public ODataExpandPath PathToNavigationProperty
        {
            get; private set;
        }

        /// <summary>
        /// Gets the navigation source for this level.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get; private set;
        }

        /// <summary>
        /// The filter clause for this expand item
        /// </summary>
        public FilterClause FilterOption
        {
            get; private set;
        }

        /// <summary>
        /// Gets the levels clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public SearchClause SearchOption
        {
            get; private set;
        }

        /// <summary>
        /// Gets the orderby clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public OrderByClause OrderByOption
        {
            get; private set;
        }

        /// <summary>
        /// Gets the compute clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public ComputeClause ComputeOption
        {
            get; private set;
        }

        /// <summary>
        /// Gets the top clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public long? TopOption
        {
            get; private set;
        }

        /// <summary>
        /// Gets the skip clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public long? SkipOption
        {
            get; private set;
        }

        /// <summary>
        /// Gets the count clause for this expand item. Can be null if not specified(and will always be null in NonOptionMode).
        /// </summary>
        public bool? CountOption
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
