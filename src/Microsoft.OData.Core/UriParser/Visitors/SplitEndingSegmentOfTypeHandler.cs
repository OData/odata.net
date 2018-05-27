//---------------------------------------------------------------------
// <copyright file="SplitEndingSegmentOfTypeHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Split Last Segment Of Certain Kind, the result of this handler is two part of ODataPath,
    /// FirstPart contains original ODataPath with out ending segments of type T,
    /// LastPart contains the ending segments of type T.
    /// </summary>
    /// <typeparam name="T">The type of ODataPathSegment to split</typeparam>
    internal sealed class SplitEndingSegmentOfTypeHandler<T> : PathSegmentHandler where T : ODataPathSegment
    {
        /// <summary>
        /// Queue contianing first part of ODataPath segment
        /// </summary>
        private readonly Queue<ODataPathSegment> first;

        /// <summary>
        /// Queue contianing last part of ODataPath segment
        /// </summary>
        private readonly Queue<ODataPathSegment> last;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplitEndingSegmentOfTypeHandler()
        {
            this.first = new Queue<ODataPathSegment>();
            this.last = new Queue<ODataPathSegment>();
        }

        /// <summary>
        /// The first part of split ODataPath
        /// </summary>
        public ODataPath FirstPart
        {
            get
            {
                return new ODataPath(this.first);
            }
        }

        /// <summary>
        /// The last part of split ODataPath
        /// </summary>
        public ODataPath LastPart
        {
            get
            {
                return new ODataPath(this.last);
            }
        }

        /// <summary>
        /// Handle a TypeSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(TypeSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(NavigationPropertySegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle an EntitySetSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(EntitySetSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle an SingletonSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(SingletonSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a KeySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(KeySegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a PropertySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(PropertySegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle an AnnotationSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(AnnotationSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle an OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(OperationImportSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle an OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(OperationSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(DynamicPathSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a CountSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(CountSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a LinksSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(NavigationPropertyLinkSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a ValueSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(ValueSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a BatchSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(BatchSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(BatchReferenceSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Handle a MetadataSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public override void Handle(MetadataSegment segment)
        {
            CommonHandler(segment);
        }

        /// <summary>
        /// Common handler to handle segments
        /// </summary>
        /// <param name="segment">The segment to deal with</param>
        private void CommonHandler(ODataPathSegment segment)
        {
            if (segment is T)
            {
                this.last.Enqueue(segment);
            }
            else
            {
                while (last.Any())
                {
                    this.first.Enqueue(this.last.Dequeue());
                }

                this.first.Enqueue(segment);
            }
        }
    }
}