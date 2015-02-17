//---------------------------------------------------------------------
// <copyright file="IAttributeToPropertyMappingAnnotationConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Interface that looks at product annotations(AttributeAnnotation) and converts them to test annotations(PropertyMappingAnnotation) in a <see cref="EntityModelSchema"/>.
    /// </summary>
    [ImplementationSelector("AstoriaAttributeToPropertyMappingAnnotationConverter", DefaultImplementation = "Default")]
    public interface IAttributeToPropertyMappingAnnotationConverter
    {
        /// <summary>
        /// Performs the conversion.
        /// </summary>
        /// <param name="model">Model to perform conversion on.</param>
        void ConvertToTestAnnotations(EntityModelSchema model);
    }
}
