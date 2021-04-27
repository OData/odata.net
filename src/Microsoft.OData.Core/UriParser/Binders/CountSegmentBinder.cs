//---------------------------------------------------------------------
// <copyright file="CountSegmentBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Class that knows how to bind an end path token, which could be several things.
    /// </summary>
    internal sealed class CountSegmentBinder
    {
        /// <summary>
        /// Constructs a CountSegmentBinder object using the given function to bind parent token.
        /// </summary>
        /// <param name="bindMethod">Method to bind the EndPathToken's parent, if there is one.</param>
        /// <param name="state">State of the metadata binding.</param>am>
        /// <remarks>
        /// Make bindMethod of return type SingleValueQueryNode.
        /// </remarks>
        internal CountSegmentBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
        }

        /// <summary>
        /// Binds an In operator token.
        /// </summary>
        /// <param name="inToken">The In operator token to bind.</param>
        /// <param name="state">State of the metadata binding.</param>
        /// <returns>The bound In operator token.</returns>
        internal QueryNode BindCountSegment(CountSegmentToken countSegmentToken)
        {
            ExceptionUtils.CheckArgumentNotNull(countSegmentToken, "countSegmentToken");

            CollectionNode node = null;
            return new CountNode(node);
        }
    }
}