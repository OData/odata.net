//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library
{
    internal class AmbiguousOperationBinding : AmbiguousBinding<IEdmOperation>, IEdmOperation
    {
        private IEdmOperation first;

        public AmbiguousOperationBinding(IEdmOperation first, IEdmOperation second)
            : base(first, second)
        {
            this.first = first;
        }

        public IEdmTypeReference ReturnType
        {
            // Not using the typical behavior for first.ReturnType as returning null is the old behavior.
            get { return null; }
        }

        public string Namespace
        {
            get { return this.first.Namespace; }
        }

        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.first.Parameters; }
        }

        public bool IsBound
        {
            get { return this.first.IsBound; }
        }

        public IEdmPathExpression EntitySetPath
        {
            get { return this.first.EntitySetPath; }
        }

        public EdmSchemaElementKind SchemaElementKind
        {
            get { return this.first.SchemaElementKind; }
        }

        public IEdmOperationParameter FindParameter(string name)
        {
            return this.first.FindParameter(name);
        }
    }
}
