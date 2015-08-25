//---------------------------------------------------------------------
// <copyright file="DetermineNavigationSourceTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Translator that determines the navigation source of a segment.
    /// </summary>
    internal sealed class DetermineNavigationSourceTranslator : PathSegmentTranslator<IEdmNavigationSource>
    {
        /// <summary>
        /// Determine the NavigationSource of a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">The NavigationPropertyLinkSegment to look in.</param>
        /// <returns>The IEdmNavigationSource of this NavigationPropertyLinkSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(NavigationPropertyLinkSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.NavigationSource;
        }

        /// <summary>
        /// Determine the NavigationSource of a TypeSegment
        /// </summary>
        /// <param name="segment">The TypeSegment to look in.</param>
        /// <returns>The IEdmNavigationSource of this TypeSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(TypeSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.NavigationSource;
        }

        /// <summary>
        /// Determine the NavigationSource of a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">The NavigationPropertySegment to look in.</param>
        /// <returns>The IEdmNavigationSource of this NavigationPropertySegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(NavigationPropertySegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.NavigationSource;
        }

        /// <summary>
        /// Determine the NavigationSource of an EntitySetSegment
        /// </summary>
        /// <param name="segment">The EntitySetSegment to look in.</param>
        /// <returns>The IEdmNavigationSource of this EntitySetSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(EntitySetSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the NavigationSource of a SingletonSegment
        /// </summary>
        /// <param name="segment">The SingletonSegment to look in.</param>
        /// <returns>The IEdmNavigationSource of this SingletonSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(SingletonSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.Singleton;
        }

        /// <summary>
        /// Determine the NavigationSource of a KeySegment
        /// </summary>
        /// <param name="segment">The KeySegment to look in.</param>
        /// <returns>The IEdmNavigationSource of this KeySegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(KeySegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.NavigationSource;
        }

        /// <summary>
        /// Determine the NavigationSource of a PropertySegment
        /// </summary>
        /// <param name="segment">The PropertySegment to look in.</param>
        /// <returns>null, since a property doesn't necessarily have an navigation source</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(PropertySegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Translate a OperationImportSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public override IEdmNavigationSource Translate(OperationImportSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the NavigationSource of an OperationSegment
        /// </summary>
        /// <param name="segment">The OperationSegment to look in.</param>
        /// <returns>The IEdmNavigationSource of this OperationSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(OperationSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the NavigationSource of a CountSegment
        /// </summary>
        /// <param name="segment">The CountSegment to look in.</param>
        /// <returns>null, since $count doesn't have an navigation source</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(CountSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the NavigationSource of a OpenPropertySegment
        /// </summary>
        /// <param name="segment">The OpenPropertySegment to look in.</param>
        /// <returns>null, since an OpenProperty doesn't have an navigation source</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(OpenPropertySegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the NavigationSource of a ValueSegment
        /// </summary>
        /// <param name="segment">The ValueSegment to look in.</param>
        /// <returns>null, since $value doesn't have an navigation source</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(ValueSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the NavigationSource of a BatchSegment
        /// </summary>
        /// <param name="segment">The BatchSegment to look in.</param>
        /// <returns>null, since $batch doesn't have an navigation source</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(BatchSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the NavigationSource of a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">The BatchReferenceSegment to look in.</param>
        /// <returns>The IEdmNavigationSource of this BatchReferenceSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(BatchReferenceSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the NavigationSource of a MetadataSegment
        /// </summary>
        /// <param name="segment">The MetadataSegment to look in.</param>
        /// <returns>null, since $batch doesn't have an navigation source</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmNavigationSource Translate(MetadataSegment segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }
    }
}