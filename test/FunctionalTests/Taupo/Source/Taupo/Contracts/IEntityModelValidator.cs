//---------------------------------------------------------------------
// <copyright file="IEntityModelValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Validates an <see cref="EntityModelSchema"/> for some aspects.
    /// </summary>
    public interface IEntityModelValidator
    {
        /// <summary>
        /// Performs the validation.
        /// </summary>
        /// <param name="model">Model to perform validation.</param>
        /// <returns>whether the model is valid</returns>        
        bool IsModelValid(EntityModelSchema model);
    }
}
