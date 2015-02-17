//---------------------------------------------------------------------
// <copyright file="EntityParameterSendOption.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Used to specify a strategy to send entity parameter.
    /// </summary>
    public enum EntityParameterSendOption
    {
        /// <summary>
        /// Send full properties of an entity parameter to service.
        /// </summary>
        SendFullProperties = 0,

        /// <summary>
        /// Send only set properties of an entity parameter to service.
        /// </summary>
        SendOnlySetProperties = 1,
    }
}
