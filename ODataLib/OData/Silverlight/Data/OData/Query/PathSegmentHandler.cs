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
