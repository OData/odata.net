//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL operation import.
    /// </summary>
    internal abstract class CsdlOperationImport : CsdlFunctionBase
    {
        private readonly bool sideEffecting;
        private readonly bool composable;
        private readonly bool bindable;
        private readonly string entitySet;
        private readonly string entitySetPath;

        protected CsdlOperationImport(
            string name,
            bool sideEffecting,
            bool composable,
            bool bindable,
            string entitySet,
            string entitySetPath,
            IEnumerable<CsdlOperationParameter> parameters,
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
