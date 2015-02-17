//---------------------------------------------------------------------
// <copyright file="IAttributeToODataLibAnnotationConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Interface that looks at serializable CSDL annotations (attribute annotations) and converts them to 
    /// ODataLib product annotations (ODataEntityPropertyMappings) on an <see cref="IEdmModel"/>.
    /// </summary>
    [ImplementationSelector("AstoriaAttributeToODataLibAnnotationConverter", DefaultImplementation = "Default")]
    public interface IAttributeToODataLibAnnotationConverter
    {
        /// <summary>
        /// Performs the conversion.
        /// </summary>
        /// <param name="model">Model to perform conversion on.</param>
        void ConvertToODataLibAnnotations(IEdmModel model);
    }
}
