//---------------------------------------------------------------------
// <copyright file="OperationKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Enumeration for the kind of service operations
    /// </summary>
    internal enum OperationKind
    {
        /// <summary>V1 Service Operation</summary>
        ServiceOperation,

        /// <summary>Side-effecting operation.</summary>
        Action,
    }
}
