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
