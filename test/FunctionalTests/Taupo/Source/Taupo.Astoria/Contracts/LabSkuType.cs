//---------------------------------------------------------------------
// <copyright file="LabSkuType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// Used to select a machine running particular type of OS SKU
    /// </summary>
    public enum LabSkuType
    {
        /// <summary>
        /// Enterprise SKU
        /// </summary>
        Enterprise,
        
        /// <summary>
        /// Enterprise IA64 Sku
        /// </summary>
        EnterpriseIA64,
        
        /// <summary>
        /// Professional Sku
        /// </summary>
        Professional,
        
        /// <summary>
        /// Pro Premium Sku
        /// </summary>
        ProPremium,
        
        /// <summary>
        /// Ultimate SKU
        /// </summary>
        Ultimate
    }
}
