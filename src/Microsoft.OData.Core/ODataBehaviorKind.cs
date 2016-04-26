//---------------------------------------------------------------------
// <copyright file="ODataBehaviorKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Enumeration for the different kinds of reader and writer behaviors
    /// supported in the OData library.
    /// </summary>
    internal enum ODataBehaviorKind
    {
        /// <summary>The default behavior of the OData library.</summary>
        Default,

        /// <summary>The behavior of the OData server.</summary>
        ODataServer,

        /// <summary>The behavior of the WCF Data Services client.</summary>
        WcfDataServicesClient,
    }
}
