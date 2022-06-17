//---------------------------------------------------------------------
// <copyright file="IsCollectionTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Segment translator to determine whether a given <see cref="ODataPathSegment"/> is a collection.
    /// </summary>
    internal sealed class IsCollectionTranslator : PathSegmentTranslator<bool>
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="IsCollectionTranslator"/> class from being created
        /// </summary>
        private IsCollectionTranslator()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the <see cref="IsCollectionTranslator"/>
        /// </summary>
        public static IsCollectionTranslator Instance { get; } = new IsCollectionTranslator();

        /// <summary>
        /// Translate a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(NavigationPropertySegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.NavigationProperty.Type.IsCollection();
        }

        /// <summary>
        /// Translate an EntitySetSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(EntitySetSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return true;
        }

        /// <summary>
        /// Translate a KeySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(KeySegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate a PropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(PropertySegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate an AnnotationSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(AnnotationSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(DynamicPathSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate a CountSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(CountSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate a FilterSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(FilterSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return true;
        }

        /// <summary>
        /// Translate a ReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(ReferenceSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return !segment.SingleResult;
        }

        /// <summary>
        /// Translate an EachSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(EachSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(NavigationPropertyLinkSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate a BatchSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(BatchSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(BatchReferenceSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate a ValueSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(ValueSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            throw new NotImplementedException(segment.ToString());
        }

        /// <summary>
        /// Translate a MetadataSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(MetadataSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return false;
        }

        /// <summary>
        /// Translate a PathTemplateSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(PathTemplateSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return !segment.SingleResult;
        }
    }
}
