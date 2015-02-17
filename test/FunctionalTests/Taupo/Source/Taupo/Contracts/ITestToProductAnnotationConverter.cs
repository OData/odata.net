//---------------------------------------------------------------------
// <copyright file="ITestToProductAnnotationConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Interface that looks at test annotations and converts it to product annotation in an <see cref="EntityModelSchema"/>.
    /// </summary>
    [ImplementationSelector("TestToProductAnnotationConverter", DefaultImplementation = "Default")]
    public interface ITestToProductAnnotationConverter
    {
        /// <summary>
        /// Performs the conversion.
        /// </summary>
        /// <param name="model">Model to perform conversion on.</param>
        /// <returns>A clone of the <paramref name="model"/> with the converted annotations.</returns>
        EntityModelSchema ConvertToProductAnnotations(EntityModelSchema model);
    }
}
