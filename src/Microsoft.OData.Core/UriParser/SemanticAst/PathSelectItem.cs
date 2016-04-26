//---------------------------------------------------------------------
// <copyright file="PathSelectItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Class to represent the selection of a specific path.
    /// </summary>
    public sealed class PathSelectItem : SelectItem
    {
        /// <summary>
        /// The selected path.
        /// </summary>
        private readonly ODataSelectPath selectedPath;

        /// <summary>
        /// Constructs a <see cref="SelectItem"/> to indicate that a specific path is selected.
        /// </summary>
        /// <param name="selectedPath">The selected path.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input selectedPath is null.</exception>
        public PathSelectItem(ODataSelectPath selectedPath)
        {
            ExceptionUtils.CheckArgumentNotNull(selectedPath, "selectedPath");
            this.selectedPath = selectedPath;
        }

        /// <summary>
        /// Gets the selected path.
        /// </summary>
        public ODataSelectPath SelectedPath
        {
            get { return this.selectedPath; }
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