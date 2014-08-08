//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query.SemanticAst
{
    using System;

    /// <summary>
    /// Handler interface for walking the path semantic tree.
    /// </summary>
    public abstract class PathSegmentHandler
    {
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
        public virtual void Handle(OperationSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handle an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Handle</param>
        public virtual void Handle(OpenPropertySegment segment)
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
    }
}
