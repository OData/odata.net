//---------------------------------------------------------------------
// <copyright file="ITypedValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Interface for represening a payload value which has a type name and can be null
    /// </summary>
    public interface ITypedValue
    {
        /// <summary>
        /// Gets or sets the fully-qualified type name for the value
        /// </summary>
        string FullTypeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the value is null
        /// </summary>
        bool IsNull { get; set; }
    }
}
