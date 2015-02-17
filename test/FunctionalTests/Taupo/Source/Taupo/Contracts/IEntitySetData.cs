//---------------------------------------------------------------------
// <copyright file="IEntitySetData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Defines entity set data.
    /// </summary>
    public interface IEntitySetData
    {
        /// <summary>
        /// Gets the container-qualified name of the <see cref="EntitySet"/> which <see cref="Data"/> belongs to.
        /// </summary>
        string EntitySetName { get; }

        /// <summary>
        /// Gets the entity set data.
        /// </summary>
        object Data { get; }
    }
}
