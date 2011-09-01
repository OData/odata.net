//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a base class for CSDL functions and function imports.
    /// </summary>
    internal abstract class CsdlFunctionBase : CsdlNamedElement
    {
        private readonly List<CsdlFunctionParameter> parameters;
        private readonly CsdlTypeReference returnType;

        protected CsdlFunctionBase(string name, IEnumerable<CsdlFunctionParameter> parameters, CsdlTypeReference returnType, CsdlDocumentation documentation, CsdlLocation location)
            : base(name, documentation, location)
        {
            this.parameters = new List<CsdlFunctionParameter>(parameters);
            this.returnType = returnType;
        }

        public IEnumerable<CsdlFunctionParameter> Parameters
        {
            get { return this.parameters; }
        }

        public CsdlTypeReference ReturnType
        {
            get { return this.returnType; }
        }
    }
}
