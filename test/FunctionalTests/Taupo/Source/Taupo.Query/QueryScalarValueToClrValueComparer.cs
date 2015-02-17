//---------------------------------------------------------------------
// <copyright file="QueryScalarValueToClrValueComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Default implementation of contract to compare a query scalar value to a clr value
    /// </summary>
    [ImplementationName(typeof(IQueryScalarValueToClrValueComparer), "Default")]
    public class QueryScalarValueToClrValueComparer : QueryScalarValueToClrValueComparerBase
    {
    }
}
