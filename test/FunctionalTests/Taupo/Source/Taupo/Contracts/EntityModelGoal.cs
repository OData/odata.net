//---------------------------------------------------------------------
// <copyright file="EntityModelGoal.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Specifies goal for Entity Model generation
    /// </summary>
    public abstract class EntityModelGoal
    {
        /// <summary>
        /// Determines whether the goal has been met for the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// <c>true</c> if the goal has been met for the specified model; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool HasBeenMet(EntityModelSchema model);

        /// <summary>
        /// Improves the specified model.
        /// </summary>
        /// <param name="model">The model to improve.</param>
        public abstract void Improve(EntityModelSchema model);
    }
}
