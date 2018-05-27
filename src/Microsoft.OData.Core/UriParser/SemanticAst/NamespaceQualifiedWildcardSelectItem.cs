//---------------------------------------------------------------------
// <copyright file="NamespaceQualifiedWildcardSelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class to represent the selection of all the actions and functions in a specified namespace.
    /// </summary>
    public sealed class NamespaceQualifiedWildcardSelectItem : SelectItem
    {
        /// <summary>
        /// Creates an instance of this class with the specified with the Namespace.
        /// </summary>
        /// <param name="namespaceName">The namespace of the wildcard.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input container is null.</exception>
        public NamespaceQualifiedWildcardSelectItem(string namespaceName)
        {
            ExceptionUtils.CheckArgumentNotNull(namespaceName, "namespaceName");
            this.Namespace = namespaceName;
        }

        /// <summary>
        /// Gets the <see cref="IEdmEntityContainer"/> whose actions and functions should be selected.
        /// </summary>
        public string Namespace { get; private set; }

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