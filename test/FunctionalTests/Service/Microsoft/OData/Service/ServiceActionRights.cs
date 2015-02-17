//---------------------------------------------------------------------
// <copyright file="ServiceActionRights.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;

    /// <summary> Access rights for service actions. </summary>
    [Flags]
    public enum ServiceActionRights
    {
        /// <summary>Specifies no rights on this service action.</summary>
        None = 0,

        /// <summary>Specifies the right to execute the service action.</summary>
        Invoke = 1,
    }
}
