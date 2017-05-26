//---------------------------------------------------------------------
// <copyright file="CsdlAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Action.
    /// </summary>
    internal class CsdlAction : CsdlOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsdlAction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="entitySetPath">The entity set path.</param>
        /// <param name="documentation">The documentation.</param>
        /// <param name="location">The location in the csdl document of the function.</param>
        public CsdlAction(
            string name,
            IEnumerable<CsdlOperationParameter> parameters,
            CsdlTypeReference returnType,
            bool isBound,
            string entitySetPath,
            CsdlDocumentation documentation,
            CsdlLocation location)
            : base(name, parameters, returnType, isBound, entitySetPath, documentation, location)
        {
        }
    }
}
