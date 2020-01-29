//---------------------------------------------------------------------
// <copyright file="CsdlFunction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL Function.
    /// </summary>
    internal class CsdlFunction : CsdlOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsdlFunction"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="operationReturn">The return of the function.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="entitySetPath">The entity set path.</param>
        /// <param name="isComposable">if set to <c>true</c> [is composable].</param>
        /// <param name="location">The location in the csdl document of the function.</param>
        public CsdlFunction(
            string name,
            IEnumerable<CsdlOperationParameter> parameters,
            CsdlOperationReturn operationReturn,
            bool isBound,
            string entitySetPath,
            bool isComposable,
            CsdlLocation location)
            : base(name, parameters, operationReturn, isBound, entitySetPath, location)
        {
            this.IsComposable = isComposable;
        }

        public bool IsComposable { get; private set; }
    }
}
