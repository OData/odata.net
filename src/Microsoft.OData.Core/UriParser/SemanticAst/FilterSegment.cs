//---------------------------------------------------------------------
// <copyright file="FilterSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// A segment representing $filter in a path.
    /// </summary>
    public sealed class FilterSegment : ODataPathSegment
    {
        /// <summary>
        /// The filter path segment's alias.
        /// </summary>
        private readonly ParameterAliasNode alias;

        /// <summary>
        /// The parameter for the alias which represents a single value from the collection.
        /// </summary>
        private readonly RangeVariable rangeVariable;

        /// <summary>
        /// The type that this segment is bound to.
        /// </summary>
        private readonly IEdmType bindingType;

        /// <summary>
        /// Build a segment representing $filter.
        /// </summary>
        /// <param name="alias">Corresponding parameter alias for the $filter path segment.</param>
        /// <param name="rangeVariable">An expression that represents a single value from the collection.</param>
        /// <param name="bindingType">The type that this segment is bound to.</param>
        /// <param name="singleResult">Whether the segment targets a single result.</param>
        /// <exception cref="System.ArgumentNullException">Throws if any input parameter is null.</exception>
        public FilterSegment(ParameterAliasNode alias, RangeVariable rangeVariable, IEdmType bindingType, bool singleResult)
        {
            ExceptionUtils.CheckArgumentNotNull(alias, "alias");
            ExceptionUtils.CheckArgumentNotNull(rangeVariable, "rangeVariable");
            ExceptionUtils.CheckArgumentNotNull(bindingType, "bindingType");

            this.alias = alias;
            this.rangeVariable = rangeVariable;
            this.bindingType = bindingType;

            this.Identifier = UriQueryConstants.FilterSegment;
            this.SingleResult = singleResult; // !!! Check during review
            this.TargetKind = RequestTargetKind.Resource; // !!! Check during review
            this.TargetEdmType = this.rangeVariable.TypeReference.Definition; // !!! Check during review
        }

        /// <summary>
        /// Gets the filter path segment's alias.
        /// </summary>
        public ParameterAliasNode Alias
        {
            get { return this.alias; }
        }

        /// <summary>
        /// Gets the parameter for the expression which represents a single value from the collection.
        /// </summary>
        public RangeVariable RangeVariable
        {
            get { return this.rangeVariable; }
        }

        /// <summary>
        /// Gets the type of item returned by this clause.
        /// </summary>
        public IEdmTypeReference ItemType
        {
            get { return this.RangeVariable.TypeReference; }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> to which this <see cref="FilterSegment"/> applies.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.bindingType; }
        }

        /// <summary>
        /// Translate a <see cref="FilterSegment"/> using an instance of <see cref="PathSegmentTranslator{T}"/>.
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
        /// Handle a <see cref="FilterSegment"/> using an instance of <see cref="PathSegmentHandler"/>.
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
        /// <exception cref="System.ArgumentNullException">throws if the input other is null.</exception>
        internal override bool Equals(ODataPathSegment other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
            FilterSegment otherSegment = other as FilterSegment;

            return otherSegment != null &&
                otherSegment.EdmType == this.EdmType &&
                otherSegment.Alias == this.Alias &&
                otherSegment.ItemType == this.ItemType &&
                otherSegment.RangeVariable == this.RangeVariable;
        }
    }
}
