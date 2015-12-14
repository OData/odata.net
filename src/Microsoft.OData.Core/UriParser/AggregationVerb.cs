//---------------------------------------------------------------------
// <copyright file="AggregationVerb.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser
{
    /// <summary>
    /// Enumeration of verbs used in the aggregation clause
    /// </summary>
    public enum AggregationVerb
    {
        Sum,
        Min,
        Max,
        Average,
        CountDistinct
    }
}
