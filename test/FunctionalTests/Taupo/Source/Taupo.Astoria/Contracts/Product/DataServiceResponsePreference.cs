//---------------------------------------------------------------------
// <copyright file="DataServiceResponsePreference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Product
{
    /// <summary>
    /// Used in place of Microsoft.OData.Client.DataServiceResponsePreference to avoid a direct dependency on the product.
    /// </summary>
    public enum DataServiceResponsePreference
    {
        /// <summary>
        /// None specified.
        /// </summary>
        None,

        /// <summary>
        /// Response Include Content.
        /// </summary>
        IncludeContent,

        /// <summary>
        /// Response with No Content.
        /// </summary>
        NoContent,

        /// <summary>
        /// Unspecified. This is for test use only.
        /// </summary>
        Unspecified
    }
}
