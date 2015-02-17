//---------------------------------------------------------------------
// <copyright file="DeleteAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    /// <summary>
    /// OnDelete Action for Foreign key
    /// </summary>
    public enum DeleteAction
    {
        /// <summary>
        /// No action will be taken
        /// </summary>
        None = 0,

        /// <summary>
        /// Cascade to other end
        /// </summary>
        Cascade = 1,
    }
}