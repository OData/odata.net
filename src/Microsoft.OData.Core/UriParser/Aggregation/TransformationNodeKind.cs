//---------------------------------------------------------------------
// <copyright file="TransformationNodeKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    /// <summary>
    /// Enumeration of kinds of transformation nodes.
    /// </summary>
    public enum TransformationNodeKind
    {
        /// <summary>
        /// Aggregations of values
        /// </summary>
        Aggregate = 0,

        /// <summary>
        /// A grouping of values by properties
        /// </summary>
        GroupBy = 1,

        /// <summary>
        /// A filter clause
        /// </summary>
        Filter = 2,

        /// <summary>
        /// A Compute expressions
        /// </summary>
        Compute = 3,
    }
}