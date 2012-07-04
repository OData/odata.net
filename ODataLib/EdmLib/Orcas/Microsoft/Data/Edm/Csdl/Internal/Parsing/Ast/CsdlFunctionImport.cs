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
    /// Represents a CSDL function import.
    /// </summary>
    internal class CsdlFunctionImport : CsdlFunctionBase
    {
        private readonly bool sideEffecting;
        private readonly bool composable;
        private readonly bool bindable;
        private readonly string entitySet;
        private readonly string entitySetPath;

        public CsdlFunctionImport(
            string name,
            bool sideEffecting,
            bool composable,
            bool bindable,
            string entitySet,
            string entitySetPath,
            IEnumerable<CsdlFunctionParameter> parameters,
            CsdlTypeReference returnType,
            CsdlDocumentation documentation,
            CsdlLocation location)
            : base(name, parameters, returnType, documentation, location)
        {
            this.sideEffecting = sideEffecting;
            this.composable = composable;
            this.bindable = bindable;
            this.entitySet = entitySet;
            this.entitySetPath = entitySetPath;
        }

        public bool SideEffecting
        {
            get { return this.sideEffecting; }
        }

        public bool Composable
        {
            get { return this.composable; }
        }

        public bool Bindable
        {
            get { return this.bindable; }
        }

        public string EntitySet
        {
            get { return this.entitySet; }
        }

        public string EntitySetPath
        {
            get { return this.entitySetPath; }
        }
    }
}
