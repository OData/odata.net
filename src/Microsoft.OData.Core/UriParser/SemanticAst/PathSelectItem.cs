//---------------------------------------------------------------------
// <copyright file="PathSelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Class to represent the selection of a specific path.
    /// </summary>
    public sealed class PathSelectItem : SelectItem
    {
        /// <summary>
        /// Constructs a <see cref="PathSelectItem"/> to indicate that a specific path is selected.
        /// </summary>
        /// <param name="selectedPath">The selected path.</param>
        public PathSelectItem(ODataSelectPath selectedPath)
            : this(selectedPath, null, null, null, null, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Constructs a <see cref="PathSelectItem"/> to indicate that a specific path is selected.
        /// </summary>
        /// <param name="selectedPath">The selected path.</param>
        /// <param name="navigationSource">The navigation source for this select item.</param>
        /// <param name="selectAndExpand">This sub select and sub expand for this select item.</param>
        /// <param name="filterOption">A filter clause for this select (can be null).</param>
        /// <param name="orderByOption">An Orderby clause for this select (can be null).</param>
        /// <param name="topOption">A top clause for this select (can be null).</param>
        /// <param name="skipOption">A skip clause for this select (can be null).</param>
        /// <param name="countOption">A count clause for this select (can be null).</param>
        /// <param name="searchOption">A search clause for this select (can be null).</param>
        /// <param name="computeOption">A compute clause for this expand (can be null).</param>
        /// <exception cref="ArgumentNullException">Throws if the input selectedPath is null.</exception>
        public PathSelectItem(ODataSelectPath selectedPath,
            IEdmNavigationSource navigationSource,
            SelectExpandClause selectAndExpand,
            FilterClause filterOption,
            OrderByClause orderByOption,
            long? topOption,
            long? skipOption,
            bool? countOption,
            SearchClause searchOption,
            ComputeClause computeOption)
        {
            ExceptionUtils.CheckArgumentNotNull(selectedPath, "selectedPath");

            SelectedPath = selectedPath;
            NavigationSource = navigationSource;
            SelectAndExpand = selectAndExpand;
            FilterOption = filterOption;
            OrderByOption = orderByOption;
            TopOption = topOption;
            SkipOption = skipOption;
            CountOption = countOption;
            SearchOption = searchOption;
            ComputeOption = computeOption;
        }

        /// <summary>
        /// Gets the selected path.
        /// </summary>
        public ODataSelectPath SelectedPath { get; private set; }

        /// <summary>
        /// Gets the navigation source for this select level.
        /// </summary>
        public IEdmNavigationSource NavigationSource { get; internal set; }

        /// <summary>
        /// The sub select clause for this select level.
        /// </summary>
        public SelectExpandClause SelectAndExpand { get; internal set; }

        /// <summary>
        /// The filter clause for this select level.
        /// </summary>
        public FilterClause FilterOption { get; internal set; }

        /// <summary>
        /// Gets the orderby clause for this select level.
        /// </summary>
        public OrderByClause OrderByOption { get; internal set; }

        /// <summary>
        /// Gets the top clause for this select level.
        /// </summary>
        public long? TopOption { get; internal set; }

        /// <summary>
        /// Gets the skip clause for this select level.
        /// </summary>
        public long? SkipOption { get; internal set; }

        /// <summary>
        /// Gets the count clause for this select level.
        /// </summary>
        public bool? CountOption { get; internal set; }

        /// <summary>
        /// Gets the search clause for this select level.
        /// </summary>
        public SearchClause SearchOption { get; internal set; }

        /// <summary>
        /// Gets the compute clause for this select level.
        /// </summary>
        public ComputeClause ComputeOption { get; internal set; }


        /// <summary>
        /// Returns whether or not the path select item has any options applied
        /// </summary>
        public bool HasOptions
        {
            get
            {
                return
                    FilterOption != null ||
                    ComputeOption != null ||
                    SearchOption != null ||
                    TopOption != null ||
                    SkipOption != null ||
                    CountOption != null ||
                    OrderByOption != null ||
                    (SelectAndExpand != null && !SelectAndExpand.AllSelected);
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