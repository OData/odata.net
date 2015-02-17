//---------------------------------------------------------------------
// <copyright file="ServiceOperationReturnTypeQualifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    /// <summary>
    /// Complements the ReturnType of the Function, providing further hints to code generation
    /// </summary>
    public enum ServiceOperationReturnTypeQualifier
    {
        /// <summary>
        /// No further qualification for the ReturnType
        /// </summary>
        None,

        /// <summary>
        /// Indicates a void operation
        /// </summary>
        Void,

        /// <summary>
        /// Indicates that the collection ReturnType is IEnumerable
        /// </summary>
        IEnumerable,

        /// <summary>
        /// Indicates that the collection ReturnType is IQueryable
        /// </summary>
        IQueryable,
    }
}
