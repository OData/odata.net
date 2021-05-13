//---------------------------------------------------------------------
// <copyright file="ExpandedCountSelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Aggregation;
    using Microsoft.OData.Edm;

    /// <summary>
    /// This represents one level of expansion for a particular expansion tree.
    /// </summary>
    public sealed class ExpandedCountSelectItem : ExpandedReferenceSelectItem
    {
        /// <summary>
        /// Create an Expand item using a nav prop, its entity set and a SelectExpandClause
        /// </summary>
        /// <param name="pathToNavigationProperty">the path to the navigation property for this expand item, including any type segments</param>
        /// <param name="navigationSource">the navigation source for this ExpandItem</param>
        /// <param name="filterOption">A filter clause for this expand (can be null)</param>
        /// <param name="searchOption">A search clause for this expand (can be null)</param>
        /// <exception cref="System.ArgumentNullException">Throws if input pathToNavigationProperty is null.</exception>
        public ExpandedCountSelectItem(ODataExpandPath pathToNavigationProperty, IEdmNavigationSource navigationSource, FilterClause filterOption, SearchClause searchOption)
            : base(pathToNavigationProperty, navigationSource, filterOption, null, null, null, null, searchOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToNavigationProperty, "pathToNavigationProperty");
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
