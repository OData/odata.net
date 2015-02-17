//---------------------------------------------------------------------
// <copyright file="IXsdlDataTypeGenerator.cs" company="Microsoft">
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
    /// Interface to generate data type information in csdl / ssdl
    /// </summary>
    public interface IXsdlDataTypeGenerator
    {
        /// <summary>
        /// Generates property data type information
        /// </summary>
        /// <param name="memberProperty">The property.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        IEnumerable<XObject> GeneratePropertyType(MemberProperty memberProperty, XNamespace xmlNamespace);

        /// <summary>
        /// Generates parameter data type information for Function
        /// </summary>
        /// <param name="parameter">The function parameter.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        IEnumerable<XObject> GenerateParameterTypeForFunction(FunctionParameter parameter, XNamespace xmlNamespace);

        /// <summary>
        /// Generates return type information for Function
        /// </summary>
        /// <param name="returnType">The return type of Function.</param>
        /// <param name="xmlNamespace">The xml namespace.</param>
        /// <returns>XElements and XAttributes for the data type.</returns>
        IEnumerable<XObject> GenerateReturnTypeForFunction(DataType returnType, XNamespace xmlNamespace);
    }
}
