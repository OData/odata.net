//   OData .NET Libraries ver. 5.6.3
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
