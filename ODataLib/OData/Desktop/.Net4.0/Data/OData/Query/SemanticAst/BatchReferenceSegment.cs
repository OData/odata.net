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

    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using Microsoft.Data.Edm;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// A segment representing an alias to another url in a batch.
    /// </summary>
    public sealed class BatchReferenceSegment : ODataPathSegment
    {
        /// <summary>
        /// The <see cref="IEdmType"/> of the resource that this placeholder <see cref="BatchReferenceSegment"/> represents.
        /// </summary>
        private readonly IEdmType edmType;

        /// <summary>
        /// The entity set from the alias.
        /// </summary>
        private readonly IEdmEntitySet entitySet;

        /// <summary>
        /// The contentId that this alias referrs to.
        /// </summary>
        private readonly string contentId;

        /// <summary>
        /// Build a BatchReferenceSegment
        /// </summary>
        /// <param name="contentId">The contentId of this segment is referring to</param>
        /// <param name="edmType">The <see cref="IEdmType"/> of the resource that this placeholder <see cref="BatchReferenceSegment"/> represents.</param>
        /// <param name="entitySet">The resulting entity set</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input edmType of contentID is null.</exception>
        /// <exception cref="ODataException">Throws if the contentID is not in the right format.</exception>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "Rule only applies to ODataLib Serialization code.")]
        public BatchReferenceSegment(string contentId, IEdmType edmType, IEdmEntitySet entitySet)
        {
            ExceptionUtils.CheckArgumentNotNull(edmType, "resultingType");
            ExceptionUtils.CheckArgumentNotNull(contentId, "contentId");
            if (!ODataPathParser.ContentIdRegex.IsMatch(contentId))
            {
                throw new ODataException(ODataErrorStrings.BatchReferenceSegment_InvalidContentID(contentId));
            }

            this.edmType = edmType;
            this.entitySet = entitySet;
            this.contentId = contentId;

            this.Identifier = this.ContentId;
            this.TargetEdmType = edmType;
            this.TargetEdmEntitySet = this.EntitySet;
            this.SingleResult = true;
            this.TargetKind = RequestTargetKind.Resource;

            if (entitySet != null)
            {
                UriParserErrorHelper.ThrowIfTypesUnrelated(edmType, entitySet.ElementType, "BatchReferenceSegments");
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmType"/> of the resource that this placeholder <see cref="BatchReferenceSegment"/> represents.
        /// </summary>
        public override IEdmType EdmType
        {
            get { return this.edmType; }
        }

        /// <summary>
        /// Gets the resulting entity set for this batch reference segment.
        /// </summary>
        public IEdmEntitySet EntitySet
        {
            get { return this.entitySet; }
        }

        /// <summary>
        /// Gets the contentId this alias is referrring to
        /// </summary>
        public string ContentId
        {
            get { return this.contentId; }
        }

        /// <summary>
        /// Translate this <see cref="BatchReferenceSegment"/> into something else.
        /// </summary>
        /// <typeparam name="T">Type that the translator will return after translating this segment.</typeparam>
        /// <param name="translator">An implementation of the translator interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the translator.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input translator is null.</exception>
        public override T Translate<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="BatchReferenceSegment"/> using an implementation of the <see cref="PathSegmentHandler"/> interface.
        /// </summary>
        /// <param name="handler">An implementation of the Handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input Handler is null.</exception>
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
        internal override bool Equals(ODataPathSegment other)
        {
            DebugUtils.CheckNoExternalCallers();
            BatchReferenceSegment otherBatchReferenceSegment = other as BatchReferenceSegment;
            return otherBatchReferenceSegment != null && 
                otherBatchReferenceSegment.EdmType == this.edmType &&
                otherBatchReferenceSegment.EntitySet == this.entitySet &&
                otherBatchReferenceSegment.ContentId == this.contentId;
        }
    }
}
