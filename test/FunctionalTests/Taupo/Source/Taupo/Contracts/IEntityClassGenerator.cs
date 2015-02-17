//---------------------------------------------------------------------
// <copyright file="IEntityClassGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Generates non-product entity classes based on <see cref="EntityModelSchema"/>
    /// </summary>
    [ImplementationSelector("EntityGenerator", DefaultImplementation = "POCO", HelpText = "Non-product generator for C#/VB entity classes.")]
    public interface IEntityClassGenerator
    {
        /// <summary>
        /// Generates non-product entity classes based on a given model.
        /// </summary>
        /// <param name="model">Input model</param>
        /// <returns>Generated file contents.</returns>
        IEnumerable<FileContents<string>> GenerateEntityClasses(EntityModelSchema model);
    }
}
