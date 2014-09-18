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
