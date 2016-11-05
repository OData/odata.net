//---------------------------------------------------------------------
// <copyright file="AggregationMethod.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Aggregation
{
    /// <summary>
    /// Enumeration of methods used in the aggregation clause
    /// </summary>
    public enum AggregationMethod
    {
        /// <summary>
        /// The aggregation method Sum.
        /// </summary>
        Sum,

        /// <summary>
        /// The aggregation method Min.
        /// </summary>
        Min,

        /// <summary>
        /// The aggregation method Max.
        /// </summary>
        Max,

        /// <summary>
        /// The aggregation method Average.
        /// </summary>
        Average,

        /// <summary>
        /// The aggregation method CountDistinct.
        /// </summary>
        CountDistinct
    }
}
