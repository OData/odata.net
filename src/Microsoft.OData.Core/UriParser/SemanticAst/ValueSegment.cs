//---------------------------------------------------------------------
// <copyright file="ValueSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// A segment representing $value
    /// </summary>
    public sealed class ValueSegment : ODataPathSegment
    {
        /// <summary>
        /// The <see cref="IEdmType"/> of this <see cref="ValueSegment"/>.
        /// </summary>
        private readonly IEdmType edmType;

        /// <summary>
        /// Build a segment to represnt $value.
        /// </summary>
        /// <param name="previousType">The type of the segment before $value. This may be null, for cases such as open properties.</param>
        /// <exception cref="ODataException">Throws if the input previousType is a colleciton type.</exception>
        public ValueSegment(IEdmType previousType)
        {
            this.Identifier = UriQueryConstants.ValueSegment;
            this.SingleResult = true;

            if (previousType is IEdmCollectionType)
            {
                throw new ODataException(ODataErrorStrings.PathParser_CannotUseValueOnCollection);
            }

            if (previousType is IEdmEntityType)
            {
                // TODO: Throw if the entity type does not have a HasStream attribute
                // $value on an entity type means default stream
                this.edmType = EdmCoreModel.Instance.GetStream(false).Definition;
            }
            else
            {
                // Otherwise $value is the value of the previous property (null is OK for open properties)
                this.edmType = previousType;
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of this <see cref="ValueSegment"/>.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.edmType; }
        }

        /// <summary>
        /// Translate a <exception cref="ValueSegment"> into another object using an instance of</exception> <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle a <see cref="ValueSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
        /// </summary>
        /// <param name="handler">An implementation of the translator interface.</param>
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
        internal override bool Equals(ODataPathSegment other)
        {
            ValueSegment otherValueSegment = other as ValueSegment;
            return otherValueSegment != null &&
                   otherValueSegment.EdmType == this.edmType;
        }
    }
}

