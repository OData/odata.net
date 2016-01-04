//---------------------------------------------------------------------
// <copyright file="AggregationVerb.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions
{
    /// <summary>
    /// Enumeration of verbs used in the aggregation clause
    /// </summary>
    public enum AggregationVerb
    {
        /// <summary>
        /// The aggregation verb Sum.
        /// </summary>
        Sum,

        /// <summary>
        /// The aggregation verb Min.
        /// </summary>
        Min,

        /// <summary>
        /// The aggregation verb Max.
        /// </summary>
        Max,

        /// <summary>
        /// The aggregation verb Average.
        /// </summary>
        Average,

        /// <summary>
        /// The aggregation verb CountDistinct.
        /// </summary>
        CountDistinct
    }
}
