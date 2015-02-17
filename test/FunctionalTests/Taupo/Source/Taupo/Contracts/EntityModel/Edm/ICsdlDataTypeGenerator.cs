//---------------------------------------------------------------------
// <copyright file="ICsdlDataTypeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Interface to generate data type information in csdl
    /// </summary>
    public interface ICsdlDataTypeGenerator : IXsdlDataTypeGenerator
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not to output the fully qualified name for primitive data type.
        /// </summary>
        bool OutputFullNameForPrimitiveDataType { get; set; }

        /// <summary>
        /// Generates parameter data type information for Function Import
        /// </summary>
        /// <param name="parameter">The function parameter.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        IEnumerable<XObject> GenerateParameterTypeForFunctionImport(FunctionParameter parameter, XNamespace xmlNamespace);

        /// <summary>
        /// Generates return type information for Function Import
        /// </summary>
        /// <param name="returnTypes">The return types of Function Import.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        IEnumerable<XObject> GenerateReturnTypeForFunctionImport(IEnumerable<FunctionImportReturnType> returnTypes, XNamespace xmlNamespace);
    }
}
