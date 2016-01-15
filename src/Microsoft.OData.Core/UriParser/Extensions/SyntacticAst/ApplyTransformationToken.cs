//---------------------------------------------------------------------
// <copyright file="ApplyTransformationToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Extensions.Syntactic
#endif
{
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Base class for Applt transformation tokens
    /// </summary>
    internal abstract class ApplyTransformationToken : QueryToken
    {
    }
}
