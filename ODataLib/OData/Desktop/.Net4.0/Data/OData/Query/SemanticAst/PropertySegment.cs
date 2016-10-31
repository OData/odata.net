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

    using System.Collections.ObjectModel;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// A segment representing a structural property
    /// </summary>
    public sealed class PropertySegment : ODataPathSegment
    {
        /// <summary>
        /// The structural property referred to by this segment
        /// </summary>
        private readonly IEdmStructuralProperty property;

        /// <summary>
        /// Build a segment based on a structural property
        /// </summary>
        /// <param name="property">The structural property that this segment represents.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input property is null.</exception>
        public PropertySegment(IEdmStructuralProperty property) 
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            this.property = property;

            this.Identifier = property.Name;
            this.TargetEdmType = property.Type.Definition;
            this.SingleResult = !property.Type.IsCollection();
        }

        /// <summary>
        /// Gets the structural property that this segment represents.
        /// </summary>
        public IEdmStructuralProperty Property
        {
            get { return this.property; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="PropertySegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.Property.Type.Definition; }
        }

        /// <summary>
        /// Translate a <see cref="PropertySegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>/>.
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
        /// Handle a <see cref="PropertySegment"/> using an instance of <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input handler is null.</exception>
        public override void Handle(PathSegmentHandler handler)
        {
            ExceptionUtils.CheckArgumentNotNull(handler, "translator");
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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            PropertySegment otherProperty = other as PropertySegment;
            return otherProperty != null && otherProperty.Property == this.Property;
        }
    }
}
