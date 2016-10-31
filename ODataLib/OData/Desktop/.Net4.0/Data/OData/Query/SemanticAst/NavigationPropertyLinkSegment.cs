//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces

    using System;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;

    #endregion Namespaces

    /// <summary>
    /// A segment representing $links or $ref
    /// </summary>
    public sealed class NavigationPropertyLinkSegment : ODataPathSegment
    {
        /// <summary>
        /// The navigation property this link or ref acts on.
        /// </summary>
        private readonly IEdmNavigationProperty navigationProperty;

        /// <summary>
        /// Build a segment to represnt $links or $ref on a Nav prop
        /// </summary>
        /// <param name="navigationProperty">The navigaiton property this link or ref acts on</param>
        /// <param name="entitySet">The set of entities linked to by this <see cref="NavigationPropertyLinkSegment"/>. This can be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input navigationProperty is null.</exception>
        public NavigationPropertyLinkSegment(IEdmNavigationProperty navigationProperty, IEdmEntitySet entitySet) 
        {
            ExceptionUtils.CheckArgumentNotNull(navigationProperty, "navigationProperty");
            this.navigationProperty = navigationProperty;
            this.TargetEdmEntitySet = entitySet;

            this.Identifier = navigationProperty.Name;
            this.TargetEdmType = navigationProperty.Type.Definition;
            this.SingleResult = !navigationProperty.Type.IsCollection();
            this.TargetKind = RequestTargetKind.Resource;
        }

        /// <summary>
        /// Gets the navigation property this link or ref acts on.
        /// </summary>
        public IEdmNavigationProperty NavigationProperty
        {
            get { return this.navigationProperty; }
        }

        /// <summary>
        /// Gets the set of entities linked to by this <see cref="NavigationPropertyLinkSegment"/>.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get { return this.TargetEdmEntitySet; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="NavigationPropertyLinkSegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.navigationProperty.Type.Definition; }
        }

        /// <summary>
        /// Translate a <see cref="PathSegmentTranslator{T}"/>
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after visiting this token.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T Translate<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Translate a <see cref="PathSegmentHandler"/> to walk a tree of <see cref="ODataPathSegment"/>s.
        /// </summary>
        /// <param name="handler">An implementation of the translator interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void Handle(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "translator");
            handler.Handle(this);
        }

        /// <summary>
        /// Check if this segment is equal to another.
        /// </summary>
        /// <param name="other">The other segment to check.</param>
        /// <returns>True if the other segment is equal.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            NavigationPropertyLinkSegment otherLinkSegment = other as NavigationPropertyLinkSegment;
            return otherLinkSegment != null && otherLinkSegment.NavigationProperty == this.navigationProperty;
        }
    }
}
