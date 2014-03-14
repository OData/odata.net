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

namespace Microsoft.Data.Edm.Library.Internal
{
    internal class AmbiguousFunctionBinding : AmbiguousBinding<IEdmFunction>, IEdmFunction
    {
        public AmbiguousFunctionBinding(IEdmFunction first, IEdmFunction second)
            : base(first, second)
        {
        }

        public IEdmTypeReference ReturnType
        {
            get { return null; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }

        public string Namespace
        {
            get
            {
                IEdmFunction first = this.Bindings.FirstOrDefault();
                return first != null ? first.Namespace : string.Empty;
            }
        }

        public string DefiningExpression
        {
            get { return null; }
        }

        public System.Collections.Generic.IEnumerable<IEdmFunctionParameter> Parameters
        {
            get
            {
                IEdmFunction first = this.Bindings.FirstOrDefault();
                return first != null ? first.Parameters : Enumerable.Empty<IEdmFunctionParameter>();
            }
        }

        public IEdmFunctionParameter FindParameter(string name)
        {
            IEdmFunction first = this.Bindings.FirstOrDefault();
            return first != null ? first.FindParameter(name) : null;
        }
    }
}
