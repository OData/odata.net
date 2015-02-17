//---------------------------------------------------------------------
// <copyright file="SchemaObjectSpecificationLevel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Available SchemaObject specification levels.
    /// </summary>
    public enum SchemaObjectSpecificationLevel
    {
        /// <summary>
        /// Specify Catalog, Schema and Name.
        /// </summary>
        Catalog,

        /// <summary>
        /// Specify Schema and Name.
        /// </summary>
        Schema,

        /// <summary>
        /// Specify Name only.
        /// </summary>
        Name,
    }
}