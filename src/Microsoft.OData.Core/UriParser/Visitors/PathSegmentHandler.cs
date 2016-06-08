//---------------------------------------------------------------------
// <copyright file="PathSegmentHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;

    /// <summary>
    /// Handler interface for walking the path semantic tree.
    /// </summary>
    public abstract class PathSegmentHandler
    {
        /// <summary>
        /// Handle a ODataPathSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(ODataPathSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a TypeSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(TypeSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(NavigationPropertySegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an EntitySetSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(EntitySetSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an SingletonSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(SingletonSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a KeySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(KeySegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a PropertySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(PropertySegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(OperationImportSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(OperationSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(DynamicPathSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a CountSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(CountSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a LinksSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(NavigationPropertyLinkSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a ValueSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(ValueSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a BatchSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(BatchSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(BatchReferenceSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a MetadataSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(MetadataSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle a PathTemplateSegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(PathTemplateSegment segment)
        {
            throw new NotImplementedException();
        }
    }
}