//---------------------------------------------------------------------
// <copyright file="WildcardSelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Class to represent a '*' selection item, indicating that all structural properties should be selected.
    /// </summary>
    public sealed class WildcardSelectItem : SelectItem
    {
        private readonly List<SelectItem> subsumedSelectItems = new List<SelectItem>();

        /// <summary>
        /// Returns all <see cref="SelectItem"/> that were provided by the client but are ignored by the framework because they are
        /// covered by the wildcard. This allows services to determine if properties were explicitly selected.
        /// </summary>
        public IEnumerable<SelectItem> SubsumedSelectItems => new ReadOnlyCollection<SelectItem>(this.subsumedSelectItems);

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

        /// <summary>
        /// Adds the provided <see cref="SelectItem"/> to the set of items that are replaced by the wildcard.
        /// </summary>
        internal void AddSubsumed(SelectItem selectItem)
        {
            this.subsumedSelectItems.Add(selectItem);
        }

        /// <summary>
        /// Adds the provided <see cref="IEnumerable{SelectItem}"/> to the set of items that are replaced by the wildcard.
        /// </summary>
        internal void AddSubsumed(IEnumerable<SelectItem> selectItems)
        {
            this.subsumedSelectItems.AddRange(selectItems);
        }
    }
}