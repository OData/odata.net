//---------------------------------------------------------------------
// <copyright file="MetadataProviderState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// An internal enumeration to track the differnt states of the metadata caching and materialization.
    /// We use a tri-state enumeration instead of a boolean flag since the URI parser will
    /// look up entity sets and service operations which don't require the full metadata
    /// but will require the entity containers.
    /// </summary>
    internal enum MetadataProviderState
    {
        /// <summary>Incremental materialization state.</summary>
        Incremental,

        /// <summary>Full materialization state.</summary>
        Full,
    }
}
