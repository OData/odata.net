//---------------------------------------------------------------------
// <copyright file="IModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Generates or returns <see cref="EntityModelSchema"/> instances.
    /// </summary>
    [ImplementationSelector("Model", DefaultImplementation = "Default", HelpText = "The model to build the workspace")]
    public interface IModelGenerator
    {
        /// <summary>
        /// Generate the model.
        /// </summary>
        /// <returns>Valid, fully resolved <see cref="EntityModelSchema"/>.</returns>
        EntityModelSchema GenerateModel();
    }
}
