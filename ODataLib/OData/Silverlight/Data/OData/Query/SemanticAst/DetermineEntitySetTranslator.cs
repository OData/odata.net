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
    using Microsoft.Data.Edm;

    /// <summary>
    /// Translator that determines the entity set of a segment.
    /// </summary>
    internal sealed class DetermineEntitySetTranslator : PathSegmentTranslator<IEdmEntitySet>
    {
        /// <summary>
        /// Determine the EntitySet of a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">The NavigationPropertyLinkSegment to look in.</param>
        /// <returns>The IEdmEntitySet of this NavigationPropertyLinkSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(NavigationPropertyLinkSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the EntitySet of a TypeSegment
        /// </summary>
        /// <param name="segment">The TypeSegment to look in.</param>
        /// <returns>The IEdmEntitySet of this TypeSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(TypeSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the EntitySet of a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">The NavigationPropertySegment to look in.</param>
        /// <returns>The IEdmEntitySet of this NavigationPropertySegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(NavigationPropertySegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the EntitySet of an EntitySetSegment
        /// </summary>
        /// <param name="segment">The EntitySetSegment to look in.</param>
        /// <returns>The IEdmEntitySet of this EntitySetSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(EntitySetSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the EntitySet of a KeySegment
        /// </summary>
        /// <param name="segment">The KeySegment to look in.</param>
        /// <returns>The IEdmEntitySet of this KeySegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(KeySegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the EntitySet of a PropertySegment
        /// </summary>
        /// <param name="segment">The PropertySegment to look in.</param>
        /// <returns>null, since a property doesn't necessarily have an entity set</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(PropertySegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the EntitySet of an OperationSegment
        /// </summary>
        /// <param name="segment">The OperationSegment to look in.</param>
        /// <returns>The IEdmEntitySet of this OperationSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(OperationSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the EntitySet of a CountSegment
        /// </summary>
        /// <param name="segment">The CountSegment to look in.</param>
        /// <returns>null, since $count doesn't have an entitySet</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(CountSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the EntitySet of a OpenPropertySegment
        /// </summary>
        /// <param name="segment">The OpenPropertySegment to look in.</param>
        /// <returns>null, since an OpenProperty doesn't have an entity set</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(OpenPropertySegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the EntitySet of a ValueSegment
        /// </summary>
        /// <param name="segment">The ValueSegment to look in.</param>
        /// <returns>null, since $value doesn't have an entity set</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(ValueSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the EntitySet of a BatchSegment
        /// </summary>
        /// <param name="segment">The BatchSegment to look in.</param>
        /// <returns>null, since $batch doesn't have an entity set</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(BatchSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }

        /// <summary>
        /// Determine the EntitySet of a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">The BatchReferenceSegment to look in.</param>
        /// <returns>The IEdmEntitySet of this BatchReferenceSegment</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(BatchReferenceSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return segment.EntitySet;
        }

        /// <summary>
        /// Determine the EntitySet of a MetadataSegment
        /// </summary>
        /// <param name="segment">The MetadataSegment to look in.</param>
        /// <returns>null, since $batch doesn't have an entity set</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input segment is null.</exception>
        public override IEdmEntitySet Translate(MetadataSegment segment)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");
            return null;
        }
    }
}
