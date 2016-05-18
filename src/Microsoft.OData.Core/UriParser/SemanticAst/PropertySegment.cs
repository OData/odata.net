//---------------------------------------------------------------------
// <copyright file="PropertySegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;

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
        public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="PropertySegment"/> using an instance of <see cref="PathSegmentHandler"/>.
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
            PropertySegment otherProperty = other as PropertySegment;
            return otherProperty != null && otherProperty.Property == this.Property;
        }
    }
}
