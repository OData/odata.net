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

using System.Linq;
using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Internal
{
    internal class AmbiguousFunctionImportBinding : AmbiguousBinding<IEdmFunctionImport>, IEdmFunctionImport
    {
        public AmbiguousFunctionImportBinding(IEdmFunctionImport first, IEdmFunctionImport second)
            : base(first, second)
        {
        }

        public IEdmTypeReference ReturnType
        {
            get { return null; }
        }

        public IEdmEntityContainer Container
        {
            get
            {
                IEdmFunctionImport first = this.Bindings.FirstOrDefault();
                return first != null ? first.Container : null;
            }
        }

        public System.Collections.Generic.IEnumerable<IEdmFunctionParameter> Parameters
        {
            get
            {
                IEdmFunctionImport first = this.Bindings.FirstOrDefault();
                return first != null ? first.Parameters : Enumerable.Empty<IEdmFunctionParameter>();
            }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.FunctionImport; }
        }

        public IEdmExpression EntitySet
        {
            get { return null; }
        }

        public bool IsSideEffecting
        {
            get { return true; }
        }

        public bool IsComposable
        {
            get { return false; }
        }

        public bool IsBindable
        {
            get { return false; }
        }

        public IEdmFunctionParameter FindParameter(string name)
        {
            IEdmFunctionImport first = this.Bindings.FirstOrDefault();
            return first != null ? first.FindParameter(name) : null;
        }
    }
}
