//---------------------------------------------------------------------
// <copyright file="ICsdlParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Parses CSDL file into <see cref="EntityModelSchema"/>.
    /// </summary>
    [ImplementationSelector("CsdlParser", DefaultImplementation = "Default")]
    public interface ICsdlParser
    {
        /// <summary>
        /// Parses the specified CSDL content.
        /// </summary>
        /// <param name="csdlContent">Content of multiple CSDL files.</param>
        /// <returns>Instance of <see cref="EntityModelSchema" /> which represents Entity Model parsed from the files.</returns>
        EntityModelSchema Parse(params XElement[] csdlContent);
    }
}
