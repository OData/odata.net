//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        private readonly IEdmEntitySetBase entitySet;

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
        public BatchReferenceSegment(string contentId, IEdmType edmType, IEdmEntitySetBase entitySet)
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
            this.TargetEdmNavigationSource = this.EntitySet;
            this.SingleResult = true;
            this.TargetKind = RequestTargetKind.Resource;

            if (entitySet != null)
            {
                UriParserErrorHelper.ThrowIfTypesUnrelated(edmType, entitySet.EntityType(), "BatchReferenceSegments");
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
        public IEdmEntitySetBase EntitySet
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
        public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
        {
            ExceptionUtils.CheckArgumentNotNull(translator, "translator");
            return translator.Translate(this);
        }

        /// <summary>
        /// Handle a <see cref="BatchReferenceSegment"/> using an implementation of the <see cref="PathSegmentHandler"/> interface.
        /// </summary>
        /// <param name="handler">An implementation of the Handler interface.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input Handler is null.</exception>
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
            BatchReferenceSegment otherBatchReferenceSegment = other as BatchReferenceSegment;
            return otherBatchReferenceSegment != null && 
                otherBatchReferenceSegment.EdmType == this.edmType &&
                otherBatchReferenceSegment.EntitySet == this.entitySet &&
                otherBatchReferenceSegment.ContentId == this.contentId;
        }
    }
}
