//---------------------------------------------------------------------
// <copyright file="ODataContextUrlLevel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// Enumeration representing the different levels of context URL.
    /// </summary>
    internal enum ODataContextUrlLevel
    {
        /// <summary>
        /// No context URL
        /// Used for json with odata.metadata=none
        /// </summary>
        None = 0,

        /// <summary>
        /// Show root context URL of the payload and the context URL for any deleted entries or added or deleted links in a delta response,
        /// or for entities or entity collections whose set cannot be determined from the root context URL
        /// Used for atom and json with odata.metadata=minimal
        /// </summary>
        OnDemand = 1,

        /// <summary>
        /// Show context URL for a collection, entity, primitive value, or service document.
        /// Used for json with odata.metadata=full
        /// </summary>
        Full = 2
    }
}
