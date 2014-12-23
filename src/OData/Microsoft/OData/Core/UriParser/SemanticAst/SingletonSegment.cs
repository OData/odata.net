//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// A segment representing an singleton in a path.
    /// </summary>
    public sealed class SingletonSegment : ODataPathSegment
    {
        /// <summary>
        /// The singleton represented by this segment.
        /// </summary>
        private readonly IEdmSingleton singleton;

        /// <summary>
        /// Build a segment representing an singleton
        /// </summary>
        /// <param name="singleton">The singleton represented by this segment.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input singleton is null.</exception>
        public SingletonSegment(IEdmSingleton singleton)
        {
            ExceptionUtils.CheckArgumentNotNull(singleton, "singleton");

            this.singleton = singleton;

            this.TargetEdmNavigationSource = singleton;
            this.TargetEdmType = singleton.EntityType();
            this.TargetKind = RequestTargetKind.Resource;
            this.SingleResult = true;
        }

        /// <summary>
        /// Gets the singleton represented by this segment.
        /// </summary>
        public IEdmSingleton Singleton
        {
            get { return this.singleton; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="SingletonSegment"/>. 
        /// This will always be an  <see cref="IEdmEntityType"/> of this singleton.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.singleton.EntityType(); }
        }

        /// <summary>
        /// Translate an <see cref="SingletonSegment"/> into another type using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle an <see cref="SingletonSegment"/> using the an instance of the <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void HandleWith(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "handler");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another segment.
        /// </summary>
        /// <param name="other">the other segment to check.</param>
        /// <returns>true if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            SingletonSegment otherSingleton = other as SingletonSegment;
            return otherSingleton != null && otherSingleton.singleton == this.Singleton;
        }
    }
}
