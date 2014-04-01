//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a base class for CSDL functions and operation imports.
    /// </summary>
    internal abstract class CsdlFunctionBase : CsdlNamedElement
    {
        private readonly List<CsdlOperationParameter> parameters;
        private readonly CsdlTypeReference returnType;

        protected CsdlFunctionBase(string name, IEnumerable<CsdlOperationParameter> parameters, CsdlTypeReference returnType, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.parameters = new List<CsdlOperationParameter>(parameters);
            this.returnType = returnType;
        }

        public IEnumerable<CsdlOperationParameter> Parameters
        {
            get { return this.parameters; }
        }

        public CsdlTypeReference ReturnType
        {
            get { return this.returnType; }
        }
    }
}
