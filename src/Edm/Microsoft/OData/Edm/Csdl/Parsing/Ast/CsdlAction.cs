//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
