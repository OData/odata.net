//---------------------------------------------------------------------
// <copyright file="SelectItemTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;

    /// <summary>
    /// Translator interface for walking a Select Item Tree.
    /// </summary>
    /// <typeparam name="T">Generic type produced by the translator.</typeparam>
    public abstract class SelectItemTranslator<T>
    {
        /// <summary>
        /// Translate a WildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Translate(WildcardSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a PathSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Translate(PathSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a ContainerQualifiedWildcardSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Translate(NamespaceQualifiedWildcardSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate an ExpandedNavigationSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Translate(ExpandedNavigationSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate an ExpandedReferenceSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Translate(ExpandedReferenceSelectItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate an ExpandedCountSelectItem
        /// </summary>
        /// <param name="item">the item to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Translate(ExpandedCountSelectItem item)
        {
            throw new NotImplementedException();
        }
    }
}