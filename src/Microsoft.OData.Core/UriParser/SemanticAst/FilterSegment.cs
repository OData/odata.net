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
        private readonly ParameterAliasNode parameterAlias;

        /// <summary>
        /// The parameter for the alias which represents a single value from the collection.
        /// </summary>
        private readonly RangeVariable rangeVariable;

        /// <summary>
        /// The type that this segment is bound to.
        /// </summary>
        private readonly IEdmType bindingType;

        /// <summary>
        /// String representing "$filter(expression)".
        /// </summary>
        private readonly string fullSegment;

        /// <summary>
        /// Build a segment representing $filter.
        /// </summary>
        /// <param name="parameterAlias">Corresponding parameter alias for the $filter path segment.</param>
        /// <param name="rangeVariable">An expression that represents a single value from the collection.</param>
        /// <param name="navigationSource">The navigation source that this filter applies to.</param>
        /// <exception cref="System.ArgumentNullException">Throws if any input parameter is null.</exception>
        /// <remarks>$filter should not be applied on singletons or single entities.</remarks>
        public FilterSegment(ParameterAliasNode parameterAlias, RangeVariable rangeVariable, IEdmNavigationSource navigationSource)
        {
            ExceptionUtils.CheckArgumentNotNull(parameterAlias, "parameterAlias");
            ExceptionUtils.CheckArgumentNotNull(rangeVariable, "rangeVariable");
            ExceptionUtils.CheckArgumentNotNull(navigationSource, "navigationSource");

            this.Identifier = UriQueryConstants.FilterSegment;
            this.SingleResult = false;
            this.TargetEdmNavigationSource = navigationSource;
            this.TargetKind = RequestTargetKind.Resource;
            this.TargetEdmType = rangeVariable.TypeReference.Definition;

            this.parameterAlias = parameterAlias;
            this.rangeVariable = rangeVariable;
            this.bindingType = navigationSource.Type;
            this.fullSegment = UriQueryConstants.FilterSegment + ExpressionConstants.SymbolOpenParen + parameterAlias.Alias
                + ExpressionConstants.SymbolClosedParen;
        }

        /// <summary>
        /// Gets the filter path segment's alias.
        /// </summary>
        public ParameterAliasNode ParameterAlias
        {
            get { return this.parameterAlias; }
        }

        /// <summary>
        /// Gets the string representing "$filter(expression)".
        /// </summary>
        public string FullSegment
        {
            get { return this.fullSegment; }
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
                otherSegment.TargetEdmNavigationSource == this.TargetEdmNavigationSource &&
                otherSegment.ParameterAlias == this.ParameterAlias &&
                otherSegment.ItemType == this.ItemType &&
                otherSegment.RangeVariable == this.RangeVariable;
        }
    }
}
