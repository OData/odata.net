//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Visitors
{
    using System;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;

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
