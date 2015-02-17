//---------------------------------------------------------------------
// <copyright file="ICsdlContentGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Generates csdl contents, given a fully resolved <see cref="EntityModelSchema"/>
    /// </summary>
    [ImplementationSelector("CSDLGenerator", DefaultImplementation = "Default")]
    public interface ICsdlContentGenerator
    {
        /// <summary>
        /// Generates csdl contents, given a fully resolved <see cref="EntityModelSchema"/>
        /// </summary>
        /// <param name="csdlVersion">The CSDL version.</param>
        /// <param name="model">Input model</param>
        /// <returns>Generated file contents.</returns>
        IEnumerable<FileContents<XElement>> Generate(EdmVersion csdlVersion, EntityModelSchema model);
    }
}
