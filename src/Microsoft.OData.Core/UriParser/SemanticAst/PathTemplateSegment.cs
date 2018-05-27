//---------------------------------------------------------------------
// <copyright file="PathTemplateSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// A segment representing an path template in a path.
    /// </summary>
    public sealed class PathTemplateSegment : ODataPathSegment
    {
        /// <summary>
        /// Build a segment representing a path template.
        /// </summary>
        /// <param name="literalText">The literal text for the template segment.</param>
        public PathTemplateSegment(string literalText)
        {
            this.LiteralText = literalText;
            this.Identifier = literalText;
            this.SingleResult = true; // This single result value is meaningless as we don't know what it should be.
            this.TargetKind = RequestTargetKind.Nothing;
        }

        /// <summary>
        /// The literal text for the template segment.
        /// </summary>
        public string LiteralText { get; private set; }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="ODataPathSegment"/>.
        /// </summary>
        /// <remarks>
        /// Path template does not resolve to certain EdmType, so this value is always null.
        /// </remarks>
        public override IEdmType EdmType
        {
            get { return null; }
        }

        /// <summary>
        /// Translate a <see cref="PathTemplateSegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="PathTemplateSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "handler");
            handler.Handle(this);
        }
    }
}
