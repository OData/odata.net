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
    using Microsoft.Data.Edm;

    /// <summary>
    /// Segment translator to determine whether a given <see cref="ODataPathSegment"/> is a collection.
    /// </summary>
    internal sealed class IsCollectionTranslator : PathSegmentTranslator<bool>
    {
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
        /// Translate an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>UserDefinedValue</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override bool Translate(OpenPropertySegment segment)
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
    }
}
