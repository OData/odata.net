//---------------------------------------------------------------------
// <copyright file="PathCountSelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Class to represent $select=collectionProperty/$count.
    /// </summary>
    public sealed class PathCountSelectItem : SelectItem
    {
        /// <summary>
        /// Initializes a new <see cref="PathCountSelectItem"/>.
        /// </summary>
        /// <param name="selectedPath">The selected path.</param>
        public PathCountSelectItem(ODataSelectPath selectedPath)
            : this(selectedPath, null, null, null)
        { }

        /// <summary>
        /// Initializes a new <see cref="PathCountSelectItem"/>.
        /// </summary>
        /// <param name="selectedPath">The selected path.</param>
        /// <param name="navigationSource">The navigation source for this select item.</param>
        /// <param name="filter">A filter clause for this select (can be null).</param>
        /// <param name="search">A search clause for this select (can be null).</param>
        public PathCountSelectItem(ODataSelectPath selectedPath,
            IEdmNavigationSource navigationSource,
            FilterClause filter,
            SearchClause search)
        {
            ExceptionUtils.CheckArgumentNotNull(selectedPath, "selectedPath");

            SelectedPath = selectedPath;
            NavigationSource = navigationSource;
            Filter = filter;
            Search = search;
        }

        /// <summary>
        /// Gets the selected path.
        /// </summary>
        public ODataSelectPath SelectedPath { get; }

        /// <summary>
        /// Gets the navigation source for this select level.
        /// </summary>
        public IEdmNavigationSource NavigationSource { get; }

        /// <summary>
        /// The filter clause for this select level.
        /// </summary>
        public FilterClause Filter { get; }

        /// <summary>
        /// Gets the search clause for this select level.
        /// </summary>
        public SearchClause Search { get; }

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