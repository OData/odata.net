//---------------------------------------------------------------------
// <copyright file="ActionOperationRights.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;

    /// <summary>
    /// Used in place of Microsoft.OData.Service.Common.ActionOperationRights
    /// </summary>
    [Flags]
    public enum ActionOperationRights
    {
        /// <summary>Specifies no rights on this service action.</summary>
        None = 0,

        /// <summary>Specifies the invoke on this service action.</summary>
        Invoke = 1,
    }
}