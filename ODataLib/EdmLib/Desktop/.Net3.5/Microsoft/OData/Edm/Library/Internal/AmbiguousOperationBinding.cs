//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Linq;

namespace Microsoft.OData.Edm.Library.Internal
{
    internal class AmbiguousOperationBinding : AmbiguousBinding<IEdmOperation>, IEdmOperation
    {
        public AmbiguousOperationBinding(IEdmOperation first, IEdmOperation second)
            : base(first, second)
        {
        }

        public IEdmTypeReference ReturnType
        {
            get { return null; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Operation; }
        }

        public string Namespace
        {
            get
            {
                IEdmOperation first = this.Bindings.FirstOrDefault();
                return first != null ? first.Namespace : string.Empty;
            }
        }

        public System.Collections.Generic.IEnumerable<IEdmOperationParameter> Parameters
        {
            get
            {
                IEdmOperation first = this.Bindings.FirstOrDefault();
                return first != null ? first.Parameters : Enumerable.Empty<IEdmOperationParameter>();
            }
        }

        public IEdmOperationParameter FindParameter(string name)
        {
            IEdmOperation first = this.Bindings.FirstOrDefault();
            return first != null ? first.FindParameter(name) : null;
        }
    }
}
