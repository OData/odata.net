//---------------------------------------------------------------------
// <copyright file="IEntityModelFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// interface for all fixup classes that work on <see cref="EntityModelSchema"/>.
    /// </summary>
    public interface IEntityModelFixup
    {
        /// <summary>
        /// Performs the fixup.
        /// </summary>
        /// <param name="model">Model to perform fixup on.</param>
        void Fixup(EntityModelSchema model);
    }
}
