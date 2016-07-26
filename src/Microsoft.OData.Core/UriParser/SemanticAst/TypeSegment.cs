//---------------------------------------------------------------------
// <copyright file="TypeSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// A segment representing a cast on the previous segment to another type.
    /// </summary>
    public sealed class TypeSegment : ODataPathSegment
    {
        /// <summary>
        /// The actual edm type of this type segment.
        /// </summary>
        private readonly IEdmType edmType;

        /// <summary>
        /// The navigation source containing the entities that we are casting.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Build a type segment using the given <paramref name="actualType"/>.
        /// </summary>
        /// <param name="actualType">The target type of this segment, which may be collection type.</param>
        /// <param name="navigationSource">The navigation source containing the entities that we are casting. This can be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the actual edmType is null.</exception>
        /// <exception cref="ODataException">Throws if the actual edmType is not related to the type of elements in the input navigationSource.</exception>
        public TypeSegment(IEdmType actualType, IEdmNavigationSource navigationSource)
            : this(actualType, navigationSource == null ? actualType : navigationSource.EntityType(), navigationSource)
        {
        }

        /// <summary>
        /// Build the type segment based on the giving <paramref name="actualType"/> and <paramref name="expectedType"/>
        /// </summary>
        /// <param name="actualType">The actual type of this segment passed from Uri, which may be collection type.</param>
        /// <param name="expectedType">The type reflected from model.</param>
        /// <param name="navigationSource">The navigation source containing the entity or complex that we are casting. This can be null.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the actual or expected edmType is null.</exception>
        /// <exception cref="ODataException">Throws if the actual edmType is not related to the expected type of elements in the input navigationSource.</exception>
        public TypeSegment(IEdmType actualType, IEdmType expectedType, IEdmNavigationSource navigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(actualType, "actualType");
            ExceptionUtils.CheckArgumentNotNull(expectedType, "expectedType");

            this.edmType = actualType;
            this.navigationSource = navigationSource;

            this.TargetEdmType = expectedType;
            this.TargetEdmNavigationSource = navigationSource;

            // Check that the type they gave us is related to the type of the set
            if (navigationSource != null)
            {
                ExceptionUtil.ThrowIfTypesUnrelated(actualType, expectedType, "TypeSegments");
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="TypeSegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.edmType; }
        }

        /// <summary>
        /// Gets the navigation source containing the entities that we are casting.
        /// </summary>
        public IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Translate a <see cref="TypeSegment"/> into another type using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle a <see cref="TypeSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
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
            TypeSegment otherType = other as TypeSegment;
            return otherType != null && otherType.EdmType == this.EdmType;
        }
    }
}
