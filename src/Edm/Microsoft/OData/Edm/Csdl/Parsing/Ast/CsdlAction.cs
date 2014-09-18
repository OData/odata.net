//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
