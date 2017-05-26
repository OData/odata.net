//---------------------------------------------------------------------
// <copyright file="SelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// An item that has been selected by the query at the current level of the tree.
    /// </summary>
    public abstract class SelectItem
    {
        /// <summary>
        /// Translate a <see cref="SelectItem"/> using an implemntation of<see cref="SelectItemTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        public abstract T TranslateWith<T>(SelectItemTranslator<T> translator);

        /// <summary>
        /// Handle a <see cref="SelectItem"/> using an implementation of a <see cref="SelectItemHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        public abstract void HandleWith(SelectItemHandler handler);
    }
}