//---------------------------------------------------------------------
// <copyright file="ODataObjectResultComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Linq;
    using Microsoft.Test.Taupo.Astoria.Client;

    /// <summary>
    /// Compares instances without the need for DataServiceContext instance
    /// </summary>
    [ImplementationName(typeof(LinqResultComparerBase), "ODataLib")]
    public class ODataObjectResultComparer : ClientQueryResultComparer
    {
    }
}
