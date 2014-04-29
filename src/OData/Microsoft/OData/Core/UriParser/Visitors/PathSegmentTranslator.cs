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

    /// <summary>
    /// Translator interface for walking the Syntactic Tree.
    /// </summary>
    /// <typeparam name="T">Generic type produced by the translator.</typeparam>
    public abstract class PathSegmentTranslator<T>
    {
        /// <summary>
        /// Translate a TypeSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer</returns>
        public virtual T Translate(TypeSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(NavigationPropertySegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate an EntitySetSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(EntitySetSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate an SingletonSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(SingletonSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a KeySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(KeySegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a PropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(PropertySegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a OperationImportSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(OperationImportSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a OperationSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(OperationSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate an OpenPropertySegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(OpenPropertySegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a CountSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(CountSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visit a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(NavigationPropertyLinkSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a ValueSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(ValueSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a BatchSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(BatchSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(BatchReferenceSegment segment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translate a MetadataSegment
        /// </summary>
        /// <param name="segment">the segment to Translate</param>
        /// <returns>Defined by the implementer.</returns>
        public virtual T Translate(MetadataSegment segment)
        {
            throw new NotImplementedException();
        }
    }
}
