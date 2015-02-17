//---------------------------------------------------------------------
// <copyright file="IObjectLayerCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Contract for generating the object layer (ie, the backing types) for a provider
    /// </summary>
    public interface IObjectLayerCodeGenerator
    {
        /// <summary>
        /// Generates the object layer for the given model
        /// </summary>
        /// <param name="compileUnit">The compile unit to add code to</param>
        /// <param name="model">The model to base the object layer on</param>
        void GenerateObjectLayer(CodeCompileUnit compileUnit, EntityModelSchema model);
    }
}
