//---------------------------------------------------------------------
// <copyright file="ApplyTransformationToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.

using Microsoft.OData.Core.UriParser.Syntactic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    /// <summary>
    /// Base class for Applt transformation tokens
    /// </summary>
    internal abstract class ApplyTransformationToken : QueryToken
    {
    }
}
