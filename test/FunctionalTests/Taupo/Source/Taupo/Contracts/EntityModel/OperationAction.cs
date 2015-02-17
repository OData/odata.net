//---------------------------------------------------------------------
// <copyright file="OperationAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// OnDelete Action for Association End
    /// </summary>
    public enum OperationAction
    {
        /// <summary>
        /// No action will be taken
        /// </summary>
        None = 0,

        /// <summary>
        /// Cascade to other ends
        /// </summary>
        Cascade = 1,
    }
}
