//---------------------------------------------------------------------
// <copyright file="ApplyTransformationToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser.Aggregation
#endif
{
    using Microsoft.OData.UriParser;

    /// <summary>
    /// Base class for Apply transformation tokens
    /// </summary>
    public abstract class ApplyTransformationToken : QueryToken
    {
    }
}
