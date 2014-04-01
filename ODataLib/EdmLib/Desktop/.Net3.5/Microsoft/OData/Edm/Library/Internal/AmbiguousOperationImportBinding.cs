//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Internal
{
    internal class AmbiguousOperationImportBinding : AmbiguousBinding<IEdmOperationImport>, IEdmOperationImport
    {
        public AmbiguousOperationImportBinding(IEdmOperationImport first, IEdmOperationImport second)
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
                IEdmOperationImport first = this.Bindings.FirstOrDefault();
                return first != null ? first.Container : null;
            }
        }

        public System.Collections.Generic.IEnumerable<IEdmOperationParameter> Parameters
        {
            get
            {
                IEdmOperationImport first = this.Bindings.FirstOrDefault();
                return first != null ? first.Parameters : Enumerable.Empty<IEdmOperationParameter>();
            }
        }

        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.OperationImport; }
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

        public IEdmOperationParameter FindParameter(string name)
        {
            IEdmOperationImport first = this.Bindings.FirstOrDefault();
            return first != null ? first.FindParameter(name) : null;
        }
    }
}
